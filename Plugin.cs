﻿using System.Collections;
using BepInEx;
using Nebula.Missions;
using UnityEngine.SceneManagement;

namespace Nebula {
    [BepInPlugin (PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess ("dyingsun.exe")]
    public class Plugin : BaseUnityPlugin {
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
            NodeSpawner.Init (scene);
            Logger.LogInfo ($"Initialization complete.");
        }
    }
}
