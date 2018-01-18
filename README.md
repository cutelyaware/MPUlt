# MPUlt
This program by Andrey Astrelin supports large variety of twisty puzzles in higher dimensional spaces.

http://superliminal.com/andrey/mpu/

# Building
You can clone this repo and build using Microsoft Visual Studio.  However, you will need to install an older version of the DirectX libraries to get the references to work.  That install can be found here:

https://www.microsoft.com/en-us/download/details.aspx?id=6812

It would be nice to modernize the DirectX usage.  Some notes about replacement packages are available here:
https://stackoverflow.com/questions/33873592/where-i-can-find-microsoft-directx-assembly-to-reference

### Troubleshooting
- When you install DirectX, if you see Error code S1023, please follow this link: https://stackoverflow.com/questions/4102259/directx-sdk-june-2010-installation-problems-error-code-s1023 to uninstall a newer version of Visual C++ 2010 Redistributable Package, install DirectX, and run Windows Update to get the redistributable package back.
- When you run the project in Visual Studio and see BadImageFormatException and if your platform selector shows "AnyCPU", please select "x86" instead.
- When you run the project in Visual Studio and see a "LoaderLock occurred" error, please uncheck "Break when this exception type is thrown", and click "Continue" to ignore it. If your Visual Studio doesn't have this option, follow https://stackoverflow.com/questions/56642/loader-lock-error to ignore this type of exceptions in a menu.
