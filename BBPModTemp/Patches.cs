using HarmonyLib;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Reflection;
using MTM101BaldAPI.Registers;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace ItsBaldiTimeRework
{
    [HarmonyPatch(typeof(MainGameManager))]
    public class MainGameManagerPatches
    {
        [HarmonyPatch("BeginPlay")]
        [HarmonyPostfix]
        public static void BeginPlayPostfix(MainGameManager __instance)
        {
            Singleton<MusicManager>.Instance.StopMidi();
        }

        [HarmonyPatch("CreateHappyBaldi")]
        [HarmonyPrefix]
        public static bool CreateHappyBaldiPrefix(MainGameManager __instance)
        {
            if (Other.ShouldBTDespawnHB(__instance))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch("AllNotebooks")]
        [HarmonyPrefix]
        public static bool AllNotebooksPrefix(MainGameManager __instance)
        {
            BaseGameManagerPatches.AllNotebooksPrefix(__instance);
            return false;
        }
    }

    [HarmonyPatch(typeof(BaseGameManager))]
    public class BaseGameManagerPatches
    {
        public static CustomAudioPlayer musPlayer;

        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        public static void InitializePrefix()
        {
            BaldiTimeActions.Reset();
        }

        [HarmonyPatch("FinishLevel")]
        [HarmonyPrefix]
        public static bool FinishLevelPrefix(BaseGameManager __instance)
        {
            if (Other.ShouldBT(__instance))
            {
                BaldiTimeActions.SpoopMusicsRandom = -1;
                BaldiTimeAnimations.TitleCardRandom = -1;
                __instance.Ec.PauseEnvironment(true);
                __instance.Ec.PauseEvents(true);
                __instance.StartCoroutine(BaldiTimeAnimations.Rank(Singleton<CoreGameManager>.Instance, __instance));
                return false;
            }
            return true;
        }

        [HarmonyPatch("CollectNotebooks")]
        [HarmonyPrefix]
        public static void CollectNotebooksPrefix(int count)
        {
            if (count > 0)
            {
                BaldiTimeActions.points += 475f * count;
                BaldiTimeActions.AddCombo(1f * count);
                if (BaldiTimeActions.war)
                {
                    BaldiTimeActions.AddWarTimer(15f);
                }
            }
        }

        [HarmonyPatch("ActivityCompleted")]
        [HarmonyPostfix]
        public static void ActivityCompletedPostfix(bool correct)
        {
            if (correct)
            {
                BaldiTimeActions.points += 225f;
                BaldiTimeActions.AddCombo(0f);
            }
            if (BaldiTimeActions.war)
            {
                if (correct)
                {
                    BaldiTimeActions.AddWarTimer(25f);
                }
                else
                {
                    BaldiTimeActions.AddWarTimer(15f);
                }
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix(BaseGameManager __instance)
        {
            if (Other.ShouldBT(__instance))
            {
                BaldiTimeActions.Update();
            }
        }

        [HarmonyPatch("FinishLevel")]
        [HarmonyPostfix]
        public static void FinishLevelPostfix()
        {
            ElevatorScreenPatches.death = false;
        }

        [HarmonyPatch("BeginPlay")]
        [HarmonyPostfix]
        public static void BeginPlayPostfix(BaseGameManager __instance)
        {
            if (Other.ShouldBT(__instance) && __instance.Ec.notebookTotal > 0)
            {
                if (__instance.gameObject.GetComponentInChildren<CustomAudioPlayer>() == null)
                {
                    GameObject cMPObj = new GameObject("CustomMusicPlayer");
                    cMPObj.transform.SetParent(__instance.transform);
                    musPlayer = cMPObj.AddComponent<CustomAudioPlayer>();
                    musPlayer.audioSource1 = cMPObj.AddComponent<AudioSource>();
                    musPlayer.audioSource2 = cMPObj.AddComponent<AudioSource>();
                }
                BaldiTimeActions.Setup(__instance);
            }
        }

        [HarmonyPatch("AllNotebooks")]
        [HarmonyPrefix]
        public static bool AllNotebooksPrefix(BaseGameManager __instance)
        {
            if (Other.ShouldBT(__instance) && __instance.Ec.notebookTotal > 0)
            {
                __instance.ReflectionSetVariable("allNotebooksFound", true);
                if (BaldiTimeActions.lap < 1)
                {
                    BaldiTimeActions.Lapping(__instance);
                }
                else
                {
                    Activity lastActivity = __instance.ReflectionGetVariable("lastActivity") as Activity;
                    foreach (Activity activity in __instance.Ec.activities)
                    {
                        if (activity != lastActivity && !activity.InBonusMode)
                        {
                            activity.Corrupt(false);
                            activity.SetBonusMode(true);
                        }
                    }
                    __instance.Ec.ElevatorManager.SetTotalOutOfOrderElevators(__instance.Ec.Elevators.Count - 1);
                    __instance.Ec.ElevatorManager.SetAllElevators(ElevatorState.OpenForExit);
                }
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(BaldiTV))]
    public class BaldiTVPatches
    {
        [HarmonyPatch("ShowLevelTimeWarning")]
        [HarmonyPrefix]
        public static bool ShowLevelTimeWarningPrefix()
        {
            return false;
        }

        [HarmonyPatch("LevelTimeWarning")]
        [HarmonyPrefix]
        public static bool LevelTimeWarningPrefix()
        {
            return false;
        }

        [HarmonyPatch("Speak")]
        [HarmonyPrefix]
        public static bool SpeaksPrefix(SoundObject sound)
        {
            if (sound.name.Contains("AllNotebook"))
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(HudManager))]
    public class HudManagerPatches
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePostfix(HudManager __instance)
        {
            if (Other.ShouldBT(Singleton<BaseGameManager>.Instance, false))
            {
                GameObject BTHudManager_Obj = new GameObject("BaldiTimeHudManager");
                BTHudManager_Obj.transform.SetParent(__instance.transform);
                BaldiTimeHudManager BTHudManager = BTHudManager_Obj.AddComponent<BaldiTimeHudManager>();
                BTHudManager.Initialize();

                //BaldiTimeUI.SetupGUI(__instance);
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix(HudManager __instance)
        {
            if (Singleton<BaseGameManager>.Instance == null)
            {
                return;
            }
            if (Other.ShouldBT(Singleton<BaseGameManager>.Instance))
            {
                //BaldiTimeUI.UpdateGUI(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(CoreGameManager))]
    public class CoreGameManagerPatches
    {
        [HarmonyPatch("EndGame")]
        [HarmonyPrefix]
        public static void EndGamePrefix(CoreGameManager __instance)
        {
            if (BaseGameManagerPatches.musPlayer != null)
            {
                BaseGameManagerPatches.musPlayer.Stop();
            }
        }
    }

    [HarmonyPatch(typeof(PointsAnimation))]
    public class PointsAnimationPatches
    {
        [HarmonyPatch("AddScore")]
        [HarmonyPrefix]
        public static void EndGamePrefix(int points)
        {
            if (Other.ShouldBT(Singleton<BaseGameManager>.Instance))
            {
                BaldiTimeActions.points += points;
                if (BaldiTimeActions.points < 0f)
                {
                    BaldiTimeActions.points = 0f;
                }
                BaldiTimeActions.AddCombo(0f, math.max(points / 10f, -5f));
            }
        }
    }

    [HarmonyPatch(typeof(ElevatorScreen))]
    public class ElevatorScreenPatches
    {
        public static bool death = false;

        [HarmonyPatch("StartGame")]
        [HarmonyPrefix]
        public static void StartGamePrefix()
        {
            BaldiTimeActions.itsBaldiTime = false;
        }

        [HarmonyPatch("StartGame")]
        [HarmonyPostfix]
        public static void StartGamePostfix()
        {
            BaseGameManager baseGameManager = Singleton<BaseGameManager>.Instance;
            BasePlugin.LogStatic(baseGameManager.managerNameKey);
            BasePlugin.LogStatic(baseGameManager.name);
            if (Other.ShouldBT(baseGameManager))
            {
                baseGameManager.StartCoroutine(BaldiTimeAnimations.TitleCard(Singleton<CoreGameManager>.Instance, baseGameManager));
            }
            if (Singleton<CoreGameManager>.Instance.GetHud(0).transform.TryFind("BaldiTimeHudManager", out Transform transform))
            {
                BaldiTimeHudManager manager = transform.gameObject.GetComponent<BaldiTimeHudManager>();
                manager.Initialize();
            }
        }

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static void StartPrefix()
        {
            if (BaseGameManagerPatches.musPlayer != null)
            {
                if (BaseGameManagerPatches.musPlayer.audioSource1 != null)
                {
                    BaseGameManagerPatches.musPlayer.Stop();
                }
            }
            BaldiTimeActions.Reset();
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPostfix(ElevatorScreen __instance)
        {
            death = true;
        }
    }

    [HarmonyPatch(typeof(TimeOut))]
    public class TimeOutPatches
    {
        [HarmonyPatch("Begin")]
        [HarmonyPostfix]
        public static void BeginPostfix()
        {
            if (Other.ShouldBT(Singleton<BaseGameManager>.Instance))
            {
                if (BaldiTimeActions.lap > 1 || BasePlugin.AssetMan.Get<AudioClip>("Lap1-Outro") == null)
                {
                    Singleton<MusicManager>.Instance.StopMidi();
                }
            }
        }
    }

    [HarmonyPatch(typeof(Baldi))]
    public class BaldiPatches
    {
        [HarmonyPatch("GetAngry")]
        [HarmonyPrefix]
        public static bool Prefix(Baldi __instance, float value)
        {
            if (Other.ShouldBT(Singleton<BaseGameManager>.Instance))
            {
                float anger = (float)__instance.ReflectionGetVariable("anger");
                float angerSpeed = 0.75f;
                anger += value * angerSpeed;
                if (anger <= 0.1f)
                {
                    anger = 0.1f;
                }
                __instance.ReflectionSetVariable("anger", anger);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(ArtsAndCrafters_Stalking), "InPlayerSight")]
    public class ArtsAndCrafters_StalkingPatches
    {
        [HarmonyPrefix]
        public static bool InPlayerSightPrefix(PlayerManager player, ArtsAndCrafters_Stalking __instance)
        {
            if (Other.ShouldBT(Singleton<BaseGameManager>.Instance))
            {
                ArtsAndCrafters crafters = __instance.ReflectionGetVariable("crafters") as ArtsAndCrafters;
                float timeInPlayerSight = (float)__instance.ReflectionGetVariable("timeInPlayerSight");

                if (!player.Tagged)
                {
                    timeInPlayerSight += Time.deltaTime * crafters.TimeScale;
                    __instance.ReflectionSetVariable("timeInPlayerSight", timeInPlayerSight);
                    if (timeInPlayerSight >= crafters.AngryTime)
                    {
                        crafters.GetAngry(player);
                    }
                }
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(ArtsAndCrafters_Teleporting), "Enter")]
    public class ArtsAndCrafters_TeleportingPatches
    {
        [HarmonyPrefix]
        public static void EnterPrefix(ArtsAndCrafters_Teleporting __instance)
        {
            BaldiTimeActions.points -= 250f;
            if (BaldiTimeActions.points < 0f)
            {
                BaldiTimeActions.points = 0f;
            }
            BaldiTimeActions.AddCombo(0f, -3f);

            ArtsAndCrafters crafters = __instance.ReflectionGetVariable("crafters") as ArtsAndCrafters;
            IEnumerator enumerator(ArtsAndCrafters crafters1)
            {
                float timer = 0f;
                while (timer < crafters1.AttackTime - 0.5f)
                {
                    if (crafters1 == null || crafters1.ec == null)
                    {
                        yield break;
                    }
                    timer += Time.deltaTime * crafters1.TimeScale;
                    yield return null;
                }
                crafters1.ec.SpawnNPC(NPCMetaStorage.Instance.Find(x => x.character == Character.Crafters).value, crafters1.ec.RandomCell(false, false, true).position);
                yield break;
            }

            crafters.StartCoroutine(enumerator(crafters));
        }
    }

    [HarmonyPatch(typeof(ArtsAndCrafters), "Initialize")]
    public class ArtsAndCraftersPatches
    {
        [HarmonyPostfix]
        public static void InitializePostfix(ArtsAndCrafters __instance)
        {
            __instance.spriteRenderer[0].sprite = AssetFinder.FindOfTypeWithName<Sprite>("CraftersSprites_0", true);
            Transform[] echo = __instance.ReflectionGetVariable("echo") as Transform[];
            foreach (var i in echo)
            {
                if (i.gameObject != null)
                {
                    i.gameObject.SetActive(false);
                }
            }
            AudioManager audMan = __instance.ReflectionGetVariable("audMan") as AudioManager;
            if (audMan != null)
            {
                audMan.FlushQueue(true);
            }
            AudioManager attackAudMan = __instance.ReflectionGetVariable("attackAudMan") as AudioManager;
            if (attackAudMan != null)
            {
                attackAudMan.FlushQueue(true);
            }
        }
    }

    [HarmonyPatch(typeof(ItemManager))]
    public class ItemManagerPatches
    {
        [HarmonyPatch("UseItem")]
        [HarmonyPrefix]
        public static void UseItemPrefix(ItemManager __instance)
        {
            if (Singleton<BaseGameManager>.Instance.GameMode != GameMode.HideAndSeek || Singleton<BaseGameManager>.Instance.InPitstop())
            {
                return;
            }
            bool disabled = (bool)__instance.ReflectionGetVariable("disabled");
            if (disabled && (!__instance.items[__instance.selectedItem].overrideDisabled || __instance.maxItem < 0))
            {
                return;
            }
            BaldiTimeActions.AddCombo(0f, __instance.items[__instance.selectedItem].price / 45f);
        }
    }

    [HarmonyPatch(typeof(Playtime))]
    public class PlaytimePatch
    {
        [HarmonyPatch("EndJumprope")]
        [HarmonyPostfix]
        public static void Postfix(bool won)
        {
            if (won)
            {
                BaldiTimeActions.points += 225f;
            }
            BaldiTimeActions.AddCombo(1f);
        }
    }

    [HarmonyPatch(typeof(DetentionRoomFunction))]
    public class DetentionPatch
    {
        [HarmonyPatch("Activate")]
        [HarmonyPostfix]
        public static void Postfix(float time)
        {
            BaldiTimeActions.points -= time * 20f;
            if (BaldiTimeActions.points < 0f)
            {
                BaldiTimeActions.points = 0f;
            }
            BaldiTimeActions.AddCombo(0f, -time / 5f);
        }
    }

    [HarmonyPatch(typeof(Elevator))]
    public class ElevatorPatch
    {
        [HarmonyPatch("BreakElevator")]
        [HarmonyPostfix]
        public static void Postfix()
        {
            BaldiTimeActions.AddCombo(1f);
        }
    }

    [HarmonyPatch(typeof(MainMenu))]
    public class MainMenuPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Postfix()
        {
            ElevatorScreenPatches.death = false;
            BaldiTimeAnimations.TitleCardRandom = -1;
            BaldiTimeActions.SpoopMusicsRandom = -1;
        }
    }

    [HarmonyPatch(typeof(GameCamera))]
    public class GameCameraPatches
    {
        [HarmonyPatch("LateUpdate")]
        [HarmonyPostfix]
        public static void LateUpdatePostfix(GameCamera __instance)
        {
            if (__instance == null)
            {
                return;
            }
            BaldiTimeUI.UpdateCam(__instance);
        }
    }

    [HarmonyPatch(typeof(Pickup))]
    public class PickupPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Postfix(Pickup __instance)
        {
            if (__instance.item == BasePlugin.AssetMan.Get<ItemObject>("BaldiClock"))
            {
                __instance.itemSprite.sprite = BasePlugin.AssetMan.Get<Sprite>("BaldiClockIcon_Large_Transparent");
            }
        }

        [HarmonyPatch("Collect")]
        [HarmonyPrefix]
        public static bool Prefix(int player, Pickup __instance)
        {
            if (__instance.item == BasePlugin.AssetMan.Get<ItemObject>("BaldiClock"))
            {
                Singleton<CoreGameManager>.Instance.audMan.PlaySingle(__instance.item.audPickupOverride);
                Object.Instantiate(__instance.item.item).Use(Singleton<CoreGameManager>.Instance.GetPlayer(player));
                __instance.free = false;
                __instance.item.price = 12251225;
                __instance.itemSprite.sprite = BasePlugin.AssetMan.Get<Sprite>("BaldiClockIcon_Large_Transparent");
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(WarningScreen))]
    public class WarningScreenPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static bool UpdatePrefix()
        {
            if (BaldiTimeAnimations.openingPlayed)
            {
                return true;
            }
            return false;
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPostfix(WarningScreen __instance)
        {
            Canvas canvas = __instance.GetComponent<Canvas>();
            AudioSource audSource = __instance.ReflectionGetVariable("audSource") as AudioSource;
            audSource.Stop();
            __instance.textBox.gameObject.SetActive(false);
            BaldiTimeAnimations.OpeningAnimationsButVoid(canvas, audSource, __instance.textBox, __instance);
        }
    }
}