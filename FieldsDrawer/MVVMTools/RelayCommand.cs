using System;
using System.Windows.Input;

namespace FieldsDrawer.MVVMTools;

public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    private RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public static RelayCommand Create(Action<object?> execute, Func<object?, bool>? canExecute = null) =>
        new(execute, canExecute);

    public void Execute(object? parameter)
        => _execute(parameter);

    public bool CanExecute(object? parameter)
        => _canExecute == null || _canExecute(parameter);
}