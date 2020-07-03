HOW... DID... THIS... HAPPEN??? Gene Kerman fumed... "We were so close to comet Linux-GG... SO CLOSE... The whole planet tuned in and watching, and... and the batteries died???"
"It was not our fault!!!" Bob Kerman complained. "The Probe turned sideways, and the solar panels weren't alligned properly."
"Oh... so you're saying it's the Engineering team again?" Bill Kerman shot back. "Typical..."
"Oh, would you two just stop!" Val shouted. "Haven't we had enough without you starting again?"
For several minutes the bickering went on, until a somewhat meek voice spoke up from the back of the conference room.
"Sir... Ma'am, please... if I may???" young Margo Kerman interjected. "Perhaps... perhaps... if we had some sort of automated resourse monitor?"
"A... what?" Gene started to ask, but was cut off by Dr von Kerman, who had stayed mostly out of it until now.
"Yeees... yeees..." von Kerman nodded. "A device to automatically monitor the resourses, and give us a warning if something is amiss... yeees... this could work"
Gene looked over at young Margo and smiled. She had only been onthe staff a few days, and already she was making a good impression.
"OK, Margo... you have ten days before our next scheduled launch." Gene nodded. "Make it so."

=====


This mod will allow you to set monitors on any resource in any/all vessels in the game.  An alarm will sound if the level is passed, any time warping will be safely stopped and you will be transported to the vessel in question.

Optionally this will interface with the Haystack Continued mod, if it is installed.

The following windows are used to set up and configure the alerts.

Resource Monitor
	There are two variants of this window, the Common Resource Monitors, used on the SpaceCenter screen, and the Vessel Resource Monitors, used in the Flight scene.
	The Common Resource Monitors are a set of monitors that you can set up, to either be applied automatically to all new vessels, or applied manually when first setting up a vessel which doesn't have any monitors set up.
	The Vessel Resource Monitors window is used to configure monitors for the currently active vessel.
	The only difference between these windows is in the top part.  
	The Vessel Resource Monitor can interface with the Haystack mod, to allow you to find a vessel and then to get a vessel from Haystack.

	
	Both windows have a toggle to only allow common resources for new monitors.  Common resources are defined in a config file, and can be modified by a player at any time, and the changes will be available the next time the game starts.  The file is called Defaults.cfg, and stores two different types of information.
	The first is the list of common resources, the default common resources are:
			ElectricCharge
			LiquidFuel
			Oxidizer
			MonoPropellant
			XenonGas

	The second is a set of default monitors.  These monitor definitions are normally only used the first time a game is loaded, after a game is loaded those definitions are ignored.  However, there is an option on the Settings page to reload them.

Monitors will only work on vessels that have a ModuleCommand (ie: probe cores, manned, etc);