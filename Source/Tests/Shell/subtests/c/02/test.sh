#!/bin/sh

# =============================================================================
# Test C02
# Tests file system path mapping functionality.
# =============================================================================

set -eu

echo "Test C02"

if [ "$GNU_TK_HOST_OS" = "windows" ]; then
    expected='C:\Phony'
    actual=$(gnu-tk.sh path -h 'C:/Phony')

    if [ "$actual" != "$expected" ]; then
        echo "Unexpected: $actual"
        exit 2
    fi
fi
