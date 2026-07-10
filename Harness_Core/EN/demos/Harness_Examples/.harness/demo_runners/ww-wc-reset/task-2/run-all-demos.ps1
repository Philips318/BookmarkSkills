# WW/WC Reset Feature Demo Runner
# Drives CT Image Display App for Gate-3 demonstration.
# Uses Windows UI Automation (no FlaUI / ffmpeg required).
# Screenshots saved to .harness/demo_evidence/ww-wc-reset/

param(
    [string]$ExePath    = "Src\ImageDisplayApp\bin\Debug\net8.0-windows\Philips.CT.Host.ImageDisplay.App.exe",
    [string]$RepoRoot   = "C:\Work\Code\Git_Code\Harness_Enhance",
    [string]$EvidenceBase = ".harness\demo_evidence\ww-wc-reset"
)

Set-Location $RepoRoot
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

function Take-Screenshot([string]$OutPath, $Window) {
    $rect = $Window.Current.BoundingRectangle
    if ($rect.Width -le 0 -or $rect.Height -le 0) {
        Write-Host "  [WARN] Window rect invalid - skipping screenshot."
        return
    }
    $bmp = New-Object System.Drawing.Bitmap([int]$rect.Width, [int]$rect.Height)
    $g   = [System.Drawing.Graphics]::FromImage($bmp)
    $g.CopyFromScreen([int]$rect.X, [int]$rect.Y, 0, 0,
        [System.Drawing.Size]::new([int]$rect.Width, [int]$rect.Height),
        [System.Drawing.CopyPixelOperation]::SourceCopy)
    $g.Dispose()
    $bmp.Save($OutPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    Write-Host "  Screenshot saved: $OutPath"
}

function Find-Element($Root, [string]$AutomationId, [int]$TimeoutMs = 8000) {
    $cond = New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::AutomationIdProperty, $AutomationId)
    $deadline = (Get-Date).AddMilliseconds($TimeoutMs)
    while ((Get-Date) -lt $deadline) {
        $el = $Root.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $cond)
        if ($null -ne $el) { return $el }
        Start-Sleep -Milliseconds 200
    }
    return $null
}

function Get-ElementText($El) {
    if ($null -eq $El) { return "(element not found)" }
    try {
        $vp = $El.GetCurrentPattern([System.Windows.Automation.TextPattern]::Pattern)
        return ($vp.DocumentRange.GetText(-1)).Trim()
    } catch {
        return $El.Current.Name.Trim()
    }
}

function Click-Button($Root, [string]$AutomationId) {
    $el = Find-Element -Root $Root -AutomationId $AutomationId
    if ($null -eq $el) {
        Write-Host "  [WARN] Button '$AutomationId' not found."
        return $false
    }
    try {
        $ip = $el.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)
        $ip.Invoke()
        return $true
    } catch {
        Write-Host "  [WARN] InvokePattern failed on '$AutomationId': $_"
        return $false
    }
}

function Launch-App([string]$LaunchArgs) {
    $fullExe = Join-Path $RepoRoot $ExePath
    $argList = $LaunchArgs.Split(' ', [System.StringSplitOptions]::RemoveEmptyEntries)
    $proc = Start-Process -FilePath $fullExe -ArgumentList $argList -PassThru
    Start-Sleep -Seconds 3
    if ($proc.HasExited) { throw "App crashed on launch (exit $($proc.ExitCode))." }

    $desktop = [System.Windows.Automation.AutomationElement]::RootElement
    $pidCond = New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::ProcessIdProperty, $proc.Id)
    $deadline = (Get-Date).AddSeconds(12)
    $window = $null
    while ((Get-Date) -lt $deadline) {
        $window = $desktop.FindFirst([System.Windows.Automation.TreeScope]::Children, $pidCond)
        if ($null -ne $window) { break }
        Start-Sleep -Milliseconds 300
    }
    if ($null -eq $window) { throw "Could not locate main window for PID $($proc.Id)." }

    try {
        $wp = $window.GetCurrentPattern([System.Windows.Automation.WindowPattern]::Pattern)
        $wp.SetWindowVisualState([System.Windows.Automation.WindowVisualState]::Maximized)
    } catch {
        Write-Host "  [INFO] Maximise pattern not supported - continuing."
    }
    Start-Sleep -Milliseconds 800
    return @{ Process = $proc; Window = $window }
}

