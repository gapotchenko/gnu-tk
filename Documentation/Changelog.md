# What's New in GNU-TK

## 2025

### GNU-TK 2025.5

Release date: not released yet

- Added support for GNU toolkit provided by Git for Windows distribution
- WSL toolkit commands are executed using a login shell
- GNU toolkit strictness is activated by `GNU_TK_STRICT` environment variable

### GNU-TK 2025.4

Release date: July 18, 2025

- Use `errexit` and `pipefail` shell options when executing commands
- Utilize MSYS2 toolkits installed by `msys2/setup-msys2` GitHub action
- Fixed handling of command arguments for Cygwin and MSYS2 toolkits
- Fixed toolkit deduplication

### GNU-TK 2025.3

Release date: July 17, 2025

- Minimize command line reconstructions which may introduce inaccuracies
- More precise MSYS2 toolkit version detection
- MSYS2 toolkit selection respects the value of `MSYSTEM` environment variable

### GNU-TK 2025.2

Release date: July 16, 2025

- Expansion of naturally occurring shebang arguments
- Automatic translation of file path arguments
- Ability to use only a GNU toolkit that operates within the host environment (`-i` command-line option)

### GNU-TK 2025.1

Release date: July 14, 2025

- Initial release
