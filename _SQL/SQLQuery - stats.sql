-- stats

select count(*) as TotalUsers, count(FacebookId) as FacebookUsers from athletes

select count(DISTINCT AthleteID) as 'iOS users' from DeviceInfoes
where Platform=1

select count(DISTINCT AthleteID) as 'Android users' from DeviceInfoes
where Platform=2

select Country, count(*) as TotalUsers, count(FacebookId) as FacebookUsers from athletes
group by Country

-- users with good emails
select TimeCreated,AthleteID,UserName,CASE WHEN RealEmail IS NULL THEN UserName ELSE RealEmail END as Email,Name--,CASE WHEN Email IS NULL Name
from athletes
where 
	(CASE WHEN RealEmail IS NULL THEN UserName ELSE RealEmail END) not like 'fb%'
	and
	(CASE WHEN RealEmail IS NULL THEN UserName ELSE RealEmail END) not like '%@snookerbyb.com'
order by timecreated desc

--select * from athletes
