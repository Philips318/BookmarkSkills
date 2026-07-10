<#
.SYNOPSIS
  Build a 3PP Device Master Record (DMR) .docx from a JSON data file.
  Part of the generate-3pp-dmr skill. PowerShell + Word COM.

.DESCRIPTION
  The agent maps the CIP/TRR/supplier inputs into a JSON file (schema below),
  then runs this script to produce the standard-template DMR Word document.
  Sections, headings, tables, and the Table of Contents follow the Philips 3PP DMR template.

.PARAMETER DataPath
  Path to the JSON data file describing the DMR content.

.PARAMETER OutPath
  Optional output .docx path. If omitted, uses the JSON 'outputPath', else '<documentId> <title>.docx'.

.PARAMETER Sample
  If set, writes a sample JSON to -DataPath (or .\dmr_sample.json) and builds it. For self-test.

.JSON SCHEMA (all string values; arrays of objects for tables)
  {
    "documentId": "D00XXXXXXX",
    "title": "Guerbet OptiVantage Injector",
    "outputPath": "C:\\path\\out.docx",
    "overviewSentence": "This document defines the specifications of ... as detailed in the PLM tool.",
    "productName": "Guerbet OptiVantage Injector",
    "interfaces": "….",
    "compatibilityStatement": "Compatibility Statement can be found in Appendix A, B and C.",
    "installation": "….",
    "terminology":     [ { "term":"CT", "def":"Computed Tomography" } ],
    "twelveNc":        [ { "nc":"989806101803", "name":"…", "model":"…" } ],
    "functionality":   [ "sentence 1", "sentence 2" ],
    "manufacturer":    [ { "mfr":"Guerbet", "model":"OptiVantage Single-Use", "pn":"V8431 (223571)…" } ],
    "affectedProducts":[ { "product":"Ingenuity CT", "number":"728321 728323…" } ],
    "references":      [ { "num":"1", "title":"…", "id":"D001564844 Rev A" } ],
    "revisionHistory": [ { "rev":"A", "date":"Per PLM tool", "author":"…", "desc":"Initial release", "cr":"CN735557" } ],
    "appendices":      [ { "letter":"A", "fileName":"Appendix A of …", "title":"…" } ]
  }
#>
param(
    [string]$DataPath,
    [string]$OutPath,
    [switch]$Sample
)
$ErrorActionPreference = 'Stop'

# ---------------- Boilerplate constants ----------------
$BP_PURPOSE = "This document describes the detailed technical design of an element of the product."
$BP_SCOPE   = "This document applies to Philips CT/AMI."
$BP_CONFID  = 'All sheets of this document contain confidential and proprietary information of Philips healthcare ("Philips") and are intended for use by current Philips personnel. Copying, disclosure to others, or other use is prohibited without the express written authorization of the Philips'' law department. Report violations of this requirement to the Philips law department.'
$BP_ARCH    = "N/A-This is a 3rd party item and the architecture views are the responsibility of the manufacturer."
$BP_QUAL    = "N/A-This is a 3rd party item and the quality aspects are the responsibility of the manufacturer."
$BP_DETAIL  = "N/A-This is a 3rd party item and the detailed design is the responsibility of the manufacturer."
$BP_CONSTR  = "N/A-This is a 3rd party item, and the manufacturer is responsible for the design."
$BP_ROBUST  = "N/A-This is a 3rd party item and the design robustness is the responsibility of the manufacturer."
$BP_NOTE    = "Note: for template information, see custom properties of this document."

