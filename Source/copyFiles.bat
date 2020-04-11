@echo off
SET "ProjectName=BetterMiniMap"
SET "SolutionDir=C:\Users\robin\Desktop\Games\RimWorld Modding\Source\%ProjectName%\Source"
SET "RWModsDir=D:\SteamLibrary\steamapps\common\RimWorld\Mods"
@echo on

xcopy /S /Y "%SolutionDir%\..\About\*" "%RWModsDir%\%ProjectName%\About\"
xcopy /S /Y "%SolutionDir%\..\Textures\*" "%RWModsDir%\%ProjectName%\Textures\"
xcopy /S /Y "%SolutionDir%\..\Languages\*" "%RWModsDir%\%ProjectName%\Languages\"