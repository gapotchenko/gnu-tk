#!/bin/sh

# =============================================================================
# Test A06
# Tests exit code passing for command line.
# =============================================================================

set -eu

echo "Test A06"

run_test() {
    expected=$1
    actual=0
    gnu-tk.sh -l exit "$expected" || actual=$?

    if [ "$actual" != "$expected" ]; then
        echo "Unexpected: $actual"
        exit 2
    fi
}

run_test 0
run_test 123
