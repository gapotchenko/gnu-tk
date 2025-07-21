#!/bin/sh

set -eu

find . -type f -name "*.sh" -exec chmod +x {} \;
export PATH=$PATH:`pwd`/tools

gnu-tk.sh --version
gnu-tk.sh check -q

echo
echo Running subtests...
echo

cd subtests && ./run.sh && cd ..
