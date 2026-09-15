namespace Byname.Net
{
    /// <summary>
    /// Publishes the local player's title where other players can read it.
    ///
    /// Valheim keeps player stats in the local character file, so no peer can compute
    /// anyone else's title. Each client computes its own and writes it into its own
    /// Player ZDO; ZDOs replicate to every peer that has that player loaded, which is
    /// exactly the set of peers close enough to see a nameplate. Nothing is sent to the
    /// server beyond the ordinary ZDO traffic Valheim already does.
    /// </summary>
    internal static class TitleNet
    {
        private static readonly int TitleKey = "byname_title".GetStableHashCode();
        private static readonly int RarityKey = "byname_rarity".GetStableHashCode();

        /// <summary>
        /// Writes the title onto the local player's own ZDO. Silently does nothing when
        /// the view is not ours to write to, which is the normal state for a remote
        /// player and is not an error.
        /// </summary>
        internal static void Publish(Player player, string title, int rarity)
        {
            if (player == null || player.m_nview == null) return;
            if (!player.m_nview.IsValid() || !player.m_nview.IsOwner()) return;

            var zdo = player.m_nview.GetZDO();
            if (zdo == null) return;

            zdo.Set(TitleKey, title ?? "");
            zdo.Set(RarityKey, rarity);
        }

        /// <summary>Reads any player's published title, local or remote. Null when unset.</summary>
        internal static string Read(Player player, out int rarity)
        {
            rarity = 1;
            if (player == null || player.m_nview == null || !player.m_nview.IsValid()) return null;

            var zdo = player.m_nview.GetZDO();
            if (zdo == null) return null;

            var title = zdo.GetString(TitleKey, "");
            if (string.IsNullOrEmpty(title)) return null;

            rarity = zdo.GetInt(RarityKey, 1);
            return title;
        }
    }
}
