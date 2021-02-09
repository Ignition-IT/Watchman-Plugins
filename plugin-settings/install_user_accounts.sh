#!/bin/sh

#install plugins
#written by Ella Hansen for Ignition www.ignitionit.com

cd /Library/MonitoringClient/Plugins && curl https://raw.githubusercontent.com/Ignition-IT/Watchman-Plugins/master/user-accounts/_user_accounts.plugin -O -L && curl https://raw.githubusercontent.com/Ignition-IT/Watchman-Plugins/master/user-accounts/_user_accounts.plist -O -L && chown root /Library/MonitoringClient/Plugins/_*.p* && chmod 755 /Library/MonitoringClient/Plugins/_*.p*