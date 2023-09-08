using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace zhibo.dpg
{


    [BepInPlugin(Plugin.GUID, Plugin.NAME, Plugin.VERSION)]
    [BepInProcess(Plugin.GAME_PROCESS)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "zhibo.dsp.plugin";
        public const string VERSION = "1.0.2";
        public const string NAME = "直播插件";
        private const string GAME_PROCESS = "DSPGAME.exe";
        private static List<Liwu> LiWuList = new List<Liwu>();
        private static ConfigEntry<int> dianzaj;
        private static ConfigEntry<string> wupinid1;
        private static ConfigEntry<int> count1;
        private static ConfigEntry<string> liwu2;
        private static ConfigEntry<string> wupinid2;
        private static ConfigEntry<int> count2;
        private static ConfigEntry<string> liwu3;
        private static ConfigEntry<string> wupinid3;
        private static ConfigEntry<int> count3;
        private static ConfigEntry<string> liwu4;
        private static ConfigEntry<string> wupinid4;
        private static ConfigEntry<int> count4;
        private static ConfigEntry<string> liwu5;
        private static ConfigEntry<string> wupinid5;
        private static ConfigEntry<int> count5;
        public static WebSocketClient websocket = new WebSocketClient("ws://127.0.0.1:8888");
        private static List<int> strList = new List<int>();

        private static List<ItemProto> dataArray = new List<ItemProto>();// = LDB.items.dataArray.ToList();

        private void Steeing()
        {
          
            var acceptableValues = new AcceptableValueList<string>(dataArray.Select(i => i.Name).ToArray());

            //var selectedItemName = Config.Bind(
            //    "Zhibo",
            //    "wupinname1",
            //    "铁矿",  
            //    new ConfigDescription("Select an item from the dropdown", )
            liwu2 = Config.Bind("Zhibo", "liwuid2", "小心心", "礼物2");
            liwu3 = Config.Bind("Zhibo", "liwuid3", "小心心", "礼物3");
            liwu4 = Config.Bind("Zhibo", "liwuid4", "小心心", "礼物4");
            liwu5 = Config.Bind("Zhibo", "liwuid5", "小心心", "礼物5");
            wupinid1 = Config.Bind("Zhibo", "wupinid1", "铁矿", new ConfigDescription("点赞物品id1", acceptableValues));
            wupinid2 = Config.Bind("Zhibo", "wupinid2", "铁矿", new ConfigDescription("物品id2", acceptableValues));
            wupinid3 = Config.Bind("Zhibo", "wupinid3", "铁矿", new ConfigDescription("物品id3", acceptableValues));
            wupinid4 = Config.Bind("Zhibo", "wupinid4", "铁矿", new ConfigDescription("物品id4", acceptableValues));
            wupinid5 = Config.Bind("Zhibo", "wupinid5", "铁矿", new ConfigDescription("物品id5", acceptableValues));
            count1 = Config.Bind("Zhibo", "count1", 1, "点赞添加数量1");
            count2 = Config.Bind("Zhibo", "count2", 1, "添加数量2");
            count3 = Config.Bind("Zhibo", "count3", 1, "添加数量3");
            count4 = Config.Bind("Zhibo", "count4", 1, "添加数量4");
            count5 = Config.Bind("Zhibo", "count5", 1, "添加数量5");
        }

        void Start()
        {
            Thread.Sleep(1000 * 2);
            dataArray = null;
            dataArray = LDB.items.dataArray.ToList();
            Steeing();
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
        public void ProcessGift(string gift, int count)
        {
            if (gift == liwu2.Value)
            {
                AddToPackage(wupinid2.Value, count * count2.Value);
            }
            else if (gift == liwu3.Value)
            {
                AddToPackage(wupinid3.Value, count * count3.Value);
            }
            else if (gift == liwu4.Value)
            {
                AddToPackage(wupinid4.Value, count * count4.Value);
            }
            else if (gift == liwu5.Value)
            {
                AddToPackage(wupinid5.Value, count * count5.Value);
            }
        }

        private void Websocket_MessageReceived(string obj)
        {
            if (GameMain.mainPlayer == null)
            {
                return;
            }
            var tt1 = SimpleJsonParser.DeserializeObject<BarrageMsgPack>(obj);
            var data = System.Text.RegularExpressions.Regex.Unescape(tt1.Data);
            switch (tt1.Type)
            {
                case BarrageMsgType.无:
                    break;
                case BarrageMsgType.弹幕消息:
                    {
                        var msg = SimpleJsonParser.DeserializeObject<Msg>(data);
                        break;
                    }
                case BarrageMsgType.点赞消息:
                    {
                        var msg = SimpleJsonParser.DeserializeObject<LikeMsg>(data);
                        var count = (int)msg.Count;
                        if (count > 0)
                        {
                            AddToPackage(wupinid1.Value, count * count1.Value);
                            return;
                        }
                        AddToPackage(wupinid1.Value, count1.Value);
                        break;
                    }
                case BarrageMsgType.进直播间:
                    {
                        var msg = SimpleJsonParser.DeserializeObject<MemberMessage>(data);
                        break;
                    }
                case BarrageMsgType.关注消息:
                    {
                        var msg = SimpleJsonParser.DeserializeObject<GiftMsg>(data);
                        break;
                    }
                case BarrageMsgType.礼物消息:
                    {
                        var msg = SimpleJsonParser.DeserializeObject<GiftMsg>(data);
                        ProcessGift(msg.GiftName, (int)msg.GiftCount);
                        break;
                    }
                case BarrageMsgType.直播间统计:
                    {
                        var msg = SimpleJsonParser.DeserializeObject<UserSeqMsg>(data);
                        break;
                    }
                case BarrageMsgType.粉丝团消息:
                    {
                        var msg = SimpleJsonParser.DeserializeObject<FansclubMsg>(data);
                        break;
                    }
                    break;
                case BarrageMsgType.直播间分享:
                    {
                        var msg = SimpleJsonParser.DeserializeObject<ShareMessage>(data);
                        break;
                    }
                case BarrageMsgType.下播:
                    break;
                default:
                    break;
            }
        }
        
        private static void AddToPackage(string itemname, int count)
        {
            var item = dataArray.FirstOrDefault(i => i.Name == itemname);
            GameMain.mainPlayer.TryAddItemToPackage(item.ID, count, 0, false, 1);
            UIItemup.Up(item.ID, count);
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
            Steeing();
        }
    }
}
