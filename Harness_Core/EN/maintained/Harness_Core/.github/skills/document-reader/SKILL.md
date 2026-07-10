---
name: document-reader
description: Extracting and structuring requirements from input artifacts: Word (.docx), PDF, Excel (.xlsx), CSV, or Markdown PRD files. Use when ingesting a PRD, telemetry file, or any unstructured input document to produce FR-XX/NFR-XX requirements.
---

# Document Reader Skill

## Reading DOCX Files

Use `pandoc` to convert Word documents to Markdown, then read the Markdown:

```powershell
pandoc -f docx -t markdown "Input PRD/MyPRD.docx" -o "$env:TEMP\prd_temp.md"
# Read prd_temp.md with standard file tools
# Delete temp file after reading
Remove-Item "$env:TEMP\prd_temp.md"
```

Alternative — use Python `python-docx` for structured extraction:

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

## Reading PDF Files

Use Python `pypdf`:

```powershell
python -c "
import pypdf
reader = pypdf.PdfReader('Input PRD/Spec.pdf')
for i, page in enumerate(reader.pages):
    print(f'--- Page {i+1} ---')
    print(page.extract_text())
"
```

## Reading XLSX Files

Use Python `openpyxl`:

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

## Extraction Strategy

1. **Extract headings first** — they give the document hierarchy; map to requirements sections
2. **Locate tables** — requirements are frequently in tabular format; extract all columns
3. **Extract numbered lists** — typically requirement items; preserve numbering as IDs
4. **Note cross-references** — if a section says "see §4.2", flag it and follow the reference
5. **Extract embedded images** — see Image Extraction below; derive requirements from diagrams
6. **Flag figures/diagrams** — describe what they show; note page/section location

## Image Extraction

Embedded images in `.docx` and `.pdf` files must be extracted before they can be viewed.

### From DOCX (images stored in word/media/):

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

### From PDF (requires pdf2image or pymupdf):

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

### From Markdown PRDs:

Images referenced in `.md` files (e.g. `![diagram](images/arch.png)`) can be read directly — use the file read tool on the image path.

## Encoding and Cleanup

After extraction:
- Normalise whitespace (multiple spaces → single space; `\r\n` → `\n`)
- Remove page headers/footers (repeated lines with page numbers at top/bottom of each page)
- Flag unclear or truncated text: `[UNCLEAR: ...]`
- Flag cross-references not yet resolved: `[SEE: PRD §4.2 — not yet extracted]`

## Verification

- Spot-check 3–5 extracted requirements against the original document
- Flag any heading that has no extracted content: `[MISSING: heading name]`
