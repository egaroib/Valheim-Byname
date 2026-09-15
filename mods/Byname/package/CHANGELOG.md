# Changelog

## 0.1.0

First build. Shared privately for testing.

- Titles generated from 254 fragments across 12 categories, drawn from 131 of Valheim's
  tracked player statistics.
- Grammar-based assembly with connector words, rarity-weighted slot selection, and a rule
  preventing two slots from coming from the same area of play.
- Titles recompute on login, waking, respawn and boss kills, and only change when the result
  actually differs. A title standing for 10 in-game days rerolls among equally-earned ones.
- Nameplate display on its own line or as a suffix, tinted by rarity; change announced by
  message and chat line; current title shown on the inventory screen.
- `Lifetime` or `World` stat scope, 12 admin-synced category toggles, and a per-fragment
  blocklist.
- Coexists with Groups and Guilds on the nameplate.
