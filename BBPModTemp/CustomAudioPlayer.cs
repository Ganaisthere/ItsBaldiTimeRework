using HarmonyLib;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Reflection;
using System.Collections;
using UnityEngine;

namespace ItsBaldiTimeRework
{
    public class CustomAudioPlayer : MonoBehaviour
    {
        public AudioSource audioSource1;
        public AudioSource audioSource2;
        public int idk = 1;

        public void Initialize()
        {
            idk = 1;
            audioSource1.playOnAwake = false;
            audioSource1.spatialBlend = 0f;
            audioSource1.ignoreListenerPause = false;
            audioSource1.volume = 1f;
            audioSource2.playOnAwake = false;
            audioSource2.spatialBlend = 0f;
            audioSource2.ignoreListenerPause = false;
            audioSource2.volume = 1f;
        }
        public void Play(string clipName, bool isLoop = false, bool fade = false)
        {
            AudioClip audioClip = BasePlugin.AssetMan.Get<AudioClip>(clipName);
            if (audioClip != null)
            {
                if (fade)
                {
                    StopAllCoroutines();
                    if (idk == 1)
                    {
                        idk = 2;
                        StartCoroutine(FadeIn(audioSource2));
                        StartCoroutine(FadeOut(audioSource1));
                        PlayAudioClip(audioClip, isLoop, audioSource2);
                    }
                    else
                    {
                        idk = 1;
                        StartCoroutine(FadeIn(audioSource1));
                        StartCoroutine(FadeOut(audioSource2));
                        PlayAudioClip(audioClip, isLoop, audioSource1);
                    }
                }
                else
                {
                    idk = 1;
                    Stop();
                    audioSource1.volume = 1f;
                    audioSource2.volume = 1f;
                    PlayAudioClip(audioClip, isLoop, audioSource1);
                }
            }
            else
            {
                Debug.LogWarning("Oh Fxxk Where Is - " + clipName + " - I Can't Find It");
            }
        }
        public void Queue(string clipName1, string clipName2, bool fade = false)
        {
            AudioClip audioClip1 = BasePlugin.AssetMan.Get<AudioClip>(clipName1);
            AudioClip audioClip2 = BasePlugin.AssetMan.Get<AudioClip>(clipName2);
            if (audioClip1 != null && audioClip2 != null)
            {
                if (fade)
                {
                    if (idk == 1)
                    {
                        idk = 2;
                        StartCoroutine(FadeIn(audioSource2));
                        StartCoroutine(FadeOut(audioSource1));
                        StartCoroutine(QueueAction(audioClip1, audioClip2, audioSource2));
                    }
                    else
                    {
                        idk = 1;
                        StartCoroutine(FadeIn(audioSource1));
                        StartCoroutine(FadeOut(audioSource2));
                        StartCoroutine(QueueAction(audioClip1, audioClip2, audioSource1));
                    }
                }
                else
                {
                    idk = 1;
                    Stop();
                    audioSource1.volume = 1f;
                    audioSource2.volume = 1f;
                    StartCoroutine(QueueAction(audioClip1, audioClip2, audioSource1));
                }
            }
            else
            {
                if (audioClip1 == null)
                    Debug.LogWarning("Oh Fxxk Where Is - " + clipName1 + " - I Can't Find It");
                if (audioClip2 == null)
                    Debug.LogWarning("Oh Fxxk Where Is - " + clipName2 + " - I Can't Find It");
            }
        }
        public void Stop(bool instant = true)
        {
            StopAllCoroutines();
            if (instant)
            {
                audioSource1.Stop();
                audioSource2.Stop();
            }
            else
            {
                StartCoroutine(FadeOut(audioSource1));
                StartCoroutine(FadeOut(audioSource2));
            }
        }
        public IEnumerator Meatophobia(Notebook notebook)
        {
            AudioClip audioClip = BasePlugin.AssetMan.Get<AudioClip>("Meatophobia");
            PlayerManager player = Singleton<CoreGameManager>.Instance.GetPlayer(0);
            SpriteRenderer spriteRenderer = notebook.ReflectionGetVariable("sprite") as SpriteRenderer;
            spriteRenderer.sprite = BaldiTimeActions.Notebook_John;
            if (audioClip == null)
            {
                Debug.LogWarning("Oh Fxxk Where Is - Meatophobia - I Can't Find It");
                yield break;
            }
            if (idk == 1)
            {
                audioSource2.Stop();
                audioSource2.clip = audioClip;
                audioSource2.loop = true;
                audioSource2.Play();
                while (!BaldiTimeActions.itsBaldiTime)
                {
                    if (notebook == null || player == null)
                    {
                        yield break;
                    }
                    float dist = Vector3.Distance(notebook.transform.position, player.transform.position);
                    if (dist <= 20f)
                    {
                        audioSource2.volume = 1f;
                        audioSource1.volume = 0f;
                    }
                    else if (dist <= 50f)
                    {
                        audioSource2.volume = 1f - (dist - 20f) / 30f;
                        audioSource1.volume = (dist - 20f) / 30f;
                    }
                    else
                    {
                        audioSource2.volume = 0f;
                        audioSource1.volume = 1f;
                    }
                    yield return null;
                }
            }
            else
            {
                audioSource1.Stop();
                audioSource1.clip = audioClip;
                audioSource1.loop = true;
                audioSource1.Play();
                while (!BaldiTimeActions.itsBaldiTime)
                {
                    if (notebook == null || player == null)
                    {
                        yield break;
                    }
                    float dist = Vector3.Distance(notebook.transform.position, player.transform.position);
                    if (dist <= 20f)
                    {
                        audioSource1.volume = 1f;
                        audioSource2.volume = 0f;
                    }
                    else if (dist <= 50f)
                    {
                        audioSource1.volume = 1f - (dist - 20f) / 30f;
                        audioSource2.volume = (dist - 20f) / 30f;
                    }
                    else
                    {
                        audioSource1.volume = 0f;
                        audioSource2.volume = 1f;
                    }
                    yield return null;
                }
            }
            yield break;
        }
        private void PlayAudioClip(AudioClip audioClip, bool isLoop, AudioSource audioSource)
        {
            if (audioClip != null)
            {
                audioSource.clip = audioClip;
                audioSource.loop = isLoop;
                audioSource.Play();
            }
        }
        private IEnumerator FadeIn(AudioSource audioSource)
        {
            float timer = 0f;
            audioSource.volume = 0f;
            while (timer < 1f)
            {
                audioSource.volume = timer;
                timer += Time.deltaTime;
                yield return null;
            }
            audioSource.volume = 1f;
            yield break;
        }
        private IEnumerator FadeOut(AudioSource audioSource)
        {
            float timer = 1f;
            audioSource.volume = 1f;
            while (timer > 0f)
            {
                audioSource.volume = timer;
                timer -= Time.deltaTime;
                yield return null;
            }
            audioSource.Stop();
            audioSource.volume = 1f;
            yield break;
        }
        private IEnumerator QueueAction(AudioClip audioClip1, AudioClip audioClip2, AudioSource audioSource)
        {
            PlayAudioClip(audioClip1, false, audioSource);
            while (audioSource.isPlaying || (AudioListener.pause && !audioSource.ignoreListenerPause))
            {
                yield return null;
            }
            PlayAudioClip(audioClip2, true, audioSource);
            yield break;
        }
    }
}
