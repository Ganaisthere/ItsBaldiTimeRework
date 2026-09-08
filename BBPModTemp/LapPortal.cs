using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ItsBaldiTimeRework
{
    public class LapPortal : NPC
    {
        public Sprite[] sprites = new Sprite[2];
        public SoundObject Sfx_Enter;
        public SoundObject Sfx_Exit;
        public SoundObject Sfx_Lapping;
        public override void Initialize()
        {
            base.Initialize();
            spriteRenderer[0].sprite = sprites[0];
            behaviorStateMachine.ChangeState(new LapPortal_WaitForPlayerTouch(this));
        }
    }
    public class LapPortal_StateBase : NpcState
    {
        public LapPortal lapPortal;
        public LapPortal_StateBase(LapPortal me) : base(me)
        {
            lapPortal = me;
        }
    }
    public class LapPortal_WaitForPlayerTouch : LapPortal_StateBase
    {
        public LapPortal_WaitForPlayerTouch(LapPortal me) : base(me)
        {
            lapPortal = me;
        }
        public override void Enter()
        {
            base.Enter();
            ChangeNavigationState(new NavigationState_Disabled(npc));
        }
        public override void Update()
        {
            base.Update();
            if (BaldiTimeActions.lap > 0 && BaldiTimeActions.lap < 2 && BaldiTimeActions.itsBaldiTime && Singleton<BaseGameManager>.Instance.FoundNotebooks >= Singleton<BaseGameManager>.Instance.Ec.notebookTotal)
            {
                lapPortal.spriteRenderer[0].sprite = lapPortal.sprites[1];
            }
            else
            {
                lapPortal.spriteRenderer[0].sprite = lapPortal.sprites[0];
            }
            if (!Singleton<CoreGameManager>.Instance.GetPlayer(0).ec.map.arrowTargets.Contains(lapPortal.Entity))
            {
                Singleton<CoreGameManager>.Instance.GetPlayer(0).ec.map.AddArrow(lapPortal.Entity, new Color(1f, 1f, 0f, 1f));
            }
        }
        public override void OnStateTriggerStay(Entity otherEntity, Collider other, bool validCollision)
        {
            base.OnStateTriggerStay(otherEntity, other, validCollision);
            if (!validCollision || !other.CompareTag("Player") || BaldiTimeActions.enteringLap || !(BaldiTimeActions.lap > 0 && BaldiTimeActions.lap < 2 && BaldiTimeActions.itsBaldiTime && Singleton<BaseGameManager>.Instance.FoundNotebooks >= Singleton<BaseGameManager>.Instance.Ec.notebookTotal))
            {
                return;
            }
            BaldiTimeActions.enteringLap = true;
            BaldiTimeActions.AddCombo(1f);
            BaldiTimeActions.points += 3000f;
            lapPortal.StartCoroutine(LapEnteringAnimation(lapPortal));
        }
        public IEnumerator LapEnteringAnimation(LapPortal lapportal)
        {
            BaldiTimeActions.enteringLap = true;

            HudManager hudManager = Singleton<CoreGameManager>.Instance.GetHud(0);
            GameObject whiteFlash_Obj = new GameObject("WhiteFlash");
            whiteFlash_Obj.transform.SetParent(hudManager.Canvas().transform, false);
            RawImage whiteFlash = whiteFlash_Obj.AddComponent<RawImage>();
            whiteFlash.color = new Color(1f, 1f, 1f, 0f);
            whiteFlash.rectTransform.anchorMin = new Vector2(0f, 0f);
            whiteFlash.rectTransform.anchorMax = new Vector2(1f, 1f);

            Singleton<BaseGameManager>.Instance.Ec.PauseEnvironment(true);
            Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.Entity.SetFrozen(true);

            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(lapportal.Sfx_Enter);

            float timer = 0f;
            while (timer < 1f)
            {
                if (npc == null)
                {
                    yield break;
                }
                whiteFlash.color = new Color(1f, 1f, 1f, timer);
                timer += Time.deltaTime;
                yield return null;
            }
            whiteFlash.color = new Color(1f, 1f, 1f, 1f);

            Elevator elevatorSelect = null;
            List<float> Dists = new List<float>();
            foreach (Elevator elevator in Singleton<BaseGameManager>.Instance.Ec.Elevators)
            {
                float dist = Vector3.Distance(elevator.transform.position, Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position);
                Dists.Add(dist);
            }
            foreach (Elevator elevator in Singleton<BaseGameManager>.Instance.Ec.Elevators)
            {
                float dist = Vector3.Distance(elevator.transform.position, Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position);
                if (dist == Dists.Min())
                {
                    elevatorSelect = elevator;
                    break;
                }
            }
            if (elevatorSelect != null)
            {
                Singleton<CoreGameManager>.Instance.GetPlayer(0).Teleport(elevatorSelect.transform.position + elevatorSelect.transform.forward * 10f);
            }

            yield return new WaitForSeconds(0.5f);
            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(lapportal.Sfx_Exit);

            timer = 0f;
            while (timer < 1f)
            {
                if (npc == null)
                {
                    yield break;
                }
                whiteFlash.color = new Color(1f, 1f, 1f, 1f - timer);
                timer += Time.deltaTime;
                yield return null;
            }
            Object.Destroy(whiteFlash_Obj);

            Singleton<BaseGameManager>.Instance.Ec.PauseEnvironment(false);
            Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.Entity.SetFrozen(false);

            BaldiTimeActions.Lapping(Singleton<BaseGameManager>.Instance, lapportal.Sfx_Lapping);

            yield break;
        }
    }
}
