#!/bin/sh

set -eu

chmod +x tools/*
export PATH=$PATH:`pwd`/tools

echo 123

my-gnu-tk
