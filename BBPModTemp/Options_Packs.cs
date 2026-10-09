using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

namespace ItsBaldiTimeRework
{
    public class Options_Packs : CustomOptionsCategory
    {
        public StandardMenuButton[] PackTextsButton = new StandardMenuButton[4];
        public TextMeshProUGUI PageText;
        public StandardMenuButton[] SwitchButton = new StandardMenuButton[2];
        public StandardMenuButton ApplyButton;
        public TextMeshProUGUI WarningText;
        public int Page = 1;
        public int PageMax = 1;
        public static List<string> LoadedPacksOld = new List<string>();

        public override void Build()
        {
            if (BasePlugin.AllPacks.Count <= 0)
            {
                return;
            }

            Page = 1;
            UpdatePageMax();

            //BasePlugin.Instance.Log(BasePlugin.AllPacks.Count);
            //BasePlugin.Instance.Log(BasePlugin.AllPacks.Count / 4);
            //BasePlugin.Instance.Log(math.floor(BasePlugin.AllPacks.Count / 4));
            //BasePlugin.Instance.Log(PageMax);

            LoadedPacksOld.Clear();
            LoadedPacksOld.AddRange(BasePlugin.LoadedPacks);

            PackTextsButton[0] = CreateTextButton(
                PackTextAction0,
                "PackText0",
                GetPackNameOnSetup(0),
                new Vector3(0f, 45f, 0f),
                BaldiFonts.ComicSans24,
                TextAlignmentOptions.Left,
                new Vector2(300f, 32f),
                Color.black);

            PackTextsButton[1] = CreateTextButton(
                PackTextAction1,
                "PackText1",
                GetPackNameOnSetup(1),
                new Vector3(0f, 5f, 0f),
                BaldiFonts.ComicSans24,
                TextAlignmentOptions.Left,
                new Vector2(300f, 32f),
                Color.black);

            PackTextsButton[2] = CreateTextButton(
                PackTextAction2,
                "PackText2",
                GetPackNameOnSetup(2),
                new Vector3(0f, -35f, 0f),
                BaldiFonts.ComicSans24,
                TextAlignmentOptions.Left,
                new Vector2(300f, 32f),
                Color.black);

            PackTextsButton[3] = CreateTextButton(
                PackTextAction3,
                "PackText3",
                GetPackNameOnSetup(3),
                new Vector3(0f, -75f, 0f),
                BaldiFonts.ComicSans24,
                TextAlignmentOptions.Left,
                new Vector2(300f, 32f),
                Color.black);

            PageText = CreateText(
                "PageText",
                Page.ToString() + "/" + PageMax.ToString(),
                new Vector3(0f, -120f, 0f),
                BaldiFonts.ComicSans24,
                TextAlignmentOptions.Center,
                new Vector2(420f, 360f),
                Color.black);

            SwitchButton[0] = CreateButton(
                SwitchButtonLeftAction,
                base.menuArrowLeft,
                base.menuArrowLeftHighlight,
                "SwitchButtonLeft",
                new Vector3(-40f, -120f, 0f));

            SwitchButton[1] = CreateButton(
                SwitchButtonRightAction,
                base.menuArrowRight,
                base.menuArrowRightHighlight,
                "SwitchButtonRight",
                new Vector3(40f, -120f, 0f));

            ApplyButton = CreateApplyButton(ApplyActions);
            if (Singleton<CoreGameManager>.Instance == null)
            {
                ApplyButton.audConfirmOverride = AssetFinder.FindOfTypeWithName<SoundObject>("Activity_Correct", true);
            }
            else
            {
                foreach (StandardMenuButton button in PackTextsButton)
                {
                    button.audConfirmOverride = AssetFinder.FindOfTypeWithName<SoundObject>("Activity_Incorrect", true);
                }
                ApplyButton.audConfirmOverride = AssetFinder.FindOfTypeWithName<SoundObject>("Activity_Incorrect", true);
            }

            if (Singleton<CoreGameManager>.Instance != null)
            {
                WarningText = CreateText(
                "WarningText",
                "You cannot change these resource packs during\ngameplay, please make changes in the main menu.",
                new Vector3(-30f, -160f, 0f),
                BaldiFonts.ComicSans12,
                TextAlignmentOptions.Center,
                new Vector2(420f, 360f),
                Color.red);
            }

            /*CreateText("PackTitle", "Pack", new Vector3(0f, 60f, 0f), BaldiFonts.BoldComicSans12, TextAlignmentOptions.Center, new Vector2(200f, 70f), Color.black);
            CreateButton(PackSelectionLeft, base.menuArrowLeft, base.menuArrowLeftHighlight, "PackSelectionLeftBtn", new Vector3(-163f, 34f, 0f));
            CreateButton(PackSelectionRight, base.menuArrowRight, base.menuArrowRightHighlight, "PackSelectionRightBtn", new Vector3(170f, 34f, 0f));
            PackNameText = CreateText("PackText", "PackText", new Vector3(0f, 37f, 0f), BaldiFonts.ComicSans24, TextAlignmentOptions.Center, new Vector2(400f, 32f), Color.black);
            BasePlugin.Instance.PackListText = PackNameText;

            PackAuthorText = CreateText("PackAuthorText", "PackAuthorText", new Vector3(0f, -20f, 0f), BaldiFonts.ComicSans24, TextAlignmentOptions.Top, new Vector2(400f, 32f), Color.black);
            BasePlugin.Instance.PackAuthorText = PackAuthorText;

            PackDescriptionText = CreateText("PackDescriptionText", "PackDescriptionText", new Vector3(0f, -60f, 0f), BaldiFonts.ComicSans24, TextAlignmentOptions.Top, new Vector2(400f, 32f), Color.black);
            BasePlugin.Instance.PackDescriptionText = PackDescriptionText;*/

            UpdatePage();
        }

