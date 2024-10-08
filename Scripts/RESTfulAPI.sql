USE [RESTDatabase]
GO
/****** Object:  Table [dbo].[Member]    Script Date: 18-Mar-24 21:43:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Member](
	[ID] [nchar](10) NOT NULL,
	[Username] [nchar](15) NULL,
	[Password] [nchar](10) NULL,
 CONSTRAINT [PK_Member] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Owner]    Script Date: 18-Mar-24 21:43:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Owner](
	[ID] [nchar](10) NOT NULL,
	[Fullname] [nchar](25) NULL,
	[Age] [int] NULL,
 CONSTRAINT [PK_Owner] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Vehicle]    Script Date: 18-Mar-24 21:43:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vehicle](
	[Patent] [nchar](6) NOT NULL,
	[Brand] [nchar](15) NULL,
	[Model] [nchar](15) NULL,
	[Type] [nchar](15) NULL,
	[Year] [int] NULL,
	[Driver] [nchar](10) NOT NULL,
 CONSTRAINT [PK_Vehicle] PRIMARY KEY CLUSTERED 
(
	[Patent] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Member]  WITH CHECK ADD  CONSTRAINT [FK_Owner_Member] FOREIGN KEY([ID])
REFERENCES [dbo].[Owner] ([ID])
GO
ALTER TABLE [dbo].[Member] CHECK CONSTRAINT [FK_Owner_Member]
GO
ALTER TABLE [dbo].[Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_Owner_Vehicle] FOREIGN KEY([Driver])
REFERENCES [dbo].[Owner] ([ID])
GO
ALTER TABLE [dbo].[Vehicle] CHECK CONSTRAINT [FK_Owner_Vehicle]
GO
