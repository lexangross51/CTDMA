using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FieldsDrawer.MVVMTools;

public class NotifyObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public bool RaiseAndSetIfChanged<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, newValue)) return false;

        field = newValue;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        return true;
    }

    public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}