        private void UpdatePageMax()
        {
            int a = BasePlugin.AllPacks.Count % 4;
            PageMax = (BasePlugin.AllPacks.Count - a) / 4;
            if (a > 0)
            {
                PageMax += 1;
            }
            if (Page > PageMax)
            {
                Page = PageMax;
            }
        }

        private void SwitchButtonLeftAction()
        {
            Page--;
            if (Page < 1)
            {
                Page = PageMax;
            }
            UpdatePage();
        }
        private void SwitchButtonRightAction()
        {
            Page++;
            if (Page > PageMax)
            {
                Page = 1;
            }
            UpdatePage();
        }

        public string GetPackName(int i)
        {
            string name = "";
            if (i < BasePlugin.AllPackStrings.Count && i >= 0)
            {
                name = BasePlugin.AllPacks[BasePlugin.AllPackStrings[i]].name;
            }
            return name;
        }

        public void UpdatePage()
        {
            UpdatePageMax();

            PageText.text = Page.ToString() + "/" + PageMax.ToString();
            for (int a = 0; a < PackTextsButton.Length; a++)
            {
                int i = a + 4 * (Page - 1);
                if (i < BasePlugin.AllPackStrings.Count && i >= 0)
                {
                    BasePlugin.PackMeta meta = BasePlugin.AllPacks[BasePlugin.AllPackStrings[i]];
                    for (int b = 0; b < LoadedPacksOld.Count; b++)
                    {
                        if (meta == BasePlugin.AllPacks[LoadedPacksOld[b]])
                        {
                            string toolTip = "<color=black>" + meta.description + "\n<color=black>Author: " + meta.author;
                            PackTextsButton[a].text.color = Color.black;
                            if (meta == BasePlugin.AllPacks[".Vanilla"])
                            {
                                PackTextsButton[a].text.color = Color.red;
                                toolTip += "\n<color=red>(This Resource Pack Cannot Be Uninstalled.)";
                            }
                            PackTextsButton[a].text.text = "(" + b.ToString() + ")" + meta.name;

                            AddTooltip(PackTextsButton[a], toolTip);
                            break;
                        }
                        else
                        {
                            PackTextsButton[a].text.color = new Color(0f, 0f, 0f, 0.5f);
                            PackTextsButton[a].text.text = meta.name;

                            string toolTip = "<color=black>" + meta.description + "\n<color=black>Author: " + meta.author;
                            AddTooltip(PackTextsButton[a], toolTip);
                        }
                    }
                }
                else
                {
                    PackTextsButton[a].text.text = "";
                    PackTextsButton[a].OnHighlight.RemoveAllListeners();
                    PackTextsButton[a].OffHighlight.RemoveAllListeners();
                }
            }
        }

        public string GetPackNameOnSetup(int i)
        {
            string packName = "";
            if (i < BasePlugin.AllPackStrings.Count && i >= 0)
            {
                BasePlugin.PackMeta meta = BasePlugin.AllPacks[BasePlugin.AllPackStrings[i]];
                for (int b = 0; b < LoadedPacksOld.Count; b++)
                {
                    if (meta == BasePlugin.AllPacks[LoadedPacksOld[b]])
                    {
                        packName = "(" + b.ToString() + ")" + meta.name;
                        break;
                    }
                    else
                    {
                        packName = meta.name;
                    }
                }
            }
            return packName;
        }

        public void PackTextAction0()
        {
            PackTextAction(0);
        }
        public void PackTextAction1()
        {
            PackTextAction(1);
        }
        public void PackTextAction2()
        {
            PackTextAction(2);
        }
        public void PackTextAction3()
        {
            PackTextAction(3);
        }
        public void PackTextAction(int number)
        {
            if (Singleton<CoreGameManager>.Instance != null)
            {
                return;
            }
            int i = number + 4 * (Page - 1);
            if (i < BasePlugin.AllPackStrings.Count && i >= 0)
            {
                string packFolderName = BasePlugin.AllPackStrings[i];
                BasePlugin.PackMeta meta = BasePlugin.AllPacks[packFolderName];
                if (meta == BasePlugin.AllPacks[".Vanilla"])
                {
                    return;
                }
                if (LoadedPacksOld.Contains(packFolderName))
                {
                    LoadedPacksOld.Remove(packFolderName);
                }
                else
                {
                    LoadedPacksOld.Add(packFolderName);
                }
                UpdatePage();
            }
        }

        public void ApplyActions()
        {
            if (Singleton<CoreGameManager>.Instance == null)
            {
                StartCoroutine(Apply());
            }
        }

        public IEnumerator Apply()
        {
            ApplyButton.text.text = "Loading...";
            yield return null;
            string saveFilePath = Path.Combine(AssetLoader.GetModPath(BasePlugin.Instance), ".Core", "Save.txt");
            File.WriteAllLines(saveFilePath, LoadedPacksOld);
            BasePlugin.Instance.LoadResources();
            yield return null;
            ApplyButton.text.text = "Apply";
            UpdatePage();
            yield break;
        }
    }
}
