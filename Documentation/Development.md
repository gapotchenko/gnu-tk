# GNU-TK Development

## Known Omissions

- BusyBox path translation on Windows does not translate `~` Unix path
  placeholder as it should
- Setup for Windows does not provide a choice to do a per-user installation (as
  opposed to the per-machine installation scope)
- GNU-TK installation via HTTPS script is not implemented
- Path translation does not work when used within WSL guest system

## Possible Features

- `gnu-tk open` command with a unified CLI over `start` / `open` / `xdg-open`
  depending on a host system
