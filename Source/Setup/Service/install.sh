#!/bin/sh

set -eu

tag=$(curl -fsSL https://api.github.com/repos/gapotchenko/gnu-tk/releases/latest | grep tag_name | cut -d'"' -f4)

echo "$tag"

echo TODO
