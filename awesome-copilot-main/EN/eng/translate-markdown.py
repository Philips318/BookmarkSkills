#!/usr/bin/env python3
"""Translate Markdown files into a mirrored output directory.

The script preserves code fences, inline code, URLs, and the leading YAML front
matter by default. Translation providers are intentionally explicit so large
repository contents are not sent to an online service by accident.
"""

from __future__ import annotations

import argparse
import json
import os
import re
import sys
import time
import urllib.error
import urllib.request
from concurrent.futures import ThreadPoolExecutor, as_completed
from pathlib import Path
from typing import Callable, Iterable, Protocol


DEFAULT_OUTPUT_DIR = "\u7ffb\u8bd1"
DEFAULT_BATCH_CHARS = 3500
PROTECTED_PATTERN = re.compile(
    r"("
    r"`[^`\n]+`"
    r"|https?://[^\s)\]>]+"
    r"|www\.[^\s)\]>]+"
    r"|<[^>\s]+>"
    r"|[\w.+-]+@[\w.-]+\.\w+"
    r"|\bAwesome GitHub Copilot\b"
    r"|\bGitHub Copilot\b"
    r"|\bGitHub Actions\b"
    r"|\bVS Code\b"
    r"|(?<!\w)(?:[\w.-]+/)+[\w.@-]+(?:\.[A-Za-z0-9]+)?"
    r"|\b[\w.-]+\.(?:agent|instructions|prompt|md|json|ya?ml|js|ts|tsx|jsx|mjs|cjs|py|cs|java|go|rb|php|rs|toml|xml|html|css|scss|png|jpe?g|gif|svg|webp|sh|ps1|bat|cmd|ini|txt|lock|config)\b"
    r"|\\\*[^\s,;)\]]+"
    r")"
)
LINK_PATTERN = re.compile(r"(!?)\[([^\]\n]*)\]\(([^)\n]*)\)|(!?)\[([^\]\n]+)\]\[([^\]\n]*)\]")
REFERENCE_DEFINITION_PATTERN = re.compile(r"^\s{0,3}\[[^\]]+\]:\s+")
TABLE_SEPARATOR_PATTERN = re.compile(r"^\s*\|?\s*:?-{3,}:?\s*(?:\|\s*:?-{3,}:?\s*)+\|?\s*$")


class TranslationError(RuntimeError):
    """Raised when the selected translation provider cannot translate text."""


class TranslateMany(Protocol):
    def __call__(self, text: str) -> str:
        ...

    def translate_many(self, texts: list[str]) -> list[str]:
        ...


class CachedTranslator:
    def __init__(self, translate: Callable[[str], str], batch_size: int, workers: int, cache_path: Path | None = None) -> None:
        self.translate = translate
        self.batch_size = batch_size
        self.workers = workers
        self.cache_path = cache_path
        self.cache: dict[str, str] = {}
        if self.cache_path and self.cache_path.exists():
            with self.cache_path.open("r", encoding="utf-8") as cache_file:
                for line in cache_file:
                    try:
                        source, target = json.loads(line)
                    except (json.JSONDecodeError, ValueError):
                        continue
                    if isinstance(source, str) and isinstance(target, str):
                        self.cache[source] = target

    def __call__(self, text: str) -> str:
        if text not in self.cache:
            self.cache[text] = self.translate(text)
        return self.cache[text]

    def translate_many(self, texts: list[str]) -> list[str]:
        missing = [text for text in dict.fromkeys(texts) if text not in self.cache]
        if missing:
            translate_many = getattr(self.translate, "translate_many", None)
            if callable(translate_many):
                for offset in range(0, len(missing), self.batch_size):
                    batch = missing[offset : offset + self.batch_size]
                    print(f"Translating segments: {offset + 1}-{offset + len(batch)} / {len(missing)}")
                    translated = translate_many(batch)
                    for source, target in zip(batch, translated):
                        self.cache[source] = target
                    self.append_cache(zip(batch, translated))
            else:
                if self.workers <= 1:
                    for index, text in enumerate(missing, start=1):
                        print(f"Translating segments: {index}-{index} / {len(missing)}")
                        self.cache[text] = self.translate(text)
                        self.append_cache([(text, self.cache[text])])
                else:
                    completed = 0
                    for offset in range(0, len(missing), self.batch_size):
                        batch = missing[offset : offset + self.batch_size]
                        with ThreadPoolExecutor(max_workers=self.workers) as executor:
                            future_map = {executor.submit(self.translate, text): text for text in batch}
                            for future in as_completed(future_map):
                                text = future_map[future]
                                target = future.result()
                                self.cache[text] = target
                                self.append_cache([(text, target)])
                                completed += 1
                        print(f"Translating segments: {completed}-{completed} / {len(missing)}")
        return [self.cache[text] for text in texts]

    def append_cache(self, entries: Iterable[tuple[str, str]]) -> None:
        if not self.cache_path:
            return
        self.cache_path.parent.mkdir(parents=True, exist_ok=True)
        with self.cache_path.open("a", encoding="utf-8", newline="\n") as cache_file:
            for source, target in entries:
                cache_file.write(json.dumps([source, target], ensure_ascii=False) + "\n")


