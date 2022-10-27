#define MyAppName "Solasta Community Mods"
#define MyAppVersion "1.2.15"
#define MyAppPublisher "Zappastuff"

[Setup]
AppId={{DCF5942E-2376-466C-B1D0-350D20764DFD}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName=D:\games\Solasta Crown of The Magister
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=C:\Users\paulo\Downloads
OutputBaseFilename=SetupSolastaCommunityMods-{#MyAppVersion}
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Types]
Name: "full"; Description: "Full installation"
Name: "mods"; Description: "Mods installation"
Name: "custom"; Description: "Custom installation"; Flags: iscustom

[Components]
Name: "UMM"; Description: "Unity Mod Manager 0.24"; Types: full
Name: "Patch"; Description: "Patching Strategy"; Types: full
Name: "Patch\Doorstep"; Description: "Doorstep"; Flags: exclusive
Name: "Patch\Assembly"; Description: "Assembly"; Flags: exclusive
Name: "SolastaModApi"; Description: "Solasta Mod Api 1.2.15.6"; Types: full mods custom; Flags: fixed
Name: "SolastaUnfinishedBusiness"; Description: "Unfinished Business 1.2.15.7"; Types: full mods custom;
Name: "SolastaUnfinishedBusinessMulticlass"; Description: "Unfinished Business Multiclass 1.2.15.17"; Types: full mods custom;
Name: "SolastaDungeonMakerPro"; Description: "Dungeon Maker Pro 1.2.15.3"; Types: full mods custom;

[Files]
;Source: "{app}\Solasta_Data\Managed\UnityEngine.UIModule.dll"; DestDir: "{app}"; DestName: "UnityEngine.UIModule.dll.original"; Flags: external skipifsourcedoesntexist uninsneveruninstall
Source: "D:\PAYLOAD\UMM\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: UMM
Source: "doorstop_config.ini"; DestDir: "{app}"; Components: Patch\Doorstep; 
Source: "winhttp.dll"; DestDir: "{app}"; Components: Patch\Doorstep;
Source: "UnityEngine.UIModule.dll"; DestDir: "{app}\Solasta_Data\Managed"; Components: Patch\Assembly;
Source: "D:\PAYLOAD\Mods\SolastaModApi\*"; DestDir: "{app}\Mods\SolastaModApi"; Components: SolastaModApi
Source: "D:\PAYLOAD\Mods\SolastaUnfinishedBusiness\*"; DestDir: "{app}\Mods\SolastaUnfinishedBusiness"; Components: SolastaUnfinishedBusiness
Source: "D:\PAYLOAD\Mods\SolastaUnfinishedBusinessMulticlass\*"; DestDir: "{app}\Mods\SolastaUnfinishedBusinessMulticlass"; Components: SolastaUnfinishedBusinessMulticlass
Source: "D:\PAYLOAD\Mods\SolastaDungeonMakerPro\*"; DestDir: "{app}\Mods\SolastaDungeonMakerPro"; Components: SolastaDungeonMakerPro

[Icons]
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"


