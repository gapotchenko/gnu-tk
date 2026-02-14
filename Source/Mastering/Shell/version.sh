#!/bin/sh

set -eu

SCRIPT_NAME=$(basename "$0")
cd "$(dirname "$(readlink -fn -- "$0")")"

# -----------------------------------------------------------------------------
# Options
# -----------------------------------------------------------------------------

OPT_COMMAND=""
while [ $# -gt 0 ]; do
    case $1 in
    semantic)
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

# -----------------------------------------------------------------------------
# Execution
# -----------------------------------------------------------------------------

sourcePath=../.NET/Version.props

version=$(xq "$sourcePath" -x //Project/PropertyGroup/Version)
if [ -z "$version" ]; then
    version=$(xq "$sourcePath" -x //Project/PropertyGroup/VersionPrefix)
    suffix=$(xq "$sourcePath" -x //Project/PropertyGroup/VersionSuffix)
    if [ -n "$suffix" ]; then
        version=$version-$suffix
    fi
fi

case $OPT_COMMAND in
"")
    echo "${version%%-*}"
    ;;
"semantic")
    echo "$version"
    ;;
*)
    echo "$SCRIPT_NAME: unhandled command: $OPT_COMMAND" >&2
    exit 1
    ;;
esac
