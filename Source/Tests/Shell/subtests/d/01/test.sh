#!/bin/sh
set -eu

# ===========================================================================
echo "Test D01"
# Tests handling of '\' character for command-line arguments.
# ===========================================================================

export VAR=ABC

# ---------------------------------------------------------------------------
# Case 1
# Handling of an escaped '$' symbol.
# ---------------------------------------------------------------------------

# Case 1.A

actual=$(sh -c 'echo "\$VAR"')
expected='$VAR'

if [ "$actual" != "$expected" ]; then
    echo "Unexpected 1.A: $actual"
    exit 2
fi

# Case 1.B

actual=$(gnu-tk.sh -c 'echo "\$VAR"')

if [ "$actual" != "$expected" ]; then
    echo "Unexpected 1.B: $actual"
    exit 2
fi

# Case 1.C

actual=$(gnu-tk.sh -l sh -c 'echo "\$VAR"')

if [ "$actual" != "$expected" ]; then
    echo "Unexpected 1.C: $actual"
    exit 2
fi

# Case 1.D

actual=$(gnu-tk.sh -l echo '$VAR')

if [ "$actual" != "$expected" ]; then
    echo "Unexpected 1.D: $actual"
    exit 2
fi

# ---------------------------------------------------------------------------
# Case 2
# Handling of an escaped '\' symbol.
# ---------------------------------------------------------------------------

# Case 2.A

actual=$(sh -c 'echo "\\$VAR"')
expected='\ABC'

if [ "$actual" != "$expected" ]; then
    echo "Unexpected 2.A: $actual"
    exit 2
fi

# Case 2.B

actual=$(gnu-tk.sh -c 'VAR=ABC; echo "\\$VAR"')

if [ "$actual" != "$expected" ]; then
    echo "Unexpected 2.B: $actual"
    exit 2
fi

# Case 2.C

actual=$(gnu-tk.sh -l sh -c 'VAR=ABC; echo "\\$VAR"')

if [ "$actual" != "$expected" ]; then
    echo "Unexpected 2.C: $actual"
    exit 2
fi
