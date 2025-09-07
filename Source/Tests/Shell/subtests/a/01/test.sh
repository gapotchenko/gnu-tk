#!/bin/sh
set -eu

# ===========================================================================
echo "Test A01"
# Tests arguments passing for command.
# ===========================================================================

expected="a b c"
actual=$(gnu-tk.sh -c 'echo $0 $*' a b c)

if [ "$actual" != "$expected" ]; then
    echo "Unexpected: $actual"
    exit 2
fi
