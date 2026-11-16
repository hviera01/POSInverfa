#define AppName "COMERCIAL INVERFA"
#define AppVersion "1.0.1"
#ifndef SourceDir
  #define SourceDir "..\CapaPresentacion\bin\Release"
#endif

[Setup]
AppId={{6E3C1D52-4B7A-4F1E-9C2B-INVERFA0WINF}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher=Comercial Inverfa
DefaultDirName={autopf}\COMERCIAL INVERFA
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
OutputBaseFilename=Instalador_ComercialInverfa_{#AppVersion}
SetupIconFile=..\CapaPresentacion\logoInverfa-modified.ico
UninstallDisplayIcon={app}\COMERCIAL INVERFA.exe
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=admin
CloseApplications=force
WizardStyle=modern

[Languages]
Name: "es"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "Crear acceso directo en el escritorio"; GroupDescription: "Accesos directos:"

[Files]
Source: "{#SourceDir}\*"; DestDir: "{app}"; Excludes: "*.pdb,*.xml,app.publish"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\COMERCIAL INVERFA.exe"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\COMERCIAL INVERFA.exe"; Tasks: desktopicon

[Run]
Filename: "{sys}\sc.exe"; Parameters: "config ""MSSQL$SQLEXPRESS"" start= auto"; Flags: runhidden waituntilterminated; StatusMsg: "Configurando SQL Server..."
Filename: "{sys}\sc.exe"; Parameters: "start ""MSSQL$SQLEXPRESS"""; Flags: runhidden waituntilterminated
Filename: "{app}\COMERCIAL INVERFA.exe"; Description: "Abrir {#AppName}"; Flags: nowait postinstall skipifsilent

[Code]
procedure CerrarProceso(Nombre: String);
var
  Res: Integer;
begin
  Exec(ExpandConstant('{sys}\taskkill.exe'), '/F /IM "' + Nombre + '"', '', SW_HIDE, ewWaitUntilTerminated, Res);
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
begin
  CerrarProceso('COMERCIAL INVERFA.exe');
  CerrarProceso('CapaPresentacion.exe');
  Result := '';
end;
