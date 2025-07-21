set working-directory := "Source"
set dotenv-load := true
set windows-shell := ["cmd", "/c"]
set unstable := true

dotnet-framework := "net9.0"

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

[script("gnu-tk", "-f")]
format:
    find . -type f -name "*.sh" -exec shfmt -i 4 -l -w {} \;

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
toolkit-list:
    dotnet run --project Gapotchenko.GnuTK -c Release -f {{ dotnet-framework }} --no-launch-profile -v q -- list -q

# Check GNU toolkit
toolkit-check toolkit="auto":
    dotnet run --project Gapotchenko.GnuTK -c Release -f {{ dotnet-framework }} --no-launch-profile -v q -- -t {{ toolkit }} check -q
