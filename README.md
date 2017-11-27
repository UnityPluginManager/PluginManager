# Unity Plugin Manager
## Introduction
Unity Plugin Manager (herein, UPM) is a tool for creating plugins and mods for games developed with the Unity engine.

## How it works
UPM injects a static constructor into the GameObject class found within the UnityEngine namespace. This way, it is able to execute code before anything else in the game. Using this, it loads plugins from a folder named "Plugins" in the game's folder, and instantiates any MonoBehaviour derivatives with the [OnGameInit] attribute.

## Compiling
In order to build UPM or create plugins, you will need the [.NET Core SDK](https://www.microsoft.com/net/core) and at least one of the following:

* An installation of Unity 5.x or 2017.x
* A game developed in Unity 5.x or 2017.x

Within the UPM repository is a directory named `Managed.UnityEngine`. This directory needs to contain specific files from one of the aforementioned prerequisites.

### Option A: Unity Installation
1. Browse to your Unity installation directory (usually `C:\Program Files\Unity [version]\Editor`)
2. Browse to `Data\Managed` and copy `UnityEngine.dll` to the `Managed.UnityEngine\Managed` directory in the UPM repository
4. If it exists, browse to `Data\Managed\UnityEngine` and copy everything _except_ UnityEngine.dll to the `Managed.UnityEngine\Managed` directory in the UPM repository
5. Browse to `Data\UnityExtensions\Unity\GUISystem\Standalone` and copy `UnityEngine.UI.dll` to the `Managed.UnityEngine\Managed` directory in the UPM repository

### Option B: Game
1. Browse to the game's `Managed` directory (usually inside the game's `Data` folder)
2. Copy every file with `UnityEngine` in its name to the `Managed.UnityEngine\Managed` directory in the UPM repository

---

You should now be able to compile UPM by either using `Build.All.bat`, `PluginManager.sln` or manually calling `dotnet build` in the appropriate directories.

## Usage
After building UPM, you should have _at least_ the following files:

* PluginManager.Setup.exe (`PluginManager.Setup/bin/...`)
* PluginManager.Core.dll (`PluginManager.Core/bin/...`)
* Mono.Cecil.dll (`PluginManager.Setup/bin/...`)

Any other files are not necessary for usage of UPM.

Place the aforementioned files into the directory where your target game is located, and run `PluginManager.Setup.exe` (using Mono to do so, if not on Windows)

If UPM installed successfully, you should see a message stating `UPM installed.`

You are now free to create a `Plugins` directory and place your plugins in there.

## Creating plugins
The creation of plugins is a relatively simple process.

1. Make a new folder for your target game in the `Managed.Games` folder, if one does not exist
2. Put the `Assembly-CSharp.dll`, `Assembly-UnityScript.dll` and/or `Assembly-Boo.dll` files from your target game in that folder, if they are present in its `Managed` folder
3. Make a new folder for your plugin in the `Plugin.Projects` folder
4. Copy one of the templates into that folder (NET46 is only for Unity 2017 projects using the new Mono runtime!)
5. Edit Config.props to point to your target game

You can compile your plugin by opening a command prompt in its folder and entering `dotnet build`.

Refer to the [sample plugin](https://github.com/UnityPluginManager/Universal.SamplePlugin) as an example of how to get started.
