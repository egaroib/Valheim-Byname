#!/usr/bin/env bash
# Compare the Thunderstore dependency pins in every mod's package/manifest.json
# against what Gale actually has installed in the dev profile, and against the
# Jotunn the decompile was taken from.
#
# setup.sh already catches a stale decompile via its stamp. Nothing caught the
# other two drifts, both of which are silent:
#
#   - a manifest pinning a Jotunn older than the one you build against, which
#     ships users a library mismatch (Jotunn enforces itself at Patch
#     strictness, so a pin that is one patch behind can stop peers connecting)
#   - a mod depending on a package that is not in the dev profile at all, so it
#     has never actually been exercised alongside it
#
# Read-only. Touches nothing but a temp copy of Gale's database.
set -euo pipefail

ROOT="${VALHEIM_WORKSPACE:-$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)}"
ENV_FILE="$ROOT/valheim.env"

[[ -f "$ENV_FILE" ]] || { echo "no valheim.env at $ENV_FILE - run scripts/setup.sh first" >&2; exit 1; }
# shellcheck disable=SC1090
set -a; source "$ENV_FILE"; set +a

[[ -n "${GALE_PROFILE:-}" ]] || { echo "GALE_PROFILE is not set in valheim.env" >&2; exit 1; }

# Gale's database is the only authoritative record of installed versions: the
# unpacked plugin folders carry a manifest.json, but BepInEx itself does not,
# and a folder can linger after Gale drops the package.
# Walk up from the profile directory to Gale's data root, which sits above the
# per-game folder. The exact depth has changed between Gale versions.
GALE_DB=""
probe="$GALE_PROFILE"
while [[ "$probe" != "/" && -n "$probe" ]]; do
  probe="$(dirname "$probe")"
  if [[ -f "$probe/data.sqlite3" ]]; then GALE_DB="$probe/data.sqlite3"; break; fi
done
[[ -n "$GALE_DB" ]] || { echo "no data.sqlite3 found above $GALE_PROFILE" >&2; exit 1; }

# Copy the database with its write-ahead log before reading. Gale is usually
# running, and querying the main file alone would return a stale snapshot.
TMP="$(mktemp -d)"
trap 'rm -rf "$TMP"' EXIT
for f in "$GALE_DB" "$GALE_DB-wal" "$GALE_DB-shm"; do
  [[ -f "$f" ]] && cp "$f" "$TMP/"
done

# The decompiled Jotunn's own version constant, for the staleness check below.
DECOMPILED_JOTUNN="$(sed -n 's/.*public const string Version = "\(.*\)";.*/\1/p' \
  "$ROOT/.valheim-src/jotunn/Jotunn/Main.cs" 2>/dev/null | head -1)"

GALE_DB="$TMP/$(basename "$GALE_DB")" \
GALE_PROFILE="$GALE_PROFILE" \
ROOT="$ROOT" \
DECOMPILED_JOTUNN="$DECOMPILED_JOTUNN" \
python3 - <<'PY'
import glob, json, os, sqlite3, sys

root = os.environ["ROOT"]
profile = os.environ["GALE_PROFILE"].rstrip("/")

con = sqlite3.connect(os.environ["GALE_DB"])
rows = con.execute("select name, path, mods from profiles where game_slug = 'valheim'").fetchall()
con.close()

# Match on path rather than name: valheim.env points at a directory, and two
# profiles may not share one.
match = next((r for r in rows if r[1].rstrip("/") == profile), None)
if match is None:
    match = next((r for r in rows if os.path.basename(r[1].rstrip("/")) == os.path.basename(profile)), None)
if match is None:
    sys.exit(f"no Gale profile registered at {profile}")

name, _, mods_json = match
installed = {}
for mod in json.loads(mods_json):
    # fullName is "author-Name-version", the same shape a manifest dependency
    # uses. Version is the last hyphenated field; the name may contain hyphens.
    pkg, _, version = mod["fullName"].rpartition("-")
    installed[pkg] = (version, mod.get("enabled", True))

print(f"Dev profile: {name}")
print(f"  {len(installed)} packages installed\n")

problems = 0

jotunn = installed.get("ValheimModding-Jotunn")
decompiled = os.environ.get("DECOMPILED_JOTUNN") or ""
if jotunn and decompiled:
    if jotunn[0] == decompiled:
        print(f"  ok     decompile matches installed Jotunn {decompiled}")
    else:
        print(f"  STALE  decompile is Jotunn {decompiled}, profile has {jotunn[0]}")
        print("         re-run scripts/setup.sh to refresh .valheim-src/")
        problems += 1
elif not decompiled:
    print("  WARN   no decompiled Jotunn found under .valheim-src/")
print()

for manifest_path in sorted(glob.glob(os.path.join(root, "mods", "*", "package", "manifest.json"))):
    mod_name = manifest_path.split(os.sep)[-3]
    with open(manifest_path) as fh:
        manifest = json.load(fh)

    lines = []
    for dep in manifest.get("dependencies", []):
        pkg, _, pinned = dep.rpartition("-")
        if pkg not in installed:
            lines.append(f"  ABSENT {pkg} pinned at {pinned}, not installed in this profile")
            problems += 1
            continue
        actual, enabled = installed[pkg]
        if actual != pinned:
            lines.append(f"  DRIFT  {pkg} pinned at {pinned}, profile has {actual}")
            problems += 1
        elif not enabled:
            lines.append(f"  OFF    {pkg} {pinned} is installed but disabled")

    if lines:
        print(mod_name)
        print("\n".join(lines))
        print()

if problems:
    print(f"{problems} issue(s). Update the profile through Gale, or correct the manifest pin.")
    sys.exit(1)

print("All manifest pins match the dev profile.")
PY
