#!/bin/sh

set -eu

(cd a && ./run.sh)
echo
(cd b && ./run.sh)
echo
(cd c && ./run.sh)
echo
(cd d && ./run.sh)
