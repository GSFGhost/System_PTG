function PTG_GUI_OverviewText()
{
	//Make sure text string size doesn't cause clients to crash!
	
	%text = "<font:arial bold:24>INTRODUCTION<br>" @ 
			"<font:arial bold:16>Thank you for choosing PTG for all of your terrain generation needs! This GUI includes a basic overview of everything included with the add-on.<br><br>" @
			"<font:arial:16>PTG (which stands for the Procedural Terrain Generator) is an add-on that generates brick terrain for you, using mathematical algorithms to make it appear realistic and hand-built." @
			"Besides generating terrain, the add-on can do much, much more, and comes with countless other features and additions to make it as versatile as possible.<br><br>" @ 
			"If you don't want this GUI to appear every time you first open one of the PTG GUIs, then make sure to uncheck the <color:0000ff>Show On First GUI Use<color:000000> option above; you can still access it by setting a keybind under your Blockland Options window. There are <font:arial bold:16>5 pages<font:arial:16> of information included, which should only take a few minutes to read though; to go to the next page, click the green button above.<br><br>" @
			
			"<font:arial bold:24>Help GUI and Video Tutorials<br>" @ 
			"<font:arial:16>If you need more information about a certain option, you can open what's known as the Help GUI, which gives descriptions and important information about the various GUI settings, relative to each category; to open the GUI, just click the question mark icon next to the category name in which the option is under.<br><br>" @ 
			"Video tutorials are also included on YouTube; opening the Help GUI will also provide a link to one of the video tutorials that talks about that option. In addition, a Help.txt file is included in the <color:0000ff>add-ons/System_PTG.zip<color:000000> directory, which explains in-depth certain features which are not covered in the Help GUI.<br><br>" @
			"To view the entire video tutorial playlist on YouTube, click the link provided: <linkColor:0000ff><a:forum.blockland.us/index.php?topic=281995.0>(temporary link)</a>";
	PTG_Overview_TxtMain.setText(%text);
	
	
	%text = "<font:arial bold:24>Chunk-Based Generation<br>" @ 
			"<font:arial:16>PTG creates terrain and landscapes in general by generating bricks within objects known as <color:0000ff>Chunks<color:000000>. Normally, bricks are calculated based on the size of the chunk, then are planted based on those calculations. Chunks can also be saved and loaded from file, allowing bricks to generate faster for that area, or even to save player builds throughout the landscape. Chunks can be saved manually, or automatically via the <color:0000ff>AutoSaving<color:000000> feature. Using the Main GUI, you can specify when and how chunks are saved, or to disable the option altogether; these files are saved in the <color:0000ff>config/server/PTGv3/ChunkCache/<color:000000> directory.";
	PTG_Overview_TxtA.setText(%text);		
			
			
	%text = "<font:arial bold:16>Blockland's .bls Saving vs. PTG's Chunk Saving<font:arial:16> - Landscapes can be saved in two different ways. One way is to use Blockland's default saving method; PTG bricks generate as normal bricks and don't break the game's trust system, so they fully support being saved in .bls format. However bricks loaded from Blockland's brick saves won't be recognized by the generator, so most of the GUI settings won't apply to them. Another way is to save a new GUI preset with the current settings used, especially with the start and end positions of the landscape defined. If any chunks are modified (such as if players build on the terrain), those chunks can also be saved and loaded from file (using a custom chunk save format) when the landscape is regenerated from that preset.<br><br>" @
			"<font:arial bold:16>Normal vs. Permanent Chunk Saves<font:arial:16> - Chunks are saved in a directory that's based on the current seed and chunk size values sent to the generator. Chunks are also saved in a <color:0000ff>Normal_Saves<color:000000> folder; files within this folder are handled normally. Another folder created is entitled <color:0000ff>Permanent_Saves<color:000000>; if you move files from the Normal_Saves folder to this one, they will never be removed by the generator under any circumstances (useful if you want to have permanent builds in the landscape that you don't accidentally clear with the chunk clearing commands). Permanent saves can still be saved to by the generator if the chunk is modified later.<br><br>" @
			"<font:arial bold:16>Static Chunks<font:arial:16> - Chunk objects (and their bricks) are removed by the generator under certain conditions. However, you can choose to tag them as <color:0000ff>Static<color:000000>, which will prevent them from being removed under those conditions. This is useful encase you want to set up areas where players, vehicles or bots spawn. Chunks can be toggled as being static or non-static using the <color:0000ff>/PTGStatic<color:000000> command, or using the <color:0000ff>Chunk Manager GUI<color:000000>.<br><br>" @
			"<font:arial bold:16>Highlighting Chunks<font:arial:16> - If you need a visual representation of what chunks look like or how large each chunk is, you can use the <color:0000ff>/PTGReveal<color:000000> command to highlight them, which will add static shape objects to outline all chunks that currently exist; different colors are used for both static and non-static chunks.<br><br>" @
			"<font:arial bold:16>Uploading Settings<font:arial:16> - PTG was designed from the very beginning to support dedicated servers, and because of this, settings in your GUI have to be uploaded to the server for them to take effect, and for the generator to know what to generate. You can choose to either upload new settings to the server, or to start the generator based on previous settings that were uploaded before. Also, when you first spawn in your server, default server settings for the generator are automatically set up for you.<br><br>" @
			"<font:arial bold:16>Routines<font:arial:16> - A generator routine is a process in which chunks of bricks are either generated or removed; only one main routine can run at a time.";
	PTG_Overview_TxtB.setText(%text);
	
	
	%text = "<font:arial bold:24>Infinite Terrain<br>" @ 
			"<font:arial:16>A new addition included with version 3 of PTG is the infinite terrain option, which works by generating chunks of the landscape around all player on your server, rather than using a predefined area you specify. Besides generating or loading chunks around players, chunks are also removed if they are a certain distance away from all players, which helps to keep the brickcount down and allows for truly infinite terrain. The generator will also automatically adapt as players join and leave your server.";
	PTG_Overview_TxtC.setText(%text);
	
	
	%text = "<font:arial bold:24>Terrain Options<br>" @ 
			"<font:arial:16>Various options are included for generating different kinds of landscapes, for both finite and infinite terrain. One option is the ability to choose 4 different types of terrain to generate: <color:0000ff>Normal Terrain<color:000000>, <color:0000ff>Skylands<color:000000>, <color:0000ff>Flatlands<color:000000> and <color:0000ff>Load Chunk Saves Only<color:000000>. You can also toggle options such as Edge Falloff (which forces the edges of terrain to snap to ground-level) and Radial Grid (which generates a circular area of terrain).";
	PTG_Overview_TxtD.setText(%text);
	
	
	%text = "<font:arial bold:16>Various Brick Sizes<font:arial:16> - Another new addition in version 3 is the ability to choose different brick sizes and types for the landscape; anything from the 1x1f plate to the 32x32 pillar can be used. Custom bricks have also been included with the add-on and can be used. All bricks will automatically snap to the height scale of the brick itself that is being generated.<br><br>" @
			"<font:arial bold:16>Modular terrain support<font:arial:16> makes a return from the previous versions and has been greatly improved. Not only do ModTer bricks generate faster and more efficiently than before, but there are also different ModTer types you can generate: <color:0000ff>Cubes & Ramps<color:000000> generates Modular Terrain as it normally appears - with cube and ramp / sloped bricks, <color:0000ff>Cubes & Wedges<color:000000> will use the wedge bricks instead of ramps and <color:0000ff>Cubes<color:000000> will only use cube ModTer bricks.";
	PTG_Overview_TxtE.setText(%text);
	
	
	%text = "<font:arial bold:24>Toggleable Landscape Features and Additions";
	PTG_Overview_TxtF.setText(%text);
	
	
	%text = "<font:arial bold:16>Biomes<font:arial:16> - Biomes also make a return in this version, which are areas of terrain that are unique to the other areas around them, and can consist of their own terrain colors, prints, water types and other attributes. The biomes include are the <color:0000ff>Default Biome<color:000000>, <color:0000ff>Shore<color:000000>, <color:0000ff>Submarine<color:000000> (underwater) and three <color:0000ff>Custom<color:000000> biomes that can be toggled for default terrain. <color:0000ff>Caves<color:000000> and <color:0000ff>Mountains<color:000000> are also treated as biomes (due to how they work) and have their own unique options.<br><br>" @
			"<font:arial bold:16>Biome detail bricks<font:arial:16> are another attribute that can be set up relative to each biome. Details are bricks that generate within certain biomes they're assigned to, which can be used to add things like trees, plants, and flowers to that biome; any bricks can be used, and will be randomly rotated once planted.<br><br>" @
			"<font:arial bold:16>Additions<font:arial:16> - You can toggle the generation of Lakes, Mountains, Caves, Clouds, Floating Islands and Boundaries throughout the landscape, which each come with their own customizable options. Also, if ModTer is used for default terrain, water bricks used for Lakes will switch to print water, which supports planting both normal and Modular Terrain bricks inside. Remember that the more features you enable will lead to more calculations that need to be performed, and could lead to an increase in server lag.";
	PTG_Overview_TxtG.setText(%text);
	
	
	%text = "<font:arial bold:24>Build Loading and Flat Areas Generation";
	PTG_Overview_TxtH.setText(%text);
	
	
	%text = "<font:arial bold:16>Loaded Builds<font:arial:16> - The Main GUI allows you to upload your own .bls saves to be randomly loaded into the landscape by the generator. Once uploaded, you can choose where your builds generate, how often, and if they will be randomly rotated when generated or not. You can also toggle the option to allow other players to upload and manage builds. Uploaded builds are converted to a custom format, and are saved in the <color:0000ff>config/server/PTGv3/BrSaveCache/<color:000000> directory.<br><br>" @
			"<font:arial bold:16>Flat Areas<font:arial:16> - You can also choose to generate <color:0000ff>Flat Areas<color:000000>, which are areas of terrain or floating islands that are flattened, within a certain grid size. Flat areas can make it easier for players to build on the terrain; areas of terrain are flattened anyway when uploaded player builds are loaded onto the landscape.";
	PTG_Overview_TxtI.setText(%text);
	
	
	%text = "<font:arial bold:24>Server Commands<br>" @ 
			"<font:arial:16>Various server or slash commands are included, which allow you to manage generator routines (such as generating or clearing terrain), display the number of chunks present, save chunk objects and more. For a complete list of all commands, enter the <color:0000ff>/PTGCmds<color:000000> command in chat; various buttons throughout the GUIs execute these commands for you.";
	PTG_Overview_TxtJ.setText(%text);
	
	
	%text = "<font:arial bold:24>Multi-User Support<br>" @ 
			"<font:arial:16>Support is included that will allow other players to do things like manager generator routines using the server commands, upload their GUI settings and builds, and to use PTG events (included as a separate download). However, you can choose to toggle permissions for these options; they are disabled for other players by default.";
	PTG_Overview_TxtK.setText(%text);
	
	
	%text = "<font:arial bold:24>Complex, Simplex and Chunk Manager GUIs<br>" @ 
			"<font:arial:16>Various GUIs are included, one of which being the Complex or Main GUI. This interface will allow you to set up how your landscape will generate, and contains all options for doing so. The Simplex GUI is similar, except is designed to be much easier to use by leaving out all the in-depth settings included with the Main GUI; when you're first starting out, it's recommended to use this GUI first. The Chunk Manger allows you to manage a single or all chunks objects that exist on the server. Both the Complex and Simplex GUIs also have buttons included which allow you to start generation, halt routines and clear chunks.";
	PTG_Overview_TxtL.setText(%text);
	
	
	%text = "<font:arial bold:24>GUI Presets and Sharing options<br>" @ 
			"<font:arial:16>You can choose to save current or load previous GUI settings to save file known as <color:0000ff>Presets<color:000000>. Presets now only allow you to load previous landscapes that you've generated, but also make it easy to share landscapes with others. Including chunk save files and uploaded build files with shared presets can also allow for more sharing options. Presets are saved in the <color:0000ff>config/client/PTGv3/Presets<color:000000> file directory.";
	PTG_Overview_TxtM.setText(%text);
	
	
	%text = "<font:arial bold:24>Cascading Dynamic Streams<br>" @ 
			"<font:arial:16>Most of the custom bricks included with the add-on are mainly meant to be used for certain functions of the generator. However, some of those bricks are known as stream source bricks, which when planted by a player, will cause a cascading stream to flow from that source. Settings for streams are included for the Main GUI, and permissions can be toggled to allow administrators to plant them as well. However, by default only the host can plant them.";
	PTG_Overview_TxtN.setText(%text);
	
	
	%text = "<font:arial bold:24>Technical Features";
	PTG_Overview_TxtO.setText(%text);
	
	
	%text = "<font:arial bold:16>Colorset Adaption<font:arial:16> - The add-on automatically takes changes between colorsets into account in different situations, and will automatically use the closest colors possible with the current colorset that is being used.<br><br>" @
			"<font:arial bold:16>Floating Brick Support<font:arial:16> - The generator supports building and removing player bricks on floating terrain and other generated bricks, and prevents chained-destruction when a single brick is removed, even though it's technically floating. Other options are included so you can choose when and how certain bricks are removed.<br><br>" @
			"<font:arial bold:16>Sylvanor Tree Brick Support<font:arial:16> - Choosing one of the tree top bricks for Sylvanor's trees will automatically, randomly plant one of the tree base bricks underneath for you.<br><br>" @
			"<font:arial bold:16>Smart Gap-Fill Algorithms<font:arial:16> - Smart fill algorithms are included that will minimize the number of bricks required (such as for filling in terrain gaps and for creating large columns of water) by using larger bricks where possible. Stream bricks will also automatically merge into large cubes and pillars when flowing to help keep the brickcount down.<br><br>" @
			"<font:arial bold:16>Seamless Terrain<font:arial:16> - When generating bricks, the height of adjacent bricks are also taken in to account, which allows terrain and the landscape in generate to be perfectly seamless.<br><br>" @
			"<font:arial bold:16>Dedicated Server and Remote Console Support<font:arial:16> - Everything was designed from the ground up to work on dedicated servers, including the GUIs. Support was also added for easily calling PTG functions through the remote console window when you're not on your server. Type <color:0000ff>PTGRmt(\"Help\");<color:000000> in the console for more information, and <color:0000ff>PTGRmt(\"List\");<color:000000> for a list of all available functions.<br><br>" @
			"<font:arial bold:16>Delays, Schedules and Lag Prevention<font:arial:16> - Customizable delays and built-in schedule delays are included to ensure the generator runs as smoothly as possible. The generator will also check if the server is lagging, and will automatically pause or halt a routine that's running if necessary - based on your settings.<br><br>" @
			"<font:arial bold:16>Public Brick Support<font:arial:16> - An option is included that will allow generating bricks under public ownership. You can also choose to allow or prevent players from destroying public detail bricks (encase they want to clear a spot to build).<br><br>" @
			"<font:arial bold:16>Gamemode / Third Party Support<font:arial:16> - A function is included which will allow you to load settings from a server-sided preset from within a gamemode file. This function can also be set up to immediately start a routine afterwards, to reset the minigame when settings are finished loading, to halt a routine that's running, to clear chunks of terrain that currently and to disabled fall damage for all players while terrain is being cleared. For more information, check out the Help.txt file located in your <color:0000ff>Add-Ons/System_PTG.zip<color:000000>.<br><br>" @
			"<font:arial bold:16>Events<font:arial:16> - Events are included as a separate download, which allow you (or other players who have permission) to manage routines, load server-sided presets, etc.<br><br>" @
			"<font:arial bold:16>Server Preference Variables<font:arial:16> - Certain server preference variables are included as well that will allow you to disable load default server settings (not recommended), disable advertising PTG when a player first joins your server and to enable greater upload security if needed. For more information, check out the Help.txt file located in your <color:0000ff>Add-Ons/System_PTG.zip<color:000000>.<br><br>" @
			"<font:arial bold:16>Built-in Hard Limits, Customizable Limits, Buffers and Security<font:arial:16> - Hard limits, customizable limits included in the Main GUI and buffer ranges help protect the server from crashing. Security is also included to prevent malicious code form being uploaded from clients.<br><br>" @
			"<font:arial bold:16>Mass Detail Listing<font:arial:16> - Normally, the Main GUI allows you to set up up to 18 detail bricks per biome. If you need to bypass this limit and include even more details for biomes, a mass detail listing option is included. This option has its own GUI which will allow you to set up up to 400 details per biome instead.<br><br>" @
			"<font:arial bold:16>Pseudo-Equator Simulation<font:arial:16> - You can choose to delay certain features from generating until it's a certain distance from the origin on the positive or negative Y-Axis. This is known as Pseudo Equator Simulation, because it can be used with biomes to simulate the earth's equator (such as if you set up an arctic biome and wanted it to generate far away from the origin). This can also be used to reward exploration.<br><br>" @
			"<font:arial bold:16>Advanced Options<font:arial:16> - You can go in-depth with the generators noise algorithms, and modify settings such as noise offsets and scales, section cut heights and more.<br><br>" @
			"<font:arial bold:16>Custom Defaults<font:arial:16> - When you first spawn in your server, default generator settings are already set up for you for both the server and the GUIs. However, you can choose to have the generator use your current GUI settings as the new, standard server and GUI defaults by entering the <color:0000ff>/PTGSetDefault<color:000000> chat command; covered in more detail in the Help.txt file located in <color:0000ff>Add-Ons/System_PTG.zip<color:000000>.<br><br>" @
			"<font:arial bold:16>Chat and Console Messages<font:arial:16> - Messages are sent to all players in chat and echoed in the server console in many different situations, such as when routines are started or halted, when chunks are cleared, etc. You can choose to disable the echo messages if desired (although some will still apear), as well as adjust the font size for PTG chat messages.<br><br>" @
			"<font:arial bold:16>Custom Noise Algorithms<font:arial:16> - Various coherent and incoherent noise algorithms (such as <color:0000ff>Perlin Noise<color:000000>, <color:0000ff>Multi-Iteration Fractals<color:000000>, <color:0000ff>random number generators<color:000000> and <color:0000ff>random zone subdivision<color:000000>) are included, which are custom made for the generator and are designed to be as fast and efficient as possible. A custom  <color:0000ff>Marching Cubes<color:000000> algorithm is also included, which is used for planting ModTer bricks.";
	PTG_Overview_TxtP.setText(%text);
}