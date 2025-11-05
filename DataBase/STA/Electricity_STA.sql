CREATE DATABASE Electricity_STA;
GO

USE Electricity_STA;
GO


--Network Hierarchy tables
/*Governorate -> Sector -> Zone -> City -> Station -> Tower -> Cabin -> Cable -> Block -> Building -> Flat -> Subscription*/

-- 1 
CREATE TABLE Governorate (
    Governorate_Key INT PRIMARY KEY IDENTITY(1,1),
    Governorate_Name NVARCHAR(100) NOT NULL UNIQUE
);
-- 2 
CREATE TABLE Sector (
    Sector_Key INT PRIMARY KEY IDENTITY(1,1),
    Governorate_Key INT NOT NULL,
    Sector_Name NVARCHAR(100) NOT NULL,
    FOREIGN KEY (Governorate_Key) REFERENCES Governorate(Governorate_Key),
    CONSTRAINT UQ_Sector UNIQUE(Governorate_Key, Sector_Name)
);

-- 3
CREATE TABLE Zone (
    Zone_Key INT PRIMARY KEY IDENTITY(1,1),
    Sector_Key INT NOT NULL,
    Zone_Name NVARCHAR(100) NOT NULL,
    FOREIGN KEY (Sector_Key) REFERENCES Sector(Sector_Key),
    CONSTRAINT UQ_Zone UNIQUE(Sector_Key, Zone_Name)
);

-- 4 
CREATE TABLE City (
    City_Key INT PRIMARY KEY IDENTITY(1,1),
    Zone_Key INT NOT NULL,
    City_Name NVARCHAR(100) NOT NULL,
    FOREIGN KEY (Zone_Key) REFERENCES Zone(Zone_Key),
    CONSTRAINT UQ_City UNIQUE(Zone_Key, City_Name)
);

-- 5
CREATE TABLE Station (
    Station_Key INT PRIMARY KEY IDENTITY(1,1),
    City_Key INT NOT NULL,
    Station_Name NVARCHAR(100) NOT NULL,
    FOREIGN KEY (City_Key) REFERENCES City(City_Key),
    CONSTRAINT UQ_Station UNIQUE(City_Key, Station_Name)
);
-- 6
CREATE TABLE Tower (
    Tower_Key INT PRIMARY KEY IDENTITY(1,1),
    Station_Key INT NOT NULL,
    Tower_Name NVARCHAR(100) NOT NULL,
    FOREIGN KEY (Station_Key) REFERENCES Station(Station_Key),
    CONSTRAINT UQ_Tower UNIQUE(Station_Key, Tower_Name)
);
-- 7
CREATE TABLE Cabin (
    Cabin_Key INT PRIMARY KEY IDENTITY(1,1),
    Tower_Key INT NOT NULL,
    Cabin_Name NVARCHAR(100) NOT NULL UNIQUE,
    FOREIGN KEY (Tower_Key) REFERENCES Tower(Tower_Key)
);
-- 8
CREATE TABLE Cable (
    Cable_Key INT PRIMARY KEY IDENTITY(1,1),
    Cabin_Key INT NOT NULL,
    Cable_Name NVARCHAR(100) NOT NULL UNIQUE,
    FOREIGN KEY (Cabin_Key) REFERENCES Cabin(Cabin_Key)
);

-- 9
CREATE TABLE Block (
    Block_Key INT PRIMARY KEY IDENTITY(1,1),
    Cable_Key INT NOT NULL,
    Block_Name NVARCHAR(100) NOT NULL,
    FOREIGN KEY (Cable_Key) REFERENCES Cable(Cable_Key)
);

-- 10
CREATE TABLE Building (
    Building_Key INT PRIMARY KEY IDENTITY(1,1),
    Block_Key INT NOT NULL,
    Building_Name NVARCHAR(100) NOT NULL,
    FOREIGN KEY (Block_Key) REFERENCES Block(Block_Key)
);

--11
CREATE TABLE Flat (
    Flat_Key INT PRIMARY KEY IDENTITY(1,1),
    Building_Key INT NOT NULL,
    Flat_Name NVARCHAR(100) NOT NULL,
    FOREIGN KEY (Building_Key) REFERENCES Building(Building_Key)
);

