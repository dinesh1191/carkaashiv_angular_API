-- V1__tbl_user_timestamp_fix.sql
-- Purpose: Fix user timestamp handling

-- Ensure idt column type
ALTER TABLE tbl_user
ALTER COLUMN idt DATETIME2 NOT NULL;
GO

-- Drop existing default constraint (safe)
DECLARE @constraintName NVARCHAR(200);

SELECT @constraintName = dc.name
FROM sys.default_constraints dc
JOIN sys.columns c
  ON dc.parent_object_id = c.object_id
 AND dc.parent_column_id = c.column_id
WHERE OBJECT_NAME(dc.parent_object_id) = 'tbl_user'
  AND c.name = 'idt';

IF @constraintName IS NOT NULL
BEGIN
    EXEC('ALTER TABLE tbl_user DROP CONSTRAINT ' + @constraintName);
END
GO

-- Add correct default
ALTER TABLE tbl_user
ADD CONSTRAINT DF_tbl_user_idt
DEFAULT SYSDATETIME() FOR idt;
GO
