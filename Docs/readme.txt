Tigereye's Allegiance Gamelogger
================================

Uber-insaneo-extra special thanks to Pook for working with me to
make this possible... and for putting up with my daily PMs :)

Special thanks to Noir and Aarmstrong for being my guinea pigs

Thanks to the community for keeping the game alive :)


What is TAG?
------------
Tigereye's Allegiance Gizmo (TAG) is a replacement for AGMNet,
providing logging services for Allegiance games.
GameServer Administrators can run TAG on their systems next to their
Allegiance Gameservers to collect ingame events and upload them
to ASGS (provided their server has been "cleared" to post statistics).

How do I use TAG?
-----------------
Usage is very simple and straight forward. Create a folder for TAG.
I recommend placing TAG in a subfolder with the Allegiance server,
but it can go anywhere you like.

To run it, simply doubleclick on TAG.exe to run TAG in console mode.

What does TAG do when started?
-------------------------------
- 1st, TAG checks for updates, and will install the updates if necessary.
- 2nd, TAG will load the configuration file. TAG only reads the file once.
- 3rd, TAG will ensure Allsrv is running.
	If Allsrv isn't running, TAG will start the Allsrv service, or execute
	Allsrv in console mode as configured
- 4th, TAG will connect to Allsrv, and start listening to games.

How do I run TAG as a service?
------------------------------
You can install and uninstall the TAG service with commandline parameters.
	"-service" will install TAG as a service if not already installed
	"-noservice" will uninstall the TAG service if it exists.

The TAG service must be executed under credentials which have access to
your Allegiance Server installation.
TAG must also have access to any folders specified in the config file
for tracing, debugging or local-logging purposes.
I recommend launching the TAG Service with the "Local System" account.

What if I shut Allsrv down while TAG is running?
------------------------------------------------
TAG has a reconnect timer that attempts to re-attach to Allsrv.
See the config file template for more details.

Are there any special commands I can send to TAG?
-------------------------------------------------
Yes. To send a command to TAG, log into a game hosted by your server
and send a PM to "Admin". The following commands are valid:

#version		TAG replies with the current version number
#update			Instructs TAG to check for updates, and apply them
				as necessary
#debugkills		TAG will report all logged kills via HQ(all) chat for
				debugging purposes
#hq [text]		Speaks the [text] as HQ to all games on current server
				if the speaker is wearing their alleg or admin token/tag
#hqgame [text]	Speaks the [text] as HQ to the current game If the speaker
				is wearing their alleg or admin token/tag

What is Tracing?
----------------
As TAG does its work, it can inform you of what it does in realtime in a console window. This can also be logged to a file for future inspection.
Any errors encountered during TAG's work are traced to the screen or logfile as well which give valuable info when trying to isolate a problem.

How does TAG update itself?
---------------------------
Yes. TAG attempts to update after each game. If there are any other games in progress as another game ended, the update is aborted. If there are no games in progress as a game ends, TAG checks for updates.
In case no games were played throughout the day, TAG can be configured with a time at which to check for updates.

What options are available in the config file?
----------------------------------------------
See the TAGConfigTemplate.xml for more information.
For optional attributes, the setting specified in the Template.xml are
the defaults used by TAG if not overridden by your setting.
