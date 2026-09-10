using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ItsBaldiTimeRework
{
    [BepInPlugin("ganaisthere.plus.itsbalditimerework", "Its Baldi Time: Rework", "0.1.0.0")]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]

    public class BasePlugin : BaseUnityPlugin
    {
        public static BasePlugin Instance { get; private set; }
        public static AssetManager AssetMan = new AssetManager();
        public static List<WeightedRoomAsset> classWeightedRoomAsset = new List<WeightedRoomAsset>();

        //---------------------------------------------------------------------
        public ConfigEntry<bool> ConfigUniqueGenerator;
        public ConfigEntry<bool> ConfigShowToppins;
        public ConfigEntry<bool> ConfigOpeningAnimations;
        //---------------------------------------------------------------------
        public void Awake()
        {
            ConfigUniqueGenerator = Config.Bind
            (
                "General",
                "Unique Generator - WIP",
                false,
                "If true, the mod's unique modification of the generator will be enabled: each floor has the floor 2 layout of the vanilla game, and the floor type is not limited by the number of floors. Also, The classroom activity will be selected from all activities."
            );
            ConfigShowToppins = Config.Bind
            (
                "General",
                "Show Toppins",
                false,
                "If true, when you get a toppin, he/she will follow you as an ENTITY instead of hiding."
            );
            ConfigOpeningAnimations = Config.Bind
            (
                "General",
                "Opening Animations",
                true,
                "If true, If true, the mod will show an opening animation before the game's warning screen."
            );
            Instance = this;
            new Harmony("ganaisthere.plus.itsbalditimerework").PatchAllConditionals();
            ModdedSaveGame.AddSaveHandler(base.Info);
            AddEnglishLocalization("Subtitles_English.json");
            LoadOpeningAssets();
            LoadingEvents.RegisterOnAssetsLoaded(base.Info, this.LoadAssets(), LoadingEventOrder.Start);
            GeneratorManagement.Register(this, GenerationModType.Addend, AddObjects);
        }

        private void LoadOpeningAssets()
        {
            AddAudioClip("TimeForASmackdown.ogg", "AudioClips/Misc", true);
            BaldiTimeAnimations.OpeningMusic = AssetMan.Get<AudioClip>("TimeForASmackdown");
            for (int i = 0; i < 3; i++)
            {
                string filename = "0_" + i.ToString();
                AddTexture2D(filename + ".png", "Textures/Misc/Opening");
                Texture2DToSprite(filename);
                BaldiTimeAnimations.Sprites_0[i] = AssetMan.Get<Sprite>(filename);
            }
            AddTexture2D("Border.png", "Textures/Misc/Opening");
            Texture2DToSprite("Border");
            BaldiTimeAnimations.Border_Sprite = AssetMan.Get<Sprite>("Border");
            for (int i = 0; i < 2; i++)
            {
                string filename = "1_" + i.ToString();
                AddTexture2D(filename + ".png", "Textures/Misc/Opening");
                Texture2DToSprite(filename);
                BaldiTimeAnimations.Sprites_1[i] = AssetMan.Get<Sprite>(filename);
            }
            for (int i = 0; i < 4; i++)
            {
                string filename = "2_" + i.ToString();
                AddTexture2D(filename + ".png", "Textures/Misc/Opening");
                Texture2DToSprite(filename);
                BaldiTimeAnimations.Sprites_2[i] = AssetMan.Get<Sprite>(filename);
            }
            for (int i = 1; i < 3; i++)
            {
                string filename = "2_2_" + i.ToString();
                AddTexture2D(filename + ".png", "Textures/Misc/Opening");
                Texture2DToSprite(filename);
                BaldiTimeAnimations.Sprites_2_2[i - 1] = AssetMan.Get<Sprite>(filename);
            }
            for (int i = 0; i < 4; i++)
            {
                string filename = "3_" + i.ToString();
                AddTexture2D(filename + ".png", "Textures/Misc/Opening");
                Texture2DToSprite(filename);
                BaldiTimeAnimations.Sprites_3[i] = AssetMan.Get<Sprite>(filename);
            }
            for (int i = 0; i < 2; i++)
            {
                string filename = "3_4_" + i.ToString();
                AddTexture2D(filename + ".png", "Textures/Misc/Opening");
                Texture2DToSprite(filename);
                BaldiTimeAnimations.Sprites_3_4[i] = AssetMan.Get<Sprite>(filename);
            }
            for (int i = 0; i < 4; i++)
            {
                string filename = "4_" + i.ToString();
                AddTexture2D(filename + ".png", "Textures/Misc/Opening");
                Texture2DToSprite(filename);
                BaldiTimeAnimations.Sprites_4[i] = AssetMan.Get<Sprite>(filename);
            }
            for (int i = 0; i < 4; i++)
            {
                string filename = "5_" + i.ToString();
                AddTexture2D(filename + ".png", "Textures/Misc/Opening");
                Texture2DToSprite(filename);
                BaldiTimeAnimations.Sprites_5[i] = AssetMan.Get<Sprite>(filename);
            }
            for (int i = 0; i < 5; i++)
            {
                string filename = "6_" + i.ToString();
                AddTexture2D(filename + ".png", "Textures/Misc/Opening");
                Texture2DToSprite(filename);
                BaldiTimeAnimations.Sprites_6[i] = AssetMan.Get<Sprite>(filename);
            }
            for (int i = 0; i < 8; i++)
            {
                string filename = "7_" + i.ToString();
                AddTexture2D(filename + ".png", "Textures/Misc/Opening");
                Texture2DToSprite(filename);
                BaldiTimeAnimations.Sprites_7[i] = AssetMan.Get<Sprite>(filename);
            }
            for (int i = 0; i < 7; i++)
            {
                string filename = "8_" + i.ToString();
                AddTexture2D(filename + ".png", "Textures/Misc/Opening");
                Texture2DToSprite(filename);
                BaldiTimeAnimations.Sprites_8[i] = AssetMan.Get<Sprite>(filename);
            }
            for (int i = 0; i < 7; i++)
            {
                string filename = "9_" + i.ToString();
                AddTexture2D(filename + ".png", "Textures/Misc/Opening");
                Texture2DToSprite(filename);
                BaldiTimeAnimations.Sprites_9[i] = AssetMan.Get<Sprite>(filename);
            }
            for (int i = 0; i < 3; i++)
            {
                string filename = "10_" + i.ToString();
                AddTexture2D(filename + ".png", "Textures/Misc/Opening");
                Texture2DToSprite(filename);
                BaldiTimeAnimations.Sprites_10[i] = AssetMan.Get<Sprite>(filename);
            }
            for (int i = 0; i < 3; i++)
            {
                string filename = "11_" + i.ToString();
                AddTexture2D(filename + ".png", "Textures/Misc/Opening");
                Texture2DToSprite(filename);
                BaldiTimeAnimations.Sprites_11[i] = AssetMan.Get<Sprite>(filename);
            }
            for (int i = 0; i < 5; i++)
            {
                string filename = "12_" + i.ToString();
                AddTexture2D(filename + ".png", "Textures/Misc/Opening");
                Texture2DToSprite(filename);
                BaldiTimeAnimations.Sprites_12[i] = AssetMan.Get<Sprite>(filename);
            }
            for (int i = 0; i < 2; i++)
            {
                string filename = "13_" + i.ToString();
                AddTexture2D(filename + ".png", "Textures/Misc/Opening");
                Texture2DToSprite(filename);
                BaldiTimeAnimations.Sprites_13[i] = AssetMan.Get<Sprite>(filename);
            }
        }

        private IEnumerator LoadAssets()
        {
            yield return 1225;

            if (ConfigUniqueGenerator.Value)
            {
                yield return "Find Rooms...";

                RoomAsset[] allRoomAssets = Resources.FindObjectsOfTypeAll<RoomAsset>().ToArray();
                foreach (RoomAsset roomAsset in allRoomAssets)
                {
                    if (roomAsset.category == RoomCategory.Class)
                    {
                        classWeightedRoomAsset.Add(new WeightedRoomAsset { selection = roomAsset, weight = 100 });
                    }
                }
            }

            yield return "Loading Textures...";
            AddTexture2D("BaldiTimeLogo_Sheet.png", "Textures/GUI");
            AddTexture2D("TimerBar_Sheet.png", "Textures/GUI");
            AddTexture2D("BaldiClockIcon_Large.png", "Textures/Items");
            AddTexture2D("BaldiClockIcon_Large_Transparent.png", "Textures/Items");
            AddTexture2D("ToppinsCage.png", "Textures/Entity");
            for (int i = 0; i < 5; i++)
            {
                AddTexture2D("Toppins_" + i.ToString() + "_Idle.png", "Textures/Entity");
                AddTexture2D("Toppins_" + i.ToString() + "_Yay.png", "Textures/Entity");
            }
            AddTexture2D("Notebook_John.png", "Textures/Misc");
            AddTexture2D("Rank_Sheet.png", "Textures/GUI");
            AddTexture2D("ComboDisplay_Sheet.png", "Textures/GUI");
            AddTexture2D("LapPortal_0.png", "Textures/Entity");
            AddTexture2D("LapPortal_1.png", "Textures/Entity");
            AddTexture2D("Lap2Flag.png", "Textures/GUI/LapFlags");
            AddTexture2D("RankAnime_Student_0.png", "Textures/Misc/RankAnime");
            AddTexture2D("RankAnime_Student_D.png", "Textures/Misc/RankAnime");
            AddTexture2D("RankAnime_Student_B.png", "Textures/Misc/RankAnime");
            AddTexture2D("RankAnime_Student_C.png", "Textures/Misc/RankAnime");
            AddTexture2D("RankAnime_Student_A.png", "Textures/Misc/RankAnime");
            AddTexture2D("RankAnime_Student_S.png", "Textures/Misc/RankAnime");
            AddTexture2D("RankAnime_Student_P.png", "Textures/Misc/RankAnime");
            AddTexture2D("RankAnime_Rank_D.png", "Textures/Misc/RankAnime");
            AddTexture2D("RankAnime_Rank_B.png", "Textures/Misc/RankAnime");
            AddTexture2D("RankAnime_Rank_C.png", "Textures/Misc/RankAnime");
            AddTexture2D("RankAnime_Rank_A.png", "Textures/Misc/RankAnime");
            AddTexture2D("RankAnime_Rank_S.png", "Textures/Misc/RankAnime");
            AddTexture2D("RankAnime_Rank_P.png", "Textures/Misc/RankAnime");

            yield return "Loading...IDK";
            string[] getfiles = Directory.GetFiles(AssetLoader.GetModPath(this) + "/Textures/GUI/ComboLevels/", "*.png", SearchOption.TopDirectoryOnly);
            if (getfiles.Length > 0)
            {
                foreach (string file in getfiles)
                {
                    string fileName = Path.GetFileName(file);
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                    AddTexture2D(fileName, "Textures/GUI/ComboLevels");
                    Texture2DToSprite(fileNameWithoutExtension);
                    BaldiTimeUI.ComboLevelsSprites.Add(AssetMan.Get<Sprite>(fileNameWithoutExtension));
                }
            }

            yield return "Loading Sounds...";
            AddSoundObject("JOHN_PILLAR_IMPACT.ogg", "SoundObjects/Effects", true);
            BaldiTimeActions.JOHN_PILLAR_IMPACT = AssetMan.Get<SoundObject>("JOHN_PILLAR_IMPACT");
            AddSoundObject("bellcollectsmall.ogg", "SoundObjects/Effects", true);
            AddSoundObject("sfx_collecttoppin.ogg", "SoundObjects/Effects", true);
            AddAudioClip("Meatophobia.ogg", "AudioClips/Misc", true);
            AddSoundObject("Lapping.ogg", "SoundObjects/Effects", true);
            AddSoundObject("sfx_lapenter.ogg", "SoundObjects/Effects", true);
            AddSoundObject("sfx_lapexit.ogg", "SoundObjects/Effects", true);
            AddSoundObject("comboup1.ogg", "SoundObjects/Effects", true);
            BaldiTimeUI.comboup[0] = AssetMan.Get<SoundObject>("comboup1");
            AddSoundObject("comboup2.ogg", "SoundObjects/Effects");
            BaldiTimeUI.comboup[1] = AssetMan.Get<SoundObject>("comboup2");
            AddSoundObject("comboup4.ogg", "SoundObjects/Effects");
            BaldiTimeUI.comboup[2] = AssetMan.Get<SoundObject>("comboup4");
            for (int i = 1; i < 6; i++)
            {
                string up = "rankup" + i.ToString();
                string down = "rankdown" + i.ToString();
                AddSoundObject(up + ".ogg", "SoundObjects/Effects", true);
                AddSoundObject(down + ".ogg", "SoundObjects/Effects", true);
                BaldiTimeUI.rankup[i - 1] = AssetMan.Get<SoundObject>(up);
                BaldiTimeUI.rankdown[i - 1] = AssetMan.Get<SoundObject>(down);
            }
            AddSoundObject("Rank_D.ogg", "SoundObjects/Effects/Rank", true);
            BaldiTimeAnimations.RankSounds[0] = AssetMan.Get<SoundObject>("Rank_D");
            AddSoundObject("Rank_C.ogg", "SoundObjects/Effects/Rank", true);
            BaldiTimeAnimations.RankSounds[1] = AssetMan.Get<SoundObject>("Rank_C");
            AddSoundObject("Rank_B.ogg", "SoundObjects/Effects/Rank", true);
            BaldiTimeAnimations.RankSounds[2] = AssetMan.Get<SoundObject>("Rank_B");
            AddSoundObject("Rank_A.ogg", "SoundObjects/Effects/Rank", true);
            BaldiTimeAnimations.RankSounds[3] = AssetMan.Get<SoundObject>("Rank_A");
            AddSoundObject("Rank_S.ogg", "SoundObjects/Effects/Rank", true);
            BaldiTimeAnimations.RankSounds[4] = AssetMan.Get<SoundObject>("Rank_S");
            AddSoundObject("Rank_P.ogg", "SoundObjects/Effects/Rank", true);
            BaldiTimeAnimations.RankSounds[5] = AssetMan.Get<SoundObject>("Rank_P");
            AddSoundObject("Rank_L.ogg", "SoundObjects/Effects/Rank", true);
            BaldiTimeAnimations.RankSounds[6] = AssetMan.Get<SoundObject>("Rank_L");

            yield return "Loading Lap Musics...";
            AddAudioClip("Lap1-Intro.ogg", "SoundObjects/Laps");
            AddAudioClip("Lap1-Loop.ogg", "SoundObjects/Laps");
            AddAudioClip("Lap1-Outro.ogg", "SoundObjects/Laps");
            AddAudioClip("Lap2-Intro.ogg", "SoundObjects/Laps");
            AddAudioClip("Lap2-Loop.ogg", "SoundObjects/Laps");

            getfiles = Directory.GetFiles(AssetLoader.GetModPath(this) + "/AudioClips/Spoop/", "*.ogg", SearchOption.TopDirectoryOnly);
            if (getfiles.Length > 0)
            {
                foreach (string file in getfiles)
                {
                    yield return "Loading Floor Musics...";
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                    string fileName = Path.GetFileName(file);
                    AddAudioClip(fileName, "AudioClips/Spoop");
                    if (fileNameWithoutExtension.Contains("_F1"))
                    {
                        BaldiTimeActions.F1Mus.Add(fileNameWithoutExtension);
                    }
                    if (fileNameWithoutExtension.Contains("_F2"))
                    {
                        BaldiTimeActions.F2Mus.Add(fileNameWithoutExtension);
                    }
                    if (fileNameWithoutExtension.Contains("_F3"))
                    {
                        BaldiTimeActions.F3Mus.Add(fileNameWithoutExtension);
                    }
                    if (fileNameWithoutExtension.Contains("_F4"))
                    {
                        BaldiTimeActions.F4Mus.Add(fileNameWithoutExtension);
                    }
                    if (fileNameWithoutExtension.Contains("_F5"))
                    {
                        BaldiTimeActions.F5Mus.Add(fileNameWithoutExtension);
                    }
                    if (!fileNameWithoutExtension.Contains("_F1") && !fileNameWithoutExtension.Contains("_F2") && !fileNameWithoutExtension.Contains("_F3") && !fileNameWithoutExtension.Contains("_F4") && !fileNameWithoutExtension.Contains("_F5"))
                    {
                        BaldiTimeActions.AllMus.Add(fileNameWithoutExtension);
                    }
                }
            }

            yield return "Loading Title Cards...";
            string[] getPng = Directory.GetFiles(AssetLoader.GetModPath(this) + "/TitleCard/", "*.png", SearchOption.TopDirectoryOnly);
            if (getPng.Length > 0)
            {
                foreach (string file in getPng)
                {
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                    string fileName = Path.GetFileName(file);
                    if (!fileName.Contains("-Title.png"))
                    {
                        AddTexture2D(fileName, "TitleCard");
                        Texture2DToSprite(fileNameWithoutExtension);
                        BaldiTimeAnimations.TitleCardBackSprites.Add(AssetMan.Get<Sprite>(fileNameWithoutExtension));

                        AddTexture2D(fileNameWithoutExtension + "-Title.png", "TitleCard");
                        Texture2DToSprite(fileNameWithoutExtension + "-Title");
                        BaldiTimeAnimations.TitleCardTitleSprites.Add(AssetMan.Get<Sprite>(fileNameWithoutExtension + "-Title"));

                        AddSoundObject(fileNameWithoutExtension + "-Sound.ogg", "TitleCard");
                        BaldiTimeAnimations.TitleCardSounds.Add(AssetMan.Get<SoundObject>(fileNameWithoutExtension + "-Sound"));
                    }
                }
            }

            yield return "Add Sprites...";
            TextureSheetToSprite("BaldiTimeLogo_Sheet", 2f, 0f, "BaldiTimeLogo_0");
            BaldiTimeUI.baldiTimeLogoSprites[0] = AssetMan.Get<Sprite>("BaldiTimeLogo_0");
            TextureSheetToSprite("BaldiTimeLogo_Sheet", 2f, 1f, "BaldiTimeLogo_1");
            BaldiTimeUI.baldiTimeLogoSprites[1] = AssetMan.Get<Sprite>("BaldiTimeLogo_1");
            SetupTimerBarSprites();
            Texture2DToSprite("BaldiClockIcon_Large", 50f);
            Texture2DToSprite("BaldiClockIcon_Large_Transparent", 50f);
            Texture2DToSprite("ToppinsCage", new Vector2(0.5f, 0.45f), 10f);
            for (int i = 0; i < 5; i++)
            {
                Texture2DToSprite("Toppins_" + i.ToString() + "_Idle", new Vector2(0.5f, 0.6f), 50f);
                Texture2DToSprite("Toppins_" + i.ToString() + "_Yay", new Vector2(0.5f, 0.6f), 50f);
            }
            Texture2DToSprite("Notebook_John", 100f);
            BaldiTimeActions.Notebook_John = AssetMan.Get<Sprite>("Notebook_John");
            SetupPointDisplaySprites();
            SetupComboBarSprites();
            Texture2DToSprite("LapPortal_0", 16f);
            Texture2DToSprite("LapPortal_1", 16f);
            Texture2DToSprite("Lap2Flag");
            BaldiTimeUI.LapFlagSprites[0] = AssetMan.Get<Sprite>("Lap2Flag");
            Texture2DToSprite("RankAnime_Student_0");
            BaldiTimeAnimations.StudentSprite = AssetMan.Get<Sprite>("RankAnime_Student_0");
            Texture2DToSprite("RankAnime_Student_D");
            BaldiTimeAnimations.StudentSprites[0] = AssetMan.Get<Sprite>("RankAnime_Student_D");
            Texture2DToSprite("RankAnime_Student_C");
            BaldiTimeAnimations.StudentSprites[1] = AssetMan.Get<Sprite>("RankAnime_Student_C");
            Texture2DToSprite("RankAnime_Student_B");
            BaldiTimeAnimations.StudentSprites[2] = AssetMan.Get<Sprite>("RankAnime_Student_B");
            Texture2DToSprite("RankAnime_Student_A");
            BaldiTimeAnimations.StudentSprites[3] = AssetMan.Get<Sprite>("RankAnime_Student_A");
            Texture2DToSprite("RankAnime_Student_S");
            BaldiTimeAnimations.StudentSprites[4] = AssetMan.Get<Sprite>("RankAnime_Student_S");
            Texture2DToSprite("RankAnime_Student_P");
            BaldiTimeAnimations.StudentSprites[5] = AssetMan.Get<Sprite>("RankAnime_Student_P");
            Texture2DToSprite("RankAnime_Rank_D");
            BaldiTimeAnimations.RankSprites[0] = AssetMan.Get<Sprite>("RankAnime_Rank_D");
            Texture2DToSprite("RankAnime_Rank_C");
            BaldiTimeAnimations.RankSprites[1] = AssetMan.Get<Sprite>("RankAnime_Rank_C");
            Texture2DToSprite("RankAnime_Rank_B");
            BaldiTimeAnimations.RankSprites[2] = AssetMan.Get<Sprite>("RankAnime_Rank_B");
            Texture2DToSprite("RankAnime_Rank_A");
            BaldiTimeAnimations.RankSprites[3] = AssetMan.Get<Sprite>("RankAnime_Rank_A");
            Texture2DToSprite("RankAnime_Rank_S");
            BaldiTimeAnimations.RankSprites[4] = AssetMan.Get<Sprite>("RankAnime_Rank_S");
            Texture2DToSprite("RankAnime_Rank_P");
            BaldiTimeAnimations.RankSprites[5] = AssetMan.Get<Sprite>("RankAnime_Rank_P");

            yield return "Add ItemObjects...";
            ItemObject BaldiClock = new ItemBuilder(Info)
                .SetNameAndDescription("Itm_BaldiClock", "Desc_BaldiClock")
                .SetSprites(AssetMan.Get<Sprite>("BaldiClockIcon_Large"), AssetMan.Get<Sprite>("BaldiClockIcon_Large_Transparent"))
                .SetEnum("BaldiClock")
                .SetShopPrice(12251225)
                .SetGeneratorCost(12251225)
                .SetItemComponent<ITM_BaldiClock>()
                .SetMeta(ItemFlags.InstantUse, new string[] { "BaldiClock" })
                .SetAsInstantUse()
                .SetPickupSound(AssetMan.Get<SoundObject>("bellcollectsmall"))
                .Build();
            AssetMan.Add("BaldiClock", BaldiClock);

            yield return "Add NPCs...";
            Toppin toppin = new NPCBuilder<Toppin>(Info)
                .SetName("Toppin")
                .SetEnum("Toppin")
                .IgnorePlayerOnSpawn()
                .SetAudioTimescaleType(TimeScaleType.Player)
                .AddMetaFlag(NPCFlags.StandardNoCollide)
                .SetAirborne()
                .Build();
            toppin.SFX_Get = AssetMan.Get<SoundObject>("sfx_collecttoppin");
            toppin.CageSprite = AssetMan.Get<Sprite>("ToppinsCage");
            toppin.spriteRenderer[0].sprite = toppin.CageSprite;
            for (int i = 0; i < 5; i++)
            {
                toppin.IdleSprite[i] = AssetMan.Get<Sprite>("Toppins_" + i.ToString() + "_Idle");
                toppin.YaySprite[i] = AssetMan.Get<Sprite>("Toppins_" + i.ToString() + "_Yay");
            }
            AssetMan.Add("Toppin", toppin);

            LapPortal lapPortal = new NPCBuilder<LapPortal>(Info)
                .SetName("LapPortal")
                .SetEnum("LapPortal")
                .IgnorePlayerOnSpawn()
                .SetAudioTimescaleType(TimeScaleType.Player)
                .AddMetaFlag(NPCFlags.StandardNoCollide)
                .SetAirborne()
                .Build();
            lapPortal.Sfx_Enter = AssetMan.Get<SoundObject>("sfx_lapenter");
            lapPortal.Sfx_Exit = AssetMan.Get<SoundObject>("sfx_lapexit");
            lapPortal.Sfx_Lapping = AssetMan.Get<SoundObject>("Lapping");
            lapPortal.sprites[0] = AssetMan.Get<Sprite>("LapPortal_0");
            lapPortal.sprites[1] = AssetMan.Get<Sprite>("LapPortal_1");
            lapPortal.spriteRenderer[0].sprite = lapPortal.sprites[0];
            AssetMan.Add("LapPortal", lapPortal);

            yield break;
        }

        private void AddObjects(string floorName, int floorNumber, SceneObject sceneObject)
        {
            CustomLevelObject[] customLevelObjects = CustomLevelObjectExtensions.GetCustomLevelObjects(sceneObject);
            if (floorName.StartsWith("F"))
            {
                foreach (CustomLevelObject customLevelObject in customLevelObjects)
                {
                    if (ConfigUniqueGenerator.Value)
                    {
                        customLevelObject.minSize = new IntVector2(25, 25);
                        customLevelObject.maxSize = new IntVector2(35, 35);
                        customLevelObject.exitCount = 2;
                        customLevelObject.minEvents = 2;
                        customLevelObject.maxEvents = 3;
                        customLevelObject.minSpecialRooms = 1;
                        customLevelObject.maxSpecialRooms = 2;
                        customLevelObject.maxItemValue = 600;
                        foreach (RoomGroup roomGroup in customLevelObject.roomGroup)
                        {
                            if (roomGroup.name == "Class")
                            {
                                roomGroup.minRooms = 7;
                                roomGroup.maxRooms = 7;
                                roomGroup.potentialRooms = roomGroup.potentialRooms.AddRangeToArray(classWeightedRoomAsset.ToArray());
                            }
                            if (roomGroup.name == "Faculty")
                            {
                                roomGroup.minRooms = 8;
                                roomGroup.maxRooms = 12;
                            }
                        }
                    }
                    customLevelObject.MarkAsNeverUnload();
                }
            }
        }

        private void SetupComboBarSprites()
        {
            Texture2D texture2D = AssetMan.Get<Texture2D>("ComboDisplay_Sheet");
            Sprite sprite1 = Sprite.Create(texture2D, new Rect(0f, texture2D.height / 3f, texture2D.width, texture2D.height / 3f * 2f), new Vector2(0.5f, 0.5f));
            Sprite sprite2 = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width / 6f * 5f, texture2D.height / 3f), new Vector2(0.5f, 0.5f));
            Sprite sprite3 = Sprite.Create(texture2D, new Rect(texture2D.width / 6f * 5f, 0f, texture2D.width / 6f, texture2D.height / 3f), new Vector2(0.5f, 0.5f));
            sprite1.name = "ComboBar_Overlay";
            sprite2.name = "ComboBar_Background";
            sprite3.name = "ComboBar_Niddle";
            AssetMan.Add("ComboBar_Overlay", sprite1);
            AssetMan.Add("ComboBar_Background", sprite2);
            AssetMan.Add("ComboBar_Niddle", sprite3);
            BaldiTimeUI.ComboBarSprites[0] = AssetMan.Get<Sprite>("ComboBar_Overlay");
            BaldiTimeUI.ComboBarSprites[1] = AssetMan.Get<Sprite>("ComboBar_Background");
            BaldiTimeUI.ComboBarSprites[2] = AssetMan.Get<Sprite>("ComboBar_Niddle");
        }
        private void SetupTimerBarSprites()
        {
            Texture2D texture2D = AssetMan.Get<Texture2D>("Rank_Sheet");
            for (int p = 0; p < 4; p++)
            {
                for (int i = 0; i < 7; i++)
                {
                    Sprite sprite = Sprite.Create(texture2D, new Rect(0f + i * texture2D.width / 7, texture2D.height / 7f * (6 - p), texture2D.width / 7f, texture2D.height / 7f), new Vector2(0.5f, 0.5f));
                    int o = i + 7 * p;
                    sprite.name = "RankColor_" + o.ToString();
                    AssetMan.Add("RankColor_" + o.ToString(), sprite);
                    BaldiTimeUI.RankColorSprites[o] = AssetMan.Get<Sprite>("RankColor_" + o.ToString());
                }
            }
            for (int i = 0; i < 3; i++)
            {
                Sprite sprite = Sprite.Create(texture2D, new Rect(0f + i * texture2D.width / 7, texture2D.height / 7f * 2, texture2D.width / 7f, texture2D.height / 7f), new Vector2(0.5f, 0.5f));
                int o = i + 28;
                sprite.name = "RankColor_" + o.ToString();
                AssetMan.Add("RankColor_" + o.ToString(), sprite);
                BaldiTimeUI.RankColorSprites[o] = AssetMan.Get<Sprite>("RankColor_" + o.ToString());
            }
            for (int i = 0; i < 7; i++)
            {
                Sprite sprite = Sprite.Create(texture2D, new Rect(0f + i * texture2D.width / 7, texture2D.height / 7f, texture2D.width / 7f, texture2D.height / 7f), new Vector2(0.5f, 0.5f));
                sprite.name = "RankOverlay_" + BaldiTimeActions.ranks[i];
                AssetMan.Add("RankOverlay_" + BaldiTimeActions.ranks[i], sprite);
                BaldiTimeUI.RankOverlaySprites[i] = AssetMan.Get<Sprite>("RankOverlay_" + BaldiTimeActions.ranks[i]);
            }
            Sprite sprite0 = Sprite.Create(texture2D, new Rect(texture2D.width / 7 * 6, texture2D.height / 7f * 2, texture2D.width / 7f, texture2D.height / 7f), new Vector2(0.5f, 0.5f));
            sprite0.name = "RankBackground";
            AssetMan.Add("RankBackground", sprite0);
            BaldiTimeUI.RankBackgroundSprite = AssetMan.Get<Sprite>("RankBackground");
        }
        private void SetupPointDisplaySprites()
        {
            Texture2D texture2D = AssetMan.Get<Texture2D>("TimerBar_Sheet");
            Sprite sprite1 = Sprite.Create(texture2D, new Rect(0f, texture2D.height / 3f * 2f, texture2D.width, texture2D.height / 3f), new Vector2(0.5f, 0.5f));
            Sprite sprite2 = Sprite.Create(texture2D, new Rect(0f, texture2D.height / 3f, texture2D.width, texture2D.height / 3f), new Vector2(0.5f, 0.5f));
            Sprite sprite3 = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width / 256f * 32f, texture2D.height / 3f), new Vector2(0.5f, 0.5f));
            sprite1.name = "TimerBar_Overlay";
            sprite2.name = "TimerBar_Background";
            sprite3.name = "TimerBar_Niddle";
            AssetMan.Add("TimerBar_Overlay", sprite1);
            AssetMan.Add("TimerBar_Background", sprite2);
            AssetMan.Add("TimerBar_Niddle", sprite3);
            BaldiTimeUI.TimerBarSprites[0] = AssetMan.Get<Sprite>("TimerBar_Overlay");
            BaldiTimeUI.TimerBarSprites[1] = AssetMan.Get<Sprite>("TimerBar_Background");
            BaldiTimeUI.TimerBarSprites[2] = AssetMan.Get<Sprite>("TimerBar_Niddle");
        }
        private void AddTexture2D(string fileNameWithExtension, string chlidPath)
        {
            string[] getfiles = Directory.GetFiles(AssetLoader.GetModPath(this) + "/" + chlidPath + "/", fileNameWithExtension);
            if (getfiles.Length <= 0)
            {
                Debug.LogError("File not found: " + AssetLoader.GetModPath(this) + "/" + chlidPath + "/" + fileNameWithExtension);
                return;
            }
            string getfile = getfiles[0];
            AssetMan.Add(Path.GetFileNameWithoutExtension(getfile), AssetLoader.TextureFromFile(getfile));
        }
        private void AddEnglishLocalization(string fileNameWithExtension)
        {
            string[] getfiles = Directory.GetFiles(AssetLoader.GetModPath(this), fileNameWithExtension);
            if (getfiles.Length <= 0)
            {
                Debug.LogError("File not found: " + AssetLoader.GetModPath(this) + "/" + fileNameWithExtension);
                return;
            }
            string getfile = getfiles[0];
            AssetLoader.LocalizationFromFile(getfile, Language.English);
        }
        private void Texture2DToSprite(string fileName, float pixelsPerUnit = 100f)
        {
            Texture2D texture2D = AssetMan.Get<Texture2D>(fileName);
            Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            sprite.name = fileName;
            AssetMan.Add(fileName, sprite);
        }
        private void Texture2DToSprite(string fileName, Vector2 vector2, float pixelsPerUnit = 100f)
        {
            Texture2D texture2D = AssetMan.Get<Texture2D>(fileName);
            Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), vector2, pixelsPerUnit);
            sprite.name = fileName;
            AssetMan.Add(fileName, sprite);
        }
        private void TextureSheetToSprite(string fileName, float rectX, float rectY, string spriteName)
        {
            Texture2D texture2D = AssetMan.Get<Texture2D>(fileName);
            Sprite sprite = Sprite.Create(texture2D, new Rect(texture2D.width / rectX * rectY, 0, texture2D.width / rectX, texture2D.height), new Vector2(0.5f, 0.5f));
            sprite.name = spriteName;
            AssetMan.Add(spriteName, sprite);
        }
        private void AddSoundObject(string fileNameWithExtension, string chlidPath, bool must = false)
        {
            string[] getfiles = Directory.GetFiles(AssetLoader.GetModPath(this) + "/" + chlidPath + "/", fileNameWithExtension);
            if (getfiles.Length <= 0)
            {
                if (must)
                {
                    Debug.LogError("File not found: " + AssetLoader.GetModPath(this) + "/" + chlidPath + "/" + fileNameWithExtension);
                }
                return;
            }
            string getfile = getfiles[0];
            SoundObject soundObject = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(getfile), "Nothing", SoundType.Music, Color.white, 0f);
            AssetMan.Add(Path.GetFileNameWithoutExtension(getfile), soundObject);
        }
        private void AddAudioClip(string fileNameWithExtension, string chlidPath, bool must = false)
        {
            string[] getfiles = Directory.GetFiles(AssetLoader.GetModPath(this) + "/" + chlidPath + "/", fileNameWithExtension);
            if (getfiles.Length <= 0)
            {
                if (must)
                {
                    Debug.LogError("File not found: " + AssetLoader.GetModPath(this) + "/" + chlidPath + "/" + fileNameWithExtension);
                }
                return;
            }
            string getfile = getfiles[0];
            AssetMan.Add(Path.GetFileNameWithoutExtension(getfile), AssetLoader.AudioClipFromFile(getfile));
        }
        /*
        //---------------------------------------------------------------------
        private void QuickAddAudioClip(string audioClipNameWithExtension)
        {
            string getfile = Directory.GetFiles(AssetLoader.GetModPath(this), audioClipNameWithExtension)[0];
            AssetMan.Add<AudioClip>(Path.GetFileNameWithoutExtension(getfile), AssetLoader.AudioClipFromFile(getfile));
        }
        private void QuickAddAudioClip(string audioClipNameWithExtension, string chlidPath)
        {
            string getfile = Directory.GetFiles(AssetLoader.GetModPath(this) + "/" + chlidPath + "/", audioClipNameWithExtension)[0];
            AssetMan.Add<AudioClip>(Path.GetFileNameWithoutExtension(getfile), AssetLoader.AudioClipFromFile(getfile));
        }
        //---------------------------------------------------------------------
        private SoundObject QuickSetupSoundObject(string soundObjectNameWithExtension, string Localization, SoundType soundType, UnityEngine.Color color, float subtitlelength)
        {
            string getfile = Directory.GetFiles(AssetLoader.GetModPath(this), soundObjectNameWithExtension)[0];
            SoundObject soundObject = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(getfile), Localization, soundType, color, subtitlelength);
            return soundObject;
        }
        private SoundObject QuickSetupSoundObject(string soundObjectNameWithExtension, string chlidPath, string Localization, SoundType soundType, UnityEngine.Color color, float subtitlelength)
        {
            string getfile = Directory.GetFiles(AssetLoader.GetModPath(this) + "/" + chlidPath + "/", soundObjectNameWithExtension)[0];
            SoundObject soundObject = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(getfile), Localization, soundType, color, subtitlelength);
            return soundObject;
        }
        //---------------------------------------------------------------------
        private void QuickAddMidi(string midiNameWithExtension)
        {
            string getfile = Directory.GetFiles(AssetLoader.GetModPath(this), midiNameWithExtension)[0];
            AssetLoader.MidiFromFile(getfile, Path.GetFileNameWithoutExtension(getfile));
        }
        private void QuickAddMidi(string midiNameWithExtension, string chlidPath)
        {
            string getfile = Directory.GetFiles(AssetLoader.GetModPath(this) + "/" + chlidPath + "/", midiNameWithExtension)[0];
            AssetLoader.MidiFromFile(getfile, Path.GetFileNameWithoutExtension(getfile));
        }*/
    }
}
