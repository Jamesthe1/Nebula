# Nebula.Missions
Originally named MissionLoader, Nebula.Missions is a BepInEx plugin that loads modded missions for House of the Dying Sun.
# Building
This repo may require a change to where the game DLL is stored.
# Using as a library
> NOTE: You will need to have a `SortieTemplate`. It is recommended that the game is decompiled first and an AssetBundle is created.
Optionally, if you want to bundle mission nodes into your AssetBundle, you may create a folder called "Nebula.Missions" in your workspace's "Plugins" folder and copy `NamedOverworldNode` into it.

> NOTE: If your node depends on a mission from another mod, please add a `BepInDependency` to that mod in your plugin so that yours loads after it. This mod does not work well with cyclic dependencies.

Nebula comes pre-packed with an event to detect when a scene is completely ready to spawn objects in, whereas Unity's built-in `SceneManager.onSceneReady` triggers before. This is useful for loading objects from an AssetBundle, as well as spawning new GameObjects.
1. At the top of your plugin script, add using directives for `Nebula` and `Nebula.Missions`.
1. Create a method called `OnSceneReady` in your plugin class with an argument for `Scene`.
1. Inside this method:
	1. Add the line `NebulaPlugin.SceneReady -= OnSceneReady`.
	1. Load your AssetBundle(s) here, if you have any.
	1. Create one or more `NamedNodeDatum`s with the name of your `SortieTemplate`, a list of the GameObject names of the nodes you want this to connect to, and the name of a GameObject positioned where you want your node to be on the map. Then, use `NodeFactory.Create` or `NodeFactory.CreateMultiple` to create them.
		> This step is only if you do not have a `NamedOverworldNode` associated with your `SortieTemplate`(s).
	1. Add the new nodes to `NodeSpawner.nodeQueue` to queue them for spawning.
1. In the `Awake` method of your plugin, add the line `NebulaPlugin.SceneReady += OnSceneReady`.

You should now have your node(s) spawn in the game with the connections you specified.