class ArgosBatchProvider:
    def __init__(self, source: str, target: str, batch_size: int) -> None:
        try:
            import ctranslate2
            from argostranslate import settings
            from argostranslate import translate as argos_translate
        except ImportError as exc:
            raise TranslationError(
                "Argos Translate is not installed. Install it in Python first, then rerun with --provider argos."
            ) from exc

        installed_languages = argos_translate.get_installed_languages()
        source_language = next((language for language in installed_languages if language.code == source), None)
        target_language = next((language for language in installed_languages if language.code == target), None)
        if source_language is None or target_language is None:
            raise TranslationError(
                f"Argos language package {source}->{target} is not installed. Install that package before rerunning."
            )

        self.translation = source_language.get_translation(target_language)
        self.pkg = getattr(self.translation, "pkg", None)
        self.translator = None
        self.ctranslate2 = ctranslate2
        self.settings = settings
        self.batch_size = batch_size

    def __call__(self, text: str) -> str:
        return self.translate_many([text])[0]

    def translate_many(self, texts: list[str]) -> list[str]:
        if not texts:
            return []
        if self.pkg is None:
            return [self.translation.translate(text) for text in texts]

        if self.translator is None:
            self.translator = self.ctranslate2.Translator(
                str(self.pkg.package_path / "model"),
                device=self.settings.device,
                inter_threads=self.settings.inter_threads,
                intra_threads=self.settings.intra_threads,
                compute_type=self.settings.compute_type,
            )

        tokenized = [self.pkg.tokenizer.encode(text) for text in texts]
        target_prefix = None
        if self.pkg.target_prefix != "":
            target_prefix = [[self.pkg.target_prefix]] * len(tokenized)

        translated_batches = self.translator.translate_batch(
            tokenized,
            target_prefix=target_prefix,
            replace_unknowns=True,
            max_batch_size=self.batch_size,
            batch_type="examples",
            beam_size=1,
            num_hypotheses=1,
            length_penalty=0.2,
            return_scores=False,
        )

        output: list[str] = []
        for translated_batch in translated_batches:
            value = self.pkg.tokenizer.decode(translated_batch.hypotheses[0])
            if self.pkg.target_prefix != "" and value.startswith(self.pkg.target_prefix):
                value = value[len(self.pkg.target_prefix) :]
            if value.startswith(" "):
                value = value[1:]
            output.append(value)
        return output


def iter_markdown_files(root: Path, output_dir: Path) -> Iterable[Path]:
    ignored_parts = {
        ".git",
        "node_modules",
        "dist",
        ".astro",
        output_dir.name,
    }

    for path in root.rglob("*.md"):
        relative_parts = set(path.relative_to(root).parts)
        if relative_parts & ignored_parts:
            continue
        if output_dir in path.parents:
            continue
        yield path


