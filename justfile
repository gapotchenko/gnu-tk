set working-directory := "Source"
set dotenv-load := true
set windows-shell := ["cmd", "/c"]
default-toolkit := "auto"

# Show the help for this justfile
@help:
    just --list

# Start IDE using the project environment
[windows]
develop:
    #!cmd /c
    @for /F "delims=" %%i in ('"dir /b | findstr ".*\.sln""') do @(start "" "%%i")

# Start IDE using the project environment
[unix]
develop:
    open *.sln?

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
test:
    dotnet test -c Release

# Produce publishable artifacts
publish:
    dotnet clean -c Release
    dotnet pack -c Release

# Build platform-dependent release artifacts
platform-build: _publish-aot

[linux]
_publish-aot:
    if [ "`uname -m`" = "x86_64" ]; then dotnet publish -c Release -p:PublishAot=true -r linux-x64 -f net9.0; fi
    if [ "`uname -m`" = "aarch64" ]; then dotnet publish -c Release -p:PublishAot=true -r linux-arm64 -f net9.0; fi

[windows]
_publish-aot:
    dotnet publish -c Release -p:PublishAot=true -r win-x64 -f net9.0
    dotnet publish -c Release -p:PublishAot=true -r win-arm64 -f net9.0

[macos]
_publish-aot:
    dotnet publish -c Release -p:PublishAot=true -r osx-arm64 -f net9.0
    dotnet publish -c Release -p:PublishAot=true -r osx-x64 -f net9.0

# Produce platform-dependent publishable artifacts
platform-publish:
    cd Packaging && just pack

# List GNU toolkits
toolkit-list:
    dotnet run --project Gapotchenko.GnuTK/Gapotchenko.GnuTK.csproj -c Release -f net9.0 --no-launch-profile -v q -- list -q

# Check GNU toolkit
toolkit-check toolkit=default-toolkit:
    dotnet run --project Gapotchenko.GnuTK/Gapotchenko.GnuTK.csproj -c Release -f net9.0 --no-launch-profile -v q -- -t {{toolkit}} check -q
