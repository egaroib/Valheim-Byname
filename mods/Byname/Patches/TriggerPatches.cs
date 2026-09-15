using Byname.Titles;
using HarmonyLib;

namespace Byname.Patches
{
    /// <summary>
    /// Login and respawn. Player.OnSpawned(bool) runs on both paths, so one hook covers
    /// "I just joined" and "I just got up from my tombstone".
    /// </summary>
    [HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
    internal static class SpawnTrigger
    {
        private static void Postfix(Player __instance)
        {
            TitleService.Evaluate(__instance, "spawn");
        }
    }

    /// <summary>
    /// Waking up.
    ///
    /// SetSleeping's own body is guarded by <c>if (m_sleeping != sleep)</c>, but a Harmony
    /// postfix runs whether or not that guard passed. So the previous state is captured in
    /// a prefix and the trigger only fires on a real asleep -> awake transition, rather
    /// than on every redundant call.
    /// </summary>
    [HarmonyPatch(typeof(Player), nameof(Player.SetSleeping))]
    internal static class WakeTrigger
    {
        private static void Prefix(Player __instance, out bool __state)
        {
            __state = __instance.m_sleeping;
        }

        private static void Postfix(Player __instance, bool sleep, bool __state)
        {
            if (!sleep && __state)
            {
                TitleService.Evaluate(__instance, "wake");
            }
        }
    }

    /// <summary>
    /// Felling a boss.
    ///
    /// Game.RPC_RegisterKill is routed to the killing player's own client and is where
    /// BossKills / BossKillSolo / BossKillMultiplayer are incremented (Game.cs:1079). As a
    /// postfix this runs after those increments, so the new totals are already visible to
    /// the evaluation. bossNumber is greater than zero only for the eight bosses.
    /// </summary>
    [HarmonyPatch(typeof(Game), nameof(Game.RPC_RegisterKill))]
    internal static class BossKillTrigger
    {
        private static void Postfix(int bossNumber)
        {
            if (bossNumber <= 0) return;
            TitleService.Evaluate(Player.m_localPlayer, $"boss {bossNumber} felled");
        }
    }
}
