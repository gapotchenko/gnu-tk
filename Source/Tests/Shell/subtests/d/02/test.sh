#!/bin/sh

set -eu

# ===========================================================================
echo "Test D02"
# Tests handling of LF and '\' characters for command-line arguments.
# ===========================================================================

# ---------------------------------------------------------------------------
# Case 1
# Handling of LF and '\' characters for a command.
# ---------------------------------------------------------------------------

actual=$(gnu-tk.sh -c 'echo CGI | sed "1i\\
HTTP/1.1 200 OK\\
Content-Type: text/plain\\
Cache-Control: public, max-age=3600\\

"')

expected="HTTP/1.1 200 OK
Content-Type: text/plain
Cache-Control: public, max-age=3600

CGI"

if [ "$actual" != "$expected" ]; then
    echo "Unexpected 1: $actual"
    exit 2
fi

# ---------------------------------------------------------------------------
# Case 2
# Handling of LF and '\' characters for a command line.
# ---------------------------------------------------------------------------

actual=$(gnu-tk.sh -l sh -c 'echo CGI | sed "1i\\
HTTP/1.1 200 OK\\
Content-Type: text/plain\\
Cache-Control: public, max-age=3600\\

"')

if [ "$actual" != "$expected" ]; then
    echo "Unexpected 2: $actual"
    exit 2
fi
