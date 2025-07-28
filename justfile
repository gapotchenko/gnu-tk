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

set windows-shell := ["cmd", "/c"]
set script-interpreter := ["gnu-tk", "-i", "-l", "sh", "-eu"]

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
    #!cmd /c
    @for /F "delims=" %%i in ('"dir /b | findstr ".*\.sln""') do @(start "" "%%i")

# Start IDE using the project environment
[group("development")]
[unix]
develop:
    open *.sln?

# Install prerequisites
[group("development")]
prerequisites:
    go install github.com/sibprogrammer/xq@latest
    npm install -g prettier
    go install mvdan.cc/sh/v3/cmd/shfmt@latest

# Format source code
[group("development")]
[script]
[working-directory('..')]
format:
    find Source -type f -name "*.sh" -exec shfmt -i 4 -l -w {} \;
    prettier --write "**/*.{js,ts,json,md,yml}"
    just --fmt

# Build release artifacts
build:
    dotnet build -c Release

# Rebuild release artifacts
rebuild:
    dotnet build --no-incremental -c Release

# Clean all artifacts
clean:
    dotnet clean -c Debug
    dotnet clean -c Release
    cd Packaging && just clean

# Run all tests
[group("diagnostics")]
test:
    dotnet build -c Release
    dotnet test -c Release -f {{ dotnet-framework }}

# Produce publishable artifacts
publish:
    dotnet clean -c Release
    dotnet pack -c Release -p:WA49799=true

# Build platform-dependent release artifacts
platform-build: _publish-aot

[linux]
_publish-aot:
    if [ "`uname -m`" = "x86_64" ]; then cd Gapotchenko.GnuTK && dotnet publish -c Release -p:PublishAot=true -r linux-x64 -f {{ dotnet-framework }}; fi
    if [ "`uname -m`" = "aarch64" ]; then cd Gapotchenko.GnuTK && dotnet publish -c Release -p:PublishAot=true -r linux-arm64 -f {{ dotnet-framework }}; fi

[windows]
_publish-aot:
    cd Gapotchenko.GnuTK && dotnet publish -c Release -p:PublishAot=true -r win-x64 -f {{ dotnet-framework }}
    cd Gapotchenko.GnuTK && dotnet publish -c Release -p:PublishAot=true -r win-arm64 -f {{ dotnet-framework }}

[macos]
_publish-aot:
    cd Gapotchenko.GnuTK && dotnet publish -c Release -p:PublishAot=true -r osx-arm64 -f {{ dotnet-framework }}
    cd Gapotchenko.GnuTK && dotnet publish -c Release -p:PublishAot=true -r osx-x64 -f {{ dotnet-framework }}

# Produce platform-dependent publishable artifacts
platform-publish:
    cd Packaging && just pack

# List GNU toolkits
[group("diagnostics")]
toolkit-list:
    dotnet run --project Gapotchenko.GnuTK -c Release -f {{ dotnet-framework }} --no-launch-profile -v q -- list -q

# Check GNU toolkit
[group("diagnostics")]
toolkit-check toolkit="auto":
    dotnet run --project Gapotchenko.GnuTK -c Release -f {{ dotnet-framework }} --no-launch-profile -v q -- -t {{ toolkit }} check -q