function Close-App($AppInfo) {
    try { $AppInfo.Process.CloseMainWindow() | Out-Null } catch {}
    Start-Sleep -Milliseconds 800
    if (-not $AppInfo.Process.HasExited) { $AppInfo.Process.Kill() }
    Start-Sleep -Milliseconds 400
}

# =============================================================================
#  DEMO 1 — T02  Happy path: Reset to DICOM default WW/WC
# =============================================================================
$demo2Steps   = [System.Collections.ArrayList]@()
$demo2EvidDir = Join-Path $RepoRoot "$EvidenceBase\task-2-20260702T000000"
New-Item -ItemType Directory -Force -Path $demo2EvidDir | Out-Null

Write-Host ""
Write-Host "=== DEMO 1 of 3 - T02 Happy Path ==="
Write-Host "Scenario: Reset restores DICOM default Window Width and Window Center"
Write-Host ""

try {
    Write-Host "Step 1: Launching app with --simulator --seed=demo ..."
    Write-Host "  Series loads with DICOM defaults WW=1500/WC=-600; display starts at WW=800/WC=-200."
    $app2 = Launch-App "--simulator --seed=demo"
    $win2 = $app2.Window
    Start-Sleep -Seconds 2
    Write-Host "Step 1 complete - app is running."

    $ww0 = Get-ElementText (Find-Element $win2 "WindowWidthValue")
    $wc0 = Get-ElementText (Find-Element $win2 "WindowCenterValue")
    Write-Host "Step 2: Observed initial display values - WW: $ww0   WC: $wc0"
    Write-Host "  (Expected WW 800 / WC -200 - the pre-reset, operator-adjusted state)"

    $shot1 = Join-Path $demo2EvidDir "01-before-reset.png"
    Take-Screenshot $shot1 $win2
    $null = $demo2Steps.Add([ordered]@{ step="Given current WW=800 WC=-200"; observed="WW=$ww0 WC=$wc0"; screenshot=$shot1 })
    Start-Sleep -Seconds 3

    Write-Host "Step 3: Clicking ResetWindowLevelButton ..."
    $resetOk = Click-Button $win2 "ResetWindowLevelButton"
    Start-Sleep -Seconds 2

    $ww1     = Get-ElementText (Find-Element $win2 "WindowWidthValue")
    $wc1     = Get-ElementText (Find-Element $win2 "WindowCenterValue")
    $status1 = Get-ElementText (Find-Element $win2 "StatusBarMessage")
    Write-Host "Step 4: Post-reset - WW: $ww1   WC: $wc1   (expected WW 1500 / WC -600)"
    Write-Host "Step 5: Status bar: '$status1'   (expected: mentions 'Lung' preset)"

    $shot2 = Join-Path $demo2EvidDir "02-after-reset-status-visible.png"
    Take-Screenshot $shot2 $win2
    $null = $demo2Steps.Add([ordered]@{ step="When ResetWindowLevelButton clicked"; observed="ResetOk=$resetOk WW=$ww1 WC=$wc1"; screenshot=$shot2 })
    $null = $demo2Steps.Add([ordered]@{ step="Then WW=1500 WC=-600"; observed="WW=$ww1 WC=$wc1" })
    $null = $demo2Steps.Add([ordered]@{ step="And status bar shows confirmation"; observed="Status: $status1" })

    Write-Host "  Pausing 3 s so the status message is visible ..."
    Start-Sleep -Seconds 3
    Write-Host "  Waiting for auto-dismiss (~2-3 s more) ..."
    Start-Sleep -Seconds 3

    $status1b = Get-ElementText (Find-Element $win2 "StatusBarMessage")
    Write-Host "Step 6: Status after auto-dismiss: '$status1b'  (expected: empty)"
    $shot3 = Join-Path $demo2EvidDir "03-status-dismissed.png"
    Take-Screenshot $shot3 $win2
    $null = $demo2Steps.Add([ordered]@{ step="And status auto-dismisses"; observed="Status after dismiss: '$status1b'"; screenshot=$shot3 })

    Close-App $app2
    Write-Host "Demo 1 COMPLETE - app closed."
    Start-Sleep -Seconds 2
} catch {
    Write-Host "  [ERROR] Demo 1: $_"
    $null = $demo2Steps.Add([ordered]@{ step="ERROR"; observed="$_" })
}

