# Gapotchenko.GnuTK
#
# Copyright © Gapotchenko and Contributors
#
# File introduced by: Oleksiy Gapotchenko
# Year of introduction: 2025

set working-directory := "Source"
set unstable := true

# ---------------------------------------------------------------------------
# Shells & Interpreters
# ---------------------------------------------------------------------------

set windows-shell := ["powershell", "-c"]
set script-interpreter := ["gnu-tk", "-i", "-l", "/bin/sh", "-eu"]

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------

set dotenv-load := true

dotnet-framework := "net9.0"

# ---------------------------------------------------------------------------
# Recipes
# ---------------------------------------------------------------------------

# Show the help for this justfile
@help:
    just --list

# Start IDE using the project environment
[group("development")]
[windows]
develop:
    @Start-Process (Get-ChildItem -Path . -Filter *.slnx | Select-Object -First 1).FullName

# Start IDE using the project environment
[group("development")]
[unix]
develop:
    open *.sln?

# Install development prerequisites
[group("development")]
prerequisites:
    go install github.com/sibprogrammer/xq@latest
    npm install -g prettier
    go install mvdan.cc/sh/v3/cmd/shfmt@latest
    gnu-tk -i -x Build/Prerequisites.sh

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
    cd Packaging; just clean
    cd Setup; just clean

# Run all tests
[group("diagnostics")]
test: build
    dotnet test -c Release -f "{{ dotnet-framework }}"

# Produce publishable artifacts
[group("build")]
publish:
    dotnet clean -c Release
    dotnet pack -c Release -p:TargetFormFactor=NuGet

# Build platform-dependent release artifacts
[group("build")]
platform-build: _publish-aot _build-setup

[linux]
_publish-aot:
    if [ "$(uname -m)" = "x86_64" ]; then cd Gapotchenko.GnuTK; dotnet publish -c Release -p:PublishAot=true -r linux-x64 -f "{{ dotnet-framework }}"; fi
    if [ "$(uname -m)" = "aarch64" ]; then cd Gapotchenko.GnuTK; dotnet publish -c Release -p:PublishAot=true -r linux-arm64 -f "{{ dotnet-framework }}"; fi

[windows]
_publish-aot:
    cd Gapotchenko.GnuTK; dotnet publish -c Release -p:PublishAot=true -r win-x64 -f "{{ dotnet-framework }}"
    cd Gapotchenko.GnuTK; dotnet publish -c Release -p:PublishAot=true -r win-arm64 -f "{{ dotnet-framework }}"

[macos]
_publish-aot:
    cd Gapotchenko.GnuTK; dotnet publish -c Release -p:PublishAot=true -r osx-arm64 -f "{{ dotnet-framework }}"
    cd Gapotchenko.GnuTK; dotnet publish -c Release -p:PublishAot=true -r osx-x64 -f "{{ dotnet-framework }}"

_build-setup:
    cd Setup; just build 

# Produce platform-dependent publishable artifacts
[group("build")]
platform-publish:
    cd Packaging; just pack

# Run GNU-TK from the source code
[group("diagnostics")]
[no-cd]
gnu-tk *args:
    dotnet run --project "{{ absolute_path("Gapotchenko.GnuTK") }}" -c Release -f "{{ dotnet-framework }}" --no-launch-profile -v q -- {{ args }}
