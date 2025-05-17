using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Files.Models;

/// <summary>
/// The "direction" of file navigation.
/// </summary>
public enum NavigationDirection
{
    /// <summary>
    /// Forward through the history.
    /// </summary>
    Forward,
    /// <summary>
    /// Backward through the history.
    /// </summary>
    Back,
    /// <summary>
    /// Up a directory.
    /// </summary>
    Up
};

/// <summary>
/// The type of directory item.
/// </summary>
public enum DirectoryItemKind
{
    Directory,
    File,
}

/// <summary>
/// A directory item, either a file or directory.
/// </summary>
/// <param name="name">The name of the item.</param>
/// <param name="kind">The kind of the item.</param>
public class DirectoryItem(string name, DirectoryItemKind kind)
{
    public string Name { get; set; } = name;
    public DirectoryItemKind Kind { get; set; } = kind;
}

/// <summary>
/// The main file explorer model.
/// Contains most of the logic for the app.
/// </summary>
public class Explorer
{
    private string _currentDirectory = "";

    /// <summary>
    /// Creates a new <c>Explorer</c> in the given directory.
    /// </summary>
    /// <param name="directory">The directory to start at.</param>
    public Explorer(string directory)
    {
        CurrentDirectory = directory;
    }

    /// <summary>
    /// Creates an explorer in the home directory.
    /// </summary>
    public Explorer() : this(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
    {
    }

    /// <summary>
    /// The current directory (as a path) of the explorer.
    /// </summary>
    public string CurrentDirectory {
        get => _currentDirectory;
        set
        {
            _currentDirectory = value;
            History.Add(_currentDirectory);
        }
    }

    /// <summary>
    /// The history of paths visited.
    /// </summary>
    public HistoryBuffer History { get; } = new HistoryBuffer();
   
    /// <summary>
    /// Enter a subdirectory of the current directory.
    /// </summary>
    /// <param name="path">The subdirectory to enter.</param>
    public void EnterSubdirectory(string path)
    {
        CurrentDirectory = Path.Combine(CurrentDirectory, path);
    }

    /// <summary>
    /// Perform a navigation. Either forwards/backwards through the history or up the directory.
    /// </summary>
    /// <param name="direction">The "direction" to navigate.</param>
    public void Navigate(NavigationDirection direction)
    {
        switch (direction)
        {
            case NavigationDirection.Up:
                CurrentDirectory = Path.GetDirectoryName(CurrentDirectory) ?? CurrentDirectory;
                break;
            case NavigationDirection.Forward:
                var forward = History.GetForward();
                _currentDirectory = forward ?? CurrentDirectory;
                break;
            case NavigationDirection.Back:
                var back = History.GetBack();
                _currentDirectory = back ?? CurrentDirectory;
                break;
            default: break;
        }

    }

    /// <summary>
    /// Enumerates the items within the current directory.
    /// </summary>
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
   
    /// <summary>
    /// Factory to create a folder item.
    /// </summary>
    /// <param name="fullPath">The full path of the item.</param>
    private static DirectoryItem CreateFolderItem(string fullPath)
    {
        var name = Path.GetFileName(fullPath);
        return new DirectoryItem(name, DirectoryItemKind.Directory);
    }

    /// <summary>
    /// Factory to create a file item.
    /// </summary>
    /// <param name="fullPath">The full path of the item.</param>
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

        // should probably handle any exceptions from File.GetAttributes()
        return elements.Where(s => (File.GetAttributes(s) & FileAttributes.Hidden) == 0);
    }
}
