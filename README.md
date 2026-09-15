# Byname

A Valheim mod that gives every player a title generated from what they have actually done.

A *byname* — Old Norse *viðrnefni* — is the descriptive nickname a Norse person earned from
their deeds. Erik the Red. Ivar the Boneless. Nobody picks their title here either: you get
the one your saga earned you, and it changes as you play.

> **Sodden Beekeeper** · **The Bare-Fisted Gravewalker** · **Helmsman of the Far Shore**
> **Serpent-Touched Great-Catcher** · **Jarl of the Nine Crowns** · **The Unproven Stranger**

Player-facing documentation — installation, every config setting, compatibility — lives in
[`mods/Byname/package/README.md`](mods/Byname/package/README.md), which is also what ships
inside the Thunderstore package.

## How it works

Valheim already tracks 205 per-player statistics for its achievement system: deaths broken
out by cause, kills per creature and per weapon class, trees felled by species and by the axe
tier they needed, fish by quality, bees harvested, sap drawn, distance walked, run, sailed and
flown, how far out the map has been pushed in each compass direction. Byname reads that data
and builds a title from it.

Titles are assembled from a grammar rather than picked from a list:

```
{epithet} {noun}                   →  Sodden Beekeeper
The {epithet}                      →  The Unflinching
{noun} of the {domain}             →  Angler of the Deep
{epithet} {noun} of the {domain}   →  Quiet Tiller of the Green Hall
```

254 fragments across 12 categories, each with a condition and a rarity. The rarest qualifying
fragment wins its slot, and no two slots may be drawn from the same category — which is what
stops every fighter converging on "Slaying Slayer".

Stats live in each player's local character file, so no peer can compute anyone else's title.
Each client works out its own and publishes it to its own Player ZDO; peers read it from
there. **Every player needs the mod, and so does the server**, which owns the admin-synced
rules.

## Layout

```
mods/Byname/
  Plugin.cs               BepInEx entry point, Harmony bind verification
  Config/                 All settings; game rules are admin-synced, appearance is local
  Stats/                  Lifetime vs per-world stat sources, per-character persistence
  Titles/                 The grammar, the selection engine, and the fragment catalog
  Net/                    Publishing a title onto the player's ZDO
  Patches/                Six Harmony patches: triggers and display
  package/                Thunderstore package contents

tools/title-preview/      Prints generated titles for synthetic players, no game needed
```

## Building

The mod targets `net462` and compiles against publicized Valheim assemblies plus BepInEx,
HarmonyX and Jötunn from a local [Gale](https://github.com/Kesomannen/gale) profile.

`Directory.Build.props` imports `Environment.props`, which is machine-specific and therefore
not committed. It defines `ValheimManaged`, `ValheimBepInExCore` and `JotunnDll` as paths into
your own Steam install and mod profile. Create it before building.

## Previewing titles without launching the game

```bash
cd tools/title-preview && dotnet run            # titles for a dozen synthetic players
cd tools/title-preview && dotnet run coverage    # catalog coverage and untouched stats
```

This compiles the real engine and catalog straight out of `mods/Byname`, so it cannot drift
out of step with the mod. It is how the fragment catalog gets tuned: editing a threshold and
re-checking the whole spread takes about two seconds instead of a play session.
