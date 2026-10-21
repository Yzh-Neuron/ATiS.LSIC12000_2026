@echo off
chcp 65001 >nul
:: 强制切换到bat自身所在目录，防止路径错乱
cd /d "%~dp0"

set "TOOL=Generic.exe"
set "TXT=Generic.txt"
set "OUT=Generic.xaml"

if not exist "%TOOL%" (
    echo [错误] 找不到 Generic.exe
    echo 完整路径：%~dp0%TOOL%
    pause
    exit /b 1
)

if not exist "%TXT%" (
    echo [错误] 找不到清单文件 Generic.txt
    pause
    exit /b 1
)

echo 正在执行合并...
"%TOOL%" "%TXT%" "%OUT%"

if exist "%OUT%" (
    echo 合并完成，输出：%~dp0%OUT%
) else (
    echo 合并失败，没有生成 Generic.xaml
)

echo.
pause
