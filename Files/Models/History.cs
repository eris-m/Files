using System.Collections.Generic;

namespace Files.Models;

// Maybe implement ICollection or similar
/// <summary>
/// The history of items visited.
///
/// Whenever a new directory is visited, <c>Add</c> should be called.
/// </summary>
/// <param name="size">The amount of items contained in the history.</param>
public sealed class History(int size)
{
    public const int DefaultSize = 8;

    private string?[] _buffer = new string[size];
    private int _index = 0;

    public History() : this(DefaultSize)
    {
    }

    /// <summary>
    /// Add a new item to the history.
    /// </summary>
    /// <remarks>
    /// Discards old items if the buffer is full.
    /// Can also discard items if the history is moved back (with <c>GetBack()</c>) and then an item is added.
    /// Will do nothing if the path is a duplicate to the last path added.
    /// </remarks>
    /// <param name="path">The path to add.</param>
    public void Add(string path)
    {
        // discard stale items
        if (_index == _buffer.Length - 1)
        {
            ShiftAll();
        }
        else
        {
            ClearRest();
        }

        // discard duplicate
        if (_buffer[_index] == path)
        {
            return;
        }

        _buffer[_index] = path;
        _index = int.Min(_index + 1, _buffer.Length - 1);
    }

    /// <summary>
    /// Moves the history back one step.
    /// </summary>
    /// <returns>The last path visited, visited if there is no last path.</returns>
    public string? GetBack()
    {
        if (_index < 2)
            return null;
        
        var item = _buffer[_index - 2];
        _index = int.Max(_index - 1, 1);

        return item;
    }

    /// <summary>
    /// Moves the history forward.
    /// Does nothing unless <c>GetBack</c> has been called.
    /// </summary>
    /// <returns>The next path in the history, or null if there is none.</returns>
    public string? GetForward()
    {
        if (_index < _buffer.Length - 1)
        {
            return _buffer[_index++];
        }

        return null;
    }

    /// <summary>
    /// Changes the amount of items in the history.
    /// </summary>
    public void Resize(int newSize)
    {
        var newBuffer = new string?[newSize];
        for (var i = 0; i < int.Min(_buffer.Length, newBuffer.Length); i++)
        {
            newBuffer[i] = _buffer[i];
        }

        _buffer = newBuffer;
        _index = int.Min(_index, newBuffer.Length - 1);
    }

    private void ShiftAll()
    {
        for (var i = 1; i < _buffer.Length; i++)
        {
            _buffer[i - 1] = _buffer[i];
        }
    }

    private void ClearRest()
    {
        for (var i = _index; i < _buffer.Length; i++)
        {
            _buffer[i] = null;
        }
    }
}
