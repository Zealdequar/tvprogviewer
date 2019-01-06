use TVProgBase
go
select ch.TitleChannel, pr.TsStartMO, pr.TsStopMO, pr.Title
from dbo.Programmes pr
join dbo.Channels ch
on pr.CID = ch.ChannelID
where pr.Title like '%Футбол%' and pr.TsStartMO > getdate()
and pr.TID = 1
order by pr.TsStartMO;