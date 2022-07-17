using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace RTask
{
    public enum AwaitState : byte
    {
        /// <summary>The operation has not yet completed.</summary>
        Pending = 0,

        /// <summary>The operation completed successfully.</summary>
        Succeeded = 1,

        /// <summary>The operation completed with an error.</summary>
        Faulted = 2,
    }
    
    [AsyncMethodBuilder(typeof (RAsyncTaskMethodBuilder))]
    public partial class RTask : ICriticalNotifyCompletion
    {
        public static Action<Exception> ExceptionHandler;
        private static RTask completedTask;
        public static RTask CompletedTask => completedTask ??= new RTask {state = AwaitState.Succeeded};

        private static readonly Queue<RTask> queue = new Queue<RTask>();

        /// <summary>
        /// 请不要随便使用ETTask的对象池，除非你完全搞懂了ETTask!!!
        /// 假如开启了池,await之后不能再操作ETTask，否则可能操作到再次从池中分配出来的ETTask，产生灾难性的后果
        /// SetResult的时候请现将tcs置空，避免多次对同一个ETTask SetResult
        /// </summary>
        public static RTask Create(bool fromPool = false)
        {
            if (!fromPool)
            {
                return new RTask();
            }
            
            if (queue.Count == 0)
            {
                return new RTask() {fromPool = true};    
            }
            return queue.Dequeue();
        }

        private void Recycle()
        {
            if (!this.fromPool)
            {
                return;
            }
            
            this.state = AwaitState.Pending;
            this.callback = null;
            queue.Enqueue(this);
            // 太多了，回收一下
            if (queue.Count > 1000)
            {
                queue.Clear();
            }
        }

        private bool fromPool;
        private AwaitState state;
        private object callback; // Action or ExceptionDispatchInfo

        private RTask() { }
        
        [DebuggerHidden]
        private async RVoid InnerCoroutine() { await this; }

        [DebuggerHidden]
        public void Coroutine()
        {
            InnerCoroutine().Coroutine();
        }

        [DebuggerHidden]
        public RTask GetAwaiter() { return this; }

        public bool IsCompleted
        {
            [DebuggerHidden]
            get => this.state != AwaitState.Pending;
        }

        [DebuggerHidden]
        public void UnsafeOnCompleted(Action action)
        {
            if (this.state != AwaitState.Pending)
            {
                action?.Invoke();
                return;
            }

            this.callback = action;
        }

        [DebuggerHidden]
        public void OnCompleted(Action action)
        {
            this.UnsafeOnCompleted(action);
        }

        [DebuggerHidden]
        public void GetResult()
        {
            switch (this.state)
            {
                case AwaitState.Succeeded:
                    this.Recycle();
                    break;
                case AwaitState.Faulted:
                    ExceptionDispatchInfo c = this.callback as ExceptionDispatchInfo;
                    this.callback = null;
                    this.Recycle();
                    c?.Throw();
                    break;
                default:
                    throw new NotSupportedException("ETTask does not allow call GetResult directly when task not completed. Please use 'await'.");
            }
        }

        [DebuggerHidden]
        public void SetResult()
        {
            if (this.state != AwaitState.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            this.state = AwaitState.Succeeded;

            Action c = this.callback as Action;
            this.callback = null;
            c?.Invoke();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public void SetException(Exception e)
        {
            if (this.state != AwaitState.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            this.state = AwaitState.Faulted;

            Action c = this.callback as Action;
            this.callback = ExceptionDispatchInfo.Capture(e);
            c?.Invoke();
        }
    }

    [AsyncMethodBuilder(typeof (RAsyncTaskMethodBuilder<>))]
    public class RTask<T> : ICriticalNotifyCompletion
    {
        private static readonly Queue<RTask<T>> queue = new Queue<RTask<T>>();
        
        /// <summary>
        /// 请不要随便使用ETTask的对象池，除非你完全搞懂了ETTask!!!
        /// 假如开启了池,await之后不能再操作ETTask，否则可能操作到再次从池中分配出来的ETTask，产生灾难性的后果
        /// SetResult的时候请现将tcs置空，避免多次对同一个ETTask SetResult
        /// </summary>
        public static RTask<T> Create(bool fromPool = false)
        {
            if (!fromPool)
            {
                return new RTask<T>();
            }
            
            if (queue.Count == 0)
            {
                return new RTask<T>() { fromPool = true };    
            }
            return queue.Dequeue();
        }
        
        private void Recycle()
        {
            if (!this.fromPool)
            {
                return;
            }
            this.callback = null;
            this.value = default;
            this.state = AwaitState.Pending;
            queue.Enqueue(this);
            // 太多了，回收一下
            if (queue.Count > 1000)
            {
                queue.Clear();
            }
        }

        private bool fromPool;
        private AwaitState state;
        private T value;
        private object callback; // Action or ExceptionDispatchInfo

        private RTask() { }

        [DebuggerHidden]
        private async RVoid InnerCoroutine() { await this; }

        [DebuggerHidden]
        public void Coroutine()
        {
            InnerCoroutine().Coroutine();
        }

        [DebuggerHidden]
        public RTask<T> GetAwaiter()
        {
            return this;
        }

        [DebuggerHidden]
        public T GetResult()
        {
            switch (this.state)
            {
                case AwaitState.Succeeded:
                    T v = this.value;
                    this.Recycle();
                    return v;
                case AwaitState.Faulted:
                    ExceptionDispatchInfo c = this.callback as ExceptionDispatchInfo;
                    this.callback = null;
                    this.Recycle();
                    c?.Throw();
                    return default;
                default:
                    throw new NotSupportedException("RTask does not allow call GetResult directly when task not completed. Please use 'await'.");
            }
        }


        public bool IsCompleted
        {
            [DebuggerHidden]
            get => state != AwaitState.Pending;
        } 

        [DebuggerHidden]
        public void UnsafeOnCompleted(Action action)
        {
            if (this.state != AwaitState.Pending)
            {
                action?.Invoke();
                return;
            }

            this.callback = action;
        }

        [DebuggerHidden]
        public void OnCompleted(Action action)
        {
            this.UnsafeOnCompleted(action);
        }

        [DebuggerHidden]
        public void SetResult(T result)
        {
            if (this.state != AwaitState.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            this.state = AwaitState.Succeeded;

            this.value = result;

            Action c = this.callback as Action;
            this.callback = null;
            c?.Invoke();
        }
        
        [DebuggerHidden]
        public void SetException(Exception e)
        {
            if (this.state != AwaitState.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            this.state = AwaitState.Faulted;

            Action c = this.callback as Action;
            this.callback = ExceptionDispatchInfo.Capture(e);
            c?.Invoke();
        }
    }
}