# BG3ModManager Avalonia Migration - Implementation Checklist

Use this checklist to track progress through each phase of the migration.

---

## Phase 1: Foundation & Setup

### 1.1 Create Avalonia Project Structure

- [ ] Create `src/GUI.Avalonia/` directory
- [ ] Create `src/GUI.Avalonia/GUI.Avalonia.csproj`
  ```xml
  <Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
      <OutputType>WinExe</OutputType>
      <TargetFramework>net8.0</TargetFramework>
      <RootNamespace>DivinityModManager</RootNamespace>
      <AssemblyName>BG3ModManager</AssemblyName>
    </PropertyGroup>
    <ItemGroup>
      <ProjectReference Include="$(SolutionDir)\src\Core\DivinityModManagerCore.csproj" />
    </ItemGroup>
    <ItemGroup>
      <PackageReference Include="Avalonia" Version="11.1.x" />
      <PackageReference Include="ReactiveUI.Avalonia" Version="20.1.x" />
      <!-- Other dependencies -->
    </ItemGroup>
  </Project>
  ```
- [ ] Create subdirectories:
  - [ ] `src/GUI.Avalonia/Views/`
  - [ ] `src/GUI.Avalonia/ViewModels/`
  - [ ] `src/GUI.Avalonia/Controls/`
  - [ ] `src/GUI.Avalonia/Converters/`
  - [ ] `src/GUI.Avalonia/Themes/`
  - [ ] `src/GUI.Avalonia/Resources/`
  - [ ] `src/GUI.Avalonia/Util/`
  - [ ] `src/GUI.Avalonia/Services/`

### 1.2 Create Avalonia Application Files

- [ ] Create `src/GUI.Avalonia/App.xaml`
  ```xml
  <Application xmlns="https://github.com/avaloniaui"
               xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
               x:Class="DivinityModManager.App">
      <Application.Resources>
          <!-- Theme resources -->
      </Application.Resources>
      <Application.Styles>
          <!-- Style includes -->
      </Application.Styles>
  </Application>
  ```

- [ ] Create `src/GUI.Avalonia/App.xaml.cs`
  ```csharp
  using Avalonia;
  using Avalonia.Controls.ApplicationLifetimes;
  using Avalonia.Markup.Xaml;
  using DivinityModManager.Views;

  namespace DivinityModManager;

  public partial class App : Application
  {
      public override void Initialize()
      {
          AvaloniaXamlLoader.Load(this);
      }

      public override void OnFrameworkInitializationCompleted()
      {
          if (ApplicationLifetime is IClassicDesktopApplicationLifetime desktop)
          {
              desktop.MainWindow = new MainWindow();
          }
          base.OnFrameworkInitializationCompleted();
      }
  }
  ```

- [ ] Create `src/GUI.Avalonia/Program.cs`
  ```csharp
  using Avalonia;
  using System;

  namespace DivinityModManager;

  class Program
  {
      [STAThread]
      public static void Main(string[] args) => BuildAvaloniaApp()
          .StartWithClassicDesktopLifetime(args);

      public static AppBuilder BuildAvaloniaApp()
          => AppBuilder.Configure<App>()
              .UsePlatformDetect()
              .WithInteractiveDebugging(true);
  }
  ```

- [ ] Create basic `src/GUI.Avalonia/Views/MainWindow.axaml` (placeholder)
- [ ] Create basic `src/GUI.Avalonia/Views/MainWindow.axaml.cs` (code-behind)

### 1.3 Update Solution File

- [ ] Open `BG3ModManager.sln`
- [ ] Add GUI.Avalonia project reference
  ```xml
  <ProjectReference Include="src\GUI.Avalonia\GUI.Avalonia.csproj" />
  ```
- [ ] Verify solution builds: `dotnet build`

### 1.4 Verify Basic Setup

- [ ] `dotnet build` completes without errors
- [ ] `dotnet run --project src/GUI.Avalonia/GUI.Avalonia.csproj` launches empty window
- [ ] Commit progress: `git commit -m "Phase 1: Basic Avalonia project structure"`

---

## Phase 2: Core Refactoring

### 2.1 Create Platform Abstraction Layer

- [ ] Create `src/Core/Platform/` directory
- [ ] Create `src/Core/Platform/IPlatformServices.cs` (main interface)
- [ ] Create `src/Core/Platform/IFileDialogService.cs`
- [ ] Create `src/Core/Platform/IFileSystemService.cs`
- [ ] Create `src/Core/Platform/IRegistryService.cs`
- [ ] Create `src/Core/Platform/IProcessService.cs`
- [ ] Create `src/Core/Platform/IAccessibilityService.cs`

