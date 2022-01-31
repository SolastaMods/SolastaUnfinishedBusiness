from os import listdir
from os.path import isfile, join
onlyfiles = [f for f in listdir("D:\games\Slasta_COTM\Solasta_Data\Managed") if isfile(join("D:\games\Slasta_COTM\Solasta_Data\Managed", f))]

for file in onlyfiles:
    if file.endswith("dll") and not file.startswith("System"):
        print(f"<Reference Include='{file}'>\n\t<HintPath>$([System.IO.Path]::Combine($(SolastaInstallDir), 'Solasta_Data\Managed\{file}'))</HintPath>\n\t<Private>false</Private>\n</Reference>")