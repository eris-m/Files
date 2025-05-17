using System.Collections.Generic;

namespace Files.Models;

// Maybe implement ICollection or similar
public sealed class HistoryBuffer(int size)
{
    public const int DefaultSize = 8;

    private string?[] _buffer = new string[size];
    private int _index = 0;

    public HistoryBuffer() : this(DefaultSize)
    {
    }

    public void Add(string path)
    {
        if (_index == _buffer.Length - 1)
        {
            ShiftAll();
        }
        else
        {
            ClearRest();
        }

        if (_buffer[_index] == path)
        {
            return;
        }

        _buffer[_index] = path;
        _index = int.Min(_index + 1, _buffer.Length - 1);
    }

    public string? GetBack()
    {
        if (_index < 2)
            return null;
        
        var item = _buffer[_index - 2];
        _index = int.Max(_index - 1, 1);

        return item;
    }

    public string? GetForward()
    {
        if (_index < _buffer.Length - 1)
        {
            return _buffer[_index++];
        }

        return null;
    }

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
