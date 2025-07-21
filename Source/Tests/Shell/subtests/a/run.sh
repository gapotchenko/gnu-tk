#!/bin/sh

set -eu

echo "Test Group A"

cd 01 && ./test.sh && cd ..
cd 02 && ./test.sh && cd ..
cd 03 && ./test.sh && cd ..
