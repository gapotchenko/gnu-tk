#!/bin/sh

set -eu

echo "Test D01"

# Tests handling of '\' character for command-line arguments.

export VAR=ABC

# Case 1.A

actual=$(sh -c 'echo "\$VAR"')
expected='$VAR'

if [ "$actual" != "$expected" ]; then
    echo "Unexpected at 1.A: $actual"
    exit 2
fi

# Case 1.B

actual=$(gnu-tk.sh -c 'echo "\$VAR"')

if [ "$actual" != "$expected" ]; then
    echo "Unexpected at 1.B: $actual"
    exit 2
fi

# Case 1.C

actual=$(gnu-tk.sh -l sh -c 'echo "\$VAR"')

if [ "$actual" != "$expected" ]; then
    echo "Unexpected at 1.C: $actual"
    exit 2
fi

# Case 2.A

actual=$(sh -c 'echo "\\$VAR"')
expected='\ABC'

if [ "$actual" != "$expected" ]; then
    echo "Unexpected at 2.A: $actual"
    exit 2
fi
