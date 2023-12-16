using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Byboy.LuckyDraw
{
    [BepInPlugin(Plugin.GUID,Plugin.NAME,Plugin.VERSION)]
    [BepInProcess(Plugin.GAME_PROCESS)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "zhibo.luckydraw.plugin";
        public const string VERSION = "1.0.0";
        public const string NAME = "幸运大抽奖";
        private const string GAME_PROCESS = "DSPGAME.exe";
        private bool IsFirst = true;
        private bool ready = false;
        private bool isRunning = false;

        private List<RecipeProto> list = new List<RecipeProto>();
        private static List<ItemIds> 物品 = new List<ItemIds>();
        private static List<ItemIds> 建筑 = new List<ItemIds>();
        private static List<ItemIds> 战斗 = new List<ItemIds>();

        Random r = new Random();
        List<RecipeProto> temp = new List<RecipeProto>();
        Type ldbType = null;
        FieldInfo recipesField = null;
        RecipeProtoSet recipse = null;


        void Start()
        {
            try {
                //跳过游戏异常检测
                Harmony.CreateAndPatchAll(typeof(AchievementLogicPatch),null);
                Harmony.CreateAndPatchAll(typeof(AbnormalityLogicPatch),null);
                GetItem();
                //通过反射获得LDB._recipes的值
                // 获取LDB类型的Type对象
                ldbType = typeof(LDB);
                // 使用反射获取私有静态成员 _recipes
                recipesField = ldbType.GetField("_recipes",BindingFlags.NonPublic | BindingFlags.Static);
                isRunning = true;
            } catch (Exception) {
                Logger.LogError("反射失败");
            }

        }

        private void GetItem()
        {
            try {
                Logger.LogInfo($"当前物品数量{物品.Count},当前建筑数量{建筑.Count},当前战斗数量{战斗.Count}");

                if (物品.Count == 0) {
                    if (!File.Exists("物品.txt")) {
                        #region itemIds
                        File.WriteAllText("物品.txt",@"1001  铁矿
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
1804  奇异湮灭燃料棒
1115  塑料
1123  石墨烯
1124  碳纳米管
1117  有机晶体
1118  钛晶石
1126  卡西米尔晶体
1128  燃烧单元
1129  爆破单元
1130  晶石爆破单元
1209  引力透镜
1210  空间翘曲器
1403  湮灭约束球
1407  动力引擎
1405  推进器
1406  加力推进器
5003  配送运输机
5001  物流运输机
5002  星际物流运输船
1125  框架材料
1502  戴森球组件
1503  小型运载火箭
1131  地基
1141  增产剂Mk.I
1142  增产剂Mk.II
1143  增产剂Mk.III
6001  电磁矩阵
6002  能量矩阵
6003  结构矩阵
6004  信息矩阵
6005  引力矩阵
6006  宇宙矩阵
1099  沙土");
                        #endregion
                    }
                    var txt = File.ReadAllText("物品.txt");
                    var tt = txt.Split('\n');
                    foreach (var t in tt) {
                        var t1 = t.Replace("  "," ");
                        var jj = t1.Split(' ');
                        if (jj.Length < 2) {
                            continue;
                        }
                        var ItemIds = new ItemIds() {
                            Id = int.Parse(jj[0]),
                            Name = jj[1],
                        };
                        物品.Add(ItemIds);
                    }
                }
                Logger.LogInfo("物品添加完成");
                if (建筑.Count == 0) {
                    if (!File.Exists("建筑.txt")) {
                        #region itemIds
                        File.WriteAllText("建筑.txt",@"1001  铁矿
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
2303  制造台Mk.I
2304  制造台Mk.II
2305  制造台Mk.III
2318  重组式制造台
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
2319  负熵熔炉
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
2902  自演化研究站");
                        #endregion
                    }
                    var txt = File.ReadAllText("建筑.txt");
                    var tt = txt.Split('\n');
                    foreach (var t in tt) {
                        var t1 = t.Replace("  "," ");
                        var jj = t1.Split(' ');
                        if (jj.Length < 2) {
                            continue;
                        }
                        var ItemIds = new ItemIds() {
                            Id = int.Parse(jj[0]),
                            Name = jj[1],
                        };
                        建筑.Add(ItemIds);
                    }
                }
                Logger.LogInfo("建筑添加完成");
                if (战斗.Count == 0) {
                    if (!File.Exists("战斗.txt")) {
                        #region itemIds
                        File.WriteAllText("战斗.txt",@"1001  铁矿
3001  高斯机枪塔
3002  高频激光塔
3003  聚爆加农炮
3004  磁化电浆炮
3005  导弹防御塔
3006  干扰塔
3007  信号塔
3008  行星护盾发生器
3009  战场分析基站
1601  机枪弹箱
1602  钛化弹箱
1603  超合金弹箱
1604  炮弹组
1605  高爆炮弹组
1606  晶石炮弹组
1607  等离子胶囊
1608  反物质胶囊
1609  导弹组
1610  超音速导弹组
1611  引力导弹组
5101  原型机
5102  精准无人机
5103  攻击无人机
5111  护卫舰
5112  驱逐舰
5201  黑雾矩阵
5202  硅基神经元
5203  物质重组器
5204  负熵奇点
5205  核心素
5206  能量碎片");
                        #endregion
                    }
                    var txt = File.ReadAllText("战斗.txt");
                    var tt = txt.Split('\n');
                    foreach (var t in tt) {
                        var t1 = t.Replace("  "," ");
                        var jj = t1.Split(' ');
                        if (jj.Length < 2) {
                            continue;
                        }
                        var ItemIds = new ItemIds() {
                            Id = int.Parse(jj[0]),
                            Name = jj[1],
                        };
                        战斗.Add(ItemIds);
                    }
                }
                Logger.LogInfo("战斗添加完成");
                Logger.LogInfo($"更新完成后物品数量{物品.Count},当前建筑数量{建筑.Count},当前战斗数量{战斗.Count}");
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }

        }

        void Update()
        {
            try {
                
                if (!isRunning) {
                    return;
                }
                if (IsFirst && LDB.recipes != null) {
                    Logger.LogInfo("初始化开始");

                    // 检查是否成功获取字段
                    if (recipesField == null) {
                        Logger.LogError("反射失败了");
                        return;
                    }
                    recipse = (RecipeProtoSet)recipesField.GetValue(null);
                    temp.AddRange(recipse.dataArray);


                    //这里处理配方的新增
                    RecipeProto rp = new RecipeProto {
                        ID = 10001,
                        Name = "幸运物品盲盒",
                        Type = ERecipeType.None,
                        //是否运行手搓
                        Handcraft = true,
                        //稀有公式都为true
                        Explicit = true,
                        //配方时间 60为1秒,难度逻辑帧率锁死的60?
                        TimeSpend = 1,
                        //输入的物品id
                        Items = new int[1] { 物品[0].Id },
                        //输入的数量
                        ItemCounts = new int[1] { 1 },
                        //输出的物品id
                        Results = new int[1] { 6006 },
                        //输出的数量
                        ResultCounts = new int[1] { 1 },
                        //决定配方存放的位置,例如1101
                        //第一位的1 感觉没啥用
                        //第二位的1 表示第1行
                        //第三四位的01 标识在第一列
                        //由于坐标是7*12的,所以第二位不能高于7 第三位不能高于1,第四位不能高于2
                        GridIndex = 1801,
                        //图标路径
                        IconPath = "Icons/Tech/1311",
                        //描述信息
                        Description = "这是一个物品盲盒",
                        //前置科技
                        //preTech = new TechProto

                    };
                    list.Add(rp);
                    //这里处理配方的新增
                    var rp1 = new RecipeProto {
                        ID = 10002,
                        Name = "幸运建筑盲盒",
                        Type = ERecipeType.None,
                        //是否运行手搓
                        Handcraft = true,
                        //稀有公式都为true
                        Explicit = true,
                        //配方时间 60为1秒,难度逻辑帧率锁死的60?
                        TimeSpend = 1,
                        //输入的物品id
                        Items = new int[1] { 建筑[0].Id },
                        //输入的数量
                        ItemCounts = new int[1] { 1 },
                        //输出的物品id
                        Results = new int[1] { 6006 },
                        //输出的数量
                        ResultCounts = new int[1] { 1 },
                        //决定配方存放的位置,例如1101
                        //第一位的1 感觉没啥用
                        //第二位的1 表示第1行
                        //第三四位的01 标识在第一列
                        //由于坐标是7*12的,所以第二位不能高于7 第三位不能高于1,第四位不能高于2
                        GridIndex = 1802,
                        //图标路径
                        IconPath = "Icons/Tech/1201",
                        //描述信息
                        Description = "这是一个建筑盲盒",
                        //前置科技
                        //preTech = new TechProto

                    };
                    list.Add(rp1);
                    var zd1 = new RecipeProto {
                        ID = 10003,
                        Name = "幸运战斗盲盒",
                        Type = ERecipeType.None,
                        //是否运行手搓
                        Handcraft = true,
                        //稀有公式都为true
                        Explicit = true,
                        //配方时间 60为1秒,难度逻辑帧率锁死的60?
                        TimeSpend = 1,
                        //输入的物品id
                        Items = new int[1] { 战斗[0].Id },
                        //输入的数量
                        ItemCounts = new int[1] { 1 },
                        //输出的物品id
                        Results = new int[1] { 6006 },
                        //输出的数量
                        ResultCounts = new int[1] { 1 },
                        //决定配方存放的位置,例如1101
                        //第一位的1 感觉没啥用
                        //第二位的1 表示第1行
                        //第三四位的01 标识在第一列
                        //由于坐标是7*12的,所以第二位不能高于7 第三位不能高于1,第四位不能高于2
                        GridIndex = 1803,
                        //图标路径
                        IconPath = "Icons/Tech/1801",
                        //描述信息
                        Description = "这是一个战斗盲盒",
                        //前置科技
                        //preTech = new TechProto

                    };
                    list.Add(zd1);
                    temp.AddRange(list);
                    recipse.dataArray = temp.ToArray();
                    recipesField.SetValue(null,recipse);

                    IsFirst = false;
                    ready = true;
                    Logger.LogInfo("初始化完成");

                }

                if (!IsFirst) {
                    var wp = temp.FirstOrDefault(t => t.ID == 10001);
                    wp.Results = new int[1] { 物品[r.Next(1,物品.Count)].Id };
                    var jz = temp.FirstOrDefault(t => t.ID == 10002);
                    jz.Results = new int[1] { 建筑[r.Next(1,建筑.Count)].Id };
                    var zd = temp.FirstOrDefault(t => t.ID == 10003);
                    zd.Results = new int[1] { 战斗[r.Next(1,战斗.Count)].Id };
                    recipse.dataArray = temp.ToArray();
                    recipesField.SetValue(null,recipse);
                }

                recipse.OnAfterDeserialize();
                if (ready && GameMain.history != null) {
                    //配方制作后需要解锁
                    foreach (var t in list) {
                        //解锁配方
                        GameMain.history.UnlockRecipe(t.ID);
                    }
                }
            } catch (Exception ex) {
                Logger.LogError(ex);
            }

        }
    }
}
