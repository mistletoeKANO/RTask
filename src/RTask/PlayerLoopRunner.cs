using System;
using System.Collections.Generic;

namespace RTask
{
    internal sealed class PlayerLoopRunner
    {
        private readonly List<IItem> runningItems = new List<IItem>();
        private readonly Type runnerType;
        internal Type RunnerType => runnerType;
        internal PlayerLoopRunner(Type runnerType) { this.runnerType = runnerType; }

        internal void AddAction(IItem item)
        {
            if (item.isDone) item.SetResult();
            else runningItems.Add(item);
        }
        
        internal void Polling()
        {
            if (runningItems.Count == 0) return;
            if (runnerType == typeof(RTaskLoopRunnerUpdate))
            {
                foreach (var item in runningItems)
                    item.Update();
                for (int i = 0; i < runningItems.Count; i++)
                {
                    var item = runningItems[i];
                    if (!item.isDone) continue;
                    item.SetResult();
                    runningItems.Remove(item);
                }
            }
            else if (runnerType == typeof(RTaskLoopRunnerPostLateUpdate))
            {
                for (int i = 0; i < runningItems.Count; i++)
                {
                    var item = runningItems[i];
                    item.SetResult();
                    runningItems.Remove(item);
                }
            }
        }

        internal void CleanUp()
        {
            this.runningItems.Clear();
        }
    }
}