REM 

set ProjectDir="%1"
set BuiltOuputPath="%2"
set Config="%3"

REM Add the "Launch <app>" checkbox on the Install Complete screen
cscript.exe "%ProjectDir%AddLaunchCheckbox.js" "%BuiltOuputPath%"

REM Sign the installation file with scribd.p12
REM REM "C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\signtool"  sign /f ../scribd.p12 /p hnvz7jf43q4wl /v /t http://tsa.starfieldtech.com /d "Scribd.com's Desktop Uploader" /du "www.scribd.com" "desktopinstaller.msi"
