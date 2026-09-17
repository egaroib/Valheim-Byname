# title-preview

Prints the titles Byname would generate for a set of synthetic players, without
launching Valheim.

```bash
cd tools/title-preview && dotnet run
```

Other modes:

```bash
dotnet run -- server        # three friends on one server, across four rerolls
dotnet run -- ladders     # every creature kill ladder, and one player climbing it
dotnet run -- lint          # duplicate ids, words shared between fragments, words too long to fit
dotnet run -- explain       # what /byname prints
dotnet run -- coverage      # fragments per category and slot, stats still unused
dotnet run -- combinations  # how many distinct titles the grammar can render
```

## Why it exists

Tuning the fragment catalog means answering two questions repeatedly: *does this read
well*, and *is this threshold set anywhere near right*. Answering either in game costs a
launch, a load, and a character that happens to have the right stats. Answering it here
costs about two seconds.

It compiles the **real** `TitleEngine`, `TitleModel`, `FragmentCatalog` and everything
under `Titles/Catalog/` directly from `mods/Byname`. It is not a reimplementation, so it
cannot drift out of step with the mod. Only the parts that genuinely need the game are
stubbed:

| Stub | Why |
|---|---|
| `shims/PlayerStatType.cs` | Copied verbatim from the decompiled game |
| `shims/KillModifiers.cs` | Same |
| `shims/Shims.cs` | Stands in for `BynameConfig` and `BynamePlugin`. Category weights come from `CategoryDefaults` in the mod itself, so they cannot drift |

Because the config is stubbed, this shows behaviour at **default settings**. It does not
exercise admin category toggles, the blocklist, or World scope — those live in code paths
that need BepInEx and a character save.

## Editing the players

The archetypes are a plain array at the top of `Program.cs`: a name and a bag of
`PlayerStatType` values. Add a row to test a play style you care about. `FakeStats` also
takes a `KillModifiers` so you can model someone who kills mostly unarmed or mostly ranged.

The second section advances the epoch, which is what a staleness reroll does — use it to
check that a player who sits at the same title for ten in-game days lands somewhere worth
landing.

## Refreshing the stubbed enums

The two enum files are vendored because `.valheim-src/` is gitignored and regenerable.
After a Valheim update adds new stats, re-copy them:

```bash
cp ../../.valheim-src/valheim/PlayerStatType.cs ../../.valheim-src/valheim/KillModifiers.cs shims/
```

## Pointing it at another mod

```bash
dotnet run -p:ModRoot=../../mods/SomeOtherMod
```

Only useful for a mod laid out with the same `Titles/` and `Stats/` structure.
