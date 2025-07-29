#!/bin/sh

set -eu

cd "$(dirname "$(readlink -fn "$0")")"

find . -type f -name "*.sh" -exec chmod +x {} \;
export PATH=$PATH:$(pwd)/tools

gnu-tk.sh --version
gnu-tk.sh check -q

echo
echo Running subtests...
echo

export GNU_TK_TEST_FILE_ASSET="${1-}"

cd subtests && ./run.sh && cd ..
