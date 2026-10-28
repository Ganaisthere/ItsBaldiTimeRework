using MTM101BaldAPI.AssetTools;
using System.Linq;
using UnityEngine;

namespace ItsBaldiTimeRework
{
    public class FileName2 : NPC
    {
        public float pushSpeed = 10f, pushAcceleration = -10f;
        public override void Initialize()
        {
            base.Initialize();
            pushSpeed = 10f;
            pushAcceleration = -10f;
            spriteRenderer[0].sprite = BasePlugin.AssetMan.Get<Sprite>("Filename2");
            navigator.SetSpeed(24f);
            navigator.SetRoomAvoidance(false);
            navigator.passableObstacles.Add(PassableObstacle.BreakableWindow);
            navigator.passableObstacles.Add(PassableObstacle.LockedDoor);
            behaviorStateMachine.ChangeState(new FileName2_ChasingPlayer(this));
        }
    }

    public class FileName2_StateBase : NpcState
    {
        public FileName2 fileName2;
        public float speed = 26f, baseSpeed = 26f, timer = 0f;

        public FileName2_StateBase(FileName2 me) : base(me)
        {
            fileName2 = me;
        }

        public override void Enter()
        {
            base.Enter();
            speed = 26f;
            baseSpeed = 26f;
            timer = 0f;
        }

        public override void OnStateTriggerStay(Entity otherEntity, Collider other, bool validCollision)
        {
            base.OnStateTriggerStay(otherEntity, other, validCollision);
            if (!validCollision)
            {
                return;
            }
            if (other.CompareTag("Player"))
            {
                Other.NpcKillPlayer(npc);
            }
            if (timer <= 0f)
            {
                if (npc.TryGetComponent(out AudioManager audMan))
                {
                    audMan.PlaySingle(AssetFinder.FindOfTypeWithName<SoundObject>("Bang", true));
                }
                Force force = new Force((otherEntity.transform.position - npc.transform.position).normalized, fileName2.pushSpeed, fileName2.pushAcceleration);
                otherEntity.AddForce(force);
                timer = 0.1f;
            }
        }

        public override void Update()
        {
            base.Update();
            if (Singleton<CoreGameManager>.Instance != null)
            {
                if (!Singleton<CoreGameManager>.Instance.GetPlayer(0).ec.map.arrowTargets.Contains(npc.Entity))
                {
                    Singleton<CoreGameManager>.Instance.GetPlayer(0).ec.map.AddArrow(npc.Entity, new Color(1f, 0f, 0f, 1f));
                }

                float edit = baseSpeed;
                float dist = Vector3.Distance(npc.transform.position, Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position);
                if (dist > 1.5f)
                {
                    edit += (dist - 1.5f) / 8f;
                }
                speed += (edit - speed) / 2f;

                npc.Navigator.SetSpeed(speed);
            }
            if (timer > 0f)
            {
                timer -= Time.deltaTime * npc.TimeScale;
            }
        }
    }

    public class FileName2_ChasingPlayer : FileName2_StateBase
    {
        public int isHasNoiseBiggerThan112 = 0;
        public FileName2_ChasingPlayer(FileName2 me) : base(me)
        {
            fileName2 = me;
        }

        public override void Enter()
        {
            base.Enter();
            isHasNoiseBiggerThan112 = 0;
            if (!npc.Navigator.HasDestination)
            {
                PlayerManager[] players = Object.FindObjectsOfType<PlayerManager>();
                int m = 0;
                for (int n = 0; n < players.Length; n++)
                {
                    if (players[n] == players.Min())
                    {
                        m = n;
                        break;
                    }
                }
                ChangeNavigationState(new NavigationState_TargetPosition(npc, 63, players[m].transform.position));
            }
        }

        public override void Hear(GameObject source, Vector3 position, int value)
        {
            base.Hear(source, position, value);
            if (!npc.looker.PlayerInSight() && value >= isHasNoiseBiggerThan112)
            {
                isHasNoiseBiggerThan112 = value;
                ChangeNavigationState(new NavigationState_TargetPosition(npc, 63, position));
            }
        }

        public override void PlayerInSight(PlayerManager player)
        {
            base.PlayerInSight(player);
            isHasNoiseBiggerThan112 = 167;
            ChangeNavigationState(new NavigationState_TargetPosition(npc, 63, player.transform.position));
        }

        public override void WindowHit(Window window)
        {
            base.WindowHit(window);
            window.Break(false);
        }

        public override void DoorHit(StandardDoor door)
        {
            base.DoorHit(door);
            door.Unlock();
        }

        public override void DestinationEmpty()
        {
            base.DestinationEmpty();
            isHasNoiseBiggerThan112 = 0;
            ChangeNavigationState(new NavigationState_WanderRandom(npc, 63));
        }
    }
}
