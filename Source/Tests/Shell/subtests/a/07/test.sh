#!/bin/sh

# =============================================================================
# Test A07
# Tests exit code passing for script file.
# =============================================================================

set -eu

echo "Test A07"

run_test() {
    expected=$1
    actual=0
    gnu-tk.sh -f script.sh "$expected" || actual=$?

    if [ "$actual" != "$expected" ]; then
        echo "Unexpected: $actual"
        exit 2
    fi
}

run_test 0
run_test 123
