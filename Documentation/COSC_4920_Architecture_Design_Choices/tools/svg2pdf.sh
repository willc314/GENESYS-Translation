#!/usr/bin/env bash
# Convert every SVG in images/ to PDF alongside it.
# NOTE: these diagrams are generated SVGs with live <text>, not draw.io exports,
# so rsvg-convert renders them correctly. (The UML document's draw.io exports
# hide their labels in a raster fallback; that caveat does not apply here.)
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
