using System;

namespace RTask
{
    public partial class RTask
    {
        public static RTask Delay(int milliseconds, bool unscaledTime = false)
        {
            var res = DelayItem.Create(milliseconds, unscaledTime);
            PlayerLoopHelper.AddAction(res, typeof(RTaskLoopRunnerUpdate));
            return res.rts;
        }

        public static RTask DelayFrame(int delayFrame)
        {
            var res = DelayFrameItem.Create(delayFrame);
            PlayerLoopHelper.AddAction(res, typeof(RTaskLoopRunnerUpdate));
            return res.rts;
        }

        public static RTask NextFrame()
        {
            var res = NextFrameItem.Create();
            PlayerLoopHelper.AddAction(res, typeof(RTaskLoopRunnerUpdate));
            return res.rts;
        }
        
        public static RTask EndOfFrame()
        {
            var res = EndOfFrameItem.Create();
            PlayerLoopHelper.AddAction(res, typeof(RTaskLoopRunnerPostLateUpdate));
            return res.rts;
        }

        public static void DelayFrameAction(int frame, Action action)
        {
            var res = global::RTask.DelayFrameAction.Create(frame, action);
            PlayerLoopHelper.AddAction(res, typeof(RTaskLoopRunnerUpdate));
        }
        
        public static void DelayAction(int milliseconds, Action action, bool unscaledTime = false)
        {
            var res = global::RTask.DelayAction.Create(milliseconds, action, unscaledTime);
            PlayerLoopHelper.AddAction(res, typeof(RTaskLoopRunnerUpdate));
        }
    }
}