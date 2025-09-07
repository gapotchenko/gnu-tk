#!/bin/sh

set -eu

# ===========================================================================
echo "Test A04"
# Tests default value of 0th command argument.
# ===========================================================================

actual=$(gnu-tk.sh -c 'echo $0')

if [ -z "$actual" ]; then
    echo "Unexpected empty actual value."
    exit 2
fi
