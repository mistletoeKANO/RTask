using System;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevel;

namespace RTask
{
    internal struct RTaskLoopRunnerUpdate { }
    internal struct RTaskLoopRunnerPostLateUpdate { }
    public static class PlayerLoopHelper
    {
        private static PlayerLoopRunner[] runners;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            if ( runners != null) return;
            
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            var copyList = playerLoop.subSystemList.ToArray();
            
            runners = new PlayerLoopRunner[2];
            InsertLoop(copyList, typeof(UnityEngine.PlayerLoop.Update), 0, typeof(RTaskLoopRunnerUpdate));
            InsertLoop(copyList, typeof(UnityEngine.PlayerLoop.PostLateUpdate), 1, typeof(RTaskLoopRunnerPostLateUpdate));

            playerLoop.subSystemList = copyList;
            PlayerLoop.SetPlayerLoop(playerLoop);

            RegisterEditorPLayerMode();
        }

        private static void InsertLoop(PlayerLoopSystem[] loopSystems, Type playerLoopType, int runnerIndex, Type runnerType, bool insertStart = true)
        {
            runners[runnerIndex] = new PlayerLoopRunner(runnerType);
            var fixedIndex = FindLoopSystemIndex(loopSystems, playerLoopType);
            var runnerLoop = new PlayerLoopSystem
            {
                type = runnerType,
                updateDelegate = runners[runnerIndex].Polling
            };
            var source = loopSystems[fixedIndex];
            var exceptOld = source.subSystemList
                .Where(ls => ls.type != runnerType)
                .ToArray();
            var dest = new PlayerLoopSystem[exceptOld.Length + 1];
            if (insertStart)
            {
                Array.Copy(exceptOld, 0, dest, 1, exceptOld.Length);
                dest[0] = runnerLoop;
            }
            else
            {
                Array.Copy(exceptOld, 0, dest, 0, exceptOld.Length);
                dest[dest.Length - 1] = runnerLoop;
            }
            loopSystems[fixedIndex].subSystemList = dest;
        }

        private static void ExistPlayMode()
        {
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            var copyList = playerLoop.subSystemList.ToArray();
            
            RemoveLoop(copyList, typeof(UnityEngine.PlayerLoop.Update), typeof(RTaskLoopRunnerUpdate));
            RemoveLoop(copyList, typeof(UnityEngine.PlayerLoop.PostLateUpdate), typeof(RTaskLoopRunnerPostLateUpdate));
            
            playerLoop.subSystemList = copyList;
            PlayerLoop.SetPlayerLoop(playerLoop);
        }

        private static void RemoveLoop(PlayerLoopSystem[] loopSystems, Type playerLoopType, Type runnerType)
        {
            var fixedIndex = FindLoopSystemIndex(loopSystems, playerLoopType);
            var source = loopSystems[fixedIndex];
            var exceptOld = source.subSystemList
                .Where(ls => ls.type != runnerType)
                .ToArray();
            loopSystems[fixedIndex].subSystemList = exceptOld;
        }

        private static void RegisterEditorPLayerMode()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged += (state) =>
            {
                if (state != UnityEditor.PlayModeStateChange.ExitingPlayMode) return;
                if (runners == null) return;
                foreach (var runner in runners)
                {
                    runner.Polling();
                    runner.CleanUp();
                }
                ExistPlayMode();
                runners = null;
            };
#endif
        }
        
        private static int FindLoopSystemIndex(PlayerLoopSystem[] playerLoopList, Type systemType)
        {
            for (int i = 0; i < playerLoopList.Length; i++)
            {
                if (playerLoopList[i].type == systemType)
                {
                    return i;
                }
            }
            throw new Exception("Target PlayerLoopSystem does not found. Type:" + systemType.FullName);
        }

        internal static void AddAction(IItem loopItem, Type runnerType)
        {
            int index = -1;
            for (int i = 0; i < runners.Length; i++)
            {
                if (runners[i].RunnerType != runnerType) continue;
                index = i;
                break;
            }
            if (index == -1)
            {
                Debug.LogError($"Runner {runnerType} not found!");
                return;
            }
            runners[index].AddAction(loopItem);
        }
    }
}