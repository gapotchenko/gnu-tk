#!/bin/sh

set -eu

GNU_TK_TEST_FILE_ASSET="${1-}"
if [ -n "$GNU_TK_TEST_FILE_ASSET" ]; then
    GNU_TK_TEST_FILE_ASSET="$(readlink -fn -- "$GNU_TK_TEST_FILE_ASSET")"
fi

cd "$(dirname "$(readlink -fn -- "$0")")"

find . -type f -name "*.sh" -exec chmod +x {} \;
export PATH="$PATH:$(pwd)/tools"

gnu-tk.sh --version
gnu-tk.sh check -q

echo
echo Running subtests...
echo

export GNU_TK_TEST_FILE_ASSET

(cd subtests && ./run.sh) || {
    rc=$?
    echo >&2
    echo "TEST FAILED" >&2
    exit "$rc"
}

echo
echo "TEST PASSED"
