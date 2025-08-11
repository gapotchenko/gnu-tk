#!/bin/sh

set -eu

(cd a && ./run.sh)
echo
(cd b && ./run.sh)
