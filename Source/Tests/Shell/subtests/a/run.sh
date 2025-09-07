#!/bin/sh
set -eu

echo "== Test Group A =="

(cd 01 && ./test.sh)
(cd 02 && ./test.sh)
(cd 03 && ./test.sh)
(cd 04 && ./test.sh)
(cd 05 && ./test.sh)
(cd 06 && ./test.sh)
(cd 07 && ./test.sh)
