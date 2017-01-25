using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs
{
    public enum PushNotificationMessageTypeEnum
    {
        FriendRequest = 1,
        PrivateMessage = 2,
        Comment = 3,

        GameInvite = 101,
        GameInviteAccepted = 102,
        GameInviteDeclined = 103,
        GameWantsToBeInvited = 104,
        GameApprovedByHost = 105,
        GameInviteComment = 106,
    }

    public class PushNotificationMessage
    {
        public string Text { get; set; }

        public int ObjectID { get; set; }

        public static PushNotificationMessage BuildFriendRequest(Athlete athlete)
        {
            string name = athlete.Name ?? "";
            if (name.Length > 20)
                name = name.Substring(0, 20);
            string text = string.Format("Friend request from '{0}'", name);
            return new PushNotificationMessage() { Text = text, ObjectID = athlete.AthleteID };
        }

        public static PushNotificationMessage BuildPrivateMessage(Athlete athlete, string message)
        {
            string name = athlete.Name ?? "";
            if (name.Length > 20)
                name = name.Substring(0, 20);
            string text = string.Format("Message from '{0}' : {1}", name, message);
            return new PushNotificationMessage() { Text = text, ObjectID = athlete.AthleteID };
        }

        public static PushNotificationMessage BuildComment(Athlete athlete, string comment, int commentID)
        {
            string text = string.Format("Comment on a tracked conversation from '{0}'", athlete.NameOrUnknown);
            return new PushNotificationMessage() { Text = text, ObjectID = commentID };
        }

        public static PushNotificationMessage BuildGameMessage(PushNotificationMessageTypeEnum type, Athlete athlete, int gameHostID)
        {
            string name = athlete.Name ?? "";
            if (name.Length > 20)
                name = name.Substring(0, 20);
            string text;
            switch (type)
            {
                case PushNotificationMessageTypeEnum.GameInvite: text = string.Format("Game invite from '{0}'", name); break;
                case PushNotificationMessageTypeEnum.GameInviteAccepted: text = string.Format("Invite accepted by '{0}'", name); break;
                case PushNotificationMessageTypeEnum.GameInviteDeclined: text = string.Format("Invite declined by '{0}'", name); break;
                case PushNotificationMessageTypeEnum.GameWantsToBeInvited: text = string.Format("Wants to be invited: '{0}'", name); break;
                case PushNotificationMessageTypeEnum.GameApprovedByHost: text = string.Format("Approved by '{0}'. Come play!", name); break;
                default: throw new Exception("Invalid type: " + type.ToString());
            }
            return new PushNotificationMessage() { Text = text, ObjectID = gameHostID };
        }

        public static PushNotificationMessage BuildGameCommentMessage(Athlete athlete, string commentText, int gameHostID)
        {
            string name = athlete.Name ?? "";
            if (name.Length > 20)
                name = name.Substring(0, 20);
            string text = string.Format("Comment from '{0}' : {1}", name, commentText);
            return new PushNotificationMessage() { Text = text, ObjectID = gameHostID };
        }

        public static PushNotificationMessageTypeEnum? ParseType(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            if (text.StartsWith("Friend request"))
                return PushNotificationMessageTypeEnum.FriendRequest;
            if (text.StartsWith("Message from"))
                return PushNotificationMessageTypeEnum.PrivateMessage;
            if (text.StartsWith("Comment on"))
                return PushNotificationMessageTypeEnum.Comment;
            if (text.StartsWith("Game invite"))
                return PushNotificationMessageTypeEnum.GameInvite;
            if (text.StartsWith("Invite accepted"))
                return PushNotificationMessageTypeEnum.GameInviteAccepted;
            if (text.StartsWith("Invite declined"))
                return PushNotificationMessageTypeEnum.GameInviteDeclined;
            if (text.StartsWith("Wants to be invited"))
                return PushNotificationMessageTypeEnum.GameWantsToBeInvited;
            if (text.StartsWith("Approved by"))
                return PushNotificationMessageTypeEnum.GameApprovedByHost;
            if (text.StartsWith("Comment from"))
                return PushNotificationMessageTypeEnum.GameInviteComment;

            return null;
        }
    }
}