--12
CREATE TABLE Subscription (
    Subscription_Key INT PRIMARY KEY IDENTITY(1,1),
    Flat_Key INT NULL,              -- اشتراك فردي
    Building_Key INT NULL,           -- اشتراك مؤسسي
    Meter_Key INT NULL,
    Palet_Key INT NULL,
    CHECK (Flat_Key IS NOT NULL OR Building_Key IS NOT NULL)
);

---------------------------------------------------------

--
CREATE TABLE Problem_Type (
    Problem_Type_Key INT PRIMARY KEY,
    Problem_Type_Name NVARCHAR(100) NOT NULL UNIQUE
);


CREATE TABLE Users (
    User_Key INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Password NVARCHAR(100) NOT NULL
);


--------------------------------------------
--Elec Incidents

--1. Source A :Incident in Cabina
CREATE TABLE Cutting_Down_A (
    Cutting_Down_A_Incident_ID INT PRIMARY KEY IDENTITY(1,1),
    Cabin_Key INT NOT NULL,
    Problem_Type_Key INT NOT NULL,
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    EndDate DATETIME NULL,                      -- NULL =close ,value =open
    IsPlanned BIT NOT NULL DEFAULT 0,
    IsGlobal BIT NOT NULL DEFAULT 0,
    PlannedStartDTS DATETIME NULL,
    PlannedEndDTS DATETIME NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedUser NVARCHAR(50) NOT NULL DEFAULT 'Source A user',
    UpdatedUser NVARCHAR(50) NULL,
    IsProcessed BIT NOT NULL DEFAULT 0,         --this mean is processed and transfer to FAT table
    FOREIGN KEY (Cabin_Key) REFERENCES Cabin(Cabin_Key),
    FOREIGN KEY (Problem_Type_Key) REFERENCES Problem_Type(Problem_Type_Key)
);


--2. Source B :Incident in Cable
CREATE TABLE Cutting_Down_B (
    Cutting_Down_B_Incident_ID INT PRIMARY KEY IDENTITY(1,1),
    Cable_Key INT NOT NULL,
    Problem_Type_Key INT NOT NULL,
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    EndDate DATETIME NULL,                     
    IsPlanned BIT NOT NULL DEFAULT 0,
    IsGlobal BIT NOT NULL DEFAULT 0,
    PlannedStartDTS DATETIME NULL,
    PlannedEndDTS DATETIME NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedUser NVARCHAR(50) NOT NULL DEFAULT 'Source B user',
    UpdatedUser NVARCHAR(50) NULL,
    IsProcessed BIT NOT NULL DEFAULT 0,         
    FOREIGN KEY (Cable_Key) REFERENCES Cable(Cable_Key),
    FOREIGN KEY (Problem_Type_Key) REFERENCES Problem_Type(Problem_Type_Key)
);

---------------------------------------------------------------------
--seed data

INSERT INTO Problem_Type VALUES 
(1, N'حريق'),
(2, N'ضغط عالي'),
(3, N'استهلاك عالي'),
(4, N'مديونية'),
(5, N'تلف عداد'),
(6, N'سرقة تيار'),
(7, N'سمطة'),
(8, N'سرقة توصيلة محايدة'),
(9, N'سرقة توصيلة خط'),
(10, N'تحديث عداد'),
(11, N'صيانة'),
(12, N'كابل مقطوع'),
(13, N'توصيل كابل');

INSERT INTO Users VALUES 
('admin', 'admin'),
('test', 'test'),
('SourceA', 'Source_A'),
('SourceB', 'Source_B');


INSERT INTO Governorate VALUES (N'Cairo'), (N'Alex'), (N'Giza'), (N'Suez');

INSERT INTO Sector VALUES 
(1, N'North'),
(1, N'East'),
(1, N'West'),
(1, N'South');

INSERT INTO Zone VALUES 
(1, N'منطقة أولى'),
(1, N'منطقة ثانية'),
(1, N'منطقة ثالثة'),
(1, N'منطقة رابعة');


INSERT INTO City VALUES 
(1, N'Nasr City'),
(1, N'Al Salam City'),
(2, N'Dar Al Salam'),
(2, N'Helwan');


