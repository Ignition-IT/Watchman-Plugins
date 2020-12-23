#!/bin/bash
​
# show current dns servers for tattling
scutil --dns | grep nameserver\[[0-9]*\]
​
# if dns is not set to home, check to see if machine is set to umbrella dns servers - if not change settings to umbrella dns servers
​
scutil --dns | grep nameserver\[[0-9]*\] | grep 127.0.0.1
if [ $? -ne 0 ]; then
	scutil --dns | grep nameserver\[[0-9]*\] | grep 208.67.222
	if [ $? -ne 0 ]; then
		echo "you've been naughty!  updating your dns settings"
		networksetup -setdnsservers Wi-Fi 208.67.222.222 208.67.220.220
	else
		echo "all is well"
	fi
else
	echo "you're already home"
fi
​
exit 0