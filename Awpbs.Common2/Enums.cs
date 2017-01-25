using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    public enum MobileAppEnum
    {
        //Full = 1,
        Snooker = 2,
        SnookerForVenues = 3,
    }

    public enum MobilePlatformEnum
    {
        IOS = 1,
        Android = 2,
    }

    public enum GenderEnum
    {
        Unknown = 0,
        Male = 1,
        Female = 2,
    }

    public enum PersonFriendshipEnum
    {
        Unknown = 0,
        NotFriend = 1,
        RequestSent = 2,
        Friend = 3
    }

    public enum FriendshipStatusEnum
    {
        Initiated = 0,
        Declined = -1,
        Confirmed = 1
    }

    //public enum WorldRecordEnum
    //{
    //    None = 0,
    //    WorldRecord = 1,
    //    FormerWorldRecord = 2,
    //}

    public enum AgeGroupEnum
    {
        None = 0,
        MOpen = 1000,
        M35 = 1035,
        M40 = 1040,
        M45 = 1045,
        M50 = 1050,
        M55 = 1055,
        M60 = 1060,
        M65 = 1065,
        M70 = 1070,
        M75 = 1075,
        M80 = 1080,
        FOpen = 2000,
        F35 = 2035,
        F40 = 2040,
        F45 = 2045,
        F50 = 2050,
        F55 = 2055,
        F60 = 2060,
        F65 = 2065,
        F70 = 2070,
        F75 = 2075,
        F80 = 2080,
    }

    public enum VenueEditTypeEnum
    {
        ConfirmOnly = 0,
        EditedMeta = 1
    }

    public enum OpponentConfirmationEnum
    {
        NotYet = 0,
        Confirmed = 1,
        Declined = 2
    }

    public enum BackgroundEnum
    {
        Black = 0,
        White = 255,
        Background1 = 2
    }

    public enum PushNotificationStatusEnum
    {
        Prepared = 0,
        Fired = 1,
        FailedWithTimeoutOnFire = 101,
        FailedWithSendError = 102,
        SentOk = 1000
    }

    public enum CommentTypeEnum
    {
        Comment = 1,
        Like = 2,
    }

    //public enum CommentObjectTypeEnum
    //{
    //    Post = 1,
    //    Result = 2,
    //    Score = 3,
    //}

    public enum NewsfeedItemTypeEnum
    {
        Post = 1,
        Result = 2,
        Score = 3,
        GameHost = 4,
        NewUser = 5,
    }

    public enum EventTypeEnum
    {
        PublicAcceptRequired = 0,
        PublicAcceptNotRequired = 1,
        Private = 999,
    }
}
