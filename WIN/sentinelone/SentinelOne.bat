@ECHO OFF 
SETLOCAL ENABLEDELAYEDEXPANSION
CD C:\Program Files\SentinelOne\Sentinel Agent*\

IF EXIST SentinelCtl.exe (
	SETLOCAL ENABLEDELAYEDEXPANSION
	CD C:\Program Files\SentinelOne\Sentinel Agent*\

	SET count=1
  	FOR /F "tokens=* USEBACKQ" %%F IN (`sentinelctl status`) DO (
    		SET status!count!=%%F
    		SET /a count=!count!+1
 	)

 	FOR /F "tokens=* USEBACKQ" %%F IN (`sentinelctl agent_id`) DO (
   		SET uuid=%%F
  	)
	SET summary="!status4!\n!status1!\n!status3!\n!status7!\nUUID: !uuid!"
	SET exitcode=20
) ELSE (
  	SET exitcode=2
  	SET summary="SentinelOne agent not found"
)
IF "%status1%"=="Disable State: Not disabled by the user" (IF "%status3%"=="Self-Protection status: On" (SET exitcode=0))

ECHO {"exit_status":%exitcode%,"visible":true,"metadata":[],"Attributes":{},"Expirations":[],"Settings":{},"Summary":%summary%}