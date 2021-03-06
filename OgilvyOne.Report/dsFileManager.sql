/****** Object:  Table [dbo].[dsFileManagerConfigs]    Script Date: 02/24/2016 13:59:53 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dsFileManagerConfigs_Status]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[dsFileManagerConfigs] DROP CONSTRAINT [DF_dsFileManagerConfigs_Status]
END
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dsFileManagerConfigs]') AND type in (N'U'))
DROP TABLE [dbo].[dsFileManagerConfigs]
GO
/****** Object:  Table [dbo].[dsFileManagerConfigs]    Script Date: 02/24/2016 13:59:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dsFileManagerConfigs]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[dsFileManagerConfigs](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Path] [nvarchar](4000) NOT NULL,
	[Maxrecord] [int] NOT NULL,
	[Status] [int] NOT NULL CONSTRAINT [DF_dsFileManagerConfigs_Status]  DEFAULT ((1)),
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_dsFileManagerConfigs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
