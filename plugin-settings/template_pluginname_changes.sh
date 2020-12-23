#!/bin/sh

#modify plugin settings
#written by Ella Hansen for Ignition www.ignitionit.com

sudo defaults write /Library/MonitoringClient/PluginSupport/check_crashplan_client_settings.plist Completed_Days_Until_Warn -int 3