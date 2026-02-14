# Gapotchenko.GnuTK
#
# Copyright © Gapotchenko and Contributors
#
# File introduced by: Oleksiy Gapotchenko
# Year of introduction: 2025

set dotenv-load := true
set working-directory := "Source"
set windows-shell := ["powershell", "-c"]
set script-interpreter := ["gnu-tk", "-i", "-l", "/bin/sh", "-eu"]

# -----------------------------------------------------------------------------

dotnet-tfm := "net10.0"

# -----------------------------------------------------------------------------

# Show the help for this justfile
@help:
    just --list

# -----------------------------------------------------------------------------
# Development
# -----------------------------------------------------------------------------

# Start IDE using the project environment
[group("development")]
[windows]
develop:
    @Start-Process (Get-ChildItem -Path . -Filter *.slnx | Select-Object -First 1).FullName

# Start IDE using the project environment
[group("development")]
[unix]
develop:
    open *.slnx

# Install development prerequisites
[group("development")]
[script]
prerequisites:
    go install github.com/sibprogrammer/xq@latest
    npm install -g prettier
    go install mvdan.cc/sh/v3/cmd/shfmt@latest
    # Prerequisites dependent on a particular GNU toolkit
    if [ -n "${GNU_TK_MSYS2_REPOSITORY_PREFIX-}" ]; then
        pacman -S --needed --noconfirm "${GNU_TK_MSYS2_REPOSITORY_PREFIX}fd"
        pacman -S --needed --noconfirm moreutils
    fi

# Format source code
[group("development")]
[script]
[working-directory("..")]
format:
    prettier --write "**/*.{js,ts,json,md,yml}"
    echo 'Formatting **/*.sh...'
    fd -e sh -x shfmt -i 4 -l -w
    echo 'Formatting **/justfile...'
    fd --glob justfile -x just --unstable --fmt --justfile
    echo 'Formatting **/README.md...'
    fd --glob README.md -x deno fmt -q
    deno fmt -q Documentation/CHANGELOG.md
    echo 'Formatting miscellaneous files...'
    (cd Source/Mastering; cat Exclusion.dic | tr '[:upper:]' '[:lower:]' | sort -u | sponge Exclusion.dic)

# Check source code
[group("development")]
[script]
check:
    echo 'Checking **/*.sh...'
    fd -e sh -x shellcheck

# -----------------------------------------------------------------------------
# Build
# -----------------------------------------------------------------------------

# Build release artifacts
[group("build")]
build:
    dotnet build -c Release

# Rebuild release artifacts
[group("build")]
rebuild:
    dotnet build --no-incremental -c Release

# Clean all artifacts
[group("build")]
clean:
    dotnet clean -c Debug
    dotnet clean -c Release
    cd Deployment; just clean

# Run all tests
[group("diagnostics")]
test: build
    dotnet test -c Release -f "{{ dotnet-tfm }}"

# Produce publishable artifacts
[group("build")]
publish: _prepare-publish
    dotnet clean -c Release
    dotnet pack -c Release -p:TargetFormFactor=NuGet

[windows]
_prepare-publish:

[unix]
_prepare-publish:
    cd Mastering/Shell; chmod +x *.sh

# Build platform-dependent release artifacts
[group("build")]
platform-build: _build-aot _build-setup

[linux]
_build-aot:
    if [ "$(uname -m)" = "x86_64" ]; then just _build-aot-arch linux-x64; fi
    if [ "$(uname -m)" = "aarch64" ]; then just _build-aot-arch linux-arm64; fi

[windows]
_build-aot: (_build-aot-arch "win-x64") (_build-aot-arch "win-arm64")

[macos]
_build-aot: (_build-aot-arch "osx-arm64") (_build-aot-arch "osx-x64")

_build-aot-arch rid:
    cd Gapotchenko.GnuTK; dotnet publish -c Release -p:PublishAot=true -r "{{ rid }}" -f "{{ dotnet-tfm }}"

_build-setup:
    cd Deployment/Setup; just build 

# Produce platform-dependent publishable artifacts
[group("build")]
platform-publish:
    cd Deployment/Packaging; just pack

# -----------------------------------------------------------------------------
# Diagnostics
# -----------------------------------------------------------------------------

# Run GNU-TK from the source code
[group("diagnostics")]
[no-cd]
gnu-tk *args:
    dotnet run --project "{{ absolute_path("Gapotchenko.GnuTK") }}" -c Release -f "{{ dotnet-tfm }}" --no-launch-profile -v q -- {{ args }}
