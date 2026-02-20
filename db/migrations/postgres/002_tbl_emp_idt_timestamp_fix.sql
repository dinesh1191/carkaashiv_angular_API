/*
 Migration: 002_tbl_idt_timestamp_fix
 DB: PostgreSQL(Neon)
 Purpose: Set default timestamp for tbl_emp.idt
 Reason: API should not Send CreadtedAt; DB must manage timestamps 
 */
ALTER TABLE tbl_emp
ALTER COLUMN idt SET DEFAULT CURRENT_TIMESTAMP;


ALTER TABLE tbl_emp
RENAME COLUMN isactive TO "IsActive";

select * from tbl_emp t 