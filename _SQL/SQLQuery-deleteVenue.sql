select * from venues where name='Office'
select * from venues where isinvalid=1

declare @venueID as int
set @venueID = 2476


----------- SELECT
-----------
select * from venues where venueid=@venueid
select * from results where venueid=@venueid
select * from scores where venueid=@venueid

----------- DELETE
-----------
--update results set venueid=NULL where venueid=@venueid
--update scores set venueid=NULL where venueid=@venueid
--delete from gamehosts where venueid=@venueid
--delete from venues where venueid=@venueid