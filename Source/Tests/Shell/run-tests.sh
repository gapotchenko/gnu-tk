#!/bin/sh

set -eu

chmod +x **/*.sh
export PATH=$PATH:`pwd`/tools

gnu-tk.sh --version
gnu-tk.sh check -q

echo
echo Running subtests...
echo

cd subtests && ./run.sh && cd ..
