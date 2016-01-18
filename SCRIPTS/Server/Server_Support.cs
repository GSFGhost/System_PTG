//Create necessary datablocks for boundary and highlighted chunk static shapes
if(!isObject(PTGBoundsWallData))
{
	datablock StaticShapeData(PTGBoundsWallData)
	{
		shapeFile = "Add-Ons/System_PTG/SHAPES/BoundsWall.dts";
	};
}
if(!isObject(PTGBoundsCeilData))
{
	datablock StaticShapeData(PTGBoundsCeilData)
	{
		shapeFile = "Add-Ons/System_PTG/SHAPES/BoundsCeil.dts";
	};
}
if(!isObject(PTGHighlightData))
{
	datablock StaticShapeData(PTGHighlightData)
	{
		shapeFile = "Add-Ons/System_PTG/SHAPES/Highlight.dts";
	};
}
if(!isObject(PTGHighlightRmvData))
{
	datablock StaticShapeData(PTGHighlightRmvData)
	{
		shapeFile = "Add-Ons/System_PTG/SHAPES/HighlightRmv.dts";
	};
}

//////////////////////////////////////////////////

if(ForceRequiredAddOn("Brick_Large_Cubes") == $Error::AddOn_NotFound) //required for water bricks
	echo("\c2>>PTG ERROR: Brick_Large_Cubes - required add-on \"Brick_Large_Cubes\" not found!");
else
{
	exec("Add-Ons/Brick_Large_Cubes/Server.cs");
	echo("\c4>>\c2P\c1T\c4G: \c0Force-enabled required add-on \c4Brick_Large_Cubes");
}

//////////////////////////////////////////////////

if($AddOn__Brick_ModTer_BasicPack == 1 && $AddOn__Brick_ModTer_InvertedPack != 1)
{
	if(ForceRequiredAddOn("Brick_ModTer_InvertedPack") == $Error::AddOn_NotFound) //required for water bricks
		echo("\c2>>PTG ERROR: Brick_ModTer_InvertedPack - required add-on \"Brick_ModTer_InvertedPack\" not found! The ModTer Inverted bricks are required when the Basic bricks are enabled.");
	else
	{
		exec("Add-Ons/Brick_ModTer_InvertedPack/Server.cs");
		echo("\c4>>\c2P\c1T\c4G: \c0Force-enabled required add-on \c4Brick_ModTer_InvertedPack\c0, since Basic Pack is enabled.");
		$PTG_ModTerInvForce = true;
	}
}

//////////////////////////////////////////////////

if(ForceRequiredAddOn("Particle_Basic") == $Error::AddOn_NotFound) //required for water type particles
	echo("\c2>>PTG ERROR: Particle_Basic - required add-on \"Particle_Basic\" not found");
else
{
	exec("Add-Ons/Particle_Basic/Server.cs");
	echo("\c4>>\c2P\c1T\c4G: \c0Force-enabled required add-on \c4Particle_Basic");
}

//////////////////////////////////////////////////

