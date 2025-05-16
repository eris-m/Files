using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    [ObservableProperty]
    private int _columns = 10;
    private Explorer _explorer;

    public MainWindowViewModel()
    {
        _explorer = new Explorer();
        RefreshItems();
    }

    public string CurrentDirectory
    {
        get => _explorer.CurrentDirectory;
        set
        {
            SetProperty(_explorer.CurrentDirectory, value, _explorer, (e, s) => e.CurrentDirectory = s);
            RefreshItems();
        }
    }
    
    [RelayCommand]
    private void ToggleHidden()
    {
        var options = Options.Instance;
        options.ShowHidden = !options.ShowHidden;
        
        RefreshItems();
    }
    
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

    [RelayCommand]
    private void UpDirectory()
    {
        _explorer.UpDirectory();
        OnPropertyChanged(nameof(CurrentDirectory));
        RefreshItems();
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