USE [Web]
GO

/****** Object:  Table [dbo].[Menu]    Script Date: 07/18/2019 21:15:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Menu](
	[Id] [nvarchar](50) NOT NULL,
	[UserId] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
	[GroupName] [nvarchar](20) NULL,
	[HttpMethod] [nvarchar](20) NOT NULL,
	[RelativePath] [nvarchar](200) NOT NULL,
	[Enable] [bit] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Menu] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


