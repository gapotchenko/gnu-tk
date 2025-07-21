#!/bin/sh

set -eu

chmod +x tools/*.sh
export PATH=$PATH:`pwd`/tools

echo 123

gnu-tk.sh --version
