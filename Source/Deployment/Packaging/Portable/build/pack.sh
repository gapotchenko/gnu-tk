#!/bin/sh

set -eu
cd ..

# -----------------------------------------------------------------------------

repoPath="$(pwd)/../../../.."

masterPath="$repoPath/Source/Mastering/Shell"
product=$("$masterPath/product.sh" name)
version=$("$masterPath/version.sh" semantic)
tfm=$("$masterPath/targeting.sh" tfm)

basePath="$repoPath/Source/Gapotchenko.GnuTK/bin/Release/$tfm"

# -----------------------------------------------------------------------------

get_output_file_name() {
    version=$1
    platform=$2
    echo "$product-$version-portable-$platform"
}

pack_windows() {
    platform=$1
    rid=${2:-$platform}
    output_file_name="$(get_output_file_name "$version" "$platform").zip"

    mkdir "obj/$platform"
    cp -l "$basePath/$rid/publish/Gapotchenko.GnuTK.exe" "obj/$platform/gnu-tk.exe"
    cp -l "$repoPath/LICENSE" "obj/$platform/LICENSE.txt"
    cp -l "$repoPath/README.md" "obj/$platform/"
    cp -l "$repoPath/Documentation/ABOUT" "obj/$platform/ABOUT.txt"

    echo "$output_file_name:"
    cd "obj/$platform"
    zip -r "../../bin/$output_file_name" .
    cd ../..
}

pack_unix() {
    platform=$1
    rid=${2:-$platform}
    output_file_name="$(get_output_file_name "$version" "$platform").tar.gz"

    mkdir "obj/$platform"
    cp -l "$basePath/$rid/publish/Gapotchenko.GnuTK" "obj/$platform/gnu-tk"
    chmod +x "obj/$platform/gnu-tk"
    cp -l "$repoPath/LICENSE" "obj/$platform/"
    cp -l "$repoPath/README.md" "obj/$platform/"
    cp -l "$repoPath/Documentation/ABOUT" "obj/$platform/"

    echo "$output_file_name:"
    cd "obj/$platform"
    tar czvf "../../bin/$output_file_name" -- * | sed 's_^_  adding: _'
    cd ../..
}

# -----------------------------------------------------------------------------

# Windows
pack_windows windows-x64 win-x64
pack_windows windows-arm64 win-arm64

# Linux
pack_unix linux-x64
pack_unix linux-arm64

# macOS
pack_unix macos-x64 osx-x64
pack_unix macos-arm64 osx-arm64

# -----------------------------------------------------------------------------

cd bin

echo "Calculating checksums..."
sha256sum -b -- * | tr '*' ' ' >SHA256SUMS
