#!/usr/bin/env bash

set -eu -o pipefail

cd ../contents

rm -rf toolkits && mkdir toolkits

basePath=../../../../Toolkits

# ---------------------------------------------------------------------------
# BusyBox
# ---------------------------------------------------------------------------

mkdir toolkits/busybox
cp -r -l "$basePath/BusyBox"/{*-*,*.md} toolkits/busybox
