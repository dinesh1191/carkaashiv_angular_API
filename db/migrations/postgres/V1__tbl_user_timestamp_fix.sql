
-- V1__tbl_user_timestamp_fix.sql
-- Purpose: Fix tbl_user timestamp handling (Neon PostgreSQL)

-- Ensure correct column type
ALTER TABLE tbl_user
ALTER COLUMN idt TYPE TIMESTAMP WITH TIME ZONE;

-- Set default timestamp
ALTER TABLE tbl_user
ALTER COLUMN idt SET DEFAULT CURRENT_TIMESTAMP;
