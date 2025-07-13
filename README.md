# GNU-TK

GNU-TK is a tiny project that provides seamless scriptable access to GNU tools on non-Unix operating systems.

## Overview

[GNU](https://www.gnu.org/software/) is a software system that has been developed by many people working together for the sake of freedom of all software users to control their computing.

The GNU system offers a wide range of tools, including essential command-line utilities such as `bash`, `cp`, `sed`, and many others.
For instance, GNU [core utilities](https://www.gnu.org/software/coreutils/) are crucial for automating cross-platform projects by enabling the creation of reproducible build scripts.
Beyond that, GNU tools support a wide variety of other use cases.

A recurring issue with GNU tools, however, is their limited accessibility on non-Unix operating systems.
For example, on Windows, you can use toolkits such as [MSYS2](https://www.msys2.org/), [Cygwin](https://cygwin.com/) or [WSL](https://learn.microsoft.com/windows/wsl/ "Windows Subsystem for Linux").
Yet, these introduce their own challenges: correctly identifying the toolkit installation path, modifying environment variables, configuring shells with toolkit-specific parameters, and, finally, managing the handoff of control to and from the toolkit.
All of this makes using GNU tools far from easy and reproducible.

GNU-TK project solves those problems by providing a small `gnu-tk` command-line utility that serves as an automatic gateway to a GNU toolkit installed in the system.
`gnu-tk` has a concise and stable command-line interface making it suitable for effortless cross-platform scripting.

## Usage

The basic usage example:

```sh
gnu-tk -l cp --help
```

The command above invokes `cp` GNU utility with `--help` command-line option.

You can also pass a command to execute as a single command-line option:

```sh
gnu-tk -c "cp --help"
```

Or as a script file:

```sh
gnu-tk -f <script-file>
```

## Installation

To install GNU-TK, you can use one of the supported package managers.
GNU-TK can be installed on Windows, macOS, and Linux operating systems.

### NPM Package Manager

NPM is a part of [Node.js](https://nodejs.org/).
Use the following command to install GNU-TK globally:

```
npm install -g @gapotchenko/gnu-tk
```

Alternatively, you can install GNU-TK on a per-project basis if you need to use `gnu-tk` in project scripts in a way reproducible by others.

### NuGet Package Manager

NuGet is a part of [.NET](a).
Use the following command to install GNU-TK as a global .NET tool:

```
dotnet tool install -g Gapotchenko.GnuTK
```

### GNU Toolkits

Aside from GNU-TK itself, you also need an actual GNU toolkit installed on your system:

- **Windows:** [MSYS2](https://www.msys2.org/) (recommended), [Cygwin](https://cygwin.com/) or [WSL](https://learn.microsoft.com/windows/wsl/ "Windows Subsystem for Linux")
- **macOS:** [Homebrew](https://brew.sh/) package manager with installed [`bash`](https://formulae.brew.sh/formula/bash) and [`coreutils`](https://formulae.brew.sh/formula/coreutils) packages as the bare minimum
- **Linux:** already comes with a pre-installed GNU toolkit

## How it Works

Internally, `gnu-tk` automatically finds and selects an appropriate GNU toolkit installed in the system, and then executes a specified command using it.

To list all available toolkits, you can use `gnu-tk list` command:

```
Available GNU Toolkits

Name               Description                Location
------------------------------------------------------------------------------
cygwin             Cygwin 3.6.2               C:\cygwin64
msys2-clang64      MSYS2 2025-02-21           C:\msys64
msys2-clangarm64   MSYS2 2025-02-21           C:\msys64
msys2-mingw32      MSYS2 2025-02-21           C:\msys64
msys2-mingw64      MSYS2 2025-02-21           C:\msys64
msys2-msys         MSYS2 2025-02-21           C:\msys64
msys2-ucrt64       MSYS2 2025-02-21           C:\msys64
wsl                WSL 2.5.9.0                C:\Program Files\WSL

Tips:
  - You can install a GNU toolkit to add it to the list:
    https://gapt.ch/help/gnu-tk/install-toolkits
  - You can use 'GNU_TK_TOOLKIT_PATH' environment variable to specify the
    directory paths of portable GNU toolkits
  - GNU toolkits supported on Windows: Cygwin, MSYS2, WSL
```

To simplify usage, GNU-TK automatically selects the most suitable GNU toolkit based on common-sense factors.
You can verify the GNU toolkit selection and availability using `gnu-tk check` command:

```
GNU Toolkit Check

Name: msys2-ucrt64
Description: MSYS2 2025-02-21
Location: C:\msys64
Semantics: GNU

Check status: PASS

Tips:
  - To change the selected toolkit, specify '--toolkit' command-line option or
    its shorthand '-t'
  - Alternatively, you can set 'GNU_TK_TOOLKIT' environment variable to the
    name of a GNU toolkit to use by default
```

You can affect GNU toolkit selection during command invocation.
For example, you can pass `-s` (or `--strict`) command-line option to `gnu-tk`, indicating that a toolkit with strict GNU semantics is required:

```
gnu-tk -s -l sed --help
```

This can be vital on Unix operating systems like macOS that have built-in versions of Unix utilities.
By specifying the `-s` command-line option, you explicitly assert that only a GNU version of `sed` utility should be ever used.
Without that option, `gnu-tk` works in relaxed mode allowing built-in Unix OS commands to work as close-enough substitutes when no corresponding GNU utilities are found.
