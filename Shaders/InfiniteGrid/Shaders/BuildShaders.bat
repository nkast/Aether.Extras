@echo off
setlocal

cd %~dp0

SET MGFX="C:\Program Files (x86)\KNI\v4.2\Tools\KNIFXC.exe"
SET XNAFX="..\..\Tools\CompileEffect\CompileEffect.exe"

@echo .
@echo Build dx11
@for /f %%f IN ('dir /b *.fx') do (
    @echo .
    @echo Compile %%~nf
    call %MGFX% %%~nf.fx ..\Resources\%%~nf.dx11.fxo /Platform:Windows
)

@echo .
@echo Build ogl
@for /f %%f IN ('dir /b *.fx') do (
    @echo .
    @echo Compile %%~nf
    call %MGFX% %%~nf.fx ..\Resources\%%~nf.ogl.fxo /Platform:DesktopGL
)

@echo .
@echo Build gles
@for /f %%f IN ('dir /b *.fx') do (
    @echo .
    @echo Compile %%~nf
    call %MGFX% %%~nf.fx ..\Resources\%%~nf.gles.fxo /Platform:Android
)

@echo .
@echo Build dx9/xna Reach
@for /f %%f IN ('dir /b *.Reach.fx') do (
    @echo .
    @echo Compile %%~nf
    call %XNAFX% Windows Reach %%~nf.fx ..\Resources\%%~nf.xna
)

@echo .
@echo Build dx9/xna HiDef
@for /f %%f IN ('dir /b *.HiDef.fx') do (
    @echo .
    @echo Compile %%~nf
    call %XNAFX% Windows HiDef %%~nf.fx ..\Resources\%%~nf.xna
)

endlocal
@pause
