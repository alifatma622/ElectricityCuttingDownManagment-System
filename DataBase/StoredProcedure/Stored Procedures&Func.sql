USE  Electricity_FTA
GO
--Stored Procedures
--SP_BuildHierarchy


CREATE PROCEDURE SP_BuildHierarchy
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
 
        -- 1. Governorate
		--  النتيجة: يتم نقل   المحافظات  من داتا الاصليه  بدون اب  الي الداتا المعالجه 
        INSERT INTO Network_Element (Network_Element_Key, Network_Element_Name, Network_Element_Type_Key, Parent_Network_Element_Key, Network_Element_Hierarchy_Path_Key)
        SELECT DISTINCT G.Governorate_Key, G.Governorate_Name, 1, NULL, 1
        FROM Electricity_STA.dbo.Governorate G
        WHERE NOT EXISTS (SELECT 1 FROM Network_Element NE WHERE NE.Network_Element_Key = G.Governorate_Key);
        

        -- 2. Sector
        INSERT INTO Network_Element (Network_Element_Key, Network_Element_Name, Network_Element_Type_Key, Parent_Network_Element_Key, Network_Element_Hierarchy_Path_Key)
        SELECT DISTINCT S.Sector_Key, S.Sector_Name, 2, S.Governorate_Key, 1
        FROM Electricity_STA.dbo.Sector S
        WHERE NOT EXISTS (SELECT 1 FROM Network_Element NE WHERE NE.Network_Element_Key = S.Sector_Key);
      

        -- 3. Zone
        INSERT INTO Network_Element (Network_Element_Key, Network_Element_Name, Network_Element_Type_Key, Parent_Network_Element_Key, Network_Element_Hierarchy_Path_Key)
        SELECT DISTINCT Z.Zone_Key, Z.Zone_Name, 3, Z.Sector_Key, 1
        FROM Electricity_STA.dbo.Zone Z
        WHERE NOT EXISTS (SELECT 1 FROM Network_Element NE WHERE NE.Network_Element_Key = Z.Zone_Key);
     

        -- 4. City
        INSERT INTO Network_Element (Network_Element_Key, Network_Element_Name, Network_Element_Type_Key, Parent_Network_Element_Key, Network_Element_Hierarchy_Path_Key)
        SELECT DISTINCT C.City_Key, C.City_Name, 4, C.Zone_Key, 1
        FROM Electricity_STA.dbo.City C
        WHERE NOT EXISTS (SELECT 1 FROM Network_Element NE WHERE NE.Network_Element_Key = C.City_Key);
        

        -- 5. Station
        INSERT INTO Network_Element (Network_Element_Key, Network_Element_Name, Network_Element_Type_Key, Parent_Network_Element_Key, Network_Element_Hierarchy_Path_Key)
        SELECT DISTINCT ST.Station_Key, ST.Station_Name, 5, ST.City_Key, 1
        FROM Electricity_STA.dbo.Station ST
        WHERE NOT EXISTS (SELECT 1 FROM Network_Element NE WHERE NE.Network_Element_Key = ST.Station_Key);
        

        -- 6. Tower
        INSERT INTO Network_Element (Network_Element_Key, Network_Element_Name, Network_Element_Type_Key, Parent_Network_Element_Key, Network_Element_Hierarchy_Path_Key)
        SELECT DISTINCT T.Tower_Key, T.Tower_Name, 6, T.Station_Key, 1
        FROM Electricity_STA.dbo.Tower T
        WHERE NOT EXISTS (SELECT 1 FROM Network_Element NE WHERE NE.Network_Element_Key = T.Tower_Key);
       
        -- 7. Cabin
        INSERT INTO Network_Element (Network_Element_Key, Network_Element_Name, Network_Element_Type_Key, Parent_Network_Element_Key, Network_Element_Hierarchy_Path_Key)
        SELECT DISTINCT CB.Cabin_Key, CB.Cabin_Name, 7, CB.Tower_Key, 1
        FROM Electricity_STA.dbo.Cabin CB
        WHERE NOT EXISTS (SELECT 1 FROM Network_Element NE WHERE NE.Network_Element_Key = CB.Cabin_Key);
        

        -- 8. Cable
        INSERT INTO Network_Element (Network_Element_Key, Network_Element_Name, Network_Element_Type_Key, Parent_Network_Element_Key, Network_Element_Hierarchy_Path_Key)
        SELECT DISTINCT CL.Cable_Key, CL.Cable_Name, 8, CL.Cabin_Key, 1
        FROM Electricity_STA.dbo.Cable CL
        WHERE NOT EXISTS (SELECT 1 FROM Network_Element NE WHERE NE.Network_Element_Key = CL.Cable_Key);
       

        -- 9. Block
        INSERT INTO Network_Element (Network_Element_Key, Network_Element_Name, Network_Element_Type_Key, Parent_Network_Element_Key, Network_Element_Hierarchy_Path_Key)
        SELECT DISTINCT BL.Block_Key, BL.Block_Name, 9, BL.Cable_Key, 1
        FROM Electricity_STA.dbo.Block BL
        WHERE NOT EXISTS (SELECT 1 FROM Network_Element NE WHERE NE.Network_Element_Key = BL.Block_Key);
       

        -- 10. Building
        INSERT INTO Network_Element (Network_Element_Key, Network_Element_Name, Network_Element_Type_Key, Parent_Network_Element_Key, Network_Element_Hierarchy_Path_Key)
        SELECT DISTINCT BD.Building_Key, BD.Building_Name, 10, BD.Block_Key, 1
        FROM Electricity_STA.dbo.Building BD
        WHERE NOT EXISTS (SELECT 1 FROM Network_Element NE WHERE NE.Network_Element_Key = BD.Building_Key);
        
        -- 11. Flat
        INSERT INTO Network_Element (Network_Element_Key, Network_Element_Name, Network_Element_Type_Key, Parent_Network_Element_Key, Network_Element_Hierarchy_Path_Key)
        SELECT DISTINCT FL.Flat_Key, FL.Flat_Name, 11, FL.Building_Key, 1
        FROM Electricity_STA.dbo.Flat FL
        WHERE NOT EXISTS (SELECT 1 FROM Network_Element NE WHERE NE.Network_Element_Key = FL.Flat_Key);
       

        -- 12. Individual Subscriptions
        INSERT INTO Network_Element (Network_Element_Key, Network_Element_Name, Network_Element_Type_Key, Parent_Network_Element_Key, Network_Element_Hierarchy_Path_Key)
        SELECT DISTINCT SUB.Subscription_Key, CAST(SUB.Subscription_Key AS NVARCHAR(100)), 12, SUB.Flat_Key, 1
        FROM Electricity_STA.dbo.Subscription SUB
        WHERE SUB.Flat_Key IS NOT NULL
        AND NOT EXISTS (SELECT 1 FROM Network_Element NE WHERE NE.Network_Element_Key = SUB.Subscription_Key);
        

        -- 13. Corporate Subscriptions
        INSERT INTO Network_Element (Network_Element_Key, Network_Element_Name, Network_Element_Type_Key, Parent_Network_Element_Key, Network_Element_Hierarchy_Path_Key)
        SELECT DISTINCT SUB.Subscription_Key + 1000, CAST(SUB.Subscription_Key AS NVARCHAR(100)) + N'-Corporate', 13, SUB.Building_Key, 2
        FROM Electricity_STA.dbo.Subscription SUB
        WHERE SUB.Flat_Key IS NULL AND SUB.Building_Key IS NOT NULL
        AND NOT EXISTS (SELECT 1 FROM Network_Element NE WHERE NE.Network_Element_Key = SUB.Subscription_Key + 1000);
        
        COMMIT TRANSACTION;
     
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        PRINT ' Wrong: ' + @ErrMsg;
        THROW;
    END CATCH
