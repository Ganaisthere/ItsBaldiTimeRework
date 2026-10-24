using System.Collections.Generic;
using UnityEngine;

namespace ItsBaldiTimeRework
{
    //Copyright (c) 2026 Denyscrasav4ik
    //Licensed under the MIT License(MIT)
    public class AfterImage : MonoBehaviour
    {
        public class AfterImageRenderer
        {
            public SpriteRenderer spriteRenderer = null;
            public Color baseColor = Color.white;
            public float timer = 0f;
        }
        public List<AfterImageRenderer> renderers = new List<AfterImageRenderer>();

        public static void CreateAfterImage(NPC npc, bool red = false, bool isOverlay = false)
        {
            if (npc == null || !npc.gameObject.activeInHierarchy)
            {
                return;
            }

            GameObject ghostObj = new GameObject(npc.name + "_Afterimage");
            ghostObj.transform.position = npc.transform.position + npc.transform.forward * -1f;
            ghostObj.transform.rotation = npc.transform.rotation;
            ghostObj.transform.localScale = npc.transform.lossyScale;
            ghostObj.transform.SetAsFirstSibling();

            AfterImage afterImage = ghostObj.AddComponent<AfterImage>();

            foreach (SpriteRenderer spriteRenderer in npc.spriteRenderer)
            {
                if (spriteRenderer != null && spriteRenderer.gameObject.activeInHierarchy)
                {
                    GameObject spriteObj = new GameObject("AfterimageSprite");
                    Transform spriteTransform = spriteObj.transform;
                    spriteTransform.SetParent(ghostObj.transform, true);
                    spriteTransform.transform.position = spriteRenderer.transform.position;
                    spriteTransform.transform.rotation = spriteRenderer.transform.rotation;
                    spriteTransform.localScale = spriteRenderer.transform.lossyScale;
                    if (isOverlay)
                    {
                        spriteObj.layer = LayerMask.NameToLayer("Overlay");
                    }
                    spriteTransform.transform.SetAsFirstSibling();

                    SpriteRenderer ghostRenderer = spriteObj.AddComponent<SpriteRenderer>();
                    ghostRenderer.sprite = spriteRenderer.sprite;
                    ghostRenderer.flipX = spriteRenderer.flipX;
                    ghostRenderer.flipY = spriteRenderer.flipY;
                    ghostRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
                    ghostRenderer.sortingOrder = spriteRenderer.sortingOrder;
                    ghostRenderer.drawMode = spriteRenderer.drawMode;
                    ghostRenderer.maskInteraction = spriteRenderer.maskInteraction;
                    ghostRenderer.spriteSortPoint = spriteRenderer.spriteSortPoint;
                    if (spriteRenderer.sharedMaterial != null)
                    {
                        ghostRenderer.material = new Material(spriteRenderer.sharedMaterial);
                    }
                    ghostRenderer.color = spriteRenderer.color;

                    Color color = red ? Color.red : spriteRenderer.color;
                    AfterImageRenderer AIR = new AfterImageRenderer
                    {
                        spriteRenderer = ghostRenderer,
                        baseColor = color,
                        timer = 0f,
                    };
                    afterImage.renderers.Add(AIR);
                }
            }
        }

        public void Update()
        {
            foreach (var renderer in renderers)
            {
                if (renderer.spriteRenderer != null)
                {
                    if (renderer.timer < 0.5f)
                    {
                        renderer.timer += Time.unscaledDeltaTime;
                        Color color = renderer.baseColor - new Color(0f, 0f, 0f, 1f) * renderer.timer * 2f;
                        renderer.spriteRenderer.color = color;
                        renderer.spriteRenderer.transform.localScale = Vector3.one * (1f + renderer.timer);
                    }
                    else
                    {
                        DestroyAfterImage(renderer.spriteRenderer);
                    }
                }
            }
        }

        public void DestroyAfterImage(SpriteRenderer renderer)
        {
            if (renderer.material != null)
            {
                Destroy(renderer.material);
            }
            Destroy(renderer.gameObject);
        }

        public void OnDestroy()
        {
            foreach (var renderer in renderers)
            {
                if (renderer.spriteRenderer != null)
                {
                    DestroyAfterImage(renderer.spriteRenderer);
                }
            }
            renderers.Clear();
        }
    }
}
