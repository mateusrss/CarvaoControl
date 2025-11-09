[Setup]
AppName=Carvão Control
AppVersion=1.0.0
AppPublisher=Chama Distribuidora
AppPublisherURL=https://www.chamadistribuidora.com
AppSupportURL=https://www.chamadistribuidora.com/support
AppUpdatesURL=https://www.chamadistribuidora.com/updates
DefaultDirName={pf}\CarvaoControl
DefaultGroupName=Carvão Control
OutputDir=userdocs:Inno Setup Examples Output
OutputBaseFilename=CarvaoControlSetup
Compression=lzma
SolidCompression=yes

[Languages]
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "bin\Release\net9.0-windows\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "c:\Users\teste\CarvaoControl\MeuAppWinForms\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Carvão Control"; Filename: "{app}\CarvaoControl.exe"
Name: "{commondesktop}\Carvão Control"; Filename: "{app}\CarvaoControl.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\CarvaoControl.exe"; Description: "{cm:LaunchProgram,Carvão Control}"; Flags: nowait postinstall skipifsilent