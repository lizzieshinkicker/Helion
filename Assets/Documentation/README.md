# Installation

1. Extract the contents of this archive to a directory of your choice.
2. Obtain one or more of the official IWADs (Doom, Doom II, Plutonia, TNT, etc.).  If you've bought the games via Steam, we'll try to automatically find them.  If that doesn't work, you can also (PICK ONE):
    1. Copy the IWADs to the same directory as Helion
    2. Run Helion.exe (or just "./Helion", on Linux) with the -iwad parameter, followed by the path to the IWAD you want to use
    3. Configure your Doom launcher of choice to pass the -iwad parameter
    4. Set values for the `DOOMWADDIR` or `DOOMWADPATH` environment variables
    5. Launch the Helion executable using one of the other methods described above, then edit the `files.directories = [".", "wads"]` line in `config.ini` to include the directory that contains your IWADs.
4. Run the Helion executable (Helion.exe on Windows, ./Helion on Linux) to play.

# Common issues

## Windows

1. If you have downloaded a file named `Helion-<version>-win-x64.zip`, you must install a Microsoft .NET 8.x runtime.  Please see https://dotnet.microsoft.com/en-us/download/dotnet/8.0 .  If you have downloaded a file named `Helion-<version>-win-x64_SelfContained.zip`, then this is not required.
2. One of our dependencies, OpenTK, needs the Microsoft Visual C Runtime.  If Helion appears to simply _not launch_, please consult errorlog.txt.  If it mentions being unable to load MSVCRT140.dll (or similar), please install the latest redistributable package: https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170#visual-studio-2015-2017-2019-and-2022

## Linux

1.  While we have included a version of LibFluidSynth that _should_ work on common Linux distributions, if it doesn't work, you may need to use your package manager to install a different one (and possibly delete the ".so" file we've included).  You can also run with `-nomusic` to disable music support.
2.  Similar to the Windows ZIP files, the standard `Helion-<version>-linux-x64.zip` file requires a .NET 8.x runtime.  See https://learn.microsoft.com/en-us/dotnet/core/install/linux .  The `Helion-<version>-linux-x64_SelfContained.zip` file provides its own self-contained runtime and does not require this.
3.  Helion requires OpenGL (GLFW) and OpenAL runtime components.  You must install these if they are not present, otherwise Helion will fail to start.  On a barebones Ubuntu install, OpenAL may need to be installed (`sudo apt-get install libopenal1`)  Additionally, the music library (ZMusic) requires libsndfile and libmpg123.  These are usually already installed by major Linux distributions (including Ubuntu) but may need to be installed manually on less common configurations.

# Contact Us

If you encounter issues using Helion and would like to report bugs you've encountered, you can:
1. Visit our thread on the Doomworld forums at https://www.doomworld.com/forum/topic/132153-helion-c-0940-824-goodbye-bsp-tree-rendering/
2. Report issues via GitHub at https://github.com/Helion-Engine/Helion/issues

