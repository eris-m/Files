using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Files.Models;

namespace Files.ViewModels;

/// <summary>
/// Main view model.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<DirectoryItemViewModel> _items = [];
    
    [ObservableProperty]
    private int _gridColumns = 10;
    
    private readonly Explorer _explorer;

    public MainWindowViewModel()
    {
        _explorer = new Explorer();
        RefreshItems();
    }

    /// <summary>
    /// The current directory as a path.
    /// </summary>
    public string CurrentDirectory
    {
        get => _explorer.CurrentDirectory;
        set
        {
            SetProperty(_explorer.CurrentDirectory, value, _explorer, (e, s) => e.CurrentDirectory = s);
            RefreshItems();
        }
    }
   
    /// <summary>
    /// Toggles hidden files on and off.
    /// </summary>
    [RelayCommand]
    private void ToggleHidden()
    {
        var options = Options.Instance;
        options.ShowHidden = !options.ShowHidden;
        
        RefreshItems();
    }
   
    /// <summary>
    /// Selects a file in the file viewer.
    /// Bound to <c>DirectoryItemViewModel.SelectedCommand</c>
    /// </summary>
    /// <param name="item">Item to select.</param>
    private void SelectItem(DirectoryItem item)
    {
        if (item.Kind == DirectoryItemKind.Directory)
        {
            _explorer.EnterSubdirectory(item.Name);
            OnPropertyChanged(nameof(CurrentDirectory));
            RefreshItems();
            return;
        }

        // TODO: file opening
    }

    /// <summary>
    /// SelectedCommand to navigate the file structure.
    /// </summary>
    /// <param name="direction">Which "direction" to navigate.</param>
    [RelayCommand]
    private void Navigate(NavigationDirection direction)
    {
        _explorer.Navigate(direction);
        RefreshItems();
    }

    private void RefreshItems()
    {
        // Items.Clear();
        OnPropertyChanged(nameof(CurrentDirectory));
        Items = new(_explorer.EnumerateItems().Select(CreateItemVM));
    }

    /// <summary>
    /// Factory method to create a <c>DirectoryItemViewModel</c>.
    /// </summary>
    private DirectoryItemViewModel CreateItemVM(DirectoryItem item)
    {
        var command = new RelayCommand(() => SelectItem(item));
        return new DirectoryItemViewModel(item, command);
    }
}