def split_front_matter(text: str) -> tuple[str, str]:
    lines = text.splitlines(keepends=True)
    if not lines or lines[0].strip() != "---":
        return "", text

    for index in range(1, len(lines)):
        if lines[index].strip() in {"---", "..."}:
            return "".join(lines[: index + 1]), "".join(lines[index + 1 :])

    return "", text


def is_code_fence(line: str, active_fence: str | None) -> str | None:
    stripped = line.lstrip()
    if active_fence:
        return None if stripped.startswith(active_fence) else active_fence
    if stripped.startswith("```"):
        return "```"
    if stripped.startswith("~~~"):
        return "~~~"
    return None


def should_preserve_line(line: str, in_fence: bool) -> bool:
    stripped = line.strip()
    if in_fence:
        return True
    if not stripped:
        return True
    if REFERENCE_DEFINITION_PATTERN.match(line):
        return True
    if TABLE_SEPARATOR_PATTERN.match(line):
        return True
    if stripped.startswith("<") and stripped.endswith(">"):
        return True
    if stripped.startswith(("<!--", "-->", "[//]:", "[//]")):
        return True
    if line.startswith(("    ", "\t")):
        return True
    return False


def is_protected_only_markdown_line(line: str) -> bool:
    content, _ = split_line_ending(line)
    content = content.strip()
    content = re.sub(r"^#{1,6}\s+", "", content)
    content = re.sub(r"^(?:[-+*]|\d+[.)])\s+", "", content)
    content = content.strip("*_`()[] ")
    return bool(content and PROTECTED_PATTERN.fullmatch(content))


def should_translate_line_separately(line: str) -> bool:
    content, _ = split_line_ending(line)
    return bool(
        re.match(r"^\s{0,3}#{1,6}\s+", content)
        or re.match(r"^\s{0,3}(?:[-+*]|\d+[.)])\s+", content)
        or re.match(r"^\s{0,3}(?:>\s*)+", content)
        or "|" in content
    )


def translate_protected_block(text: str, translate: Callable[[str], str], max_chars: int) -> str:
    if not has_translatable_text(text):
        return text
    protected_block, protected = protect_inline(text)
    translated_chunks = [translate(chunk) for chunk in chunk_text(protected_block, max_chars)]
    return restore_inline("".join(translated_chunks), protected)


def protect_inline(text: str) -> tuple[str, dict[str, str]]:
    protected: dict[str, str] = {}

    def replace(match: re.Match[str]) -> str:
        token = f"xqz{len(protected)}xqz"
        protected[token] = match.group(0)
        return token

    return PROTECTED_PATTERN.sub(replace, text), protected


def restore_inline(text: str, protected: dict[str, str]) -> str:
    for token, original in sorted(protected.items(), key=lambda item: len(item[0]), reverse=True):
        number = re.escape(token[3:-3])
        token_pattern = r"\s*xqz\s*" + number + r"\s*xqz\s*"
        text = re.sub(token_pattern, lambda _match: original, text, flags=re.IGNORECASE)
    return text


def chunk_text(text: str, max_chars: int) -> list[str]:
    if len(text) <= max_chars:
        return [text]

    chunks: list[str] = []
    current: list[str] = []
    current_size = 0
    for paragraph in re.split(r"(\n{2,})", text):
        if current_size + len(paragraph) > max_chars and current:
            chunks.append("".join(current))
            current = []
            current_size = 0
        if len(paragraph) > max_chars:
            for offset in range(0, len(paragraph), max_chars):
                if current:
                    chunks.append("".join(current))
                    current = []
                    current_size = 0
                chunks.append(paragraph[offset : offset + max_chars])
            continue
        current.append(paragraph)
        current_size += len(paragraph)
    if current:
        chunks.append("".join(current))
    return chunks


def display_path(path: Path, root: Path) -> str:
    try:
        return path.relative_to(root).as_posix()
    except ValueError:
        return Path(os.path.relpath(path, root)).as_posix()


def split_line_ending(line: str) -> tuple[str, str]:
    if line.endswith("\r\n"):
        return line[:-2], "\r\n"
    if line.endswith("\n"):
        return line[:-1], "\n"
    return line, ""


