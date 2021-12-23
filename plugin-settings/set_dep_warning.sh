#!/bin/sh

#modify plugin settings
#written by Ella Hansen for Ignition www.ignitionit.com

/usr/libexec/PlistBuddy -c "Set :DEP_Warning 20" /Library/MonitoringClient/PluginSupport/_mdm_settings.plist