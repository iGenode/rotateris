using System.Collections.Generic;
using UnityEngine;

public abstract class PollingPool<T> where T : Component
{
    private readonly T prefab;

    private readonly Queue<T> pool = new();
    private readonly LinkedList<T> inuse = new();
    private readonly Queue<LinkedListNode<T>> nodePool = new();

    private int sizeLimit;
    private int lastCheckFrame = -1;

    protected PollingPool(T prefab, int sizeLimit)
    {
        this.prefab = prefab;
        this.sizeLimit = sizeLimit;
    }

    private void CheckInUse()
    {
        var node = inuse.First;
        while (node != null)
        {
            var current = node;
            // What's this?
            node = node.Next;

            if (!IsActive(current.Value))
            {
                current.Value.gameObject.SetActive(false);
                pool.Enqueue(current.Value);
                inuse.Remove(current);
                nodePool.Enqueue(current);
            }
        }
    }

    protected T Get()
    {
        T item;

        if (lastCheckFrame != Time.frameCount)
        {
            lastCheckFrame = Time.frameCount;
            CheckInUse();
        }

        if (pool.Count == 0)
        {
            if (inuse.Count <= sizeLimit)
            {
                item = Object.Instantiate(prefab);
            }
            else
            {
                item = nodePool.Count == 0 ? inuse.First.Value : nodePool.Dequeue().Value;
            }
        }
        else
        {
            item = pool.Dequeue();
        }

        if (nodePool.Count == 0)
        {
            inuse.AddLast(item);
        }
        else
        {
            var node = nodePool.Dequeue();
            node.Value = item;
            inuse.AddLast(node);
        }

        item.gameObject.SetActive(true);

        return item;
    }

    protected abstract bool IsActive(T component);
}
