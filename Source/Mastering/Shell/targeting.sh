#!/bin/sh

set -eu

SCRIPT_NAME=$(basename "$0")
cd "$(dirname "$(readlink -fn -- "$0")")"

# -----------------------------------------------------------------------------
# Options
# -----------------------------------------------------------------------------

OPT_COMMAND=
while [ $# -gt 0 ]; do
    case $1 in
    tfm | min-tfm)
        if [ -n "$OPT_COMMAND" ]; then
            echo "$SCRIPT_NAME: command already specified" >&2
            exit 1
        fi
        OPT_COMMAND=$1
        shift
        ;;
    *)
        echo "$SCRIPT_NAME: unknown option: $1" >&2
        exit 1
        ;;
    esac
done

if [ $# -eq 0 ] && [ -z "$OPT_COMMAND" ]; then
    echo "$SCRIPT_NAME: missing program arguments" >&2
    exit 1
fi

# -----------------------------------------------------------------------------
# Execution
# -----------------------------------------------------------------------------

sourcePath=../.NET/Targeting.props

case $OPT_COMMAND in
tfm)
    xq "$sourcePath" -x "//Project/PropertyGroup/MainTargetFramework"
    ;;
min-tfm)
    xq "$sourcePath" -x "//Project/PropertyGroup/MinimalTargetFramework"
    ;;
*)
    echo "$SCRIPT_NAME: unhandled command: $OPT_COMMAND" >&2
    exit 1
    ;;
esac
