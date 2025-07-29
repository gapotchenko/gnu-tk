#!/bin/sh

set -eu

cd a && ./run.sh && cd ..
echo
cd b && ./run.sh && cd ..
