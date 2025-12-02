@echo off
setlocal enabledelayedexpansion

REM Determine script directory
set SCRIPT_DIR=%~dp0

REM Call PowerShell script, capture exit code
powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%start-api-dev.ps1" %*
set EXITCODE=%ERRORLEVEL%

if %EXITCODE% EQU 0 (
    echo.
    echo [OK] Script completed successfully.
) else (
    echo.
    echo [ERROR] Script failed with exit code %EXITCODE%.
)

echo.
echo Press any key to close this window...
pause >nul

endlocal
exit /b %EXITCODE%
