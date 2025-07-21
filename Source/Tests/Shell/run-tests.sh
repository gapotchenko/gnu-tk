#!/bin/sh

set -eu

chmod +x tools/*.sh
export PATH=$PATH:`pwd`/tools

gnu-tk.sh --version
gnu-tk.sh check -q

echo
echo Running subtests...

cd subtests && ./run.sh && cd ..
