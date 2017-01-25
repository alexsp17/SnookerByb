declare @country nvarchar(50)
set @country='GBR'

-------------------------
--------- COUNTS --------

-- total
select count(*) from venues 
where country=@country

-- closed down
select count(*) from venues 
where country=@country and isinvalid=1

-- verified
select count(DISTINCT venues.name) from venues
inner join venueedits on venues.venueid=venueedits.venueid
where country=@country

--unverified
select * from venues
where (select count(*) from venueedits where venueedits.venueid=venues.venueid)=0
and country=@country
order by metroid

--unverified with metro names
select metroes.name,venues.* from venues
inner join metroes on metroes.metroid=venues.metroid
where (select count(*) from venueedits where venueedits.venueid=venues.venueid)=0
and venues.country=@country
order by metroes.name