# =============================================================================
#  DEMO 2 — T03  Fallback when DICOM windowing tags absent
# =============================================================================
$demo3Steps   = [System.Collections.ArrayList]@()
$demo3EvidDir = Join-Path $RepoRoot "$EvidenceBase\task-3-20260702T000000"
New-Item -ItemType Directory -Force -Path $demo3EvidDir | Out-Null

Write-Host ""
Write-Host "=== DEMO 2 of 3 - T03 Fallback (--no-dicom-ww) ==="
Write-Host "Scenario: Fallback values applied when DICOM windowing tags are absent"
Write-Host ""

try {
    Write-Host "Step 1: Launching app with --simulator --seed=demo --no-dicom-ww ..."
    Write-Host "  Series has NO DICOM windowing tags. Reset must use fallback WW=400/WC=40."
    $app3 = Launch-App "--simulator --seed=demo --no-dicom-ww"
    $win3 = $app3.Window
    Start-Sleep -Seconds 2
    Write-Host "Step 1 complete - app running in no-DICOM-ww mode."

    $resetBtnEl   = Find-Element $win3 "ResetWindowLevelButton"
    $resetEnabled = if ($null -ne $resetBtnEl) { $resetBtnEl.Current.IsEnabled } else { $false }
    Write-Host "Step 2: ResetWindowLevelButton.IsEnabled = $resetEnabled  (expected: True)"

    $shot4 = Join-Path $demo3EvidDir "01-before-reset-no-dicom.png"
    Take-Screenshot $shot4 $win3
    $null = $demo3Steps.Add([ordered]@{ step="Given CT series loaded without DICOM windowing tags"; observed="ResetButton.IsEnabled=$resetEnabled"; screenshot=$shot4 })
    Start-Sleep -Seconds 3

    Write-Host "Step 3: Clicking ResetWindowLevelButton - fallback applies ..."
    Click-Button $win3 "ResetWindowLevelButton" | Out-Null
    Start-Sleep -Seconds 2

    $ww3     = Get-ElementText (Find-Element $win3 "WindowWidthValue")
    $wc3     = Get-ElementText (Find-Element $win3 "WindowCenterValue")
    $status3 = Get-ElementText (Find-Element $win3 "StatusBarMessage")
    Write-Host "Step 4: Post-fallback-reset - WW: $ww3   WC: $wc3   (expected WW 400 / WC 40)"
    Write-Host "Step 5: Status: '$status3'  (expected: indicates fallback defaults used)"

    $shot5 = Join-Path $demo3EvidDir "02-after-fallback-reset.png"
    Take-Screenshot $shot5 $win3
    $null = $demo3Steps.Add([ordered]@{ step="When ResetWindowLevelButton clicked (no DICOM tags)"; observed="WW=$ww3 WC=$wc3"; screenshot=$shot5 })
    $null = $demo3Steps.Add([ordered]@{ step="Then WW=400 WC=40 (fallback)"; observed="WW=$ww3 WC=$wc3" })
    $null = $demo3Steps.Add([ordered]@{ step="And status indicates fallback used"; observed="Status: $status3" })

    Start-Sleep -Seconds 3
    Close-App $app3
    Write-Host "Demo 2 COMPLETE - app closed."
    Start-Sleep -Seconds 2
} catch {
    Write-Host "  [ERROR] Demo 2: $_"
    $null = $demo3Steps.Add([ordered]@{ step="ERROR"; observed="$_" })
}

