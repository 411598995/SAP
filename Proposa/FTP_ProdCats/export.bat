@echo off
for /f "delims=" %%a in ('wmic OS Get localdatetime  ^| find "."') do set dt=%%a
set datestamp=%dt:~0,14%
sqlcmd -S ubaid-pc -d sbodemoau -E -s, -W -i Qry.sql | findstr /V /C:"-" /B > prodcats_%datestamp%.csv
echo open ftp://inishpharmacyftpuser:CC554^^^^1@54.77.137.93/> upload.txt
echo cd inbox>> upload.txt
echo put -transfer=ascii prodcats_%datestamp%.csv>> upload.txt
echo bye>>upload.txt

winscp.com /script=upload.txt /ini=nul


del upload.txt
del prodcats_%datestamp%.csv
