# GNU-TK Development

## Omissions

- BusyBox path translation on Windows does not translate `~` Unix path
  placeholder as it should
- Setup for Windows does not provide a choice to do a per-user installation (as
  opposed to per-machine installation scope)
- Installation scripts are not implemented
- Path translation does not work when used from WSL guest system

## Nice to Have Features

- `gnu-tk open` command with a unified CLI over `start` / `open` / `xdg-open`
  depending on a host system

## General Thoughts

- Commands like `gnu-tk open` are better to be scoped as subcommands of a common
  parent (like `gnu-tk system open`) due to the fact that GNU-TK provides its
  own operational environment separate to host and guest systems
- Make `gnu-tk open` appear in help, but annotate it: `open` - shorthand for
  `system open`. This solves usage vs. taxonomy dillema