END;
GO

------------------------------------------------------------------------------------------------
-- 6.2 SP_Create
--SP_Create: نقل الحوادث المفتوحة بس  من الداتا الاصليه لداتل المعالجه 
GO
CREATE PROCEDURE SP_Create
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @ProcessedA INT = 0, @IgnoredA INT = 0;
        DECLARE @ProcessedB INT = 0, @IgnoredB INT = 0;
        DECLARE @TotalProcessed INT = 0;

        -- ═══ Source A (Cabins) ═══
        DECLARE @IncidentsA TABLE (
            SourceId INT, CabinName NVARCHAR(100), ProblemType INT,
            CreateDate DATETIME, NetworkElementKey INT, IsPlanned BIT,
            IsGlobal BIT, PlannedStartDTS DATETIME, PlannedEndDTS DATETIME
        );

        INSERT INTO @IncidentsA
        SELECT A.Cutting_Down_A_Incident_ID, cb.Cabin_Name, A.Problem_Type_Key,
               A.CreateDate, NE.Network_Element_Key, A.IsPlanned, A.IsGlobal,
               A.PlannedStartDTS, A.PlannedEndDTS
        FROM Electricity_STA.dbo.Cutting_Down_A A
        INNER JOIN Electricity_STA.dbo.Cabin cb ON cb.Cabin_Key = A.Cabin_Key
        INNER JOIN Network_Element NE ON cb.Cabin_Name = NE.Network_Element_Name
        WHERE A.EndDate IS NULL 
        AND A.IsProcessed = 0;

        -- Insert Headers for Source A
        INSERT INTO Cutting_Down_Header (Cutting_Down_Incident_ID, Channel_Key, Cutting_Down_Problem_Type_Key,
            ActualCreateDate, SynchCreateDate, IsPlanned, IsGlobal, IsActive,
            PlannedStartDTS, PlannedEndDTS, CreateSystemUserID)
        SELECT CAST(IA.SourceId AS NVARCHAR(100)), 1, IA.ProblemType,
               IA.CreateDate, GETDATE(), IA.IsPlanned, IA.IsGlobal, 1,
               IA.PlannedStartDTS, IA.PlannedEndDTS, 'Source A user'
        FROM @IncidentsA IA
        WHERE NOT EXISTS (
            SELECT 1 FROM Cutting_Down_Header h 
            WHERE h.Cutting_Down_Incident_ID = CAST(IA.SourceId AS NVARCHAR(100))
            AND h.Channel_Key = 1
        );
        SET @ProcessedA = @@ROWCOUNT;

        -- Insert Details for Source A
        INSERT INTO Cutting_Down_Detail (Cutting_Down_Key, Network_Element_Key,
            ActualCreateDate, ImpactedCustomers)
        SELECT H.Cutting_Down_Key, IA.NetworkElementKey, IA.CreateDate,
               dbo.FN_CalculateImpactedCustomers(IA.CabinName)
        FROM @IncidentsA IA
        JOIN Cutting_Down_Header H ON H.Cutting_Down_Incident_ID = CAST(IA.SourceId AS NVARCHAR(100)) 
        AND H.Channel_Key = 1
        WHERE NOT EXISTS (
            SELECT 1 FROM Cutting_Down_Detail d 
            WHERE d.Cutting_Down_Key = H.Cutting_Down_Key
        );

        -- Insert Ignored for Source A
        INSERT INTO Cutting_Down_Ignored (Cutting_Down_Incident_ID, ActualCreateDate,
            Cable_Name, Cabin_Name, SynchCreateDate, CreatedUser)
        SELECT CAST(A.Cutting_Down_A_Incident_ID AS NVARCHAR(100)), A.CreateDate,
               NULL, cb.Cabin_Name, GETDATE(), 'Source A user'
        FROM Electricity_STA.dbo.Cutting_Down_A A
        INNER JOIN Electricity_STA.dbo.Cabin cb ON cb.Cabin_Key = A.Cabin_Key
        WHERE A.EndDate IS NULL 
        AND A.IsProcessed = 0
        AND NOT EXISTS (SELECT 1 FROM Network_Element NE WHERE NE.Network_Element_Name = cb.Cabin_Name)
        AND NOT EXISTS (
            SELECT 1 FROM Cutting_Down_Ignored ci 
            WHERE ci.Cutting_Down_Incident_ID = CAST(A.Cutting_Down_A_Incident_ID AS NVARCHAR(100))
        );
        SET @IgnoredA = @@ROWCOUNT;

        -- Mark Source A as processed
        UPDATE Electricity_STA.dbo.Cutting_Down_A
        SET IsProcessed = 1
        WHERE EndDate IS NULL AND IsProcessed = 0;

        -- ═══ Source B (Cables) ═══
        DECLARE @IncidentsB TABLE (
            SourceId INT, CableName NVARCHAR(100), ProblemType INT,
            CreateDate DATETIME, NetworkElementKey INT, IsPlanned BIT,
            IsGlobal BIT, PlannedStartDTS DATETIME, PlannedEndDTS DATETIME
        );

        INSERT INTO @IncidentsB
        SELECT B.Cutting_Down_B_Incident_ID, cb.Cable_Name, B.Problem_Type_Key,
               B.CreateDate, NE.Network_Element_Key, B.IsPlanned, B.IsGlobal,
               B.PlannedStartDTS, B.PlannedEndDTS
        FROM Electricity_STA.dbo.Cutting_Down_B B
        INNER JOIN Electricity_STA.dbo.Cable cb ON B.Cable_Key = cb.Cable_Key
        INNER JOIN Network_Element NE ON cb.Cable_Name = NE.Network_Element_Name
        WHERE B.EndDate IS NULL 
        AND B.IsProcessed = 0;

        -- Insert Headers for Source B
        INSERT INTO Cutting_Down_Header (Cutting_Down_Incident_ID, Channel_Key, Cutting_Down_Problem_Type_Key,
            ActualCreateDate, SynchCreateDate, IsPlanned, IsGlobal, IsActive,
            PlannedStartDTS, PlannedEndDTS, CreateSystemUserID)
        SELECT CAST(IB.SourceId AS NVARCHAR(100)), 2, IB.ProblemType,
               IB.CreateDate, GETDATE(), IB.IsPlanned, IB.IsGlobal, 1,
               IB.PlannedStartDTS, IB.PlannedEndDTS, 'Source B user'
        FROM @IncidentsB IB
        WHERE NOT EXISTS (
            SELECT 1 FROM Cutting_Down_Header h 
            WHERE h.Cutting_Down_Incident_ID = CAST(IB.SourceId AS NVARCHAR(100))
            AND h.Channel_Key = 2
        );
        SET @ProcessedB = @@ROWCOUNT;

        -- Insert Details for Source B
        INSERT INTO Cutting_Down_Detail (Cutting_Down_Key, Network_Element_Key,
            ActualCreateDate, ImpactedCustomers)
        SELECT H.Cutting_Down_Key, IB.NetworkElementKey, IB.CreateDate,
               dbo.FN_CalculateImpactedCustomers(IB.CableName)
        FROM @IncidentsB IB
        JOIN Cutting_Down_Header H ON H.Cutting_Down_Incident_ID = CAST(IB.SourceId AS NVARCHAR(100)) 
        AND H.Channel_Key = 2
        WHERE NOT EXISTS (
            SELECT 1 FROM Cutting_Down_Detail d 
            WHERE d.Cutting_Down_Key = H.Cutting_Down_Key
        );

        -- Insert Ignored for Source B
        INSERT INTO Cutting_Down_Ignored (Cutting_Down_Incident_ID, ActualCreateDate,
            Cable_Name, Cabin_Name, SynchCreateDate, CreatedUser)
        SELECT CAST(B.Cutting_Down_B_Incident_ID AS NVARCHAR(100)), B.CreateDate,
               cb.Cable_Name, NULL, GETDATE(), 'Source B user'
        FROM Electricity_STA.dbo.Cutting_Down_B B
        INNER JOIN Electricity_STA.dbo.Cable cb ON cb.Cable_Key = B.Cable_Key
        WHERE B.EndDate IS NULL 
        AND B.IsProcessed = 0
        AND NOT EXISTS (SELECT 1 FROM Network_Element NE WHERE NE.Network_Element_Name = cb.Cable_Name)
        AND NOT EXISTS (
            SELECT 1 FROM Cutting_Down_Ignored ci 
            WHERE ci.Cutting_Down_Incident_ID = CAST(B.Cutting_Down_B_Incident_ID AS NVARCHAR(100))
        );
        SET @IgnoredB = @@ROWCOUNT;

        -- Mark Source B as processed
        UPDATE Electricity_STA.dbo.Cutting_Down_B
        SET IsProcessed = 1
        WHERE EndDate IS NULL AND IsProcessed = 0;

        -- Calculate total
        SET @TotalProcessed = @ProcessedA + @ProcessedB + @IgnoredA + @IgnoredB;

        PRINT '✓ SP_Create completed successfully';
        PRINT '  Source A - Headers: ' + CAST(@ProcessedA AS VARCHAR) + ', Ignored: ' + CAST(@IgnoredA AS VARCHAR);
        PRINT '  Source B - Headers: ' + CAST(@ProcessedB AS VARCHAR) + ', Ignored: ' + CAST(@IgnoredB AS VARCHAR);
        PRINT '  Total Processed: ' + CAST(@TotalProcessed AS VARCHAR);

        COMMIT TRANSACTION;
        
        -- Return total count
        RETURN @TotalProcessed;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrLine INT = ERROR_LINE();
        PRINT '❌ ERROR at line ' + CAST(@ErrLine AS VARCHAR) + ': ' + @ErrMsg;
        RETURN -1;
    END CATCH
