using UnityEngine;

namespace ItsBaldiTimeRework
{
    public class ITM_BaldiClock : Item
    {
        [SerializeField]
        public float staminaValue = 25f;
        public int pointValue = 15;

        public override bool Use(PlayerManager pm)
        {
            Singleton<CoreGameManager>.Instance.AddPoints(pointValue, pm.playerNumber, true, true, true);
            pm.plm.AddStamina(staminaValue, true);
            return true;
        }
    }
}
