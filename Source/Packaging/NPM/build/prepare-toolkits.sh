#!/usr/bin/env bash

set -eu -o pipefail

cd ../gnu-tk

rm -rf toolkits && mkdir toolkits

basePath=../../../../Toolkits

# ---------------------------------------------------------------------------
# BusyBox
# ---------------------------------------------------------------------------

mkdir toolkits/busybox
cp -r -l "$basePath/BusyBox"/{*-*,*.md} toolkits/busybox
