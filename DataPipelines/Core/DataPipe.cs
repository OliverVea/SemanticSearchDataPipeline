using System.Collections.Concurrent;

namespace DataPipelines.Core;

public class DataPipe<T>
{
    public int Id { get; } = PipeCounter.I++;
    private readonly ConcurrentQueue<T> _queue = new();
    public bool IsEmpty => _queue.IsEmpty;
    public int Count => _queue.Count;
    
    public void Enqueue(T item)
    {
        _queue.Enqueue(item);
    }
    
    public void Enqueue(IEnumerable<T> items)
    {
        foreach (var item in items) Enqueue(item);
    }
    
    public IReadOnlyCollection<T> Dequeue(int count)
    {
        var items = new List<T>();
        for (var i = 0; i < count; i++)
        {
            if (!_queue.TryDequeue(out var item)) break;
            items.Add(item);
        }
        return items;
    }
}

public static class PipeCounter
{
    public static int I;
}