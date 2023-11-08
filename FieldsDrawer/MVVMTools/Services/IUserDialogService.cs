using FieldsDrawer.Core.Graphics.Objects;

namespace FieldsDrawer.MVVMTools.Services;

public interface IUserDialogService
{
    void OpenMainWindow();
    string? OpenSelectFileWindow();
    void SendObjectToView(IBaseObject obj);
    void DeleteObjectFromView(IBaseObject obj);
}