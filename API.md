# Nebula API
> NOTE: This plugin is under the `Nebula` namespace. The class that these items are exposed under will always be `NebulaPlugin`.

## Scene events
Nebula provides an event that allows spawning objects and loading AssetBundles when the game loads the main scene, `NebulaPlugin.SceneReady`. This addresses the misnomer `SceneManager.sceneLoaded`, which only triggers *before* the scene is loaded. You can subscribe to it by:
1. Creating a method on any class that only takes in a `Scene` as an argument, preferably private and in your plugin class
1. Adding the function name to the event with `+=`, function name on the right hand side

You may now use this function to use any API dealing with `MonoBehaviour` components and `ScriptableObject` resources.

It is recommended that you check that the scene name is `"Workbench"`, because even though the base game will never change the scene, some mods may do so for their own reasons.