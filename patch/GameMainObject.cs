using System;
using AMDaemon;
using HarmonyLib;
using Main;
using Manager;
using UnityEngine;

namespace LongPressExit.Patch
{
    /// <summary>
    /// Restores the Select-button long-press exit that is absent from the target DLL.
    /// </summary>
    [HarmonyPatch(typeof(GameMainObject), "LateUpdate")]
    public static class GameMainObjectPatch
    {
        private const float LongPressSeconds = 10f;
        private const float PromptDelaySeconds = 2f;

        private static bool _isQuitting;
        private static bool _exitRequested;
        private static float _longPressStartTime;
        private static Texture2D _backgroundTexture;
        private static Texture2D _trackTexture;
        private static Texture2D _progressTexture;

        [HarmonyPostfix]
        public static void LateUpdatePostfix()
        {
            bool selectHeld = ModPreferences.IsLongPressButtonHeld();

            if (!selectHeld)
            {
                _isQuitting = false;
                _exitRequested = false;
                return;
            }

            if (!_isQuitting)
            {
                _isQuitting = true;
                _longPressStartTime = Time.time;
            }

            if (!_exitRequested && Time.time - _longPressStartTime >= LongPressSeconds)
            {
                _exitRequested = true;
                try
                {
                    Core.Kill(NextProcess.Auto);
                }
                catch (Exception exception)
                {
                    MelonLoader.MelonLogger.Warning("LongPressExit could not signal the next process: " + exception.Message);
                }
                finally
                {
                    Application.Quit();
                }
            }
        }

        /// <summary>
        /// Called from MelonMod.OnGUI because some target DLLs do not contain GameMainObject.OnGUI.
        /// </summary>
        public static void DrawExitPrompt()
        {
            if (!_isQuitting)
            {
                return;
            }

            float elapsed = Time.time - _longPressStartTime;
            if (elapsed < PromptDelaySeconds)
            {
                return;
            }

            EnsureTextures();

            string text = "继续按 " + Mathf.Max(0f, LongPressSeconds - elapsed).ToString("F1") + " 秒强行关闭游戏";
            float panelTop = Screen.height * 0.4375f;
            float panelHeight = Screen.height - panelTop;
            float panelWidth = panelHeight;
            float panelLeft = (Screen.width - panelWidth) / 2f;
            GUI.DrawTexture(new Rect(panelLeft, panelTop, panelWidth, panelHeight), _backgroundTexture);

            float textWidth = 500f * (Screen.height / 1920f);
            float textHeight = 50f * (Screen.height / 1920f);
            float textLeft = panelLeft + (panelWidth - textWidth) / 2f;
            float textTop = panelTop + (panelHeight - textHeight) / 2f;
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.RoundToInt(32f * (Screen.height / 1920f)),
                fontStyle = FontStyle.Bold
            };
            style.normal.textColor = Color.white;
            GUI.Label(new Rect(textLeft, textTop - 10f, textWidth, textHeight + 20f), text, style);

            float barWidth = 350f * (Screen.height / 1920f);
            float barHeight = 30f * (Screen.height / 1920f);
            float barLeft = panelLeft + (panelWidth - barWidth) / 2f;
            float barTop = textTop + textHeight + 30f * (Screen.height / 1920f);
            float progress = Mathf.Clamp01(elapsed / LongPressSeconds);
            GUI.DrawTexture(new Rect(barLeft, barTop, barWidth, barHeight), _trackTexture);
            GUI.DrawTexture(new Rect(barLeft, barTop, barWidth * progress, barHeight), _progressTexture);
            GUI.Box(new Rect(barLeft, barTop, barWidth, barHeight), GUIContent.none);
        }

        private static void EnsureTextures()
        {
            if (_backgroundTexture != null)
            {
                return;
            }

            _backgroundTexture = CreateTexture(new Color(0.2f, 0.2f, 0.2f, 0.8f));
            _trackTexture = CreateTexture(new Color(0.5f, 0.5f, 0.5f, 0.5f));
            _progressTexture = CreateTexture(Color.red);
        }

        private static Texture2D CreateTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
    }
}





