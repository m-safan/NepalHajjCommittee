--------------------------------------------Database---------------------------------------------------------------------------------------------------
use [master]
go

create database [NepalHajjCommittee]
go

-------------------------------------------Tables------------------------------------------------------------------------------------------------------

use [NepalHajjCommittee]
go

create table [dbo].[Room]
(
	[ID] [int] identity(1,1) not null,
	[HotelName] [nvarchar](50) not null,
	[City] [nvarchar](20) not null,
	[RoomNumber] [nvarchar](20) not null,
	[IsDirty] [bit] not null,
	[IsAvailable] [bit] not null,
	constraint [PK_Room_ID] primary key clustered ([ID] asc)
)

create table [dbo].[Bed]
(
	[ID] [int] identity(1,1) not null,
	[FK_ID_Room] [int] not null,
	[BedNumber] [nvarchar](20) not null,
	[IsAvailable] [bit] not null,
	constraint [PK_Bed_ID] primary key clustered ([ID] asc),
	constraint [FK_Bed_Room] foreign key ([FK_ID_Room]) references [dbo].[Room] ([ID])
)

create table [dbo].[HaajiGroup]
(
	[ID] [int] identity(1,1) not null,
	[Name] [nvarchar](50) not null,
	[ArrivalDateMakkah] [datetime] not null,
	[DepartureDateMakkah] [datetime] not null,
	[ArrivalDateMadinah] [datetime] not null,
	[DepartureDateMadinah] [datetime] not null,
	[IncomingFlight] [nvarchar](50) not null,
	[OutgoingFlight] [nvarchar](50) not null,
	[StateName] [nvarchar](100) not null,
	[VisitYear] [int] not null,
	[IsRoomAllotedMakkah] [bit] not null,
	[IsRoomAllotedMadinah] [bit] not null,
	constraint [PK_HaajiGroup_ID] primary key clustered ([ID] asc)
)

create table [dbo].[Batch]
(
	[ID] [int] identity(1,1) not null,
	[FK_ID_HaajiGroup] [int] not null,
	[BatchName] [nvarchar](100) not null,
	[Photo] [nvarchar](200) null,
	[ContactNo] [nvarchar](20) null,
	constraint [PK_Batch_ID] primary key clustered ([ID] asc),
	constraint [FK_Batch_HaajiGroup] foreign key ([FK_ID_HaajiGroup]) references [dbo].[HaajiGroup] ([ID])
)

create table [dbo].[Person]
(
	[ID] [int] identity(1,1) not null,
	[FK_ID_Batch] [int] not null,
	[Name] [nvarchar](100) not null,
	[Gender] [nvarchar](10) not null,
	[PassportNo] [nvarchar](50) not null,
	[FK_ID_Bed_Makkah] [int] null,
	[FK_ID_Bed_Madinah] [int] null,
	[MakkahToMadinahBusNumber] [nvarchar](500) null,
	[MadinahToMakkahBusNumber] [nvarchar](500) null,
	[MadinahToAirportBusNumber] [nvarchar](500) null,
	[MakkahToAirportBusNumber] [nvarchar](500) null,
	[Photo] [nvarchar](200) null,
	[ContactNo] [nvarchar](20) null,
	constraint [PK_Person_ID] primary key clustered ([ID] asc),
	constraint [FK_Batch_Person] foreign key ([FK_ID_Batch]) references [dbo].[Batch] ([ID]),
	constraint [FK_Batch_Bed_Makkah] foreign key ([FK_ID_Bed_Makkah]) references [dbo].[Bed] ([ID]),
	constraint [FK_Batch_Bed_Madinah] foreign key ([FK_ID_Bed_Madinah]) references [dbo].[Bed] ([ID])
)

-------------------------------------------Indexes------------------------------------------------------------------------------------------------------

create index [IX_Room_IsDirty] on [dbo].[Room] ([IsDirty]);
create index [IX_Room_IsAvailable] on [dbo].[Room] ([IsAvailable]);

create index [IX_Bed_FK_ID_Room] on [dbo].[Bed] ([FK_ID_Room]);
create index [IX_Bed_IsAvailable] on [dbo].[Bed] ([IsAvailable]);

create index [IX_Group_ArrivalDateMakkah] on [dbo].[HaajiGroup] ([ArrivalDateMakkah]);
create index [IX_Group_DepartureDateMakkah] on [dbo].[HaajiGroup] ([DepartureDateMakkah]);
create index [IX_Group_ArrivalDateMadinah] on [dbo].[HaajiGroup] ([ArrivalDateMadinah]);
create index [IX_Group_DepartureDateMadinah] on [dbo].[HaajiGroup] ([DepartureDateMadinah]);
create index [IX_Group_StateName] on [dbo].[HaajiGroup] ([StateName]);
create index [IX_Group_VisitYear] on [dbo].[HaajiGroup] ([VisitYear]);

create index [IX_Batch_FK_ID_HaajiGroup] on [dbo].[Batch] ([FK_ID_HaajiGroup]);

create index [IX_Person_FK_ID_Batch] on [dbo].[Person] ([FK_ID_Batch]);
create index [IX_Person_FK_ID_Bed_Makkah] on [dbo].[Person] ([FK_ID_Bed_Makkah]);
create index [IX_Person_FK_ID_Bed_Madinah] on [dbo].[Person] ([FK_ID_Bed_Madinah]);

-------------------------------------------Stored Procedures--------------------------------------------------------------------------------------------

use [NepalHajjCommittee]
go

-------------------------------------------Default Data-------------------------------------------------------------------------------------------------

use [NepalHajjCommittee]
go

--------------------------------------------------------------------------------------------------------------------------------------------------------