#!/bin/sh

set -eu

# ===========================================================================
echo "Test C01"
# Tests value of GNU_TK_VERSION environment variable.
# ===========================================================================

actual=$(gnu-tk.sh -c 'echo $GNU_TK_VERSION')

if [ -z "$actual" ]; then
    echo "Unexpected empty actual value."
    exit 2
fi
