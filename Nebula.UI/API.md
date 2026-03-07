# Nebula.UI API
> NOTE: All classes are available under the `Nebula.UI` namespace. `UIFactory` provides extension methods, so they will automatically be applied to a given `GameObject`.

> NOTE: You may, optionally, create an AssetBundle with your NGUI elements if you do not need them to be created programatically. This API is not necessary if you already have an AssetBundle or some other means to create elements.

## Creating elements
Most elements are created through `UIFactory`'s extension methods on any game object, after the scene is ready. See [scene events](../API.md#scene-events) in the main API to subscribe to scene readiness.
- `CreateLabel`
- `CreateTextureBackground`
- `CreateButton`
- `AddTooltip`
- `CreateVerticalScrollBar`

## Getting fonts
There are many fonts available through the `StockFonts` static class; these are used throughout the game.
- `serifGothic`: ITC Serif Gothic
    - `Normal`
    - `Black`
    - `Bold`
    - `ExtraBold` (Header text for starmap and mission titles)
    - `Heavy`
    - `Light`
- `microgramma`: Microgramma
    - `Bold`
    - `BoldDynamic` (Used in most buttons, menu titles)
    - `BoldStatic`
    - `Med`
    - `MedDynamic` (Used in some subtitles)
    - `MedStatic`
- `blender`: Blender
    - `Bold` (Used in starmap and mission subtitles, main menu descriptions, and any place that needs "plain" text)
    - `Book`

## Creating buttons procedurally
In your plugin's "scene ready" function:
1. Find the object in the given scene you want to attach this `UIButton` to.
    > Most UI elements exist in `# CUI_2D/Camera/ROOT_Menus` and a menu of your choice. You can use `Nebula.Utils.GameObjectUtils` with its `GetMenu` function to make this process much easier, provided you reference `Nebula.Utils.dll` in your project.
1. Create a `UIButton` with `CreateButton`.
1. To properly name your `UIButton`s and set an MOTD, Create a `ButtonDatum` for every `UIButton` you have with the menu's root path, as well as:
    - The button's priority in the list (0 is most important, higher numbers are less; up to 3 digits)
    - Optionally, MOTD information (with a path relative to the menu root)
    > NOTE: System.Linq is available to turn the list of `UIButton`s into button data. The `LinqSubstitute` in `Nebula.Utils` is deprecated.
1. Add the new button data to `ButtonSpawner.buttonQueue` to queue them for spawning.

You should now have your button(s) spawn in game.