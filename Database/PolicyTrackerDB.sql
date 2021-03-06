USE [PolicyTracker_D]
GO
/****** Object:  Table [dbo].[AppUsers]    Script Date: 1/14/2020 1:27:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppUsers](
	[UserId] [varchar](50) NOT NULL,
	[UserName] [varchar](50) NOT NULL,
	[UserRole] [varchar](100) NOT NULL,
	[CreateDT] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastUpdateDT] [datetime] NOT NULL,
	[LastUpdateUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_AppUsers] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Bureaus]    Script Date: 1/14/2020 1:27:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bureaus](
	[BurCode] [varchar](50) NOT NULL,
	[BurDesc] [varchar](100) NOT NULL,
	[DeptCode] [varchar](50) NOT NULL,
	[Disabled] [bit] NOT NULL,
	[CreateDT] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastUpdateDT] [datetime] NOT NULL,
	[LastUpdateUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Bureaus] PRIMARY KEY CLUSTERED 
(
	[BurCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Classifications]    Script Date: 1/14/2020 1:27:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Classifications](
	[ClassCode] [varchar](50) NOT NULL,
	[ClassDesc] [varchar](100) NOT NULL,
	[ClassType] [varchar](50) NOT NULL,
	[Disabled] [bit] NOT NULL,
	[CreateDT] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastUpdateDT] [datetime] NOT NULL,
	[LastUpdateUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Classifications] PRIMARY KEY CLUSTERED 
(
	[ClassCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CommonCodes]    Script Date: 1/14/2020 1:27:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CommonCodes](
	[CmId] [int] IDENTITY(1,1) NOT NULL,
	[CmCatg] [varchar](100) NOT NULL,
	[CmCode] [varchar](100) NOT NULL,
	[CmCodeDesc] [nvarchar](255) NOT NULL,
	[DispOrder] [int] NOT NULL,
	[Disabled] [bit] NOT NULL,
	[CreateDT] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastUpdateDT] [datetime] NOT NULL,
	[LastUpdateUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CommonCodes] PRIMARY KEY CLUSTERED 
(
	[CmId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Configurations]    Script Date: 1/14/2020 1:27:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Configurations](
	[ConfigKey] [varchar](50) NOT NULL,
	[ConfigDesc] [nvarchar](100) NOT NULL,
	[ConfigValue] [nvarchar](50) NOT NULL,
	[CreateDT] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastUpdateDT] [datetime] NOT NULL,
	[LastUpdateUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Configuration] PRIMARY KEY CLUSTERED 
(
	[ConfigKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Departments]    Script Date: 1/14/2020 1:27:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Departments](
	[DeptCode] [varchar](50) NOT NULL,
	[DeptDesc] [varchar](100) NOT NULL,
	[FamisOrgCode] [varchar](50) NULL,
	[ADOU] [varchar](50) NULL,
	[DefaultUserGroup] [varchar](50) NULL,
	[Disabled] [bit] NOT NULL,
	[CreateDT] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastUpdateDT] [datetime] NOT NULL,
	[LastUpdateUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Departments] PRIMARY KEY CLUSTERED 
(
	[DeptCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Divisions]    Script Date: 1/14/2020 1:27:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Divisions](
	[DivCode] [varchar](50) NOT NULL,
	[DivDesc] [varchar](100) NOT NULL,
	[BurCode] [varchar](50) NOT NULL,
	[CMDept] [varchar](50) NULL,
	[PersUnitCode] [varchar](50) NULL,
	[FamisOrg] [varchar](50) NULL,
	[FamisDefaultIndex] [varchar](50) NULL,
	[Disabled] [bit] NOT NULL,
	[CreateDT] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastUpdateDT] [datetime] NOT NULL,
	[LastUpdateUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Divisions] PRIMARY KEY CLUSTERED 
(
	[DivCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employees]    Script Date: 1/14/2020 1:27:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employees](
	[EmpId] [varchar](50) NOT NULL,
	[PIN] [varchar](50) NULL,
	[Status] [varchar](50) NOT NULL,
	[FirstName] [varchar](50) NOT NULL,
	[MiddleName] [varchar](50) NULL,
	[LastName] [varchar](50) NOT NULL,
	[Suffix] [varchar](50) NULL,
	[OrgCode] [varchar](50) NOT NULL,
	[ClassCode] [varchar](50) NOT NULL,
	[HireDate] [date] NULL,
	[Disabled] [bit] NOT NULL,
	[CreateDT] [datetime] NOT NULL,
	[LastUpdateDT] [datetime] NOT NULL,
 CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
(
	[EmpId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Messages]    Script Date: 1/14/2020 1:27:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Messages](
	[MsgNo] [int] NOT NULL,
	[MsgText] [nvarchar](max) NOT NULL,
	[MsgTitle] [nvarchar](50) NOT NULL,
	[CreateDT] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastUpdateDT] [datetime] NOT NULL,
	[LastUpdateUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED 
(
	[MsgNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notices]    Script Date: 1/14/2020 1:27:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notices](
	[NoticeId] [int] IDENTITY(1,1) NOT NULL,
	[NoticeTitle] [nvarchar](50) NOT NULL,
	[NoticeText] [nvarchar](max) NOT NULL,
	[StartDate] [smalldatetime] NOT NULL,
	[EndDate] [smalldatetime] NOT NULL,
	[DispOrder] [int] NULL,
	[Disabled] [bit] NOT NULL,
	[CreateDT] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastUpdateDT] [datetime] NOT NULL,
	[LastUpdateUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Notices] PRIMARY KEY CLUSTERED 
(
	[NoticeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrgAdminNotices]    Script Date: 1/14/2020 1:27:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrgAdminNotices](
	[ReleaseNoticeId] [int] NOT NULL,
	[From] [varchar](500) NOT NULL,
	[To] [varchar](max) NOT NULL,
	[Subject] [nvarchar](1000) NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[CreateDT] [datetime] NOT NULL,
	[SentDT] [datetime] NULL,
 CONSTRAINT [PK_OrgAdminNotices] PRIMARY KEY CLUSTERED 
(
	[ReleaseNoticeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PacketAckFiles]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PacketAckFiles](
	[ReleaseId] [int] NOT NULL,
	[RecipientId] [varchar](50) NOT NULL,
	[FileId] [varchar](50) NOT NULL,
	[Disabled] [bit] NOT NULL,
 CONSTRAINT [PK_PacketAckFiles] PRIMARY KEY CLUSTERED 
(
	[ReleaseId] ASC,
	[RecipientId] ASC,
	[FileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Policies]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Policies](
	[PolicyId] [int] IDENTITY(1,1) NOT NULL,
	[PolicyName] [nvarchar](100) NOT NULL,
	[PolicyDesc] [nvarchar](max) NULL,
	[ShowDisclaimer] [bit] NOT NULL,
	[Disclaimer] [nvarchar](max) NULL,
	[ShareType] [varchar](100) NOT NULL,
	[DeptCode] [varchar](50) NULL,
	[Disabled] [bit] NOT NULL,
	[CreateDT] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastUpdateDT] [datetime] NOT NULL,
	[LastUpdateUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Policies] PRIMARY KEY CLUSTERED 
(
	[PolicyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PolicyFiles]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PolicyFiles](
	[PolicyId] [int] NOT NULL,
	[FileId] [varchar](50) NOT NULL,
 CONSTRAINT [PK_PolicyFiles] PRIMARY KEY CLUSTERED 
(
	[PolicyId] ASC,
	[FileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PolicyOwners]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PolicyOwners](
	[PolicyId] [int] NOT NULL,
	[UserId] [varchar](50) NOT NULL,
 CONSTRAINT [PK_PolicyOwners] PRIMARY KEY CLUSTERED 
(
	[PolicyId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Preferences]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Preferences](
	[Catg] [varchar](255) NOT NULL,
	[UserId] [varchar](50) NOT NULL,
	[Val] [varchar](50) NULL,
	[LastUpdateDT] [datetime] NOT NULL,
	[LastUpdateUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Preferences] PRIMARY KEY CLUSTERED 
(
	[Catg] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RecipGroupClasses]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecipGroupClasses](
	[RecipGroupId] [int] NOT NULL,
	[ClassCode] [varchar](50) NOT NULL,
 CONSTRAINT [PK_RecipGroupClasses] PRIMARY KEY CLUSTERED 
(
	[RecipGroupId] ASC,
	[ClassCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RecipGroupMembers]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecipGroupMembers](
	[RecipGroupId] [int] NOT NULL,
	[EmpId] [varchar](50) NOT NULL,
 CONSTRAINT [PK_RecipGroupMembers] PRIMARY KEY CLUSTERED 
(
	[RecipGroupId] ASC,
	[EmpId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RecipGroupOrgs]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecipGroupOrgs](
	[RecipGroupId] [int] NOT NULL,
	[OrgCode] [varchar](50) NOT NULL,
 CONSTRAINT [PK_RecipGroupOrgs] PRIMARY KEY CLUSTERED 
(
	[RecipGroupId] ASC,
	[OrgCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RecipGroupOwners]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecipGroupOwners](
	[RecipGroupId] [int] NOT NULL,
	[UserId] [varchar](50) NOT NULL,
 CONSTRAINT [PK_RecipGroupOwners] PRIMARY KEY CLUSTERED 
(
	[RecipGroupId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RecipGroups]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecipGroups](
	[RecipGroupId] [int] IDENTITY(1,1) NOT NULL,
	[GroupName] [varchar](100) NOT NULL,
	[RecipGroupType] [varchar](100) NOT NULL,
	[ShareType] [varchar](100) NOT NULL,
	[DeptCode] [varchar](50) NULL,
	[Disabled] [bit] NOT NULL,
	[CreateDT] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastUpdateDT] [datetime] NOT NULL,
	[LastUpdateUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_RecipGroups] PRIMARY KEY CLUSTERED 
(
	[RecipGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RecipientNotices]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecipientNotices](
	[ReleaseNoticeId] [int] NOT NULL,
	[RecipientId] [varchar](50) NOT NULL,
	[From] [varchar](500) NOT NULL,
	[To] [varchar](500) NOT NULL,
	[Subject] [nvarchar](1000) NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[CreateDT] [datetime] NOT NULL,
	[SentDT] [datetime] NULL,
 CONSTRAINT [PK_RecipNotices] PRIMARY KEY CLUSTERED 
(
	[ReleaseNoticeId] ASC,
	[RecipientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReleaseNotices]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReleaseNotices](
	[ReleaseNoticeId] [int] IDENTITY(1,1) NOT NULL,
	[ReleaseId] [int] NOT NULL,
	[NoticeType] [varchar](100) NOT NULL,
	[NoticeDate] [smalldatetime] NOT NULL,
	[StartDT] [datetime] NULL,
	[CompleteDT] [datetime] NULL,
	[CreateDT] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastUpdateDT] [datetime] NOT NULL,
	[LastUpdateUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_ReleaseNotices] PRIMARY KEY CLUSTERED 
(
	[ReleaseNoticeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReleasePolicy]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReleasePolicy](
	[ReleaseId] [int] NOT NULL,
	[PolicyId] [int] NOT NULL,
	[PolicyName] [nvarchar](100) NOT NULL,
	[PolicyDesc] [nvarchar](max) NULL,
	[ShowDisclaimer] [bit] NOT NULL,
	[Disclaimer] [nvarchar](max) NULL,
 CONSTRAINT [PK_ReleasePolicy] PRIMARY KEY CLUSTERED 
(
	[ReleaseId] ASC,
	[PolicyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReleasePolicyFiles]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReleasePolicyFiles](
	[ReleaseId] [int] NOT NULL,
	[PolicyId] [int] NOT NULL,
	[FileId] [varchar](50) NOT NULL,
 CONSTRAINT [PK_ReleasePolicyFile] PRIMARY KEY CLUSTERED 
(
	[ReleaseId] ASC,
	[PolicyId] ASC,
	[FileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReleaseRecipients]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReleaseRecipients](
	[ReleaseId] [int] NOT NULL,
	[RecipientId] [varchar](50) NOT NULL,
	[RecipientName] [varchar](500) NOT NULL,
	[RecipientEmail] [varchar](500) NULL,
	[OrgCode] [varchar](50) NULL,
	[AckDT] [datetime] NULL,
	[AckUserId] [varchar](50) NULL,
	[AckClientIP] [varchar](50) NULL,
	[RecipientViewDT] [datetime] NULL,
	[RecipientViewClientIP] [varchar](50) NULL,
	[AdminMemo] [nvarchar](max) NULL,
	[Exception] [varchar](100) NULL,
	[CreateDT] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastUpdateDT] [datetime] NOT NULL,
	[LastUpdateUser] [varchar](50) NOT NULL,
	[AckAuthType] [varchar](50) NULL,
 CONSTRAINT [PK_RelelaseRecip] PRIMARY KEY CLUSTERED 
(
	[ReleaseId] ASC,
	[RecipientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Releases]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Releases](
	[ReleaseId] [int] IDENTITY(1,1) NOT NULL,
	[ScheduleId] [int] NOT NULL,
	[ReleaseDate] [smalldatetime] NOT NULL,
	[DeadlineDate] [smalldatetime] NOT NULL,
	[To] [nvarchar](500) NULL,
	[From] [nvarchar](500) NULL,
	[Subject] [nvarchar](500) NULL,
	[Disclaimer] [nvarchar](max) NULL,
	[CreateDT] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastUpdateDT] [datetime] NOT NULL,
	[LastUpdateUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Releases] PRIMARY KEY CLUSTERED 
(
	[ReleaseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScheduleOwners]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScheduleOwners](
	[ScheduleId] [int] NOT NULL,
	[UserId] [varchar](50) NOT NULL,
 CONSTRAINT [PK_ScheduleOwners] PRIMARY KEY CLUSTERED 
(
	[ScheduleId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SchedulePolicy]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SchedulePolicy](
	[ScheduleId] [int] NOT NULL,
	[PolicyId] [int] NOT NULL,
 CONSTRAINT [PK_SchedulePolicy] PRIMARY KEY CLUSTERED 
(
	[PolicyId] ASC,
	[ScheduleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScheduleRecipGroup]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScheduleRecipGroup](
	[ScheduleId] [int] NOT NULL,
	[RecipGroupId] [int] NOT NULL,
 CONSTRAINT [PK_ScheduleRecipGroup] PRIMARY KEY CLUSTERED 
(
	[ScheduleId] ASC,
	[RecipGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Schedules]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Schedules](
	[ScheduleId] [int] IDENTITY(1,1) NOT NULL,
	[ScheduleName] [nvarchar](100) NOT NULL,
	[ScheduleDesc] [nvarchar](max) NULL,
	[Frequency] [varchar](100) NOT NULL,
	[RepeatMonth] [smallint] NULL,
	[RepeatDay] [smallint] NULL,
	[FixedReleaseDate] [smalldatetime] NULL,
	[DaysToDeadline] [smallint] NOT NULL,
	[DaysToReminder] [varchar](50) NULL,
	[To] [nvarchar](500) NULL,
	[From] [nvarchar](500) NULL,
	[Subject] [nvarchar](500) NULL,
	[Disclaimer] [nvarchar](max) NULL,
	[Disabled] [bit] NOT NULL,
	[CreateDT] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastUpdateDT] [datetime] NOT NULL,
	[LastUpdateUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Schedules] PRIMARY KEY CLUSTERED 
(
	[ScheduleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UploadFiles]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UploadFiles](
	[FileId] [varchar](50) NOT NULL,
	[FileUrl] [nvarchar](1000) NULL,
	[FileData] [varbinary](max) NULL,
	[OriginalName] [nvarchar](500) NULL,
	[ContentType] [varchar](500) NULL,
	[Length] [decimal](18, 0) NOT NULL,
	[CreateDT] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_UploadFiles] PRIMARY KEY CLUSTERED 
(
	[FileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserOrgs]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserOrgs](
	[UserId] [varchar](50) NOT NULL,
	[OrgCode] [varchar](50) NOT NULL,
	[AccessLevel] [varchar](100) NOT NULL,
	[CreateDT] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastUpdateDT] [datetime] NOT NULL,
	[LastUpdateUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_UserOrgs] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[OrgCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vOrganizations]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vOrganizations]
AS
select DeptCode OrgCode, DeptCode + ' - ' + DeptDesc OrgDesc, Disabled
from Departments 
union
select BurCode OrgCode, BurCode + ' - ' + BurDesc OrgDesc, Disabled
from Bureaus 
union
select DivCode OrgCode, DivCode + ' - ' + DivDesc OrgDesc, Disabled
from Divisions 
GO
/****** Object:  View [dbo].[vOrgAdmins]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vOrgAdmins]
AS
select uo.UserId, u.UserName, uo.AccessLevel, uo.OrgCode, o.OrgDesc 
, uo.CreateDT, uo.CreateUser, uo.LastUpdateDT, uo.LastUpdateUser
from UserOrgs uo
left outer join AppUsers u on u.UserId = uo.UserId
left outer join vOrganizations o on o.OrgCode = uo.OrgCode
--where uo.AccessLevel = 'PA'
GO
/****** Object:  View [dbo].[vEmployees]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vEmployees]
AS
select e.*, dep.DeptDesc, bur.BurDesc, div.DivDesc OrgDesc, c.ClassDesc 
from Employees e
left outer join Departments dep on dep.DeptCode = SUBSTRING(e.OrgCode, 1, 2)
left outer join Bureaus bur on bur.BurCode = SUBSTRING(e.OrgCode, 1, 3)
left outer join Divisions div on div.DivCode = e.OrgCode 
left outer join Classifications c on c.ClassCode = e.ClassCode 
GO
/****** Object:  View [dbo].[vPackets]    Script Date: 1/14/2020 1:27:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE view [dbo].[vPackets]
as
SELECT p.[ReleaseId],p.[RecipientId],p.[RecipientName],[RecipientEmail],p.[OrgCode], d.DivDesc as OrgDesc, e.ClassCode, e.ClassDesc 
,[AckDT],[AckUserId],[AckClientIP],[AckAuthType],[RecipientViewDT],[RecipientViewClientIP],[AdminMemo],[Exception]
,r.ReleaseDate, r.DeadlineDate,r.[To],r.[From],r.[Subject]
,p.[CreateDT],p.[CreateUser],p.[LastUpdateDT],p.[LastUpdateUser]
, (select count(0) from PacketAckFiles af where af.ReleaseId = p.ReleaseId and af.RecipientId = p.RecipientId and af.Disabled = 0) as AckFilesCount
FROM [dbo].[ReleaseRecipients] p
join Releases r on r.ReleaseId = p.ReleaseId
left outer join Divisions d on d.DivCode = p.OrgCode 
left outer join vEmployees e on e.EmpId = p.RecipientId

GO
ALTER TABLE [dbo].[Bureaus] ADD  CONSTRAINT [DF_Bureaus_Disabled]  DEFAULT ((0)) FOR [Disabled]
GO
ALTER TABLE [dbo].[Classifications] ADD  CONSTRAINT [DF_Classifications_Disabled]  DEFAULT ((0)) FOR [Disabled]
GO
ALTER TABLE [dbo].[CommonCodes] ADD  CONSTRAINT [DF_CommonCodes_Disabled]  DEFAULT ((0)) FOR [Disabled]
GO
ALTER TABLE [dbo].[Departments] ADD  CONSTRAINT [DF_Departments_Disabled]  DEFAULT ((0)) FOR [Disabled]
GO
ALTER TABLE [dbo].[Divisions] ADD  CONSTRAINT [DF_Divisions_Disabled]  DEFAULT ((0)) FOR [Disabled]
GO
ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [DF_Employees_Disabled]  DEFAULT ((0)) FOR [Disabled]
GO
ALTER TABLE [dbo].[Notices] ADD  CONSTRAINT [DF_Notices_Disabled]  DEFAULT ((0)) FOR [Disabled]
GO
ALTER TABLE [dbo].[PacketAckFiles] ADD  DEFAULT ((0)) FOR [Disabled]
GO
ALTER TABLE [dbo].[ReleaseNotices] ADD  CONSTRAINT [DF_ReleaseNotices_CreateDT]  DEFAULT (getdate()) FOR [CreateDT]
GO
ALTER TABLE [dbo].[ReleaseNotices] ADD  CONSTRAINT [DF_ReleaseNotices_CreateUser]  DEFAULT ('PT_Batch') FOR [CreateUser]
GO
ALTER TABLE [dbo].[ReleaseNotices] ADD  CONSTRAINT [DF_ReleaseNotices_LastUpdateDT]  DEFAULT (getdate()) FOR [LastUpdateDT]
GO
ALTER TABLE [dbo].[ReleaseNotices] ADD  CONSTRAINT [DF_ReleaseNotices_LastUpdateUser]  DEFAULT ('PT_Batch') FOR [LastUpdateUser]
GO
ALTER TABLE [dbo].[Bureaus]  WITH CHECK ADD  CONSTRAINT [FK_Bur_Dept] FOREIGN KEY([DeptCode])
REFERENCES [dbo].[Departments] ([DeptCode])
GO
ALTER TABLE [dbo].[Bureaus] CHECK CONSTRAINT [FK_Bur_Dept]
GO
ALTER TABLE [dbo].[Divisions]  WITH CHECK ADD  CONSTRAINT [FK_Div_Bur] FOREIGN KEY([BurCode])
REFERENCES [dbo].[Bureaus] ([BurCode])
GO
ALTER TABLE [dbo].[Divisions] CHECK CONSTRAINT [FK_Div_Bur]
GO
ALTER TABLE [dbo].[OrgAdminNotices]  WITH CHECK ADD  CONSTRAINT [FK_OAN_RelNotice] FOREIGN KEY([ReleaseNoticeId])
REFERENCES [dbo].[ReleaseNotices] ([ReleaseNoticeId])
GO
ALTER TABLE [dbo].[OrgAdminNotices] CHECK CONSTRAINT [FK_OAN_RelNotice]
GO
ALTER TABLE [dbo].[PacketAckFiles]  WITH CHECK ADD  CONSTRAINT [FK_PAF_File] FOREIGN KEY([FileId])
REFERENCES [dbo].[UploadFiles] ([FileId])
GO
ALTER TABLE [dbo].[PacketAckFiles] CHECK CONSTRAINT [FK_PAF_File]
GO
ALTER TABLE [dbo].[PacketAckFiles]  WITH CHECK ADD  CONSTRAINT [FK_PAF_ReleaseRecipient] FOREIGN KEY([ReleaseId], [RecipientId])
REFERENCES [dbo].[ReleaseRecipients] ([ReleaseId], [RecipientId])
GO
ALTER TABLE [dbo].[PacketAckFiles] CHECK CONSTRAINT [FK_PAF_ReleaseRecipient]
GO
ALTER TABLE [dbo].[PolicyFiles]  WITH CHECK ADD  CONSTRAINT [FK_PF_File] FOREIGN KEY([FileId])
REFERENCES [dbo].[UploadFiles] ([FileId])
GO
ALTER TABLE [dbo].[PolicyFiles] CHECK CONSTRAINT [FK_PF_File]
GO
ALTER TABLE [dbo].[PolicyFiles]  WITH CHECK ADD  CONSTRAINT [FK_PF_Policy] FOREIGN KEY([PolicyId])
REFERENCES [dbo].[Policies] ([PolicyId])
GO
ALTER TABLE [dbo].[PolicyFiles] CHECK CONSTRAINT [FK_PF_Policy]
GO
ALTER TABLE [dbo].[PolicyOwners]  WITH CHECK ADD  CONSTRAINT [FK_PO_Policy] FOREIGN KEY([PolicyId])
REFERENCES [dbo].[Policies] ([PolicyId])
GO
ALTER TABLE [dbo].[PolicyOwners] CHECK CONSTRAINT [FK_PO_Policy]
GO
ALTER TABLE [dbo].[RecipGroupClasses]  WITH CHECK ADD  CONSTRAINT [FK_RGC_Class] FOREIGN KEY([ClassCode])
REFERENCES [dbo].[Classifications] ([ClassCode])
GO
ALTER TABLE [dbo].[RecipGroupClasses] CHECK CONSTRAINT [FK_RGC_Class]
GO
ALTER TABLE [dbo].[RecipGroupClasses]  WITH CHECK ADD  CONSTRAINT [FK_RGC_RecipGroup] FOREIGN KEY([RecipGroupId])
REFERENCES [dbo].[RecipGroups] ([RecipGroupId])
GO
ALTER TABLE [dbo].[RecipGroupClasses] CHECK CONSTRAINT [FK_RGC_RecipGroup]
GO
ALTER TABLE [dbo].[RecipGroupMembers]  WITH CHECK ADD  CONSTRAINT [FK_RGM_Emp] FOREIGN KEY([EmpId])
REFERENCES [dbo].[Employees] ([EmpId])
GO
ALTER TABLE [dbo].[RecipGroupMembers] CHECK CONSTRAINT [FK_RGM_Emp]
GO
ALTER TABLE [dbo].[RecipGroupMembers]  WITH CHECK ADD  CONSTRAINT [FK_RGM_RecipGroup] FOREIGN KEY([RecipGroupId])
REFERENCES [dbo].[RecipGroups] ([RecipGroupId])
GO
ALTER TABLE [dbo].[RecipGroupMembers] CHECK CONSTRAINT [FK_RGM_RecipGroup]
GO
ALTER TABLE [dbo].[RecipGroupOrgs]  WITH CHECK ADD  CONSTRAINT [FK_RGO_RecipGroup] FOREIGN KEY([RecipGroupId])
REFERENCES [dbo].[RecipGroups] ([RecipGroupId])
GO
ALTER TABLE [dbo].[RecipGroupOrgs] CHECK CONSTRAINT [FK_RGO_RecipGroup]
GO
ALTER TABLE [dbo].[RecipGroupOwners]  WITH CHECK ADD  CONSTRAINT [FK_RO_RecipGroup] FOREIGN KEY([RecipGroupId])
REFERENCES [dbo].[RecipGroups] ([RecipGroupId])
GO
ALTER TABLE [dbo].[RecipGroupOwners] CHECK CONSTRAINT [FK_RO_RecipGroup]
GO
ALTER TABLE [dbo].[RecipientNotices]  WITH CHECK ADD  CONSTRAINT [FK_RN_RelNotice] FOREIGN KEY([ReleaseNoticeId])
REFERENCES [dbo].[ReleaseNotices] ([ReleaseNoticeId])
GO
ALTER TABLE [dbo].[RecipientNotices] CHECK CONSTRAINT [FK_RN_RelNotice]
GO
ALTER TABLE [dbo].[ReleaseNotices]  WITH CHECK ADD  CONSTRAINT [FK_RelN_Release] FOREIGN KEY([ReleaseId])
REFERENCES [dbo].[Releases] ([ReleaseId])
GO
ALTER TABLE [dbo].[ReleaseNotices] CHECK CONSTRAINT [FK_RelN_Release]
GO
ALTER TABLE [dbo].[ReleasePolicy]  WITH CHECK ADD  CONSTRAINT [FK_RP_Policy] FOREIGN KEY([PolicyId])
REFERENCES [dbo].[Policies] ([PolicyId])
GO
ALTER TABLE [dbo].[ReleasePolicy] CHECK CONSTRAINT [FK_RP_Policy]
GO
ALTER TABLE [dbo].[ReleasePolicy]  WITH CHECK ADD  CONSTRAINT [FK_RP_Release] FOREIGN KEY([ReleaseId])
REFERENCES [dbo].[Releases] ([ReleaseId])
GO
ALTER TABLE [dbo].[ReleasePolicy] CHECK CONSTRAINT [FK_RP_Release]
GO
ALTER TABLE [dbo].[ReleasePolicyFiles]  WITH CHECK ADD  CONSTRAINT [FK_RPF_File] FOREIGN KEY([FileId])
REFERENCES [dbo].[UploadFiles] ([FileId])
GO
ALTER TABLE [dbo].[ReleasePolicyFiles] CHECK CONSTRAINT [FK_RPF_File]
GO
ALTER TABLE [dbo].[ReleasePolicyFiles]  WITH CHECK ADD  CONSTRAINT [FK_RPF_ReleasePolicy] FOREIGN KEY([ReleaseId], [PolicyId])
REFERENCES [dbo].[ReleasePolicy] ([ReleaseId], [PolicyId])
GO
ALTER TABLE [dbo].[ReleasePolicyFiles] CHECK CONSTRAINT [FK_RPF_ReleasePolicy]
GO
ALTER TABLE [dbo].[ReleaseRecipients]  WITH CHECK ADD  CONSTRAINT [FK_RelRecip_Div] FOREIGN KEY([OrgCode])
REFERENCES [dbo].[Divisions] ([DivCode])
GO
ALTER TABLE [dbo].[ReleaseRecipients] CHECK CONSTRAINT [FK_RelRecip_Div]
GO
ALTER TABLE [dbo].[ReleaseRecipients]  WITH CHECK ADD  CONSTRAINT [FK_RelRecip_Release] FOREIGN KEY([ReleaseId])
REFERENCES [dbo].[Releases] ([ReleaseId])
GO
ALTER TABLE [dbo].[ReleaseRecipients] CHECK CONSTRAINT [FK_RelRecip_Release]
GO
ALTER TABLE [dbo].[Releases]  WITH CHECK ADD  CONSTRAINT [FK_Rel_Schedule] FOREIGN KEY([ScheduleId])
REFERENCES [dbo].[Schedules] ([ScheduleId])
GO
ALTER TABLE [dbo].[Releases] CHECK CONSTRAINT [FK_Rel_Schedule]
GO
ALTER TABLE [dbo].[ScheduleOwners]  WITH CHECK ADD  CONSTRAINT [FK_SO_Schedule] FOREIGN KEY([ScheduleId])
REFERENCES [dbo].[Schedules] ([ScheduleId])
GO
ALTER TABLE [dbo].[ScheduleOwners] CHECK CONSTRAINT [FK_SO_Schedule]
GO
ALTER TABLE [dbo].[SchedulePolicy]  WITH CHECK ADD  CONSTRAINT [FK_SP_Policy] FOREIGN KEY([PolicyId])
REFERENCES [dbo].[Policies] ([PolicyId])
GO
ALTER TABLE [dbo].[SchedulePolicy] CHECK CONSTRAINT [FK_SP_Policy]
GO
ALTER TABLE [dbo].[SchedulePolicy]  WITH CHECK ADD  CONSTRAINT [FK_SP_Schedule] FOREIGN KEY([ScheduleId])
REFERENCES [dbo].[Schedules] ([ScheduleId])
GO
ALTER TABLE [dbo].[SchedulePolicy] CHECK CONSTRAINT [FK_SP_Schedule]
GO
ALTER TABLE [dbo].[ScheduleRecipGroup]  WITH CHECK ADD  CONSTRAINT [FK_SRG_RecipGroup] FOREIGN KEY([RecipGroupId])
REFERENCES [dbo].[RecipGroups] ([RecipGroupId])
GO
ALTER TABLE [dbo].[ScheduleRecipGroup] CHECK CONSTRAINT [FK_SRG_RecipGroup]
GO
ALTER TABLE [dbo].[ScheduleRecipGroup]  WITH CHECK ADD  CONSTRAINT [FK_SRG_Schedule] FOREIGN KEY([ScheduleId])
REFERENCES [dbo].[Schedules] ([ScheduleId])
GO
ALTER TABLE [dbo].[ScheduleRecipGroup] CHECK CONSTRAINT [FK_SRG_Schedule]
GO
ALTER TABLE [dbo].[UserOrgs]  WITH CHECK ADD  CONSTRAINT [FK_UserOrgs_AppUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AppUsers] ([UserId])
GO
ALTER TABLE [dbo].[UserOrgs] CHECK CONSTRAINT [FK_UserOrgs_AppUsers]
GO
