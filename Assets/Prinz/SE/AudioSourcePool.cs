using Steamworks;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePool
{
    private readonly Transform parent;
    private readonly Queue<AudioSource> pool = new();
    private readonly LinkedList<AudioSource> active = new();
    private readonly int maxSounds;

    public AudioSourcePool(int initialSize, Transform parent, int maxPoolSize)
    {
        this.parent = parent;
        this.maxSounds = maxPoolSize;

        for (int i = 0; i < initialSize; i++)
            pool.Enqueue(CreateSource());
        this.maxSounds = maxPoolSize;
    }

    private AudioSource CreateSource()
    {
        var go = new GameObject("PooledAudioSource");
        go.transform.SetParent(parent);
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        return src;
    }

    public AudioSource Get()
    {
        if(pool.Count > 0)
        {
            var src = pool.Dequeue();
            active.AddLast(src);
            return src;
        }

        if (active.Count >= maxSounds)
        {
            //steal oldest SE extreme overlapping
            var stolen = active.First.Value;
            active.RemoveFirst();
            stolen.Stop();
            active.AddLast(stolen);
            return stolen;
        }

        var created = CreateSource();
        active.AddLast(created);
        return created;
    }

    public void Release(AudioSource src)
    {
        active.Remove(src);
        pool.Enqueue(src);
    }
}