END;
GO
---------------------------------------------------------------------------------------------
-- 6.3 SP_Close


CREATE PROCEDURE SP_Close
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;  
        DECLARE @ClosedA INT = 0;
        DECLARE @ClosedB INT = 0;
        DECLARE @DetailsClosed INT = 0;
        --  Source A (Cabins):Close incidents
        UPDATE H
        SET 
            H.ActualEndDate = A.EndDate,
            H.SynchUpdateDate = GETDATE(),
            H.UpdateSystemUserID = 'Source A user'
        FROM Cutting_Down_Header H
        INNER JOIN Electricity_STA.dbo.Cutting_Down_A A
            ON H.Cutting_Down_Incident_ID = CAST(A.Cutting_Down_A_Incident_ID AS NVARCHAR(100))
        WHERE 
            A.IsProcessed = 1                    
            AND A.EndDate IS NOT NULL            
            AND H.Channel_Key = 1                
            AND H.ActualEndDate IS NULL;         
        
        SET @ClosedA = @@ROWCOUNT;
        
        IF @ClosedA > 0
        BEGIN
            PRINT '   تم إغلاق ' + CAST(@ClosedA AS VARCHAR) + ' حادثة في Header';
            
            --  Details uppdate related to headers
            UPDATE D
            SET 
                D.ActualEndDate = H.ActualEndDate
            FROM Cutting_Down_Detail D
            INNER JOIN Cutting_Down_Header H
                ON D.Cutting_Down_Key = H.Cutting_Down_Key
            WHERE 
                H.Channel_Key = 1
                AND H.ActualEndDate IS NOT NULL
                AND D.ActualEndDate IS NULL;
            
            PRINT '  تم تحديث ' + CAST(@@ROWCOUNT AS VARCHAR) + ' Detail';
        END
        ELSE
        BEGIN
            PRINT '  لا توجد حوادث Source A للإغلاق';
        END
        
        PRINT '';

        --source B
     
        UPDATE H
        SET 
            H.ActualEndDate = B.EndDate,
            H.SynchUpdateDate = GETDATE(),
            H.UpdateSystemUserID = 'Source B user'
        FROM Cutting_Down_Header H
        INNER JOIN Electricity_STA.dbo.Cutting_Down_B B
            ON H.Cutting_Down_Incident_ID = CAST(B.Cutting_Down_B_Incident_ID AS NVARCHAR(100))
        WHERE 
            B.IsProcessed = 1
            AND B.EndDate IS NOT NULL
            AND H.Channel_Key = 2                -- Source B
            AND H.ActualEndDate IS NULL;
        
        SET @ClosedB = @@ROWCOUNT;
        
        IF @ClosedB > 0
        BEGIN
            PRINT ' تم إغلاق ' + CAST(@ClosedB AS VARCHAR) + ' حادثة في Header';
            
         
            UPDATE D
            SET 
                D.ActualEndDate = H.ActualEndDate
            FROM Cutting_Down_Detail D
            INNER JOIN Cutting_Down_Header H
                ON D.Cutting_Down_Key = H.Cutting_Down_Key
            WHERE 
                H.Channel_Key = 2
                AND H.ActualEndDate IS NOT NULL
                AND D.ActualEndDate IS NULL;
            
            SET @DetailsClosed = @@ROWCOUNT;
            PRINT '  تم تحديث ' + CAST(@DetailsClosed AS VARCHAR) + ' Detail';
        END
        ELSE
        BEGIN
            PRINT ' لا توجد حوادث Source B للإغلاق';
        END
        
        COMMIT TRANSACTION;
        
   
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrLine INT = ERROR_LINE();
        DECLARE @ErrSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrState INT = ERROR_STATE();
        RAISERROR(@ErrMsg, @ErrSeverity, @ErrState);
    END CATCH
