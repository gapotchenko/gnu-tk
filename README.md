# GNU-TK

GNU-TK is a tiny project that provides seamless scriptable access to GNU tools
on non-Unix operating systems.

## Overview

[GNU](https://www.gnu.org/software/) is a software system that has been
developed by many people working together for the sake of freedom of all
software users to control their computing.

The GNU system offers a wide range of tools, including essential command-line
utilities such as `bash`, `cp`, `sed`, and many others. For instance, GNU
[core utilities](https://www.gnu.org/software/coreutils/) are crucial for
automating cross-platform projects by enabling the creation of reproducible
scripts. Beyond that, GNU tools support a wide variety of other use cases.

A **recurring issue with GNU tools**, however, is their limited accessibility on
non-Unix operating systems. For example, on Windows, you can use toolkits such
as [MSYS2](https://www.msys2.org/), [Cygwin](https://cygwin.com/) or
[WSL](https://learn.microsoft.com/windows/wsl/ "Windows Subsystem for Linux").
Yet, they introduce their own challenges: correctly identifying the toolkit
installation path, modifying environment variables, configuring shells with
toolkit-specific parameters, and, finally, managing the handoff of control to
and from the toolkit. All of this makes using GNU tools far from easy and
reproducible.

GNU-TK project addresses these problems by providing a small `gnu-tk`
command-line utility that acts as an automatic gateway to a GNU toolkit
installed on the system. `gnu-tk` has a concise and stable command-line
interface making it suitable for effortless cross-platform scripting. In
addition, it smoothes out the inconsistencies and shortcomings that typically
come with individual GNU toolkit distributions.

## Usage

The basic usage example:

```sh
gnu-tk -l cp --help
```

The command above invokes `cp` GNU utility with `--help` command-line option.

You can also pass a command to execute as a single command-line argument:

```sh
gnu-tk -c "cp --help"
```

Or as a script file:

```sh
gnu-tk -f <script-file>
```

## Installation

To install GNU-TK, you can use one of the supported package managers.

### Package Managers

#### NPM

NPM is a part of [Node.js](https://nodejs.org/). Use the following command to
install GNU-TK globally:

```sh
npm install -g @gapotchenko/gnu-tk
```

Alternatively, you can install GNU-TK locally within the Node.js project that
uses it: `npm install --save-dev @gapotchenko/gnu-tk`.

#### NuGet

NuGet is a part of [.NET](https://dotnet.microsoft.com/). Use the following
command to install GNU-TK as a global .NET tool:

```sh
dotnet tool install -g Gapotchenko.GnuTK
```

### Supported Platforms

GNU-TK can be readily used on the following platforms:

- Operating systems: Windows, macOS, Linux
- CPU architectures: x64, ARM64

### GNU Toolkits

Aside from GNU-TK utility itself, you may also need an actual GNU toolkit
installed on your system:

- **Windows:**
  - [BusyBox](https://frippery.org/busybox/)
  - [Cygwin](https://cygwin.com/)
  - [Git](https://git-scm.com/downloads/win) (not a GNU toolkit by itself; comes with a stripped down, immutable MSYS2 subset)
  - [MSYS2](https://www.msys2.org/) (recommended)
  - [WSL](https://learn.microsoft.com/windows/wsl/ "Windows Subsystem for Linux")
- **macOS:**
  - [Homebrew](https://brew.sh/) package manager with installed GNU packages;
    [`bash`](https://formulae.brew.sh/formula/bash) and
    [`coreutils`](https://formulae.brew.sh/formula/coreutils) is a recommended
    bare minimum
  - Includes a pre-installed GNU-like toolkit
- **Linux:**
  - [BusyBox](http://www.busybox.net/)
  - Includes a pre-installed GNU toolkit

#### Built-in Toolkits

The following built-in toolkits are bundled with GNU-TK packages to guarantee
that GNU tools minimally work even when no specialized GNU toolkit is available:

- **Windows:** [BusyBox](https://frippery.org/busybox/)

## How it Works

Internally, `gnu-tk` automatically finds and selects an appropriate GNU toolkit
installed in the system, and then executes a specified command using it.

To list all available toolkits, you can use `gnu-tk list` command:

```
Available GNU Toolkits

Name               Description                Location
------------------------------------------------------------------------------
busybox            BusyBox 1.38.0             (built-in)
cygwin             Cygwin 3.6.2               C:\cygwin64
git                Git 2.50.1                 C:\Program Files\Git
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
  - GNU toolkits supported on Windows: BusyBox, Cygwin, Git, MSYS2, WSL
```

To simplify usage, GNU-TK automatically selects the most suitable GNU toolkit
based on common-sense factors. You can verify the GNU toolkit selection and
availability using `gnu-tk check` command:

```
GNU Toolkit Check

Name: msys2-ucrt64
Description: MSYS2 2025-02-21
Location: C:\msys64
Semantics: GNU
Isolation: none

Check status: PASS

Tips:
  - To change the selected toolkit, specify '--toolkit' command-line option or
    its shorthand '-t'
  - Alternatively, you can set 'GNU_TK_TOOLKIT' environment variable to the
    name of a GNU toolkit to use by default
```

You can affect GNU toolkit selection during command invocation. For example, you
can pass `-s` (or `--strict`) command-line option to `gnu-tk`, indicating that a
toolkit with strict GNU semantics is required:

```
gnu-tk -s -l sed --help
```

This can be vital on Unix operating systems like macOS that have built-in
versions of Unix utilities. By specifying the `-s` command-line option, you
explicitly assert that only a GNU version of `sed` utility should be ever used.
Without that option, `gnu-tk` works in relaxed mode allowing built-in Unix OS
commands to work as close-enough substitutes when no corresponding GNU utilities
are found.

## Examples

GNU-TK is an adaptable tool that can work with a variety of technologies. In
this section, we'll explore a few integration examples.

### Node.js

Node.js projects use a `package.json` file to define project configuration. One
key feature of this file is the ability to specify custom scripts, which can be
executed using the `npm run <script>` command.

By default, custom scripts defined in `package.json` are executed using the
command shell of the host operating system. This is sufficient for basic tasks,
but more complex scenarios may require commands like `cp`, `mv`, `sed`, and
others. While some of these commands have OS-specific equivalents, relying on
them can reduce the portability of your project across different environments.

In that case, you can use `gnu-tk` to make your scripts provably portable across
different platforms. Let's examine a sample `package.json` file:

```json
{
  "scripts": {
    "rebuild": "npm run clear && npm run build",
    "build": "docusaurus build && gnu-tk -l rm -rf public && gnu-tk -l mkdir public && gnu-tk -l cp -r build public/content && gnu-tk -l mv public/content/_* public && gnu-tk -l mv public/content/*.txt public && gnu-tk -s -l sed -i public/content/sitemap.xml -f src/patches/sitemap.xml.sed",
    "clear": "docusaurus clear && gnu-tk -l rm -rf public"
  }
}
```

Note how the `gnu-tk` tool is used in the script definitions. In this mode, you
can seamlessly switch between native shell commands and GNU tools as needed.

For this to work, `gnu-tk` must be installed either [globally](#installation),
or locally within the Node.js project that uses `gnu-tk`:

```sh
npm install --save-dev @gapotchenko/gnu-tk
```

### Just

[`just`](https://github.com/casey/just) is a cross-platform script runner.

By default, `just` executes scripts using the command shell of the host
operating system. This approach works well for trivial cases but falls apart on
more complex scenarios. Let's examine a sample `justfile`:

```just
run:
    echo Just hello
    cp --help
```

If you try to run this script with `just run` command on a Unix system, you will
get the expected correct result. On Windows, you'll encounter an error because
the `cp` command is not available on this OS.

One way to solve that problem is to use `gnu-tk` in place:

```just
set windows-shell := ["cmd", "/c"]

run:
    echo Just hello
    gnu-tk -l cp --help
```

This gives us the desired result.

While the above approach works, repeatedly invoking `gnu-tk` can become tedious.
To simplify things, we can configure `gnu-tk` as the default command shell for
`justfile`:

```just
set windows-shell := ["gnu-tk", "-i", "-c"]

run:
    echo Just hello
    cp --help
```

Now, `just run` command will produce the correct results on all supported
platforms.

Setting `windows-shell` to `gnu-tk` for an entire `justfile` can sometimes be
too aggressive. In such cases, a more gradual approach using the `[script]`
attribute may be preferable
([docs](https://just.systems/man/en/script-recipes.html)):

```just
set windows-shell := ["cmd", "/c"]
set script-interpreter := ["gnu-tk", "-i", "-l", "/bin/sh", "-eu"]

[script]
run:
    echo Just hello
    cp --help
```

And if you would like to write your cross-platform `just` recipes in another
language – say, Python:

```just
[script("gnu-tk", "-i", "-l", "/usr/bin/env", "python3")]
run:
    print("Hello from Python script")
```
