set working-directory := "Source"
set dotenv-load := true
set windows-shell := ["cmd", "/c"]

dotnet-framework := "net9.0"
default-gnu-toolkit := "auto"

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
    dotnet test -c Release -f {{ dotnet-framework }}

# Produce publishable artifacts
publish:
    dotnet clean -c Release
    dotnet pack -c Release

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
toolkit-list:
    dotnet run --project Gapotchenko.GnuTK/Gapotchenko.GnuTK.csproj -c Release -f {{ dotnet-framework }} --no-launch-profile -v q -- list -q

# Check GNU toolkit
toolkit-check toolkit=default-gnu-toolkit:
    dotnet run --project Gapotchenko.GnuTK/Gapotchenko.GnuTK.csproj -c Release -f {{ dotnet-framework }} --no-launch-profile -v q -- -t {{ toolkit }} check -q