def has_translatable_text(text: str) -> bool:
    return re.search(r"[A-Za-z]", text) is not None


def split_plain(text: str) -> tuple[str, str, str]:
    leading_match = re.match(r"\s*", text)
    trailing_match = re.search(r"\s*$", text)
    leading = leading_match.group(0) if leading_match else ""
    trailing = trailing_match.group(0) if trailing_match else ""
    core_start = len(leading)
    core_end = len(text) - len(trailing)
    return leading, text[core_start:core_end], trailing


def translate_plain(text: str, translate: Callable[[str], str], max_chars: int) -> str:
    if not has_translatable_text(text):
        return text

    leading, core, trailing = split_plain(text)

    if not has_translatable_text(core):
        return text

    translated_chunks = [translate(chunk) for chunk in chunk_text(core, max_chars)]
    return leading + "".join(translated_chunks) + trailing


def translate_protected_plain(text: str, translate: Callable[[str], str], max_chars: int) -> str:
    output: list[str] = []
    index = 0
    for match in PROTECTED_PATTERN.finditer(text):
        output.append(translate_plain(text[index : match.start()], translate, max_chars))
        output.append(match.group(0))
        index = match.end()
    output.append(translate_plain(text[index:], translate, max_chars))
    return "".join(output)


def translate_inline(text: str, translate: Callable[[str], str], max_chars: int) -> str:
    output: list[str] = []
    index = 0
    for match in LINK_PATTERN.finditer(text):
        output.append(translate_protected_plain(text[index : match.start()], translate, max_chars))
        if match.group(3) is not None:
            marker = match.group(1)
            label = translate_inline(match.group(2), translate, max_chars)
            destination = match.group(3)
            output.append(f"{marker}[{label}]({destination})")
        else:
            marker = match.group(4)
            label = translate_inline(match.group(5), translate, max_chars)
            reference = match.group(6)
            output.append(f"{marker}[{label}][{reference}]")
        index = match.end()
    output.append(translate_protected_plain(text[index:], translate, max_chars))
    return "".join(output)


def translate_table_line(line: str, translate: Callable[[str], str], max_chars: int) -> str:
    content, ending = split_line_ending(line)
    parts = content.split("|")
    translated_parts: list[str] = []
    for part in parts:
        leading_match = re.match(r"\s*", part)
        trailing_match = re.search(r"\s*$", part)
        leading = leading_match.group(0) if leading_match else ""
        trailing = trailing_match.group(0) if trailing_match else ""
        core = part[len(leading) : len(part) - len(trailing)]
        translated_parts.append(leading + translate_inline(core, translate, max_chars) + trailing)
    return "|".join(translated_parts) + ending


def translate_markdown_line(line: str, translate: Callable[[str], str], max_chars: int) -> str:
    content, ending = split_line_ending(line)

    if "|" in content and not TABLE_SEPARATOR_PATTERN.match(content):
        return translate_table_line(line, translate, max_chars)

    heading = re.match(r"^(\s{0,3}#{1,6}\s+)(.*?)(\s+#+\s*)?$", content)
    if heading:
        suffix = heading.group(3) or ""
        return heading.group(1) + translate_inline(heading.group(2), translate, max_chars) + suffix + ending

    blockquote = re.match(r"^(\s{0,3}(?:>\s*)+)(.*)$", content)
    if blockquote:
        return blockquote.group(1) + translate_inline(blockquote.group(2), translate, max_chars) + ending

    list_item = re.match(r"^(\s{0,3}(?:[-+*]|\d+[.)])\s+(?:\[[ xX]\]\s+)?)(.*)$", content)
    if list_item:
        return list_item.group(1) + translate_inline(list_item.group(2), translate, max_chars) + ending

    return translate_inline(content, translate, max_chars) + ending


def iter_plain_segments(text: str) -> Iterable[str]:
    index = 0
    for match in PROTECTED_PATTERN.finditer(text):
        yield text[index : match.start()]
        index = match.end()
    yield text[index:]


