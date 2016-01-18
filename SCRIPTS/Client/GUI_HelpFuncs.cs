function PTG_GUI_HelpList(%cat)
{
	switch$(%cat)
	{
		////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////
		//Miscellaneous
		
		case "ComplexGUI":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Complex GUI Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Complex GUI\" segment of the \"Introduction\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "gotoWebPage(\"http://blockland.us/Store.html\");";
			
			%text =	"The Complex GUI contains in-depth options to give you complete control over the generator; if you're just starting out using PTG, it's recommended to try using the Simplex GUI first, which is less overwhelming in terms of the options provided. <br><br>" @
					"<font:arial bold:16>Note:<font:arial:14> This GUI is the only one that can be expanded if necessary - two expansion options have been included. You can either click and drag the edge of the window, or use the white diagonal arrow, located on the top title bar of the window.<br><br>" @ 
					"The diagonal arrow button will run through one of the three different window scale variants when clicked. Also, when using either the diagonal arrow button or when dragging the edge of the GUI window, certain GUI objects automatically adjust to accommodate the change in size.<br><br>";

			PTG_Help_TxtCatSetList.setText(%text);
			
		//////////////////////////////////////////////////
		
		case "Navigation":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Complex GUI: Navigation Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Navigation Window\" segment of the \"Introduction\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "gotoWebPage(\"http://blockland.us/Store.html\");";
			
			%text =	"The mini navigation window allows you to cycle through various windows of the Main GUI, each of which contain different options for the generator." SPC
					"The navigation buttons are aligned from the most basic settings, to the most advanced - in descending order.<br><br>" @
					
					"<font:arial bold:16>Preview<font:arial:14> - Opens up the preview window, which allows you to see how the landscape will generate before actually generating it.<br><br>" @
					"<font:arial bold:16>Setup<font:arial:14> - The most basic and fundamental options for the generator - contains options for the seed, terrain brick size, finite / infinite terrain grid sizes, and more (you can generate completely different landscapes by changing these settings alone).<br><br>" @
					"<font:arial bold:16>Features<font:arial:14> - Additional features to be added to the landscape, such as mountains, clouds, floating islands and boundaries.<br><br>" @
					"Note: Use caution when enabling multiple features at the same time; the more features that are enabled requires more calculations to be performed.<br><br>" @
					"<font:arial bold:16>Biomes<font:arial:14> - Options that allow you to set up areas of terrain which are unique to terrain around it; you can set up biomes such as artic taigas, forests, swamps, etc.<br><br>" @
					"<font:arial bold:16>Builds<font:arial:14> - This window allows you to upload and generate your own brick saves randomly into the landscape.<br><br>" @
					"<font:arial bold:16>Presets<font:arial:14> - Allows you to save current and load past GUI settings.<br><br>" @
					"<font:arial bold:16>Advanced<font:arial:14> - Options that allow you to go in-depth for various features, such as managing noise algorithm offsets and scales, setting section cut heights, changing pseudo-equator start positions and other settings.<br><br>" @
					"<font:arial bold:16>Routines<font:arial:14> - Contains options relating to the generator's inner workings, regarding options such as schedules and delays, server limits, chunk options and more.<br><br>";
			
			PTG_Help_TxtCatSetList.setText(%text);
			
		//////////////////////////////////////////////////
		
		case "MiniCtrl":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Complex GUI: Control Window Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Mini Control Window\" segment of the \"Introduction\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"Contains commands that allow you to control routines for the generator, i.e. starting, halting, clearing, saving, etc.<br><br>" @
					
					"<font:arial bold:16>Apply & Start<font:arial:14> - Uploads your current GUI settings and applies them to the server, and immediately starts a finite or infinite routine afterwards. Note: if you're the host, all settings, even for Routines, will be uploaded. Otherwise, Routines settings won't be uploaded.<br><br>" @
					"<font:arial bold:16>Resume<font:arial:14> - Starts a finite or infinite generation routine, without uploading settings - previously uploaded settings are used instead; same as using \"/PTGStart\" in chat.<br><br>" @ 
					"<font:arial bold:16>Halt<font:arial:14> - Stops a finite / infinite generation routine, or a clearing routine if chunks are being cleared; same as using \"/PTGHalt\" in chat.<br><br>" @ 
					"<font:arial bold:16>Clear<font:arial:14> - Clears all existing, non-static chunks, and doesn't save chunks no matter your settings for saving; same as using \"/PTGClear 1\" in chat.<br><br>" @ 
					"<font:arial bold:16>more >><font:arial:14> - Expands the miniature control window to show even more commands you can use.<br><br>" @ 
					"<font:arial bold:16>Apply<font:arial:14> - Uploads your current GUI settings and applies them to the server, but doesn't start a routine afterwards (this option appears when the control window is expanded). Note: You can upload and apply settings using this command even while a routine is running, or while chunks are being cleared.<br><br>" @ 
					"<font:arial bold:16>Clear Spam<font:arial:14> - Clears bricks from all existing chunks, if they were planted by a player with the Blockland ID that matches the one you specify; same as using \"/PTGClearSpawn %ID\" in chat (make sure to replace %ID with the players Blockland ID).<br><br>" @ 
					"<font:arial bold:16>Clear<font:arial:14> - (This button moves to a new position once the control window is expanded, and is replaced by the Apply button. Once the control window is retracted, the Clear button returns to its initial position.)<br><br>" @ 
					"<font:arial bold:16>Clear & Save<font:arial:14> - Clears all existing, non-static chunks, and saves chunks depending on your settings for saving; same as using \"/PTGClear 0\" in chat.<br><br>" @ 
					"<font:arial bold:16>ClearAll<font:arial:14> - Clears all existing chunks, even if static, and doesn't save chunks no matter your settings for saving; same as using \"/PTGClearAll 1\" in chat.<br><br>" @ 
					"<font:arial bold:16>ClearAll & Save<font:arial:14> - Clears all existing chunks, even if static, and saves chunks depending on your settings for saving; same as using \"/PTGClearAll 0\" in chat.<br><br>" @ 
					"<font:arial bold:16>Purge<font:arial:14> - Clears all existing chunks, even if static, and removes any chunk saves for them if present; same as using \"/PTGPurge\" in chat.<br><br>" @ 
					"<font:arial bold:16>Clear Saves<font:arial:14> - Clears all chunk saves for this particular seed and chunk size; same as using \"/PTGClearAllSaves\" in chat (if you want to clear all chunk saves for all seed values, you can enter \"/PTGClearAllSaves AllSaves\" in chat, instead).<br><br>" @ 
					"<font:arial bold:16>Save<font:arial:14> - Saves all existing chunks, depending on your settings for saving; same as using \"/PTGSave\" in chat. Chunks are not automatically saved when you close or exit the server, so make sure to save before shutting down - if you don't want to lose what was generated.<br><br>" @ 
					"Note: By default, chunks are not saved by the generator unless they've been modified. If you want to save all chunks whether they've been modified or not, try setting the \"Chunk Save Method\" (under Chunk Options of the Routines window) to \"Always Save\", then apply the changes.<br><br>" @
					"<font:arial bold:16>Chunk Count<font:arial:14> - Displays in chat the number of current, existing chunk objects, the number of bricks within those chunks, and other statistics; same as using \"/PTGCount\" in chat.<br><br>" @ 
					"<font:arial bold:16>Show Pos<font:arial:14> - Displays your player's current, rounded position on the X, Y and Z axis in chat, as well as the direction your facing; same as using \"/PTGPos\" in chat, which by default can be used by all players (unless you disable permissions under Routines options).<br><br>" @ 
					"Note: PTG chat messages with \">\" in front mean only you can see that message.<br><br>" @
					"<font:arial bold:16>Show Cmds<font:arial:14> - Displays in chat all available chat commands included with the add-on; same as using \"/PTGCmds\" in chat.<br><br>" @ 
					"<font:arial bold:16><< less<font:arial:14> - Retracts the miniature control window to normal size.";
			
			PTG_Help_TxtCatSetList.setText(%text);
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////
		//Preview
		
		case "LandscapePreview":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Landscape Preview Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Landscape Preview\" segment of the \"Complex GUI: Previews\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"The GUI Preview window allows you to see how your landscape will generate, before actually starting a routine to generate it. Previews are done client-sided to prevent server lag, and are based on your current GUI settings; they don't take biome details, loaded builds or loaded chunks into account. " @
					"Previews also aren't affected by the delays set under Routine settings, and although aren't 100% accurate, are very close to the actual generated landscape.<br><br>" @
					
					"<font:arial bold:16>Small Toggle Window (Preview Options):<br><font:arial:14>" @
					"The toggle button labelled with a \"vvv\" allows you to open / close a small window which contains options for previewing your landscape. <br><br>" @ 

					"<font:arial bold:16>Layers<font:arial:14> - Previews are handled in layers, and the first 7 checkboxes for the small Preview Options window allows you to toggle which of those layers will be visible or not (encase you want see how a certain feature will generate by itself).<br><br>" @
					"<font:arial bold:16>Generate Fog / Depth<font:arial:14> - Simulates depth in the previews by darkening terrain that is higher up, and saturating (or whitening-out) terrain that is closer to the ground.<br><br>" @ 
					"<font:arial bold:16>Fog Ref Height<font:arial:14> - This is the height reference value used when generating depth, if enabled. Any terrain above this value will be darked, terrain below it will be saturated, and terrain closer to the value will be closer to its main color.<br><br>" @ 
					"<font:arial bold:16>Start, Halt and Clear<font:arial:14> - These buttons allow you to control the preview routine.<br><br>" @ 
					"<font:arial bold:16>Scale<font:arial:14> - Shows the relationship in previews between brick size and pixel size; 1 pixel equals 1 in-game meter which equals 2 brick studs in length.<br><br>" @ 
					"<font:arial bold:16>Total Bricks<font:arial:14> - Estimates how many bricks will be used if the previewed landscape is generated (doesn't take detail bricks, loaded builds or loaded chunks into account).<br><br>";
			
			PTG_Help_TxtCatSetList.setText(%text);
			
		//////////////////////////////////////////////////
		
		case "ServerStressEstimation":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Server Stress Estimation Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Server Stress Estimation\" segment of the \"Complex GUI: Previews\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			
			%text =	"An optional graph that shows how long, in milliseconds, each chunk takes to calculate. In theory, the longer a chunk takes to be calculated, the more time and resources will be required to actually generate the bricks within it; enabling the \"Generate Fog / Depth\" option for previews will slightly affect the graph results.<br><br>" @ 
					
					"<font:arial bold:16>Small Toggle Window (Lag Graph Options):<br><font:arial:14>" @
					"This window displays options related to the lag graph.<br><br>" @
					
					"<font:arial bold:16>Show Lag w/ Preview<font:arial:14> - A checkbox option that enables / disables rendering the lag graph during previews.<br><br>" @
					"<font:arial bold:16>Stress Ref Max<font:arial:14> - The amount in milliseconds used as the main reference for measuring server stress. If a chunk takes 20% of this value or less to calculate, it will be shown on the graph as blue, 20% - 40% will be shown as green, 50% - 60% as light yellow, etc.<br><br>";
					
			PTG_Help_TxtCatSetList.setText(%text);
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////
		//Setup
		
		case "Landscape":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Setup: Landscape Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Landscape\" segment of the \"Complex GUI: Setup\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"This category refers to basic options for the landscape in general; the other categories for this window allow you to change where or how the landscape generates.<br><br>" @
					"<font:arial bold:16>Seed<font:arial:14> - A value between -9999999 and 99999999 which changes how the landscape generates (seed values of 1 and 2 will both generate completely different landscapes).<br><br>" @
					"<font:arial bold:16>Random<font:arial:14> - Randomly chooses a seed value for you.<br><br>" @
					"<font:arial bold:16>Brick, Color and Print<font:arial:14> - Allows you to set the brick, color and print (if using Modular Terrain bricks) for default terrain - sometimes referred to as the default biome.<br><br>" @
					"<font:arial bold:16>Terrain Type<font:arial:14> - Contains 4 different options for generating terrain. \"Normal Terrain\" generates terrain as it normally would appear. \"Flatlands\" generates completely flat terrain (other features such as Mountains will still generate normally, however).<br><br>" @ 
					"\"Skylands\" is like normal terrain, except it only generates terrain above a certain section cut height, copies and inverts what is generated to add a bottom layer, and then randomly displaces the height of the resultant terrain.<br><br>" @
					"Note: The \"Load Chunk Saves Only\" terrain type will only load chunks from save files, instead of generating / calculating new chunks. In theory, using the \"Load Chunk Saves Only\" option to load a landscape only from saves can be much faster than calculating new chunks. This option can also be used to create infinite freebuilds.<br><br>" @ 
					"Vehicles are not automatically spawned when vehicle spawns are loaded from chunk saves.<br><br>" @
					"<font:arial bold:16>ModTer Type<font:arial:14> - Defines how Modular Terrain bricks generate for default terrain (\"Cubes & Ramps\" generates normal ModTer using cube and ramp / sloped bricks, \"Cubes & Wedges\" uses wedge bricks instead of ramps, and \"Cubes\" only uses cubed ModTer bricks instead of wedges or ramps).<br><br>" @
					"<font:arial bold:16>Enable AutoSaving<font:arial:14> - Toggles the autosaving feature, which works for both finite and infinite terrain. If enabled, the generator will automatically save all existing chunks to save files, based on your settings in the Routines window for how chunks are saved and how often. There are also ways to manually save chunks (such as using \"/PTGSave\"), which is recommended before closing your server.<br><br>" @
					"<font:arial bold:16>Gradual Player-Grid Generating<font:arial:14> - When infinite terrain is enabled, the generator first generates chunks within a certain area around one player, then moves on to the next. However, this could cause other players to wait for awhile before seeing any terrain generate for them. To counter that, this option will uniformly generate chunks around all players by first generating one chunk for one player, then moving on to the next. <br><br>Note: Gradual Player-Grid Generating is meant for infinite terrain only, but also works for finite terrain.<br><br>" @
					"<font:arial bold:16>Use Radial Grid<font:arial:14> - Generates a circular grid of chunks, for both infinite and finite terrain. If using this option for finite terrain, the shortest distance (based on the start / end values for X and Y) is used to determine the grid's radius. Otherwise for infinite terrain, the radius values for normal players and super admins is used.<br><br>";
			
			PTG_Help_TxtCatSetList.setText(%text);
			
		//////////////////////////////////////////////////
		
		case "FiniteTerrain":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Setup: Finite Terrain Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Finite Terrain\" segment of the \"Complex GUI: Setup\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"Options related to finite terrain - the standard setting for generating landscapes within a predefined area.<br><br>" @
					"<font:arial bold:16>Start Pos X / Y and End Pos X / Y<font:arial:14> - Allows you to set the start and end positions (in meters) for the grid of terrain that will generate; for finite terrain only. Both the start and end positions must be relative to the chunk size, although the generator will snap these values to the chunk size automatically. You can also use the \"+\" and \"-\" buttons to increment and decrement the values relative to the chunk size; make sure the start positions are < the end positions, negative values can also be used for both.<br><br>" @
					"<font:arial bold:16>Pos As Start<font:arial:14> - Grabs your player's current position, and uses it as the start position for both X and Y - automatically snapping them to the chunk size.<br><br>" @
					"<font:arial bold:16>Pos As Center<font:arial:14> - Grabs your player's current position, and modifies the current start and end values so that your player's position becomes the center of the landscape grid. This option will attempt to maintain the distances you set for the start and end positions, and will automatically snap the new values to the chunk size.<br><br>" @
					"<font:arial bold:16>Enable Edge Falloff<font:arial:14> - Forces terrain within a certain distance of the generated grid's edge to snap to the ground, gradually. This option also closes up openings for skylands, floating islands and clouds near the grid's edge, so that they will appear seamless.<br><br>" @
					"<font:arial bold:16>Falloff Grid Edge Distance<font:arial:14> - The distance from all four edges (in meters) of the generated grid in which terrain and other features begin gradually decreasing in height.<br><br>";
					
			PTG_Help_TxtCatSetList.setText(%text);
			
		//////////////////////////////////////////////////
		
		case "InfiniteTerrain":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Setup: Infinite Terrain Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Infinite Terrain\" segment of the \"Complex GUI: Setup\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"<font:arial bold:16>Introduction<font:arial:14> - The infinite terrain feature works similar to finite terrain, except it generates chunks of terrain relative to players as they move around the server, rather than using a predefined grid. By default, chunks are also removed if they exceed a certain distance from all players - to help keep the brick / chunk count down and to reduce lag caused by having too many objects being handled by the server.<br><br>" @
					"Infinite terrain works for all players on your server, and automatically adapts as players join and leave your server.<br><br>" @ 
					
					"<font:arial bold:16>Chunk Radius (Normal Players)<font:arial:14> - The amount of chunks to generate around players (who are not super administrators) when using infinite terrain (applies to both normal and radial grids).<br><br>" @
					"<font:arial bold:16>Chunk Radius (Super Admins)<font:arial:14> - The amount of chunks to generate around players (who have super admin status) when using infinite terrain (applies to both normal and radial grids).<br><br>" @
					"<font:arial bold:16>Remove Distant Chunks<font:arial:14> - If enabled, this option will remove chunks that exceed a certain distance from all players, based on the Chunk Radius settings for normal players and super admins. This option only applies to infinite terrain generation, and helps keep the brick / chunk count down. When disabled, you can generate areas of terrain by flying around when infinite terrain is enabled. <br><br>Note: If changes are made to this option while terrain is generating, you need to stop generation and restart for it to take effect.";
			
			PTG_Help_TxtCatSetList.setText(%text);
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////
		//Features
		
		case "TerrainOptions":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Features: Terrain Options Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Terrain Options\" segment of the \"Complex GUI: Features\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"This category refers to options related to terrain in general.<br><br>" @
					"<font:arial bold:16>Dirt Color and Print<font:arial:14> - Allows you to set the color and print texture for bricks that generate under the main terrain brick, and is usually handled as cubes or pillars to fill in gaps under terrain.<br><br>" @
					"<font:arial bold:16>Stone Color and Print<font:arial:14> - (Meant for the Skylands terrain type) Allows you to set the color and print texture for bricks that generate for the bottom layer for Skylands (also usually generate as cubes and pillars, similar to the dirt bricks).<br><br>" @
					"<font:arial bold:16>Water Level<font:arial:14> - The height level (in meters) where water bricks and the submarine biome generate on the terrain (note: this also affects where the submarine biome generates for Skylands, but water bricks won't generate for that terrain type).<br><br>" @
					"<font:arial bold:16>Sand Level<font:arial:14> - The height level where the shore biome generates for terrain (also works for Skylands).<br><br>" @
					"<font:arial bold:16>Terrain Z-Offset<font:arial:14> - Raises or lowers the overall height of terrain bricks. Note: The sand and water levels are not relative to this value; changing the Terrain Z-Offset will also affect where sand and water generate.<br><br>" @
					"<font:arial bold:16>Connect Lakes<font:arial:14> - Forces terrain that is below a certain level to gradually sink toward the ground, (based on the Connect Lakes H value in Advanced options). The Connect Lakes option should cause areas in-between lakes (if low enough) to sink below the water surface, allowing for more water bricks to generate.<br><br>" @
					"<font:arial bold:16>Enable Plate-Capping<font:arial:14> - Adds plate bricks on top of terrain bricks, with the default terrain color / print, and replaces the bricks below with the dirt color / print (works great when using cube bricks for terrain; doesn't apply to floating islands).<br><br>" @
					"<font:arial bold:16>Dirt Color / Prints Same As Relative Biome Terrain<font:arial:14> - Overrides the the color and print of dirt bricks (if within a custom biome) so that they are the same color and print as terrian (doesn't apply to dirt bricks in the default biome).<br><br>" @
					"<font:arial bold:16>Shore Color / Prints Same As Relative Biome Terrain<font:arial:14> - Normally, terrain bricks within the shore biome don't change their color or print if within one of the custom biomes. This option however will override the colors and prints of shore terrain bricks to be the same as the custom biome they're in (useful for biomes in which you don't want the shore colors / prints to appear when near water).<br><br>" @
					"<font:arial bold:16>Disable Water Bricks<font:arial:14> - Disables the generation of water bricks when terrain or caves are below the water level, but still allows the submarine biome to generate for terrain.<br><br>";
			
			PTG_Help_TxtCatSetList.setText(%text);
			
		//////////////////////////////////////////////////
		
		case "Details&Biomes":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Features: Details & Biomes Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Details & Biomes\" segment of the \"Complex GUI: Features\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"Options related to biomes, and the detail bricks that generate within them.<br><br>" @
					"<font:arial bold:16>Enable Detail Bricks<font:arial:14> - If enabled, allows detail bricks to generate, relative to the bricks you setup under the Biomes options window.<br><br>" @
					"<font:arial bold:16>Generate Default Biome Terrain Details For Floating Islands<font:arial:14> - (Only applies if Enable Biome Details is also enabled) Normally, detail bricks don't generate for floating islands. However, this option will allow the same detail bricks for default terrain to generate for floating islands as well.<br><br>" @
					"Note: Details for floating islands are relative to the floating islands brick size, so using a smaller or larger brick size for them will affect the amount of details generated (depending on the detail frequency).<br><br>" @
					"<font:arial bold:16>Detail Frequency<font:arial:14> - How frequently to generate detail bricks for terrain (and sometimes floating islands). Note: The Detail Frequency value is relative to the brick size set for terrain (smaller bricks will cause more details to generate, and larger bricks will cause less to generate).<br><br>" @ 
					"<font:arial bold:16>Enable Custom Biome A<font:arial:14> - Allows generation of the Custom A Biome, based on your settings in the Biomes options window.<br><br>" @ 
					"<font:arial bold:16>Enable Custom Biome B<font:arial:14> - Allows generation of the Custom B Biome.<br><br>" @ 
					"<font:arial bold:16>Enable Custom Biome C<font:arial:14> - Allows generation of the Custom C Biome.<br><br>" @ 
					"<font:arial bold:16>Auto-Hide Generated Spawn Bricks<font:arial:14> - Disables rendering of any bricks (which generate as a biome detail) that contain the text \"Spawn\" in its datablock name (applies to player and bot spawns, vehicle spawns, etc.).<br><br>" @ 
					"<font:arial bold:16>Set Up Mass Biome Details<font:arial:14> - Normally, the generator allows you to set up up to 18 different detail bricks to generate for biomes, under the Biomes options window. This button however will open up another window, which will allow you to set up countless details if needed, with a hard limit of up to 400 bricks for each biome.<br><br>";
					
			PTG_Help_TxtCatSetList.setText(%text);
			
		//////////////////////////////////////////////////
		
		case "Mountains":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Features: Mountains Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Mountains\" segment of the \"Complex GUI: Features\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"Mountains are areas of terrain which have their own unique height scale, detail bricks and settings (they are handled as a separate biome, so details, color and prints for them can be set up under Biomes options).<br><br>" @
					"<font:arial bold:16>Enable Snow Layer For Mountains<font:arial:14> - If enabled, will cause terrain bricks above this level to use the snow color / print (defined in Biomes options).<br><br>" @ 
					"<font:arial bold:16>Snow Start<font:arial:14> - The height level (in meters) in which snow generates for mountains.<br><br>" @ 
					"<font:arial bold:16>Z-Snap Mult.<font:arial:14> - Terrain normally snaps to a height level relative to the height of the brick you set for terrain. However, with this option you can set mountains to snap to an entirely different height level. Note: This value is a multiple of the default terrain brick height - chosen under Setup options.<br><br>" @ 
					"<font:arial bold:16>Z-Multiplier<font:arial:14> - Modifies the overall height scale of mountains by multiplying them by this value.<br><br>";
			
			PTG_Help_TxtCatSetList.setText(%text);
		
		//////////////////////////////////////////////////
		
		case "Caves":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Features: Caves Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Caves\" segment of the \"Complex GUI: Features\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"Caves, like Mountains, also are treated as a separate biome from default terrain - both the top and bottom layers of caves are handled separately. Note: Caves won't generate when the Skylands terrain type is selected.<br><br>" @
					"<font:arial bold:16>Z-Offset<font:arial:14> - Raises or lowers the overall height for caves (in meters).<br><br>";
			
			PTG_Help_TxtCatSetList.setText(%text);
		
		//////////////////////////////////////////////////
		
		case "Clouds":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Features: Clouds Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Clouds\" segment of the \"Complex GUI: Features\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"Clouds are kind of like a separate layer of terrain that generates independently of normal, default terrain; they aren't treated like biomes, so you can't define details for them. You can however set up how high clouds generate, as well as custom colors, prints and other aspects for them.<br><br>" @
					"<font:arial bold:16>Brick, Print and Color<font:arial:14> - Allows you to set a custom brick size for clouds (independent of the terrain brick size), as well as the print texture (if using ModTer) and color for cloud bricks.<br><br>" @
					"<font:arial bold:16>Z-Offset<font:arial:14> - Raises or lowers the overall height for clouds (in meters; not relative to the Terrain Z-Offset height).<br><br>" @
					"<font:arial bold:16>ModTer Type<font:arial:14> - Allows you to define how Modular Terrain bricks generate for clouds, if a ModTer brick is selected for them (independent of the ModTer type for default terrain) - \"Cubes & Ramps\" generates normal ModTer using cube and ramp / sloped bricks, \"Cubes & Wedges\" uses wedge bricks instead of ramps, and \"Cubes\" only uses cubed ModTer bricks instead of wedges or ramps.<br><br>" @
					"<font:arial bold:16>Clouds Have Collision<font:arial:14> - Toggles collision for generated cloud bricks.<br><br>";
			
			PTG_Help_TxtCatSetList.setText(%text);
		
		//////////////////////////////////////////////////
		
		case "FloatingIslands":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Features: Floating Islands Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Floating Islands\" segment of the \"Complex GUI: Features\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"Floating Islands are similar to clouds, except a bottom layer is added underneath by copying and inverting what is generated. If enabled, the floating islands option will generate two unique sets of islands - you can define different generation heights for both sets.<br><br>" @
					"<font:arial bold:16>Terrain Brick, Print and Color<font:arial:14> - Allows you to define a custom brick size, print texture (if using ModTer) and color for bricks generated for floating islands.<br><br>" @
					"<font:arial bold:16>IslandA Z-Offset<font:arial:14> - The overall height at which the first floating island set generates.<br><br>" @ 
					"<font:arial bold:16>IslandB Z-Offset<font:arial:14> - The overall height at which the second floating island set generates.<br><br>" @ 
					"<font:arial bold:16>Dirt Print and Color<font:arial:14> - The print texture and color for bricks that generate under the main floating island terrain brick.<br><br>" @ 
					"<font:arial bold:16>Stone Print and Color<font:arial:14> - The print texture and color for bricks that generate on the bottom, inverted layer for both sets of islands.<br><br>" @ 
					"<font:arial bold:16>ModTer Type<font:arial:14> - Defines how ModTer bricks generate for floating islands, if a ModTer brick is selected for them - \"Cubes & Ramps\" generates normal ModTer using cube and ramp / sloped bricks, \"Cubes & Wedges\" uses wedge bricks instead of ramps, and \"Cubes\" only uses cubed ModTer bricks instead of wedges or ramps.<br><br>";
			
			PTG_Help_TxtCatSetList.setText(%text);
		
		//////////////////////////////////////////////////
		
		case "Boundaries":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Features: Boundaries Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Boundaries\" segment of the \"Complex GUI: Features\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"This option creates a wall of bricks around generated landscapes; works for both normal and radial grids, for infinite and finite terrain, and for both generated chunks and for chunks loaded from saves (however, boundaries are not saved with chunks). <br><br>" @ 
					"Note: If a Modular Terrain brick is selected for the terrain, boundaries will switch to ModTer print bricks as well. Also, ceiling bricks can be toggled for boundaries, allowing you to completely enclose your landscapes if desired.<br><br>" @
					"<font:arial bold:16>Walls Print and Color<font:arial:14> - The print texture (if ModTer is used for the default terrain brick) and color for boundary walls.<br><br>" @
					"<font:arial bold:16>Ceiling Print and Color<font:arial:14> - The print texture and color for boundary ceilings.<br><br>" @
					"<font:arial bold:16>Bricks Above Terrain (Relative)<font:arial:14> - Boundaries can be set up to either have a pre-defined height, or to be relative to the highest terrain brick within each chunk. If the latter is true and boundaries are relative to the terrain height, this value allows you to set how many boundary bricks will generate above that point.<br><br>" @
					"<font:arial bold:16>Bounds Height<font:arial:14> - The max height for boundary walls to generate, and the offset height for which ceiling bricks generate.<br><br>" @
					"<font:arial bold:16>Bounds Height Relative To Terrain Height<font:arial:14> - Toggles whether or not the height of boundary bricks will be relative to the height of terrain in chunks.<br><br>" @
					"<font:arial bold:16>Offset Bounds Start Height Based on Terrain Z-Offset<font:arial:14> - <br><br>" @
					"<font:arial bold:16>Enable Ceiling Boundary<font:arial:14> - Toggles generation of ceiling bricks.<br><br>" @
					"<font:arial bold:16>Use Static Shapes<font:arial:14> - Toggles generating boundary walls and ceilings as normal bricks or static shape objects. Static shapes are usually recommended since only one object is required per pillar (the objects are stretched to reduce the amount of objects generated). Static shapes also don't use textures, but still use the colors you define for them, so they are created and removed much faster, and thus are better suited for infinite terrain.<br><br>" @
					"One drawback for static shapes is that defining transparent colors for them can cause problems with rendering - such as causing faces to flicker during movement. Static shapes are also recommended to be used if ModTer is set up for default terrain (ModTer boundaries with print textures can cause lag during generation / removal).<br><br>" @
					"<font:arial bold:16>Invisible Static Shapes<font:arial:14> - If static shapes are enabled for boundaries, this option (if enabled) will disable rendering for them, making them completely invisible while still retaining their collision (this option is useful for preventing players and bots from falling off the edge of the landscape, especially when infinite terrain is enabled).<br><br>";
			
			PTG_Help_TxtCatSetList.setText(%text);
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////
		//Biomes
		
		case "Biomes":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Biomes Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Biomes\" segment of the \"Complex GUI: Biomes\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"Biomes are areas of terrain that have their own unique colors, prints, detail bricks (to add things like trees, plants, flowers, etc. to the biome) and other features. Three custom biomes are also included, which can be toggled to generate randomly on top of normal, default terrain.<br><br>" @
					"Note: You can cycle through each biome by using the popup menu near the top-right of the window.<br><br>" @
					"<font:arial bold:16>Print and Color (Terrain / Sand / Rock)<font:arial:14> - The print texture (if a ModTer brick is defined for terrain) and color for bricks that generate for that biome (the print and color option doesn't appear for the Default biome because they are handled under the Setup options window).<br><br>" @
					"<font:arial bold:16>Water Print and Color<font:arial:14> - The print texture (if a ModTer brick is defined for terrain) and color for water bricks that generate within those biomes (only appears for the Default and Custom biomes).<br><br>" @
					"<font:arial bold:16>Terrain Height Mod<font:arial:14> - Allows you to modify the height of terrain within certain biomes - this option multiplies the terrain height by the value you define here (this option only appears for the Custom biomes, and is based on the Iteration A XY noise scale set for terrain under Advanced options).<br><br>" @
					"<font:arial bold:16>Water Type<font:arial:14> - The type of water to generate for water bricks within that biome; each option has unique properties that are applied to the generated bricks.<br><br>" @
					"\"Normal Water\" generates water as it normally would appear and act. \"Lava\" adds an undulo wave effect to water bricks, as well as fire particles and a physical zone that injures players when the brick is entered. \"Ice\" adds collision to water, disables the wave effect and adds vapor particles to the brick. \"QuickSand\" also removes the wave effect, but adds a physical zone to water that causes players to sink into the brick when entered.<br><br>" @
					"Note: It's not recommended to only use transparent colors for water (due to a glitch with the engine that affects how bricks are rendered), even though the option is available. Water Type effects are also applied when water bricks are loaded from chunk saves.<br><br>" @
					"<font:arial bold:16>Common Details<font:arial:14> - Detail bricks that will generate normally for the particular biome its in, based on the detail frequency set under Features options.<br><br>" @
					"<font:arial bold:16>Uncommon Details<font:arial:14> - Detail bricks that will generate 1/4 as often as Common Details.<br><br>" @
					"<font:arial bold:16>Rare Details<font:arial:14> - Detail bricks that will generate 1/8 as often as Common Details.<br><br>" @
					"<font:arial bold:16>Clear<font:arial:14> - This button will reset all detail brick options for common, uncommon or rare details for that biome.<br><br>" @
					"<font:arial bold:16>Clear Biome Details<font:arial:14> - Resets all common, uncommon and rare detail options for that biome.<br><br>" @
					"<font:arial bold:16>Clear All Details<font:arial:14> - Resets all common, uncommon and rare detail options for all biomes.<br><br>";

			PTG_Help_TxtCatSetList.setText(%text);
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////
		//Build Loading
		
		case "BuildLoadOptions":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Build Loading: Options Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Options\" segment of the \"Complex GUI: Build Loading\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"This window lists general options related to Build Loading and Flat Areas. Build Loading is a feature of the generator that allows you to generate your own .bls brick saves / builds randomly through out the landscape. You can set up a build to be used by the generator by first uploading that build to the server, where it's converted and stored in a custom format. You can define placement parameters for each build, as well as the frequency.<br><br>" @
					"When a build is loaded / generator into the landscape, the generator will automatically flatten the terrain to accomodate the build. However, you can also set up areas of terrain to flatten without loading builds, which makes building on terrain much easier.<br><br>" @
					"<font:arial bold:16>Enable Build Loading<font:arial:14> - If enabled, will allow uploaded / converted builds to generate throughout the landscape.<br><br>" @
					"<font:arial bold:16>Enable Flat Area Generation<font:arial:14> - If enabled, will allow flat areas to generate throughout the landscape (for default terrain and for floating islands). This feature is independent of build loading, so you can have either or both enabled at the same time.<br><br>" @
					"<font:arial bold:16>Generate Details on Flat Areas<font:arial:14> - Toggles generation of biome detail bricks if within a flat area.<br><br>" @
					"<font:arial bold:16>Generate builds / flat areas on highest...<font:arial:14> - If enabled, will flatten terrain relative to the highest point within the relative area (otherwise if disabled, uses lowest area).<br><br>" @
					"<font:arial bold:16>Flat Area Frequency<font:arial:14> - How often to generate flat areas.<br><br>" @
					"<font:arial bold:16>Flat Area Smallest Grid Size<font:arial:14> - The smallest grid size used for flat areas (the generator will randomly choose a grid size between the smallest and largest values you define, and will flatten terrain within that grid area).<br><br>" @
					"<font:arial bold:16>Flat Area Largest Grid Size<font:arial:14> - The largest grid size for flat areas.<br><br>" @ 
					"Note: Only one build and flat area grid can be generated per chunk, but large builds and flat area grids can span multiple chunks. Also, when loading a build, the generator will first try to generate that build on floating islands. If that option is disabled for the build or fails, it will then try to load it on the water surface. Lastly, if that option is disabled or fails, it attempts to load the build either underwater (if the build is fully submerged) or on terrain.<br><br>" @ 
					"Note: Vehicles are not automatically spawned when vehicle spawns are loaded from builds.";

			PTG_Help_TxtCatSetList.setText(%text);
			
		//////////////////////////////////////////////////
		
		case "BuildLoadSaves":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Build Loading: Player Saves Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Player Saves\" segment of the \"Complex GUI: Build Loading\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"This window lists all of your .bls saves along with the brick count per file, and allows you to choose which saves to upload to the server. If you're hosting your own server, saves will be stored in a custom format on your computer. If you're on someone else's server and they give you permission to upload a build, that file will be stored in a custom format on their computer.<br><br>" @
					"<font:arial bold:16>Reload List<font:arial:14> - Reloads the list of .bls saves on your computer.<br><br>" @
					"<font:arial bold:16>Upload Selected<font:arial:14> - Opens a new window for the selected save that will allow you to define parameters for that build, and to upload it to the server. Note: Make sure you select a save from the list first.<br><br>" @
					"<font:arial bold:16>Sort By Name<font:arial:14> - Reorganizes the .bls saves list to sort files according to name (saves are also organized according to color - if blue or red).<br><br>" @
					"<font:arial bold:16>Sort By Bricks<font:arial:14> - Reorganizes the .bls saves list to sort files according to the amount of bricks per file (saves are also organized according to color - if blue or red).<br><br>" @
					"Note: Selecting a file from the list will cause the preview screenshot for that save to appear in the preview window.<br><br>" @ 
					"Note: When a save is being uploaded, you can still change GUI settings, preview landscapes etc. - it won't interfere. You can also close the GUI and wait for it to finish; you will be notified once uploading is complete.";

			PTG_Help_TxtCatSetList.setText(%text);
		
		//////////////////////////////////////////////////
		
		case "BuildLoadBuilds":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Build Loading: Loaded Builds Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Loaded Builds\" segment of the \"Complex GUI: Build Loading\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"This window lists all of the uploaded / converted builds on the host's computer (if you're the host, then on your computer). Note: You can't upload or manage builds on someone else's computer unless they enable permissions to do so.<br><br>" @
					"<font:arial bold:16>Reload List<font:arial:14> - Reloads the list of all uploaded builds stored on the host's computer.<br><br>" @
					"<font:arial bold:16>Sort By #<font:arial:14> - Reorganizes the uploaded builds list to sort files according to their listed number (saves are also organized according to color - if blue or red).<br><br>" @
					"<font:arial bold:16>Sort By Name<font:arial:14> - Reorganizes the uploaded builds list to sort files according to their name (saves are also organized according to color - if blue or red).<br><br>" @
					"<font:arial bold:16>Remove Selected<font:arial:14> - Removes a selected file from the host's computer and the build list.<br><br>" @
					"<font:arial bold:16>Edit Selected<font:arial:14> - Opens a new window which allows you to make changes to the selected build's frequency and parameters.<br><br>" @
					"<font:arial bold:16>Toggle On / Off<font:arial:14> - Toggles the selected file to be enabled or not (if disabled, it won't be used by the generator, but will still remain on the host's computer).<br><br>" @
					"Note: Preview screenshots for uploaded builds will only appear for selected files if you're locally connected to the server (if you're hosting your own, non-dedicated server).";

			PTG_Help_TxtCatSetList.setText(%text);
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////
		//Presets
		
		case "Presets":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Presets Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Presets\" segment of the \"Complex GUI: Presets\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"Presets are GUI settings which have been saved to file, and can be reloaded back into your GUI. This window allows you to list / manage saved presets, as well as to save new ones or to load selected presets.<br><br>" @ 
					"Presets also allow you to easily share landscapes with others. If you want to generate a landscape that someone else created, just download and install their preset, load the settings into your GUI and start the generator. Presets can be installed by placing the file in the \"config/client/PTGv3/Presets/\" directory; you can keep them in .zip files (if in a pack), but it's usually best to unzip them so you can overwrite them later.<br><br>" @
					"Note: Selecting a file from the list will automatically load the preset name and description into the GUI fields, as well as a screenshot of the file in the preview window.<br><br>" @
					"<font:arial bold:16>Reload List<font:arial:14> - Reloads the list of saved presets. Note: A preset file will not appear in the list if installed at runtime (while the game is running). However, presets saved in subfolders or .zip files will be listed and can be loaded into the GUI (if not installed at runtime).<br><br>" @
					"<font:arial bold:16>Rename Selected<font:arial:14> - Renames the selected preset file to what you entered in the Preset Name field.<br><br>" @
					"<font:arial bold:16>Load Selected<font:arial:14> - Loads the settings from the selected preset file into your GUI.<br><br>" @
					"Note: If certain bricks or prints couldn't be found from the loaded preset, an Error Report GUI will pop up, notifying you of the UI names for each. Make sure you have the bricks and prints enabled (that were used for the preset) before loading it into your GUI.<br><br>" @
					"<font:arial bold:16>Delete Selected<font:arial:14> - Deletes the selected preset file and automatically removes it from list.<br><br>" @
					"<font:arial bold:16>Resort List<font:arial:14> - Reorganizes the list of presets according to name (this option switches between ascending and descending order each time it's selected).<br><br>" @
					"<font:arial bold:16>Save New<font:arial:14> - Saves your current Complex GUI settings to a preset save file, (using the name and description you entered), and takes a screenshot of the generated landscape, which is saved with the file. If a file already exists with the given name, the GUI will ask you if you want to overwrite the file instead of automatically deleting it.<br><br>" @
					"Note: Preset saves and screenshots are saved in the following directory: \"config/client/PTGv3/Presets/\". Saving a preset with the name \"Default\" will automatically treat that file as the new GUI default settings.<br><br>" @
					"<font:arial bold:16>Clear Fields<font:arial:14> - Resets the Preset Name and Preset Description fields.<br><br>" @
					"Note: When starting or joining a new server, the GUIs will automatically reset back to default settings. If you want to save your settings to a preset file, make sure to do so before disconnecting. Also, it's recommended to revert back to default settings (by loading the \"<<Default Settings>>\" preset) before setting up your own preset save.";
					
			PTG_Help_TxtCatSetList.setText(%text);
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////
		//Advanced
		
		case "AdvancedMisc":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Advanced: Miscellaneous Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Miscellaneous\" segment of the \"Complex GUI: Advanced\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"In-depth options relating to various features.<br><br>" @ 
					"<font:arial bold:16>Chunk Size<font:arial:14> - Terrain and other features are handled within chunk objects, this option allows you to set the size of those objects (in meters).<br><br>" @
					"Note: Larger chunk objects could cause an increase in lag, so use with caution. Smaller chunk sizes also allow you to use smaller noise scale iteration sizes, if desired.<br><br>" @
					"<font:arial bold:16>Cave Top Z-Multiplier<font:arial:14> - Modifies the overall height scale of the top cave layer by multiplying it by this value.<br><br>" @
					"<font:arial bold:16>Noise Control Height<font:arial:14> - The standard reference height used by all noise algorithms; changing this value will affect the Z-Axis noise scale values for all features (terrain, mountains, caves, etc.).<br><br>" @
					"<font:arial bold:16>Connect Lakes H<font:arial:14> - The height (in meters) above the water level in which terrain gradually sinks below the water surface (if the Connect Lakes feature is enabled).<br><br>" @
					"<font:arial bold:16>Sylvanor Tree Base Colors<font:arial:14> - The generator has built-in support for using Sylvanor's tree bricks - when one of the tree top bricks are used as a biome detail, the generator will automatically plant one of the tree base bricks underneath. Here you can define three colors that will be randomly used for the tree base bricks.<br><br>" @
					"<font:arial bold:16>Ensure FIFO Chunk Removal<font:arial:14> - Sometimes during infinite terrain generation, new chunks can be added to the top of the chunk list, which causes problems when distant chunks are removed - as the list is frequently updated, old chunks are never checked for removal. This option prevents that from happening by checking older chunk objects for removal first, but might also slightly slow down the chunk removal process.<br><br>" @
					"<font:arial bold:16>Ensure seamless terrain for ModTer<font:arial:14> - Normally, when Modular Terrain is calculated for terrain, only bricks within chunks and on the edges of chunks are checked, which sometimes leaves gaps in chunk corners where ModTer should generate. This option makes sure that corners of chunks are also checked, ensuring that terrain remains seamless. Note: This also works for other features that use ModTer.<br><br>" @
					"<font:arial bold:16>Ensure seamless terrain for Build Loading / Flat Areas<font:arial:14> - This option prevents gaps from appear in terrain (and sometimes floating islands) when flat areas are generated by checking adjacent chunks before filling in gaps with fill bricks.<br><br>";
					
			PTG_Help_TxtCatSetList.setText(%text);
		
		//////////////////////////////////////////////////
		
		case "NoiseOffsets":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Advanced: Noise Offsets Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Noise Offsets\" segment of the \"Complex GUI: Advanced\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"This section allows you to manipulate the generator's noise algorithms by changing the location where certain features (such as mountains, caves, clouds, etc.) generate, with out actually moving them. It works by \"treadmilling\" those features; they still generate in the same location, but are generated as if they were somewhere else.<br><br>" @
					"The text boxes provided allow you to define the value (in meters) by which to offset the position of the landscape features; both positive and negative integers can be used, but larger values seem to work best.";
					
			PTG_Help_TxtCatSetList.setText(%text);
		
		//////////////////////////////////////////////////
		
		case "SectionCuts":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Advanced: Section Cuts Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Section Cuts\" segment of the \"Complex GUI: Advanced\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"Most features for the generator work by adding various iterations of noise scales together (like overlaying a series of hills on top of each other), then cutting the result at a certain level and generating what is above or below that level. These values allow you to set the section cut height level (on the Z-Axis) for each of the various features.<br><br>" @
					"The text in red will inform you whether each feature will generate above (>) or below (<) the section cut height, and will display the max height (in meters) possible per feature - the max height values auto update as the noise scale Z-Axis values and the Noise Control Height values are modified.";
					
			PTG_Help_TxtCatSetList.setText(%text);
		
		//////////////////////////////////////////////////
		
		case "NoiseScales":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Advanced: Noise Scales Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Noise Scales\" segment of the \"Complex GUI: Advanced\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"As mentioned for Section Cut values, most features for the generator work by adding various iterations of noise scales together (similar to overlaying a series of hills on top of each other). The textboxes provided here allow you to set the length / width of those iterations (XY-Axis values), as well as their height scales (Z-Axis values).<br><br>" @
					"Note: Z-Axis values are multiples of the Noise Control Height (for instance, if the Noise Control Height is 50 and a Z-Axis scale values is 2, that iteration would have a height of 100. Z-Axis values can be any float / decimal value between 0.0 and 8.0<br><br>" @
					"Note: XY-Axis values must be relative to the Chunk Size, and are handled in meters; iteration A and B values must be >= the Chunk Size, and iteration C values must be <= the Chunk Size.<br><br>" @
					"<font:arial bold:16>Auto-Fix Any Issues<font:arial:14> - When in doubt, this button with automatically adjust all noise scale values to prevent any issues when settings are uploaded or during previews (it also attempts to maintain the values you previously entered).";
					
			PTG_Help_TxtCatSetList.setText(%text);
		
		//////////////////////////////////////////////////
		
		case "PseudoEquator":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Advanced: Pseudo-Equator Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Pseudo-Equator\" segment of the \"Complex GUI: Advanced\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"When enabled, this option attempts to simulate the earth's equator by preventing certain features (especially biomes) from generating until a certain distance away from the origin, on either the positive or negative Y-Axis. Here you can toggle the Pseudo-Equator option, as well as define how far to offset when features begin generating from the origin (setting a value of \"0\" will allow them to generate normally).<br><br>" @
			"Note: Make sure to only use positive integers for the Y-Axis offsets. Also, these values are relative to the iteration A noise scales for each feature, so setting the same value for different features may not necessarily cause them to start generating in the same location.<br><br>" @
			"Note: This option is useful for biomes, especially if you want to set up an arctic biome to generate further away from the origin, and a humid-tropical biome to generate closer to it. This option can also be used to reward exploration; remember than you can set up loaded builds to only generate for certain biomes.";
					
			PTG_Help_TxtCatSetList.setText(%text);
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////
		//Routines
		
		case "RoutineStreams":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Routines: Streams Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Pseudo-Equator\" segment of the \"Complex GUI: Routines\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"The generator supports dynamic, cascading streams, which can be planted using the stream source bricks included with the add-on. When planted, streams will flow downward under the source until it reaches another brick, then flows outward a certain distance, or until it can find a space to continue descending.<br><br>" @
					"This section allows you to control certain aspects of the stream bricks and the scripts that handle dynamic cascades.<br><br>" @
					"Note: Streams also automatically group smaller bricks together into large bricks (if above the last brick planted) to help keep the brick count as low as possible.<br><br>" @
					"<font:arial bold:16>Update Tick<font:arial:14> - How frequently (in milliseconds) stream bricks update / spread.<br><br>" @
					"<font:arial bold:16>Max Spread Distance<font:arial:14> - How far (in bricks) streams spread on flat surfaces.<br><br>" @
					"<font:arial bold:16>Set As Host-Only<font:arial:14> - If enabled, this option prevents players from planting source bricks unless they're the server host (otherwise, only administrators can plant them).<br><br>" @
					"<font:arial bold:16>Streams Have Collision<font:arial:14> - Toggles collision for stream bricks.<br><br>" @
					"<font:arial bold:16>Streams Can Clear Detail Bricks<font:arial:14> - If enabled, streams will automatically remove biome details they collide with (useful to allow streams to continue generating, otherwise streams will go around details).<br><br>" @
					"<font:arial bold:16>Add Water Zones To Larger Bricks<font:arial:14> - This option adds water zones to streams that are grouped into larger bricks.<br><br>" @
					"<font:arial bold:16>Allow Planting Floating Streams<font:arial:14> - Toggles the restriction for planting streams source bricks in mid-air (the source brick doesn't actually get planted, but streams will start generating from it)." @
					"Note: You can immediately stop any streams that are generating by simply unchecking the \"Enable Stream Bricks\" option and applying the changes.<br><br>";
					
			PTG_Help_TxtCatSetList.setText(%text);
		
		//////////////////////////////////////////////////
		
		case "GenLimits":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Routines: Generator Limits Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Generator Limits\" segment of the \"Complex GUI: Routines\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"Various limits you can set for the generator; certain limits can be used to prevent lag and to prevent the server from crashing.<br><br>" @
					"<font:arial bold:16>Near Brick Limit Amount Buffer<font:arial:14> - The amount of total bricks, subtracted from the server brick limit, which the generator shouldn't exceed. If this limit is exceeded and both Infinite Terrain and Remove Distant Chunks are enabled under Setup options, the current routine will be paused. Otherwise, the routine will be halted.<br><br>" @
					"<font:arial bold:16>Max Ping Lag Buffer<font:arial:14> - The maximum server ping value before the server is considered to be lagging (if lag is detected according to this value, the generator will pause a routine that's running).<br><br>" @
					"<font:arial bold:16>Dedicated Server Function Lag Buffer<font:arial:14> - Similar to the Max Ping Lag Buffer setting, but applies to dedicated servers; if the server is dedicated, the generator checks for lag using another method, since the Server Connection object doesn't exist and thus server ping can't be accessed.<br><br>" @ 
					"Instead, the generator will check how long it takes to complete a routine loop from one chunk to the next (in milliseconds). If this limit is exceeded in the time required to complete a routine loop, the server will be determined to be lagging and the current routine will be paused / halted.<br><br>" @
					"<font:arial bold:16>Server Chunk Object Limit<font:arial:14> - The maximum amount of chunk objects allowed to exist on the server at the same time. If this limit is exceeded and both Infinite Terrain and Remove Distant Chunks are enabled under Setup options, the current routine will be paused. Otherwise, the routine will be halted.<br><br>" @
					"<font:arial bold:16>Max Chunk Saves Per Seed<font:arial:14> - Chunk saves are saved relative to the server seed (and chunk size values); this option allows you to set the maximum amount of chunks that can be saved per seed (you can specify what happens when this value is exceeded under Chunk Options).<br><br>" @
					"<font:arial bold:16>Max Total Chunk Saves<font:arial:14> - The maximum amount of chunk saves in total (you can specify what happens when this value is exceeded under Chunk Options).<br><br>" @
					"<font:arial bold:16>Max Build-Load Files<font:arial:14> - The maximum amount of player builds that can be uploaded to the server (for the Build Loading feature).<br><br>" @
					"<font:arial bold:16>Disable Brick Limit Amount Buffer<font:arial:14> - Disables the option to pause or halt a routine if the \"Near Brick Limit Amount Buffer\" value is exceeded (only checked when new chunk objects are created).<br><br>" @
					"<font:arial bold:16>Disable Chunk Object Limit Buffer<font:arial:14> - Disables the option to pause or halt a routine if the \"Server Chunk Object Limit\" value is exceeded (only checked when new chunk objects are created).<br><br>" @
					"<font:arial bold:16>Disable Lag Check for Normal Servers<font:arial:14> - Disables the option to check for lag on normal servers based on the \"Max Ping Lag Buffer\" value; if this option isn't selected, the generator will pause or halt if lag is detected.<br><br>" @
					"<font:arial bold:16>Disable Lag Check for Dedicated Servers<font:arial:14> - Disables the option to check for lag on dedicated servers based on the \"Dedicated Server Function Lag Buffer\" value; if this option isn't selected, the generator will pause or halt if lag is detected.<br><br>";
					
			PTG_Help_TxtCatSetList.setText(%text);
		
		//////////////////////////////////////////////////
		
		case "GenSchedules":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Routines: Generator Schedules Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Generator Schedules\" segment of the \"Complex GUI: Routines\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"Options related to generator schedules and delays. Delays can be used to reduce lag and improve generator performance. However, increasing delays will also increase the amount of time required to generate and remove bricks / chunks.<br><br>" @
					"<font:arial bold:16>Pause Tick<font:arial:14> - How long (in seconds) to pause a routine if lag or a certain server limit is exceeded.<br><br>" @
					"<font:arial bold:16>AutoSave Schedule<font:arial:14> - How often (in minutes) to autosave chunks (if the option is enabled).<br><br>" @
					"<font:arial bold:16>Primary Generation Delay<font:arial:14> - How long to wait (in milliseconds) before generating a certain feature for a chunk.<br><br>" @
					"<font:arial bold:16>Secondary Generation Delay<font:arial:14> - How long to wait (in milliseconds) before moving on to generate another feature, if the current feature is disabled or finished generating.<br><br>" @
					"<font:arial bold:16>Primary Calculation Delay<font:arial:14> - How long to wait (in milliseconds) before calculating a certain feature for a chunk.<br><br>" @
					"<font:arial bold:16>Secondary Calculation Delay<font:arial:14> - How long to wait (in milliseconds) before moving on to calculate another feature, if the current feature is disabled or finished being calculated.<br><br>" @
					"<font:arial bold:16>Brick Plant Delay<font:arial:14> - How long to delay (in milliseconds) after a brick is generated.<br><br>" @
					"<font:arial bold:16>Brick Removal Delay<font:arial:14> - How long to delay (in milliseconds) after a brick is removed (useful for removing larger bricks that sometimes caused lag after removal).<br><br>" @
					"<font:arial bold:16>Generator Speed<font:arial:14> - The generator has built in delays that improves performance, by forcing functions to wait at least one frame before continuing - after each brick is planted or removed. However, the \"Generator Speed\" option allows you to speed up the routines by bypassing those delays.<br><br>" @
					"Increasing the generation speed by 8% will bypass the single-frame delay for each row of bricks that are planted; the delays mentioned above will still apply at times. Increasing the generation speed by 15% will bypass the delay altogether (per chunk); only the calculation delays mentioned above should apply with this option. Use with caution, especially if the speed is increased by 15% as there will be no delay for bricks which are removed or loaded from chunk saves / loaded builds. Increasing the generator speed increases the possibility of a server crash, however fail safes are included when loading bricks from chunk / build saves for every 250 bricks.<br><br>" @
					"Note: If you want the generator to run as quickly as possible, and both the server and your client can handle it, try settings all delays to \"0\", setting the generation speed to +15% and enabling network fast packets. Otherwise for better performance, try setting the speed to normal, disabling fast packets and increasing the delays.";

			PTG_Help_TxtCatSetList.setText(%text);
		
		//////////////////////////////////////////////////
		
		case "ChunkOptions":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Routines: Chunk Options Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Chunk Options\" segment of the \"Complex GUI: Routines\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"Options related to chunk objects.<br><br>" @
					"<font:arial bold:16>Force Bricks Into Chunks<font:arial:14> - If enabled, player bricks won't be planted unless within a chunk object that already exists (useful encase you want to prevent building outside of a predefined area).<br><br>" @
					"<font:arial bold:16>Automatically create new chunks for planted...<font:arial:14> - If Force Brick Into Chunks is enabled, and if a chunk object doesn't already exist for a planted brick, this option will automatically create a new chunk object for that brick.<br><br>" @ 
					"Note: This can cause areas of a landscape not to generate, since a chunk object would already exist due to the planted brick, and thus a new one wouldn't need to be calculated.<br><br>" @
					"<font:arial bold:16>Set Chunk to be \"Edited\" when a new brick...<font:arial:14> - Tags a chunk object as \"Edited\" if a player brick is planted within it, or if bricks from a .bls save are loaded within it (useful for saving player builds on the landscape).<br><br>" @
					"<font:arial bold:16>Set Chunk to be \"Edited\" when new wrench...<font:arial:14> - Tags a chunk object as \"Edited\" if wrench options or events are applied to a brick within that chunk.<br><br>" @
					"<font:arial bold:16>Set Chunk to be \"Edited\" when a brick is painted...<font:arial:14> - Tags a chunk object as \"Edited\" if a brick within that chunk is painted, given a print texture with the printer, or destroyed.<br><br>" @
					"<font:arial bold:16>Set Chunk to \"Static\" when a player...<font:arial:14> - Tags a chunk object as \"Static\" if a player plants a spawn brick within that chunk, or if a brick is loaded from a .bls file within it (useful for infinite terrain when you want to prevent chunks from being removed when vehicle spawns are planted within them).<br><br>" @
					"<font:arial bold:16>Load and apply static values from file...<font:arial:14> - Chunks can be tagged as either \"Static\" or \"Non-Static\"; Static chunks are not removed under certain conditions. When a chunk is saved to file, it also saves the static value with it; this option toggles whether or not to load and apply that static value to the chunk when loaded from file.<br><br>" @
					"<font:arial bold:16>Chunk Save Method<font:arial:14> - Defines when chunks are saved to file. \"if Edited\" only saves chunks that are tagged as \"Edited\". \"Always Save\" will save chunks under any conditions. \"Never Save\" will prevent saving chunks under any conditions.<br><br>" @
					"<font:arial bold:16>If saving a new chunk when chunk save limit...<font:arial:14> - Defines how the generator responds when the chunk save limit for the current seed or in general is reached. \"Remove Old Save\" will make room for the new chunk save by removing one of the older saves. \"Don't Save New File\" will instead prevent saving the new file.<br><br>";
					
			PTG_Help_TxtCatSetList.setText(%text);
		
		//////////////////////////////////////////////////
		
		case "RoutineMisc":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Routines: Miscellaneous Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Miscellaneous\" segment of the \"Complex GUI: Routines\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"Miscellaneous Routines options that don't fit into other categories.<br><br>" @
					"<font:arial bold:16>Generate / Load PTG Bricks Under Public Ownership<font:arial:14> - Generates terrain, details, etc. under public ownership (only applies to bricks that generate after this option is enabled and applied). Bricks loaded from chunk saves or upload builds will also be treated as public, if enabled.<br><br>" @
					"Note: When generated bricks are under public ownership, players can build on top of or underneath bricks without requiring trust from the host, however they can't be modified in any way nor destroyed - unless a player uses the admin / destructo wand. If this option is disabled, generated bricks are handled as if you planted them - players would need basic trust from you to build on / underneath them, and full trust to modify them.<br><br>" @
					"If public ownership is disabled for generated bricks, the restrictions mentioned below for destroying certain bricks will apply to them instead.<br><br>" @
					"<font:arial bold:16>Allow destroying detail bricks under public...<font:arial:14> - Toggles allowing detail bricks to be destroyed even if under public ownership; useful for allowing players to remove details - for clearing areas of terrain for building, when the public ownership option mentioned above is enabled.<br><br>" @
					"<font:arial bold:16>Load player bricks (from chunk saves) and ...<font:arial:14> - If enabled, player builds on the landscape (which are saved and reloaded from chunk saves) will be loaded under public ownership. Also, bricks generated from uploaded builds will also be included under public ownership. If disabled, both player bricks and bricks from uploaded builds will be included in the brickgroup of their owner; if their brickgroup doesn't exist, then they will be included in the host's brickgroup, and if the host's brickgroup doesn't exist, they will by default be included as public.<br><br>" @ 
					"<font:arial bold:16>Hide \"Ghosting\" Notification<font:arial:14> - Hides the ghosting notification window that appears when bricks are being ghosted - although still makes it slightly visible. This option is meant for infinite terrain, since constant generation and removal of bricks will cause this message to appear quite frequently.<br><br>" @
					"<font:arial bold:16>Allow PTG Console Echos<font:arial:14> - Toggles PTG messages that are echoed in the console (some messages will still appear, even if disabled).<br><br>" @
					"<font:arial bold:16>Prevent destroying detail bricks unless using...<font:arial:14> - Prevents players from destroying detail bricks unless they use the admin / destructo wand.<br><br>" @
					"<font:arial bold:16>Prevent destroying terrain unless using admin...<font:arial:14> - Prevents players from destroying terrain bricks unless they use the admin / destructo wand.<br><br>" @
					"<font:arial bold:16>Prevent destroying boundaries unless using admin...<font:arial:14> - Prevents players from destroying boundary bricks unless they use the admin / destructo wand.<br><br>" @
					"<font:arial bold:16>Allow Non-Host Build Uploading & Management<font:arial:14> - Toggles permissions for allowing non-host players to upload .bls saves (for the build loading feature), and to managed builds already uploaded to the server.<br><br>" @
					"Note: You can interrupt another player's build upload progress by simply unchecking this option and applying the changes.<br><br>" @ 
					"<font:arial bold:16>Allow Non-Host Settings Uploading<font:arial:14> - Toggles permissions for allowing non-host players to upload their GUI settings using the Complex or Simplex GUIs.<br><br>" @
					"<font:arial bold:16>Allow Non-Host Slash Command and Event Usage<font:arial:14> - Toggles permissions for allowing non-host players to use server chat commands and PTG events (if this option is disabled, players can still use the \"/PTGPos\" command, unless you disable permissions for that too).<br><br>" @
					"<font:arial bold:16>Allow any player to request their position using...<font:arial:14> - Toggles permissions for allowing players to use the \"/PTGPos\" chat command, which will notify the player of their current XYZ position, and the direction they're facing. If this option is enabled, any players, even if they're not administrators, can use this command.<br><br>" @
					
					"<font:arial bold:16>PTG Chat Message Font Size<font:arial:14> - The font size for PTG messages that appear in chat.<br><br>" @
					"<font:arial bold:16>Chunk Highlight (Static) Color<font:arial:14> - The color of highlighted static chunks (for the \"/PTGReveal\" chat command).<br><br>" @
					"<font:arial bold:16>Chunk Highlight (NonStatic) Color<font:arial:14> - The color of highlighted non-static chunks (for the \"/PTGReveal\" chat command).<br><br>" @
					
					"<font:arial bold:16>Save<font:arial:14> - Saves all settings for the Routines options window, and automatically reloads them into the GUI during the next game instance or when joining / starting a new server.<br><br>" @
					"<font:arial bold:16>Restore Defaults<font:arial:14> - Deletes the Routines settings save file (if present), and restores Routines to default settings.<br><br>" @
					"<font:arial bold:16>Apply Changes<font:arial:14> - Uploads and applies only your Routines settings to the server (settings can be uploaded even while a routine is running or while clearing chunks).<br><br>";

			PTG_Help_TxtCatSetList.setText(%text);
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////
		//Chunk Manager
		
		case "ChunkManagerGUI":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Chunk Manager GUI Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Chunk Manager GUI\" segment of the \"Introduction\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"Generated landscapes are handled in chunk objects, this GUI allows you to manage one or all of those objects at a time. In this GUI you can remove chunks, toggle static values, highlight chunks and display certain tagged fields for them.<br><br>" @
					"Note: The Chunk Manager GUI doesn't automatically refresh while open (for instance, if a certain chunk field shown in the GUI is changed), you have to close and reopen the window to update it.<br><br>" @
					"<font:arial bold:16>V More Options V<font:arial:14> - Expands the GUI window to list more options for all chunks, otherwise only options for the chunk you're in are shown.<br><br>" @
					"<font:arial bold:16>^ Less Options ^<font:arial:14> - Retracts the GUI window back to its original size.<br><br>";

			PTG_Help_TxtCatSetList.setText(%text);
			
		//////////////////////////////////////////////////
		
		case "RelativeChunk":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Chunk Manager: This Chunk Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Relative Chunk\" segment of the \"Chunk Manager GUI\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"Displays options and certain tagged fields for the current chunk your player object is within, if the object exists.<br><br>" @
					"<font:arial bold:16>Chunk Position<font:arial:14> - The position of the relative chunk, based on the bottom-left corner of the chunk object. This can also be used to determine the name of the chunk, which is saved as \"Chunk_X_Y\", with X and Y being it's position on the X and Y axis.<br><br>" @
					"<font:arial bold:16>Your Position<font:arial:14> - Your player's current position.<br><br>" @
					"<font:arial bold:16>Chunk Is Static<font:arial:14> - Displays if the chunk is tagged as \"Static\" or not; if a chunk object is static, that means it won't be removed under normal conditions (such as when removing chunks during infinite generator or when using the \"/PTGClear\" chat command).<br><br>" @
					"<font:arial bold:16>Chunk Is Stable<font:arial:14> - Displays if the chunk is tagged as \"Stable\" or not; chunks that are unstable mean they are currently being accessed by a script, and can't be accessed or modified by another script at the same time.<br><br>" @
					"<font:arial bold:16>Chunk Is Edited<font:arial:14> - Displays if the chunk is tagged as \"Stable\" or not; this is mainly used for saving chunks to file; you can define how a chunk becomes edited under Routines options.<br><br>" @
					"<font:arial bold:16>Bricks In Chunk<font:arial:14> - Shows how many bricks current exist in the chunk (doesn't take boundary bricks into account).<br><br>" @
					"<font:arial bold:16>Chunk Save Present<font:arial:14> - Shows if a save is present for this chunk object, relative to the seed and chunk size values (if a save is present, the generator will load bricks from it for this location during generation, rather than calculate that segment of the landscape).<br><br>" @
					"<font:arial bold:16>Toggle Static<font:arial:14> - Toggles the static value for the chunk object; this is the same as using the \"/PTGStatic\" slash command in chat.<br><br>" @
					"<font:arial bold:16>Clear Chunk<font:arial:14> - Removes the chunk object and all bricks within it, after waiting 5 seconds for all players to clear the area, and doesn't save bricks within the object once removed (no matter your settings for saving); using this command is the same as using the \"/PTGRemoveChunk 0\" slash command in chat.<br><br>" @
					"<font:arial bold:16>Clear w/ Save<font:arial:14> - Removes the chunk object and all bricks within it, after waiting 5 seconds for all players to clear the area, and erases the save file for the chunk (if it exists); same as using \"/PTGRemoveChunk 1\" in chat.<br><br>";

			PTG_Help_TxtCatSetList.setText(%text);
			
		//////////////////////////////////////////////////
		
		case "AllChunks":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Chunk Manager: All Chunks Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"All Chunks\" segment of the \"Chunk Manager GUI\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"Lists commands relating to all chunk objects that currently exist on the server, as well as the number of chunks present.<br><br>" @
					"<font:arial bold:16>Total saves for seed / chunk size<font:arial:14> - Chunk save files are saved according to the current seed and chunk size values; the amount of saves for the current seed / chunk size values is shown here.<br><br>" @
					"<font:arial bold:16>Toggle Static<font:arial:14> - Toggles the static values for all chunks; if a chunk object is static, that means it won't be removed under normal conditions (such as when removing chunks during infinite generator or when using \"/PTGClear\"). Note: This is the same as using the \"/PTGAllStatic\" slash command in chat.<br><br>" @
					"<font:arial bold:16>Clear<font:arial:14> - Clears all, non-static chunk objects and the bricks within them; same as using \"/PTGClear 1\" in chat. Note: Bricks won't be saved with this command after being removed, no matter your settings for saving.<br><br>" @
					"<font:arial bold:16>Clear & Save<font:arial:14> - Clears all, non-static chunk objects and the bricks within them; same as using \"/PTGClear 0\" in chat. Note: Bricks will be saved after being removed, depending on your settings for saving (in the Routines window).<br><br>" @
					"<font:arial bold:16>Save All<font:arial:14> - Saves all chunks that currently exist on the server, depending on your options for saving; same as using \"/PTGSave\" in chat.<br><br>" @
					"<font:arial bold:16>Clear All<font:arial:14> - Clears all chunk objects, even if static, and the bricks within them; same as using \"/PTGClearAll 1\" in chat. Note: Bricks won't be saved with this command after being removed, no matter your settings for saving.<br><br>" @
					"<font:arial bold:16>Clear All & Save<font:arial:14> - Clears all chunk objects, even if static, and the bricks within them; same as using \"/PTGClearAll 0\" in chat. Note: Bricks will be saved after being removed, depending on your settings for saving.<br><br>" @
					"<font:arial bold:16>Show / Hide<font:arial:14> - Toggles highlighting all present chunk objects by outlining them with the colors defined under Routines options; same as using \"/PTGReveal\" in chat. This is useful encase you need help visualizing how many chunks are present, or how large each chunk is.<br><br>" @ 
					"Note: Different colors are used for static and non-static chunks when they're being highlighted. The highlight objects also automatically adjust their height when new bricks are planted / loaded from .bls files, and are dynamically created / destroyed with the chunks they're outlining.<br><br>" @
					"Highlight objects for chunks consist of static shapes, so having too many on the server at the same time can cause lag; use this feature with caution.<br><br>" @ 
					"<font:arial bold:16>Purge<font:arial:14> - Clears all existing chunks, even if static, and removes any chunk saves for them if present; same as using \"/PTGPurge\" in chat.<br><br>" @ 
					"<font:arial bold:16>Clear All Saves<font:arial:14> - Clears all chunk saves for this particular seed and chunk size; same as using \"/PTGClearAllSaves\" in chat (if you want to clear all chunk saves for all seed values, you can enter \"/PTGClearAllSaves AllSaves\" in chat, instead).<br><br>";

			PTG_Help_TxtCatSetList.setText(%text);
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////
		//Simplex GUI
		
		case "SimplexGUI":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Simplex GUI Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Simplex GUI\" segment of the \"Introduction\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"The Simplex GUI is similar to the Complex (or Main) GUI, but leaves out the detail options for the various features (in short the generator takes care of the detailed options for you when using this GUI). It's much easier to use, especially for those just starting to use the add-on, and comes pre-packaged with various biomes you can choose from.<br><br>";
					
			PTG_Help_TxtCatSetList.setText(%text);
			
		//////////////////////////////////////////////////
		
		case "SimplexMain":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Simplex GUI: Main Options Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Main Options\" segment of the \"Simplex GUI\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"<font:arial bold:16>Seed<font:arial:14> - A value between -9999999 and 99999999 which changes how the landscape generates (seed values of 1 and 2 will both generate completely different landscapes).<br><br>" @
					"<font:arial bold:16>Random<font:arial:14> - Randomly chooses a seed value for you.<br><br>" @
					"<font:arial bold:16>Terrain / Clouds / Floating Islands<font:arial:14> - Allows you to set different brick sizes and types for default terrain, clouds and floating islands. Note: Choosing a Modular Terrain brick for any of these features will automatically generate the ModTer ramp bricks with them (The ModTer type for all three defaults to the \"Cubes & Ramps\" option, generating ModTer as it's normally used).<br><br>" @
					"<font:arial bold:16>Infinite Terrain<font:arial:14> - Enables the infinite terrain option, which generates chunks of bricks relative to players, instead of using the Terrain Width and Terrain Length options.<br><br>" @ 
					"If infinite terrain is enabled, the landscape will generate in a circular radius of three chunks around normal players, and five chunks around super administrators. Chunks that exceed a certain distance from all players are also removed - to help keep the brick count and lag down to a minimum.<br><br>" @
					"<font:arial bold:16>Edge Falloff<font:arial:14> - Forces bricks near the edge of the landscape to gradually descend toward ground level; this can also be used to close up seams at the edges of skylands, clouds and floating islands. Note: This option only works when finite terrain is disabled.<br><br>" @
					"<font:arial bold:16>Terrain Length on Y-Axis<font:arial:14> - How long (in meters) the generated, finite landscape will be.<br><br>" @
					"<font:arial bold:16>Terrain Width on X-Axis<font:arial:14> - How wide (in meters) the generated, finite landscape will be.<br><br>" @
					"<font:arial bold:16>Terrain Type<font:arial:14> - Contains 4 different options for generating terrain. \"Normal Terrain\" generates terrain as it normally would appear. \"Flatlands\" generates completely flat terrain (other features such as Mountains will still generate normally, however).<br><br>" @ 
					"\"Skylands\" is like normal terrain, except it only generates terrain above a certain section cut height, copies and inverts what is generated to add a bottom layer, and then randomly displaces the height of the resultant terrain.<br><br>";
					
			PTG_Help_TxtCatSetList.setText(%text);
			
		//////////////////////////////////////////////////
		
		case "SimplexFeatures":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Simplex GUI: Features Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Additional Features\" segment of the \"Simplex GUI\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"The checkboxes provided in this portion of the GUI allow you to toggle various features to be added to the final, generated landscape.<br><br>";

			PTG_Help_TxtCatSetList.setText(%text);
			
		//////////////////////////////////////////////////
		
		case "SimplexBiomes":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Simplex GUI: Optional Biomes Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Optional Biomes\" segment of the \"Simplex GUI\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"The checkboxes provided in this portion of the GUI allow you to toggle various biomes to be generated in the final landscape. Biomes are areas of terrain that are unique to the other areas around it, and consist of its own colors and prints, detail bricks, water brick effects and terrain height. The default biome affects terrain in general, the other custom biome options allow you to overlay other biomes on top of the default terrain.<br><br>" @
					"Note: Certain biomes (such as \"Plains\") modify the overall height of terrain by scaling it lower or higher than it normally generates. These biomes are best to be used for the default option due to how they work, but you can still use them for the custom biomes as well (however, the height of terrain within those biomes may not always generate as intended).<br><br>";

			PTG_Help_TxtCatSetList.setText(%text);
		
		//////////////////////////////////////////////////
		
		case "SimplexControls":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Simplex GUI: Controls Help");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Generator Controls\" segment of the \"Simplex GUI\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"Button controls for the generator.<br><br>" @
					"<font:arial bold:16>Apply & Start<font:arial:14> - Uploads your Simplex GUI settings to the server and immediately begins generation afterwards (you can also enter \"/PTGStart\" in chat to start generation using the last settings sent to the server).<br><br>" @
					"<font:arial bold:16>Halt<font:arial:14> - Stops generation or stops clearing terrain - if chunks are being cleared; same as using the \"/PTGHalt\" slash command in chat.<br><br>" @
					"<font:arial bold:16>Clear<font:arial:14> - Clears all generated bricks and chunks; same as using the \"/PTGClear\" slash command in chat.<br><br>" @
					"<font:arial bold:16>Close<font:arial:14> - Closes the GUI.<br><br>";

			PTG_Help_TxtCatSetList.setText(%text);
			
		////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////
		//Mass Details GUI
		
		case "MassBiomeDetails":
		
			PTG_Help_TxtCatName.setText("<font:impact:33><color:ffffff>Mass Biome Details GUI");
			PTG_Help_TxtVidLinkInfo.setText("<font:arial bold:14>For more information, check out the \"Msss Biome Details GUI\" segment of the \"Introduction\" YouTube video tutorial for PTG <font:arial:14>(click the image to the left to open the video in your default browser).");
			//PTG_Help_BmpVidLink.command = "";
			
			%text =	"Allows setting up numerous detail bricks per biome, with a hard limit of 400 for each biome.<br><br>" @
					"Note: Common, Uncommon and Rare placement ratios (that are included when setting up details under Biomes options for the Complex GUI) don't apply to details in this GUI, so if you want to increase the frequency of certain bricks, make sure to add them in multiple times. Also, make sure the \"Use mass details for this biome\" checkbox is selected for every biome you want to use this option for; you can switch between the different biomes by using the popup menu included in the GUI.<br><br>" @
					"<font:arial bold:16>Brick, Print and Color<font:arial:14> - Allows you to set up the brick datablock, print (if the brick supports prints) and color for the next brick that can be added to the list.<br><br>" @
					"<font:arial bold:16>Remove Selected<font:arial:14> - Removes the brick currently selected from the details list, for that particular biome.<br><br>" @
					"<font:arial bold:16>Add New<font:arial:14> - Adds a new brick to the details list, for that particular biome - based on the brick, print and color options that are currently set up.<br><br>" @
					"<font:arial bold:16>Clear<font:arial:14> - Removes all details from the list, for that particular biome.<br><br>" @
					"<font:arial bold:16>Clear All<font:arial:14> - Removes all details from all 9 biome detail lists.<br><br>" @
					"<font:arial bold:16>Use mass details for this biome<font:arial:14> - Toggles between using the mass details option for the biome selected (if the checkbox is selected), or between using standard biomes options in the Complex GUI. Note: selecting different biomes will also display the checkbox relative to that biome only.<br><br>";

			PTG_Help_TxtCatSetList.setText(%text);
		
	}
	
	canvas.PushDialog(PTG_Help);
}