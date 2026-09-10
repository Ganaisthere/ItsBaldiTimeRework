using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Reflection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace ItsBaldiTimeRework
{
    public class BaldiTimeAnimations
    {
        public static bool openingPlayed = false;
        public static AudioClip OpeningMusic;
        public static RawImage OpeningMain;
        public static Image[] Layer = new Image[10];
        public static Sprite[] Sprites_0 = new Sprite[3];
        public static Sprite[] Sprites_1 = new Sprite[2];
        public static Sprite[] Sprites_2 = new Sprite[6];
        public static Sprite[] Sprites_2_2 = new Sprite[2];
        public static Sprite[] Sprites_3 = new Sprite[4];
        public static Sprite[] Sprites_3_4 = new Sprite[2];
        public static Sprite[] Sprites_4 = new Sprite[4];
        public static Sprite[] Sprites_5 = new Sprite[4];
        public static Sprite[] Sprites_6 = new Sprite[5];
        public static Sprite[] Sprites_7 = new Sprite[8];
        public static Sprite[] Sprites_8 = new Sprite[7];
        public static Sprite[] Sprites_9 = new Sprite[7];
        public static Sprite[] Sprites_10 = new Sprite[3];
        public static Sprite[] Sprites_11 = new Sprite[3];
        public static Sprite[] Sprites_12 = new Sprite[5];
        public static Sprite[] Sprites_13 = new Sprite[2];
        public static Image Border;
        public static Sprite Border_Sprite;
        public static RawImage whiteFlash;
        //-----------------------------------------------------
        public static List<Sprite> TitleCardBackSprites = new List<Sprite>();
        public static List<Sprite> TitleCardTitleSprites = new List<Sprite>();
        public static List<SoundObject> TitleCardSounds = new List<SoundObject>();
        //-----------------------------------------------------
        public static Sprite StudentSprite;
        public static Sprite[] StudentSprites = new Sprite[7];
        public static SoundObject[] RankSounds = new SoundObject[7];
        public static Sprite[] RankSprites = new Sprite[7];

        public static void RankAnimationsButVoid(AudioManager audMan, ElevatorScreen elevatorScreen)
        {
            Singleton<MusicManager>.Instance.StartCoroutine(RankAnimations(audMan, elevatorScreen));
        }
        public static IEnumerator RankAnimations(AudioManager audMan, ElevatorScreen elevatorScreen)
        {
            while (elevatorScreen.transform.localScale.x <= 0f)
            {
                yield return null;
            }
            while (elevatorScreen.transform.localScale != Vector3.one)
            {
                yield return null;
            }

            GameObject WhiteBackGround_Obj = new GameObject("WhiteBackGround");
            WhiteBackGround_Obj.transform.SetParent(elevatorScreen.Canvas.transform, false);
            RawImage WhiteBackGround = WhiteBackGround_Obj.AddComponent<RawImage>();
            WhiteBackGround.rectTransform.sizeDelta = new Vector2(480f, 360f);
            WhiteBackGround.rectTransform.anchoredPosition = new Vector2(0f, 720f);
            WhiteBackGround.color = Color.white;

            GameObject Rank_Obj = new GameObject("Rank");
            Rank_Obj.transform.SetParent(elevatorScreen.Canvas.transform, false);
            Image Rank = Rank_Obj.AddComponent<Image>();
            Rank.rectTransform.sizeDelta = new Vector2(480f, 360f);
            Rank.rectTransform.anchoredPosition = new Vector2(960f, 0f);
            Rank.color = Color.white;

            GameObject Student_Obj = new GameObject("Student");
            Student_Obj.transform.SetParent(elevatorScreen.Canvas.transform, false);
            Image Student = Student_Obj.AddComponent<Image>();
            Student.rectTransform.sizeDelta = new Vector2(480f, 360f);
            Student.rectTransform.anchoredPosition = new Vector2(960f, 0f);
            Student.sprite = StudentSprite;
            Student.color = Color.white;

            GameObject Border_L_Obj = new GameObject("Border_L");
            Border_L_Obj.transform.SetParent(elevatorScreen.Canvas.transform, false);
            RawImage Border_L = Border_L_Obj.AddComponent<RawImage>();
            Border_L.rectTransform.sizeDelta = new Vector2(480f, 360f);
            Border_L.rectTransform.anchoredPosition = new Vector2(-480f, 0f);
            Border_L.color = Color.black;

            GameObject Border_R_Obj = new GameObject("Border_R");
            Border_R_Obj.transform.SetParent(elevatorScreen.Canvas.transform, false);
            RawImage Border_R = Border_R_Obj.AddComponent<RawImage>();
            Border_R.rectTransform.sizeDelta = new Vector2(480f, 360f);
            Border_R.rectTransform.anchoredPosition = new Vector2(480f, 0f);
            Border_R.color = Color.black;//95874046

            audMan.PlaySingle(RankSounds[BaldiTimeUI.numOld]);
            Rank.sprite = RankSprites[BaldiTimeUI.numOld];

            float timer = 0f;
            while (timer <= 14f)
            {
                if (elevatorScreen == null)
                {
                    yield break;
                }
                elevatorScreen.ReflectionSetVariable("busy", true);
                Singleton<MusicManager>.Instance.StopMidi();
                if (timer <= 1f)
                {
                    WhiteBackGround.rectTransform.anchoredPosition = new Vector2(0f, 360f - 360f * math.sin(timer * (math.PI / 2)));
                }
                else
                {
                    WhiteBackGround.rectTransform.anchoredPosition = new Vector2(0f, 0f);
                }

                if (timer <= 1f)
                {
                    Student.rectTransform.anchoredPosition = new Vector2(480f - 480f * math.sin(timer * (math.PI / 2)), 0f);
                }
                else if (timer <= 2f)
                {
                    Student.rectTransform.anchoredPosition = new Vector2(0f, 0f);
                }
                else if (timer <= 3f)
                {
                    Student.rectTransform.anchoredPosition = new Vector2(-480f * (1f + math.sin((timer + 1f) * (math.PI / 2))), 0f);
                }
                else if (timer <= 4f)
                {
                    Student.sprite = StudentSprites[BaldiTimeUI.numOld];
                    Student.rectTransform.anchoredPosition = new Vector2(-480f + 480f * math.sin((timer - 3f) * (math.PI / 2)), 0f);
                }
                else
                {
                    Student.rectTransform.anchoredPosition = new Vector2(0f, 0f);
                }

                if (timer <= 3f)
                {
                    Rank.rectTransform.anchoredPosition = new Vector2(480f, 0f);
                }
                else if (timer <= 4f)
                {
                    Rank.rectTransform.anchoredPosition = new Vector2(480f - 480f * math.sin((timer - 3f) * (math.PI / 2)), 0f);
                }
                else if (timer <= 9.3f)
                {
                    Rank.rectTransform.anchoredPosition = new Vector2(0f, 0f);
                }
                else
                {
                    Rank.rectTransform.anchoredPosition = new Vector2(0f, 0f);
                    Color color = new Color(1f, 0.5f, 0f, 1f);
                    WhiteBackGround.color = color;
                    Student.color = color;
                    Rank.color = color;
                }
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            Singleton<MusicManager>.Instance.PlayMidi("Elevator", true);
            timer = 0f;
            while (timer <= 1f)
            {
                timer += Time.unscaledDeltaTime;
                WhiteBackGround.rectTransform.anchoredPosition = new Vector2(0f, -360f * math.sin(timer * (math.PI / 2)));
                Student.rectTransform.anchoredPosition = new Vector2(0f, -360f * math.sin(timer * (math.PI / 2)));
                Rank.rectTransform.anchoredPosition = new Vector2(0f, -360f * math.sin(timer * (math.PI / 2)));
                yield return null;
            }
            Object.Destroy(WhiteBackGround_Obj);
            Object.Destroy(Student_Obj);
            Object.Destroy(Rank_Obj);
            Object.Destroy(Border_L_Obj);
            Object.Destroy(Border_R_Obj);

            List<IEnumerator> queuedEnumerators = elevatorScreen.ReflectionGetVariable("queuedEnumerators") as List<IEnumerator>;
            queuedEnumerators.Clear();
            elevatorScreen.ReflectionSetVariable("busy", false);

            yield break;
        }
        public static void TitleCardAnimationsButVoid(Canvas canvas, AudioManager audMan, ElevatorScreen elevatorScreen)
        {
            Singleton<MusicManager>.Instance.StartCoroutine(TitleCardAnimations(canvas, audMan, elevatorScreen));
        }
        public static IEnumerator TitleCardAnimations(Canvas canvas, AudioManager audMan, ElevatorScreen elevatorScreen)
        {
            int randomInt;
            Debug.LogWarning("TitleCardBackSprites.Count = " + TitleCardBackSprites.Count);
            Debug.LogWarning("TitleCardTitleSprites.Count = " + TitleCardTitleSprites.Count);
            Debug.LogWarning("TitleCardSounds.Count = " + TitleCardSounds.Count);

            while (elevatorScreen.transform.localScale.x <= 0f)
            {
                yield return null;
            }
            while (elevatorScreen.transform.localScale != Vector3.one)
            {
                yield return null;
            }

            GameObject TitleCardMain_Obj = new GameObject("TitleCardMain");
            TitleCardMain_Obj.transform.SetParent(canvas.transform, false);
            RawImage TitleCardMain = TitleCardMain_Obj.AddComponent<RawImage>();
            TitleCardMain.color = new Color(0f, 0f, 0f, 0f);
            TitleCardMain.rectTransform.sizeDelta = new Vector2(480f, 360f);

            GameObject TitleCardBack_Obj = new GameObject("TitleCardBack");
            TitleCardBack_Obj.transform.SetParent(TitleCardMain.transform, false);
            Image TitleCardBack = TitleCardBack_Obj.AddComponent<Image>();
            TitleCardBack.rectTransform.sizeDelta = new Vector2(480f, 360f);
            randomInt = UnityEngine.Random.Range(0, TitleCardBackSprites.Count - 1);
            TitleCardBack.sprite = TitleCardBackSprites[randomInt];

            GameObject TitleCardTitle_Obj = new GameObject("TitleCardTitle");
            TitleCardTitle_Obj.transform.SetParent(TitleCardMain.transform, false);
            Image TitleCardTitle = TitleCardTitle_Obj.AddComponent<Image>();
            TitleCardTitle.rectTransform.sizeDelta = new Vector2(480f, 360f);
            if (TitleCardTitleSprites[randomInt] != null)
            {
                TitleCardTitle.sprite = TitleCardTitleSprites[randomInt];
            }
            else
            {
                TitleCardTitle.color = new Color(1f, 1f, 1f, 0f);
            }

            GameObject Flash_Obj = new GameObject("Flash");
            Flash_Obj.transform.SetParent(TitleCardMain.transform, false);
            RawImage Flash = Flash_Obj.AddComponent<RawImage>();
            Flash.color = new Color(0f, 0f, 0f, 0f);
            Flash.rectTransform.anchorMin = new Vector2(0f, 0f);
            Flash.rectTransform.anchorMax = new Vector2(1f, 1f);

            if (TitleCardSounds[randomInt] != null)
            {
                audMan.PlaySingle(TitleCardSounds[randomInt]);
            }

            float timer = 0f;
            float timer0 = 0f;
            while (timer < 4f)
            {
                if (elevatorScreen == null)
                {
                    yield break;
                }
                elevatorScreen.ReflectionSetVariable("busy", true);
                Singleton<MusicManager>.Instance.StopMidi();
                if (timer < 3f)
                {
                    Flash.color = new Color(0f, 0f, 0f, 0f);
                }
                else
                {
                    Flash.color = new Color(0f, 0f, 0f, timer - 3f);
                }

                if (timer < 1.5f)
                {
                    float edit = math.sin((timer / 1.5f) * (math.PI / 2));
                    TitleCardBack.rectTransform.anchoredPosition = new Vector2(480f - 480f * edit, 0f);
                    TitleCardTitle.rectTransform.anchoredPosition = new Vector2(480f - 480f * edit, 0f);
                }
                else
                {
                    TitleCardBack.rectTransform.anchoredPosition = new Vector2(0f, 0f);
                    TitleCardTitle.rectTransform.anchoredPosition = new Vector2(0f, 0f);
                }

                if (timer0 < 0.1f)
                {
                    timer0 += Time.unscaledDeltaTime;
                }
                else
                {
                    timer0 = 0f;
                    float randomFloat0 = UnityEngine.Random.Range(-2f, 2f);
                    float randomFloat1 = UnityEngine.Random.Range(-2f, 2f);
                    TitleCardTitle.rectTransform.anchoredPosition = new Vector2(TitleCardTitle.rectTransform.anchoredPosition.x + randomFloat0, TitleCardTitle.rectTransform.anchoredPosition.y + randomFloat1);
                }

                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            Object.Destroy(TitleCardBack_Obj);
            Object.Destroy(TitleCardTitle_Obj);

            List<IEnumerator> queuedEnumerators = elevatorScreen.ReflectionGetVariable("queuedEnumerators") as List<IEnumerator>;
            queuedEnumerators.Clear();
            elevatorScreen.ReflectionSetVariable("busy", false);

            //elevatorScreen.ReflectionSetVariable("readyToStart", true);
            //audMan.PlaySingle(AssetFinder.FindOfTypeWithName<SoundObject>("Elv_Buzz", true));
            //elevatorScreen.UpdateFloorDisplay();
            //elevatorScreen.ReflectionGetVariable("UpdateLives");
            //elevatorScreen.StartGame();

            timer = 0f;
            while (timer < 1f)
            {
                Flash.color = new Color(0f, 0f, 0f, 1f - timer);
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            Object.Destroy(Flash_Obj);

            yield break;
        }
        public static void OpeningAnimationsButVoid(Canvas canvas, AudioSource audSource, TMP_Text textBox, MonoBehaviour monoBehaviour)
        {
            monoBehaviour.StartCoroutine(OpeningAnimations(canvas, audSource, textBox, monoBehaviour));
            monoBehaviour.StartCoroutine(Skip(canvas, audSource, textBox, monoBehaviour));
        }

        public static IEnumerator Skip(Canvas canvas, AudioSource audSource, TMP_Text textBox, MonoBehaviour monoBehaviour)
        {
            while (!(Input.anyKeyDown || Singleton<InputManager>.Instance.GetDigitalInput("MouseSubmit", onDown: true) || Singleton<InputManager>.Instance.GetDigitalInput("Pause", onDown: true) || Singleton<InputManager>.Instance.AnyButton(onDown: true)))
            {
                if (openingPlayed)
                {
                    yield break;
                }
                yield return null;
            }
            while (Input.anyKeyDown || Singleton<InputManager>.Instance.GetDigitalInput("MouseSubmit", onDown: true) || Singleton<InputManager>.Instance.GetDigitalInput("Pause", onDown: true) || Singleton<InputManager>.Instance.AnyButton(onDown: true))
            {
                if (openingPlayed)
                {
                    yield break;
                }
                yield return null;
            }
            monoBehaviour.StopCoroutine(OpeningAnimations(canvas, audSource, textBox, monoBehaviour));
            Object.Destroy(OpeningMain.gameObject);
            Object.Destroy(Border.gameObject);
            Object.Destroy(whiteFlash.gameObject);
            openingPlayed = true;
            textBox.gameObject.SetActive(true);
            audSource.Stop();
            audSource.clip = AssetFinder.FindOfTypeWithName<AudioClip>("ErrorScreen", true);
            audSource.loop = true;
            audSource.Play();

            yield break;
        }
        public static IEnumerator OpeningAnimations(Canvas canvas, AudioSource audSource, TMP_Text textBox, MonoBehaviour monoBehaviour)
        {
            openingPlayed = false;
            audSource.clip = OpeningMusic;
            audSource.loop = false;
            audSource.Play();

            GameObject OpeningMain_Obj = new GameObject("OpeningMain");
            OpeningMain_Obj.transform.SetParent(canvas.transform, false);
            OpeningMain = OpeningMain_Obj.AddComponent<RawImage>();
            OpeningMain.color = new Color(0f, 0f, 0f, 0f);
            OpeningMain.rectTransform.sizeDelta = new Vector2(640f, 360f);

            for (int i = 0; i < Layer.Length; i++)
            {
                GameObject Layer_Obj = new GameObject("Layer_" + i.ToString());
                Layer_Obj.transform.SetParent(OpeningMain.transform, false);
                Layer[i] = Layer_Obj.AddComponent<Image>();
                Layer[i].rectTransform.sizeDelta = new Vector2(640f, 360f);
                Layer[i].color = new Color(1f, 1f, 1f, 0f);
            }

            GameObject Border_Obj = new GameObject("Border");
            Border_Obj.transform.SetParent(canvas.transform, false);
            Border = Border_Obj.AddComponent<Image>();
            Border.rectTransform.sizeDelta = new Vector2(640f, 520f);
            Border.sprite = Border_Sprite;

            GameObject Flash_Obj = new GameObject("Flash");
            Flash_Obj.transform.SetParent(OpeningMain.transform, false);
            whiteFlash = Flash_Obj.AddComponent<RawImage>();
            whiteFlash.color = new Color(0f, 0f, 0f, 1f);
            whiteFlash.rectTransform.anchorMin = new Vector2(0f, 0f);
            whiteFlash.rectTransform.anchorMax = new Vector2(1f, 1f);

            Layer[0].sprite = Sprites_0[0];
            Layer[0].color = new Color(1f, 1f, 1f, 1f);
            Layer[1].sprite = Sprites_0[1];
            Layer[1].color = new Color(1f, 1f, 1f, 1f);
            Layer[2].sprite = Sprites_0[2];
            Layer[2].color = new Color(1f, 1f, 1f, 1f);

            float timer = 0f;
            while (timer < 6f)
            {
                whiteFlash.color = new Color(0f, 0f, 0f, math.max(1f - timer, 0f));

                float edit = math.sin(timer / 6f * (math.PI / 2));

                Layer[0].rectTransform.anchoredPosition = new Vector2(-80f + 160f * (timer / 6f), 0f);
                Layer[1].rectTransform.anchoredPosition = new Vector2(80f - 80f * edit, 0f);
                Layer[2].rectTransform.anchoredPosition = new Vector2(-80f + 80f * edit, 0f);

                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            whiteFlash.color = new Color(0f, 0f, 0f, 0f);

            Layer[0].sprite = Sprites_1[0];
            Layer[1].sprite = Sprites_1[1];
            Layer[2].color = new Color(1f, 1f, 1f, 0f);

            timer = 0f;
            while (timer < 3f)
            {
                float edit = math.sin(timer / 3f * (math.PI / 2));

                Layer[0].rectTransform.anchoredPosition = new Vector2(0f, 0f);
                Layer[1].rectTransform.anchoredPosition = new Vector2(-40f + 40f * edit, 0f);

                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            Layer[0].sprite = Sprites_2[0];
            Layer[1].sprite = Sprites_2[1];
            Layer[2].color = new Color(1f, 1f, 1f, 1f);
            Layer[2].sprite = Sprites_2[2];
            Layer[3].color = new Color(1f, 1f, 1f, 1f);
            Layer[3].sprite = Sprites_2[3];

            timer = 0f;
            float timer0 = 0f;
            while (timer < 3f)
            {
                Layer[0].rectTransform.anchoredPosition = new Vector2(0f + 80f * (timer / 3f), 0f);
                Layer[1].rectTransform.anchoredPosition = new Vector2(0f, 0f);
                Layer[2].rectTransform.anchoredPosition = new Vector2(0f, 0f);
                Layer[3].rectTransform.anchoredPosition = new Vector2(0f, 0f);

                if (timer0 < 1f)
                {
                    if (timer0 > 0.5f)
                    {
                        Layer[2].sprite = Sprites_2_2[1];
                    }
                    else if (timer0 > 0.5f)
                    {
                        Layer[2].sprite = Sprites_2_2[0];
                    }
                    else
                    {
                        Layer[2].sprite = Sprites_2[2];
                    }
                    timer0 += Time.unscaledDeltaTime;
                }
                else
                {
                    timer0 = 0f;
                }

                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            Layer[0].sprite = Sprites_3[0];
            Layer[1].sprite = Sprites_3[1];
            Layer[2].sprite = Sprites_3[2];
            Layer[3].sprite = Sprites_3[3];
            Layer[4].color = new Color(1f, 1f, 1f, 1f);
            Layer[4].sprite = Sprites_3_4[0];

            timer = 0f;
            timer0 = 0f;
            while (timer < 2f)
            {
                float edit = math.sin(timer / 2f * (math.PI / 2));
                float edit2 = math.sin(timer * (math.PI / 2));
                if (timer >= 1f)
                {
                    edit2 = 1f;
                }

                Layer[0].rectTransform.anchoredPosition = new Vector2(0f, 0f);
                Layer[1].rectTransform.anchoredPosition = new Vector2(0f + 80f * edit, -360f + 360f * edit2);
                Layer[2].rectTransform.anchoredPosition = new Vector2(0f - 80f * edit, -360f + 360f * edit2);
                Layer[3].rectTransform.anchoredPosition = new Vector2(0f, -360f + 360f * edit2);
                Layer[4].rectTransform.anchoredPosition = new Vector2(0f, 0f);

                if (timer0 < 0.2f)
                {
                    if (timer0 > 0.1f)
                    {
                        Layer[4].sprite = Sprites_3_4[0];
                    }
                    else
                    {
                        Layer[4].sprite = Sprites_3_4[1];
                    }
                    timer0 += Time.unscaledDeltaTime;
                }
                else
                {
                    timer0 = 0f;
                }

                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            Layer[0].sprite = Sprites_3[0];
            Layer[1].sprite = Sprites_4[1];
            Layer[2].color = new Color(1f, 1f, 1f, 0f);
            Layer[3].color = new Color(1f, 1f, 1f, 0f);
            Layer[4].color = new Color(1f, 1f, 1f, 0f);

            timer = 0f;
            timer0 = 0f;
            while (timer < 4f)
            {
                float edit = math.sin(timer * (math.PI / 2));
                if (timer >= 1f)
                {
                    edit = 1f;
                }

                Layer[0].rectTransform.anchoredPosition = new Vector2(0f, 0f);

                Layer[0].sprite = Sprites_3[0];
                Layer[1].sprite = Sprites_4[1];
                if (timer >= 2f)
                {
                    Layer[0].sprite = Sprites_4[0];
                    Layer[1].sprite = Sprites_4[2];
                    timer0 += Time.unscaledDeltaTime;
                    edit = math.sin(timer0 * math.PI * 2f);
                    Layer[1].rectTransform.anchoredPosition = new Vector2(0f, edit * 12f);
                }
                else
                {
                    Layer[1].rectTransform.anchoredPosition = new Vector2(0f, -360f + 360f * edit);
                }

                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            Layer[0].sprite = Sprites_5[0];
            Layer[1].sprite = Sprites_5[1];
            Layer[2].color = new Color(1f, 1f, 1f, 1f);
            Layer[2].sprite = Sprites_5[2];
            Layer[3].color = new Color(1f, 1f, 1f, 1f);
            Layer[3].sprite = Sprites_5[3];

            timer = 0f;
            while (timer < 3f)
            {
                float edit = math.sin(timer * math.PI * 2);

                Layer[1].rectTransform.rotation = Quaternion.Euler(0f, 0f, timer * 360);
                Layer[2].rectTransform.anchoredPosition = new Vector2(edit * 8f, 0f);
                Layer[3].rectTransform.anchoredPosition = new Vector2(0f, edit * 4f);

                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            Layer[1].rectTransform.rotation = Quaternion.Euler(0f, 0f, 0f);

            Layer[0].sprite = Sprites_6[0];
            Layer[1].sprite = Sprites_6[1];
            Layer[2].sprite = Sprites_6[2];
            Layer[3].sprite = Sprites_6[3];
            Layer[4].color = new Color(1f, 1f, 1f, 1f);
            Layer[4].sprite = Sprites_6[4];

            timer = 0f;
            while (timer < 3f)
            {
                float edit = math.sin(timer * math.PI * 2);

                Layer[1].rectTransform.rotation = Quaternion.Euler(0f, 0f, timer * 360);
                Layer[3].rectTransform.anchoredPosition = new Vector2(edit * 8f, 0f);
                Layer[4].rectTransform.anchoredPosition = new Vector2(0f, edit * 2f);

                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            Layer[1].rectTransform.rotation = Quaternion.Euler(0f, 0f, 0f);

            Layer[0].sprite = Sprites_7[0];
            Layer[1].sprite = Sprites_7[1];
            Layer[2].sprite = Sprites_7[2];
            Layer[3].color = new Color(1f, 1f, 1f, 0f);
            Layer[4].color = new Color(1f, 1f, 1f, 0f);

            yield return new WaitForSecondsRealtime(0.75f);
            Layer[1].sprite = Sprites_7[3];
            yield return new WaitForSecondsRealtime(0.75f);
            Layer[1].sprite = Sprites_7[4];
            yield return new WaitForSecondsRealtime(0.75f);
            Layer[1].sprite = Sprites_7[5];
            yield return new WaitForSecondsRealtime(0.75f);
            Layer[1].sprite = Sprites_7[6];
            Layer[2].sprite = Sprites_7[7];
            yield return new WaitForSecondsRealtime(2f);

            timer = 0f;
            while (timer < 1f)
            {
                whiteFlash.color = new Color(1f, 1f, 1f, timer);
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            Layer[0].sprite = Sprites_8[0];
            Layer[1].sprite = Sprites_8[1];
            Layer[1].rectTransform.anchoredPosition = new Vector2(80f, 0f);
            Layer[2].sprite = Sprites_8[2];
            Layer[3].color = new Color(1f, 1f, 1f, 1f);
            Layer[3].sprite = Sprites_8[6];

            timer = 0f;
            timer0 = 0f;
            int idk = 0;
            while (timer < 6f)
            {
                if (timer <= 1f)
                {
                    whiteFlash.color = new Color(1f, 1f, 1f, 1f - timer);
                }
                else if (timer <= 5f)
                {
                    whiteFlash.color = new Color(1f, 1f, 1f, 0f);
                }
                else
                {
                    whiteFlash.color = new Color(185f / 255f, 30f / 225f, 125f / 225f, timer - 5f);
                }

                Layer[0].rectTransform.anchoredPosition = new Vector2(80f - timer / 6f * 120f, 0f);
                if (Layer[1].rectTransform.anchoredPosition.x - Time.unscaledDeltaTime > -80f)
                {
                    Layer[1].rectTransform.anchoredPosition = new Vector2(Layer[1].rectTransform.anchoredPosition.x - Time.unscaledDeltaTime * 30f, 0f);
                }
                else
                {
                    Layer[1].rectTransform.anchoredPosition = new Vector2(80f, 0f);
                }

                if (timer < 2f)
                {
                    Layer[2].rectTransform.anchoredPosition = new Vector2(-640f, 0f);
                }
                else if (timer < 3f)
                {
                    float edit = math.sin((timer - 2f) * (math.PI / 2));
                    Layer[2].rectTransform.anchoredPosition = new Vector2(-640f + 640f * edit, 0f);
                }
                else if (timer < 5f)
                {
                    Layer[2].rectTransform.anchoredPosition = new Vector2(0f, 0f);
                }
                else
                {
                    float edit = math.sin((timer - 2f) * (math.PI / 2));
                    Layer[2].rectTransform.anchoredPosition = new Vector2(640f + 640f * edit, 0f);
                }

                if (timer < 1f)
                {
                    Layer[3].rectTransform.anchoredPosition = new Vector2(-640f, Layer[3].rectTransform.anchoredPosition.y);
                }
                else if (timer < 2f)
                {
                    float edit = math.sin((timer - 1f) * (math.PI / 2));
                    Layer[3].rectTransform.anchoredPosition = new Vector2(-640f + 640f * edit, Layer[3].rectTransform.anchoredPosition.y);
                }
                else if (timer < 5f)
                {
                    Layer[3].rectTransform.anchoredPosition = new Vector2(0f, Layer[3].rectTransform.anchoredPosition.y);
                }
                else
                {
                    float edit = math.sin((timer - 2f) * (math.PI / 2));
                    Layer[3].rectTransform.anchoredPosition = new Vector2(640f + 640f * edit, Layer[3].rectTransform.anchoredPosition.y);
                }
                Layer[3].rectTransform.anchoredPosition = new Vector2(Layer[3].rectTransform.anchoredPosition.x, math.sin(timer * math.PI * 2) * 8f);

                if (timer0 < 0.05f)
                {
                    timer0 += Time.unscaledDeltaTime;
                }
                else
                {
                    timer0 = 0f;
                    if (idk == 0)
                    {
                        idk = 1;
                    }
                    else if (idk == 1)
                    {
                        idk = 2;
                    }
                    else if (idk == 2)
                    {
                        idk = 3;
                    }
                    else
                    {
                        idk = 0;
                    }
                    Layer[2].sprite = Sprites_8[2 + idk];
                }

                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            whiteFlash.color = new Color(1f, 1f, 1f, 0f);

            Layer[0].sprite = Sprites_9[0];
            Layer[1].sprite = Sprites_9[1];
            Layer[2].sprite = Sprites_9[2];
            Layer[3].sprite = Sprites_9[3];
            Layer[4].color = new Color(1f, 1f, 1f, 1f);
            Layer[4].sprite = Sprites_9[4];
            Layer[5].color = new Color(1f, 1f, 1f, 1f);
            Layer[5].sprite = Sprites_9[5];
            Layer[6].color = new Color(1f, 1f, 1f, 1f);
            Layer[6].sprite = Sprites_9[6];

            timer = 0f;
            while (timer < 6f)
            {
                if (timer < 1f)
                {
                    float edit = math.sin(timer * (math.PI / 2));
                    Layer[1].rectTransform.anchoredPosition = new Vector2(0f, -360f + 360f * edit);
                }
                else
                {
                    Layer[1].rectTransform.anchoredPosition = new Vector2(0f, 0f);
                }

                if (timer < 0.5f)
                {
                    Layer[2].rectTransform.anchoredPosition = new Vector2(0f, 360f);
                }
                else if (timer < 1.5f)
                {
                    float edit = math.sin((timer - 0.5f) * (math.PI / 2));
                    Layer[2].rectTransform.anchoredPosition = new Vector2(0f, 360f - 360f * edit);
                }
                else
                {
                    Layer[2].rectTransform.anchoredPosition = new Vector2(0f, 0f);
                }

                if (timer < 1f)
                {
                    Layer[3].rectTransform.anchoredPosition = new Vector2(0f, -360f);
                }
                else if (timer < 2f)
                {
                    float edit = math.sin((timer - 1f) * (math.PI / 2));
                    Layer[3].rectTransform.anchoredPosition = new Vector2(0f, -360f + 360f * edit);
                }
                else
                {
                    Layer[3].rectTransform.anchoredPosition = new Vector2(0f, 0f);
                }

                if (timer < 2f)
                {
                    Layer[4].rectTransform.anchoredPosition = new Vector2(640f, 0f);
                }
                else if (timer < 3f)
                {
                    float edit = math.sin((timer - 2f) * (math.PI / 2));
                    Layer[4].rectTransform.anchoredPosition = new Vector2(640f - 640f * edit, 0f);
                }
                else
                {
                    Layer[4].rectTransform.anchoredPosition = new Vector2(0f, 0f);
                }

                if (timer < 3f)
                {
                    Layer[5].rectTransform.anchoredPosition = new Vector2(0f, -360f);
                }
                else if (timer < 4f)
                {
                    float edit = math.sin((timer - 3f) * (math.PI / 2));
                    Layer[5].rectTransform.anchoredPosition = new Vector2(0f, -360f + 360f * edit);
                }
                else
                {
                    Layer[5].rectTransform.anchoredPosition = new Vector2(0f, 0f);
                }

                if (timer < 5f)
                {
                    Layer[6].rectTransform.localScale = new Vector3(0f, 0f, 1f);
                }
                else
                {
                    Layer[6].rectTransform.localScale = new Vector3((timer - 5f) * 7f, (timer - 5f) * 7f, 1f);
                }

                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            timer = 0f;
            while (timer < 1f)
            {
                whiteFlash.color = new Color(1f, 1f, 1f, timer);
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            Layer[6].rectTransform.localScale = Vector3.one;

            Layer[0].sprite = Sprites_10[0];
            Layer[1].sprite = Sprites_10[1];
            Layer[2].sprite = Sprites_10[2];
            Layer[3].color = new Color(1f, 1f, 1f, 0f);
            Layer[4].color = new Color(1f, 1f, 1f, 0f);
            Layer[5].color = new Color(1f, 1f, 1f, 0f);
            Layer[6].color = new Color(1f, 1f, 1f, 0f);

            timer = 0f;
            while (timer < 2.5f)
            {
                if (timer < 1f)
                {
                    whiteFlash.color = new Color(1f, 1f, 1f, 1f - timer);
                }
                else
                {
                    whiteFlash.color = new Color(0f, 0f, 0f, 0f);
                }
                Layer[0].rectTransform.anchoredPosition = new Vector2(0f, 0f);
                Layer[1].rectTransform.anchoredPosition = new Vector2(-20f + 40f * timer / 3f, 0f);
                Layer[2].rectTransform.anchoredPosition = new Vector2(20f - 40f * timer / 3f, 0f);
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            Layer[0].sprite = Sprites_11[0];
            Layer[1].sprite = Sprites_11[1];
            Layer[2].sprite = Sprites_11[2];

            timer = 0f;
            while (timer < 2.5f)
            {
                Layer[0].rectTransform.anchoredPosition = new Vector2(0f, 0f);
                Layer[1].rectTransform.anchoredPosition = new Vector2(-20f + 40f * timer / 3f, 0f);
                Layer[2].rectTransform.anchoredPosition = new Vector2(20f - 40f * timer / 3f, 0f);
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            Layer[0].sprite = Sprites_12[0];
            Layer[1].sprite = Sprites_12[1];
            Layer[2].sprite = Sprites_12[2];
            Layer[3].color = new Color(1f, 1f, 1f, 1f);
            Layer[3].sprite = Sprites_12[3];
            Layer[4].color = new Color(1f, 1f, 1f, 1f);
            Layer[4].sprite = Sprites_12[4];

            timer = 0f;
            while (timer < 5f)
            {
                Layer[0].rectTransform.anchoredPosition = new Vector2(0f, 0f);

                if (timer < 1f)
                {
                    float edit = math.sin((timer - 0f) * (math.PI / 2));
                    Layer[1].rectTransform.anchoredPosition = new Vector2(0f, 360f - 360f * edit);
                }
                else
                {
                    Layer[1].rectTransform.anchoredPosition = new Vector2(0f, 0f);
                }

                if (timer < 1f)
                {
                    Layer[2].rectTransform.anchoredPosition = new Vector2(0f, -360f);
                }
                else if (timer < 2f)
                {
                    float edit = math.sin((timer - 1f) * (math.PI / 2));
                    Layer[2].rectTransform.anchoredPosition = new Vector2(0f, -360f + 360f * edit);
                }
                else
                {
                    Layer[2].rectTransform.anchoredPosition = new Vector2(0f, 0f);
                }

                if (timer < 2f)
                {
                    Layer[3].rectTransform.anchoredPosition = new Vector2(0f, 360f);
                }
                else if (timer < 3f)
                {
                    float edit = math.sin((timer - 2f) * (math.PI / 2));
                    Layer[3].rectTransform.anchoredPosition = new Vector2(0f, 360f - 360f * edit);
                }
                else
                {
                    Layer[3].rectTransform.anchoredPosition = new Vector2(0f, 0f);
                }

                if (timer < 3f)
                {
                    Layer[4].rectTransform.anchoredPosition = new Vector2(0f, -360f);
                }
                else if (timer < 4f)
                {
                    float edit = math.sin((timer - 3f) * (math.PI / 2));
                    Layer[4].rectTransform.anchoredPosition = new Vector2(0f, -360f + 360f * edit);
                }
                else
                {
                    Layer[4].rectTransform.anchoredPosition = new Vector2(0f, 0f);
                }

                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            timer = 0f;
            while (timer < 1f)
            {
                whiteFlash.color = new Color(1f, 1f, 1f, timer);
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            whiteFlash.color = new Color(1f, 1f, 1f, 0f);

            Layer[0].sprite = Sprites_13[0];
            Layer[1].sprite = Sprites_13[1];
            Layer[2].color = new Color(1f, 1f, 1f, 0f);
            Layer[3].color = new Color(1f, 1f, 1f, 0f);
            Layer[4].color = new Color(1f, 1f, 1f, 0f);

            timer = 0f;
            while (timer < 1f)
            {
                Layer[0].rectTransform.anchoredPosition = new Vector2(0f, 0f);
                Layer[0].rectTransform.localScale = Vector3.one;

                Layer[1].rectTransform.localScale = new Vector3(4f - 3f * timer, 4f - 3f * timer, 1f);

                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            Layer[1].rectTransform.localScale = Vector3.one;

            yield return new WaitForSecondsRealtime(3f);

            timer = 0f;
            while (timer < 3f)
            {
                whiteFlash.color = new Color(0f, 0f, 0f, timer / 3f);
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            whiteFlash.color = new Color(1f, 1f, 1f, 0f);
            Object.Destroy(OpeningMain_Obj);
            Object.Destroy(Border_Obj);
            Object.Destroy(Flash_Obj);

            yield return new WaitForSecondsRealtime(1f);

            openingPlayed = true;
            textBox.gameObject.SetActive(true);
            audSource.Stop();
            audSource.clip = AssetFinder.FindOfTypeWithName<AudioClip>("ErrorScreen", true);
            audSource.loop = true;
            audSource.Play();
            monoBehaviour.StopCoroutine(Skip(canvas, audSource, textBox, monoBehaviour));

            yield break;
        }
    }
}
