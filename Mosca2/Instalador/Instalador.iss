[Languages]
Name: "brazilian"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

[Setup]
AppName=Mosca
AppVersion=2.0.0
AppPublisher=Boca Software
DefaultDirName={autopf}\Mosca
DefaultGroupName=Mosca
OutputDir=.
SetupIconFile="..\Mosca.ico"
WizardImageFile=WizardImage164.png,WizardImage192.png,WizardImage246.png,WizardImage273.png,WizardImage328.png
WizardSmallImageFile=WizardSmall58.png,WizardSmall87.png,WizardSmall116.png
UninstallDisplayIcon={app}\Mosca.ico
OutputBaseFilename=Mosca-2.0.0
Compression=lzma
SolidCompression=yes
DisableProgramGroupPage=yes
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

[Files]
; Copia todos os arquivos da pasta Publicar para a pasta de instalação
Source: "..\bin\Publicar\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\Mosca.ico"; DestDir: "{app}"

[Icons]
Name: "{group}\Mosca"; Filename: "{app}\Mosca.exe"
Name: "{commondesktop}\Mosca"; Filename: "{app}\Mosca.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Criar atalho na área de trabalho"; GroupDescription: "Opções adicionais:"

[Run]
; Executa o programa após a instalação
Filename: "{app}\Mosca.exe"; Description: "Executar Mosca"; Flags: nowait postinstall skipifsilent
