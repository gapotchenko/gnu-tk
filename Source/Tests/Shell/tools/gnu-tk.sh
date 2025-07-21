#!/bin/sh

set -eu

GNU_TK="${GNU_TK:-gnu-tk}"

if [ -n "${WSL_DISTRO_NAME:-}" ]; then
    # On WSL, call the Windows executable back.
    cmd.exe /c "$GNU_TK" $*
else
    # On Unix, directly run the executable.
    "$GNU_TK" $*
fi
