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
    [BepInPlugin("ganaisthere.plus.itsbalditimerework", "Its Baldi Time: Rework", "0.0.0.0")]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]

    public class BasePlugin : BaseUnityPlugin
    {
        public static BasePlugin Instance { get; private set; }
        public static AssetManager AssetMan = new AssetManager();
        public static List<WeightedRoomAsset> classWeightedRoomAsset = new List<WeightedRoomAsset>();

        //---------------------------------------------------------------------
        public ConfigEntry<bool> ConfigUniqueGenerator;

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
            Instance = this;
            new Harmony("ganaisthere.plus.itsbalditimerework").PatchAllConditionals();
            ModdedSaveGame.AddSaveHandler(base.Info);
            AddEnglishLocalization("Subtitles_English.json");
            LoadingEvents.RegisterOnAssetsLoaded(base.Info, this.LoadAssets(), LoadingEventOrder.Start);
            GeneratorManagement.Register(this, GenerationModType.Addend, AddObjects);
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
            AddTexture2D("PointDisplay.png", "Textures/GUI");
            AddTexture2D("Rank_Sheet.png", "Textures/GUI");
            AddTexture2D("ComboDisplay_Sheet.png", "Textures/GUI");
            AddTexture2D("LapPortal_0.png", "Textures/Entity");
            AddTexture2D("LapPortal_1.png", "Textures/Entity");
            AddTexture2D("Lap2Flag.png", "Textures/GUI/LapFlags");

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

            yield return "Loading SoundEffects...";
            AddSoundObject("JOHN_PILLAR_IMPACT.ogg", "SoundObjects/Effects");
            BaldiTimeActions.JOHN_PILLAR_IMPACT = AssetMan.Get<SoundObject>("JOHN_PILLAR_IMPACT");
            AddSoundObject("bellcollectsmall.ogg", "SoundObjects/Effects");
            AddSoundObject("sfx_collecttoppin.ogg", "SoundObjects/Effects");
            AddAudioClip("Meatophobia.ogg", "SoundObjects/Effects");
            AddSoundObject("Lapping.ogg", "SoundObjects/Effects");
            AddSoundObject("sfx_lapenter.ogg", "SoundObjects/Effects");
            AddSoundObject("sfx_lapexit.ogg", "SoundObjects/Effects");
            AddSoundObject("comboup1.ogg", "SoundObjects/Effects");
            BaldiTimeUI.comboup[0] = AssetMan.Get<SoundObject>("comboup1");
            AddSoundObject("comboup2.ogg", "SoundObjects/Effects");
            BaldiTimeUI.comboup[1] = AssetMan.Get<SoundObject>("comboup2");
            AddSoundObject("comboup4.ogg", "SoundObjects/Effects");
            BaldiTimeUI.comboup[2] = AssetMan.Get<SoundObject>("comboup4");

            yield return "Loading Lap Musics...";
            AddAudioClip("Lap1-Intro.ogg", "SoundObjects/Laps");
            AddAudioClip("Lap1-Loop.ogg", "SoundObjects/Laps");
            AddAudioClip("Lap1-Outro.ogg", "SoundObjects/Laps");
            AddAudioClip("Lap2-Intro.ogg", "SoundObjects/Laps");
            AddAudioClip("Lap2-Loop.ogg", "SoundObjects/Laps");

            getfiles = Directory.GetFiles(AssetLoader.GetModPath(this) + "/SoundObjects/Spoop/", "*.ogg", SearchOption.TopDirectoryOnly);
            if (getfiles.Length > 0)
            {
                foreach (string file in getfiles)
                {
                    yield return "Loading Floor Musics...";
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                    string fileName = Path.GetFileName(file);
                    AddAudioClip(fileName, "SoundObjects/Spoop");
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

            yield return "Add ItemObjects...";
            ItemObject BaldiClock = new ItemBuilder(Info)
                .SetNameAndDescription("Itm_BaldiClock", "Desc_BaldiClock")
                .SetSprites(AssetMan.Get<Sprite>("BaldiClockIcon_Large"), AssetMan.Get<Sprite>("BaldiClockIcon_Large_Transparent"))
                .SetEnum("BaldiClock")
                .SetShopPrice(12251225)
                .SetGeneratorCost(12251225)
                .SetItemComponent<ITM_BaldiClock>()
                .SetMeta(ItemFlags.InstantUse, new string[]{"BaldiClock"})
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
            Texture2DToSprite("PointDisplay");
            BaldiTimeUI.PointDisplayBackgroundSprite = AssetMan.Get<Sprite>("PointDisplay");
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
