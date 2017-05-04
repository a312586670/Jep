copy .\*.dll %windir%\system32\
copy .\*.ocx %windir%\system32\
copy .\License.dat C:\
regsvr32 %windir%\system32\MSCOMM32.OCX
regsvr32 %windir%\system32\pskEx.ocx
