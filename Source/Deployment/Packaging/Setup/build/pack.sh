#!/bin/sh

set -eu

cd ..

# ---------------------------------------------------------------------------

repoPath="$(pwd)/../../../.."
version=$(xq "$repoPath/Source/Mastering/.NET/Version.props" -x //Project/PropertyGroup/Version)

# ---------------------------------------------------------------------------

get_output_file_name() {
    version=$1
    platform=$2
    echo "gnu-tk-$version-setup-$platform"
}

pack_windows() {
    platform=$1
    arch=$2
    lang="en-US"

    output_file_name="$(get_output_file_name "$version" "$platform").msi"

    echo "$output_file_name"
    cp -l "$repoPath/Source/Deployment/Setup/MSI/bin/$arch/Release/$lang/Gapotchenko.GnuTK.Setup.Msi.msi" "bin/$output_file_name"
}

# ---------------------------------------------------------------------------

# Windows
pack_windows windows-x64 x64
pack_windows windows-arm64 ARM64
