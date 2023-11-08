using System;
using System.Drawing;
using System.Windows.Input;
using FieldsDrawer.Core.Graphics.Camera;
using FieldsDrawer.Core.Graphics.Objects;
using FieldsDrawer.Core.Graphics.RenderContext;
using FieldsDrawer.Core.Graphics.Text;
using FieldsDrawer.Core.Graphics.Viewport;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Wpf;
using Indent = FieldsDrawer.Core.Graphics.RenderContext.Indent;
using ScreenSize = FieldsDrawer.Core.Graphics.RenderContext.ScreenSize;

namespace FieldsDrawer.Core.Graphics.Scene;

public partial class GraphicControl
{
    private readonly Viewport2DRenderer _viewPortRenderer;
    private readonly IRenderContext _baseGraphic;
    private bool _isMouseDown;
    private double _mouseXPrevious, _mouseYPrevious;

    public GraphicControl()
    {
        InitializeComponent();

        GlControl.Start(new GLWpfControlSettings
        {
            MajorVersion = 2,
            MinorVersion = 1,
            RenderContinuously = false
        });

        var font = new SharpPlotFont
        {
            Color = Color.Black,
            Size = 12,
        };
        var indent = TextPrinter.TextMeasure("0", font).Height;
        var renderSettings = new RenderSettings
        {
            ScreenSize = new ScreenSize
            {
                Width = 1,
                Height = 1,
            },
            Indent = new Indent
            {
                Left = indent,
                Bottom = indent
            }
        };

        var camera = new Camera2D(new OrthographicProjection(new double[] { -1, 1, -1, 1, -1, 1 }, 1));

        _viewPortRenderer = new Viewport2DRenderer(renderSettings, camera) { Font = font };
        _baseGraphic = new BaseGraphic2D(renderSettings, camera);

        GL.ClearColor(Color.White);
    }

    private void OnRender(TimeSpan obj)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);

        _baseGraphic.DrawObjects();
        _viewPortRenderer.RenderAxis();
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var pos = e.GetPosition(this);
        var projection = _viewPortRenderer.GetCamera().GetProjection();
        var renderSettings = _viewPortRenderer.GetRenderSettings();

        projection.FromWorldToProjection(pos.X, pos.Y, renderSettings, out var x, out var y);

        _viewPortRenderer.GetCamera().Zoom(x, y, e.Delta);
        _viewPortRenderer.UpdateView();
        GlControl.InvalidateVisual();
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isMouseDown = true;
        var mousePosition = e.GetPosition(this);
        _mouseXPrevious = mousePosition.X;
        _mouseYPrevious = mousePosition.Y;
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isMouseDown = false;
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        var projection = _viewPortRenderer.GetCamera().GetProjection();
        var renderSettings = _viewPortRenderer.GetRenderSettings();

        var mousePosition = e.GetPosition(this);
        projection.FromWorldToProjection(mousePosition.X, mousePosition.Y, renderSettings, out var xCurrent,
            out var yCurrent);

        if (!_isMouseDown) return;

        projection.FromWorldToProjection(_mouseXPrevious, _mouseYPrevious, renderSettings, out var xPrevious,
            out var yPrevious);

        _viewPortRenderer.GetCamera().Move(-xCurrent + xPrevious, -yCurrent + yPrevious);
        _mouseXPrevious = mousePosition.X;
        _mouseYPrevious = mousePosition.Y;

        _viewPortRenderer.UpdateView();
        GlControl.InvalidateVisual();
    }

    private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        _viewPortRenderer.DrawingGrid = !_viewPortRenderer.DrawingGrid;
        GlControl.InvalidateVisual();
    }

    public void OnChangeSize(ScreenSize newSize)
    {
        _viewPortRenderer.GetNewViewport(newSize);
        _baseGraphic.GetNewViewport(newSize);
        _viewPortRenderer.UpdateView();

        Width = newSize.Width;
        Height = newSize.Height;
        GlControl.InvalidateArrange();
    }


    public void AddObject(IBaseObject obj)
    {
        _baseGraphic.AddObject(obj);
        GlControl.InvalidateVisual();
    }

    public bool DeleteObject(IBaseObject obj)
    {
        var res = _baseGraphic.DeleteObject(obj);
        GlControl.InvalidateVisual();

        return res;
    }

    public void ClearView()
    {
        _baseGraphic.ClearView();
        GlControl.InvalidateVisual();
    }
}