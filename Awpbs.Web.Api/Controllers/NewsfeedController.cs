using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Awpbs.Web.Api.Controllers
{
    [RoutePrefix("api/Newsfeed")]
    public class NewsfeedController : ApiController
    {
        private ApplicationDbContext db;

        public NewsfeedController()
        {
            this.db = new ApplicationDbContext();
        }

        [HttpGet]
        [Route("SingleFromCommentID")]
        public NewsfeedItemWebModel SingleFromCommentID(int commentID)
        {
            int myAthleteID = new UserProfileLogic(db).GetAthleteIDForUserName(User.Identity.Name);

            var comment = db.Comments.Where(i => i.CommentID == commentID).Single();
            int objectID = new FeedLogic(db).GetObjectIDFromComment(comment);
            return new FeedLogic(db).GetNewsfeedItem(myAthleteID, (NewsfeedItemTypeEnum)comment.ObjectType, objectID);
        }

        [HttpGet]
        [Route("Metro")]
        public NewsfeedWebModel Metro(int id)
        {
            int myAthleteID = 0;
            if (User.Identity.IsAuthenticated)
                myAthleteID = new UserProfileLogic(db).GetAthleteIDForUserName(User.Identity.Name);
            var newsfeed = new FeedLogic(db).GetNewsfeedForMetro(myAthleteID, id);
            return newsfeed;
        }

        [HttpGet]
        [Route("Country")]
        public NewsfeedWebModel Country(string countryCode)
        {
            int myAthleteID = 0;
            if (User.Identity.IsAuthenticated)
                myAthleteID = new UserProfileLogic(db).GetAthleteIDForUserName(User.Identity.Name);
            if (countryCode == "all")
                return new FeedLogic(db).GetGlobalNewsfeed(myAthleteID);
            else
                return new FeedLogic(db).GetNewsfeedForCountry(myAthleteID, Awpbs.Country.Get(countryCode));
        }

        [HttpGet]
        [Route("Comments")]
        public List<CommentWebModel> Comments(int objectType, int objectID)
        {
            return new FeedLogic(db).GetComments((NewsfeedItemTypeEnum)objectType, objectID);
        }

        [Authorize]
        [HttpPost]
        [Route("MakePost")]
        public int MakePost(NewPostWebModel model)
        {
            int myAthleteID = new UserProfileLogic(db).GetAthleteIDForUserName(User.Identity.Name);
            if (model.MetroID > 0)
            {
                return new FeedLogic(db).MakePost(myAthleteID, model.MetroID, model.Text);
            }
            else
            {
                var country = Awpbs.Country.Get(model.Country);
                return new FeedLogic(db).MakePost(myAthleteID, country, model.Text);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("MakeComment")]
        public async Task<int> MakeComment(NewCommentWebModel model)
        {
            int myAthleteID = new UserProfileLogic(db).GetAthleteIDForUserName(User.Identity.Name);
            int id = await new FeedLogic(db).MakeComment(myAthleteID, model.ObjectType, model.ObjectID, model.Text, true);

            return id;
        }

        [Authorize]
        [HttpPost]
        [Route("DeleteComment")]
        public bool DeleteComment(int commentID)
        {
            int myAthleteID = new UserProfileLogic(db).GetAthleteIDForUserName(User.Identity.Name);
            var comment = db.Comments.Where(i => i.CommentID == commentID).Single();
            if (comment.AthleteID != myAthleteID)
                throw new Exception("Cannot delete someone else's comment");
            db.Comments.Remove(comment);
            db.SaveChanges();
            return true;
        }

        [Authorize]
        [HttpPost]
        [Route("SetLike")]
        public int SetLike(SetLikeWebModel model)
        {
            int myAthleteID = new UserProfileLogic(db).GetAthleteIDForUserName(User.Identity.Name);
            new FeedLogic(db).SetLike(myAthleteID, model.ObjectType, model.ObjectID, model.Like);
            return 1;
        }


    }
}