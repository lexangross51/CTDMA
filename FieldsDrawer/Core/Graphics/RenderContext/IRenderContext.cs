using FieldsDrawer.Core.Graphics.Objects;

namespace FieldsDrawer.Core.Graphics.RenderContext;

public interface IRenderContext
{
    void AddObject(IBaseObject obj);
    bool DeleteObject(IBaseObject obj);
    void ClearView();
    void DrawObjects();
    void UpdateView();
    int[] GetNewViewport(ScreenSize newScreenSize);
}