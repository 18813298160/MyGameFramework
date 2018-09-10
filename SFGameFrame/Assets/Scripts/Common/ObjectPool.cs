using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ObjectPool<T> where T : new()
{
    private readonly Stack<T> _stack = new Stack<T>();

    private readonly Action<T> _actionOnGet;

    private readonly Action<T> _actionOnRecycle;

    public int count { get; private set; }
    public int activeCount { get { return count - inactiveCount; } }
    public int inactiveCount { get { return _stack.Count; } }

    public ObjectPool(Action<T> actionOnGet, Action<T> actionOnRecycle)
    {
        _actionOnGet = actionOnGet;
        _actionOnRecycle = actionOnRecycle;
    }

    public T Get()
    {
        T element;
        if (_stack.Count == 0)
        {
            element = new T();
            count++;
        }
        else
        {
            element = _stack.Pop();
        }
        if (_actionOnGet != null)
            _actionOnGet(element);
        return element;
    }

    public void Recycle(T element)
    {
        if (_stack.Count > 0 && ReferenceEquals(_stack.Peek(), element))
        {
            throw new Exception("Internal error. Trying to destroy object that is already released to pool.");
        }

        if (_actionOnRecycle != null)
        {
            _actionOnRecycle(element);
        }
        _stack.Push(element);
    }

    public void Clear()
    {
        _stack.Clear();
        count = 0;
    }
}

public static class ListObjectPool<T>
{
    private static readonly ObjectPool<List<T>> _listPool = new ObjectPool<List<T>>(null, l => l.Clear());

    public static List<T> Get()
    {
        return _listPool.Get();
    }

    public static void Recycle(List<T> element)
    {
        _listPool.Recycle(element);
    }
}


// 简单的可自增长的对象池
public class IncreasObjectPool<T> where T : class, new()
{
    private int _preInstNum = 40;    // 预加载数量
    private int _increaseNum = 20;   // 增长数量

    private int _allocCount = 0;     // 总共生成的对象个数
    private List<T> _objects = new List<T>(40);

    public IncreasObjectPool(int preInstNum = 40, int increaseNum = 20)
    {
        _preInstNum = preInstNum;
        _increaseNum = increaseNum;

        if (_preInstNum <= 0)
            _preInstNum = 40;

        if (_increaseNum <= 0)
            _increaseNum = 20;

        Enlarge(_preInstNum);
    }

    // 释放
    public void Clean()
    {
        for (int i = 0; i < _objects.Count; ++i)
        {
            _objects[i] = null;
        }
        _objects.Clear();
        _allocCount = 0;
    }

    // 获取空闲对象
    public T Alloc()
    {

        if (_objects.Count == 0)
        {
            Enlarge(_increaseNum);
        }

        T obj = _objects[_objects.Count - 1];
        if (obj == null)
        {
            Debug.LogError("IncreasObjectPool alloc object is null");
            obj = new T();
        }
        _objects.RemoveAt(_objects.Count - 1);
        return obj;
    }

    // 归还对象
    public void Free(T obj)
    {
        if (obj != null)
        {
            _objects.Add(obj);
        }
    }

    // 增加一部分空闲对象
    void Enlarge(int num)
    {
        for (int i = 0; i < num; ++i)
        {
            T temp = new T();
            _objects.Add(temp);
        }
        _allocCount += num;
    }

    public bool Contain(T obj)
    {
        for (int i = 0; i < _objects.Count; ++i)
        {
            if (obj == _objects[i])
            {
                return true;
            }
        }
        return false;
    }

    public void DebugFreeCount()
    {
        string name = typeof(T).ToString();
        if (_allocCount == _objects.Count)
        {
            Debug.Log(name + ", " + _allocCount);
        }
        else
        {
            Debug.LogError(name + ", " + _allocCount + ", " + _objects.Count);
        }
    }
    public int GetFreeCount()
    {
        return _objects.Count;
    }
}
