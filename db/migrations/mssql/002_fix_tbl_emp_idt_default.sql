/*
Migration: 002_fix_tbl_emp_idt_default
Purpose: Fix tbl_emp.idt column to auto-insert current datatime
DB: Mssql
Reason:API should not send CreatedAt;DB must handle timestamp
*/

-- 1. Drop existing default constraint (if any)
DECLARE @constraintName NVARCHAR(200);

SELECT @constraintName = dc.name
FROM sys.default_constraints dc
JOIN sys.columns c 
    ON dc.parent_object_id = c.object_id 
   AND dc.parent_column_id = c.column_id
WHERE OBJECT_NAME(dc.parent_object_id) = 'tbl_emp'
  AND c.name = 'idt';

IF @constraintName IS NOT NULL
    EXEC('ALTER TABLE tbl_emp DROP CONSTRAINT ' + @constraintName);

-- 2. Add default SYSDATETIME()
ALTER TABLE tbl_emp
ADD CONSTRAINT DF_tbl_emp_idt
DEFAULT SYSDATETIME() FOR idt;

