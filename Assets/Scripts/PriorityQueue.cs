using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///  简单的优先队列，只添加了需要的东西
/// </summary>
public class PriorityQueue
{
    private SortedList<float, List<Vector3>> mQueue;

    public PriorityQueue() { mQueue = new SortedList<float, List<Vector3>>(); }

    /// <summary>
    ///  入队列
    /// </summary>
    public void Enqueue(float key,Vector3 value)
    {
        if(!mQueue.ContainsKey(key))
        {
            mQueue.Add(key, new List<Vector3>());
        }
        mQueue[key].Add(value);
    }

    /// <summary>
    ///  出队列
    /// </summary>
    /// <returns></returns>
    public Vector3 Dequeue()
    {
        float key =  mQueue.Keys[0];
        Vector3 res = mQueue[key][0];

        if (mQueue[key].Count == 1)
        {
            mQueue.Remove(key);
        }
        else
        {
            mQueue[key].RemoveAt(0);
        }
        return res;
    }


    public bool Empty()
    {
        return mQueue.Count == 0;
    }


    public void Clear()
    {
        mQueue.Clear();
    }


    public int Size()
    {
        int res = 0;

        for(int i = 0; i < mQueue.Count; i++ )
        {
            float key = mQueue.Keys[i];
            for(int j = 0; j < mQueue[key].Count; j++)
            {
                res++;
            }
        }

        return res;
    }

}
