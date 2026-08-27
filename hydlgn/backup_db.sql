CREATE PROCEDURE dbo.BackupDatabaseDirect
    @TargetDatabaseName NVARCHAR(128),
    @BackupFilePath NVARCHAR(260)
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Validate that the database actually exists before backing up
    IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = @TargetDatabaseName)
    BEGIN
        RAISERROR('The specified database "%s" does not exist on this server.', 16, 1, @TargetDatabaseName);
        RETURN;
    END

    -- 2. Construct the dynamic BACKUP DATABASE command
    -- FORMAT overwrites any existing backup file at that path
    -- COMPRESSION reduces file size and speeds up modern disk I/O
    DECLARE @BackupSql NVARCHAR(MAX);
    SET @BackupSql = N'BACKUP DATABASE ' + QUOTENAME(@TargetDatabaseName) + 
                     N' TO DISK = ' + QUOTENAME(@BackupFilePath, '''') + 
                     N' WITH FORMAT, COMPRESSION, STATS = 10, ' +
                     N'NAME = ' + QUOTENAME(N'Full Backup of ' + @TargetDatabaseName, '''') + N';';

    -- 3. Construct the verification command to ensure the backup file is valid
    DECLARE @VerifySql NVARCHAR(MAX);
    SET @VerifySql = N'RESTORE VERIFYONLY FROM DISK = ' + QUOTENAME(@BackupFilePath, '''') + N';';

    BEGIN TRY
        -- 4. Execute the backup process
        PRINT 'Executing database backup for "' + @TargetDatabaseName + '"...';
        EXEC sp_executesql @BackupSql;
        
        -- 5. Execute the verification process
        PRINT 'Verifying backup file integrity...';
        EXEC sp_executesql @VerifySql;
        
        PRINT 'Database "' + @TargetDatabaseName + '" backed up and verified successfully at: ' + @BackupFilePath;
    END TRY
    BEGIN CATCH
        PRINT 'An error occurred during the database backup process.';
        THROW;
    END CATCH
END;
GO