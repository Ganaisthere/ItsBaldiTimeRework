using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Reflection;
using System.Collections;
using System.Collections.Generic;
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
        public static List<NPC> toppins = new List<NPC>();
        public static float points = 0f;
        public static float comboPoints = 0f;
        public static float pointsForPRank = 0f;
        public static List<string> ranks = new List<string> { "D", "C", "B", "A", "S", "P", "L", "X" };
        public static string rank = "D";
        public static float combo = 0f;
        public static float comboTimer = 0f;
        public static float comboTimerMax = 20f;
        public static bool comboKeep = true;
        public static bool enteringLap = false;
        public static int notebookmax = 4;
        public static float staminaOld = 100f;

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
        public static IEnumerator StopMidi(BaseGameManager baseGameManager)
        {
            for (int i = 0; i < 10; i++)
            {
                if (baseGameManager == null)
                {
                    yield break;
                }
                Singleton<MusicManager>.Instance.StopMidi();
                yield return null;
            }
            yield break;
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
                        if (ElevatorScreenPatches.death)
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
                            Singleton<CoreGameManager>.Instance.GetHud(0).StartCoroutine(BaldiTimeUI.ShowComboLevels(baseGameManager));
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
            staminaOld = Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.StaminaMax;
            notebookmax = baseGameManager.Ec.notebookTotal;
            points = 0f;
            comboPoints = 0f;
            pointsForPRank = 0f;
            rank = "D";
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
                if (num == 0)
                {
                    baseGameManager.Ec.RespawnItemInRoom(BasePlugin.AssetMan.Get<ItemObject>("BaldiClock"), room);
                    pointsForPRank += 25f;
                    num = Random.Range(0, 2);
                    if (num == 0)
                    {
                        baseGameManager.Ec.RespawnItemInRoom(BasePlugin.AssetMan.Get<ItemObject>("BaldiClock"), room);
                        pointsForPRank += 25f;
                    }
                    List<Pickup> items = room.pickups;
                    foreach (Pickup item in items)
                    {
                        if (item.item == BasePlugin.AssetMan.Get<ItemObject>("BaldiClock"))
                        {
                            item.free = false;
                            item.price = 12251225;
                            item.itemSprite.sprite = BasePlugin.AssetMan.Get<Sprite>("BaldiClockIcon_Large_Transparent");
                        }
                    }
                }
            }
            List<RoomController> SpawnableRooms = new List<RoomController>();
            foreach (RoomController room in rooms)
            {
                RoomCategory category = room.category;
                if (category == RoomCategory.Class || category == RoomCategory.Faculty || category == RoomCategory.Office)
                {
                    SpawnableRooms.Add(room);
                }
            }
            for (int i = 0; i < 5; i++)
            {
                int num = Random.Range(0, SpawnableRooms.Count);
                baseGameManager.Ec.SpawnNPC(BasePlugin.AssetMan.Get<Toppin>("Toppin"), SpawnableRooms[num].RandomEntitySafeCellNoGarbage().position);
                pointsForPRank += 1000f;
            }
            int mun = Random.Range(0, SpawnableRooms.Count);
            baseGameManager.Ec.SpawnNPC(BasePlugin.AssetMan.Get<LapPortal>("LapPortal"), SpawnableRooms[mun].RandomEntitySafeCellNoGarbage().position);
            pointsForPRank += 3000f;
            foreach (Activity activity in baseGameManager.Ec.activities)
            {
                if (activity.GetType() == typeof(NoActivity))
                {
                    pointsForPRank += 100f;
                }
                else
                {
                    pointsForPRank += 1500f;
                }
            }
            pointsForPRank += 3000f;
            //baseGameManager.StartCoroutine(WaitForRun(baseGameManager));
            baseGameManager.StartCoroutine(ComboMechanism(baseGameManager));
            baseGameManager.StartCoroutine(StopMidi(baseGameManager));
            Start(baseGameManager);
        }
        public static void Reset()
        {
            lap = 0;
            pizzaTimer = 0f;
            itsBaldiTime = false;
            toppins.Clear();
            combo = 0f;
            comboTimer = 10f;
            BaldiTimeUI.pointsEdit = 0f;
            BaldiTimeUI.ComboBarTimer = 0f;
            BaldiTimeUI.ComboTimerEdit = 0f;
            comboKeep = true;
            enteringLap = false;
            BaldiTimeUI.numOld = 0;
            BaldiTimeUI.rankAniTimer = 0f;
        }
        public static IEnumerator WaitForRun(BaseGameManager baseGameManager)
        {
            while (MainGameManagerPatches.happyBaldi == null)
            {
                if (baseGameManager == null)
                {
                    yield break;
                }
                yield return null;
            }
            float dist = 0f;
            float distMax = 60f;
            PlayerManager playerManager = Singleton<CoreGameManager>.Instance.GetPlayer(0);
            while (dist < distMax)
            {
                if (playerManager == null || MainGameManagerPatches.happyBaldi == null || baseGameManager == null)
                {
                    yield break;
                }
                distMax = 60f + Singleton<StickerManager>.Instance.StickerValue(Sticker.BaldiCountdown) * 30f;
                dist = Vector3.Distance(playerManager.transform.position, MainGameManagerPatches.happyBaldi.transform.position);
                yield return null;
            }
            Singleton<MusicManager>.Instance.StopMidi();
            baseGameManager.BeginSpoopMode();
            baseGameManager.Ec.SpawnNPCs();
            baseGameManager.Ec.StartEventTimers();
            if (Singleton<CoreGameManager>.Instance.currentMode == Mode.Main)
            {
                baseGameManager.Ec.GetBaldi().transform.position = MainGameManagerPatches.happyBaldi.transform.position;
                Singleton<BaseGameManager>.Instance.Ec.GetBaldi().AudMan.PlaySingle(AssetFinder.FindOfTypeWithName<SoundObject>("BAL_ReadyOrNot", true));
            }
            else if (Singleton<CoreGameManager>.Instance.currentMode == Mode.Free)
            {
                baseGameManager.Ec.GetBaldi().Despawn();
            }

            if (!BasePlugin.IsNullscapeinBBInstalled)
            {
                List<AudioClip> chaseMusics = new List<AudioClip>();
                if (AllSpoopMusics.Count > 0)
                {
                    foreach (AudioClip music in AllSpoopMusics)
                    {
                        string levelTitle = Singleton<CoreGameManager>.Instance.sceneObject.levelTitle;
                        chaseMusics.Add(music);

                        /*bool StartWithF = levelTitle.StartsWith("F");
                        if (music.name.Contains("_" + Singleton<CoreGameManager>.Instance.sceneObject.levelTitle) || !StartWithF)
                        {
                            chaseMusics.Add(music);
                        }
                        if (!music.name.Contains("_F1") && !music.name.Contains("_F2") && !music.name.Contains("_F3") && !music.name.Contains("_F4") && !music.name.Contains("_F5"))
                        {
                            chaseMusics.Add(music);
                        }*/
                    }
                }
                if (chaseMusics.Count > 0)
                {
                    AudioClip choosedMusic = chaseMusics[Random.Range(0, chaseMusics.Count - 1)];
                    BaseGameManagerPatches.musPlayer.Stop();
                    BaseGameManagerPatches.musPlayer.Play(choosedMusic, true);
                }
            }

            Object.Destroy(MainGameManagerPatches.happyBaldi.gameObject);
            MainGameManagerPatches.happyBaldi = null;
            Singleton<CoreGameManager>.Instance.GetHud(0).StartCoroutine(BaldiTimeUI.Flash(baseGameManager));
            baseGameManager.StartCoroutine(WaitForLastNotebook(baseGameManager));
            yield break;
        }
        public static void Start(BaseGameManager baseGameManager)
        {
            Singleton<MusicManager>.Instance.StopMidi();
            baseGameManager.BeginSpoopMode();
            baseGameManager.Ec.SpawnNPCs();
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
                if (Office != null)
                {
                    baseGameManager.Ec.GetBaldi().transform.position = Office.RandomEntitySafeCellNoGarbage().TileTransform.position;
                }
            }
            else if (Singleton<CoreGameManager>.Instance.currentMode == Mode.Free)
            {
                baseGameManager.Ec.GetBaldi().Despawn();
            }

            if (!BasePlugin.IsNullscapeinBBInstalled)
            {
                List<AudioClip> chaseMusics = new List<AudioClip>();
                if (AllSpoopMusics.Count > 0)
                {
                    foreach (AudioClip music in AllSpoopMusics)
                    {
                        string levelTitle = Singleton<CoreGameManager>.Instance.sceneObject.levelTitle;
                        chaseMusics.Add(music);

                        /*bool StartWithF = levelTitle.StartsWith("F");
                        if (music.name.Contains("_" + Singleton<CoreGameManager>.Instance.sceneObject.levelTitle) || !StartWithF)
                        {
                            chaseMusics.Add(music);
                        }
                        if (!music.name.Contains("_F1") && !music.name.Contains("_F2") && !music.name.Contains("_F3") && !music.name.Contains("_F4") && !music.name.Contains("_F5"))
                        {
                            chaseMusics.Add(music);
                        }*/
                    }
                }
                if (chaseMusics.Count > 0)
                {
                    AudioClip choosedMusic = chaseMusics[Random.Range(0, chaseMusics.Count - 1)];
                    BaseGameManagerPatches.musPlayer.Stop();
                    BaseGameManagerPatches.musPlayer.Play(choosedMusic, true);
                }
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
                        BaseGameManagerPatches.musPlayer.StartCoroutine(BaseGameManagerPatches.musPlayer.Meatophobia(notebook));
                        break;
                    }
                }
            }
            yield break;
        }
        public static void Lapping(BaseGameManager baseGameManager, SoundObject soundObject = null)
        {
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
                Activity lastActivity = baseGameManager.ReflectionGetVariable("lastActivity") as Activity;
                foreach (Activity activity in baseGameManager.Ec.activities)
                {
                    if (activity != lastActivity)
                    {
                        activity.InstantReset();
                    }
                    if (!Singleton<CoreGameManager>.Instance.timeLimitChallenge)
                    {
                        if (activity.GetType() == typeof(NoActivity))
                        {
                            pizzaTimerTotal += 30f;
                        }
                        else
                        {
                            pizzaTimerTotal += 40f;
                        }
                    }
                    if (!poweredRooms.Contains(activity.room))
                    {
                        activity.room.SetPower(true);
                    }
                }
                if (Singleton<CoreGameManager>.Instance.timeLimitChallenge)
                {
                    pizzaTimerTotal = 60f;
                }
                baseGameManager.AddNotebookTotal(notebookmax - 1);
                pizzaTimer = pizzaTimerTotal;
            }
            else
            {
                if (Singleton<CoreGameManager>.Instance.inventoryChallenge)
                {
                    Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.ResetMaxItem();
                }
                foreach (Activity activity in baseGameManager.Ec.activities)
                {
                    activity.InstantReset();
                    if (!poweredRooms.Contains(activity.room))
                    {
                        activity.room.SetPower(true);
                        activity.ReflectionSetVariable("completed", false);
                    }
                }
                baseGameManager.AddNotebookTotal(notebookmax);
            }
            baseGameManager.CollectNotebooks(0);
            Singleton<MusicManager>.Instance.StopMidi();
            Singleton<CoreGameManager>.Instance.musicMan.FlushQueue(true);
            if (!BasePlugin.IsNullscapeinBBInstalled)
            {
                if (lap == 1)
                {
                    BaseGameManagerPatches.musPlayer.Stop();
                    if (BasePlugin.AssetMan.Get<AudioClip>("Lap1-Intro") != null)
                    {
                        if (BasePlugin.AssetMan.Get<AudioClip>("Lap1-Loop") == null)
                        {
                            BaseGameManagerPatches.musPlayer.Play(BasePlugin.AssetMan.Get<AudioClip>("Lap1-Intro"), true);
                        }
                        else
                        {
                            BaseGameManagerPatches.musPlayer.Queue(BasePlugin.AssetMan.Get<AudioClip>("Lap1-Intro"), BasePlugin.AssetMan.Get<AudioClip>("Lap1-Loop"));
                        }
                    }
                    else if (BasePlugin.AssetMan.Get<AudioClip>("Lap1-Loop") != null)
                    {
                        BaseGameManagerPatches.musPlayer.Play(BasePlugin.AssetMan.Get<AudioClip>("Lap1-Loop"));
                    }
                    if (BasePlugin.AssetMan.Get<SoundObject>("JOHN_PILLAR_IMPACT") != null)
                    {
                        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("JOHN_PILLAR_IMPACT"));
                    }
                    baseGameManager.StartCoroutine(WaitUntilTimeGoesTo60Seconds(baseGameManager));
                    baseGameManager.StartCoroutine(BaldiTimeCostPoints(baseGameManager));
                }
                else if (lap == 2)
                {
                    if (BasePlugin.AssetMan.Get<AudioClip>("Lap2-Intro") != null)
                    {
                        if (BasePlugin.AssetMan.Get<AudioClip>("Lap2-Loop") == null)
                        {
                            BaseGameManagerPatches.musPlayer.Play(BasePlugin.AssetMan.Get<AudioClip>("Lap2-Intro"), false, true);
                        }
                        else
                        {
                            BaseGameManagerPatches.musPlayer.Queue(BasePlugin.AssetMan.Get<AudioClip>("Lap2-Intro"), BasePlugin.AssetMan.Get<AudioClip>("Lap2-Loop"));
                        }
                    }
                    else
                    {
                        BaseGameManagerPatches.musPlayer.Play(BasePlugin.AssetMan.Get<AudioClip>("Lap2-Loop"), true, true);
                    }
                }
            }
            if (lap > 1 && soundObject != null)
            {
                Singleton<CoreGameManager>.Instance.audMan.PlaySingle(soundObject);
                baseGameManager.Ec.ElevatorManager.SetAllElevators(ElevatorState.OutOfOrder);
            }
            baseGameManager.StartCoroutine(BaldiTimeUI.LappingAnimations(baseGameManager));
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
                    timer += Time.deltaTime;
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
            while (pizzaTimer > 60f)
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
            if (lap == 1)
            {
                if (BasePlugin.AssetMan.Get<AudioClip>("Lap1-Outro") != null)
                {
                    BaseGameManagerPatches.musPlayer.Play(BasePlugin.AssetMan.Get<AudioClip>("Lap1-Outro"), false, true);
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
            Singleton<CoreGameManager>.Instance.GetHud(0).BaldiTv.AnnounceEvent(AssetFinder.FindOfTypeWithName<SoundObject>("BAL_TimeOut", true));
            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(AssetFinder.FindOfTypeWithName<SoundObject>("TimeLimitBell", true));
            Singleton<CoreGameManager>.Instance.GetHud(0).StartCoroutine(BaldiTimeUI.TimerBarGoDown(baseGameManager));
            yield return new WaitForSeconds(3f);
            baseGameManager.Ec.CloseSchool();
            Object.FindObjectOfType<TimeOut>().Begin();
            yield break;
        }

        public static void Update()
        {
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
    }
}
