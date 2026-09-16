@echo off
SETLOCAL ENABLEDELAYEDEXPANSION

dotnet build --configuration Release

set SOURCE_URL=http://git.cdhk.hr:12345/v3/index.json
set API_KEY=57f8d3d5-16c4-4b8b-8929-196d2d9686f2
set PROJECT_PATH=%1
set CONFIG=Release
set OUTPUT_DIR=.\bin\%CONFIG%

echo.
echo ==============================
echo 打包 %PROJECT_PATH%
echo ==============================
dotnet pack "%PROJECT_PATH%" -c %CONFIG% -o "%OUTPUT_DIR%" --no-build

if ERRORLEVEL 1 (
    echo 打包失败！
    exit /b 1
)

echo.
echo ==============================
echo 查找最新生成的 .nupkg
echo ==============================
set "LATEST_PACKAGE="

for /f "delims=" %%f in ('dir /b /o-d "%OUTPUT_DIR%\*.nupkg"') do (
    set "LATEST_PACKAGE=%%f"
    goto :found
)

:found
if "%LATEST_PACKAGE%"=="" (
    echo 没有找到 .nupkg 文件！
    exit /b 1
)

echo 最新包: %LATEST_PACKAGE%

echo.
echo ==============================
echo 推送到 NuGet 服务器 %SOURCE_URL%
echo ==============================
dotnet nuget push -s %SOURCE_URL% -k %API_KEY% "%OUTPUT_DIR%\%LATEST_PACKAGE%" --skip-duplicate

if ERRORLEVEL 1 (
    echo 推送失败！
    exit /b 1
)

echo.
echo ==============================
echo 完成！
echo ==============================
pause