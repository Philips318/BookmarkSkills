# WW/WC Reset — Component Diagram

## Level 2: Container / Component View

```mermaid
graph TD
    subgraph "View Layer (WPF / XAML)"
        V_IDV["ImageDisplayView.xaml<br/><i>WPF UserControl</i><br/>Reset button + status bar"]
    end

    subgraph "ViewModel Layer"
        VM_IDV["ImageDisplayViewModel<br/><i>: IImageDisplayViewModel</i><br/><i>: INotifyPropertyChanged</i><br/>WindowWidth, WindowCenter,<br/>ResetWindowLevelCommand,<br/>StatusMessage, IsSeriesLoaded"]
        CMD_RWL["ResetWindowLevelCommand<br/><i>: IRelayCommand</i><br/>Execute / CanExecute logic"]
    end

    subgraph "Model Layer"
        M_DIM["DicomImageModel<br/><i>: IDicomImageModel</i><br/>DefaultWindowLevel,<br/>HasValidDefaultWindowLevel"]
        VO_WL["WindowLevel<br/><i>record (value object)</i><br/>Width &gt; 0, Center, PresetName"]
        UA_WLC["WindowLevelChangeAction<br/><i>: IUndoableAction</i><br/>Pre-reset WW/WC snapshot"]
    end

    subgraph "Service Layer"
        S_SBN["StatusBarNotifier<br/><i>: IStatusNotifier</i><br/>ShowTransientMessage + auto-dismiss"]
        S_WLUS["WindowLevelUndoService<br/><i>: IUndoService</i><br/>Bounded undo/redo stack"]
        S_WLD["WindowLevelDefaults<br/><i>: IWindowLevelDefaults</i><br/>Reads fallback WW/WC<br/>from app config"]
    end

    subgraph "Constants"
        C_DDT["DicomDisplayTags<br/><i>static class</i><br/>(0028,1050) WindowCenter<br/>(0028,1051) WindowWidth<br/>(0028,1055) Explanation"]
    end

    subgraph "ExtInf/ — Interface Contracts"
        I_IIDVM["IImageDisplayViewModel"]
        I_IDIM["IDicomImageModel"]
        I_ISN["IStatusNotifier"]
        I_IUS["IUndoService"]
        I_IUA["IUndoableAction"]
        I_IWLD["IWindowLevelDefaults"]
    end

    %% View → ViewModel bindings
    V_IDV -- "Command={Binding<br/>ResetWindowLevelCommand}" --> VM_IDV
    V_IDV -- "Text={Binding<br/>StatusMessage}" --> VM_IDV

    %% ViewModel owns Command
    VM_IDV -- "owns" --> CMD_RWL

    %% Command dependencies (via ViewModel injection)
    CMD_RWL -- "reads defaults" --> M_DIM
    CMD_RWL -- "fallback if<br/>DICOM absent" --> S_WLD
    CMD_RWL -- "pushes undo<br/>action" --> S_WLUS
    CMD_RWL -- "shows confirmation" --> S_SBN
    CMD_RWL -- "creates" --> UA_WLC
    CMD_RWL -- "validates via" --> VO_WL

    %% Model uses constants
    M_DIM -- "uses tag IDs" --> C_DDT
    M_DIM -- "produces" --> VO_WL

    %% Interface implementations
    VM_IDV -. "implements" .-> I_IIDVM
    M_DIM -. "implements" .-> I_IDIM
    S_SBN -. "implements" .-> I_ISN
    S_WLUS -. "implements" .-> I_IUS
    UA_WLC -. "implements" .-> I_IUA
    S_WLD -. "implements" .-> I_IWLD

    %% Undo service manages undo actions
    S_WLUS -- "stores" --> UA_WLC
```

## Layer Dependency Rules

1. **View → ViewModel** via WPF data binding only (no direct method calls).
2. **ViewModel → Model/Services** via constructor-injected interfaces (`IDicomImageModel`, `IStatusNotifier`, `IUndoService`, `IWindowLevelDefaults`).
3. **Model → Constants** — `DicomImageModel` uses `DicomDisplayTags` for DICOM tag identifiers.
4. **No upward dependencies** — Model and Services never reference ViewModel or View.
5. **ExtInf/ contracts** are the compilation boundary — `Src/` depends on `ExtInf/`, never the reverse.

## Project Layout Mapping

| Layer | Project / Folder | Namespace |
|-------|-----------------|-----------|
| Contracts | `ExtInf/ImageDisplay/` | `Philips.CT.Host.ImageDisplay` |
| View | `Src/ImageDisplay/Views/` | `Philips.CT.Host.ImageDisplay.Views` |
| ViewModel | `Src/ImageDisplay/` | `Philips.CT.Host.ImageDisplay` |
| Commands | `Src/ImageDisplay/Commands/` | `Philips.CT.Host.ImageDisplay.Commands` |
| Models | `Src/ImageDisplay/Models/` | `Philips.CT.Host.ImageDisplay.Models` |
| Services | `Src/ImageDisplay/Services/` | `Philips.CT.Host.ImageDisplay.Services` |
| Constants | `Src/ImageDisplay/Constants/` | `Philips.CT.Host.ImageDisplay.Constants` |
| Resources | `Src/ImageDisplay/Resources/` | `Philips.CT.Host.ImageDisplay.Resources` |
| Unit Tests | `Src/ImageDisplay/Test/` | `Philips.CT.Host.ImageDisplay.Test` |
| Module Tests | `Src/ModuleTests/WwWcReset/` | `Philips.CT.Host.ImageDisplay.ModuleTests` |
