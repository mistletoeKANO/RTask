using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;

namespace RTask
{
    public struct RAsyncTaskMethodBuilder
    {
        private RTask tcs;

        // 1. Static Create method.
        [DebuggerHidden]
        public static RAsyncTaskMethodBuilder Create()
        {
            RAsyncTaskMethodBuilder builder = new RAsyncTaskMethodBuilder() { tcs = RTask.Create(true) };
            return builder;
        }

        // 2. TaskLike Task property.
        [DebuggerHidden]
        public RTask Task => this.tcs;

        // 3. SetException
        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            this.tcs.SetException(exception);
        }

        // 4. SetResult
        [DebuggerHidden]
        public void SetResult()
        {
            this.tcs.SetResult();
        }

        // 5. AwaitOnCompleted
        [DebuggerHidden]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        // 6. AwaitUnsafeOnCompleted
        [DebuggerHidden]
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        // 7. Start
        [DebuggerHidden]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        // 8. SetStateMachine
        [DebuggerHidden]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }
    }

    public struct RAsyncTaskMethodBuilder<T>
    {
        private RTask<T> tcs;

        // 1. Static Create method.
        [DebuggerHidden]
        public static RAsyncTaskMethodBuilder<T> Create()
        {
            RAsyncTaskMethodBuilder<T> builder = new RAsyncTaskMethodBuilder<T>() { tcs = RTask<T>.Create(true) };
            return builder;
        }

        // 2. TaskLike Task property.
        [DebuggerHidden]
        public RTask<T> Task => this.tcs;

        // 3. SetException
        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            this.tcs.SetException(exception);
        }

        // 4. SetResult
        [DebuggerHidden]
        public void SetResult(T ret)
        {
            this.tcs.SetResult(ret);
        }

        // 5. AwaitOnCompleted
        [DebuggerHidden]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        // 6. AwaitUnsafeOnCompleted
        [DebuggerHidden]
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        // 7. Start
        [DebuggerHidden]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        // 8. SetStateMachine
        [DebuggerHidden]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }
    }
}