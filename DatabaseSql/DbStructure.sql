-- --------------------------------------------------------
-- Värd:                         192.168.10.38
-- Serverversion:                10.5.16-MariaDB-log - MariaDB Server
-- Server-OS:                    Linux
-- HeidiSQL Version:             11.3.0.6295
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Dumpar databasstruktur för XivMarketBoard
CREATE DATABASE IF NOT EXISTS `XivMarketBoard` /*!40100 DEFAULT CHARACTER SET utf8mb4 */;
USE `XivMarketBoard`;

-- Dumpar struktur för tabell XivMarketBoard.DataCenters
CREATE TABLE IF NOT EXISTS `DataCenters` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` longtext NOT NULL,
  `Region` longtext NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4;

-- Dataexport var bortvalt.

-- Dumpar struktur för tabell XivMarketBoard.Ingredients
CREATE TABLE IF NOT EXISTS `Ingredients` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Amount` int(11) NOT NULL,
  `ItemId` int(11) NOT NULL,
  `RecipeId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Ingredients_ItemId` (`ItemId`),
  KEY `IX_Ingredients_RecipeId` (`RecipeId`),
  CONSTRAINT `FK_Ingredients_Items_ItemId` FOREIGN KEY (`ItemId`) REFERENCES `Items` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Ingredients_Recipes_RecipeId` FOREIGN KEY (`RecipeId`) REFERENCES `Recipes` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=51585 DEFAULT CHARSET=utf8mb4;

-- Dataexport var bortvalt.

-- Dumpar struktur för tabell XivMarketBoard.Items
CREATE TABLE IF NOT EXISTS `Items` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` longtext NOT NULL,
  `CanBeCrafted` tinyint(1) DEFAULT NULL,
  `IsMarketable` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=37493 DEFAULT CHARSET=utf8mb4;

-- Dataexport var bortvalt.