### 2.2 Create Windows Platform Implementations

- [ ] Create `src/Core/Platform/Windows/` directory
- [ ] Create `src/Core/Platform/Windows/WindowsFileDialogService.cs`
- [ ] Create `src/Core/Platform/Windows/WindowsFileSystemService.cs`
- [ ] Create `src/Core/Platform/Windows/WindowsRegistryService.cs`
- [ ] Create `src/Core/Platform/Windows/WindowsProcessService.cs`
- [ ] Create `src/Core/Platform/Windows/WindowsAccessibilityService.cs`
- [ ] Create `src/Core/Platform/Windows/WindowsPlatformServices.cs` (aggregator)

### 2.3 Create Linux Platform Implementations

- [ ] Create `src/Core/Platform/Linux/` directory
- [ ] Create `src/Core/Platform/Linux/LinuxFileDialogService.cs`
- [ ] Create `src/Core/Platform/Linux/LinuxFileSystemService.cs`
- [ ] Create `src/Core/Platform/Linux/LinuxRegistryService.cs`
- [ ] Create `src/Core/Platform/Linux/LinuxProcessService.cs`
- [ ] Create `src/Core/Platform/Linux/LinuxAccessibilityService.cs`
- [ ] Create `src/Core/Platform/Linux/LinuxPlatformServices.cs` (aggregator)

### 2.4 Update Core Project

- [ ] Update `src/Core/DivinityModManagerCore.csproj`:
  - [ ] Change target framework from `net8.0-windows` to `net8.0`
  - [ ] Remove `<UseWPF>true</UseWPF>`

- [ ] Update `src/Core/Services.cs`:
  - [ ] Add method to create platform services based on OS
  - [ ] Register `IPlatformServices` in service locator

- [ ] Refactor existing utilities:
  - [ ] `DivinityRegistryHelper.cs` → Implement `IRegistryService`
  - [ ] `JunctionPoint.cs` → Implement `IFileSystemService`
  - [ ] `RecycleBinHelper.cs` → Implement `IFileSystemService`
  - [ ] `ProcessHelper.cs` → Implement `IProcessService`
  - [ ] `ScreenReaderService.cs` → Implement `IAccessibilityService`

### 2.5 Remove WPF Dependencies from Core

- [ ] Remove `System.Windows.*` using statements
- [ ] Remove `System.Windows.Automation` references
- [ ] Remove WPF-specific converters/behaviors from Core
- [ ] Move accessibility peers to `src/GUI.Avalonia/Util/ScreenReader/`

### 2.6 Verify Core Refactoring

- [ ] `dotnet build` completes without errors
- [ ] `src/Core/` has no WPF dependencies
- [ ] Platform services are properly injected
- [ ] Test platform service initialization on Windows and Linux
- [ ] Commit progress: `git commit -m "Phase 2: Core refactoring and platform abstraction"`

---

## Phase 3: Avalonia UI Implementation

### 3.1 Port Custom Controls (13 controls)

For each control, complete:

- [ ] **AlertBar**
  - [ ] Copy from `src/GUI/Controls/AlertBar.xaml` → `src/GUI.Avalonia/Controls/AlertBar.axaml`
  - [ ] Convert XAML syntax to Avalonia
  - [ ] Update code-behind
  - [ ] Test display and functionality

- [ ] **BusyIndicator**
  - [ ] Copy/adapt from WPF
  - [ ] Create AXAML
  - [ ] Implement with Avalonia animations

- [ ] **ModEntryGrid** (Priority: High)
  - [ ] Analyze WPF implementation
  - [ ] Create Avalonia data grid custom control
  - [ ] Implement drag-and-drop
  - [ ] Test with large mod lists

- [ ] **ModListView** (Priority: High)
  - [ ] Create Avalonia ListBox control
  - [ ] Implement drag-and-drop using Avalonia APIs
  - [ ] Test mod reordering

- [ ] **HyperlinkText**
  - [ ] Create Avalonia implementation
  - [ ] Support clicking links

- [ ] **SelectableTextBlock**
  - [ ] Create copyable text implementation
  - [ ] Test text selection and copying

- [ ] **AutoGrayableImage**
  - [ ] Implement disabled state rendering
  - [ ] Test opacity changes

