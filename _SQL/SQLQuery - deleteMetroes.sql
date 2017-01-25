delete from metroes where 
(name like 'wolver%' or name like 'dudley' or name like 'watford' or name like 'slough' or name like 'wastborne' 
or name like 'exe%' or name like 'newport' or name like 'chelt%' or name like 'glouce%' or name like 'east%')
and metroes.country='GBR'


update venues set metroid=null 
where venues.metroid in 
(
select metroid from metroes
where 
(name like 'wolver%' or name like 'dudley' or name like 'watford' or name like 'slough' or name like 'wastborne' 
or name like 'exe%' or name like 'newport' or name like 'chelt%' or name like 'glouce%' or name like 'east%')
and metroes.country='GBR'
)

select * from venues 
where venues.metroid in 
(
select metroid from metroes
where 
(name like 'wolver%' or name like 'dudley' or name like 'watford' or name like 'slough' or name like 'wastborne' 
or name like 'exe%' or name like 'newport' or name like 'chelt%' or name like 'glouce%' or name like 'east%')
and metroes.country='GBR'
)


select * from metroes
where 
(name like 'wolver%' or name like 'dudley' or name like 'watford' or name like 'slough' or name like 'wastborne' 
or name like 'exe%' or name like 'newport' or name like 'chelt%' or name like 'glouce%' or name like 'east%')
and metroes.country='GBR'
