using Il2CppHutongGames.PlayMaker;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(RedPoolMemoMod.RedPoolMemo), "RedPoolMemo", "1.0.0", "Blupe Rince")]
[assembly: MelonGame("Dogubomb", "BLUE PRINCE")]

namespace RedPoolMemoMod
{
    public class RedPoolMemo : MelonMod
    {
        private const string HEADER = "[RedPoolMemo]";
        private GameObject bluePaper;
        private GameObject blueLight;
        private GameObject redPaper;
        private GameObject rPaper;
        private GameObject redLight;
        private FsmVariables tentVariables;
        private bool isPool = false;
        private int RoomCount = 0;
        private GameObject roomPool;

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);

            MelonLogger.Msg($"{HEADER} {buildIndex}: {sceneName}");
            if (buildIndex != 2)
                return;

            try
            {
                FindRoomPool();
                MakePoolMemoRedAgain();
            }
            catch (Exception ex)
            {
                MelonLogger.Msg(System.ConsoleColor.Red, $"{HEADER} Error: {ex}");
            }
        }

        private void FindRoomPool()
        {
            const string go_roomPool = "Room Spawn Pools";

            roomPool = GameObject.Find(go_roomPool);
            if (roomPool == null)
            {
                MelonLogger.Msg(System.ConsoleColor.Yellow, $"{HEADER} GameObject '{go_roomPool}' not found");
                return;
            }
        }


        private void MakePoolMemoRedAgain()
        {
            //UI OVERLAY CAM/ UI Documents / DOCUMENTS / Tent Blue Memos - doc
            const string go_UiDocs = "UI Documents";
            const string go_docs = "DOCUMENTS";
            const string go_TentMemosDoc = "Tent Blue Memos - doc";

            //find ui docs first, the memo is inactive
            var uiDoc = GameObject.Find(go_UiDocs);
            if (uiDoc == null)
            {
                MelonLogger.Msg(System.ConsoleColor.Yellow, $"{HEADER} GameObject '{go_UiDocs}' not found");
                return;
            }
            var docs = uiDoc.transform.FindChild(go_docs);
            var tentMemos = docs.FindChild(go_TentMemosDoc).gameObject;
            if (tentMemos == null)
            {
                MelonLogger.Msg(System.ConsoleColor.Yellow, $"{HEADER} GameObject '{go_TentMemosDoc}' not found");
                return;
            }

            const string go_redMemo = "MEMO trove44 - doc";
            var redMemo = docs.FindChild(go_redMemo);
            if (redMemo == null)
            {
                MelonLogger.Msg(System.ConsoleColor.Yellow, $"{HEADER} GameObject '{go_redMemo}' not found");
                return;
            }

            bluePaper = tentMemos.transform.FindChild("paper").gameObject;
            blueLight = tentMemos.transform.FindChild("Document Light (2)").gameObject;

            redPaper = GameObject.Instantiate(redMemo.transform.FindChild("paper").gameObject, tentMemos.transform);
            redPaper.name = "red Paper";
            redPaper.transform.localScale = bluePaper.transform.localScale;

            redLight = GameObject.Instantiate(redMemo.transform.FindChild("Document Light (2)").gameObject, tentMemos.transform);
            redLight.name = "red Light";
            tentVariables = tentMemos.GetComponent<Il2Cpp.PlayMakerFSM>().FsmVariables;

        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            CheckForPoolRoomInRoomPool();

            if (tentVariables == null)
                return;

            const string referenceText = "There is only one location in the estate where every room in the drafting pool can be drafted.";
            bool isPoolMemo = tentVariables.stringVariables[0].RawValue.ToString().Contains(referenceText);

            //only change active when the memo changes to save on processing power or something idk
            if (isPoolMemo == isPool)
                return;

            isPool = isPoolMemo;
            bluePaper?.SetActive(!isPoolMemo);
            blueLight?.SetActive(!isPoolMemo);

            redPaper?.SetActive(isPoolMemo);
            redLight?.SetActive(isPoolMemo);
        }

        private void CheckForPoolRoomInRoomPool()
        {
            if (roomPool == null) return;
            if (roomPool.transform.childCount <= RoomCount) return;

            for (int i = RoomCount; i < roomPool.transform.childCount; i++)
            {
                var item = roomPool.transform.GetChild(i);

                //&& pools.Contains(item)
                if (item.name.Contains("The Pool"))
                {
                    var doc = item.transform.Find("_GAMEPLAY/MEMO CHECK/Tent Blue Memos - doc");
                    var bPaper = doc.transform.Find("paper");
                    var rePaper = GameObject.Find("Trunk red1").transform.Find("Trunk/MEMO SPAWN/MEMO RED1 - doc/paper");

                    var rPaper = GameObject.Instantiate(rePaper, doc);
                    rPaper.name = "red Paper";
                    rPaper.transform.localScale = bPaper.transform.localScale;

                    bPaper.gameObject.SetActive(false);
                }
            }
            RoomCount = roomPool.transform.childCount;
        }
    }
}
