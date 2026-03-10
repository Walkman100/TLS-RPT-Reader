@cd %~dp0
dotnet ef migrations add %* --context Reader.Common.Data.DBContext -o Data/Migrations