//if((%error = ForceRequiredAddOn("Print_ModTer_Default")) == $Error::AddOn_NotFound)
//	error("ERROR: Print_ModTer_Default - required add-on \"Print_ModTer_Default\" not found");
//else
//{
//	exec("Add-Ons/Print_ModTer_Default/Server.cs");
//	echo("\c2P\c1T\c4G: \c0Force-enabling required add-on \c4Print_ModTer_Default");
//}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//// REMOTELY EXECUTE FUNCTIONS THROUGH EXTERNAL CONSOLE WINDOW ////
function PTGRmt(%function,%var)
{
	//if($PTG.lastClientID $= "" && %function !$= "Help" && %function !$= "List")
	//{
	//	echo("\c2>>P\c1T\c4G \c2ERROR: \"PTGRmt\" failed; no past client data detected! Make sure you join your server, upload your settings and start a routine first. [!] \c0->" SPC getWord(getDateTime(),1));
	//	return;
	//}
	
	switch$(%function)
	{
		case "Help":
		
			echo("\c2P\c1T\c4G Remote Function Help:");
			echo(" ");
			echo("     This function allows you to easily execute PTG server commands remotely through the console, and is meant for managing dedicated servers without having to join or be present on your server.");
			echo("     Type  PTGRmt(\"List\");  for a list of all available functions.");
			
		case "List":
		
			echo("\c2P\c1T\c4G Remote Function Command List:");
			echo(" ");
			echo(">     \c4PTGRmt(\"Start\");\c0 - resumes generation of a finite or infinite routine.");
			echo(" ");
			echo(">     \c4PTGRmt(\"Halt\");\c0 - stops the current routine that's running (including clearing routines).");
			echo(" ");
			echo(">     \c4PTGRmt(\"Clear\",\c2%bool\c4);\c0 - clears all stable, non-static chunks. If %Bool = 1, no removed chunks will be saved, no matter your saving method selected; otherwise they will be.");
			echo(" ");
			echo(">     \c4PTGRmt(\"ClearAll\",\c2%bool\c4);\c0 - exactly the same as /PTGClear, except it also clears static chunks as well.");
			echo(" ");
			echo(">     \c4PTGRmt(\"ClearSpam\",\c2%id\c4);\c0 - clears all bricks in all existing chunks planted by the player blockland id: %id");
			echo(" ");
			echo(">     \c4PTGRmt(\"ClearAllSaves\",\c2%clearMethod\c4);\c0 -  clears all chunks saves depending on the method you define. If %clearMethod = \"AllSaves\", all chunk saves will be removed. If %clearMethod = \"ForSeed\" or is left blank, only saves for the current seed value will be removed.");
			echo(" ");
			echo(">     \c4PTGRmt(\"Purge\");\c0 - clears all stable chunks, even if static, and also removes any present saves for those chunks.");
			echo(" ");
			echo(">     \c4PTGRmt(\"Count\");\c0 - Lists all currently existing chunk objects and other info about them.");
			echo(" ");
			echo(">     \c4PTGRmt(\"AllStatic\",\c2%bool\c4);\c0 - sets all existing chunks to the static value you set. If %bool = 1, all chunks will be set to \"Static\", otherwise they will be \"Non-Static\".");
			echo(" ");
			echo(">     \c4PTGRmt(\"Save\");\c0 - Saves all existing chunks, based on the chunk save method you set in the Routines window.");
			echo(" ");
			echo(">     \c4PTGRmt(\"Reveal\");\c0 - Highlights all chunk objects on the server, based on their relative height, and displays their static value and .");
			echo(" ");
			echo(">     \c4PTGRmt(\"Preset\",\c2%fields\c4);\c0 - Loads server presets and applies the settings to the server. For more information, check out the Help.txt file in Add-Ons/System_PTG.zip");
			echo(" ");
			echo(">     \c4ListClients();\c0 - This isn't actually a PTG function, but it's still a very useful default function that lists the current players / clients on your server.");
		
		case "Start":
		
			$PTG_SekKeyDedRmt = true;
			SERVERCMDPTGStart("REMOTE");
			
		case "Halt":
		
			$PTG_SekKeyDedRmt = true;
			SERVERCMDPTGHalt("REMOTE");
			
		case "Clear":
		
			$PTG_SekKeyDedRmt = true;
			SERVERCMDPTGClear("REMOTE",%var);
			
		case "ClearAll":
		
			$PTG_SekKeyDedRmt = true;
			SERVERCMDPTGClearAll("REMOTE",%var);
			
		case "ClearSpam":
		
			$PTG_SekKeyDedRmt = true;
			SERVERCMDPTGClearSpam("REMOTE",%var);
			
		case "ClearAllSaves":
		
			$PTG_SekKeyDedRmt = true;
			SERVERCMDPTGClearAllSaves("REMOTE",%var);
			
		case "Purge":
		
			$PTG_SekKeyDedRmt = true;
			SERVERCMDPTGPurge("REMOTE");
			
		case "Count":
		
			$PTG_SekKeyDedRmt = true;
			SERVERCMDPTGCount("REMOTE");
			
		case "AllStatic":
		
			$PTG_SekKeyDedRmt = true;
			SERVERCMDPTGAllStatic("REMOTE",%var);
			
		case "Save":
		
			$PTG_SekKeyDedRmt = true;
			SERVERCMDPTGSave("REMOTE");
			
		case "Reveal":
		
			$PTG_SekKeyDedRmt = true;
			SERVERCMDPTGReveal("REMOTE");
			
		case "Preset":
		
			//$PTG_SekKeyDedRmt = true;
			%fpPreset = getField(%var,0);
			%noFallDamage = getField(%var,1);
			%autoHalt = getField(%var,2);
			%autoClearFunc = getField(%var,3);
			%autoReset = getField(%var,4);
			
				if(%restart = getField(%var,5))
					%pass = "Restart";
			
			PTG_LoadServerPreset(%fpPreset,%noFallDamage,%autoHalt,%autoClearFunc,%autoReset,%pass);
			
		default:
		
			if($PTG_SekKeyDedRmt)
				deleteVariables("$PTG_SekKeyDedRmt");
	
			echo("\c4>>\c2P\c1T\c4G \c2ERROR: Invalid Function\c0 - Enter  PTGRmt(\"List\");  for a list of all available functions you can call remotely.");
			return;
	}
	
	if(%function !$= "Help" && %function !$= "List")
	{
		if(isObject(ServerConnection)) //if the function is called by a super admin on a normal server (usually doesn't happen)
			%txtMod = " or a super admin";
		if((%fontSz = $PTG.fontSize) == 0) //if font size hasn't been received / set up yet, use default size
			%fontSz = 18;
		
		messageAll('',"<font:Verdana Bold:" @ %fontSz @ ">\c0P\c3T\c1G: \c6The server host" @ %txtMod @ " remotely executed the \"" @ %function @ "\" function through the console. \c8[Console]");
		echo("\c4>>\c2P\c1T\c4G: \"" @ %function @ "\" function was remotely executed successfully.");
	}
	
	if($PTG_SekKeyDedRmt)
		deleteVariables("$PTG_SekKeyDedRmt");
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_HostCheck(%cl) //might not work on dedicated lan or multiplayer lan servers (?)
{
	if(isObject(%cl))
	{
		if(%cl.isLocalConnection() || %cl.bl_id == getNumKeyID())
			return true;
		else
			return false;
	}
	else
		return false;
	
	//else
	//{
	//	if($PTG.lastClientisLocal || $PTG.lastClientID == getNumKeyID())
	//		return true;
	//	else
	//		return false;
	//}
}


////////////////////////////////////////////////////////////////////////////////////////////////////

//// REQUEST FILEPATHS ////
function PTG_GetFP(%type,%fileName,%rot,%relGrid)
{
	switch$(%type)
	{
		//Chunk Saves
		case "Chunk-Norm":
			%fp = "Config/Server/PTGv3/ChunkCache/Seed_" @ getSubStr($PTGm.seed,0,8) @ "/ChunkSize_" @ mClamp($PTGm.chSize,16,256) @ "/Normal_Saves/" @ %fileName @ ".txt";
		case "Chunk-Perm":
			%fp = "Config/Server/PTGv3/ChunkCache/Seed_" @ getSubStr($PTGm.seed,0,8) @ "/ChunkSize_" @ mClamp($PTGm.chSize,16,256) @ "/Permanent_Saves/" @ %fileName @ ".txt";
		case "ChunkSeed":
			%fp = "Config/Server/PTGv3/ChunkCache/Seed_" @ getSubStr($PTGm.seed,0,8) @ "/*/*/*.txt"; // */???
		case "ChunkCache":
			%fp = "Config/Server/PTGv3/ChunkCache/*/*/*/*.txt";
		case "ChunkSeed-NonPerm":
			%fp = "Config/Server/PTGv3/ChunkCache/Seed_" @ getSubStr($PTGm.seed,0,8) @ "/*/Normal_Saves/*.txt"; //for accessing non permanent chunk saves only (i.e. for removing old saves)
		case "Info-PermFldr":
			%fp = "Config/Server/PTGv3/ChunkCache/Seed_" @ getSubStr($PTGm.seed,0,8) @ "/ChunkSize_" @ mClamp($PTGm.chSize,16,256) @ "/Permanent_Saves/Info.ptg";
		case "PermFldr":
			%fp = "Config/Server/PTGv3/ChunkCache/Seed_" @ getSubStr($PTGm.seed,0,8) @ "/ChunkSize_" @ mClamp($PTGm.chSize,16,256) @ "/Permanent_Saves/";
	
		//Build Saves
		case "Build":
			%fp = "Config/Server/PTGv3/BrSaveCache/RelGridSize_" @ %relGrid @ "/" @ %fileName @ "/" @ %rot @ ".txt"; //!!!build structures and build details!!!
		case "BuildCache":
			%fp = "Config/Server/PTGv3/BrSaveCache/*/*/Info.txt";
		case "RelGridFldr":
			%fp = "Config/Server/PTGv3/BrSaveCache/RelGridSize_" @ %relGrid @ "/*/Info.txt"; //???
		case "BuildInfo":
			%fp = "Config/Server/PTGv3/BrSaveCache/RelGridSize_" @ %relGrid @ "/" @ %fileName @ "/Info.txt";
		
		//Other
		case "Download":
			%fp = "<linkColor:ffffff><a:forum.blockland.us/index.php?topic=281995.0>\c0Procedural \c3Terrain \c1Generator</a>";
		case "ServerPresetFP":
			%fp = "Config/Server/PTGv3/ServerPresets/";
		case "NewDefaultDirFP":
			%fp = "Config/Server/PTGv3/";
	}
	
	return %fp;
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_MsgClient(%cl,%icon,%title,%text)
{
	if(isObject(%cl))
	{
		if(%cl.PTGver == 3)
			commandToClient(%cl,'PTG_ReceiveMsg',%icon,%title,%text);
		else
			commandtoClient(%cl,'messageboxOK',%title,%text);
	}
	
	//If no client exists when messaging client, but client being messaged is host (or if host var not yet set up), message instead in console
		//prevents echoing messages in console for other players if they are not on the server to receive the message
	else if(($PTG_HostCl > 0 && %cl == $PTG_HostCl) || $PTG_HostCl $= "")
	{
		switch$(%icon)
		{
			case "Denied": 
				echo("\c2>>P\c1T\c4G \c2DENIED: \c0" @ %text @ " \c2[!] \c0->" SPC getWord(getDateTime(),1));
			case "Failed": 
				echo("\c2>>P\c1T\c4G \c2FAILED: \c0" @ %text @ " \c2[!] \c0->" SPC getWord(getDateTime(),1));
			case "Error": 
				echo("\c2>>P\c1T\c4G \c2ERROR: \c0" @ %text @ " \c2[!] \c0->" SPC getWord(getDateTime(),1));
			case "Success": 
				echo("\c4>>\c2P\c1T\c4G \c4Success: \c0" @ %text @ " ->" SPC getWord(getDateTime(),1));
			case "Info": 
				echo("\c4>>\c2P\c1T\c4G \c4Info: \c0" @ %text @ " ->" SPC getWord(getDateTime(),1));
			default:
				echo("\c4>>\c2P\c1T\c4G: \c0" @ %text @ " ->" SPC getWord(getDateTime(),1));
		}
	}
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_ErrorCheck(%cl)
{
	if(!isObject(PTG_MainSO_tmp) && isObject(PTG_MainSO))
		%so = PTG_MainSO;
	else
		%so = PTG_MainSO_tmp;
	
	//Grid Size Check (Finite Terrain)
	if(%so.genType $= "Finite")
	{
		if(%so.gridStartX >= %so.gridEndX)
		{
			PTG_MsgClient(%cl,"Failed","PTG ERROR: Incorrect Grid Size","The Grid start position for the X-Axis must be < the grid end position for X.");
			%fail = true;
		}
		if(%so.gridStartY >= %so.gridEndY)
		{
			PTG_MsgClient(%cl,"Failed","PTG ERROR: Incorrect Grid Size","The Grid start position for the Y-Axis must be < the grid end position for Y.");
			%fail = true;
		}
		
		if(%fail && !isObject(%cl)) //for external console when hosting a dedicated server
			echo("\c2>>P\c1T\c4G \c2ERROR: Grid size check failed; routine start aborted! [!] \c0->" SPC getWord(getDateTime(),1));
	}
	
	//Chunk Size Check
	if(!%fail)
	{
		if(%so.chSize <= %so.brTer_XYsize)
		{
			PTG_MsgClient(%cl,"Failed","PTG ERROR: Invalid Chunk Size","The chunk size you have chosen must be > the terrain brick size; please choose a larger chunk size.");
			%fail = true;
		}
		if(%so.chSize <= %so.brClouds_XYsize)
		{
			PTG_MsgClient(%cl,"Failed","PTG ERROR: Invalid Chunk Size","The chunk size you have chosen must be > the clouds brick size; please choose a larger chunk size.");
			%fail = true;
		}
		if(%so.chSize <= %so.brFltIslds_XYsize)
		{
			PTG_MsgClient(%cl,"Failed","PTG ERROR: Invalid Chunk Size","The chunk size you have chosen must be > the floating islands brick size; please choose a larger chunk size.");
			%fail = true;
		}
	}
	
	//Datablocks Check
	if(!%fail)
	{
		if(!isObject(%so.brTer_DB))
		{
			if(%so.enabModTer)
				PTG_MsgClient(%cl,"Failed","PTG ERROR: Invalid Brick Datablock","Brick datablock for \"Terrain\" doesn't exist! Either the 4x ModTer pack or both the Basic and Inv ModTer packs are disabled.");
			else
				PTG_MsgClient(%cl,"Failed","PTG ERROR: Invalid Brick Datablock","Brick datablock for \"Terrain\" doesn't exist!");
			
			%fail = true; //what if multiple error check fails?
		}
		if(%so.enabClouds && !isObject(%so.brClouds_DB))
		{
			if(%so.enabModTer_Clouds)
				PTG_MsgClient(%cl,"Failed","PTG ERROR: Invalid Brick Datablock","Brick datablock for \"Clouds\" doesn't exist! Either the 4x ModTer pack or both the Basic and Inv ModTer packs are disabled.");
			else
				PTG_MsgClient(%cl,"Failed","PTG ERROR: Invalid Brick Datablock","Brick datablock for \"Clouds\" doesn't exist!");
			
			%fail = true;
		}
		if(%so.enabFltIslds && !isObject(%so.brFltIslds_DB))
		{
			if(%so.enabModTer_FltIslds)
				PTG_MsgClient(%cl,"Failed","PTG ERROR: Invalid Brick Datablock","Brick datablock for \"Floating Islands\" doesn't exist! Either the 4x ModTer pack or both the Basic and Inv ModTer packs are disabled.");
			else
				PTG_MsgClient(%cl,"Failed","PTG ERROR: Invalid Brick Datablock","Brick datablock for \"Floating Islands\" doesn't exist!");
			
			%fail = true;
		}
		
		if(%fail && !isObject(%cl)) //for external console when hosting a dedicated server
			echo("\c2>>P\c1T\c4G \c2ERROR: Brick datablocks check failed; routine start aborted! [!] \c0->" SPC getWord(getDateTime(),1));
	}
	
	//Biome (Relative to Terrain) Scales Check
	if(!%fail)
	{
		if(%so.enabBio_CustA && %so.bio_CustA_itrA_XY < %so.chSize) //custom biome A
		{
			PTG_MsgClient(%cl,"Failed","PTG ERROR: Incorrect Biome XY Scale","The biome XY noise scale for \"Custom Biome A\" must be >= the chunk size!");
			%fail = true;
		}
		if(%so.enabBio_CustB && %so.bio_CustB_itrA_XY < %so.chSize) //custom biome B
		{
			PTG_MsgClient(%cl,"Failed","PTG ERROR: Incorrect Biome XY Scale","The biome XY noise scale for \"Custom Biome B\" must be >= the chunk size!");
			%fail = true;
		}
		if(%so.enabBio_CustC && %so.bio_CustC_itrA_XY < %so.chSize) //custom biome C
		{
			PTG_MsgClient(%cl,"Failed","PTG ERROR: Incorrect Biome XY Scale","The biome XY noise scale for \"Custom Biome C\" must be >= the chunk size!");
			%fail = true;
		}
		
		if(%fail && !isObject(%cl)) //for external console when hosting a dedicated server
			echo("\c2>>P\c1T\c4G \c2ERROR: Biome scales check failed; routine start aborted! [!] \c0->" SPC getWord(getDateTime(),1));
	}
	
	//Noise Scales Check
	if(!%fail)
	{
		if(%so.ter_itrA_XY < %so.ter_itrB_XY || %so.ter_itrA_XY < %so.chSize || %so.ter_itrB_XY < %so.chSize || %so.ter_itrC_XY > %so.chSize) //terrain
		{
			PTG_MsgClient(%cl,"Failed","PTG ERROR: Incorrect Noise Scales","XY noise scales for \"Terrain\" must follow the following rule: <color:ff0000>Iteration A >= Iteration B >= Chunk Size >= Iteration C<color:000000>.<br><br>Example: 256 >= 64 >= 32 >= 16");
			%fail = true;
		}
		if(%so.enabMntns && (%so.mntn_itrA_XY < %so.mntn_itrB_XY || %so.mntn_itrA_XY < %so.chSize || %so.mntn_itrB_XY < %so.chSize)) //mountains
		{
			PTG_MsgClient(%cl,"Failed","PTG ERROR: Incorrect Noise Scales","XY noise scales for \"Mountains\" must follow the following rule: <color:ff0000>Iteration A >= Iteration B >= Chunk Size<color:000000>.<br><br>Example: 256 >= 64 >= 32");
			%fail = true;
		}
		if(%so.enabCaves)
		{
			if(%so.caveA_itrA_XY < %so.caveA_itrB_XY || %so.caveA_itrA_XY < %so.chSize || %so.caveA_itrB_XY < %so.chSize || %so.caveA_itrC_XY > %so.chSize) //caves
			{
				PTG_MsgClient(%cl,"Failed","PTG ERROR: Incorrect Noise Scales","XY noise scales for \"Caves\" must follow the following rule: <color:ff0000>Iteration A >= Iteration B >= Chunk Size >= Iteration C<color:000000>.<br><br>Example: 128 >= 64 >= 32 >= 32");
				%fail = true;
			}
			if(%so.caveB_itrA_XY < %so.chSize) //caves height mod
			{
				PTG_MsgClient(%cl,"Failed","PTG ERROR: Incorrect Noise Scales","XY noise scales for \"Caves Height Mod\" must follow the following rule: <color:ff0000>Iteration A >= Chunk Size<color:000000>.<br><br>Example: 256 >= 32");
				%fail = true;
			}
		}
		if((%so.enabBio_CustA && %so.bio_CustA_itrA_XY < %so.ter_itrA_XY) || (%so.enabBio_CustB && %so.bio_CustB_itrA_XY < %so.ter_itrA_XY) || (%so.enabBio_CustC && %so.bio_CustC_itrA_XY < %so.ter_itrA_XY))//custom biomes
		{
			PTG_MsgClient(%cl,"Failed","PTG ERROR: Incorrect Noise Scales","XY noise scales for \"Custom Biomes\" must follow the following rule: <color:ff0000>Biome Iteration A >= Terrain Iteration A<color:000000>.<br><br>Example: 512 >= 256");
			%fail = true;
		}
		if(%so.terType $= "Skylands" && %so.skyLnds_itrA_XY < %so.chSize) //skylands terrain height mod
		{
			PTG_MsgClient(%cl,"Failed","PTG ERROR: Incorrect Noise Scales","XY noise scales for \"Skylands Height Mod\" must follow the following rule: <color:ff0000>Iteration A >= Chunk Size<color:000000>.<br><br>Example: 128 >= 32");
			%fail = true;
		}
		if(%so.enabClouds && (%so.clouds_itrA_XY < %so.clouds_itrB_XY || %so.clouds_itrA_XY < %so.chSize || %so.clouds_itrB_XY < %so.chSize)) //clouds
		{
			PTG_MsgClient(%cl,"Failed","PTG ERROR: Incorrect Noise Scales","XY noise scales for \"Clouds\" must follow the following rule: <color:ff0000>Iteration A >= Iteration B >= Chunk Size<color:000000>.<br><br>Example: 128 >= 32 >= 32");
			%fail = true;
		}
		if(%so.enabFltIslds && (%so.fltIslds_itrA_XY < %so.fltIslds_itrB_XY || %so.fltIslds_itrA_XY < %so.chSize || %so.fltIslds_itrB_XY < %so.chSize)) //floating islands
		{
			PTG_MsgClient(%cl,"Failed","PTG ERROR: Incorrect Noise Scales","XY noise scales for \"Floating Islands\" must follow the following rule: <color:ff0000>Iteration A >= Iteration B >= Chunk Size<color:000000>.<br><br>Example: 64 >= 32 >= 32");
			%fail = true;
		}
		
		if(%fail && !isObject(%cl)) //for external console when hosting a dedicated server
			echo("\c2>>P\c1T\c4G \c2ERROR: Noise scales check failed; routine start aborted! [!] \c0->" SPC getWord(getDateTime(),1));		
	}
	
	//////////////////////////////////////////////////
	
	if(isObject(%cl))
		%cl.PTGupldSecKey = "";
	
	if(!%fail)
	{
		if(PTG_HostCheck(%cl))
		{
			if(isObject(PTG_GlobalSO_tmp))
			{
				if(isObject(PTG_GlobalSO))
					PTG_GlobalSO.delete();
				PTG_GlobalSO_tmp.setName("PTG_GlobalSO");
				$PTG = PTG_GlobalSO;
				
				%newSet = true;
				%newSetGbl = true;
			}
		}
		if(isObject(PTG_MainSO_tmp))
		{
			if(isObject(PTG_MainSO))
				PTG_MainSO.delete();
			PTG_MainSO_tmp.setName("PTG_MainSO");
			$PTGm = PTG_MainSO;
			
			%newSet = true;
		}
		if(isObject(PTG_BiomesSO_tmp))
		{
			if(isObject(PTG_BiomesSO))
				PTG_BiomesSO.delete();
			PTG_BiomesSO_tmp.setName("PTG_BiomesSO");
			$PTGbio = PTG_BiomesSO;
			
			%newSet = true;
			
		}
		
		$PTG_init = true;
		//if(isObject(%cl))
		//	%cl.PTGupldSecKey = "";
		
		if(!isObject(%cl))
		{
			%name = "Unknown";
			%id = "??????";
		}
		else
		{
			%name = %cl.name;
			%id = %cl.bl_id;
			
			if(PTG_HostCheck(%cl) && %newSetGbl)
				%txtAdd = " & routine";
		}
		
		if(%newSet)
		{
			messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 \"" @ %name @ "\" uploaded new server" @ %txtAdd @ " settings <color:000000>[^]"); //players will be notified of newly uploaded settings if no issues come up during upload
			if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %name @ " (" @ %id @ ") \c0uploaded new server" @ %txtAdd @ " settings. \c4[^] \c0->" SPC getWord(getDateTime(),1));
		}
		
		$PTG.uploadingSettings = false;
		return true;
	}
	else
	{
		if(isObject(PTG_GlobalSO_tmp)) 
			PTG_GlobalSO_tmp.delete();
		if(isObject(PTG_MainSO_tmp)) 
			PTG_MainSO_tmp.delete();
		if(isObject(PTG_BiomesSO_tmp)) 
			PTG_BiomesSO_tmp.delete();
		
		if(isObject(%cl))
			%cl.PTGupldSecKey = "";
	
		$PTG.uploadingSettings = false;
		return false;
	}
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_FindClosestColor(%oldColorStr,%action)
{
	switch$(%action)
	{
		//Convert RGBA value to closest, existing RGBA value
		case "RGBA-RGBA":
		
			%exctMatch = false;
			%resMin = 2.0;
			
			for(%d = 0; %d < 64 && !%exctMatch; %d++)
			{
				%newColorStr = getColorIDTable(%d);
				
				if(%oldColorStr !$= %newColorStr)
				{
					%tmpMin = vectorDist(%newColorStr,%oldColorStr);
					%transCond = mabs(getWord(%newColorStr,3) - getWord(%oldColorStr,3)) < 0.3;  //credit to SpaceGuy's script for this part; slightly modified
					
					if(%tmpMin < %resMin && %transCond)
					{
						%resMin = %tmpMin;
						%colorRef = %d;
					}
				}
				else
				{
					%colorRef = %d;
					%exctMatch = true;
				}
			}

			if((%colCheck = getColorIDTable(%colorRef)) $= "1.000000 0.000000 1.000000 0.000000" || %colCheck $= "0.000000 0.000000 0.000000 0.000000")
				%colorRef = 0;
			
			return %colorRef;
			
		//////////////////////////////////////////////////
		
		//Convert RGBA value to closest, non-trans Hex value
		case "RGBA-Hex":

			for(%c = 0; %c < 3; %c++)
			{
				%relColSeg = getWord(%oldColorStr,%c);
				%relColNum = mFloatLength(15 * %relColSeg,0); //actually "16 *"
				%newColSeg = getWord("00 11 22 33 44 55 66 77 88 99 AA BB CC DD EE FF",mClamp(%relColNum,0,15));
				
				%newCol = setWord(%newCol,%c,%newColSeg); //ignore alpha value
			}
			
			return "<color:" @ strReplace(%newCol," ","") @ ">";
		
		//////////////////////////////////////////////////
		
		//Convert RGBA value to closest, existing RGBA value; return / set up array value
		case "RGBA-RGBAarr":
		
			%exctMatch = false;
			%resMin = 2.0;
			
			if($PTGSrvrColArr[%oldColorStr] !$= "")
				return $PTGSrvrColArr[%oldColorStr];
			
			for(%d = 0; %d < 64 && !%exctMatch; %d++)
			{
				%newColorStr = getColorIDTable(%d);
				
				if(%oldColorStr !$= %newColorStr)
				{
					%tmpMin = vectorDist(%newColorStr,%oldColorStr);
					%transCond = mabs(getWord(%newColorStr,3) - getWord(%oldColorStr,3)) < 0.3;  //credit to SpaceGuy's script for this part; slightly modified
					
					if(%tmpMin < %resMin && %transCond)
					{
						%resMin = %tmpMin;
						%colorRef = %d;
					}
				}
				else
				{
					%colorRef = %d;
					%exctMatch = true;
				}
			}

			if((%colCheck = getColorIDTable(%colorRef)) $= "1.000000 0.000000 1.000000 0.000000" || %colCheck $= "0.000000 0.000000 0.000000 0.000000")
				%colorRef = 0;
			
			$PTGSrvrColArr[%oldColorStr] = %colorRef;
			return %colorRef;
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//// RECURSIVE FUNCTIONS ////


function PTGClear_Recurs(%cl,%chCount,%brCount,%CHTotalC,%clearStatic,%dontSave,%func,%chPassCount) //separate function to prevent issues with setting and checking routine-action variable
{
	//Don't run lag check if generating finite / infinite terrain; only check if clearing (already checks in another func when generating)
	if($PTG.routine_Process !$= "Gen")
	{
		//Lag check, if enabled
		if((%lagCheck = PTG_Routine_Append_LimitsChk(%cl,"Clear")) !$= "Continue")
		{
			if(%lagCheck $= "Pause")
			{
				%delay = mClamp($PTG.delay_PauseTickS,1,30) * 1000;
				$PTG.dedSrvrFuncCheckTime += %delay;
				
				scheduleNoQuota(%delay,0,PTGClear_Recurs,%cl,%chCount,%brCount,%CHTotalC,%clearStatic,%dontSave,%func,%chPassCount);
			}
			else
				if(isObject(PTG_MainSO_SPtmp) && PTG_MainSO_SPtmp.presetLoadInProg)
					PTG_LoadServerPreset(getField(PTG_MainSO_SPtmp.srvrPresData,0),getField(PTG_MainSO_SPtmp.srvrPresData,1),getField(PTG_MainSO_SPtmp.srvrPresData,2),getField(PTG_MainSO_SPtmp.srvrPresData,3),getField(PTG_MainSO_SPtmp.srvrPresData,4),"Clear");
			
			return;
		}
	}
	
	//CLEAR CHUNKS
	if(isObject(BrickGroup_Chunks) && BrickGroup_Chunks.getCount() > 0 && (%chCount + %chPassCount) < %CHTotalC) //BrickGroup_Chunks check in PTGClear func, but added here just encase
	{
		%Chunk = BrickGroup_Chunks.getObject(0 + %chPassCount); //.getName();

		if(isObject(%Chunk) && !%Chunk.ChUnstablePTG && (!%Chunk.ChStaticPTG || %clearStatic)) //Unstable chunk check, even though no chunks should be unstable since routines would be halted.
		{
			%Chunk = %Chunk.getName(); //make sure it exists first to prevent console errors
			
			if((%count = %Chunk.getCount()) > 0)
			{
				%VARS = %chCount SPC %brCount SPC %CHTotalC SPC %clearStatic SPC %dontSave SPC %chPassCount; //same for purge func
				
				//SAVING OPTIONS
				if(!%dontSave) //if not purging chunks
				{
					if($PTG.chSaveMethod !$= "Never") //if chunk saving method is set to save either edited or all chunks
					{
						if($PTG.chSaveMethod $= "IfEdited") //if chunk saving method set to only edited chunks
						{
							if(%Chunk.ChEditedPTG)
								scheduleNoQuota(mClamp($PTG.delay_priFuncMS,0,100),0,PTG_Chunk_SaveRemoveBricks,%cl,%Chunk,%count,0,"",true,%func,%VARS);
							else
								scheduleNoQuota(mClamp($PTG.delay_priFuncMS,0,100),0,PTG_Chunk_RemoveBricks,%cl,%Chunk,%count,0,%func,%VARS);
						}
						else //if chunk saving method set to all generated chunks
							scheduleNoQuota(mClamp($PTG.delay_priFuncMS,0,100),0,PTG_Chunk_SaveRemoveBricks,%cl,%Chunk,%count,0,"",true,%func,%VARS);
					}
					else
						scheduleNoQuota(mClamp($PTG.delay_priFuncMS,0,100),0,PTG_Chunk_RemoveBricks,%cl,%Chunk,%count,0,%func,%VARS);
				}
				else
					scheduleNoQuota(mClamp($PTG.delay_priFuncMS,0,100),0,PTG_Chunk_RemoveBricks,%cl,%Chunk,%count,0,%func,%VARS);
			}
			else
			{
				%Chunk.delete();
				scheduleNoQuota(mClamp($PTG.delay_secFuncMS,0,100),0,PTGClear_Recurs,%cl,%chCount++,%brCount,%CHTotalC,%clearStatic,%dontSave,%func,%chPassCount);
			}
			
			return;
		}

		scheduleNoQuota(mClamp($PTG.delay_secFuncMS,0,100),0,PTGClear_Recurs,%cl,%chCount,%brCount,%CHTotalC,%clearStatic,%dontSave,%func,%chPassCount++);
	}
	
	//IF FINISHED CLEARING / PURGING ROUTINE
	else
	{		
		//Chat message notification to all players
		
		//If at least one chunk was removed
		if(%chCount > 0)
		{
			if(%chCount == 1) 
				%plurCH = "";
			else 
				%plurCH = "s";
			if(%brCount == 1) 
				%plurBR = "";
			else 
				%plurBR = "s";
			if(%brCount == 0) 
				%brCount = "0"; //otherwise shows blank character
			
			switch$(%func)
			{
				case "Clear":
					messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c3" SPC %chCount SPC "\c6of" SPC %CHTotalC SPC "Stable, Non-Static Chunk" @ %plurCH @ " and\c3" SPC %brCount SPC "\c6Brick" @ %plurBR SPC "cleared! \c3[C]");
					if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c4" @ %chCount SPC "\c0of" SPC %CHTotalC SPC "Stable, Non-Static Chunk" @ %plurCH @ " and\c4" SPC %brCount SPC "\c0Brick" @ %plurBR SPC "cleared! \c4[C] \c0->" SPC getWord(getDateTime(),1));
				
				case "ClearAll":
					messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:<color:ffdd00>" SPC %chCount SPC "\c6of" SPC %CHTotalC SPC "Stable Chunk" @ %plurCH @ " and<color:ffdd00>" SPC %brCount SPC "\c6Brick" @ %plurBR SPC "cleared! <color:ffdd00>[CA]");
					if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c4" @ %chCount SPC "\c0of" SPC %CHTotalC SPC "Stable Chunk" @ %plurCH @ " and\c4" SPC %brCount SPC "\c0Brick" @ %plurBR SPC "cleared! \c4[CA] \c0->" SPC getWord(getDateTime(),1));
				
				case "Purge":
					messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:<color:ff7700>" SPC %chCount SPC "\c6of" SPC %CHTotalC SPC "Stable Chunk" @ %plurCH @ " and<color:ff7700>" SPC %brCount SPC "\c6Brick" @ %plurBR SPC "purged! <color:ff7700>[P]");
					if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c4" @ %chCount SPC "\c0of" SPC %CHTotalC SPC "Stable Chunk" @ %plurCH @ " and\c4" SPC %brCount SPC "\c0Brick" @ %plurBR SPC "cleared! \c4[P] \c0->" SPC getWord(getDateTime(),1));
			}
		}
		
		//If no chunks were removed
		else
		{
			switch$(%func)
			{
				case "Clear":
					messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6No stable / non-static chunks to clear! \c3[C]");
					if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0No stable / non-static chunks to clear! ->" SPC getWord(getDateTime(),1));

				case "ClearAll":
					messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6No stable chunks to clear! <color:ffdd00>[CA]");
					if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0No stable chunks to clear! ->" SPC getWord(getDateTime(),1));
					
				case "Purge":
					messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6No stable chunks to purge! <color:ff7700>[P]");
					if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0No stable / non-static chunks to purge! ->" SPC getWord(getDateTime(),1));
			}
		}
	
		$PTG.routine_Process = "None";
		
		if(isObject(PTG_MainSO_SPtmp) && PTG_MainSO_SPtmp.presetLoadInProg)
			PTG_LoadServerPreset(getField(PTG_MainSO_SPtmp.srvrPresData,0),getField(PTG_MainSO_SPtmp.srvrPresData,1),getField(PTG_MainSO_SPtmp.srvrPresData,2),getField(PTG_MainSO_SPtmp.srvrPresData,3),getField(PTG_MainSO_SPtmp.srvrPresData,4),"Load");
	}
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTGClearSpam_Recurs(%cl,%chRmvCount,%brCount,%chNum,%chTotal,%BL_ID)
{
	//CLEAR BRICKS
	if(isObject(BrickGroup_Chunks) && %chNum < %chTotal) //BrickGroup_Chunks check in PTGClear func, but added here just encase
	{
		%Chunk = BrickGroup_Chunks.getObject(%chNum);

		if(isObject(%Chunk) && !%Chunk.ChUnstablePTG)
		{
			if((%count = %Chunk.getCount()) > 0)
			{
				%VARS = %chRmvCount SPC %brCount SPC %chNum SPC %chTotal SPC %BL_ID;
				scheduleNoQuota(mClamp($PTG.delay_priFuncMS,0,100),0,PTG_Chunk_RemoveBricksByID_Recurs,%cl,%Chunk,%Chunk.getCount(),0,0,0,%BL_ID,%VARS);
			}
			else
				scheduleNoQuota(mClamp($PTG.delay_secFuncMS,0,100),0,PTGClearSpam_Recurs,%cl,%chRmvCount,%brCount,%chNum++,%chTotal,%BL_ID);
		}
		else
			scheduleNoQuota(mClamp($PTG.delay_secFuncMS,0,100),0,PTGClearSpam_Recurs,%cl,%chRmvCount,%brCount,%chNum++,%chTotal,%BL_ID);
	}
	
	//IF FINISHED SPAM-CLEARING ROUTINE
	else
	{
		if(isObject(%cl))
			%nameB = %cl.name @ " (" @ %cl.bl_id @ ")";
		else
			%nameB = "CONSOLE";
	
		if(%brCount > 0)
		{
			if(%chRmvCount == 1) %plurCh = "";
			else %plurCh = "s";
			if(%brCount == 1) %plurBr = "";
			else %plurBr = "s";
			if(%brCount == 0) %brCount = "0"; //otherwise shows blank character (chunk count should automatically be > 0)

			messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c3 " @ %brCount @ " \c6brick" @ %plurBr @ " from \c3" @ %chRmvCount @ " \c6stable chunk" @ %plurCh @ " were removed successfully! <color:ffbb00>[CS]");
			if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %brCount @ " \c0brick" @ %plurBr @ " from \c4" @ %chRmvCount @ " \c0stable chunk" @ %plurCh @ " were removed successfully by \c4" @ %nameB @ "\c0. \c4[CS] \c0->" SPC getWord(getDateTime(),1));
		}
		else
		{
			messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6No spam bricks could be cleared! <color:ffbb00>[CS]");
			if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0No spam bricks could be cleared. \c4[CS] \c0->" SPC getWord(getDateTime(),1));
		}
		
		$PTG.routine_ProcessAux = "None";
	}
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTGRemoveChunk_Recurs(%cl,%Chunk,%chCount,%brCount,%removeSave,%rmvAttempt)
{
	if(!%rmvAttempt && isObject(BrickGroup_Chunks) && isObject(%Chunk) && !%Chunk.ChUnstablePTG) //included these checks again just encase
	{
		if((%count = %Chunk.getCount()) > 0)
		{
			%VARS = %chCount SPC %removeSave; //same for purge func
			PTG_Chunk_RemoveBricks(%cl,%Chunk,%count,0,"RemoveChunk",%VARS); //schedule not needed here //chunk doesn't get saved
		}
		else
		{
			%Chunk.delete();
			scheduleNoQuota(0,0,PTGRemoveChunk_Recurs,%cl,%ChunkN,%chCount++,%brCount,%removeSave,true);
		}
	}
	
	else
	{
		if(%brCount == 1)
			%plurBR = "";
		else
			%plurBR = "s";
		if(%brCount == 0)
			%brCount = "0"; //otherwise shows blank character
		
		%chPos = strReplace(%Chunk,"_"," "); //even though chunk obj was deleted, can still reference obj name
		%posX = getWord(%chPos,1);
		%posY = getWord(%chPos,2);
		
		if(isObject(%cl))
			%nameB = %cl.name @ " (" @ %cl.bl_id @ ")";
		else
			%nameB = "CONSOLE";
		
		
		if(isFile(%tmpFile = PTG_GetFP("Chunk-Norm",%Chunk,"",""))) //Account for if chunk save is permanent
		{
			if(%chCount > 0)
			{
				if(%removeSave && isFile(%tmpFile))
				{
					fileDelete(%tmpFile);
					
					if(!isFile(%tmpFile))
					{
						messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0>P\c3T\c1G:\c6 Both the chunk object and save file for location X:<color:ff9900>" @ %posX SPC "\c6Y:<color:ff9900>" @ %posY @ "\c6, along with<color:ff9900>" SPC %brCount SPC "\c6Brick" @ %plurBR @ ", removed successfully! <color:ff9900>[RC]");
						if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0Both the chunk object and save file for location X:\c4" @ %posX SPC "\c0Y:\c4" @ %posY @ "\c0, along with\c4" SPC %brCount SPC "\c0Brick" @ %plurBR @ ", removed successfully by \c4" @ %nameB @ "\c0. \c4[RC] \c0->" SPC getWord(getDateTime(),1));
					}
					else
						PTG_MsgClient(%cl,"Failed","PTG ERROR: Chunk Removal Failed","Chunk save file was found, but couldn't be removed; reason unknown.");
				}
				else
				{
					messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0>P\c3T\c1G:\c6 Chunk object for location X:<color:ff9900>" @ %posX SPC "\c6Y:<color:ff9900>" @ %posY @ "\c6, along with<color:ff9900>" SPC %brCount SPC "\c6Brick" @ %plurBR @ ", removed successfully! <color:ff9900>[RC]");
					if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0Chunk object for location X:\c4" @ %posX SPC "\c0Y:\c4" @ %posY @ "\c0, along with\c4" SPC %brCount SPC "\c0Brick" @ %plurBR @ ", removed successfully by \c4" @ %nameB @ "\c0. \c4[RC] \c0->" SPC getWord(getDateTime(),1));
				}
			}
			else
			{
				if(%removeSave)
					PTG_MsgClient(%cl,"Failed","PTG ERROR: Action Failed","Chunk and chunk save file couldn't be removed!");
				else
					PTG_MsgClient(%cl,"Failed","PTG ERROR: Action Failed","Chunk object couldn't be removed!");
			}
		}
		else
		{
			if(%chCount > 0)
			{
				if(!%removeSave || !isFile(PTG_GetFP("Chunk-Perm",%Chunk,"","")))
				{
					messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0>P\c3T\c1G:\c6 Chunk object for location X:<color:ff9900>" @ %posX SPC "\c6Y:<color:ff9900>" @ %posY @ "\c6, along with<color:ff9900>" SPC %brCount SPC "\c6Brick" @ %plurBR @ ", removed successfully! <color:ff9900>[RC]");
					if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0Chunk object for location X:\c4" @ %posX SPC "\c0Y:\c4" @ %posY @ "\c0, along with\c4" SPC %brCount SPC "\c0Brick" @ %plurBR @ ", removed successfully by \c4" @ %nameB @ "\c0. \c4[RC] \c0->" SPC getWord(getDateTime(),1));
				}
				else
				{
					messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0>P\c3T\c1G:\c6 Chunk object for location X:<color:ff9900>" @ %posX SPC "\c6Y:<color:ff9900>" @ %posY @ "\c6, along with<color:ff9900>" SPC %brCount SPC "\c6Brick" @ %plurBR @ ", removed successfully! Chunk save is permanent and couldn't be removed. <color:ff9900>[RC]");
					if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0Chunk object for location X:\c4" @ %posX SPC "\c0Y:\c4" @ %posY @ "\c0, along with\c4" SPC %brCount SPC "\c0Brick" @ %plurBR @ ", removed successfully by \c4" @ %nameB @ "\c0. Chunk save is permanent and couldn't be removed. \c4[RC] \c0->" SPC getWord(getDateTime(),1));
				}
			}
			else
			{
				if(%removeSave)
				{
					if(isFile(PTG_GetFP("Chunk-Perm",%Chunk,"","")))
						PTG_MsgClient(%cl,"Failed","PTG ERROR: Chunk Removal Failed","Chunk object couldn't be removed! Chunk save is set to permanent and can only be removed manually.");
					else
						PTG_MsgClient(%cl,"Failed","PTG ERROR: Chunk Removal Failed","Both the chunk object and save file couldn't be removed!");
				}
				else
					PTG_MsgClient(%cl,"Failed","PTG ERROR: Chunk Removal Failed","Chunk object couldn't be removed!");
			}
		}
		
		$PTG.routine_ProcessAux = "None";
	}
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTGSave_Recurs(%cl,%chNum,%chTotal,%chSaveAm,%chPotSaveAm) //separate function to prevent issues with setting and checking routine-action variable
{
	//SAVE CHUNKS (can run while normal routines are running)
	if(isObject(BrickGroup_Chunks) && %chNum < %chTotal) //BrickGroup_Chunks check in PTGSave func, but added here just encase
	{
		%Chunk = BrickGroup_Chunks.getObject(%chNum);

		if(isObject(%Chunk))
		{
			%Chunk = %Chunk.getName(); //make sure it exists first to prevent console errors
			
			if((%count = %Chunk.getCount()) > 0)
			{
				%VARS = %chNum SPC %chTotal SPC %chSaveAm SPC %chPotSaveAm;
				
				//SAVING OPTIONS
				if(!%dontSave) //if not purging chunks
				{
					if($PTG.chSaveMethod !$= "Never") //if chunk saving method is set to save either edited or all chunks
					{
						if($PTG.chSaveMethod $= "IfEdited") //if chunk saving method set to only edited chunks
						{
							if(%Chunk.ChEditedPTG)
							{
								if(!%Chunk.ChUnstablePTG)  //save (if chunk is stable)
									scheduleNoQuota(mClamp($PTG.delay_priFuncMS,0,100),0,PTG_Chunk_SaveRemoveBricks,%cl,%Chunk,%count,0,"",false,"Save",%VARS);
								else
									scheduleNoQuota(mClamp($PTG.delay_secFuncMS,0,100),0,PTGSave_Recurs,%cl,%chNum++,%chTotal,%chSaveAm,%chPotSaveAm++);
							}
							else
								scheduleNoQuota(mClamp($PTG.delay_secFuncMS,0,100),0,PTGSave_Recurs,%cl,%chNum++,%chTotal,%chSaveAm,%chPotSaveAm);
						}
						else //if chunk saving method set to all generated chunks
						{
							if(!%Chunk.ChUnstablePTG) //save (if chunk is stable)
								scheduleNoQuota(mClamp($PTG.delay_priFuncMS,0,100),0,PTG_Chunk_SaveRemoveBricks,%cl,%Chunk,%count,0,"",false,"Save",%VARS);
							else
								scheduleNoQuota(mClamp($PTG.delay_secFuncMS,0,100),0,PTGSave_Recurs,%cl,%chNum++,%chTotal,%chSaveAm,%chPotSaveAm++);
						}
					}
					else
						scheduleNoQuota(mClamp($PTG.delay_secFuncMS,0,100),0,PTGSave_Recurs,%cl,%chNum++,%chTotal,%chSaveAm,%chPotSaveAm);
				}
				else
					scheduleNoQuota(mClamp($PTG.delay_secFuncMS,0,100),0,PTGSave_Recurs,%cl,%chNum++,%chTotal,%chSaveAm,%chPotSaveAm);
			}
			else
				scheduleNoQuota(mClamp($PTG.delay_secFuncMS,0,100),0,PTGSave_Recurs,%cl,%chNum++,%chTotal,%chSaveAm,%chPotSaveAm);
			
			return;
		}

		scheduleNoQuota(mClamp($PTG.delay_secFuncMS,0,100),0,PTGSave_Recurs,%cl,%chNum++,%chTotal,%chSaveAm,%chPotSaveAm);
	}
	
	//IF FINISHED CLEARING / PURGING ROUTINE
	else
	{
		if(isObject(%cl))
		{
			%name = %cl.name;
			%nameB = %cl.name @ " (" @ %cl.bl_id @ ")";
		}
		else
		{
			%name = "CONSOLE";
			%nameB = "CONSOLE";
		}
			
		if(%chTotal > 0)
		{
			if(%chSaveAm > 0)
			{
				if(%chSaveAm == 1) 
					%plurCh = "";
				else 
					%plurCh = "s";
				if(%chPotSaveAm == 1) 
					%plurPCh = "";
				else 
					%plurPCh = "s";
				//if(%chPotSaveAm == 0) %chPotSaveAm = "0"; //shouldn't == 0 because save as saveAm, also shouldn't == 0 //otherwise shows blank character
				if(%chTotal == 1) 
					%plurPChT = "";
				else 
					%plurPChT = "s";
				
				messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: <color:00ff33>" @ %chSaveAm @ " \c6of " @ %chPotSaveAm @ " potential chunk" @ %plurPCh @ " of " @ %chTotal @ " total chunk" @ %plurPChT @ " saved successfully by \"" @ %name @ "\"! <color:00ff33>[S]");
				if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %chSaveAm @ " \c0of " @ %chPotSaveAm @ " potential chunk" @ %plurPCh @ " of " @ %chTotal @ " total chunk" @ %plurPChT @ " saved successfully by \c4" @ %nameB @ "\c0! \c4[S] \c0->" SPC getWord(getDateTime(),1));
			}
			else
			{
				switch$($PTG.chSaveMethod)
				{
					case "IfEdited":
						if(isObject(%cl))
							messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0>P\c3T\c1G: \c6No stable, edited chunks could be saved! <color:00ff33>[S]");
						if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %nameB @ " \c0attempted to save chunks, but no stable, edited chunks could be saved. \c4[S] \c0->" SPC getWord(getDateTime(),1));
					case "Always":
						if(isObject(%cl))
							messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0>P\c3T\c1G: \c6Action Failed","No stable chunks could be saved!");
						if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %nameB @ " \c0attempted to save chunks, but no stable chunks could be saved. \c4[S] \c0->" SPC getWord(getDateTime(),1));
					case "Never":
						if(isObject(%cl))
							messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0>P\c3T\c1G: \c6Your current chunk saving method prevents saving any chunks!");
						if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %nameB @ " \c0attempted to save chunks, but their current chunk saving method prevents saving any chunks. \c4[S] \c0->" SPC getWord(getDateTime(),1));
					default: //???
						if(isObject(%cl))
							messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0>P\c3T\c1G: \c6No stable chunks could be saved!");
						if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %nameB @ " \c0attempted to save chunks, but no stable chunks could be saved! \c4[S] \c0->" SPC getWord(getDateTime(),1));
				}
			}
		}
		else
		{
			//messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6No stable chunks exist to save! \c1[S]");
			if(isObject(%cl))
				messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0>P\c3T\c1G: \c6No stable chunks exist to save!");
			if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %nameB @ " \c0attempted to save chunks, but no stable chunks exist to save! \c4[S] \c0->" SPC getWord(getDateTime(),1));
		}
		
		$PTG.routine_ProcessAux = "None";
	}
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Chunk_RemoveBricksByID_Recurs(%cl,%Chunk,%brTotal,%brCurr,%brPass,%brRmv,%clearID,%VARS)
{
	if(%brCurr == 0)
	{
		if(%Chunk.ChUnstablePTG) // || %Chunk.ChStaticPTG)
		{
			%chRmvCount = getWord(%VARS,0);
			%brCount = getWord(%VARS,1);
			%chNum = getWord(%VARS,2);
			%chTotal = getWord(%VARS,3);
			%BL_ID = getWord(%VARS,4);
			
			PTGClearSpam_Recurs(%cl,%chRmvCount,%brCount,%chNum++,%chTotal,%BL_ID);
			return;
		}

		if(!%Chunk.ChUnstablePTG)
			%Chunk.ChUnstablePTG = true;
	}
	
	
	%brick = %Chunk.getObject(0 + %brPass);
	
	if(%brick.stackBL_ID == %clearID && %brick.getClassName() $= "fxDTSBrick" && %brick.isPlanted)
	{
		%brick.delete();
		%brRmv++;
	}
	else
		%brPass++;
	
	if(%brCurr++ >= %brTotal)
	{
		//%Chunk.delete(); //remove chunk if no bricks remaining?
		%Chunk.ChUnstablePTG = false;

		%chRmvCount = getWord(%VARS,0);
		%brCount = getWord(%VARS,1) + %brRmv;
		%chNum = getWord(%VARS,2);
		%chTotal = getWord(%VARS,3);
		%BL_ID = getWord(%VARS,4);
		
		if(%brRmv > 0) %chRmvCount++;
		
		PTGClearSpam_Recurs(%cl,%chRmvCount,%brCount,%chNum++,%chTotal,%BL_ID);
		return;
	}
	else
	{
		//$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_remMS,0,50));// + 0; //(%delay = mClamp($PTG.delay_priFuncMS,0,100));
		scheduleNoQuota(mClamp($PTG.brDelay_remMS,0,50),0,PTG_Chunk_RemoveBricksByID,%cl,%Chunk,%brTotal,%brCurr,%brPass,%brRmv,%clearID,%VARS);
	}
}