END;
GO
------------------------------------------------------------------------------
-- SP_Get_Parent_Elements
-- Gets all parent elements up the hierarchy for a given element

CREATE PROCEDURE SP_Get_Parent_Elements
    @Network_Element_Key INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        WITH ParentHierarchy AS (
            -- Base case: Start with the given element
            SELECT
                NE.Network_Element_Key,
                NE.Network_Element_Name,
                NE.Parent_Network_Element_Key,
                NET.Network_Element_Type_Name,
                CAST(NET.Network_Element_Type_Name AS NVARCHAR(MAX)) AS PathName,
                1 AS Level
            FROM Network_Element NE
            INNER JOIN Network_Element_Type NET 
                ON NE.Network_Element_Type_Key = NET.Network_Element_Type_Key
            WHERE NE.Network_Element_Key = @Network_Element_Key
            
            UNION ALL
            
            -- Recursive case: Move up to parent
            SELECT
                ParentNE.Network_Element_Key,
                ParentNE.Network_Element_Name,
                ParentNE.Parent_Network_Element_Key,
                ParentNET.Network_Element_Type_Name,
                CAST(ParentNET.Network_Element_Type_Name + ' => ' + PH.PathName AS NVARCHAR(MAX)),
                PH.Level + 1
            FROM Network_Element ParentNE
            INNER JOIN Network_Element_Type ParentNET 
                ON ParentNE.Network_Element_Type_Key = ParentNET.Network_Element_Type_Key
            INNER JOIN ParentHierarchy PH 
                ON ParentNE.Network_Element_Key = PH.Parent_Network_Element_Key
            WHERE ParentNE.Parent_Network_Element_Key IS NOT NULL
        )
        SELECT
            Network_Element_Key,
            Network_Element_Name,
            Network_Element_Type_Name,
            PathName,
            Level
        FROM ParentHierarchy
        ORDER BY Level;
        
    END TRY
    BEGIN CATCH
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        PRINT 'ERROR in SP_Get_Parent_Elements: ' + @ErrMsg;
        THROW;
    END CATCH
