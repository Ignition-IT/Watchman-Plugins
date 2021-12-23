#!/bin/sh

#modify plugin settings
#written by Ella Hansen for Ignition www.ignitionit.com

/usr/libexec/PlistBuddy -c "Set :MDM_Warning 2" /Library/MonitoringClient/PluginSupport/_mdm_settings.plist