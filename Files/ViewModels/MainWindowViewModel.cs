using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Files.Models;
using Microsoft.VisualBasic.FileIO;

namespace Files.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
 
    [ObservableProperty]
    private ObservableCollection<DirectoryItemViewModel> _items = [];
    private Explorer _explorer;

    public MainWindowViewModel()
    {
        _explorer = new Explorer();
        RefreshItems();
    }

    public int Columns { get; set; } = 10;

    private void SelectItem(DirectoryItem item)
    {
        if (item.Kind == DirectoryItemKind.Directory)
        {
            _explorer.EnterSubdirectory(item.Name);
            RefreshItems();
            return;
        }

        // TODO: file opening
    }

    public void RefreshItems()
    {
        // Items.Clear();
        Items = new(_explorer.EnumerateItems().Select(CreateItemVM));
    }

    private DirectoryItemViewModel CreateItemVM(DirectoryItem item)
    {
        var command = new RelayCommand(() => SelectItem(item));
        return new DirectoryItemViewModel(item, command);
    }
}