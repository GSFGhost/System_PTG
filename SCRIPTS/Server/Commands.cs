//// LIST ALL ADMIN-ONLY SERVER COMMANDS ////
function SERVERCMDPTGCmds(%cl)
{
	 if(!isObject(%cl)) //for remote console
	{
		echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTGCmds\" failed; client \"" @ %cl @ "\" not found! [!] \c0->" SPC getWord(getDateTime(),1));
		return;
	}
	if(!$PTG.allowNH_SrvrCmdEventUse && !PTG_HostCheck(%cl))
	{
		PTG_MsgClient(%cl,"Denied","PTG: Action Denied","The host has disabled non-host server command and event usage.");
		return;
	}
	if(!%cl.isAdmin)
	{
		PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only admins are allowed to list PTG server commands.");
		return;
	}
	if(%cl.PTGver != 3) //Version check
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Denied","Version 3 of PTG is required, you don't appear to have it downloaded!");
		return;
	}
	
	//Make sure necessary script / sim groups are set up (mainly to check if host has allowed non-host server cmd usage above)
	if(!isObject(PTG_GlobalSO))
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","No routine settings detected; please have the server host apply their routine settings to the server first.");
		return;
	}
	//if(!isObject(PTG_MainSO))
	//{
	//	PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Main settings not detected; please have the server host or a super admin apply their settings to the server first.");
	//	return;
	//}
	
	//////////////////////////////////////////////////
	
	messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ ">>\c0P\c3T\c1G: <color:ffffff>Slash Commands List (Admin or Super Admin only):");
	messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ "><color:00ff33>/PTGStart<color:ffffff> - Starts an infinite / finite routine after checking for errors (using the last settings uploaded to the server).");
	messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ "><color:ff0000>/PTGHalt<color:ffffff> - Stops an infinite / finite routine (and clearing routines), after the most recent chunk is finished being accessed.");
	messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ "><color:ffff00>/PTGClear %Bool<color:ffffff> - Clears all stable, non-static chunks. If %Bool = 1, no removed chunks will be saved, no matter your saving method selected; otherwise they will be.");
	messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ "><color:ffdd00>/PTGClearAll %Bool<color:ffffff> - Exactly the same as /PTGClear, except it also clears static chunks as well.");
	messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ "><color:ffbb00>/PTGClearSpam %ID<color:ffffff> - Clears all bricks in all stable, existing chunks that were planted by the player ID you specify (%ID).");
	messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ "><color:ff9900>/PTGRemoveChunk %Bool<color:ffffff> - Removes the current chunk you are in if stable (after 5 seconds), and removes the chunk save file (if present and depending if %Bool = 1).");
	messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ "><color:ff0000>/PTGClearAllSaves %String<color:ffffff> - If %String = \"AllSaves\", remove all chunk saves, otherwise clears chunk saves for current chunk size and seed (SuperAdmin Only).");
	messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ "><color:ff7700>/PTGPurge<color:ffffff> - Clears all stable chunks, even if static, and also removes any present saves for those chunks (SuperAdmin Only).");
	messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ "><color:ffffff>/PTGCount<color:ffffff> - Displays the current number of existing chunks, including edited and static chunks, and the number of all bricks within chunks.");
	messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ "><color:0088ff>/PTGStatic<color:ffffff> - Toggles the relative chunk to be either static or non-static, depending on it's current static value.");
	messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ "><color:0088ff>/PTGAllStatic %Bool<color:ffffff> - Sets all present stable chunks as either static or non-static (%Bool as 1 or 0).");
	messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ "><color:00ff33>/PTGSave<color:ffffff> - Saves all existing stable chunks, depending on your settings for saving chunks.");
	messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ "><color:ffffff>/PTGPos<color:ffffff> - Sends a chat message to the player with their current X and Y position, and the direction they're facing (any player can use this, unless disabled).");
	messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ "><color:ffffff>/PTGReveal<color:ffffff> - Highlights all chunk objects that currently exist, and displays their current static value.");
	messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ "><color:ffffff>/PTGSetDefault<color:ffffff> - Sets your current GUI settings (Main and Routine) as the new default for your server and GUI (Host / Local Only).");
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//// START A FINITE OR INFINITE ROUTINE ////
function SERVERCMDPTGStart(%cl)
{
	if(%cl !$= "REMOTE" || !$PTG_SekKeyDedRmt)
	{
		 if(!isObject(%cl)) //for remote console
		{
			echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTGStart\" failed; client \"" @ %cl @ "\" not found! If you're attempting to execute a PTG function through the console, use the PTGRmt function instead. [!] \c0->" SPC getWord(getDateTime(),1));
			return;
		}
		if(!$PTG.allowNH_SrvrCmdEventUse && !PTG_HostCheck(%cl))
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","The host has disabled non-host server command and event usage.");
			return;
		}
		if(!%cl.isAdmin)
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only admins are allowed to start a routine.");
			return;
		}
		if(%cl.PTGver != 3) //Version check
		{
			PTG_MsgClient(%cl,"Failed","PTG: Action Denied","Version 3 of PTG is required, you don't appear to have it downloaded!");
			return;
		}
	}

	//Prevent issues with conflicting server commands
	if($PTG.routine_isHalting)
	{
		PTG_MsgClient(%cl,"Error","PTG: Action Failed","Terrain generation routine is currently ending, please wait for it to finish first.");
		return;
	}
	
	//Prevent starting routine while new settings are being uploaded or loaded via third party support
	if(isObject(PTG_GlobalSO_tmp) && PTG_GlobalSO_tmp.uploadingSettings)
	{
		PTG_MsgClient(%cl,"Error","PTG: Action Failed","Can't start a routine while new settings are being uploaded.");
		return;
	}
	if(isObject(PTG_MainSO_SPtmp) && PTG_MainSO_SPtmp.presetLoadInProg)
	{
		PTG_MsgClient(%cl,"Error","PTG: Action Failed","Can't start a routine while new settings are being loaded from a server preset.");
		return;
	}

	if($PTG.routine_Process !$= "None" && $PTG.routine_Process !$= "")
	{
		if($PTG.routine_Process $= "Gen")
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Generation routine already running!");
		if($PTG.routine_Process $= "Clear")
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Can't generate terrain while terrain is being cleared! Either halt the clearing routine, or wait for it to finish.");
		
		return;
	}

	//Create brickgroup for chunks if it doesn't already exist
	if(!isObject("BrickGroup_Chunks"))
	{
		%Chunk = new SimGroup("BrickGroup_Chunks");
		mainBrickGroup.add(%Chunk);
	}
	
	//Make sure necessary script / sim groups are set up
	if(!isObject(PTG_GlobalSO)) 
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","No routine settings detected; please have the server host apply their routine settings to the server first.");
		return;
	}
	if(!isObject(PTG_MainSO))
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Main settings not detected; please have the server host or a super admin apply their settings to the server first.");
		return;
	}
	
	//////////////////////////////////////////////////

	//if(!$PTG_initAttempt)
	//{
	//	PTG_MsgClient(%cl,"Failed","PTG: Resume Failed","No settings detected! The Host or a SuperAdmin must upload generator settings (by selecting the \"Apply and Start\" button) before running a routine.");
	//	return;
	//}
	if(!PTG_ErrorCheck(%cl))
		return;
	if(!$PTG_init)
	{
		PTG_MsgClient(%cl,"Failed","PTG: Resume Failed","No settings detected! The Host or a SuperAdmin must upload generator settings (by selecting the \"Apply and Start\" button) before running a routine.");
		return;
	}

	if(isObject(%cl))
	{
		$PTG.lastClientName = %cl.name;
		$PTG.lastClientID = %cl.bl_id;
		$PTG.lastClientisLocal = %cl.islocalconnection();
		$PTG.lastClientisAdmin = %cl.lastClientisAdmin;
		$PTG.lastClientisSuperAdmin = %cl.isSuperAdmin;
		$PTG.lastClientisHost = PTG_HostCheck(%cl);
		$PTG.lastClientPtgver = %cl.Ptgver;
	}
	else
	{
		if($Server::Lan)
			$PTG.lastClientID = 999999;
		else
			$PTG.lastClientID = getNumKeyID();
		
		if($PTG_SekKeyDedRmt)
			deleteVariables("$PTG_SekKeyDedRmt");
	}
	//else if($PTG.lastClient $= "")
	//{
	//	echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTGStart\" failed; no stored client found! You need to join your dedicated server and start a routine while present (at least once), to start future routines remotely.");
	//	return;
	//}

	//////////////////////////////////////////////////

	$PTG.routine_isHalting = false;
	$PTG.routine_Process = "Gen";
	
	if(isObject(%cl))
	{
		%name = %cl.name;
		%nameB = %cl.name @ " (" @ %cl.bl_id @ ")";
		$PTG.lastClient = %cl; //set last client to client of player who started a routine, not who uploaded settings
	}
	else
	{
		%name = "CONSOLE";
		%nameB = "CONSOLE";
		%cl = $PTG.lastClient;
	}

	//FINITE LANDSCAPE
	if($PTGm.genType $= "Finite")
	{
		if($PTGm.enabAutoSave)
		{
			messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 Finite Generation and AutoSaving routines started by \"" @ %name @ "\" <color:00ff33>[>]");
			if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G:\c0 Finite Generation and AutoSaving routines started by \c4" @ %nameB @ " \c4[>] \c0->" SPC getWord(getDateTime(),1));
			
			%procAm = 2;
		}
		else
		{
			messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 Finite Generation routine started by \"" @ %name @ "\" <color:00ff33>[>]");
			if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G:\c0 Finite Generation routine started by \c4" @ %nameB @ " \c4[>] \c0->" SPC getWord(getDateTime(),1));
			
			%procAm = 1;
		}
		
		$PTG.routine_StartMS = getSimTime(); //routine start time
		$PTG.routine_ProcessAm = %procAm;
		$PTG.dedSrvrFuncCheckTime = getSimTime(); //$PTG.routine_StartMS;
		
		PTG_Routine_Append(%cl,0,0,0,0,0);
		
		if($PTGm.enabAutoSave)
			PTG_Routine_AutoSaveChunks(%cl,0,BrickGroup_Chunks.getCount(),0,true);
	}

	//INFINITE LANDSCAPE
	else
	{
		if($PTGm.remDistChs)
		{
			if($PTGm.enabAutoSave)
			{
				messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 Infinite Generation, Culling and AutoSaving routines started by \"" @ %name @ "\" <color:00ff33>[>>]");
				if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G:\c0 Infinite Generation, Culling and AutoSaving routines started by \c4" @ %nameB @ " \c4[>>] \c0->" SPC getWord(getDateTime(),1));
				%procAm = 3;
			}
			else
			{
				messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 Infinite Generation and Culling routines started by \"" @ %name @ "\" <color:00ff33>[>>]");
				if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G:\c0 Infinite Generation and Culling routines started by \c4" @ %nameB @ " \c4[>>] \c0->" SPC getWord(getDateTime(),1));
				%procAm = 2;
			}
		}
		else
		{
			if($PTGm.enabAutoSave)
			{
				messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 Infinite Generation and AutoSaving routines started by \"" @ %name @ "\" <color:00ff33>[>>]");
				if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G:\c0 Infinite Generation and AutoSaving routines started by \c4" @ %nameB @ " \c4[>>] \c0->" SPC getWord(getDateTime(),1));
				%procAm = 2;
			}
			else
			{
				messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 Infinite Generation routine started by \"" @ %name @ "\" <color:00ff33>[>>]");
				if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G:\c0 Infinite Generation routine started by \c4" @ %nameB @ " \c4[>>] \c0->" SPC getWord(getDateTime(),1));
				%procAm = 1;
			}
		}
		
		if($PTGm.gridType $= "Radial")	//Radial Grid
		{
			%xmod = ($PTGm.chRad * -1);
			%ymod = ($PTGm.chRad * -1);
		}
		else							//Square Grid
		{
			%xmod = 0;
			%ymod = 0;
		}
		
		$PTG.routine_ProcessAm = %procAm;
		$PTG.dedSrvrFuncCheckTime = getSimTime();
		
		PTG_Routine_Append(%cl,0,0,%xmod,%ymod,0);
		
		if($PTGm.remDistChs)
			PTG_Routine_ChunkCull(%cl,0,BrickGroup_Chunks.getCount());

		if($PTGm.enabAutoSave)
			PTG_Routine_AutoSaveChunks(%cl,0,BrickGroup_Chunks.getCount(),0,true);
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function SERVERCMDPTGHalt(%cl)
{
	if(%cl !$= "REMOTE" || !$PTG_SekKeyDedRmt)
	{
		 if(!isObject(%cl)) //for remote console
		{
			echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTGHalt\" failed; client \"" @ %cl @ "\" not found! If you're attempting to execute a PTG function through the console, use the PTGRmt function instead. [!] \c0->" SPC getWord(getDateTime(),1));
			return;
		}
		if(!$PTG.allowNH_SrvrCmdEventUse && !PTG_HostCheck(%cl))
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","The host has disabled non-host server command and event usage.");
			return;
		}
		if(!%cl.isAdmin)
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only admins are allowed to halt a routine.");
			return;
		}
		if(%cl.PTGver != 3) //Version check
		{
			PTG_MsgClient(%cl,"Failed","PTG: Action Denied","Version 3 of PTG is required, you don't appear to have it downloaded!");
			return;
		}
	}
	
	//Prevent issues with conflicting server commands
	if($PTG.routine_isHalting)
	{
		PTG_MsgClient(%cl,"Error","PTG: Action Failed","Already halting routine, please wait.");
		return;
	}
	if($PTG.routine_Process !$= "Gen")
	{
		if($PTG.routine_Process $= "None")
		{
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","No generation or clearing routine is currently running; routine-halt failed.");
			return;
		}
		//if($PTG.routine_Process $= "Clear")
		//	PTG_MsgClient(%cl,"Error","PTG: Action Failed","Can't halt a routine while clearing terrain.");
		
		//return;
	}
	
	//Make sure necessary script / sim groups are set up (don't prevent function from running using "return;", just encase)
	if(!isObject(PTG_GlobalSO)) 
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","No routine settings detected; please have the server host apply their routine settings to the server first.");
		return; //don't use "return;" so that function can still halt?
	}
	if(!isObject(PTG_MainSO))
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Main settings not detected; please have the server host or a super admin apply their settings to the server first.");
		return; //don't use "return;" so that function can still halt?
	}
	
	//////////////////////////////////////////////////

	if($PTG.routine_Process $= "Clear")
		%temp = "clearing";
	else
	{
		if($PTGm.genType $= "Finite")
			%temp = "finite";
		else
			%temp = "infinite";
	}
	
	$PTG.routine_isHalting = true; //stop terrain
	
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
	
	messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 \"" @ %name @ "\" halted the" SPC %temp SPC "routine, waiting for last function to finish... <color:ff0000>[X]");
	if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %nameB @ " \c0halted the" SPC %temp SPC "routine, waiting for last function to finish... \c4[X] \c0->" SPC getWord(getDateTime(),1));
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function SERVERCMDPTGClear(%cl,%dontSave)
{
	if(%cl !$= "REMOTE" || !$PTG_SekKeyDedRmt)
	{
		 if(!isObject(%cl)) //for remote console
		{
			echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTGClear\" failed; client \"" @ %cl @ "\" not found! If you're attempting to execute a PTG function through the console, use the PTGRmt function instead. [!] \c0->" SPC getWord(getDateTime(),1));
			return;
		}
		if(!$PTG.allowNH_SrvrCmdEventUse && !PTG_HostCheck(%cl))
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","The host has disabled non-host server command and event usage.");
			return;
		}
		if(!%cl.isAdmin)
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only admins are allowed to clear the terrain.");
			return;
		}
		if(%cl.PTGver != 3) //Version check
		{
			PTG_MsgClient(%cl,"Failed","PTG: Action Denied","Version 3 of PTG is required, you don't appear to have it downloaded!");
			return;
		}
	}
	
	//Prevent issues with conflicting server commands
	if($PTG.routine_isHalting)
	{
		PTG_MsgClient(%cl,"Error","PTG: Action Failed","Terrain generation routine is currently ending, please wait for it to finish first.");
		return;
	}
	switch$($PTG.routine_Process)
	{
		case "None": 
		case "Gen":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Can't clear terrain while a routine is running; please stop the routine first.");
			return;
		case "Clear":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Already clearing terrain.");
			return;
	}
	
	//Make sure Chunks BrickGroup exists
	if(!isObject(BrickGroup_Chunks))
	{
		%BGm = new SimGroup("BrickGroup_Chunks");
		mainBrickGroup.add(%BGm);
	}
	
	//Make sure necessary script / sim groups are set up
	if(!isObject(PTG_GlobalSO)) 
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","No routine settings detected; please have the server host apply their routine settings to the server first.");
		return;
	}
	if(!isObject(PTG_MainSO))
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Main settings not detected; please have the server host or a super admin apply their settings to the server first.");
		return;
	}
	
	//////////////////////////////////////////////////
	
	$PTG.routine_Process = "Clear";
	
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
	
	messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 \"" @ %name @ "\" is clearing the terrain, please wait... \c3[C]");
	if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %nameB @ " \c0is clearing the terrain, please wait... \c4[C] \c0->" SPC getWord(getDateTime(),1));
	
	$PTG.dedSrvrFuncCheckTime = getSimTime();
	PTGClear_Recurs(%cl,"","",BrickGroup_Chunks.getCount(),false,%dontSave,"Clear",0);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function SERVERCMDPTGClearAll(%cl,%dontSave)
{
	if(%cl !$= "REMOTE" || !$PTG_SekKeyDedRmt)
	{
		 if(!isObject(%cl)) //for remote console
		{
			echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTGClearAll\" failed; client \"" @ %cl @ "\" not found! If you're attempting to execute a PTG function through the console, use the PTGRmt function instead. [!] \c0->" SPC getWord(getDateTime(),1));
			return;
		}
		if(!$PTG.allowNH_SrvrCmdEventUse && !PTG_HostCheck(%cl))
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","The host has disabled non-host server command and event usage.");
			return;
		}
		if(!%cl.isAdmin)
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only admins are allowed to clear the terrain.");
			return;
		}
		if(%cl.PTGver != 3) //Version check
		{
			PTG_MsgClient(%cl,"Failed","PTG: Action Denied","Version 3 of PTG is required, you don't appear to have it downloaded!");
			return;
		}
	}
	
	//Prevent issues with conflicting server commands
	if($PTG.routine_isHalting)
	{
		PTG_MsgClient(%cl,"Error","PTG: Action Failed","Terrain generation routine is currently ending, please wait for it to finish first.");
		return;
	}
	switch$($PTG.routine_Process)
	{
		case "None": 
		case "Gen":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Can't clear terrain while a routine is running; please stop the routine first.");
			return;
		case "Clear":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Already clearing terrain.");
			return;
	}
	
	//Make sure Chunks BrickGroup exists
	if(!isObject(BrickGroup_Chunks))
	{
		%BGm = new SimGroup("BrickGroup_Chunks");
		mainBrickGroup.add(%BGm);
	}
	
	//Make sure necessary script / sim groups are set up
	if(!isObject(PTG_GlobalSO)) 
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","No routine settings detected; please have the server host apply their routine settings to the server first.");
		return;
	}
	if(!isObject(PTG_MainSO))
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Main settings not detected; please have the server host or a super admin apply their settings to the server first.");
		return;
	}
	
	//////////////////////////////////////////////////
	
	$PTG.routine_Process = "Clear";
	
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
	
	messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 \"" @ %name @ "\" is clearing the terrain, please wait... <color:ffdd00>[CA]");
	if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %nameB @ " \c0is clearing the terrain, please wait... \c4[CA] \c0->" SPC getWord(getDateTime(),1));
	
	$PTG.dedSrvrFuncCheckTime = getSimTime();
	PTGClear_Recurs(%cl,"","",BrickGroup_Chunks.getCount(),true,%dontSave,"ClearAll",0);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function SERVERCMDPTGClearSpam(%cl,%BL_ID)
{
	if(%cl !$= "REMOTE" || !$PTG_SekKeyDedRmt)
	{
		 if(!isObject(%cl)) //for remote console
		{
			echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTGClearSpam\" failed; client \"" @ %cl @ "\" not found! If you're attempting to execute a PTG function through the console, use the PTGRmt function instead. [!] \c0->" SPC getWord(getDateTime(),1));
			return;
		}
		if(!$PTG.allowNH_SrvrCmdEventUse && !PTG_HostCheck(%cl))
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","The host has disabled non-host server command and event usage.");
			return;
		}
		if(!%cl.isAdmin)
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only admins are allowed to clear all chunk saves.");
			return;
		}
		if(%cl.PTGver != 3) //Version check
		{
			PTG_MsgClient(%cl,"Failed","PTG: Action Denied","Version 3 of PTG is required, you don't appear to have it downloaded!");
			return;
		}
	}
	
	//Prevent issues with conflicting server commands
	if($PTG.routine_isHalting)
	{
		PTG_MsgClient(%cl,"Error","PTG: Action Failed","Terrain generation routine is currently ending, please wait for it to finish first.");
		return;
	}
	if($PTG.routine_Process $= "Clear")
	{
		PTG_MsgClient(%cl,"Error","PTG: Action Failed","Can't clear all chunk saves while terrain is being cleared.");
		return;
	}
	switch$($PTG.routine_ProcessAux)
	{
		case "None":
		case "ClearAllSaves":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Currently clearing all chunk saves, please wait for the function to finish...");
			return;
		case "SaveAllChunks":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Currently saving all chunks, please wait for the function to finish...");
			return;
		case "RemoveChunk":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Currently removing a chunk, please wait for the function to finish...");
			return;
		case "AllStatic":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Currently setting all chunks to static, please wait for the function to finish...");
			return;
		case "ClearSpam":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Already clearing player spam!");
			return;
	}
	
	//Make sure Chunks BrickGroup exists
	if(!isObject(BrickGroup_Chunks))
	{
		%BGm = new SimGroup("BrickGroup_Chunks");
		mainBrickGroup.add(%BGm);
	}
	
	//Make sure necessary script / sim groups are set up
	if(!isObject(PTG_GlobalSO)) 
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","No routine settings detected; please have the server host apply their routine settings to the server first.");
		return;
	}
	if(!isObject(PTG_MainSO))
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Main settings not detected; please have the server host or a super admin apply their settings to the server first.");
		return;
	}
	
	//////////////////////////////////////////////////
	
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

	if(%BL_ID != getNumKeyID() && %BL_ID != 999999 && %BL_ID != 888888)
	{
		if(%BL_ID !$= "Spammer ID")
		{
			%BL_ID *= 1; //filter out strings
			
			$PTG.routine_ProcessAux = "ClearSpam";
			
			messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 \"" @ %name @ "\" is clearing spam bricks for ID: <color:ffbb00>" @ %BL_ID @ "\c6, please wait... <color:ffbb00>[CS]");
			if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %nameB @ " \c0is clearing spam bricks for ID: \c4" @ %BL_ID @ "\c0, please wait... \c4[CS] \c0->" SPC getWord(getDateTime(),1));

			PTGClearSpam_Recurs(%cl,0,0,0,BrickGroup_Chunks.getCount(),%BL_ID);
		}
		else
			PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Please replace \"Spammer ID\" with the Blockland ID for the player who's bricks you want to clear.");
	}
	else
	{
		if(%BL_ID == 888888)
			PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Can't clear public bricks.");
		else
			PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Can't clear bricks planted by the host.");
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function SERVERCMDPTGPurge(%cl)
{
	if(%cl !$= "REMOTE" || !$PTG_SekKeyDedRmt)
	{
		 if(!isObject(%cl)) //for remote console
		{
			echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTGPurge\" failed; client \"" @ %cl @ "\" not found! If you're attempting to execute a PTG function through the console, use the PTGRmt function instead. [!] \c0->" SPC getWord(getDateTime(),1));
			return;
		}
		if(!$PTG.allowNH_SrvrCmdEventUse && !PTG_HostCheck(%cl))
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","The host has disabled non-host server command and event usage.");
			return;
		}
		if(!%cl.isSuperAdmin)
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only super admins are allowed to purge the terrain.");
			return;
		}
		if(%cl.PTGver != 3) //Version check
		{
			PTG_MsgClient(%cl,"Failed","PTG: Action Denied","Version 3 of PTG is required, you don't appear to have it downloaded!");
			return;
		}
	}
	
	//Prevent issues with conflicting server commands
	if($PTG.routine_isHalting)
	{
		PTG_MsgClient(%cl,"Error","PTG: Action Failed","Terrain generation routine is currently ending, please wait for it to finish first.");
		return;
	}
	if($PTG.routine_Process !$= "None")
	{
		if($PTG.routine_Process $= "Gen")
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Can't purge terrain while a routine is running; please stop the routine first.");
		if($PTG.routine_Process $= "Clear")
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Already purging terrain.");
		
		return;
	}
	
	//Make sure Chunks BrickGroup exists
	if(!isObject(BrickGroup_Chunks))
	{
		%BGm = new SimGroup("BrickGroup_Chunks");
		mainBrickGroup.add(%BGm);
	}
	
	//Make sure necessary script / sim groups are set up
	if(!isObject(PTG_GlobalSO)) 
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","No routine settings detected; please have the server host apply their routine settings to the server first.");
		return;
	}
	if(!isObject(PTG_MainSO))
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Main settings not detected; please have the server host or a super admin apply their settings to the server first.");
		return;
	}
	
	//////////////////////////////////////////////////
	
	$PTG.routine_Process = "Clear";
	
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
	
	messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 \"" @ %name @ "\" is purging the terrain, please wait... <color:ff7700>[P]");
	if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %nameB @ " \c0is purging the terrain, please wait... \c4[P] \c0->" SPC getWord(getDateTime(),1));
	
	$PTG.dedSrvrFuncCheckTime = getSimTime();
	PTGClear_Recurs(%cl,"","",BrickGroup_Chunks.getCount(),true,true,"Purge",0);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function SERVERCMDPTGRemoveChunk(%cl,%removeSave)
{
	 if(!isObject(%cl)) //for remote console
	{
		echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTGRemoveChunk\" failed; client \"" @ %cl @ "\" not found! [!] \c0->" SPC getWord(getDateTime(),1));
		return;
	}
	if(!$PTG.allowNH_SrvrCmdEventUse && !PTG_HostCheck(%cl))
	{
		PTG_MsgClient(%cl,"Denied","PTG: Action Denied","The host has disabled non-host server command and event usage.");
		return;
	}
	if(!%cl.isAdmin)
	{
		PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only admins are allowed to remove a chunk.");
		return;
	}
	if(%cl.PTGver != 3) //Version check
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Denied","Version 3 of PTG is required, you don't appear to have it downloaded!");
		return;
	}
	
	//Prevent issues with conflicting server commands
	if($PTG.routine_isHalting)
	{
		PTG_MsgClient(%cl,"Error","PTG: Action Failed","Terrain generation routine is currently ending, please wait for it to finish first.");
		return;
	}
	if($PTG.routine_Process $= "Clear")
	{
		PTG_MsgClient(%cl,"Error","PTG: Action Failed","Can't remove a chunk while terrain is being cleared.");
		return;
	}
	switch$($PTG.routine_ProcessAux)
	{
		case "None": 
		case "ClearAllSaves":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Currently clearing all saves, please wait for the function to finish...");
			return;
		case "SaveAllChunks":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Currently saving all chunks, please wait for the function to finish...");
			return;
		case "RemoveChunk":
		//	PTG_MsgClient(%cl,"Error","PTG: Action Failed","Already removing a chunk, please wait for the function to finish...");
		//	return;
		case "AllStatic":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Currently setting all chunks to static, please wait for the function to finish...");
			return;
		case "ClearSpam":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Currently clearing player spam, please wait for the function to finish...");
			return;
	}
	
	//Make sure Chunks BrickGroup exists
	if(!isObject(BrickGroup_Chunks))
	{
		%BGm = new SimGroup("BrickGroup_Chunks");
		mainBrickGroup.add(%BGm);
	}
	
	//Make sure necessary script / sim groups are set up
	if(!isObject(PTG_GlobalSO)) 
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","No routine settings detected; please have the server host apply their routine settings to the server first.");
		return;
	}
	if(!isObject(PTG_MainSO))
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Main settings not detected; please have the server host or a super admin apply their settings to the server first.");
		return;
	}
	
	//////////////////////////////////////////////////

	if(isObject(%pl = %cl.player))
	{
		%plPos = %pl.position;
		%CHPosX = mFloor(getWord(%plPos,0) / mClamp($PTGm.chSize,16,256)) * mClamp($PTGm.chSize,16,256);
		%CHPosY = mFloor(getWord(%plPos,1) / mClamp($PTGm.chSize,16,256)) * mClamp($PTGm.chSize,16,256);
		%Chunk = "Chunk_" @ %CHPosX @ "_" @ %CHPosY;
		
		if(isObject(%Chunk))
		{
			if(!%Chunk.ChUnstablePTG)
			{
				if(!%Chunk.ChRemovalPTG)
				{
					$PTG.routine_ProcessAux = "RemoveChunk";
					messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0>P\c3T\c1G:\c6 Clearing current chunk in 5 seconds, make sure you're not in the chunk! <color:ff9900>[RC]"); //message client only
					
					%Chunk.ChRemovalPTG = true;
					scheduleNoQuota(5000,0,PTGRemoveChunk_Recurs,%cl,%Chunk,0,0,%removeSave,false); //5 sec delay so player can leave chunk
					
					
					//Highlight chunk to notify other players of imminent removal
					%newHLChunkN = strReplace(%Chunk,"Chunk","HLChunk");
					
					//If highest point isn't stored in chunk (i.e. if created for planted / loaded brick), run through all objects in chunk and find highest
					if(!isObject(%newHLChunkN))
					{
						if((%tmpChZ = %Chunk.PTGHighestZpos) == 0)
						{
							for(%d = 0; %d < %Chunk.getCount(); %d++)
							{
								%tmpObj = %Chunk.getObject(%d);
								%tmpObjPosZ = getWord(%tmpObj.getPosition(),2);
								
								//Update highlighted chunk color (if revealed)
								if(%tmpObj.getClassName() $= "fxDTSBrick" && %tmpObjPosZ > %tmpChZ)
									%tmpChZ = %tmpObjPosZ + ((%tmpObjPosZ.brickSizeZ * 0.2) * 0.5);
							}
						}
						
						%tmpRefName = strReplace(%Chunk,"_"," ");
						%tmpChX = getWord(%tmpRefName,1);
						%tmpChY = getWord(%tmpRefName,2);
					
						%newChZ = mCeil(%tmpChZ / 32);
						%pos = %tmpChX+(mClamp($PTGm.chSize,16,256) / 2) SPC %tmpChY+(mClamp($PTGm.chSize,16,256) / 2) SPC ((%newChZ / 2) * 32); //$PTGm.boundsHLevel+2;
						%scale = (mClamp($PTGm.chSize,16,256) / 16) SPC (mClamp($PTGm.chSize,16,256) / 16) SPC %newChZ;
					}
					
					//If highlight object exists from PTGReveal function, copy data to new object instead
					else
					{
						%pos = %newHLChunkN.position;
						%scale = %newHLChunkN.scale;
						%newHLChunkN.delete();
					}
					
					//Spawn static shape (add to chunk itself) and add to highlight chunk brickgroup
						//also add name to reference if static value is changed for chunk
						//static shape highlight obj will be deleted if chunk itself is deleted
					%tmpStcObj = PTG_Chunk_SpawnStatic(%Chunk,%pos,%scale,"HL-Removal");
					%tmpStcObj.setName(%newHLChunkN);
				}
				else
				{
					PTG_MsgClient(%cl,"Error","PTG: Action Failed","This chunk is already scheduled for removal.");
					return;
				}
			}
			else
			{
				PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Chunk is unstable, meaning it's currently being accessed by a script. Please wait a few seconds and try again.");
				return;
			}	
		}
		else
		{
			PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Chunk object at position X:" @ %CHPosX @ " Y:" @ %CHPosY @ " doesn't exist!");
			return;
		}
	}
	else
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Your player object doesn't exist, can't find a relative chunk.");
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function SERVERCMDPTGClearAllSaves(%cl,%clrMethod)
{
	if(%cl !$= "REMOTE" || !$PTG_SekKeyDedRmt)
	{
		 if(!isObject(%cl)) //for remote console
		{
			echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTGClearAllSaves\" failed; client \"" @ %cl @ "\" not found! If you're attempting to execute a PTG function through the console, use the PTGRmt function instead. [!] \c0->" SPC getWord(getDateTime(),1));
			return;
		}
		if(!$PTG.allowNH_SrvrCmdEventUse && !PTG_HostCheck(%cl))
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","The host has disabled non-host server command and event usage.");
			return;
		}
		if(!%cl.isSuperAdmin)
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only super admins are allowed to clear all chunk saves.");
			return;
		}
		if(%cl.PTGver != 3) //Version check
		{
			PTG_MsgClient(%cl,"Failed","PTG: Action Denied","Version 3 of PTG is required, you don't appear to have it downloaded!");
			return;
		}
	}
	
	//Prevent issues with conflicting server commands
	if($PTG.routine_isHalting)
	{
		PTG_MsgClient(%cl,"Error","PTG: Action Failed","Terrain generation routine is currently ending, please wait for it to finish first.");
		return;
	}
	if($PTG.routine_Process $= "Clear")
	{
		PTG_MsgClient(%cl,"Error","PTG: Action Failed","Can't clear all chunk saves while terrain is being cleared.");
		return;
	}
	switch$($PTG.routine_ProcessAux)
	{
		case "None":
		case "ClearAllSaves":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Already clearing all chunk saves for seed!");
			return;
		case "SaveAllChunks":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Currently saving all chunks, please wait for the function to finish...");
			return;
		case "RemoveChunk":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Currently removing a chunk, please wait for the function to finish...");
			return;
		case "AllStatic":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Currently setting all chunks to static, please wait for the function to finish...");
			return;
		case "ClearSpam":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Currently clearing player spam, please wait for the function to finish...");
			return;
	}

	//Make sure necessary script / sim groups are set up
	if(!isObject(PTG_GlobalSO)) 
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","No routine settings detected; please have the server host apply their routine settings to the server first.");
		return;
	}
	if(!isObject(PTG_MainSO))
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Main settings not detected; please have the server host or a super admin apply their settings to the server first.");
		return;
	}
	
	//////////////////////////////////////////////////
	
	if(%clrMethod $= "AllSaves")
		%fp = PTG_GetFP("ChunkCache","","","");
	else
		%fp = PTG_GetFP("ChunkSeed","","","");
	
	%chSaveAm = getFileCount(%fp);

	if(%chSaveAm == 0)
	{
		if(%clrMethod $= "AllSaves")
			PTG_MsgClient(%cl,"Failed","PTG: Action Failed","No chunk saves found.");
		else
			PTG_MsgClient(%cl,"Failed","PTG: Action Failed","No chunk saves found for seed:" SPC getSubStr($PTGm.seed,0,8));
		
		return;
	}

	$PTG.routine_ProcessAux = "ClearAllSaves";
	
	for(%c = 0; %c < %chSaveAm; %c++)
	{
		%tmpFile = findNextFile(%fp);
		%Chunk = fileName(%tmpFile);
		
		if(isFile(%tmpFile) && (!isObject(%Chunk) || !%Chunk.ChUnstablePTG)) //don't remove files for chunks being accessed by scripts (might be saving to file)
		{
			if(strStr(%tmpFile,"Permanent_Saves") == -1) //Account for permanent saves
			{
				fileDelete(%tmpFile);
				%fileCount++;
			}
		}
	}
	
	$PTG.routine_ProcessAux = "None";
	
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
	
	if(%fileCount > 0)
	{
		if(%clrMethod $= "AllSaves")
		{
			messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 \"" @ %name @ "\" cleared all chunk saves;\c0" SPC %fileCount SPC "\c6of" SPC %chSaveAm SPC "saves removed successfully! \c0[CAS]");
			if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %nameB @ " \c0cleared all chunk saves;\c4" SPC %fileCount SPC "\c0of" SPC %chSaveAm SPC "saves removed successfully. \c4[CAS] ->" SPC getWord(getDateTime(),1));
		}
		else
		{
			messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 \"" @ %name @ "\" cleared all chunk saves for seed: \c0" @ getSubStr($PTGm.seed,0,8) @ "\c6;\c0" SPC %fileCount SPC "\c6of" SPC %chSaveAm SPC "saves removed successfully! \c0[CAS]");
			if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %nameB @ " \c0cleared all chunk saves for seed: \c4" @ getSubStr($PTGm.seed,0,8) @ "\c0;\c4" SPC %fileCount SPC "\c0of" SPC %chSaveAm SPC "saves removed successfully. \c4[CAS] ->" SPC getWord(getDateTime(),1));
		}
	}
	else
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","No chunk saves could be removed.");
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function SERVERCMDPTGCount(%cl)
{
	if(%cl !$= "REMOTE" || !$PTG_SekKeyDedRmt)
	{
		 if(!isObject(%cl)) //for remote console
		{
			echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTGCount\" failed; client \"" @ %cl @ "\" not found! If you're attempting to execute a PTG function through the console, use the PTGRmt function instead. [!] \c0->" SPC getWord(getDateTime(),1));
			return;
		}
		if(!$PTG.allowNH_SrvrCmdEventUse && !PTG_HostCheck(%cl))
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","The host has disabled non-host server command and event usage.");
			return;
		}
		if(!%cl.isAdmin)
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only admins are allowed to request the chunk count.");
			return;
		}
		if(%cl.PTGver != 3) //Version check
		{
			PTG_MsgClient(%cl,"Failed","PTG: Action Denied","Version 3 of PTG is required, you don't appear to have it downloaded!");
			return;
		}
	}
	
	//Make sure Chunks BrickGroup exists
	if(!isObject(BrickGroup_Chunks))
	{
		%BGm = new SimGroup("BrickGroup_Chunks");
		mainBrickGroup.add(%BGm);
	}
	
	//Make sure necessary script / sim groups are set up
	if(!isObject(PTG_GlobalSO)) 
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","No routine settings detected; please have the server host apply their routine settings to the server first.");
		return;
	}
	if(!isObject(PTG_MainSO))
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Main settings not detected; please have the server host or a super admin apply their settings to the server first.");
		return;
	}
	
	//////////////////////////////////////////////////

	for(%c = 0; %c < (%CHcount = BrickGroup_Chunks.getCount()); %c++)
	{
		%chunk = BrickGroup_Chunks.getObject(%c);
		
		if(%Chunk.ChEditedPTG) 
			%EdtCHcount++;
		if(%Chunk.ChStaticPTG) 
			%StcCHcount++;
		
		%BRcount += %chunk.getCount();
	}
	
	//Prevent blank characters for any values == 0
	if(%CHcount == 0)
		%CHcount = "0";
	if(%BRcount == 0)
		%BRcount = "0";
	if(%EdtCHcount == 0)
		%EdtCHcount = "0";
	if(%StcCHcount == 0)
		%StcCHcount = "0";
	
	//message client only here
	if(isObject(%cl))
		messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0>P\c3T\c1G\c6: <color:0033ff>" @ %CHcount @ "\c6 Chunk(s), <color:0033ff>" @ %BRcount @ "\c6 Brick(s) In Chunk(s), <color:0033ff>" @ %EdtCHcount @ " \c6Edited Chunk(s) and <color:0033ff>" @ %StcCHcount @ "\c6 Static Chunk(s) currently exist. <color:0033ff>");
	if($PTG.allowEchos || !isObject(%cl)) //else
		echo("\c4>>\c2P\c1T\c4G: " @ %CHcount @ "\c0 Chunk(s), \c4" @ %BRcount @ "\c0 Brick(s) In Chunk(s), \c4" @ %EdtCHcount @ " \c0Edited Chunk(s) and \c4" @ %StcCHcount @ "\c0 Static Chunk(s) currently exist. \c4[chunk count] \c0->" SPC getWord(getDateTime(),1));
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function SERVERCMDPTGStatic(%cl)
{
	 if(!isObject(%cl)) //for remote console
	{
		echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTGStatic\" failed; client \"" @ %cl @ "\" not found! [!] \c0->" SPC getWord(getDateTime(),1));
		return;
	}
	if(!$PTG.allowNH_SrvrCmdEventUse && !PTG_HostCheck(%cl))
	{
		PTG_MsgClient(%cl,"Denied","PTG: Action Denied","The host has disabled non-host server command and event usage.");
		return;
	}
	if(!%cl.isAdmin)
	{
		PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only admins are allowed to toggle the static value for a chunk.");
		return;
	}
	if(%cl.PTGver != 3) //Version check
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Denied","Version 3 of PTG is required, you don't appear to have it downloaded!");
		return;
	}
	
	//Make sure Chunks BrickGroup exists
	if(!isObject(BrickGroup_Chunks))
	{
		%BGm = new SimGroup("BrickGroup_Chunks");
		mainBrickGroup.add(%BGm);
	}

	//Make sure necessary script / sim groups are set up
	if(!isObject(PTG_GlobalSO)) 
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","No routine settings detected; please have the server host apply their routine settings to the server first.");
		return;
	}
	if(!isObject(PTG_MainSO))
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Main settings not detected; please have the server host or a super admin apply their settings to the server first.");
		return;
	}
	
	//////////////////////////////////////////////////

	if(isObject(%pl = %cl.player))
	{
		%plPos = %pl.position;
		%posX = mFloor(getWord(%plPos,0) / mClamp($PTGm.chSize,16,256)) * mClamp($PTGm.chSize,16,256);
		%posY = mFloor(getWord(%plPos,1) / mClamp($PTGm.chSize,16,256)) * mClamp($PTGm.chSize,16,256);
		%Chunk = "Chunk_" @ %posX @ "_" @ %posY;
		
		if(isObject(%Chunk))
		{
			//Set to static
			if(!%Chunk.ChStaticPTG)
			{
				%Chunk.ChStaticPTG = true;
				%staticCol = PTG_FindClosestColor(getColorIDTable($PTG.ChunkHLACol),"RGBA-Hex");
				
				messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0>P\c3T\c1G: \c6Chunk set to " @ %staticCol @ "\"static\".");
				if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %cl.name @ " \c0set chunk at position X:\c4" @ %posX @ " \c0Y:\c4" @ %posY @ " \c0to \c4static\c0. ->" SPC getWord(getDateTime(),1));
			
				if(isObject(%tmpStcObj = strReplace(%Chunk,"Chunk","HLChunk")))
					%tmpStcObj.setNodeColor(ColMain,getColorIDTable($PTG.ChunkHLACol));
			}
			
			//Set to non-static
			else
			{
				%Chunk.ChStaticPTG = false;
				%nonStaticCol = PTG_FindClosestColor(getColorIDTable($PTG.ChunkHLBCol),"RGBA-Hex");
				
				messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0>P\c3T\c1G: \c6Chunk set to " @ %nonStaticCol @ "\"non-static\".");
				if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %cl.name @ " \c0set chunk at position X:\c4" @ %posX @ " \c0Y:\c4" @ %posY @ " \c0to \c4non-static\c0. ->" SPC getWord(getDateTime(),1));
			
				if(isObject(%tmpStcObj = strReplace(%Chunk,"Chunk","HLChunk")))
					%tmpStcObj.setNodeColor(ColMain,getColorIDTable($PTG.ChunkHLBCol));
			}
		}
		else
			PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Chunk object at position X:" @ %posX @ " Y:" @ %posY @ " doesn't exist!");
	}
	else
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Your player object doesn't exist, can't figure relative chunk.");
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function SERVERCMDPTGAllStatic(%cl,%stcValue)
{
	if(%cl !$= "REMOTE" || !$PTG_SekKeyDedRmt)
	{
		 if(!isObject(%cl)) //for remote console
		{
			echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTGAllStatic\" failed; client \"" @ %cl @ "\" not found! If you're attempting to execute a PTG function through the console, use the PTGRmt function instead. [!] \c0->" SPC getWord(getDateTime(),1));
			return;
		}
		if(!$PTG.allowNH_SrvrCmdEventUse && !PTG_HostCheck(%cl))
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","The host has disabled non-host server command and event usage.");
			return;
		}
		if(!%cl.isAdmin)
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only admins are allowed to toggle the static values for all chunks.");
			return;
		}
		if(%cl.PTGver != 3) //Version check
		{
			PTG_MsgClient(%cl,"Failed","PTG: Action Denied","Version 3 of PTG is required, you don't appear to have it downloaded!");
			return;
		}
	}
	
	switch$($PTG.routine_ProcessAux)
	{
		case "None": 
		case "ClearAllSaves":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Currently clearing all saves, please wait for the function to finish...");
			return;
		case "SaveAllChunks":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Currently saving all chunks, please wait for the function to finish...");
			return;
		case "RemoveChunk":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Currently removing a chunk, please wait for the function to finish...");
			return;
		case "AllStatic":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Already toggling static value for all chunks, please wait for the function to finish...");
			return;
		case "ClearSpam":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Currently clearing player spam, please wait for the function to finish...");
			return;
	}
	
	//Make sure Chunks BrickGroup exists
	if(!isObject(BrickGroup_Chunks))
	{
		%BGm = new SimGroup("BrickGroup_Chunks");
		mainBrickGroup.add(%BGm);
	}
	
	//Make sure necessary script / sim groups are set up
	if(!isObject(PTG_GlobalSO)) 
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","No routine settings detected; please have the server host apply their routine settings to the server first.");
		return;
	}
	if(!isObject(PTG_MainSO))
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Main settings not detected; please have the server host or a super admin apply their settings to the server first.");
		return;
	}
	
	//////////////////////////////////////////////////
	
	$PTG.routine_ProcessAux = "AllStatic";
	
	for(%c = 0; %c < BrickGroup_Chunks.getCount(); %c++)
	{
		%Chunk = BrickGroup_Chunks.getObject(%c);

		if(%stcValue)
		{
			%Chunk.ChStaticPTG = true;
			
			//Update highlighted chunk color (if revealed)
			if(isObject(%tmpStcObj = strReplace(%Chunk.getName(),"Chunk","HLChunk")))
				%tmpStcObj.setNodeColor(ColMain,getColorIDTable($PTG.ChunkHLACol));
		}
		else
		{
			%Chunk.ChStaticPTG = false;
			
			//Update highlighted chunk color (if revealed)
			if(isObject(%tmpStcObj = strReplace(%Chunk.getName(),"Chunk","HLChunk")))
				%tmpStcObj.setNodeColor(ColMain,getColorIDTable($PTG.ChunkHLBCol));
		}
		
		%chCount++;
	}
	
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
	
	if(%chCount > 0)
	{
		if(%stcValue)
		{
			%staticCol = PTG_FindClosestColor(getColorIDTable($PTG.ChunkHLACol),"RGBA-Hex");

			if(%chCount > 1) //MESSAGE ALL
			{
				messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6\"" @ %name @ "\" set all" SPC %staticCol @ %chCount SPC "\c6chunk(s) to " @ %staticCol @ "\"static\".");
				if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %nameB @ " \c0set all\c4 " @ %chCount @ " \c6chunk(s) to \c4static\c0. ->" SPC getWord(getDateTime(),1));
			}
			else
			{
				messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6\"" @ %name @ "\" set the single existing chunk to " @ %staticCol @ "\"static\".");
				if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %nameB @ " \c0set the single existing chunk to \c4static\c0. ->" SPC getWord(getDateTime(),1));
			}
		}
		else
		{
			%nonStaticCol = PTG_FindClosestColor(getColorIDTable($PTG.ChunkHLBCol),"RGBA-Hex");

			if(%chCount > 1)
			{
				messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6\"" @ %name @ "\" set all" SPC %nonStaticCol @ %chCount SPC "\c6chunk(s) to " @ %nonStaticCol @ "\"non-static\".");
				if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %nameB @ " \c0set all\c4 " @ %chCount @ " \c6chunk(s) to \c4non-static\c0. ->" SPC getWord(getDateTime(),1));
			}
			else
			{
				messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6\"" @ %name @ "\" set the single existing chunk to " @ %nonStaticCol @ "\"non-static\".");
				if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %nameB @ " \c0set the single existing chunk to \c4non-static\c0. ->" SPC getWord(getDateTime(),1));
			}
		}
	}
	else
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","No chunks exist!");
	
	$PTG.routine_ProcessAux = "None";
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function SERVERCMDPTGSave(%cl)
{
	if(%cl !$= "REMOTE" || !$PTG_SekKeyDedRmt)
	{
		 if(!isObject(%cl)) //for remote console
		{
			echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTGSave\" failed; client \"" @ %cl @ "\" not found! If you're attempting to execute a PTG function through the console, use the PTGRmt function instead. [!] \c0->" SPC getWord(getDateTime(),1));
			return;
		}
		if(!$PTG.allowNH_SrvrCmdEventUse && !PTG_HostCheck(%cl))
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","The host has disabled non-host server command and event usage.");
			return;
		}
		if(!%cl.isAdmin)
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only admins are allowed to save chunks.");
			return;
		}
		if(%cl.PTGver != 3) //Version check
		{
			PTG_MsgClient(%cl,"Failed","PTG: Action Denied","Version 3 of PTG is required, you don't appear to have it downloaded!");
			return;
		}
	}
	
	//Make sure Chunks BrickGroup exists
	if(!isObject(BrickGroup_Chunks))
	{
		%BGm = new SimGroup("BrickGroup_Chunks");
		mainBrickGroup.add(%BGm);
	}
	
	//Make sure necessary script / sim groups are set up
	if(!isObject(PTG_GlobalSO)) 
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","No routine settings detected; please have the server host apply their routine settings to the server first.");
		return;
	}
	if(!isObject(PTG_MainSO))
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Main settings not detected; please have the server host or a super admin apply their settings to the server first.");
		return;
	}
	
	//Prevent issues with conflicting server commands
	if($PTG.routine_isHalting)
	{
		PTG_MsgClient(%cl,"Error","PTG: Action Failed","Terrain generation routine is currently ending, please wait for it to finish first.");
		return;
	}
	switch$($PTG.routine_Process)
	{
		case "None":
		case "Gen":
			//PTG_MsgClient(%cl,"Error","PTG: Action Failed","Can't save all chunks while a routine is running; please stop the routine first.");
			//return;
		case "Clear":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Can't save all chunks while terrain is being cleared.");
			return;
	}
	switch$($PTG.routine_ProcessAux)
	{
		case "None": 
		
		case "ClearAllSaves":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Currently clearing all saves, please wait for the function to finish...");
			return;
		case "SaveAllChunks":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Already saving all chunks!");
			return;
		case "RemoveChunk":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Currently removing a chunk, please wait for the function to finish...");
			return;
		case "AllStatic":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Currently setting all chunks to static, please wait for the function to finish...");
			return;
		case "ClearSpam":
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Currently clearing player spam, please wait for the function to finish...");
			return;
	}
	
	//////////////////////////////////////////////////
	
	$PTG.routine_ProcessAux = "SaveAllChunks";
	PTGSave_Recurs(%cl,0,BrickGroup_Chunks.getCount(),0,0);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function SERVERCMDPTGPos(%cl)
{
	 if(!isObject(%cl)) //for remote console
	{
		echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTGPos\" failed; client \"" @ %cl @ "\" not found! [!] \c0->" SPC getWord(getDateTime(),1));
		return;
	}
	if(!$PTG.AllowPlyrPosChk && !PTG_HostCheck(%cl))
	{
		PTG_MsgClient(%cl,"Denied","PTG: Action Denied","The host has disabled non-host usage for this server command.");
		return;
	}
	
	//Make sure necessary script / sim groups are set up (mainly to check if host has allowed non-host server cmd usage above)
	if(!isObject(PTG_GlobalSO))
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","No routine settings detected; please have the server host apply their routine settings to the server first.");
		return;
	}
	
	//Version check not necessary for this function
	
	//////////////////////////////////////////////////

	if(isObject(%pl = %cl.player))
	{
		%plPos = %pl.position;
		%PlPosX = mFloatLength(getWord(%plPos,0),1);
		%PlPosY = mFloatLength(getWord(%plPos,1),1);
		%PlPosZ = mFloatLength(getWord(%plPos,2),1);
		
		%plRot = %pl.rotation;
		%plDir = mFloatLength(getWord(%plRot,3) / 45,0) * 45;
		%plDirB = getWord(%plRot,2);
		
		switch(%plDir)
		{
			case 0:
				%facing = "North (+y)";
			case 45:
				if(%plDirB == -1)
					%facing = "North-West (+y,-x)";
				else
					%facing = "North-East (+y,+x)";
			case 90:
				if(%plDirB == -1)
					%facing = "West (-x)";
				else
					%facing = "East (+x)";
			case 135:
				%facing = "South-East (-y,+x)";
			case 180:
				%facing = "South (-y)";
			case 225:
				%facing = "South-West (-y,-x)";
			case 280: //also same as for case 90 above, but for -1 %plDirB
				%facing = "West (-x)";
			default:
				%facing = "Unknown (?)";
		}
	
		messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\<color:ffffff>>Your position is X: \"\c1" @ %PlPosX @ "<color:ffffff>\" Y: \"\c1" @ %PlPosY @ "<color:ffffff>\" Z: \"\c1" @ %PlPosZ @ "<color:ffffff>\" Facing: \"\c1" @ %facing @ "<color:ffffff>\"");
	}
	else
		PTG_MsgClient(%cl,"Error","PTG: No Player","No player object found! Can't access your coordinates or facing-direction.");
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function SERVERCMDPTGReveal(%cl)
{
	if(%cl !$= "REMOTE" || !$PTG_SekKeyDedRmt)
	{
		 if(!isObject(%cl)) //for remote console
		{
			echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTGAllStatic\" failed; client \"" @ %cl @ "\" not found! [!] \c0->" SPC getWord(getDateTime(),1));
			return;
		}
		if(!$PTG.allowNH_SrvrCmdEventUse && !PTG_HostCheck(%cl))
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","The host has disabled non-host server command and event usage.");
			return;
		}
		if(!%cl.isAdmin)
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only admins are allowed to reveal chunks.");
			return;
		}
		if(%cl.PTGver != 3) //Version check
		{
			PTG_MsgClient(%cl,"Failed","PTG: Action Denied","Version 3 of PTG is required, you don't appear to have it downloaded!");
			return;
		}
	}
	
	//Make sure Chunks BrickGroup exists
	if(!isObject(BrickGroup_Chunks))
	{
		%BGm = new SimGroup("BrickGroup_Chunks");
		mainBrickGroup.add(%BGm);
	}
	
	//Make sure necessary script / sim groups are set up
	if(!isObject(PTG_GlobalSO))
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","No routine settings detected; please have the server host apply their routine settings to the server first.");
		return;
	}
	if(!isObject(PTG_MainSO))
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Main settings not detected; please have the server host or a super admin apply their settings to the server first.");
		return;
	}
	
	//////////////////////////////////////////////////
	
	//If "brickgroup" that holds highlighted chunk objects doesn't exist, create and generate highlight objects
	if(!isObject(BrickGroup_HighlightChunks))
	{
		if(isObject(BrickGroup_Chunks))
		{
			if((%ChCount = BrickGroup_Chunks.getCount()) == 0)
			{
				messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0>P\c3T\c1G: \c6Reveal chunks \c2enabled\c6, but no chunks currently exist.");
				//return; //don't prevent function from continuing encase new bricks are planted / generated and thus if new chunks created
			}
			else
				messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0>P\c3T\c1G: \c6Reveal chunks \c2enabled\c6.");
			
			%nameB = %cl.name @ " (" @ %cl.bl_id @ ")";
			if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %nameB @ " \c0enabled the \"reveal chunks\" option. \c4[R] \c0->" SPC getWord(getDateTime(),1));
			
			//Create necessary sim group(s) / set(s)
			%BGhc = new SimGroup("BrickGroup_HighlightChunks");
			mainBrickGroup.add(%BGhc);
			
			for(%c = 0; %c < %ChCount; %c++)
			{
				%tmpChunk = BrickGroup_Chunks.getObject(%c);
				%tmpChunkNtmp = %tmpChunk.getName();
				%tmpChunkN = strReplace(%tmpChunkNtmp,"_"," ");
				%tmpChX = getWord(%tmpChunkN,1);
				%tmpChY = getWord(%tmpChunkN,2);
				
				//If highest point isn't stored in chunk (i.e. if created for planted / loaded brick), run through all objects in chunk and find highest
				if((%tmpChZ = %tmpChunk.PTGHighestZpos) == 0)
				{
					for(%d = 0; %d < %tmpChunk.getCount(); %d++)
					{
						%tmpObj = %tmpChunk.getObject(%d);
						%tmpObjPosZ = getWord(%tmpObj.getPosition(),2);
						
						//Update highlighted chunk color (if revealed)
						if(%tmpObj.getClassName() $= "fxDTSBrick" && %tmpObjPosZ > %tmpChZ)
							%tmpChZ = %tmpObjPosZ + ((%tmpObjPosZ.brickSizeZ * 0.2) * 0.5);
					}
				}
				
				%newChZ = mCeil(%tmpChZ / 32);
					if(%tmpChunk.PTGChSize > 0) //take variations in chunk size between chunk objects into account
						%ChSize = %tmpChunk.PTGChSize;
					else
						%ChSize = mClamp($PTGm.chSize,16,256);
				%pos = %tmpChX+(%ChSize / 2) SPC %tmpChY+(%ChSize / 2) SPC ((%newChZ / 2) * 32);//$PTGm.boundsHLevel+2;
				%scale = (%ChSize / 16) SPC (%ChSize / 16) SPC %newChZ;
				
				if(%tmpChunk.ChStaticPTG)
					%HLCtype = "HL-Static";
				else
					%HLCtype = "HL-NonStatic";
				
				//Spawn static shape (add to chunk itself) and add to highlight chunk brickgroup
					//also add name to reference if static value is changed for chunk
					//static shape highlight obj will be deleted if chunk itself is deleted
				%tmpStcObj = PTG_Chunk_SpawnStatic(%tmpChunk,%pos,%scale,%HLCtype);
				BrickGroup_HighlightChunks.add(%tmpStcObj);
				%tmpStcObj.setName(strReplace(%tmpChunkNtmp,"Chunk","HLChunk"));
			}
		}
	}
	
	//Otherwise, clear all highlight objects
	else
	{
		for(%c = 0; %c < BrickGroup_HighlightChunks.getCount(); %c++)
			BrickGroup_HighlightChunks.getObject(0).delete();
		
		BrickGroup_HighlightChunks.delete();
		messageClient(%cl,'',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0>P\c3T\c1G: \c6Reveal chunks \c0disabled\c6.");
		
		%nameB = %cl.name @ " (" @ %cl.bl_id @ ")";
		if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: " @ %nameB @ " \c0disabled the \"reveal chunks\" option. \c4[R] \c0->" SPC getWord(getDateTime(),1));
	}
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function SERVERCMDPTGSetDefault(%cl)
{
	 if(!isObject(%cl)) //for remote console
	{
		echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTGSetDefault\" failed; client \"" @ %cl @ "\" not found! [!] \c0->" SPC getWord(getDateTime(),1));
		return;
	}
	if(!%cl.isSuperAdmin && !PTG_HostCheck(%cl)) //super admin check if just a precaution; not necessary
	{
		PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only the host can save new default server settings.");
		return;
	}
	if(%cl.PTGver != 3) //Version check
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Denied","Version 3 of PTG is required, you don't appear to have it downloaded!");
		return;
	}
	
	//Continue if locally connected (otherwise, don't save new defaults - this function requires access to both client-sided and server-sided scripts)
	if(!%cl.isLocalConnection()) //"DatablockGroup.getCount() <= 0" only applies to the server, doesn't check if the client is locally connected
	{
		PTG_MsgClient(%cl,"Error","PTG: Action Denied","You must be locally connected in order to save default settings; saving new defaults doesn't work on dedicated or on other players' servers.");
		return;
	}
	
	//////////////////////////////////////////////////
	
	//Initial
	if($PTG_DefaultSetupPass !$= "Final")
	{
		//"$PTG_DefaultSetupPass" is also set up to be referenced by this function the client preset save / load function
		$PTG_DefaultSetupPass = "Final";
		
		//Save current GUI settings (if you have a preset saved as "Default.txt", it will be overwritten, as will previously saved Routine settings)
		PTG_Cmplx_EdtPresetName.setValue("Default");
		PTG_GUI_PresetFuncs("Save");
		PTG_GUI_SaveLoadRoutine("Save");
		

		scheduleNoQuota(200,0,SERVERCMDPTGSetDefault,%cl);
	}
	
	//Final
	else
	{
		%fpMain = "config/client/PTGv3/Presets/";
		%fpRtn = "config/client/PTGv3/";
		%fpFinal = PTG_GetFP("NewDefaultDirFP","","","");
		
		PTG_Cmplx_EdtPresetName.setValue("");
		deleteVariables("$PTG_DefaultSetupPass");
		
		//Move files to appropriate directory (check if prev and new settings exist)
		if(isFile(%mainSetFile = %fpMain @ "Default.txt")) //main settings
		{
			fileCopy(%mainSetFile,%fpFinal @ "Default.txt");
			%MainPass = true;
		}
		if(isFile(%mainRtnFile = %fpRtn @ "Routines.txt")) //routine settings
		{
			fileCopy(%mainRtnFile,%fpFinal @ "Routines.txt");
			%RtnPass = true;
		}
		if(isFile(%fpFinal @ "Default.txt") && isFile(%fpFinal @ "Routines.txt")) //new default settings
			%FinalPass = true;


		if(!%MainPass || !%RtnPass || !%FinalPass)
		{
			PTG_MsgClient(%cl,"Failed","PTG: Defaults Setup Failed","Either your main or routine settings couldn't be saved successfully; new defaults setup aborted.");
			
			if(isFile(%mainSetFile))
				fileDelete(%mainSetFile);
			if(isFile(%mainRtnFile))
				fileDelete(%mainRtnFile);
		}
		else
		{
			PTG_MsgClient(%cl,"Success","PTG: New Defaults Saved!","New default settings have been saved and will be applied automatically from now on when starting a server.");
			
			//If option to disable default settings is enabled, set to false so new default settings will load next time
			$Pref::Server::PTG::DontLoadDefault = false;
		}
	}
}