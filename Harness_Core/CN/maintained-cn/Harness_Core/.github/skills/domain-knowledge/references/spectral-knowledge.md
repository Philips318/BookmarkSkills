# Spectral CT Knowledge

## Overview

Spectral CT 使用 **dual-layer detector** 在 **single acquisition** 中同时采集 higher-energy 和 lower-energy X-ray spectra。这支持 material decomposition 和超越 conventional CT 的 energy-selective imaging。

## Key Concepts

| 术语 | 描述 |
|------|-------------|
| **SBI** | Spectral Base Images — proprietary packed DICOM format，包含 base decomposition data |
| **MonoE** | selected keV（40-200 keV）下的 Mono-Energetic images |
| **VNC** | Virtual Non-Contrast — 通过计算移除 iodine |
| **Iodine Map** | Iodine concentration visualization（mg/ml） |
| **Iodine Density** | Quantitative iodine in mg/ml |
| **Z Effective** | Effective atomic number（range: 5-30） |
| **Electron Density** | %EDW — for radiation therapy planning |
| **keV** | MonoE images 的 photon energy unit |
| **MBS** | Maximum Basis Spectra — core decomposition method |
| **KeWi** | keV Weighting — optimized spectral blending |
| **MagicGlass** | 在 conventional image 上显示 spectral results 的 interactive overlay tool |
| **Fusion** | 两种 result types 的 side-by-side 或 overlay display |

## Spectral Result Types

| Result Type | 单位 | Diagnostic Use | Key Constraints |
|-------------|------|---------------|-----------------|
| Conventional | HU | Standard CT viewing | — |
| MonoE | HU | Reduce beam hardening（high keV）；enhance iodine（low keV） | HU varies with keV；< 60 keV → ±8 HU water variation |
| MonoE ≈Conventional | HU | Same HU as 120 kVp with improved IQ | — |
| Iodine no Water | mg/ml* | Iodine concentration | Non-enhanced tissue ≈ 0 mg/ml |
| Iodine Density | mg/ml | Quantitative iodine | Accuracy drops below 5 mg/ml |
| VNC | HU* | Replace true non-contrast | Body only；optimal at < 10 mg/ml（100 kVp）or < 20 mg/ml（120 kVp） |
| Contrast-Enhanced Structures | HU | Bone-free vascular view | — |
| Iodine Removed | HU | Non-enhanced structures only | — |
| Z Effective | 5-30 | Tissue differentiation | Non-HU unit → save as RGB |
| Electron Density | %EDW | RT planning | Non-HU unit → save as RGB |
| Uric Acid | HU | Gout diagnosis, stone characterization | Stones < 3 mm may be missed at typical dose |
| Uric Acid Removed | HU | Complement to Uric Acid | — |
| Calcium Suppressed | HU | Bone suppression | — |

**Cardiac Spectral Results**（subset）: Cardiac MonoE、Cardiac Iodine Density、Cardiac VNC

## SBI Format Versioning

| Version | Capability |
|---------|-----------|
| 2.x | Original base format |
| 3.x | Extended result types |
| 4.x | Cardiac spectral support |
| 5.0 | Current；all result types available |

**Rule:** Result availability 由 **SBI version** gate。Requirements 必须为 new result types 指定 minimum SBI version。

## Architecture (4 Layers)

```
┌─────────────────────┐
│   Presentation      │  SpectralCTViewer, MagicGlass, MonoE Slider
├─────────────────────┤
│   Business Logic    │  Result generation, parameter management
├─────────────────────┤
│   Data Access       │  SBI reader/writer, DICOM I/O
├─────────────────────┤
│   Algorithm         │  SpectralSDK (MEF-based, no memory allocation)
└─────────────────────┘
```

## SpectralSDK Constraints

- **MEF-based** plugin architecture — components 通过 Managed Extensibility Framework 加载
- **No memory allocation** inside SDK — caller 提供 pre-allocated buffers
- **Thread safety** — SDK is NOT thread-safe；caller must serialize access
- **Max 4 concurrent MonoE** calculations allowed
- **Result generation order** matters — some results depend on intermediate decomposition

## Key Algorithms

| Algorithm | Input | 输出 |
|-----------|-------|--------|
| BaseDecompose | Raw spectral data | Base material images |
| MonoEnergeticImage | Base images + keV | MonoE at specified keV |
| MaterialDecompose | Base images + material pair | Material-specific map |
| VirtualNonContrast | Base images | Iodine-removed image |
| IodineQuantification | Base images | Iodine density map（mg/ml） |
| EffectiveZCalc | Base images | Atomic number map |
| ElectronDensityCalc | Base images | Electron density map（%EDW） |
| UricAcidDecompose | Base images | Uric acid / non-uric acid pair |

## Key Interfaces

| 接口 | 用途 |
|-----------|---------|
| ISpectralResultGenerator | Generate spectral result from SBI |
| ISpectralViewer | Display engine for spectral results |
| ISbiReader | Read and parse SBI DICOM files |
| ISpectralParameterProvider | Manage keV, material pair, display presets |
| ISpectralFusionEngine | Overlay spectral onto conventional |
| IBaseMaterialDecomposer | Core decomposition |
| IMagicGlassRenderer | Interactive spectral overlay region |
| IMonoEnergeticCalculator | MonoE image generation |
| ISpectralPlotProvider | Spectral attenuation curve data |

## Clinical Workflows

| Workflow | Key Steps |
|----------|-----------|
| General Spectral | Scan → SBI generated → View Conventional → Switch to MonoE/VNC/Iodine |
| Oncology | Scan → Iodine map for perfusion → VNC for baseline → Z Effective for tissue typing |
| Vascular | Scan with contrast → MonoE low keV（enhance iodine）→ Contrast-Enhanced Structures（remove bone） |
| Neuro | Scan → MonoE high keV（reduce bone artifact）→ VNC（detect hemorrhage vs contrast staining） |
| Gout | Scan extremity → Uric Acid result → Uric Acid Removed |
| RT Planning | Scan → Electron Density map → Export to TPS |

## Non-HU Image Warning

**non-HU units**（Iodine no Water、Z Effective、Iodine Density、Electron Density）中的 spectral results：
- Default: 保存为 **RGB format**，防止 third-party viewers 显示错误 "HU" window
- Configurable: HU-like format with **burned-in unit warning text**
- **Requirement implication:** export/archive features 必须处理两种 formats

## Extended FOV (EFOV)

- EFOV 支持最大 **800 mm** diameter（non-gated helical only）
- EFOV 内 spectral results 限制为 **500 mm**
- 超过 500 mm 后 HU 和 geometry **not guaranteed**
- **Requirement implication:** spectral features 必须 validate FOV ≤ 500 mm

## Embedded Configuration Files

| File | 用途 |
|------|---------|
| SpectralResultConfig.xml | Result type availability, version gates |
| MonoEPresets.xml | Default keV values per clinical category |
| MaterialPairConfig.xml | Available material pairs for decomposition |
| SpectralViewerLayout.xml | Layout presets（General, Oncology, Vascular, Neuro, Fusion） |
| SbiVersionMap.xml | SBI version → supported result type mapping |
| SpectralCalibration.xml | Detector calibration parameters |
