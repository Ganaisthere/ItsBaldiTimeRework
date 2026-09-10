using HarmonyLib;
using MTM101BaldAPI.Reflection;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace ItsBaldiTimeRework
{
    [HarmonyPatch(typeof(MainGameManager))]
    public class MainGameManagerPatches
    {
        public static HappyBaldi happyBaldi = null;

        [HarmonyPatch("BeginPlay")]
        [HarmonyPostfix]
        public static void BeginPlayPostfix(MainGameManager __instance)
        {
            BaldiTimeActions.Setup(__instance);
        }

        [HarmonyPatch("CreateHappyBaldi")]
        [HarmonyPrefix]
        public static bool CreateHappyBaldiPrefix()
        {
            return false;
        }

        [HarmonyPatch("AllNotebooks")]
        [HarmonyPrefix]
        public static bool AllNotebooksPrefix(MainGameManager __instance)
        {
            __instance.ReflectionSetVariable("allNotebooksFound", true);
            if (BaldiTimeActions.lap < 1)
            {
                List<RoomController> rooms = __instance.Ec.rooms;
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
                BaldiTimeActions.Lapping(__instance);
            }
            else
            {
                Activity lastActivity = __instance.ReflectionGetVariable("lastActivity") as Activity;
                foreach (Activity activity in __instance.Ec.activities)
                {
                    if (activity != lastActivity)
                    {
                        activity.Corrupt(val: false);
                        activity.SetBonusMode(val: true);
                    }
                }
                __instance.Ec.ElevatorManager.SetTotalOutOfOrderElevators(__instance.Ec.Elevators.Count - 1);
                __instance.Ec.ElevatorManager.SetAllElevators(ElevatorState.OpenForExit);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(HudManager))]
    public class HudManagerPatches
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePostfix(HudManager __instance)
        {
            if (Singleton<BaseGameManager>.Instance.GameMode != GameMode.HideAndSeek)
            {
                return;
            }
            BaldiTimeUI.SetupGUI(__instance);
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix(HudManager __instance)
        {
            if (Singleton<BaseGameManager>.Instance == null)
            {
                return;
            }
            if (Singleton<BaseGameManager>.Instance.GameMode != GameMode.HideAndSeek)
            {
                return;
            }
            BaldiTimeUI.UpdateGUI(__instance);
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
            if (Singleton<BaseGameManager>.Instance.InPitstop())
            {
                return;
            }
            BaldiTimeActions.points += points;
            if (BaldiTimeActions.points < 0f)
            {
                BaldiTimeActions.points = 0f;
            }
            BaldiTimeActions.AddCombo(0f, math.max(points / 10f, -10f));
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

        [HarmonyPatch("CollectNotebooks")]
        [HarmonyPrefix]
        public static void CollectNotebooksPrefix(int count)
        {
            if (count > 0)
            {
                BaldiTimeActions.points += 475f * count;
                BaldiTimeActions.AddCombo(1f * count);
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
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix()
        {
            if (Singleton<BaseGameManager>.Instance.GameMode != GameMode.HideAndSeek || Singleton<BaseGameManager>.Instance.InPitstop())
            {
                return;
            }
            BaldiTimeActions.Update();
        }

        [HarmonyPatch("FinishLevel")]
        [HarmonyPostfix]
        public static void FinishLevelPostfix()
        {
            ElevatorScreenPatches.death = false;
        }

        [HarmonyPatch("BeginPlay")]
        [HarmonyPostfix]
        public static void BeginPlayPostfix(MainGameManager __instance)
        {
            if (Singleton<BaseGameManager>.Instance.GameMode != GameMode.HideAndSeek || Singleton<BaseGameManager>.Instance.InPitstop())
            {
                return;
            }
            if (__instance.gameObject.GetComponentInChildren<CustomAudioPlayer>() == null)
            {
                GameObject cMPObj = new GameObject("CustomMusicPlayer");
                cMPObj.transform.SetParent(__instance.transform);
                musPlayer = cMPObj.AddComponent<CustomAudioPlayer>();
                musPlayer.audioSource1 = cMPObj.AddComponent<AudioSource>();
                musPlayer.audioSource2 = cMPObj.AddComponent<AudioSource>();
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
            if (BaldiTimeUI.TimerBar != null)
            {
                BaldiTimeUI.TimerBar.rectTransform.anchoredPosition = new Vector2(0f, -38f);
                if (Singleton<BaseGameManager>.Instance.GameMode != GameMode.HideAndSeek || Singleton<BaseGameManager>.Instance.InPitstop())
                {
                    BaldiTimeUI.PointDisplay.rectTransform.localScale = Vector3.zero;
                    BaldiTimeUI.ComboBar.rectTransform.localScale = Vector3.zero;
                }
                else
                {
                    BaldiTimeUI.PointDisplay.rectTransform.localScale = Vector3.one;
                    BaldiTimeUI.ComboBar.rectTransform.localScale = Vector3.one;
                    BaldiTimeUI.RankBackground.color = BaldiTimeUI.RankColors[0];
                    BaldiTimeUI.RankColor.sprite = BaldiTimeUI.RankColorSprites[0];
                    BaldiTimeUI.RankOverlay.sprite = BaldiTimeUI.RankOverlaySprites[0];
                }
            }
        }

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static void StartPrefix()
        {
            MainGameManagerPatches.happyBaldi = null;
            if (BaseGameManagerPatches.musPlayer != null)
            {
                BaseGameManagerPatches.musPlayer.Stop();
            }
            if (BaldiTimeUI.PointDisplay != null)
            {
                BaldiTimeUI.PointDisplayText.text = "0";
            }
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPostfix(ElevatorScreen __instance)
        {
            if (death)
            {
                return;
            }
            List<IEnumerator> queuedEnumerators = __instance.ReflectionGetVariable("queuedEnumerators") as List<IEnumerator>;
            AudioManager audMan = __instance.ReflectionGetVariable("audMan") as AudioManager;
            //Debug.LogWarning("queuedEnumerators.Count = " + queuedEnumerators.Count);
            if (queuedEnumerators.Count == 2 && BaldiTimeAnimations.TitleCardBackSprites.Count > 0)
            {
                //__instance.StopCoroutine("ZoomIntro");
                //__instance.StopCoroutine("Shut");
                Singleton<MusicManager>.Instance.StopMidi();
                //queuedEnumerators.Clear();
                //CursorInitiator cursorInitiator = __instance.ReflectionGetVariable("cursorInitiator") as CursorInitiator;
                //cursorInitiator.enabled = true;
                //__instance.transform.localScale = Vector3.one;
                //__instance.ReflectionSetVariable("busy", true);
                BaldiTimeAnimations.TitleCardAnimationsButVoid(__instance.Canvas, audMan, __instance);
            }
            else if (queuedEnumerators.Count == 3)
            {
                Singleton<MusicManager>.Instance.StopMidi();
                BaldiTimeAnimations.RankAnimationsButVoid(audMan, __instance);
            }
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
            if (Singleton<BaseGameManager>.Instance.GameMode != GameMode.HideAndSeek || Singleton<BaseGameManager>.Instance.InPitstop())
            {
                return;
            }
            if (BaldiTimeActions.lap > 1 || BasePlugin.AssetMan.Get<AudioClip>("Lap1-Outro") == null)
            {
                Singleton<MusicManager>.Instance.StopMidi();
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
            if (Singleton<BaseGameManager>.Instance.GameMode != GameMode.HideAndSeek || Singleton<BaseGameManager>.Instance.InPitstop())
            {
                return true;
            }
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
    }

    [HarmonyPatch(typeof(HappyBaldi))]
    public class HappyBaldiPatches
    {
        [HarmonyPatch("Activate")]
        [HarmonyPrefix]
        public static bool Prefix(HappyBaldi __instance)
        {
            if (Singleton<BaseGameManager>.Instance.GameMode != GameMode.HideAndSeek || Singleton<BaseGameManager>.Instance.InPitstop())
            {
                return true;
            }
            MainGameManagerPatches.happyBaldi = __instance;
            return false;
        }
    }

    [HarmonyPatch(typeof(ItemManager))]
    public class ItemManagerPatches
    {
        [HarmonyPatch("UseItem")]
        [HarmonyPrefix]
        public static bool UseItemPrefix(ItemManager __instance)
        {
            if (Singleton<BaseGameManager>.Instance.GameMode != GameMode.HideAndSeek || Singleton<BaseGameManager>.Instance.InPitstop())
            {
                return true;
            }
            bool disabled = (bool)__instance.ReflectionGetVariable("disabled");
            if (disabled && (!__instance.items[__instance.selectedItem].overrideDisabled || __instance.maxItem < 0))
            {
                return false;
            }
            Item item = Object.Instantiate(__instance.items[__instance.selectedItem].item);
            if (item.Use(__instance.pm))
            {
                if (__instance.items[__instance.selectedItem].itemType != Items.None)
                {
                    BaldiTimeActions.points += math.round(__instance.items[__instance.selectedItem].price / 2f);
                    BaldiTimeActions.AddCombo(0f, __instance.items[__instance.selectedItem].price / 90f);
                }
                if (Singleton<CoreGameManager>.Instance.inventoryChallenge)
                {
                    __instance.ReduceTargetInventorySize();
                }
                __instance.RemoveItem(__instance.selectedItem);
                item?.PostUse(__instance.pm);
            }
            return false;
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
        }
    }

    [HarmonyPatch(typeof(WarningScreen))]
    public class WarningScreenPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static bool UpdatePrefix()
        {
            if (BaldiTimeAnimations.openingPlayed || !BasePlugin.Instance.ConfigOpeningAnimations.Value)
            {
                return true;
            }
            return false;
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPostfix(WarningScreen __instance)
        {
            if (!BasePlugin.Instance.ConfigOpeningAnimations.Value)
            {
                return;
            }
            Canvas canvas = __instance.GetComponent<Canvas>();
            AudioSource audSource = __instance.ReflectionGetVariable("audSource") as AudioSource;
            audSource.Stop();
            __instance.textBox.gameObject.SetActive(false);
            BaldiTimeAnimations.OpeningAnimationsButVoid(canvas, audSource, __instance.textBox, __instance);
        }
    }
}
