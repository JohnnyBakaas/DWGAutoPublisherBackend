@echo off
setlocal enabledelayedexpansion
set dwgfile=%~1
set layoutnames=%~2
set plotscript="C:\Test for Autocad greier\P-10001\DWG\plot.scr"

REM Page setup name
set page_setup=isoA1

REM Output device name (plot configuration)
set output_device="C:\Program Files\Autodesk\AutoCAD 2022\Plot Styles\DWG To PDF.pc3"

for %%a in (%layoutnames%) do (
    >%plotscript% (
        echo _.-layout s %%~a
        echo _.-plot n
        echo %%~a
        echo %page_setup%
        echo "y"
        echo n
        echo n
        echo n
        echo %%~a.pdf
        echo y
        echo .quit
    )

    "C:\Program Files\Autodesk\AutoCAD 2022\accoreconsole.exe" /i "%dwgfile%" /s %plotscript% /l en-US
    del %plotscript%
)

endlocal