END;
GO
----------------------------------------------------------------------------------------------------
-- SP_Get_Child_Elements
-- Gets all direct child elements for a given parent element
CREATE PROCEDURE SP_Get_Child_Elements
    @Network_Element_Key INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT
            NE.Network_Element_Key,
            NE.Network_Element_Name,
            NE.Network_Element_Type_Key,
            NET.Network_Element_Type_Name,
            NE.Parent_Network_Element_Key,
            (SELECT COUNT(*) FROM Network_Element NE2 
             WHERE NE2.Parent_Network_Element_Key = NE.Network_Element_Key) AS ChildCount
        FROM Network_Element NE
        INNER JOIN Network_Element_Type NET 
            ON NE.Network_Element_Type_Key = NET.Network_Element_Type_Key
        WHERE NE.Parent_Network_Element_Key = @Network_Element_Key
        ORDER BY NE.Network_Element_Name;
        
    END TRY
    BEGIN CATCH
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        PRINT 'ERROR in SP_Get_Child_Elements: ' + @ErrMsg;
        THROW;
    END CATCH
END;
GO

-----------------------------------------------------------------------------
--Function
-- 5.1  حساب الناس المتأثرين لعنصر ده من الشبكه 

GO
CREATE FUNCTION dbo.FN_CalculateImpactedCustomers
(
    @ElementName NVARCHAR(100)
)
RETURNS INT
AS
BEGIN

    DECLARE @ImpactedCount INT = 0;
    DECLARE @ElementKey INT;
    DECLARE @ElementTypeKey INT;
    
    SELECT 
        @ElementKey = Network_Element_Key,
        @ElementTypeKey = Network_Element_Type_Key
    FROM Network_Element
    WHERE Network_Element_Name = @ElementName;
    
 
    IF @ElementKey IS NULL
    BEGIN
        RETURN 0;
    END
    
 
    IF @ElementTypeKey = 12 OR @ElementTypeKey = 13
    BEGIN
        
        SET @ImpactedCount = 1;
    END
    ELSE IF @ElementTypeKey = 11
    BEGIN
       
        SELECT @ImpactedCount = COUNT(*)
        FROM Network_Element
        WHERE Parent_Network_Element_Key = @ElementKey
          AND Network_Element_Type_Key = 12;  -- Individual Subscriptions
    END
    ELSE IF @ElementTypeKey = 10
    BEGIN
       
        SELECT @ImpactedCount = COUNT(*)
        FROM Network_Element Flat
        INNER JOIN Network_Element Sub 
            ON Sub.Parent_Network_Element_Key = Flat.Network_Element_Key
            AND Sub.Network_Element_Type_Key = 12
        WHERE Flat.Parent_Network_Element_Key = @ElementKey
          AND Flat.Network_Element_Type_Key = 11;
        
        
        SELECT @ImpactedCount = @ImpactedCount + COUNT(*)
        FROM Network_Element
        WHERE Parent_Network_Element_Key = @ElementKey
          AND Network_Element_Type_Key = 13;  -- Corporate Subscriptions
    END
    ELSE
    BEGIN
       
        
        WITH ChildElements AS
        (
            SELECT 
                Network_Element_Key,
                Network_Element_Type_Key
            FROM Network_Element
            WHERE Network_Element_Key = @ElementKey
            
            UNION ALL

            SELECT 
                NE.Network_Element_Key,
                NE.Network_Element_Type_Key
            FROM Network_Element NE
            INNER JOIN ChildElements CE 
                ON NE.Parent_Network_Element_Key = CE.Network_Element_Key
        )
      
        SELECT @ImpactedCount = COUNT(*)
        FROM ChildElements
        WHERE Network_Element_Type_Key IN (12, 13);  -- Individual & Corporate
    END
    
    RETURN @ImpactedCount;
END;
GO


--------------------------------------------




