using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Files.Models;

namespace Files.ViewModels;

public class DirectoryItemViewModel(DirectoryItem item, ICommand? command = null) : ViewModelBase
{
    private DirectoryItem _item = item;

    public string Name
    {
        get => _item.Name;
        set => SetProperty(_item.Name, value, _item, (item, s) => item.Name = s);
    }

    public ICommand? Command { get; private set; } = command;

    public DirectoryItemKind Kind
    {
        get => _item.Kind;
    }
}