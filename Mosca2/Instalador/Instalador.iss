[Languages]
Name: "brazilian"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

[Setup]
AppName=Mosca
AppVersion=2.0.0
AppPublisher=Boca Software
DefaultDirName={pf}\Mosca
DefaultGroupName=Mosca
OutputDir=.
SetupIconFile="..\Mosca.ico"
UninstallDisplayIcon={app}\Mosca.ico
OutputBaseFilename=Mosca-2.0.0
Compression=lzma
SolidCompression=yes
DisableProgramGroupPage=yes

[Files]
; Copia todos os arquivos da pasta Release para a pasta de instalação
Source: "..\bin\Release\net10.0-windows\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\Mosca.ico"; DestDir: "{app}"

[Icons]
Name: "{group}\Mosca"; Filename: "{app}\Mosca.exe"
Name: "{commondesktop}\Mosca"; Filename: "{app}\Mosca.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Criar atalho na área de trabalho"; GroupDescription: "Opções adicionais:"

[Run]
; Executa o programa após a instalação
Filename: "{app}\Mosca.exe"; Description: "Executar Mosca"; Flags: nowait postinstall skipifsilent
