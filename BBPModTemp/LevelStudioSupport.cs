using MTM101BaldAPI;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using PlusLevelStudio.Editor.Tools;
using PlusStudioLevelFormat;
using UnityEngine;

namespace ItsBaldiTimeRework
{
    [ConditionalPatchMod("mtm101.rulerp.baldiplus.levelstudio")]
    public class LevelStudioSupport
    {
        internal static void Setup()
        {
            //EditorInterface.AddNPCVisual("LapPortal", BasePlugin.AssetMan.Get<NPC>("LapPortal"));
            EditorInterface.AddNPCVisual("Toppin", BasePlugin.AssetMan.Get<NPC>("Toppin"));
            EditorInterface.AddNPCVisual("BaldiFace", BasePlugin.AssetMan.Get<NPC>("BaldiFace"));
            EditorInterface.AddNPCVisual("FileName2", BasePlugin.AssetMan.Get<NPC>("FileName2"));

            LevelStudioPlugin.Instance.selectableTextures.Add("SecretRoomTile_Ceil");
            LevelStudioPlugin.Instance.selectableTextures.Add("SecretRoomTile_Floor");
            LevelStudioPlugin.Instance.selectableTextures.Add("SecretRoomTile_Wall");
            LevelStudioPlugin.Instance.defaultRoomTextures.Add(
                "SecretRoom",
                new TextureContainer(
                    "SecretRoomTile_Floor",
                    "SecretRoomTile_Wall",
                    "SecretRoomTile_Ceil"));

            EditorInterfaceModes.AddModeCallback(Add);
        }

        internal static void Add(EditorMode mode, bool vanillaCompliant)
        {
            //ItemObject
            void AddItemObject(string item)
            {
                EditorInterfaceModes.AddToolToCategory(mode, "items", new ItemTool(item, BasePlugin.AssetMan.Get<ItemObject>(item).itemSpriteSmall, true));
            }
            AddItemObject("BaldiClock");

            //NPC
            void AddNPC(string npc, string icon)
            {
                EditorInterfaceModes.AddToolToCategory(mode, "npcs", new NPCTool(npc, BasePlugin.AssetMan.Get<Sprite>(icon)));
            }
            //AddNpc("LapPortal", "npc_lapportal");
            AddNPC("Toppin", "npc_toppin");
            AddNPC("BaldiFace", "npc_baldiface");
            AddNPC("FileName2", "npc_filename2");

            //Poster
            void AddNpcPoster(string npc)
            {
                EditorInterfaceModes.AddToolToCategory(mode, "posters", new PosterTool(BasePlugin.AssetMan.Get<NPC>(npc).Poster.baseTexture.name));
            }
            AddNpcPoster("LapPortal");
            AddNpcPoster("Toppin");
            AddNpcPoster("BaldiFace");
            AddNpcPoster("FileName2");

            //Rooms
            void AddRoom(string room, string icon)
            {
                EditorInterfaceModes.AddToolToCategory(mode, "rooms", new RoomTool(room, BasePlugin.AssetMan.Get<Sprite>(icon)));
            }
            AddRoom("SecretRoom", "room_secret");
        }
    }
}
