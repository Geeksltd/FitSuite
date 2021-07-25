@echo off

cd Website
call yarn
call tsc
call pushd wwwroot\styles\build\ && .\sass-to-css.bat && popd
call dotnet build