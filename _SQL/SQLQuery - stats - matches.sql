select * from scores
select distinct athleteaid,athletes.name from scores
inner join athletes on scores.athleteaid=athletes.athleteid