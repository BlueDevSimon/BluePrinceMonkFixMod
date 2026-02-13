using MelonLoader;
using UnityEngine;
using Il2Cpp;
using Il2CppHutongGames.PlayMaker;
using System.ComponentModel;

[assembly: MelonInfo(typeof(BluePrinceMonkFixMod.MonkFix), "MonkFix", "1.0.0", "Blupe Rince")]
[assembly: MelonGame("Dogubomb", "BLUE PRINCE")]

namespace BluePrinceMonkFixMod
{
    public class MonkFix : MelonMod
    {
        private const string HEADER = "[MonkFix]";

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);

            MelonLogger.Msg($"{HEADER} {buildIndex}: {sceneName}");
            if (buildIndex != 2)
                return;

            try
            {
                AddMissingRoomsToIdTable();
            }
            catch (Exception ex)
            {
                MelonLogger.Msg(System.ConsoleColor.Red, $"{HEADER} Error: {ex}");
            }
        }

        Dictionary<string, string> missingRooms = new()
            {
                { "Speakeasy", "Billiard Room" },
                { "Break Room", "Billiard Room" },
                { "Funeral Parlor", "Parlor" },
                { "Throne of the Blue Prince", "Throne Room" },
            };

        private void AddMissingRoomsToIdTable()
        {
            const string go_RoomIDTable = "Room ID Table";
            var roomIDtable = GameObject.Find(go_RoomIDTable);
            if (roomIDtable == null)
            {
                MelonLogger.Msg(System.ConsoleColor.Yellow, $"{HEADER} GameObject '{go_RoomIDTable}' not found");
                return;
            }
            MelonLogger.Msg(System.ConsoleColor.Green, $"{HEADER} GameObject '{go_RoomIDTable}' was found");
            var hashtable = roomIDtable.GetComponent<PlayMakerHashTableProxy>().hashTable;

            if (hashtable == null)
                return;

            //Speakeasy
            //Break Room
            //Funeral Parlor
            //Throne of the Blue Prince
            foreach (var item in missingRooms)
            {
                AddRoomID(hashtable, item.Key, item.Value);
            }

            //Fix Sauna spelling
            const string sauan = "Sauan";
            if (hashtable.ContainsKey(sauan))
            {
                var sauanValue = hashtable[sauan];
                hashtable.Remove(sauan);
                hashtable.Add("Sauna", sauanValue);
            }

            return;
        }

        private void AddRoomID(Il2CppSystem.Collections.Hashtable hashtable, string roomID, string baseRoomName)
        {
            if (!hashtable.ContainsKey(roomID))
                hashtable.Add(roomID, hashtable[baseRoomName]);
        }
    }
}
