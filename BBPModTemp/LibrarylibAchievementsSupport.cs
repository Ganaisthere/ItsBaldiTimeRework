using LibraryLibrary.AchievementsAPI;
using MTM101BaldAPI;
using System.Collections.Generic;

namespace ItsBaldiTimeRework
{
    [ConditionalPatchMod("skid.librarylib")]
    public class LibrarylibAchievementsSupport
    {
        internal static void CreateAchievements()
        {
            void AA(string Key, AchievementSection AS = AchievementSection.None, AchievementRarity AR = AchievementRarity.None, bool secret = false)
            {
                LLAchievement lL = new LLAchievement(
                    "Ach_" + Key,
                    "Ach_" + Key + "_Desc",
                    "BaldiTimeRE_" + Key,
                    AS,
                    AR,
                    secret
                    );

                LLAchievementManager.AddAchievement(
                    "BaldiTimeRE_" + Key,
                    lL
                    );

                //AllBaldiTimeAchievements.Add(lL);
            }
            //AllBaldiTimeAchievements.Clear();

            AA("StudentSavior", AchievementSection.Characters, AchievementRarity.Uncommon);

            AA("SecretExplorer", AchievementSection.Rooms, AchievementRarity.Rare);

            //AA("Treasure", AchievementSection.Items, AchievementRarity.Epic);

            AA("DRank", AchievementSection.Floors, AchievementRarity.Rare);
            AA("SRank", AchievementSection.Floors, AchievementRarity.Rare);
            AA("PRank", AchievementSection.Floors, AchievementRarity.Epic);
            AA("LostPRank", AchievementSection.Floors, AchievementRarity.Uncommon);
            //AA("LRank", AchievementSection.Floors, AchievementRarity.Legendary);

            AA("Lap1", AchievementSection.Events, AchievementRarity.Common);
            AA("Lap2", AchievementSection.Events, AchievementRarity.Common);
            AA("Lap3", AchievementSection.Events, AchievementRarity.Uncommon);
            AA("Lap4", AchievementSection.Events, AchievementRarity.Uncommon);
            //AA("Lap5", AchievementSection.Events, AchievementRarity.Rare);
            //AA("Lap6", AchievementSection.Events, AchievementRarity.Rare);
            //AA("Lap7", AchievementSection.Events, AchievementRarity.Epic);
            //AA("Lap8", AchievementSection.Events, AchievementRarity.Epic);
            //AA("Lap9", AchievementSection.Events, AchievementRarity.Legendary);
            //AA("Lap10", AchievementSection.Events, AchievementRarity.Mythic);

            AA("CrackTower", AchievementSection.Other, AchievementRarity.Uncommon);
            AA("OtherTower", AchievementSection.Other, AchievementRarity.Epic);
            AA("HiresTower", AchievementSection.Other, AchievementRarity.Legendary);
            AA("MusicTower", AchievementSection.Other, AchievementRarity.Rare);
            AA("MilkTower", AchievementSection.Other, AchievementRarity.Epic);
        }

        public static void RewardAchievement(string Key, bool playAnimaton = true)
        {
            string get = Key;
            if (!get.StartsWith("BaldiTimeRE_"))
            {
                get = "BaldiTimeRE_" + Key;
            }
            LLAchievementManager.RewardAchievement(get, playAnimaton);
        }

        public static void ClearAllBaldiTimeAchievements()
        {
            List<LLAchievement> AllBaldiTimeAchievements = new List<LLAchievement>();
            foreach (var thing in LLAchievementManager.AllAchievements)
            {
                if (thing.Key.StartsWith("BaldiTimeRE_") || thing.Key.Contains("BaldiTimeRE_"))
                {
                    AllBaldiTimeAchievements.Add(thing.Value);
                }
            }
            foreach (LLAchievement lL in AllBaldiTimeAchievements)
            {
                LLAchievementManager.RemoveAchievment(lL);
            }
        }
    }
}