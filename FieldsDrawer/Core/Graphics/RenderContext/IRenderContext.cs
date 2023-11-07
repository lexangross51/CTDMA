using FieldsDrawer.Core.Graphics.Objects;

namespace FieldsDrawer.Core.Graphics.RenderContext;

public interface IRenderContext
{
    void AddObject(IBaseObject obj);
    void DeleteObject(IBaseObject obj);
    void DrawObjects();
    void UpdateView();
    int[] GetNewViewport(ScreenSize newScreenSize);
}