# ---------------- Sample data (self-test) ----------------
if ($Sample) {
    if (-not $DataPath) { $DataPath = Join-Path (Get-Location) "dmr_sample.json" }
    $sampleData = [ordered]@{
        documentId       = "D00SAMPLE1"
        title            = "Sample Vendor SampleInjector"
        outputPath       = [System.IO.Path]::ChangeExtension($DataPath, ".docx")
        overviewSentence = "This document defines the specifications of Sample Vendor SampleInjector for Incisive CT (728143)/CT 5300 (728285) as detailed in the PLM tool."
        productName      = "Sample Vendor SampleInjector"
        interfaces       = "Sample Vendor SampleInjector interfaces with CT gantry by an electrical cable designed by Philips. The 12 NC of this cable is 454110124243."
        compatibilityStatement = "Compatibility Statement can be found in Appendix A and B."
        installation     = "Sample Vendor SampleInjector is a 3rd party injector, the installation shall refer the service manual of Sample Vendor."
        terminology      = @(
            @{ term = "CT";  def = "Computed Tomography" },
            @{ term = "DMR"; def = "Device Master Record" },
            @{ term = "P/N"; def = "Part Number" }
        )
        twelveNc         = @(
            @{ nc = "989806101999"; name = "Sample Injector - Ped"; model = "SampleInjector Dual Head - Pedestal Model 1000 (S1)" },
            @{ nc = "989806102000"; name = "Sample Injector - Ceil"; model = "SampleInjector Dual Head - Ceiling Model 1001 (S2)" }
        )
        functionality    = @(
            "Sample Vendor SampleInjector is used to inject contrast agent and normal saline into the human body in the Contrast Scan workflow to improve the contrast of CT images.",
            "Spiral Auto Scan or Start Automatic Scan (SAS) - Provides an interface that triggers both the CT scan process and the injector injection process from the injector panel at the same time."
        )
        manufacturer     = @(
            @{ mfr = "Sample Vendor"; model = "SampleInjector Single-Use"; pn = "S1 (1000) S2 (1001)" }
        )
        affectedProducts = @(
            @{ product = "Incisive CT"; number = "728143 728144 728147" },
            @{ product = "CT 5300";     number = "728285 728286" }
        )
        references       = @(
            @{ num = "1"; title = "Technical Review Report Of Compatibility With Sample Injector"; id = "D00TRR0001 Rev A" },
            @{ num = "2"; title = "Technical Review Report of CANOpen Function for Taichi";        id = "D002112428 Rev B" }
        )
        revisionHistory  = @(
            @{ rev = "A"; date = "Per PLM tool"; author = "[TBD]"; desc = "Initial release"; cr = "[TBD]" }
        )
        appendices       = @(
            @{ letter = "A"; fileName = "Appendix A of D00SAMPLE1 Sample Vendor Compatibility Statement"; title = "Sample Vendor Compatibility Statement" },
            @{ letter = "B"; fileName = "Appendix B of D00SAMPLE1 Sample Vendor Operators Manual";       title = "Sample Vendor Operators Manual" }
        )
    }
    ($sampleData | ConvertTo-Json -Depth 6) | Out-File -FilePath $DataPath -Encoding UTF8
    Write-Host "Sample JSON written to: $DataPath"
}

if (-not $DataPath -or -not (Test-Path $DataPath)) {
    throw "DataPath '$DataPath' not found. Provide a JSON data file (see schema in the script header) or run with -Sample."
}

$data = Get-Content -LiteralPath $DataPath -Raw -Encoding UTF8 | ConvertFrom-Json

if (-not $OutPath) {
    if ($data.outputPath) { $OutPath = [string]$data.outputPath }
    else { $OutPath = Join-Path (Split-Path -Parent (Resolve-Path $DataPath)) ("{0} {1}.docx" -f $data.documentId, $data.title) }
}
$OutPath = [string]$OutPath