////////////////////////////////////////////////////////////////////////////////////////////////////

//// GROUP STREAM BRICKS INTO LARGER STREAM BRICKS TO REDUCE TOTAL BRICK COUNT ////
function PTG_GroupStreamBr_Recurs(%BrPosX,%BrPosY,%tempZ,%BrOBx,%BrOBy,%tempSizeXY,%count,%cl,%BG,%Chunk)
{
	%size = getWord(%tempSizeXY/16 SPC %tempSizeXY/8 SPC %tempSizeXY/4 SPC %tempSizeXY/2 SPC %tempSizeXY SPC %tempSizeXY*2 SPC %tempSizeXY*4,%count); //0.6 SPC 
	%tempZb = %tempZ + 0.1; //"+ 0.1" is necessary, otherwise function doesn't work (takes top of initial plate brick into account)
	%tempZrel = mFloor(%tempZb / %size) * %size;
	%tempZrelb = %tempZrel + %size;

	//Continue if bottom position of larger brick is above bottom position of initial reference plate brick
	//(if below, it'll cause problems with grouping streams that haven't been planted yet)
	if(%tempZrel > %tempZ)
	{
		//Brick datablock setup
		%tempSizeB = %tempSizeXY * 2;
		%tempSizeC = %tempSizeXY * 4;
		%tempSizeD = %tempSizeXY * 8;

		if(%tempSizeXY == 2) // < 4
			%ptgCMod = "PTG";
		
		%tempA = "brick" @ %tempSizeXY @ "x" @ %tempSizeXY @ "x" @ %tempSizeD @ "PTGData"; //for smaller bricks(?)
		%tempB = "brick" @ %tempSizeXY @ "x" @ %tempSizeXY @ "x" @ %tempSizeC @ "PTGData";
		%tempC = "brick" @ %tempSizeXY @ "x" @ %tempSizeXY @ "x" @ %tempSizeB @ "PTGData";
		%tempD = "brick" @ %tempSizeXY @ "xCube" @ %ptgCMod @ "Data";
		%tempE = "brick" @ %tempSizeXY @ "xHalfHCubePTGData";
		%tempF = "brick" @ %tempSizeXY @ "xQuarterHCubePTGData";
		%tempG = "brick" @ %tempSizeXY @ "xEighthHCubePTGData";
			//%tempH = "brick" @ %tempSizeXY @ "x" @ %tempSizeXY @ %ptgNMod @ "Data"; //doesn't divide evenly with other zone sizes, thus omitted
			//%tempH = "brick" @ %tempSizeXY @ "x" @ %tempSizeXY @ "fData"; //not necessary since script will be using bricks larger than a plate for grouping streams
		%db = getWord(%tempG SPC %tempF SPC %tempE SPC %tempD SPC %tempC SPC %tempB SPC %tempA,%count);
		
		//Make sure datablock object exists (prevent using bricks that are too large vertically)
		if(isObject(%db))
		{
			%BrposXYZ = %BrPosx SPC %BrPosY SPC %tempZrel + (%size / 2);
			%BrsizeXYZ = %BrOBx - 0.1 SPC %BrOBy - 0.1 SPC %size - 0.1;
			initContainerBoxSearch(%BrposXYZ,%BrsizeXYZ,$TypeMasks::FxBrickObjectType);
			%contZminTmp = %tempZrelb;

			//Container search of area where new, larger stream brick will be planted
			while(%objTemp = containerSearchNext())
			{
				//Abort if one of the bricks within container search is not a stream brick (or if too many bricks are present - as a safety precaution)
				if((!%objTemp.PTGStreamBr && !%objTemp.PTGStreamBrAux) || %failSafe++ > 100) //!%objTemp.PTGStreamBr || 
				{
					if(%count < 7)
						schedule(0,0,PTG_GroupStreamBr_Recurs,%BrPosX,%BrPosY,%tempZ,%BrOBx,%BrOBy,%tempSizeXY,%count++,%cl,%BG,%Chunk); //$PTG.streamsMTickMS
					
					return;
				}
				
				//Grab color and print ID information from previous bricks to apply to new brick
				%col = %objTemp.colorID;
				%pri = %objTemp.printID;
				
				//Find highest and lowest area of all bricks within container search
				%BrWBTmp = %objTemp.getWorldBox(); //WorldBox automatically takes brick global position into account
				%contZminTmp = getMin(getWord(%BrWBTmp,2),%contZminTmp);
				%contZmaxTmp = getMax(getWord(%BrWBTmp,5),%contZmaxTmp);
			}

			//If stream bricks fill container search (don't group bricks if total area doesn't exceed or at least fill container search area)
			if(%contZmaxTmp >= %tempZrelb && %contZminTmp <= %tempZrel)
			{
				initContainerBoxSearch(%BrposXYZ,%BrsizeXYZ,$TypeMasks::FxBrickObjectType);
				%delay = mClamp($PTG.delay_secFuncMS,0,100);
				%schdCnt = %delay;
				
				//Remove all stream bricks where new brick will be planted
				while(%objTemp = containerSearchNext())
				{
					//Delete smaller stream brick (physical water zones should be removed automatically with streams via packaged functions)
					%objTemp.schedule(%schdCnt += %delay,delete);
				}
				
				//Create water zones for merged streams, if enabled
				if(isObject(PTG_GlobalSO) && $PTG.genStreamZones)// && %size >= 2) //%tempSizeXY >= 4 && 
					%createZone = true;
				else
					%createZone = false;

				//Plant new brick to fill gap
				schedule(%schdCnt += %delay,0,PTG_Chunk_PlantBrick,%db,%BrposXYZ,%col,0,0,0,%pri,%cl,%BG,%Chunk,"StreamBrAux",%createZone);
				
				//Check next brick size (add 33ms to total delay, otherwise there will be issues with planted / culled bricks)
				if(%count < 7)
					schedule(%schdCnt += %delay,0,PTG_GroupStreamBr_Recurs,%BrPosX,%BrPosY,%tempZ,%BrOBx,%BrOBy,%tempSizeXY,%count++,%cl,%BG,%Chunk); //$PTG.streamsMTickMS
				
				return;
			}
		}
	}
	
	if(%count < 7)
		schedule(0,0,PTG_GroupStreamBr_Recurs,%BrPosX,%BrPosY,%tempZ,%BrOBx,%BrOBy,%tempSizeXY,%count++,%cl,%BG,%Chunk); //$PTG.streamsMTickMS
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_ConvertBuild_Recurs(%cl,%lineNum,%fileObjs,%xMod,%yMod,%ZMod,%brPass)
{
	//make sure to auto-rotate bricks w/ bounds, items and emitters, even if option is disabled (directional events don't get rotated)
	
	if(!$PTG.BuildUploading || !isObject(%cl) || %cl.bl_id != $PTG.lastBuildUploadID || $PTG.ForceCancelBldUpld || (!$PTG.allowNonHost_BuildManage && !PTG_HostCheck(%cl)))
	{
		%end = true;
		%fail = true;
	}
	else
	{
		if(!isObject(%file = getWord(%fileObjs,0))) //don't include .bls extension
		{
			%file = new FileObject();
			%data = $PTG_TmpBuildArr_BuildData;
			%buildName = getField(%data,1); //%relGridSz = getField(%data,13);
			
			%xMin = getField(%data,14);
			%xMax = getField(%data,15);
			%xMod = (%xMin + ((%xMax - %xMin) / 2)); //mFloor
			%yMin = getField(%data,16);
			%yMax = getField(%data,17);
			%yMod = (%yMin + ((%yMax - %yMin) / 2)); //mFloor
			%zMin = getField(%data,18);
			%zMax = getField(%data,19);
			%zMod = %zMin + 0; //snap Z to ground
			
			//%xMod = mFloatLength(%xMod / 0.5,0) * 0.5;
			//%yMod = mFloatLength(%yMod / 0.5,0) * 0.5;
			//%zMod = mFloatLength(%zMod / 0.5,0) * 0.5;
			%xMod = mFloor(%xMod / 0.5) * 0.5;
			%yMod = mFloor(%yMod / 0.5) * 0.5;
			%zMod = mFloor(%zMod / 0.5) * 0.5;
			
			//Figure relative grid size for build
			%maxTmpGridSz = mClamp(getMax(mAbs(%xMax - %xMin),mAbs(%yMax - %yMin)),2,256); //should fit in this range anyway
			
			for(%c = 0; %c < 8; %c++)
			{
				if(%maxTmpGridSz <= (%tmpGridSz = getWord("2 4 8 16 32 64 128 256",%c)))
				{
					%relGridSz = mClamp(%tmpGridSz,2,256); //relative grid size must be between 2m and 256m
					$PTG_TmpBuildArr_BuildData = setField($PTG_TmpBuildArr_BuildData,13,%relGridSz); //necessary encase build file needs to be removed / accessing filepath
					break;
				}
			}
			
			%file.openForWrite(PTG_GetFP("BuildInfo",%buildName,"",%relGridSz)); //info file
		
			//mClamp removes blank characters, and forces at least a value of "0" instead of " "
			%file.writeLine(">>PTG Brick-Save Info (Please don't edit this file; it could cause issues when loading the save)");
			%file.writeLine(">Enab:" SPC mClamp(getField(%data,0),0,1) SPC "Freq:" SPC mClamp(getField(%data,12),0,100) SPC "Rot:" SPC mClamp(getField(%data,2),0,1) SPC "BioDef:" SPC getField(%data,3) SPC "BioShore:" SPC getField(%data,4) SPC "BioSubM:" SPC getField(%data,5) SPC "BioCustA:" SPC getField(%data,6) SPC "BioCustB:" SPC getField(%data,7) SPC "BioCustC:" SPC getField(%data,8) SPC "Water:" SPC getField(%data,9) SPC "Mntns:" SPC getField(%data,10) SPC "FltIslds:" SPC getField(%data,11));
			%file.writeLine(">Bounds:" SPC %xMin-%xMod SPC %xMax-%xMod SPC %yMin-%yMod SPC %yMax-%yMod SPC %zMin-%zMod SPC %zMax-%zMod);
			%file.writeLine(">Owner:" TAB $PTG.lastBuildUploadID TAB $PTG.lastBuildUploadPlayer);
			%file.writeLine(">Fldr:" TAB %buildName);
			
			//Colorset Data
			for(%c = 0; %c < 64; %c++)
				%file.writeLine(getColorIDTable(%c)); //$PTG_TmpBuildArr_LineUpld[%c] //%lineNum = 64;
			
			%fileA = new FileObject();
			%fileB = new FileObject();
			%fileC = new FileObject();
			%fileD = new FileObject();
			%fileObjs = %file SPC %fileA SPC %fileB SPC %fileC SPC %fileD;
			
			%fileA.openForWrite(PTG_GetFP("Build",%buildName,0,%relGridSz));
			%fileB.openForWrite(PTG_GetFP("Build",%buildName,1,%relGridSz));
			%fileC.openForWrite(PTG_GetFP("Build",%buildName,2,%relGridSz));
			%fileD.openForWrite(PTG_GetFP("Build",%buildName,3,%relGridSz));
		}
		
		//////////////////////////////////////////////////

		%readLn = $PTG_TmpBuildArr_LineUpld[%lineNum];
		%frstWrd = firstWord(%readLn);
		%frstWrdB = getSubStr(%readLn,0,7); //for events
		%remPos = striPos(%readLn,"\"",0); //remove brick UI name (and space after, using "%remPos + 1" below)
		%strLen = strLen(%readLn);
		%newString = getSubStr(%readLn,%remPos + 2,%strLen);
		%writeLine = false;

		if(%frstWrdB $= "+-EVENT")
			%frstWrd = "+-EVENT";
				
		switch$(%frstWrd)
		{
			case "+-OWNER" or "+-NTOBJECTNAME" or "+-EVENT" or "Linecount":

				if(%brPass || %frstWrd $= "Linecount")
				{
					%newStringFinalA = %readLn;
					%newStringFinalB = %readLn;
					%newStringFinalC = %readLn;
					%newStringFinalD = %readLn;
					%writeLine = true;
				}
				
			case "+-EMITTER":

				%objUIN = getSubStr(%readLn,10,%remPos-10);
				%objDB = $uiNameTable_Emitters[%objUIN];

				if(%brPass && isObject(%objDB))
				{
					%objDBn = %objDB.getName();
					%emitPos = getWord(%newString,0);
					
					//Emitter Position
					if(%emitPos >= 2)
					{
						%emitPosA = %emitPos;
						if((%emitPosB = ((%emitPos + 1) % 6)) < 2)
							%emitPosB += 2;
						if((%emitPosC = ((%emitPos + 2) % 6)) < 2)
							%emitPosC += 2;
						if((%emitPosD = ((%emitPos + 3) % 6)) < 2)
							%emitPosD += 2;
					}
					else
						%emitPosA = %emitPosB = %emitPosC = %emitPosD = %emitPos;
					
					
					%newStringFinalA = "+-EMITTER" SPC %objDBn SPC %emitPosA;
					%newStringFinalB = "+-EMITTER" SPC %objDBn SPC %emitPosB;
					%newStringFinalC = "+-EMITTER" SPC %objDBn SPC %emitPosC;
					%newStringFinalD = "+-EMITTER" SPC %objDBn SPC %emitPosD;
					
					%writeLine = true;
				}
				
			case "+-LIGHT":

				%objUIN = getSubStr(%readLn,8,%remPos-8);
				%objDB = $uiNameTable_Lights[%objUIN];
				
				if(%brPass && isObject(%objDB))
				{
					%objDBn = %objDB.getName();
					%newStringFinalA = "+-LIGHT" SPC %objDBn SPC 1; //1?
					%newStringFinalB = "+-LIGHT" SPC %objDBn SPC 1;
					%newStringFinalC = "+-LIGHT" SPC %objDBn SPC 1;
					%newStringFinalD = "+-LIGHT" SPC %objDBn SPC 1;
					%writeLine = true;
				}
				
			case "+-ITEM":

				%objUIN = getSubStr(%readLn,7,%remPos-7);
				%objDB = $uiNameTable_Items[%objUIN];

				if(%brPass && isObject(%objDB))
				{
					//Both the item postion and direction have a value range between 0 to 6, but a direction value < 2 is still 2.
					
					%objDBn = %objDB.getName();
					%itemPos = getWord(%newString,0);
					%itemDir = getWord(%newString,1);

					//Item Position
					if(%itemPos >= 2)
					{
						%itemPosA = %itemPos;
						if((%itemPosB = ((%itemPos + 1) % 6)) < 2)
							%itemPosB += 2;
						if((%itemPosC = ((%itemPos + 2) % 6)) < 2)
							%itemPosC += 2;
						if((%itemPosD = ((%itemPos + 3) % 6)) < 2)
							%itemPosD += 2;
					}
					else
						%itemPosA = %itemPosB = %itemPosC = %itemPosD = %itemPos;
					
					//Item Direction
					%itemDirA = %itemDir;
					if((%itemDirB = ((%itemDir + 1) % 6)) < 2)
						%itemDirB += 2;
					if((%itemDirC = ((%itemDir + 2) % 6)) < 2)
						%itemDirC += 2;
					if((%itemDirD = ((%itemDir + 3) % 6)) < 2)
						%itemDirD += 2;
					
					
					%newStringFinalA = "+-ITEM" SPC %objDBn SPC %itemPosA SPC %itemDirA SPC getWord(%newString,2);
					%newStringFinalB = "+-ITEM" SPC %objDBn SPC %itemPosB SPC %itemDirB SPC getWord(%newString,2);
					%newStringFinalC = "+-ITEM" SPC %objDBn SPC %itemPosC SPC %itemDirC SPC getWord(%newString,2);
					%newStringFinalD = "+-ITEM" SPC %objDBn SPC %itemPosD SPC %itemDirD SPC getWord(%newString,2);
					
					%writeLine = true;
				}
				
			case "+-AUDIOEMITTER":

				%objUIN = getSubStr(%readLn,16,%remPos-16);
				%objDB = $uiNameTable_Music[%objUIN];

				if(%brPass && isObject(%objDB))
				{
					%objDBn = %objDB.getName();
					%newStringFinalA = "+-AUDIOEMITTER" SPC %objDBn;
					%newStringFinalB = "+-AUDIOEMITTER" SPC %objDBn;
					%newStringFinalC = "+-AUDIOEMITTER" SPC %objDBn;
					%newStringFinalD = "+-AUDIOEMITTER" SPC %objDBn;
					%writeLine = true;
				}
				
			case "+-VEHICLE":

				%objUIN = getSubStr(%readLn,10,%remPos-10);
				%objDB = $uiNameTable_Vehicle[%objUIN];

				if(%brPass && isObject(%objDB))
				{
					%objDBn = %objDB.getName();
					%newStringFinalA = "+-VEHICLE" SPC %objDBn SPC getWord(%newString,0); //or can just use %newString
					%newStringFinalB = "+-VEHICLE" SPC %objDBn SPC getWord(%newString,0);
					%newStringFinalC = "+-VEHICLE" SPC %objDBn SPC getWord(%newString,0);
					%newStringFinalD = "+-VEHICLE" SPC %objDBn SPC getWord(%newString,0);
					%writeLine = true;
				}
				
			case "BRICKDATA":
			
				%brDB = getWord(%readLn,1);

				if(isObject(%brDB))
				{
					%brNewPosX = getWord(%readLn,2) - %xMod;
					%brNewPosY = getWord(%readLn,3) - %yMod;
					%brNewPosZ = getWord(%readLn,4) - %zMod;
					%curRot = getWord(%readLn,5);
					%brDataCont = getWords(%readLn,6,13);

					%newStringFinalA = "BRICKDATA" SPC %brDB SPC %brNewPosX SPC %brNewPosY SPC %brNewPosZ SPC %curRot % 4 SPC %brDataCont SPC "PlayerBr"; //load as PlayerBr
					%newStringFinalB = "BRICKDATA" SPC %brDB SPC %brNewPosY SPC -1 * %brNewPosX SPC %brNewPosZ SPC (%curRot + 1) % 4 SPC %brDataCont SPC "PlayerBr";
					%newStringFinalC = "BRICKDATA" SPC %brDB SPC -1 * %brNewPosX SPC -1 * %brNewPosY SPC %brNewPosZ SPC (%curRot + 2) % 4 SPC %brDataCont SPC "PlayerBr";
					%newStringFinalD = "BRICKDATA" SPC %brDB SPC -1 * %brNewPosY SPC %brNewPosX SPC %brNewPosZ SPC (%curRot + 3) % 4 SPC %brDataCont SPC "PlayerBr";
					%writeLine = true;
					%brPass = true;
					
					$PTG_BuildLBrConvPass++;
				}
				else
					%brPass = false;
		}

		//Need to setup these vars for next function call below, even if this line is not written to the files
		%fileA = getWord(%fileObjs,1);
		%fileB = getWord(%fileObjs,2);
		%fileC = getWord(%fileObjs,3);
		%fileD = getWord(%fileObjs,4);
			
		if(%writeLine)
		{
			%fileA.writeLine(%newStringFinalA);
			%fileB.writeLine(%newStringFinalB);
			%fileC.writeLine(%newStringFinalC);
			%fileD.writeLine(%newStringFinalD);
		}
		
		//////////////////////////////////////////////////
		
		//Line max exceed check
		if(!%end && (%lineNum + 1) >= 20100) //tertiary check to protect server (error message not necessary since it runs this check twice already)
		{
			%end = true;
			%fail = true;
		}
		
		//Line count check (if finished)
		if(!%end && (%lineNum + 1) >= $PTG_TmpBuildArr_LineCount)
			%end = true;
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//If uploading finished
	if(%end)
	{
		%file = getWord(%fileObjs,0);
		%fileA = getWord(%fileObjs,1);
		%fileB = getWord(%fileObjs,2);
		%fileC = getWord(%fileObjs,3);
		%fileD = getWord(%fileObjs,4);
		
		%file.writeLine(">>END"); //close Info file here to prevent failing isObject(%file) check at beginning of function
		%file.close();
		%file.delete();

		%fileA.writeLine(">>END");
		%fileA.close();
		%fileA.delete();
		
		%fileB.writeLine(">>END");
		%fileB.close();
		%fileB.delete();
		
		%fileC.writeLine(">>END");
		%fileC.close();
		%fileC.delete();
		
		%fileD.writeLine(">>END");
		%fileD.close();
		%fileD.delete();
		
		//If no bricks were authenticated / uploaded
		if($PTG_BuildLBrConvPass <= 0)
		{
			PTG_MsgClient(%cl,"Failed","PTG: Build Upload Failed","No bricks from save could be authenticated; upload failed.");
			if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G \c2ERROR: \c0Build \"\c4" @ getField($PTG_TmpBuildArr_BuildData,1) @ "\c0\" upload failed; no bricks could be authenticated. \c4[!^] \c0->" SPC getWord(getDateTime(),1));
		
			fileDelete(PTG_GetFP("BuildInfo",getField($PTG_TmpBuildArr_BuildData,1),"",getField($PTG_TmpBuildArr_BuildData,13))); //check if files exist first?
			fileDelete(PTG_GetFP("Build",%buildName,0,%relGridSz));
			fileDelete(PTG_GetFP("Build",%buildName,1,%relGridSz));
			fileDelete(PTG_GetFP("Build",%buildName,2,%relGridSz));
			fileDelete(PTG_GetFP("Build",%buildName,3,%relGridSz));
			
			if(isObject(%cl))
				commandToClient(%cl,'PTG_ReceiveGUIData',"CloseBldProgWndw","");
			SERVERCMDPTG_BuildLoadSrvrFuncs(%cl,"CancelBuildUpload","");  //call cancel func even if no client exists (should auto-terminate though anyway)
			
			deleteVariables("$PTG_BuildLBrConvPass");
			return;
		}
		
		//Continue otherwise
		if(%fail)
		{
			fileDelete(PTG_GetFP("BuildInfo",getField($PTG_TmpBuildArr_BuildData,1),"",getField($PTG_TmpBuildArr_BuildData,13))); //check if files exist first?
			fileDelete(PTG_GetFP("Build",%buildName,0,%relGridSz));
			fileDelete(PTG_GetFP("Build",%buildName,1,%relGridSz));
			fileDelete(PTG_GetFP("Build",%buildName,2,%relGridSz));
			fileDelete(PTG_GetFP("Build",%buildName,3,%relGridSz));
			
			if(isObject(%cl))
			{
				if(!$PTG.ForceCancelBldUpld)
				{
					PTG_MsgClient(%cl,"Failed","PTG: Build Upload FAILED","Upload progress was interrupted.");
					if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c2ERROR: \c0\Build upload progress was interrupted. \c4[!^] \c0->" SPC getWord(getDateTime(),1));
				}
				else
				{
					PTG_MsgClient(%cl,"Success","PTG: Build Upload Cancelled","Build-upload was cancelled successfully.");
					if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0Current build upload was force-cancelled by player with ID \"" @ %cl.bl_id @  "\" \c4[!^] \c0->" SPC getWord(getDateTime(),1));
					//SERVERCMDPTG_BuildLoadSrvrFuncs(%cl,"CancelBuildUpload","");
					//commandToClient(%cl,'PTG_ReceiveGUIData',"CloseBldProgWndw","");
					
					//return; //to prevent calling "SERVERCMDPTG_BuildLoadSrvrFuncs" twice
				}
			}
		}
		else
		{
			if(isObject(%cl))
			{
				if($PTG_BuildLBrConvFail == 0)
					PTG_MsgClient(%cl,"Success","PTG: Build Upload Success","Build file was uploaded and converted successfully! Your Loaded-Builds GUI has been updated.");
				else
					PTG_MsgClient(%cl,"SuccessError","PTG: Build Upload Success","Build file was uploaded and converted successfully, except " @ $PTG_BuildLBrConvFail @ " brick(s) couldn't be authenticated because they don't exist on the server. Your Loaded-Builds GUI has been updated.");
				commandToClient(%cl,'PTG_ReceiveGUIData',"ListServerBuilds",getFields($PTG_TmpBuildArr_BuildData,0,12) TAB %cl.bl_id); //make sure data sent is < 255 chars! (strip of relGridSz and mod values using getFields to shorten as well)
			
				%plName = getSubStr(%cl.name,0,30);
				%plID = %cl.bl_id;
			}
			else
			{
				%plName = "Unknown";
				%plID = "??????";
			}
			
			if($PTG_BuildLBrConvFail == 0)
			{
				if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0Build \"\c4" @ getField($PTG_TmpBuildArr_BuildData,1) @ "\c0\" was uploaded successfully by player \"\c4" @ %plName @ " (" @ %plID  @ ")\c0\". \c4[^] \c0->" SPC getWord(getDateTime(),1));
			}
			else if($PTG.allowEchos)
				echo("\c4>>\c2P\c1T\c4G: \c0Build \"\c4" @ getField($PTG_TmpBuildArr_BuildData,1) @ "\c0\" was uploaded successfully by player \"\c4" @ %plName @ " (" @ %plID  @ ")\c0\", although " @ $PTG_BuildLBrConvFail @ " brick(s) couldn't be authenticated. \c4[^] \c0->" SPC getWord(getDateTime(),1));

			
			//notify other clients with add-on that build list was updated
			for(%c = 0; %c < clientGroup.getCount(); %c++)
			{
				%tmpCl = clientGroup.getObject(%c);
				
				if(%tmpCl.PTGver == 3 && %tmpCl != %cl)
					commandToClient(%tmpCl,'PTG_ReceiveGUIData',"NotifyBldListUpdate",%plName TAB %plID);
			}
		}

		if(isObject(%cl))
			commandToClient(%cl,'PTG_ReceiveGUIData',"CloseBldProgWndw","");
		SERVERCMDPTG_BuildLoadSrvrFuncs(%cl,"CancelBuildUpload","");  //call cancel func even if no client exists (should auto-terminate though anyway)
		
		deleteVariables("$PTG_BuildLBrConvPass");
		return;
	}

	%fileObjs = %file SPC %fileA SPC %fileB SPC %fileC SPC %fileD;
	commandToClient(%cl,'PTG_ReceiveGUIData',"BldConvProgBar",%lineNum SPC $PTG_TmpBuildArr_LineCount); //update client progress bar in GUI
	
	scheduleNoQuota(0,0,PTG_ConvertBuild_Recurs,%cl,%lineNum++,%fileObjs,%xMod,%yMod,%ZMod,%brPass); //gradual conversion (instead of using loops) to put less stress on server
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_SendBuildList_Recurs(%cl,%totCount,%relCount,%fp)
{
	if($PTG.ListingBuild && isObject(%cl) && %cl.bl_id == $PTG.lastBuildListID && ($PTG.allowNonHost_BuildManage || PTG_HostCheck(%cl)))
	{
		if(%totCount == 0)
			%totCount = getFileCount(%fp = PTG_GetFP("BuildCache","","",""));
		
		%tmpFile = findNextFile(%fp);
		%file = new FileObject();
		%file.openForRead(%tmpFile);
		
		%file.readLine();
		%strAllow = %file.readLine();
		%file.readLine();
		%strOwner = %file.readLine();
		%strFldr = %file.readLine();
		%name = getSubStr(getField(%strFldr,1),0,30);
		
		if(%name !$= $PTG.UploadBuildName) //don't list build that is currently build uploaded by another player
		{
			%enab = getWord(%strAllow,1);
			%freq = mClamp(getWord(%strAllow,3),0,100);
			%rot = getWord(%strAllow,5);
			%allowDef = getWord(%strAllow,7);
			%allowShore = getWord(%strAllow,9);
			%allowSubM = getWord(%strAllow,11);
			%allowCustA = getWord(%strAllow,13);
			%allowCustB = getWord(%strAllow,15);
			%allowCustC = getWord(%strAllow,17);
			%allowWat = getWord(%strAllow,19);
			%allowMntns = getWord(%strAllow,21);
			%allowFltIslds = getWord(%strAllow,23);
			%ownerID = getField(%strOwner,1);
			
			%data = %enab TAB %name TAB %rot TAB %allowDef TAB %allowShore TAB %allowSubM TAB %allowCustA TAB %allowCustB TAB %allowCustC TAB %allowWat TAB %allowMntns TAB %allowFltIslds TAB %freq TAB %ownerID;
			commandToClient(%cl,'PTG_ReceiveGUIData',"ListServerBuilds",%data);
		}
		%file.close();
		%file.delete();
		
		if(%relCount++ >= %totCount || %relCount >= 400) //hard-limit of 400
		{
			$PTG.ListingBuild = false;
			$PTG.lastBuildListPlayer = "";
			$PTG.lastBuildListID = "";
			$PTG.lastBuildListTime = "";
			
			return;
		}
		
		scheduleNoQuota(0,0,PTG_SendBuildList_Recurs,%cl,%totCount,%relCount,%fp); //gradual conversion (instead of using loops) to put less stress on server
	}
	else
	{
		if(isObject(%cl))
			PTG_MsgClient(%cl,"Failed","PTG: Build Upload FAILED","Build-listing progress was interrupted."); //echo("\c2>>P\c1T\c4G \c2ERROR: \"PTG_SendBuildList_Recurs\" unexpected halt; progress was interrupted. [!] \c0->" SPC getWord(getDateTime(),1));
		
		if(%cl.bl_id == $PTG.lastBuildListID)
		{
			$PTG.ListingBuild = false;
			$PTG.lastBuildListPlayer = "";
			$PTG.lastBuildListID = "";
			$PTG.lastBuildListTime = "";
		}
	}
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_ObjLimitCheck(%objType) //,%objClient
{
	//For some reason, still spams host with message that bot quota limit for server was reached...
	
	switch$(%objType)
	{
		case "Bots":
		
			%objClassName = "AIPlayer";
			%objMax = $Server::MaxPlayerVehicles_Total; //$Pref works dedicated???
			//%group = mainHoleBotSet;
	
		case "Vehicles":
		
			%objClassName = "WheeledVehicle";
			%objMax = $Server::MaxPhysVehicles_Total;
			%group = MissionCleanup;
		
		case "PlyrItems": //??? //Not sure how to access amount for certain player / client only //!!! check if client and player exist, if used !!!
		
			%objClassName = "Item";
			%group = MissionCleanup;
			
			if($Server::Lan) 
				%objMax = $Server::QuotaLAN::Item;
			else 
				%objMax = $Server::Quota::Item;
			
		case "PlyrLight": //??? //Not sure how to access amount for certain player / client only
		
			%objClassName = "fxLight";
			%group = MissionCleanup;
			
			if($Server::Lan) 
				%objMax = $Server::QuotaLAN::Environment;
			else 
				%objMax = $Server::Quota::Environment;

		case "PlyrEmitter": //??? //Not sure how to access amount for certain player / client only
		
			%objClassName = "ParticleEmitterNode";
			%group = MissionCleanup;
			
			if($Server::Lan) 
				%objMax = $Server::QuotaLAN::Environment;
			else 
				%objMax = $Server::Quota::Environment;
	}
	
	//////////////////////////////////////////////////
	
	if(%objType $= "Bots")
	{
		if(isObject(mainHoleBotSet))
			%objTotC = mainHoleBotSet.getCount();
		else
			return false;
	}
	else
	{
		for(%c = 0; %c < MissionCleanup.getCount(); %c++) //if(isObject(MissionCleanup))?
		{
			if(MissionCleanup.getObject(%c).getClassName() $= %objClassName) 
				%objTotC++;
		}
	}

	if(%objTotC >= %objMax) 
		return true;
	else 
		return false;
}