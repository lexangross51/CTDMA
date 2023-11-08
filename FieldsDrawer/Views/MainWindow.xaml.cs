using System.Windows;
using FieldsDrawer.Core.Graphics.RenderContext;

namespace FieldsDrawer.Views;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void MainWindowOnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        GraphicControl.OnChangeSize(new ScreenSize
        {
            Width = Width - MainGrid.ColumnDefinitions[1].ActualWidth - 20,
            Height = Height - 59,
        });
    }
}