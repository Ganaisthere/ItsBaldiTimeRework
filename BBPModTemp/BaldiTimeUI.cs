using MTM101BaldAPI;
using MTM101BaldAPI.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ItsBaldiTimeRework
{
    internal class BaldiTimeHudManager : MonoBehaviour
    {
        public RectTransform RectTransform => gameObject.GetOrAddComponent<RectTransform>();

        public float displayedPoint = 0f, rankAniTimer = 0f, numOld = 0f;

        public GameObject TimerBar_Obj;
        public Image TimerBarBackground, TimerBarOverlay, TimerBarNiddle, TimerBarBaldiFace;
        public Mask TimerBarBackgroundMask;
        public TextMeshProUGUI TimerBarTextShadow, TimerBarText;
        public float TimerBarTimer = 0f;

        public GameObject PointDisplay_Obj;
        public Image Rank, RankOverlay;
        public Mask RankOverlayMask;
        public TextMeshProUGUI PointDisplayText;
        public float oldPoint = 0f;

        public GameObject ComboBar_Obj;
        public Image ComboBarBackGround, ComboBarNiddle, ComboBarOverlay;
        public TextMeshProUGUI ComboBarTextShadow, ComboBarText;
        public float ComboBarTimer = 0f, ComboTimer = 0f;

        public GameObject WarTimerBase_Obj;
        public Image WarTimer;
        public TextMeshProUGUI WarTimerText;

        public void Initialize()
        {
            displayedPoint = 0f;
            rankAniTimer = 0f;
            numOld = 0f;
            ComboBarTimer = 0f;
            ComboTimer = 0f;
            TimerBarTimer = 0f;

            transform.SetAsFirstSibling();

            KeepSame();
            if (TimerBar_Obj == null)
            {
                SetupTimerBar();
            }
            if (PointDisplay_Obj == null)
            {
                SetupPointDisplay();
            }
            if (ComboBar_Obj == null)
            {
                SetupComboBar();
            }
            if (WarTimerBase_Obj == null)
            {
                SetupWarTimer();
            }

            RectTransform TimerBar_Obj_RectTransform = TimerBar_Obj.GetOrAddComponent<RectTransform>();
            TimerBar_Obj_RectTransform.anchoredPosition3D = new Vector3(0f, -48f, 0f);
        }
        public void Update()
        {
            KeepSame();
            if (TimerBar_Obj != null)
            {
                UpdateTimerBar();
            }
            if (PointDisplay_Obj != null)
            {
                UpdatePointDisplay();
            }
            if (ComboBar_Obj != null)
            {
                UpdateComboBar();
            }
            if (WarTimerBase_Obj != null)
            {
                UpdateWarTimer();
            }
        }

        public void KeepSame()
        {
            RectTransform parentRectTransform = transform.parent.gameObject.GetComponent<RectTransform>();
            RectTransform.anchoredPosition3D = parentRectTransform.anchoredPosition3D;
            RectTransform.sizeDelta = parentRectTransform.sizeDelta;
            transform.rotation = parentRectTransform.rotation;

            transform.localScale = Vector3.one;
            if (Singleton<BaseGameManager>.Instance == null)
            {
                displayedPoint = 0f;
                numOld = 0f;
                transform.localScale = Vector3.zero;
            }
            else if (Singleton<BaseGameManager>.Instance.managerNameKey.Contains("Pit") || Singleton<BaseGameManager>.Instance.InPitstop())
            {
                transform.localScale = Vector3.zero;
            }
        }

        public void SetupTimerBar()
        {
            TimerBar_Obj = new GameObject("TimerBar");
            TimerBar_Obj.transform.SetParent(gameObject.transform);
            TimerBar_Obj.transform.localScale = Vector3.one;
            RectTransform TimerBar_Obj_RectTransform = TimerBar_Obj.GetOrAddComponent<RectTransform>();
            TimerBar_Obj_RectTransform.anchoredPosition3D = new Vector3(0f, -48f, 0f);
            TimerBar_Obj_RectTransform.anchorMax = new Vector2(0.5f, 0f);
            TimerBar_Obj_RectTransform.anchorMin = new Vector2(0.5f, 0f);

            GameObject TimerBarBackgroundMask_Obj = new GameObject("TimerBarBackgroundMask");
            TimerBarBackgroundMask_Obj.transform.SetParent(TimerBar_Obj.transform, false);
            TimerBarBackgroundMask_Obj.AddComponent<RawImage>();
            TimerBarBackgroundMask = TimerBarBackgroundMask_Obj.AddComponent<Mask>();
            TimerBarBackgroundMask.rectTransform.sizeDelta = new Vector2(0f, 32f);
            TimerBarBackgroundMask.rectTransform.pivot = new Vector2(0f, 0.5f);
            TimerBarBackgroundMask.rectTransform.anchoredPosition3D = new Vector3(-128f, 0f, 0f);
            TimerBarBackgroundMask.showMaskGraphic = false;

            GameObject TimerBarBackground_Obj = new GameObject("TimerBarBackground");
            TimerBarBackground_Obj.transform.SetParent(TimerBarBackgroundMask_Obj.transform, false);
            TimerBarBackground = TimerBarBackground_Obj.AddComponent<Image>();
            TimerBarBackground.sprite = BasePlugin.AssetMan.Get<Sprite>("TimerBar_1");
            TimerBarBackground.rectTransform.sizeDelta = new Vector2(512f, 32f);
            TimerBarBackground.rectTransform.anchoredPosition3D = new Vector3(256f, 0f, 0f);
            TimerBarBackground.maskable = true;

            GameObject TimerBarOverlay_Obj = new GameObject("TimerBarOverlay");
            TimerBarOverlay_Obj.transform.SetParent(TimerBar_Obj.transform, false);
            TimerBarOverlay = TimerBarOverlay_Obj.AddComponent<Image>();
            TimerBarOverlay.sprite = BasePlugin.AssetMan.Get<Sprite>("TimerBar_0");
            TimerBarOverlay.rectTransform.sizeDelta = new Vector2(256f, 32f);
            TimerBarOverlay.rectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);

            GameObject TimerBarNiddle_Obj = new GameObject("TimerBarNiddle");
            TimerBarNiddle_Obj.transform.SetParent(TimerBar_Obj.transform, false);
            TimerBarNiddle = TimerBarNiddle_Obj.AddComponent<Image>();
            TimerBarNiddle.sprite = BasePlugin.AssetMan.Get<Sprite>("TimerBar_2");
            TimerBarNiddle.rectTransform.sizeDelta = new Vector2(48f, 48f);
            TimerBarNiddle.rectTransform.anchoredPosition3D = new Vector3(-128f, 0f, 0f);

            GameObject TimerBarBaldiFace_Obj = new GameObject("TimerBarBaldiFace");
            TimerBarBaldiFace_Obj.transform.SetParent(TimerBar_Obj.transform, false);
            TimerBarBaldiFace = TimerBarBaldiFace_Obj.AddComponent<Image>();
            TimerBarBaldiFace.sprite = BasePlugin.AssetMan.Get<Sprite>("TimerBar_3");
            TimerBarBaldiFace.rectTransform.sizeDelta = new Vector2(96f, 96f);
            TimerBarBaldiFace.rectTransform.anchoredPosition3D = new Vector3(128f, 0f, 0f);

            GameObject TimerBarTextShadow_Obj = new GameObject("TimerBarTextShadow");
            TimerBarTextShadow_Obj.transform.SetParent(TimerBar_Obj.transform, false);
            TimerBarTextShadow = TimerBarTextShadow_Obj.AddComponent<TextMeshProUGUI>();
            TimerBarTextShadow.rectTransform.sizeDelta = new Vector2(128f, 128f);
            TimerBarTextShadow.rectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
            TimerBarTextShadow.alignment = TextAlignmentOptions.Center;
            TimerBarTextShadow.color = Color.black;
            TimerBarTextShadow.fontSize = 24f;
            TimerBarTextShadow.text = "0:00";

            GameObject TimerBarText_Obj = new GameObject("TimerBarText");
            TimerBarText_Obj.transform.SetParent(TimerBarTextShadow_Obj.transform, false);
            TimerBarText = TimerBarText_Obj.AddComponent<TextMeshProUGUI>();
            TimerBarText.rectTransform.sizeDelta = new Vector2(128f, 128f);
            TimerBarText.rectTransform.anchoredPosition3D = new Vector3(-1f, 1f, 0f);
            TimerBarText.alignment = TextAlignmentOptions.Center;
            TimerBarText.color = Color.white;
            TimerBarText.fontSize = 24f;
            TimerBarText.text = TimerBarTextShadow.text;
        }
        public void UpdateTimerBar()
        {
            if (BaldiTimeActions.pizzaTimer > 0)
            {
                if (TimerBarTimer < 20f)
                {
                    TimerBarTimer += Time.deltaTime;
                }
                else
                {
                    TimerBarTimer = 0f;
                }

                if (TimerBarTimer % 2f > 1f)
                {
                    TimerBarBaldiFace.sprite = BasePlugin.AssetMan.Get<Sprite>("TimerBar_3");
                }
                else
                {
                    TimerBarBaldiFace.sprite = BasePlugin.AssetMan.Get<Sprite>("TimerBar_4");
                }

                TimerBarBackground.rectTransform.anchoredPosition3D = new Vector3(128f - 256f * (TimerBarTimer / 20f), 0f, 0f);

                TimerBarBackgroundMask.rectTransform.sizeDelta = new Vector2(256f * (1f - BaldiTimeActions.pizzaTimer / BaldiTimeActions.pizzaTimerTotal), 32f);

                TimerBarNiddle.rectTransform.anchoredPosition3D = new Vector3(-128f + 256f * (1f - BaldiTimeActions.pizzaTimer / BaldiTimeActions.pizzaTimerTotal), 0f, 0f);

                TimerBarNiddle.transform.localScale = new Vector3(1f + 0.5f * math.sin(BaldiTimeActions.pizzaTimer * math.PI / 2f), 1f - 0.5f * math.sin(BaldiTimeActions.pizzaTimer * math.PI / 2f), 1f);

                TimerBarTextShadow.text = string.Format("{0}:{1}", Mathf.Floor(BaldiTimeActions.pizzaTimer / 60f).ToString("0"), (BaldiTimeActions.pizzaTimer % 60f).ToString("00"));
                TimerBarText.text = TimerBarTextShadow.text;
            }
            else
            {
                TimerBarTimer = 0f;
            }
        }

        public IEnumerator TimerBarRaise()
        {
            if (TimerBar_Obj == null)
            {
                yield break;
            }

            yield return new WaitForSeconds(2f);

            RectTransform TimerBar_Obj_RectTransform = TimerBar_Obj.GetOrAddComponent<RectTransform>();
            float timer = 0f;
            while (timer < 3f)
            {
                timer += Time.deltaTime;
                TimerBar_Obj_RectTransform.anchoredPosition3D = new Vector3(0f, -48f + 140f * math.sin(timer / 3f * math.PI / 2f), 0f);
                yield return null;
            }
            TimerBar_Obj_RectTransform.anchoredPosition3D = new Vector3(0f, 92f, 0f);
            yield break;
        }

        public IEnumerator TimerBarFall()
        {
            if (TimerBar_Obj == null)
            {
                yield break;
            }

            float timer = 0f;
            while (timer < 0.5f)
            {
                timer += Time.deltaTime;
                TimerBarBaldiFace.transform.localScale = new Vector3(1f - 1f * timer / 0.5f, 1f, 1f);
                yield return null;
            }
            TimerBarBaldiFace.sprite = BasePlugin.AssetMan.Get<Sprite>("TimerBar_5");
            timer = 0f;
            while (timer < 0.5f)
            {
                timer += Time.deltaTime;
                TimerBarBaldiFace.transform.localScale = new Vector3(1f * timer / 0.5f, 1f, 1f);
                yield return null;
            }
            TimerBarBaldiFace.transform.localScale = Vector3.one;

            yield return new WaitForSeconds(2f);

            RectTransform TimerBar_Obj_RectTransform = TimerBar_Obj.GetOrAddComponent<RectTransform>();
            timer = 0f;
            while (timer < 3f)
            {
                timer += Time.deltaTime;
                TimerBar_Obj_RectTransform.anchoredPosition3D = new Vector3(0f, -48f + 140f * math.sin((1f - timer / 3f) * math.PI / 2f), 0f);
                yield return null;
            }
            TimerBar_Obj_RectTransform.anchoredPosition3D = new Vector3(0f, -48f, 0f);
            yield break;
        }

        public void SetupPointDisplay()
        {
            PointDisplay_Obj = new GameObject("PointDisplay");
            PointDisplay_Obj.transform.SetParent(gameObject.transform);
            PointDisplay_Obj.transform.localScale = Vector3.one;
            RectTransform PointDisplay_Obj_RectTransform = PointDisplay_Obj.GetOrAddComponent<RectTransform>();
            PointDisplay_Obj_RectTransform.anchoredPosition3D = new Vector3(130f, -68f, 0f);
            PointDisplay_Obj_RectTransform.anchorMax = new Vector2(0f, 1f);
            PointDisplay_Obj_RectTransform.anchorMin = new Vector2(0f, 1f);
            PointDisplay_Obj_RectTransform.sizeDelta = new Vector2(240f, 60f);

            GameObject Rank_Obj = new GameObject("Rank");
            Rank_Obj.transform.SetParent(PointDisplay_Obj.transform, false);
            Rank = Rank_Obj.AddComponent<Image>();
            Rank.sprite = BasePlugin.AssetMan.Get<Sprite>("Rank_D_0");
            Rank.rectTransform.sizeDelta = new Vector2(40f, 40f);
            Rank.rectTransform.anchoredPosition3D = new Vector3(-106f, 0f, 0f);

            GameObject RankOverlayMask_Obj = new GameObject("RankOverlayMask");
            RankOverlayMask_Obj.transform.SetParent(Rank_Obj.transform, false);
            RankOverlayMask_Obj.AddComponent<RawImage>();
            RankOverlayMask = RankOverlayMask_Obj.AddComponent<Mask>();
            RankOverlayMask.rectTransform.sizeDelta = Rank.rectTransform.sizeDelta;
            RankOverlayMask.rectTransform.anchoredPosition3D = new Vector3(0f, -48f, 0f);
            RankOverlayMask.showMaskGraphic = false;

            GameObject RankOverlay_Obj = new GameObject("RankOverlay");
            RankOverlay_Obj.transform.SetParent(RankOverlayMask_Obj.transform, false);
            RankOverlay = RankOverlay_Obj.AddComponent<Image>();
            RankOverlay.sprite = BasePlugin.AssetMan.Get<Sprite>("Rank_D_1");
            RankOverlay.rectTransform.sizeDelta = Rank.rectTransform.sizeDelta;
            RankOverlay.rectTransform.anchoredPosition3D = new Vector3(0f, 48f, 0f);
            RankOverlay.maskable = true;

            GameObject PointDisplayText_Obj = new GameObject("PointDisplayText");
            PointDisplayText_Obj.transform.SetParent(PointDisplay_Obj.transform, false);
            PointDisplayText = PointDisplayText_Obj.AddComponent<TextMeshProUGUI>();
            PointDisplayText.rectTransform.anchoredPosition3D = new Vector3(40f, 0f, 0f);
            PointDisplayText.rectTransform.sizeDelta = new Vector2(240f, 60f);
            PointDisplayText.alignment = TextAlignmentOptions.Left;
            PointDisplayText.font = UIExtensions.FontAsset(BaldiFonts.ComicSans36);
            PointDisplayText.fontSize = 36f;
            PointDisplayText.text = "0";
            PointDisplayText.color = Color.black;
        }
        public void UpdatePointDisplay()
        {
            if (displayedPoint < BaldiTimeActions.points - 5)
            {
                displayedPoint += 5;
            }
            else if (displayedPoint > BaldiTimeActions.points + 5)
            {
                displayedPoint -= 5;
            }
            if (math.abs(displayedPoint - BaldiTimeActions.points) <= 5)
            {
                displayedPoint = BaldiTimeActions.points;
            }
            PointDisplayText.text = displayedPoint.ToString();

            int num = 4;
            float fill = 1f;
            void JustDoIt(float a, float b, int c)
            {
                float totalPoints = BaldiTimeActions.points + BaldiTimeActions.comboPoints;
                if (totalPoints >= BaldiTimeActions.pointsForPRank * a && totalPoints < BaldiTimeActions.pointsForPRank * b)
                {
                    num = c;
                    fill = (totalPoints - BaldiTimeActions.pointsForPRank * a) / (BaldiTimeActions.pointsForPRank * b - BaldiTimeActions.pointsForPRank * a);
                }
            }
            JustDoIt(-1f, 1f / 8f, 0);
            JustDoIt(0f, 1f / 8f, 0);
            JustDoIt(1f / 8f, 1f / 4f, 1);
            JustDoIt(1f / 4f, 1f / 2f, 2);
            JustDoIt(1f / 2f, 1f, 3);
            if (num == 4 && BaldiTimeActions.CanGetPRank)
            {
                num = 5;
            }
            fill = math.abs(fill);
            RankOverlayMask.rectTransform.anchoredPosition = new Vector2(0f, -48f + fill * 48f);
            RankOverlay.rectTransform.anchoredPosition = new Vector2(0f, 48f - fill * 48f);

            BaldiTimeActions.rank = BaldiTimeActions.ranks[num];

            Rank.sprite = BasePlugin.AssetMan.Get<Sprite>("Rank_" + BaldiTimeActions.rank + "_0");
            Sprite Rank_1 = BasePlugin.AssetMan.Get<Sprite>("Rank_" + BaldiTimeActions.rank + "_1");
            if (Rank_1 == null)
            {
                RankOverlay.color = new Color(0f, 0f, 0f, 0f);
            }
            else
            {
                RankOverlay.sprite = Rank_1;
                RankOverlay.color = new Color(1f, 1f, 1f, 1f);
            }
            if (numOld != num && BaldiTimeActions.pointsForPRank > 0 && BaldiTimeActions.points > 0)
            {
                if (Time.timeScale > 0)
                {
                    if (numOld < num)
                    {
                        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("rankup" + (math.min(num, 4) + 1).ToString()));
                    }
                    else
                    {
                        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("rankdown" + (math.max(num, 0) + 1).ToString()));
                    }
                }
                numOld = num;
                rankAniTimer = 0.25f;
            }
            if (rankAniTimer > 0f)
            {
                Rank.rectTransform.localScale = new Vector3(1f + rankAniTimer * 2f, 1f + rankAniTimer * 2f, 1f);
                rankAniTimer -= Time.deltaTime;
            }
            else
            {
                Rank.rectTransform.localScale = Vector3.one;
            }

            if (oldPoint != BaldiTimeActions.points + BaldiTimeActions.comboPoints)
            {
                float chaju = BaldiTimeActions.points + BaldiTimeActions.comboPoints - oldPoint;
                oldPoint = BaldiTimeActions.points + BaldiTimeActions.comboPoints;

                IEnumerator PDAni()
                {
                    GameObject pd_Obj = new GameObject("pd");
                    pd_Obj.transform.SetParent(PointDisplayText.transform);
                    TMP_Text pd = pd_Obj.AddComponent<TextMeshProUGUI>();
                    pd.rectTransform.localScale = Vector3.one;
                    pd.rectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
                    pd.rectTransform.sizeDelta = PointDisplayText.rectTransform.sizeDelta;
                    pd.alignment = TextAlignmentOptions.Left;
                    pd.fontSize = 24f;
                    pd.text = chaju.ToString();
                    pd.color = Color.red;
                    if (chaju > 0f)
                    {
                        pd.text = "+" + chaju.ToString();
                        pd.color = Color.green;
                    }

                    Vector3 vector = pd.rectTransform.anchoredPosition;
                    float timer = 0f;
                    while (timer < 0.5f)
                    {
                        if (Singleton<BaseGameManager>.Instance == null)
                        {
                            Destroy(pd.gameObject);
                            yield break;
                        }
                        pd.rectTransform.anchoredPosition = vector + new Vector3(0f, 64f, 0f) * timer;
                        timer += Time.unscaledDeltaTime;
                        pd.color = new Color(pd.color.r, pd.color.g, pd.color.b, 1f - timer * 2f);
                        yield return null;
                    }

                    Destroy(pd_Obj);
                    yield break;
                }

                StartCoroutine(PDAni());
            }
        }

        public void SetupComboBar()
        {
            ComboBar_Obj = new GameObject("ComboBar");
            ComboBar_Obj.transform.SetParent(gameObject.transform);
            ComboBar_Obj.transform.localScale = Vector3.one;
            RectTransform ComboBar_Obj_RectTransform = ComboBar_Obj.GetOrAddComponent<RectTransform>();
            ComboBar_Obj_RectTransform.anchoredPosition3D = new Vector3(-96f, 80f, 0f);
            ComboBar_Obj_RectTransform.anchorMax = Vector2.one;
            ComboBar_Obj_RectTransform.anchorMin = Vector2.one;
            ComboBar_Obj_RectTransform.sizeDelta = new Vector2(192f, 192f);

            GameObject ComboBarBackGround_Obj = new GameObject("ComboBarBackGround");
            ComboBarBackGround_Obj.transform.SetParent(ComboBar_Obj.transform);
            ComboBarBackGround = ComboBarBackGround_Obj.AddComponent<Image>();
            ComboBarBackGround.transform.localScale = Vector3.one;
            ComboBarBackGround.rectTransform.anchoredPosition3D = Vector3.zero;
            ComboBarBackGround.rectTransform.sizeDelta = new Vector2(192f, 192f);
            ComboBarBackGround.sprite = BasePlugin.AssetMan.Get<Sprite>("ComboDisplay_1");

            GameObject ComboBarNiddle_Obj = new GameObject("ComboBarNiddle");
            ComboBarNiddle_Obj.transform.SetParent(ComboBar_Obj.transform);
            ComboBarNiddle = ComboBarNiddle_Obj.AddComponent<Image>();
            ComboBarNiddle.transform.localScale = Vector3.one;
            ComboBarNiddle.rectTransform.anchoredPosition3D = new Vector3(120f, 0f, 0f);
            ComboBarNiddle.rectTransform.sizeDelta = new Vector2(192f, 192f);
            ComboBarNiddle.sprite = BasePlugin.AssetMan.Get<Sprite>("ComboDisplay_3");

            GameObject ComboBarOverlay_Obj = new GameObject("ComboBarOverlay");
            ComboBarOverlay_Obj.transform.SetParent(ComboBar_Obj.transform);
            ComboBarOverlay = ComboBarOverlay_Obj.AddComponent<Image>();
            ComboBarOverlay.transform.localScale = Vector3.one;
            ComboBarOverlay.rectTransform.anchoredPosition3D = Vector3.zero;
            ComboBarOverlay.rectTransform.sizeDelta = new Vector2(192f, 192f);
            ComboBarOverlay.sprite = BasePlugin.AssetMan.Get<Sprite>("ComboDisplay_0");

            GameObject ComboBarTextShadow_Obj = new GameObject("ComboBarTextShadow");
            ComboBarTextShadow_Obj.transform.SetParent(ComboBar_Obj.transform);
            ComboBarTextShadow = ComboBarTextShadow_Obj.AddComponent<TextMeshProUGUI>();
            ComboBarTextShadow.transform.localScale = Vector3.one;
            ComboBarTextShadow.rectTransform.anchoredPosition3D = new Vector3(10f, -60f, 0f);
            ComboBarTextShadow.rectTransform.sizeDelta = new Vector2(130f, 32f);
            ComboBarTextShadow.alignment = TextAlignmentOptions.Right;
            ComboBarTextShadow.fontSize = 24;
            ComboBarTextShadow.color = BasePlugin.GetComboTextShadowColor();
            ComboBarTextShadow.text = Other.GLT("Hud_Combo");

            GameObject ComboBarText_Obj = new GameObject("ComboBarText");
            ComboBarText_Obj.transform.SetParent(ComboBarTextShadow_Obj.transform);
            ComboBarText = ComboBarText_Obj.AddComponent<TextMeshProUGUI>();
            ComboBarText.transform.localScale = Vector3.one;
            ComboBarText.rectTransform.anchoredPosition3D = new Vector3(-1f, 1f, 0f);
            ComboBarText.rectTransform.sizeDelta = new Vector2(130f, 32f);
            ComboBarText.alignment = TextAlignmentOptions.Right;
            ComboBarText.fontSize = 24;
            ComboBarText.color = BasePlugin.GetComboTextColor();
            ComboBarText.text = ComboBarTextShadow.text;
        }
        public void UpdateComboBar()
        {
            float shouldXPos = math.round(120f * (BaldiTimeActions.comboTimer / BaldiTimeActions.comboTimerMax));
            if (BaldiTimeActions.comboTimer > 0 && BaldiTimeActions.combo > 0)
            {
                ComboBarTimer += Time.deltaTime;
                ComboBarTimer = math.min(0.75f, ComboBarTimer);
            }
            else
            {
                ComboBarTimer -= Time.deltaTime;
                ComboBarTimer = math.max(0f, ComboBarTimer);
                shouldXPos = 120f;
            }

            if (ComboBarNiddle.rectTransform.anchoredPosition3D.x < shouldXPos - 1)
            {
                ComboBarNiddle.rectTransform.anchoredPosition3D += new Vector3(1f, 0f, 0f);
            }
            else if (ComboBarNiddle.rectTransform.anchoredPosition3D.x > shouldXPos + 1)
            {
                ComboBarNiddle.rectTransform.anchoredPosition3D -= new Vector3(1f, 0f, 0f);
            }
            else
            {
                ComboBarNiddle.rectTransform.anchoredPosition3D = new Vector3(shouldXPos, 0f, 0f);
            }

            RectTransform ComboBar_Obj_RectTransform = ComboBar_Obj.GetOrAddComponent<RectTransform>();
            ComboBar_Obj_RectTransform.anchoredPosition3D = new Vector3(-96f, 96f - 192f * math.sin((ComboBarTimer / 0.75f) * math.PI / 2f), 0f);

            ComboBarTextShadow.text = string.Format(Other.GLT("Hud_Combo"), BaldiTimeActions.combo.ToString());
            //ComboBarTextShadow.text = BaldiTimeActions.combo + " " + Other.GLT("Hud_Combo");
            ComboBarText.text = ComboBarTextShadow.text;

            if (BaldiTimeActions.comboKeep)
            {
                ComboBarNiddle.sprite = BasePlugin.AssetMan.Get<Sprite>("ComboDisplay_3");
            }
            else
            {
                ComboBarNiddle.sprite = BasePlugin.AssetMan.Get<Sprite>("ComboDisplay_2");
            }
        }

        public IEnumerator ShowComboLevel()
        {
            if (BaldiTimeUI.AllComboLevels.Count <= 0)
            {
                yield break;
            }

            GameObject ComboLevel_Obj = new GameObject("ComboLevel");
            ComboLevel_Obj.transform.SetParent(gameObject.transform);
            ComboLevel_Obj.transform.SetAsFirstSibling();
            Image ComboLevel = ComboLevel_Obj.AddComponent<Image>();
            ComboLevel.rectTransform.anchorMax = Vector2.one;
            ComboLevel.rectTransform.anchorMin = Vector2.one;
            ComboLevel.rectTransform.anchoredPosition3D = new Vector3(96f, -216f, 0f);
            ComboLevel.rectTransform.sizeDelta = new Vector2(160f, 96f);
            ComboLevel.transform.localScale = Vector3.one;
            ComboLevel.sprite = BaldiTimeUI.AllComboLevels[Random.Range(0, BaldiTimeUI.AllComboLevels.Count - 1)];

            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("comboup" + Random.Range(1, 3).ToString()));

            float timer = 0f;
            while (timer < 3f)
            {
                timer += Time.deltaTime;
                float edit = math.sin(timer / 3f * math.PI);
                ComboLevel.rectTransform.anchoredPosition3D = new Vector3(96f - 192f * edit, -232f, 0f);
                yield return null;
            }

            Destroy(ComboLevel_Obj);
            yield break;
        }

        public IEnumerator YouFoundSecret()
        {
            GameObject FoundSecretTextShadow_Obj = new GameObject("FoundSecretTextShadow");
            FoundSecretTextShadow_Obj.transform.SetParent(transform.parent.transform);
            TextMeshProUGUI FoundSecretTextShadow = FoundSecretTextShadow_Obj.AddComponent<TextMeshProUGUI>();
            FoundSecretTextShadow.transform.localScale = Vector3.one;
            FoundSecretTextShadow.rectTransform.anchorMax = new Vector2(0.5f, 0f);
            FoundSecretTextShadow.rectTransform.anchorMin = new Vector2(0.5f, 0f);
            FoundSecretTextShadow.rectTransform.sizeDelta = new Vector2(400f, 240f);
            float i = FoundSecretTextShadow.rectTransform.sizeDelta.y / 2f;
            FoundSecretTextShadow.rectTransform.anchoredPosition3D = new Vector3(0f, -i, 0f);
            FoundSecretTextShadow.text = string.Format(Other.GLT("Hud_FoundSecret"), BaldiTimeActions.secretRoomFound.Count.ToString(), BaldiTimeActions.secretRoomTotal.ToString());
            FoundSecretTextShadow.fontSize = 24;
            FoundSecretTextShadow.alignment = TextAlignmentOptions.Top;
            FoundSecretTextShadow.color = Color.black;

            GameObject FoundSecretText_Obj = new GameObject("FoundSecretText");
            FoundSecretText_Obj.transform.SetParent(FoundSecretTextShadow_Obj.transform);
            TextMeshProUGUI FoundSecretText = FoundSecretText_Obj.AddComponent<TextMeshProUGUI>();
            FoundSecretText.transform.localScale = Vector3.one;
            FoundSecretText.rectTransform.anchoredPosition3D = new Vector3(-1f, 1f, 0f);
            FoundSecretText.rectTransform.sizeDelta = FoundSecretTextShadow.rectTransform.sizeDelta;
            FoundSecretText.text = FoundSecretTextShadow.text;
            FoundSecretText.fontSize = 24;
            FoundSecretText.alignment = TextAlignmentOptions.Top;
            FoundSecretText.color = Color.white;

            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("sfx_secretfound"));

            float timer = 0f;
            while (timer < 3f)
            {
                timer += Time.deltaTime;
                float edit = math.sin(timer / 3f * math.PI);
                FoundSecretTextShadow.rectTransform.anchoredPosition3D = new Vector3(0f, -i + i * edit, 0f);
                yield return null;
            }

            Destroy(FoundSecretTextShadow_Obj);
            yield break;
        }

        public void SetupWarTimer()
        {
            WarTimerBase_Obj = new GameObject("WarTimerBase");
            WarTimerBase_Obj.transform.SetParent(gameObject.transform);
            WarTimerBase_Obj.transform.localScale = Vector3.one;
            RectTransform WarTimer_Obj_RectTransform = WarTimerBase_Obj.GetOrAddComponent<RectTransform>();
            WarTimer_Obj_RectTransform.anchoredPosition3D = new Vector3(0f, -35f, 0f);
            WarTimer_Obj_RectTransform.anchorMax = new Vector2(0.5f, 0f);
            WarTimer_Obj_RectTransform.anchorMin = new Vector2(0.5f, 0f);

            GameObject WarTimer_Obj = new GameObject("WarTimer");
            WarTimer_Obj.transform.SetParent(WarTimerBase_Obj.transform);
            WarTimer = WarTimer_Obj.AddComponent<Image>();
            WarTimer.rectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
            WarTimer.rectTransform.sizeDelta = new Vector2(210f, 70f);
            WarTimer.transform.localScale = Vector3.one;
            WarTimer.sprite = BasePlugin.AssetMan.Get<Sprite>("WarTimer_0");

            GameObject WarTimerText_Obj = new GameObject("WarTimerText");
            WarTimerText_Obj.transform.SetParent(WarTimerBase_Obj.transform);
            WarTimerText = WarTimerText_Obj.AddComponent<TextMeshProUGUI>();
            WarTimerText.rectTransform.anchoredPosition3D = new Vector3(16f, -8f, 0f);
            WarTimerText.rectTransform.sizeDelta = new Vector2(165f, 55f);
            WarTimerText.transform.localScale = Vector3.one;
            WarTimerText.alignment = TextAlignmentOptions.Center;
            WarTimerText.font = UIExtensions.FontAsset(BaldiFonts.ComicSans36);
            WarTimerText.fontSize = 36f;
            WarTimerText.text = "0 0 : 0 0";
            WarTimerText.color = Color.red;
        }
        public void UpdateWarTimer()
        {
            string m = Mathf.Floor(BaldiTimeActions.warTimer / 60f).ToString("00");
            string n = (BaldiTimeActions.warTimer % 60f).ToString("00");

            WarTimerText.text = string.Format("{0} {1} : {2} {3}", m.Substring(0, 1), m.Substring(1, 1), n.Substring(0, 1), n.Substring(1, 1));

            if (BaldiTimeActions.warTimer > 30f)
            {
                WarTimer.sprite = BasePlugin.AssetMan.Get<Sprite>("WarTimer_0");
            }
            else
            {
                WarTimer.sprite = BasePlugin.AssetMan.Get<Sprite>("WarTimer_1");
            }
        }
        public IEnumerator WarTimerRaise()
        {
            RectTransform WarTimer_Obj_RectTransform = WarTimerBase_Obj.GetOrAddComponent<RectTransform>();

            float timer = 0f;
            while (timer < 1f)
            {
                yield return null;
                WarTimer_Obj_RectTransform.anchoredPosition3D = new Vector3(0f, -35f + 70f * timer, 0f);
                timer += Time.deltaTime;
            }

            WarTimer_Obj_RectTransform.anchoredPosition3D = new Vector3(0f, 35f, 0f);
            yield break;
        }
    }

    public class BaldiTimeUI
    {
        public static List<Sprite> AllComboLevels = new List<Sprite>();

        public static IEnumerator LappingAnimations(BaseGameManager baseGameManager)
        {
            HudManager hudManager = Singleton<CoreGameManager>.Instance.GetHud(0);
            if (BaldiTimeActions.lap == 1)
            {
                GameObject whiteFlash_Obj = new GameObject("WhiteFlash");
                whiteFlash_Obj.transform.SetParent(hudManager.transform, false);
                RawImage whiteFlash = whiteFlash_Obj.AddComponent<RawImage>();
                whiteFlash.color = new Color(1f, 1f, 1f, 1f);
                whiteFlash.rectTransform.anchorMin = new Vector2(0f, 0f);
                whiteFlash.rectTransform.anchorMax = Vector2.one;

                GameObject baldiTimeLogo_Obj = new GameObject("BaldiTimeLogo");
                baldiTimeLogo_Obj.transform.SetParent(hudManager.transform, false);
                Image baldiTimeLogoImage = baldiTimeLogo_Obj.AddComponent<Image>();
                baldiTimeLogoImage.sprite = BasePlugin.AssetMan.Get<Sprite>("BaldiTimeLogo_0");
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
                        Object.Destroy(whiteFlash_Obj);
                        Object.Destroy(baldiTimeLogo_Obj);
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
                            baldiTimeLogoImage.sprite = BasePlugin.AssetMan.Get<Sprite>("BaldiTimeLogo_0");
                        }
                        else
                        {
                            frame = 0;
                            baldiTimeLogoImage.sprite = BasePlugin.AssetMan.Get<Sprite>("BaldiTimeLogo_1");
                        }
                    }
                    timer += Time.deltaTime * Time.timeScale;
                    yield return null;
                }
                Object.Destroy(whiteFlash_Obj);
                Object.Destroy(baldiTimeLogo_Obj);
            }
            else if (BaldiTimeActions.lap >= 2)
            {
                GameObject LapFlag_Obj = new GameObject("LapFlag");
                LapFlag_Obj.transform.SetParent(hudManager.Canvas().transform, false);
                Image LapFlag = LapFlag_Obj.AddComponent<Image>();
                LapFlag.rectTransform.anchorMax = new Vector2(0.5f, 1f);
                LapFlag.rectTransform.anchorMin = new Vector2(0.5f, 1f);
                LapFlag.rectTransform.sizeDelta = new Vector2(190f, 90f);
                LapFlag.rectTransform.pivot = new Vector2(0.5f, 0f);
                LapFlag.sprite = BasePlugin.AssetMan.Get<Sprite>("Lap" + BaldiTimeActions.lap.ToString() + "Flag");//LapFlagSprites[BaldiTimeActions.lap - 2];

                float timer = 0f;
                while (timer < 2f)
                {
                    if (baseGameManager == null)
                    {
                        Object.Destroy(LapFlag_Obj);
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

        public static void UpdateCam(GameCamera gameCamera)
        {
            if (!BaldiTimeActions.itsBaldiTime)
            {
                return;
            }
            float lmd = BasePlugin.Instance.ConfigCameraShakeSpeed.Value;
            float sd = BasePlugin.Instance.ConfigCameraShakeIntensity.Value;
            if (BasePlugin.Instance.ConfigCameraShakeStyle.Value == CameraShakeStyle.Smooth)
            {
                float f0 = math.sin(Time.time * lmd * math.PI / 4f);
                gameCamera.transform.rotation *= Quaternion.Euler(0f, 0f, f0 * sd);
            }
            else if (BasePlugin.Instance.ConfigCameraShakeStyle.Value == CameraShakeStyle.Beat)
            {
                float f0 = math.abs(Time.time % lmd);
                float f1 = (Time.time - f0) / lmd;
                float f2 = f1 % 2f;
                float e0;
                if (f2 == 0)
                {
                    e0 = math.sin((f0 / lmd) * math.PI / 2f + math.PI) + 1f;
                }
                else
                {
                    e0 = math.sin((f0 / lmd) * math.PI / 2f) - 1f;
                }
                gameCamera.transform.rotation *= Quaternion.Euler(0f, 0f, e0 * sd);
            }
        }
    }
}
