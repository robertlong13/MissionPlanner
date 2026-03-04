#!/usr/bin/env python3
"""Send commands to a TCP debug listener and print responses.

Usage:
    python scripts/tcp_cmd.py "state"
    python scripts/tcp_cmd.py "sub 30 10 test" "wait 2" "rates 30" "release 1"
    python scripts/tcp_cmd.py --port 7400 "help"

"wait N" is handled client-side (sleeps N seconds without sending anything).
"""
import argparse
import os
import socket
import sys
import time

os.environ.setdefault("PYTHONIOENCODING", "utf-8")
sys.stdout.reconfigure(encoding="utf-8", errors="replace")


def recv_response(sock):
    """Read one response (possibly multi-line) from the server."""
    lines = []
    sock.settimeout(2)
    while True:
        try:
            data = sock.recv(4096)
            if not data:
                break
            lines.append(data.decode("utf-8", errors="replace"))
            sock.settimeout(0.3)
        except socket.timeout:
            break
    return "".join(lines).rstrip()


def main():
    parser = argparse.ArgumentParser(description="TCP command sender")
    parser.add_argument("commands", nargs="+", help="Commands to send")
    parser.add_argument("--port", type=int, default=7400)
    parser.add_argument("--host", default="127.0.0.1")
    args = parser.parse_args()

    with socket.create_connection((args.host, args.port), timeout=10) as sock:
        # Read welcome banner
        banner = recv_response(sock)
        print(banner)

        for cmd in args.commands:
            # Client-side wait
            if cmd.startswith("wait "):
                secs = float(cmd.split()[1])
                print(f"> {cmd}")
                time.sleep(secs)
                continue

            sock.sendall((cmd + "\n").encode())
            response = recv_response(sock)
            print(f"> {cmd}")
            if response:
                for line in response.splitlines():
                    print(f"  {line}")

        sock.sendall(b"quit\n")


if __name__ == "__main__":
    main()
