using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public bool ShowWindow;
        public bool FirstOpen = true;
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

        private static List<ItemIds> dataArray = new List<ItemIds>();

        public float Window_Width { get; private set; }
        public float Window_Height { get; private set; }

        public float Window_X  = 450f;
        public float Window_Y  = 200f;
        public float Temp_Window_Moving_X;
        public float Temp_Window_Moving_Y;
        public float Temp_Window_X_move;
        public float Temp_Window_Y_move;
        public bool Window_moving;

        private void GetItem()
        {
            if (dataArray.Count == 0)
            {
                if (!File.Exists("itemids.txt"))
                {
                    #region itemIds
                    File.WriteAllText("itemids.txt", @"1001  铁矿
1002  铜矿
1003  硅石
1004  钛石
1005  石矿
1006  煤矿
1030  木材
1031  植物燃料
1011  可燃冰
1012  金伯利矿石
1013  分形硅石
1014  光栅石
1015  刺笋结晶
1016  单极磁石
1101  铁块
1104  铜块
1105  高纯硅块
1106  钛块
1108  石材
1109  高能石墨
1103  钢材
1107  钛合金
1110  玻璃
1119  钛化玻璃
1111  棱镜
1112  金刚石
1113  晶格硅
1201  齿轮
1102  磁铁
1202  磁线圈
1203  电动机
1204  电磁涡轮
1205  超级磁场环
1206  粒子容器
1127  奇异物质
1301  电路板
1303  处理器
1305  量子芯片
1302  微晶元件
1304  位面过滤器
1402  粒子宽带
1401  电浆激发器
1404  光子合并器
1501  太阳帆
1000  水
1007  原油
1114  精炼油
1116  硫酸
1120  氢
1121  重氢
1122  反物质
1208  临界光子
1801  液氢燃料棒
1802  氘核燃料棒
1803  反物质燃料棒
1115  塑料
1123  石墨烯
1124  碳纳米管
1117  有机晶体
1118  钛晶石
1126  卡西米尔晶体
1209  引力透镜
1210  空间翘曲器
1403  湮灭约束球
1405  推进器
1406  加力推进器
5003  配送运输机
5001  物流运输机
5002  星际物流运输船
1125  框架材料
1502  戴森球组件
1503  小型运载火箭
1131  地基
1141  增产剂 Mk.I
1142  增产剂 Mk.II
1143  增产剂 Mk.III
2001  传送带
2002  高速传送带
2003  极速传送带
2011  分拣器
2012  高速分拣器
2013  极速分拣器
2020  四向分流器
2040  自动集装机
2030  流速监测器
2313  喷涂机
2107  物流配送器
2101  小型储物仓
2102  大型储物仓
2106  储液罐
2303  制造台 Mk.I
2304  制造台 Mk.II
2305  制造台 Mk.III
2201  电力感应塔
2202  无线输电塔
2212  卫星配电站
2203  风力涡轮机
2204  火力发电厂
2211  微型聚变发电站
2213  地热发电站
2301  采矿机
2316  大型采矿机
2306  抽水站
2302  电弧熔炉
2315  位面熔炉
2307  原油萃取站
2308  原油精炼厂
2309  化工厂
2317  量子化工厂
2314  分馏塔
2205  太阳能板
2206  蓄电器
2207  蓄电器（满）
2311  电磁轨道弹射器
2208  射线接收站
2312  垂直发射井
2209  能量枢纽
2310  微型粒子对撞机
2210  人造恒星
2103  行星内物流运输站
2104  星际物流运输站
2105  轨道采集器
2901  矩阵研究站
3001  -
3002  |
3003  --
3004  -|
3005  |-
3006  ---
3007  --|
6001  电磁矩阵
6002  能量矩阵
6003  结构矩阵
6004  信息矩阵
6005  引力矩阵
6006  宇宙矩阵");
                    #endregion
                }
                var txt = File.ReadAllText("itemids.txt");
                var tt = txt.Split('\n');
                foreach (var t in tt)
                {
                    var t1 = t.Replace("  ", " ");
                    var jj = t1.Split(' ');
                    var ItemIds = new ItemIds()
                    {
                        Id = int.Parse(jj[0]),
                        Name = jj[1],
                    };
                    dataArray.Add(ItemIds);
                }
            }

        }
        private void Steeing()
        {

            var acceptableValues = new AcceptableValueList<string>(dataArray.Select(i => i.Name).ToArray());
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
            this.ShowWindow = false;
            GetItem();
            Thread.Sleep(5 * 1000);
            Steeing();
            Logger.LogInfo($"直播插件加载成功");
            websocket.MessageReceived += Websocket_MessageReceived;
            websocket.ErrorOccurred += Websocket_ErrorOccurred;
            _ = websocket.ConnectAsync();
            this.Temp_Window_Moving_X = 0f;
            this.Temp_Window_Moving_Y = 0f;
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
            GameMain.mainPlayer.TryAddItemToPackage(item.Id, count, 0, false, 1);
            UIItemup.Up(item.Id, count);
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

        void OnGUI()
        {
            bool showWindow = this.ShowWindow;
            if (showWindow)
            {
                //KeyCode
                if (this.FirstOpen)
                {
                    FirstOpen = false;
                    GUI.skin.label.fontSize = 16;
                    GUI.skin.button.fontSize =16;
                    GUI.skin.toggle.fontSize = 16;
                    GUI.skin.textField.fontSize = 16;
                    GUI.skin.textArea.fontSize = 16;
                    this.Window_Width = (float)(16 * 15);
                    this.Window_Height = (float)((16 + 4) * 5);
                }
                int num = GUI.skin.toggle.fontSize + 4;
                this.Window_Height = (float)(num * 13);
                Rect rect = new Rect(this.Window_X, this.Window_Y, this.Window_Width, this.Window_Height);
                this.moveWindow(ref this.Window_X, ref this.Window_Y, ref this.Temp_Window_X_move, ref this.Temp_Window_Y_move, ref this.Window_moving, ref this.Temp_Window_Moving_X, ref this.Temp_Window_Moving_Y, this.Window_Width);
                rect = GUI.Window(20231005, rect, new GUI.WindowFunction(this.ZhiboConfigWindow), NAME+"配置" + VERSION);
            }
        }

        private void ZhiboConfigWindow(int id)
        {

        }

        public void moveWindow(ref float x, ref float y, ref float x_move, ref float y_move, ref bool movewindow, ref float tempx, ref float tempy, float x_width)
        {
            Vector2 vector = Input.mousePosition;
            int height = Screen.height;
            bool flag = vector.x > x && vector.x < x + x_width && (float)height - vector.y > y && (float)height - vector.y < y + 20f;
            if (flag)
            {
                bool mouseButton = Input.GetMouseButton(0);
                if (mouseButton)
                {
                    bool flag2 = !movewindow;
                    if (flag2)
                    {
                        x_move = x;
                        y_move = y;
                        tempx = vector.x;
                        tempy = (float)height - vector.y;
                    }
                    movewindow = true;
                    x = x_move + vector.x - tempx;
                    y = y_move + ((float)height - vector.y) - tempy;
                }
                else
                {
                    movewindow = false;
                    tempx = x;
                    tempy = y;
                }
            }
            else
            {
                bool flag3 = movewindow;
                if (flag3)
                {
                    movewindow = false;
                    x = x_move + vector.x - tempx;
                    y = y_move + ((float)height - vector.y) - tempy;
                }
            }
        }
        void Update()
        {

            Steeing();
        }
    }
}
