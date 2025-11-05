USE Electricity_FTA;
GO

--1. Hierarchy Metadata
--Network_Element_Hierarchy_Path :2 paths
CREATE TABLE Network_Element_Hierarchy_Path (
    Network_Element_Hierarchy_Path_Key INT PRIMARY KEY,
    Path_Name NVARCHAR(200) NOT NULL,
    Abbreviation NVARCHAR(200) NOT NULL
);

INSERT INTO Network_Element_Hierarchy_Path VALUES
(1, N'Governrate, Sector, Zone, City, Station, Tower, Cabin, Cable, Block, Building, Flat, Individual Subscription', 
    N'Governrate -> Individual Subscription'),
(2, N'Governrate, Sector, Zone, City, Station, Tower, Cabin, Cable, Block, Building, Corporate Subscription', 
    N'Governrate -> Corporate Subscription');

-----------------------------------------------------------------------------

--2. Network_Element_Type

CREATE TABLE Network_Element_Type (
    Network_Element_Type_Key INT PRIMARY KEY,
    Network_Element_Type_Name NVARCHAR(100) NOT NULL,
    Parent_Network_Element_Type_Key INT NULL,
    Network_Element_Hierarchy_Path_Key INT NOT NULL,
    CONSTRAINT FK_NET_ParentType FOREIGN KEY (Parent_Network_Element_Type_Key) 
        REFERENCES Network_Element_Type(Network_Element_Type_Key),
    CONSTRAINT FK_NET_HierarchyPath FOREIGN KEY (Network_Element_Hierarchy_Path_Key) 
        REFERENCES Network_Element_Hierarchy_Path(Network_Element_Hierarchy_Path_Key)
);


--Path 1: Individual Subscription >> his parent is flat 
--Path 2: Corporate Subscription >> his parent is building
INSERT INTO Network_Element_Type VALUES
(1,  N'Governorate', NULL, 1),
(2,  N'Sector', 1, 1),
(3,  N'Zone', 2, 1),
(4,  N'City', 3, 1),
(5,  N'Station', 4, 1),
(6,  N'Tower', 5, 1),
(7,  N'Cabin', 6, 1),
(8,  N'Cable', 7, 1),
(9,  N'Block', 8, 1),
(10, N'Building', 9, 1),
(11, N'Flat', 10, 1),
(12, N'Individual Subscription', 11, 1),
(13, N'Corporate Subscription', 10, 2);

---------------------------------------------------------------------------------

--3.Network_Element 
CREATE TABLE Network_Element (
    Network_Element_Key INT PRIMARY KEY,
    Network_Element_Name NVARCHAR(100) NOT NULL,
    Network_Element_Type_Key INT NOT NULL,
    Parent_Network_Element_Key INT NULL,
    Network_Element_Hierarchy_Path_Key INT NOT NULL,
    CONSTRAINT FK_NE_Type FOREIGN KEY (Network_Element_Type_Key) 
        REFERENCES Network_Element_Type(Network_Element_Type_Key),
    CONSTRAINT FK_NE_Parent FOREIGN KEY (Parent_Network_Element_Key) 
        REFERENCES Network_Element(Network_Element_Key),
    CONSTRAINT FK_NE_HierarchyPath FOREIGN KEY (Network_Element_Hierarchy_Path_Key) 
        REFERENCES Network_Element_Hierarchy_Path(Network_Element_Hierarchy_Path_Key)
);


CREATE NONCLUSTERED INDEX IX_Network_Element_Name 
    ON Network_Element(Network_Element_Name);

CREATE NONCLUSTERED INDEX IX_Network_Element_Parent 
    ON Network_Element(Parent_Network_Element_Key);

CREATE NONCLUSTERED INDEX IX_Network_Element_Type 
    ON Network_Element(Network_Element_Type_Key);
-------------------------------------------------------------------


-- 4. Sources  
CREATE TABLE Channel (
    Channel_Key INT PRIMARY KEY,
    Channel_Name NVARCHAR(100) NOT NULL UNIQUE
);

