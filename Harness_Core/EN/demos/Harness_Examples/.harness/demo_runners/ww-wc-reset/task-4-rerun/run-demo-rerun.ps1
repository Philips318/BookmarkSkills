# WW/WC Reset Feature Demo Re-Run — 2026-07-02 (post undo-fix)
# Focuses on Demo 3 (T04 undo/redo) but runs all three scenarios.
# Uses Windows UI Automation (UIAutomationClient).
# Screenshots saved under .harness/demo_evidence/ww-wc-reset/

param(
    [string]$ExePath      = "Src\ImageDisplayApp\bin\Debug\net8.0-windows\Philips.CT.Host.ImageDisplay.App.exe",
    [string]$RepoRoot     = "C:\Work\Code\Git_Code\Harness_Enhance",
    [string]$EvidenceBase = ".harness\demo_evidence\ww-wc-reset"
)

Set-Location $RepoRoot

Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

$Timestamp = Get-Date -Format "yyyyMMddTHHmmss"

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------
function Take-Screenshot([string]$OutPath, $Window) {
    $rect = $Window.Current.BoundingRectangle
    if ($rect.Width -le 0 -or $rect.Height -le 0) {
        Write-Host "  [WARN] Window rect invalid — skipping screenshot."
        return $null
    }
    $bmp = New-Object System.Drawing.Bitmap([int]$rect.Width, [int]$rect.Height)
    $g   = [System.Drawing.Graphics]::FromImage($bmp)
    $g.CopyFromScreen([int]$rect.X, [int]$rect.Y, 0, 0,
        [System.Drawing.Size]::new([int]$rect.Width, [int]$rect.Height),
        [System.Drawing.CopyPixelOperation]::SourceCopy)
    $g.Dispose()
    $bmp.Save($OutPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    $relPath = $OutPath.Replace($RepoRoot + "\", "")
    Write-Host "  Screenshot: $relPath  ($([int](Get-Item $OutPath).Length / 1KB) KB)"
    return $relPath
}

function Find-Element($Root, [string]$AutomationId, [int]$TimeoutMs = 8000) {
    $cond     = New-Object System.Windows.Automation.PropertyCondition(
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
    if ($null -eq $El) { return "(not found)" }
    try {
        $vp = $El.GetCurrentPattern([System.Windows.Automation.TextPattern]::Pattern)
        return ($vp.DocumentRange.GetText(-1)).Trim()
    } catch {
        return $El.Current.Name.Trim()
    }
}

function Get-IsEnabled($Root, [string]$AutomationId, [int]$TimeoutMs = 5000) {
    $el = Find-Element -Root $Root -AutomationId $AutomationId -TimeoutMs $TimeoutMs
    if ($null -eq $el) { return "(element not found)" }
    return $el.Current.IsEnabled
}

function Click-Button($Root, [string]$AutomationId, [int]$WaitAfterMs = 2000) {
    $el = Find-Element -Root $Root -AutomationId $AutomationId
    if ($null -eq $el) {
        Write-Host "  [WARN] Button '$AutomationId' not found."
        return $false
    }
    if (-not $el.Current.IsEnabled) {
        Write-Host "  [WARN] Button '$AutomationId' is DISABLED — cannot invoke."
        return $false
    }
    try {
        $ip = $el.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)
        $ip.Invoke()
        Start-Sleep -Milliseconds $WaitAfterMs
        return $true
    } catch {
        Write-Host "  [WARN] InvokePattern failed on '$AutomationId': $_"
        return $false
    }
}

function Launch-App([string]$LaunchArgs) {
    $fullExe = Join-Path $RepoRoot $ExePath
    $argList = $LaunchArgs.Split(' ', [System.StringSplitOptions]::RemoveEmptyEntries)
    $proc    = Start-Process -FilePath $fullExe -ArgumentList $argList -PassThru
    Start-Sleep -Seconds 3
    if ($proc.HasExited) { throw "App crashed on launch (exit $($proc.ExitCode))." }

    $desktop = [System.Windows.Automation.AutomationElement]::RootElement
    $pidCond = New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::ProcessIdProperty, $proc.Id)
    $deadline = (Get-Date).AddSeconds(12)
    $window   = $null
    while ((Get-Date) -lt $deadline) {
        $window = $desktop.FindFirst([System.Windows.Automation.TreeScope]::Children, $pidCond)
        if ($null -ne $window) { break }
        Start-Sleep -Milliseconds 300
    }
    if ($null -eq $window) { throw "Could not locate main window for PID $($proc.Id)." }

    try {
        $wp = $window.GetCurrentPattern([System.Windows.Automation.WindowPattern]::Pattern)
        $wp.SetWindowVisualState([System.Windows.Automation.WindowVisualState]::Maximized)
    } catch { }
    Start-Sleep -Milliseconds 800
    return @{ Process = $proc; Window = $window }
}

function Close-App($AppInfo) {
    try { $AppInfo.Process.CloseMainWindow() | Out-Null } catch {}
    Start-Sleep -Milliseconds 800
    if (-not $AppInfo.Process.HasExited) { $AppInfo.Process.Kill() }
    Start-Sleep -Milliseconds 400
}

# ---------------------------------------------------------------------------
# ========================  DEMO 1 — T02 Happy Path  ========================
# ---------------------------------------------------------------------------
$demo2Steps   = [System.Collections.ArrayList]@()
$demo2EvidDir = Join-Path $RepoRoot "$EvidenceBase\task-2-$Timestamp"
New-Item -ItemType Directory -Force -Path $demo2EvidDir | Out-Null

Write-Host ""
Write-Host "================================================================="
Write-Host "  DEMO 1 of 3 — T02 Happy Path"
Write-Host "  Scenario: Reset restores DICOM default Window Width and Center"
Write-Host "================================================================="

try {
    Write-Host "`nStep 1: Launching app --simulator --seed=demo ..."
    Write-Host "  (Series starts at WW=800/WC=-200 with DICOM defaults WW=1500/WC=-600 stored)"
    $app2 = Launch-App "--simulator --seed=demo"
    $win2 = $app2.Window
    Start-Sleep -Seconds 2
    Write-Host "  App is running."

    $ww0 = Get-ElementText (Find-Element $win2 "WindowWidthValue")
    $wc0 = Get-ElementText (Find-Element $win2 "WindowCenterValue")
    Write-Host "Step 2: Initial display — WW: $ww0   WC: $wc0  (expected WW≈800 / WC≈-200)"

    $rel1 = Take-Screenshot (Join-Path $demo2EvidDir "01-before-reset.png") $win2
    $null = $demo2Steps.Add([ordered]@{
        step       = "Given current WW=800 WC=-200"
        observed   = "WW=$ww0 WC=$wc0"
        screenshot = $rel1
    })
    Start-Sleep -Seconds 3

    Write-Host "`nStep 3: Clicking ResetWindowLevelButton ..."
    $resetOk = Click-Button $win2 "ResetWindowLevelButton" -WaitAfterMs 2000
    Write-Host "  InvokePattern result: $resetOk"

    $ww1     = Get-ElementText (Find-Element $win2 "WindowWidthValue")
    $wc1     = Get-ElementText (Find-Element $win2 "WindowCenterValue")
    $status1 = Get-ElementText (Find-Element $win2 "StatusBarMessage")
    Write-Host "Step 4: Post-reset — WW: $ww1   WC: $wc1   (expected WW≈1500 / WC≈-600)"
    Write-Host "  Status bar: '$status1'  (expected: confirmation message)"

    $rel2 = Take-Screenshot (Join-Path $demo2EvidDir "02-after-reset-status-visible.png") $win2
    $null = $demo2Steps.Add([ordered]@{
        step       = "When ResetWindowLevelButton clicked"
        observed   = "ResetOk=$resetOk WW=$ww1 WC=$wc1 Status='$status1'"
        screenshot = $rel2
    })
    Start-Sleep -Seconds 3

    Write-Host "  Waiting for status auto-dismiss (~3 s) ..."
    Start-Sleep -Seconds 3

    $status1b = Get-ElementText (Find-Element $win2 "StatusBarMessage")
    Write-Host "Step 5: Status after auto-dismiss: '$status1b'  (expected: empty)"
    $rel3 = Take-Screenshot (Join-Path $demo2EvidDir "03-status-dismissed.png") $win2
    $null = $demo2Steps.Add([ordered]@{
        step       = "Then status auto-dismisses"
        observed   = "Status after dismiss: '$status1b'"
        screenshot = $rel3
    })

    Close-App $app2
    Write-Host "Demo 1 COMPLETE — app closed."
    Start-Sleep -Seconds 2
} catch {
    Write-Host "  [ERROR] Demo 1: $_"
    $null = $demo2Steps.Add([ordered]@{ step = "ERROR"; observed = "$_" })
}

# ---------------------------------------------------------------------------
# =================  DEMO 2 — T03 Fallback (no DICOM tags)  ================
# ---------------------------------------------------------------------------
$demo3Steps   = [System.Collections.ArrayList]@()
$demo3EvidDir = Join-Path $RepoRoot "$EvidenceBase\task-3-$Timestamp"
New-Item -ItemType Directory -Force -Path $demo3EvidDir | Out-Null

Write-Host ""
Write-Host "================================================================="
Write-Host "  DEMO 2 of 3 — T03 Fallback (--no-dicom-ww)"
Write-Host "  Scenario: Fallback values applied when DICOM windowing tags absent"
Write-Host "================================================================="

try {
    Write-Host "`nStep 1: Launching app --simulator --seed=demo --no-dicom-ww ..."
    Write-Host "  (Series has NO DICOM windowing tags — fallback WW=400/WC=40 expected)"
    $app3 = Launch-App "--simulator --seed=demo --no-dicom-ww"
    $win3 = $app3.Window
    Start-Sleep -Seconds 2

    $resetBtnEl   = Find-Element $win3 "ResetWindowLevelButton"
    $resetEnabled = if ($null -ne $resetBtnEl) { $resetBtnEl.Current.IsEnabled } else { "(not found)" }
    Write-Host "Step 2: ResetWindowLevelButton.IsEnabled = $resetEnabled  (expected: True — series IS loaded)"

    $rel4 = Take-Screenshot (Join-Path $demo3EvidDir "01-before-reset-no-dicom.png") $win3
    $null = $demo3Steps.Add([ordered]@{
        step       = "Given CT series loaded without DICOM windowing tags"
        observed   = "ResetButton.IsEnabled=$resetEnabled"
        screenshot = $rel4
    })
    Start-Sleep -Seconds 3

    Write-Host "`nStep 3: Clicking ResetWindowLevelButton (fallback applies) ..."
    $resetOk3 = Click-Button $win3 "ResetWindowLevelButton" -WaitAfterMs 2000
    Write-Host "  InvokePattern result: $resetOk3"

    $ww3     = Get-ElementText (Find-Element $win3 "WindowWidthValue")
    $wc3     = Get-ElementText (Find-Element $win3 "WindowCenterValue")
    $status3 = Get-ElementText (Find-Element $win3 "StatusBarMessage")
    Write-Host "Step 4: Post-fallback-reset — WW: $ww3   WC: $wc3   (expected WW=400 / WC=40)"
    Write-Host "  Status: '$status3'  (expected: fallback-defaults message)"

    $rel5 = Take-Screenshot (Join-Path $demo3EvidDir "02-after-fallback-reset.png") $win3
    $null = $demo3Steps.Add([ordered]@{
        step       = "When ResetWindowLevelButton clicked (no DICOM tags)"
        observed   = "ResetOk=$resetOk3 WW=$ww3 WC=$wc3 Status='$status3'"
        screenshot = $rel5
    })

    Start-Sleep -Seconds 3
    Close-App $app3
    Write-Host "Demo 2 COMPLETE — app closed."
    Start-Sleep -Seconds 2
} catch {
    Write-Host "  [ERROR] Demo 2: $_"
    $null = $demo3Steps.Add([ordered]@{ step = "ERROR"; observed = "$_" })
}

# ---------------------------------------------------------------------------
# ===================  DEMO 3 — T04 Undo/Redo (KEY FIX)  ===================
# ---------------------------------------------------------------------------
$demo4Steps   = [System.Collections.ArrayList]@()
$demo4EvidDir = Join-Path $RepoRoot "$EvidenceBase\task-4-$Timestamp"
New-Item -ItemType Directory -Force -Path $demo4EvidDir | Out-Null

Write-Host ""
Write-Host "================================================================="
Write-Host "  DEMO 3 of 3 — T04 Undo/Redo  *** POST-FIX VERIFICATION ***"
Write-Host "  Fix: DelegateCommand.RaiseCanExecuteChanged() now raised in"
Write-Host "  WindowWidth/WindowCenter setters via RefreshUndoRedoState()."
Write-Host "================================================================="

$undoEnabledBefore = "(not measured)"
$undoEnabledAfter  = "(not measured)"
$undoInvokeOk      = $false
$redoInvokeOk      = $false

try {
    Write-Host "`nStep 1: Launching app --simulator --seed=demo ..."
    $app4 = Launch-App "--simulator --seed=demo"
    $win4 = $app4.Window
    Start-Sleep -Seconds 2

    # ---- BEFORE RESET: read initial values + UndoButton state ----
    $ww4pre = Get-ElementText (Find-Element $win4 "WindowWidthValue")
    $wc4pre = Get-ElementText (Find-Element $win4 "WindowCenterValue")
    $undoEnabledBefore = Get-IsEnabled $win4 "UndoButton"
    Write-Host "Step 2: At startup (before any reset):"
    Write-Host "  WW: $ww4pre   WC: $wc4pre"
    Write-Host "  UndoButton.IsEnabled = $undoEnabledBefore  <<< expected: FALSE"
    Write-Host "  (Nothing has been pushed to the undo stack yet.)"

    $rel6 = Take-Screenshot (Join-Path $demo4EvidDir "01-startup-undo-disabled.png") $win4
    $null = $demo4Steps.Add([ordered]@{
        step       = "Given no WL changes made yet"
        observed   = "WW=$ww4pre WC=$wc4pre UndoButton.IsEnabled=$undoEnabledBefore"
        screenshot = $rel6
    })
    Start-Sleep -Seconds 3

    # ---- CLICK RESET ----
    Write-Host "`nStep 3: Clicking ResetWindowLevelButton ..."
    Write-Host "  (This pushes a WindowLevelChangeAction to the undo stack, then applies"
    Write-Host "   new WW/WC via the WindowWidth/WindowCenter setters, which now call"
    Write-Host "   RefreshUndoRedoState() → RaiseCanExecuteChanged() on UndoCommand.)"
    $resetOk4 = Click-Button $win4 "ResetWindowLevelButton" -WaitAfterMs 2500
    Write-Host "  ResetWindowLevelButton InvokePattern: $resetOk4"

    # Allow WPF dispatcher to process the CanExecuteChanged notification
    Start-Sleep -Milliseconds 500

    $ww4r          = Get-ElementText (Find-Element $win4 "WindowWidthValue")
    $wc4r          = Get-ElementText (Find-Element $win4 "WindowCenterValue")
    $undoEnabledAfter = Get-IsEnabled $win4 "UndoButton"
    $redoEnabledAfterReset = Get-IsEnabled $win4 "RedoButton"
    Write-Host "Step 4: After reset:"
    Write-Host "  WW: $ww4r   WC: $wc4r   (expected WW=1500 / WC=-600)"
    Write-Host "  UndoButton.IsEnabled = $undoEnabledAfter  <<< expected: TRUE  *** KEY FIX ***"
    Write-Host "  RedoButton.IsEnabled = $redoEnabledAfterReset  (expected: False — nothing to redo)"

    $rel7 = Take-Screenshot (Join-Path $demo4EvidDir "02-after-reset-undo-enabled.png") $win4
    $null = $demo4Steps.Add([ordered]@{
        step       = "After ResetWindowLevelButton clicked — UndoButton state (THE FIX)"
        observed   = "WW=$ww4r WC=$wc4r UndoButton.IsEnabled=$undoEnabledAfter RedoButton.IsEnabled=$redoEnabledAfterReset"
        screenshot = $rel7
    })
    Start-Sleep -Seconds 3

    # ---- CLICK UNDO (only if enabled — proves InvokePattern now succeeds) ----
    Write-Host "`nStep 5: Clicking UndoButton (InvokePattern) — expected WW=800 / WC=-200 restored ..."
    $undoInvokeOk = Click-Button $win4 "UndoButton" -WaitAfterMs 2000
    Write-Host "  UndoButton InvokePattern: $undoInvokeOk  (expected: True)"

    $ww4u = Get-ElementText (Find-Element $win4 "WindowWidthValue")
    $wc4u = Get-ElementText (Find-Element $win4 "WindowCenterValue")
    $redoEnabledAfterUndo = Get-IsEnabled $win4 "RedoButton"
    Write-Host "Step 5 result: WW: $ww4u   WC: $wc4u   (expected WW≈800 / WC≈-200)"
    Write-Host "  RedoButton.IsEnabled = $redoEnabledAfterUndo  (expected: True)"

    $rel8 = Take-Screenshot (Join-Path $demo4EvidDir "03-after-undo.png") $win4
    $null = $demo4Steps.Add([ordered]@{
        step       = "When UndoButton clicked (InvokePattern)"
        observed   = "InvokeOk=$undoInvokeOk WW=$ww4u WC=$wc4u RedoButton.IsEnabled=$redoEnabledAfterUndo"
        screenshot = $rel8
    })
    Start-Sleep -Seconds 3

    # ---- CLICK REDO ----
    Write-Host "`nStep 6: Clicking RedoButton — expected WW=1500 / WC=-600 reapplied ..."
    $redoInvokeOk = Click-Button $win4 "RedoButton" -WaitAfterMs 2000
    Write-Host "  RedoButton InvokePattern: $redoInvokeOk  (expected: True)"

    $ww4rd = Get-ElementText (Find-Element $win4 "WindowWidthValue")
    $wc4rd = Get-ElementText (Find-Element $win4 "WindowCenterValue")
    $undoEnabledAfterRedo = Get-IsEnabled $win4 "RedoButton"
    Write-Host "Step 6 result: WW: $ww4rd   WC: $wc4rd   (expected WW≈1500 / WC≈-600)"

    $rel9 = Take-Screenshot (Join-Path $demo4EvidDir "04-after-redo.png") $win4
    $null = $demo4Steps.Add([ordered]@{
        step       = "When RedoButton clicked (InvokePattern)"
        observed   = "InvokeOk=$redoInvokeOk WW=$ww4rd WC=$wc4rd"
        screenshot = $rel9
    })
    Start-Sleep -Seconds 3

    Close-App $app4
    Write-Host "Demo 3 COMPLETE — app closed."
} catch {
    Write-Host "  [ERROR] Demo 3: $_"
    $null = $demo4Steps.Add([ordered]@{ step = "ERROR"; observed = "$_" })
}

# ---------------------------------------------------------------------------
# Write summary to stdout so the calling agent can capture it
# ---------------------------------------------------------------------------
Write-Host ""
Write-Host "================================================================="
Write-Host "  DEMO RUN SUMMARY"
Write-Host "================================================================="
Write-Host "TIMESTAMP: $Timestamp"
Write-Host ""
Write-Host "DEMO 3 — T04 Undo/Redo (KEY FIX VERIFICATION):"
Write-Host "  UndoButton.IsEnabled BEFORE reset : $undoEnabledBefore"
Write-Host "  UndoButton.IsEnabled AFTER  reset : $undoEnabledAfter"
Write-Host "  Undo InvokePattern succeeded      : $undoInvokeOk"
Write-Host "  Redo InvokePattern succeeded      : $redoInvokeOk"
Write-Host ""
Write-Host "Evidence folder: .harness\demo_evidence\ww-wc-reset\"
Write-Host "  task-2-$Timestamp\"
Write-Host "  task-3-$Timestamp\"
Write-Host "  task-4-$Timestamp\"

# Return structured result for agent to parse
[PSCustomObject]@{
    Timestamp            = $Timestamp
    Demo3_UndoBefore     = $undoEnabledBefore
    Demo3_UndoAfter      = $undoEnabledAfter
    Demo3_UndoInvokeOk   = $undoInvokeOk
    Demo3_RedoInvokeOk   = $redoInvokeOk
    Demo2Steps           = $demo2Steps
    Demo3Steps           = $demo3Steps
    Demo4Steps           = $demo4Steps
}