- [ ] Remaining 6 controls:
  - [ ] HotkeyEditorControl
  - [ ] Markdown
  - [ ] CircleDecorator
  - [ ] UnfocusableTextBox
  - [ ] (others as needed)

### 3.2 Port Converters (11 converters)

- [ ] **BoolToVisibilityConverter**
  - [ ] Implement `IValueConverter` for Avalonia
  - [ ] Test conversion logic
  - [ ] Note: Avalonia uses `IsVisible` boolean instead of `Visibility` enum

- [ ] **EnumToStringConverter**
  - [ ] Implement for Avalonia
  - [ ] Test with mod properties

- [ ] **IntToVisibilityConverter** → BoolToVisibilityConverter (consolidate)
  - [ ] Create converter for numeric visibility

- [ ] **ModExistsConverter**
  - [ ] Implement mod existence check display

- [ ] **ModIsActiveConverter**
  - [ ] Implement active mod styling

- [ ] **ModToDisplayNameConverter**
  - [ ] Convert mod data to display name

- [ ] **StringNotEmptyToVisibilityConverter**
  - [ ] Implement string check

- [ ] **StringToLinearBrushConverter**
  - [ ] Implement gradient brush creation

- [ ] **StringToSolidBrushConverter**
  - [ ] Implement color brush creation

- [ ] **StringToUriConverter**
  - [ ] Implement URI parsing

- [ ] **UriToBitmapImageConverter**
  - [ ] Implement image loading from URI

### 3.3 Port Views (13 views)

**Priority Order: HIGH → LOW**

**HIGH Priority (Core functionality):**

- [ ] **MainWindow** (PRIMARY UI)
  - [ ] Copy `src/GUI/Views/MainWindow.xaml`
  - [ ] Convert to `src/GUI.Avalonia/Views/MainWindow.axaml`
  - [ ] Remove AdonisUI controls
  - [ ] Update bindings for Avalonia
  - [ ] Test layout and rendering
  - [ ] Verify ViewModel binding works

- [ ] **MainViewControl** (Mod list view)
  - [ ] Copy from WPF
  - [ ] Adapt for Avalonia DataGrid
  - [ ] Implement drag-and-drop
  - [ ] Test with sample mods

- [ ] **HorizontalModLayout** (Layout variant)
  - [ ] Copy and adapt
  - [ ] Test responsive layout

- [ ] **VerticalModLayout** (Layout variant)
  - [ ] Copy and adapt
  - [ ] Test responsive layout

**MEDIUM Priority (Important dialogs):**

- [ ] **SettingsWindow**
  - [ ] Copy and adapt
  - [ ] Test all settings controls
  - [ ] Verify settings persistence

- [ ] **ExportOrderToArchiveView**
  - [ ] Copy and adapt
  - [ ] Test export functionality

- [ ] **ModUpdatesLayout**
  - [ ] Copy and adapt
  - [ ] Display update information

**LOW Priority (Optional dialogs):**

- [ ] **AboutWindow**
  - [ ] Copy and adapt

- [ ] **AppUpdateWindow**
  - [ ] Copy and adapt
  - [ ] (May replace with custom implementation)

- [ ] **DeleteFilesConfirmationView**
  - [ ] Copy and adapt

- [ ] **HelpWindow**
  - [ ] Copy and adapt

- [ ] **VersionGeneratorWindow**
  - [ ] Copy and adapt

- [ ] **LeaderLibSettingsWindow**
  - [ ] Copy and adapt

### 3.4 Implement Theme System

- [ ] Create `src/GUI.Avalonia/Themes/Colors.axaml`
  - [ ] Define color palette
  - [ ] Include primary, secondary, accent colors
  - [ ] Include neutral/gray colors

- [ ] Create `src/GUI.Avalonia/Themes/Brushes.axaml`
  - [ ] Define solid color brushes
  - [ ] Define gradient brushes
  - [ ] Define dynamic resources for theming

- [ ] Create `src/GUI.Avalonia/Themes/Light.axaml`
  - [ ] Light color scheme
  - [ ] Include all control styles

- [ ] Create `src/GUI.Avalonia/Themes/Dark.axaml`
  - [ ] Dark color scheme
  - [ ] Override brush colors

- [ ] Create `src/GUI.Avalonia/Themes/Styles/` directory
  - [ ] Button.axaml
  - [ ] TextBox.axaml
  - [ ] Window.axaml
  - [ ] ComboBox.axaml
  - [ ] (other controls as needed)

