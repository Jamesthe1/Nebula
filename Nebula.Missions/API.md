# Nebula.Missions API
> NOTE: All classes are available under the `Nebula.Missions` namespace.

> NOTE: You will need to have a `SortieTemplate`. It is recommended that the game is decompiled first, and an AssetBundle is created and embedded with the assembly.
Optionally, if you want to bundle mission nodes into your AssetBundle, you may create a folder called "Nebula.Missions" in your workspace's "Plugins" folder and copy `NamedOverworldNode` into it.

## Creating missions
> NOTE: Though a new mission may be created through code, it is cumbersome and is often hard to debug. Therefore, it is recommended to create the prefab through an AssetBundle in a decompiled environment.

A guide exists on the [modding wikipedia](https://hotdsmodding.wiki.gg/wiki/Guide:Creating_a_Mission) to create missions from scratch.

## Creating nodes
> NOTE: If your nodes are reliant on those in another mod, ensure your plugin has a `BepInDependency` to said mod.

Starmap nodes are created and loaded after the scene is ready. See [scene events](../API.md#scene-events) in the main API to subscribe to scene readiness.

In your plugin's "scene ready" function:
1. Load your AssetBundle(s) here, if you have any.
1. Create one or more `NamedNodeDatum`s with the name of your `SortieTemplate`, a list of the GameObject names of the nodes you want this to connect to, and the name of a GameObject positioned where you want your node to be on the map. Then, use `NodeFactory.Create` or `NodeFactory.CreateMultiple` to create them.
    > This step is only if you do not have a `NamedOverworldNode` associated with your `SortieTemplate`(s).
1. Add the new nodes to `NodeSpawner.nodeQueue` to queue them for spawning.

You should now have your node(s) spawn in the game with the connections you specified.