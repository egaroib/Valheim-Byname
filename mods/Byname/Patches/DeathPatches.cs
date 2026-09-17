using System;
using Byname.Stats;
using HarmonyLib;
using UnityEngine;

namespace Byname.Patches
{
    /// <summary>
    /// Remembers the last creature to hurt the local player.
    ///
    /// Needed because the blow that actually kills often names nobody. SE_Poison builds
    /// its damage tick with no attacker at all, so a player killed by a blob's poison dies
    /// to a HitData whose GetAttacker() is null — and poison is the swamp's signature way
    /// of killing people. Character.ApplyDamage runs on the damaged character's owner,
    /// which for your own character is your own client.
    /// </summary>
    [HarmonyPatch(typeof(Character), nameof(Character.ApplyDamage))]
    internal static class RecentAttackerTracker
    {
        /// <summary>How long a creature stays to blame after its last hit. Covers a full poison tick-down.</summary>
        internal const float BlameWindowSeconds = 45f;

        internal static string LastToken;
        internal static float LastTime = float.NegativeInfinity;

        private static void Prefix(Character __instance, HitData hit)
        {
            // Reference compare first: this runs for every hit on every character this
            // client owns, and almost none of them are the local player.
            if (__instance != Player.m_localPlayer || hit == null) return;

            var attacker = hit.GetAttacker();
            if (attacker == null || attacker.IsPlayer()) return;

            LastToken = attacker.m_name;
            LastTime = Time.time;
        }
    }

    /// <summary>
    /// Writes each death of the local player into the DeathLedger: the creature to blame
    /// and the biome it happened in.
    ///
    /// A prefix, so m_lastHit is read before anything in OnDeath can touch it, and guarded
    /// exactly as OnDeath guards itself (owner only) so the ledger counts the same deaths
    /// the game's own Deaths stat does.
    /// </summary>
    [HarmonyPatch(typeof(Player), nameof(Player.OnDeath))]
    internal static class DeathRecorder
    {
        private static void Prefix(Player __instance)
        {
            if (__instance != Player.m_localPlayer) return;
            if (__instance.m_nview == null || !__instance.m_nview.IsOwner()) return;

            // Cosmetic bookkeeping must never be why a death throws.
            try
            {
                var killer = ResolveKiller(__instance);
                var biome = __instance.GetCurrentBiome();
                var worldUid = ZNet.instance != null ? ZNet.instance.GetWorldUID() : 0L;

                DeathLedger.Record(__instance, worldUid, killer, biome);

                // One creature, one death. Without this a troll that killed you could still
                // be blamed for the cliff you fall off right after respawning.
                RecentAttackerTracker.LastTime = float.NegativeInfinity;
                BynamePlugin.LogVerbose($"death recorded: killer {killer ?? "<none>"}, biome {biome}");
            }
            catch (Exception e)
            {
                BynamePlugin.LogError($"could not record death: {e.Message}");
            }
        }

        private static string ResolveKiller(Player player)
        {
            var hit = player.m_lastHit;
            if (hit != null)
            {
                // Your own doing, or another player's. Neither is a creature to blame.
                if (hit.m_hitType == HitData.HitType.PlayerHit || hit.m_hitType == HitData.HitType.Self)
                {
                    return null;
                }

                var attacker = hit.GetAttacker();
                if (attacker != null) return attacker.IsPlayer() ? null : attacker.m_name;
            }

            // Poison, burning, or a fall while fleeing: blame whatever hurt you last, if it
            // did so recently enough to plausibly be responsible.
            return Time.time - RecentAttackerTracker.LastTime <= RecentAttackerTracker.BlameWindowSeconds
                ? RecentAttackerTracker.LastToken
                : null;
        }
    }
}
