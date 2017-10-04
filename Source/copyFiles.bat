@echo off
SET "ProjectName=BetterMiniMap"
SET "SolutionDir=C:\Users\robin\Desktop\Games\RimWorld Modding\Source\%ProjectName%\Source"
@echo on

xcopy /S /Y "%SolutionDir%\..\About\*" "C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\%ProjectName%\About\"
xcopy /S /Y "%SolutionDir%\..\Textures\*" "C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\%ProjectName%\Textures\"
xcopy /S /Y "%SolutionDir%\..\Languages\*" "C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\%ProjectName%\Languages\"