# =============================================================================
#  DEMO 3 — T04  Undo / Redo
# =============================================================================
$demo4Steps   = [System.Collections.ArrayList]@()
$demo4EvidDir = Join-Path $RepoRoot "$EvidenceBase\task-4-20260702T000000"
New-Item -ItemType Directory -Force -Path $demo4EvidDir | Out-Null

Write-Host ""
Write-Host "=== DEMO 3 of 3 - T04 Undo/Redo ==="
Write-Host "Scenario: Undo restores the previous Window Width and Window Center"
Write-Host ""

try {
    Write-Host "Step 1: Launching app with --simulator --seed=demo ..."
    $app4 = Launch-App "--simulator --seed=demo"
    $win4 = $app4.Window
    Start-Sleep -Seconds 2

    $undoEl0      = Find-Element $win4 "UndoButton"
    $undoEnabled0 = if ($null -ne $undoEl0) { $undoEl0.Current.IsEnabled } else { "(not found)" }
    Write-Host "Step 2: UndoButton.IsEnabled at startup (before any reset) = $undoEnabled0  (expected: False)"

    $shot6 = Join-Path $demo4EvidDir "01-startup-undo-disabled.png"
    Take-Screenshot $shot6 $win4
    $null = $demo4Steps.Add([ordered]@{ step="Given no WL changes made yet"; observed="UndoButton.IsEnabled=$undoEnabled0"; screenshot=$shot6 })
    Start-Sleep -Seconds 3

    $ww4pre = Get-ElementText (Find-Element $win4 "WindowWidthValue")
    $wc4pre = Get-ElementText (Find-Element $win4 "WindowCenterValue")
    Write-Host "Step 3: Pre-reset - WW: $ww4pre  WC: $wc4pre  (expected WW 800 / WC -200)"

    Write-Host "Step 4: Clicking ResetWindowLevelButton ..."
    Click-Button $win4 "ResetWindowLevelButton" | Out-Null
    Start-Sleep -Seconds 2

    $ww4r         = Get-ElementText (Find-Element $win4 "WindowWidthValue")
    $wc4r         = Get-ElementText (Find-Element $win4 "WindowCenterValue")
    $undoEnabled1 = (Find-Element $win4 "UndoButton").Current.IsEnabled
    Write-Host "Step 4 result: WW: $ww4r  WC: $wc4r  UndoButton.IsEnabled=$undoEnabled1  (expected: True)"

    $shot7 = Join-Path $demo4EvidDir "02-after-reset-undo-enabled.png"
    Take-Screenshot $shot7 $win4
    $null = $demo4Steps.Add([ordered]@{ step="After ResetWindowLevelButton clicked"; observed="WW=$ww4r WC=$wc4r UndoEnabled=$undoEnabled1"; screenshot=$shot7 })
    Start-Sleep -Seconds 3

    Write-Host "Step 5: Clicking UndoButton - expect WW 800 / WC -200 restored ..."
    Click-Button $win4 "UndoButton" | Out-Null
    Start-Sleep -Seconds 2

    $ww4u = Get-ElementText (Find-Element $win4 "WindowWidthValue")
    $wc4u = Get-ElementText (Find-Element $win4 "WindowCenterValue")
    Write-Host "Step 5 result: WW: $ww4u  WC: $wc4u  (expected WW 800 / WC -200)"

    $shot8 = Join-Path $demo4EvidDir "03-after-undo.png"
    Take-Screenshot $shot8 $win4
    $null = $demo4Steps.Add([ordered]@{ step="When UndoButton clicked"; observed="WW=$ww4u WC=$wc4u"; screenshot=$shot8 })
    $null = $demo4Steps.Add([ordered]@{ step="Then WW=800 WC=-200 restored"; observed="WW=$ww4u WC=$wc4u" })
    Start-Sleep -Seconds 3

    Write-Host "Step 6: Clicking RedoButton - expect WW 1500 / WC -600 reapplied ..."
    Click-Button $win4 "RedoButton" | Out-Null
    Start-Sleep -Seconds 2

    $ww4rd = Get-ElementText (Find-Element $win4 "WindowWidthValue")
    $wc4rd = Get-ElementText (Find-Element $win4 "WindowCenterValue")
    Write-Host "Step 6 result: WW: $ww4rd  WC: $wc4rd  (expected WW 1500 / WC -600)"

    $shot9 = Join-Path $demo4EvidDir "04-after-redo.png"
    Take-Screenshot $shot9 $win4
    $null = $demo4Steps.Add([ordered]@{ step="When RedoButton clicked"; observed="WW=$ww4rd WC=$wc4rd"; screenshot=$shot9 })
    $null = $demo4Steps.Add([ordered]@{ step="Then WW=1500 WC=-600 reapplied"; observed="WW=$ww4rd WC=$wc4rd" })

    Start-Sleep -Seconds 3
    Close-App $app4
    Write-Host "Demo 3 COMPLETE - app closed."
} catch {
    Write-Host "  [ERROR] Demo 3: $_"
    $null = $demo4Steps.Add([ordered]@{ step="ERROR"; observed="$_" })
}

