using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using AloftCamera;
using Level_Manager;
using System.Reflection;
using System.Collections;
using UnityEngine.InputSystem;
using BepInEx.Unity.Mono;
using System;
using UnityEngine.InputSystem.Controls;
#pragma warning disable BepInEx002

namespace AloftFovChanger
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal new static ManualLogSource Logger;
        private static ConfigEntry<string> _increaseFovKey;
        private static ConfigEntry<string> _decreaseFovKey;
        private static ConfigEntry<string> _increaseFovKeyNumpad;
        private static ConfigEntry<string> _decreaseFovKeyNumpad;
        private static ConfigEntry<string> _modifierKey;
        private static ConfigEntry<string> _toggleOsdKey;
        private static ConfigEntry<bool> _enableDebug;
        private static ConfigEntry<bool> _alwaysDisplayOsd;
        private static ConfigEntry<float> _defaultFov;
        private static ConfigEntry<float> _adjustedFov;
        private static float _targetFov;
        private static bool _shouldOverrideFov;
        private static bool _showOsd = true; // Initial OSD state
        private float _lastAdjustmentTime; // Time of the last FOV adjustment
        private const float OsdDisplayDuration = 2f; // Duration to display OSD after adjustment
        private const float MaxFov = 90f; // Maximum FOV limit
        private const float MinFov = 60f; // Minimum FOV limit
        private FovManager _fovManager;
        private PropertyInfo _currentFovProperty;
        private float _flashStartTime; // Time when the flash started
        private const float FlashDuration = 0.5f; // Duration of the flash in seconds

        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            // Create Harmony instance
            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            // Define configuration settings for keybindings
            _increaseFovKey = Config.Bind("Key Bindings", "IncreaseFovKey", "equals", "Key to increase FOV");
            _decreaseFovKey = Config.Bind("Key Bindings", "DecreaseFovKey", "minus", "Key to decrease FOV");
            _increaseFovKeyNumpad = Config.Bind("Key Bindings", "IncreaseFovKeyNumpad", "numpadPlus", "Numpad key to increase FOV");
            _decreaseFovKeyNumpad = Config.Bind("Key Bindings", "DecreaseFovKeyNumpad", "numpadMinus", "Numpad key to decrease FOV");
            _modifierKey = Config.Bind("Key Bindings", "ModifierKey", "leftShift", "Modifier key for fine adjustments");
            _toggleOsdKey = Config.Bind("Key Bindings", "ToggleOSDKey", "f10", "Key to toggle OSD display");

            // Define configuration settings for FOV and debug mode
            _defaultFov = Config.Bind("FOV Settings", "DefaultFov", 60f, "Default FOV value");
            _adjustedFov = Config.Bind("FOV Settings", "AdjustedFov", Mathf.Clamp(_defaultFov.Value, MinFov, MaxFov), "User-adjusted FOV value");
            _enableDebug = Config.Bind("Debug Settings", "EnableDebug", false, "Enable debug mode");
            _alwaysDisplayOsd = Config.Bind("OSD Settings", "AlwaysDisplayOSD", false, "Always display OSD");

            // Start a coroutine to find the FovManager instance
            StartCoroutine(FindFovManager());
        }

        private IEnumerator FindFovManager()
        {
            while (FindObjectOfType<FovManager>() == null)
            {
                Logger.LogWarning("FovManager instance not found. Retrying...");
                yield return new WaitForSeconds(1);
            }

            _fovManager = FindObjectOfType<FovManager>();
            if (_fovManager == null) yield break;
            _currentFovProperty = _fovManager.GetType().GetProperty("CurrentFov", BindingFlags.Public | BindingFlags.Instance);
            if (_currentFovProperty == null) yield break;
            // Set initial target FOV from game code
            var gameDefaultFov = (float)_currentFovProperty.GetValue(_fovManager);

            if (Math.Abs(_defaultFov.Value - gameDefaultFov) > 0.001f)
            {
                // Update config with the game's default FOV if it has changed
                _defaultFov.Value = gameDefaultFov;
            }

            _targetFov = Mathf.Clamp(_adjustedFov.Value, MinFov, MaxFov); // Clamp the loaded FOV value
            Logger.LogInfo($"Initial FOV set to {_targetFov}");
        }

        // ReSharper disable once UnusedMember.Local
        private void Update()
        {
            if (GameplayState.CurrentState != GameplayState.GameState.InGame)
            {
                return;
            }

            var increment = GetKeyControl(_modifierKey.Value).isPressed ? 1f : 5f;

            if (GetKeyControl(_increaseFovKey.Value).wasPressedThisFrame || GetKeyControl(_increaseFovKeyNumpad.Value).wasPressedThisFrame)
            {
                AdjustFov(increment);
            }
            else if (GetKeyControl(_decreaseFovKey.Value).wasPressedThisFrame || GetKeyControl(_decreaseFovKeyNumpad.Value).wasPressedThisFrame)
            {
                AdjustFov(-increment);
            }

            if (GetKeyControl(_toggleOsdKey.Value).wasPressedThisFrame)
            {
                _showOsd = !_showOsd;
                Logger.LogInfo($"OSD display toggled to: {_showOsd}");
            }

            if (_enableDebug.Value)
            {
                LogDebugInfo();
            }
        }

        private KeyControl GetKeyControl(string keyName)
        {
            // Retrieve KeyControl based on the key name
            return (KeyControl)Keyboard.current[keyName];
        }

        private void AdjustFov(float increment)
        {
            _targetFov = Mathf.Clamp(_targetFov + increment, MinFov, MaxFov); // Clamp the FOV value
            Logger.LogInfo($"FOV adjusted to {_targetFov}");
            Debug.Log($"[AloftFovChanger] FOV adjusted to {_targetFov}");

            // Save the adjusted FOV to the config file
            _adjustedFov.Value = _targetFov;

            if (_targetFov is MinFov or MaxFov)
            {
                _flashStartTime = Time.time; // Start the flash timer
            }

            _lastAdjustmentTime = Time.time; // Update the last adjustment time

            _shouldOverrideFov = true;
        }

        private void LogDebugInfo()
        {
            if (_fovManager == null || _currentFovProperty == null) return;
            var currentFov = (float)_currentFovProperty.GetValue(_fovManager);
            var mainCamera = Camera.main;

            Logger.LogDebug($"Current FOV: {currentFov}");
            Logger.LogDebug($"Camera Field of View: {mainCamera.fieldOfView}");
            Logger.LogDebug($"Camera Aspect Ratio: {mainCamera.aspect}");
            Logger.LogDebug($"Screen Width: {Screen.width}");
            Logger.LogDebug($"Screen Height: {Screen.height}");
        }

        [HarmonyPatch(typeof(FovManager), "CustomUpdate")]
        private class FovManagerCustomUpdatePatch
        {
            // ReSharper disable once InconsistentNaming
            // ReSharper disable once UnusedMember.Local
            // ReSharper disable once UnusedParameter.Local
            private static void Postfix(FovManager __instance, float dt)
            {
                if (!_shouldOverrideFov || GameplayState.CurrentState != GameplayState.GameState.InGame)
                {
                    return;
                }

                var currentFovProperty = typeof(FovManager).GetProperty("CurrentFov", BindingFlags.Public | BindingFlags.Instance);
                if (currentFovProperty == null) return;
                currentFovProperty.SetValue(__instance, _targetFov);
                // Ensure no other dynamic updates conflict with our custom FOV
                __instance.SetFpsCamFov(_targetFov);
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void OnGUI()
        {
            if (GameplayState.CurrentState != GameplayState.GameState.InGame || (!_alwaysDisplayOsd.Value && Time.time - _lastAdjustmentTime > OsdDisplayDuration))
            {
                return;
            }

            var fovText = $"Current FOV: {_targetFov}";
            switch (_targetFov)
            {
                case MinFov:
                    fovText += " (Minimum)";
                    break;
                case MaxFov:
                    fovText += " (Maximum)";
                    break;
            }

            var fovStyle = new GUIStyle(GUI.skin.label);
            fovStyle.normal.textColor = Time.time - _flashStartTime < FlashDuration ? Color.red : // Flash red text
                Color.white; // Normal text color

            GUI.Label(new Rect(10, 10, 200, 20), fovText, fovStyle);
        }
    }
}
