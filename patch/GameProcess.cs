using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;
using Manager;
using MAI2.Util;
using Monitor;
using Process;
using UnityEngine;

namespace LongPressExit.Patch
{
    /// <summary>
    /// Restores the Space/KeypadPlus force-release shortcut removed from the target DLL.
    /// </summary>
    [HarmonyPatch(typeof(GameProcess), "OnUpdate")]
    public static class GameProcessPatch
    {
        private static readonly BindingFlags InstanceNonPublic = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly FieldInfo SequenceField = typeof(GameProcess).GetField("_sequence", InstanceNonPublic);
        private static readonly FieldInfo MonitorsField = typeof(GameProcess).GetField("_monitors", InstanceNonPublic);
        private static readonly FieldInfo MessageField = typeof(GameProcess).GetField("_message", InstanceNonPublic);
        private static readonly FieldInfo ContainerField = typeof(ProcessBase).GetField("container", InstanceNonPublic);
        private static readonly MethodInfo UpdateSubbMonitorDataMethod = typeof(GameProcess).GetMethod("UpdateSubbMonitorData", InstanceNonPublic);
        private static readonly MethodInfo SetReleaseMethod = typeof(GameProcess).GetMethod("SetRelease", InstanceNonPublic);
        private static readonly int PlaySequence = GetSequenceValue("Play");
        private static readonly int ReleaseSequence = GetSequenceValue("Release");

        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        public static void OnUpdatePostfix(GameProcess __instance)
        {
            if (__instance == null || GameManager.IsNoteCheckMode || !IsPlaying(__instance) || !IsForceReleaseKeyDown())
            {
                return;
            }

            try
            {
                GameMonitor[] monitors = MonitorsField.GetValue(__instance) as GameMonitor[];
                Message[] messages = MessageField.GetValue(__instance) as Message[];
                if (monitors == null || messages == null)
                {
                    return;
                }

                ProcessDataContainer container = ContainerField.GetValue(__instance) as ProcessDataContainer;
                for (int monitorIndex = 0; monitorIndex < monitors.Length; monitorIndex++)
                {
                    if (!Singleton<UserDataManager>.Instance.GetUserData((long)monitorIndex).IsEntry)
                    {
                        continue;
                    }

                    UpdateSubbMonitorDataMethod.Invoke(__instance, new object[] { monitorIndex });
                    container.processManager.SendMessage(messages[monitorIndex]);
                    Singleton<GamePlayManager>.Instance.SetSyncResult(monitorIndex, -1);
                }

                SetReleaseMethod.Invoke(__instance, null);
            }
            catch (Exception exception)
            {
                MelonLoader.MelonLogger.Error("LongPressExit force-release failed:");
                MelonLoader.MelonLogger.Error(exception.ToString());
            }
        }

        private static bool IsPlaying(GameProcess instance)
        {
            object sequence = SequenceField.GetValue(instance);
            int value = Convert.ToInt32(sequence);
            return value >= PlaySequence && value < ReleaseSequence;
        }

        private static bool IsForceReleaseKeyDown()
        {
            return ModPreferences.IsTrackSkipKeyDown();
        }

        private static int GetSequenceValue(string name)
        {
            System.Type enumType = typeof(GameProcess).GetNestedType("GameSequence", BindingFlags.NonPublic);
            return Convert.ToInt32(Enum.Parse(enumType, name));
        }
    }
}






