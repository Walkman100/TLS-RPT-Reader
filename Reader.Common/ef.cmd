@cd %~dp0
dotnet ef %* --project Reader.Common --context Reader.Common.Data.DBContext