# =============================================================================
#  Write demo log JSON files
# =============================================================================
$ts      = (Get-Date -Format "yyyy-MM-ddTHH:mm:ss")
$jsonBase = Join-Path $RepoRoot ".harness\demo_evidence"

$log2 = [ordered]@{
    task            = "2"
    demo_scenario   = "Scenario: Reset restores DICOM default Window Width and Window Center"
    demonstrated_at = $ts
    video           = "(not captured - ffmpeg not installed)"
    screenshots     = @(
        "$EvidenceBase\task-2-20260702T000000\01-before-reset.png",
        "$EvidenceBase\task-2-20260702T000000\02-after-reset-status-visible.png",
        "$EvidenceBase\task-2-20260702T000000\03-status-dismissed.png"
    )
    steps_shown     = @($demo2Steps)
    notes           = "ffmpeg not installed; screenshots captured instead. Demo driven via Windows UI Automation (UIAutomationClient)."
}
$log3 = [ordered]@{
    task            = "3"
    demo_scenario   = "Scenario: Fallback values applied when DICOM windowing tags are absent"
    demonstrated_at = $ts
    video           = "(not captured - ffmpeg not installed)"
    screenshots     = @(
        "$EvidenceBase\task-3-20260702T000000\01-before-reset-no-dicom.png",
        "$EvidenceBase\task-3-20260702T000000\02-after-fallback-reset.png"
    )
    steps_shown     = @($demo3Steps)
    notes           = "ffmpeg not installed; screenshots captured instead."
}
$log4 = [ordered]@{
    task            = "4"
    demo_scenario   = "Scenario: Undo restores the previous Window Width and Window Center"
    demonstrated_at = $ts
    video           = "(not captured - ffmpeg not installed)"
    screenshots     = @(
        "$EvidenceBase\task-4-20260702T000000\01-startup-undo-disabled.png",
        "$EvidenceBase\task-4-20260702T000000\02-after-reset-undo-enabled.png",
        "$EvidenceBase\task-4-20260702T000000\03-after-undo.png",
        "$EvidenceBase\task-4-20260702T000000\04-after-redo.png"
    )
    steps_shown     = @($demo4Steps)
    notes           = "ffmpeg not installed; screenshots captured instead."
}

$log2 | ConvertTo-Json -Depth 10 | Set-Content (Join-Path $jsonBase "ww-wc-reset_2_demo.json") -Encoding UTF8
$log3 | ConvertTo-Json -Depth 10 | Set-Content (Join-Path $jsonBase "ww-wc-reset_3_demo.json") -Encoding UTF8
$log4 | ConvertTo-Json -Depth 10 | Set-Content (Join-Path $jsonBase "ww-wc-reset_4_demo.json") -Encoding UTF8

Write-Host ""
Write-Host "Demo logs written:"
Write-Host "  .harness/demo_evidence/ww-wc-reset_2_demo.json"
Write-Host "  .harness/demo_evidence/ww-wc-reset_3_demo.json"
Write-Host "  .harness/demo_evidence/ww-wc-reset_4_demo.json"
Write-Host "All demos complete."
