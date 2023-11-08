using System;
using System.Windows;
using FieldsDrawer.MVVMTools.Services;
using FieldsDrawer.MVVMTools.Services.Implementations;
using FieldsDrawer.ViewModels;
using FieldsDrawer.Views;
using Microsoft.Extensions.DependencyInjection;

namespace FieldsDrawer;

public partial class App
{
    private static IServiceProvider? _services;

    public static IServiceProvider Services => _services ??= InitializeServices().BuildServiceProvider();

    private static IServiceCollection InitializeServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton(s =>
        {
            var viewModel = s.GetRequiredService<MainViewModel>();
            var window = new MainWindow { DataContext = viewModel };

            return window;
        });

        services.AddSingleton<IUserDialogService, UserDialogService>();

        return services;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Services.GetRequiredService<IUserDialogService>().OpenMainWindow();
    }
}