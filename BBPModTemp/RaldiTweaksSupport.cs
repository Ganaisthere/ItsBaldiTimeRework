using HarmonyLib;
using MTM101BaldAPI;
using RaldiTweaks.Patches;
using RaldiTweaks.PlayerModel;
using UnityEngine;

namespace ItsBaldiTimeRework
{
    [ConditionalPatchMod("il.modded.raldi.tweaks")]
    public class RaldiTweaksSupport
    {
        public static Color GetShirtColor()
        {
            if (Singleton<PlayerModel_Manager>.Instance == null)
            {
                return Color.green;
            }
            return Singleton<PlayerModel_Manager>.Instance.ShirtColor;
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