-- Dumpar struktur för tabell XivMarketBoard.Jobs
CREATE TABLE IF NOT EXISTS `Jobs` (
  `Name` longtext NOT NULL,
  `UserName` varchar(255) DEFAULT NULL,
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`Id`),
  KEY `IX_Jobs_UserName` (`UserName`),
  CONSTRAINT `FK_Jobs_Users_UserName` FOREIGN KEY (`UserName`) REFERENCES `Users` (`UserName`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4;

-- Dataexport var bortvalt.

-- Dumpar struktur för tabell XivMarketBoard.Posts
CREATE TABLE IF NOT EXISTS `Posts` (
  `Id` varchar(255) NOT NULL,
  `UserName` varchar(255) DEFAULT NULL,
  `RetainerId` int(11) DEFAULT NULL,
  `RetainerName` longtext NOT NULL,
  `Price` double NOT NULL,
  `Amount` int(11) NOT NULL,
  `TotalAmount` double NOT NULL,
  `HighQuality` tinyint(1) NOT NULL,
  `LastReviewDate` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00.000000',
  `UniversalisEntryId` int(11) DEFAULT NULL,
  `SellerId` longtext NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Posts_RetainerId` (`RetainerId`),
  KEY `IX_Posts_UserName` (`UserName`),
  KEY `IX_Posts_UniversalisEntryId` (`UniversalisEntryId`),
  CONSTRAINT `FK_Posts_Retainers_RetainerId` FOREIGN KEY (`RetainerId`) REFERENCES `Retainers` (`Id`),
  CONSTRAINT `FK_Posts_UniversalisEntries_UniversalisEntryId` FOREIGN KEY (`UniversalisEntryId`) REFERENCES `UniversalisEntries` (`Id`),
  CONSTRAINT `FK_Posts_Users_UserName` FOREIGN KEY (`UserName`) REFERENCES `Users` (`UserName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Dataexport var bortvalt.

-- Dumpar struktur för tabell XivMarketBoard.Recipes
CREATE TABLE IF NOT EXISTS `Recipes` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ItemId` int(11) NOT NULL,
  `Name` longtext NOT NULL,
  `AmountResult` int(11) NOT NULL DEFAULT 0,
  `jobId` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  KEY `IX_Recipes_ItemId` (`ItemId`),
  KEY `IX_Recipes_jobId` (`jobId`),
  CONSTRAINT `FK_Recipes_Items_ItemId` FOREIGN KEY (`ItemId`) REFERENCES `Items` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Recipes_Jobs_jobId` FOREIGN KEY (`jobId`) REFERENCES `Jobs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=35026 DEFAULT CHARSET=utf8mb4;

-- Dataexport var bortvalt.

-- Dumpar struktur för tabell XivMarketBoard.Retainers
CREATE TABLE IF NOT EXISTS `Retainers` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` longtext NOT NULL,
  `UserName` varchar(255) NOT NULL,
  `Description` longtext DEFAULT NULL,
  `WorldId` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  KEY `IX_Retainers_UserName` (`UserName`),
  KEY `IX_Retainers_WorldId` (`WorldId`),
  CONSTRAINT `FK_Retainers_Users_UserName` FOREIGN KEY (`UserName`) REFERENCES `Users` (`UserName`) ON DELETE CASCADE,
  CONSTRAINT `FK_Retainers_Worlds_WorldId` FOREIGN KEY (`WorldId`) REFERENCES `Worlds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Dataexport var bortvalt.

-- Dumpar struktur för tabell XivMarketBoard.SaleHistory
CREATE TABLE IF NOT EXISTS `SaleHistory` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Quantity` int(11) NOT NULL,
  `HighQuality` tinyint(1) NOT NULL,
  `SaleDate` datetime(6) NOT NULL,
  `UniversalisEntryId` int(11) DEFAULT NULL,
  `Total` double NOT NULL,
  `BuyerName` longtext NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_SaleHistory_UniversalisEntryId` (`UniversalisEntryId`),
  CONSTRAINT `FK_SaleHistory_UniversalisEntries_UniversalisEntryId` FOREIGN KEY (`UniversalisEntryId`) REFERENCES `UniversalisEntries` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=23783 DEFAULT CHARSET=utf8mb4;

-- Dataexport var bortvalt.

-- Dumpar struktur för tabell XivMarketBoard.UniversalisEntries
CREATE TABLE IF NOT EXISTS `UniversalisEntries` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ItemId` int(11) NOT NULL,
  `WorldId` int(11) NOT NULL,
  `LastUploadDate` datetime(6) NOT NULL,
  `QueryDate` datetime(6) NOT NULL,
  `CurrentAveragePrice` double NOT NULL,
  `CurrentAveragePrinceNQ` double NOT NULL,
  `CurrentAveragePriceHQ` double NOT NULL,
  `RegularSaleVelocity` double NOT NULL,
  `NqSaleVelocity` double NOT NULL,
  `HqSaleVelocity` double NOT NULL,
  `AveragePrice` double NOT NULL,
  `AveragePriceNQ` double NOT NULL,
  `AveragePriceHQ` double NOT NULL,
  `MinPrice` double NOT NULL,
  `MinPriceNQ` double NOT NULL,
  `MinPriceHQ` double NOT NULL,
  `MaxPrice` double NOT NULL,
  `MaxPriceNQ` double NOT NULL,
  `MaxPriceHQ` double NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_UniversalisEntries_ItemId` (`ItemId`),
  KEY `IX_UniversalisEntries_WorldId` (`WorldId`),
  CONSTRAINT `FK_UniversalisEntries_Items_ItemId` FOREIGN KEY (`ItemId`) REFERENCES `Items` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_UniversalisEntries_Worlds_WorldId` FOREIGN KEY (`WorldId`) REFERENCES `Worlds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4768 DEFAULT CHARSET=utf8mb4;

-- Dataexport var bortvalt.

-- Dumpar struktur för tabell XivMarketBoard.Users
CREATE TABLE IF NOT EXISTS `Users` (
  `UserName` varchar(255) NOT NULL,
  `Email` longtext NOT NULL,
  `CharacterName` longtext DEFAULT NULL,
  PRIMARY KEY (`UserName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Dataexport var bortvalt.

-- Dumpar struktur för tabell XivMarketBoard.Worlds
CREATE TABLE IF NOT EXISTS `Worlds` (
  `Name` longtext NOT NULL,
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `DataCenterId` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  KEY `IX_Worlds_DataCenterId` (`DataCenterId`),
  CONSTRAINT `FK_Worlds_DataCenters_DataCenterId` FOREIGN KEY (`DataCenterId`) REFERENCES `DataCenters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=100 DEFAULT CHARSET=utf8mb4;

-- Dataexport var bortvalt.

-- Dumpar struktur för tabell XivMarketBoard.__EFMigrationsHistory
CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Dataexport var bortvalt.

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
