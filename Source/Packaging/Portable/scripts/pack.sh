set -eu
cd ..

repoPath=../../..
basePath=$repoPath/Source/Gapotchenko.GnuTK/bin/Release/net9.0

# Windows

pack_windows() {
    platform=$1
    rid=$2
    output_file_name="gnu-tk-portable-$platform.zip"

    mkdir obj/$platform
    cp -l "$basePath/$rid/publish/Gapotchenko.GnuTK.exe" "obj/$platform/gnu-tk.exe"
    cp -l "$repoPath/LICENSE" "obj/$platform/LICENSE.txt"
    cp -l "$repoPath/README.md" "obj/$platform/"
    cp -l "$repoPath/Documentation/ABOUT" "obj/$platform/ABOUT.txt"

    echo "$output_file_name:"
    cd "obj/$platform"
    zip -r "../../bin/$output_file_name" .
    cd ../..
}

pack_windows windows-x64 win-x64
pack_windows windows-arm64 win-arm64
