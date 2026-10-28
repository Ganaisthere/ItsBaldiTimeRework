using UnityEngine;

namespace ItsBaldiTimeRework
{
    public class SecretRoomFunction : RoomFunction
    {
        public override void OnGenerationFinished()
        {
            base.OnGenerationFinished();
            foreach (Cell cell in room.cells)
            {
                cell.hideFromMap = true;
            }
            room.entitySafeCells.Clear();
            room.eventSafeCells.Clear();
        }

        public override void OnPlayerEnter(PlayerManager player)
        {
            base.OnPlayerEnter(player);
            if (!BaldiTimeActions.secretRoomFound.Contains(room))
            {
                BaldiTimeActions.secretRoomFound.Add(room);
                if (Singleton<CoreGameManager>.Instance.GetHud(0).transform.TryFind("BaldiTimeHudManager", out Transform transform))
                {
                    BaldiTimeHudManager manager = transform.gameObject.GetComponent<BaldiTimeHudManager>();
                    manager.StartCoroutine(manager.YouFoundSecret());
                }
            }
            if (Other.CLC("skid.librarylib") && BaldiTimeActions.secretRoomFound.Count >= BaldiTimeActions.secretRoomTotal)
            {
                LibrarylibAchievementsSupport.RewardAchievement("SecretExplorer");
            }
        }
    }
}
