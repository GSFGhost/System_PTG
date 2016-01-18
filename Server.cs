//exec("Add-Ons/System_PTG/Server.cs");

exec("Add-Ons/System_PTG/SCRIPTS/Server/Server_Support.cs"); //exec first
exec("Add-Ons/System_PTG/SCRIPTS/Server/Chunks.cs");
exec("Add-Ons/System_PTG/SCRIPTS/Server/Chunks_Support.cs");
exec("Add-Ons/System_PTG/SCRIPTS/Server/Noise.cs");
exec("Add-Ons/System_PTG/SCRIPTS/Server/Noise_Support.cs");
exec("Add-Ons/System_PTG/SCRIPTS/Server/CustomBricks.cs");
exec("Add-Ons/System_PTG/SCRIPTS/Server/Routines.cs");
exec("Add-Ons/System_PTG/SCRIPTS/Server/Commands.cs");
exec("Add-Ons/System_PTG/SCRIPTS/Server/Packages.cs");
exec("Add-Ons/System_PTG/SCRIPTS/Server/ThirdParty.cs");


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////// SCRIPT OBJECTS ////////

//Reset variables
deleteVariables("$PTG");
deleteVariables("$PTGm");
deleteVariables("$PTGbio");
$PTG_init = false;

//Clear previous script objects
if(isObject(PTG_GlobalSO))
	PTG_GlobalSO.delete();
if(isObject(PTG_MainSO)) 
	PTG_MainSO.delete();
if(isObject(PTG_BiomesSO)) 
	PTG_BiomesSO.delete();

if(isObject(PTG_GlobalSO_tmp))
	PTG_GlobalSO_tmp.delete();
if(isObject(PTG_MainSO_tmp)) 
	PTG_MainSO_tmp.delete();
if(isObject(PTG_BiomesSO_tmp)) 
	PTG_BiomesSO_tmp.delete();

if(isObject(PTG_GlobalSO_SPtmp))
	PTG_GlobalSO_SPtmp.delete();
if(isObject(PTG_MainSO_SPtmp)) 
	PTG_MainSO_SPtmp.delete();
if(isObject(PTG_BiomesSO_SPtmp)) 
	PTG_BiomesSO_SPtmp.delete();

//Create necessary sim group(s) / set(s)	//mainBrickGroup wouldn't have been created yet...
//if(!isObject("BrickGroup_Chunks"))
//{
//	%Chunk = new SimGroup("BrickGroup_Chunks");
//	mainBrickGroup.add(%Chunk);
//}

//Make sure server presets directory exists (encase installing preset for server early on) ("PTG_GetFP" function should exist by now)
createPath(PTG_GetFP("ServerPresetFP","","",""));


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////// STANDARD CLIENT/SERVER COMM ////////

