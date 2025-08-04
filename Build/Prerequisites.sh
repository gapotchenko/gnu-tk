#!gnu-tk -f

# Installs development prerequisites dependent on a GNU toolkit.

set -eu

if [ -n "${GNU_TK_MSYS2_REPOSITORY_PREFIX-}" ]; then
    pacman -S --needed "${GNU_TK_MSYS2_REPOSITORY_PREFIX}fd"
fi
