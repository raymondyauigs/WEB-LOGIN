ALTER PROCEDURE [dbo].[RestoreDatabaseWithNewLogicalNames]
    @BackupFilePath NVARCHAR(260),
    @TargetDatabaseName NVARCHAR(128),
    @DataDirectory NVARCHAR(260),
    @LogDirectory NVARCHAR(260),
    @NewDataLogicalName NVARCHAR(128), -- Added Parameter
    @NewLogLogicalName NVARCHAR(128)   -- Added Parameter
AS
BEGIN
    SET NOCOUNT ON;

    -- Ensure directories end with a backslash
    IF RIGHT(@DataDirectory, 1) <> '\' SET @DataDirectory = @DataDirectory + '\';
    IF RIGHT(@LogDirectory, 1) <> '\' SET @LogDirectory = @LogDirectory + '\';

    -- Table variable to hold backup file layout details
    DECLARE @FileList TABLE (
        LogicalName NVARCHAR(128), PhysicalName NVARCHAR(260), Type CHAR(1), 
        FileGroupName NVARCHAR(128), Size NUMERIC(20,0), MaxSize NUMERIC(20,0), 
        FileId BIGINT, CreateLSN NUMERIC(25,0), DropLSN NUMERIC(25,0), 
        UniqueId UNIQUEIDENTIFIER, ReadOnlyLSN NUMERIC(25,0), ReadWriteLSN NUMERIC(25,0),
        BackupSizeInBytes BIGINT, SourceBlockSize INT, FileGroupId INT, 
        LogGroupGUID UNIQUEIDENTIFIER, DifferentialBaseLSN NUMERIC(25,0), 
        DifferentialBaseGUID UNIQUEIDENTIFIER, IsReadOnly BIT, IsPresent BIT, 
        TDEThumbprint VARBINARY(32), SnapshotURL NVARCHAR(360)
    );

    -- Get list of files from the backup
    DECLARE @ListSql NVARCHAR(MAX) = 'RESTORE FILELISTONLY FROM DISK = ' + QUOTENAME(@BackupFilePath, '''');
    INSERT INTO @FileList EXEC sp_executesql @ListSql;

    -- Variables to construct the dynamic RESTORE command
    DECLARE @LogicalName NVARCHAR(128);
    DECLARE @FileType CHAR(1);
    DECLARE @FileId INT;
    DECLARE @NewPhysicalName NVARCHAR(500);
    DECLARE @MoveClause NVARCHAR(MAX) = '';

    -- Cursor to loop through the data and log files in the backup (Added FileId)
    DECLARE FileCursor CURSOR LOCAL FAST_FORWARD FOR 
    SELECT LogicalName, Type, FileId FROM @FileList;

    OPEN FileCursor;
    FETCH NEXT FROM FileCursor INTO @LogicalName, @FileType, @FileId;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Determine new physical path and append MOVE clause
        IF @FileType = 'D' -- Data File
        BEGIN
            -- Ensure naming patterns use Target Database Name and the parameterised logical identity
            IF @FileId = 1
                SET @NewPhysicalName = @DataDirectory + @TargetDatabaseName + '_' + @NewDataLogicalName + '.mdf';
            ELSE
                SET @NewPhysicalName = @DataDirectory + @TargetDatabaseName + '_' + @LogicalName + '.ndf'; -- Secondary data files handler
                
            SET @MoveClause = @MoveClause + ', MOVE ' + QUOTENAME(@LogicalName, '''') + ' TO ' + QUOTENAME(@NewPhysicalName, '''');
        END
        ELSE IF @FileType = 'L' -- Log File
        BEGIN
            IF @FileId = 2
                SET @NewPhysicalName = @LogDirectory + @TargetDatabaseName + '_' + @NewLogLogicalName + '.ldf';
            ELSE
                SET @NewPhysicalName = @LogDirectory + @TargetDatabaseName + '_' + @LogicalName + '.ldf'; -- Secondary log files handler
                
            SET @MoveClause = @MoveClause + ', MOVE ' + QUOTENAME(@LogicalName, '''') + ' TO ' + QUOTENAME(@NewPhysicalName, '''');
        END

        FETCH NEXT FROM FileCursor INTO @LogicalName, @FileType, @FileId;
    END

    CLOSE FileCursor;
    DEALLOCATE FileCursor;

    -- Construct and execute the full RESTORE statement
    DECLARE @RestoreSql NVARCHAR(MAX);
    SET @RestoreSql = 'RESTORE DATABASE ' + QUOTENAME(@TargetDatabaseName) + 
                      ' FROM DISK = ' + QUOTENAME(@BackupFilePath, '''') + 
                      ' WITH REPLACE, STATS = 10' + @MoveClause + ';';

    BEGIN TRY
        -- Drop connections to target database if it already exists to avoid "Database in use" error
        IF EXISTS (SELECT 1 FROM sys.databases WHERE name = @TargetDatabaseName)
        BEGIN
            EXEC ('ALTER DATABASE ' + @TargetDatabaseName + ' SET SINGLE_USER WITH ROLLBACK IMMEDIATE');
        END

        -- Execute the restore
        EXEC sp_executesql @RestoreSql;

        -- Identify original primary logical names from backup to pass into ALTER statement
        DECLARE @OldDataLogicalName NVARCHAR(128);
        DECLARE @OldLogLogicalName NVARCHAR(128);

        SELECT TOP 1 @OldDataLogicalName = LogicalName FROM @FileList WHERE Type = 'D' ORDER BY FileId ASC;
        SELECT TOP 1 @OldLogLogicalName = LogicalName FROM @FileList WHERE Type = 'L' ORDER BY FileId ASC;

        -- Apply the new logical name changes inside database system catalogs
        DECLARE @RenameSql NVARCHAR(MAX);
        
        SET @RenameSql = 'ALTER DATABASE ' + QUOTENAME(@TargetDatabaseName) + ' MODIFY FILE (NAME = ' + QUOTENAME(@OldDataLogicalName, '''') + ', NEWNAME = ' + QUOTENAME(@NewDataLogicalName, '''') + ');';
        EXEC sp_executesql @RenameSql;

        SET @RenameSql = 'ALTER DATABASE ' + QUOTENAME(@TargetDatabaseName) + ' MODIFY FILE (NAME = ' + QUOTENAME(@OldLogLogicalName, '''') + ', NEWNAME = ' + QUOTENAME(@NewLogLogicalName, '''') + ');';
        EXEC sp_executesql @RenameSql;
        
        -- Bring database back to multi-user mode
        EXEC ('ALTER DATABASE ' + @TargetDatabaseName + ' SET MULTI_USER');
        
        PRINT 'Database restored and logical files renamed successfully as: ' + @TargetDatabaseName;
    END TRY
    BEGIN CATCH
        -- Ensure database is not stuck in single user mode if restore fails mid-way
        IF EXISTS (SELECT 1 FROM sys.databases WHERE name = @TargetDatabaseName)
        BEGIN
            EXEC ('ALTER DATABASE ' + @TargetDatabaseName + ' SET MULTI_USER');
        END
        PRINT 'Error occurred during database restore.';
        THROW;
    END CATCH
END;