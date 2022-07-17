using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTask
{
    public interface IItem
    {
        bool isDone { get; set; }
        int duration { get; set; }
        void Update();
        void SetResult();
    }
    public interface IPlayerLoopItem : IItem
    {
        RTask rts { get; set; }
    }

    public interface IPlayerActionItem : IItem
    {
        Action action { get; set; }
    }

    public class ItemPool
    {
        private static readonly Dictionary<Type, Queue<IItem>> ItemPools = new Dictionary<Type, Queue<IItem>>();

        public static bool TryGetFromPool(Type itemType, out IItem item)
        {
            item = default;
            if (!ItemPools.TryGetValue(itemType, out var queue)) return false;
            if (queue.Count == 0) return false;
            item = queue.Dequeue();
            return true;
        }

        public static void Recycle(IItem item)
        {
            Type itemType = item.GetType();
            if (!ItemPools.TryGetValue(itemType, out var queue))
            {
                queue = new Queue<IItem>();
                ItemPools.Add(itemType, queue);
            }
            item.isDone = false;
            queue.Enqueue(item);
        }
    }

    public class NextFrameItem : IPlayerLoopItem
    {
        private NextFrameItem(){}
        public static NextFrameItem Create()
        {
            if (!ItemPool.TryGetFromPool(typeof(NextFrameItem), out var item))
                item = new NextFrameItem();
            var cur = (NextFrameItem) item;
            cur.duration = 1;
            cur.rts = RTask.Create(true);
            return cur;
        }

        public bool isDone { get; set; }
        public int duration { get; set; }
        public void Update()
        {
            this.isDone = true;
        }

        public void SetResult()
        {
            this.rts.SetResult();
            ItemPool.Recycle(this);
        }

        public RTask rts { get; set; }
    }

    public class EndOfFrameItem : IPlayerLoopItem
    {
        private EndOfFrameItem(){}
        public static EndOfFrameItem Create()
        {
            if (!ItemPool.TryGetFromPool(typeof(EndOfFrameItem), out var item))
                item = new EndOfFrameItem();
            var cur = (EndOfFrameItem) item;
            cur.rts = RTask.Create(true);
            return cur;
        }

        public bool isDone { get; set; }
        public int duration { get; set; }
        public void Update() { this.isDone = true; }
        public void SetResult()
        {
            this.rts.SetResult();
            ItemPool.Recycle(this);
        }
        public RTask rts { get; set; }
    }

    public class DelayFrameItem : IPlayerLoopItem
    {
        private DelayFrameItem(){}
        public static DelayFrameItem Create(int frame)
        {
            if (!ItemPool.TryGetFromPool(typeof(DelayFrameItem), out var item))
                item = new DelayFrameItem();
            var cur = (DelayFrameItem) item;
            cur.duration = frame;
            cur.rts = RTask.Create(true);
            if (frame <= 0) cur.isDone = true;
            return cur;
        }

        public bool isDone { get; set; }
        public int duration { get; set; }
        public void Update()
        {
            if (this.duration > 0) this.duration--;
            if (this.duration <= 0) this.isDone = true;
        }

        public void SetResult()
        {
            this.rts.SetResult();
            ItemPool.Recycle(this);
        }

        public RTask rts { get; set; }
    }

    public class DelayItem : IPlayerLoopItem
    {
        private DelayItem(){}
        public static DelayItem Create(int milliseconds, bool unscaledTime)
        {
            if (!ItemPool.TryGetFromPool(typeof(DelayItem), out var item))
                item = new DelayItem();
            var cur = (DelayItem) item;
            cur.unscaledTime = unscaledTime;
            cur.duration = milliseconds;
            cur.elapsed = 0f;
            cur.rts = RTask.Create(true);
            if (milliseconds <= 0) cur.isDone = true;
            return cur;
        }
        private bool unscaledTime;
        public bool isDone { get; set; }
        public int duration { get; set; }
        private float elapsed;
        public void Update()
        {
            if (this.elapsed < this.duration)
            {
                float addTime = this.unscaledTime ? UnityEngine.Time.unscaledDeltaTime * 1000 : Time.deltaTime * 1000;
                this.elapsed += addTime;
            }
            if (this.elapsed >= this.duration) this.isDone = true;
        }

        public void SetResult()
        {
            this.rts.SetResult();
            ItemPool.Recycle(this);
        }

        public RTask rts { get; set; }
    }

    public class DelayAction : IPlayerActionItem
    {
        private DelayAction(){}
        public static DelayAction Create(int milliseconds, Action action, bool unscaledTime)
        {
            if (!ItemPool.TryGetFromPool(typeof(DelayAction), out var item))
                item = new DelayAction();
            var cur = (DelayAction) item;
            cur.duration = milliseconds;
            cur.elapsed = 0f;
            cur.action = action;
            cur.unscaledTime = unscaledTime;
            if (milliseconds <= 0) cur.isDone = true;
            return cur;
        }
        private bool unscaledTime;
        public bool isDone { get; set; }
        public int duration { get; set; }
        private float elapsed;
        public void Update()
        {
            if (this.elapsed < this.duration)
            {
                float addTime = this.unscaledTime ? Time.unscaledDeltaTime * 1000 : Time.deltaTime * 1000;
                this.elapsed += addTime;
            }
            if (this.elapsed >= this.duration) this.isDone = true;
        }

        public void SetResult()
        {
            this.action?.Invoke();
            ItemPool.Recycle(this);
        }

        public Action action { get; set; }
    }

    public class DelayFrameAction : IPlayerActionItem
    {
        private DelayFrameAction(){}
        public static DelayFrameAction Create(int frame, Action action)
        {
            if (!ItemPool.TryGetFromPool(typeof(DelayFrameAction), out var item))
                item = new DelayFrameAction();
            var cur = (DelayFrameAction) item;
            cur.duration = frame;
            cur.action = action;
            if (frame <= 0) cur.isDone = true;
            return cur;
        }
        public bool isDone { get; set; }
        public int duration { get; set; }
        public void Update()
        {
            if (this.duration > 0) this.duration--;
            if (this.duration <= 0) this.isDone = true;
        }

        public void SetResult()
        {
            this.action?.Invoke();
            ItemPool.Recycle(this);
        }

        public Action action { get; set; }
    }
    
}