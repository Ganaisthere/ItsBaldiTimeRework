using MTM101BaldAPI.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItsBaldiTimeRework
{
    public class CustomAudioPlayer : MonoBehaviour
    {
        public AudioSource audioSource1;
        public AudioSource audioSource2;
        public int idk = 1;
        public float VolumeMax => BasePlugin.Instance.ConfigMusicVolume.Value / 10f;
        public List<IEnumerator> enumerators = new List<IEnumerator>();
        public bool isFadeing = false;

        public void Initialize()
        {
            idk = 1;
            isFadeing = false;
            audioSource1.playOnAwake = false;
            audioSource1.spatialBlend = 0f;
            audioSource1.ignoreListenerPause = false;
            audioSource1.volume = 1f * VolumeMax;
            audioSource1.ignoreListenerVolume = false;
            audioSource2.playOnAwake = false;
            audioSource2.spatialBlend = 0f;
            audioSource2.ignoreListenerPause = false;
            audioSource2.volume = 1f * VolumeMax;
            audioSource2.ignoreListenerVolume = false;
            enumerators.Clear();
        }

        public void Update()
        {
            if (!isFadeing)
            {
                audioSource1.volume = 1f * VolumeMax;
                audioSource2.volume = 1f * VolumeMax;
            }
        }

        public void StopAllCoroutinesIguess()
        {
            if (enumerators.Count <= 0) return;
            while (enumerators.Count > 0)
            {
                StopCoroutine(enumerators[0]);
                enumerators.RemoveAt(0);
            }
        }

        public void StartCoroutineIguess(IEnumerator enumerator)
        {
            enumerators.Add(enumerator);
            StartCoroutine(enumerator);
        }

        public void Play(AudioClip audioClip, bool isLoop = false, bool fade = false)
        {
            if (audioClip != null)
            {
                if (fade)
                {
                    StopAllCoroutinesIguess();
                    if (idk == 1)
                    {
                        idk = 2;
                        StartCoroutineIguess(FadeIn(audioSource2));
                        StartCoroutineIguess(FadeOut(audioSource1));
                        PlayAudioClip(audioClip, isLoop, audioSource2);
                    }
                    else
                    {
                        idk = 1;
                        StartCoroutineIguess(FadeIn(audioSource1));
                        StartCoroutineIguess(FadeOut(audioSource2));
                        PlayAudioClip(audioClip, isLoop, audioSource1);
                    }
                }
                else
                {
                    idk = 1;
                    Stop();
                    audioSource1.volume = 1f * VolumeMax;
                    audioSource2.volume = 1f * VolumeMax;
                    PlayAudioClip(audioClip, isLoop, audioSource1);
                }
            }
        }
        public void Queue(AudioClip audioClip1, AudioClip audioClip2, bool fade = false)
        {
            if (audioClip1 != null && audioClip2 != null)
            {
                if (fade)
                {
                    if (idk == 1)
                    {
                        idk = 2;
                        StartCoroutineIguess(FadeIn(audioSource2));
                        StartCoroutineIguess(FadeOut(audioSource1));
                        StartCoroutineIguess(QueueAction(audioClip1, audioClip2, audioSource2));
                    }
                    else
                    {
                        idk = 1;
                        StartCoroutineIguess(FadeIn(audioSource1));
                        StartCoroutineIguess(FadeOut(audioSource2));
                        StartCoroutineIguess(QueueAction(audioClip1, audioClip2, audioSource1));
                    }
                }
                else
                {
                    idk = 1;
                    Stop();
                    audioSource1.volume = 1f * VolumeMax;
                    audioSource2.volume = 1f * VolumeMax;
                    StartCoroutineIguess(QueueAction(audioClip1, audioClip2, audioSource1));
                }
            }
        }
        public void Stop(bool instant = true)
        {
            StopAllCoroutinesIguess();
            if (instant)
            {
                audioSource1.Stop();
                audioSource2.Stop();
            }
            else
            {
                StartCoroutineIguess(FadeOut(audioSource1));
                StartCoroutineIguess(FadeOut(audioSource2));
            }
        }
        public IEnumerator Meatophobia(Notebook notebook)
        {
            AudioClip audioClip = BasePlugin.AssetMan.Get<AudioClip>("Meatophobia");
            PlayerManager player = Singleton<CoreGameManager>.Instance.GetPlayer(0);
            SpriteRenderer spriteRenderer = notebook.ReflectionGetVariable("sprite") as SpriteRenderer;
            spriteRenderer.sprite = BasePlugin.AssetMan.Get<Sprite>("Notebook_John");
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
                    isFadeing = true;

                    if (notebook == null || player == null)
                    {
                        yield break;
                    }
                    float dist = Vector3.Distance(notebook.transform.position, player.transform.position);
                    if (dist <= 20f)
                    {
                        audioSource2.volume = 1f * VolumeMax;
                        audioSource1.volume = 0f;
                    }
                    else if (dist <= 50f)
                    {
                        audioSource2.volume = (1f - (dist - 20f) / 30f) * VolumeMax;
                        audioSource1.volume = ((dist - 20f) / 30f) * VolumeMax;
                    }
                    else
                    {
                        audioSource2.volume = 0f;
                        audioSource1.volume = 1f * VolumeMax;
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
                    isFadeing = true;

                    if (notebook == null || player == null)
                    {
                        yield break;
                    }
                    float dist = Vector3.Distance(notebook.transform.position, player.transform.position);
                    if (dist <= 20f)
                    {
                        audioSource1.volume = 1f * VolumeMax;
                        audioSource2.volume = 0f;
                    }
                    else if (dist <= 50f)
                    {
                        audioSource1.volume = (1f - (dist - 20f) / 30f) * VolumeMax;
                        audioSource2.volume = ((dist - 20f) / 30f) * VolumeMax;
                    }
                    else
                    {
                        audioSource1.volume = 0f;
                        audioSource2.volume = 1f * VolumeMax;
                    }
                    yield return null;
                }
            }

            isFadeing = false;
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
                isFadeing = true;

                audioSource.volume = timer * VolumeMax;
                timer += Time.deltaTime;
                yield return null;
            }
            audioSource.volume = 1f * VolumeMax;

            isFadeing = false;
            yield break;
        }
        private IEnumerator FadeOut(AudioSource audioSource)
        {
            float timer = 1f;
            audioSource.volume = 1f * VolumeMax;
            while (timer > 0f)
            {
                isFadeing = true;

                audioSource.volume = timer * VolumeMax;
                timer -= Time.deltaTime;
                yield return null;
            }
            audioSource.Stop();
            audioSource.volume = 1f * VolumeMax;

            isFadeing = false;
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
