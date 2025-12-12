using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DivinityModManager;
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
            // Platform services initialization deferred - app runs without full service initialization
            System.Console.WriteLine("App initializing without platform services");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"App initialization error: {ex.Message}");
        }

        // Set up the main window
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}

