@ECHO OFF 
SETLOCAL ENABLEDELAYEDEXPANSION

SET count=1
FOR /F "tokens=* USEBACKQ" %%F IN (`reg query HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Provisioning\OMADM\Accounts /v Name /s`) DO (
	SET Name!count!=%%F
	SET /a count=!count!+1
)

SET count=1
FOR /F "tokens=* USEBACKQ" %%F IN (`reg query HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Provisioning\OMADM\Accounts /v ServerId /s`) DO (
	SET ServerId!count!=%%F
	SET /a count=!count!+1
)

SET count=1
FOR /F "tokens=* USEBACKQ" %%F IN (`reg query HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Provisioning\OMADM\Accounts /v Addr /s`) DO (
	SET Addr!count!=%%F
	SET /a count=!count!+1
)

IF "%Name1%"=="End of search: 0 match(es) found." (
	SET Name=
) ELSE (
	FOR /F "tokens=1,2,3,4,5,6" %%a IN ("%Name2%") DO SET Name=%%c %%d %%e %%f
)

IF "%ServerId1%"=="End of search: 0 match(es) found." (
	SET ServerId=
) ELSE (
	FOR /F "tokens=1,2,3,4,5,6" %%a IN ("%ServerId2%") DO SET ServerId=%%c
)

IF "%Addr1%"=="End of search: 0 match(es) found." (
	SET Addr=
	SET Status=Not Enrolled
	SET Exit=2
) ELSE (
	FOR /F "tokens=1,2,3,4,5,6 delims=/ " %%a IN ("%Addr2%") DO SET Addr=%%d
	SET Status=Enrolled
	SET Exit=0
)

SET Summary="Status: !Status!\n\nPlatform: !Name!\nServer ID: !ServerId!\nServer Address: !Addr!"

ECHO {"exit_status":%Exit%,"visible":true,"metadata":[],"Attributes":{},"Expirations":[],"Settings":{},"Summary":%Summary%}