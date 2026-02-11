#!/bin/sh

# =============================================================================
# Test C01
# Tests value of GNU_TK_VERSION environment variable.
# =============================================================================

set -eu

echo "Test C01"

actual=$(gnu-tk.sh -c 'echo $GNU_TK_VERSION')

if [ -z "$actual" ]; then
    echo "Unexpected empty actual value."
    exit 2
fi
