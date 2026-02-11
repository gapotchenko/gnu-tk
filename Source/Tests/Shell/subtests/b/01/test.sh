#!/bin/sh

# =============================================================================
# Test B01
# Tests mapping of file path arguments between the host and GNU environments.
# =============================================================================

set -eu

echo "Test B01"

if [ -z "$GNU_TK_TEST_FILE_ASSET" ]; then
    echo "File asset test parameter is not specified."
    exit 2
fi

expected="This is an asset file."
actual=$(cat "$GNU_TK_TEST_FILE_ASSET")

if [ "$actual" != "$expected" ]; then
    echo "Unexpected: $actual"
    exit 2
fi
