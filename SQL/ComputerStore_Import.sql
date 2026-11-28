USE [master]
GO
/****** Объект:  Database [ComputerStore_Imported]    Дата создания скрипта: 28.11.2026 20:41:20 ******/ 
CREATE DATABASE [ComputerStore_Imported]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ComputerStore_Imported', FILENAME = N'C:\Users\ZZOOL\ComputerStore_Imported.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'ComputerStore_Imported_log', FILENAME = N'C:\Users\ZZOOL\ComputerStore_Imported_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [ComputerStore_Imported] SET COMPATIBILITY_LEVEL = 170
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ComputerStore_Imported].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ComputerStore_Imported] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ComputerStore_Imported] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ComputerStore_Imported] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ComputerStore_Imported] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ComputerStore_Imported] SET ARITHABORT OFF 
GO
ALTER DATABASE [ComputerStore_Imported] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [ComputerStore_Imported] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ComputerStore_Imported] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ComputerStore_Imported] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ComputerStore_Imported] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ComputerStore_Imported] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ComputerStore_Imported] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ComputerStore_Imported] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ComputerStore_Imported] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ComputerStore_Imported] SET  ENABLE_BROKER 
GO
ALTER DATABASE [ComputerStore_Imported] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ComputerStore_Imported] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ComputerStore_Imported] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ComputerStore_Imported] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ComputerStore_Imported] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ComputerStore_Imported] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ComputerStore_Imported] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ComputerStore_Imported] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [ComputerStore_Imported] SET  MULTI_USER 
GO
ALTER DATABASE [ComputerStore_Imported] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ComputerStore_Imported] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ComputerStore_Imported] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ComputerStore_Imported] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [ComputerStore_Imported] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [ComputerStore_Imported] SET OPTIMIZED_LOCKING = OFF 
GO
ALTER DATABASE [ComputerStore_Imported] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [ComputerStore_Imported] SET QUERY_STORE = ON
GO
ALTER DATABASE [ComputerStore_Imported] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [ComputerStore_Imported]
GO
/****** Объект:  Table [dbo].[Categories]    Дата создания скрипта: 28.11.2026 20:41:21 ******/ 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[CategoryId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Объект:  Table [dbo].[Customers]    Дата создания скрипта: 28.11.2026 20:41:21 ******/ 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customers](
	[CustomerId] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](150) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[RegisteredAt] [datetime2](7) NOT NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Объект:  Table [dbo].[OrderItems]    Дата создания скрипта: 28.11.2026 20:41:21 ******/ 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItems](
	[OrderId] [int] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[Quantity] [int] NOT NULL,
	[UnitPrice] [decimal](12, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC,
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Объект:  Table [dbo].[Orders]    Дата создания скрипта: 28.11.2026 20:41:21 ******/ 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[IsPaid] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Объект:  Table [dbo].[Products]    Дата создания скрипта: 28.11.2026 20:41:21 ******/ 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ProductId] [uniqueidentifier] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Price] [decimal](12, 2) NOT NULL,
	[StockQuantity] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Categories] ON 

INSERT [dbo].[Categories] ([CategoryId], [Name]) VALUES (1, N'Компьютеры')
INSERT [dbo].[Categories] ([CategoryId], [Name]) VALUES (2, N'Периферия')
INSERT [dbo].[Categories] ([CategoryId], [Name]) VALUES (3, N'Мониторы')
INSERT [dbo].[Categories] ([CategoryId], [Name]) VALUES (4, N'Принтеры')
SET IDENTITY_INSERT [dbo].[Categories] OFF
GO
SET IDENTITY_INSERT [dbo].[Customers] ON 

INSERT [dbo].[Customers] ([CustomerId], [FullName], [Email], [RegisteredAt], [IsActive]) VALUES (1, N'Иван Иванов', N'ivan@mail.ru', CAST(N'2026-11-28T20:28:22.5662965' AS DateTime2), 1)
INSERT [dbo].[Customers] ([CustomerId], [FullName], [Email], [RegisteredAt], [IsActive]) VALUES (2, N'Анна Петрова', N'anna@mail.ru', CAST(N'2026-11-28T20:28:22.5662965' AS DateTime2), 1)
INSERT [dbo].[Customers] ([CustomerId], [FullName], [Email], [RegisteredAt], [IsActive]) VALUES (3, N'Мария Смирнова', N'maria@mail.ru', CAST(N'2026-11-28T20:28:22.5662965' AS DateTime2), 1)
SET IDENTITY_INSERT [dbo].[Customers] OFF
GO
INSERT [dbo].[OrderItems] ([OrderId], [ProductId], [Quantity], [UnitPrice]) VALUES (1, N'0aca7979-d45d-4142-8180-3b5ac75f7340', 2, CAST(3500.00 AS Decimal(12, 2)))
INSERT [dbo].[OrderItems] ([OrderId], [ProductId], [Quantity], [UnitPrice]) VALUES (1, N'e550b4db-2454-415b-a6b5-5bab37eaafee', 1, CAST(85000.00 AS Decimal(12, 2)))
INSERT [dbo].[OrderItems] ([OrderId], [ProductId], [Quantity], [UnitPrice]) VALUES (2, N'46db08cd-c0ac-4738-9c98-590e9e84cd8b', 1, CAST(25000.00 AS Decimal(12, 2)))
INSERT [dbo].[OrderItems] ([OrderId], [ProductId], [Quantity], [UnitPrice]) VALUES (3, N'65d891cf-50ca-4c70-9e43-516d348bdd51', 1, CAST(7000.00 AS Decimal(12, 2)))
INSERT [dbo].[OrderItems] ([OrderId], [ProductId], [Quantity], [UnitPrice]) VALUES (4, N'0aca7979-d45d-4142-8180-3b5ac75f7340', 3, CAST(3500.00 AS Decimal(12, 2)))
GO
SET IDENTITY_INSERT [dbo].[Orders] ON 

