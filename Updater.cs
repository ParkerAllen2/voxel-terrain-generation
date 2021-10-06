using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Updater : Singleton<Updater>
{
    protected Updater() { }

    public override void Awake()
    {
        base.Awake();
    }

    Queue<ThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<ThreadInfo<MapData>>();
    Queue<ThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<ThreadInfo<MeshData>>();

    public void EnqueueMapDataInfo(Action<MapData> callback, MapData mapData)
    {
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new ThreadInfo<MapData>(callback, mapData));
        }
    }

    public void EnqueueMeshDataInfo(Action<MeshData> callback, MeshData meshData)
    {
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new ThreadInfo<MeshData>(callback, meshData));
        }
    }

    private void Update()
    {
        while (mapDataThreadInfoQueue.Count > 0)
        {
            ThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
            threadInfo.callback(threadInfo.parameter);
        }
        while (meshDataThreadInfoQueue.Count > 0)
        {
            ThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
            threadInfo.callback(threadInfo.parameter);
        }
    }

    struct ThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public ThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}
