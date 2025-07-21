#!/bin/sh

set -eu

echo Test A03
# Tests script file arguments passing.

expected="a b c"
actual=`gnu-tk.sh -f script.sh a b c`

if [ "$actual" != "$expected" ]
then
  echo "Unexpected: $actual"
  exit 2
fi
