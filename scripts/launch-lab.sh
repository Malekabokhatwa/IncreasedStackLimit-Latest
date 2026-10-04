#!/usr/bin/env bash
# Launch the IL2CPP lab copy windowed via umu-run (Steam must be running).
set -euo pipefail
GAME_DIR="${S1_IL2CPP_DIR:-$HOME/Games/Schedule I IL2CPP Beta}"
cd "$GAME_DIR"
export WINEPREFIX="${S1_PREFIX:-$HOME/Games/umu/schedule1-lab}"
export GAMEID=umu-3164500
export PROTONPATH="${PROTONPATH:-$HOME/.local/share/Steam/compatibilitytools.d/GE-Proton11-6-x86_64}"
export WINEDLLOVERRIDES="version=n,b"
exec umu-run "Schedule I.exe" -screen-fullscreen 0 -screen-width 1280 -screen-height 720 "$@"
