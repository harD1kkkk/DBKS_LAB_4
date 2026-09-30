IF DB_ID(N'BooksDB') IS NULL
BEGIN
    CREATE DATABASE BooksDB;
END
GO

USE BooksDB;
GO

IF OBJECT_ID(N'dbo.Books', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Books
    (
        Id INT IDENTITY(1,1) CONSTRAINT PK_Books PRIMARY KEY,
        Title NVARCHAR(100) NOT NULL,
        Text NVARCHAR(MAX) NOT NULL,
        ImageData VARBINARY(MAX) NULL
    );
END
GO

SELECT * FROM dbo.Books;
GO
