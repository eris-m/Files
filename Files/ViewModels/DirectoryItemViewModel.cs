using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Files.Models;

namespace Files.ViewModels;

/// <summary>
/// View model for a directory item (folder or file).
/// </summary>
/// <param name="item">The directory item to wrap.</param>
/// <param name="selectedCommand">Command to perform when the item is selected.</param>
public class DirectoryItemViewModel(DirectoryItem item, ICommand? selectedCommand = null) : ViewModelBase
{
    /// <summary>
    /// Command to perform when the item is selected.
    /// </summary>
    public ICommand? SelectedCommand { get; private set; } = selectedCommand;
    
    /// <summary>
    /// The name of the item.
    /// </summary>
    public string Name
    {
        get => item.Name;
        set => SetProperty(item.Name, value, item, (i, s) => i.Name = s);
    }

    /// <summary>
    /// The kind of item. If it is a folder or file.
    /// </summary>
    public DirectoryItemKind Kind
    {
        get => item.Kind;
    }
}