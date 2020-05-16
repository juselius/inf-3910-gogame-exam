#!/bin/sh

host=$1; shift

curl -i -N -v \
    -H "Connection: Upgrade"\
    -H "Upgrade: websocket"\
    -H "Sec-WebSocket-Key: SGVsbG8sIHdvcmxkIQ=="\
    -H "Sec-WebSocket-Version: 13"\
    -H "Origin: http://127.0.0.1:8085/"\
    -H "Host: $host" $@

