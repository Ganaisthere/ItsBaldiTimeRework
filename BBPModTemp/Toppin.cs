using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace ItsBaldiTimeRework
{
    public class Toppin : NPC
    {
        public SoundObject SFX_Get;
        public Sprite CageSprite;
        public Sprite[] IdleSprite = new Sprite[5];
        public Sprite[] YaySprite = new Sprite[5];
        public int intSprite = 0;
        public bool isSaved = false;

        public override void Initialize()
        {
            base.Initialize();
            navigator.SetSpeed(16f);
            navigator.SetRoomAvoidance(false);
            intSprite = 0;
            isSaved = false;
            spriteRenderer[0].sprite = CageSprite;
            behaviorStateMachine.ChangeState(new Toppin_WaitForPlayerPickup(this));
        }
    }
    public class Toppin_StateBase : NpcState
    {
        public Toppin toppin;
        public float dist = 0;
        public Vector3 pos;
        public Toppin_StateBase(Toppin me) : base(me)
        {
            toppin = me;
        }
        public override void Enter()
        {
            base.Enter();
        }
        public override void Update()
        {
            base.Update();
            if (toppin.intSprite == 0 || BaldiTimeActions.toppins.Count <= 0)
            {
                pos = Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position;
            }
            else
            {
                pos = BaldiTimeActions.toppins[toppin.intSprite - 1].transform.position;
            }
            dist = Vector3.Distance(npc.transform.position, pos);
            if (dist > 150f)
            {
                toppin.Entity.Teleport(Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position);
            }
            npc.Navigator.SetSpeed(14f + dist / 3f);
        }
        public override void Exit()
        {
            base.Exit();
        }
    }
    public class Toppin_WaitForPlayerPickup : Toppin_StateBase
    {
        public Toppin_WaitForPlayerPickup(Toppin me) : base(me)
        {
            toppin = me;
        }
        public override void Enter()
        {
            base.Enter();
            if (!npc.Navigator.HasDestination)
            {
                //ChangeNavigationState(new NavigationState_DoNothing(npc, 63));
                ChangeNavigationState(new NavigationState_DoNothing(npc, 63));
                //ChangeNavigationState(new NavigationState_Disabled(npc));
            }
        }
        public override void OnStateTriggerStay(Entity otherEntity, Collider other, bool validCollision)
        {
            base.OnStateTriggerStay(otherEntity, other, validCollision);
            if (!validCollision || !other.CompareTag("Player") || toppin.isSaved)
            {
                return;
            }

            BaldiTimeActions.points += 1000f;
            BaldiTimeActions.AddCombo(1f);
            toppin.isSaved = true;
            toppin.intSprite = BaldiTimeActions.toppins.Count;
            if (toppin.intSprite < 5)
            {
                toppin.spriteRenderer[0].sprite = toppin.YaySprite[toppin.intSprite];
            }
            else
            {
                int num = UnityEngine.Random.Range(0, 4);
                toppin.spriteRenderer[0].sprite = toppin.YaySprite[num];
            }
            BaldiTimeActions.toppins.Add(toppin);
            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(toppin.SFX_Get);

            toppin.StartCoroutine(SaveAnimations(toppin));
        }
        public IEnumerator SaveAnimations(Toppin toppin)
        {
            float timer = 0f;
            while (timer < 1f)
            {
                float change = math.sin(timer * math.PI);
                toppin.spriteRenderer[0].transform.localScale = new Vector3(1f + change, 1f + change, 1f);
                timer += Time.deltaTime;
                yield return null;
            }
            toppin.spriteRenderer[0].sprite = toppin.IdleSprite[base.toppin.intSprite];
            toppin.behaviorStateMachine.ChangeState(new Toppin_FollowPlayer(toppin));
            yield break;
        }
        public override void Update()
        {
            ChangeNavigationState(new NavigationState_DoNothing(npc, 63));
        }
        public override void Exit()
        {
            base.Exit();
            //Debug.LogWarning("Toppins_WaitPlayer");
        }
    }
    public class Toppin_FollowPlayer : Toppin_StateBase
    {
        public Toppin_FollowPlayer(Toppin me) : base(me)
        {
            toppin = me;
        }
        public override void Enter()
        {
            base.Enter();
            //ChangeNavigationState(new NavigationState_WanderRandom(npc, 63));
        }
        public override void Update()
        {
            base.Update();
            if (dist < 5f)
            {
                ChangeNavigationState(new NavigationState_DoNothing(npc, 63));
            }
            if (dist > 10f)
            {
                ChangeNavigationState(new NavigationState_TargetPosition(npc, 63, pos));
            }
        }
    }
}
