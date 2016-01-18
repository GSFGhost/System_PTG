//Example: PTG_LoadServerPreset("PresetName",true,true,"None",false);

function PTG_LoadServerPreset(%fpPreset,%noFallDamage,%autoHalt,%autoClearFunc,%autoReset,%pass)
{
	//Load default settings if fpPreset is blank
	//No client or player necessary for this function; can be executed via gamemode server.cs script
	//Prevent running multiple instances of this function at the same time

	
	//Prevent conflicting with server settings if being uploaded at the same time
		//Uploading Simplex GUI settings happens instantly, so this function should simply execute afterwards w/o conflicting with the Simplex start
	if(PTG_GlobalSO_tmp.uploadingSettings)
	{
		//%fpPreset = getSubStr(%fpPreset,0,30);
		
		//%fp = PTG_GetFP("ServerPresetFP","","","");
		//%fpPresetRef = strReplace(%fpPreset,".txt","");
		//%fpPresetRef = strReplace(%fpPresetRef,%fp,"");
		//%fpPreset = %fpPresetRef;
					
		//if(%fpPreset $= "" || %fpPreset $= " ")
		//	%fpPreset = "DEFAULT";

		//messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6Server preset \"\c0" @ %fpPreset @ "\c6\" failed to load; other server settings are currently being uploaded. <color:4400cc>[*]");
		//echo("\c2>>P\c1T\c4G \c2ERROR: Server-side preset \"" @ %fpPreset @ "\" load aborted; other server settings are currently being uploaded. [!] \c0->" SPC getWord(getDateTime(),1));
		
		//if(PTG_MainSO_SPtmp.presetLoadInProg) scheduleNoQuota(0,0,PTG_LoadServerPreset,%fpPreset,%noFallDamage,%autoHalt,%autoClearFunc,%autoReset,"Cancel");
		//return;
		
		PTG_GlobalSO_tmp.uploadingSettings = false;
		
		if(isObject(PTG_GlobalSO_tmp.lastUploadClient))
			PTG_GlobalSO_tmp.lastUploadClient.PTGupldSecKey = "";
		
		if(isObject(PTG_GlobalSO_tmp))
			PTG_GlobalSO_tmp.delete(); //auto-clears previous upload reference fields (such as ".lastSetUploadTime")
		if(isObject(PTG_MainSO_tmp)) 
			PTG_MainSO_tmp.delete();
		if(isObject(PTG_BiomesSO_tmp)) 
			PTG_BiomesSO_tmp.delete();

		deleteVariables("$PTGSrvrColArr*");
		deleteVariables("$tmpPriRefArr*");
		
		//If previously uploading Complex GUI settings
		deleteVariables("$PTG_secRelayCnt");
		deleteVariables("$PTG_massDetCurrBiome");
		deleteVariables("$PTG_massDetCurrNum");
		deleteVariables("$PTG_massDetActCount");
		deleteVariables("$PTG_MDfailsafe");
		
		echo("\c2>>P\c1T\c4G: \c0Server settings are already being set up / uploaded; overwriting... \c4[*] \c0->" SPC getWord(getDateTime(),1));
	}
	
	//Make sure Chunks BrickGroup exists
	if(!isObject(BrickGroup_Chunks))
		mainBrickGroup.add(new SimGroup("BrickGroup_Chunks"));
	
	//Make sure necessary script / sim groups are set up
	//if(!isObject(PTG_GlobalSO)) //required to determine if console echo notifications are allowed in the "PTGRemoveChunk_Recurs" function
	//{
	//	messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6Server preset \"\c0" @ %fpPreset @ "\c6\" failed to load; please check server console for more information. <color:4400cc>[*]");
	//	echo("\c2>>P\c1T\c4G \c2ERROR: No routine settings detected; please have the server host apply their routine settings to the server first. Server preset load aborted. [!] \c0->" SPC getWord(getDateTime(),1));
		
	//	if(PTG_MainSO_SPtmp.presetLoadInProg) scheduleNoQuota(0,0,PTG_LoadServerPreset,%fpPreset,%noFallDamage,%autoHalt,%autoClearFunc,%autoReset,"Cancel");
	//	return;
	//}
	if(!isObject(PTG_MainSO) && %fpPreset !$= "" && %fpPreset !$= " ")
	{
		messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6Server preset \"\c0" @ %fpPreset @ "\c6\" failed to load; main script object not found. <color:4400cc>[*]");
		echo("\c2>>P\c1T\c4G \c2ERROR: Main script object detected; please have the server host or a super admin apply their settings to the server first. Server preset load aborted. [!] \c0->" SPC getWord(getDateTime(),1));
		
		if(PTG_MainSO_SPtmp.presetLoadInProg) scheduleNoQuota(0,0,PTG_LoadServerPreset,%fpPreset,%noFallDamage,%autoHalt,%autoClearFunc,%autoReset,"Cancel");
		return;
	}
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////

	switch$(%pass)
	{
		case "Initialize" or " " or "" or "Restart": //"Restart" force cancels current progress (?)

			//Make sure preset load process is initialized first to prevent issues
			if(!PTG_MainSO_SPtmp.presetLoadInProg || %pass $= "Restart")
			{
				//If restarting preset load progress
				if(%pass $= "Restart")
				{
					presetLoadFallRes.presetLoadFallRes = false;
					PTG_MainSO_SPtmp.presetLoadInProg = false;
					PTG_MainSO_SPtmp.srvrPresData = "";
					
					//Delete color arrays
					deleteVariables("$PTGSrvrColArr*");
					deleteVariables("$tmpColRefArr*");
					deleteVariables("$tmpPriRefArr*");
				}
				
				%fpPreset = getSubStr(%fpPreset,0,30);
				
				//Make sure preset file path exists (when actually loading settings later as well)
				if(%fpPreset !$= "" && %fpPreset !$= " ") //if not default (leaving %fpPreset empty load default settings to server)
				{
					%fp = PTG_GetFP("ServerPresetFP","","","");
					%fpPresetRef = strReplace(%fpPreset,".txt","");
					%fpPresetRef = strReplace(%fpPresetRef,%fp,"");
					%fpPreset = %fpPresetRef;
				}
				else
				{
					//Check for custom default settings, otherwise use standard default settings if not found
					if(isFile("config/server/PTGv3/Default.txt"))
						%fp = "config/server/PTGv3/";
					else
						%fp = "Add-Ons/System_PTG/PRESETS/";
					
					%fpPresetRef = "Default";
				}
					
				if(!isFile(%fileFP = %fp @ %fpPresetRef @ ".txt"))
				{
					messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6Server preset \"\c0" @ %fpPresetRef @ "\c6\" failed to load; file wasn't found. <color:4400cc>[*]");
					echo("\c2>>P\c1T\c4G \c2ERROR: Server-sided preset \"" @ %fileFP @ "\" load aborted; file wasn't found. [!] \c0->" SPC getWord(getDateTime(),1));
					
					scheduleNoQuota(0,0,PTG_LoadServerPreset,%fpPreset,%noFallDamage,%autoHalt,%autoClearFunc,%autoReset,"Cancel");
					return;
				}
				
				//////////          //////////          //////////

				//Continue if file exists

				if(isObject(PTG_GlobalSO_SPtmp)) 
					PTG_GlobalSO_SPtmp.delete();
				if(isObject(PTG_MainSO_SPtmp)) 
					PTG_MainSO_SPtmp.delete();
				if(isObject(PTG_BiomesSO_SPtmp)) 
					PTG_BiomesSO_SPtmp.delete();

				MissionCleanup.add(new ScriptObject(PTG_MainSO_SPtmp));
				MissionCleanup.add(new ScriptObject(PTG_BiomesSO_SPtmp));
				
				//Don't send initial message when loading default settings (for first time)
				if(%fpPreset !$= "" && %fpPreset !$= " " && isObject(PTG_GlobalSO)) //$PTG?
				{
					if(%noFallDamage)
						%textA = "Disable Fall Damage; ";
					if(%autoHalt && !PTG_GlobalSO.routine_isHalting && PTG_GlobalSO.routine_Process !$= "None") //isObject(PTG_GlobalSO) && 
						%textB = "Halt Routine; ";
					if(%autoClearFunc !$= "None" && %autoClearFunc !$= " " && %autoClearFunc !$= "")
						%textC = "Clear Terrain; ";
					if(%autoReset)
						%textD = "Reset Minigame; ";
					
					if(%textA $= "" && %textB $= "" && %textC $= "" && %textD $= "")
						%txtMod = "... ";
					else
						%txtMod = " <color:4400cc>-> \c6";
					%initTasks = %txtMod @ %textA @ %textB @ %textC @ %textD;
					
					messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 Loading server preset \"<color:4400cc>" @ %fpPreset @ "\c6\"" @ %initTasks @ "<color:4400cc>[*]");
					if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0Loading server preset \"\c4" @ %fpPreset @ "\"\c0" @ strReplace(%initTasks,"<color:4400cc>","") @ "\c4[*] \c0->" SPC getWord(getDateTime(),1));
				}

				if(%noFallDamage) PTG_MainSO_SPtmp.presetLoadFallRes = true; //usually store fields like these in the global script object, but global script object may not always exist
				PTG_MainSO_SPtmp.presetLoadInProg = true;
				PTG_MainSO_SPtmp.srvrPresData = %fpPreset TAB %noFallDamage TAB %autoHalt TAB %autoClearFunc TAB %autoReset;// TAB %pass;
				

				scheduleNoQuota(0,0,PTG_LoadServerPreset,%fpPreset,%noFallDamage,%autoHalt,%autoClearFunc,%autoReset,"Halt");
			}
			else
			{
				if(%fpPreset $= "" || %fpPreset $= " ")
					%fpPreset = "DEFAULT";
				
				messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6Server preset \"\c0" @ %fpPreset @ "\c6\" failed to load; another server preset is already being loaded. <color:4400cc>[*]");
				echo("\c2>>P\c1T\c4G \c2ERROR: Server-side preset \"" @ %fpPreset @ "\" load aborted; a preset is already being loaded to the server. [!] \c0->" SPC getWord(getDateTime(),1));
				
				scheduleNoQuota(0,0,PTG_LoadServerPreset,%fpPreset,%noFallDamage,%autoHalt,%autoClearFunc,%autoReset,"Cancel"); //might stop first load routine...
			}
		
		//////////////////////////////////////////////////
		
		case "Halt":
		
			if(PTG_MainSO_SPtmp.presetLoadInProg)
			{
				if(%autoHalt && isObject(PTG_GlobalSO) && (PTG_GlobalSO.routine_Process !$= "None" && PTG_GlobalSO.routine_Process !$= "")) //None, Gen, Clear
				{
					//If current routine is a clearing routine, and this function is set to clear terrain before loading the preset, in theory we could just wait for 
						//terrain to be cleared and then jump to the loading portion of the function. However, the current clearing routine may not be the same one 
						//defined for this function, so just to be safe, routine will be halted anyway.

					if(!PTG_GlobalSO.routine_isHalting) //isObject(PTG_GlobalSO) &&
						PTG_GlobalSO.routine_isHalting = true; //continue preset load function after gen or clear routine halts
				}
				else
					scheduleNoQuota(0,0,PTG_LoadServerPreset,%fpPreset,%noFallDamage,%autoHalt,%autoClearFunc,%autoReset,"Clear");
			}
			else
			{
				messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6Server preset \"\c0" @ %fpPreset @ "\c6\" failed to load; preset load function wasn't initialized. <color:4400cc>[*]");
				echo("\c2>>P\c1T\c4G \c2ERROR: Server-side preset \"" @ %fpPreset @ "\" load aborted; attempting to halt current generation / clear routine, but preset load function wasn't initialized. [!] \c0->" SPC getWord(getDateTime(),1));
				
				scheduleNoQuota(0,0,PTG_LoadServerPreset,%fpPreset,%noFallDamage,%autoHalt,%autoClearFunc,%autoReset,"Cancel");
			}
		
		//////////////////////////////////////////////////
			
		case "Clear":

			if(PTG_MainSO_SPtmp.presetLoadInProg)
			{
				//Force halt is attempting to clear terrain while terrain is still being generated (auto-halting would have been disabled for this situation)
				if(isObject(PTG_GlobalSO) && PTG_GlobalSO.routine_Process $= "Gen")
				{
					messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6Force-halting generation routine in order to clear terrain... <color:4400cc>[*]");
					echo("\c2>>P\c1T\c4G \c0Force-halting generation routine in order to clear terrain... \c4[*] \c0->" SPC getWord(getDateTime(),1));
					
					PTG_MainSO_SPtmp.srvrPresData = %fpPreset TAB %noFallDamage TAB true TAB %autoClearFunc TAB %autoReset;

						if(!PTG_GlobalSO.routine_isHalting) //isObject(PTG_GlobalSO) &&
							PTG_GlobalSO.routine_isHalting = true; //continue preset load function after gen or clear routine halts
					
					//scheduleNoQuota(0,0,PTG_LoadServerPreset,%fpPreset,%noFallDamage,true,%autoClearFunc,%autoReset,"Halt");
					return;
				}

				//None, Clear, ClearAll or Purge
				switch$(%autoClearFunc) //no need to check "PTG_GlobalSO.routine_Process !$= "Clear"" since routine would be stopped above no matter the situation
				{
					case "None" or " " or "":
					
						scheduleNoQuota(0,0,PTG_LoadServerPreset,%fpPreset,%noFallDamage,%autoHalt,%autoClearFunc,%autoReset,"Load");
						return;
						
					case "Clear" or "Clear-Save":
					
						if(%autoClearFunc $= "Clear-Save")
							%dontSave = false;
						else
							%dontSave = true;
							
						PTGClear_Recurs(%cl,"","",BrickGroup_Chunks.getCount(),false,%dontSave,"Clear",0);
						return;
						
					case "ClearAll" or "ClearAll-Save":
					
						if(%autoClearFunc $= "ClearAll-Save")
							%dontSave = false;
						else
							%dontSave = true;
						
						PTGClear_Recurs(%cl,"","",BrickGroup_Chunks.getCount(),true,%dontSave,"ClearAll",0);
						return;
						
					case "Purge":
					
						PTGClear_Recurs(%cl,"","",BrickGroup_Chunks.getCount(),true,true,"Purge",0);
						return;
						
					default:
					
						messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6Server preset \"\c0" @ %fpPreset @ "\c6\" failed to load; unknown clear method defined. <color:4400cc>[*]");
						echo("\c2>>P\c1T\c4G \c2ERROR: Server-side preset \"" @ %fpPreset @ "\" load aborted; attempting to clear terrain, but unknown clear method \"" @ %autoClearFunc @ "\" was defined. [!] \c0->" SPC getWord(getDateTime(),1));
						
						scheduleNoQuota(0,0,PTG_LoadServerPreset,%fpPreset,%noFallDamage,%autoHalt,%autoClearFunc,%autoReset,"Cancel");
						return;
				}
			}
			
			//If function wasn't initialized
			messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6Server preset \"\c0" @ %fpPreset @ "\c6\" failed to load; preset load function wasn't initialized. <color:4400cc>[*]");
			echo("\c2>>P\c1T\c4G \c2ERROR: Server-side preset \"" @ %fpPreset @ "\" load aborted; attempting to clear terrain, but preset load function wasn't initialized. [!] \c0->" SPC getWord(getDateTime(),1));

			scheduleNoQuota(0,0,PTG_LoadServerPreset,%fpPreset,%noFallDamage,%autoHalt,%autoClearFunc,%autoReset,"Cancel");
		
		//////////////////////////////////////////////////
		
		case "Load":

			if(!PTG_MainSO_SPtmp.presetLoadInProg)
			{
				messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6Server preset \"\c0" @ %fpPreset @ "\c6\" failed to load; preset load function wasn't initialized. <color:4400cc>[*]");
				echo("\c2>>P\c1T\c4G \c2ERROR: Server-side preset \"" @ %fpPreset @ "\" load aborted; attempting to clear terrain, but preset load function wasn't initialized. [!] \c0->" SPC getWord(getDateTime(),1));
				
				scheduleNoQuota(0,0,PTG_LoadServerPreset,%fpPreset,%noFallDamage,%autoHalt,%autoClearFunc,%autoReset,"Cancel");
				return;
			}
			
			PTG_LoadServerPreset_Recurs(%fpPreset,"Initialize","","");
		
		//////////////////////////////////////////////////
		
		case "Finalize" or "Cancel":
		
			PTG_MainSO_SPtmp.presetLoadFallRes = "";
			PTG_MainSO_SPtmp.presetLoadInProg = "";
			PTG_MainSO_SPtmp.srvrPresData = "";

			if(%pass $= "Finalize")
			{
				if(%noFallDamage) 
					%textA = "Restoring Fall Damage; ";
				if(%autoReset && isObject(MiniGameGroup) && MiniGameGroup.getCount() > 0)
					%textB = "Resetting Minigame; ";
				//if(%noFallDamage && %autoReset)
				//	%
				%finalTasks = %textA @ %textB;
				
				if(isObject(PTG_GlobalSO_SPtmp)) //if routine settings were loaded, reference new font size for chat messages, otherwise use prev size
					%fontSz = PTG_GlobalSO_SPtmp.fontSize;
				else
					%fontSz = $PTG.fontSize;
				
				if(%fpPreset !$= "" && %fpPreset !$= " ")
				{
					messageAll('',"<font:Verdana Bold:" @ %fontSz @ ">\c0P\c3T\c1G:\c6 Preset was loaded successfully! " @ %finalTasks @ "<color:4400cc>[*]");
					if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0Preset was loaded successfully! " @ %finalTasks @ "\c4[*] \c0->" SPC getWord(getDateTime(),1));
				}
				else
				{
					if(isObject(PTG_GlobalSO))
					{
						messageAll('',"<font:Verdana Bold:" @ %fontSz @ ">\c0P\c3T\c1G:\c6 Default server settings loaded successfully! " @ %finalTasks @ "<color:4400cc>[*]");
						if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0Default server settings loaded successfully! " @ %finalTasks @ "\c4[*] \c0->" SPC getWord(getDateTime(),1));
					}
					
					//If loading settings for first time, echo in console only
					else
					{
						$PTG_init = true;
						echo("\c4>>\c2P\c1T\c4G: \c0Default server settings loaded successfully! " @ %finalTasks @ "\c4[*] \c0->" SPC getWord(getDateTime(),1));
					}
				}
			
				//Delete script objects
				if(isObject(PTG_MainSO)) 
					PTG_MainSO.delete();
				if(isObject(PTG_BiomesSO)) 
					PTG_BiomesSO.delete();
				
				//Rename script objects
				if(isObject(PTG_GlobalSO_SPtmp))
				{
					if(isObject(PTG_GlobalSO)) //don't delete global script group unless replacing it with a new one
						PTG_GlobalSO.delete();
					PTG_GlobalSO_SPtmp.setName("PTG_GlobalSO");
					$PTG = PTG_GlobalSO;
				}
				if(isObject(PTG_MainSO_SPtmp)) 
					PTG_MainSO_SPtmp.setName("PTG_MainSO");
				$PTGm = PTG_MainSO;
				
				if(isObject(PTG_BiomesSO_SPtmp)) 
					PTG_BiomesSO_SPtmp.setName("PTG_BiomesSO");
				$PTGbio = PTG_BiomesSO;
				
				//If resetting minigame (make sure a minigame is running first)
				if(%autoReset && isObject(MiniGameGroup) && MiniGameGroup.getCount() > 0 && MiniGameGroup.getObject(0).class $= "MiniGameSO")
				{
					MiniGameGroup.getObject(0).reset(0); //.scheduleReset(#?);
					
					if(%fpPreset !$= "" && %fpPreset !$= " ")
					{
						$PTG_SekKeyDedRmt = true;
						schedule(500,0,serverCmdPTGStart,"Remote");
					}
				}
				else if(%fpPreset !$= "" && %fpPreset !$= " ")
				{
					$PTG_SekKeyDedRmt = true;
					schedule(100,0,serverCmdPTGStart,"Remote");
				}
			}
			else
			{
				//Delete script objects
				if(isObject(PTG_GlobalSO_SPtmp))
					PTG_GlobalSO_SPtmp.delete();
				if(isObject(PTG_MainSO_SPtmp)) 
					PTG_MainSO_SPtmp.delete();
				if(isObject(PTG_BiomesSO_SPtmp)) 
					PTG_BiomesSO_SPtmp.delete();
			}
			
			//Delete color arrays
			deleteVariables("$PTGSrvrColArr*");
			deleteVariables("$tmpColRefArr*");
			deleteVariables("$tmpPriRefArr*");
	}
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_LoadServerPreset_Recurs(%fpPreset,%pass,%relBioSO,%file)
{
	//Loading the default preset also loads routine settings to the server (custom if routines save file found, otherwise default as well)
	//Reference brick UInames ("$uiNameTable") instead of datablock names (datablock names are not always saved to presets - i.e. if not locally connected to server when saving)
	
	if(!PTG_MainSO_SPtmp.presetLoadInProg)
	{
		messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6Server preset \"\c0" @ %fpPreset @ "\c6\" failed to load; \"PTG_LoadServerPreset\" function wasn't initialized. <color:4400cc>[*]");
		echo("\c2>>P\c1T\c4G \c2ERROR: Server-side preset \"" @ %fpPreset @ "\" load aborted; \"PTG_LoadServerPreset\" function wasn't initialized. [!] \c0->" SPC getWord(getDateTime(),1));
		
		PTG_LoadServerPreset(%fpPreset,%noFallDamage,%autoHalt,%autoClearFunc,%autoReset,"Cancel");
	}

	%strpTxt = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ|(){}[]<>\"\'\\/~`!@#$%^&*_+=,?:;"; //for Biomes (evals) only //must include both upper and lower casing
	%printNum = $globalPrintCount; //"getNumPrintTextures()" doesn't seem to update when prints are disabled

	
	switch$(%pass)
	{
		case "Initialize":

			if(isObject(%file)) 
				%file.delete();
			if(%fpPreset !$= "" && %fpPreset !$= " ") //file path and extension should already be filtered our for non-default presets in func above (prior to this func)
			{
				%fp = PTG_GetFP("ServerPresetFP","","","");
				%fpPresetRef = %fpPreset;
				//%fpPreset = strReplace(%fpPreset,".txt","");
				//%fpPreset = strReplace(%fpPreset,%fp,"");
			}
			else
			{
				//Check for custom default settings, otherwise use standard default settings if not found
				if(isFile("config/server/PTGv3/Default.txt"))
					%fp = "config/server/PTGv3/";
				else
					%fp = "Add-Ons/System_PTG/PRESETS/";
				
				%fpPresetRef = "Default";
			}
				
			//Make sure preset file path exists
			if(!isFile(%fileFP = %fp @ %fpPresetRef @ ".txt"))
			{
				messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6Server preset \"\c0" @ %fpPresetRef @ "\c6\" failed to load; file wasn't found. <color:4400cc>[*]");
				echo("\c2>>P\c1T\c4G \c2ERROR: Server-sided preset \"" @ %fileFP @ "\" load aborted; file wasn't found. [!] \c0->" SPC getWord(getDateTime(),1));
				
				PTG_LoadServerPreset(%fpPreset,%noFallDamage,%autoHalt,%autoClearFunc,%autoReset,"Cancel");
				return;
			}
			
			%file = new FileObject();
			%file.openForRead(%fileFP);
			
			%file.readLine();
			%file.readLine();
			%file.readLine();
			
			//Colorset data
			for(%c = 0; %c < 64; %c++)
				$tmpColRefArr[%c] = %file.readLine();
			
			
			scheduleNoQuota(0,0,PTG_LoadServerPreset_Recurs,%fpPreset,"Setup",%relBioSO,%file);
			
			
		//////////////////////////////////////////////////
		
		case "Setup":
		
			%file.readLine();
			%file.readLine();
			
			PTG_MainSO_SPtmp.seed = mFloor(getSubStr(%file.readLine(),0,8)); //"mFloor" prevents float values for seed

			//Main Landscape
			if(isObject(%db = $uiNameTable[getField(%file.readLine(),0)]) && %db.getClassName() $= "fxDTSBrickData")
			{
				PTG_MainSO_SPtmp.brTer_DB = %db.getName();
					PTG_MainSO_SPtmp.brTer_XYsize = %db.brickSizeX * 0.5;
					PTG_MainSO_SPtmp.brTer_Zsize = %db.brickSizeZ * 0.2;
					PTG_MainSO_SPtmp.brTer_FillXYZSize = %db.brickSizeX * 0.5;
					
				PTG_MainSO_SPtmp.enabModTer = %file.readLine();
				
				PTG_MainSO_SPtmp.Bio_Def_TerPri = PTG_LSP_ConvertPrint(%file.readLine()) % %printNum;
				PTG_MainSO_SPtmp.Bio_Def_TerCol = PTG_FindClosestColor($tmpColRefArr[%file.readLine()],"RGBA-RGBAarr") % 64; //use mClamp instead of %???
			}
			else
			{
				if(%fpPreset !$= "" && %fpPreset !$= " ") //file path and extension should already be filtered our for non-default presets in func above (prior to this func)
				{
					%fp = PTG_GetFP("ServerPresetFP","","","");
					%fpPresetRef = %fpPreset;
				}
				else
				{
					//Check for custom default settings, otherwise use standard default settings if not found
					if(isFile("config/server/PTGv3/Default.txt"))
						%fp = "config/server/PTGv3/";
					else
						%fp = "Add-Ons/System_PTG/PRESETS/";
					
					%fpPresetRef = "Default";
				}
				%fileFP = %fp @ %fpPresetRef @ ".txt";
				
				messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6Server preset \"\c0" @ %fpPresetRef @ "\c6\" failed to load; terrain brick datablock couldn't be authenticated. <color:4400cc>[*]");
				echo("\c2>>P\c1T\c4G \c2ERROR: Server-sided preset \"" @ %fileFP @ "\" load aborted; terrain brick datablock couldn't be authenticated / wasn't found on server. [!] \c0->" SPC getWord(getDateTime(),1));
				
				PTG_LoadServerPreset(%fpPreset,%noFallDamage,%autoHalt,%autoClearFunc,%autoReset,"Cancel");
				return;
				
				//%file.readLine();
				//%file.readLine();
				//%file.readLine();
			}
			//PTG_MainSO_SPtmp.treeBaseCol = PTG_FindClosestColor($tmpColRefArr[%file.readLine()],"RGBA-RGBAarr") % 64;
				
			PTG_MainSO_SPtmp.terType = getWord("Normal SkyLands FlatLands NoTerrain",mClamp(%file.readLine(),0,3));
			PTG_MainSO_SPtmp.gridType = getWord("Square Radial",mClamp(%file.readLine(),0,1));
			PTG_MainSO_SPtmp.modTerGenType = getWord("CubesRamps CubesWedges Cubes",mClamp(%file.readLine(),0,2));
			PTG_MainSO_SPtmp.genMethod = getWord("Complete Gradual",mClamp(%file.readLine(),0,1)); //Checkbox / Boolean
			PTG_MainSO_SPtmp.enabAutoSave = mClamp(%file.readLine(),0,1);
			
			PTG_MainSO_SPtmp.gridStartX = getSubStr(%file.readLine(),0,8); //snap to chunksize after finding in Advanced settings
			PTG_MainSO_SPtmp.gridStartY = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.gridEndX = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.gridEndY = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.enabEdgeFallOff = mClamp(%file.readLine(),0,1);
			PTG_MainSO_SPtmp.edgeFOffDist = mClamp(mAbs(%file.readLine()),0,9999); //mclamp? //set character limit in GUI???
			
			PTG_MainSO_SPtmp.genType = getWord("Finite Infinite",mClamp(%file.readLine(),0,1));
			PTG_MainSO_SPtmp.chrad_P = mClamp(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.chrad_SA = mClamp(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.remDistChs = mClamp(%file.readLine(),0,1);


			scheduleNoQuota(0,0,PTG_LoadServerPreset_Recurs,%fpPreset,"Features",%relBioSO,%file);
			
		//////////////////////////////////////////////////
		
		case "Features":

			%file.readLine();
		
			PTG_MainSO_SPtmp.dirtPri = PTG_LSP_ConvertPrint(%file.readLine()) % %printNum;
			PTG_MainSO_SPtmp.dirtCol = PTG_FindClosestColor($tmpColRefArr[%file.readLine()],"RGBA-RGBAarr") % 64;
			PTG_MainSO_SPtmp.skylandsBtmPri = PTG_LSP_ConvertPrint(%file.readLine()) % %printNum;
			PTG_MainSO_SPtmp.skylandsBtmCol = PTG_FindClosestColor($tmpColRefArr[%file.readLine()],"RGBA-RGBAarr") % 64;
			
			PTG_MainSO_SPtmp.lakesHLevel = mClamp(mFloor(%file.readLine() / PTG_MainSO_SPtmp.brTer_Zsize) * PTG_MainSO_SPtmp.brTer_Zsize,0,800);
			PTG_MainSO_SPtmp.sandHLevel = mClamp(%file.readLine(),0,800);
			PTG_MainSO_SPtmp.terHLevel =  mClamp(mFloor(%file.readLine() / PTG_MainSO_SPtmp.brTer_Zsize) * PTG_MainSO_SPtmp.brTer_Zsize,0,400);
			PTG_MainSO_SPtmp.enabCnctLakes = %file.readLine();
				if(!PTG_MainSO_SPtmp.enabModTer || PTG_MainSO_SPtmp.modTerGenType $= "Cubes")
					PTG_MainSO_SPtmp.enabPlateCap = mClamp(%file.readLine(),0,1); //make sure plate-capping auto-disables when terrain ModTer is used
				else
				{
					%file.readLine();
					PTG_MainSO_SPtmp.enabPlateCap = false;
				}
			PTG_MainSO_SPtmp.dirtSameTer = mClamp(%file.readLine(),0,1);
			PTG_MainSO_SPtmp.shoreSameCustBiome = mClamp(%file.readLine(),0,1);
			PTG_MainSO_SPtmp.DisableWatGen = mClamp(%file.readLine(),0,1);
			
			PTG_MainSO_SPtmp.enabDetails = mClamp(%file.readLine(),0,1);
			PTG_MainSO_SPtmp.detailFreq = mClamp(%file.readLine(),0,100);
				if(PTG_MainSO_SPtmp.detailFreq <= 0)
					PTG_MainSO_SPtmp.enabDetails = false;
			PTG_MainSO_SPtmp.enabBio_CustA = mClamp(%file.readLine(),0,1);
			PTG_MainSO_SPtmp.enabBio_CustB = mClamp(%file.readLine(),0,1);
			PTG_MainSO_SPtmp.enabBio_CustC = mClamp(%file.readLine(),0,1);
			PTG_MainSO_SPtmp.autoHideSpawns = mClamp(%file.readLine(),0,1);
			PTG_MainSO_SPtmp.enabFltIsldDetails = mClamp(%file.readLine(),0,1);
			
			PTG_MainSO_SPtmp.enabMntns = mClamp(%file.readLine(),0,1);
			PTG_MainSO_SPtmp.enabMntnSnow = mClamp(%file.readLine(),0,1);
			PTG_MainSO_SPtmp.MntnsSnowHLevel = mClamp(%file.readLine(),0,800);
			PTG_MainSO_SPtmp.mntnsZSnap = mClamp(%file.readLine(),1,40);
			PTG_MainSO_SPtmp.mntnsZMult = mClamp(%file.readLine(),1,16); //mClamp floors automatically
			
			PTG_MainSO_SPtmp.enabCaves = mClamp(%file.readLine(),0,1);
			PTG_MainSO_SPtmp.cavesHLevel = mClamp(mFloor(%file.readLine() / PTG_MainSO_SPtmp.brTer_Zsize) * PTG_MainSO_SPtmp.brTer_Zsize,0,400);

			//Clouds
			PTG_MainSO_SPtmp.enabClouds = mClamp(%file.readLine(),0,1);
			
			if(isObject(%db = $uiNameTable[getField(%file.readLine(),0)]) && %db.getClassName() $= "fxDTSBrickData")
			{
				PTG_MainSO_SPtmp.brClouds_DB = %db.getName();
					PTG_MainSO_SPtmp.brClouds_XYsize = %db.brickSizeX * 0.5;
					PTG_MainSO_SPtmp.brClouds_Zsize = %db.brickSizeZ * 0.2;
					PTG_MainSO_SPtmp.brClouds_FillXYZSize = %db.brickSizeX * 0.5;

				//
				PTG_MainSO_SPtmp.enabModTer_Clouds = mClamp(%file.readLine(),0,1);
				
				PTG_MainSO_SPtmp.cloudsPri = PTG_LSP_ConvertPrint(%file.readLine()) % %printNum;
				PTG_MainSO_SPtmp.cloudsCol = PTG_FindClosestColor($tmpColRefArr[%file.readLine()],"RGBA-RGBAarr") % 64;
				PTG_MainSO_SPtmp.cloudsHLevel = mFloor(%file.readLine() / PTG_MainSO_SPtmp.brClouds_Zsize) * PTG_MainSO_SPtmp.brClouds_Zsize;
				PTG_MainSO_SPtmp.cloudsColl = mClamp(%file.readLine(),0,1);
				PTG_MainSO_SPtmp.modTerGenType_clouds = getWord("CubesRamps CubesWedges Cubes",mClamp(%file.readLine(),0,2));
			}
			else
			{
				echo("\c2>>P\c1T\c4G \c2ERROR: Cloud brick datablock for server preset couldn't be authenticated / wasn't found on server; preset load continuing... [!] \c0->" SPC getWord(getDateTime(),1));
				%file.readLine();
				%file.readLine();
				%file.readLine();
				%file.readLine();
				%file.readLine();
				%file.readLine();
			}
			

			//Floating Islands
			PTG_MainSO_SPtmp.enabFltIslds = mClamp(%file.readLine(),0,1);
			
			if(isObject(%db = $uiNameTable[getField(%file.readLine(),0)]) && %db.getClassName() $= "fxDTSBrickData")
			{
				PTG_MainSO_SPtmp.brFltIslds_DB = %db.getName();
					PTG_MainSO_SPtmp.brFltIslds_XYsize = %db.brickSizeX * 0.5;
					PTG_MainSO_SPtmp.brFltIslds_Zsize = %db.brickSizeZ * 0.2;
					PTG_MainSO_SPtmp.brFltIslds_FillXYZSize = %db.brickSizeX * 0.5;

				//
				PTG_MainSO_SPtmp.enabModTer_FltIslds = mClamp(%file.readLine(),0,1); //!!! if decide to use plate-capping for floating islands, auto disable if floating islands ModTer enabled (like terrain) !!!

				PTG_MainSO_SPtmp.fltIsldsTerPri = PTG_LSP_ConvertPrint(%file.readLine()) % %printNum;
				PTG_MainSO_SPtmp.fltIsldsTerCol = PTG_FindClosestColor($tmpColRefArr[%file.readLine()],"RGBA-RGBAarr") % 64;
				PTG_MainSO_SPtmp.fltIsldsAHLevel = mFloor(%file.readLine() / PTG_MainSO_SPtmp.brFltIslds_Zsize) * PTG_MainSO_SPtmp.brFltIslds_Zsize;
				PTG_MainSO_SPtmp.fltIsldsBHLevel = mFloor(%file.readLine() / PTG_MainSO_SPtmp.brFltIslds_Zsize) * PTG_MainSO_SPtmp.brFltIslds_Zsize;
				PTG_MainSO_SPtmp.fltIsldsDirtPri = PTG_LSP_ConvertPrint(%file.readLine()) % %printNum;
				PTG_MainSO_SPtmp.fltIsldsDirtCol = PTG_FindClosestColor($tmpColRefArr[%file.readLine()],"RGBA-RGBAarr") % 64;
				PTG_MainSO_SPtmp.fltIsldsBtmPri = PTG_LSP_ConvertPrint(%file.readLine()) % %printNum;
				PTG_MainSO_SPtmp.fltIsldsBtmCol = PTG_FindClosestColor($tmpColRefArr[%file.readLine()],"RGBA-RGBAarr") % 64;
				PTG_MainSO_SPtmp.modTerGenType_fltislds = getWord("CubesRamps CubesWedges Cubes",mClamp(%file.readLine(),0,2));
			}
			else
			{
				echo("\c2>>P\c1T\c4G \c2ERROR: Floating Islands brick datablock for server preset couldn't be authenticated / wasn't found on server; preset load continuing... [!] \c0->" SPC getWord(getDateTime(),1));
				%file.readLine();
				%file.readLine();
				%file.readLine();
				%file.readLine();
				%file.readLine();
				%file.readLine();
				%file.readLine();
				%file.readLine();
				%file.readLine();
				%file.readLine();
			}

			PTG_MainSO_SPtmp.enabBounds = mClamp(%file.readLine(),0,1);
			PTG_MainSO_SPtmp.boundsWallPri = PTG_LSP_ConvertPrint(%file.readLine()) % %printNum;
			PTG_MainSO_SPtmp.boundsWallCol = PTG_FindClosestColor($tmpColRefArr[%file.readLine()],"RGBA-RGBAarr") % 64;
			PTG_MainSO_SPtmp.boundsCeilPri = PTG_LSP_ConvertPrint(%file.readLine()) % %printNum;
			PTG_MainSO_SPtmp.boundsCeilCol = PTG_FindClosestColor($tmpColRefArr[%file.readLine()],"RGBA-RGBAarr") % 64;
			PTG_MainSO_SPtmp.boundsHTerRel = mClamp(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.boundsHLevel = mClamp((mFloor(%file.readLine() / 32) * 32),0,992);
			PTG_MainSO_SPtmp.boundsH_RelToTer = mClamp(%file.readLine(),0,1);
			PTG_MainSO_SPtmp.boundsH_RefTerOff = mClamp(%file.readLine(),0,1); //PTG_Cmplx_ChkBndsStrtRelOffset
			PTG_MainSO_SPtmp.boundsCeil = mClamp(%file.readLine(),0,1);
			PTG_MainSO_SPtmp.boundsStatic = mClamp(%file.readLine(),0,1);
			PTG_MainSO_SPtmp.boundsInvisStatic = mClamp(%file.readLine(),0,1);


			scheduleNoQuota(0,0,PTG_LoadServerPreset_Recurs,%fpPreset,"BuildLoad",%relBioSO,%file);
			
		//////////////////////////////////////////////////
		
		case "BuildLoad":

			%file.readLine();

			PTG_MainSO_SPtmp.enabBuildLoad = mClamp(%file.readLine(),0,1);
			PTG_MainSO_SPtmp.allowFlatAreas = mClamp(%file.readLine(),0,1);
			PTG_MainSO_SPtmp.allowDetFlatAreas = mClamp(%file.readLine(),0,1);
			PTG_MainSO_SPtmp.BldLdUseMaxHTer = mClamp(%file.readLine(),0,1);
			PTG_MainSO_SPtmp.flatAreaFreq = mClamp(%tmpFreq = %file.readLine(),0,100);
			
				if(%tmpFreq <= 0)
					PTG_MainSO_SPtmp.allowFlatAreas = false;
			
			//PTG_Cmplx_ChkGenDetFlatAreas
			PTG_MainSO_SPtmp.BLfaGridSizeSmall = mClamp(getWord("2 4 8 16 32 64 128 256",%file.readLine()),2,256);
			PTG_MainSO_SPtmp.BLfaGridSizeLarge = mClamp(getWord("2 4 8 16 32 64 128 256",%file.readLine()),2,256);
			//	PTG_MainSO_SPtmp.buildUploadCount = mClamp(%file.readLine(),0,400); //only accessed when uploading builds (as a fail-safe)


			scheduleNoQuota(0,0,PTG_LoadServerPreset_Recurs,%fpPreset,"BiomeDef",%relBioSO,%file);
		
		//////////////////////////////////////////////////
		
		case "BiomeDef" or "BiomeShore" or "BiomeSubM" or "BiomeCustA" or "BiomeCustB" or "BiomeCustC" or "BiomeCaveTop" or "BiomeCaveBtm" or "BiomeMntn":
		
			%bioStr = getSubStr(getWord(strReplace(%pass,"Biome",""),0),0,11); //bioStr doesn't get sent to eval command below unless it matches a case above, so it's secure (getWord and getSubStr used just encase)
			%file.readLine();

			//Mountains
			if(%pass $= "BiomeMntn")
			{
				PTG_BiomesSO_SPtmp.Bio_Mntn_RockPri = PTG_LSP_ConvertPrint(%file.readLine()) % %printNum;
				PTG_BiomesSO_SPtmp.Bio_Mntn_RockCol = PTG_FindClosestColor($tmpColRefArr[%file.readLine()],"RGBA-RGBAarr") % 64;
				
				PTG_BiomesSO_SPtmp.Bio_Mntn_SnowPri = PTG_LSP_ConvertPrint(%file.readLine()) % %printNum;
				PTG_BiomesSO_SPtmp.Bio_Mntn_SnowCol = PTG_FindClosestColor($tmpColRefArr[%file.readLine()],"RGBA-RGBAarr") % 64;
			}
			
			//Biomes and Caves
			else
			{
				if(%pass !$= "BiomeDef")
				{
					eval("PTG_BiomesSO_SPtmp.Bio_" @ %bioStr @ "_TerPri =  PTG_LSP_ConvertPrint(PTG_FilterChars(%file.readLine())) % %printNum;");  //evals are secure
					eval("PTG_BiomesSO_SPtmp.Bio_" @ %bioStr @ "_TerCol = PTG_FindClosestColor($tmpColRefArr[PTG_FilterChars(%file.readLine())],\"RGBA-RGBAarr\") % 64;");
				}
				
				if(%pass $= "BiomeDef" || %pass $= "BiomeCustA" || %pass $= "BiomeCustB" || %pass $= "BiomeCustC")
				{
					eval("PTG_BiomesSO_SPtmp.Bio_" @ %bioStr @ "_Wat_Pri =  PTG_LSP_ConvertPrint(PTG_FilterChars(%file.readLine())) % %printNum;");
					eval("PTG_BiomesSO_SPtmp.Bio_" @ %bioStr @ "_Wat_Col = PTG_FindClosestColor($tmpColRefArr[PTG_FilterChars(%file.readLine())],\"RGBA-RGBAarr\") % 64;");
					
					if(%pass !$= "BiomeDef")
						eval("PTG_BiomesSO_SPtmp.Bio_" @ %bioStr @ "_TerHMod = mClampF(PTG_FilterChars(%file.readLine()),0.25,8);");
					
					//eval("PTG_BiomesSO_SPtmp.Bio_" @ %bioStr @ "_AllowMntns = PTG_FilterChars(%file.readLine());"); //dropped for this version
					eval("PTG_BiomesSO_SPtmp.Bio_" @ %bioStr @ "_Wat_Type = getWord(\"Normal Lava Ice QuickSand\",mClamp(PTG_FilterChars(%file.readLine()),0,3));");
				}
			}
			
			for(%c = 0; %c < 18; %c++)
			{
				//Details

				if(isObject(%db = $uiNameTable[getField(%file.readLine(),0)]) && %db.getClassName() $= "fxDTSBrickData")
				{
					eval("PTG_BiomesSO_SPtmp.Bio_" @ %bioStr @ "_Det" @ %c @ "_BrDB = PTG_FilterChars(%db);"); //.getName()?
					eval("PTG_BiomesSO_SPtmp.Bio_" @ %bioStr @ "_Det" @ %c @ "_Pri = PTG_LSP_ConvertPrint(PTG_FilterChars(%file.readLine())) % %printNum;");
					eval("PTG_BiomesSO_SPtmp.Bio_" @ %bioStr @ "_Det" @ %c @ "_Col = PTG_FindClosestColor($tmpColRefArr[PTG_FilterChars(%file.readLine())],\"RGBA-RGBAarr\") % 64;");
				}
				else
				{
					%file.readLine();
					%file.readLine();
				}
			}
			
			%nxtFunc = "Advanced";
			
			//Grab values for next feature from client
			for(%c = 0; %c < 9; %c++)
			{
				if(getWord("BiomeDef BiomeShore BiomeSubM BiomeCustA BiomeCustB BiomeCustC BiomeCaveTop BiomeCaveBtm BiomeMntn",%c) $= %pass)
				{
					%nxtFunc = getWord("BiomeShore BiomeSubM BiomeCustA BiomeCustB BiomeCustC BiomeCaveTop BiomeCaveBtm BiomeMntn Advanced",%c);
					break;
				}
			}
			
			
			scheduleNoQuota(0,0,PTG_LoadServerPreset_Recurs,%fpPreset,%nxtFunc,%relBioSO,%file);
		
		//////////////////////////////////////////////////
		
		case "Advanced":

			%file.readLine();
			
			%ChSize = PTG_MainSO_SPtmp.chSize = getWord("16 32 64 128 256",mClamp(%file.readLine(),0,4)); //mClamp((mFloor(%file.readLine() / 16) * 16),16,256);
				PTG_MainSO_SPtmp.gridStartX = mFloor(PTG_MainSO_SPtmp.gridStartX / %ChSize) * %ChSize; //snap finite grid start/end pos to chunk size
				PTG_MainSO_SPtmp.gridStartY = mFloor(PTG_MainSO_SPtmp.gridStartY / %ChSize) * %ChSize;
				PTG_MainSO_SPtmp.gridEndX = mCeil(PTG_MainSO_SPtmp.gridEndX / %ChSize) * %ChSize; //snap to ceiling value for XY end positions, also relative to chunk size
				PTG_MainSO_SPtmp.gridEndY = mCeil(PTG_MainSO_SPtmp.gridEndY / %ChSize) * %ChSize;
			PTG_MainSO_SPtmp.caveTopZMult = mClamp(%file.readLine(),1,8);
			PTG_MainSO_SPtmp.zMod = mFloor(mClamp(mAbs(%file.readLine()),0,200) / 2) * 2;
			PTG_MainSO_SPtmp.cnctLakesStrt = mClamp(mAbs(%file.readLine()),1,200);
			PTG_MainSO_SPtmp.TreeBaseACol = PTG_FindClosestColor($tmpColRefArr[%file.readLine()],"RGBA-RGBAarr") % 64;
			PTG_MainSO_SPtmp.TreeBaseBCol = PTG_FindClosestColor($tmpColRefArr[%file.readLine()],"RGBA-RGBAarr") % 64;
			PTG_MainSO_SPtmp.TreeBaseCCol = PTG_FindClosestColor($tmpColRefArr[%file.readLine()],"RGBA-RGBAarr") % 64;
			PTG_MainSO_SPtmp.FIFOchClr = mClamp(%file.readLine(),0,1);
			PTG_MainSO_SPtmp.seamlessModTer = mClamp(%file.readLine(),0,1);
			PTG_MainSO_SPtmp.seamlessBuildL = mClamp(%file.readLine(),0,1);


			PTG_MainSO_SPtmp.terOff_X = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.terOff_y = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.mntnsOff_X = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.mntnsOff_Y = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.bio_CustAOff_X = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.bio_CustAOff_Y = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.bio_CustBOff_X = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.bio_CustBOff_Y = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.bio_CustCOff_X = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.bio_CustCOff_Y = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.caveAOff_X = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.caveAOff_Y = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.caveBOff_X = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.caveBOff_Y = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.fltIsldsOff_X = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.fltIsldsOff_Y = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.fltIsldsBOff_X = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.fltIsldsBOff_Y = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.skyLandsOff_X = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.skyLandsOff_Y = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.detailsOff_X = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.detailsOff_Y = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.cloudsOff_X = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.cloudsOff_Y = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.buildLoad_X = getSubStr(%file.readLine(),0,8);
			PTG_MainSO_SPtmp.buildLoad_Y = getSubStr(%file.readLine(),0,8);
			
			
			PTG_MainSO_SPtmp.cavesSecZ = getSubStr(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.skyLndsSecZ = getSubStr(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.fltIsldsSecZ = getSubStr(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.bio_CustASecZ = getSubStr(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.bio_CustBSecZ = getSubStr(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.bio_CustCSecZ = getSubStr(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.cloudsSecZ = getSubStr(mAbs(%file.readLine()),0,8);
			//PTG_MainSO_SPtmp.cntLakesSecZ = mAbs(%file.readLine());
			PTG_MainSO_SPtmp.mntnsStrtSecZ = getSubStr(mAbs(%file.readLine()),0,8);

			
			%tmpBrSize = PTG_MainSO_SPtmp.brTer_XYsize;
			
			PTG_MainSO_SPtmp.ter_itrA_XY = mClamp(mFloor(mAbs(%file.readLine()) / %ChSize) * %ChSize,0,16384); //0 is minimum clamp value to alert player of issue if value is too small
			PTG_MainSO_SPtmp.ter_itrA_Z = mClampF(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.ter_itrB_XY = mClamp(mFloor(mAbs(%file.readLine()) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_SPtmp.ter_itrB_Z = mClampF(mAbs(%file.readLine()),0,8);
			
			if((%ItrCxy = mAbs(%file.readLine())) > %ChSize)
				%ItrCxy = %ChSize;
			else
			{
				%failsafe = 0;
				
				for(%tmpItr = %ChSize; %tmpItr >= 1; %tmpItr /= 2)
				{
					if(%failsafe++ > 16 || %tmpItr <= %tmpBrSize || %tmpItr == 1)
					{
						%ItrCxy = %ChSize;
						break;
					}
					if(%tmpItr == %ItrCxy)
					{
						%ItrCxy = %tmpItr;
						break;
					}
					else if(%tmpItr < %ItrCxy)
					{
						%ItrCxy = %tmpItr * 2;
						break;
					}
				}
			}
			PTG_MainSO_SPtmp.ter_itrC_XY = mClamp(%ItrCxy,0,16384); //since itrC can be < chSize, snap to terrain brick size instead (if issue w/ brick datablock, this will be 0)

			PTG_MainSO_SPtmp.ter_itrC_Z = mClampF(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.mntn_itrA_XY = mClamp(mFloor(mAbs(%file.readLine()) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_SPtmp.mntn_itrA_Z = mClampF(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.mntn_itrB_XY = mClamp(mFloor(mAbs(%file.readLine()) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_SPtmp.mntn_itrB_Z = mClampF(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.caveA_itrA_XY = mClamp(mFloor(mAbs(%file.readLine()) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_SPtmp.caveA_itrA_Z = mClampF(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.caveA_itrB_XY = mClamp(mFloor(mAbs(%file.readLine()) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_SPtmp.caveA_itrB_Z = mClampF(mAbs(%file.readLine()),0,8);
			
			if((%ItrCxy = mAbs(%file.readLine())) > %ChSize)
				%ItrCxy = %ChSize;
			else
			{
				%failsafe = 0;
				
				for(%tmpItr = %ChSize; %tmpItr >= 1; %tmpItr /= 2)
				{
					if(%failsafe++ > 16 || %tmpItr <= %tmpBrSize || %tmpItr == 1)
					{
						%ItrCxy = %ChSize;
						break;
					}
					if(%tmpItr == %ItrCxy)
					{
						%ItrCxy = %tmpItr;
						break;
					}
					else if(%tmpItr < %ItrCxy)
					{
						%ItrCxy = %tmpItr * 2;
						break;
					}
				}
			}
			PTG_MainSO_SPtmp.caveA_itrC_XY = mClamp(%ItrCxy,0,16384); //also snaps to terrain brick size
			
			PTG_MainSO_SPtmp.caveA_itrC_Z = mClampF(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.caveB_itrA_XY = mClamp(mFloor(mAbs(%file.readLine()) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_SPtmp.caveB_itrA_Z = mClampF(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.bio_CustA_itrA_XY = mClamp(mFloor(mAbs(%file.readLine()) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_SPtmp.bio_CustA_itrA_Z = mClampF(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.bio_CustB_itrA_XY = mClamp(mFloor(mAbs(%file.readLine()) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_SPtmp.bio_CustB_itrA_Z = mClampF(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.bio_CustC_itrA_XY = mClamp(mFloor(mAbs(%file.readLine()) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_SPtmp.bio_CustC_itrA_Z = mClampF(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.skyLnds_itrA_XY = mClamp(mFloor(mAbs(%file.readLine()) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_SPtmp.skyLnds_itrA_Z = mClampF(mAbs(%file.readLine()),0,8);
			
			PTG_MainSO_SPtmp.clouds_itrA_XY = mClamp(mFloor(mAbs(%file.readLine()) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_SPtmp.clouds_itrA_Z = mClampF(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.clouds_itrB_XY = mClamp(mFloor(mAbs(%file.readLine()) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_SPtmp.clouds_itrB_Z = mClampF(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.fltIslds_itrA_XY = mClamp(mFloor(mAbs(%file.readLine()) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_SPtmp.fltIslds_itrA_Z = mClampF(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.fltIslds_itrB_XY = mClamp(mFloor(mAbs(%file.readLine()) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_SPtmp.fltIslds_itrB_Z = mClampF(mAbs(%file.readLine()),0,8);
			
			PTG_MainSO_SPtmp.enabPseudoEqtr = getSubStr(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.Bio_CustA_YStart = getSubStr(mAbs(%file.readLine()),0,8); //changed from PTG_BiomesSO_SPtmp
			PTG_MainSO_SPtmp.Bio_CustB_YStart = getSubStr(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.Bio_CustC_YStart = getSubStr(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.Cave_YStart = getSubStr(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.Mntn_YStart = getSubStr(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.Cloud_YStart = getSubStr(mAbs(%file.readLine()),0,8);
			PTG_MainSO_SPtmp.FltIsld_YStart = getSubStr(mAbs(%file.readLine()),0,8);
			
			
			%file.readLine();
			
			$PTG_massDetCurrBiome = "Default";
			$PTG_massDetCurrNum = 0; //+= 56
			$PTG_massDetActCount = 0;
			%nextRelBio = "Shore";
			
			for(%c = 0; %c < 9; %c++)
			{
				%tmpBioN = getWord("Default Shore SubMarine CustomA CustomB CustomC CaveTop CaveBottom Mountains",%c);
				
				if(isObject(%tmpBioSO = "PTG_MassBioDetails_" @ %tmpBioN))
					%tmpBioSo.delete(); //".clear()" doesn't work for clearing fields
			}
			
			
			//Clear previous biome mass detail script objects
			for(%c = 0; %c < 9; %c++)
			{
				%tmpBioN = getWord("Default Shore SubMarine CustomA CustomB CustomC CaveTop CaveBottom Mountains",%c);
				
				if(isObject(%tmpBioSO = "PTG_MassBioDetails_" @ %tmpBioN))
					%tmpBioSo.delete(); //".clear()" doesn't work for clearing fields
			}
			
			scheduleNoQuota(0,0,PTG_LoadServerPreset_Recurs,%fpPreset,"MassDetails",%relBioSO,%file);
		
		//////////////////////////////////////////////////
		
		case "MassDetails":
		
		//only upload if mass details for a certain biome is enabled, otherwise skip that biome...

		//%file.readLine();
		//if(%file.readLine() $= "" || %file.readLine() $= " ") %file.readLine() = "NULL";
		//if(%file.readLine() $= "" || %file.readLine() $= " ") %file.readLine() = "NULL";
		//if(%file.readLine() $= "" || %file.readLine() $= " ") %file.readLine() = "NULL";
		//if(%file.readLine() $= "" || %file.readLine() $= " ") %file.readLine() = "NULL";
		
		while(!(%readLn = %file.readLine()).isEOF && %MDfailSafe++ <= 3609)
		{			
			switch$(%relbioN = getField(%readLn,0))
			{
				case ">>Default" or ">>Shore" or ">>SubMarine" or ">>CustomA" or ">>CustomB" or ">>CustomC" or ">>CaveTop" or ">>CaveBottom" or ">>Mountains":
				
					//Create new script object for biome, if mass details are enabled for that biome 
						//Setting upload function creates script object based on number of details and if enabled
					if(!isObject(%relBioSOn = "PTG_MassBioDetails_" @ strReplace(%relbioN,">>","")) && getField(%readLn,1))
					{
						%relBioSO = new SimGroup(%relBioSOn);
						MissionCleanup.add(%relBioSO);
					}
					else
						%relBioSO = "";
					//%massDetActCount = 0;

					//Take a break at beginning of each biome list to free up server resources  (otherwise, numerous details could cause lag spikes)
					scheduleNoQuota(0,0,PTG_LoadServerPreset_Recurs,%fpPreset,"MassDetails",%relBioSO,%file);
					return;
					
				case ">END":
				
					break;
				
				default:

					if((%tmpBr = $uiNameTable[getField(%readLn,0)]) !$= "" && isObject(%tmpBr) && isObject(%relBioSO))
					{
						//Add string data to fields on appropriate script objects
						%relBioSO.detail[%massDetActCount] = %tmpBr SPC PTG_LSP_ConvertPrint(getField(%readLn,4)) % %printNum SPC PTG_FindClosestColor(getColorIDTable(getField(%readLn,3)),"RGBA-RGBAarr");
						%massDetActCount++;
						%relBioSO.totalDetAm++;
					}
			}
		}
		

		if(%fpPreset $= "" || %fpPreset $= " ")
			scheduleNoQuota(0,0,PTG_LoadServerPreset_Recurs,%fpPreset,"Routines",%relBioSO,%file);
		else
			scheduleNoQuota(0,0,PTG_LoadServerPreset_Recurs,%fpPreset,"ErrorCheck",%relBioSO,%file);
		
		//////////////////////////////////////////////////
		
		case "Routines": // or "RoutinesOnly":

			MissionCleanup.add(new ScriptObject(PTG_GlobalSO_SPtmp));
			
			if(!isFile(%fileFP = "config/server/PTGv3/Routines.txt")) //search for a custom routines save first
			{
				%fileFP = "Add-Ons/System_PTG/PRESETS/Routines.txt"; //use default settings if custom save wasn't found for routines
				if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0No custom routine settings found, using default routine settings... \c4[*] \c0->" SPC getWord(getDateTime(),1));
			}
			else if($PTG.allowEchos)
				echo("\c4>>\c2P\c1T\c4G: \c0Loading custom routine settings... \c4[*] \c0->" SPC getWord(getDateTime(),1));
			
			%fileB = new FileObject();
			%fileB.openForRead(%fileFP);
			%fileB.readLine();
			
			if(isObject(PTG_GlobalSO))
			{
				//Carry over auxiliary fields (used for storing server instance data) when new routine settings are uploaded
				PTG_GlobalSO_SPtmp.routine_StartMS = $PTG.routine_StartMS;
				PTG_GlobalSO_SPtmp.routine_isHalting = $PTG.routine_isHalting;
				PTG_GlobalSO_SPtmp.routine_Process = $PTG.routine_Process;
				PTG_GlobalSO_SPtmp.routine_ProcessAux = $PTG.routine_ProcessAux;
				PTG_GlobalSO_SPtmp.routine_ProcessAm = $PTG.routine_ProcessAm;
						PTG_GlobalSO_SPtmp.zMax = 4000; //mClamp($PTG.zMax,0,4000);
				PTG_GlobalSO_SPtmp.lastSetUploadTime = $PTG.lastSetUploadTime;
				PTG_GlobalSO_SPtmp.lastSetUploadPlayer = $PTG.lastSetUploadPlayer;
				PTG_GlobalSO_SPtmp.lastSetUploadID = $PTG.lastSetUploadID;
				PTG_GlobalSO_SPtmp.uploadingSettings = $PTG.uploadingSettings;
				
				PTG_GlobalSO_SPtmp.lastBuildUploadPlayer = $PTG.lastBuildUploadPlayer;
				PTG_GlobalSO_SPtmp.lastBuildUploadID = $PTG.lastBuildUploadID;
				PTG_GlobalSO_SPtmp.lastBuildUploadTime = $PTG.lastBuildUploadTime;
				PTG_GlobalSO_SPtmp.BuildUploading = $PTG.BuildUploading;
				PTG_GlobalSO_SPtmp.ForceCancelBldUpld = $PTG.ForceCancelBldUpld;
				PTG_GlobalSO_SPtmp.UploadBuildName = $PTG.UploadBuildName;
				
				PTG_GlobalSO_SPtmp.ListingBuild = $PTG.ListingBuild;
				PTG_GlobalSO_SPtmp.lastBuildListPlayer = $PTG.lastBuildListPlayer;
				PTG_GlobalSO_SPtmp.lastBuildListID = $PTG.lastBuildListID;
				PTG_GlobalSO_SPtmp.lastBuildListTime = $PTG.lastBuildListTime;
			
				PTG_GlobalSO_SPtmp.dedSrvrFuncCheckTime = $PTG.dedSrvrFuncCheckTime;
				PTG_GlobalSO_SPtmp.lastClientName = $PTG.lastClientName; //not currently used
				PTG_GlobalSO_SPtmp.lastClientID = $PTG.lastClientID;
				PTG_GlobalSO_SPtmp.lastClientisLocal = $PTG.lastClientisLocal;
				PTG_GlobalSO_SPtmp.lastClientisAdmin = $PTG.lastClientisAdmin; //???
				PTG_GlobalSO_SPtmp.lastClientisSuperAdmin = $PTG.lastClientisSuperAdmin; //???
				PTG_GlobalSO_SPtmp.lastClientisHost = $PTG.lastClientisHost; //???
				PTG_GlobalSO_SPtmp.lastClientPtgver = $PTG.lastClientPtgver;
				PTG_GlobalSO_SPtmp.lastClient = $PTG.lastClient;
				PTG_GlobalSO_SPtmp.lastUploadClient = $PTG.lastUploadClient;
				
				PTG_GlobalSO_SPtmp.massDetCurrBiome = $PTG.massDetCurrBiome;
				PTG_GlobalSO_SPtmp.massDetCurrNum = $PTG.massDetCurrNum;
			}
			else
			{
				PTG_GlobalSO_SPtmp.routine_isHalting = false; //PTG_GlobalSO_SPtmp. ???
				PTG_GlobalSO_SPtmp.routine_Process = "None"; //PTG_GlobalSO_SPtmp. ???
				PTG_GlobalSO_SPtmp.routine_ProcessAux = "None"; //PTG_GlobalSO_SPtmp. ???
				PTG_GlobalSO_SPtmp.zMax = 4000;	//PTG_GlobalSO_SPtmp.zMax = 4000; //2000 //400; //auto-calculate?
				PTG_GlobalSO_SPtmp.uploadingSettings = false;
				
				PTG_GlobalSO_SPtmp.ForceCancelBldUpld = false; //PTG_GlobalSO_SPtmp. ???
				PTG_GlobalSO_SPtmp.BuildUploading = false; //PTG_GlobalSO_SPtmp. ???
				PTG_GlobalSO_SPtmp.ListingBuild = false; //PTG_GlobalSO_SPtmp. ???
				PTG_GlobalSO_SPtmp.UploadBuildName = ""; //PTG_GlobalSO_SPtmp. ???
			}
			
			%tmpHLColA = %fileB.readLine();
			%tmpHLColB = %fileB.readLine();
			
			%fileB.readLine();
			
			PTG_GlobalSO_SPtmp.enabStreams = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.streamsTickMS = mClamp(%fileB.readLine(),33,2013);
			PTG_GlobalSO_SPtmp.StreamsHostOnly = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.solidStreams = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.streamsClrDetails = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.streamsMaxDist = mClamp(%fileB.readLine(),0,8);
			PTG_GlobalSO_SPtmp.genStreamZones = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.floatStreams = mClamp(%fileB.readLine(),0,1);
			
			%fileB.readLine();
			
			PTG_GlobalSO_SPtmp.brLimitBuffer = mClamp(%fileB.readLine(),0,20000);
			PTG_GlobalSO_SPtmp.pingMaxBuffer = mClamp(%fileB.readLine(),100,1000);
			PTG_GlobalSO_SPtmp.DedSrvrFuncBuffer = mClamp(%fileB.readLine(),20,2000);
			PTG_GlobalSO_SPtmp.chObjLimit = mClamp(%fileB.readLine(),20,4000);
			PTG_GlobalSO_SPtmp.chSaveLimit_FilesPerSeed = mClamp(%fileB.readLine(),0,100000);
			PTG_GlobalSO_SPtmp.chSaveLimit_TotalFiles = mClamp(%fileB.readLine(),0,100000);
			PTG_GlobalSO_SPtmp.buildLoadFileLimit = mClamp(%fileB.readLine(),0,400);
			PTG_GlobalSO_SPtmp.disBrBuffer = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.disChBuffer = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.disNormLagCheck = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.disDedLagCheck = mClamp(%fileB.readLine(),0,1);
			
			%fileB.readLine();
			
			PTG_GlobalSO_SPtmp.delay_PauseTickS = mClamp(%fileB.readLine(),1,30);
			PTG_GlobalSO_SPtmp.schedM_autosave = mClamp(%fileB.readLine(),1,60);
			PTG_GlobalSO_SPtmp.delay_priFuncMS = mClamp(%fileB.readLine(),0,100);
			PTG_GlobalSO_SPtmp.delay_secFuncMS = mClamp( %fileB.readLine(),0,100);
			PTG_GlobalSO_SPtmp.calcDelay_priFuncMS = mClamp(%fileB.readLine(),0,100);
			PTG_GlobalSO_SPtmp.calcDelay_secFuncMS = mClamp(%fileB.readLine(),0,100);
			PTG_GlobalSO_SPtmp.brDelay_genMS = mClamp(%fileB.readLine(),0,50);
			PTG_GlobalSO_SPtmp.brDelay_remMS = mClamp(%fileB.readLine(),0,50);
			PTG_GlobalSO_SPtmp.genSpeed = mClamp(%fileB.readLine(),0,2);
			
			%fileB.readLine();
			
			PTG_GlobalSO_SPtmp.frcBrIntoChunks = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.AutoCreateChunks = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.chEditBrPlant = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.chEditOnWrenchData = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.chEditBrPPD = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.chStcBrSpwnPlnt = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.LoadChFileStc = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.chSaveMethod = getWord("ifEdited Always Never",mClamp(%fileB.readLine(),0,2));
			PTG_GlobalSO_SPtmp.chSaveExcdResp = getWord("RemoveOld DontSave",mClamp(%fileB.readLine(),0,1));
			
			%fileB.readLine();
			
			PTG_GlobalSO_SPtmp.publicBricks = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.DestroyPublicBr = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.publicBricksPBLs = mClamp(%fileB.readLine(),0,1);
			%fileB.readLine(); //skip "hide ghosting" option
			PTG_GlobalSO_SPtmp.allowEchos = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.PreventDestDetail = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.PreventDestTerrain = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.PreventDestBounds = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.allowNonHost_BuildManage = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.allowNOnHost_SetUpload = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.allowNH_SrvrCmdEventUse = mClamp(%fileB.readLine(),0,1);
			PTG_GlobalSO_SPtmp.AllowPlyrPosChk = mClamp(%fileB.readLine(),0,1);
			
			PTG_GlobalSO_SPtmp.fontSize = mClamp(%test = %fileB.readLine(),1,30);
			
			PTG_GlobalSO_SPtmp.ChunkHLACol = PTG_FindClosestColor(%tmpHLColA,"RGBA-RGBAarr") % 64;
			PTG_GlobalSO_SPtmp.ChunkHLBCol = PTG_FindClosestColor(%tmpHLColB,"RGBA-RGBAarr") % 64;
			
			
			%fileB.close();
			%fileB.delete();
			
			scheduleNoQuota(0,0,PTG_LoadServerPreset_Recurs,%fpPreset,"ErrorCheck",%relBioSO,%file);
		
		//////////////////////////////////////////////////
			
		case "ErrorCheck":
		
			//Grid Size Check (Finite Terrain)
			if(PTG_MainSO_SPtmp.genType $= "Finite")
			{
				if(PTG_MainSO_SPtmp.gridStartX >= PTG_MainSO_SPtmp.gridEndX)
				{
					echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0The Grid start position for the X-Axis must be < the grid end position for X.");
					%fail = true;
				}
				if(PTG_MainSO_SPtmp.gridStartY >= PTG_MainSO_SPtmp.gridEndY)
				{
					echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0The Grid start position for the Y-Axis must be < the grid end position for Y.");
					%fail = true;
				}
				
				//if(%fail) //for external console when hosting a dedicated server
				//{
					//echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0Grid size check failed; server preset load aborted! [!] \c0->" SPC getWord(getDateTime(),1));
				//	PTG_LoadServerPreset(%fpPreset,%noFallDamage,%autoHalt,%autoClearFunc,%autoReset,"Cancel"); //grab variables???
				//	return;
				//}
			}
			
			//Chunk Size Check
			//if(!%fail)
			//{
				if(PTG_MainSO_SPtmp.chSize <= PTG_MainSO_SPtmp.brTer_XYsize)
				{
					echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0The chunk size you have chosen must be > the terrain brick size; please choose a larger chunk size.");
					%fail = true;
				}
				if(PTG_MainSO_SPtmp.chSize <= PTG_MainSO_SPtmp.brClouds_XYsize)
				{
					echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0The chunk size you have chosen must be > the clouds brick size; please choose a larger chunk size.");
					%fail = true;
				}
				if(PTG_MainSO_SPtmp.chSize <= PTG_MainSO_SPtmp.brFltIslds_XYsize)
				{
					echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0The chunk size you have chosen must be > the floating islands brick size; please choose a larger chunk size.");
					%fail = true;
				}
			//}
			
			//Datablocks Check
			//if(!%fail)
			//{
				if(!isObject(PTG_MainSO_SPtmp.brTer_DB))
				{
					if(PTG_MainSO_SPtmp.enabModTer)
						echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0Brick datablock for \"Terrain\" doesn't exist! Either the 4x ModTer pack or both the Basic and Inv ModTer packs are disabled.");
					else
						echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0Brick datablock for \"Terrain\" doesn't exist!");
					
					%fail = true; //what if multiple error check fails?
				}
				if(PTG_MainSO_SPtmp.enabClouds && !isObject(PTG_MainSO_SPtmp.brClouds_DB))
				{
					if(PTG_MainSO_SPtmp.enabModTer_Clouds)
						echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0Brick datablock for \"Clouds\" doesn't exist! Either the 4x ModTer pack or both the Basic and Inv ModTer packs are disabled.");
					else
						echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0Brick datablock for \"Clouds\" doesn't exist!");
					
					%fail = true;
				}
				if(PTG_MainSO_SPtmp.enabFltIslds && !isObject(PTG_MainSO_SPtmp.brFltIslds_DB))
				{
					if(PTG_MainSO_SPtmp.enabModTer_FltIslds)
						echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0Brick datablock for \"Floating Islands\" doesn't exist! Either the 4x ModTer pack or both the Basic and Inv ModTer packs are disabled.");
					else
						echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0Brick datablock for \"Floating Islands\" doesn't exist!");
					
					%fail = true;
				}
				
				//if(%fail) //for external console when hosting a dedicated server
				//{
					//echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0Brick datablocks check failed; server preset load aborted! [!] \c0->" SPC getWord(getDateTime(),1));
				//	PTG_LoadServerPreset(%fpPreset,%noFallDamage,%autoHalt,%autoClearFunc,%autoReset,"Cancel"); //grab variables???
				//	return;
				//}
			//}
			
			//Biome (Relative to Terrain) Scales Check
			//if(!%fail)
			//{
				if(PTG_MainSO_SPtmp.enabBio_CustA && PTG_MainSO_SPtmp.bio_CustA_itrA_XY < PTG_MainSO_SPtmp.chSize) //custom biome A
				{
					echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0The biome XY noise scale for \"Custom Biome A\" must be >= the chunk size!");
					%fail = true;
				}
				if(PTG_MainSO_SPtmp.enabBio_CustB && PTG_MainSO_SPtmp.bio_CustB_itrA_XY < PTG_MainSO_SPtmp.chSize) //custom biome B
				{
					echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0The biome XY noise scale for \"Custom Biome B\" must be >= the chunk size!");
					%fail = true;
				}
				if(PTG_MainSO_SPtmp.enabBio_CustC && PTG_MainSO_SPtmp.bio_CustC_itrA_XY < PTG_MainSO_SPtmp.chSize) //custom biome C
				{
					echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0The biome XY noise scale for \"Custom Biome C\" must be >= the chunk size!");
					%fail = true;
				}
				
				//if(%fail) //for external console when hosting a dedicated server
				//{
					//echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0Biome scales check failed; server preset load aborted! [!] \c0->" SPC getWord(getDateTime(),1));
				//	PTG_LoadServerPreset(%fpPreset,%noFallDamage,%autoHalt,%autoClearFunc,%autoReset,"Cancel"); //grab variables???
				//	return;
				//}
			//}
			
			//Noise Scales Check
			//if(!%fail)
			//{
				if(PTG_MainSO_SPtmp.ter_itrA_XY < PTG_MainSO_SPtmp.ter_itrB_XY || PTG_MainSO_SPtmp.ter_itrA_XY < PTG_MainSO_SPtmp.chSize || PTG_MainSO_SPtmp.ter_itrB_XY < PTG_MainSO_SPtmp.chSize || PTG_MainSO_SPtmp.ter_itrC_XY > PTG_MainSO_SPtmp.chSize) //terrain
				{				
					echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0XY noise scales for \"Terrain\" must follow the following rule: Iteration A >= Iteration B >= Chunk Size >= Iteration C. Example: 256 >= 64 >= 32 >= 16");
					%fail = true;
				}
				if(PTG_MainSO_SPtmp.enabMntns && (PTG_MainSO_SPtmp.mntn_itrA_XY < PTG_MainSO_SPtmp.mntn_itrB_XY || PTG_MainSO_SPtmp.mntn_itrA_XY < PTG_MainSO_SPtmp.chSize || PTG_MainSO_SPtmp.mntn_itrB_XY < PTG_MainSO_SPtmp.chSize)) //mountains
				{
					echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0XY noise scales for \"Mountains\" must follow the following rule: Iteration A >= Iteration B >= Chunk Size. Example: 256 >= 64 >= 32");
					%fail = true;
				}
				if(PTG_MainSO_SPtmp.enabCaves)
				{
					if(PTG_MainSO_SPtmp.caveA_itrA_XY < PTG_MainSO_SPtmp.caveA_itrB_XY || PTG_MainSO_SPtmp.caveA_itrA_XY < PTG_MainSO_SPtmp.chSize || PTG_MainSO_SPtmp.caveA_itrB_XY < PTG_MainSO_SPtmp.chSize || PTG_MainSO_SPtmp.caveA_itrC_XY > PTG_MainSO_SPtmp.chSize) //caves
					{
						echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0XY noise scales for \"Caves\" must follow the following rule: Iteration A >= Iteration B >= Chunk Size >= Iteration C. Example: 128 >= 64 >= 32 >= 32");
						%fail = true;
					}
					if(PTG_MainSO_SPtmp.caveB_itrA_XY < PTG_MainSO_SPtmp.chSize) //caves height mod
					{
						echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0XY noise scales for \"Caves Height Mod\" must follow the following rule: Iteration A >= Chunk Size. Example: 256 >= 32");
						%fail = true;
					}
				}
				if((PTG_MainSO_SPtmp.enabBio_CustA && PTG_MainSO_SPtmp.bio_CustA_itrA_XY < PTG_MainSO_SPtmp.ter_itrA_XY) || (PTG_MainSO_SPtmp.enabBio_CustB && PTG_MainSO_SPtmp.bio_CustB_itrA_XY < PTG_MainSO_SPtmp.ter_itrA_XY) || (PTG_MainSO_SPtmp.enabBio_CustC && PTG_MainSO_SPtmp.bio_CustC_itrA_XY < PTG_MainSO_SPtmp.ter_itrA_XY))//custom biomes
				{
					echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0XY noise scales for \"Custom Biomes\" must follow the following rule: Biome Iteration A >= Terrain Iteration A. Example: 512 >= 256");
					%fail = true;
				}
				if(PTG_MainSO_SPtmp.terType $= "Skylands" && PTG_MainSO_SPtmp.skyLnds_itrA_XY < PTG_MainSO_SPtmp.chSize) //skylands terrain height mod
				{
					echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0XY noise scales for \"Skylands Height Mod\" must follow the following rule: Iteration A >= Chunk Size. Example: 128 >= 32");
					%fail = true;
				}
				if(PTG_MainSO_SPtmp.enabClouds && (PTG_MainSO_SPtmp.clouds_itrA_XY < PTG_MainSO_SPtmp.clouds_itrB_XY || PTG_MainSO_SPtmp.clouds_itrA_XY < PTG_MainSO_SPtmp.chSize || PTG_MainSO_SPtmp.clouds_itrB_XY < PTG_MainSO_SPtmp.chSize)) //clouds
				{
					echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0XY noise scales for \"Clouds\" must follow the following rule: Iteration A >= Iteration B >= Chunk Size. Example: 128 >= 32 >= 32");
					%fail = true;
				}
				if(PTG_MainSO_SPtmp.enabFltIslds && (PTG_MainSO_SPtmp.fltIslds_itrA_XY < PTG_MainSO_SPtmp.fltIslds_itrB_XY || PTG_MainSO_SPtmp.fltIslds_itrA_XY < PTG_MainSO_SPtmp.chSize || PTG_MainSO_SPtmp.fltIslds_itrB_XY < PTG_MainSO_SPtmp.chSize)) //floating islands
				{
					echo("\c2>>P\c1T\c4G \c2Server Preset Load ERROR: \c0XY noise scales for \"Floating Islands\" must follow the following rule: Iteration A >= Iteration B >= Chunk Size. Example: 64 >= 32 >= 32");
					%fail = true;
				}	
			//}
			
			if(%fail) //for external console when hosting a dedicated server
			{
				echo("\c2>>P\c1T\c4G \c2Server preset error check failed; load aborted. [*] \c0->" SPC getWord(getDateTime(),1));
				PTG_LoadServerPreset(%fpPreset,%noFallDamage,%autoHalt,%autoClearFunc,%autoReset,"Cancel"); //grab variables???

				%file.close();
				%file.delete();
				
				return;
			}			
			
			%file.close();
			%file.delete();
			
			PTG_LoadServerPreset(getField(PTG_MainSO_SPtmp.srvrPresData,0),getField(PTG_MainSO_SPtmp.srvrPresData,1),getField(PTG_MainSO_SPtmp.srvrPresData,2),getField(PTG_MainSO_SPtmp.srvrPresData,3),getField(PTG_MainSO_SPtmp.srvrPresData,4),"Finalize");
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//// FIND PRINT ID FROM PRINT UI NAME ////


function PTG_LSP_ConvertPrint(%printUIN)
{
	if((%printID = $tmpPriRefArr[%printUIN]) !$= "")
		return %printID;

	%priCount = $globalPrintCount; //"getNumPrintTextures()" doesn't seem to update when prints are disabled

	for(%c = 0; %c < %priCount; %c++)
	{
		%tmpPriTex = getPrintTexture(%c);
		%priRatio = strReplace(%tmpPriTex,"/"," ");
		%priRatio = strReplace(%priRatio,"_"," ");
		%priRatio = getWord(%priRatio,2);
		
		%tmpPriUIN = %priRatio @ "/" @ fileBase(%tmpPriTex);

		if(%tmpPriUIN $= %printUIN)
		{
			$tmpPriRefArr[%printUIN] = %c;
			return %c;
		}
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//// FILTER OUT CHARACTERS FROM A STRING THAT COULD COMPROMISE SECURITY VIA EVALS ////


function PTG_FilterChars(%dataStr)
{
	for(%c = 0; %c < strLen(%dataStr); %c++)
	{
		%strPass = false;
		
		for(%d = 0; %d < 13 && !%strPass; %d++)
		{
			if((%tmpChar = getSubStr(%dataStr,%c,1)) $= getSubStr(".0123456789- ",%d,1))
			{
				%strPass = true;
				%newStr = %newStr @ %tmpChar;
			}
		}
		if(!%strPass)
			%newStr = %newStr @ " ";
	}
	
	return %newStr; //%dataStr = %newStr;
}