//// RECEIVE CLIENT VERSION; SEND PRINT & DATABLOCK INFO TO CLIENT ////
function SERVERCMDPTG_ReceiveClVer(%cl,%ver,%secKey)
{
	if(!isObject(%cl)) //for remote console
	{
		echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTG_ReceiveClVer\" failed; client \"" @ %cl @ "\" not found! [!] \c0->" SPC getWord(getDateTime(),1));
		return;
	}
	if(%cl.PTGsecKey && %secKey == %cl.PTGsecKey && %secKey > 0)
	{
		if(%ver > 0)
			%cl.PTGver = %ver;
		else
			%cl.PTGver = "NA"; //prevents constantly rechecking version on player spawn
		%cl.PTGsecKey = "";
		
		//If player has v3, send Color and Print data for GUIs and Presets
		SERVERCMDPTG_Request(%cl,"Prints&Categories");
		
		//If player has v3, send datablock obj count to client (so looking up objects is more efficient for clients, esp. when loading presets)
		SERVERCMDPTG_Request(%cl,"DatablockCount");
	}
	else
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","PTG security key relay failed! This function must be relayed from the server first, then through the client.");
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//// REQUEST SENT FROM CLIENT TO OPEN GUIs OR TO RETRIEVE DATA FOR GUIs ////
function SERVERCMDPTG_Request(%cl,%reqType)
{
	if(!isObject(%cl)) //for remote console
	{
		echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTG_Request\" failed; client object \"" @ %cl @ "\" not found! [!] \c0->" SPC getWord(getDateTime(),1));
		return;
	}
	
	if(%reqType $= "DatablockCount" || %reqType $= "Prints&Categories") //player and add-on version checks not necessary when sending PTG data to client
		%reqTypePass = true;
		
	if(%cl.PTGver != 3) //Version check
	{
		if(!%reqTypePass)
			PTG_MsgClient(%cl,"Failed","PTG: Action Denied","Version 3 of PTG is required, you don't appear to have it downloaded!"); //msg icon not necessary...
		return;
	}
	if(!isObject(%cl.player))
	{
		if(!%reqTypePass)
		{
			PTG_MsgClient(%cl,"Error","PTG: Action Denied","Please wait until you spawn; no player object detected.");
			return;
		}
	}
	
	//////////////////////////////////////////////////
			
	switch$(%reqType)
	{		
		case "ComplexGUI" or "SimplexGUI":
		
			//If non-host settings uploading is disabled
				//Still allow admin to open Complex / Simplex GUI
			//if(!$PTG.allowNOnHost_SetUpload && !PTG_HostCheck(%cl))
			//{
			//	if(%reqType $= "ComplexGUI")
			//		PTG_MsgClient(%cl,"Denied","PTG: Action Denied","The host has disabled non-host settings uploading, so for now only the host can open the PTG Complex GUI.");
			//	else
			//		PTG_MsgClient(%cl,"Denied","PTG: Action Denied","The host has disabled non-host settings uploading, so for now only the host can open the PTG Simplex GUI.");
			//	return;
			//}
			
			//What if non-host server command / event usage is disabled?
			
			if(!%cl.isAdmin)
			{
				PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only admins are allowed to change settings for the generator."); //events too?
				return;
			}

			//Send ModTer Enabled Info (For Server, Not Client)
			if($AddOn__Brick_ModTer_BasicPack == 1 && ($AddOn__Brick_ModTer_InvertedPack == 1 || $PTG_ModTerInvForce)) //client-sided var?
				%Enab_BasicPack = true;
			switch($AddOn__Brick_ModTer_4xPack) //client-sided var?
			{
				case -1: %Enab_4xPack = "Disabled"; //test
				case 0: %Enab_4xPack = "NotFound";
				case 1: %Enab_4xPack = "Enabled";
			}
			
			commandToClient(%cl,'PTG_GUI_OpenGUI',%reqType,%Enab_BasicPack,%Enab_4xPack);
			
		//////////////////////////////////////////////////
			
		case "ChunkManagerGUI":

			if(!%cl.isAdmin)
			{
				PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only admins are allowed to use the Chunk Manager GUI.");
				return;
			}
			
			if(isObject(%pl = %cl.player))
			{
				//// ACCESS DATA FOR THIS CURRENT CHUNK ////
				
				%plPos = %pl.position;
				%posX = mFloor(getWord(%plPos,0) / mClamp($PTGm.chSize,16,256)) * mClamp($PTGm.chSize,16,256);
				%posY = mFloor(getWord(%plPos,1) / mClamp($PTGm.chSize,16,256)) * mClamp($PTGm.chSize,16,256);
				%Chunk = "Chunk_" @ %posX @ "_" @ %posY;
				
				//Chunk brick count
				if(isObject(%Chunk))
				{
					%exists = true;
					//%brCount = %Chunk.getCount(); //this isn't accurate since it'll also take boundaries, ghost bricks and static shapes into account
					
					for(%c = 0; %c < %Chunk.getCount(); %c++)
					{
						//"%tmpBr.isPlanted" filters out temp / ghost brick (not really necessary though, just precaution)
						if(!(%tmpBr = %Chunk.getObject(%c)).ChBoundsPTG && %tmpBr.getClassName() $= "fxDTSBrick" && %tmpBr.isPlanted)
							%brCount++;
					}
				}
				else
					%brCount = 0;
				
				//Check if chunk save is present (take both normal and permanent saves into account)
				if(isFile(PTG_GetFP("Chunk-Norm",%Chunk)) || isFile(PTG_GetFP("Chunk-Perm",%Chunk))) 
					%savePres = true;
				
				//Chunk pos, player pos, if chunk is static, if unstable, if edited, brick count and if chunk save is present
				%info = %posX SPC %posY TAB getWord(%plPos,0) SPC getWord(%plPos,1) TAB %Chunk.ChStaticPTG TAB %Chunk.ChUnstablePTG TAB %Chunk.ChEditedPTG TAB %brCount TAB %savePres;
				
				
				//// ACCESS DATA FOR ALL EXISTING CHUNKS ////
				
				//Total chunk count
				if(isObject(BrickGroup_Chunks))
					%chCount = BrickGroup_Chunks.getCount();
				else
					%chCount = 0;
				
				//Total chunk save count
				if(!$PTG_init)
					%saveCount = "N/A";
				else
					%saveCount = getFileCount(PTG_GetFP("ChunkSeed","","",""));

				//Send data to client
				commandToClient(%cl,'PTG_ReceiveChunkInfo',%exists,%info,%chCount,%saveCount);
			}
			else
				PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Your player object doesn't exist, can't figure relative chunk.");
			
		//////////////////////////////////////////////////
			
		case "Prints&Categories":
		
			//Note: "getPrintTexture(%num)" doesn't remove prints that are disabled after restarting server, it only adds new prints and overwrites
				//those already in list when necessary. Using "$globalPrintCount" as "%num" (instead of "getNumPrintTextures()") will allow scripts to 
				//only reference prints that are overwritten, instead of entire print list.
			
			%printNum = $globalPrintCount; //"getNumPrintTextures()" doesn't seem to update when prints are disabled
			commandToClient(%cl,'PTG_ReceiveGUIPrints',"PrintCount",%printNum);
			
			for(%c = 0; %c < %printNum; %c++)
			{
				%priFP = getPrintTexture(%c);
				%data = %c TAB %priFP;
				commandToClient(%cl,'PTG_ReceiveGUIPrints',"CreateList",%data);
			}
			
			commandToClient(%cl,'PTG_ReceiveGUIPrints',"SetupCatList","");
		
		//////////////////////////////////////////////////
		
		case "PosAsStart" or "PosAsCenter":

			if(!isObject(%cl.player))
			{
				PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Your player object doesn't exist, can't grab position!");
				return;
			}
			
			//If host-only server cmd / GUI access enabled
			if(!$PTG.allowNOnHost_SetUpload && !PTG_HostCheck(%cl))
			{
				PTG_MsgClient(%cl,"Denied","PTG: Action Denied","The host has disabled non-host settings uploading, so for now only the host can use this GUI command.");
				return;
			}
			if(!%cl.isAdmin)
			{
				PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only admins are allowed to use this GUI command."); //events too?
				return;
			}
			
			//if(!mClamp($PTGm.chSize,16,256))
			//{
			//	PTG_MsgClient(%cl,"Failed","PTG: No Chunk Size","Current chunk size not found! GUI settings must first be sent to the server by clicking the \"Apply and Start\" button.");
			//	return;
			//}
			
			%pos = %cl.player.position;
			commandToClient(%cl,'PTG_ReceiveGUIData',%reqType,getWord(%pos,0) SPC getWord(%pos,1)); // SPC mClamp($PTGm.chSize,16,256));
		
		//////////////////////////////////////////////////
		
		case "DatablockCount":
		
			//Shouldn't have to check if Datablock group exists; should always exist for server
				//A var is sent to all clients (encase they become admin later on) to prevent searching through all existing objs on server when loading presets, etc.
				//Clients will only search through first objs relative to the datablock count, thus only datablocks are referenced when necessary by clients
			commandToClient(%cl,'PTG_ReceiveGUIData',%reqType,DatablockGroup.getCount());
		
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//// UPLOAD COMPLEX GUI SETTINGS FROM CLIENT AND APPLY TO SERVER ////
function SERVERCMDPTG_SetUploadRelay(%cl,%action,%strA,%strB,%strC,%strD,%secKey,%autoStart)
{
	if(%action !$= "Clear") //if simply clearing progress, don't run checks below
	{
		if(!isObject(%cl)) //for remote console
		{
			echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTG_SetUploadRelay\" failed; client \"" @ %cl @ "\" not found! [!] \c0->" SPC getWord(getDateTime(),1));
			SERVERCMDPTG_SetUploadRelay(%cl,"Clear","","","","","","");
			return;
		}
		if(!%cl.isAdmin)
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only admins are allowed to upload their settings to the server.");
			SERVERCMDPTG_SetUploadRelay(%cl,"Clear","","","","","","");
			return;
		}
		if(%cl.PTGver != 3) //Version check
		{
			PTG_MsgClient(%cl,"Failed","PTG: Action Denied","Version 3 of PTG is required, you don't appear to have it downloaded!"); //msg icon not necessary...
			SERVERCMDPTG_SetUploadRelay(%cl,"Clear","","","","","","");
			return;
		}
		
		//If host-only uploading enabled
		if(!$PTG.allowNOnHost_SetUpload && !PTG_HostCheck(%cl))
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","The host has disabled non-host settings uploading.");
			SERVERCMDPTG_SetUploadRelay(%cl,"Clear","","","","","","");
			return;
		}

		//Prevent issues with conflicting server commands
		if($PTG.routine_isHalting)
		{
			PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Terrain generation routine is currently ending, please wait for it to finish first.");
			SERVERCMDPTG_SetUploadRelay(%cl,"Clear","","","","","","");
			return;
		}

		//Prevent uploading settings while clearing terrain (disabled)
		//if($PTG.routine_Process $= "Clear" && %action !$= "RoutinesOnly") // && $PTG_init //$PTG.routine_Process !$= "None"
		//{
			//if($PTG.routine_Process $= "Gen")
			//	PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Generation routine already running! Please stop the routine before updating settings.");
			//if($PTG.routine_Process $= "Clear")
				
		//	PTG_MsgClient(%cl,"Error","PTG: Action Failed","Can't upload settings while terrain is being cleared.");
		//	SERVERCMDPTG_SetUploadRelay(%cl,"Clear","","","","","","");
		//	return;
		//}
		
		//Prevent conflicting with server preset that might also be loading
		if(isObject(PTG_MainSO_SPtmp) && PTG_MainSO_SPtmp.presetLoadInProg)
		{
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Can't upload settings while a server preset is also being loaded.");
			SERVERCMDPTG_SetUploadRelay(%cl,"Clear","","","","","","");
			return;
		}

		//Security Key and Script Object Checks
		if(%action !$= "Initialize" && %action !$= "RoutinesOnly")
		{
			if(!%cl.PTGupldSecKey || %secKey != %cl.PTGupldSecKey || %secKey <= 0)
			{
				PTG_MsgClient(%cl,"Failed","PTG: Action Failed","PTG security keys don't match; settings upload failed. Either this function wasn't initialized, or you attempted a rapid execution of the function.");
				SERVERCMDPTG_SetUploadRelay(%cl,"Clear","","","","","","");
				return;
			}
			if((%action $= "Routines" && !isObject(PTG_GlobalSO_tmp)) || !isObject(PTG_MainSO_tmp) || !isObject(PTG_BiomesSO_tmp))
			{
				PTG_MsgClient(%cl,"Failed","PTG: Action Failed","PTG temporary script objects can't be found!");
				SERVERCMDPTG_SetUploadRelay(%cl,"Clear","","","","","","");
				return;
			}
		}
	}
	
	//////////////////////////////////////////////////
	
	//Use fast secure filter as default, otherwise use more secure (yet slightly slower) filter if server pref var is true)
	if(!$Pref::Server::PTG::GreaterUpldSec)
	{
		//make sure only integers and floats are received from client as additional security ("." and "-" are not excluded)
		%strA = stripChars(%strA,"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ|(){}[]<>\"\'\\/~`!@#$%^&*_+=,?:;"); //must include both upper and lower casing
		%strB = stripChars(%strB,"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ|(){}[]<>\"\'\\/~`!@#$%^&*_+=,?:;");
		%strC = stripChars(%strC,"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ|(){}[]<>\"\'\\/~`!@#$%^&*_+=,?:;");
		%strD = stripChars(%strD,"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ|(){}[]<>\"\'\\/~`!@#$%^&*_+=,?:;");
		//ASCII code and UTF-8 code shouldn't be an issue
	}
	else
	{
		//New Security Measure (only accepts integer and float values)
		for(%b = 0; %b < 4; %b++)
		{
			switch(%b)
			{
				case 0: %strA = PTG_FilterChars(%strA); //getSubStr(%newStr,1,strLen(%newStr)-1);
				case 1: %strB = PTG_FilterChars(%strB); //getSubStr(%newStr,1,strLen(%newStr)-1);
				case 2: %strC = PTG_FilterChars(%strC); //getSubStr(%newStr,1,strLen(%newStr)-1);
				case 3: %strD = PTG_FilterChars(%strD); //getSubStr(%newStr,1,strLen(%newStr)-1);
			}
		}
	}
	
	%printNum = $globalPrintCount; //"getNumPrintTextures()" doesn't seem to update when prints are disabled
	
	if($PTG_secRelayCnt > 100)
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Excessive amount of requests detected while uploading settings; upload progress aborted.");

		if(isObject(%cl))
		{
			messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6Player \c0\"" @ %cl.name @ " (" @ %cl.bl_id @ ")\" \c6sent excessive requests to the server while uploading GUI settings. <color:000000>[^]");
			if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G \c2WARNING: \c0Player \c2\"" @ %cl.name @ " (" @ %cl.bl_id @ ")\" \c0sent excessive requests to the server while uploading GUI settings. \c4[^] \c0->" SPC getWord(getDateTime(),1));
		}
		else
		{
			messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6Unknown player sent excessive requests to the server while uploading GUI settings. <color:000000>[^]");
			if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G \c2WARNING: \c0Unknown player sent excessive requests to the server while uploading GUI settings. \c4[^] \c0->" SPC getWord(getDateTime(),1));
		}
		
		SERVERCMDPTG_SetUploadRelay(%cl,"Clear","","","","","","");
		return;
	}
	
	switch$(%action)
	{
		case "Initialize":

			$PTG_secRelayCnt++;
			%currT = mFloor(getSimTime() / 1000);
			
			if(isObject(PTG_GlobalSO_tmp) || isObject(PTG_MainSO_tmp) || isObject(PTG_BiomesSO_tmp))
			{
				if(isObject(PTG_GlobalSO_tmp))
				{
					if((%currT - PTG_GlobalSO_tmp.lastSetUploadTime) > 30000 || !isObject(PTG_GlobalSO_tmp.lastSetUploadID)) //!isObject($PTG.lastSetUploadID) added recently, double-check!
					{
						PTG_GlobalSO_tmp.uploadingSettings = false; //should be deleted below, but added to be safe for settings upload check below

						if(isObject(PTG_GlobalSO_tmp.lastUploadClient))
							PTG_GlobalSO_tmp.lastUploadClient.PTGupldSecKey = "";
		
						if(isObject(PTG_GlobalSO_tmp)) 
							PTG_GlobalSO_tmp.delete();
						if(isObject(PTG_MainSO_tmp)) 
							PTG_MainSO_tmp.delete();
						if(isObject(PTG_BiomesSO_tmp)) 
							PTG_BiomesSO_tmp.delete();
					}
					else
					{
						if((%rem = mFloor(%currT - PTG_GlobalSO_tmp.lastSetUploadTime)) == 1)
							%plur = "";
						else
							%plur = "s";
						
						PTG_MsgClient(%cl,"Failed","PTG: Previous Upload In Progress","Player \"" @ PTG_GlobalSO_tmp.lastSetUploadPlayer @ "\" ID \"" @ PTG_GlobalSO_tmp.lastSetUploadID @ "\" is currently uploading their settings; started \"" @ %rem @ "\" second" @ %plur @ " ago. Please wait... (they will be dropped if 30s are exceeded)");
						return;
					}

					if(PTG_GlobalSO_tmp.uploadingSettings) //isObject(PTG_GlobalSO_tmp) && 
					{
						PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Settings upload has already been initialized; upload progress aborted.");
						SERVERCMDPTG_SetUploadRelay(%cl,"Clear","","","","","","");
						return;
					}
				}
			}

			if(PTG_HostCheck(%cl))
				MissionCleanup.add(new ScriptObject(PTG_GlobalSO_tmp)); //delete under certain conditions (i.e. created a few minutes prior and not applied to server or altered since)
			MissionCleanup.add(new ScriptObject(PTG_MainSO_tmp));
			MissionCleanup.add(new ScriptObject(PTG_BiomesSO_tmp));
			
			if(isObject(PTG_GlobalSO_tmp))
			{
				PTG_GlobalSO_tmp.lastSetUploadTime = %currT;
				PTG_GlobalSO_tmp.lastSetUploadPlayer = getSubStr(%cl.name,0,30);
				PTG_GlobalSO_tmp.lastSetUploadID = %cl.bl_id;
				PTG_GlobalSO_tmp.uploadingSettings = true; //for server preset loading (prevents conflicting)
				PTG_GlobalSO_tmp.lastUploadClient = %cl;
			}
			$PTG_secRelayCnt = 0;
			
			deleteVariables("$PTG_massDetCurrBiome");
			deleteVariables("$PTG_massDetCurrNum");
			deleteVariables("$PTG_massDetActCount");
			deleteVariables("$PTG_MDfailsafe");

			if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0Player \c4" @ %cl.name @ " (" @ %cl.bl_id @ ") \c0is currently uploading their GUI settings to the server. \c4[^] \c0->" SPC getWord(getDateTime(),1)); //notify host of settings upload attempt (doesn't echo first time settings are uploaded)
			commandToClient(%cl,'PTG_SetUploadRelay',"Setup","",(%cl.PTGupldSecKey = getRandom(1,999999)),%autoStart);
			
		//////////////////////////////////////////////////
		
		case "Setup":
		
			$PTG_secRelayCnt++;
			
			PTG_MainSO_tmp.seed = mFloor(getSubStr(getWord(%strA,0),0,8)); //"mFloor" prevents float values for seed

			//Main Landscape
			if(isObject(%db = getWord(%strA,1)) && %db.getClassName() $= "fxDTSBrickData")
			{
				PTG_MainSO_tmp.brTer_DB = %db.getName();
					PTG_MainSO_tmp.brTer_XYsize = %db.brickSizeX * 0.5;
					PTG_MainSO_tmp.brTer_Zsize = %db.brickSizeZ * 0.2;
					PTG_MainSO_tmp.brTer_FillXYZSize = %db.brickSizeX * 0.5;
					
				PTG_MainSO_tmp.enabModTer = mClamp(getWord(%strA,2),0,1);
				
				PTG_MainSO_tmp.Bio_Def_TerPri = getWord(%strA,3) % %printNum;
				PTG_MainSO_tmp.Bio_Def_TerCol = getWord(%strA,4) % 64; //use mClamp instead of %???
			}
			//PTG_MainSO_tmp.treeBaseCol = getWord(%strA,5) % 64;
				
			PTG_MainSO_tmp.terType = getWord("Normal SkyLands FlatLands NoTerrain",mClamp(getWord(%strA,5),0,3));
			PTG_MainSO_tmp.gridType = getWord("Square Radial",mClamp(getWord(%strA,6),0,1));
			PTG_MainSO_tmp.modTerGenType = getWord("CubesRamps CubesWedges Cubes",mClamp(getWord(%strA,7),0,2));
			PTG_MainSO_tmp.genMethod = getWord("Complete Gradual",mClamp(getWord(%strA,8),0,1)); //Checkbox / Boolean
			PTG_MainSO_tmp.enabAutoSave = mClamp(getWord(%strA,9),0,1);
			
			PTG_MainSO_tmp.gridStartX = getSubStr(getWord(%strA,10),0,8); //snap to chunksize after finding in Advanced settings
			PTG_MainSO_tmp.gridStartY = getSubStr(getWord(%strA,11),0,8);
			PTG_MainSO_tmp.gridEndX = getSubStr(getWord(%strA,12),0,8);
			PTG_MainSO_tmp.gridEndY = getSubStr(getWord(%strA,13),0,8);
			PTG_MainSO_tmp.enabEdgeFallOff = mClamp(getWord(%strA,14),0,1);
			PTG_MainSO_tmp.edgeFOffDist = mClamp(mAbs(getWord(%strA,15)),0,9999); //mclamp? //set character limit in GUI???
			
			PTG_MainSO_tmp.genType = getWord("Finite Infinite",mClamp(getWord(%strA,16),0,1));
			PTG_MainSO_tmp.chrad_P = mClamp(getWord(%strA,17),0,8);
			PTG_MainSO_tmp.chrad_SA = mClamp(getWord(%strA,18),0,8);
			PTG_MainSO_tmp.remDistChs = mClamp(getWord(%strA,19),0,1);
			
			
			commandToClient(%cl,'PTG_SetUploadRelay',"Features","",%cl.PTGupldSecKey,%autoStart);
			
		//////////////////////////////////////////////////
		
		case "Features":

			$PTG_secRelayCnt++;
			
			PTG_MainSO_tmp.dirtPri = getWord(%strA,0) % %printNum;
			PTG_MainSO_tmp.dirtCol = getWord(%strA,1) % 64;
			PTG_MainSO_tmp.skylandsBtmPri = getWord(%strA,2) % %printNum;
			PTG_MainSO_tmp.skylandsBtmCol = getWord(%strA,3) % 64;
			
			PTG_MainSO_tmp.lakesHLevel = mClamp(mFloor(getWord(%strA,4) / PTG_MainSO_tmp.brTer_Zsize) * PTG_MainSO_tmp.brTer_Zsize,0,800);
			PTG_MainSO_tmp.sandHLevel = mClamp(getWord(%strA,5),0,800);
			PTG_MainSO_tmp.terHLevel =  mClamp(mFloor(getWord(%strA,6) / PTG_MainSO_tmp.brTer_Zsize) * PTG_MainSO_tmp.brTer_Zsize,0,400);
			PTG_MainSO_tmp.enabCnctLakes = getWord(%strA,7);
				if(!PTG_MainSO_tmp.enabModTer || PTG_MainSO_tmp.modTerGenType $= "Cubes")
					PTG_MainSO_tmp.enabPlateCap = mClamp(getWord(%strA,8),0,1); //make sure plate-capping auto-disables when terrain ModTer is used
				else
					PTG_MainSO_tmp.enabPlateCap = false;
			PTG_MainSO_tmp.dirtSameTer = mClamp(getWord(%strA,9),0,1);
			PTG_MainSO_tmp.shoreSameCustBiome = mClamp(getWord(%strA,10),0,1);
			PTG_MainSO_tmp.DisableWatGen = mClamp(getWord(%strA,11),0,1);
			
			PTG_MainSO_tmp.detailFreq = mClamp(getWord(%strA,13),0,100);
				if(PTG_MainSO_tmp.detailFreq > 0)
					PTG_MainSO_tmp.enabDetails = mClamp(getWord(%strA,12),0,1);
				else
					PTG_MainSO_tmp.enabDetails = false;
			PTG_MainSO_tmp.enabBio_CustA = mClamp(getWord(%strA,14),0,1);
			PTG_MainSO_tmp.enabBio_CustB = mClamp(getWord(%strA,15),0,1);
			PTG_MainSO_tmp.enabBio_CustC = mClamp(getWord(%strA,16),0,1);
			PTG_MainSO_tmp.autoHideSpawns = mClamp(getWord(%strA,17),0,1);
			PTG_MainSO_tmp.enabFltIsldDetails = mClamp(getWord(%strA,18),0,1);
			
			PTG_MainSO_tmp.enabMntns = mClamp(getWord(%strA,19),0,1);
			PTG_MainSO_tmp.enabMntnSnow = mClamp(getWord(%strA,20),0,1);
			PTG_MainSO_tmp.MntnsSnowHLevel = mClamp(getWord(%strA,21),0,800);
			PTG_MainSO_tmp.mntnsZSnap = mClamp(getWord(%strA,22),1,40);
			PTG_MainSO_tmp.mntnsZMult = mClamp(getWord(%strA,23),1,16); //mClamp floors automatically
			
			PTG_MainSO_tmp.enabCaves = mClamp(getWord(%strA,24),0,1);
			PTG_MainSO_tmp.cavesHLevel = mClamp(mFloor(getWord(%strA,25) / PTG_MainSO_tmp.brTer_Zsize) * PTG_MainSO_tmp.brTer_Zsize,0,400);

			//Clouds
			if(isObject(%db = getWord(%strA,27)) && %db.getClassName() $= "fxDTSBrickData")
			{
				PTG_MainSO_tmp.brClouds_DB = %db.getName();
					PTG_MainSO_tmp.brClouds_XYsize = %db.brickSizeX * 0.5;
					PTG_MainSO_tmp.brClouds_Zsize = %db.brickSizeZ * 0.2;
					PTG_MainSO_tmp.brClouds_FillXYZSize = %db.brickSizeX * 0.5;

				PTG_MainSO_tmp.enabClouds = mClamp(getWord(%strA,26),0,1);
				PTG_MainSO_tmp.enabModTer_Clouds = mClamp(getWord(%strA,28),0,1);
				
				PTG_MainSO_tmp.cloudsPri = getWord(%strA,29) % %printNum;
				PTG_MainSO_tmp.cloudsCol = getWord(%strA,30) % 64;
				PTG_MainSO_tmp.cloudsHLevel = mFloor(getWord(%strA,31) / PTG_MainSO_tmp.brClouds_Zsize) * PTG_MainSO_tmp.brClouds_Zsize;
				PTG_MainSO_tmp.cloudsColl = mClamp(getWord(%strA,32),0,1);
				PTG_MainSO_tmp.modTerGenType_clouds = getWord("CubesRamps CubesWedges Cubes",mClamp(getWord(%strA,33),0,2));
			}
			
			//Floating Islands
			if(isObject(%db = getWord(%strB,1)) && %db.getClassName() $= "fxDTSBrickData")
			{
				PTG_MainSO_tmp.brFltIslds_DB = %db.getName();
					PTG_MainSO_tmp.brFltIslds_XYsize = %db.brickSizeX * 0.5;
					PTG_MainSO_tmp.brFltIslds_Zsize = %db.brickSizeZ * 0.2;
					PTG_MainSO_tmp.brFltIslds_FillXYZSize = %db.brickSizeX * 0.5;

				PTG_MainSO_tmp.enabFltIslds = mClamp(getWord(%strB,0),0,1);
				PTG_MainSO_tmp.enabModTer_FltIslds = mClamp(getWord(%strB,2),0,1); //!!! if decide to use plate-capping for floating islands, auto disable if floating islands ModTer enabled (like terrain) !!!

				PTG_MainSO_tmp.fltIsldsTerPri = getWord(%strB,3) % %printNum;
				PTG_MainSO_tmp.fltIsldsTerCol = getWord(%strB,4) % 64;
				PTG_MainSO_tmp.fltIsldsAHLevel = mFloor(getWord(%strB,5) / PTG_MainSO_tmp.brFltIslds_Zsize) * PTG_MainSO_tmp.brFltIslds_Zsize;
				PTG_MainSO_tmp.fltIsldsBHLevel = mFloor(getWord(%strB,6) / PTG_MainSO_tmp.brFltIslds_Zsize) * PTG_MainSO_tmp.brFltIslds_Zsize;
				PTG_MainSO_tmp.fltIsldsDirtPri = getWord(%strB,7) % %printNum;
				PTG_MainSO_tmp.fltIsldsDirtCol = getWord(%strB,8) % 64;
				PTG_MainSO_tmp.fltIsldsBtmPri = getWord(%strB,9) % %printNum;
				PTG_MainSO_tmp.fltIsldsBtmCol = getWord(%strB,10) % 64;
				PTG_MainSO_tmp.modTerGenType_fltislds = getWord("CubesRamps CubesWedges Cubes",mClamp(getWord(%strB,11),0,2));
			}
			
			PTG_MainSO_tmp.enabBounds = mClamp(getWord(%strB,12),0,1);
			PTG_MainSO_tmp.boundsWallPri = getWord(%strB,13) % %printNum;
			PTG_MainSO_tmp.boundsWallCol = getWord(%strB,14) % 64;
			PTG_MainSO_tmp.boundsCeilPri = getWord(%strB,15) % %printNum;
			PTG_MainSO_tmp.boundsCeilCol = getWord(%strB,16) % 64;
			PTG_MainSO_tmp.boundsHTerRel = mClamp(getWord(%strB,17),0,8);
			PTG_MainSO_tmp.boundsHLevel = mClamp((mFloor(getWord(%strB,18) / 32) * 32),0,992);
			PTG_MainSO_tmp.boundsH_RelToTer = mClamp(getWord(%strB,19),0,1);
			PTG_MainSO_tmp.boundsH_RefTerOff = mClamp(getWord(%strB,20),0,1); //PTG_Cmplx_ChkBndsStrtRelOffset
			PTG_MainSO_tmp.boundsCeil = mClamp(getWord(%strB,21),0,1);
			PTG_MainSO_tmp.boundsStatic = mClamp(getWord(%strB,22),0,1);
			PTG_MainSO_tmp.boundsInvisStatic = mClamp(getWord(%strB,23),0,1);
			
			
			commandToClient(%cl,'PTG_SetUploadRelay',"BuildLoad","",%cl.PTGupldSecKey,%autoStart);
			
		//////////////////////////////////////////////////
		
		case "BuildLoad":
			
			$PTG_secRelayCnt++;
			
			PTG_MainSO_tmp.enabBuildLoad = mClamp(getWord(%strA,0),0,1);
			PTG_MainSO_tmp.allowDetFlatAreas = mClamp(getWord(%strA,2),0,1);
			PTG_MainSO_tmp.BldLdUseMaxHTer = mClamp(getWord(%strA,3),0,1);
			PTG_MainSO_tmp.flatAreaFreq = mClamp(getWord(%strA,4),0,100);
			
				if(PTG_MainSO_tmp.flatAreaFreq > 0)
					PTG_MainSO_tmp.allowFlatAreas = mClamp(getWord(%strA,1),0,1);
				else
					PTG_MainSO_tmp.allowFlatAreas = false;
			
			//PTG_Cmplx_ChkGenDetFlatAreas
			PTG_MainSO_tmp.BLfaGridSizeSmall = mClamp(getWord("2 4 8 16 32 64 128 256",getWord(%strA,5)),2,256);
			PTG_MainSO_tmp.BLfaGridSizeLarge = mClamp(getWord("2 4 8 16 32 64 128 256",getWord(%strA,6)),2,256);
			//	PTG_MainSO_tmp.buildUploadCount = mClamp(getWord(%strA,7),0,400); //only accessed when uploading builds (as a fail-safe)
			
			
			commandToClient(%cl,'PTG_SetUploadRelay',"BiomeDef","",%cl.PTGupldSecKey,%autoStart);
		
		//////////////////////////////////////////////////
		
		case "BiomeDef" or "BiomeShore" or "BiomeSubM" or "BiomeCustA" or "BiomeCustB" or "BiomeCustC" or "BiomeCaveTop" or "BiomeCaveBtm" or "BiomeMntn":
		
			$PTG_secRelayCnt++;
			%bioStr = getSubStr(getWord(strReplace(%action,"Biome",""),0),0,11); //bioStr doesn't get sent to eval command below unless it matches a case above, so it's secure (getWord and getSubStr used just encase)

			//Mountains
			if(%action $= "BiomeMntn")
			{
				PTG_BiomesSO_tmp.Bio_Mntn_RockPri = getWord(%strA,0) % %printNum;
				PTG_BiomesSO_tmp.Bio_Mntn_RockCol = getWord(%strA,1) % 64;
				
				PTG_BiomesSO_tmp.Bio_Mntn_SnowPri = getWord(%strA,2) % %printNum;
				PTG_BiomesSO_tmp.Bio_Mntn_SnowCol = getWord(%strA,3) % 64;
			}
			
			//Biomes and Caves
			else
			{
				if(%action !$= "BiomeDef")
				{
					eval("PTG_BiomesSO_tmp.Bio_" @ %bioStr @ "_TerPri = getWord(%strA,0) % %printNum;");  //evals are secure
					eval("PTG_BiomesSO_tmp.Bio_" @ %bioStr @ "_TerCol = getWord(%strA,1) % 64;");
				}
				
				if(%action $= "BiomeDef" || %action $= "BiomeCustA" || %action $= "BiomeCustB" || %action $= "BiomeCustC")
				{
					eval("PTG_BiomesSO_tmp.Bio_" @ %bioStr @ "_Wat_Pri = getWord(%strA,2) % %printNum;");
					eval("PTG_BiomesSO_tmp.Bio_" @ %bioStr @ "_Wat_Col = getWord(%strA,3) % 64;");
					
					if(%action !$= "BiomeDef")
						eval("PTG_BiomesSO_tmp.Bio_" @ %bioStr @ "_TerHMod = mClampF(getWord(%strA,4),0.25,8);");
					
					//eval("PTG_BiomesSO_tmp.Bio_" @ %bioStr @ "_AllowMntns = getWord(%strA,5);"); //dropped for this version
					eval("PTG_BiomesSO_tmp.Bio_" @ %bioStr @ "_Wat_Type = getWord(\"Normal Lava Ice QuickSand\",mClamp(getWord(%strA,6),0,3));");
				}
			}
			
			for(%c = 0; %c < 18; %c++)
			{
				switch(%c)
				{
					case 0:
						%detNum = 0;
						%relStr = %strB;
					case 6:
						%detNum = 0;
						%relStr = %strC;
					case 12:
						%detNum = 0;
						%relStr = %strD;
				}

				//Details
				if(isObject(%db = getWord(%relStr,%detNum)) && %db.getClassName() $= "fxDTSBrickData")
				{
					eval("PTG_BiomesSO_tmp.Bio_" @ %bioStr @ "_Det" @ %c @ "_BrDB = %db;"); //.getName()?
					eval("PTG_BiomesSO_tmp.Bio_" @ %bioStr @ "_Det" @ %c @ "_Pri = getWord(%relStr,%detNum+1) % %printNum;");
					eval("PTG_BiomesSO_tmp.Bio_" @ %bioStr @ "_Det" @ %c @ "_Col = getWord(%relStr,%detNum+2) % 64;");
				}
				
				%detNum += 3;
			}
			
			%nxtFunc = "Advanced";
			
			//Grab values for next feature from client
			for(%c = 0; %c < 9; %c++)
			{
				if(getWord("BiomeDef BiomeShore BiomeSubM BiomeCustA BiomeCustB BiomeCustC BiomeCaveTop BiomeCaveBtm BiomeMntn",%c) $= %action)
				{
					%nxtFunc = getWord("BiomeShore BiomeSubM BiomeCustA BiomeCustB BiomeCustC BiomeCaveTop BiomeCaveBtm BiomeMntn Advanced",%c);
					break;
				}
			}
			
			
			commandToClient(%cl,'PTG_SetUploadRelay',%nxtFunc,"",%cl.PTGupldSecKey,%autoStart);
		
		//////////////////////////////////////////////////
		
		case "Advanced":

			$PTG_secRelayCnt++;
			
			%ChSize = PTG_MainSO_tmp.chSize = getWord("16 32 64 128 256",mClamp(getWord(%strA,0),0,4)); //mClamp((mFloor(getWord(%strA,0) / 16) * 16),16,256);
				PTG_MainSO_tmp.gridStartX = mFloor(PTG_MainSO_tmp.gridStartX / %ChSize) * %ChSize; //snap finite grid start/end pos to chunk size
				PTG_MainSO_tmp.gridStartY = mFloor(PTG_MainSO_tmp.gridStartY / %ChSize) * %ChSize;
				PTG_MainSO_tmp.gridEndX = mCeil(PTG_MainSO_tmp.gridEndX / %ChSize) * %ChSize; //snap to ceiling value for XY end positions, also relative to chunk size
				PTG_MainSO_tmp.gridEndY = mCeil(PTG_MainSO_tmp.gridEndY / %ChSize) * %ChSize;
			PTG_MainSO_tmp.caveTopZMult = mClamp(getWord(%strA,1),1,8);
			PTG_MainSO_tmp.zMod = mFloor(mClamp(mAbs(getWord(%strA,2)),0,200) / 2) * 2;
			PTG_MainSO_tmp.cnctLakesStrt = mClamp(mAbs(getWord(%strA,3)),1,200);
			PTG_MainSO_tmp.TreeBaseACol = getWord(%strA,4) % 64;
			PTG_MainSO_tmp.TreeBaseBCol = getWord(%strA,5) % 64;
			PTG_MainSO_tmp.TreeBaseCCol = getWord(%strA,6) % 64;
			PTG_MainSO_tmp.FIFOchClr = mClamp(getWord(%strA,7),0,1);
			PTG_MainSO_tmp.seamlessModTer = mClamp(getWord(%strA,8),0,1);
			PTG_MainSO_tmp.seamlessBuildL = mClamp(getWord(%strA,9),0,1);
			
			
			PTG_MainSO_tmp.cavesSecZ = getSubStr(mAbs(getWord(%strA,10)),0,8);
			PTG_MainSO_tmp.skyLndsSecZ = getSubStr(mAbs(getWord(%strA,11)),0,8);
			PTG_MainSO_tmp.fltIsldsSecZ = getSubStr(mAbs(getWord(%strA,12)),0,8);
			PTG_MainSO_tmp.bio_CustASecZ = getSubStr(mAbs(getWord(%strA,13)),0,8);
			PTG_MainSO_tmp.bio_CustBSecZ = getSubStr(mAbs(getWord(%strA,14)),0,8);
			PTG_MainSO_tmp.bio_CustCSecZ = getSubStr(mAbs(getWord(%strA,15)),0,8);
			PTG_MainSO_tmp.cloudsSecZ = getSubStr(mAbs(getWord(%strA,16)),0,8);
			PTG_MainSO_tmp.mntnsStrtSecZ = getSubStr(mAbs(getWord(%strA,17)),0,8);
			
			
			PTG_MainSO_tmp.terOff_X = getSubStr(getWord(%strB,0),0,8);
			PTG_MainSO_tmp.terOff_y = getSubStr(getWord(%strB,1),0,8);
			PTG_MainSO_tmp.mntnsOff_X = getSubStr(getWord(%strB,2),0,8);
			PTG_MainSO_tmp.mntnsOff_Y = getSubStr(getWord(%strB,3),0,8);
			PTG_MainSO_tmp.bio_CustAOff_X = getSubStr(getWord(%strB,4),0,8);
			PTG_MainSO_tmp.bio_CustAOff_Y = getSubStr(getWord(%strB,5),0,8);
			PTG_MainSO_tmp.bio_CustBOff_X = getSubStr(getWord(%strB,6),0,8);
			PTG_MainSO_tmp.bio_CustBOff_Y = getSubStr(getWord(%strB,7),0,8);
			PTG_MainSO_tmp.bio_CustCOff_X = getSubStr(getWord(%strB,8),0,8);
			PTG_MainSO_tmp.bio_CustCOff_Y = getSubStr(getWord(%strB,9),0,8);
			PTG_MainSO_tmp.caveAOff_X = getSubStr(getWord(%strB,10),0,8);
			PTG_MainSO_tmp.caveAOff_Y = getSubStr(getWord(%strB,11),0,8);
			PTG_MainSO_tmp.caveBOff_X = getSubStr(getWord(%strB,12),0,8);
			PTG_MainSO_tmp.caveBOff_Y = getSubStr(getWord(%strB,13),0,8);
			PTG_MainSO_tmp.fltIsldsOff_X = getSubStr(getWord(%strB,14),0,8);
			PTG_MainSO_tmp.fltIsldsOff_Y = getSubStr(getWord(%strB,15),0,8);
			PTG_MainSO_tmp.fltIsldsBOff_X = getSubStr(getWord(%strB,16),0,8);
			PTG_MainSO_tmp.fltIsldsBOff_Y = getSubStr(getWord(%strB,17),0,8);
			PTG_MainSO_tmp.skyLandsOff_X = getSubStr(getWord(%strB,18),0,8);
			PTG_MainSO_tmp.skyLandsOff_Y = getSubStr(getWord(%strB,19),0,8);
			PTG_MainSO_tmp.detailsOff_X = getSubStr(getWord(%strB,20),0,8);
			PTG_MainSO_tmp.detailsOff_Y = getSubStr(getWord(%strB,21),0,8);
			PTG_MainSO_tmp.cloudsOff_X = getSubStr(getWord(%strB,22),0,8);
			PTG_MainSO_tmp.cloudsOff_Y = getSubStr(getWord(%strB,23),0,8);
			PTG_MainSO_tmp.buildLoad_X = getSubStr(getWord(%strB,24),0,8);
			PTG_MainSO_tmp.buildLoad_Y = getSubStr(getWord(%strB,25),0,8);

			
			%tmpBrSize = PTG_MainSO_tmp.brTer_XYsize;
			
			PTG_MainSO_tmp.ter_itrA_XY = mClamp(mFloor(mAbs(getWord(%strC,0)) / %ChSize) * %ChSize,0,16384); //0 is minimum clamp value to alert player of issue if value is too small
			PTG_MainSO_tmp.ter_itrA_Z = mClampF(mAbs(getWord(%strC,1)),0,8);
			PTG_MainSO_tmp.ter_itrB_XY = mClamp(mFloor(mAbs(getWord(%strC,2)) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_tmp.ter_itrB_Z = mClampF(mAbs(getWord(%strC,3)),0,8);
			
			if((%ItrCxy = mAbs(getWord(%strC,4))) > %ChSize)
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
			PTG_MainSO_tmp.ter_itrC_XY = mClamp(%ItrCxy,0,16384); //since itrC can be < chSize, snap to terrain brick size instead (if issue w/ brick datablock, this will be 0)
			
			PTG_MainSO_tmp.ter_itrC_Z = mClampF(mAbs(getWord(%strC,5)),0,8);
			PTG_MainSO_tmp.mntn_itrA_XY = mClamp(mFloor(mAbs(getWord(%strC,6)) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_tmp.mntn_itrA_Z = mClampF(mAbs(getWord(%strC,7)),0,8);
			PTG_MainSO_tmp.mntn_itrB_XY = mClamp(mFloor(mAbs(getWord(%strC,8)) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_tmp.mntn_itrB_Z = mClampF(mAbs(getWord(%strC,9)),0,8);
			PTG_MainSO_tmp.caveA_itrA_XY = mClamp(mFloor(mAbs(getWord(%strC,10)) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_tmp.caveA_itrA_Z = mClampF(mAbs(getWord(%strC,11)),0,8);
			PTG_MainSO_tmp.caveA_itrB_XY = mClamp(mFloor(mAbs(getWord(%strC,12)) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_tmp.caveA_itrB_Z = mClampF(mAbs(getWord(%strC,13)),0,8);
			
			if((%ItrCxy = mAbs(getWord(%strC,14))) > %ChSize)
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
			PTG_MainSO_tmp.caveA_itrC_XY = mClamp(%ItrCxy,0,16384); //also snaps to terrain brick size
			
			PTG_MainSO_tmp.caveA_itrC_Z = mClampF(mAbs(getWord(%strC,15)),0,8);
			PTG_MainSO_tmp.caveB_itrA_XY = mClamp(mFloor(mAbs(getWord(%strC,16)) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_tmp.caveB_itrA_Z = mClampF(mAbs(getWord(%strC,17)),0,8);
			PTG_MainSO_tmp.bio_CustA_itrA_XY = mClamp(mFloor(mAbs(getWord(%strC,18)) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_tmp.bio_CustA_itrA_Z = mClampF(mAbs(getWord(%strC,19)),0,8);
			PTG_MainSO_tmp.bio_CustB_itrA_XY = mClamp(mFloor(mAbs(getWord(%strC,20)) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_tmp.bio_CustB_itrA_Z = mClampF(mAbs(getWord(%strC,21)),0,8);
			PTG_MainSO_tmp.bio_CustC_itrA_XY = mClamp(mFloor(mAbs(getWord(%strC,22)) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_tmp.bio_CustC_itrA_Z = mClampF(mAbs(getWord(%strC,23)),0,8);
			PTG_MainSO_tmp.skyLnds_itrA_XY = mClamp(mFloor(mAbs(getWord(%strC,24)) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_tmp.skyLnds_itrA_Z = mClampF(mAbs(getWord(%strC,25)),0,8);
			
			PTG_MainSO_tmp.clouds_itrA_XY = mClamp(mFloor(mAbs(getWord(%strD,0)) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_tmp.clouds_itrA_Z = mClampF(mAbs(getWord(%strD,1)),0,8);
			PTG_MainSO_tmp.clouds_itrB_XY = mClamp(mFloor(mAbs(getWord(%strD,2)) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_tmp.clouds_itrB_Z = mClampF(mAbs(getWord(%strD,3)),0,8);
			PTG_MainSO_tmp.fltIslds_itrA_XY = mClamp(mFloor(mAbs(getWord(%strD,4)) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_tmp.fltIslds_itrA_Z = mClampF(mAbs(getWord(%strD,5)),0,8);
			PTG_MainSO_tmp.fltIslds_itrB_XY = mClamp(mFloor(mAbs(getWord(%strD,6)) / %ChSize) * %ChSize,0,16384);
			PTG_MainSO_tmp.fltIslds_itrB_Z = mClampF(mAbs(getWord(%strD,7)),0,8);
			
			PTG_MainSO_tmp.enabPseudoEqtr = getSubStr(mAbs(getWord(%strD,8)),0,1);
			PTG_MainSO_tmp.Bio_CustA_YStart = getSubStr(mAbs(getWord(%strD,9)),0,8); //changed from PTG_BiomesSO_tmp
			PTG_MainSO_tmp.Bio_CustB_YStart = getSubStr(mAbs(getWord(%strD,10)),0,8);
			PTG_MainSO_tmp.Bio_CustC_YStart = getSubStr(mAbs(getWord(%strD,11)),0,8);
			PTG_MainSO_tmp.Cave_YStart = getSubStr(mAbs(getWord(%strD,12)),0,8);
			PTG_MainSO_tmp.Mntn_YStart = getSubStr(mAbs(getWord(%strD,13)),0,8);
			PTG_MainSO_tmp.Cloud_YStart = getSubStr(mAbs(getWord(%strD,14)),0,8);
			PTG_MainSO_tmp.FltIsld_YStart = getSubStr(mAbs(getWord(%strD,15)),0,8);
			
			
			$PTG_massDetCurrBiome = "Default";
			$PTG_massDetCurrNum = 0; //+= 56
			$PTG_massDetActCount = 0;
			%nextRelBio = "Shore";
			
			//Clear previous biome mass detail script objects
			for(%c = 0; %c < 9; %c++)
			{
				%tmpBioN = getWord("Default Shore SubMarine CustomA CustomB CustomC CaveTop CaveBottom Mountains",%c);
				
				if(isObject(%tmpBioSO = "PTG_MassBioDetails_" @ %tmpBioN))
					%tmpBioSo.delete(); //".clear()" doesn't work for clearing fields
			}
			
			commandToClient(%cl,'PTG_SetUploadRelay',"MassDetails","Default" SPC 0,%cl.PTGupldSecKey,%autoStart);
	
			//if(PTG_HostCheck(%cl))
			//	commandToClient(%cl,'PTG_SetUploadRelay',"Routines",%cl.PTGupldSecKey,%autoStart);
			//else
			//	PTG_ErrorCheck(%cl);
			//	SERVERCMDPTGStart(%cl);
		
		//////////////////////////////////////////////////
		
		case "MassDetails":
		
		$PTG_secRelayCnt++;
		
		//only upload if mass details for a certain biome is enabled, otherwise skip that biome...

		if(%strA $= "" || %strA $= " ") %strA = "NULL";
		if(%strB $= "" || %strB $= " ") %strB = "NULL";
		if(%strC $= "" || %strC $= " ") %strC = "NULL";
		if(%strD $= "" || %strD $= " ") %strD = "NULL";
		
		switch$($PTG_massDetCurrBiome)
		{
			case "Default": %nextRelBio = "Shore";
			case "Shore": %nextRelBio = "SubMarine";
			case "SubMarine": %nextRelBio = "CustomA";
			case "CustomA": %nextRelBio = "CustomB";
			case "CustomB": %nextRelBio = "CustomC";
			case "CustomC": %nextRelBio = "CaveTop";
			case "CaveTop": %nextRelBio = "CaveBottom";
			case "CaveBottom": %nextRelBio = "Mountains";
			case "Mountains": %nextRelBio = "END";
			
			default:
				//
		}
		
		
		//Add string data to fields on appropriate script groups
		if(($PTG_massDetCurrNum + 14) <= 400 && %strA !$= "NULL")
		{
			//Make sure script object exists first
			if(!isObject(%tmpRelBioN = "PTG_MassBioDetails_" @ $PTG_massDetCurrBiome)) //delete after (on re-upload if not used?)
			{
				%MDlist = new SimGroup(%tmpRelBioN);
				MissionCleanup.add(%MDlist);
			}
			else
				%MDlist = %tmpRelBioN;
			
			
			for(%a = 0; %a < 14; %a++) //make sure %a++
			{
				%relCount = (%a * 3);

				if((%tmpBr = getWord(%strA,%relCount)) != -1 && %tmpBr !$= " " && isObject(%tmpBr) && $PTG_massDetActCount < 400)
				{
					%MDlist.detail[$PTG_massDetActCount] = getWords(%strA,%relCount,%relCount+2);
					$PTG_massDetActCount++;
				}
			}
						
			if(($PTG_massDetCurrNum += 14) <= 400 && $PTG_massDetActCount < 400 && %strB !$= "NULL")
			{
				for(%b = 0; %b < 14; %b++) //make sure %b++
				{
					%relCount = %b * 3;
					
					if((%tmpBr = getWord(%strB,%relCount)) != -1 && %tmpBr !$= " " && isObject(%tmpBr) && $PTG_massDetActCount < 400)
					{
						%MDlist.detail[$PTG_massDetActCount] = getWords(%strB,%relCount,%relCount+2);
						$PTG_massDetActCount++;
					}
				}
				
				if(($PTG_massDetCurrNum += 14) <= 400 && $PTG_massDetActCount < 400 && %strC !$= "NULL")
				{
					for(%c = 0; %c < 14; %c++) //make sure %c++
					{
						%relCount = %c * 3;
						
						if((%tmpBr = getWord(%strC,%relCount)) != -1 && %tmpBr !$= " " && isObject(%tmpBr) && $PTG_massDetActCount < 400)
						{
							%MDlist.detail[$PTG_massDetActCount] = getWords(%strC,%relCount,%relCount+2);
							$PTG_massDetActCount++;
						}
					}
						
					if(($PTG_massDetCurrNum += 14) <= 400 && $PTG_massDetActCount < 400 && %strD !$= "NULL")
					{
						for(%d = 0; %d < 14; %d++) //make sure %d++
						{
							%relCount = %d * 3;
							
							if((%tmpBr = getWord(%strD,%relCount)) != -1 && %tmpBr !$= " " && isObject(%tmpBr) && $PTG_massDetActCount < 400)
							{
								%MDlist.detail[$PTG_massDetActCount] = getWords(%strD,%relCount,%relCount+2);
								$PTG_massDetActCount++;
							}
						}
						
						%MDlist.totalDetAm = $PTG_massDetActCount; //otherwise doesn't update correctly
						commandToClient(%cl,'PTG_SetUploadRelay',"MassDetails",$PTG_massDetCurrBiome SPC $PTG_massDetCurrNum += 14,%cl.PTGupldSecKey,%autoStart);
					}
					else
						%relBioEnd = true;
				}
				else
					%relBioEnd = true;
			}
			else
				%relBioEnd = true;
		}
		else
			%relBioEnd = true;
		
		
		if($PTG_MDfailsafe++ > 72) //3600 (use "72" instead since 56 details are sent at a time -  (mCeil(400max / 56) * 9biomes))
		{
			%relBioEnd = true;
			%nextRelBio = "END";
		}
		if(%relBioEnd)
		{
			%MDlist.totalDetAm = $PTG_massDetActCount;

			if(%nextRelBio $= "END")
			{
				deleteVariables("$PTG_massDetCurrBiome"); //make sure deleted in packages as well
				deleteVariables("$PTG_massDetCurrNum"); //make sure deleted in packages as well
				deleteVariables("$PTG_massDetActCount");
				deleteVariables("$PTG_MDfailsafe"); //make sure deleted in packages as well
				
				if(PTG_HostCheck(%cl))
					commandToClient(%cl,'PTG_SetUploadRelay',"Routines","",%cl.PTGupldSecKey,%autoStart);
				else
				{
					%errPass = PTG_ErrorCheck(%cl);
				
					if(%errPass && %autoStart)
						SERVERCMDPTGStart(%cl);
					
					SERVERCMDPTG_SetUploadRelay(%cl,"Clear","","","","","","");
					//SERVERCMDPTGStart(%cl);
				}
			}
			else
			{
				$PTG_massDetCurrBiome = %nextRelBio;
				$PTG_massDetCurrNum = 0;
				$PTG_massDetActCount = 0;
				
				commandToClient(%cl,'PTG_SetUploadRelay',"MassDetails",%nextRelBio SPC 0,%cl.PTGupldSecKey,%autoStart);
			}
		}
		
		//////////////////////////////////////////////////
		
		case "Routines" or "RoutinesOnly":

			$PTG_secRelayCnt++;
			
			if(!PTG_HostCheck(%cl))
			{
				if(isObject(PTG_GlobalSO_tmp)) 
					PTG_GlobalSO_tmp.delete();
				if(isObject(PTG_MainSO_tmp)) 
					PTG_MainSO_tmp.delete();
				if(isObject(PTG_BiomesSO_tmp)) 
					PTG_BiomesSO_tmp.delete();
				%cl.PTGupldSecKey = "";
				
				PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only the server host is allowed to change settings for routines!");
				return;
			}
			if(%action $= "RoutinesOnly")
			{
				if(!isObject(PTG_GlobalSO_tmp)) 
					MissionCleanup.add(new ScriptObject(PTG_GlobalSO_tmp));
			}
			
			if(isObject(PTG_GlobalSO))
			{
				//Carry over auxiliary fields (used for storing server instance data) when new routine settings are uploaded
				PTG_GlobalSO_tmp.routine_StartMS = $PTG.routine_StartMS;
				PTG_GlobalSO_tmp.routine_isHalting = $PTG.routine_isHalting;
				PTG_GlobalSO_tmp.routine_Process = $PTG.routine_Process;
				PTG_GlobalSO_tmp.routine_ProcessAux = $PTG.routine_ProcessAux;
				PTG_GlobalSO_tmp.routine_ProcessAm = $PTG.routine_ProcessAm;
						PTG_GlobalSO_tmp.zMax = 4000; //mClamp($PTG.zMax,0,4000);
				PTG_GlobalSO_tmp.lastSetUploadTime = $PTG.lastSetUploadTime;
				PTG_GlobalSO_tmp.lastSetUploadPlayer = $PTG.lastSetUploadPlayer;
				PTG_GlobalSO_tmp.lastSetUploadID = $PTG.lastSetUploadID;
				PTG_GlobalSO_tmp.uploadingSettings = $PTG.uploadingSettings;
				
				PTG_GlobalSO_tmp.lastBuildUploadPlayer = $PTG.lastBuildUploadPlayer;
				PTG_GlobalSO_tmp.lastBuildUploadID = $PTG.lastBuildUploadID;
				PTG_GlobalSO_tmp.lastBuildUploadTime = $PTG.lastBuildUploadTime;
				PTG_GlobalSO_tmp.BuildUploading = $PTG.BuildUploading;
				PTG_GlobalSO_tmp.ForceCancelBldUpld = $PTG.ForceCancelBldUpld;
				PTG_GlobalSO_tmp.UploadBuildName = $PTG.UploadBuildName;
				
				PTG_GlobalSO_tmp.ListingBuild = $PTG.ListingBuild;
				PTG_GlobalSO_tmp.lastBuildListPlayer = $PTG.lastBuildListPlayer;
				PTG_GlobalSO_tmp.lastBuildListID = $PTG.lastBuildListID;
				PTG_GlobalSO_tmp.lastBuildListTime = $PTG.lastBuildListTime;
			
				PTG_GlobalSO_tmp.dedSrvrFuncCheckTime = $PTG.dedSrvrFuncCheckTime;
				PTG_GlobalSO_tmp.lastClientName = $PTG.lastClientName; //not currently used
				PTG_GlobalSO_tmp.lastClientID = $PTG.lastClientID;
				PTG_GlobalSO_tmp.lastClientisLocal = $PTG.lastClientisLocal;
				PTG_GlobalSO_tmp.lastClientisAdmin  = $PTG.lastClientisAdmin; //???
				PTG_GlobalSO_tmp.lastClientisSuperAdmin  = $PTG.lastClientisSuperAdmin; //???
				PTG_GlobalSO_tmp.lastClientisHost  = $PTG.lastClientisHost; //???
				PTG_GlobalSO_tmp.lastClientPtgver = $PTG.lastClientPtgver;
				PTG_GlobalSO_tmp.lastClient = $PTG.lastClient;
				PTG_GlobalSO_tmp.lastUploadClient = $PTG.lastUploadClient;
				
				PTG_GlobalSO_tmp.massDetCurrBiome = $PTG.massDetCurrBiome;
				PTG_GlobalSO_tmp.massDetCurrNum = $PTG.massDetCurrNum;
			}
			else
			{
				PTG_GlobalSO_tmp.routine_isHalting = false; //$PTG. ???
				PTG_GlobalSO_tmp.routine_Process = "None"; //$PTG. ???
				PTG_GlobalSO_tmp.routine_ProcessAux = "None"; //$PTG. ???
				PTG_GlobalSO_tmp.zMax = 4000;	//PTG_GlobalSO_tmp.zMax = 4000; //2000 //400; //auto-calculate?
				PTG_GlobalSO_tmp.uploadingSettings = false;
				
				PTG_GlobalSO_tmp.ForceCancelBldUpld = false; //$PTG. ???
				PTG_GlobalSO_tmp.BuildUploading = false; //$PTG. ???
				PTG_GlobalSO_tmp.ListingBuild = false; //$PTG. ???
				PTG_GlobalSO_tmp.UploadBuildName = ""; //$PTG. ???
			}
			
			
			PTG_GlobalSO_tmp.enabStreams = mClamp(getWord(%strA,0),0,1);
			PTG_GlobalSO_tmp.streamsTickMS = mClamp(getWord(%strA,1),33,2013);
			PTG_GlobalSO_tmp.StreamsHostOnly = mClamp(getWord(%strA,2),0,1);
			PTG_GlobalSO_tmp.solidStreams = mClamp(getWord(%strA,3),0,1);
			PTG_GlobalSO_tmp.streamsClrDetails = mClamp(getWord(%strA,4),0,1);
			PTG_GlobalSO_tmp.streamsMaxDist = mClamp(getWord(%strA,5),0,8);
			PTG_GlobalSO_tmp.genStreamZones = mClamp(getWord(%strA,6),0,1);
			PTG_GlobalSO_tmp.floatStreams = mClamp(getWord(%strA,7),0,1);
			
			PTG_GlobalSO_tmp.brLimitBuffer = mClamp(getWord(%strA,8),0,20000);
			PTG_GlobalSO_tmp.pingMaxBuffer = mClamp(getWord(%strA,9),100,1000);
			PTG_GlobalSO_tmp.DedSrvrFuncBuffer = mClamp(getWord(%strA,10),20,2000);
			PTG_GlobalSO_tmp.chObjLimit = mClamp(getWord(%strA,11),20,4000);
			PTG_GlobalSO_tmp.chSaveLimit_FilesPerSeed = mClamp(getWord(%strA,12),0,100000);
			PTG_GlobalSO_tmp.chSaveLimit_TotalFiles = mClamp(getWord(%strA,13),0,100000);
			PTG_GlobalSO_tmp.buildLoadFileLimit = mClamp(getWord(%strA,14),0,400);
			PTG_GlobalSO_tmp.disBrBuffer = mClamp(getWord(%strA,15),0,1);
			PTG_GlobalSO_tmp.disChBuffer = mClamp(getWord(%strA,16),0,1);
			PTG_GlobalSO_tmp.disNormLagCheck = mClamp(getWord(%strA,17),0,1);
			PTG_GlobalSO_tmp.disDedLagCheck = mClamp(getWord(%strA,18),0,1);
			
			PTG_GlobalSO_tmp.delay_PauseTickS = mClamp(getWord(%strB,0),1,30);
			PTG_GlobalSO_tmp.schedM_autosave = mClamp(getWord(%strB,1),1,60);
			PTG_GlobalSO_tmp.delay_priFuncMS = mClamp(getWord(%strB,2),0,100);
			PTG_GlobalSO_tmp.delay_secFuncMS = mClamp(getWord(%strB,3),0,100);
			PTG_GlobalSO_tmp.calcDelay_priFuncMS = mClamp(getWord(%strB,4),0,100);
			PTG_GlobalSO_tmp.calcDelay_secFuncMS = mClamp(getWord(%strB,5),0,100);
			PTG_GlobalSO_tmp.brDelay_genMS = mClamp(getWord(%strB,6),0,50);
			PTG_GlobalSO_tmp.brDelay_remMS = mClamp(getWord(%strB,7),0,50);
			PTG_GlobalSO_tmp.genSpeed = mClamp(getWord(%strB,8),0,2);
			
			PTG_GlobalSO_tmp.frcBrIntoChunks = mClamp(getWord(%strB,9),0,1);
			PTG_GlobalSO_tmp.AutoCreateChunks = mClamp(getWord(%strB,10),0,1);
			PTG_GlobalSO_tmp.chEditBrPlant = mClamp(getWord(%strB,11),0,1);
			PTG_GlobalSO_tmp.chEditOnWrenchData = mClamp(getWord(%strB,12),0,1);
			PTG_GlobalSO_tmp.chEditBrPPD = mClamp(getWord(%strB,13),0,1);
			PTG_GlobalSO_tmp.chStcBrSpwnPlnt = mClamp(getWord(%strB,14),0,1);
			PTG_GlobalSO_tmp.LoadChFileStc = mClamp(getWord(%strB,15),0,1);
			PTG_GlobalSO_tmp.chSaveMethod = getWord("ifEdited Always Never",mClamp(getWord(%strB,16),0,2));
			PTG_GlobalSO_tmp.chSaveExcdResp = getWord("RemoveOld DontSave",mClamp(getWord(%strB,17),0,1));
			
			PTG_GlobalSO_tmp.publicBricks = mClamp(getWord(%strB,18),0,1);
			PTG_GlobalSO_tmp.DestroyPublicBr = mClamp(getWord(%strB,19),0,1);
			PTG_GlobalSO_tmp.publicBricksPBLs = mClamp(getWord(%strB,20),0,1);
			//getWord(%strB,20); //client-sided only (don't receive from client)
			PTG_GlobalSO_tmp.allowEchos = mClamp(getWord(%strB,21),0,1);
			PTG_GlobalSO_tmp.PreventDestDetail = mClamp(getWord(%strB,22),0,1);
			PTG_GlobalSO_tmp.PreventDestTerrain = mClamp(getWord(%strB,23),0,1);
			PTG_GlobalSO_tmp.PreventDestBounds = mClamp(getWord(%strB,24),0,1);
			PTG_GlobalSO_tmp.allowNonHost_BuildManage = mClamp(getWord(%strB,25),0,1);
			PTG_GlobalSO_tmp.allowNOnHost_SetUpload = mClamp(getWord(%strB,26),0,1);
			PTG_GlobalSO_tmp.allowNH_SrvrCmdEventUse = mClamp(getWord(%strB,27),0,1);
			PTG_GlobalSO_tmp.AllowPlyrPosChk = mClamp(getWord(%strB,28),0,1);
			
			PTG_GlobalSO_tmp.fontSize = mClamp(getWord(%strB,29),1,30);
			
			PTG_GlobalSO_tmp.ChunkHLACol = getWord(%strB,30) % 64;
			PTG_GlobalSO_tmp.ChunkHLBCol = getWord(%strB,31) % 64;			
			
			//Don't run error check if only uploading routine settings (not necessary)
			
			if(%action $= "RoutinesOnly")
			{
				if(isObject(PTG_GlobalSO_tmp))
				{
					PTG_GlobalSO_tmp.uploadingSettings = false;

					if(isObject(PTG_GlobalSO))
						PTG_GlobalSO.delete();
					PTG_GlobalSO_tmp.setName("PTG_GlobalSO");
					$PTG = PTG_GlobalSO;
					
					//PTG_MsgClient(%cl,"Success","PTG: Update Success","Routine settings updated successfully!"); //direct message box notification not really necessary
					messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 \"" @ %cl.name @ "\" uploaded new routine settings <color:000000>[^]");
					if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c4" @ %cl.name @ " (" @ %cl.bl_id @ ") \c0uploaded new routine settings. \c4[^] \c0->" SPC getWord(getDateTime(),1));
				
					SERVERCMDPTG_SetUploadRelay(%cl,"Clear","","","","","","");
				}
			}
			else
			{
				//$PTG_initAttempt = true; //remove eventually???
				//SERVERCMDPTGStart(%cl);
				
				%errPass = PTG_ErrorCheck(%cl);
				
				if(%errPass && %autoStart)
					SERVERCMDPTGStart(%cl);
				
				SERVERCMDPTG_SetUploadRelay(%cl,"Clear","","","","","","");
			}
		
		//////////////////////////////////////////////////
		
		case "Clear":

			//don't allow other players (SuperAdmins) to clear / halt the current player's upload progress
			if(%cl.bl_id == PTG_GlobalSO_tmp.lastSetUploadID)
			{
				if(isObject(PTG_GlobalSO_tmp)) 
					PTG_GlobalSO_tmp.delete();
				if(isObject(PTG_MainSO_tmp)) 
					PTG_MainSO_tmp.delete();
				if(isObject(PTG_BiomesSO_tmp)) 
					PTG_BiomesSO_tmp.delete();
				
				//Should auto-clear these fields once the script object is deleted above
				//$PTG.lastSetUploadTime = ""; //PTG_GlobalSO_tmp???
				//$PTG.lastSetUploadPlayer = ""; //PTG_GlobalSO_tmp???
				//$PTG.lastSetUploadID = ""; //PTG_GlobalSO_tmp???
				//$PTG.uploadingSettings = false; //PTG_GlobalSO_tmp???

				deleteVariables("$PTG_secRelayCnt");
				
				%cl.PTGupldSecKey = ""; //only clear for prev client
			}
			//else
			//	PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Can't clear script objects nor interrupt setting upload progress for other players.");
				
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//// UPLOAD BUILD AND SAVE TO HOST'S COMPUTER IN CUSTOM, CONVERTED FORMAT ////
function SERVERCMDPTG_BuildLoadSrvrFuncs(%cl,%action,%data)
{
	if(!isObject(%cl)) //for remote console
	{
		echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTG_BuildLoadSrvrFuncs\" failed; client \"" @ %cl @ "\" not found! [!] \c0->" SPC getWord(getDateTime(),1));
		return;
	}
	if(!$PTG.allowNonHost_BuildManage && !PTG_HostCheck(%cl))
	{
		PTG_MsgClient(%cl,"Denied","PTG: Action Denied","The host has disabled non-host build uploading and management.");
		commandToClient(%cl,'PTG_BuildUploadRelay',"ForceClear","");
		return;
	}
	if(!%cl.isAdmin)
	{
		PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only admins are allowed to upload and manage builds.");
		commandToClient(%cl,'PTG_BuildUploadRelay',"ForceClear","");
		return;
	}
	if(%cl.PTGver != 3) //Version check
	{
		PTG_MsgClient(%cl,"Failed","PTG: Action Denied","Version 3 of PTG is required, you don't appear to have it downloaded!"); //msg icon not necessary...
		return;
	}
	if(!isObject(PTG_GlobalSO)) //Routine settings check (reference name instead???)
	{
		PTG_MsgClient(%cl,"Failed","PTG: No Routines ScriptGroup","The required server scriptgroup for storing Routine settings doesn't exist! The server host needs to upload their Routine settings before using the build-uploading feature.");
		commandToClient(%cl,'PTG_BuildUploadRelay',"ForceClear","");
		
		return;
	}
	
	//////////////////////////////////////////////////

	//!!!make sure no other action is currently running by this or another player!!!
	//echo messages in console of player changes (if SuperAdmin)
	//cancel upload if client suddenly doesn't exist (leaves) - for GUI settings uploading too!
	//also check if upload canceled or failed (send clear function to client)
	
	switch$(%action)
	{
		case "RequestBuildList":
		
			%currT = mFloor(getSimTime() / 1000);
			
			//Prevent having mult players request builds list at same time due to "PTG_SendBuildList_Recurs" recursion schedules
			if($PTG.ListingBuild)
			{
				if((%currT - $PTG.lastBuildListTime) <= 30000) //encase listing a lot of files
				{
					if((%rem = mFloor(%currT - $PTG.lastBuildListTime)) == 1)
						%plur = "";
					else
						%plur = "s";
					
					PTG_MsgClient(%cl,"Failed","PTG: Previous Listing In Progress","Player \"" @ $PTG.lastBuildListPlayer @ "\" ID \"" @ $PTG.lastBuildListID @ "\" is currently requesting server builds; started \"" @ %rem @ "\" second" @ %plur @ " ago. Please wait... (they will be dropped if 30s are exceeded)");
					return;
				}
			}

			$PTG.ListingBuild = true;
			$PTG.lastBuildListPlayer = getSubStr(%cl.name,0,30);
			$PTG.lastBuildListID = %cl.bl_id;
			$PTG.lastBuildListTime = %currT;
			
			PTG_SendBuildList_Recurs(%cl,0,0); //use recursive function to put less stress on server (esp. if file count is close to the hard-limit of 400)
		
		//////////////////////////////////////////////////
			
		case "InitializeBuildUpload":
		
			//check build file total limit and make sure uploaded file doesn't already exist
			if((%count = getFileCount(%fp = PTG_GetFP("BuildCache","","",""))) >= mClamp($PTG.buildLoadFileLimit,0,400))
			{
				PTG_MsgClient(%cl,"Failed","PTG: Build Limit Reached","The host has set a limit of \"" @ $PTG.buildLoadFileLimit @ "\" build(s) for the server, which has been reached. To make room for new uploads, either have the host raise the limit or remove old builds.");
				commandToClient(%cl,'PTG_ReceiveGUIData',"CloseBldProgWndw","");
				return;
			}
			else
			{
				%buildName = getSubStr(getField(%data,1),0,30);
				
				if(strLen(%buildName) >= 30)
				{
					PTG_MsgClient(%cl,"Failed","PTG: Excessive Name Length","Builds must have a file name that is less than 30 characters; upload aborted for file \"" @ %buildName @ "\".");
					commandToClient(%cl,'PTG_ReceiveGUIData',"CloseBldProgWndw","");
					return;
				}
				
				for(%c = 0; %c < %count; %c++) //make separate file?
				{
					%tmpFP = findNextFile(%fp);
					%newTmpFP = strReplace(%tmpFP,"/","\t"); //convert spaces to tabs in filepath
					%countFld = getFieldCount(%newTmpFP);
					%tmpBuildName = getField(%newTmpFP,%countFld - 2);
					
					if(%buildName $= %tmpBuildName)
					{
						PTG_MsgClient(%cl,"Failed","PTG: Build Already Exists","The build file \"" @ %buildName @ "\" has already been uploaded to the server. Either remove the uploaded file using the Loaded Builds GUI, or upload a different save.");
						commandToClient(%cl,'PTG_ReceiveGUIData',"CloseBldProgWndw","");
						return;
					}
				}
			}
			
			%currT = mFloor(getSimTime() / 1000);
			
			//Prevent uploading my multiple players at the same time
			if($PTG.BuildUploading)
			{
				if(mFloor(%currT - $PTG.lastBuildUploadTime) > 1500000 || !isObject($PTG.lastBuildUploadID)) //1,500,000ms = 25mins //mFloor and "!isObject($PTG.lastBuildUploadID)" added recently, double-check!
				{
					deleteVariables("$PTG_TmpBuildArr_LineUpld*");
					deleteVariables("$PTG_TmpBuildArr_LineCount");
					deleteVariables("$PTG_TmpBuildArr_BuildData");
					deleteVariables("$PTG_BuildLBrConvPass");
				}
				else
				{
					if((%rem = mFloor(%currT - $PTG.lastBuildUploadTime)) == 1)
						%plur = "";
					else
						%plur = "s";
					
					PTG_MsgClient(%cl,"Failed","PTG: Previous Upload In Progress","Player \"" @ $PTG.lastBuildUploadPlayer @ "\" ID \"" @ $PTG.lastBuildUploadID @ "\" is currently uploading a build; started \"" @ mFloor(%rem / 60) @ "\" minute" @ %plur @ " ago. Please wait... (they will be dropped if 25mins are exceeded)");
					
					if($PTG.lastBuildUploadID != %cl.bl_id)
						commandToClient(%cl,'PTG_ReceiveGUIData',"CloseBldProgWndw","");
					
					return;
				}
			}

			$PTG.lastBuildUploadPlayer = getSubStr(%cl.name,0,30);
			$PTG.lastBuildUploadID = %cl.bl_id;
			$PTG.lastBuildUploadTime = %currT;
			$PTG.BuildUploading = true;
			$PTG.UploadBuildName = %buildName;
			$PTG_TmpBuildArr_BuildData = %data TAB 2 TAB 100000 TAB -100000 TAB 100000 TAB -100000 TAB 100000 TAB -100000; //%data with temp relGridSz and min/max XYZ values
			$PTG_TmpBuildArr_LineCount = 0; //?
			$PTG.ForceCancelBldUpld = false;

			if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0Player \"\c4" @ %cl.name @ " (" @ %cl.bl_id @ ")\c0\" is currently uploading the build file \"\c4" @ %buildName @ "\c0\". \c4[^] \c0->" SPC getWord(getDateTime(),1)); //notify host of settings upload attempt
			commandToClient(%cl,'PTG_BuildUploadRelay',"Stage2-GradUpload",0);
			
			//notify of build upload attempt?
			//require (or remove from other funcs) seckey setup and check?
		
		//////////////////////////////////////////////////
		
		case "UploadBuildFile":

			if($PTG.BuildUploading && %cl.bl_id == $PTG.lastBuildUploadID)
			{
				if($PTG.ForceCancelBldUpld)
				{
					PTG_MsgClient(%cl,"Success","PTG: Build Upload Cancelled","Build upload was cancelled successfully.");
					if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0Current build upload was force-cancelled by player with ID \"" @ %cl.bl_id @  "\" \c4[!^] \c0->" SPC getWord(getDateTime(),1));
					SERVERCMDPTG_BuildLoadSrvrFuncs(%cl,"CancelBuildUpload","");
					commandToClient(%cl,'PTG_ReceiveGUIData',"CloseBldProgWndw","");
					
					return;
				}
				
				%buildName = getField($PTG_TmpBuildArr_BuildData,1);
				
				if(strLen(%data) > 255)
				{
					if($PTG_TmpBuildArr_LineCount == 0)
						$PTG_TmpBuildArr_LineCount = "0";
					
					PTG_MsgClient(%cl,"Failed","PTG: Build Upload Failed","Data string on line \"" @ $PTG_TmpBuildArr_LineCount @ "\" for save file \"" @ %buildName @ ".bls\" is too large; limit is 255.");
					SERVERCMDPTG_BuildLoadSrvrFuncs(%cl,"CancelBuildUpload","");
					commandToClient(%cl,'PTG_BuildUploadRelay',"ForceClear","");

					return;
				}
				
				//Brick count check
				if(firstWord(%data) $= "Linecount")
				{
					if((%brCount = getWord(%data,1)) >= 20000)
					{
						PTG_MsgClient(%cl,"Failed","PTG: Build Upload Failed","Brick count for file \"" @ %buildName @ ".bls\" is too large! It must be under 20,000 bricks to be uploaded.");
						SERVERCMDPTG_BuildLoadSrvrFuncs(%cl,"CancelBuildUpload","");
						commandToClient(%cl,'PTG_BuildUploadRelay',"ForceClear","");

						return;
					}
					else if(%brCount <= 0) //"<=" is a precaution, only "==" is necessary
					{
						PTG_MsgClient(%cl,"Failed","PTG: Build Upload Failed","Brick count for file \"" @ %buildName @ ".bls\" doesn't contain any bricks!");
						SERVERCMDPTG_BuildLoadSrvrFuncs(%cl,"CancelBuildUpload","");
						commandToClient(%cl,'PTG_BuildUploadRelay',"ForceClear","");

						return;
					}
					
					$PTG_TmpBuildArr_LineUpld[$PTG_TmpBuildArr_LineCount] = %data;
				}
				
				//Brick Data
				if(getWord(%data,0) $= "BRICKDATA" && isObject(%brObj = getWord(%data,1)))
				{
					//Take brick's rotation into account when finding the X & Y size of the brick (can't use getWorldBox method for brick datablocks)
					if((%rot = getWord(%data,5)) == 0 || %rot == 2)
					{
						%brSzXh = %brObj.brickSizeX * 0.25; //* 0.5 * 0.5
						%brSzYh = %brObj.brickSizeY * 0.25;
					}
					else
					{
						%brSzXh = %brObj.brickSizeY * 0.25; //* 0.5 * 0.5
						%brSzYh = %brObj.brickSizeX * 0.25;
					}
					%brSzZh = %brObj.brickSizeZ * 0.1; //* 0.2 * 0.5
					
					%brPosX = getWord(%data,2);
					%brPosY = getWord(%data,3);
					%brPosZ = getWord(%data,4);
					
					if(mAbs(%brPosX) > 100000 || mAbs(%brPosY) > 100000 || mAbs(%brPosZ) > 100000)
					{
						PTG_MsgClient(%cl,"Failed","PTG: Build Upload Failed","Bricks are out of bounds! The position of bricks (within a save file you wish to upload) must be within 100,000 meters of the origin for X, Y & Z.");
						SERVERCMDPTG_BuildLoadSrvrFuncs(%cl,"CancelBuildUpload","");
						commandToClient(%cl,'PTG_BuildUploadRelay',"ForceClear","");

						return;
					}
					if((%brPosZ - %brSzZh) < 0)
					{
						PTG_MsgClient(%cl,"Failed","PTG: Build Upload Failed","Bricks must rest on or be above the ground in order for a save to be uploaded.");
						SERVERCMDPTG_BuildLoadSrvrFuncs(%cl,"CancelBuildUpload","");
						commandToClient(%cl,'PTG_BuildUploadRelay',"ForceClear","");

						return;
					}
					
					%xMin = %brPosX - %brSzXh - 0.5; //- / + 0.25 to fix area being slightly off
					%xMax = %brPosX + %brSzXh + 0.5;
					%yMin = %brPosY - %brSzYh - 0.5;
					%yMax = %brPosY + %brSzYh + 0.5;
					%zMin = %brPosZ - %brSzZh;
					%zMax = %brPosZ + %brSzZh;
					
					if(%xMin < getField($PTG_TmpBuildArr_BuildData,14))
						$PTG_TmpBuildArr_BuildData = setField($PTG_TmpBuildArr_BuildData,14,%xMin);
					if(%xMax > getField($PTG_TmpBuildArr_BuildData,15))
						$PTG_TmpBuildArr_BuildData = setField($PTG_TmpBuildArr_BuildData,15,%xMax);
					if(%yMin < getField($PTG_TmpBuildArr_BuildData,16))
						$PTG_TmpBuildArr_BuildData = setField($PTG_TmpBuildArr_BuildData,16,%yMin);
					if(%yMax > getField($PTG_TmpBuildArr_BuildData,17))
						$PTG_TmpBuildArr_BuildData = setField($PTG_TmpBuildArr_BuildData,17,%yMax);
					if(%zMin < getField($PTG_TmpBuildArr_BuildData,18))
						$PTG_TmpBuildArr_BuildData = setField($PTG_TmpBuildArr_BuildData,18,%zMin);
					if(%zMax > getField($PTG_TmpBuildArr_BuildData,19))
						$PTG_TmpBuildArr_BuildData = setField($PTG_TmpBuildArr_BuildData,19,%zMax);
					
					if(mAbs(%xMax - %xMin) > 256 || mAbs(%yMax - %yMin) > 256  || mAbs(%zMax - %zMin) > 256) //Z is not for determining relative grid size, but builds should still fit within 4 6x cubes for Z too
					{
						PTG_MsgClient(%cl,"Failed","PTG: Build Upload Failed","Save bounds is too large! Saves must fit within a 8x8x8 grid of 64x cubes in order to be uploaded.");
						SERVERCMDPTG_BuildLoadSrvrFuncs(%cl,"CancelBuildUpload","");
						commandToClient(%cl,'PTG_BuildUploadRelay',"ForceClear","");

						return;
					}
					
					//Convert brick ID to datablock name (for saving to file and for easy-reference when loaded)
					%data = setWord(%data,1,%brObj.getName());
					$PTG_TmpBuildArr_LineUpld[$PTG_TmpBuildArr_LineCount] = %data;
				}
				
				//Events, Owner ID, Name, etc.
				if(getSubStr(%data,0,2) $= "+-")
					$PTG_TmpBuildArr_LineUpld[$PTG_TmpBuildArr_LineCount] = %data;

				$PTG_TmpBuildArr_LineCount++;
				
				if($PTG_TmpBuildArr_LineCount < 20100) //$PTG_TmpBuildArr_LineCount++?
					commandToClient(%cl,'PTG_BuildUploadRelay',"Stage2-GradUpload",$PTG_TmpBuildArr_LineCount); //client sends convert command to this server func
				else
				{
					PTG_MsgClient(%cl,"Failed","PTG: Build Upload Failed","Too many lines of data for save file \"" @ %buildName @ ".bls\"; limit is 20,100.");
					SERVERCMDPTG_BuildLoadSrvrFuncs(%cl,"CancelBuildUpload","");
					commandToClient(%cl,'PTG_BuildUploadRelay',"ForceClear","");

					return;
				}
			}
			else
			{
				PTG_MsgClient(%cl,"Failed","PTG: Build Upload Failed","Either the build upload routine hasn't been initialized, or the client's Blockland ID couldn't be found; upload aborted.");
				//SERVERCMDPTG_BuildLoadSrvrFuncs(%cl,"CancelBuildUpload",""); //??? //might cancel progress for someone else...

				commandToClient(%cl,'PTG_BuildUploadRelay',"ForceClear","");
				return;
			}
		
		//////////////////////////////////////////////////
		
		case "ConvertBuildFile": //find relative bounds and convert to PTG build format
		
			//don't allow other players (SuperAdmins) to create issues with the current player's upload progress

			if($PTG.BuildUploading && $PTG.lastBuildUploadID !$= "")
				PTG_ConvertBuild_Recurs(%cl,0,"",0,0,0,false);
			else
			{
				PTG_MsgClient(%cl,"Failed","PTG: Build Upload Failed","Either the build upload routine hasn't been initialized, or the client's Blockland ID couldn't be found; conversion aborted.");
				SERVERCMDPTG_BuildLoadSrvrFuncs(%cl,"CancelBuildUpload","");
				commandToClient(%cl,'PTG_ReceiveGUIData',"CloseBldProgWndw","");

				return;
			}
		
		//////////////////////////////////////////////////
		
		case "ForceCancelBuildUpload":
			
			if(%cl.bl_id != $PTG.lastBuildUploadID)
			{
				PTG_MsgClient(%cl,"Failed","PTG: Build Cancel Failed","You are not allowed to force-cancel the upload progress for player ID \"" @  $PTG.lastBuildUploadID @ "\".");
				return;
			}
			
			$PTG.ForceCancelBldUpld = true;
			
		//////////////////////////////////////////////////
		
		case "CancelBuildUpload": //when upload finished, upload cancelled, server closed or on upload error halt
		
			//don't allow other players (SuperAdmins) to clear / halt the current player's upload progress
			//same for settings uploading!

			if(!$PTG.BuildUploading)
			{
				PTG_MsgClient(%cl,"Error","PTG: Build Cancel Failed","No build is currently being uploaded.");
				return;
			}
			
			if(%cl.bl_id == $PTG.lastBuildUploadID)
			{
				deleteVariables("$PTG_TmpBuildArr_LineUpld*");
				deleteVariables("$PTG_TmpBuildArr_LineCount");
				deleteVariables("$PTG_TmpBuildArr_BuildData");
				deleteVariables("$PTG_BuildLBrConvPass");
				
				$PTG.lastBuildUploadPlayer = "";
				$PTG.lastBuildUploadID = "";
				$PTG.lastBuildUploadTime = "";
				$PTG.BuildUploading = false;
				$PTG.ForceCancelBldUpld = false;
				$PTG.UploadBuildName = "";
			}
			else
				PTG_MsgClient(%cl,"Failed","PTG: Build Cancel Failed","You are not allowed to cancel the upload progress for player ID \"" @  $PTG.lastBuildUploadID @ "\".");

		//////////////////////////////////////////////////
		
		case "ApplyBuildEdit":

			%enab = getField(%data,0);
			%buildName = stripChars(getSubStr(getField(%data,1),0,30),"^");
			%count = getFileCount(%fp = PTG_GetFP("BuildCache","","",""));
			
			if(%buildName $= $PTG.UploadBuildName)
			{
				PTG_MsgClient(%cl,"Failed","PTG: File Edit Failed","The save file \"" @ %buildName @ "\" is currently being uploaded by another player, and can't be edited until the upload is complete.");
				return;
			}
			
			for(%c = 0; %c < %count; %c++)
			{
				%tmpFP = findNextFile(%fp);
				%newTmpFP = strReplace(%tmpFP,"/","\t"); //convert spaces to tabs in filepath
				%countFld = getFieldCount(%newTmpFP);
				%tmpBuildName = getField(%newTmpFP,%countFld - 2);
				
				if(%buildName $= %tmpBuildName)
				{
					%fileFP = %tmpFP;
					%fileFound = true;
					break;
				}
			}
			
			if(%fileFound && isFile(%fileFP))
			{
				%file = new FileObject();
				%file.openForRead(%fileFP);
				%lineCount = 0;
				
				while(!%file.isEOF())
				{
					%readLn = %file.readLine();
					
					//make permissions check a toggle option in GUI for host?
					if(%lineCount == 1 && getSubStr(%readLn,0,6) $= ">Enab:")
						%fileArr[%lineCount] = ">Enab:" SPC %enab SPC "Freq:" SPC (%freq = getField(%data,12)) SPC "Rot:" SPC (%rot = getField(%data,2)) SPC "BioDef:" SPC getField(%data,3) SPC "BioShore:" SPC getField(%data,4) SPC "BioSubM:" SPC getField(%data,5) SPC "BioCustA:" SPC getField(%data,6) SPC "BioCustB:" SPC getField(%data,7) SPC "BioCustC:" SPC getField(%data,8) SPC "Water:" SPC getField(%data,9) SPC "Mntns:" SPC getField(%data,10) SPC "FltIslds" SPC getField(%data,11);
					else
					{
						%fileArr[%lineCount] = %readLn;
						
						if(%lineCount == 3 && getSubStr(%readLn,0,7) $= ">Owner:")
							%ownerID = getField(%readLn,1);
					}
					%lineCount++;
				}

				%file.openForWrite(%fileFP);
				
				for(%c = 0; %c < %lineCount; %c++)
					%file.writeLine(%fileArr[%c]);
				
				%file.close();
				%file.delete();
				
				
				if(isObject(%cl))
				{
					%plName = getSubStr(%cl.name,0,30);
					%plID = %cl.bl_id;
				}
				else
				{
					%plName = "Unknown";
					%plID = "??????";
				}
				
				//notify other clients with add-on that build list was updated
				for(%c = 0; %c < clientGroup.getCount(); %c++)
				{
					%tmpCl = clientGroup.getObject(%c);
					
					if(%tmpCl.PTGver == 3 && %tmpCl != %cl)
						commandToClient(%tmpCl,'PTG_ReceiveGUIData',"NotifyBldListUpdate",%plName TAB %plID);
				}

				PTG_MsgClient(%cl,"Success","PTG Success: Build File Updated","Changes for the build file \"" @ %buildName @ "\" were applied successfully.");
				commandToClient(%cl,'PTG_ReceiveGUIData',"UpdateBuildList",%enab TAB %buildName TAB %rot TAB getFields(%data,3,11) TAB %freq TAB %ownerID);
			}
			else
			{
				PTG_MsgClient(%cl,"Failed","PTG: File Edit Failed","The build file \"" @ %buildName @ "\" couldn't be found!");
				commandToClient(%cl,'PTG_ReceiveGUIData',"RemoveFromBuildList",%buildName);
			}
			
		//////////////////////////////////////////////////
			
		case "RemoveBuildFile":

			%buildName = stripChars(getSubStr(%data,0,30),"^");
			%count = getFileCount(%fp = PTG_GetFP("BuildCache","","",""));
			
			if(%buildName $= $PTG.UploadBuildName)
			{
				PTG_MsgClient(%cl,"Failed","PTG: File Removal Failed","The save file \"" @ %buildName @ "\" is currently being uploaded by another player, and can't be removed until the upload is complete.");
				return;
			}
			
			for(%c = 0; %c < %count; %c++)
			{
				%tmpFP = findNextFile(%fp);
				%newTmpFP = strReplace(%tmpFP,"/","\t"); //convert spaces to tabs in filepath
				%countFld = getFieldCount(%newTmpFP);
				%tmpBuildName = getField(%newTmpFP,%countFld - 2);
				
				if(%buildName $= %tmpBuildName)
				{
					%fileFP = %tmpFP;
					%fileFound = true;
					break;
				}
			}
			
			if(%fileFound)
			{
				%refFile = %fileFP;
				
				for(%d = 0; %d < 5; %d++)
					if(isfile(%tmpFile = strReplace(%fileFP,"Info",getWord("Info 0 1 2 3",%d))))
						fileDelete(%tmpFile);
					
				//Message player if file was removed or not
				if(!isFile(%refFile))
				{
					if(isObject(%cl))
					{
						%plName = getSubStr(%cl.name,0,30);
						%plID = %cl.bl_id;
					}
					else
					{
						%plName = "Unknown";
						%plID = "??????";
					}
					
					//notify other clients with add-on that build list was updated
					for(%e = 0; %e < clientGroup.getCount(); %e++)
					{
						%tmpCl = clientGroup.getObject(%e);
						
						if(%tmpCl.PTGver == 3 && %tmpCl != %cl)
							commandToClient(%tmpCl,'PTG_ReceiveGUIData',"NotifyBldListUpdate",%plName TAB %plID);
					}
					
					PTG_MsgClient(%cl,"Success","PTG: File Removed Successfully","The build file \"" @ %buildName @ "\" was removed successfully.");
					commandToClient(%cl,'PTG_ReceiveGUIData',"RemoveFromBuildList",%buildName);
				}
				else
					PTG_MsgClient(%cl,"Failed","PTG: File Removal Failed","The build file \"" @ %buildName @ "\" couldn't be removed; reason unknown.");
			}
			else
			{
				PTG_MsgClient(%cl,"Failed","PTG: File Removal Failed","The build file \"" @ %buildName @ "\" couldn't be found!");
				commandToClient(%cl,'PTG_ReceiveGUIData',"RemoveFromBuildList",%buildName);
			}
			
		//////////////////////////////////////////////////
		
		case "QuickToggle":
		
			%buildName = stripChars(getSubStr(%data,0,30),"^");
			%count = getFileCount(%fp = PTG_GetFP("BuildCache","","",""));
			
			if(%buildName $= $PTG.UploadBuildName)
			{
				PTG_MsgClient(%cl,"Failed","PTG: Quick-Toggle Failed","The save file \"" @ %buildName @ "\" is currently being uploaded by another player, and can't be toggled until the upload is complete.");
				return;
			}
			
			for(%c = 0; %c < %count; %c++)
			{
				%tmpFP = findNextFile(%fp);
				%newTmpFP = strReplace(%tmpFP,"/","\t"); //convert spaces to tabs in filepath
				%countFld = getFieldCount(%newTmpFP);
				%tmpBuildName = getField(%newTmpFP,%countFld - 2);
				
				if(%buildName $= %tmpBuildName)
				{
					%fileFP = %tmpFP;
					%fileFound = true;
					break;
				}
			}
			
			if(%fileFound && isFile(%fileFP))
			{
				%file = new FileObject();
				%file.openForRead(%fileFP);
				%lineCount = 0;
				
				while(!%file.isEOF())
				{
					%readLn = %file.readLine();
					
					if(%lineCount == 1 && getSubStr(%readLn,0,6) $= ">Enab:") //make permissions check a toggle option in GUI for host?
					{
						if(%enab = getWord(%readLn,1))
							%enab = 0;
						else
							%enab = 1;
						
						%readLn = setWord(%readLn,1,%enab);
						%freq = getWord(%readLn,3);
						%rot = getWord(%readLn,5);
						%allow = getWord(%readLn,7) TAB getWord(%readLn,9) TAB getWord(%readLn,11) TAB getWord(%readLn,13) TAB getWord(%readLn,15) TAB getWord(%readLn,17) TAB getWord(%readLn,19) TAB getWord(%readLn,21) TAB getWord(%readLn,23);
					}
					if(%lineCount == 3 && getSubStr(%readLn,0,7) $= ">Owner:")
						%ownerID = getField(%readLn,1);
					
					%fileArr[%lineCount] = %readLn;
					%lineCount++;
				}

				%file.openForWrite(%fileFP);
				
				for(%c = 0; %c < %lineCount; %c++)
					%file.writeLine(%fileArr[%c]);
				
				%file.close();
				%file.delete();
				
				
				if(isObject(%cl))
				{
					%plName = getSubStr(%cl.name,0,30);
					%plID = %cl.bl_id;
				}
				else
				{
					%plName = "Unknown";
					%plID = "??????";
				}
				
				//notify other clients with add-on that build list was updated (?)
				for(%c = 0; %c < clientGroup.getCount(); %c++)
				{
					%tmpCl = clientGroup.getObject(%c);
					
					if(%tmpCl.PTGver == 3 && %tmpCl != %cl)
						commandToClient(%tmpCl,'PTG_ReceiveGUIData',"NotifyBldListUpdate",%plName TAB %plID);
				}

				commandToClient(%cl,'PTG_ReceiveGUIData',"UpdateBuildList",%enab TAB %buildName TAB %rot TAB %allow TAB %freq TAB %ownerID);
			}
			else
			{
				PTG_MsgClient(%cl,"Failed","PTG: Quick-Toggle Failed","The build file \"" @ %buildName @ "\" couldn't be found!");
				commandToClient(%cl,'PTG_ReceiveGUIData',"RemoveFromBuildList",%buildName);
			}
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//// UPLOAD BASIC SIMPLEX GUI SETTINGS FROM CLIENT AND APPLY TO SERVER ////
function SERVERCMDPTGSimplexStart(%cl,%data)
{
	if(%action !$= "Clear") //if simply clearing progress, don't run checks below
	{
		if(!isObject(%cl)) //for remote console
		{
			echo("\c2>>P\c1T\c4G \c2ERROR: \"SERVERCMDPTG_SetUploadRelay\" failed; client \"" @ %cl @ "\" not found! [!] \c0->" SPC getWord(getDateTime(),1));
			SERVERCMDPTG_SetUploadRelay(%cl,"Clear","","","","","","");
			return;
		}
		if(!%cl.isAdmin)
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","Only admins are allowed to upload their settings to the server.");
			SERVERCMDPTG_SetUploadRelay(%cl,"Clear","","","","","","");
			return;
		}
		if(%cl.PTGver != 3) //Version check
		{
			PTG_MsgClient(%cl,"Failed","PTG: Action Denied","Version 3 of PTG is required, you don't appear to have it downloaded!"); //msg icon not necessary...
			SERVERCMDPTG_SetUploadRelay(%cl,"Clear","","","","","","");
			return;
		}
		
		//If host-only uploading enabled
		if(!$PTG.allowNOnHost_SetUpload && !PTG_HostCheck(%cl))
		{
			PTG_MsgClient(%cl,"Denied","PTG: Action Denied","The host has disabled non-host settings uploading.");
			SERVERCMDPTG_SetUploadRelay(%cl,"Clear","","","","","","");
			return;
		}

		//Prevent issues with conflicting server commands
		if($PTG.routine_isHalting)
		{
			PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Terrain generation routine is currently ending, please wait for it to finish first.");
			SERVERCMDPTG_SetUploadRelay(%cl,"Clear","","","","","","");
			return;
		}

		//Prevent uploading settings while clearing terrain (disabled)
		//if($PTG.routine_Process $= "Clear" && %action !$= "RoutinesOnly") // && $PTG_init //$PTG.routine_Process !$= "None"
		//{
			//if($PTG.routine_Process $= "Gen")
			//	PTG_MsgClient(%cl,"Failed","PTG: Action Failed","Generation routine already running! Please stop the routine before updating settings.");
			//if($PTG.routine_Process $= "Clear")
				
		//	PTG_MsgClient(%cl,"Error","PTG: Action Failed","Can't upload settings while terrain is being cleared.");
		//	SERVERCMDPTG_SetUploadRelay(%cl,"Clear","","","","","","");
		//	return;
		//}
		
		//Prevent conflicting with server preset that might also be loading
		if(isObject(PTG_MainSO_SPtmp) && PTG_MainSO_SPtmp.presetLoadInProg)
		{
			PTG_MsgClient(%cl,"Error","PTG: Action Failed","Can't upload settings while a server preset is also being loaded.");
			SERVERCMDPTG_SetUploadRelay(%cl,"Clear","","","","","","");
			return;
		}
	}

	//Main upload has 2 different upload filter options, but Simplex upload only receives small amount of data from client, thus uses more secure method only
	%data = PTG_FilterChars(%data); //make sure only integers and floats are received from client
	%printNum = $globalPrintCount;
	
	//prevent mult simplex upload at same time by same player, and by various players
	//setup var so complex GUI and third party func can detect if simplex upload is running
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////

	
	%currT = mFloor(getSimTime() / 1000);
	
	if(isObject(PTG_GlobalSO_tmp) || isObject(PTG_MainSO_tmp) || isObject(PTG_BiomesSO_tmp))
	{
		if(isObject(PTG_GlobalSO_tmp))
		{
			if((%currT - PTG_GlobalSO_tmp.lastSetUploadTime) > 30000 || !isObject(PTG_GlobalSO_tmp.lastSetUploadID)) //!isObject($PTG.lastSetUploadID) added recently, double-check!
			{
				PTG_GlobalSO_tmp.uploadingSettings = false; //should be deleted below, but added to be safe for settings upload check below
				
				if(isObject(PTG_GlobalSO_tmp)) 
					PTG_GlobalSO_tmp.delete();
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
			}
			else
			{
				if((%rem = mFloor(%currT - PTG_GlobalSO_tmp.lastSetUploadTime)) == 1)
					%plur = "";
				else
					%plur = "s";
				
				PTG_MsgClient(%cl,"Failed","PTG: Previous Upload In Progress","Player \"" @ PTG_GlobalSO_tmp.lastSetUploadPlayer @ "\" ID \"" @ PTG_GlobalSO_tmp.lastSetUploadID @ "\" is currently uploading their settings; started \"" @ %rem @ "\" second" @ %plur @ " ago. Please wait... (they will be dropped if 30s are exceeded)");
				return;
			}
		}
	}

	MissionCleanup.add(new ScriptObject(PTG_MainSO_tmp));
	MissionCleanup.add(new ScriptObject(PTG_BiomesSO_tmp));

	if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0Player \c4" @ %cl.name @ " (" @ %cl.bl_id @ ") \c0is currently uploading their Simplex GUI settings to the server. \c4[^] \c0->" SPC getWord(getDateTime(),1)); //notify host of settings upload attempt (doesn't echo first time settings are uploaded)
	

	////////////////////////////////////////////////////////////////////////////////////////////////////


	%seed = getWord(%data,0);
	
	%terBrDB = getWord(%data,1);
	%terBrModTer = getWord(%data,2);
	%terBrCol = getWord(%data,3);
	
	%cloudBrDB = getWord(%data,4);
	%cloudBrModTer = getWord(%data,5);
	%cloudBrCol = getWord(%data,6);
	
	%fltisldsBrDB = getWord(%data,7);
	%fltisldsBrModTer = getWord(%data,8);
	%fltisldsBrCol = getWord(%data,9);
	
	%enabInfTer = getWord(%data,10);
	%enabEdgeFO = getWord(%data,11);
	%terLenY = getWord(%data,12);
	%terWidX = getWord(%data,13);
	%terType = getWord(%data,14);
	
	%enabMntns = getWord(%data,15);
	%enabCaves = getWord(%data,16);
	%enabLakes = getWord(%data,17);
	%enabClouds = getWord(%data,18);
	%enabFltIslds = getWord(%data,19);
	%enabBounds = getWord(%data,20);
	
	%bioDefType = getWord(%data,21) + 1;
	%bioCustAType = getWord(%data,22);
	%bioCustBType = getWord(%data,23);
	%bioCustCType = getWord(%data,24);
	
	
	//////////////////////////////////////////////////
	//Setup
	

	PTG_MainSO_tmp.seed = mFloor(getSubStr(%seed,0,8)); //"mFloor" prevents float values for seed

	//Main Landscape
	if(isObject(%db = %terBrDB) && %db.getClassName() $= "fxDTSBrickData")
	{
		PTG_MainSO_tmp.brTer_DB = %db.getName();
			PTG_MainSO_tmp.brTer_XYsize = %db.brickSizeX * 0.5;
			PTG_MainSO_tmp.brTer_Zsize = %db.brickSizeZ * 0.2;
			PTG_MainSO_tmp.brTer_FillXYZSize = %db.brickSizeX * 0.5;
		
		%brSize = %db.brickSizeX; //used for determining detail frequency
			
		PTG_MainSO_tmp.enabModTer = mClamp(%terBrModTer,0,1);
		
		PTG_MainSO_tmp.Bio_Def_TerPri = PTG_LSP_ConvertPrint(getField("ModTer/Grass" TAB "ModTer/Grass" TAB "ModTer/Grass" TAB "ModTer/Grass" TAB "ModTer/snow" TAB "ModTer/Grass" TAB "ModTer/Grass" TAB "ModTer/rock" TAB "ModTer/whitesand" TAB "ModTer/snow" TAB "ModTer/Grass" TAB "ModTer/Grass" TAB "ModTer/rock" TAB "ModTer/TTdirt01" TAB "ModTer/snow" TAB "ModTer/Grass_03",mClamp(%bioDefType,1,16)));
		PTG_MainSO_tmp.Bio_Def_TerCol = PTG_FindClosestColor(getField("VOID" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.976471 0.976471 0.976471 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.258824 0.247059 0.239216 1.000000" TAB "0.741176 0.584314 0.356863 1.000000" TAB "0.976471 0.976471 0.976471 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.431373 0.541176 0.239216 1.000000" TAB "0.411765 0.392157 0.376471 1.000000" TAB "0.454902 0.313726 0.109804 1.000000" TAB "0.831373 0.800000 0.768627 1.000000" TAB "0.294118 0.000000 0.509804 1.000000",mClamp(%bioDefType,1,16)),"RGBA-RGBAarr");	//PTG_FindClosestColor(getColorIDTable(%terBrCol),"RGBA-RGBAarr");
	}
		
	PTG_MainSO_tmp.terType = getWord("Normal SkyLands FlatLands NoTerrain",mClamp(%terType,0,3));
		//PTG_MainSO_tmp.gridType = getWord("Square Radial",mClamp(getWord(%strA,6),0,1));
	PTG_MainSO_tmp.modTerGenType = "CubesRamps";
	PTG_MainSO_tmp.genMethod = "Complete";
	PTG_MainSO_tmp.enabAutoSave = false;
	
	PTG_MainSO_tmp.gridStartX = 0;
	PTG_MainSO_tmp.gridStartY = 0;
	PTG_MainSO_tmp.gridEndX = mClamp(mFloor(getSubStr(%terWidX,0,8) / 32) * 32,32,2048);
	PTG_MainSO_tmp.gridEndY = mClamp(mFloor(getSubStr(%terLenY,0,8) / 32) * 32,32,2048);
	
	if(mClamp(%enabEdgeFO,0,1) && !mClamp(%enabInfTer,0,1))
	{
		PTG_MainSO_tmp.enabEdgeFallOff = true;
		PTG_MainSO_tmp.edgeFOffDist = 96;
	}
	
	if(!mClamp(%enabInfTer,0,1))
	{
		PTG_MainSO_tmp.genType = "Finite";
		PTG_MainSO_tmp.gridType = "Square";
	}
	else
	{
		PTG_MainSO_tmp.genType = "Infinite";
		PTG_MainSO_tmp.gridType = "Radial";
		
		PTG_MainSO_tmp.chrad_P = 3;
		PTG_MainSO_tmp.chrad_SA = 5;
		PTG_MainSO_tmp.remDistChs = true;
	}
	
	
	//////////////////////////////////////////////////
	//Features
	
	
	PTG_MainSO_tmp.dirtPri = PTG_LSP_ConvertPrint("ModTer/TTdirt01");
	PTG_MainSO_tmp.dirtCol = PTG_FindClosestColor("0.392157 0.192157 0.000000 1.000000","RGBA-RGBAarr");
	PTG_MainSO_tmp.skylandsBtmPri = PTG_LSP_ConvertPrint("ModTer/rock");
	PTG_MainSO_tmp.skylandsBtmCol = PTG_FindClosestColor("0.529412 0.513726 0.494118 1.000000","RGBA-RGBAarr");
	
	if(mClamp(%enabLakes,0,1))
	{
		PTG_MainSO_tmp.DisableWatGen = false;
		
		//Sand / Water Level
		//if(%bioDefType != 16)
		//{
			switch(%bioDefType)
			{
				case 7 or 9 or 10: //Plains or Desert or Arctic Plains
					PTG_MainSO_tmp.sandHLevel = 114;
					PTG_MainSO_tmp.lakesHLevel = 112;
				//case 12: //Swamp
				//	PTG_MainSO_tmp.sandHLevel = 0;
				default:
					PTG_MainSO_tmp.sandHLevel = 128;
					PTG_MainSO_tmp.lakesHLevel = 124;
			}
		//}
		//else
		//	PTG_MainSO_tmp.sandHLevel = 0; //disable sand for alien biome
	}
	else
	{
		PTG_MainSO_tmp.DisableWatGen = true;
		PTG_MainSO_tmp.lakesHLevel = 0;
		PTG_MainSO_tmp.sandHLevel = 0; //sand is disabled if lakes are disabled
	}
	
	

	PTG_MainSO_tmp.terHLevel =  mFloor(100 / PTG_MainSO_tmp.brTer_Zsize) * PTG_MainSO_tmp.brTer_Zsize;
	PTG_MainSO_tmp.enabCnctLakes = false;
	PTG_MainSO_tmp.enabPlateCap = false;
	PTG_MainSO_tmp.dirtSameTer = false;
	PTG_MainSO_tmp.shoreSameCustBiome = false; //true?
	
	//Detail Frequency (auto-adjusts depending on terrain brick size)
	if(%bioDefType == 2 || %bioDefType == 3)
	{
		switch(%brSize)
		{
			case 1:
				PTG_MainSO_tmp.detailFreq = 5;
			case 2:
				PTG_MainSO_tmp.detailFreq = 9;
			case 4:
				PTG_MainSO_tmp.detailFreq = 17;
			case 8:
				PTG_MainSO_tmp.detailFreq = 35;
			case 16:
				PTG_MainSO_tmp.detailFreq = 70;
			case 32:
				PTG_MainSO_tmp.detailFreq = 100;
		}
	}
	else
	{
		switch(%brSize)
		{
			case 1:
				PTG_MainSO_tmp.detailFreq = 2;
			case 2:
				PTG_MainSO_tmp.detailFreq = 5;
			case 4:
				PTG_MainSO_tmp.detailFreq = 10;
			case 8:
				PTG_MainSO_tmp.detailFreq = 20;
			case 16:
				PTG_MainSO_tmp.detailFreq = 40;
			case 32:
				PTG_MainSO_tmp.detailFreq = 80;
		}
	}
	
	PTG_MainSO_tmp.enabDetails = true;
		//PTG_MainSO_tmp.enabBio_CustA = mClamp(getWord(%strA,14),0,1);
		//PTG_MainSO_tmp.enabBio_CustB = mClamp(getWord(%strA,15),0,1);
		//PTG_MainSO_tmp.enabBio_CustC = mClamp(getWord(%strA,16),0,1);
	PTG_MainSO_tmp.autoHideSpawns = false;
	PTG_MainSO_tmp.enabFltIsldDetails = false;
	
	//Mountains
	if(mClamp(%enabMntns,0,1))// || %bioDefType == 8)
	{
		PTG_MainSO_tmp.enabMntns = true;
		PTG_MainSO_tmp.enabMntnSnow = true;
		PTG_MainSO_tmp.MntnsSnowHLevel = 230;
		PTG_MainSO_tmp.mntnsZSnap = 7;
		PTG_MainSO_tmp.mntnsZMult = 5;
	}
	
	//Caves
	if(mClamp(%enabCaves,0,1))
	{
		PTG_MainSO_tmp.enabCaves = true;
		PTG_MainSO_tmp.cavesHLevel = mFloor(40 / PTG_MainSO_tmp.brTer_Zsize) * PTG_MainSO_tmp.brTer_Zsize;
	}

	//Clouds
	if(mClamp(%enabClouds,0,1))
	{
		if(isObject(%db = %cloudBrDB) && %db.getClassName() $= "fxDTSBrickData")
		{
			PTG_MainSO_tmp.brClouds_DB = %db.getName();
				PTG_MainSO_tmp.brClouds_XYsize = %db.brickSizeX * 0.5;
				PTG_MainSO_tmp.brClouds_Zsize = %db.brickSizeZ * 0.2;
				PTG_MainSO_tmp.brClouds_FillXYZSize = %db.brickSizeX * 0.5;

			PTG_MainSO_tmp.enabClouds = true;
			PTG_MainSO_tmp.enabModTer_Clouds = mClamp(%cloudBrModTer,0,1);
			
			PTG_MainSO_tmp.cloudsPri = PTG_LSP_ConvertPrint("ModTer/brickTOP");
			PTG_MainSO_tmp.cloudsCol = PTG_FindClosestColor("0.976471 0.976471 0.976471 1.000000","RGBA-RGBAarr");
			PTG_MainSO_tmp.cloudsHLevel = mFloor(300 / PTG_MainSO_tmp.brClouds_Zsize) * PTG_MainSO_tmp.brClouds_Zsize;
			PTG_MainSO_tmp.cloudsColl = false;
			PTG_MainSO_tmp.modTerGenType_clouds = "CubesRamps";
		}
	}
	
	//Floating Islands
	if(mClamp(%enabFltIslds,0,1))
	{
		if(isObject(%db = %fltisldsBrDB) && %db.getClassName() $= "fxDTSBrickData")
		{
			PTG_MainSO_tmp.brFltIslds_DB = %db.getName();
				PTG_MainSO_tmp.brFltIslds_XYsize = %db.brickSizeX * 0.5;
				PTG_MainSO_tmp.brFltIslds_Zsize = %db.brickSizeZ * 0.2;
				PTG_MainSO_tmp.brFltIslds_FillXYZSize = %db.brickSizeX * 0.5;

			PTG_MainSO_tmp.enabFltIslds = true;
			PTG_MainSO_tmp.enabModTer_FltIslds = mClamp(%fltisldsBrModTer,0,1); //!!! if decide to use plate-capping for floating islands, auto disable if floating islands ModTer enabled (like terrain) !!!

			PTG_MainSO_tmp.fltIsldsTerPri = PTG_LSP_ConvertPrint("ModTer/Grass_03");
			PTG_MainSO_tmp.fltIsldsTerCol = PTG_FindClosestColor("0.000000 0.501961 0.231373 1.000000","RGBA-RGBAarr");
			PTG_MainSO_tmp.fltIsldsAHLevel = mFloor(400 / PTG_MainSO_tmp.brFltIslds_Zsize) * PTG_MainSO_tmp.brFltIslds_Zsize;
			PTG_MainSO_tmp.fltIsldsBHLevel = mFloor(500 / PTG_MainSO_tmp.brFltIslds_Zsize) * PTG_MainSO_tmp.brFltIslds_Zsize;
			PTG_MainSO_tmp.fltIsldsDirtPri = PTG_LSP_ConvertPrint("ModTer/dirt2");
			PTG_MainSO_tmp.fltIsldsDirtCol = PTG_FindClosestColor("0.392157 0.192157 0.000000 1.000000","RGBA-RGBAarr");
			PTG_MainSO_tmp.fltIsldsBtmPri = PTG_LSP_ConvertPrint("ModTer/Old_Stone_Road");
			PTG_MainSO_tmp.fltIsldsBtmCol = PTG_FindClosestColor("0.529412 0.513726 0.494118 1.000000","RGBA-RGBAarr");
			PTG_MainSO_tmp.modTerGenType_fltislds = "CubesRamps";
		}
	}
	
	//Boundaries
	if(mClamp(%enabBounds,0,1))
	{
		PTG_MainSO_tmp.enabBounds = true;
		PTG_MainSO_tmp.boundsWallPri = PTG_LSP_ConvertPrint("ModTer/TTdirt01");
		PTG_MainSO_tmp.boundsWallCol = PTG_FindClosestColor("0.419608 0.258824 0.149020 1.000000","RGBA-RGBAarr");
		PTG_MainSO_tmp.boundsCeilPri = PTG_LSP_ConvertPrint("ModTer/brickTOP");
		PTG_MainSO_tmp.boundsCeilCol = PTG_FindClosestColor("1.000000 1.000000 1.000000 0.694118","RGBA-RGBAarr");
		PTG_MainSO_tmp.boundsHTerRel = 0;
		PTG_MainSO_tmp.boundsHLevel = 320;
		PTG_MainSO_tmp.boundsH_RelToTer = true;
		PTG_MainSO_tmp.boundsH_RefTerOff = false;
		PTG_MainSO_tmp.boundsCeil = false;
		if(mClamp(%enabInfTer,0,1))
			PTG_MainSO_tmp.boundsStatic = true;
		else
			PTG_MainSO_tmp.boundsStatic = false;
		PTG_MainSO_tmp.boundsInvisStatic = false;
	}
	
	
	//////////////////////////////////////////////////
	//Build Loading

	
	PTG_MainSO_tmp.enabBuildLoad = false;
	PTG_MainSO_tmp.allowDetFlatAreas = false;
	PTG_MainSO_tmp.BldLdUseMaxHTer = false;
	PTG_MainSO_tmp.flatAreaFreq = 100;
	PTG_MainSO_tmp.allowFlatAreas = false;
	PTG_MainSO_tmp.BLfaGridSizeSmall = 2;
	PTG_MainSO_tmp.BLfaGridSizeLarge = 256;


	//////////////////////////////////////////////////
	//Default & Custom Biomes
	

	for(%c = 0; %c < 4; %c++)
	{
		%relBioType = getWord(%bioDefType SPC %bioCustAType SPC %bioCustBType SPC %bioCustCType,%c);
		%bioRelEnab = false;
		
		switch(%c)
		{
			case 0:

				%bioRelEnab = true;
				%bioN = "Def";
				PTG_MainSO_tmp.ter_itrB_Z = getWord("VOID 1 1 1 1 1 1 0.25 1 1 0.25 4 0.5 0.5 0.75 0.75 0.5",mClamp(%relBioType,1,16));
				
				PTG_BiomesSO_tmp.Bio_Def_Wat_Pri = PTG_LSP_ConvertPrint(getField("ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/Port_of_Taganrog" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/lava5" TAB "ModTer/water3" TAB "ModTer/Port_of_Taganrog" TAB "ModTer/water3",mClamp(%relBioType,1,16)));
				PTG_BiomesSO_tmp.Bio_Def_Wat_Col = PTG_FindClosestColor(getField("VOID" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.00000 0.00000 1.000000 0.694118" TAB "0.529412 0.803922 0.976471 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.529412 0.803922 0.976471 0.694118" TAB "0.00000 0.00000 1.000000 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.258824 0.247059 0.239216 0.694118" TAB "1.000000 0.137255 0.000000 0.694118" TAB "0.258824 0.247059 0.239216 0.694118" TAB "0.00000 0.00000 1.000000 0.694118" TAB "1.000000 1.000000 0.000000 0.694118",mClamp(%relBioType,1,16)),"RGBA-RGBAarr");
				PTG_BiomesSO_tmp.Bio_Def_Wat_Type = getWord("VOID Normal Normal Normal Normal Normal Normal Normal Normal Normal Ice Normal QuickSand Lava QuickSand Ice Normal",mClamp(%relBioType,1,16));
				
				switch(%relBioType)
				{
					case 11: //Extreme Hills
						PTG_MainSO_tmp.ter_itrA_Z = 3.0;
						PTG_MainSO_tmp.ter_itrB_Z = 2.0;
						%modItrA = true;
					case 9: //Desert
						PTG_MainSO_tmp.dirtSameTer = true;
						PTG_MainSO_tmp.ter_itrA_Z = 1.0;
						%modItrA = true;
					case 8: //Rockies
						PTG_MainSO_tmp.dirtSameTer = true;
						PTG_MainSO_tmp.ter_itrA_Z = 5.0;
						%modItrA = true;
					case 7 or 10: //Plains or Arctic Plains
						PTG_MainSO_tmp.ter_itrA_Z = 1.0;
						%modItrA = true;
				}
				
			case 1:
			
				if(%bioCustAType > 0)
				{
					PTG_MainSO_tmp.enabBio_CustA = true;
					%bioRelEnab = true;
					%bioN = "CustA";
					
					PTG_BiomesSO_tmp.Bio_CustA_TerPri = PTG_LSP_ConvertPrint(getField("ModTer/Grass" TAB "ModTer/Grass" TAB "ModTer/Grass" TAB "ModTer/Grass" TAB "ModTer/snow" TAB "ModTer/Grass" TAB "ModTer/Grass" TAB "ModTer/rock" TAB "ModTer/whitesand" TAB "ModTer/snow" TAB "ModTer/Grass" TAB "ModTer/Grass" TAB "ModTer/rock" TAB "ModTer/TTdirt01" TAB "ModTer/snow" TAB "ModTer/Grass_03",mClamp(%relBioType,1,16)));
					PTG_BiomesSO_tmp.Bio_CustA_TerCol = PTG_FindClosestColor(getField("VOID" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.976471 0.976471 0.976471 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.258824 0.247059 0.239216 1.000000" TAB "0.741176 0.584314 0.356863 1.000000" TAB "0.976471 0.976471 0.976471 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.431373 0.541176 0.239216 1.000000" TAB "0.411765 0.392157 0.376471 1.000000" TAB "0.454902 0.313726 0.109804 1.000000" TAB "0.831373 0.800000 0.768627 1.000000" TAB "0.294118 0.000000 0.509804 1.000000",mClamp(%relBioType,1,16)),"RGBA-RGBAarr");
					
					PTG_BiomesSO_tmp.Bio_CustA_Wat_Pri = PTG_LSP_ConvertPrint(getField("ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/Port_of_Taganrog" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/lava5" TAB "ModTer/water3" TAB "ModTer/Port_of_Taganrog" TAB "ModTer/water3",mClamp(%relBioType,1,16)));
					PTG_BiomesSO_tmp.Bio_CustA_Wat_Col = PTG_FindClosestColor(getField("VOID" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.00000 0.00000 1.000000 0.694118" TAB "0.529412 0.803922 0.976471 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.529412 0.803922 0.976471 0.694118" TAB "0.00000 0.00000 1.000000 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.258824 0.247059 0.239216 0.694118" TAB "1.000000 0.137255 0.000000 0.694118" TAB "0.258824 0.247059 0.239216 0.694118" TAB "0.00000 0.00000 1.000000 0.694118" TAB "1.000000 1.000000 0.000000 0.694118",mClamp(%relBioType,1,16)),"RGBA-RGBAarr");
					PTG_BiomesSO_tmp.Bio_CustA_Wat_Type = getWord("VOID Normal Normal Normal Normal Normal Normal Normal Normal Normal Ice Normal QuickSand Lava QuickSand Ice Normal",mClamp(%relBioType,1,16));
					PTG_BiomesSO_tmp.Bio_CustA_TerHMod = getWord("VOID 1 1 1 1 1 1 0.25 3 1 0.25 4 0.5 0.5 0.75 0.75 0.5",mClamp(%relBioType,1,16));
				
					if(%relBioType == 8)
					{
						PTG_MainSO_tmp.dirtSameTer = true;
						PTG_MainSO_tmp.ter_itrA_Z = 5.0;
						%modItrA = true;
					}
				}
				
			case 2:
			
				if(%bioCustBType > 0)
				{
					PTG_MainSO_tmp.enabBio_CustB = true;
					%bioRelEnab = true;
					%bioN = "CustB";
					
					PTG_BiomesSO_tmp.Bio_CustB_TerPri = PTG_LSP_ConvertPrint(getField("ModTer/Grass" TAB "ModTer/Grass" TAB "ModTer/Grass" TAB "ModTer/Grass" TAB "ModTer/snow" TAB "ModTer/Grass" TAB "ModTer/Grass" TAB "ModTer/rock" TAB "ModTer/whitesand" TAB "ModTer/snow" TAB "ModTer/Grass" TAB "ModTer/Grass" TAB "ModTer/rock" TAB "ModTer/TTdirt01" TAB "ModTer/snow" TAB "ModTer/Grass_03",mClamp(%relBioType,1,16)));
					PTG_BiomesSO_tmp.Bio_CustB_TerCol = PTG_FindClosestColor(getField("VOID" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.976471 0.976471 0.976471 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.258824 0.247059 0.239216 1.000000" TAB "0.741176 0.584314 0.356863 1.000000" TAB "0.976471 0.976471 0.976471 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.431373 0.541176 0.239216 1.000000" TAB "0.411765 0.392157 0.376471 1.000000" TAB "0.454902 0.313726 0.109804 1.000000" TAB "0.831373 0.800000 0.768627 1.000000" TAB "0.294118 0.000000 0.509804 1.000000",mClamp(%relBioType,1,16)),"RGBA-RGBAarr");
					
					PTG_BiomesSO_tmp.Bio_CustB_Wat_Pri = PTG_LSP_ConvertPrint(getField("ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/Port_of_Taganrog" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/lava5" TAB "ModTer/water3" TAB "ModTer/Port_of_Taganrog" TAB "ModTer/water3",mClamp(%relBioType,1,16)));
					PTG_BiomesSO_tmp.Bio_CustB_Wat_Col = PTG_FindClosestColor(getField("VOID" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.00000 0.00000 1.000000 0.694118" TAB "0.529412 0.803922 0.976471 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.529412 0.803922 0.976471 0.694118" TAB "0.00000 0.00000 1.000000 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.258824 0.247059 0.239216 0.694118" TAB "1.000000 0.137255 0.000000 0.694118" TAB "0.258824 0.247059 0.239216 0.694118" TAB "0.00000 0.00000 1.000000 0.694118" TAB "1.000000 1.000000 0.000000 0.694118",mClamp(%relBioType,1,16)),"RGBA-RGBAarr");
					PTG_BiomesSO_tmp.Bio_CustB_Wat_Type = getWord("VOID Normal Normal Normal Normal Normal Normal Normal Normal Normal Ice Normal QuickSand Lava QuickSand Ice Normal",mClamp(%relBioType,1,16));
					PTG_BiomesSO_tmp.Bio_CustB_TerHMod = getWord("VOID 1 1 1 1 1 1 0.25 3 1 0.25 4 0.5 0.5 0.75 0.75 0.5",mClamp(%relBioType,1,16));
				
					if(%relBioType == 8)
					{
						PTG_MainSO_tmp.dirtSameTer = true;
						PTG_MainSO_tmp.ter_itrA_Z = 5.0;
						%modItrA = true;
					}
				}
				
			case 3:
			
				if(%bioCustCType > 0)
				{
					PTG_MainSO_tmp.enabBio_CustC = true;
					%bioRelEnab = true;
					%bioN = "CustC";
					
					PTG_BiomesSO_tmp.Bio_CustC_TerPri = PTG_LSP_ConvertPrint(getField("ModTer/Grass" TAB "ModTer/Grass" TAB "ModTer/Grass" TAB "ModTer/Grass" TAB "ModTer/snow" TAB "ModTer/Grass" TAB "ModTer/Grass" TAB "ModTer/rock" TAB "ModTer/whitesand" TAB "ModTer/snow" TAB "ModTer/Grass" TAB "ModTer/Grass" TAB "ModTer/rock" TAB "ModTer/TTdirt01" TAB "ModTer/snow" TAB "ModTer/Grass_03",mClamp(%relBioType,1,16)));
					PTG_BiomesSO_tmp.Bio_CustC_TerCol = PTG_FindClosestColor(getField("VOID" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.976471 0.976471 0.976471 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.258824 0.247059 0.239216 1.000000" TAB "0.741176 0.584314 0.356863 1.000000" TAB "0.976471 0.976471 0.976471 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.431373 0.541176 0.239216 1.000000" TAB "0.411765 0.392157 0.376471 1.000000" TAB "0.454902 0.313726 0.109804 1.000000" TAB "0.831373 0.800000 0.768627 1.000000" TAB "0.294118 0.000000 0.509804 1.000000",mClamp(%relBioType,1,16)),"RGBA-RGBAarr");
					
					PTG_BiomesSO_tmp.Bio_CustC_Wat_Pri = PTG_LSP_ConvertPrint(getField("ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/Port_of_Taganrog" TAB "ModTer/water3" TAB "ModTer/water3" TAB "ModTer/lava5" TAB "ModTer/water3" TAB "ModTer/Port_of_Taganrog" TAB "ModTer/water3",mClamp(%relBioType,1,16)));
					PTG_BiomesSO_tmp.Bio_CustC_Wat_Col = PTG_FindClosestColor(getField("VOID" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.00000 0.00000 1.000000 0.694118" TAB "0.529412 0.803922 0.976471 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.529412 0.803922 0.976471 0.694118" TAB "0.00000 0.00000 1.000000 0.694118" TAB "0.113725 0.564706 1.000000 0.694118" TAB "0.258824 0.247059 0.239216 0.694118" TAB "1.000000 0.137255 0.000000 0.694118" TAB "0.258824 0.247059 0.239216 0.694118" TAB "0.00000 0.00000 1.000000 0.694118" TAB "1.000000 1.000000 0.000000 0.694118",mClamp(%relBioType,1,16)),"RGBA-RGBAarr");
					PTG_BiomesSO_tmp.Bio_CustC_Wat_Type = getWord("VOID Normal Normal Normal Normal Normal Normal Normal Normal Normal Ice Normal QuickSand Lava QuickSand Ice Normal",mClamp(%relBioType,1,16));
					PTG_BiomesSO_tmp.Bio_CustC_TerHMod = getWord("VOID 1 1 1 1 1 1 0.25 3 1 0.25 4 0.5 0.5 0.75 0.75 0.5",mClamp(%relBioType,1,16));
				
					if(%relBioType == 8)
					{
						PTG_MainSO_tmp.dirtSameTer = true;
						PTG_MainSO_tmp.ter_itrA_Z = 5.0;
						%modItrA = true;
					}
				}
		}


		if(%bioRelEnab)
		{
			//					  				Forest					Flower Forest			Plant Forest			Autumn Forest			Boreal Forest			Tropics					Plains					Rockies					Desert					Arctic Plains			Extreme Hills			Swamp					Volcanic				Wasteland				Frozen Wasteland		Other World
			%detA = getWord(stripChars("VOID 	brickPineTreeData 		brick1x1FlowersData 	brickPineTreeData 		brickPineTreeData 		brick1x1FlowersData 	- 						brick1x1FlowersData 	brick1x1Data 			- 						brick2x2petalsData 		- 						- 						- 						- 						- 						- 						","\t"),mClamp(%relBioType,1,16));
			%detB = getWord(stripChars("VOID 	- 						brick2x2flowerData 		brickBushData 			brick1x1FlowersData 	brick1x1FlowersData 	brickBushData 			brick1x1FlowersData 	brick1x1fData 			- 						brick1x1FlowersData 	- 						- 						- 						brickTree1Data 			- 						- 						","\t"),mClamp(%relBioType,1,16));
			%detC = getWord(stripChars("VOID 	brickBushData 			- 						brick1x1StemData 		- 						- 						- 						brick2x2petalsData 		brick2x2Data 			- 						brick2x2flowerData 		brick1x1FlowersData 	- 						- 						- 						- 						- 						","\t"),mClamp(%relBioType,1,16));
			%detD = getWord(stripChars("VOID 	brick1x1FlowersData 	brick2x2flowerData 		- 						- 						brick1x1FlowersData 	- 						brick2x2flowerData 		brick2x2fData 			- 						brick1x1FlowersData 	brickBushData 			- 						- 						brickBushData 			- 						- 						","\t"),mClamp(%relBioType,1,16));
			%detE = getWord(stripChars("VOID 	brick2x2flowerData 		- 						brickPineTreeData 		- 						brickTreeTop12Data 		brickBushData 			brick2x2petalsData 		brick1x2Data 			- 						brick2x2flowerData 		- 						brick1x1StemData 		brick1x1StemData 		brick1x1StemData 		brickTree1Data 			brickPineTreeData 		","\t"),mClamp(%relBioType,1,16));
			%detF = getWord(stripChars("VOID 	brick2x2petalsData 		brick2x2petalsData 		- 						- 						- 						brickBushData 			brick2x2flowerData 		brick1x2fData 			- 						brick2x2petalsData 		brick1x1FlowersData 	brickBushData 			- 						- 						- 						- 						","\t"),mClamp(%relBioType,1,16));
			
			%detG = getWord(stripChars("VOID 	brickTreeTop11Data 		brickTreeTop17Data 		brickBushData 			brickTreeTop14Data 		TreePineBrickData 		TreePalmBrickData 		brick1x1FlowersData 	brick1x2RampData 		- 						- 						brickPineTreeData 		- 						- 						- 						- 						brickTreeTop14FatData 	","\t"),mClamp(%relBioType,1,16));
			%detH = getWord(stripChars("VOID 	brickTreeTop13bData 	brick2x2flowerData 		brick2x2Data 			brickPineTreeData 		- 						- 						- 						brick2x2RampData 		- 						brick1x1FlowersData 	- 						- 						brickTree1Data 			brickTree2Data 			- 						brickTreeTop15Data 		","\t"),mClamp(%relBioType,1,16));
			%detI = getWord(stripChars("VOID 	brickTreeTop14Data 		brick2x2petalsData 		brick1x1StemData 		brickTreeTop11Data 		brickPineTreeData 		brickBushData 			- 						brick2x2RampCornerData 	- 						brickPineTreeData 		brick1x1FlowersData 	- 						- 						- 						- 						brick1x1StemData 		","\t"),mClamp(%relBioType,1,16));
			%detJ = getWord(stripChars("VOID 	brickTreeTop14FatData 	brick1x1FlowersData 	brickBushData 			brickTreeTop14FatData 	brickTreeTop12Data 		brick1x2fData 			brickPineTreeData 		brick1x1Data 			- 						- 						- 						brickseagrassData 		- 						- 						- 						brickPineTreeData 		","\t"),mClamp(%relBioType,1,16));
			%detK = getWord(stripChars("VOID 	brickTreeTop15Data 		brick1x1FlowersData 	brickBushData 			brickTreeTop13bData 	brick1x1FlowersData 	TreePalmBrickData 		brick1x1FlowersData 	brick1x2Data 			brickBushData 			- 						brickBushData 			- 						brickTree2Data 			- 						brickTree2Data 			brickTreeTop13bData 	","\t"),mClamp(%relBioType,1,16));
			%detL = getWord(stripChars("VOID 	brickTreeTop16Data 		brick2x2petalsData 		brickPineTreeData 		brickTreeTop15Data 		TreePineBrickData 		brickTreeTop17Data 		- 						- 						- 						- 						brickPineTreeData 		brickTreeTop17Data 		- 						- 						- 						brick1x1Data 			","\t"),mClamp(%relBioType,1,16));
			
			%detM = getWord(stripChars("VOID 	brickTreeTop17Data 		brick2x2petalsData 		brickTreeTop16Data 		- 						TreePineBrickData 		brick2x3Data 			- 						brick1x2RampData 		TreePalmBrickData 		- 						- 						- 						- 						- 						- 						- 						","\t"),mClamp(%relBioType,1,16));
			%detN = getWord(stripChars("VOID 	brickPineTreeData 		brick2x2flowerData 		brick1x1Data 			brickTreeTop16Data 		brickPineTreeData 		TreePalmBrickData 		- 						- 						brickBushData 			- 						brickBushData 			brickTreeTop11Data 		- 						brickTree3Data 			- 						- 						","\t"),mClamp(%relBioType,1,16));
			%detO = getWord(stripChars("VOID 	brick2x2petalsData 		brickTreeTop15Data 		brick1x1StemData 		brickPineTreeData 		brickTree2Data 			TreePalmLargeBrickData 	- 						brick2x2RampData 		TreePalmLargeBrickData 	brickTree3Data 			- 						brickTree1Data 			brick1x1StemData 		- 						- 						brick1x1StemData 		","\t"),mClamp(%relBioType,1,16));
			%detP = getWord(stripChars("VOID 	brick2x2flowerData 		brickTreeTop16Data 		brickPineTreeData 		brickTreeTop17Data 		- 						brickTreeTop17Data 		- 						- 						brick1x1StemData 		brickTree2Data 			- 						brickTreeTop13aData 	- 						- 						brickTree3Data 			brick1x1Data 			","\t"),mClamp(%relBioType,1,16));
			%detQ = getWord(stripChars("VOID 	- 						brickTreeTop14FatData 	brickTreeTop15Data 		TreeStumpBrickData 		- 						brickBushData 			- 						brick2x2RampCornerData 	- 						- 						brickPineTreeData 		brickTree3Data 			- 						brickBushData 			brick1x1StemData 		- 						","\t"),mClamp(%relBioType,1,16));
			%detR = getWord(stripChars("VOID 	TreeStumpBrickData 		brick2x2flowerData 		brickTreeTop14Data 		- 						brickTree3Data 			brick4xCubeData 		brickPineTreeData 		brick1x3fData 			brickBushData 			- 						brickPineTreeData 		- 						- 						- 						- 						brickPineTreeData 		","\t"),mClamp(%relBioType,1,16));
			
			
			//							Forest									  Flower Forest								Plant Forest							  Autumn Forest								Boreal Forest							  Tropics									Plains									  Rockies									Desert									  Arctic Plains								Extreme Hills							  Swamp										Volcanic								  Wasteland									Frozen Wasteland						  Other World
			%colA = getField("VOID" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.631373 0.341176 0.000000 1.000000" TAB "0.831373 0.800000 0.768627 1.000000" TAB "-------- -------- -------- --------" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.529412 0.513726 0.494118 1.000000" TAB "-------- -------- -------- --------" TAB "0.000000 0.000000 1.000000 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------",mClamp(%relBioType,1,16));
			%colB = getField("VOID" TAB "-------- -------- -------- --------" TAB "0.501961 0.000000 0.501961 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.831373 0.800000 0.768627 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.654902 0.639216 0.627451 1.000000" TAB "-------- -------- -------- --------" TAB "0.976471 0.976471 0.976471 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "0.529412 0.392157 0.192157 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------",mClamp(%relBioType,1,16));
			%colC = getField("VOID" TAB "0.000000 0.501961 0.231373 1.000000" TAB "-------- -------- -------- --------" TAB "0.000000 0.501961 0.231373 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "0.101961 0.458824 0.764706 1.000000" TAB "0.529412 0.513726 0.494118 1.000000" TAB "-------- -------- -------- --------" TAB "0.239216 0.349020 0.666667 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------",mClamp(%relBioType,1,16));
			%colD = getField("VOID" TAB "0.000000 0.501961 0.231373 1.000000" TAB "1.000000 0.494118 0.000000 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "0.831373 0.800000 0.768627 1.000000" TAB "-------- -------- -------- --------" TAB "0.501961 0.000000 0.501961 1.000000" TAB "0.411765 0.392157 0.376471 1.000000" TAB "-------- -------- -------- --------" TAB "0.976471 0.976471 0.976471 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "0.541176 0.486275 0.419608 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------",mClamp(%relBioType,1,16));
			%colE = getField("VOID" TAB "0.501961 0.000000 0.501961 1.000000" TAB "-------- -------- -------- --------" TAB "0.000000 0.501961 0.231373 1.000000" TAB "-------- -------- -------- --------" TAB "0.976471 0.976471 0.976471 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.239216 0.349020 0.666667 1.000000" TAB "0.654902 0.639216 0.627451 1.000000" TAB "-------- -------- -------- --------" TAB "0.101961 0.458824 0.764706 1.000000" TAB "-------- -------- -------- --------" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.529412 0.392157 0.192157 1.000000" TAB "0.854902 0.647059 0.121569 1.000000" TAB "0.541176 0.349020 0.000000 1.000000" TAB "1.000000 0.494118 0.000000 1.000000",mClamp(%relBioType,1,16));
			%colF = getField("VOID" TAB "0.101961 0.458824 0.764706 1.000000" TAB "0.101961 0.458824 0.764706 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.854902 0.647059 0.121569 1.000000" TAB "0.411765 0.392157 0.376471 1.000000" TAB "-------- -------- -------- --------" TAB "0.000000 0.494118 1.000000 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.431373 0.541176 0.239216 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------",mClamp(%relBioType,1,16));
			
			%colG = getField("VOID" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.129412 0.266667 0.266667 1.000000" TAB "0.701961 0.129412 0.129412 1.000000" TAB "0.976471 0.976471 0.976471 1.000000" TAB "0.000000 0.647059 0.000000 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.529412 0.513726 0.494118 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "0.000000 0.501961 0.231373 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "1.000000 0.494118 0.000000 0.694118",mClamp(%relBioType,1,16));
			%colH = getField("VOID" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.000000 1.000000 1.000000" TAB "0.529412 0.513726 0.494118 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "0.654902 0.639216 0.627451 1.000000" TAB "-------- -------- -------- --------" TAB "0.976471 0.976471 0.976471 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "0.454902 0.313726 0.109804 1.000000" TAB "0.454902 0.313726 0.109804 1.000000" TAB "-------- -------- -------- --------" TAB "1.000000 1.000000 0.000000 0.694118",mClamp(%relBioType,1,16));
			%colI = getField("VOID" TAB "0.000000 0.501961 0.231373 1.000000" TAB "1.000000 1.000000 0.000000 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.129412 0.266667 0.266667 1.000000" TAB "0.729412 0.721569 0.701961 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "-------- -------- -------- --------" TAB "0.258824 0.247059 0.239216 1.000000" TAB "-------- -------- -------- --------" TAB "0.729412 0.721569 0.701961 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "0.631373 0.341176 0.000000 1.000000",mClamp(%relBioType,1,16));
			%colJ = getField("VOID" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.431373 0.541176 0.239216 1.000000" TAB "0.631373 0.341176 0.000000 1.000000" TAB "0.976471 0.976471 0.976471 1.000000" TAB "0.529412 0.513726 0.494118 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.411765 0.392157 0.376471 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "0.431373 0.541176 0.239216 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "0.631373 0.341176 0.000000 1.000000",mClamp(%relBioType,1,16));
			%colK = getField("VOID" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.631373 0.631373 0.000000 1.000000" TAB "0.831373 0.800000 0.768627 1.000000" TAB "0.000000 0.647059 0.000000 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.258824 0.247059 0.239216 1.000000" TAB "0.741176 0.584314 0.356863 1.000000" TAB "-------- -------- -------- --------" TAB "0.000000 0.501961 0.231373 1.000000" TAB "-------- -------- -------- --------" TAB "0.313726 0.239216 0.113725 1.000000" TAB "-------- -------- -------- --------" TAB "0.529412 0.392157 0.192157 1.000000" TAB "1.000000 0.494118 0.000000 0.694118",mClamp(%relBioType,1,16));
			%colL = getField("VOID" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.000000 0.631373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.129412 0.266667 0.266667 1.000000" TAB "0.976471 0.976471 0.976471 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "-------- -------- -------- --------" TAB "0.411765 0.392157 0.376471 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "0.631373 0.631373 0.000000 1.000000",mClamp(%relBioType,1,16));
			
			%colM = getField("VOID" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.239216 0.349020 0.666667 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "-------- -------- -------- --------" TAB "0.976471 0.976471 0.976471 1.000000" TAB "0.529412 0.513726 0.494118 1.000000" TAB "-------- -------- -------- --------" TAB "0.529412 0.513726 0.494118 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------",mClamp(%relBioType,1,16));
			%colN = getField("VOID" TAB "0.431373 0.541176 0.239216 1.000000" TAB "0.000000 0.494118 1.000000 1.000000" TAB "0.654902 0.639216 0.627451 1.000000" TAB "0.784314 0.113725 0.000000 1.000000" TAB "0.729412 0.721569 0.701961 1.000000" TAB "0.000000 0.647059 0.000000 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "0.741176 0.584314 0.356863 1.000000" TAB "-------- -------- -------- --------" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.431373 0.541176 0.239216 1.000000" TAB "-------- -------- -------- --------" TAB "0.313726 0.239216 0.113725 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------",mClamp(%relBioType,1,16));
			%colO = getField("VOID" TAB "0.000000 0.000000 0.631373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.854902 0.647059 0.121569 1.000000" TAB "0.454902 0.313726 0.109804 1.000000" TAB "0.000000 0.647059 0.000000 1.000000" TAB "-------- -------- -------- --------" TAB "0.654902 0.639216 0.627451 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.529412 0.392157 0.192157 1.000000" TAB "-------- -------- -------- --------" TAB "0.454902 0.313726 0.109804 1.000000" TAB "0.529412 0.392157 0.192157 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "0.631373 0.631373 0.000000 1.000000",mClamp(%relBioType,1,16));
			%colP = getField("VOID" TAB "0.294118 0.000000 0.509804 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "1.000000 1.000000 0.000000 1.000000" TAB "-------- -------- -------- --------" TAB "0.000000 0.501961 0.231373 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.454902 0.313726 0.109804 1.000000" TAB "-------- -------- -------- --------" TAB "0.000000 0.501961 0.231373 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "0.454902 0.313726 0.109804 1.000000" TAB "0.631373 0.341176 0.000000 1.000000",mClamp(%relBioType,1,16));
			%colQ = getField("VOID" TAB "-------- -------- -------- --------" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.392157 0.192157 0.000000 1.000000" TAB "-------- -------- -------- --------" TAB "0.000000 0.501961 0.231373 1.000000" TAB "-------- -------- -------- --------" TAB "0.529412 0.513726 0.494118 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.313726 0.239216 0.113725 1.000000" TAB "-------- -------- -------- --------" TAB "0.541176 0.486275 0.419608 1.000000" TAB "0.313726 0.239216 0.113725 1.000000" TAB "-------- -------- -------- --------",mClamp(%relBioType,1,16));
			%colR = getField("VOID" TAB "0.313726 0.239216 0.113725 1.000000" TAB "0.294118 0.000000 0.509804 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "-------- -------- -------- --------" TAB "0.313726 0.239216 0.113725 1.000000" TAB "0.411765 0.392157 0.376471 1.000000" TAB "0.000000 0.501961 0.231373 1.000000" TAB "0.529412 0.513726 0.494118 1.000000" TAB "0.741176 0.584314 0.356863 1.000000" TAB "-------- -------- -------- --------" TAB "0.000000 0.501961 0.231373 1.000000" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "-------- -------- -------- --------" TAB "1.000000 1.000000 0.000000 0.694118",mClamp(%relBioType,1,16));

			//negate print data for biome details
		
			for(%d = 0; %d < 18; %d++)
			{
				eval("%relDet = %det" @ getSubStr("ABCDEFGHIJKLMNOPQR",%d,1) @ ";");
				eval("%relCol = %col" @ getSubStr("ABCDEFGHIJKLMNOPQR",%d,1) @ ";");
				
				if(isObject(%relDet))
				{
					eval("PTG_BiomesSO_tmp.Bio_" @ %bioN @ "_Det" @ %d @ "_BrDB = %relDet;");
					//no print data
					
					if((%relCol = stripChars(%relCol,"-")) $= "   ")
						eval("PTG_BiomesSO_tmp.Bio_" @ %bioN @ "_Det" @ %d @ "_Col = 0;");
					else
						eval("PTG_BiomesSO_tmp.Bio_" @ %bioN @ "_Det" @ %d @ "_Col = PTG_FindClosestColor(%relCol,\"RGBA-RGBAarr\");");
				}
			}
		}
	}
	
	
	//////////////////////////////////////////////////
	//Shore Biome
	
	
	PTG_BiomesSO_tmp.Bio_Shore_TerPri = PTG_LSP_ConvertPrint("ModTer/sand03");
	PTG_BiomesSO_tmp.Bio_Shore_TerCol = PTG_FindClosestColor("0.803922 0.666667 0.486275 1.000000","RGBA-RGBAarr");
	
	PTG_BiomesSO_tmp.Bio_Shore_Det0_BrDB = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det0_Col = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det1_BrDB = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det1_Col = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det2_BrDB = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det2_Col = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det3_BrDB = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det3_Col = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det4_BrDB = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det4_Col = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det5_BrDB = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det5_Col = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det6_BrDB = "brickBushData";
	PTG_BiomesSO_tmp.Bio_Shore_Det6_Col = PTG_FindClosestColor("1.000000 0.901961 0.729412 1.000000","RGBA-RGBAarr");
	PTG_BiomesSO_tmp.Bio_Shore_Det7_BrDB = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det7_Col = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det8_BrDB = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det8_Col = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det9_BrDB = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det9_Col = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det10_BrDB = "TreePalmBrickData";
	PTG_BiomesSO_tmp.Bio_Shore_Det10_Col = PTG_FindClosestColor("0.000000 0.501961 0.231373 1.000000","RGBA-RGBAarr");
	PTG_BiomesSO_tmp.Bio_Shore_Det11_BrDB = "brick1x1fData";
	PTG_BiomesSO_tmp.Bio_Shore_Det11_Col = PTG_FindClosestColor("0.101961 0.458824 0.764706 1.000000","RGBA-RGBAarr");
	PTG_BiomesSO_tmp.Bio_Shore_Det12_BrDB = "brick1x2fData";
	PTG_BiomesSO_tmp.Bio_Shore_Det12_Col = PTG_FindClosestColor("0.501961 0.000000 0.000000 1.000000","RGBA-RGBAarr");
	PTG_BiomesSO_tmp.Bio_Shore_Det13_BrDB = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det13_Col = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det14_BrDB = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det14_Col = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det15_BrDB = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det15_Col = "";
	PTG_BiomesSO_tmp.Bio_Shore_Det16_BrDB = "TreePalmLargeBrickData";
	PTG_BiomesSO_tmp.Bio_Shore_Det16_Col = PTG_FindClosestColor("0.000000 0.501961 0.231373 1.000000","RGBA-RGBAarr");
	PTG_BiomesSO_tmp.Bio_Shore_Det17_BrDB = "brick1x1fData";
	PTG_BiomesSO_tmp.Bio_Shore_Det17_Col = PTG_FindClosestColor("0.976471 0.976471 0.976471 1.000000","RGBA-RGBAarr");
	
	
	//////////////////////////////////////////////////
	//Submarine Biome
	
	
	PTG_BiomesSO_tmp.Bio_SubM_TerPri = PTG_LSP_ConvertPrint("ModTer/sand2");
	PTG_BiomesSO_tmp.Bio_SubM_TerCol = PTG_FindClosestColor("0.741176 0.584314 0.356863 1.000000","RGBA-RGBAarr");
	
	PTG_BiomesSO_tmp.Bio_SubM_Det0_BrDB = "brickseagrassData";
	PTG_BiomesSO_tmp.Bio_SubM_Det0_Col = PTG_FindClosestColor("0.000000 0.501961 0.231373 1.000000","RGBA-RGBAarr");
	PTG_BiomesSO_tmp.Bio_SubM_Det1_BrDB = "";
	PTG_BiomesSO_tmp.Bio_SubM_Det1_Col = "";
	PTG_BiomesSO_tmp.Bio_SubM_Det2_BrDB = "";
	PTG_BiomesSO_tmp.Bio_SubM_Det2_Col = "";
	PTG_BiomesSO_tmp.Bio_SubM_Det3_BrDB = "";
	PTG_BiomesSO_tmp.Bio_SubM_Det3_Col = "";
	PTG_BiomesSO_tmp.Bio_SubM_Det4_BrDB = "";
	PTG_BiomesSO_tmp.Bio_SubM_Det4_Col = "";
	PTG_BiomesSO_tmp.Bio_SubM_Det5_BrDB = "brick1x1fData";
	PTG_BiomesSO_tmp.Bio_SubM_Det5_Col = PTG_FindClosestColor("0.729412 0.721569 0.701961 1.000000","RGBA-RGBAarr");
	PTG_BiomesSO_tmp.Bio_SubM_Det6_BrDB = "brickseagrassData";
	PTG_BiomesSO_tmp.Bio_SubM_Det6_Col = PTG_FindClosestColor("1.000000 0.494118 0.000000 1.000000","RGBA-RGBAarr");
	PTG_BiomesSO_tmp.Bio_SubM_Det7_BrDB = "";
	PTG_BiomesSO_tmp.Bio_SubM_Det7_Col = "";
	PTG_BiomesSO_tmp.Bio_SubM_Det8_BrDB = "";
	PTG_BiomesSO_tmp.Bio_SubM_Det8_Col = "";
	PTG_BiomesSO_tmp.Bio_SubM_Det9_BrDB = "brickseagrassData";
	PTG_BiomesSO_tmp.Bio_SubM_Det9_Col = PTG_FindClosestColor("0.000000 0.501961 0.231373 1.000000","RGBA-RGBAarr");
	PTG_BiomesSO_tmp.Bio_SubM_Det10_BrDB = "brick1x2fData";
	PTG_BiomesSO_tmp.Bio_SubM_Det10_Col = PTG_FindClosestColor("0.529412 0.513726 0.494118 1.000000","RGBA-RGBAarr");
	PTG_BiomesSO_tmp.Bio_SubM_Det11_BrDB = "brickseagrassData";
	PTG_BiomesSO_tmp.Bio_SubM_Det11_Col = PTG_FindClosestColor("0.101961 0.458824 0.764706 1.000000","RGBA-RGBAarr");
	PTG_BiomesSO_tmp.Bio_SubM_Det12_BrDB = "brickseagrassData";
	PTG_BiomesSO_tmp.Bio_SubM_Det12_Col = PTG_FindClosestColor("0.258824 0.247059 0.239216 1.000000","RGBA-RGBAarr");
	PTG_BiomesSO_tmp.Bio_SubM_Det13_BrDB = "brick4xCubeData";
	PTG_BiomesSO_tmp.Bio_SubM_Det13_Col = PTG_FindClosestColor("0.529412 0.513726 0.494118 1.000000","RGBA-RGBAarr");
	PTG_BiomesSO_tmp.Bio_SubM_Det14_BrDB = "brick1x1fData";
	PTG_BiomesSO_tmp.Bio_SubM_Det14_Col = PTG_FindClosestColor("0.976471 0.976471 0.976471 1.000000","RGBA-RGBAarr");
	PTG_BiomesSO_tmp.Bio_SubM_Det15_BrDB = "brickseagrassData";
	PTG_BiomesSO_tmp.Bio_SubM_Det15_Col = PTG_FindClosestColor("0.501961 0.000000 0.501961 1.000000","RGBA-RGBAarr");
	PTG_BiomesSO_tmp.Bio_SubM_Det16_BrDB = "";
	PTG_BiomesSO_tmp.Bio_SubM_Det16_Col = "";
	PTG_BiomesSO_tmp.Bio_SubM_Det17_BrDB = "brickseagrassData";
	PTG_BiomesSO_tmp.Bio_SubM_Det17_Col = PTG_FindClosestColor("0.729412 0.721569 0.701961 1.000000","RGBA-RGBAarr");
	
	
	//////////////////////////////////////////////////
	//Cave Top Biome
	
	
	if(%enabCaves)
	{
		PTG_BiomesSO_tmp.Bio_CaveTop_TerPri = PTG_LSP_ConvertPrint("ModTer/rock");
		PTG_BiomesSO_tmp.Bio_CaveTop_TerCol = PTG_FindClosestColor("0.411765 0.392157 0.376471 1.000000","RGBA-RGBAarr");
		
		PTG_BiomesSO_tmp.Bio_CaveTop_Det0_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det0_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det1_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det1_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det2_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det2_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det3_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det3_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det4_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det4_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det5_BrDB = "brick2x2x5Data";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det5_Col = PTG_FindClosestColor("0.529412 0.513726 0.494118 1.000000","RGBA-RGBAarr");
		PTG_BiomesSO_tmp.Bio_CaveTop_Det6_BrDB = "brick2x2x3Data";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det6_Col = PTG_FindClosestColor("0.529412 0.513726 0.494118 1.000000","RGBA-RGBAarr");
		PTG_BiomesSO_tmp.Bio_CaveTop_Det7_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det7_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det8_BrDB = "brickseagrassInvData";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det8_Col = PTG_FindClosestColor("0.000000 0.501961 0.231373 1.000000","RGBA-RGBAarr");
		PTG_BiomesSO_tmp.Bio_CaveTop_Det9_BrDB = "brick2x2Data";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det9_Col = PTG_FindClosestColor("0.529412 0.513726 0.494118 1.000000","RGBA-RGBAarr");
		PTG_BiomesSO_tmp.Bio_CaveTop_Det10_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det10_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det11_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det11_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det12_BrDB = "brick1x1x5Data";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det12_Col = PTG_FindClosestColor("0.729412 0.721569 0.701961 1.000000","RGBA-RGBAarr");
		PTG_BiomesSO_tmp.Bio_CaveTop_Det13_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det13_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det14_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det14_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det15_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det15_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det16_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det16_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det17_BrDB = "brickseagrassInvData";
		PTG_BiomesSO_tmp.Bio_CaveTop_Det17_Col = PTG_FindClosestColor("0.000000 0.501961 0.231373 1.000000","RGBA-RGBAarr");
	}
	
	
	//////////////////////////////////////////////////
	//Cave Bottom Biome
	
	
	if(%enabCaves)
	{
		PTG_BiomesSO_tmp.Bio_CaveBtm_TerPri = PTG_LSP_ConvertPrint("ModTer/rock");
		PTG_BiomesSO_tmp.Bio_CaveBtm_TerCol = PTG_FindClosestColor("0.529412 0.513726 0.494118 1.000000","RGBA-RGBAarr");
		
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det0_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det0_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det1_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det1_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det2_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det2_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det3_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det3_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det4_BrDB = "brick2x2x3Data";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det4_Col = PTG_FindClosestColor("0.654902 0.639216 0.627451 1.000000","RGBA-RGBAarr");
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det5_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det5_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det6_BrDB = "brick1x1Data";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det6_Col = PTG_FindClosestColor("0.729412 0.721569 0.701961 1.000000","RGBA-RGBAarr");
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det7_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det7_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det8_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det8_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det9_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det9_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det10_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det10_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det11_BrDB = "brick2x2x5Data";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det11_Col = PTG_FindClosestColor("0.654902 0.639216 0.627451 1.000000","RGBA-RGBAarr");
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det12_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det12_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det13_BrDB = "brick1x1x5Data";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det13_Col = PTG_FindClosestColor("0.654902 0.639216 0.627451 1.000000","RGBA-RGBAarr");
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det14_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det14_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det15_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det15_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det16_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det16_Col = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det17_BrDB = "";
		PTG_BiomesSO_tmp.Bio_CaveBtm_Det17_Col = "";
	}
	
	
	//////////////////////////////////////////////////
	//Mountain Biome
	
	
	if(%enabMntns)
	{		
		PTG_BiomesSO_tmp.Bio_Mntn_RockPri = PTG_LSP_ConvertPrint("ModTer/rockface");
		PTG_BiomesSO_tmp.Bio_Mntn_RockCol = PTG_FindClosestColor("0.258824 0.247059 0.239216 1.000000","RGBA-RGBAarr");
		
		PTG_BiomesSO_tmp.Bio_Mntn_SnowPri = PTG_LSP_ConvertPrint("ModTer/snow");
		PTG_BiomesSO_tmp.Bio_Mntn_SnowCol = PTG_FindClosestColor("0.976471 0.976471 0.976471 1.000000","RGBA-RGBAarr");
		
		PTG_BiomesSO_tmp.Bio_Mntn_Det0_BrDB = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det0_Col = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det1_BrDB = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det1_Col = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det2_BrDB = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det2_Col = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det3_BrDB = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det3_Col = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det4_BrDB = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det4_Col = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det5_BrDB = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det5_Col = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det6_BrDB = "brick1x1StemData";
		PTG_BiomesSO_tmp.Bio_Mntn_Det6_Col = PTG_FindClosestColor("0.258824 0.247059 0.239216 1.000000","RGBA-RGBAarr");
		PTG_BiomesSO_tmp.Bio_Mntn_Det7_BrDB = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det7_Col = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det8_BrDB = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det8_Col = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det9_BrDB = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det9_Col = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det10_BrDB = "brickTreeTop12Data";
		PTG_BiomesSO_tmp.Bio_Mntn_Det10_Col = PTG_FindClosestColor("0.000000 0.501961 0.231373 1.000000","RGBA-RGBAarr");
		PTG_BiomesSO_tmp.Bio_Mntn_Det11_BrDB = "brick1x1fData";
		PTG_BiomesSO_tmp.Bio_Mntn_Det11_Col = PTG_FindClosestColor("0.529412 0.513726 0.494118 1.000000","RGBA-RGBAarr");
		PTG_BiomesSO_tmp.Bio_Mntn_Det12_BrDB = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det12_Col = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det13_BrDB = "TreePineBrickData";
		PTG_BiomesSO_tmp.Bio_Mntn_Det13_Col = PTG_FindClosestColor("0.000000 0.501961 0.231373 1.000000","RGBA-RGBAarr");
		PTG_BiomesSO_tmp.Bio_Mntn_Det14_BrDB = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det14_Col = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det15_BrDB = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det15_Col = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det16_BrDB = "brickPineTreeData";
		PTG_BiomesSO_tmp.Bio_Mntn_Det16_Col = PTG_FindClosestColor("0.000000 0.501961 0.231373 1.000000","RGBA-RGBAarr");
		PTG_BiomesSO_tmp.Bio_Mntn_Det17_BrDB = "";
		PTG_BiomesSO_tmp.Bio_Mntn_Det17_Col = "";
	}
	

	//////////////////////////////////////////////////
	//Advanced

	
	%ChSize = PTG_MainSO_tmp.chSize = 32; //mClamp((mFloor(getWord(%strA,0) / 16) * 16),16,256);

	if(mClamp(%enabCaves,0,1)) PTG_MainSO_tmp.caveTopZMult = 2;
	PTG_MainSO_tmp.zMod = 50;
	PTG_MainSO_tmp.cnctLakesStrt = 12;
	PTG_MainSO_tmp.TreeBaseACol = PTG_FindClosestColor("0.529412 0.392157 0.192157 1.000000","RGBA-RGBAarr");
	PTG_MainSO_tmp.TreeBaseBCol = PTG_FindClosestColor("0.454902 0.313726 0.109804 1.000000","RGBA-RGBAarr");
	PTG_MainSO_tmp.TreeBaseCCol = PTG_FindClosestColor("0.313726 0.239216 0.113725 1.000000","RGBA-RGBAarr");
	PTG_MainSO_tmp.FIFOchClr = true;
	PTG_MainSO_tmp.seamlessModTer = true;
	PTG_MainSO_tmp.seamlessBuildL = true;
	
	PTG_MainSO_tmp.cavesSecZ = 30;
	PTG_MainSO_tmp.skyLndsSecZ = 40;
	PTG_MainSO_tmp.fltIsldsSecZ = 30;
	PTG_MainSO_tmp.bio_CustASecZ = 20;
	PTG_MainSO_tmp.bio_CustBSecZ = 20;
	PTG_MainSO_tmp.bio_CustCSecZ = 20;
	PTG_MainSO_tmp.cloudsSecZ = 45;
	
	//if(!%genRockies)
		PTG_MainSO_tmp.mntnsStrtSecZ = 30; //25
	//else
	//	PTG_MainSO_tmp.mntnsStrtSecZ = 15;
	
	PTG_MainSO_tmp.terOff_X = 0;
	PTG_MainSO_tmp.terOff_y = 0;
	PTG_MainSO_tmp.mntnsOff_X = 0;
	PTG_MainSO_tmp.mntnsOff_Y = 0;
	PTG_MainSO_tmp.bio_CustAOff_X = 256;
	PTG_MainSO_tmp.bio_CustAOff_Y = 256;
	PTG_MainSO_tmp.bio_CustBOff_X = 300;
	PTG_MainSO_tmp.bio_CustBOff_Y = 0;
	PTG_MainSO_tmp.bio_CustCOff_X = 0;
	PTG_MainSO_tmp.bio_CustCOff_Y = 500;
	PTG_MainSO_tmp.caveAOff_X = 0;
	PTG_MainSO_tmp.caveAOff_Y = 0;
	PTG_MainSO_tmp.caveBOff_X = 0;
	PTG_MainSO_tmp.caveBOff_Y = 0;
	PTG_MainSO_tmp.fltIsldsOff_X = 0;
	PTG_MainSO_tmp.fltIsldsOff_Y = 0;
	PTG_MainSO_tmp.fltIsldsBOff_X = 0;
	PTG_MainSO_tmp.fltIsldsBOff_Y = 0;
	PTG_MainSO_tmp.skyLandsOff_X = 0;
	PTG_MainSO_tmp.skyLandsOff_Y = 0;
	PTG_MainSO_tmp.detailsOff_X = 0;
	PTG_MainSO_tmp.detailsOff_Y = 0;
	PTG_MainSO_tmp.cloudsOff_X = 0;
	PTG_MainSO_tmp.cloudsOff_Y = 0;
	PTG_MainSO_tmp.buildLoad_X = 0;
	PTG_MainSO_tmp.buildLoad_Y = 0;
	
	PTG_MainSO_tmp.ter_itrA_XY = 256;
	if(!%modItrA) PTG_MainSO_tmp.ter_itrA_Z = 2;
	PTG_MainSO_tmp.ter_itrB_XY = 64;
	//PTG_MainSO_tmp.ter_itrB_Z = 1;
	PTG_MainSO_tmp.ter_itrC_XY = 16;
	PTG_MainSO_tmp.ter_itrC_Z = 0.09375;
	PTG_MainSO_tmp.mntn_itrA_XY = 256;
	PTG_MainSO_tmp.mntn_itrA_Z = 1;
	PTG_MainSO_tmp.mntn_itrB_XY = 128;
	PTG_MainSO_tmp.mntn_itrB_Z = 0.5;
	PTG_MainSO_tmp.caveA_itrA_XY = 128;
	PTG_MainSO_tmp.caveA_itrA_Z = 1;
	PTG_MainSO_tmp.caveA_itrB_XY = 64;
	PTG_MainSO_tmp.caveA_itrB_Z = 1;
	PTG_MainSO_tmp.caveA_itrC_XY = 16;
	PTG_MainSO_tmp.caveA_itrC_Z = 0.25;
	PTG_MainSO_tmp.caveB_itrA_XY = 256;
	PTG_MainSO_tmp.caveB_itrA_Z = 8;
	PTG_MainSO_tmp.bio_CustA_itrA_XY = 256;
	PTG_MainSO_tmp.bio_CustA_itrA_Z = 2;
	PTG_MainSO_tmp.bio_CustB_itrA_XY = 256;
	PTG_MainSO_tmp.bio_CustB_itrA_Z = 2;
	PTG_MainSO_tmp.bio_CustC_itrA_XY = 256;
	PTG_MainSO_tmp.bio_CustC_itrA_Z = 2;
	PTG_MainSO_tmp.skyLnds_itrA_XY = 128;
	PTG_MainSO_tmp.skyLnds_itrA_Z = 4;
	PTG_MainSO_tmp.clouds_itrA_XY = 128;
	PTG_MainSO_tmp.clouds_itrA_Z = 2;
	PTG_MainSO_tmp.clouds_itrB_XY = 32;
	PTG_MainSO_tmp.clouds_itrB_Z = 1;
	PTG_MainSO_tmp.fltIslds_itrA_XY = 128;
	PTG_MainSO_tmp.fltIslds_itrA_Z = 1;
	PTG_MainSO_tmp.fltIslds_itrB_XY = 32;
	PTG_MainSO_tmp.fltIslds_itrB_Z = 0.5;
	
	PTG_MainSO_tmp.enabPseudoEqtr = false;
	PTG_MainSO_tmp.Bio_CustA_YStart = 0;
	PTG_MainSO_tmp.Bio_CustB_YStart = 0;
	PTG_MainSO_tmp.Bio_CustC_YStart = 0;
	PTG_MainSO_tmp.Cave_YStart = 0;
	PTG_MainSO_tmp.Mntn_YStart = 0;
	PTG_MainSO_tmp.Cloud_YStart = 0;
	PTG_MainSO_tmp.FltIsld_YStart = 0;
	
	
	//////////////////////////////////////////////////
	//Routines
	
	//If default routine settings not uploaded, load either custom default or standard default for server
	if(!isObject(PTG_GlobalSO))
	{
		MissionCleanup.add(new ScriptObject(PTG_GlobalSO_tmp));
		
		PTG_GlobalSO_tmp.lastSetUploadTime = %currT;
		PTG_GlobalSO_tmp.lastSetUploadPlayer = getSubStr(%cl.name,0,30);
		PTG_GlobalSO_tmp.lastSetUploadID = %cl.bl_id;
		PTG_GlobalSO_tmp.uploadingSettings = true; //for server preset loading (prevents conflicting)
		PTG_GlobalSO_tmp.lastUploadClient = ""; //don't need to set this up, only for main settings upload (referenced by third party func)
		
	
		if(!isFile(%fileFP = "config/server/PTGv3/Routines.txt")) //search for a custom routines save first
		{
			%fileFP = "Add-Ons/System_PTG/PRESETS/Routines.txt"; //use default settings if custom save wasn't found for routines
			if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0No routine settings found, setting up custom default routine settings... \c4[^] \c0->" SPC getWord(getDateTime(),1));
		}
		else if($PTG.allowEchos)
			echo("\c4>>\c2P\c1T\c4G: \c0No routine settings found, setting up standard default routine settings... \c4[^] \c0->" SPC getWord(getDateTime(),1));
		
		%file = new FileObject();
		%file.openForRead(%fileFP);
		%file.readLine();
		

		PTG_GlobalSO_tmp.routine_isHalting = false;
		PTG_GlobalSO_tmp.routine_Process = "None";
		PTG_GlobalSO_tmp.routine_ProcessAux = "None";
		PTG_GlobalSO_tmp.zMax = 4000;	//auto-calculate?
		PTG_GlobalSO_tmp.uploadingSettings = false;
		
		PTG_GlobalSO_tmp.ForceCancelBldUpld = false;
		PTG_GlobalSO_tmp.BuildUploading = false;
		PTG_GlobalSO_tmp.ListingBuild = false;
		PTG_GlobalSO_tmp.UploadBuildName = "";
		
		
		%tmpHLColA = %file.readLine();
		%tmpHLColB = %file.readLine();
		
		%file.readLine();
		
		PTG_GlobalSO_tmp.enabStreams = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.streamsTickMS = mClamp(%file.readLine(),33,2013);
		PTG_GlobalSO_tmp.StreamsHostOnly = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.solidStreams = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.streamsClrDetails = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.streamsMaxDist = mClamp(%file.readLine(),0,8);
		PTG_GlobalSO_tmp.genStreamZones = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.floatStreams = mClamp(%file.readLine(),0,1);
		
		%file.readLine();
		
		PTG_GlobalSO_tmp.brLimitBuffer = mClamp(%file.readLine(),0,20000);
		PTG_GlobalSO_tmp.pingMaxBuffer = mClamp(%file.readLine(),100,1000);
		PTG_GlobalSO_tmp.DedSrvrFuncBuffer = mClamp(%file.readLine(),20,2000);
		PTG_GlobalSO_tmp.chObjLimit = mClamp(%file.readLine(),20,4000);
		PTG_GlobalSO_tmp.chSaveLimit_FilesPerSeed = mClamp(%file.readLine(),0,100000);
		PTG_GlobalSO_tmp.chSaveLimit_TotalFiles = mClamp(%file.readLine(),0,100000);
		PTG_GlobalSO_tmp.buildLoadFileLimit = mClamp(%file.readLine(),0,400);
		PTG_GlobalSO_tmp.disBrBuffer = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.disChBuffer = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.disNormLagCheck = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.disDedLagCheck = mClamp(%file.readLine(),0,1);
		
		%file.readLine();
		
		PTG_GlobalSO_tmp.delay_PauseTickS = mClamp(%file.readLine(),1,30);
		PTG_GlobalSO_tmp.schedM_autosave = mClamp(%file.readLine(),1,60);
		PTG_GlobalSO_tmp.delay_priFuncMS = mClamp(%file.readLine(),0,100);
		PTG_GlobalSO_tmp.delay_secFuncMS = mClamp( %file.readLine(),0,100);
		PTG_GlobalSO_tmp.calcDelay_priFuncMS = mClamp(%file.readLine(),0,100);
		PTG_GlobalSO_tmp.calcDelay_secFuncMS = mClamp(%file.readLine(),0,100);
		PTG_GlobalSO_tmp.brDelay_genMS = mClamp(%file.readLine(),0,50);
		PTG_GlobalSO_tmp.brDelay_remMS = mClamp(%file.readLine(),0,50);
		PTG_GlobalSO_tmp.genSpeed = mClamp(%file.readLine(),0,2);
		
		%file.readLine();
		
		PTG_GlobalSO_tmp.frcBrIntoChunks = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.AutoCreateChunks = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.chEditBrPlant = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.chEditOnWrenchData = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.chEditBrPPD = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.chStcBrSpwnPlnt = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.LoadChFileStc = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.chSaveMethod = getWord("ifEdited Always Never",mClamp(%file.readLine(),0,2));
		PTG_GlobalSO_tmp.chSaveExcdResp = getWord("RemoveOld DontSave",mClamp(%file.readLine(),0,1));
		
		%file.readLine();
		
		PTG_GlobalSO_tmp.publicBricks = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.DestroyPublicBr = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.publicBricksPBLs = mClamp(%file.readLine(),0,1);
		%file.readLine(); //skip "hide ghosting" option
		PTG_GlobalSO_tmp.allowEchos = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.PreventDestDetail = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.PreventDestTerrain = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.PreventDestBounds = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.allowNonHost_BuildManage = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.allowNOnHost_SetUpload = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.allowNH_SrvrCmdEventUse = mClamp(%file.readLine(),0,1);
		PTG_GlobalSO_tmp.AllowPlyrPosChk = mClamp(%file.readLine(),0,1);
		
		PTG_GlobalSO_tmp.fontSize = mClamp(%test = %file.readLine(),1,30);
		
		PTG_GlobalSO_tmp.ChunkHLACol = PTG_FindClosestColor(%tmpHLColA,"RGBA-RGBAarr") % 64;
		PTG_GlobalSO_tmp.ChunkHLBCol = PTG_FindClosestColor(%tmpHLColB,"RGBA-RGBAarr") % 64;
		
		
		%file.close();
		%file.delete();
	}

	
	//////////////////////////////////////////////////
	//Finialize / Clear Past Information

	
	if(PTG_ErrorCheck(%cl))
		SERVERCMDPTGStart(%cl);
	
	$PTG.lastSetUploadTime = "";
	$PTG.lastSetUploadPlayer = "";
	$PTG.lastSetUploadID = "";
	$PTG.uploadingSettings = false;

	deleteVariables("$PTGSrvrColArr*");
	deleteVariables("$tmpPriRefArr*");
}

