#!gnu-tk -f

# Installs system-dependent development prerequisites.

set -eu

if [ -n "${GNU_TK_MSYS2_REPOSITORY_PREFIX-}" ]; then
    pacman -S --needed "${GNU_TK_MSYS2_REPOSITORY_PREFIX}fd"
fi
