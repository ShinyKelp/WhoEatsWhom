This is a creature relationship customiser! With this mod you can change creature dynamics on a generic level: make lizards afraid of lantern mice, make scavengers hunt deers, or anything else you fancy!

HOW TO USE
1. First setup and folder location
The first thing you want to do is enable the mod and load the game once. The mod will automatically create some folders for you to get started.

Now, find your Rain World installation folder (just google it if you don't know). From there, go to RainWorld_Data > StreamingAssets > WhoEatsWhom.
Inside WhoEatsWhom, you should see two things: a 'Readme.txt', and another folder called 'Relationships'.

2. Info on Readme
The Readme.txt file will contain every defined relationship type (the ones you can see in the wiki), and it will contain every single creature's internal name, including modded creatures for whatever mods you had installed the last time you loaded the game.
You must use EXACTLY these names to create the relationship modifications!

3. Customising relationships
Go into the Relationships folder. Everything outside that folder will be completely ignored. Sub-folders are not supported as of yet either.
In the folder, you must create text files (.txt extension). Any names you want, as many files as you want.

Then, each of the txt files can have as many lines as you want. Each line will define a relationship modification.
RELATIONSHIP FORMATTING
	Creature_A:Creature_B:Relationship_Type:Relationship_Intensity
^ This is the format that each line must follow. The creatures and the relationship type names are self-explanatory. The intensity must be number between 0 and 1, both inclusive. It can be higher than one *technically*, but it's not recommended.

FORMAT EXAMPLE
	GreenLizard:LanternMouse:Eats:0.8
	LanternMouse:GreenLizard:Afraid:1
These two lines represent two relationship modifications: green lizard eats lantern mice, and lantern mice are afraid of green lizards.

OTHER FORMAT DETAILS
You can introduce spaces to make it more readable:
	GreenLizard : LanternMouse : Eats : 0.8
You can add comments with the '//' symbol. Everything after the symbol in a line will be ignored.
	GreenLizard : LanternMouse : Eats : 0.8 //This is a comment.
	//LanternMouse : GreenLizard : Afraid : 1 This line is commented from the beginning, it will be ignored.
	//This line is only a comment.
You can also have completely empty lines to simulate paragraphs and make your files more readable.

4. HANDLING MISTAKES
No matter what you write, the game should never ever crash. However, that doesn't mean every modification you did is working correctly. Every line that has a typo or doesn't follow proper formatting will be ignored and not applied.

In order to verify that everything is correct, load the game AFTER creating and saving all of your relationship files. When the game is loaded, you will see a new text file in the WhoEatsWhom folder called 'output.txt'.

This output file will have every relationship line you've created. It will tag faulty lines with '(INVALID)' at the beginning of the line, and provide the reason for validity at the end of the line.

Once again, having invalid lines does not crash the game and does not impede the implementation of correct lines before or after. Every line is completely independent from the rest.

5. CONFLICTING RELATIONSIHPS
If there are two or more lines that target the same creature pair (note: order matters):
	GreenLizard : LanternMouse : Eats : 0.8
	GreenLizard : LanternMouse : Ignores : 0
Then the mod will only apply one of them, with a defined priority:
-It applies the files in alphabetical order.
-Within each file, applies each line in order from top to bottom.
-The FIRST applied line has priority over the rest.

The output file will display the lines in the exact order that they are applied. When it finds a conflict, it will tag the subsequent lines with a '(WARN)'.

6. FOR MOD DEVELOPERS
As of version 1.1, you can use this mod to add relationship modifications to your own mod!
Simply create a folder called 'WhoEatsWhom' in your mod's root directory (same directory as modinfo.json). Include relationship txt files DIRECTLY in the WhoEatsWhom folder, in the same way you would use the 'Relationships' folder. The output and readme files will remain in the StreamingAssets path, though.

MOD PRIORITY
The output file will also display the relationship modifications of any other mods that use Who Eats Whom. Once again, in the exact order that they are applied.

Relationships defined by mods take priority over relationships defined by the user, so modders can try to enforce their own customization as a part of their mod. The user can work around this if they really want to, though.

If multiple mods have conflicting relationsips, then priority will be defined by mod load order. First one takes priority.

IF YOU USE LIZARD CUSTOMIZER
...Then, all lizards that have modifications enabled in Lizard Customizer will *probably* override Who Eats Whom's relationships (most likely, but not guaranteed). I recommend defining all the lizard relationships with the customizer, and not touching lizards with Who Eats Whom, if you want to use both mods.