def collect_inline_text(text: str, items: list[str], max_chars: int) -> None:
    index = 0
    for match in LINK_PATTERN.finditer(text):
        collect_protected_plain(text[index : match.start()], items, max_chars)
        if match.group(3) is not None:
            collect_inline_text(match.group(2), items, max_chars)
        else:
            collect_inline_text(match.group(5), items, max_chars)
        index = match.end()
    collect_protected_plain(text[index:], items, max_chars)


def collect_protected_plain(text: str, items: list[str], max_chars: int) -> None:
    for segment in iter_plain_segments(text):
        _, core, _ = split_plain(segment)
        if has_translatable_text(core):
            items.extend(chunk_text(core, max_chars))


def collect_table_line(line: str, items: list[str], max_chars: int) -> None:
    content, _ = split_line_ending(line)
    for part in content.split("|"):
        _, core, _ = split_plain(part)
        collect_inline_text(core, items, max_chars)


def collect_markdown_line(line: str, items: list[str], max_chars: int) -> None:
    content, _ = split_line_ending(line)

    if "|" in content and not TABLE_SEPARATOR_PATTERN.match(content):
        collect_table_line(line, items, max_chars)
        return

    heading = re.match(r"^(\s{0,3}#{1,6}\s+)(.*?)(\s+#+\s*)?$", content)
    if heading:
        collect_inline_text(heading.group(2), items, max_chars)
        return

    blockquote = re.match(r"^(\s{0,3}(?:>\s*)+)(.*)$", content)
    if blockquote:
        collect_inline_text(blockquote.group(2), items, max_chars)
        return

    list_item = re.match(r"^(\s{0,3}(?:[-+*]|\d+[.)])\s+(?:\[[ xX]\]\s+)?)(.*)$", content)
    if list_item:
        collect_inline_text(list_item.group(2), items, max_chars)
        return

    collect_inline_text(content, items, max_chars)


def collect_markdown_text(text: str, max_chars: int, preserve_structure: bool, compact_blocks: bool) -> list[str]:
    _, body = split_front_matter(text)
    items: list[str] = []
    pending: list[str] = []
    active_fence: str | None = None

    def flush_pending() -> None:
        if not pending:
            return
        block = "".join(pending)
        if not has_translatable_text(block):
            pending.clear()
            return
        protected_block, _ = protect_inline(block)
        items.extend(chunk_text(protected_block, max_chars))
        pending.clear()

    for line in body.splitlines(keepends=True):
        fence_marker = is_code_fence(line, active_fence)
        if compact_blocks and not line.strip() and active_fence is None:
            pending.append(line)
            continue
        if fence_marker is not None or should_preserve_line(line, active_fence is not None) or is_protected_only_markdown_line(line):
            flush_pending()
            if fence_marker is not None:
                active_fence = fence_marker
            elif active_fence and line.lstrip().startswith(active_fence):
                active_fence = None
            continue
        if preserve_structure and should_translate_line_separately(line):
            flush_pending()
            collect_markdown_line(line, items, max_chars)
            continue
        pending.append(line)

    flush_pending()
    return items


def translate_markdown(text: str, translate: Callable[[str], str], max_chars: int, preserve_structure: bool, compact_blocks: bool) -> str:
    front_matter, body = split_front_matter(text)
    output: list[str] = [front_matter]
    pending: list[str] = []
    active_fence: str | None = None

    def flush_pending() -> None:
        if not pending:
            return
        output.append(translate_protected_block("".join(pending), translate, max_chars))
        pending.clear()

    for line in body.splitlines(keepends=True):
        fence_marker = is_code_fence(line, active_fence)
        if compact_blocks and not line.strip() and active_fence is None:
            pending.append(line)
            continue
        if fence_marker is not None or should_preserve_line(line, active_fence is not None) or is_protected_only_markdown_line(line):
            flush_pending()
            output.append(line)
            if fence_marker is not None:
                active_fence = fence_marker
            elif active_fence and line.lstrip().startswith(active_fence):
                active_fence = None
            continue
        if preserve_structure and should_translate_line_separately(line):
            flush_pending()
            output.append(translate_markdown_line(line, translate, max_chars))
            continue
        pending.append(line)

    flush_pending()
    return "".join(output)


