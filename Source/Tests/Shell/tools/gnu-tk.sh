#!/bin/sh

set -eu

GNU_TK="${GNU_TK:-gnu-tk}"
exec "$GNU_TK" "$@"
