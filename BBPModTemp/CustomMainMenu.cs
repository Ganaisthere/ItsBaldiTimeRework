using CustomMainMenusAPI;
using MTM101BaldAPI;
using UnityEngine;

namespace ItsBaldiTimeRework
{
    [ConditionalPatchMod("CustomMainMenusAPI")]
    public class CustomMainMenu
    {
        public static string localized = "Men_BaldiTimeMenu";
        public static Sprite sprite;
        //public static SoundObject sound;
        public static string midi = "custom_TitlePEPBRMG";

        public static void Setup()
        {
            MainMenuObject.CreateMenuObject(localized, sprite, midi);
        }
    }
}
