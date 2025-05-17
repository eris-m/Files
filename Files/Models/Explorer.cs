using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Files.Models;

public enum NavigationDirection
{
    Forward,
    Back,
    Up
};

public enum DirectoryItemKind
{
    Directory,
    File,
}

public class DirectoryItem(string name, DirectoryItemKind kind)
{
    public string Name { get; set; } = name;
    public DirectoryItemKind Kind { get; set; } = kind;
}

public class Explorer
{
    private string _currentDirectory = "";

    public Explorer(string directory)
    {
        CurrentDirectory = directory;
    }

    public Explorer() : this(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
    {
    }

    public string CurrentDirectory {
        get => _currentDirectory;
        set
        {
            _currentDirectory = value;
            History.Add(_currentDirectory);
        }
    }

    public HistoryBuffer History { get; } = new HistoryBuffer();
    
    public void EnterSubdirectory(string path)
    {
        CurrentDirectory = Path.Combine(CurrentDirectory, path);
    }

    public void Navigate(NavigationDirection direction)
    {
        switch (direction)
        {
            case NavigationDirection.Up:
                CurrentDirectory = Path.GetDirectoryName(CurrentDirectory) ?? CurrentDirectory;
                break;
            case NavigationDirection.Forward:
                var forward = History.GetForward();
                CurrentDirectory = forward ?? CurrentDirectory;
                break;
            case NavigationDirection.Back:
                CurrentDirectory = History.GetBack() ?? CurrentDirectory;
                break;
        }

    }

    public IEnumerable<DirectoryItem> EnumerateItems()
    {
        try
        {
            var directories = Directory
                .EnumerateDirectories(CurrentDirectory)
                .StripHidden()
                .Select(CreateFolderItem);

            var files = Directory
                .EnumerateFiles(CurrentDirectory)
                .StripHidden()
                .Select(CreateFileItem);

            return directories.Concat(files);
        } 
        catch (DirectoryNotFoundException ex)
        {
            //TODO!
            return [];
        }
    }
    
    private static DirectoryItem CreateFolderItem(string fullPath)
    {
        var name = Path.GetFileName(fullPath);
        return new DirectoryItem(name, DirectoryItemKind.Directory);
    }

    private static DirectoryItem CreateFileItem(string fullPath)
    {
        var name = Path.GetFileName(fullPath);
        return new DirectoryItem(name, DirectoryItemKind.File); 
    }
}

file static class StripExtension 
{
     /// <summary>
    /// Strips hidden files and folders if `Options.ShowHidden` is true.
    /// </summary>
    public static IEnumerable<string> StripHidden(this IEnumerable<string> elements)
    {
        if (!Options.Instance.ShowHidden)
            return elements;

        return elements.Where(s => (File.GetAttributes(s) & FileAttributes.Hidden) == 0);
    }
}
