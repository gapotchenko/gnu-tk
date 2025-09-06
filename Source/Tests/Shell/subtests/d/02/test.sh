#!/bin/sh

set -eu

echo "Test D02"

# Tests handling of '\' character for command.

actual=$(gnu-tk.sh -c 'echo "BODY" | sed "1i\\
HTTP/1.1 200 OK\\
Content-Type: text/plain\\
Cache-Control: public, max-age=3600\\
"')

expected="HTTP/1.1 200 OK
Content-Type: text/plain
Cache-Control: public, max-age=3600

BODY"

if [ "$actual" != "$expected" ]; then
    echo "Unexpected: $actual"
    exit 2
fi
