# Avalonia Developer Guide

A practical guide for working with the BG3ModManager Avalonia codebase.

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Project Structure](#project-structure)
3. [Common Development Tasks](#common-development-tasks)
4. [Platform Abstraction Layer](#platform-abstraction-layer)
5. [Styling & Theming](#styling--theming)
6. [Debugging Tips](#debugging-tips)
7. [Testing Guide](#testing-guide)

---

## Architecture Overview

### Core Principle: Separation of Concerns

```
┌─────────────────────────────────────────┐
│         BG3ModManager.sln              │
├─────────────────────────────────────────┤
│                                         │
│  ┌──────────────────────────────────┐  │
│  │  src/GUI.Avalonia (WPF Legacy)   │  │
│  │  ├─ Views/                       │  │
│  │  ├─ ViewModels/                  │  │
│  │  ├─ Controls/                    │  │
│  │  ├─ Converters/                  │  │
│  │  ├─ Themes/                      │  │
│  │  └─ Services/ (Platform-specific)│  │
│  └──────────────────────────────────┘  │
│                ▼                         │
│  ┌──────────────────────────────────┐  │
│  │  src/Core/                        │  │
│  │  ├─ Models/ (Data)               │  │
│  │  ├─ ViewModels/ (MVVM Base)      │  │
│  │  ├─ Services/ (Business Logic)   │  │
│  │  ├─ Platform/ (Abstraction)      │  │
│  │  └─ Util/                        │  │
│  └──────────────────────────────────┘  │
│                ▼                         │
│  ┌──────────────────────────────────┐  │
│  │  External/ (Dependencies)        │  │
│  │  ├─ LSLib (Game files)          │  │
│  │  └─ CrossSpeak (Accessibility)  │  │
│  └──────────────────────────────────┘  │
│                                         │
└─────────────────────────────────────────┘
```

### Data Flow

```
User Interaction (View)
    ▼
Event/Command (Binding)
    ▼
ViewModel (ReactiveUI)
    ▼
Service (Business Logic)
    ▼
Model (Data Structure)
    ▼
Core Service / Platform Abstraction
    ▼
Platform-Specific Implementation (Windows/Linux)
```

### Key Patterns

**MVVM (Model-View-ViewModel):**
- `View` (AXAML) - UI structure and layout
- `ViewModel` - UI logic, commands, reactive properties
- `Model` - Data structures in Core project

**Reactive Programming (ReactiveUI):**
- ViewModels inherit from `ReactiveObject`
- Properties are `ReactiveProperty<T>` or reactive LINQ
- Commands use `ReactiveCommand`

**Dependency Injection:**
- Service locator pattern in `src/Core/Services.cs`
- Platform services registered based on runtime OS

---

## Project Structure

### src/GUI.Avalonia/

```
GUI.Avalonia/
├── Views/                    # AXAML views and windows
│   ├── MainWindow.axaml     # Main application window
│   ├── MainViewControl.axaml # Mod list interface
│   ├── SettingsWindow.axaml  # Application settings
│   ├── AboutWindow.axaml
│   └── (other views...)
│
├── ViewModels/              # See src/Core/ViewModels/ for base classes
│   ├── MainWindowViewModel.cs
│   ├── SettingsWindowViewModel.cs
│   └── (other ViewModels...)
│
├── Controls/                # Custom Avalonia controls
│   ├── AlertBar.axaml       # Status/notification bar
│   ├── BusyIndicator.axaml  # Loading spinner
│   ├── ModListView.axaml    # Mod list with drag-drop
│   └── (other controls...)
│
├── Converters/              # Value converters for bindings
│   ├── BoolToVisibilityConverter.cs
│   ├── EnumToStringConverter.cs
│   └── (other converters...)
│
├── Themes/                  # Styling and theming
│   ├── Light.axaml          # Light theme resources
│   ├── Dark.axaml           # Dark theme resources
│   ├── Colors.axaml         # Color palette
│   ├── Brushes.axaml        # Brush definitions
│   ├── Styles/              # Control-specific styles
│   │   ├── Button.axaml
│   │   ├── TextBox.axaml
│   │   └── (other styles...)
│   └── ThemeManager.cs      # Handle theme switching
│
├── Services/                # Avalonia-specific services
│   ├── AvaloniaFileDialogService.cs
│   ├── AvaloniaUpdateService.cs
│   └── (other services...)
│
├── Util/                    # Utility classes
│   ├── BindingHelper.cs
│   ├── ScreenReader/        # Accessibility (Linux: libspeechd)
│   └── (other utilities...)
│
├── Resources/               # Non-XAML resources
│   ├── Icons/               # PNG images
│   └── *.json               # Data files
│
├── App.xaml                 # Application resources
├── App.xaml.cs              # Application startup
├── Program.cs               # Entry point
└── GUI.Avalonia.csproj      # Project file
```

### src/Core/

```
Core/
├── Models/                  # Data structures
│   ├── App/                 # Application data (Settings, Hotkeys, etc.)
│   ├── *ModData.cs          # Mod-related data classes
│   └── (others...)
│
├── Platform/                # Cross-platform abstraction layer
│   ├── IPlatformServices.cs # Main interface
│   ├── IFileDialogService.cs
│   ├── IRegistryService.cs
│   ├── IFileSystemService.cs
│   ├── IProcessService.cs
│   ├── IAccessibilityService.cs
│   ├── Windows/             # Windows implementations
│   │   ├── WindowsFileSystemService.cs
│   │   ├── WindowsRegistryService.cs
│   │   └── (other implementations...)
│   └── Linux/               # Linux implementations
│       ├── LinuxFileSystemService.cs
│       ├── LinuxRegistryService.cs
│       └── (other implementations...)
│
├── Services/                # Application services (platform-agnostic)
│   ├── ModRegistryService.cs  # Mod loading/parsing
│   ├── FileWatcherService.cs  # File system monitoring
│   ├── IUpdateService.cs      # Update checking abstraction
│   └── (others...)
│
├── ViewModels/              # Base MVVM classes
│   ├── BaseViewModel.cs      # Extends ReactiveObject
│   ├── BaseHistoryViewModel.cs # Undo/redo support
│   └── IDivinityAppViewModel.cs # Main app ViewModel interface
│
├── Util/                    # Legacy utilities (being refactored)
│   ├── DivinityModDataLoader.cs
│   ├── ProcessHelper.cs (being abstracted)
│   └── (others...)
│
└── DivinityApp.cs           # Main application class
```

---

## Common Development Tasks

### Task 1: Adding a New View

**Scenario:** You want to add a new dialog or window to the application.

**Step-by-step:**

1. **Create the AXAML file:**
   ```bash
   # src/GUI.Avalonia/Views/MyNewWindow.axaml
   ```

   ```xml
   <Window xmlns="https://github.com/avaloniaui"
           xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
           x:Class="DivinityModManager.Views.MyNewWindow"
           Title="My New Window"
           Width="600"
           Height="400">
       <Grid ColumnDefinitions="*" RowDefinitions="*,Auto">
           <!-- Content here -->
           <TextBlock Text="Hello World" Grid.Row="0" />
       </Grid>
   </Window>
   ```

2. **Create the code-behind:**
   ```csharp
   // src/GUI.Avalonia/Views/MyNewWindow.axaml.cs
   using Avalonia.Controls;

   namespace DivinityModManager.Views;

   public partial class MyNewWindow : Window
   {
       public MyNewWindow()
       {
           InitializeComponent();
       }
   }
   ```

3. **Create the ViewModel:**
   ```csharp
   // src/GUI.Avalonia/ViewModels/MyNewWindowViewModel.cs
   using DivinityModManager.Core.ViewModels;
   using ReactiveUI;

   namespace DivinityModManager.ViewModels;

   public class MyNewWindowViewModel : BaseViewModel
   {
       private string _title = "My New Window";
       public string Title
       {
           get => _title;
           set => this.RaiseAndSetIfChanged(ref _title, value);
       }

       public MyNewWindowViewModel()
       {
           // Initialize reactive commands if needed
       }
   }
   ```

4. **Bind the ViewModel to the View:**
   ```csharp
   // In MyNewWindow.axaml.cs code-behind
   public MyNewWindow()
   {
       InitializeComponent();
       DataContext = new MyNewWindowViewModel();
   }
   ```

5. **Or use design-time bindings in AXAML:**
   ```xml
   <Window ...
           d:DataContext="{x:Static local:MyNewWindowViewModel}">
   ```

### Task 2: Adding a New Converter

**Scenario:** You need to convert a data type for display (e.g., boolean → visibility).

**Steps:**

1. **Create the converter class:**
   ```csharp
   // src/GUI.Avalonia/Converters/MyCustomConverter.cs
   using Avalonia.Data.Converters;
   using System.Globalization;

   namespace DivinityModManager.Converters;

   public class MyCustomConverter : IValueConverter
   {
       public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
       {
           if (value is MyType myValue)
           {
               return TransformValue(myValue);
           }
           return null;
       }

       public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
       {
           // Implement reverse conversion if needed
           throw new NotImplementedException();
       }

       private string TransformValue(MyType value)
       {
           // Your conversion logic
           return $"Converted: {value}";
       }
   }
   ```

2. **Register the converter in App.xaml (optional, for XAML access):**
   ```xml
   <!-- src/GUI.Avalonia/App.xaml -->
   <Application.Resources>
       <local:MyCustomConverter x:Key="MyCustomConverter" />
   </Application.Resources>
   ```

3. **Use in a view:**
   ```xml
   <TextBlock Text="{Binding MyValue, Converter={StaticResource MyCustomConverter}}" />
   ```

### Task 3: Adding a Custom Control

**Scenario:** You need to create a reusable UI component (like a custom button or input field).

**Steps:**

1. **Create the control file:**
   ```csharp
   // src/GUI.Avalonia/Controls/MyCustomControl.axaml.cs
   using Avalonia;
   using Avalonia.Controls;

   namespace DivinityModManager.Controls;

   public partial class MyCustomControl : UserControl
   {
       public static readonly StyledProperty<string> TextProperty =
           AvaloniaProperty.Register<MyCustomControl, string>(nameof(Text));

       public string Text
       {
           get => GetValue(TextProperty);
           set => SetValue(TextProperty, value);
       }

       public MyCustomControl()
       {
           InitializeComponent();
       }
   }
   ```

2. **Create the AXAML for the control:**
   ```xml
   <!-- src/GUI.Avalonia/Controls/MyCustomControl.axaml -->
   <UserControl xmlns="https://github.com/avaloniaui"
                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                x:Class="DivinityModManager.Controls.MyCustomControl">
       <Border Background="White" Padding="8">
           <TextBlock Text="{Binding Text, RelativeSource={RelativeSource AncestorType=local:MyCustomControl}}" />
       </Border>
   </UserControl>
   ```

3. **Use in a view:**
   ```xml
   <Window ...
           xmlns:controls="clr-namespace:DivinityModManager.Controls">
       <controls:MyCustomControl Text="Hello from custom control" />
   </Window>
   ```

### Task 4: Using Platform-Specific Services

**Scenario:** Your feature needs to access the file system in a cross-platform way.

**Approach:**

1. **Inject the platform service:**
   ```csharp
   public class MyViewModel : BaseViewModel
   {
       private readonly IPlatformServices _platform;

       public MyViewModel(IPlatformServices platform)
       {
           _platform = platform;
       }

       public async Task OpenFile()
       {
           var filePath = await _platform.FileDialogs.OpenFileAsync(
               "Select a mod",
               "Zip Files|*.zip");

           if (filePath != null)
           {
               // Process file
           }
       }
   }
   ```

2. **Get the service from the service locator (legacy pattern):**
   ```csharp
   var platform = Services.GetService<IPlatformServices>();
   var fileDialogs = platform.FileDialogs;
   ```

3. **Don't do direct P/Invoke or platform-specific code in ViewModels:**
   ```csharp
   // ❌ DON'T DO THIS
   [DllImport("kernel32.dll")]
   private static extern bool CreateJunction(string link, string target);

   // ✅ DO THIS INSTEAD
   _platform.FileSystem.CreateJunctionPoint(linkPath, targetPath);
   ```

---

## Platform Abstraction Layer

### Understanding the Abstraction

The platform abstraction layer ensures that cross-platform code doesn't contain Windows-specific P/Invoke or Linux-specific system calls scattered throughout the codebase.

### Available Platform Services

#### 1. IFileDialogService
```csharp
public interface IFileDialogService
{
    Task<string?> OpenFileAsync(string title, string filter);
    Task<IReadOnlyList<string>> OpenFilesAsync(string title, string filter);
    Task<string?> OpenFolderAsync(string title);
    Task<string?> SaveFileAsync(string title, string filter, string suggestedFileName);
}
```

**Usage:**
```csharp
var file = await _platform.FileDialogs.OpenFileAsync(
    "Select Mod File",
    "ZIP Files (*.zip)|*.zip|All Files (*.*)|*.*");
```

#### 2. IFileSystemService
```csharp
public interface IFileSystemService
{
    void DeleteToRecycleBin(string path);
    void CreateJunctionPoint(string linkPath, string targetPath);
    void DeleteJunctionPoint(string linkPath);
    bool IsJunctionPoint(string path);
}
```

**Windows Implementation:** Uses P/Invoke to NTFS junction points, Recycle Bin API
**Linux Implementation:** Uses symlinks (junctions not applicable), file deletion

#### 3. IRegistryService
```csharp
public interface IRegistryService
{
    string? GetValue(string path, string key);
    void SetValue(string path, string key, string value);
}
```

**Windows Implementation:** Registry access via `Microsoft.Win32.Registry`
**Linux Implementation:** File-based configuration (e.g., `~/.config/bg3modmanager/`)

#### 4. IProcessService
```csharp
public interface IProcessService
{
    void LaunchGame(string processPath, string arguments);
    void OpenUrl(string url);
    void OpenFolder(string folderPath);
}
```

#### 5. IAccessibilityService
```csharp
public interface IAccessibilityService
{
    void Announce(string message);
    Task AnnounceAsync(string message);
}
```

**Windows Implementation:** Uses `System.Windows.Automation.Peers.AutomationPeer`
**Linux Implementation:** Uses `libspeechd` via CrossSpeak

### Adding a New Platform Service

1. **Define the interface in `src/Core/Platform/`:**
   ```csharp
   public interface IMyService
   {
       void DoSomething();
   }
   ```

2. **Implement for Windows in `src/Core/Platform/Windows/`:**
   ```csharp
   public class WindowsMyService : IMyService
   {
       public void DoSomething()
       {
           // Windows implementation using P/Invoke or Windows APIs
       }
   }
   ```

3. **Implement for Linux in `src/Core/Platform/Linux/`:**
   ```csharp
   public class LinuxMyService : IMyService
   {
       public void DoSomething()
       {
           // Linux implementation using Linux APIs or shell commands
       }
   }
   ```

4. **Add to `IPlatformServices` interface:**
   ```csharp
   public interface IPlatformServices
   {
       IFileDialogService FileDialogs { get; }
       IRegistryService Registry { get; }
       IFileSystemService FileSystem { get; }
       IProcessService Process { get; }
       IAccessibilityService Accessibility { get; }
       IMyService MyService { get; } // Add new service
   }
   ```

5. **Register in `Services.cs`:**
   ```csharp
   public static IPlatformServices CreatePlatformServices()
   {
       return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
           ? new WindowsPlatformServices()
           : new LinuxPlatformServices();
   }
   ```

---

## Styling & Theming

### Understanding Avalonia Styles

Avalonia uses a resource/styling system similar to WPF:

```xml
<!-- Define a style -->
<Style Selector="Button.Primary">
    <Setter Property="Background" Value="{DynamicResource PrimaryBrush}" />
    <Setter Property="Foreground" Value="White" />
    <Setter Property="Padding" Value="12,8" />
</Style>

<!-- Use the style -->
<Button Classes="Primary">Click Me</Button>
```

### Theme Architecture

```
Themes/
├── Colors.axaml         # Color palette (RGB values)
├── Brushes.axaml        # Brush definitions (references Colors)
├── Light.axaml          # Light theme (swaps brush values)
├── Dark.axaml           # Dark theme (swaps brush values)
├── Styles/              # Control-specific styles
│   ├── Button.axaml
│   ├── TextBox.axaml
│   ├── Window.axaml
│   └── (others...)
└── ThemeManager.cs      # Code to switch themes
```

### How Themes Work

**Example: Primary Color**

1. **Define in Colors.axaml:**
   ```xml
   <Color x:Key="Primary_Dark">#2196F3</Color>
   <Color x:Key="Primary_Light">#E3F2FD</Color>
   ```

2. **Reference in Brushes.axaml:**
   ```xml
   <!-- Light theme brush (default) -->
   <SolidColorBrush x:Key="PrimaryBrush" Color="{StaticResource Primary_Light}" />
   ```

3. **Override in Dark.axaml:**
   ```xml
   <SolidColorBrush x:Key="PrimaryBrush" Color="{StaticResource Primary_Dark}" />
   ```

4. **Use in controls:**
   ```xml
   <Button Background="{DynamicResource PrimaryBrush}" />
   ```

5. **Switch at runtime (ThemeManager.cs):**
   ```csharp
   public static void SetTheme(bool isDark)
   {
       var app = Application.Current;
       app.Resources.MergedDictionaries.Clear();

       if (isDark)
           app.Resources.MergedDictionaries.Add(
               new ResourceInclude(new Uri("avares://GUI.Avalonia/Themes/Dark.axaml")));
       else
           app.Resources.MergedDictionaries.Add(
               new ResourceInclude(new Uri("avares://GUI.Avalonia/Themes/Light.axaml")));
   }
   ```

### Adding a New Control Style

1. **Create `Styles/MyControl.axaml`:**
   ```xml
   <Styles xmlns="https://github.com/avaloniaui">
       <Style Selector="MyControl">
           <Setter Property="Background" Value="{DynamicResource ControlBackgroundBrush}" />
           <Setter Property="Foreground" Value="{DynamicResource ControlForegroundBrush}" />
           <Setter Property="BorderBrush" Value="{DynamicResource ControlBorderBrush}" />
           <Setter Property="BorderThickness" Value="1" />
       </Style>

       <Style Selector="MyControl:disabled">
           <Setter Property="Opacity" Value="0.5" />
       </Style>
   </Styles>
   ```

2. **Include in `App.xaml`:**
   ```xml
   <Application.Styles>
       <StyleInclude Source="avares://GUI.Avalonia/Themes/Styles/MyControl.axaml" />
   </Application.Styles>
   ```

---

## Debugging Tips

### 1. Debug Mode in Visual Studio / Rider

```bash
# Build and run in debug mode
dotnet run --project src/GUI.Avalonia/GUI.Avalonia.csproj --configuration Debug
```

### 2. Enable Avalonia Dev Tools

Add to `Program.cs`:
```csharp
.WithInteractiveDebugging(true)
```

Then press `F12` in the running application to inspect the visual tree.

### 3. Logging

```csharp
// Enable Avalonia logging
LogicalTree.Attach(control);
```

### 4. Debugging Bindings

```xml
<!-- Add ElementName debugging attribute -->
<TextBlock Text="{Binding MyProperty,
                          RelativeSource={RelativeSource AncestorType=Window}}"
           x:Name="DebugBlock" />
```

### 5. Common Issues

**Issue:** Binding doesn't work
- ✅ Check binding path syntax: `Binding MyProperty` (not `{Binding MyProperty}`)
- ✅ Ensure `DataContext` is set on parent control
- ✅ Check that property raises `INotifyPropertyChanged`

**Issue:** Theme colors not applying
- ✅ Use `{DynamicResource BrushName}` not `{StaticResource}`
- ✅ Ensure theme file is included in `App.xaml`

**Issue:** Controls not appearing
- ✅ Check `Grid.Row` and `Grid.Column` are set correctly
- ✅ Verify `IsVisible` binding (default is True)
- ✅ Check z-order (which control is on top)

---

## Testing Guide

### Unit Testing

Tests for business logic in `src/Core/` should not require GUI:

```csharp
[TestClass]
public class DivinityModDataLoaderTests
{
    [TestMethod]
    public void LoadMods_ValidFolder_ReturnsMods()
    {
        // Arrange
        var loader = new DivinityModDataLoader();
        var testModPath = @"C:\BG3\Mods";

        // Act
        var mods = loader.LoadMods(testModPath);

        // Assert
        Assert.IsNotNull(mods);
        Assert.IsTrue(mods.Count > 0);
    }
}
```

### Integration Testing

Test platform services:

```csharp
[TestClass]
public class FileDialogServiceTests
{
    [TestMethod]
    public async Task OpenFileAsync_ValidFilter_ReturnsPath()
    {
        // Arrange
        IPlatformServices platform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new WindowsPlatformServices()
            : new LinuxPlatformServices();

        // Act
        var result = await platform.FileDialogs.OpenFileAsync(
            "Test Dialog",
            "Text Files|*.txt");

        // Assert (user interaction test - may be null if user cancels)
        Assert.IsTrue(result == null || File.Exists(result));
    }
}
```

### UI Testing

Manual testing is recommended for UI:

```
1. Run application: dotnet run
2. Test mod loading
3. Test drag-and-drop reordering
4. Test settings window
5. Test export functionality
6. Test on target OS (Windows and Linux)
```

---

## Quick Reference

### Avalonia AXAML vs WPF XAML

| Feature | WPF XAML | Avalonia AXAML | Notes |
|---------|----------|----------------|-------|
| Namespace | `http://schemas.microsoft.com/winfx/` | `https://github.com/avaloniaui` | Different URL |
| Class attr | `x:Class` | `x:Class` | Same |
| Binding | `{Binding}` | `{Binding}` | Same |
| DynamicResource | `{DynamicResource}` | `{DynamicResource}` | Same |
| Visibility | `Visibility.Collapsed` | `IsVisible="False"` | Different property |
| DependencyProperty | `DependencyProperty.Register()` | `AvaloniaProperty.Register()` | Different API |

### Common Avalonia Controls

```xml
<!-- Text -->
<TextBlock Text="Label" />
<TextBox />

<!-- Buttons -->
<Button Content="Click" Click="OnButtonClick" />

<!-- Lists -->
<ListBox Items="{Binding Items}" />
<DataGrid Items="{Binding Items}" />

<!-- Layout -->
<Grid ColumnDefinitions="*,Auto" RowDefinitions="Auto,*" />
<StackPanel Orientation="Vertical" />
<WrapPanel Orientation="Horizontal" />

<!-- Popups -->
<Popup />
<Window />

<!-- Input -->
<ComboBox Items="{Binding Options}" SelectedItem="{Binding Selected}" />
<CheckBox Content="Option" />
<RadioButton Content="Choice" />
```

---

## Getting Help

- **Avalonia Docs:** https://docs.avaloniaui.net/
- **ReactiveUI Docs:** https://reactiveui.net/
- **Community Chat:** https://gitter.im/AvaloniaUI/Avalonia

