#!/bin/sh

# =============================================================================
# Test A03
# Tests arguments passing for script file.
# =============================================================================

set -eu

echo "Test A03"

expected="a b c"
actual=$(gnu-tk.sh -f script.sh a b c)

if [ "$actual" != "$expected" ]; then
    echo "Unexpected: $actual"
    exit 2
fi
