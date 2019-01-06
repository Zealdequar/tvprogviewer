use [TVProgBase]
go

if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'spDelTwoWeekAgo'))
drop procedure spDelTwoWeekAgo;
go

create procedure spDelTwoWeekAgo
as 
begin
   delete from dbo.Programmes
   where TsStartMO >= dateadd(week, 2, getdate());
   
   delete from dboProgrammes
   where TsStartMO < 
   (select top 1 dateMin4weeks from
   (select dateadd(day, N.num, dateadd(week, -4, MaxTsStartMO))  dateMin4weeks
   from (
         select max(TsStartMO) MaxTsStartMO 
         from dbo.Programmes
		   ) pr,
   (select num from (values (0), (1), (2), (3),(4), (5),(6)) as N(num)) N)A
   where datepart(dw, dateMin4weeks) = datepart(dw, '1900-01-01'))
end
go
