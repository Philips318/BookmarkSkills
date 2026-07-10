---
name: document-reader
description: 从输入工件中提取并结构化需求：Word (.docx)、PDF、Excel (.xlsx)、CSV 或 Markdown PRD files。在摄取 PRD、telemetry file 或任何非结构化输入文档以产出 FR-XX/NFR-XX requirements 时使用。
---

# Document Reader Skill

## 读取 DOCX 文件

使用 `pandoc` 将 Word 文档转换为 Markdown，然后读取 Markdown：

```powershell
pandoc -f docx -t markdown "Input PRD/MyPRD.docx" -o "$env:TEMP\prd_temp.md"
# Read prd_temp.md with standard file tools
# Delete temp file after reading
Remove-Item "$env:TEMP\prd_temp.md"
```

替代方式 — 使用 Python `python-docx` 进行结构化提取：

```powershell
python -c "
from docx import Document
doc = Document('Input PRD/MyPRD.docx')
for para in doc.paragraphs:
    if para.text.strip():
        print(f'[{para.style.name}] {para.text}')
for table in doc.tables:
    for row in table.rows:
        print([cell.text for cell in row.cells])
"
```

## 读取 PDF 文件

使用 Python `pypdf`：

```powershell
python -c "
import pypdf
reader = pypdf.PdfReader('Input PRD/Spec.pdf')
for i, page in enumerate(reader.pages):
    print(f'--- Page {i+1} ---')
    print(page.extract_text())
"
```

## 读取 XLSX 文件

使用 Python `openpyxl`：

```powershell
python -c "
import openpyxl
wb = openpyxl.load_workbook('Input Telemetry/Data.xlsx')
for sheet_name in wb.sheetnames:
    ws = wb[sheet_name]
    print(f'=== Sheet: {sheet_name} ===')
    for row in ws.iter_rows(values_only=True):
        print(row)
"
```

## 提取策略

1. **先提取 headings** — 它们给出文档层级；映射到 requirements sections
2. **定位 tables** — requirements 经常以表格格式出现；提取所有列
3. **提取 numbered lists** — 通常是 requirement items；保留编号作为 IDs
4. **记录 cross-references** — 如果某节说 “see §4.2”，标记并跟踪该引用
5. **提取 embedded images** — 见下面的 Image Extraction；从 diagrams 推导 requirements
6. **标记 figures/diagrams** — 描述它们展示的内容；记录 page/section location

## Image Extraction

`.docx` 和 `.pdf` 文件中的嵌入图像必须先提取，之后才能查看。

### From DOCX（images stored in word/media/）：

```powershell
python -c "
import zipfile, os
output_dir = '$env:TEMP/prd_images'
os.makedirs(output_dir, exist_ok=True)
with zipfile.ZipFile('Input PRD/MyPRD.docx') as z:
    for name in z.namelist():
        if name.startswith('word/media/'):
            z.extract(name, output_dir)
            print(f'Extracted: {name}')
"
# Then read each extracted image file to view its content
```

### From PDF（requires pdf2image or pymupdf）：

```powershell
python -c "
import fitz  # pymupdf
import os
output_dir = '$env:TEMP/prd_images'
os.makedirs(output_dir, exist_ok=True)
doc = fitz.open('Input PRD/Spec.pdf')
for page_num, page in enumerate(doc):
    for img_idx, img in enumerate(page.get_images()):
        xref = img[0]
        base_image = doc.extract_image(xref)
        ext = base_image['ext']
        path = f'{output_dir}/page{page_num+1}_img{img_idx+1}.{ext}'
        with open(path, 'wb') as f:
            f.write(base_image['image'])
        print(f'Extracted: {path}')
"
# Then read each extracted image file to view its content
```

### From Markdown PRDs：

`.md` 文件中引用的 images（例如 `![diagram](images/arch.png)`）可以直接读取 — 对 image path 使用 file read tool。

## Encoding and Cleanup

提取后：
- Normalise whitespace（多个空格 → 单个空格；`\r\n` → `\n`）
- 移除 page headers/footers（每页顶部/底部带页码的重复行）
- 标记不清楚或截断文本：`[UNCLEAR: ...]`
- 标记尚未解析的 cross-references：`[SEE: PRD §4.2 — not yet extracted]`

## Verification

- 将 3–5 条提取出的 requirements 与原始文档抽查比对
- 标记没有提取到内容的 heading：`[MISSING: heading name]`
