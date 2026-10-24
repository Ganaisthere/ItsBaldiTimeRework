using System.Collections.Generic;
using BepInEx.Bootstrap;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Reflection;
using NilLib;
using UnityEngine;

namespace ItsBaldiTimeRework
{
    public static class Other
    {
        public static List<WeightedSoundObject> DefaultDeathSoundsWeighted = new List<WeightedSoundObject>();
        public static List<SoundObject> DefaultDeathSounds = new List<SoundObject>();

        public static List<RoomFunction> Functions(this RoomFunctionContainer roomFunctionContainer)
        {
            return roomFunctionContainer.ReflectionGetVariable("functions") as List<RoomFunction>;
        }

        public static bool AnykeyDown()
        {
            return Input.anyKeyDown || Singleton<InputManager>.Instance.GetDigitalInput("MouseSubmit", onDown: true) || Singleton<InputManager>.Instance.GetDigitalInput("Pause", onDown: true) || Singleton<InputManager>.Instance.AnyButton(onDown: true);
        }

        public static SoundObject GetRandomDeathSound()
        {
            if (DefaultDeathSounds.Count <= 0)
            {
                SoundObject soundObject = AssetFinder.FindOfTypeWithName<SoundObject>("Lose_Buzz", true);
                DefaultDeathSounds.Add(soundObject);
                soundObject = AssetFinder.FindOfTypeWithName<SoundObject>("Lose_Corruption", true);
                DefaultDeathSounds.Add(soundObject);
                soundObject = AssetFinder.FindOfTypeWithName<SoundObject>("Lose_Corruption2", true);
                DefaultDeathSounds.Add(soundObject);
                soundObject = AssetFinder.FindOfTypeWithName<SoundObject>("Lose_Creepy", true);
                DefaultDeathSounds.Add(soundObject);
                soundObject = AssetFinder.FindOfTypeWithName<SoundObject>("Lose_GlassBoom", true);
                DefaultDeathSounds.Add(soundObject);
                soundObject = AssetFinder.FindOfTypeWithName<SoundObject>("Lose_NO", true);
                DefaultDeathSounds.Add(soundObject);
                soundObject = AssetFinder.FindOfTypeWithName<SoundObject>("Lose_What", true);
                DefaultDeathSounds.Add(soundObject);
            }
            int i = Random.Range(0, DefaultDeathSounds.Count - 1);
            return DefaultDeathSounds[i];
        }

        public static void NpcKillPlayer(NPC npc)
        {
            if (DefaultDeathSoundsWeighted.Count <= 0)
            {
                SoundObject soundObject = AssetFinder.FindOfTypeWithName<SoundObject>("Lose_Buzz", true);
                DefaultDeathSoundsWeighted.Add(new WeightedSoundObject { selection = soundObject, weight = 100 });

                soundObject = AssetFinder.FindOfTypeWithName<SoundObject>("Lose_Corruption", true);
                DefaultDeathSoundsWeighted.Add(new WeightedSoundObject { selection = soundObject, weight = 100 });

                soundObject = AssetFinder.FindOfTypeWithName<SoundObject>("Lose_Corruption2", true);
                DefaultDeathSoundsWeighted.Add(new WeightedSoundObject { selection = soundObject, weight = 100 });

                soundObject = AssetFinder.FindOfTypeWithName<SoundObject>("Lose_Creepy", true);
                DefaultDeathSoundsWeighted.Add(new WeightedSoundObject { selection = soundObject, weight = 100 });

                soundObject = AssetFinder.FindOfTypeWithName<SoundObject>("Lose_GlassBoom", true);
                DefaultDeathSoundsWeighted.Add(new WeightedSoundObject { selection = soundObject, weight = 100 });

                soundObject = AssetFinder.FindOfTypeWithName<SoundObject>("Lose_NO", true);
                DefaultDeathSoundsWeighted.Add(new WeightedSoundObject { selection = soundObject, weight = 100 });

                soundObject = AssetFinder.FindOfTypeWithName<SoundObject>("Lose_What", true);
                DefaultDeathSoundsWeighted.Add(new WeightedSoundObject { selection = soundObject, weight = 100 });
            }

            UsefulHelpers.KillPlayer(npc, DefaultDeathSoundsWeighted.ToArray());
        }

        public static string GLT(string key)
        {
            return Singleton<LocalizationManager>.Instance.GetLocalizedText(key);
        }

        public static bool IsReady()
        {
            if (Application.version == "0.14.5" || Application.version == "0.14.4")
            {
                return IsReady0145();
            }
            return true;
        }
        private static bool IsReady0145()
        {
            if (Singleton<BaseGameManager>.Instance.GameReady)
            {
                return true;
            }
            return false;
        }

        public static List<string> DenyBTNameKey = new List<string>()
        {
            "Mode_Pitstop",
            "Mode_Endless",
            "Mode_Tutorial",
            //"Mode_Challenge"
        };
        public static List<string> DenyBTName = new List<string>()
        {
            "?"
        };
        public static bool ShouldBT(BaseGameManager manager, bool noPitStop = true)
        {
            foreach (string text in DenyBTName)
            {
                if (manager.name.Contains(text))
                {
                    return false;
                }
            }
            return (!manager.InPitstop() || !noPitStop) && !DenyBTNameKey.Contains(manager.managerNameKey);
        }

        public static List<string> DenyBTSpawnNpcsNameKey = new List<string>()
        {
            "Mode_Challenge"
        };
        public static List<string> DenyBTSpawnNpcsName = new List<string>()
        {
            "KaizoBaldiGameManager"
        };
        public static bool ShouldBTSpawnNpcs(BaseGameManager manager)
        {
            foreach (string text in DenyBTSpawnNpcsName)
            {
                if (manager.name.Contains(text))
                {
                    return false;
                }
            }
            return !DenyBTSpawnNpcsNameKey.Contains(manager.managerNameKey);
        }

        public static List<string> DenyBTPlayMusNameKey = new List<string>()
        {
            "?"
        };
        public static List<string> DenyBTPlayMusName = new List<string>()
        {
            "KaizoBaldiGameManager"
        };
        public static bool ShouldBTPlayMus(BaseGameManager manager)
        {
            foreach (string text in DenyBTPlayMusName)
            {
                if (manager.name.Contains(text))
                {
                    return false;
                }
            }
            return !DenyBTPlayMusNameKey.Contains(manager.managerNameKey);
        }

        public static List<string> DenyBTDespawnHBUUID = new List<string>()
        {
            "confusedseagull.baldiplus.principalsday",
            "temp.bbp.baldisbadday"
        };
        public static List<string> DenyBTDespawnHBNameKey = new List<string>()
        {
            "?"
        };
        public static List<string> DenyBTDespawnHBName = new List<string>()
        {
            "?"
        };
        public static bool ShouldBTDespawnHB(BaseGameManager manager)
        {
            foreach (string text in DenyBTDespawnHBUUID)
            {
                if (CLC(text))
                {
                    return false;
                }
            }
            foreach (string text in DenyBTDespawnHBName)
            {
                if (manager.name.Contains(text))
                {
                    return false;
                }
            }
            return !DenyBTDespawnHBNameKey.Contains(manager.managerNameKey);
        }


        public static bool CLC(string i)
        {
            return Chainloader.PluginInfos.ContainsKey(i);
        }
    }
}