INSERT INTO Station VALUES 
(1, N'prod-1-1'),
(1, N'prod-1-2'),
(2, N'prod-2-1'),
(2, N'prod-2-2');


INSERT INTO Tower VALUES 
(1, N'dc-1-1'),
(1, N'dc-1-2'),
(2, N'dc-2-1'),
(2, N'dc-2-2');


INSERT INTO Cabin VALUES 
(1, N'cab-1-1'),
(1, N'cab-1-2'),
(1, N'cab-1-3'),
(1, N'cab-1-4'),
(1, N'cab-1-5'),
(1, N'cab-1-6'),
(2, N'cab-2-1'),
(2, N'cab-2-2');

INSERT INTO Cable VALUES 
(1, N'ch-1-1'),
(1, N'ch-1-2'),
(1, N'ch-1-3'),
(1, N'ch-1-4'),
(1, N'ch-1-5'),
(1, N'ch-1-6'),
(2, N'ch-2-1'),
(3, N'ch-2-2');


INSERT INTO Block VALUES 
(1, N'111-111-111'),
(1, N'222-222-222'),
(2, N'333-333-333'),
(2, N'444-444-444');


INSERT INTO Building VALUES 
(1, N'asd-1-1'),
(1, N'asd-1-2'),
(2, N'asd-2-1'),
(2, N'asd-2-2');

INSERT INTO Flat VALUES 
(1, N'1'),
(1, N'2'),
(2, N'3'),
(2, N'4');

-- Subscription
INSERT INTO Subscription VALUES 
(1, NULL, 1, 11),  -- فردي
(2, NULL, 2, NULL),
(NULL, 2, NULL, NULL),  -- مؤسسي
(NULL, 2, NULL, NULL);


-- Source A:cabina
--open accidents
INSERT INTO Cutting_Down_A (Cabin_Key, Problem_Type_Key, CreateDate, EndDate, IsPlanned, IsGlobal) VALUES
(1, 1, '2020-01-01', NULL, 0, 0),  -- cab-1-1, حريق
(2, 2, '2021-01-01', NULL, 1, 0),  -- cab-1-2, ضغط عالي
(3, 3, '2022-01-01', NULL, 0, 1);  -- cab-1-3, استهلاك عالي

-- closed accidents
INSERT INTO Cutting_Down_A (Cabin_Key, Problem_Type_Key, CreateDate, EndDate, IsPlanned, IsGlobal, PlannedStartDTS, PlannedEndDTS) VALUES
(4, 4, '2019-01-01', '2019-06-30', 1, 1, '2019-01-01', '2019-06-30'),  
(5, 5, '2020-03-01', '2020-06-30', 0, 0, NULL, NULL),                  
(6, 6, '2021-02-01', '2021-10-30', 1, 1, '2021-01-01', '2021-11-30'); 

--------------------------------------------
-- Source B :cables
-- open accidents
INSERT INTO Cutting_Down_B (Cable_Key, Problem_Type_Key, CreateDate, EndDate, IsPlanned, IsGlobal) VALUES
(1, 1, '2020-01-01', NULL, 0, 0),  -- ch-1-1
(2, 2, '2021-01-01', NULL, 1, 0),  -- ch-1-2
(3, 3, '2022-01-01', NULL, 0, 1);  -- ch-1-3

-- closed accidents
INSERT INTO Cutting_Down_B (Cable_Key, Problem_Type_Key, CreateDate, EndDate, IsPlanned, IsGlobal, PlannedStartDTS, PlannedEndDTS) VALUES
(4, 4, '2019-01-01', '2019-06-30', 1, 1, '2019-01-01', '2019-06-30'),
(5, 5, '2020-03-01', '2020-06-30', 0, 0, NULL, NULL),
(6, 6, '2021-02-01', '2021-10-30', 1, 1, '2021-01-01', '2021-11-30');



/*SELECT COUNT(*) FROM Governorate
SELECT COUNT(*) FROM Cabin
SELECT COUNT(*) FROM Cable
SELECT COUNT(*) FROM Flat
SELECT COUNT(*) FROM Cutting_Down_A
SELECT COUNT(*) FROM Cutting_Down_B
*/
