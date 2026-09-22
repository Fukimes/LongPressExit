using HarmonyLib;
using MelonLoader;
using LongPressExit.Patch;

namespace LongPressExit
{
    public static class BuildInfo
    {
        public const string Name = "LongPressExit";
        public const string Description = "为原版DLL添加长按退出功能，并支持按键TrackSkip。";
        public const string Author = "Fukimes";
        public const string Version = "1.0.0";
    }

    public sealed class LongPressExitMod : MelonMod
    {
        private const string HarmonyId = "com.fukimes.longpressexit";
        private bool _initialized;

        public override void OnInitializeMelon()
        {
            if (_initialized)
            {
                return;
            }

            MelonLogger.Msg("[" + BuildInfo.Name + "] Initializing...");
            try
            {
                ModPreferences.Initialize();
            }
            catch (System.Exception exception)
            {
                MelonLogger.Error("[" + BuildInfo.Name + "] Preferences initialization: FAILED");
                MelonLogger.Error(exception.ToString());
                return;
            }

            bool mainPatch = ApplyPatch(typeof(GameMainObjectPatch), "GameMainObject.LateUpdate");
            bool processPatch = ApplyPatch(typeof(GameProcessPatch), "GameProcess.OnUpdate");
            MelonLogger.Msg("[" + BuildInfo.Name + "] Patch status: GameMainObject="
                + Status(mainPatch) + ", GameProcess=" + Status(processPatch));
            _initialized = mainPatch || processPatch;
        }

        public override void OnGUI()
        {
            GameMainObjectPatch.DrawExitPrompt();
        }

        private static bool ApplyPatch(System.Type patchType, string target)
        {
            try
            {
                HarmonyLib.Harmony.CreateAndPatchAll(patchType, HarmonyId);
                MelonLogger.Msg("[" + BuildInfo.Name + "] Patch " + target + ": OK");
                return true;
            }
            catch (System.Exception exception)
            {
                MelonLogger.Error("[" + BuildInfo.Name + "] Patch " + target + ": FAILED");
                MelonLogger.Error(exception.ToString());
                return false;
            }
        }

        private static string Status(bool success)
        {
            return success ? "OK" : "FAILED";
        }
    }
}
