#!/bin/sh

set -eu

echo "Test Group B"

cd 01 && ./test.sh && cd ..
