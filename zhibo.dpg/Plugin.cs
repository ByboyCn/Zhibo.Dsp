using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace zhibo.dpg
{


    [BepInPlugin(Plugin.GUID, Plugin.NAME, Plugin.VERSION)]
    [BepInProcess(Plugin.GAME_PROCESS)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "zhibo.dsp.plugin";
        public const string VERSION = "1.0.0";
        public const string NAME = "直播插件";
        private const string GAME_PROCESS = "DSPGAME.exe";
        private static List<Liwu> LiWuList = new List<Liwu>();
        private static ConfigEntry<int> dianzaj;
        private static ConfigEntry<WuPinEnum> wupinid1;
        private static ConfigEntry<int> count1;
        private static ConfigEntry<string> liwu2;
        private static ConfigEntry<WuPinEnum> wupinid2;
        private static ConfigEntry<int> count2;
        private static ConfigEntry<string> liwu3;
        private static ConfigEntry<WuPinEnum> wupinid3;
        private static ConfigEntry<int> count3;
        private static ConfigEntry<string> liwu4;
        private static ConfigEntry<WuPinEnum> wupinid4;
        private static ConfigEntry<int> count4;
        private static ConfigEntry<string> liwu5;
        private static ConfigEntry<WuPinEnum> wupinid5;
        private static ConfigEntry<int> count5;
        public static WebSocketClient websocket = new WebSocketClient("ws://127.0.0.1:8888");
        private static List<int> strList = new List<int>();


        void Start()
        {
            liwu2 = Config.Bind("Zhibo", "liwuid2", "小心心", "礼物2");
            liwu3 = Config.Bind("Zhibo", "liwuid3", "小心心", "礼物3");
            liwu4 = Config.Bind("Zhibo", "liwuid4", "小心心", "礼物4");
            liwu5 = Config.Bind("Zhibo", "liwuid5", "小心心", "礼物5");
            wupinid1 = Config.Bind("Zhibo", "wupinid1", WuPinEnum.铁矿, new ConfigDescription("点赞物品id1", null, new WuPinEnum()));
            wupinid2 = Config.Bind("Zhibo", "wupinid2", WuPinEnum.铁矿, new ConfigDescription("物品id2", null, new WuPinEnum()));
            wupinid3 = Config.Bind("Zhibo", "wupinid3", WuPinEnum.铁矿, new ConfigDescription("物品id3", null, new WuPinEnum()));
            wupinid4 = Config.Bind("Zhibo", "wupinid4", WuPinEnum.铁矿, new ConfigDescription("物品id4", null, new WuPinEnum()));
            wupinid5 = Config.Bind("Zhibo", "wupinid5", WuPinEnum.铁矿, new ConfigDescription("物品id5", null, new WuPinEnum()));
            count1 = Config.Bind("Zhibo", "count1", 1, "点赞添加数量1");
            count2 = Config.Bind("Zhibo", "count2", 1, "添加数量2");
            count3 = Config.Bind("Zhibo", "count3", 1, "添加数量3");
            count4 = Config.Bind("Zhibo", "count4", 1, "添加数量4");
            count5 = Config.Bind("Zhibo", "count5", 1, "添加数量5");
            Logger.LogInfo($"直播插件加载成功");
            websocket.MessageReceived += Websocket_MessageReceived;
            websocket.ErrorOccurred += Websocket_ErrorOccurred;
            _ = websocket.ConnectAsync();
        }

        private void Websocket_ErrorOccurred(string obj)
        {
            Logger.LogError(obj);
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                _ = websocket.ConnectAsync();
            });
        }

        private void Websocket_MessageReceived(string obj)
        {
            Logger.LogInfo(obj);
            //var json = new JsonParser<Resp>(obj);
            //var person = json.Parse();
            //Logger.LogInfo("当前事件" + person.Type);
            //Logger.LogInfo("当前数据" + person.Data);
            if (GameMain.mainPlayer == null)
            {
                return;
            }
            string result = SubString(obj, "\"Type\":", ",\"Data\"");
            Logger.LogInfo("当前事件" + result);
            if (result == "2")
            {
                var tt = SubString(obj, "Count\\\":", ",\\\"Total");
                Logger.LogInfo("点赞数量" + tt);
                var count = int.Parse(tt);

                if (count > 0)
                {
                    AddToPackage((int)wupinid1.Value, count * count1.Value);
                    return;
                }
                AddToPackage((int)wupinid1.Value, count1.Value);


            }
            else if (result == "5")
            {
                var GiftName = SubString(obj, @"GiftName\"":\""", @"\"",\""GroupId");
                var count = int.Parse(SubString(obj, @"GiftCount\"":", @",\""RepeatCount"));
                if (GiftName == liwu2.Value)
                {
                    AddToPackage((int)wupinid2.Value, count * count2.Value);

                }
                else if (GiftName == liwu3.Value)
                {
                    AddToPackage((int)wupinid3.Value, count * count3.Value);
                }
                else if (GiftName == liwu4.Value)
                {
                    AddToPackage((int)wupinid4.Value, count * count4.Value);
                }
                else if (GiftName == liwu5.Value)
                {
                    AddToPackage((int)wupinid5.Value, count * count5.Value);
                }
            }
        }

        private static void AddToPackage(int itemid, int count)
        {
            GameMain.mainPlayer.TryAddItemToPackage(itemid, count, 0, false, 1);
            UIItemup.Up(itemid, count);
        }

        public string SubString(string or, string start, string end)
        {
            int startIndex = or.IndexOf(start) + start.Length;
            int endIndex = or.IndexOf(end);
            if (startIndex > 0 && endIndex > startIndex)
            {
                string result = or.Substring(startIndex, endIndex - startIndex);
                return result;
            }
            return or;
        }
        void Update()
        {


            liwu2 = Config.Bind("Zhibo", "liwuid2", "小心心", "礼物2");
            liwu3 = Config.Bind("Zhibo", "liwuid3", "小心心", "礼物3");
            liwu4 = Config.Bind("Zhibo", "liwuid4", "小心心", "礼物4");
            liwu5 = Config.Bind("Zhibo", "liwuid5", "小心心", "礼物5");
            wupinid1 = Config.Bind("Zhibo", "wupinid1", WuPinEnum.铁矿, new ConfigDescription("点赞物品id1", null, new WuPinEnum()));
            wupinid2 = Config.Bind("Zhibo", "wupinid2", WuPinEnum.铁矿, new ConfigDescription("礼物2物品", null, new WuPinEnum()));
            wupinid3 = Config.Bind("Zhibo", "wupinid3", WuPinEnum.铁矿, new ConfigDescription("礼物3物品", null, new WuPinEnum()));
            wupinid4 = Config.Bind("Zhibo", "wupinid4", WuPinEnum.铁矿, new ConfigDescription("礼物4物品", null, new WuPinEnum()));
            wupinid5 = Config.Bind("Zhibo", "wupinid5", WuPinEnum.铁矿, new ConfigDescription("礼物5物品", null, new WuPinEnum()));
            count1 = Config.Bind("Zhibo", "count1", 1, "点赞添加数量");
            count2 = Config.Bind("Zhibo", "count2", 1, "礼物2添加数量");
            count3 = Config.Bind("Zhibo", "count3", 1, "礼物3添加数量");
            count4 = Config.Bind("Zhibo", "count4", 1, "礼物4添加数量");
            count5 = Config.Bind("Zhibo", "count5", 1, "礼物5添加数量");
        }

    }

    public class Resp
    {
        public int Type { get; set; }
        public string Data { get; set; }
    }

}
