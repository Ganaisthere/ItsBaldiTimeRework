using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.SaveSystem;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static BepInEx.BepInDependency;

namespace ItsBaldiTimeRework
{
    [BepInPlugin("ganaisthere.plus.itsbalditimerework", "Its Baldi Time Reworked", "0.2.2.0")]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]
    [BepInDependency("Nil.Library")]
    [BepInDependency("pixelguy.pixelmodding.baldiplus.custommainmenusapi", DependencyFlags.SoftDependency)]
    //[BepInDependency("il.modded.raldi.tweaks", DependencyFlags.SoftDependency)]//IDK why it's broken
    //[BepInDependency("Nil.NullscapeinBB", DependencyFlags.SoftDependency)]//IDK why it's broken too
    //[BepInIncompatibility("com.styx.baldi.quickstart")]//Broken

    public class BasePlugin : BaseUnityPlugin
    {
        public static BasePlugin Instance { get; private set; }
        public static AssetManager AssetMan = new AssetManager();
        public static List<WeightedRoomAsset> classWeightedRoomAsset = new List<WeightedRoomAsset>();
        public static Harmony harmony = new Harmony("ganaisthere.plus.itsbalditimerework");

        //Copyright (c) 2023 benjaminpants
        //Licensed under the MIT License(MIT)
        public class PackMeta
        {
            [JsonProperty("Name")]
            public string name;

            [JsonProperty("Description")]
            public string description;

            [JsonProperty("Author")]
            public string author;

            [JsonProperty("ReplaceSpoopMusic")]
            public bool replaceSpoopMusic;

            [JsonProperty("ReplaceTitleCard")]
            public bool replaceTitleCard;

            [JsonProperty("ReplaceComboLevels")]
            public bool replaceComboLevels;

            public PackMeta()
            {
                name = "Unnamed Pack";
                description = "No description added yet.";
                author = "IDK";
                replaceSpoopMusic = false;
                replaceTitleCard = false;
                replaceComboLevels = false;
            }
        }
        public static Dictionary<string, PackMeta> AllPacks = new Dictionary<string, PackMeta>();
        public static List<string> AllPackStrings = new List<string>();
        public static List<string> LoadedPacks = new List<string>();

        //---------------------------------------------------------------------
        public ConfigEntry<bool> ConfigUniqueGenerator;
        public ConfigEntry<bool> ConfigShowToppins;
        public ConfigEntry<bool> ConfigOpeningAnimations;
        public ConfigEntry<int> ConfigMusicVolume;
        public ConfigEntry<CameraShakeStyle> ConfigCameraShakeStyle;
        public ConfigEntry<float> ConfigCameraShakeSpeed;
        public ConfigEntry<float> ConfigCameraShakeIntensity;
        //---------------------------------------------------------------------
        //public TextMeshProUGUI PackListText;
        //public TextMeshProUGUI PackDescriptionText;
        //public TextMeshProUGUI PackAuthorText;
        //public List<string> PackList = new List<string>();
        //public int PackIndex = 0;
        //public ConfigEntry<int> configPackIndex;
        //public int PackOptionsIndex => configPackIndex.Value;
        //----------------------------------------------------------------------
        public static bool IsRaldiTweaksInstalled = false;
        public static bool IsNullscapeinBBInstalled = false;
        //----------------------------------------------------------------------
        public void Update()
        {
            IsNullscapeinBBInstalled = Chainloader.PluginInfos.ContainsKey("Nil.NullscapeinBB");
            IsRaldiTweaksInstalled = Chainloader.PluginInfos.ContainsKey("il.modded.raldi.tweaks");

            //-----------------------------------------------------------------


        }
        //---------------------------------------------------------------------
        public void Awake()
        {
            ConfigOpeningAnimations = Config.Bind
            (
                "General",
                "Opening Animations",
                true,
                "If true, the mod will show an opening animation before the game's warning screen."
            );
            ConfigMusicVolume = Config.Bind
            (
                "General",
                "Music Volume",
                10,
                "The volume of the music in this mod. (Max 10, Min 0)"
            );
            ConfigCameraShakeStyle = Config.Bind
            (
                "Camera",
                "Camera Shake Style",
                CameraShakeStyle.Beat,
                @"The camera shake style after you begining baldi time.
Disabled - Disabled camera shake;
Smooth - The camera will shake smoothly left and right;
Beat - The camera will sway rhythmically from side to side."
            );
            ConfigCameraShakeSpeed = Config.Bind
            (
                "Camera",
                "Camera Shake Speed",
                0.666f,
                "The camera shake speed after you begining baldi time."
            );
            ConfigCameraShakeIntensity = Config.Bind
            (
                "Camera",
                "Camera Shake Intensity",
                4f,
                "The camera shake intensity after you begining baldi time."
            );
            ConfigUniqueGenerator = Config.Bind
            (
                "Gameplay",
                "Unique Generator - WIP",
                false,
                @"If true, the mod's unique modification of the generator will be enabled:
Each floor has the floor 2 layout of the vanilla game (Expect Floor 5);
Floor type will not limited by the number of floors;
The classroom activity will be selected from all activities."
            );
            ConfigShowToppins = Config.Bind
            (
                "Gameplay",
                "Show Toppins",
                false,
                "If true, when you get a toppin, he/she will follow you as an ENTITY instead of hiding."
            );

            Instance = this;
            harmony.PatchAllConditionals();
            ModdedSaveGame.AddSaveHandler(base.Info);
            LoadingEvents.RegisterOnAssetsLoaded(base.Info, this.LoadAssets(), LoadingEventOrder.Start);
            GeneratorManagement.Register(this, GenerationModType.Addend, AddObjects);

            if (Chainloader.PluginInfos.ContainsKey("pixelguy.pixelmodding.baldiplus.custommainmenusapi"))
            {
                string corePath = Path.Combine(AssetLoader.GetModPath(this), ".Core");
                if (Directory.Exists(corePath))
                {
                    CustomMainMenuSupport.sprite = AssetLoader.SpriteFromFile(Path.Combine(corePath, "Menu.png"), new Vector2(0.5f, 0.5f));
                    AssetLoader.MidiFromFile(Path.Combine(corePath, "TitlePEPBRMG.mid"), "TitlePEPBRMG");
                    CustomMainMenuSupport.Setup();
                }
            }

            AddEnglishLocalization("Subtitles_English.json", ".Core");
            LoadResources(harmony);

            CustomOptionsCore.OnMenuInitialize += OnMen;
        }

        private void OnMen(OptionsMenu __instance, CustomOptionsHandler handler)
        {
            handler.AddCategory<Options_General>("BaldiTimeRE\n- General -");
            handler.AddCategory<Options_Packs>("BaldiTimeRE\n- Packs -");
        }

        public void LoadResources(Harmony harmony = null)
        {
            Log("Loading ResourcePacks, Please be patient .. (May take a long time)");

            AllPacks.Clear();
            AllPackStrings.Clear();
            LoadedPacks.Clear();

            AssetMan.ClearAll<AudioClip>();
            AssetMan.ClearAll<SoundObject>();
            AssetMan.ClearAll<Texture2D>();
            AssetMan.ClearAll<Sprite>();

            string resourcePackPath = Path.Combine(AssetLoader.GetModPath(this), "ResourcePacks");
            if (!Directory.Exists(resourcePackPath))
            {
                Directory.CreateDirectory(resourcePackPath);
            }
            string[] allResourcePackPaths = Directory.GetDirectories(resourcePackPath, "*", SearchOption.TopDirectoryOnly);

            if (allResourcePackPaths.Length <= 0 && harmony != null)
            {
                Log("No pack found, Please check mod's folder: " + resourcePackPath, 2);
                harmony.UnpatchSelf();
                StopAllCoroutines();
                return;
            }
            foreach (string path in allResourcePackPaths)
            {
                string packJsonPath = Path.Combine(path, "pack.json");
                if (File.Exists(packJsonPath))
                {
                    string packname = Path.GetFileName(path);
                    PackMeta packMeta = JsonConvert.DeserializeObject<PackMeta>(File.ReadAllText(Path.Combine(path, "pack.json")));
                    Log("Found Pack: " + packname);
                    AllPacks.Add(packname, packMeta);
                    AllPackStrings.Add(packname);
                }
                else
                {
                    Log("This pack hasn't 'pack.json'!, Path: " + path, 2);
                }
            }
            if (AllPacks.Count <= 0 && harmony != null)
            {
                Log("No pack found, Please check mod's folder: " + resourcePackPath, 2);
                harmony.UnpatchSelf();
                StopAllCoroutines();
                return;
            }

            string saveFilePath = Path.Combine(AssetLoader.GetModPath(this), ".Core", "Save.txt");
            if (!File.Exists(saveFilePath))
            {
                File.WriteAllText(saveFilePath, ".Vanilla");
            }
            else if (File.ReadAllText(saveFilePath).Length <= 0)
            {
                File.WriteAllText(saveFilePath, ".Vanilla");
            }
            string[] loadedPacks = File.ReadAllLines(saveFilePath);
            LoadedPacks.Add(".Vanilla");
            foreach (string loadedPack in loadedPacks)
            {
                if (AllPacks.ContainsKey(loadedPack) && loadedPack.Length > 0 && !LoadedPacks.Contains(loadedPack))
                {
                    LoadedPacks.Add(loadedPack);
                    Log("LoadedPack: " + loadedPack);
                }
            }
            File.WriteAllLines(saveFilePath, LoadedPacks);

            // AudioClips/Laps
            AddLapMusics();
            // AudioClips/Misc
            AddAudioClip("Meatophobia.ogg", "AudioClips/Misc");
            AddAudioClip("TimeForASmackdown.ogg", "AudioClips/Misc");
            // AudioClips/Spoop
            AddSpoopMusics();

            // SoundObjects/Effects
            AddSoundObject("BaldiFaceLaugh.ogg", "SoundObjects/Effects");
            AddSoundObject("bellcollectsmall.ogg", "SoundObjects/Effects");
            AddSoundObject("comboup1.ogg", "SoundObjects/Effects");
            AddSoundObject("comboup2.ogg", "SoundObjects/Effects");
            AddSoundObject("comboup3.ogg", "SoundObjects/Effects");
            AddSoundObject("JOHN_PILLAR_IMPACT.ogg", "SoundObjects/Effects");
            AddSoundObject("Lapping.ogg", "SoundObjects/Effects");
            AddSoundObject("rankdown1.ogg", "SoundObjects/Effects");
            AddSoundObject("rankdown2.ogg", "SoundObjects/Effects");
            AddSoundObject("rankdown3.ogg", "SoundObjects/Effects");
            AddSoundObject("rankdown4.ogg", "SoundObjects/Effects");
            AddSoundObject("rankdown5.ogg", "SoundObjects/Effects");
            AddSoundObject("rankup1.ogg", "SoundObjects/Effects");
            AddSoundObject("rankup2.ogg", "SoundObjects/Effects");
            AddSoundObject("rankup3.ogg", "SoundObjects/Effects");
            AddSoundObject("rankup4.ogg", "SoundObjects/Effects");
            AddSoundObject("rankup5.ogg", "SoundObjects/Effects");
            AddSoundObject("sfx_collecttoppin.ogg", "SoundObjects/Effects");
            AddSoundObject("sfx_explosion.ogg", "SoundObjects/Effects");
            AddSoundObject("Explosion.wav", "SoundObjects/Effects");
            AddSoundObject("sfx_lapenter.ogg", "SoundObjects/Effects");
            AddSoundObject("sfx_lapexit.ogg", "SoundObjects/Effects");
            AddSoundObject("sfx_taunt.ogg", "SoundObjects/Effects");
            AddSoundObject("sfx_parry.ogg", "SoundObjects/Effects");
            AddSoundObject("sfx_comboend.ogg", "SoundObjects/Effects");
            // SoundObjects/Effects/Rank
            AddSoundObject("Rank_D.ogg", "SoundObjects/Effects/Rank");
            AddSoundObject("Rank_C.ogg", "SoundObjects/Effects/Rank");
            AddSoundObject("Rank_B.ogg", "SoundObjects/Effects/Rank");
            AddSoundObject("Rank_A.ogg", "SoundObjects/Effects/Rank");
            AddSoundObject("Rank_S.ogg", "SoundObjects/Effects/Rank");
            AddSoundObject("Rank_P.ogg", "SoundObjects/Effects/Rank");
            AddSoundObject("Rank_L.ogg", "SoundObjects/Effects/Rank");

            // Textures/Entity
            AddTexture2D("BaldiFace.png", "Textures/Entity", new Vector2(0.5f, 0.4f), 8f);
            AddTexture2D("BaldiFace_Scraed.png", "Textures/Entity", new Vector2(0.5f, 0.4f), 8f);
            AddTexture2D("BaldiFace_Defeated.png", "Textures/Entity", new Vector2(0.5f, 0.4f), 8f);
            AddTexture2D("LapPortal_0.png", "Textures/Entity", true, 16f);
            AddTexture2D("LapPortal_1.png", "Textures/Entity", true, 16f);
            AddTexture2D("Toppins_0_Idle.png", "Textures/Entity", new Vector2(0.5f, 0.6f));
            AddTexture2D("Toppins_0_Yay.png", "Textures/Entity", new Vector2(0.5f, 0.6f));
            AddTexture2D("Toppins_1_Idle.png", "Textures/Entity", new Vector2(0.5f, 0.6f));
            AddTexture2D("Toppins_1_Yay.png", "Textures/Entity", new Vector2(0.5f, 0.6f));
            AddTexture2D("Toppins_2_Idle.png", "Textures/Entity", new Vector2(0.5f, 0.6f));
            AddTexture2D("Toppins_2_Yay.png", "Textures/Entity", new Vector2(0.5f, 0.6f));
            AddTexture2D("Toppins_3_Idle.png", "Textures/Entity", new Vector2(0.5f, 0.6f));
            AddTexture2D("Toppins_3_Yay.png", "Textures/Entity", new Vector2(0.5f, 0.6f));
            AddTexture2D("Toppins_4_Idle.png", "Textures/Entity", new Vector2(0.5f, 0.6f));
            AddTexture2D("Toppins_4_Yay.png", "Textures/Entity", new Vector2(0.5f, 0.6f));
            AddTexture2D("ToppinsCage.png", "Textures/Entity", new Vector2(0.5f, 0.45f), 10f);
            // Textures/GUI
            AddTexture2D("BaldiTimeLogo_0.png", "Textures/GUI", true);
            AddTexture2D("BaldiTimeLogo_1.png", "Textures/GUI", true);
            AddTexture2D("ComboDisplay_0.png", "Textures/GUI", true);
            AddTexture2D("ComboDisplay_1.png", "Textures/GUI", true);
            AddTexture2D("ComboDisplay_2.png", "Textures/GUI", true);
            AddTexture2D("ComboDisplay_3.png", "Textures/GUI", true);
            AddTexture2D("TimerBar_0.png", "Textures/GUI", true);
            AddTexture2D("TimerBar_1.png", "Textures/GUI", true);
            AddTexture2D("TimerBar_2.png", "Textures/GUI", true);
            AddTexture2D("TimerBar_3.png", "Textures/GUI", true);
            // Textures/GUI/ComboLevels
            AddComboLevels();
            // Textures/GUI/LapFlags
            AddTexture2D("Lap2Flag.png", "Textures/GUI/LapFlags", true);
            AddTexture2D("Lap3Flag.png", "Textures/GUI/LapFlags", true);
            // Textures/GUI/RankDisplay
            AddTexture2D("Rank_D_0.png", "Textures/GUI/RankDisplay", true);
            AddTexture2D("Rank_D_1.png", "Textures/GUI/RankDisplay", true);
            AddTexture2D("Rank_C_0.png", "Textures/GUI/RankDisplay", true);
            AddTexture2D("Rank_C_1.png", "Textures/GUI/RankDisplay", true);
            AddTexture2D("Rank_B_0.png", "Textures/GUI/RankDisplay", true);
            AddTexture2D("Rank_B_1.png", "Textures/GUI/RankDisplay", true);
            AddTexture2D("Rank_A_0.png", "Textures/GUI/RankDisplay", true);
            AddTexture2D("Rank_A_1.png", "Textures/GUI/RankDisplay", true);
            AddTexture2D("Rank_S_0.png", "Textures/GUI/RankDisplay", true);
            AddTexture2D("Rank_P_0.png", "Textures/GUI/RankDisplay", true);
            AddTexture2D("Rank_L_0.png", "Textures/GUI/RankDisplay", true);
            // Textures/Items
            AddTexture2D("BaldiClockIcon_Large.png", "Textures/Items", true);
            AddTexture2D("BaldiClockIcon_Large_Transparent.png", "Textures/Items", true);
            // Textures/Misc
            AddTexture2D("Notebook_John.png", "Textures/Misc", true, 100f);
            // Textures/Misc/Opening
            AddOpeningStuffs();
            // Textures/Misc/RankAnime
            AddTexture2D("RankAnime_Student_0.png", "Textures/Misc/RankAnime", true);
            AddTexture2D("RankAnime_Student_1.png", "Textures/Misc/RankAnime", true);
            AddTexture2D("RankAnime_Student_2.png", "Textures/Misc/RankAnime", true);
            AddTexture2D("RankAnime_Student_3.png", "Textures/Misc/RankAnime", true);
            AddTexture2D("RankAnime_Student_4.png", "Textures/Misc/RankAnime", true);
            AddTexture2D("RankAnime_Student_5.png", "Textures/Misc/RankAnime", true);
            AddTexture2D("RankAnime_Student_D.png", "Textures/Misc/RankAnime", true);
            AddTexture2D("RankAnime_Student_C.png", "Textures/Misc/RankAnime", true);
            AddTexture2D("RankAnime_Student_B.png", "Textures/Misc/RankAnime", true);
            AddTexture2D("RankAnime_Student_A.png", "Textures/Misc/RankAnime", true);
            AddTexture2D("RankAnime_Student_S.png", "Textures/Misc/RankAnime", true);
            AddTexture2D("RankAnime_Student_P.png", "Textures/Misc/RankAnime", true);
            AddTexture2D("RankAnime_Rank_D.png", "Textures/Misc/RankAnime", true);
            AddTexture2D("RankAnime_Rank_C.png", "Textures/Misc/RankAnime", true);
            AddTexture2D("RankAnime_Rank_B.png", "Textures/Misc/RankAnime", true);
            AddTexture2D("RankAnime_Rank_A.png", "Textures/Misc/RankAnime", true);
            AddTexture2D("RankAnime_Rank_S.png", "Textures/Misc/RankAnime", true);
            AddTexture2D("RankAnime_Rank_P.png", "Textures/Misc/RankAnime", true);
            AddTexture2D("RankAnime_BaldiFace.png", "Textures/Misc/RankAnime", true);
            // Textures/Misc/TitleCard
            AddTitleCardStuffs();

            BaldiTimeUI.RankDSprite = AssetMan.Get<Sprite>("Rank_D_0");
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

            //MTM101BaldAPI.Registers.ItemMetaStorage.Instance.Get(null).tags.Contains("");

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
            lapPortal.sprites[0] = AssetMan.Get<Sprite>("LapPortal_0");
            lapPortal.sprites[1] = AssetMan.Get<Sprite>("LapPortal_1");
            lapPortal.spriteRenderer[0].sprite = lapPortal.sprites[0];
            AssetMan.Add("LapPortal", lapPortal);

            BaldiFace baldiFace = new NPCBuilder<BaldiFace>(Info)
                .SetName("BaldiFace")
                .SetEnum("BaldiFace")
                .IgnorePlayerOnSpawn()
                .SetAudioTimescaleType(TimeScaleType.Npc)
                .AddMetaFlag(NPCFlags.StandardNoCollide)
                .SetAirborne()
                .IgnorePlayerVisibility()
                .SetWanderEnterRooms()
                .Build();
            baldiFace.spriteRenderer[0].sprite = AssetMan.Get<Sprite>("BaldiFace");
            //baldiFace.spriteRenderer[0].
            AssetMan.Add("BaldiFace", baldiFace);

            yield break;
        }

        internal void AddObjects(string floorName, int floorNumber, SceneObject sceneObject)
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
        internal void AddTitleCardStuffs()
        {
            BaldiTimeAnimations.TitleCardBackSprites.Clear();
            BaldiTimeAnimations.TitleCardTitleSprites.Clear();
            BaldiTimeAnimations.TitleCardSounds.Clear();

            string chlidPath = "Textures/Misc/TitleCard";
            int basePackInt = 0;

            for (int i = LoadedPacks.Count - 1; i >= 0; i--)
            {
                string pack = LoadedPacks[i];
                if (AllPacks[pack].replaceTitleCard)
                {
                    basePackInt = i;
                    break;
                }
            }

            for (int i = LoadedPacks.Count - 1; i >= basePackInt; i--)
            {
                string pack = LoadedPacks[i];
                string packPath = Path.Combine(AssetLoader.GetModPath(this), "ResourcePacks", pack, chlidPath);
                if (Directory.Exists(packPath))
                {
                    string[] getfiles = Directory.GetFiles(packPath, "*.png", SearchOption.TopDirectoryOnly);
                    if (getfiles.Length > 0)
                    {
                        foreach (string file in getfiles)
                        {
                            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                            string fileName = Path.GetFileName(file);
                            if (!fileName.Contains("-Title.png"))
                            {
                                Log(fileName);

                                BaldiTimeAnimations.TitleCardBackSprites.Add(AssetLoader.SpriteFromFile(Path.Combine(packPath, fileName), new Vector2(0.5f, 0.5f)));

                                BaldiTimeAnimations.TitleCardTitleSprites.Add(AssetLoader.SpriteFromFile(Path.Combine(packPath, fileNameWithoutExtension + "-Title.png"), new Vector2(0.5f, 0.5f)));

                                SoundObject soundObject = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(packPath, fileNameWithoutExtension + "-Sound.ogg")), "Nothing", SoundType.Music, Color.white, 0f);
                                BaldiTimeAnimations.TitleCardSounds.Add(soundObject);
                            }
                        }
                    }
                }
            }
        }
        internal void AddOpeningStuffs()
        {
            string chlidPath = "Textures/Misc/Opening";

            AddTexture2D("Border.png", chlidPath, true);
            BaldiTimeAnimations.Border_Sprite = AssetMan.Get<Sprite>("Border");

            for (int i = 0; i < 3; i++)
            {
                string fileName = "0_" + i.ToString();
                AddTexture2D(fileName + ".png", chlidPath, true);
                BaldiTimeAnimations.Sprites_0[i] = AssetMan.Get<Sprite>(fileName);
            }
            for (int i = 0; i < 2; i++)
            {
                string fileName = "1_" + i.ToString();
                AddTexture2D(fileName + ".png", chlidPath, true);
                BaldiTimeAnimations.Sprites_1[i] = AssetMan.Get<Sprite>(fileName);
            }
            for (int i = 0; i < 4; i++)
            {
                string fileName = "2_" + i.ToString();
                AddTexture2D(fileName + ".png", chlidPath, true);
                BaldiTimeAnimations.Sprites_2[i] = AssetMan.Get<Sprite>(fileName);
            }
            for (int i = 0; i < 2; i++)
            {
                string fileName = "2_2_" + (i + 1).ToString();
                AddTexture2D(fileName + ".png", chlidPath, true);
                BaldiTimeAnimations.Sprites_2_2[i] = AssetMan.Get<Sprite>(fileName);
            }
            for (int i = 0; i < 4; i++)
            {
                string fileName = "3_" + i.ToString();
                AddTexture2D(fileName + ".png", chlidPath, true);
                BaldiTimeAnimations.Sprites_3[i] = AssetMan.Get<Sprite>(fileName);
            }
            for (int i = 0; i < 2; i++)
            {
                string fileName = "3_4_" + i.ToString();
                AddTexture2D(fileName + ".png", chlidPath, true);
                BaldiTimeAnimations.Sprites_3_4[i] = AssetMan.Get<Sprite>(fileName);
            }
            for (int i = 0; i < 4; i++)
            {
                string fileName = "4_" + i.ToString();
                AddTexture2D(fileName + ".png", chlidPath, true);
                BaldiTimeAnimations.Sprites_4[i] = AssetMan.Get<Sprite>(fileName);
            }
            for (int i = 0; i < 4; i++)
            {
                string fileName = "5_" + i.ToString();
                AddTexture2D(fileName + ".png", chlidPath, true);
                BaldiTimeAnimations.Sprites_5[i] = AssetMan.Get<Sprite>(fileName);
            }
            for (int i = 0; i < 5; i++)
            {
                string fileName = "6_" + i.ToString();
                AddTexture2D(fileName + ".png", chlidPath, true);
                BaldiTimeAnimations.Sprites_6[i] = AssetMan.Get<Sprite>(fileName);
            }
            for (int i = 0; i < 8; i++)
            {
                string fileName = "7_" + i.ToString();
                AddTexture2D(fileName + ".png", chlidPath, true);
                BaldiTimeAnimations.Sprites_7[i] = AssetMan.Get<Sprite>(fileName);
            }
            for (int i = 0; i < 7; i++)
            {
                string fileName = "8_" + i.ToString();
                AddTexture2D(fileName + ".png", chlidPath, true);
                BaldiTimeAnimations.Sprites_8[i] = AssetMan.Get<Sprite>(fileName);
            }
            for (int i = 0; i < 7; i++)
            {
                string fileName = "9_" + i.ToString();
                AddTexture2D(fileName + ".png", chlidPath, true);
                BaldiTimeAnimations.Sprites_9[i] = AssetMan.Get<Sprite>(fileName);
            }
            for (int i = 0; i < 3; i++)
            {
                string fileName = "10_" + i.ToString();
                AddTexture2D(fileName + ".png", chlidPath, true);
                BaldiTimeAnimations.Sprites_10[i] = AssetMan.Get<Sprite>(fileName);
            }
            for (int i = 0; i < 3; i++)
            {
                string fileName = "11_" + i.ToString();
                AddTexture2D(fileName + ".png", chlidPath, true);
                BaldiTimeAnimations.Sprites_11[i] = AssetMan.Get<Sprite>(fileName);
            }
            for (int i = 0; i < 5; i++)
            {
                string fileName = "12_" + i.ToString();
                AddTexture2D(fileName + ".png", chlidPath, true);
                BaldiTimeAnimations.Sprites_12[i] = AssetMan.Get<Sprite>(fileName);
            }
            for (int i = 0; i < 2; i++)
            {
                string fileName = "13_" + i.ToString();
                AddTexture2D(fileName + ".png", chlidPath, true);
                BaldiTimeAnimations.Sprites_13[i] = AssetMan.Get<Sprite>(fileName);
            }
        }
        internal void AddComboLevels()
        {
            BaldiTimeUI.AllComboLevels.Clear();
            string chlidPath = "Textures/GUI/ComboLevels";
            int basePackInt = 0;

            for (int i = LoadedPacks.Count - 1; i >= 0; i--)
            {
                string pack = LoadedPacks[i];
                if (AllPacks[pack].replaceComboLevels)
                {
                    basePackInt = i;
                    break;
                }
            }

            for (int i = LoadedPacks.Count - 1; i >= basePackInt; i--)
            {
                string pack = LoadedPacks[i];
                string packPath = Path.Combine(AssetLoader.GetModPath(this), "ResourcePacks", pack, chlidPath);
                if (Directory.Exists(packPath))
                {
                    string[] getfiles = Directory.GetFiles(packPath, "*.png", SearchOption.TopDirectoryOnly);
                    if (getfiles.Length > 0)
                    {
                        foreach (string file in getfiles)
                        {
                            AddTexture2D(Path.GetFileName(file), chlidPath, true);
                            BaldiTimeUI.AllComboLevels.Add(AssetMan.Get<Sprite>(Path.GetFileNameWithoutExtension(file)));
                        }
                    }
                    else
                    {
                        Log("This pack hasn't ComboLevels: " + pack, 1);
                    }
                }
                else
                {
                    Log("This pack hasn't ComboLevels Folder: " + pack, 1);
                }
            }
        }
        internal void AddSpoopMusics()
        {
            BaldiTimeActions.AllSpoopMusics.Clear();
            string chlidPath = "AudioClips/Spoop";
            int basePackInt = 0;

            for (int i = LoadedPacks.Count - 1; i >= 0; i--)
            {
                string pack = LoadedPacks[i];
                if (AllPacks[pack].replaceSpoopMusic)
                {
                    basePackInt = i;
                    break;
                }
            }

            for (int i = LoadedPacks.Count - 1; i >= basePackInt; i--)
            {
                string pack = LoadedPacks[i];
                string packPath = Path.Combine(AssetLoader.GetModPath(this), "ResourcePacks", pack, chlidPath);
                if (Directory.Exists(packPath))
                {
                    string[] getfiles = Directory.GetFiles(packPath, "*.ogg", SearchOption.TopDirectoryOnly);
                    if (getfiles.Length > 0)
                    {
                        foreach (string file in getfiles)
                        {
                            AudioClip audio = AssetLoader.AudioClipFromFile(file);
                            audio.name = Path.GetFileNameWithoutExtension(file);
                            BaldiTimeActions.AllSpoopMusics.Add(audio);
                        }
                    }
                }
            }
        }
        internal void AddLapMusics()
        {
            string chlidPath = "AudioClips/Laps";
            string fileNameWithExtension = "Lap1-Loop.ogg";
            string selectedPack = null;

            for (int i = LoadedPacks.Count - 1; i >= 0; i--)
            {
                string pack = LoadedPacks[i];
                string file = Path.Combine(AssetLoader.GetModPath(this), "ResourcePacks", pack, chlidPath, fileNameWithExtension);
                if (File.Exists(file))
                {
                    selectedPack = pack;
                    break;
                }
            }
            if (selectedPack == null)
            {
                Log("Can't load Lap1 musics, Because not every files are exists (Must has Lap1-Loop.ogg).", 2);
            }
            else
            {
                AddAudioClip("Lap1-Intro.ogg", "AudioClips/Laps", selectedPack);
                AddAudioClip("Lap1-Loop.ogg", "AudioClips/Laps", selectedPack);
                AddAudioClip("Lap1-Outro.ogg", "AudioClips/Laps", selectedPack);
            }

            fileNameWithExtension = "Lap2-Loop.ogg";
            selectedPack = null;
            for (int i = LoadedPacks.Count - 1; i >= 0; i--)
            {
                string pack = LoadedPacks[i];
                string file = Path.Combine(AssetLoader.GetModPath(this), "ResourcePacks", pack, chlidPath, fileNameWithExtension);
                if (File.Exists(file))
                {
                    selectedPack = pack;
                    break;
                }
            }
            if (selectedPack == null)
            {
                Log("Can't load Lap2 musics, Because not every files are exists (Must has Lap2-Loop.ogg).", 2);
            }
            else
            {
                AddAudioClip("Lap2-Intro.ogg", "AudioClips/Laps", selectedPack);
                AddAudioClip("Lap2-Loop.ogg", "AudioClips/Laps", selectedPack);
            }

            fileNameWithExtension = "Lap3-Loop.ogg";
            selectedPack = null;
            for (int i = LoadedPacks.Count - 1; i >= 0; i--)
            {
                string pack = LoadedPacks[i];
                string file = Path.Combine(AssetLoader.GetModPath(this), "ResourcePacks", pack, chlidPath, fileNameWithExtension);
                if (File.Exists(file))
                {
                    selectedPack = pack;
                    break;
                }
            }
            if (selectedPack == null)
            {
                Log("Can't load Lap3 musics, Because not every files are exists (Must has Lap3-Loop.ogg).", 2);
            }
            else
            {
                AddAudioClip("Lap3-Intro.ogg", "AudioClips/Laps", selectedPack);
                AddAudioClip("Lap3-Loop.ogg", "AudioClips/Laps", selectedPack);
            }
        }
        internal void AddTexture2D(string fileNameWithExtension, string chlidPath, bool btwSprite = false, float pixelsPerUnit = 50f)
        {
            bool failed = true;
            for (int i = LoadedPacks.Count - 1; i >= 0; i--)
            {
                string pack = LoadedPacks[i];
                string file = Path.Combine(AssetLoader.GetModPath(this), "ResourcePacks", pack, chlidPath, fileNameWithExtension);

                //Log("Adding: " + Path.GetFileName(file));
                //Log(file);

                if (File.Exists(file))
                {
                    AssetMan.Add(Path.GetFileNameWithoutExtension(file), AssetLoader.TextureFromFile(file));
                    if (btwSprite)
                    {
                        Texture2D texture2D = AssetMan.Get<Texture2D>(Path.GetFileNameWithoutExtension(file));
                        AssetMan.Add(Path.GetFileNameWithoutExtension(file), AssetLoader.SpriteFromTexture2D(texture2D, pixelsPerUnit));
                    }
                    failed = false;
                    break;
                }
            }
            if (failed)
            {
                string file = Path.Combine(chlidPath, fileNameWithExtension);
                Log("File not found: " + file, 1);
            }
        }
        internal void AddTexture2D(string fileNameWithExtension, string chlidPath, Vector2 vector2, float pixelsPerUnit = 50f)
        {
            bool failed = true;
            for (int i = LoadedPacks.Count - 1; i >= 0; i--)
            {
                string pack = LoadedPacks[i];
                string file = Path.Combine(AssetLoader.GetModPath(this), "ResourcePacks", pack, chlidPath, fileNameWithExtension);
                if (File.Exists(file))
                {
                    AssetMan.Add(Path.GetFileNameWithoutExtension(file), AssetLoader.TextureFromFile(file));
                    Texture2D texture2D = AssetMan.Get<Texture2D>(Path.GetFileNameWithoutExtension(file));
                    AssetMan.Add(Path.GetFileNameWithoutExtension(file), AssetLoader.SpriteFromTexture2D(texture2D, vector2, pixelsPerUnit));
                    failed = false;
                    break;
                }
            }
            if (failed)
            {
                string file = Path.Combine(chlidPath, fileNameWithExtension);
                Log("File not found: " + file, 1);
            }
        }
        internal void AddSoundObject(string fileNameWithExtension, string chlidPath, string subtitle = "Nothing", SoundType soundType = SoundType.Effect, float sublength = 0f)
        {
            bool failed = true;
            for (int i = LoadedPacks.Count - 1; i >= 0; i--)
            {
                string pack = LoadedPacks[i];
                string file = Path.Combine(AssetLoader.GetModPath(this), "ResourcePacks", pack, chlidPath, fileNameWithExtension);
                if (File.Exists(file))
                {
                    SoundObject soundObject = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(file), subtitle, soundType, Color.white, sublength);
                    AssetMan.Add(Path.GetFileNameWithoutExtension(file), soundObject);
                    failed = false;
                    break;
                }
            }
            if (failed)
            {
                string file = Path.Combine(chlidPath, fileNameWithExtension);
                Log("File not found: " + file, 1);
            }
        }
        internal void AddAudioClip(string fileNameWithExtension, string chlidPath, string selectedPack = null)
        {
            if (selectedPack != null)
            {
                string file = Path.Combine(AssetLoader.GetModPath(this), "ResourcePacks", selectedPack, chlidPath, fileNameWithExtension);
                if (File.Exists(file))
                {
                    AssetMan.Add(Path.GetFileNameWithoutExtension(file), AssetLoader.AudioClipFromFile(file));
                }
                else
                {
                    file = Path.Combine(chlidPath, fileNameWithExtension);
                    Log("File not found: " + file, 1);
                }
                return;
            }

            bool failed = true;
            for (int i = LoadedPacks.Count - 1; i >= 0; i--)
            {
                string pack = LoadedPacks[i];
                string file = Path.Combine(AssetLoader.GetModPath(this), "ResourcePacks", pack, chlidPath, fileNameWithExtension);
                if (File.Exists(file))
                {
                    AssetMan.Add(Path.GetFileNameWithoutExtension(file), AssetLoader.AudioClipFromFile(file));
                    failed = false;
                    break;
                }
            }
            if (failed)
            {
                string file = Path.Combine(chlidPath, fileNameWithExtension);
                Log("File not found: " + file, 1);
            }
        }
        internal void AddTexture2DInCore(string fileNameWithExtension, bool btwSprite = true, float pixelsPerUnit = 50f)
        {
            string file = Path.Combine(AssetLoader.GetModPath(this), ".Core", fileNameWithExtension);
            if (File.Exists(file))
            {
                AssetMan.Add(Path.GetFileNameWithoutExtension(file), AssetLoader.TextureFromFile(file));
                if (btwSprite)
                {
                    Texture2D texture2D = AssetMan.Get<Texture2D>(Path.GetFileNameWithoutExtension(file));
                    AssetMan.Add(Path.GetFileNameWithoutExtension(file), AssetLoader.SpriteFromTexture2D(texture2D, pixelsPerUnit));
                }
            }
            else
            {
                Log("File not found: " + file, 2);
            }
        }
        internal void AddTexture2DInCore(string fileNameWithExtension, Vector2 vector2, float pixelsPerUnit = 50f)
        {
            string file = Path.Combine(AssetLoader.GetModPath(this), ".Core", fileNameWithExtension);
            if (File.Exists(file))
            {
                AssetMan.Add(Path.GetFileNameWithoutExtension(file), AssetLoader.TextureFromFile(file));
                Texture2D texture2D = AssetMan.Get<Texture2D>(Path.GetFileNameWithoutExtension(file));
                AssetMan.Add(Path.GetFileNameWithoutExtension(file), AssetLoader.SpriteFromTexture2D(texture2D, vector2, pixelsPerUnit));
            }
            else
            {
                Log("File not found: " + file, 2);
            }
        }
        internal void AddMidiInCore(string fileNameWithExtension)
        {
            string file = Path.Combine(AssetLoader.GetModPath(this), ".Core", fileNameWithExtension);
            if (File.Exists(file))
            {
                AssetLoader.MidiFromFile(fileNameWithExtension, Path.GetFileNameWithoutExtension(file));
            }
            else
            {
                Log("File not found: " + file, 2);
            }
        }
        internal void AddEnglishLocalization(string fileNameWithExtension, string chlidPath)
        {
            string path = Path.Combine(AssetLoader.GetModPath(this), chlidPath);
            if (!Directory.Exists(path))
            {
                Debug.LogError("Directory not found: " + path);
                return;
            }
            string[] getfiles = Directory.GetFiles(path, fileNameWithExtension);
            if (getfiles.Length <= 0)
            {
                Debug.LogError("File not found: " + path + "/" + fileNameWithExtension);
                return;
            }
            string getfile = getfiles[0];
            AssetLoader.LocalizationFromFile(getfile, Language.English);
        }

        internal void Log(object data, int level = 0)
        {
            if (level == 1)
            {
                Logger.LogWarning(data);
            }
            else if (level == 2)
            {
                Logger.LogError(data);
            }
            else
            {
                Logger.LogInfo(data);
            }
        }

        internal static void LogStatic(object data, int level = 0)
        {
            if (level == 1)
            {
                Instance.Logger.LogWarning(data);
            }
            else if (level == 2)
            {
                Instance.Logger.LogError(data);
            }
            else
            {
                Instance.Logger.LogInfo(data);
            }
        }
    }

    public enum CameraShakeStyle
    {
        Disabled,
        Smooth,
        Beat
    }
}
