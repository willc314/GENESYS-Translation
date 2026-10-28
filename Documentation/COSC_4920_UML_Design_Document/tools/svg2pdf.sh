#!/usr/bin/env bash
# Convert every SVG in images/ to PDF alongside it.
# Run this if you'd rather not compile with -shell-escape, or if Inkscape
# isn't available. The figure macro prefers a PDF when one exists.
set -euo pipefail
cd "$(dirname "$0")/../images"
shopt -s nullglob
for f in *.svg; do
  out="${f%.svg}.pdf"
  if   command -v rsvg-convert >/dev/null; then rsvg-convert -f pdf -o "$out" "$f"
  elif command -v inkscape      >/dev/null; then inkscape "$f" --export-type=pdf --export-filename="$out"
  else echo "Install librsvg2-bin or inkscape first." >&2; exit 1
  fi
  echo "  $f -> $out"
done