INSERT INTO Channel VALUES
(1, N'Source A'),
(2, N'Source B'),
(3, N'Manual');



-----------------------------------------------------------
--5.Problem_Types

CREATE TABLE Problem_Type (
    Problem_Type_Key INT PRIMARY KEY,
    Problem_Type_Name NVARCHAR(100) NOT NULL UNIQUE
);

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

-----------------------------------------------------------------

--6.tables of incidents 

--Header of incidents 
CREATE TABLE Cutting_Down_Header (
    Cutting_Down_Key INT IDENTITY(1,1) PRIMARY KEY,
    Cutting_Down_Incident_ID NVARCHAR(100) NOT NULL,
    Channel_Key INT NOT NULL,
    Cutting_Down_Problem_Type_Key INT NOT NULL,
    ActualCreateDate DATETIME NOT NULL,
    ActualEndDate DATETIME NULL,
    SynchCreateDate DATETIME NOT NULL,
    SynchUpdateDate DATETIME NULL,
    IsPlanned BIT NOT NULL DEFAULT 0,
    IsGlobal BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    PlannedStartDTS DATETIME NULL,
    PlannedEndDTS DATETIME NULL,
    CreateSystemUserID NVARCHAR(100) NOT NULL,
    UpdateSystemUserID NVARCHAR(100) NULL,
    CONSTRAINT FK_CDH_Channel FOREIGN KEY (Channel_Key) 
        REFERENCES Channel(Channel_Key),
    CONSTRAINT FK_CDH_ProblemType FOREIGN KEY (Cutting_Down_Problem_Type_Key) 
        REFERENCES Problem_Type(Problem_Type_Key),
    CONSTRAINT UQ_Incident_Channel UNIQUE(Cutting_Down_Incident_ID, Channel_Key)
);

-- Indexes
CREATE NONCLUSTERED INDEX IX_Header_CreateDate 
    ON Cutting_Down_Header(ActualCreateDate);

CREATE NONCLUSTERED INDEX IX_Header_EndDate 
    ON Cutting_Down_Header(ActualEndDate);

CREATE NONCLUSTERED INDEX IX_Header_Channel 
    ON Cutting_Down_Header(Channel_Key);

CREATE NONCLUSTERED INDEX IX_Header_IsActive 
    ON Cutting_Down_Header(IsActive);


-- Details incidents

CREATE TABLE Cutting_Down_Detail (
    Cutting_Down_Detail_Key INT IDENTITY(11,1) PRIMARY KEY,
    Cutting_Down_Key INT NOT NULL,
    Network_Element_Key INT NOT NULL,
    ActualCreateDate DATETIME NOT NULL,
    ActualEndDate DATETIME NULL,
    ImpactedCustomers INT NOT NULL DEFAULT 0,
    CONSTRAINT FK_CDD_Header FOREIGN KEY (Cutting_Down_Key) 
        REFERENCES Cutting_Down_Header(Cutting_Down_Key),
    CONSTRAINT FK_CDD_NetworkElement FOREIGN KEY (Network_Element_Key) 
        REFERENCES Network_Element(Network_Element_Key)
);

-- 
CREATE NONCLUSTERED INDEX IX_Detail_Header 
    ON Cutting_Down_Detail(Cutting_Down_Key);

CREATE NONCLUSTERED INDEX IX_Detail_NetworkElement 
    ON Cutting_Down_Detail(Network_Element_Key);


--Ignored incidents

CREATE TABLE Cutting_Down_Ignored (
    Cutting_Down_Incident_ID NVARCHAR(100) PRIMARY KEY,
    ActualCreateDate DATETIME NOT NULL,
    Cable_Name NVARCHAR(100) NULL,
    Cabin_Name NVARCHAR(100) NULL,
    SynchCreateDate DATETIME NOT NULL,
    CreatedUser NVARCHAR(100) NOT NULL
);

--------------------------------------------------------