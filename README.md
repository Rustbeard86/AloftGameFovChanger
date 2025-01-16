# AloftGameFovChanger
## This project is not in any way associated with the developers of Aloft game.
This plugin may have a negative affect on your game, game save or even PC.
I've done my very best to make sure issues don't arise, but I and the developers and publishing company for Aloft game will not be help responsible
if issues arise.

## Notes for users new to modding.
bepinex is a widely used plugin modding framework for unity games.
When you start your game a console window will appear, that is part of bepinex this can be disabled if you wish through a configuration option.

In the config file located "\BepInEx\config\BepInEx.cfg" change (Enabled = true) to (Enabled = false)

```
[Logging.Console]

## Enables showing a console for log output.
# Setting type: Boolean
# Default value: true
Enabled = true
```
## Notes on bugs / issues and current limitations

a.) As of now I have limited the FOV changer to between 60 and 90 FOV inclusive. (Things start looking very strage and unpleasant beyond these.)

b.) When adjusting the FOV, sometimes the FOV appears to snap or pop into the new FOV, I'm looking into this and have no fix for now.

c.) FOV values 74 and 76 appear to stay stuck in place and not correctly transition to 75, (Not a very big deal, but if I find time I'll investigate further)

If you experience any bugs, please do not post on the steam forums or the developers other means of communications in regard to this.

Modding is not as of now officially supported.

Please ask your questions here by posting an issue or contacting me on steam.

(I will provide some support when I am available so please be patient.)

## Usage:
1.) Download the latest release from https://builds.bepinex.dev/projects/bepinex_be (BepInEx Unity (Mono) for Windows (x64) games)

2.) Extract the zip archive named something similar to "BepInEx-Unity.Mono-win-x64-6.0.0-be.733+995f049.zip"
    You will get a folder with the same name.
    Navigate into the folder and copy all of the folders contents.
    Locate where you installed Aloft
    Paste the files you copied previously into the root folder of the game. ( This folder will contain Aloft.exe, Aloft_Data'folder'... etc.)
    Keep this folder open. (We need to copy the plugin into in just a moment.)

3.) Once you have copied all the files and folders into your game folder, start the game once and let it load all the way to the screen where you can exit, and then exit the game.

4.) Downlaod the latest release of the plugin from https://github.com/Rustbeard86/AloftGameFovChanger/releases/latest
    Copy this plugin and paste it into "\BepInEx\plugins\" where your game is installed.

5. Once the game loads again, a new config file will be created with options you can change in "\BepInEx\config\AloftFovChanger.cfg" (This file can be edited with notepad or any other text editor.)

The important settinsgs should be good for most users.
F10 to toggle the OSD (On Screen Display) of current FOV
+ and - both standard (above ENTER key) and numpad. To increase and decrease FOV in increments of 5
  If you hold the shift key while pressing + or - the FOV will adjust in increments of 1.

The plugin will save your desired FOV when adjusted (to the config file), you do not need to set your FOV when starting the game again.

## Uninstallation / Removal of bepinex and the plugin.
Simply remove the following files and folder from your game directory, and if you wish, can you re-verify your files through steam.
```
"Folder" [BepInEx]

"File" [.doorstop_version]

"File" [changelog.txt]

"File" [doorstop_config.ini]

"File" [winhttp.dll]
```

## Config options.
```configuration
## Settings file was created by plugin Bepinex plugin to modify FOV in Aloft game. v1.0.0
## Plugin GUID: AloftFovChanger

[Debug Settings]

## Enable debug mode
# Setting type: Boolean
# Default value: false
EnableDebug = false

[FOV Settings]

## Default FOV value
# Setting type: Single
# Default value: 60
DefaultFov = 60.999

## User-adjusted FOV value
# Setting type: Single
# Default value: 80
AdjustedFov = 90

[Key Bindings]

## Key to increase FOV
# Setting type: String
# Default value: equals
IncreaseFovKey = equals

## Key to decrease FOV
# Setting type: String
# Default value: minus
DecreaseFovKey = minus

## Numpad key to increase FOV
# Setting type: String
# Default value: numpadPlus
IncreaseFovKeyNumpad = numpadPlus

## Numpad key to decrease FOV
# Setting type: String
# Default value: numpadMinus
DecreaseFovKeyNumpad = numpadMinus

## Modifier key for fine adjustments
# Setting type: String
# Default value: leftShift
ModifierKey = leftShift

## Key to toggle OSD display
# Setting type: String
# Default value: f10
ToggleOSDKey = f10

[OSD Settings]

## Always display OSD
# Setting type: Boolean
# Default value: false
AlwaysDisplayOSD = false
```
