using HarmonyLib;
using MTM101BaldAPI.Reflection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace ItsBaldiTimeRework
{
    public class BaldiTimeUI
    {
        public static TMP_Text notebookText = null;
        public static Sprite[] baldiTimeLogoSprites = new Sprite[2];
        public static RawImage TimerBar;
        public static Image TimerBarOverlay;
        public static Image TimerBarBackground;
        public static Image TimerBarNiddle;
        public static TMP_Text TimerBarTextShadow;
        public static TMP_Text TimerBarText;
        public static Sprite[] TimerBarSprites = new Sprite[3];
        public static RawImage PointDisplay;
        //public static Image PointDisplayBackground;
        public static TMP_Text PointDisplayText;
        public static Image RankBackground;
        public static Image RankColor;
        public static Image RankOverlay;
        //public static Sprite PointDisplayBackgroundSprite;
        public static Sprite RankBackgroundSprite;
        public static Sprite[] RankColorSprites = new Sprite[31];
        public static Sprite[] RankOverlaySprites = new Sprite[7];
        public static Color[] RankColors = new Color[7]
        {
            new Color(0f, 0.2f, 0.2f, 1f),
            new Color(0f, 1f, 0f, 1f),
            new Color(0f, 0.5f, 1f, 1f),
            new Color(1f, 0f, 0f, 1f),
            new Color(1f, 0.8f, 0f, 1f),
            new Color(0.6f, 0f, 1f, 1f),
            new Color(0f, 1f, 1f, 1f)
        };
        public static float PointDisplayOffset = 0f;
        public static Sprite[] ComboBarSprites = new Sprite[3];
        public static RawImage ComboBar;
        public static Image ComboBarOverlay;
        public static Image ComboBarBackground;
        public static Image ComboBarNiddle;
        public static TMP_Text ComboTextShadow;
        public static TMP_Text ComboText;
        public static float ComboBarOffset = 0f;
        public static float pointsEdit = 0f;
        public static float ComboBarTimer = 0f;
        public static float ComboTimerEdit = 0f;
        public static Sprite[] LapFlagSprites = new Sprite[7];
        public static List<Sprite> ComboLevelsSprites = new List<Sprite>();
        public static SoundObject[] comboup = new SoundObject[3];
        public static int numOld = 0;
        public static SoundObject[] rankup = new SoundObject[5];
        public static SoundObject[] rankdown = new SoundObject[5];
        public static float rankAniTimer = 0f;


        public static IEnumerator Flash(BaseGameManager baseGameManager)
        {
            HudManager hudManager = Singleton<CoreGameManager>.Instance.GetHud(0);

            GameObject whiteFlash_Obj = new GameObject("WhiteFlash");
            whiteFlash_Obj.transform.SetParent(hudManager.Canvas().transform, false);
            RawImage whiteFlash = whiteFlash_Obj.AddComponent<RawImage>();
            whiteFlash.color = new Color(1f, 1f, 1f, 1f);
            whiteFlash.rectTransform.anchorMin = new Vector2(0f, 0f);
            whiteFlash.rectTransform.anchorMax = new Vector2(1f, 1f);

            float timer = 0f;

            while (timer < 0.5f)
            {
                if (baseGameManager == null)
                {
                    yield break;
                }
                if (timer < 0.5f)
                {
                    whiteFlash.color = new Color(1f, 1f, 1f, 1f - timer * 2f);
                }
                timer += Time.deltaTime;
                yield return null;
            }
            Object.Destroy(whiteFlash_Obj);

            yield break;
        }

        public static IEnumerator TimerBarGoDown(BaseGameManager baseGameManager)
        {
            float timer = 0f;
            while (timer < 3f)
            {
                if (baseGameManager == null)
                {
                    yield break;
                }
                if (TimerBar != null)
                {
                    TimerBar.rectTransform.anchoredPosition = new Vector2(0f, 90f - 128f * (timer / 3f));
                }
                timer += Time.deltaTime * Time.timeScale;
                yield return null;
            }
            if (TimerBar != null)
            {
                TimerBar.rectTransform.anchoredPosition = new Vector2(0f, -38f);
            }
            yield break;
        }

        public static IEnumerator LappingAnimations(BaseGameManager baseGameManager)
        {
            HudManager hudManager = Singleton<CoreGameManager>.Instance.GetHud(0);
            if (BaldiTimeActions.lap == 1)
            {
                GameObject whiteFlash_Obj = new GameObject("WhiteFlash");
                whiteFlash_Obj.transform.SetParent(hudManager.Canvas().transform, false);
                RawImage whiteFlash = whiteFlash_Obj.AddComponent<RawImage>();
                whiteFlash.color = new Color(1f, 1f, 1f, 1f);
                whiteFlash.rectTransform.anchorMin = new Vector2(0f, 0f);
                whiteFlash.rectTransform.anchorMax = new Vector2(1f, 1f);

                GameObject baldiTimeLogo_Obj = new GameObject("BaldiTimeLogo");
                baldiTimeLogo_Obj.transform.SetParent(hudManager.Canvas().transform, false);
                Image baldiTimeLogoImage = baldiTimeLogo_Obj.AddComponent<Image>();
                baldiTimeLogoImage.sprite = baldiTimeLogoSprites[0];
                baldiTimeLogoImage.rectTransform.sizeDelta = new Vector2(250f, 200f);
                baldiTimeLogoImage.rectTransform.anchorMin = new Vector2(0.5f, -0.5f);
                baldiTimeLogoImage.rectTransform.anchorMax = new Vector2(0.5f, -0.5f);

                float timer = 0f;
                float timerBTLI = 0f;
                int frame = 0;
                while (timer < 2f)
                {
                    if (baseGameManager == null)
                    {
                        yield break;
                    }
                    if (timer < 0.5f)
                    {
                        whiteFlash.color = new Color(1f, 1f, 1f, 1f - timer * 2f);
                    }
                    else
                    {
                        whiteFlash.color = new Color(1f, 1f, 1f, 0f);
                    }
                    baldiTimeLogoImage.rectTransform.anchorMin = new Vector2(0.5f, -0.5f + 2f * (timer / 2f));
                    baldiTimeLogoImage.rectTransform.anchorMax = new Vector2(0.5f, -0.5f + 2f * (timer / 2f));
                    if (timerBTLI < 0.08f)
                    {
                        timerBTLI += Time.deltaTime * Time.timeScale;
                    }
                    else
                    {
                        timerBTLI = 0f;
                        if (frame == 0)
                        {
                            frame = 1;
                        }
                        else
                        {
                            frame = 0;
                        }
                        baldiTimeLogoImage.sprite = baldiTimeLogoSprites[frame];
                    }
                    timer += Time.deltaTime * Time.timeScale;
                    yield return null;
                }
                Object.Destroy(whiteFlash_Obj);
                Object.Destroy(baldiTimeLogo_Obj);

                timer = 0f;
                while (timer < 3f)
                {
                    if (baseGameManager == null)
                    {
                        yield break;
                    }
                    if (TimerBar != null)
                    {
                        TimerBar.rectTransform.anchoredPosition = new Vector2(0f, -38f + 128f * (timer / 3f));
                    }
                    timer += Time.deltaTime * Time.timeScale;
                    yield return null;
                }
                if (TimerBar != null)
                {
                    TimerBar.rectTransform.anchoredPosition = new Vector2(0f, 90f);
                }
            }
            else if (BaldiTimeActions.lap == 2)
            {
                GameObject LapFlag_Obj = new GameObject("LapFlag");
                LapFlag_Obj.transform.SetParent(hudManager.Canvas().transform, false);
                Image LapFlag = LapFlag_Obj.AddComponent<Image>();
                LapFlag.rectTransform.anchorMax = new Vector2(0.5f, 1f);
                LapFlag.rectTransform.anchorMin = new Vector2(0.5f, 1f);
                LapFlag.rectTransform.sizeDelta = new Vector2(190f, 90f);
                LapFlag.rectTransform.pivot = new Vector2(0.5f, 0f);
                LapFlag.sprite = LapFlagSprites[BaldiTimeActions.lap - 2];

                float timer = 0f;
                while (timer < 2f)
                {
                    if (baseGameManager == null)
                    {
                        yield break;
                    }
                    float num = math.sin(timer * math.PI / 2f);
                    LapFlag.rectTransform.anchoredPosition = new Vector2(0f, num * -120);
                    timer += Time.deltaTime;
                    yield return null;
                }

                Object.Destroy(LapFlag_Obj);
            }
            yield break;
        }

        public static void UpdateGUI(HudManager hudManager)
        {
            if (TimerBarBackground != null)
            {
                float fillAmount = 1f - (BaldiTimeActions.pizzaTimer / BaldiTimeActions.pizzaTimerTotal);
                TimerBarBackground.rectTransform.localScale = new Vector3(fillAmount, 1f, 1f);
                TimerBarNiddle.rectTransform.anchoredPosition = new Vector2(-121f + 242f * fillAmount, TimerBarNiddle.rectTransform.anchoredPosition.y);
                TimerBarNiddle.rectTransform.localScale = new Vector3(1f + 0.25f * math.sin(BaldiTimeActions.pizzaTimer * math.PI), 1f + 0.25f * math.sin((BaldiTimeActions.pizzaTimer + math.PI) * math.PI), 1f);
                TimerBarTextShadow.text = string.Format("{0}:{1}", Mathf.Floor(BaldiTimeActions.pizzaTimer / 60).ToString("0"), (BaldiTimeActions.pizzaTimer % 60).ToString("00"));
                TimerBarText.text = TimerBarTextShadow.text;
                if (Singleton<BaseGameManager>.Instance != null)
                {
                    if (!Singleton<BaseGameManager>.Instance.InPitstop() && Singleton<BaseGameManager>.Instance.GameReady)
                    {
                        int num = 0;
                        float fill = 1f;
                        if (BaldiTimeActions.points + BaldiTimeActions.comboPoints >= BaldiTimeActions.pointsForPRank)
                        {
                            if (BaldiTimeActions.lap >= 2 && BaldiTimeActions.comboKeep)
                            {
                                num = 5;
                            }
                            else
                            {
                                num = 4;
                            }
                        }
                        else if (BaldiTimeActions.points + BaldiTimeActions.comboPoints >= BaldiTimeActions.pointsForPRank / 2f)
                        {
                            num = 3;
                            fill = (BaldiTimeActions.points + BaldiTimeActions.comboPoints - BaldiTimeActions.pointsForPRank / 2f) / (BaldiTimeActions.pointsForPRank / 2f);
                        }
                        else if (BaldiTimeActions.points + BaldiTimeActions.comboPoints >= BaldiTimeActions.pointsForPRank / 4f)
                        {
                            num = 2;
                            fill = (BaldiTimeActions.points + BaldiTimeActions.comboPoints - BaldiTimeActions.pointsForPRank / 4f) / (BaldiTimeActions.pointsForPRank / 4f);
                        }
                        else if (BaldiTimeActions.points + BaldiTimeActions.comboPoints >= BaldiTimeActions.pointsForPRank / 8f)
                        {
                            num = 1;
                            fill = (BaldiTimeActions.points + BaldiTimeActions.comboPoints - BaldiTimeActions.pointsForPRank / 8f) / (BaldiTimeActions.pointsForPRank / 8f);
                        }
                        else
                        {
                            fill = (BaldiTimeActions.points + BaldiTimeActions.comboPoints) / (BaldiTimeActions.pointsForPRank / 8f);
                        }
                        for (float i = 0f; i < 31f; i++)
                        {
                            if (fill >= i * (1f / 31f) && fill < (i + 1f) * (1f / 31f))
                            {
                                RankColor.sprite = RankColorSprites[(int)i];
                                break;
                            }
                        }
                        BaldiTimeActions.rank = BaldiTimeActions.ranks[num];
                        RankBackground.color = RankColors[num];
                        RankColor.color = new Color(0f, 0f, 0f, 0f);
                        if (num < 4)
                        {
                            RankColor.color = RankColors[num + 1];
                        }
                        RankOverlay.sprite = RankOverlaySprites[num];
                        if (pointsEdit > BaldiTimeActions.points)
                        {
                            pointsEdit -= 5;
                        }
                        if (pointsEdit < BaldiTimeActions.points)
                        {
                            pointsEdit += 5;
                        }
                        if (math.abs(pointsEdit - BaldiTimeActions.points) < 10)
                        {
                            pointsEdit = BaldiTimeActions.points;
                        }
                        PointDisplayText.text = pointsEdit.ToString();
                        if (numOld != num)
                        {
                            if (numOld < num)
                            {
                                numOld = num;
                                Singleton<CoreGameManager>.Instance.audMan.PlaySingle(rankup[math.min(num, rankup.Length)]);
                            }
                            else
                            {
                                numOld = num;
                                Singleton<CoreGameManager>.Instance.audMan.PlaySingle(rankdown[math.max(num, 0)]);
                            }
                            rankAniTimer = 0.5f;
                        }
                        if (rankAniTimer > 0f)
                        {
                            float mun = math.sin(rankAniTimer * math.PI);
                            RankBackground.rectTransform.localScale = new Vector3(1f + mun, 1f + mun, 1f + mun);
                            rankAniTimer -= Time.deltaTime;
                        }
                        else
                        {
                            RankBackground.rectTransform.localScale = Vector3.one;
                        }
                    }
                }
                //----------------------------------------------------------------------
                if (BaldiTimeActions.comboTimer > 0 && BaldiTimeActions.combo > 0)
                {
                    ComboBarTimer += Time.unscaledDeltaTime;
                    if (ComboBarTimer > 1f)
                    {
                        ComboBarTimer = 1f;
                    }
                }
                else
                {
                    ComboBarTimer -= Time.unscaledDeltaTime;
                    if (ComboBarTimer < 0f)
                    {
                        ComboBarTimer = 0f;
                    }
                }
                ComboBarOffset = 256f * (1f - math.sin(ComboBarTimer * math.PI / 2f));
                //----------------------------------------------------------------------
                if (notebookText != null)
                {
                    float pointDisplayXFix = notebookText.GetComponent<RectTransform>().anchoredPosition.x - 50f;
                    float ComboBarXFix = hudManager.inventory.rectTransform.anchoredPosition.x - 160f;
                    PointDisplay.rectTransform.anchoredPosition = new Vector2(-80f - pointDisplayXFix + PointDisplayOffset, PointDisplay.rectTransform.anchoredPosition.y);
                    ComboBar.rectTransform.anchoredPosition = new Vector2(-96f - ComboBarXFix + ComboBarOffset, ComboBar.rectTransform.anchoredPosition.y);
                }
                //----------------------------------------------------------------------
                ComboTextShadow.text = BaldiTimeActions.combo.ToString() + " Combo!";
                ComboText.text = ComboTextShadow.text;
                ComboText.color = Color.white;
                if (BaldiTimeActions.comboKeep)
                {
                    ComboText.color = new Color(1f, 0.6f, 1f, 1f);
                }
                //----------------------------------------------------------------------
                float filled = BaldiTimeActions.comboTimer / BaldiTimeActions.comboTimerMax;
                if (ComboTimerEdit > filled)
                {
                    ComboTimerEdit -= Time.unscaledDeltaTime;
                }
                if (ComboTimerEdit < filled)
                {
                    ComboTimerEdit += Time.unscaledDeltaTime;
                }
                if (math.abs(ComboTimerEdit - filled) < Time.unscaledDeltaTime * 2)
                {
                    ComboTimerEdit = filled;
                }
                ComboBarNiddle.rectTransform.anchoredPosition = new Vector2(math.round(-183f + 143f * ComboTimerEdit), ComboBarNiddle.rectTransform.anchoredPosition.y);
            }
        }
        public static IEnumerator ShowComboLevels(BaseGameManager baseGameManager)
        {
            GameObject ComboLevels_Obj = new GameObject("ComboLevels");
            ComboLevels_Obj.transform.SetParent(ComboBar.transform, false);
            Image ComboLevels = ComboLevels_Obj.AddComponent<Image>();
            ComboLevels.rectTransform.sizeDelta = new Vector2(160f, 96f);
            ComboLevels.rectTransform.anchoredPosition = new Vector2(88f, -96f);//-112f
            if (ComboLevelsSprites.Count > 0)
            {
                ComboLevels.sprite = ComboLevelsSprites[UnityEngine.Random.Range(0, ComboLevelsSprites.Count - 1)];
            }
            int num = UnityEngine.Random.Range(0, comboup.Length - 1);
            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(comboup[num]);

            float timer = 0f;
            while (timer < 3f)
            {
                if (baseGameManager == null)
                {
                    yield break;
                }
                float edit = math.sin(timer / 3f * math.PI);
                ComboLevels.rectTransform.anchoredPosition = new Vector2(88f - edit * 200f, -96f);
                timer += Time.deltaTime;
                yield return null;
            }
            Object.Destroy(ComboLevels_Obj);
            yield break;
        }
        public static void SetupGUI(HudManager hudManager)
        {
            notebookText = (hudManager.ReflectionGetVariable("textBox") as TMP_Text[])[0];

            GameObject TimerBar_Obj = new GameObject("TimerBar");
            TimerBar_Obj.transform.SetParent(hudManager.Canvas().transform, false);
            TimerBar = TimerBar_Obj.AddComponent<RawImage>();
            TimerBar.color = new Color(0f, 0f, 0f, 0f);
            TimerBar.rectTransform.anchorMin = new Vector2(0.5f, 0f);
            TimerBar.rectTransform.anchorMax = new Vector2(0.5f, 0f);
            TimerBar.rectTransform.anchoredPosition = new Vector2(0f, -38f);

            GameObject TimerBarBackground_Obj = new GameObject("TimerBarBackground");
            TimerBarBackground_Obj.transform.SetParent(TimerBar.transform, false);
            TimerBarBackground = TimerBarBackground_Obj.AddComponent<Image>();
            TimerBarBackground.sprite = TimerBarSprites[1];
            TimerBarBackground.rectTransform.sizeDelta = new Vector2(242f, 18f);
            TimerBarBackground.rectTransform.pivot = new Vector2(0f, 0.5f);
            TimerBarBackground.rectTransform.anchoredPosition = new Vector2(-121f, 0f);

            GameObject TimerBarOverlay_Obj = new GameObject("TimerBarOverlay");
            TimerBarOverlay_Obj.transform.SetParent(TimerBar.transform, false);
            TimerBarOverlay = TimerBarOverlay_Obj.AddComponent<Image>();
            TimerBarOverlay.sprite = TimerBarSprites[0];
            TimerBarOverlay.rectTransform.sizeDelta = new Vector2(256f, 32f);

            GameObject TimerBarNiddle_Obj = new GameObject("TimerBarNiddle");
            TimerBarNiddle_Obj.transform.SetParent(TimerBar.transform, false);
            TimerBarNiddle = TimerBarNiddle_Obj.AddComponent<Image>();
            TimerBarNiddle.sprite = TimerBarSprites[2];
            TimerBarNiddle.rectTransform.sizeDelta = new Vector2(32f, 32f);

            GameObject TimerBarTextShadow_Obj = new GameObject("TimerBarTextShadow");
            TimerBarTextShadow_Obj.transform.SetParent(TimerBar.transform, false);
            TimerBarTextShadow = TimerBarTextShadow_Obj.AddComponent<TextMeshProUGUI>();
            TimerBarTextShadow.fontSize = 24f;
            TimerBarTextShadow.alignment = TextAlignmentOptions.Center;
            TimerBarTextShadow.rectTransform.sizeDelta = new Vector2(256f, 32f);
            TimerBarTextShadow.color = Color.black;

            GameObject TimerBarText_Obj = new GameObject("TimerBarText");
            TimerBarText_Obj.transform.SetParent(TimerBarTextShadow.transform, false);
            TimerBarText = TimerBarText_Obj.AddComponent<TextMeshProUGUI>();
            TimerBarText.fontSize = 24f;
            TimerBarText.alignment = TextAlignmentOptions.Center;
            TimerBarText.rectTransform.sizeDelta = new Vector2(256f, 32f);
            TimerBarText.rectTransform.anchoredPosition = new Vector2(-1f, 1f);
            TimerBarText.color = Color.white;

            GameObject PointDisplay_Obj = new GameObject("PointDisplay");
            PointDisplay_Obj.transform.SetParent(notebookText.transform, false);
            PointDisplay = PointDisplay_Obj.AddComponent<RawImage>();
            PointDisplay.color = new Color(0f, 0f, 0f, 0f);
            PointDisplay.rectTransform.anchoredPosition = new Vector2(-80f, -45f);
            PointDisplay.rectTransform.anchorMax = new Vector2(0f, 0.5f);
            PointDisplay.rectTransform.anchorMin = new Vector2(0f, 0.5f);

            /*GameObject PointDisplayBackground_Obj = new GameObject("PointDisplayBackground");
            PointDisplayBackground_Obj.transform.SetParent(PointDisplay.transform, false);
            PointDisplayBackground = PointDisplayBackground_Obj.AddComponent<Image>();
            PointDisplayBackground.sprite = PointDisplayBackgroundSprite;
            PointDisplayBackground.rectTransform.sizeDelta = new Vector2(196f, 36f);
            PointDisplayBackground.rectTransform.pivot = new Vector2(0f, 0.5f);*/

            GameObject PointDisplayText_Obj = new GameObject("PointDisplayText");
            PointDisplayText_Obj.transform.SetParent(PointDisplay.transform, false);
            PointDisplayText = PointDisplayText_Obj.AddComponent<TextMeshProUGUI>();
            PointDisplayText.fontSize = 24f;
            PointDisplayText.alignment = TextAlignmentOptions.Left;
            PointDisplayText.color = Color.black;
            PointDisplayText.rectTransform.sizeDelta = new Vector2(256f, 32f);
            PointDisplayText.rectTransform.anchoredPosition = new Vector2(70f, 0f);
            PointDisplayText.rectTransform.pivot = new Vector2(0f, 0.5f);
            PointDisplayText.text = "0";

            GameObject RankBackground_Obj = new GameObject("RankBackground");
            RankBackground_Obj.transform.SetParent(PointDisplay.transform, false);
            RankBackground = RankBackground_Obj.AddComponent<Image>();
            RankBackground.sprite = RankBackgroundSprite;
            RankBackground.rectTransform.sizeDelta = new Vector2(32f, 32f);
            RankBackground.rectTransform.anchoredPosition = new Vector2(50f, 0f);

            GameObject RankColor_Obj = new GameObject("RankColor");
            RankColor_Obj.transform.SetParent(RankBackground.transform, false);
            RankColor = RankColor_Obj.AddComponent<Image>();
            RankColor.sprite = RankColorSprites[0];
            RankColor.rectTransform.sizeDelta = new Vector2(32f, 32f);

            GameObject RankOverlay_Obj = new GameObject("RankOverlay");
            RankOverlay_Obj.transform.SetParent(RankBackground.transform, false);
            RankOverlay = RankOverlay_Obj.AddComponent<Image>();
            RankOverlay.sprite = RankOverlaySprites[0];
            RankOverlay.rectTransform.sizeDelta = new Vector2(32f, 32f);

            GameObject ComboBar_Obj = new GameObject("ComboBar");
            ComboBar_Obj.transform.SetParent(hudManager.inventory.transform, false);
            ComboBar = ComboBar_Obj.AddComponent<RawImage>();
            ComboBar.color = new Color(0f, 0f, 0f, 0f);
            ComboBar.rectTransform.anchoredPosition = new Vector2(96f, -60f);

            GameObject ComboBarBackground_Obj = new GameObject("ComboBarBackground");
            ComboBarBackground_Obj.transform.SetParent(ComboBar.transform, false);
            ComboBarBackground = ComboBarBackground_Obj.AddComponent<Image>();
            ComboBarBackground.sprite = ComboBarSprites[1];
            ComboBarBackground.rectTransform.sizeDelta = new Vector2(160f, 32f);
            ComboBarBackground.rectTransform.anchoredPosition = new Vector2(-112f, 16f);

            GameObject ComboBarNiddle_Obj = new GameObject("ComboBarNiddle");
            ComboBarNiddle_Obj.transform.SetParent(ComboBar.transform, false);
            ComboBarNiddle = ComboBarNiddle_Obj.AddComponent<Image>();
            ComboBarNiddle.sprite = ComboBarSprites[2];
            ComboBarNiddle.rectTransform.sizeDelta = new Vector2(32f, 32f);
            ComboBarNiddle.rectTransform.anchoredPosition = new Vector2(-183f, 16f);

            GameObject ComboBarOverlay_Obj = new GameObject("ComboBarOverlay");
            ComboBarOverlay_Obj.transform.SetParent(ComboBar.transform, false);
            ComboBarOverlay = ComboBarOverlay_Obj.AddComponent<Image>();
            ComboBarOverlay.sprite = ComboBarSprites[0];
            ComboBarOverlay.rectTransform.sizeDelta = new Vector2(192f, 64f);
            ComboBarOverlay.rectTransform.anchoredPosition = new Vector2(-96f, 0f);

            GameObject ComboTextShadow_Obj = new GameObject("ComboTextShadow");
            ComboTextShadow_Obj.transform.SetParent(ComboBar.transform, false);
            ComboTextShadow = ComboTextShadow_Obj.AddComponent<TextMeshProUGUI>();
            ComboTextShadow.fontSize = 24f;
            ComboTextShadow.alignment = TextAlignmentOptions.Right;
            ComboTextShadow.rectTransform.sizeDelta = new Vector2(256f, 32f);
            ComboTextShadow.rectTransform.pivot = new Vector2(1f, 0.5f);
            ComboTextShadow.rectTransform.anchoredPosition = new Vector2(-36f, -8f);
            ComboTextShadow.color = Color.black;
            ComboTextShadow.text = "1225 Combo!";

            GameObject ComboText_Obj = new GameObject("ComboText");
            ComboText_Obj.transform.SetParent(ComboTextShadow.transform, false);
            ComboText = ComboText_Obj.AddComponent<TextMeshProUGUI>();
            ComboText.fontSize = 24f;
            ComboText.alignment = TextAlignmentOptions.Right;
            ComboText.rectTransform.sizeDelta = new Vector2(256f, 32f);
            ComboText.rectTransform.pivot = new Vector2(1f, 0.5f);
            ComboText.rectTransform.anchoredPosition = new Vector2(-1f, 1f);
            ComboText.rectTransform.anchorMax = new Vector2(1f, 0.5f);
            ComboText.rectTransform.anchorMin = new Vector2(1f, 0.5f);
            ComboText.color = Color.white;
            ComboText.text = "1225 Combo!";

        }
    }
}
