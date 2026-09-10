using HarmonyLib;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ItsBaldiTimeRework
{
    public class BaldiTimeActions
    {
        public static SoundObject JOHN_PILLAR_IMPACT = null;
        public static int lap = 0;
        public static float pizzaTimer = 0f;
        public static float pizzaTimerTotal = 0f;
        public static bool itsBaldiTime = false;
        public static List<string> AllMus = new List<string>();
        public static List<string> F1Mus = new List<string>();
        public static List<string> F2Mus = new List<string>();
        public static List<string> F3Mus = new List<string>();
        public static List<string> F4Mus = new List<string>();
        public static List<string> F5Mus = new List<string>();
        public static List<NPC> toppins = new List<NPC>();
        public static Sprite Notebook_John = null;
        public static float points = 0f;
        public static float comboPoints = 0f;
        public static float pointsForPRank = 0f;
        public static string[] ranks = new string[8] { "D", "C", "B", "A", "S", "P", "L", "X" };
        public static string rank = "D";
        public static float combo = 0f;
        public static float comboTimer = 0f;
        public static float comboTimerMax = 15f;
        public static bool comboKeep = true;
        public static bool enteringLap = false;
        public static int notebookmax = 4;

        public static void AddCombo(float comboAdd, float time = 1225f)
        {
            combo += comboAdd;
            comboPoints += 100f;
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
                        comboTimer -= Time.deltaTime * baseGameManager.Ec.EnvironmentTimeScale;
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
            notebookmax = baseGameManager.Ec.notebookTotal;
            points = 0f;
            comboPoints = 0f;
            pointsForPRank = 0f;
            rank = "D";
            List<RandomEvent> events = baseGameManager.Ec.ReflectionGetVariable("events") as List<RandomEvent>;
            /*RandomEvent a = null;
            RandomEvent b = null;
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].Type == RandomEventType.TimeOut)
                {
                    a = events[i];
                }
                if (events[i].Type == RandomEventType.Lockdown)
                {
                    b = events[i];
                }
            }
            if (a != null)
            {
                events.Remove(a);
            }
            if (b != null)
            {
                events.Remove(b);
            }*/
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
                pointsForPRank += 1225f;
            }
            int mun = Random.Range(0, SpawnableRooms.Count);
            baseGameManager.Ec.SpawnNPC(BasePlugin.AssetMan.Get<LapPortal>("LapPortal"), SpawnableRooms[mun].RandomEntitySafeCellNoGarbage().position);
            pointsForPRank += 3000f;
            /*int spawnToppins = 5;
            bool lapPortalSpawned = false;
            if (baseGameManager.Ec.rooms.Count > 0)
            {
                for (int i = 0; i < baseGameManager.Ec.rooms.Count; i++)
                {
                    if (spawnToppins <= 0)
                    {
                        break;
                    }
                    RoomController room = baseGameManager.Ec.rooms[i];
                    RoomCategory category = room.category;
                    if (category == RoomCategory.Class || category == RoomCategory.Faculty || category == RoomCategory.Office)
                    {
                        int num = Random.Range(0, baseGameManager.Ec.rooms.Count);
                        if (num < 5 || i >= baseGameManager.Ec.rooms.Count - spawnToppins)
                        {
                            baseGameManager.Ec.SpawnNPC(BasePlugin.AssetMan.Get<Toppin>("Toppin"), room.RandomEntitySafeCellNoGarbage().position);
                            pointsForPRank += 1225f;
                            spawnToppins -= 1;
                        }
                        if ((num < 7 || i >= baseGameManager.Ec.rooms.Count - 1) && !lapPortalSpawned)
                        {
                            baseGameManager.Ec.SpawnNPC(BasePlugin.AssetMan.Get<LapPortal>("LapPortal"), room.RandomEntitySafeCellNoGarbage().position);
                            lapPortalSpawned = true;
                        }
                    }
                }
            }
            if (spawnToppins > 0 && baseGameManager.Ec.cells.Length > 0)
            {
                while (spawnToppins > 0)
                {
                    baseGameManager.Ec.SpawnNPC(BasePlugin.AssetMan.Get<Toppin>("Toppin"), baseGameManager.Ec.RandomCell(false, false, true).position);
                    pointsForPRank += 1225f;
                    spawnToppins -= 1;
                }
            }
            if (!lapPortalSpawned && baseGameManager.Ec.cells.Length > 0)
            {
                baseGameManager.Ec.SpawnNPC(BasePlugin.AssetMan.Get<LapPortal>("LapPortal"), baseGameManager.Ec.RandomCell(false, false, true).position);
                lapPortalSpawned = true;
            }*/
            foreach (Activity activity in baseGameManager.Ec.activities)
            {
                if (activity.GetType() == typeof(NoActivity))
                {
                    pointsForPRank += 1000f;
                }
                else
                {
                    pointsForPRank += 1500f;
                }
            }
            pointsForPRank += 3000f;
            //baseGameManager.StartCoroutine(WaitForRun(baseGameManager));
            baseGameManager.StartCoroutine(ComboMechanism(baseGameManager));
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
            List<string> chaseMusics = new List<string>();
            if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F1")
            {
                chaseMusics.AddRange(F1Mus);
                chaseMusics.AddRange(AllMus);
            }
            else if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F2")
            {
                chaseMusics.AddRange(F2Mus);
                chaseMusics.AddRange(AllMus);
            }
            else if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F3")
            {
                chaseMusics.AddRange(F3Mus);
                chaseMusics.AddRange(AllMus);
            }
            else if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F4")
            {
                chaseMusics.AddRange(F4Mus);
                chaseMusics.AddRange(AllMus);
            }
            else if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F5")
            {
                chaseMusics.AddRange(F5Mus);
                chaseMusics.AddRange(AllMus);
            }
            if (chaseMusics.Count > 0)
            {
                BaseGameManagerPatches.musPlayer.Stop();
                BaseGameManagerPatches.musPlayer.Play(chaseMusics[Random.Range(0, chaseMusics.Count - 1)], true);
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
                RoomController Office = null ;
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
            List<string> chaseMusics = new List<string>();
            if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F1")
            {
                chaseMusics.AddRange(F1Mus);
                chaseMusics.AddRange(AllMus);
            }
            else if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F2")
            {
                chaseMusics.AddRange(F2Mus);
                chaseMusics.AddRange(AllMus);
            }
            else if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F3")
            {
                chaseMusics.AddRange(F3Mus);
                chaseMusics.AddRange(AllMus);
            }
            else if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F4")
            {
                chaseMusics.AddRange(F4Mus);
                chaseMusics.AddRange(AllMus);
            }
            else if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F5")
            {
                chaseMusics.AddRange(F5Mus);
                chaseMusics.AddRange(AllMus);
            }
            if (chaseMusics.Count > 0)
            {
                BaseGameManagerPatches.musPlayer.Stop();
                BaseGameManagerPatches.musPlayer.Play(chaseMusics[Random.Range(0, chaseMusics.Count - 1)], true);
            }
            baseGameManager.StartCoroutine(WaitForLastNotebook(baseGameManager));
        }
        public static IEnumerator WaitForLastNotebook(BaseGameManager baseGameManager)
        {
            int notebookTotal = baseGameManager.Ec.notebookTotal;
            while (baseGameManager.FoundNotebooks < notebookTotal - 1)
            {
                if (Notebook_John == null)
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
            if (lap == 1)
            {
                if (BasePlugin.AssetMan.Get<AudioClip>("Lap1-Intro") != null)
                {
                    if (BasePlugin.AssetMan.Get<AudioClip>("Lap1-Loop") == null)
                    {
                        BaseGameManagerPatches.musPlayer.Play("Lap1-Intro", true);
                    }
                    else
                    {
                        BaseGameManagerPatches.musPlayer.Queue("Lap1-Intro", "Lap1-Loop");
                    }
                }
                else if (BasePlugin.AssetMan.Get<AudioClip>("Lap1-Loop") != null)
                {
                    BaseGameManagerPatches.musPlayer.Play("Lap1-Loop", true);
                }
                if (JOHN_PILLAR_IMPACT != null)
                {
                    Singleton<CoreGameManager>.Instance.audMan.PlaySingle(JOHN_PILLAR_IMPACT);
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
                        BaseGameManagerPatches.musPlayer.Play("Lap2-Intro", false, true);
                    }
                    else
                    {
                        BaseGameManagerPatches.musPlayer.Queue("Lap2-Intro", "Lap2-Loop", true);
                    }
                }
                else if (BasePlugin.AssetMan.Get<AudioClip>("Lap2-Loop") != null)
                {
                    BaseGameManagerPatches.musPlayer.Play("Lap2-Loop", true, true);
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
            if (BasePlugin.AssetMan.Get<AudioClip>("Lap1-Outro") != null && lap == 1)
            {
                BaseGameManagerPatches.musPlayer.Play("Lap1-Outro", false, true);
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
        }
    }
}
