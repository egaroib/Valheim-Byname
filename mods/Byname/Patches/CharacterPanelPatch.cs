using Byname.Config;
using Byname.Net;
using Byname.Titles;
using HarmonyLib;

namespace Byname.Patches
{
    /// <summary>
    /// Shows the local player their own title on the inventory screen.
    ///
    /// This exists because EnemyHud.LateUpdate skips the local player outright
    /// (<c>if (!(allCharacter == localPlayer) ...)</c>), so a player never sees their own
    /// nameplate and would otherwise never see the title they earned. The change toast
    /// announces the moment; this is where they look afterwards.
    ///
    /// UpdateCharacterStats reassigns m_playerName.text from the profile on every call,
    /// so appending here is safe and self-cleaning rather than compounding each frame.
    /// </summary>
    [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.UpdateCharacterStats))]
    internal static class CharacterPanelPatch
    {
        private static void Postfix(InventoryGui __instance, Player player)
        {
            if (!BynameConfig.Enabled.Value) return;
            if (!BynameConfig.ShowOnCharacterPanel.Value) return;
            if (__instance.m_playerName == null || player == null) return;

            var title = TitleNet.Read(player, out var rarity);
            if (string.IsNullOrEmpty(title)) return;

            __instance.m_playerName.text +=
                $"\n<size=70%>{TitleService.Decorate(title, rarity)}</size>";
        }
    }
}
