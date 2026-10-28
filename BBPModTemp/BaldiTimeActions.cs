using MTM101BaldAPI.PlusExtensions;
using MTM101BaldAPI.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ItsBaldiTimeRework
{
    public class BaldiTimeActions
    {
        public static int lap = 0;
        public static float pizzaTimer = 0f;
        public static float pizzaTimerTotal = 0f;
        public static bool itsBaldiTime = false;
        public static List<AudioClip> AllSpoopMusics = new List<AudioClip>();
        public static int SpoopMusicsRandom = -1;
        public static List<NPC> toppins = new List<NPC>();
        public static int toppinsTotal = 0;
        public static float points = 0f;
        public static float comboPoints = 0f;
        public static float pointsForPRank = 0f;
        public static List<string> ranks = new List<string>() { "D", "C", "B", "A", "S", "P", "L", "X" };
        public static string rank = "D";
        public static float combo = 0f;
        public static float comboTimer = 0f;
        public static float comboTimerMax = 20f;
        public static bool comboKeep = true;
        public static bool enteringLap = false;
        public static int notebookmax = 4;
        public static float staminaOld = 100f;
        public static List<RoomController> secretRoomFound = new List<RoomController>();
        public static int secretRoomTotal = 0;
        public static bool CanGetPRank => (lap >= 2) && comboKeep && (secretRoomFound.Count >= secretRoomTotal);
        public static float warTimer = 0f;
        public static bool war = false;

        public static void PlaySpoopMusic(BaseGameManager baseGameManager)
        {
            StopAllMusOnce();
            List<AudioClip> chaseMusics = new List<AudioClip>();
            if (AllSpoopMusics.Count > 0)
            {
                foreach (AudioClip music in AllSpoopMusics)
                {
                    chaseMusics.Add(music);

                    /*
                    string levelTitle = Singleton<CoreGameManager>.Instance.sceneObject.levelTitle;
                    bool StartWithF = levelTitle.StartsWith("F");
                    if (music.name.Contains("_" + Singleton<CoreGameManager>.Instance.sceneObject.levelTitle) || !StartWithF)
                    {
                        chaseMusics.Add(music);
                    }
                    if (!music.name.Contains("_F1") && !music.name.Contains("_F2") && !music.name.Contains("_F3") && !music.name.Contains("_F4") && !music.name.Contains("_F5"))
                    {
                        chaseMusics.Add(music);
                    }
                    */
                }
            }
            if (chaseMusics.Count > 0)
            {
                if (SpoopMusicsRandom == -1)
                {
                    SpoopMusicsRandom = Random.Range(0, chaseMusics.Count - 1);
                }
                AudioClip choosedMusic = chaseMusics[SpoopMusicsRandom];
                BaseGameManagerPatches.musPlayer.Stop();
                BaseGameManagerPatches.musPlayer.Play(choosedMusic, true);
            }
        }
        public static void AddCombo(float comboAdd, float time = 1225f)
        {
            combo += comboAdd;
            comboPoints += 100f * comboAdd;
            if (combo <= 0f)
            {
                return;
            }
            comboTimer += time;
            if (comboTimer > comboTimerMax || time == 1225f)
            {
                comboTimer = comboTimerMax;
            }
        }
        public static IEnumerator StopAllMus(BaseGameManager baseGameManager)
        {
            for (int i = 0; i < 3; i++)
            {
                if (baseGameManager == null)
                {
                    yield break;
                }
                Singleton<MusicManager>.Instance.StopMidi();
                Singleton<CoreGameManager>.Instance.musicMan.FlushQueue(true);
                /*
                foreach (AudioManager audioMan in Object.FindObjectsOfType<AudioManager>())
                {
                    if (audioMan != Singleton<CoreGameManager>.Instance.audMan)
                    {
                        audioMan.FlushQueue(true);
                    }
                }
                */
                /*
                foreach (AudioSource audioSou in Object.FindObjectsOfType<AudioSource>())
                {
                    //BasePlugin.LogStatic(audioSou.gameObject.name);
                    if (audioSou.spatialBlend == 0f && !audioSou.gameObject.name.Contains("CoreGameManager") && !audioSou.gameObject.name.Contains("CustomMusicPlayer"))
                    {
                        audioSou.Stop();
                    }
                }
                */
                foreach (AudioSourceManager audioMan in Object.FindObjectsOfType<AudioSourceManager>())
                {
                    AudioSource audioSou = audioMan.ReflectionGetVariable("mscSource") as AudioSource;
                    if ((bool)audioMan.ReflectionGetVariable("mscSourceEnabled"))
                    {
                        audioSou.Stop();
                    }
                }
                yield return null;
            }
            yield break;
        }
        public static void StopAllMusOnce()
        {
            Singleton<MusicManager>.Instance.StopMidi();
            Singleton<CoreGameManager>.Instance.musicMan.FlushQueue(true);
            foreach (AudioSourceManager audioMan in Object.FindObjectsOfType<AudioSourceManager>())
            {
                AudioSource audioSou = audioMan.ReflectionGetVariable("mscSource") as AudioSource;
                if ((bool)audioMan.ReflectionGetVariable("mscSourceEnabled"))
                {
                    audioSou.Stop();
                }
            }
        }
        public static IEnumerator ComboMechanism(BaseGameManager baseGameManager)
        {
            comboKeep = true;
            bool idk = false;
            bool idktoo = false;
            while (baseGameManager != null)
            {
                if (idk)
                {
                    if (comboTimer > comboTimerMax)
                    {
                        comboTimer = comboTimerMax;
                    }
                    if (comboTimer > 0f)
                    {
                        if (ElevatorScreenPatches.death && !Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.Entity.Frozen)
                        {
                            comboTimer -= Time.deltaTime * baseGameManager.Ec.EnvironmentTimeScale;
                        }
                        else
                        {
                            comboTimer = comboTimerMax;
                        }
                        if (combo % 5 == 0f && !idktoo)
                        {
                            idktoo = true;
                            if (Singleton<CoreGameManager>.Instance.GetHud(0).transform.TryFind("BaldiTimeHudManager", out Transform transform))
                            {
                                BaldiTimeHudManager manager = transform.gameObject.GetComponent<BaldiTimeHudManager>();
                                manager.StartCoroutine(manager.ShowComboLevel());
                            }
                        }
                        if (combo % 5 != 0f && idktoo)
                        {
                            idktoo = false;
                        }
                    }
                    else
                    {
                        points += comboPoints;
                        comboPoints = 0;
                        combo = 0f;
                        comboTimer = 0f;
                        comboKeep = false;
                        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("sfx_comboend"));
                        idk = false;
                    }
                }
                else
                {
                    if (combo > 0)
                    {
                        idk = true;
                    }
                }
                yield return null;
            }
            yield break;
        }
        public static void Setup(BaseGameManager baseGameManager)
        {
            war = false;
            staminaOld = Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.StaminaMax;
            points = 0f;
            comboPoints = 0f;
            pointsForPRank = 0f;
            rank = "D";
            secretRoomTotal = 0;
            secretRoomFound.Clear();
            toppinsTotal = 0;
            if (Singleton<CoreGameManager>.Instance.GetHud(0).transform.TryFind("BaldiTimeHudManager", out Transform transform))
            {
                transform.gameObject.GetComponent<BaldiTimeHudManager>().Initialize();
            }
            List<RandomEvent> events = baseGameManager.Ec.ReflectionGetVariable("events") as List<RandomEvent>;
            foreach (RandomEvent randomEvent in events)
            {
                if (randomEvent.Type == RandomEventType.TimeOut)
                {
                    events.Remove(randomEvent);
                    break;
                }
            }
            foreach (RandomEvent randomEvent in events)
            {
                if (randomEvent.Type == RandomEventType.Lockdown)
                {
                    events.Remove(randomEvent);
                    break;
                }
            }
            List<RoomController> rooms = baseGameManager.Ec.rooms;
            foreach (RoomController room in rooms)
            {
                int num = Random.Range(0, 1);
                ItemObject BaldiClock = BasePlugin.AssetMan.Get<ItemObject>("BaldiClock");
                if (num == 0 && room.pickups != null && BaldiClock != null)
                {
                    bool isNull = false;
                    if (room.pickups.Count < 0)
                    {
                        isNull = true;
                    }
                    foreach (Pickup pickup in room.pickups)
                    {
                        if (isNull)
                        {
                            break;
                        }
                        if (pickup == null)
                        {
                            isNull = true;
                        }
                        else if (pickup.item == null)
                        {
                            isNull = true;
                        }
                    }
                    if (!isNull)
                    {
                        baseGameManager.Ec.RespawnItemInRoom(BaldiClock, room);
                        pointsForPRank += 25f;
                        num = Random.Range(0, 2);
                        if (num == 0)
                        {
                            baseGameManager.Ec.RespawnItemInRoom(BaldiClock, room);
                            pointsForPRank += 25f;
                        }
                        List<Pickup> items = room.pickups;
                        foreach (Pickup item in items)
                        {
                            if (item.item == BaldiClock)
                            {
                                item.free = false;
                                item.price = 12251225;
                                item.itemSprite.sprite = BasePlugin.AssetMan.Get<Sprite>("BaldiClockIcon_Large_Transparent");
                            }
                        }
                    }
                }
            }
            List<RoomController> SpawnableRooms = new List<RoomController>();
            foreach (RoomController room in rooms)
            {
                if (room.functionObject.TryGetComponent<SecretRoomFunction>(out  _))
                {
                    secretRoomTotal += 1;
                }
                RoomCategory category = room.category;
                if (category == RoomCategory.Class || category == RoomCategory.Faculty || category == RoomCategory.Office)
                {
                    SpawnableRooms.Add(room);
                }
            }
            Toppin[] toppins = Object.FindObjectsOfType<Toppin>();
            for (int i = 0; i < toppins.Length; i++)
            {
                pointsForPRank += 1000f;
                toppinsTotal += 1;
            }
            if (toppins.Length < 5)
            {
                for (int i = 0; i < 5 - toppins.Length; i++)
                {
                    int num = Random.Range(0, SpawnableRooms.Count);
                    baseGameManager.Ec.SpawnNPC(BasePlugin.AssetMan.Get<Toppin>("Toppin"), SpawnableRooms[num].RandomEntitySafeCellNoGarbage().position);
                    pointsForPRank += 1000f;
                    toppinsTotal += 1;
                }
            }
            if (Object.FindObjectsOfType<LapPortal>().Length == 0)
            {
                int mun = Random.Range(0, SpawnableRooms.Count);
                baseGameManager.Ec.SpawnNPC(BasePlugin.AssetMan.Get<LapPortal>("LapPortal"), SpawnableRooms[mun].RandomEntitySafeCellNoGarbage().position);
            }
            pointsForPRank += 3000f;
            foreach (Activity activity in baseGameManager.Ec.activities)
            {
                if (activity.GetType() == typeof(NoActivity))
                {
                    pointsForPRank += 1050f;
                }
                else
                {
                    pointsForPRank += 1550f;
                }
            }
            //baseGameManager.StartCoroutine(WaitForRun(baseGameManager));
            baseGameManager.StartCoroutine(ComboMechanism(baseGameManager));
            if (Other.ShouldBTPlayMus(baseGameManager))
            {
                baseGameManager.StartCoroutine(StopAllMus(baseGameManager));
            }
            Start(baseGameManager);
        }
        public static void Reset()
        {
            lap = 0;
            pizzaTimer = 0f;
            pizzaTimerTotal = 0f;
            itsBaldiTime = false;
            toppins.Clear();
            toppinsTotal = 0;
            combo = 0f;
            comboTimer = 10f;
            comboKeep = true;
            enteringLap = false;
            secretRoomTotal = 0;
            secretRoomFound.Clear();
        }
        public static void Start(BaseGameManager baseGameManager)
        {
            Singleton<MusicManager>.Instance.StopMidi();
            baseGameManager.BeginSpoopMode();
            if (Other.ShouldBTSpawnNpcs(baseGameManager) && Other.ShouldBTDespawnHB(baseGameManager))
            {
                baseGameManager.Ec.SpawnNPCs();
            }
            baseGameManager.Ec.StartEventTimers();

            if (Singleton<CoreGameManager>.Instance.currentMode == Mode.Main)
            {
                RoomController Office = null;
                foreach (RoomController room in baseGameManager.Ec.rooms)
                {
                    RoomCategory category = room.category;
                    if (category == RoomCategory.Class || category == RoomCategory.Faculty || category == RoomCategory.Office)
                    {
                        Office = room;
                        break;
                    }
                }
                if (Office != null && baseGameManager.Ec.GetBaldi() != null)
                {
                    baseGameManager.Ec.GetBaldi().transform.position = Office.RandomEntitySafeCellNoGarbage().TileTransform.position;
                }
            }
            else if (Singleton<CoreGameManager>.Instance.currentMode == Mode.Free && baseGameManager.Ec.GetBaldi() != null)
            {
                baseGameManager.Ec.GetBaldi().Despawn();
            }

            baseGameManager.StartCoroutine(WaitForLastNotebook(baseGameManager));
        }
        public static IEnumerator WaitForLastNotebook(BaseGameManager baseGameManager)
        {
            int notebookTotal = baseGameManager.Ec.notebookTotal;
            while (baseGameManager.FoundNotebooks < notebookTotal - 1)
            {
                if (BasePlugin.AssetMan.Get<Sprite>("Notebook_John") == null)
                {
                    yield break;
                }
                yield return null;
            }
            if (baseGameManager.FoundNotebooks == notebookTotal - 1)
            {
                foreach (Notebook notebook in baseGameManager.Ec.notebooks)
                {
                    if (!notebook.hidden)
                    {
                        BaseGameManagerPatches.musPlayer.StartCoroutine(BaseGameManagerPatches.musPlayer.Meatophobia(notebook, baseGameManager));
                        break;
                    }
                }
            }
            yield break;
        }
        public static void Lapping(BaseGameManager baseGameManager, SoundObject soundObject = null)
        {
            /*for (int i = 1; i <= 4; i++)
            {
                if (Other.CLC("skid.librarylib") && lap == i)
                {
                    LibrarylibAchievementsSupport.RewardAchievement("Lap" + i.ToString());
                }
            }*/

            lap++;
            PowerLeverController[] powerLeverControllers = Object.FindObjectsOfType<PowerLeverController>();
            List<RoomController> poweredRooms = new List<RoomController>();
            if (powerLeverControllers.Length > 0)
            {
                foreach (PowerLeverController powerLeverController in powerLeverControllers)
                {
                    poweredRooms.Add(powerLeverController.PoweredRoom);
                }
            }
            (baseGameManager.Ec.ElevatorManager.ReflectionGetVariable("brokenElevators") as List<Elevator>).Clear();
            baseGameManager.Ec.ElevatorManager.ReflectionSetVariable("foundOutOfOrderElevators", 0);
            List<RoomController> rooms = baseGameManager.Ec.rooms;
            foreach (RoomController room in rooms)
            {
                List<Pickup> items = room.pickups;
                foreach (Pickup item in items)
                {
                    if (item.item == BasePlugin.AssetMan.Get<ItemObject>("BaldiClock"))
                    {
                        item.free = true;
                        item.itemSprite.sprite = BasePlugin.AssetMan.Get<Sprite>("BaldiClockIcon_Large");
                    }
                }
            }
            if (lap == 1)
            {
                pizzaTimerTotal = 0f;
                itsBaldiTime = true;
                Activity closetActivity = null;
                List<float> dists = new List<float>();
                foreach (Activity activity1 in baseGameManager.Ec.activities)
                {
                    dists.Add(Vector3.Distance(activity1.transform.position, Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position));
                }
                for (int i = 0; i < dists.Count; i++)
                {
                    if (dists[i] == dists.Min())
                    {
                        closetActivity = baseGameManager.Ec.activities[i];
                        break;
                    }
                }
                foreach (Activity activity in baseGameManager.Ec.activities)
                {
                    if (activity != null && activity.room != null && activity != closetActivity)
                    {
                        //Mimicry Why You Didn't Write The MusicMachine's Code InstantReset() From RMSInBBP ?!?!?!?!
                        if (!activity.GetType().ToString().Contains("MusicMachine"))
                        {
                            activity.InstantReset();
                            baseGameManager.AddNotebookTotal(1);
                            if (Singleton<CoreGameManager>.Instance.timeLimitChallenge)
                            {
                                pizzaTimerTotal = 60f;
                            }
                            else
                            {
                                if (activity.GetType() == typeof(NoActivity))
                                {
                                    pizzaTimerTotal += 35f;
                                }
                                else
                                {
                                    pizzaTimerTotal += 45f;
                                }
                            }
                            if (!poweredRooms.Contains(activity.room))
                            {
                                activity.room.SetPower(true);
                            }
                        }
                    }
                }
                pizzaTimer = pizzaTimerTotal;
            }
            else
            {
                foreach (NPC npc in baseGameManager.Ec.Npcs)
                {
                    if (npc.TryGetComponent(out BaldiFace face))
                    {
                        face.behaviorStateMachine.ChangeState(new BaldiFace_Wait(face, false));
                    }
                }
                if (Singleton<CoreGameManager>.Instance.inventoryChallenge)
                {
                    Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.ResetMaxItem();
                }
                foreach (Activity activity in baseGameManager.Ec.activities)
                {
                    if (activity != null && activity.room != null)
                    {
                        //Mimicry Why You Didn't Write The MusicMachine's Code InstantReset() From RMSInBBP ?!?!?!?!
                        if (!activity.GetType().ToString().Contains("MusicMachine"))
                        {
                            activity.InstantReset();
                            baseGameManager.AddNotebookTotal(1);
                            if (!poweredRooms.Contains(activity.room))
                            {
                                activity.room.SetPower(true);
                            }
                        }
                    }
                }
            }
            baseGameManager.CollectNotebooks(0);
            Singleton<MusicManager>.Instance.StopMidi();
            Singleton<CoreGameManager>.Instance.musicMan.FlushQueue(true);

            if (lap == 1)
            {
                BaseGameManagerPatches.musPlayer.Stop();
                baseGameManager.StartCoroutine(WaitUntilTimeGoesTo60Seconds(baseGameManager));
                baseGameManager.StartCoroutine(BaldiTimeCostPoints(baseGameManager));
                if (Other.ShouldBTPlayMus(baseGameManager))
                {
                    baseGameManager.StartCoroutine(StopAllMus(baseGameManager));
                }
                if (Singleton<CoreGameManager>.Instance.GetHud(0).transform.TryFind("BaldiTimeHudManager", out Transform transform))
                {
                    BaldiTimeHudManager manager = transform.gameObject.GetComponent<BaldiTimeHudManager>();
                    manager.StartCoroutine(manager.TimerBarRaise());
                }
            }
            if (Other.ShouldBTPlayMus(baseGameManager))
            {
                if (lap == 1)
                {
                    if (BasePlugin.AssetMan.Get<AudioClip>("Lap1-Intro") != null)
                    {
                        if (BasePlugin.AssetMan.Get<AudioClip>("Lap1-Loop") == null)
                        {
                            BaseGameManagerPatches.musPlayer.Play(BasePlugin.AssetMan.Get<AudioClip>("Lap1-Intro"));
                        }
                        else
                        {
                            BaseGameManagerPatches.musPlayer.Queue(BasePlugin.AssetMan.Get<AudioClip>("Lap1-Intro"), BasePlugin.AssetMan.Get<AudioClip>("Lap1-Loop"));
                        }
                    }
                    else if (BasePlugin.AssetMan.Get<AudioClip>("Lap1-Loop") != null)
                    {
                        BaseGameManagerPatches.musPlayer.Play(BasePlugin.AssetMan.Get<AudioClip>("Lap1-Loop"), true);
                    }
                    if (BasePlugin.AssetMan.Get<SoundObject>("JOHN_PILLAR_IMPACT") != null)
                    {
                        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("JOHN_PILLAR_IMPACT"));
                    }
                }
                else
                {
                    AudioClip intro = BasePlugin.AssetMan.Get<AudioClip>("Lap" + lap.ToString() + "-Intro");
                    AudioClip loop = BasePlugin.AssetMan.Get<AudioClip>("Lap" + lap.ToString() + "-Loop");
                    if (intro != null)
                    {
                        if (intro == null)
                        {
                            BaseGameManagerPatches.musPlayer.Play(intro, false, true);
                        }
                        else
                        {
                            BaseGameManagerPatches.musPlayer.Queue(intro, loop, true);
                        }
                    }
                    else
                    {
                        BaseGameManagerPatches.musPlayer.Play(loop, true, true);
                    }
                }
            }
            if (lap > 1 && soundObject != null)
            {
                Singleton<CoreGameManager>.Instance.audMan.PlaySingle(soundObject);
                baseGameManager.Ec.ElevatorManager.SetAllElevators(ElevatorState.OutOfOrder);
            }
            if (lap == 3)
            {
                pizzaTimer = 0f;
            }
            if (lap == 4)
            {
                IntVector2 pos = baseGameManager.Ec.GetBaldi() == null ? baseGameManager.Ec.RandomCell(false, false, true).position : baseGameManager.Ec.CellFromPosition(baseGameManager.Ec.GetBaldi().transform.position).position;
                baseGameManager.Ec.SpawnNPC(BasePlugin.AssetMan.Get<FileName2>("FileName2"), pos);
                Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("Null_PreBoss_Start"));
                AddWarTimer(30f, false);
            }

            Singleton<CoreGameManager>.Instance.GetHud(0).StartCoroutine(BaldiTimeUI.LappingAnimations(baseGameManager));

            PlayerMovementStatModifier statModifier = Singleton<CoreGameManager>.Instance.GetPlayer(0).GetMovementStatModifier();
            statModifier.AddModifier("walkSpeed", new MTM101BaldAPI.Components.ValueModifier(1f, 2f));
            statModifier.AddModifier("runSpeed", new MTM101BaldAPI.Components.ValueModifier(1f, 2.5f));
        }

        public static IEnumerator BaldiTimeCostPoints(BaseGameManager baseGameManager)
        {
            float timer = 0f;
            while (baseGameManager != null && itsBaldiTime)
            {
                if (baseGameManager.Ec.notebookTotal <= 0)
                {
                    yield break;
                }
                if (timer < 1f)
                {
                    timer += Time.deltaTime * baseGameManager.Ec.EnvironmentTimeScale;
                }
                else
                {
                    timer = 0f;
                    points -= 5;
                    if (points < 0f)
                    {
                        points = 0f;
                    }
                }
                yield return null;
            }
            yield break;
        }

        public static IEnumerator WaitUntilTimeGoesTo60Seconds(BaseGameManager baseGameManager)
        {
            while (pizzaTimer >= 60f)
            {
                if (baseGameManager == null)
                {
                    yield break;
                }
                if (lap > 1)
                {
                    break;
                }
                yield return null;
            }
            if (!BasePlugin.IsNullscapeinBBInstalled)
            {
                if (lap == 1)
                {
                    if (BasePlugin.AssetMan.Get<AudioClip>("Lap1-Outro") != null)
                    {
                        BaseGameManagerPatches.musPlayer.Play(BasePlugin.AssetMan.Get<AudioClip>("Lap1-Outro"), false, true);
                    }
                }
            }
            while (pizzaTimer > 0f)
            {
                if (baseGameManager == null)
                {
                    yield break;
                }
                yield return null;
            }

            /*Singleton<CoreGameManager>.Instance.GetHud(0).BaldiTv.AnnounceEvent(AssetFinder.FindOfTypeWithName<SoundObject>("BAL_TimeOut", true));
            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(AssetFinder.FindOfTypeWithName<SoundObject>("TimeLimitBell", true));
            yield return new WaitForSeconds(3f);
            baseGameManager.Ec.CloseSchool();
            Object.FindObjectOfType<TimeOut>().Begin();*/

            if (Singleton<CoreGameManager>.Instance.GetHud(0).transform.TryFind("BaldiTimeHudManager", out Transform transform))
            {
                BaldiTimeHudManager manager = transform.gameObject.GetComponent<BaldiTimeHudManager>();
                manager.StartCoroutine(manager.TimerBarFall());
            }

            foreach (NPC npc in baseGameManager.Ec.Npcs)
            {
                if (npc.TryGetComponent<BaldiFace>(out _))
                {
                    yield break;
                }
            }
            baseGameManager.Ec.SpawnNPC(BasePlugin.AssetMan.Get<BaldiFace>("BaldiFace"), baseGameManager.Ec.CellFromPosition(Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position).position);

            yield break;
        }

        public static void Update()
        {
            if (Singleton<BaseGameManager>.Instance == null)
            {
                return;
            }
            if (Singleton<CoreGameManager>.Instance == null)
            {
                return;
            }
            if (Singleton<CoreGameManager>.Instance.GetPlayer(0) == null)
            {
                return;
            }
            if (itsBaldiTime)
            {
                if (pizzaTimer > 0f)
                {
                    pizzaTimer -= Time.deltaTime * Time.timeScale * Singleton<BaseGameManager>.Instance.Ec.EnvironmentTimeScale;
                }
                else if (pizzaTimer < 0f)
                {
                    pizzaTimer = 0f;
                }
            }
            if (Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.stamina != staminaOld)
            {
                if (Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.stamina - staminaOld > 5f)
                {
                    AddCombo(0f, (Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.stamina - staminaOld) / (Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.StaminaMax / comboTimerMax));
                }
                staminaOld = Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.stamina;
            }
        }

        public static void AddWarTimer(float time = 30f, bool sound = true)
        {
            warTimer += time;
            if (!war)
            {
                Singleton<BaseGameManager>.Instance.StartCoroutine(WarStuffs(Singleton<BaseGameManager>.Instance));
                if (Singleton<CoreGameManager>.Instance.GetHud(0).transform.TryFind("BaldiTimeHudManager", out Transform transform))
                {
                    BaldiTimeHudManager manager = transform.gameObject.GetComponent<BaldiTimeHudManager>();
                    manager.StartCoroutine(manager.WarTimerRaise());
                }
            }
            war = true;
            if (sound)
            {
                Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("WarTimerUp"));
            }
        }

        public static IEnumerator WarStuffs(BaseGameManager baseGameManager)
        {
            float timer = 0f;
            while (baseGameManager != null && warTimer > 0f)
            {
                timer += Time.deltaTime * baseGameManager.Ec.EnvironmentTimeScale;
                if (timer > 8f)
                {
                    timer = timer % 8f;
                }
                float edit;
                edit = timer / 4f;
                if (timer > 4f)
                {
                    edit = (8f - timer) / 4f;
                }
                Color color = new Color(1f, edit, edit, 1f);
                foreach (Cell cell in baseGameManager.Ec.lights)
                {
                    cell.lightColor = color;
                    baseGameManager.Ec.UpdateLightingAtCell(cell);
                }
                Shader.SetGlobalColor("_SkyboxColor", color);

                warTimer -= Time.deltaTime * baseGameManager.Ec.EnvironmentTimeScale;

                yield return null;
            }
            Transform player = Singleton<CoreGameManager>.Instance.GetPlayer(0).transform;
            if (baseGameManager.Ec.GetBaldi() != null)
            {
                baseGameManager.EndGame(player, baseGameManager.Ec.GetBaldi());
            }
            else if (baseGameManager.Ec.TryGetCharacter(out BaldiFace baldiFace))
            {
                Other.NpcKillPlayer(baldiFace);
            }
            else if (baseGameManager.Ec.TryGetCharacter(out FileName2 fileName2))
            {
                Other.NpcKillPlayer(fileName2);
            }
            else
            {
                Other.NpcKillPlayer(baseGameManager.Ec.Npcs[0]);
            }
            yield break;
        }
    }
}