def passthrough_provider() -> Callable[[str], str]:
    def translate(text: str) -> str:
        return text

    return translate


def argos_provider(source: str, target: str, batch_size: int) -> Callable[[str], str]:
    return ArgosBatchProvider(source, target, batch_size)


def libretranslate_provider(source: str, target: str, delay: float) -> Callable[[str], str]:
    endpoint = os.environ.get("LIBRETRANSLATE_URL")
    if not endpoint:
        raise TranslationError("Set LIBRETRANSLATE_URL before running with --provider libretranslate.")
    api_key = os.environ.get("LIBRETRANSLATE_API_KEY", "")
    endpoint = endpoint.rstrip("/") + "/translate"

    def translate(text: str) -> str:
        payload = json.dumps(
            {
                "q": text,
                "source": source,
                "target": target,
                "format": "text",
                "api_key": api_key,
            }
        ).encode("utf-8")
        request = urllib.request.Request(
            endpoint,
            data=payload,
            headers={"Content-Type": "application/json"},
            method="POST",
        )
        try:
            with urllib.request.urlopen(request, timeout=60) as response:
                result = json.loads(response.read().decode("utf-8"))
        except (urllib.error.URLError, TimeoutError) as exc:
            raise TranslationError(f"LibreTranslate request failed: {exc}") from exc
        if delay:
            time.sleep(delay)
        translated = result.get("translatedText")
        if not isinstance(translated, str):
            raise TranslationError(f"LibreTranslate returned an unexpected response: {result}")
        return translated

    return translate


def translators_provider(service: str, source: str, target: str, delay: float) -> Callable[[str], str]:
    try:
        import translators as ts
    except ImportError as exc:
        raise TranslationError(
            "The translators package is not installed. Install it in Python first, then rerun with --provider translators."
        ) from exc

    target_map = {
        "zh-Hans": "zh",
        "zh-CN": "zh",
    }
    source_code = "auto" if source == "auto" else source
    target_code = target_map.get(target, target)

    def translate(text: str) -> str:
        translated = ts.translate_text(
            text,
            translator=service,
            from_language=source_code,
            to_language=target_code,
            timeout=60,
        )
        if delay:
            time.sleep(delay)
        return translated

    return translate


def azure_provider(source: str, target: str, delay: float) -> Callable[[str], str]:
    endpoint = os.environ.get("AZURE_TRANSLATOR_ENDPOINT")
    key = os.environ.get("AZURE_TRANSLATOR_KEY")
    region = os.environ.get("AZURE_TRANSLATOR_REGION")
    if not endpoint or not key:
        raise TranslationError("Set AZURE_TRANSLATOR_ENDPOINT and AZURE_TRANSLATOR_KEY before running with --provider azure.")

    endpoint = endpoint.rstrip("/") + "/translate?api-version=3.0"
    if source != "auto":
        endpoint += f"&from={source}"
    endpoint += f"&to={target}"

    headers = {
        "Content-Type": "application/json",
        "Ocp-Apim-Subscription-Key": key,
    }
    if region:
        headers["Ocp-Apim-Subscription-Region"] = region

    def translate(text: str) -> str:
        payload = json.dumps([{"text": text}]).encode("utf-8")
        request = urllib.request.Request(endpoint, data=payload, headers=headers, method="POST")
        try:
            with urllib.request.urlopen(request, timeout=60) as response:
                result = json.loads(response.read().decode("utf-8"))
        except (urllib.error.URLError, TimeoutError) as exc:
            raise TranslationError(f"Azure Translator request failed: {exc}") from exc
        if delay:
            time.sleep(delay)
        try:
            return result[0]["translations"][0]["text"]
        except (IndexError, KeyError, TypeError) as exc:
            raise TranslationError(f"Azure Translator returned an unexpected response: {result}") from exc

    return translate