INSERT [dbo].[Orders] ([OrderId], [CustomerId], [CreatedAt], [IsPaid]) VALUES (1, 1, CAST(N'2026-11-28T20:28:22.5662965' AS DateTime2), 1)
INSERT [dbo].[Orders] ([OrderId], [CustomerId], [CreatedAt], [IsPaid]) VALUES (2, 2, CAST(N'2026-11-28T20:28:22.5662965' AS DateTime2), 1)
INSERT [dbo].[Orders] ([OrderId], [CustomerId], [CreatedAt], [IsPaid]) VALUES (3, 1, CAST(N'2026-11-28T20:28:22.5662965' AS DateTime2), 0)
INSERT [dbo].[Orders] ([OrderId], [CustomerId], [CreatedAt], [IsPaid]) VALUES (4, 3, CAST(N'2026-11-28T20:28:22.5662965' AS DateTime2), 1)
SET IDENTITY_INSERT [dbo].[Orders] OFF
GO
INSERT [dbo].[Products] ([ProductId], [CategoryId], [Name], [Price], [StockQuantity], [CreatedAt], [IsActive]) VALUES (N'0aca7979-d45d-4142-8180-3b5ac75f7340', 2, N'Мышь Logitech', CAST(3500.00 AS Decimal(12, 2)), 25, CAST(N'2026-11-28T20:28:22.5662965' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [Name], [Price], [StockQuantity], [CreatedAt], [IsActive]) VALUES (N'65d891cf-50ca-4c70-9e43-516d348bdd51', 2, N'Клавиатура HyperX', CAST(7000.00 AS Decimal(12, 2)), 15, CAST(N'2026-11-28T20:28:22.5662965' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [Name], [Price], [StockQuantity], [CreatedAt], [IsActive]) VALUES (N'46db08cd-c0ac-4738-9c98-590e9e84cd8b', 3, N'Монитор Samsung', CAST(25000.00 AS Decimal(12, 2)), 8, CAST(N'2026-11-28T20:28:22.5662965' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [Name], [Price], [StockQuantity], [CreatedAt], [IsActive]) VALUES (N'e550b4db-2454-415b-a6b5-5bab37eaafee', 1, N'Ноутбук ASUS', CAST(93500.00 AS Decimal(12, 2)), 10, CAST(N'2026-11-28T20:28:22.5662965' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [Name], [Price], [StockQuantity], [CreatedAt], [IsActive]) VALUES (N'e04e14cc-f4ea-4b76-b222-7ba399728814', 3, N'Монитор LG', CAST(22000.00 AS Decimal(12, 2)), 0, CAST(N'2026-11-28T20:28:22.5662965' AS DateTime2), 0)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [Name], [Price], [StockQuantity], [CreatedAt], [IsActive]) VALUES (N'25f9c8b8-9df6-4de2-8246-ab1eeb8781c2', 1, N'Ноутбук Lenovo', CAST(65000.00 AS Decimal(12, 2)), 5, CAST(N'2026-11-28T20:28:22.5662965' AS DateTime2), 1)
GO
SET ANSI_PADDING ON
GO
/****** Объект:  Index [UX_Customers_Email]    Дата создания скрипта: 28.11.2026 20:41:21 ******/ 
CREATE UNIQUE NONCLUSTERED INDEX [UX_Customers_Email] ON [dbo].[Customers]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Customers] ADD  DEFAULT (sysdatetime()) FOR [RegisteredAt]
GO
ALTER TABLE [dbo].[Customers] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (sysdatetime()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT ((0)) FOR [IsPaid]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT (newid()) FOR [ProductId]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT (sysdatetime()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([OrderId])
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([ProductId])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerId])
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([CategoryId])
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD CHECK  (([Quantity]>(0)))
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD CHECK  (([UnitPrice]>=(0)))
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD CHECK  (([Price]>=(0)))
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD CHECK  (([StockQuantity]>=(0)))
GO
USE [master]
GO
ALTER DATABASE [ComputerStore_Imported] SET  READ_WRITE 
GO
