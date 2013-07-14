#
# Regular cron jobs for the pidgeon package
#
0 4	* * *	root	[ -x /usr/bin/pidgeon_maintenance ] && /usr/bin/pidgeon_maintenance
