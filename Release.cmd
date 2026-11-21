@echo off
setlocal

REM 1. Publicar em Release (x64): .NET framework-dependent
if exist Mosca2\bin\Publicar rmdir /s /q Mosca2\bin\Publicar
dotnet publish Mosca2\Mosca2.csproj -c Release -r win-x64 --self-contained false -o Mosca2\bin\Publicar
if errorlevel 1 (
    echo Erro ao publicar o projeto.
	pause
    exit /b 1
)

REM 2. Excluir o arquivo do instalador anterior, se existir
if exist Mosca2\Instalador\Mosca-*.exe (
    del /f /q Mosca2\Instalador\Mosca-*.exe
)

REM 3. Buildar o instalador com o Inno Setup
iscc Mosca2\Instalador\Instalador.iss
if errorlevel 1 (
    echo Erro ao gerar o instalador.
	pause
    exit /b 1
)

echo Processo concluído com sucesso.
endlocal
pause
