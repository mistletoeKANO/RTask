using System;
using System.Collections.Generic;

namespace RTask
{
    public static class RTaskHelper
    {
        private class CoroutineBlocker
        {
            private int count;

            private List<RTask> tcss = new List<RTask>();

            public CoroutineBlocker(int count)
            {
                this.count = count;
            }

            public async RTask WaitAsync()
            {
                --this.count;
                if (this.count < 0)
                {
                    return;
                }
                if (this.count == 0)
                {
                    List<RTask> t = this.tcss;
                    this.tcss = null;
                    foreach (RTask ttcs in t)
                    {
                        ttcs.SetResult();
                    }

                    return;
                }
                RTask tcs = RTask.Create(true);

                tcss.Add(tcs);
                await tcs;
            }
        }

        public static async RTask<bool> WaitAny<T>(RTask<T>[] tasks, RCancellationToken cancellationToken = null)
        {
            if (tasks.Length == 0)
            {
                return false;
            }

            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(2);

            foreach (RTask<T> task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            async RVoid RunOneTask(RTask<T> task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }

            await coroutineBlocker.WaitAsync();

            if (cancellationToken == null)
            {
                return true;
            }

            return !cancellationToken.IsCancel();
        }

        public static async RTask<bool> WaitAny(RTask[] tasks, RCancellationToken cancellationToken = null)
        {
            if (tasks.Length == 0)
            {
                return false;
            }

            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(2);

            foreach (RTask task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            async RVoid RunOneTask(RTask task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }

            await coroutineBlocker.WaitAsync();

            if (cancellationToken == null)
            {
                return true;
            }

            return !cancellationToken.IsCancel();
        }

        public static async RTask<bool> WaitAll<T>(RTask<T>[] tasks, RCancellationToken cancellationToken = null)
        {
            if (tasks.Length == 0)
            {
                return false;
            }

            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(tasks.Length + 1);

            foreach (RTask<T> task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            async RVoid RunOneTask(RTask<T> task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }

            await coroutineBlocker.WaitAsync();

            if (cancellationToken == null)
            {
                return true;
            }

            return !cancellationToken.IsCancel();
        }

        public static async RTask<bool> WaitAll<T>(List<RTask<T>> tasks, RCancellationToken cancellationToken = null)
        {
            if (tasks.Count == 0)
            {
                return false;
            }

            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(tasks.Count + 1);

            foreach (RTask<T> task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            async RVoid RunOneTask(RTask<T> task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }

            await coroutineBlocker.WaitAsync();

            if (cancellationToken == null)
            {
                return true;
            }

            return !cancellationToken.IsCancel();
        }

        public static async RTask<bool> WaitAll(RTask[] tasks, RCancellationToken cancellationToken = null)
        {
            if (tasks.Length == 0)
            {
                return false;
            }

            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(tasks.Length + 1);

            foreach (RTask task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            await coroutineBlocker.WaitAsync();

            async RVoid RunOneTask(RTask task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }

            if (cancellationToken == null)
            {
                return true;
            }

            return !cancellationToken.IsCancel();
        }

        public static async RTask<bool> WaitAll(List<RTask> tasks, RCancellationToken cancellationToken = null)
        {
            if (tasks.Count == 0)
            {
                return false;
            }

            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(tasks.Count + 1);

            foreach (RTask task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            await coroutineBlocker.WaitAsync();

            async RVoid RunOneTask(RTask task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }

            if (cancellationToken == null)
            {
                return true;
            }

            return !cancellationToken.IsCancel();
        }
    }
}