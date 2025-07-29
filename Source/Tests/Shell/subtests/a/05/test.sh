#!/bin/sh

set -eu

echo Test A05

# Tests exit code passing for command.

run_test() {
    expected=$1
    actual=0
    $(gnu-tk.sh -c "exit $expected") || actual=$?

    if [ "$actual" != "$expected" ]; then
        echo "Unexpected: $actual"
        exit 2
    fi
}

run_test 0
run_test 123
