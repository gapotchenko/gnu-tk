#!/bin/sh

set -eu

scriptPath=$(pwd)
cd ../contents

rm -rf platforms && mkdir platforms

basePath=../../../../Gapotchenko.GnuTK/bin/Release/net9.0
assetsPath=$scriptPath/../assets

# ---------------------------------------------------------------------------
# Windows
# ---------------------------------------------------------------------------

mkdir platforms/win-x64
cp -l "$basePath/win-x64/publish/Gapotchenko.GnuTK.exe" platforms/win-x64/gnu-tk.exe
cp "$assetsPath/gnu-tk.ini" platforms/win-x64

mkdir platforms/win-arm64
cp -l "$basePath/win-arm64/publish/Gapotchenko.GnuTK.exe" platforms/win-arm64/gnu-tk.exe
cp "$assetsPath/gnu-tk.ini" platforms/win-arm64

# ---------------------------------------------------------------------------
# Linux
# ---------------------------------------------------------------------------

mkdir platforms/linux-x64
cp -l "$basePath/linux-x64/publish/Gapotchenko.GnuTK" platforms/linux-x64/gnu-tk
chmod +x platforms/linux-x64/gnu-tk

mkdir platforms/linux-arm64
cp -l "$basePath/linux-arm64/publish/Gapotchenko.GnuTK" platforms/linux-arm64/gnu-tk
chmod +x platforms/linux-arm64/gnu-tk

# ---------------------------------------------------------------------------
# macOS
# ---------------------------------------------------------------------------

mkdir platforms/osx-x64
cp -l "$basePath/osx-x64/publish/Gapotchenko.GnuTK" platforms/osx-x64/gnu-tk
chmod +x platforms/osx-x64/gnu-tk

mkdir platforms/osx-arm64
cp -l "$basePath/osx-arm64/publish/Gapotchenko.GnuTK" platforms/osx-arm64/gnu-tk
chmod +x platforms/osx-arm64/gnu-tk