def build_provider(args: argparse.Namespace) -> Callable[[str], str]:
    if args.provider == "passthrough":
        return passthrough_provider()
    if args.provider == "argos":
        return argos_provider(args.source, args.target, args.batch_size)
    if args.provider == "libretranslate":
        return libretranslate_provider(args.source, args.target, args.delay)
    if args.provider == "translators":
        return translators_provider(args.translator_service, args.source, args.target, args.delay)
    if args.provider == "azure":
        return azure_provider(args.source, args.target, args.delay)
    raise TranslationError(f"Unknown provider: {args.provider}")


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Translate all Markdown files into a mirrored output directory.")
    parser.add_argument("--root", default=".", help="Repository root. Defaults to the current directory.")
    parser.add_argument("--output", default=DEFAULT_OUTPUT_DIR, help="Output folder. Defaults to ./翻译.")
    parser.add_argument("--provider", choices=["argos", "azure", "libretranslate", "passthrough", "translators"], required=True)
    parser.add_argument("--translator-service", default="bing", help="Service name for --provider translators. Defaults to bing.")
    parser.add_argument("--source", default="en", help="Source language code. Use auto for providers that support detection.")
    parser.add_argument("--target", default="zh-Hans", help="Target language code. Defaults to zh-Hans.")
    parser.add_argument("--max-chars", type=int, default=3500, help="Maximum characters sent per translation request.")
    parser.add_argument("--batch-size", type=int, default=256, help="Number of unique text segments translated per batch.")
    parser.add_argument("--workers", type=int, default=1, help="Concurrent workers for providers without native batch translation.")
    parser.add_argument("--cache", default="", help="Optional JSONL translation cache path.")
    parser.add_argument("--delay", type=float, default=0.0, help="Delay between provider requests, in seconds.")
    parser.add_argument("--limit", type=int, default=0, help="Only process the first N files.")
    parser.add_argument("--preserve-structure", action="store_true", help="Translate Markdown headings, lists, blockquotes, and tables line by line to preserve more syntax.")
    parser.add_argument("--compact-blocks", action="store_true", help="Keep blank-line-separated Markdown text together for faster translation.")
    parser.add_argument("--dry-run", action="store_true", help="Print planned files without writing translated output.")
    parser.add_argument("--overwrite", action="store_true", help="Overwrite translated files that already exist.")
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    root = Path(args.root).resolve()
    output_dir = (root / args.output).resolve()
    files = list(iter_markdown_files(root, output_dir))
    if args.limit:
        files = files[: args.limit]

    print(f"Markdown files: {len(files)}")
    print(f"Output: {output_dir}")
    if args.dry_run:
        for path in files:
            print(display_path(path, root), "->", display_path(output_dir / path.relative_to(root), root))
        return 0

    pending_files: list[Path] = []
    for path in files:
        destination = output_dir / path.relative_to(root)
        if destination.exists() and not args.overwrite:
            continue
        pending_files.append(path)

    try:
        cache_path = Path(args.cache).resolve() if args.cache else None
        translate = CachedTranslator(build_provider(args), args.batch_size, args.workers, cache_path)
    except TranslationError as exc:
        print(f"error: {exc}", file=sys.stderr)
        return 2

    if pending_files:
        print(f"Collecting translatable segments: {len(pending_files)} files")
        all_segments: list[str] = []
        for path in pending_files:
            all_segments.extend(collect_markdown_text(path.read_text(encoding="utf-8"), args.max_chars, args.preserve_structure, args.compact_blocks))
        unique_segments = list(dict.fromkeys(all_segments))
        print(f"Translatable segments: {len(unique_segments)} unique")
        translate.translate_many(unique_segments)

    for index, path in enumerate(files, start=1):
        relative = path.relative_to(root)
        destination = output_dir / relative
        if destination.exists() and not args.overwrite:
            print(f"[{index}/{len(files)}] skip {relative.as_posix()}")
            continue

        destination.parent.mkdir(parents=True, exist_ok=True)
        text = path.read_text(encoding="utf-8")
        translated = translate_markdown(text, translate, args.max_chars, args.preserve_structure, args.compact_blocks)
        destination.write_text(translated, encoding="utf-8", newline="")
        print(f"[{index}/{len(files)}] wrote {display_path(destination, root)}")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
