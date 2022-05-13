# Solasta Community Expansion

This is a collection of work from the Solasta modding community. It includes multiclass, feats, classes, subclasses, items, crafting recipes, gameplay options, UI improvements, and more. The general philosophy is everything is optional to enable, so you can install the mod and then enable the pieces you want. There are some minor bug fixes that are enabled by default.

The goal with the modifications provided here is to minimize code patches. Minimizing code patches means compatibility with other mods is easier as well as makes maintenance easier. Any code patches should be as targeted and minimal as possible to reduce the chance of unintended side effects. There are a lot of interesting things that can be done with just edits to the database.

# How to contribute

Do you have a mod you want to see included here? We are happy to take new contributions! The best way to get involved is to:

1. Make a branch off of the `Dev` branch, name it something related to the changes you are making.
2. Commit your edits to your branch.
3. Make sure to test your changes. This is a good opportunity to take screen shots so you can show off the changes.
4. Make a pull request to merge your branch into `Dev`. You can help this process by sharing screen shots to show off the change as well as including an english description of what the change does.
5. Wait for a review. We try to stay on top of this, but it's a hobby.
6. Once the review is done, the changes will get merged in to the `Dev` branch. The `Dev` branch will periodically be tested and merged in to `master` to build releases.

# How to Compile

0. Install all required development pre-requisites:
	- [Visual Studio 2019 Community Edition](https://visualstudio.microsoft.com/downloads/)
	- [.NET "Current" x86 SDK](https://dotnet.microsoft.com/download/visual-studio-sdks)
1. Download and install [Unity Mod Manager (UMM)](https://www.nexusmods.com/site/mods/21)
2. Execute UMM, Select Solasta, and Install
3. Create the environment variable *SolastaInstallDir* and point it to your Solasta game home folder
	- tip: search for "edit the system environment variables" on windows search bar
4. Use "Install Release" or "Install Debug" to have the Mod installed directly to your Game Mods folder

NOTE Unity Mod Manager and this mod template make use of [Harmony](https://go.microsoft.com/fwlink/?linkid=874338)

# How to Debug

1. Open Solasta game folder
	* Rename UnityPlayer.dll to UnityPlayer.dll.original
	* Add below entries to *Solasta_Data\boot.config*:
		```
		wait-for-managed-debugger=1
		player-connection-debug=1
		```
2. Download and install [7zip](https://www.7-zip.org/a/7z1900-x64.exe)
3. Download [Unity Editor 2019.4.32](https://download.unity3d.com/download_unity/f88bf0bee961/Windows64EditorInstaller/UnitySetup64-2019.4.32f1.exe)
4. Open Downloads folder
	* Right-click UnitySetup64-2019.4.32f1.exe, 7Zip -> Extract Here
	* Navigate to Editor\Data\PlaybackEngines\windowsstandalonesupport\Variations\win64_development_mono
		* Copy *UnityPlayer.dll* and *WinPixEventRuntime.dll* to clipboard
	* Navigate to the Solasta game folder
		* Rename *UnityPlayer.dll* to *UnityPlayer.dll.original*
		* Paste *UnityPlayer.dll* and *WinPixEventRuntime.dll* from clipboard
5. You can now attach the Unity Debugger from Visual Studio 2019, Debug -> Attach Unity Debug