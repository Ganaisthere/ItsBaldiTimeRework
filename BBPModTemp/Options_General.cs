using MTM101BaldAPI.OptionsAPI;
using TMPro;
using UnityEngine;
using MTM101BaldAPI.UI;

namespace ItsBaldiTimeRework
{
    internal class Options_General : CustomOptionsCategory
    {
        public AdjustmentBars MusicVolumeBar;
        public TextMeshProUGUI MusicVolumeBarText;
        public StandardMenuButton CameraStyle;

        public override void Build()
        {
            MusicVolumeBar = CreateBars(
                ChangeMusicVolume,
                "MusicVolumeBar",
                new Vector3(20f, 40f, 0f),
                10);
            AddTooltip(MusicVolumeBar, Other.GLT("Tip_MusicVolumeBar"));

            MusicVolumeBarText = CreateText(
                "MusicVolumeBarText",
                Other.GLT("Opt_MusicVolume"),
                new Vector3(-60f, 40f, 0f),
                BaldiFonts.ComicSans24,
                TextAlignmentOptions.Right,
                new Vector2(100f, 360f),
                Color.black);

            CameraStyle = CreateTextButton(
                ChangeCameraStyle,
                "CameraStyleBut",
                GetCameraStyle(),
                new Vector3(16f, 0f, 0f),
                BaldiFonts.ComicSans24,
                TextAlignmentOptions.Right,
                new Vector2(300f, 28f),
                Color.black);
            AddTooltip(CameraStyle, Other.GLT("Tip_CameraStyle"));

            Reflesh();
        }

        public void ChangeMusicVolume()
        {
            BasePlugin.Instance.ConfigMusicVolume.Value = MusicVolumeBar.GetRaw();
        }

        public void ChangeCameraStyle()
        {
            if (BasePlugin.Instance.ConfigCameraShakeStyle.Value == CameraShakeStyle.Disabled)
            {
                BasePlugin.Instance.ConfigCameraShakeStyle.Value = CameraShakeStyle.Smooth;
            }
            else if (BasePlugin.Instance.ConfigCameraShakeStyle.Value == CameraShakeStyle.Smooth)
            {
                BasePlugin.Instance.ConfigCameraShakeStyle.Value = CameraShakeStyle.Beat;
            }
            else
            {
                BasePlugin.Instance.ConfigCameraShakeStyle.Value = CameraShakeStyle.Disabled;
            }
            CameraStyle.text.text = GetCameraStyle();
        }

        public string GetCameraStyle()
        {
            string text = Other.GLT("But_CameraStyle_0");
            if (BasePlugin.Instance.ConfigCameraShakeStyle.Value == CameraShakeStyle.Smooth)
            {
                text = Other.GLT("But_CameraStyle_1");
            }
            else if (BasePlugin.Instance.ConfigCameraShakeStyle.Value == CameraShakeStyle.Beat)
            {
                text = Other.GLT("But_CameraStyle_2");
            }
            return text;
        }

        public void Reflesh()
        {
            MusicVolumeBar.Adjust(BasePlugin.Instance.ConfigMusicVolume.Value);
        }
    }
}
