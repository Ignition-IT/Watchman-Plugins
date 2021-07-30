# Watchman-Plugins

A collection of [custom plugins for Watchman Monitoring](https://github.com/watchmanmonitoring/customplugins)

---

## Activation Lock

Activation Lock can be a major block for managed fleets of Macs. This plugin allows you to see the status of both Activation Lock and Find My Mac. It also reports the current user's iCloud account details, and whether or not the iCloud account is managed or personal.

### All Clear

No iCloud account is signed in and Find My Mac and Activation Lock are disabled:
![](https://github.com/Ignition-IT/Watchman-Plugins/blob/master/images/activation_lock_no_icloud_ok.png?raw=true)

An unmanaged iCloud account is signed in, but Find My Mac and Activation Lock are disabled:
![](https://github.com/Ignition-IT/Watchman-Plugins/blob/master/images/activation_lock_ok.png?raw=true)

### Informational

Activation Lock is disabled, but Find My Mac is enabled and unmanaged iCloud account is signed in:
![](https://github.com/Ignition-IT/Watchman-Plugins/blob/master/images/activation_lock_fmm_info.png?raw=true)

Pre-T2 chip Mac, or pre-Catalina macOS that doesn't support Activation Lock, 
but Find My Mac is enabled and unmanaged iCloud account is signed in:
![](https://github.com/Ignition-IT/Watchman-Plugins/blob/master/images/activation_lock_pre_t2_info.png?raw=true)

### Warning

Activation Lock is enabled and an unmanaged iCloud account is signed in:
![](https://github.com/Ignition-IT/Watchman-Plugins/blob/master/images/activation_lock_enabled_warning.png?raw=true)


## macOS Updates

This plugin aims to replicate the functionality of the Windows Update plugin, but for macOS. It reports the status of available updates for the Macs in your fleet, showing the names up available updates or an All Clear status if the computer is up to date.

### All Clear

macOS is up to date (within the major release installed):
![](https://github.com/Ignition-IT/Watchman-Plugins/blob/master/images/macos_updates_ok.png?raw=true)

### Informational

An error occurred while checking for updates:
![](https://github.com/Ignition-IT/Watchman-Plugins/blob/master/images/macos_updates_info.png?raw=true)

### Warning

There are updates available:
![](https://github.com/Ignition-IT/Watchman-Plugins/blob/master/images/macos_updates_warning.png?raw=true)


## macOS User Accounts

This plugin aims to replicate the functionality of the User Accounts plugin for Windows. It reports all of the user accounts on macOS, including account type and SecureToken status. This plugin always reports All Clear.

### All Clear

A list of all user accounts on the computer:
![](https://github.com/Ignition-IT/Watchman-Plugins/blob/master/images/macos_user_accounts.png?raw=true)


## SentinelOne

This plugin shows the status of the SentinelOne agent installed on an endpoint. There are versions for both macOS and Windows. It reports the version, ready status, protection status, infection status, and UUID of the endpoint.

### All Clear

SentinelOne is ready and enabled:
![](https://github.com/Ignition-IT/Watchman-Plugins/blob/master/images/sentinelone_ok.png?raw=true)

### Informational

SentinelOne is either not ready, not enabled, or not installed:
![](https://github.com/Ignition-IT/Watchman-Plugins/blob/master/images/sentinelone_info.png?raw=true)

### Warning

The endpoint is reporting an infection


## Umbrella DNS

This plugin reports the status of the Cisco Umbrella DNS agent installed on macOS. It reports the enabled status, VPN status, last enabled date, Org ID, and Device ID. The first time the plugin is run, it will create a settings file that contains the grace period setting, which can be customized later via editing the file or sending a terminal command (see the Watchman documentation for remotely updating plugin settings). The plugin will report a warning if Umbrella has been disabled for longer than the specified grace period (the default is 24 hours).

### All Clear

Umbrella is enabled:
![](https://github.com/Ignition-IT/Watchman-Plugins/blob/master/images/umbrella_dns_ok.png?raw=true)

### Informational

Umbrella status is unknown:
![](https://github.com/Ignition-IT/Watchman-Plugins/blob/master/images/umbrella_dns_info.png?raw=true)

### Warning

Umbrella has been disabled for longer than the specified grace period:
![](https://github.com/Ignition-IT/Watchman-Plugins/blob/master/images/umbrella_dns_warning.png?raw=true)


---

Created by Ella Hansen for Ignition, Inc., a California corporation https://www.ignitionit.com
