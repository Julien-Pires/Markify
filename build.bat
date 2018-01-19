build\nant-0.92\bin\NAnt.exe -f:"%cd%"\default.build %1
if %ERRORLEVEL% == 0 goto :next

:quit
exit /b %ERRORLEVEL%

:next
@echo.
@echo %date%
@echo %time%
@echo.