- [ ] Create `src/GUI.Avalonia/Themes/ThemeManager.cs`
  - [ ] Implement theme switching logic
  - [ ] Handle Light/Dark toggle
  - [ ] Persist theme preference

- [ ] Update `App.xaml`
  - [ ] Include theme resources
  - [ ] Include style files

- [ ] Test theming
  - [ ] Light mode renders correctly
  - [ ] Dark mode renders correctly
  - [ ] Theme switching works at runtime

### 3.5 Copy Resources

- [ ] Copy `src/GUI/Resources/Icons/` to `src/GUI.Avalonia/Resources/Icons/`
- [ ] Copy `src/GUI/Resources/*.json` to `src/GUI.Avalonia/Resources/`
- [ ] Copy `src/GUI/BG3ModManager.ico` to `src/GUI.Avalonia/`

### 3.6 Verify UI Implementation

- [ ] All major views display without crashes
- [ ] Bindings work with ViewModels
- [ ] Controls respond to user input
- [ ] Theming system works (Light/Dark toggle)
- [ ] Icons and resources display correctly
- [ ] Commit progress: `git commit -m "Phase 3: Avalonia UI implementation (75% complete)"`

---

## Phase 4: Platform-Specific Integrations

### 4.1 File Dialogs Integration

- [ ] Create `src/GUI.Avalonia/Services/AvaloniaFileDialogService.cs`
  - [ ] Implement `IFileDialogService` using Avalonia dialogs
  - [ ] Use `OpenFileDialog`, `SaveFileDialog`, `OpenFolderDialog`
  - [ ] Handle filter parameters correctly

- [ ] Test file dialogs
  - [ ] Open file dialog works
  - [ ] Save file dialog works
  - [ ] Folder selection works
  - [ ] Test on Windows and Linux

### 4.2 Drag-and-Drop Implementation

- [ ] Update `src/GUI.Avalonia/Views/MainViewControl.axaml`
  - [ ] Remove `gong-wpf-dragdrop` bindings
  - [ ] Use Avalonia's native drag-and-drop

- [ ] Update `src/GUI.Avalonia/Controls/ModListView.axaml`
  - [ ] Implement drag-and-drop handlers
  - [ ] Reference ViewModel handlers

- [ ] Create/update handlers (may move from WPF):
  - [ ] Drag handler implementation
  - [ ] Drop handler implementation
  - [ ] Mod reordering logic

- [ ] Test drag-and-drop
  - [ ] Drag mod and reorder
  - [ ] Visual feedback during drag
  - [ ] Proper list reordering
  - [ ] Test on Windows and Linux

### 4.3 Update Auto-Update System

- [ ] Create `src/Core/Services/IUpdateService.cs`
  - [ ] Define update checking interface
  - [ ] Define update installation interface

- [ ] Create `src/GUI.Avalonia/Services/AvaloniaUpdateService.cs`
  - [ ] Implement update checking
  - [ ] Show update notification dialog
  - [ ] Handle update installation (platform-specific)

- [ ] Remove `Autoupdater.NET` from dependencies

- [ ] Test update system
  - [ ] Check for updates works
  - [ ] Update notification shows
  - [ ] Update installation triggers correctly

### 4.4 Accessibility (Screen Readers)

- [ ] Verify CrossSpeak integration works on Windows
- [ ] Create Linux accessibility integration
  - [ ] Use libspeechd or equivalent
  - [ ] Test screen reader announcement
  - [ ] Add accessibility attributes to UI elements

- [ ] Test with screen reader (Windows and Linux)
  - [ ] Menu announcements work
  - [ ] Status messages announce
  - [ ] Focus navigation works

### 4.5 Process Integration

- [ ] Test game launching from mod manager
- [ ] Test opening URLs (Nexus Mods links, etc.)
- [ ] Test opening file explorer to mod folders
- [ ] Test on Windows and Linux

---

## Phase 5: Build & Release System

### 5.1 Update BuildRelease.py

- [ ] Modify `BuildRelease.py` to support multi-platform:
  ```python
  # Pseudo-code structure:
  for platform in ["windows", "linux"]:
      dotnet publish -c Release -r {platform}-x64 \
          --project src/GUI.Avalonia/GUI.Avalonia.csproj

      if platform == "windows":
          create_windows_installer()  # NSIS or similar
      else:
          create_appimage()  # AppImage for Linux
  ```

- [ ] Test Windows build
  - [ ] Build completes successfully
  - [ ] Executable runs on clean Windows machine
  - [ ] All dependencies bundled

