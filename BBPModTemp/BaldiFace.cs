using UnityEngine;
using System.Collections;

namespace ItsBaldiTimeRework
{
    public class BaldiFace : NPC
    {
        public override void Initialize()
        {
            base.Initialize();
            spriteRenderer[0].sprite = BasePlugin.AssetMan.Get<Sprite>("BaldiFace");
            spriteRenderer[0].transform.gameObject.layer = LayerMask.NameToLayer("Overlay");
            behaviorStateMachine.ChangeState(new BaldiFace_Wait(this, false, true));
        }
    }

    public class BaldiFace_StateBase : NpcState
    {
        public BaldiFace baldiFace;
        public bool isLethal = false;

        public BaldiFace_StateBase(BaldiFace me, bool lethal = true) : base(me)
        {
            baldiFace = me;
            isLethal = lethal;
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void OnStateTriggerStay(Entity otherEntity, Collider other, bool validCollision)
        {
            base.OnStateTriggerStay(otherEntity, other, validCollision);
            if (!validCollision || !other.CompareTag("Player") || !isLethal)
            {
                return;
            }
            Other.NpcKillPlayer(npc);
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
        }
    }

    public class BaldiFace_Wait : BaldiFace_StateBase
    {
        bool sound = true;
        public BaldiFace_Wait(BaldiFace me, bool lethal = false, bool playSound = false) : base(me)
        {
            baldiFace = me;
            isLethal = lethal;
            sound = playSound;
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

            npc.behaviorStateMachine.ChangeState(new BaldiFace_Chase(baldiFace, true, 16f));

            yield break;
        }
    }

    public class BaldiFace_Chase : BaldiFace_StateBase
    {
        [SerializeField]
        public float touchSodaTimer = 0f;

        [SerializeField]
        public float speed = 16f;

        public BaldiFace_Chase(BaldiFace me, bool lethal = true, float setSpeed = 16f) : base(me, lethal)
        {
            baldiFace = me;
            isLethal = lethal;
            speed = setSpeed;
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
                if (Singleton<CoreGameManager>.Instance.GetPlayer(0) != null && touchSodaTimer <= 0f)
                {
                    npc.transform.LookAt(Singleton<CoreGameManager>.Instance.GetPlayer(0).transform);
                    npc.transform.position += npc.transform.forward * Time.deltaTime * 18f * npc.TimeScale;
                    npc.Entity.SetVisible(true);
                    //npc.Entity.SetFrozen(false);
                    npc.Entity.SetHidden(false);
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
                        if (dist < 5f)
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
                    foreach (ITM_BSODA bSODA in iTM_BSODAs)
                    {
                        float dist = Vector3.Distance(npc.transform.position, bSODA.transform.position);
                        if (dist < 5f)
                        {
                            Object.Destroy(bSODA);
                        }
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
