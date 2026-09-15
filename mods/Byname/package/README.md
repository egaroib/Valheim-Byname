# Byname

A *byname* — Old Norse *viðrnefni* — is the descriptive nickname a Norse person earned from
their deeds. Erik the Red. Ivar the Boneless. Harald Bluetooth.

Byname gives every player one, generated from what they have actually done in Valheim.
Nobody picks their title. You get the one your saga earned you, and it changes as you play.

> **Sodden Beekeeper** · **The Bare-Fisted Gravewalker** · **Helmsman of the Far Shore**
> **Serpent-Touched Great-Catcher** · **Jarl of the Nine Crowns** · **The Unproven Stranger**

Titles appear on the second line of a player's nameplate, tinted by how rare they are.

## Who needs to install this

**Every player, and the server.**

Valheim keeps player statistics in each player's own local character file — no peer can read
anyone else's. So each client works out its own title and publishes it for others to see. A
player without Byname installed neither publishes a title nor sees anyone else's.

The server needs it because the server owns the rules: which categories of title are allowed,
whether titles are judged on a character's whole life or only on this world, and how often a
stale title rerolls. Those settings are admin-synced, so a client cannot quietly switch a
category back on.

Byname is declared `ClientMustHaveMod`. A server running it requires it of everyone who
joins. A Byname client can still join a vanilla server — there will simply be no titles.

## How titles are built

A title is assembled from a grammar, not picked from a list. Each slot is filled from a
different area of your play, so a title describes more than one thing about you:

```
{epithet} {noun}                   →  Sodden Beekeeper
The {epithet}                      →  The Unflinching
{noun} of the {domain}             →  Angler of the Deep
{epithet} {noun} of the {domain}   →  Quiet Tiller of the Green Hall
```

254 fragments are drawn from 131 of Valheim's tracked statistics — deaths by cause, which
creatures you kill and with what, trees felled by species and by the axe they needed, fish by
quality, the largest hall you have ever raised, bees kept, sap drawn, distance sailed, how
far out you have pushed the map without a map.

Two rules keep the results honest: rarer deeds win their slot, and no two slots may come from
the same area of play — so nobody ends up "Slaying Slayer of the Slain".

## When your title changes

Titles are recomputed on **login, waking from sleep, respawning, and felling a boss**.

Recomputing is not the same as changing. If nothing notable has happened, the result is
identical and nothing is announced — your title does not churn every time you sleep.

The exception is staleness: if a title has stood for **10 in-game days** (configurable), it
deliberately rerolls among the titles you equally deserve, so a long-settled character still
sees their story move. It never demotes you to something you did not earn.

Valheim never draws a nameplate for your own character, so Byname shows you your own title in
two other places: a message when it changes, and a line under your name on the inventory
screen.

## Commands

`/byname` in chat, or `byname` in the console, explains your current title:

```
You are Drowned of the Green Hall  (Rare)

  Drowned            death by drowning: 12 (needed 8)
  Green Hall         harvest crop: 520 (needed 400)

Standing since day 84 — rerolls on day 94 if nothing changes.
```

It reads your own statistics only, changes nothing, and needs no `devcommands`.

It deliberately does not tell you what you are close to earning. A byname is a record of how
you played, not a list of chores — knowing the next threshold would turn it into one.

## Requirements

- [BepInExPack Valheim](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/)
- [Jotunn](https://valheim.thunderstore.io/package/ValheimModding/Jotunn/)

## Installation

Install with a mod manager, or drop `Byname.dll` into `BepInEx/plugins/`.

## Configuration

`BepInEx/config/com.ragemedia.byname.cfg`, generated on first launch.

### Rules — server-controlled (admin only)

| Setting | Default | What it does |
|---|---|---|
| `Enabled` | `true` | Master switch |
| `StatScope` | `Lifetime` | `Lifetime` judges a character on everything it has ever done, in any world, and titles existing characters retroactively. `World` counts only deeds done since first joining this server, so everyone starts unproven |
| `StalenessDays` | `10` | In-game days a title may stand before it deliberately rerolls. `0` never rerolls |
| `MaxTitleLength` | `32` | Longest rendered title. Longer combinations are skipped in favour of shorter ones |
| `Blocklist` | empty | Comma-separated fragment ids never to use, for removing one title without disabling its whole category |

### Categories — server-controlled (admin only)

One switch each for `Combat`, `Bosses`, `Building`, `Death`, `Harvest`, `Travel`,
`Exploration`, `Taming`, `Fishing`, `Cooking`, `Crafting`, `Misc`. All on by default. Turn off
the ones that do not suit your server — a serious roleplay server might drop `Death`; a
builder server might drop `Combat`.

### Appearance — each player's own

| Setting | Default | What it does |
|---|---|---|
| `Placement` | `SecondLine` | `SecondLine` gives the title its own line under the name. `Suffix` appends it to the name instead, adding no vertical space |
| `SecondLineOffset` | `-12` | How far below the name the title sits |
| `ShowRarityColor` | `true` | Tint the title by rarity |
| `ShowChangeToast` | `true` | Centre-screen message when your title changes |
| `ShowChangeInChat` | `true` | Also write it to your chat window, where it stays in the scrollback. Local only — nothing is sent to the server |
| `ShowOnCharacterPanel` | `true` | Show your title on the inventory screen |
| `VerboseLogging` | `false` | Log every qualifying fragment and your closest near-misses |

## Compatibility

**Groups** and **Guilds** (blaxxun-boop) both draw on the player nameplate, and Byname is
built to share it with them:

- Groups tints the player *name* to mark group members. Byname only ever colours the *title*,
  never the name, so that highlight keeps working.
- Guilds adds a guild tag below the name. Byname detects it at runtime and stacks below it
  rather than on top of it.

If three stacked lines look cramped on your screen, set `Placement = Suffix`. That puts the
title on the name's own line and cannot collide with anything.
