using System;
using FieldsDrawer.Core.Graphics.Objects;
using FieldsDrawer.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace FieldsDrawer.MVVMTools.Services.Implementations;

public class UserDialogService : IUserDialogService
{
    private readonly IServiceProvider _services;
    private MainWindow? _mainWindow;
    
    public UserDialogService(IServiceProvider services)
        => _services = services;
    
    public void OpenMainWindow()
    {
        if (_mainWindow is { } window)
        {
            window.Show();
            return;
        }

        window = _services.GetRequiredService<MainWindow>();
        window.Closed += (_, _) => _mainWindow = null;

        _mainWindow = window;

        window.Show();
    }

    public string? OpenSelectFileWindow()
    {
        var window = new OpenFileDialog();
        
        if ((bool)window.ShowDialog()!)
        {
            return window.FileName;
        }

        return null;
    }

    public void SendObjectToView(IBaseObject obj)
    {
        if (_mainWindow is not { } window) return;
        
        window.GraphicControl.AddObject(obj);
    }

    public void DeleteObjectFromView(IBaseObject obj)
    {
        if (_mainWindow is not { } window) return;
        
        window.GraphicControl.DeleteObject(obj);
    }
}