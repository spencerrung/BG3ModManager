using System;
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
        // Initialize platform services early in the application lifecycle
        try
        {
            // NOTE: PlatformServiceInitializer requires the Core project which has external dependencies
            // For now, this is deferred until the Core project builds successfully
            System.Console.WriteLine("Platform services initialization deferred (Core project pending build)");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Platform service initialization error: {ex.Message}");
        }

        // Set up the main window
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}

