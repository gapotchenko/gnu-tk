#!/bin/sh

set -eu

echo "== Test Group D =="

(cd 01 && ./test.sh)
(cd 02 && ./test.sh)
