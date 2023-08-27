using BarrageGrab.Modles.JsonEntity;
using System;

namespace test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var json = "{\"Type\":2,\"Data\":\"{\\\"Count\\\":11,\\\"Total\\\":155999,\\\"MsgId\\\":7271863154539449124,\\\"User\\\":{\\\"FollowingCount\\\":142,\\\"Id\\\":69660987667,\\\"ShortId\\\":77077867,\\\"DisplayId\\\":\\\"77077867\\\",\\\"Nickname\\\":\\\"简简^O^丹丹\\\",\\\"Level\\\":1,\\\"PayLevel\\\":12,\\\"Gender\\\":2,\\\"HeadImgUrl\\\":\\\"https://p6.douyinpic.com/aweme/100x100/aweme-avatar/tos-cn-i-0813_ooQAXJvBeEfANA7IADgAN8DBa2nfwqAcSbwdnG.jpeg?from=3067671334\\\",\\\"SecUid\\\":\\\"MS4wLjABAAAAql_8iRQT6DmfuZImI75rpmnRXOkn9xiCy0Nrk6N9vGo\\\",\\\"FansClub\\\":{\\\"ClubName\\\":\\\"矮豆\\\",\\\"Level\\\":4},\\\"FollowerCount\\\":305,\\\"FollowStatus\\\":1},\\\"Content\\\":\\\"简简^O^丹丹 为主播点了11个赞，总点赞155999\\\",\\\"RoomId\\\":7271836504396237620}\"}";
            var tt = SimpleJsonParser.DeserializeObject<BarrageMsgPack>(json);
            Console.WriteLine(tt.Type);
            Console.WriteLine(tt.Data);
            var data = System.Text.RegularExpressions.Regex.Unescape(tt.Data);
            switch (tt.Type)
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
    }
    public class UserData
    {
        public int FollowingCount { get; set; }
        public long Id { get; set; }
        public int ShortId { get; set; }
        public string DisplayId { get; set; }
        public string Nickname { get; set; }
        public int Level { get; set; }
        public int PayLevel { get; set; }
        public int Gender { get; set; }
        public string HeadImgUrl { get; set; }
        public string SecUid { get; set; }
        public FansClubData FansClub { get; set; }
        public int FollowerCount { get; set; }
        public int FollowStatus { get; set; }
    }

    public class FansClubData
    {
        public string ClubName { get; set; }
        public int Level { get; set; }
    }

    public class MainData
    {
        public int CurrentCount { get; set; }
        public long MsgId { get; set; }
        public UserData User { get; set; }
        public string Content { get; set; }
        public long RoomId { get; set; }
    }

    public class Root
    {
        public int Type { get; set; }
        public string Data { get; set; }
        public MainData GetParsedData()
        {
            return SimpleJsonParser.DeserializeObject<MainData>(System.Text.RegularExpressions.Regex.Unescape(Data));
        }
    }

}
