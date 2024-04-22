# Nebula.UI
Nebula.UI is a BepInEx plugin that loads modded UI elements for House of the Dying Sun.
# Building
This repo may require a change to where the game DLL is stored.
# Using as a library
> NOTE: You may, optionally, create an AssetBundle with a UIButton prefab.

> NOTE: If your button depends on code from another mod, please add a `BepInDependency` to that mod in your plugin so that yours loads after it. This mod does not work well with cyclic dependencies.

Nebula comes pre-packed with an event to detect when a scene is completely ready to spawn objects in, whereas Unity's built-in `SceneManager.onSceneReady` triggers before. This is useful for loading objects from an AssetBundle, as well as spawning new GameObjects.
1. At the top of your plugin script, add using directives for `Nebula` and `Nebula.UI`.
1. Create a method called `OnSceneReady` in your plugin class with an argument for `Scene`.
1. Inside this method:
	1. Add the line `NebulaPlugin.SceneReady -= OnSceneReady`.
	1. Load your AssetBundle(s) here, if you have any.
	1. Create one or more `ButtonFactoryDatum`s with:
		- The name of your `UIButton`
		- The text inside it
		- Any functions to trigger on click
		- The font size
		- The button size
		- The menu path, relative to "# CUI_2D/Camera" in the scene
        - The button container path, relative to the menu root
		- Any optional detail settings
		
		Then, use `ButtonFactory.Create` or `ButtonFactory.CreateMultiple` to create them.
		> This step is only if you want to create the button programmatically. If you wish to have a prefab, you may load it from an AssetBundle.
    1. Create a `ButtonDatum` for every `UIButton` you have with the menu's root path, as well as:
        - The button's priority in the list (0 is most important, higher numbers are less)
        - Optionally, MOTD information (with a path relative to the menu root)
		> There is a Select-like helper function since System.Linq does not come packaged with the game. Include `Nebula.Utils` in your script, and use `[your button list].Remap` to iteratively turn the list into button data.
	1. Add the new button data to `ButtonSpawner.buttonQueue` to queue them for spawning.
1. In the `Awake` method of your plugin, add the line `NebulaPlugin.SceneReady += OnSceneReady`.

You should now have your node(s) spawn in the game with the connections you specified.