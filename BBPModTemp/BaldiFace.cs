using MTM101BaldAPI.AssetTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItsBaldiTimeRework
{
    public class BaldiFace : NPC
    {
        public override void Initialize()
        {
            base.Initialize();
            spriteRenderer[0].sprite = BasePlugin.AssetMan.Get<Sprite>("BaldiFace");
            spriteRenderer[0].gameObject.layer = LayerMask.NameToLayer("Overlay");
            behaviorStateMachine.ChangeState(new BaldiFace_Wait(this, false, true));
        }
    }

    public class BaldiFace_StateBase : NpcState
    {
        public BaldiFace baldiFace;
        public bool isLethal = false;
        public bool afterImage = true;

        [SerializeField]
        public float afterImageTimer = 0f;

        public BaldiFace_StateBase(BaldiFace me, bool lethal = true, bool isAfterImage = true) : base(me)
        {
            baldiFace = me;
            isLethal = lethal;
            afterImage = isAfterImage;
        }

        public override void Enter()
        {
            base.Enter();
            afterImageTimer = 0f;
        }

        public override void OnStateTriggerStay(Entity otherEntity, Collider other, bool validCollision)
        {
            base.OnStateTriggerStay(otherEntity, other, validCollision);
            if (!validCollision || !other.CompareTag("Player") || !isLethal)
            {
                return;
            }
            PlayerManager component = other.GetComponent<PlayerManager>();
            ItemManager itm = component.itm;
            if (!component.invincible)
            {
                if (itm.Has(Items.Teleporter))
                {
                    itm.Remove(Items.Teleporter);
                    npc.behaviorStateMachine.ChangeState(new BaldiFace_Teleport(baldiFace));
                }
                else
                {
                    afterImage = false;
                    Other.NpcKillPlayer(npc);
                }
            }
        }

        public override void OnStateTriggerExit(Entity otherEntity, Collider other, bool validCollision)
        {
            base.OnStateTriggerExit(otherEntity, other, validCollision);
        }

        public override void Update()
        {
            base.Update();
            if (Singleton<CoreGameManager>.Instance != null)
            {
                if (!Singleton<CoreGameManager>.Instance.GetPlayer(0).ec.map.arrowTargets.Contains(npc.Entity))
                {
                    Singleton<CoreGameManager>.Instance.GetPlayer(0).ec.map.AddArrow(npc.Entity, new Color(0f, 1f, 0f, 1f));
                }
            }
            if (afterImage && BasePlugin.Instance.ConfigBaldiFaceAfterImage.Value)
            {
                if (afterImageTimer < 0.05f)
                {
                    afterImageTimer += Time.unscaledDeltaTime;
                }
                else
                {
                    afterImageTimer = 0f;
                    AfterImage.CreateAfterImage(npc, true, true);
                }
            }
            else
            {
                afterImageTimer = 0f;
            }
        }
    }

    public class BaldiFace_Wait : BaldiFace_StateBase
    {
        bool sound = true;
        public BaldiFace_Wait(BaldiFace me, bool lethal = false, bool playSound = false, bool isAfterImage = false) : base(me)
        {
            baldiFace = me;
            isLethal = lethal;
            sound = playSound;
            afterImage = isAfterImage;
        }

        public override void Enter()
        {
            base.Enter();
            ChangeNavigationState(new NavigationState_Disabled(npc));
            npc.Entity.SetFrozen(true);
            baldiFace.StartCoroutine(Wait());
        }

        public IEnumerator Wait()
        {
            if (Singleton<CoreGameManager>.Instance == null)
            {
                yield break;
            }

            if (sound)
            {
                Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("BaldiFaceLaugh"));
            }
            npc.Entity.Teleport(Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position);

            float timer = 0f;
            while (timer < 3f)
            {
                if (Singleton<CoreGameManager>.Instance == null)
                {
                    yield break;
                }
                npc.spriteRenderer[0].color = new Color(1f, 0f + timer / 3f, 0f + timer / 3f, 0f + timer / 3f);
                timer += Time.deltaTime * npc.TimeScale;
                yield return null;
            }
            npc.spriteRenderer[0].color = Color.white;

            npc.behaviorStateMachine.ChangeState(new BaldiFace_Chase(baldiFace));

            yield break;
        }
    }

    public class BaldiFace_Teleport : BaldiFace_StateBase
    {
        [SerializeField]
        public float teleportTimer = 0f;

        public BaldiFace_Teleport(BaldiFace me, bool isAfterImage = false) : base(me, false)
        {
            baldiFace = me;
            isLethal = false;
            afterImage = isAfterImage;
        }

        public override void Enter()
        {
            base.Enter();
            npc.StartCoroutine(TeleportActions());
        }

        public IEnumerator TeleportActions()
        {
            void PlaySingle(SoundObject soundObject)
            {
                if (npc.TryGetComponent(out AudioManager audioMan))
                {
                    audioMan.PlaySingle(soundObject);
                }
            }
            void Teleport()
            {
                npc.Entity.Teleport(npc.ec.RandomCell(includeOffLimits: false, includeWithObjects: false, useEntitySafeCell: true).FloorWorldPosition + Vector3.up * 5f);
                PlaySingle(AssetFinder.FindOfTypeWithName<SoundObject>("Teleport", true));
            }

            npc.spriteRenderer[0].sprite = BasePlugin.AssetMan.Get<Sprite>("BaldiFace_Scraed");

            PlaySingle(Other.GetRandomDeathSound());

            teleportTimer = 0f;
            while (teleportTimer < 1f)
            {
                teleportTimer += Time.deltaTime;
            }
            npc.Entity.SetInteractionState(value: false);
            npc.Entity.SetFrozen(value: true);
            int teleports = Random.Range(12, 16);
            int teleportCount = 0;
            float baseTime = 0.2f;
            float currentTime = baseTime;
            float increaseFactor = 1.1f;
            while (teleportCount < teleports)
            {
                currentTime -= Time.deltaTime;
                if (currentTime <= 0f)
                {
                    Teleport();
                    teleportCount++;
                    baseTime *= increaseFactor;
                    currentTime = baseTime;
                }
                yield return null;
            }

            npc.Entity.SetInteractionState(value: true);
            npc.Entity.SetFrozen(value: false);

            npc.behaviorStateMachine.ChangeState(new BaldiFace_Chase(baldiFace, true));
        }
    }

    public class BaldiFace_Chase : BaldiFace_StateBase
    {
        [SerializeField]
        public float touchSodaTimer = 0f;

        [SerializeField]
        public float speed = 16f;

        [SerializeField]
        public float baseSpeed = 16f;

        public BaldiFace_Chase(BaldiFace me, bool lethal = true, bool isAfterImage = true, float setSpeed = 16f) : base(me, lethal)
        {
            baldiFace = me;
            isLethal = lethal;
            speed = setSpeed;
            baseSpeed = setSpeed;
            afterImage = isAfterImage;
        }

        public override void Enter()
        {
            base.Enter();
            npc.Entity.SetFrozen(false);
            ChangeNavigationState(new NavigationState_Disabled(npc));
        }

        public override void Update()
        {
            base.Update();
            if (Singleton<CoreGameManager>.Instance != null)
            {
                if (Singleton<CoreGameManager>.Instance.GetPlayer(0) != null)
                {
                    Transform transform = Singleton<CoreGameManager>.Instance.GetPlayer(0).transform;
                    npc.transform.LookAt(transform);

                    float edit = baseSpeed;
                    float dist = Vector3.Distance(npc.transform.position, transform.position);
                    if (dist > 1.5f)
                    {
                        edit += (dist - 1.5f) / 6f;
                    }
                    speed += (edit - speed) / 2f;

                    npc.Entity.SetVisible(true);
                    //npc.Entity.SetFrozen(false);
                    npc.Entity.SetHidden(false);
                    if (touchSodaTimer <= 0f)
                    {
                        npc.transform.position += npc.transform.forward * Time.deltaTime * speed * npc.TimeScale;
                    }
                }
                ITM_BSODA[] iTM_BSODAs = Object.FindObjectsOfType<ITM_BSODA>();
                if (iTM_BSODAs.Length <= 0)
                {
                    touchSodaTimer = 0f;
                }
                else
                {
                    bool touched = false;
                    foreach (ITM_BSODA bSODA in iTM_BSODAs)
                    {
                        float dist = Vector3.Distance(npc.transform.position, bSODA.transform.position);
                        if (dist < 10f)
                        {
                            touchSodaTimer += Time.deltaTime;
                            touched = true;
                            break;
                        }
                    }
                    if (!touched)
                    {
                        touchSodaTimer = 0f;
                    }
                }
                if (touchSodaTimer > 1.5f)
                {
                    touchSodaTimer = 0f;
                    List<ITM_BSODA> bsodas = new List<ITM_BSODA>();
                    foreach (ITM_BSODA bSODA in iTM_BSODAs)
                    {
                        float dist = Vector3.Distance(npc.transform.position, bSODA.transform.position);
                        if (dist < 10f)
                        {
                            bsodas.Add(bSODA);
                        }
                    }
                    while (bsodas.Count > 0)
                    {
                        Object.Destroy(bsodas[0]);
                        bsodas.RemoveAt(0);
                    }
                    Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("Explosion"));
                }
                if (touchSodaTimer > 0f)
                {
                    npc.spriteRenderer[0].sprite = BasePlugin.AssetMan.Get<Sprite>("BaldiFace_Scraed");
                }
                else
                {
                    npc.spriteRenderer[0].sprite = BasePlugin.AssetMan.Get<Sprite>("BaldiFace");
                }
            }
        }
    }
}
