using System;
using System.Collections.Generic;
using System.Linq;

using PoolObject = System.Object;

namespace Assets.oojjrs.Script
{
    public class ObjectPool
    {
        private class Entry
        {
            public bool Allocated { get; set; }
            public DateTime LatestTime { get; set; }
            public PoolObject UnityObject { get; set; }
        }

        public int AllocatedCount { get; private set; }
        private Func<PoolObject> Allocator { get; }
        public bool AutoScaling { get; }
        private int ChunkSize { get; }
        public int Count => Objects.Count;
        private Action<PoolObject> Deallocator { get; }
        private List<Entry> Objects { get; }

        public ObjectPool(Func<PoolObject> allocator, Action<PoolObject> deallocator)
            : this(allocator, deallocator, 512)
        {
        }

        public ObjectPool(Func<PoolObject> allocator, Action<PoolObject> deallocator, int chunkSize)
            : this(allocator, deallocator, chunkSize, true)
        {
        }

        public ObjectPool(Func<PoolObject> allocator, Action<PoolObject> deallocator, int chunkSize, bool autoScaling)
        {
            Allocator = allocator;
            AutoScaling = autoScaling;
            ChunkSize = chunkSize;
            Deallocator = deallocator;
            Objects = new(chunkSize);
        }

        public void AllocatePage()
        {
            for (int i = 0; i < ChunkSize; ++i)
            {
                Objects.Add(new()
                {
                    UnityObject = Allocator(),
                });
            }
        }

        public void Clear()
        {
            if (Deallocator != default)
            {
                foreach (var obj in Objects)
                    Deallocator(obj.UnityObject);
            }
            Objects.Clear();
        }

        public PoolObject GetObject()
        {
            var reqCount = 1;
            if (AllocatedCount + reqCount > Count)
            {
                if (AutoScaling)
                {
                    while (Count - AllocatedCount < reqCount)
                        AllocatePage();
                }
                else
                {
                    throw new OutOfMemoryException();
                }
            }

            var entry = Objects.FirstOrDefault(t => t.Allocated == false);
            if (entry == default)
                throw new InvalidProgramException();

            entry.Allocated = true;
            entry.LatestTime = DateTime.Now;

            ++AllocatedCount;
            return entry.UnityObject;
        }

        public PoolObject[] GetObjects(int reqCount)
        {
            if (reqCount <= 0)
                throw new IndexOutOfRangeException();

            if (AllocatedCount + reqCount > Count)
            {
                if (AutoScaling)
                {
                    while (Count - AllocatedCount < reqCount)
                        AllocatePage();
                }
                else
                {
                    throw new OutOfMemoryException();
                }
            }

            var entries = Objects.Where(t => t.Allocated == false).Take(reqCount);
            if (entries.Count() != reqCount)
                throw new InvalidProgramException();

            var now = DateTime.Now;
            foreach (var entry in entries)
            {
                entry.Allocated = true;
                entry.LatestTime = now;
            }

            AllocatedCount += reqCount;
            return entries.Select(t => t.UnityObject).ToArray();
        }

        public void ReleaseObjects(params PoolObject[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            foreach (var value in values)
            {
                var entry = Objects.FirstOrDefault(t => t.UnityObject == value);
                if (entry != null)
                {
                    entry.Allocated = false;
                    --AllocatedCount;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void TrimExcess()
        {
            Objects.RemoveAll(t => t.Allocated == false);
            Objects.TrimExcess();
        }
    }
}