# Unity Plugin Manager
## Introduction
Unity Plugin Manager (herein, UPM) is a tool for creating plugins and mods for games developed with the Unity engine.

## How it works
UPM injects a static constructor into the GameObject class found within UnityEngine.dll. This way, it is able to execute code before anything else in the game. Using this, it loads plugins from a folder named "Plugins" in the game's folder, and instantiates any MonoBehaviour derivatives with the [OnGameInit] attribute.

## Building
In order to build, you will need a copy of UnityEngine.dll  
This file can be found in the _Managed_ folder of any Unity game, or within Unity itself.

If you don't have a built Unity game on-hand, you can retrieve UnityEngine.dll from the following location:

(Unity Install Folder)/Editor/Data/Managed/UnityEngine.dll

After obtaining this file, place it in the _Managed_ folder present with the UPM source code. Then, if on Windows, execute `generate.bat` to create project files.

If not on Windows, follow the Protobuild project generation instructions for your platform.

From here, building UPM should be as simple as compiling the generated solution file in your C# IDE of choice.

## Usage
After building UPM, you should have the following files (present in _Source/(Project Name)/.../bin_)

* PluginManager.Installer.exe
* PluginManager.dll
* Mono.Cecil.dll

Any other files are not necessary for usage of UPM.

Place the aforementioned files into the directory where your target game is located, and run `PluginManager.Installer.exe` (using Mono to do so, if not on Windows)

If UPM installed successfully, you should see a message stating `Plugin manager installed.`

Running the UPM installer again will uninstall UPM from the game.

You are now free to create a `Plugins` directory and place your plugins in there.

## Creating plugins
Refer to the [sample plugin](https://github.com/UnityPluginManager/SamplePlugin) as an example of how to set up a project. Essentially, you must create a .NET 3.5 library project that references UnityEngine, PluginManager (UPM) and any other assemblies you may require from your target game. The sample project can be used as a starting point (see the repository for more information).
