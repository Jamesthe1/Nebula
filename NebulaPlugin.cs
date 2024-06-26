﻿using System.Collections;
using BepInEx;
using Nebula.Missions;
using Nebula.UI;
using UnityEngine.SceneManagement;

namespace Nebula {
    [BepInPlugin (PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess ("dyingsun.exe")]
    public class NebulaPlugin : BaseUnityPlugin {
        /// <summary>
        /// A function that must trigger when our scene is fully loaded
        /// </summary>
        public delegate void SceneReadyDelegate (Scene scene);
        /// <summary>
        /// An event that must trigger when our scene is fully loaded
        /// </summary>
        public static event SceneReadyDelegate SceneReady;

        private void Awake () {
            Logger.LogInfo ($"Hello world!");
            Logger.LogInfo ($"Welcome to a nebula of possible log spam. Hang tight!");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded (Scene scene, LoadSceneMode mode) {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            StartCoroutine (InitAfterFrame (scene));    // Coroutine requirement explained in function
        }

        private IEnumerator InitAfterFrame (Scene scene) {
            yield return null;  // Wait 1 frame so that the scene can load fully
            Logger.LogInfo ($"Scene fully loaded. Calling for mods to add to the queues.");
            SceneReady?.Invoke (scene);
            NodeSpawner.InitQueue ();
            ButtonSpawner.InitQueue ();
            Logger.LogInfo ($"Initialization complete.");
        }
    }
}
