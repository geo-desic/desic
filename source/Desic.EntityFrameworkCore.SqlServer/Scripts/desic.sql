DECLARE @FastMode         BIT = $(FastMode);
DECLARE @DatabaseCreated  BIT = 0;
DECLARE @LoginCreated     BIT = 0;
DECLARE @UserCreated      BIT = 0;
DECLARE @RoleCreated      BIT = 0;

-- create database if it does not exist
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE [name] = 'Desic')
BEGIN
    CREATE DATABASE [Desic];
    SET @DatabaseCreated = 1;
    PRINT 'Database [Desic] created';
END
ELSE
BEGIN
    PRINT 'Database [Desic] already exists';
END

IF (@FastMode = 1 AND @DatabaseCreated = 0)
BEGIN
    SET NOEXEC ON;
    RETURN;
END

-- create login if it does not exist
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE [name] = '$(DbUserName)')
BEGIN
    CREATE LOGIN [$(DbUserName)] WITH PASSWORD = '$(DbUserPassword)';
    PRINT 'LOGIN [$(DbUserName)] created';
END

IF (@FastMode = 1 AND @LoginCreated = 0)
BEGIN
    SET NOEXEC ON;
    RETURN;
END

GO
USE [Desic];

-- create schemas if they do not exist
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE [name] = 'ref')
BEGIN
    EXEC('CREATE SCHEMA [ref]');
    PRINT 'Schema [ref] created';
END

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE [name] = 'app')
BEGIN
    EXEC('CREATE SCHEMA [app]');
    PRINT 'Schema [app] created';
END

GO
USE [Desic];

DECLARE @FastMode         BIT = $(FastMode);
DECLARE @UserCreated      BIT = 0;
DECLARE @RoleCreated      BIT = 0;

-- create user if it does not exist
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE [name] = '$(DbUserName)')
BEGIN
   CREATE USER [$(DbUserName)] FOR LOGIN [$(DbUserName)];
   SET @UserCreated = 1;
   PRINT 'User [$(DbUserName)] created';
END

IF (@FastMode = 1 AND @UserCreated = 0)
BEGIN
    SET NOEXEC ON;
    RETURN;
END

-- create role if it does not exist
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE [name] = 'rl_$(DbUserName)')
BEGIN
    CREATE ROLE [rl_$(DbUserName)];
    SET @RoleCreated = 1;
    PRINT 'Role [rl_$(DbUserName)] created';
END

IF (@FastMode = 1 AND @RoleCreated = 0)
BEGIN
    SET NOEXEC ON;
    RETURN;
END

-- add user to role
ALTER ROLE [rl_$(DbUserName)] ADD MEMBER [$(DbUserName)];
PRINT 'User $(DbUserName) added to role [rl_$(DbUserName)]';

-- grant permissions
GRANT SELECT ON SCHEMA::[ref] TO [rl_$(DbUserName)];
GRANT SELECT ON SCHEMA::[app] TO [rl_$(DbUserName)];
GRANT INSERT ON SCHEMA::[app] TO [rl_$(DbUserName)];
GRANT UPDATE ON SCHEMA::[app] TO [rl_$(DbUserName)];
GRANT DELETE ON SCHEMA::[app] TO [rl_$(DbUserName)];
PRINT 'Permissions granted to role [rl_$(DbUserName)]';

SET NOEXEC OFF;
GO