using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs
{
    public class NewsfeedItemWebModel
    {
        public NewsfeedItemTypeEnum ItemType { get; set; }
        public int ID { get; set; }
        public int AthleteID { get; set; }
        public string AthleteName { get; set; }
        public string AthletePicture { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public DateTime? TimeOfEvent { get; set; }
        public DateTime? TimeOfEventLocal { get; set; }

        public int Athlete2ID { get; set; }
        public string Athlete2Name { get; set; }
        public string Athlete2Picture { get; set; }

        public string VenueName { get; set; }
        public int VenueID { get; set; }

        public int CommentsCount { get; set; }
        public int LikesCount { get; set; }
        public bool Liked { get; set; }
    }

    public class NewsfeedWebModel
    {
        /// <summary>
        /// Maximum number items returned by the API
        /// </summary>
        public const int MaxItems = 100;

        public List<NewsfeedItemWebModel> Items { get; set; }
    }

    public class CommentWebModel
    {
        public int ID { get; set; }
        public int AthleteID { get; set; }
        public string AthleteName { get; set; }
        public string AthletePicture { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
    }

    //public class NewsfeedWithCommentsWebModel
    //{
    //    public List<NewsfeedItemWithCommentsModel> Items { get; set; }
    //}

    //public class NewsfeedItemWithCommentsModel
    //{
    //    public NewsfeedItemWebModel Item { get; set; }
    //    public List<CommentWebModel> Comments { get; set; }
    //}

    public class NewPostWebModel
    {
        public string Country { get; set; }
        public int MetroID { get; set; }
        public string Text { get; set; }
    }

    public class NewCommentWebModel
    {
        public int ObjectID { get; set; }
        public NewsfeedItemTypeEnum ObjectType { get; set; }
        public string Text { get; set; }
    }

    public class SetLikeWebModel
    {
        public int ObjectID { get; set; }
        public NewsfeedItemTypeEnum ObjectType { get; set; }
        public bool Like { get; set; }
    }
}
