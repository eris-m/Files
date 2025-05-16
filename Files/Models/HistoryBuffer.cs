using System;
using System.Collections.Generic;

namespace Files.Models;

// Maybe implement ICollection or similar
public sealed class HistoryBuffer(int size)
{
    public const int DefaultSize = 8;
    
    private string[] _buffer = new string[size];
    private int _index = 0;

    public HistoryBuffer() : this(DefaultSize)
    {
    }
    public bool IsEmpty => _index == 0;

    public IList<string> Items => _buffer;
    
    public void Add(string path)
    {
        if (_index == _buffer.Length)
        {
            ShiftAll();
        }

        _buffer[_index] = path;
        _index = int.Min(_index + 1, _buffer.Length - 1);
    }
    
    public string? GetLast()
    {
        if (IsEmpty)
            return null;

        return _buffer[_index];
    }

    public void Resize(int newSize)
    {
        var newBuffer = new string[newSize];
        for (var i = 0; i < int.Min(_buffer.Length, newBuffer.Length); i++)
        {
            newBuffer[i] = _buffer[i];
        }

        _buffer = newBuffer;
        _index = int.Min(_index, newBuffer.Length-1);
    }

    private void ShiftAll()
    {
        for (var i = 1; i < _buffer.Length; i++)
        {
            _buffer[i - 1] = _buffer[i];
        }
    }
}