# ---------------- Word COM ----------------
$word = New-Object -ComObject Word.Application
$word.Visible = $false
$wdSeekPrimaryHeaderFooter = 1
try {
    $doc = $word.Documents.Add()
    $sel = $word.Selection

    function Set-Style([string]$name) { try { $sel.Style = $doc.Styles.Item($name) } catch {} }
    function Add-Para([string]$text, [string]$style = "Normal") {
        Set-Style $style
        $sel.TypeText([string]$text)
        $sel.TypeParagraph()
    }
    function Add-Table($rows, $headers, [int]$cols) {
        # $rows = array of string[] ; $headers = string[] or $null
        $allRows = @()
        if ($headers) { $allRows += ,$headers }
        foreach ($r in $rows) { $allRows += ,$r }
        $nRows = $allRows.Count
        if ($nRows -eq 0) { return }
        Set-Style "Normal"
        $range = $sel.Range
        $tbl = $doc.Tables.Add($range, $nRows, $cols)
        $tbl.Borders.Enable = 1
        for ($i = 0; $i -lt $nRows; $i++) {
            $rowCells = $allRows[$i]
            for ($j = 0; $j -lt $cols; $j++) {
                $val = ""
                if ($j -lt $rowCells.Count) { $val = [string]$rowCells[$j] }
                $cell = $tbl.Cell($i + 1, $j + 1)
                $cell.Range.Text = $val
                if ($headers -and $i -eq 0) { $cell.Range.Bold = 1 }
            }
        }
        # move past table
        $sel.EndKey(6) | Out-Null   # wdStory
    }

    # ---- Title ----
    Add-Para ([string]$data.title) "Title"
    if ($data.documentId) { Add-Para ("Document ID: " + [string]$data.documentId) "Normal" }

    # ---- Table of Contents ----
    Add-Para "Table of Contents" "Heading 1"
    Set-Style "Normal"
    $tocRange = $sel.Range
    $doc.TablesOfContents.Add($tocRange, $true, 1, 3) | Out-Null
    $sel.EndKey(6) | Out-Null
    $sel.TypeParagraph()

    # ---- 1 Purpose ----
    Add-Para "Purpose" "Heading 1"
    Add-Para $BP_PURPOSE

    # ---- 2 Scope ----
    Add-Para "Scope" "Heading 1"
    Add-Para $BP_SCOPE

    # ---- 3 Terminology & Abbreviations ----
    Add-Para "Terminology & Abbreviations" "Heading 1"
    $termRows = @()
    foreach ($t in $data.terminology) { $termRows += ,@([string]$t.term, [string]$t.def) }
    Add-Table $termRows @("Terminology & Abbreviations", "Description/Definition") 2

    # ---- 4 Overview ----
    Add-Para "Overview" "Heading 1"
    if ($data.overviewSentence) { Add-Para ([string]$data.overviewSentence) }
    Add-Para ("The detail 12NC list of " + [string]$data.productName + " are listed below:")
    $ncRows = @()
    foreach ($n in $data.twelveNc) { $ncRows += ,@([string]$n.nc, [string]$n.name, [string]$n.model) }
    Add-Table $ncRows @("12NC", "12NC Name", "Model Numbers and Description") 3

    # ---- 5 Architecture Views ----
    Add-Para "Architecture Views" "Heading 1"
    Add-Para $BP_ARCH

    # ---- 6 Design Details ----
    Add-Para "Design Details" "Heading 1"
    Add-Para $BP_CONFID
    Add-Para "Allocation of Quality Aspects" "Heading 2"
    Add-Para $BP_QUAL
    Add-Para "Element detailed design" "Heading 2"
    Add-Para $BP_DETAIL
    Add-Para "Interfaces" "Heading 2"
    Add-Para ([string]$data.interfaces)
    Add-Para "Parts" "Heading 2"
    Add-Para ([string]$data.productName) "Heading 3"
    Add-Para "Functionality" "Heading 4"
    foreach ($f in $data.functionality) { Add-Para ([string]$f) }
    Add-Para "Design Constraints" "Heading 4"
    Add-Para $BP_CONSTR
    Add-Para "Compatibility Statement" "Heading 4"
    Add-Para ([string]$data.compatibilityStatement)
    Add-Para "Manufacturer" "Heading 4"
    $mfrRows = @()
    foreach ($m in $data.manufacturer) { $mfrRows += ,@([string]$m.mfr, [string]$m.model, [string]$m.pn) }
    Add-Table $mfrRows @("Manufacturer", "Manufacturer Model", "Manufacturer P/N") 3
    Add-Para "Installation" "Heading 4"
    Add-Para ([string]$data.installation)
    Add-Para "Affected Products" "Heading 4"
    Add-Para "Philips CT System"
    $apRows = @()
    foreach ($a in $data.affectedProducts) { $apRows += ,@([string]$a.product, [string]$a.number) }
    Add-Table $apRows @("Product", "Product Number") 2
    Add-Para "Design robustness" "Heading 2"
    Add-Para $BP_ROBUST

    # ---- 7 References ----
    Add-Para "References" "Heading 1"
    $refRows = @()
    foreach ($r in $data.references) { $refRows += ,@([string]$r.num, [string]$r.title, [string]$r.id) }
    Add-Table $refRows @("Reference Number", "Document Title", "Document ID") 3

    # ---- 8 Document Revision History ----
    Add-Para "Document Revision History" "Heading 1"
    $revRows = @()
    foreach ($r in $data.revisionHistory) { $revRows += ,@([string]$r.rev, [string]$r.date, [string]$r.author, [string]$r.desc, [string]$r.cr) }
    Add-Table $revRows @("Revision", "Release Date", "Author", "Description of changes", "CR / Reason") 5

    # ---- 9 Appendices ----
    Add-Para "Appendices" "Heading 1"
    foreach ($ap in $data.appendices) {
        $hdr = "Appendix " + [string]$ap.letter + " - " + [string]$ap.title
        Add-Para $hdr "Heading 2"
        $body = 'The file "' + [string]$ap.fileName + '" is attached to this record in the PLM tool and represents Appendix ' + [string]$ap.letter + ' of this document. This is a file of external origin.'
        Add-Para $body
    }
    Add-Para $BP_NOTE

    # ---- Update TOC fields ----
    foreach ($toc in $doc.TablesOfContents) { $toc.Update() }

    # ---- Save (FIXED: [string] path + SaveAs2 + wdFormatDocumentDefault=16) ----
    $outFull = [string]$OutPath
    $outDir = Split-Path -Parent $outFull
    if ($outDir -and -not (Test-Path $outDir)) { New-Item -ItemType Directory -Force -Path $outDir | Out-Null }
    $doc.SaveAs2($outFull, 16)
    Write-Host "DMR saved to: $outFull"
    $doc.Close($false)
}
catch {
    Write-Host "ERROR: $_" -ForegroundColor Red
    throw
}
finally {
    $word.Quit()
    [System.Runtime.InteropServices.Marshal]::ReleaseComObject($word) | Out-Null
    [System.GC]::Collect(); [System.GC]::WaitForPendingFinalizers()
}
