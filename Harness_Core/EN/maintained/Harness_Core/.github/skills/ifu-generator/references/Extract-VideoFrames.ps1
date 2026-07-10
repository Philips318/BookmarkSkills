<#
.SYNOPSIS
  Extract key frames from a workflow screen-recording for IFU generation.
  Frames become candidate IFU screenshots (Mode A).

.DESCRIPTION
  Uses ffmpeg to extract frames from a video. Two modes:
    - Scene mode (default): extract frames at scene changes (UI transitions).
    - Interval mode: extract one frame every N seconds.
  Output PNG files are written to "{video_basename}_frames/".

  Requires ffmpeg on PATH (https://ffmpeg.org/). Check with: ffmpeg -version

.PARAMETER VideoPath
  Path to the source video (.mp4, .mov, .avi, .webm, .gif).

.PARAMETER Mode
  'Scene' (default) or 'Interval'.

.PARAMETER SceneThreshold
  Scene-change sensitivity for Scene mode (0.0-1.0). Lower = more frames. Default 0.30.

.PARAMETER IntervalSeconds
  Seconds between frames for Interval mode. Default 2.

.EXAMPLE
  .\Extract-VideoFrames.ps1 -VideoPath .\precise_surview_workflow.mp4

.EXAMPLE
  .\Extract-VideoFrames.ps1 -VideoPath .\workflow.mp4 -Mode Interval -IntervalSeconds 3
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$VideoPath,

    [ValidateSet('Scene', 'Interval')]
    [string]$Mode = 'Scene',

    [double]$SceneThreshold = 0.30,

    [int]$IntervalSeconds = 2
)

$ErrorActionPreference = 'Stop'

# --- Verify ffmpeg availability ---
$ffmpeg = Get-Command ffmpeg -ErrorAction SilentlyContinue
if (-not $ffmpeg) {
    Write-Error "ffmpeg not found on PATH. Install from https://ffmpeg.org/ and retry. Verify with: ffmpeg -version"
    return
}

# --- Resolve paths ---
$video = Get-Item -LiteralPath $VideoPath
$baseName = $video.BaseName
$outDir = Join-Path $video.DirectoryName "${baseName}_frames"
if (-not (Test-Path $outDir)) {
    New-Item -ItemType Directory -Path $outDir | Out-Null
}

$outPattern = Join-Path $outDir "frame_%03d.png"
Write-Host "Extracting frames from: $($video.FullName)"
Write-Host "Output folder: $outDir"
Write-Host "Mode: $Mode"

# --- Build ffmpeg filter ---
if ($Mode -eq 'Scene') {
    # Select frames where scene-change score exceeds the threshold.
    $filter = "select='gt(scene,$SceneThreshold)',showinfo"
    $args = @('-i', $video.FullName, '-vf', $filter, '-vsync', 'vfr', '-frame_pts', 'true', $outPattern)
}
else {
    # One frame every IntervalSeconds.
    $fps = "1/$IntervalSeconds"
    $filter = "fps=$fps"
    $args = @('-i', $video.FullName, '-vf', $filter, $outPattern)
}

# --- Run ffmpeg ---
& ffmpeg @args 2>&1 | Out-Null

$frames = Get-ChildItem -Path $outDir -Filter 'frame_*.png' | Sort-Object Name
if ($frames.Count -eq 0) {
    Write-Warning "No frames extracted. Try a lower -SceneThreshold (e.g. 0.20) or -Mode Interval."
}
else {
    Write-Host "Extracted $($frames.Count) frame(s):"
    $frames | ForEach-Object { Write-Host "  $($_.Name)" }
    Write-Host ""
    Write-Host "Next: review and de-identify frames, then pass the folder to the IFU Generator as Mode A screenshots."
}
