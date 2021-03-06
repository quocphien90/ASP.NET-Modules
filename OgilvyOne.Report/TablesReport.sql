/****** Object:  Table [dbo].[rpQueries]    Script Date: 08/24/2015 14:09:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rpQueries]') AND type in (N'U'))
DROP TABLE [dbo].[rpQueries]
GO
/****** Object:  Table [dbo].[rpQueryProperties]    Script Date: 08/24/2015 14:09:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rpQueryProperties]') AND type in (N'U'))
DROP TABLE [dbo].[rpQueryProperties]
GO
/****** Object:  Table [dbo].[rpQueryProperties]    Script Date: 08/24/2015 14:09:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rpQueryProperties]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[rpQueryProperties](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[QueryID] [bigint] NOT NULL,
	[Alias] [nvarchar](255) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](1000) NULL,
	[DataType] [int] NOT NULL,
	[DataPreValue] [nvarchar](500) NULL,
	[SortOrder] [int] NOT NULL,
	[Mandatory] [bit] NOT NULL,
 CONSTRAINT [PK_rpQueryProperties] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[rpQueries]    Script Date: 08/24/2015 14:09:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rpQueries]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[rpQueries](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[QueryCount] [nvarchar](max) NULL,
	[QueryData] [nvarchar](max) NOT NULL,
	[Status] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](200) NOT NULL,
	[ModifiedBy] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_rpQueries] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
