using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Files.Models;

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

public class Explorer(string directory)
{
    public Explorer() : this(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
    {
    }

    public string CurrentDirectory { get; set; } = directory;

    public void EnterSubdirectory(string path)
    {
        CurrentDirectory = Path.Combine(CurrentDirectory, path);
    }

    public void UpDirectory()
    {
        CurrentDirectory = Path.GetDirectoryName(CurrentDirectory) ?? CurrentDirectory;
    }

    public IEnumerable<DirectoryItem> EnumerateItems()
    {
        var directories = Directory
            .EnumerateDirectories(CurrentDirectory)
            .Select(CreateFolderItem);
    
        var files = Directory
            .EnumerateFiles(CurrentDirectory)
            .Select(CreateFileItem);
    
        return directories.Concat(files);
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