- [ ] Test Linux build
  - [ ] Build completes successfully
  - [ ] AppImage/archive created
  - [ ] Runs on clean Linux machine
  - [ ] All dependencies bundled

### 5.2 Create CI/CD Workflows

- [ ] Create `.github/workflows/build-windows.yml`
  - [ ] Trigger on push/PR
  - [ ] Build Windows release
  - [ ] Upload artifacts

- [ ] Create `.github/workflows/build-linux.yml`
  - [ ] Trigger on push/PR
  - [ ] Build Linux release
  - [ ] Upload artifacts

- [ ] Test workflows
  - [ ] Trigger manually
  - [ ] Verify builds complete
  - [ ] Artifacts available for download

### 5.3 Update Project Configuration

- [ ] Update `BG3ModManager.sln` to use GUI.Avalonia as default startup project
- [ ] Update build configurations in .csproj files
- [ ] Remove or archive old WPF GUI.csproj (optional, may keep for reference)

---

## Phase 6: Documentation & Testing

### 6.1 Documentation

- [ ] ✅ AVALONIA_MIGRATION_PLAN.md (Created)
- [ ] ✅ AVALONIA_DEVELOPER_GUIDE.md (Created)
- [ ] ✅ GETTING_STARTED_AVALONIA.md (Created)
- [ ] ✅ TEAM_MIGRATION_SUMMARY.md (Created)
- [ ] ✅ IMPLEMENTATION_CHECKLIST.md (This file)

### 6.2 Testing Checklist

**Unit Testing:**
- [ ] Core business logic tests pass
- [ ] Platform abstraction tests pass

**Integration Testing:**
- [ ] Mod loading works
- [ ] Mod list displays correctly
- [ ] Drag-and-drop reordering works
- [ ] Settings save and load
- [ ] Export to game works
- [ ] File dialogs work
- [ ] Theme switching works

**Platform Testing - Windows:**
- [ ] Build and package successful
- [ ] Application launches
- [ ] All features functional
- [ ] Performance acceptable
- [ ] Registry access works (settings)
- [ ] Game launching works
- [ ] Update checking works

**Platform Testing - Linux:**
- [ ] Build and package successful
- [ ] AppImage/archive runs
- [ ] All features functional
- [ ] Performance acceptable
- [ ] Config file access works (settings)
- [ ] Game launching works
- [ ] Update checking works

**Regression Testing (vs WPF):**
- [ ] UI layout identical
- [ ] Colors/themes match
- [ ] Keyboard shortcuts work
- [ ] Hotkeys work
- [ ] All menu items accessible
- [ ] Dialogs display correctly

### 6.3 Bug Fix & Polish

- [ ] Identify and fix any blocking issues
- [ ] Performance tuning if needed
- [ ] Visual polish and refinement
- [ ] Final accessibility audit

---

## Release Preparation

### Pre-Release Checklist

- [ ] All tests passing on Windows
- [ ] All tests passing on Linux
- [ ] No critical bugs remaining
- [ ] Documentation complete and reviewed
- [ ] Team trained and ready
- [ ] Release notes prepared

### Release Steps

- [ ] Create release branch: `git checkout -b release/v1.1.0-avalonia`
- [ ] Update version numbers in .csproj files
- [ ] Update CHANGELOG
- [ ] Commit release: `git commit -m "Release v1.1.0-avalonia"`
- [ ] Create git tag: `git tag -a v1.1.0-avalonia -m "Avalonia cross-platform release"`
- [ ] Build final releases (Windows + Linux)
- [ ] Create GitHub release page
- [ ] Publish release notes
- [ ] Announce to community

---

## Post-Release

### Monitoring

- [ ] Monitor GitHub issues for bugs
- [ ] Collect user feedback
- [ ] Track performance metrics
- [ ] Monitor crash reports

### Maintenance

- [ ] Fix reported bugs quickly
- [ ] Keep dependencies updated
- [ ] Plan future improvements
- [ ] Consider macOS support

### Deprecation (Optional)

- [ ] Archive old WPF source code (if desired)
- [ ] Remove WPF references from main branch (if desired)
- [ ] Redirect old releases to Avalonia version

---

## Notes

- This checklist can be customized based on team availability
- Some items can run in parallel (e.g., Windows + Linux implementations)
- Regular testing should occur during each phase
- Team communication is critical for coordinating work

**Last Updated:** [Date]
**Migration Lead:** [Name]
**Status:** 🎯 Ready to Start Phase 1

