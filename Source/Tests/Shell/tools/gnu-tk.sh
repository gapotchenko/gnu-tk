#!/bin/sh

set -eu

GNU_TK="${GNU_TK:-gnu-tk}"

if [ -n "${WSL_DISTRO_NAME:-}" ]; then 
    cmd.exe /c "$GNU_TK" $*
else
    "$GNU_TK" $*
fi
