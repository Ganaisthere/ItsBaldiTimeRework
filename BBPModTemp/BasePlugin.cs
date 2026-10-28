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
using NilLib.Builders;
using PlusStudioLevelFormat;
using PlusStudioLevelLoader;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static BepInEx.BepInDependency;

namespace ItsBaldiTimeRework
{
    [BepInPlugin("ganaisthere.plus.itsbalditimerework", "Its Baldi Time: Reworked", "0.3.0.0")]

    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]
    [BepInDependency("mtm101.rulerp.baldiplus.levelstudioloader")]
    [BepInDependency("Nil.Library")]

    [BepInDependency("pixelguy.pixelmodding.baldiplus.custommainmenusapi", DependencyFlags.SoftDependency)]
    [BepInDependency("mtm101.rulerp.baldiplus.levelstudio", DependencyFlags.SoftDependency)]
    [BepInDependency("skid.librarylib", DependencyFlags.SoftDependency)]
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
            [JsonProperty("ReplaceLapMusic")]
            public bool replaceLapMusic;

            [JsonProperty("ComboTextColor")]
            public float[] comboTextColor;
            [JsonProperty("ComboTextShadowColor")]
            public float[] comboTextShadowColor;

            public PackMeta()
            {
                name = "Unnamed Pack";
                description = "No description added yet.";
                author = "IDK";

                replaceSpoopMusic = false;
                replaceTitleCard = false;
                replaceComboLevels = false;
                replaceLapMusic = false;

                comboTextColor = new float[4] { 11f, 45f, 14f, 1f };
                comboTextShadowColor = new float[4] { 11f, 45f, 14f, 1f };
            }
        }

        public class Texture2DMeta
        {
            [JsonProperty("PixelsPerUnit")]
            public float pixelsPerUnit;

            [JsonProperty("Pivot")]
            public string[] vector2;

            public Texture2DMeta()
            {
                pixelsPerUnit = 114514f;
                vector2 = new string[2] { "0.5f", "0.5f" };
            }
        }

        public static Dictionary<string, PackMeta> AllPacks = new Dictionary<string, PackMeta>();
        public static List<string> AllPackStrings = new List<string>();
        public static List<string> LoadedPacks = new List<string>();
        private static float[] ComboTextColor = new float[4] { 1f, 1f, 1f, 1f };
        private static float[] ComboTextShadowColor = new float[4] { 1f, 1f, 1f, 1f };
        public static Color GetComboTextColor()
        {
            if (ComboTextColor.Length == 4)
            {
                return new Color(ComboTextColor[0], ComboTextColor[1], ComboTextColor[2], ComboTextColor[3]);
            }
            return Color.white;
        }
        public static Color GetComboTextShadowColor()
        {
            if (ComboTextShadowColor.Length == 4)
            {
                return new Color(ComboTextShadowColor[0], ComboTextShadowColor[1], ComboTextShadowColor[2], ComboTextShadowColor[3]);
            }
            return Color.black;
        }
        //---------------------------------------------------------------------
        public ConfigEntry<bool> ConfigShowToppins;
        public ConfigEntry<bool> ConfigLapPortalPositions;
        public ConfigEntry<bool> ConfigOpeningAnimations;
        public ConfigEntry<int> ConfigMusicVolume;
        public ConfigEntry<CameraShakeStyle> ConfigCameraShakeStyle;
        public ConfigEntry<float> ConfigCameraShakeSpeed;
        public ConfigEntry<float> ConfigCameraShakeIntensity;
        public ConfigEntry<bool> ConfigBaldiFaceAfterImage;
        public ConfigEntry<bool> ConfigRemoveAllBaldiTimeAchievements;
        //---------------------------------------------------------------------
        public static MenuToggle ToggleBaldiFaceAfterImage;
        //----------------------------------------------------------------------
        public static bool IsRaldiTweaksInstalled => Chainloader.PluginInfos.ContainsKey("il.modded.raldi.tweaks");
        public static bool IsNullscapeinBBInstalled => Chainloader.PluginInfos.ContainsKey("Nil.NullscapeinBB");
        //----------------------------------------------------------------------
        public static RoomCategory SRCategory = EnumExtensions.ExtendEnum<RoomCategory>("BT_SecretRoom");
        public static List<WeightedRoomAsset> WeightedSecretRoomAssets = new List<WeightedRoomAsset>();
        //----------------------------------------------------------------------
        public void Update()
        {
            if (ToggleBaldiFaceAfterImage != null)
            {
                if (ToggleBaldiFaceAfterImage.gameObject != null)
                {
                    ConfigBaldiFaceAfterImage.Value = ToggleBaldiFaceAfterImage.Value;
                }
            }
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
            ConfigBaldiFaceAfterImage = Config.Bind
            (
                "General",
                "BaldiFace Effects",
                true,
                "If true, BaldiFace will have a red trailing effect when moving, just like what Pizzaface does in Pizzatower."
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
            ConfigShowToppins = Config.Bind
            (
                "Gameplay",
                "Show Toppins",
                true,
                "If true, when you get a toppin, he/she will follow you as an ENTITY instead of hiding."
            );
            ConfigLapPortalPositions = Config.Bind
            (
                "Gameplay",
                "Lap Portal Spawn Near Elevator",
                true,
                "If true, The Lap Portal will automatically teleport to the nearest elevator instead of staying at random places."
            );
            ConfigRemoveAllBaldiTimeAchievements = Config.Bind
            (
                "Debug",
                "Remove All BaldiTime Achievements",
                false,
                "If true, the mod will try clear all achievements added by this mod when you open the game (if Librarylib is installed), Then reset this configuration option to false."
            );

            Instance = this;
            harmony.PatchAllConditionals();
            ModdedSaveGame.AddSaveHandler(base.Info);
            LoadingEvents.RegisterOnAssetsLoaded(base.Info, this.LoadAssets(), LoadingEventOrder.Pre);
            GeneratorManagement.Register(this, GenerationModType.Addend, ChangeGenerators);
            CustomOptionsCore.OnMenuInitialize += OnMen;

            AddEnglishLocalization("Subtitles_English.json", ".Core");

            string corePath = Path.Combine(AssetLoader.GetModPath(this), ".Core");
            if (Other.CLC("pixelguy.pixelmodding.baldiplus.custommainmenusapi"))
            {
                CustomMainMenuSupport.sprite = AssetLoader.SpriteFromFile(Path.Combine(corePath, "Menu.png"), new Vector2(0.5f, 0.5f));
                AssetLoader.MidiFromFile(Path.Combine(corePath, "TitlePEPBRMG.mid"), "TitlePEPBRMG");
                CustomMainMenuSupport.Setup();
            }
            if (Other.CLC("skid.librarylib"))
            {
                LibrarylibAchievementsSupport.CreateAchievements();
            }

            LoadResources(harmony);
        }

        private void OnMen(OptionsMenu __instance, CustomOptionsHandler handler)
        {
            if (Other.CLC("skid.librarylib") && ConfigRemoveAllBaldiTimeAchievements.Value)
            {
                LibrarylibAchievementsSupport.ClearAllBaldiTimeAchievements();
            }
            ConfigRemoveAllBaldiTimeAchievements.Value = false;

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

            ComboTextColor = new float[4] { 1f, 1f, 1f, 1f };
            for (int i = LoadedPacks.Count - 1; i >= 0; i--)
            {
                string loadedPack = LoadedPacks[i];
                float[] comboTextColor = AllPacks[loadedPack].comboTextColor;

                Log(loadedPack);

                if (comboTextColor.Length == 4)
                {
                    if (comboTextColor[0] == 11 && comboTextColor[1] == 45 && comboTextColor[2] == 14)
                    {
                        Log("This Pack's Hasn't \"ComboTextColor\"! - " + loadedPack, 1);
                    }
                    else
                    {
                        ComboTextColor = comboTextColor;

                        string color = comboTextColor[0].ToString() + ", ";
                        color += comboTextColor[1].ToString() + ", ";
                        color += comboTextColor[2].ToString() + ", ";
                        color += comboTextColor[3].ToString();
                        Log("\"ComboTextColor\" Is Overrided By This Pack: " + loadedPack + ", Color: (" + color + ")", 1);

                        break;
                    }
                }
                else
                {
                    Log("This Pack's \"ComboTextColor\" Format Is Wrong! - " + loadedPack, 2);
                }
            }

            ComboTextShadowColor = new float[4] { 1f, 1f, 1f, 1f };
            for (int i = LoadedPacks.Count - 1; i >= 0; i--)
            {
                string loadedPack = LoadedPacks[i];
                float[] comboTextShadowColor = AllPacks[loadedPack].comboTextShadowColor;
                if (comboTextShadowColor.Length == 4)
                {
                    if (comboTextShadowColor[0] == 11 && comboTextShadowColor[1] == 45 && comboTextShadowColor[2] == 14)
                    {
                        Log("This Pack's Hasn't \"ComboTextShadowColor\"! - " + loadedPack, 1);
                    }
                    else
                    {
                        ComboTextShadowColor = comboTextShadowColor;

                        string color = comboTextShadowColor[0].ToString() + ", ";
                        color += comboTextShadowColor[1].ToString() + ", ";
                        color += comboTextShadowColor[2].ToString() + ", ";
                        color += comboTextShadowColor[3].ToString();
                        Log("\"ComboTextShadowColor\" Is Overrided By This Pack: " + loadedPack + ", Color: (" + color + ")", 1);

                        break;
                    }
                }
                else
                {
                    Log("This Pack's \"ComboTextShadowColor\" Format Is Wrong! - " + loadedPack, 2);
                }
            }

            // AudioClips/Laps
            AddLapMusics();
            // AudioClips/Misc
            AddAudioClip("Meatophobia.ogg", "AudioClips/Misc");
            AddAudioClip("TimeForASmackdown.ogg", "AudioClips/Misc");
            // AudioClips/Spoop
            AddSpoopMusics();

            // SoundObjects/Effects
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
            AddSoundObject("sfx_secretfound.ogg", "SoundObjects/Effects");
            AddSoundObject("sfx_explosion.ogg", "SoundObjects/Effects");
            AddSoundObject("Explosion.wav", "SoundObjects/Effects");
            AddSoundObject("sfx_lapenter.ogg", "SoundObjects/Effects");
            AddSoundObject("sfx_lapexit.ogg", "SoundObjects/Effects");
            AddSoundObject("sfx_taunt.ogg", "SoundObjects/Effects");
            AddSoundObject("sfx_parry.ogg", "SoundObjects/Effects");
            AddSoundObject("sfx_comboend.ogg", "SoundObjects/Effects");
            AddSoundObject("WarTimerUp.ogg", "SoundObjects/Effects");
            // SoundObjects/Effects/Rank
            AddSoundObject("Rank_D.ogg", "SoundObjects/Effects/Rank");
            AddSoundObject("Rank_C.ogg", "SoundObjects/Effects/Rank");
            AddSoundObject("Rank_B.ogg", "SoundObjects/Effects/Rank");
            AddSoundObject("Rank_A.ogg", "SoundObjects/Effects/Rank");
            AddSoundObject("Rank_S.ogg", "SoundObjects/Effects/Rank");
            AddSoundObject("Rank_P.ogg", "SoundObjects/Effects/Rank");
            AddSoundObject("Rank_L.ogg", "SoundObjects/Effects/Rank");
            // SoundObjects/Voice
            AddSoundObject("BaldiFaceLaugh.ogg", "SoundObjects/Voice", "Nothing", SoundType.Voice);
            AddSoundObject("Null_PreBoss_Start.ogg", "SoundObjects/Voice", "Vfx_Null_PreBoss_Start", SoundType.Voice, -1f);

            // Textures/Entity
            AddTexture2D("BaldiFace.png", "Textures/Entity", new Vector2(0.5f, 0.4f), 8f);
            AddTexture2D("BaldiFace_Scraed.png", "Textures/Entity", new Vector2(0.5f, 0.4f), 8f);
            AddTexture2D("BaldiFace_Defeated.png", "Textures/Entity", new Vector2(0.5f, 0.4f), 8f);
            AddTexture2D("LapPortal_0.png", "Textures/Entity", new Vector2(0.5f, 0.4f), 32f);
            AddTexture2D("LapPortal_1.png", "Textures/Entity", new Vector2(0.5f, 0.4f), 32f);
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
            AddTexture2D("Filename2.png", "Textures/Entity", new Vector2(0.5f, 0.4f), 64f);
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
            AddTexture2D("TimerBar_4.png", "Textures/GUI", true);
            AddTexture2D("TimerBar_5.png", "Textures/GUI", true);
            AddTexture2D("WarTimer_0.png", "Textures/GUI", true);
            AddTexture2D("WarTimer_1.png", "Textures/GUI", true);
            // Textures/GUI/ComboLevels
            AddComboLevels();
            // Textures/GUI/LapFlags
            AddTexture2D("Lap2Flag.png", "Textures/GUI/LapFlags", true);
            AddTexture2D("Lap3Flag.png", "Textures/GUI/LapFlags", true);
            AddTexture2D("Lap4Flag.png", "Textures/GUI/LapFlags", true);
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
        }

        private IEnumerator LoadAssets()
        {
            yield return 1225;

            foreach (var i in Chainloader.PluginInfos)
            {
                Log("----------------------");
                Log(i.Key);
            }
            Log("----------------------");

            yield return "Adding ItemObjects...";
            ItemObject BaldiClock = new ItemBuilder(Info)
                .SetNameAndDescription("Itm_BaldiClock", "Desc_BaldiClock")
                .SetSprites(AssetMan.Get<Sprite>("BaldiClockIcon_Large"), AssetMan.Get<Sprite>("BaldiClockIcon_Large_Transparent"))
                .SetEnum("BaldiClock")
                .SetShopPrice(12251225)
                .SetGeneratorCost(12251225)
                .SetItemComponent<ITM_BaldiClock>()
                .SetMeta(
                    ItemFlags.InstantUse,
                    new string[]
                    {
                        "BaldiClock",
                        "crmp_iou_sticker_nooverride",
                        "crmp_iou_sticker_nodisguise"
                    })
                .SetAsInstantUse()
                .SetPickupSound(AssetMan.Get<SoundObject>("bellcollectsmall"))
                .Build();
            AssetMan.Add("BaldiClock", BaldiClock);
            LevelLoaderPlugin.Instance.itemObjects.Add("BaldiClock", BaldiClock);

            //MTM101BaldAPI.Registers.ItemMetaStorage.Instance.Get(null).tags.Contains("");

            yield return "Adding NPCs...";

            Texture2D PosterTexture = AddTexture2DInCore("pri_toppins.png", false, "Posters");
            Toppin toppin = new NPCBuilder<Toppin>(Info)
                .SetName("Toppin")
                .SetEnum("Toppin")
                .IgnorePlayerOnSpawn()
                .SetAudioTimescaleType(TimeScaleType.Player)
                .SetMetaTags(new string[]
                    {
                        "ignoreGabby",
                        "RebProtection",
                        "RMS_VensaCanNotEat",
                        "RMS_Guilar_NoStrings",
                        "npp_flare_fireimmune",
                        "npp_felix_ignore",
                        "npp_detentionman_ignore",
                        "npp_noloot",
                        "no_balloon_frenzy",
                        "nhp_stickers_wrathimmunity",
                        "nhp_npcs_ignoreportalstar",
                        "nhp_items_ignoreherring",
                        "nhp_events_canteat"
                    })
                .AddMetaFlag(NPCFlags.StandardNoCollide | NPCFlags.NonStandardSpawn)
                .SetPoster(PosterTexture, "PST_PRI_Toppin1", "PST_PRI_Toppin2")
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
            LevelLoaderPlugin.Instance.npcAliases.Add("Toppin", toppin);
            PosterObject poster = toppin.Poster;
            LevelLoaderPlugin.Instance.posterAliases.Add(poster.baseTexture.name, poster);

            PosterTexture = AddTexture2DInCore("pri_lapportal.png", false, "Posters");
            LapPortal lapPortal = new NPCBuilder<LapPortal>(Info)
                .SetName("LapPortal")
                .SetEnum("LapPortal")
                .IgnorePlayerOnSpawn()
                .SetAudioTimescaleType(TimeScaleType.Player)
                .SetMetaTags(new string[]
                    {
                        "ignoreGabby",
                        "RebProtection",
                        "RMS_VensaCanNotEat",
                        "RMS_Guilar_NoStrings",
                        "npp_flare_fireimmune",
                        "npp_felix_ignore",
                        "npp_detentionman_ignore",
                        "npp_noloot",
                        "no_balloon_frenzy",
                        "nhp_leveltypes_bridgeignore",
                        "nhp_stickers_wrathimmunity",
                        "nhp_npcs_ignoreportalstar",
                        "nhp_items_ignoreherring",
                        "nhp_events_canteat"
                    })
                .AddMetaFlag(NPCFlags.HasSprite | NPCFlags.NonStandardSpawn)
                .SetAirborne()
                .SetPoster(PosterTexture, "PST_PRI_LapPortal1", "PST_PRI_LapPortal2")
                .Build();
            lapPortal.spriteRenderer[0].sprite = AssetMan.Get<Sprite>("LapPortal_0");
            AssetMan.Add("LapPortal", lapPortal);
            LevelLoaderPlugin.Instance.npcAliases.Add("LapPortal", lapPortal);
            poster = lapPortal.Poster;
            LevelLoaderPlugin.Instance.posterAliases.Add(poster.baseTexture.name, poster);

            PosterTexture = AddTexture2DInCore("pri_baldiface.png", false, "Posters");
            BaldiFace baldiFace = new NPCBuilder<BaldiFace>(Info)
                .SetName("BaldiFace")
                .SetEnum("BaldiFace")
                .IgnorePlayerOnSpawn()
                .SetAudioTimescaleType(TimeScaleType.Npc)
                .SetMetaTags(new string[]
                    {
                        "npp_noloot",
                        "no_balloon_frenzy",
                        "custom_mode_force_npc_in_list",
                        "nhp_leveltypes_bridgeignore",
                        "nhp_items_ignoreherring",
                        "nhp_events_canteat"
                    })
                .AddMetaFlag(NPCFlags.StandardNoCollide | NPCFlags.NonStandardSpawn)
                .SetAirborne()
                .IgnorePlayerVisibility()
                .SetWanderEnterRooms()
                .SetPoster(PosterTexture, "PST_PRI_BaldiFace1", "PST_PRI_BaldiFace2")
                .Build();
            baldiFace.spriteRenderer[0].sprite = AssetMan.Get<Sprite>("BaldiFace");
            AssetMan.Add("BaldiFace", baldiFace);
            LevelLoaderPlugin.Instance.npcAliases.Add("BaldiFace", baldiFace);
            poster = baldiFace.Poster;
            LevelLoaderPlugin.Instance.posterAliases.Add(poster.baseTexture.name, poster);

            PosterTexture = AddTexture2DInCore("pri_filename2.png", false, "Posters");
            FileName2 fileName2 = new NPCBuilder<FileName2>(Info)
                .SetName("FileName2")
                .SetEnum("FileName2")
                .IgnorePlayerOnSpawn()
                .SetAudioTimescaleType(TimeScaleType.Npc)
                .SetMetaTags(new string[]
                    {
                        "npp_noloot",
                        "no_balloon_frenzy",
                        "custom_mode_force_npc_in_list",
                        "nhp_items_ignoreherring",
                        "nhp_events_canteat"
                    })
                .AddMetaFlag(NPCFlags.StandardNoCollide | NPCFlags.NonStandardSpawn)
                .AddLooker()
                .SetWanderEnterRooms()
                .SetPoster(PosterTexture, "PST_PRI_FileName2-1", "PST_PRI_FileName2-2")
                .Build();
            fileName2.spriteRenderer[0].sprite = AssetMan.Get<Sprite>("FileName2");
            AssetMan.Add("FileName2", fileName2);
            LevelLoaderPlugin.Instance.npcAliases.Add("FileName2", fileName2);
            poster = fileName2.Poster;
            LevelLoaderPlugin.Instance.posterAliases.Add(poster.baseTexture.name, poster);

            yield return "Adding Rooms...";

            RoomFunctionContainer secretRoomFunctionContainer = RoomFunctionContainerBuilder.CreateBaseObject()
                .SetName("secretRoomFunctionContainer")
                .AddFunction<SecretRoomFunction>(out _)
                .AddFunction<CoverRoomFunction>(out _)
                .AddFunction<CoverInGameRoomFunction>(out _)
                .AddFunction<BlockNavigationRoomFunction>(out _)
                .Build();
            secretRoomFunctionContainer.gameObject.GetComponent<CoverInGameRoomFunction>().hardCover = true;

            AssetMan.Add("secretRoomFunctionContainer", secretRoomFunctionContainer);

            Texture2D SRT_C = AddTexture2DInCore("SecretRoomTile_Ceil.png", false, "Rooms/SecretRoom");
            Texture2D SRT_F = AddTexture2DInCore("SecretRoomTile_Floor.png", false, "Rooms/SecretRoom");
            Texture2D SRT_W = AddTexture2DInCore("SecretRoomTile_Wall.png", false, "Rooms/SecretRoom");
            Texture2D SRD_C = AddTexture2DInCore("Balls_Closed.png", false, "Rooms/SecretRoom");
            Texture2D SRD_O = AddTexture2DInCore("Balls_Opened.png", false, "Rooms/SecretRoom");

            LevelLoaderPlugin.Instance.roomTextureAliases.Add("SecretRoomTile_Ceil", SRT_C);
            LevelLoaderPlugin.Instance.roomTextureAliases.Add("SecretRoomTile_Floor", SRT_F);
            LevelLoaderPlugin.Instance.roomTextureAliases.Add("SecretRoomTile_Wall", SRT_W);

            StandardDoorMats SRD = ObjectCreators.CreateDoorDataObject("BallsDoor", SRD_O, SRD_C);
            
            RoomSettings SecretRoomSettings = new RoomSettings(SRCategory, RoomType.Room, new Color(0f, 0f, 0f, 0f), SRD)
            {
                container = secretRoomFunctionContainer
                //container = LevelLoaderPlugin.Instance.roomSettings["saferoom"].container
            };

            LevelLoaderPlugin.Instance.roomSettings.Add("SecretRoom", SecretRoomSettings);

            string mainRoomPath = Path.Combine(AssetLoader.GetModPath(this), ".Core", "Rooms", "SecretRoom");
            string[] roomPaths = Directory.GetFiles(mainRoomPath, "*.rbpl", SearchOption.TopDirectoryOnly);
            foreach (string filePath in roomPaths)
            {
                using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
                {
                    ExtendedRoomAsset asset = LevelImporter.CreateRoomAsset(BaldiRoomAsset.Read(reader), false);
                    asset.spawnWeight = 500;
                    asset.maxItemValue = 300;
                    asset.minItemValue = 50;
                    WeightedSecretRoomAssets.Add
                    (
                        new WeightedRoomAsset()
                        {
                            selection = asset,
                            weight = 100
                        }
                    );
                    reader.Close();
                }
            }

            if (Other.CLC("mtm101.rulerp.baldiplus.levelstudio"))
            {
                yield return "Adding Level Studio Support...";

                AddTexture2DInCore("room_secret.png", true, "EditorUI");
                AddTexture2DInCore("npc_lapportal.png", true, "EditorUI");
                AddTexture2DInCore("npc_toppin.png", true, "EditorUI");
                AddTexture2DInCore("npc_baldiface.png", true, "EditorUI");
                AddTexture2DInCore("npc_filename2.png", true, "EditorUI");

                LevelStudioSupport.Setup();
            }

            yield break;
        }

        internal void ChangeGenerators(string floorName, int floorNumber, SceneObject sceneObject)
        {
            CustomLevelObject[] customLevelObjects = CustomLevelObjectExtensions.GetCustomLevelObjects(sceneObject);
            if (floorName.StartsWith("F"))
            {
                foreach (CustomLevelObject CLO in customLevelObjects)
                {
                    RoomGroup SRRoomGroup = new RoomGroup()
                    {
                        name = "SecretRoom",
                        maxRooms = 3,
                        minRooms = 3,
                        stickToHallChance = 0.5f,
                        ceilingTexture = new WeightedTexture2D[]
                        {
                            new WeightedTexture2D()
                            {
                                selection = AssetMan.Get<Texture2D>("SecretRoomTile_Ceil"),
                                weight = 100
                            }
                        },
                        floorTexture = new WeightedTexture2D[]
                        {
                            new WeightedTexture2D()
                            {
                                selection = AssetMan.Get<Texture2D>("SecretRoomTile_Floor"),
                                weight = 100
                            }
                        },
                        wallTexture = new WeightedTexture2D[]
                        {
                            new WeightedTexture2D()
                            {
                                selection = AssetMan.Get<Texture2D>("SecretRoomTile_Wall"),
                                weight = 100
                            }
                        },
                        light = new WeightedTransform[]
                        {
                            new WeightedTransform
                            {
                                selection = LevelLoaderPlugin.Instance.lightTransforms["fluorescent"],
                                weight = 100
                            }
                        },
                        potentialRooms = WeightedSecretRoomAssets.ToArray(),
                    };

                    CLO.roomGroup = CLO.roomGroup.AddRangeToArray(new RoomGroup[] { SRRoomGroup });
                    CLO.MarkAsNeverUnload();
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
            int basePackInt = 0;

            for (int i = LoadedPacks.Count - 1; i >= 0; i--)
            {
                string pack = LoadedPacks[i];
                if (AllPacks[pack].replaceLapMusic)
                {
                    basePackInt = i;
                    break;
                }
            }
            void AddMusic(string fullName)
            {
                for (int i = LoadedPacks.Count - 1; i >= basePackInt; i--)
                {
                    string pack = LoadedPacks[i];
                    string packPath = Path.Combine(AssetLoader.GetModPath(this), "ResourcePacks", pack, chlidPath, fullName);
                    if (File.Exists(packPath))
                    {
                        AudioClip audio = AssetLoader.AudioClipFromFile(packPath);
                        audio.name = Path.GetFileNameWithoutExtension(packPath);
                        AssetMan.Add(audio.name, audio);
                        break;
                    }
                }
                Log("Couldn't Find " + fullName, 1);
            }

            AddMusic("Lap1-Intro.ogg");
            AddMusic("Lap1-Loop.ogg");
            AddMusic("Lap1-Outro.ogg");

            for (int i = 2; i <= 4; i++)
            {
                string name = "Lap" + i.ToString();
                AddMusic(name + "-Intro.ogg");
                AddMusic(name + "-Loop.ogg");
            }
        }
        internal void AddTexture2D(string fileNameWithExtension, string chlidPath, bool btwSprite = false, float pixelsPerUnit = 50f)
        {
            bool failed = true;
            for (int i = LoadedPacks.Count - 1; i >= 0; i--)
            {
                string pack = LoadedPacks[i];
                string file = Path.Combine(AssetLoader.GetModPath(this), "ResourcePacks", pack, chlidPath, fileNameWithExtension);

                if (File.Exists(file))
                {
                    AssetMan.Add(Path.GetFileNameWithoutExtension(file), AssetLoader.TextureFromFile(file));
                    if (btwSprite)
                    {
                        Texture2D texture2D = AssetMan.Get<Texture2D>(Path.GetFileNameWithoutExtension(file));

                        float pixels = pixelsPerUnit;
                        Vector2 vector = new Vector2(0.5f, 0.5f);
                        string file2 = Path.Combine(AssetLoader.GetModPath(this), "ResourcePacks", pack, chlidPath, Path.GetFileNameWithoutExtension(file) + ".json");
                        if (File.Exists(file2))
                        {
                            Texture2DMeta texture2DMeta = JsonConvert.DeserializeObject<Texture2DMeta>(File.ReadAllText(file2));
                            if (texture2DMeta.pixelsPerUnit != 114514f)
                            {
                                pixels = texture2DMeta.pixelsPerUnit;
                                Log(fileNameWithExtension + " - Overrided PixelsPerUnit: " + pixels, 1);
                            }
                            if (texture2DMeta.vector2.Length == 2)
                            {
                                if (texture2DMeta.vector2[0] != "114f" && texture2DMeta.vector2[1] != "514f")
                                {
                                    float a = 0.5f;
                                    float b = 0.5f;
                                    if (float.TryParse(texture2DMeta.vector2[0], out float c))
                                    {
                                        a = c;
                                    }
                                    else if (int.TryParse(texture2DMeta.vector2[0], out int d))
                                    {
                                        a = d;
                                    }
                                    if (float.TryParse(texture2DMeta.vector2[1], out float e))
                                    {
                                        b = e;
                                    }
                                    else if (int.TryParse(texture2DMeta.vector2[1], out int f))
                                    {
                                        b = f;
                                    }
                                    vector = new Vector2(a, b);
                                    Log(fileNameWithExtension + " - Overrided Pivot: " + vector, 1);
                                }
                            }
                        }

                        /*string file3 = Path.Combine(AssetLoader.GetModPath(this), "ResourcePacks", ".Vanilla", chlidPath, Path.GetFileNameWithoutExtension(file) + ".json");
                        string[] Lines = new string[] {
                            "{",
                            "       \"PixelsPerUnit\": " + pixelsPerUnit.ToString() + ",",
                            "       \"Pivot\": [0.5f, 0.5f],",
                            "}"
                        };
                        File.WriteAllLines(file3, Lines);*/

                        AssetMan.Add(Path.GetFileNameWithoutExtension(file), AssetLoader.SpriteFromTexture2D(texture2D, vector, pixels));
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

                    float pixels = pixelsPerUnit;
                    Vector2 vector = vector2;
                    string file2 = Path.Combine(AssetLoader.GetModPath(this), "ResourcePacks", pack, chlidPath, Path.GetFileNameWithoutExtension(file) + ".json");
                    if (File.Exists(file2))
                    {
                        Texture2DMeta texture2DMeta = JsonConvert.DeserializeObject<Texture2DMeta>(File.ReadAllText(file2));
                        if (texture2DMeta.pixelsPerUnit != 114514f)
                        {
                            pixels = texture2DMeta.pixelsPerUnit;
                            Log(fileNameWithExtension + " - Overrided PixelsPerUnit: " + pixels, 1);
                        }
                        if (texture2DMeta.vector2.Length == 2)
                        {
                            if (texture2DMeta.vector2[0] != "114f" && texture2DMeta.vector2[1] != "514f")
                            {
                                float a = vector2.x;
                                float b = vector2.y;
                                if (float.TryParse(texture2DMeta.vector2[0], out float c))
                                {
                                    a = c;
                                }
                                else if (int.TryParse(texture2DMeta.vector2[0], out int d))
                                {
                                    a = d;
                                }
                                if (float.TryParse(texture2DMeta.vector2[1], out float e))
                                {
                                    b = e;
                                }
                                else if (int.TryParse(texture2DMeta.vector2[1], out int f))
                                {
                                    b = f;
                                }
                                vector = new Vector2(a, b);
                                Log(fileNameWithExtension + " - Overrided Pivot: " + vector, 1);
                            }
                        }
                    }

                    /*string file3 = Path.Combine(AssetLoader.GetModPath(this), "ResourcePacks", ".Vanilla", chlidPath, Path.GetFileNameWithoutExtension(file) + ".json");
                    string[] Lines = new string[] {
                        "{",
                        "       \"PixelsPerUnit\": " + pixelsPerUnit.ToString() + ",",
                        "       \"Pivot\": [" + vector2.x.ToString() + ", " + vector2.y.ToString() + "],",
                        "}"
                    };
                    File.WriteAllLines(file3, Lines); */

                    AssetMan.Add(Path.GetFileNameWithoutExtension(file), AssetLoader.SpriteFromTexture2D(texture2D, vector, pixels));
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
        internal Texture2D AddTexture2DInCore(string fileNameWithExtension, bool btwSprite = true, string chlidPath = null, float pixelsPerUnit = 50f)
        {
            string file = Path.Combine(AssetLoader.GetModPath(this), ".Core", fileNameWithExtension);
            if (chlidPath != null)
            {
                file = Path.Combine(AssetLoader.GetModPath(this), ".Core", chlidPath, fileNameWithExtension);
            }
            if (File.Exists(file))
            {
                Texture2D texture = AssetLoader.TextureFromFile(file);
                AssetMan.Add(Path.GetFileNameWithoutExtension(file), texture);
                if (btwSprite)
                {
                    AssetMan.Add(Path.GetFileNameWithoutExtension(file), AssetLoader.SpriteFromTexture2D(texture, pixelsPerUnit));
                }
                return texture;
            }
            else
            {
                Log("File not found: " + file, 2);
            }
            return null;
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
        internal string AddMidiInCore(string fileNameWithExtension)
        {
            string file = Path.Combine(AssetLoader.GetModPath(this), ".Core", fileNameWithExtension);
            string midi = null;
            if (File.Exists(file))
            {
                midi = AssetLoader.MidiFromFile(fileNameWithExtension, Path.GetFileNameWithoutExtension(file));
            }
            else
            {
                Log("File not found: " + file, 2);
            }
            return midi;
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
