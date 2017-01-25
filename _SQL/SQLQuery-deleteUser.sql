select * from athletes order by timemodified desc
select * from aspnetusers

declare @userToDelete as nvarchar(50)
set @userToDelete='test101@snookerbyb.com'

declare @athleteID as int
set @athleteID = (select athleteID from athletes where username=@usertodelete)

--- SELECT
---
select * from results where athleteid=@athleteid
select * from scores where athleteaid=@athleteid OR athletebid=@athleteid
select * from friendships where athlete1id=@athleteid OR athlete2id=@athleteid
select * from aspnetusers where email=@usertodelete
select * from athletes where username=@usertodelete

--- DELETE
---
--delete from results where athleteid=@athleteid
--delete from scores where athleteaid=@athleteid OR athletebid=@athleteid
--delete from friendships where athlete1id=@athleteid OR athlete2id=@athleteid
--delete from aspnetusers where email=@usertodelete
--delete from athletes where username=@usertodelete
