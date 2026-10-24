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
                Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("sfx_collecttoppin"));
            }
            if (Other.CLC("skid.librarylib") && BaldiTimeActions.secretRoomFound.Count >= BaldiTimeActions.secretRoomTotal)
            {
                LibrarylibAchievementsSupport.RewardAchievement("SecretExplorer");
            }
        }
    }
}
