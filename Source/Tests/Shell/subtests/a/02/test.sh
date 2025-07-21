#!/bin/sh

set -eu

echo Test A02
# Tests command line arguments passing.

expected="a b c"
actual=`gnu-tk.sh -l echo a b c`

if [ "$actual" != "$expected" ]
then
  echo "Unexpected: $actual"
  exit 2
fi
