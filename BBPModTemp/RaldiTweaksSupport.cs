using HarmonyLib;
using MTM101BaldAPI;
using RaldiTweaks.Patches;
using RaldiTweaks.PlayerModel;

namespace ItsBaldiTimeRework
{
    [ConditionalPatchMod("il.modded.raldi.tweaks")]
    public class RaldiTweaksSupport
    {
        public static void ChangeColor()
        {
            if (BaldiTimeUI.ComboBar.gameObject == null)
            {
                return;
            }
            if (Singleton<PlayerModel_Manager>.Instance == null)
            {
                return;
            }
            BaldiTimeUI.ComboBarOverlay.color = Singleton<PlayerModel_Manager>.Instance.ShirtColor;
            BaldiTimeUI.TimerBarOverlay.color = Singleton<PlayerModel_Manager>.Instance.ShirtColor;
        }
    }

    [ConditionalPatchMod("il.modded.raldi.tweaks")]
    [HarmonyPatch(typeof(MGM_Patch))]
    public static class MGM_Patch_Patch
    {
        [HarmonyPatch("Postfix")]
        [HarmonyPostfix]
        public static void Postfix()
        {
            Singleton<MusicManager>.Instance.StopMidi();
            //RaldiTweaks.Tweaks_Plugin
        }
    }
}
