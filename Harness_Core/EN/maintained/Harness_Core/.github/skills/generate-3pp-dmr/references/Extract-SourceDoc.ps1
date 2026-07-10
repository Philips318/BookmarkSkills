<#
.SYNOPSIS
  Extract paragraphs (with style names) and tables from a Word .docx for 3PP DMR drafting.
  Read-only; merged-cell safe. Part of the generate-3pp-dmr skill.

.PARAMETER DocPath
  Path to the source .docx (CIP, TRR, or supplier document).

.PARAMETER OutPath
  Path to write the extracted UTF-8 text dump.

.EXAMPLE
  .\Extract-SourceDoc.ps1 -DocPath "E:\3PP\Taichi CIP\CR244787 CIP.docx" -OutPath ".\_dmr_work\CIP.txt"
#>
param(
    [Parameter(Mandatory = $true)][string]$DocPath,
    [Parameter(Mandatory = $true)][string]$OutPath
)
$ErrorActionPreference = 'Stop'

$outDir = Split-Path -Parent $OutPath
if ($outDir -and -not (Test-Path $outDir)) { New-Item -ItemType Directory -Force -Path $outDir | Out-Null }

$word = New-Object -ComObject Word.Application
$word.Visible = $false
$sb = New-Object System.Text.StringBuilder
try {
    $doc = $word.Documents.Open($DocPath, $false, $true)  # ConfirmConversions=$false, ReadOnly=$true
    [void]$sb.AppendLine("==== DOC: $DocPath ====")
    [void]$sb.AppendLine("Paragraphs: $($doc.Paragraphs.Count); Tables: $($doc.Tables.Count)")
    [void]$sb.AppendLine("---- PARAGRAPHS (style | text) ----")
    $pi = 0
    foreach ($p in $doc.Paragraphs) {
        $pi++
        if ($pi -gt 1200) { [void]$sb.AppendLine("...(truncated paragraphs)"); break }
        $style = $p.Style.NameLocal
        $text = ($p.Range.Text -replace "[\r\n\a\t\x07]", " ").Trim()
        if ($text.Length -gt 0) { [void]$sb.AppendLine("[$style] $text") }
    }
    [void]$sb.AppendLine("")
    [void]$sb.AppendLine("---- TABLES ----")
    $ti = 0
    foreach ($tbl in $doc.Tables) {
        $ti++
        [void]$sb.AppendLine("### TABLE $ti  rows=$($tbl.Rows.Count) cols=$($tbl.Columns.Count)")
        $rowOk = $true
        try {
            $rmax = [Math]::Min($tbl.Rows.Count, 60)
            for ($r = 1; $r -le $rmax; $r++) {
                $cells = @()
                $ccount = $tbl.Rows.Item($r).Cells.Count
                for ($c = 1; $c -le $ccount; $c++) {
                    try {
                        $ct = ($tbl.Cell($r, $c).Range.Text -replace "[\r\n\a\t\x07]", " ").Trim()
                        $cells += $ct
                    } catch { $cells += "" }
                }
                [void]$sb.AppendLine("R${r}: " + ($cells -join " | "))
            }
            if ($tbl.Rows.Count -gt $rmax) { [void]$sb.AppendLine("...(more rows)") }
        } catch {
            $rowOk = $false
        }
        if (-not $rowOk) {
            [void]$sb.AppendLine("(merged cells - linear cell dump)")
            $ci = 0
            foreach ($cell in $tbl.Range.Cells) {
                $ci++
                if ($ci -gt 300) { [void]$sb.AppendLine("...(truncated cells)"); break }
                $ct = ($cell.Range.Text -replace "[\r\n\a\t\x07]", " ").Trim()
                [void]$sb.AppendLine("C[$($cell.RowIndex),$($cell.ColumnIndex)]: $ct")
            }
        }
        [void]$sb.AppendLine("")
    }
    $doc.Close($false)
} finally {
    $word.Quit()
    [System.Runtime.InteropServices.Marshal]::ReleaseComObject($word) | Out-Null
    [System.GC]::Collect(); [System.GC]::WaitForPendingFinalizers()
}
$sb.ToString() | Out-File -FilePath $OutPath -Encoding UTF8
Write-Host "Wrote $OutPath"
