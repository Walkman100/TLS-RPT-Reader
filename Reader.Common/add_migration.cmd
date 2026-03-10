@cd %~dp0
dotnet ef migrations add %* --project Reader.Common --context Redaer.Common.Data.DBContext -o Data/Migrations
