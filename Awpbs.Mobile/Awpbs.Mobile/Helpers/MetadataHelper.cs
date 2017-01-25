using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs.Mobile
{
    public class MetadataHelper
    {
        public SnookerMatchMetadata CreateDefault()
        {
            var myAthlete = App.Repository.GetMyAthlete();

            var metadata = new SnookerMatchMetadata();
            metadata.Date = DateTime.Now;
            metadata.TableSize = SnookerTableSizeEnum.Table12Ft;
            metadata.PrimaryAthleteID = myAthlete.AthleteID;
            metadata.PrimaryAthleteName = myAthlete.Name;
            metadata.PrimaryAthletePicture = myAthlete.Picture;

            return metadata;
        }

        public SnookerMatchMetadata FromBreak(SnookerBreak snookerBreak)
        {
            var me = App.Repository.GetMyAthlete();

            SnookerMatchMetadata metadata = new SnookerMatchMetadata()
            {
                 Date = snookerBreak.Date,
                 PrimaryAthleteID = me.AthleteID,
                 PrimaryAthleteName = me.Name,
                 PrimaryAthletePicture = me.Picture,
                 OpponentAthleteID = snookerBreak.OpponentAthleteID,
                 OpponentAthleteName = snookerBreak.OpponentName,
                 OpponentPicture = null,
                 TableSize = snookerBreak.TableSize,
                 VenueID = snookerBreak.VenueID,
                 VenueName = snookerBreak.VenueName,
            };

            if (metadata.OpponentAthleteID > 0)
            {
                var person = App.Cache.People.Get(metadata.OpponentAthleteID);
                if (person != null)
                {
                    metadata.OpponentAthleteName = person.Name;
                    metadata.OpponentPicture = person.Picture;
                }
            }

            return metadata;
        }

        public SnookerMatchMetadata FromScoreForYou(SnookerMatchScore score)
        {
            var me = App.Repository.GetMyAthlete();

            SnookerMatchMetadata metadata = new SnookerMatchMetadata()
            {
                Date = score.Date,
                PrimaryAthleteID = me.AthleteID,
                PrimaryAthleteName = me.Name ?? "",
                PrimaryAthletePicture = me.Picture,
                OpponentAthleteID = score.OpponentAthleteID,
                OpponentAthleteName = score.OpponentName ?? "",
                OpponentPicture = score.OpponentPicture,
                TableSize = score.TableSize,
                VenueID = score.VenueID,
                VenueName = score.VenueName,
            };

            if (metadata.OpponentAthleteID > 0)
            {
                var person = App.Cache.People.Get(metadata.OpponentAthleteID);
                if (person != null)
                {
                    metadata.OpponentAthleteName = person.Name;
                    metadata.OpponentPicture = person.Picture;
                }
            }

            if (metadata.VenueID > 0)
            {
                var venue = App.Cache.Venues.Get(metadata.VenueID);
                if (venue != null)
                {
                    metadata.VenueName = venue.Name;
                }
            }

            return metadata;
        }

        public SnookerMatchMetadata FromScoreForOpponent(SnookerMatchScore score)
        {
            var me = App.Repository.GetMyAthlete();

            SnookerMatchMetadata metadata = new SnookerMatchMetadata()
            {
                Date = score.Date,
                OpponentAthleteID = me.AthleteID,
                OpponentAthleteName = me.Name ?? "",
                OpponentPicture = me.Picture,
                PrimaryAthleteID = score.OpponentAthleteID,
                PrimaryAthleteName = score.OpponentName ?? "",
                PrimaryAthletePicture = score.OpponentPicture,
                TableSize = score.TableSize,
                VenueID = score.VenueID,
                VenueName = score.VenueName,
            };

            if (metadata.PrimaryAthleteID > 0)
            {
                var person = App.Cache.People.Get(metadata.PrimaryAthleteID);
                if (person != null)
                    metadata.PrimaryAthletePicture = person.Picture;
            }

            if (metadata.VenueID > 0)
            {
                var venue = App.Cache.Venues.Get(metadata.VenueID);
                if (venue != null)
                {
                    metadata.VenueName = venue.Name;
                }
            }

            return metadata;
        }

        public void ToScore(SnookerMatchMetadata metadata, SnookerMatchScore score)
        {
            score.YourAthleteID = metadata.PrimaryAthleteID;
            score.YourName = metadata.PrimaryAthleteName;
            score.YourPicture = metadata.PrimaryAthletePicture;
            score.Date = metadata.Date;
            score.TableSize = metadata.TableSize;
            score.OpponentAthleteID = metadata.OpponentAthleteID;
            score.OpponentName = metadata.OpponentAthleteName;
            score.OpponentPicture = metadata.OpponentPicture;
            score.VenueID = metadata.VenueID;
            score.VenueName = metadata.VenueName;
        }
    }
}
