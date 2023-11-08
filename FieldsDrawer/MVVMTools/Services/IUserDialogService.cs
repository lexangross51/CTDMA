using FieldsDrawer.Core.Graphics.Colorbar;
using FieldsDrawer.Core.Graphics.Objects;

namespace FieldsDrawer.MVVMTools.Services;

public interface IUserDialogService
{
    void OpenMainWindow();
    string? OpenSelectFileWindow();
    void SendObjectToView(IBaseObject obj);
    bool DeleteObjectFromView(IBaseObject obj);
    void ClearView();

    void SendColorbar(Colorbar colorbar);
    void DeleteColorbar(Colorbar colorbar);
}