@cd %~dp0
dotnet ef %* --context Reader.Common.Data.DBContext
