 //Always return parent function (unless function would return void / doesn't return a value) (?)
 
 package PTG_Main_ServerPackage
{
	//// DESTROY SCRIPT OBJECTS ON SERVER EXIT / SHUTDOWN ////
	function onExit()
	{
		//not sure when this function is executed...
		
		%parent = Parent::onExit();
		
		//if($PTG.routine_Process !$= "None" && !$PTG.routine_isHalting) //this check might not work because server PTG object is deleted below before routine can halt //test anyway
		//{
		//	$PTG.routine_isHalting = true;
		//	
		//	messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 Unexpected server exit, attempting to force halt PTG generation routine... <color:ff0000>[X]");
		//	if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: Unexpected server exit, attempting to force halt PTG generation routine... \c4[X] \c0->" SPC getWord(getDateTime(),1));
		//}
		
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
		
		if(isObject(BrickGroup_Chunks))
			BrickGroup_Chunks.delete(); //???
		if(isObject(BrickGroup_HighlightChunks))
			BrickGroup_HighlightChunks.delete(); //???
		
		deleteVariables("$PTG");
		deleteVariables("$PTGmain");
		deleteVariables("$PTGbio");		
		deleteVariables("$PTG_TmpBuildArr_LineUpld*"); //don't use clear for build upload function because it requires a client, no build upload in progress, etc. and could cause problems
		deleteVariables("$PTG_TmpBuildArr_LineCount");
		deleteVariables("$PTG_TmpBuildArr_BuildData");
		deleteVariables("$PTG_BuildLBrConvPass");
		deleteVariables("$StrArrayData_Builds"); //should be deleted automatically when build loading is finished, but added to be safe
		deleteVariables("$PTG_massDetCurrBiome");
		deleteVariables("$PTG_massDetCurrNum");
		deleteVariables("$PTG_massDetActCount");
		deleteVariables("$PTG_MDfailsafe");
		deleteVariables("$PTG_secRelayCnt");
		deleteVariables("$PTG_DefaultSetupPass");
		deleteVariables("$PTG_HostCl");
		deleteVariables("$PTG_init");
		deleteVariables("$PTG_ModTerInvForce");
		
		return %parent;
	}
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	
	function onServerDestroyed() //destroys objects automatically?
	{
		%parent = Parent::onServerDestroyed();
		
		deleteVariables("$PTG");
		deleteVariables("$PTGmain");
		deleteVariables("$PTGbio");		
		deleteVariables("$PTG_TmpBuildArr_LineUpld*"); //don't use clear for build upload function because it requires a client, no build upload in progress, etc. and could cause problems
		deleteVariables("$PTG_TmpBuildArr_LineCount");
		deleteVariables("$PTG_TmpBuildArr_BuildData");
		deleteVariables("$PTG_BuildLBrConvPass");
		deleteVariables("$StrArrayData_Builds"); //should be deleted automatically when build loading is finished, but added to be safe
		deleteVariables("$PTG_massDetCurrBiome");
		deleteVariables("$PTG_massDetCurrNum");
		deleteVariables("$PTG_massDetActCount");
		deleteVariables("$PTG_MDfailsafe");
		deleteVariables("$PTG_secRelayCnt");
		deleteVariables("$PTG_DefaultSetupPass");
		deleteVariables("$PTG_HostCl");
		deleteVariables("$PTG_init");
		deleteVariables("$PTG_ModTerInvForce");
		
		return %parent;
	}
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//// FIND CLIENT VERSION OF ADD-ON ////
	function GameConnection::SpawnPlayer(%cl)
	{
		%parent = parent::SpawnPlayer(%cl);
		
		if(!%cl.PTGver)
		{
			%secKey = %cl.PTGsecKey = getRandom(1,999999);
			commandToClient(%cl,'PTG_RequestClVer',%secKey);
		}
		if(!$PTG_HostCl && PTG_HostCheck(%cl))
			$PTG_HostCl = %cl;
		
		return %parent;
	}
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//// JOIN SERVER MESSAGE ////
	function GameConnection::autoAdminCheck(%cl)
	{
		%parent = parent::autoAdminCheck(%cl);
		
		if(!$Pref::Server::PTG::DontAdvertise) //set "$Pref::Server::PTG::DontAdvertise = 1;" to disable server join message
			messageClient(%cl,"","\c6This server is running version \c13 \c6of the " SPC PTG_GetFP("Download","","",""));
		
		return %parent;
	}
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//// FORCE BRICKS LOADED FROM A .BLS SAVE INTO CHUNKS, IF ENABLED ////
	function fxDtsBrick::onLoadPlant(%brick)
	{
		%parent = Parent::onLoadPlant(%brick);
		%cl = %brick.client;
		
		//Streams loaded from .bls saves don't have custom collision settings nor physical water zones applied
		
		//Make sure necessary script / sim groups are set up
		if(!isObject(PTG_GlobalSO))
		{
			//Use bottomPrintAll instead of commandToClient
			bottomPrintAll("\c0P\c3T\c1G \c0ERROR: <color:ffffff>No routine settings detected; please have the server host apply \c6their routine settings first.",5);

			%brick.scheduleNoQuota(100,killBrick); //delete instead? //schedule prevents issues with "servercmdPlantBrick" function
			return %parent;
		}
		else if($PTG.frcBrIntoChunks && !isObject(PTG_MainSO)) //main settings required to access chunk size to determine chunk name (if "force bricks into chunks" is enabled)
		{
			bottomPrintAll("\c0P\c3T\c1G \c0ERROR: <color:ffffff>\"Force Bricks Into Chunks\" is enabled, but current chunk size not detected; please have the host or a super admin \"Apply and Start\" a routine first to upload settings.",7);

			%brick.scheduleNoQuota(100,killBrick); //delete instead? //schedule prevents issues with "servercmdPlantBrick" function
			return %parent;
		}
		if(!isObject("BrickGroup_Chunks"))
		{
			%BGm = new SimGroup("BrickGroup_Chunks");
			mainBrickGroup.add(%BGm);
		}
		
		%ChSize = mClamp($PTGm.chSize,16,256);
		%CHPosX = mFloor(getWord(%brick.position,0) / %ChSize) * %ChSize;
		%CHPosY = mFloor(getWord(%brick.position,1) / %ChSize) * %ChSize;
		%Chunk = "Chunk_" @ %CHPosX @ "_" @ %CHPosY;
		
		%brick.PTGPlayerBr = true;
		%brick.PTGBrType = "PlayerBr";
		%brick.PTG = true;
		%BrZ = getWord(%brick.getWorldBox(),5);

		//If "force bricks into chunks" is disabled, only add brick to chunk if chunk exists
		if(!$PTG.frcBrIntoChunks)
		{
			if(isObject(%Chunk))
			{
				%Chunk.add(%brick);
				
				//If chunks are set to be "edited" if a brick is loaded inside, or "static" if a spawn brick is loaded inside
				if($PTG.chEditBrPlant)
					%Chunk.ChEditedPTG = true;
				if($PTG.chStcBrSpwnPlnt && strStr(%brick.getDataBlock().getName(),"Spawn") != -1)
					%Chunk.ChStaticPTG = true;
			}
		}
		
		//Otherwise, attempt to create new chunk object to add brick to, or remove brick if chunk creation fails
		else //if($PTG.frcBrIntoChunks)
		{
			if(!isObject(%Chunk))
			{
				//If automatic chunk creation is enabled
				if($PTG.AutoCreateChunks) //"$PTG.frcBrIntoChunks" would also be enabled
				{
					//Make sure a chunk save doesn't exist for this location (otherwise could cause problems with new chunk)
					if(!isFile(PTG_GetFP("Chunk-Norm",%Chunk,"","")) && !isFile(PTG_GetFP("Chunk-Perm",%Chunk,"","")))
					{
						BrickGroup_Chunks.add(new SimSet(%Chunk));
						%Chunk.PTGChSize = %ChSize;
					}
					
					//If brick can't be added to a chunk object and since "Force Bricks Into Chunks" is enabled, destroy brick
					else
					{
						bottomPrintAll("\c0P\c3T\c1G \c0ERROR: <color:ffffff>Attempting to create new chunk for loaded brick, but a chunk save exists for this location (\"Force Bricks Into Chunks\" is enabled)",7);
						
						if(isObject(%brick))
							%brick.scheduleNoQuota(100,killBrick); //delete instead? //schedule prevents issues with "servercmdPlantBrick" function

						return %parent;
					}
				}
				
				//If automatic chunk creation is disabled
				else
				{
					bottomPrintAll("\c0P\c3T\c1G \c0ERROR: <color:ffffff>Attempting to create new chunk for loaded brick, but this feature has been disabled by the host.",7);
					
					if(isObject(%brick))
						%brick.scheduleNoQuota(100,killBrick); //delete instead? //schedule prevents issues with "servercmdPlantBrick" function

					return %parent;
				}
			}
			
			%Chunk.add(%brick);
			
			//If chunks are set to be "edited" if a brick is loaded inside, or "static" if a spawn brick is loaded inside
			if($PTG.chEditBrPlant)
				%Chunk.ChEditedPTG = true;
			if($PTG.chStcBrSpwnPlnt && strStr(%brick.getDataBlock().getName(),"Spawn") != -1)
				%Chunk.ChStaticPTG = true;
		}
		
		//If chunks are highlighted, add new highlight object for newly created chunk
		%tmpZ = mCeil((%BrZ + 16) / 32) * 32;
		if(isObject(BrickGroup_HighlightChunks) && %tmpZ > %Chunk.PTGHighestZpos)
		{
			%newChZ = mCeil((%BrZ + 16) / 32);
			%pos = %CHPosX+(%ChSize / 2) SPC %CHPosY+(%ChSize / 2) SPC ((%newChZ / 2) * 32);
			%scale = (%ChSize / 16) SPC (%ChSize / 16) SPC %newChZ;
			
			if(isObject(%HLChunk = strReplace(%Chunk.getName(),"Chunk","HLChunk")))
			{
				%HLChunk.setTransform(%pos);
				%HLChunk.setScale(%scale);
			}
			else
			{
				//Spawn static shape (add to chunk itself) and add to highlight chunk brickgroup
				%tmpStcObj = PTG_Chunk_SpawnStatic(%Chunk,%pos,%scale,"HL-NonStatic");
				BrickGroup_HighlightChunks.add(%tmpStcObj);
				%tmpStcObj.setName(strReplace(%Chunk.getName(),"Chunk","HLChunk")); //already displays name???
			}
		}
		
		//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
		if(%tmpZ > %Chunk.PTGHighestZpos)
			%Chunk.PTGHighestZpos = %tmpZ;
	

		return %parent; //test returning parent (overrides custom function?)
	}
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//// FORCE PLANTED BRICKS INTO CHUNKS, IF ENABLED ////
	function SERVERCMDPlantBrick(%cl)
	{
		//%parent = Parent::servercmdPlantBrick(%cl);
		%tmpBr = %cl.player.tempbrick;
		
		//if(!isObject(%cl) || !isObject(%tmpBr))
		//{
		//	echo("\c2>>P\c1T\c4G \c2ERROR: Either client or temp / ghost brick for packaged func \"servercmdPlantBrick\" doesn't exist. [!] \c0->" SPC getWord(getDateTime(),1));
		//	return;
		//}
		if(!isObject(%tmpBr))
		{
			Parent::servercmdPlantBrick(%cl);
			return;
		}

		//Make sure necessary script / sim groups are set up
		if(!isObject(PTG_GlobalSO))
		{
			if(isObject(%cl)) commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ERROR: <color:ffffff>No routine settings detected; please have the server host apply \c6their routine settings first.",5);
			return;
		}
		else if($PTG.frcBrIntoChunks && !isObject(PTG_MainSO)) //main settings required to access chunk size to determine chunk name (if "force bricks into chunks" is enabled)
		{
			if(isObject(%cl)) commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ERROR: <color:ffffff>\"Force Bricks Into Chunks\" is enabled, but current chunk size \c6not detected; please have the host or a super admin \"Apply and Start\" a \c6routine first to upload settings.",7);
			return;
		}
		if(!isObject("BrickGroup_Chunks"))
		{
			%BGm = new SimGroup("BrickGroup_Chunks");
			mainBrickGroup.add(%BGm);
		}

		//Stream Plant Permissions Check
		if(strStr(%tmpBr.getDataBlock().getName(),"StreamPTGData") > -1)
		{
			if($PTG.enabStreams)
			{		
				//If a stream source brick is planted, make sure client exists (for permission checks), player is a super admin, and (if host only is enab) if host
				if(!%cl.isAdmin)
				{
					commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Only admins can plant stream bricks.",5);
					%streamsFail = true;
				}
				if($PTG.StreamsHostOnly && !PTG_HostCheck(%cl))
				{
					commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Stream bricks have been set to host-only.",5);
					%StreamsFail = true;
				}
				
				if(%StreamsFail)
					return %parent;
			}
			else
			{
				commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Stream bricks have been disabled by the host.",5);
				return %parent;
			}
		}
	
		%BrPosXYZ = %tmpBr.getPosition();
		%BrWrldBox = %tmpBr.getWorldBox();
		
		//Determine temp brick size
		%BrWBx = getWord(%BrWrldBox,3) - getWord(%BrWrldBox,0);
		%BrWBy = getWord(%BrWrldBox,4) - getWord(%BrWrldBox,1);
		%BrWBz = getWord(%BrWrldBox,5) - getWord(%BrWrldBox,2);
		
		%ChSize = mClamp($PTGm.chSize,16,256);
		%CHPosX = mFloor(getWord(%tmpBr.position,0) / %ChSize) * %ChSize;
		%CHPosY = mFloor(getWord(%tmpBr.position,1) / %ChSize) * %ChSize;
		%Chunk = "Chunk_" @ %CHPosX @ "_" @ %CHPosY;
		
		if($PTG.frcBrIntoChunks && !isObject(%Chunk))
		{
			//If automatic chunk creation is enabled
			if($PTG.AutoCreateChunks) //"$PTG.frcBrIntoChunks" would also be enabled
			{
				//Make sure a chunk save doesn't exist for this location (otherwise could cause problems with new chunk)
				if(isFile(PTG_GetFP("Chunk-Norm",%Chunk,"","")) || isFile(PTG_GetFP("Chunk-Perm",%Chunk,"","")))
				{
					if(isObject(%cl)) commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ERROR: <color:ffffff>Attempting to create new chunk for brick, but a chunk save exists for this location (\"Force Bricks Into Chunks\" is enabled)",7);
					return;
				}
			}
			
			//If automatic chunk created is disabled
			else
			{
				if(isObject(%cl)) commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ERROR: <color:ffffff>Attempting to create new chunk for brick, but this feature has been disabled by the host.",5);
				return;
			}
		}

		
		return Parent::servercmdPlantBrick(%cl);
	}
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//// DYNAMIC STREAM BRICK GENERATION CASCADE FROM SOURCE ////
	function fxDtsBrick::onPlant(%brick)
	{
		//Removing streams dynamically as well?
		//Attempt to merge below stream brick as well? //Merge with other streams?
		//necessary script objects should exist by now
		
		if(isObject(%brick))
			%cl = %brick.client;
		if(!isObject(%cl) || !isObject(%brick))
		{
			//echo("\c2>>P\c1T\c4G \c2ERROR: Either client or temp / ghost brick for packaged func \"onPlant\" doesn't exist. [!] \c0->" SPC getWord(getDateTime(),1));
			return;
		}

		//Make sure necessary script / sim groups are set up
		if(!isObject(PTG_GlobalSO))
		{
			if(isObject(%cl)) commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ERROR: <color:ffffff>No routine settings detected; please have the server host apply \c6their routine settings first.",5);
			return;
		}
		else if($PTG.frcBrIntoChunks && !isObject(PTG_MainSO)) //main settings required to access chunk size to determine chunk name //omit "$PTG.frcBrIntoChunks && " because chunk size required anyway
		{
			if(isObject(%cl)) commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ERROR: <color:ffffff>\"Force Bricks Into Chunks\" is enabled, but current chunk size not detected; please have the host or a super admin \"Apply and Start\" a routine first to upload settings.",7);
			return;
		}
		if(!isObject("BrickGroup_Chunks"))
		{
			%BGm = new SimGroup("BrickGroup_Chunks");
			mainBrickGroup.add(%BGm);
		}

		%parent = Parent::onPlant(%brick);
		
		
		//if(!isObject(%brick))// || %brick.PTG) //???
		//	return %parent;

		%brDB = %brick.getDataBlock();
		%BrPos = %brick.getPosition();
		%BrWB = %brick.getWorldBox();
		//%brDBn = %brDB.getName();
		%tempSizeXY = %brDB.brickSizeX;
		%BrZ = getWord(%BrWB,5); //getWord(%BrPos,2) + getWord(%BrWB,5);
		%newBrDB = "brick" @ %tempSizeXY @ "x" @ %tempSizeXY @ "fData";
		%ChSize = mClamp($PTGm.chSize,16,256);
		
		%CHPosX = mFloor(getWord(%BrPos,0) / %ChSize) * %ChSize;
		%CHPosY = mFloor(getWord(%BrPos,1) / %ChSize) * %ChSize;
		%Chunk = "Chunk_" @ %CHPosX @ "_" @ %CHPosY;


		//If brick not planted by generator
		if(!%brick.PTG)// || %brDB.PTGStreamSrc || %brick.PTGStreamBr)
		{
			%brick.PTGPlayerBr = true;
			%brick.PTGBrType = "PlayerBr";
			%brick.PTG = true;
			%tempAllowBr = true;
			
				//this won't work (below)
				//%brick.PTGAllowEventEdit = true; //allow chunk to be "edited" (if option is enabled) when events are added manually to a brick (instead of auto via onPlant)
			
			//If brick isn't already in a chunk, add to relative chunk object
			if(!%brick.inPTGChunk)
			{
				//If "force bricks into chunks" is disabled, only add brick to chunk if chunk exists
				if(!$PTG.frcBrIntoChunks)
				{
					if(isObject(%Chunk))
					{
						%Chunk.add(%brick);
						%brick.inPTGChunk = true;
						
						//If chunks are set to be "edited" if any brick planted inside, or "static" if a spawn brick planted inside
						if($PTG.chEditBrPlant)
							%Chunk.ChEditedPTG = true;
						if($PTG.chStcBrSpwnPlnt && strStr(%brick.getDataBlock().getName(),"Spawn") != -1)
							%Chunk.ChStaticPTG = true;
					}
				}
				
				///Otherwise, attempt to create new chunk object to add brick to, or remove brick if chunk creation fails
				else //if(!$PTG.frcBrIntoChunks)
				{
					if(!isObject(%Chunk))
					{
						//If automatic chunk creation is enabled
						if($PTG.AutoCreateChunks) //"$PTG.frcBrIntoChunks" would also be enabled
						{
							//Make sure a chunk save doesn't exist for this location (otherwise could cause problems with new chunk)
							if(!isFile(PTG_GetFP("Chunk-Norm",%Chunk,"","")) && !isFile(PTG_GetFP("Chunk-Perm",%Chunk,"","")))
							{
								BrickGroup_Chunks.add(%Chunk = new SimSet(%Chunk));
								%Chunk.PTGChSize = %ChSize;
								%Chunk.add(%brick);
								%brick.inPTGChunk = true;
								
								//If chunks are set to be "edited" if a brick planted inside, or "static" if a spawn brick planted inside
								if($PTG.chEditBrPlant)
									%Chunk.ChEditedPTG = true;
								if($PTG.chStcBrSpwnPlnt && strStr(%brick.getDataBlock().getName(),"Spawn") != -1)
									%Chunk.ChStaticPTG = true;
							}
							
							//If brick can't be added to a chunk object and since "Force Bricks Into Chunks" is enabled, destroy brick
							else
							{
								if(isObject(%cl)) commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ERROR: <color:ffffff>Attempting to create new chunk for brick, but a chunk save exists for this location (\"Force Bricks Into Chunks\" is enabled)",7);
								return;
							}
						}
						
						//If automatic chunk created is disabled
						else
						{
							if(isObject(%cl)) commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ERROR: <color:ffffff>Attempting to create new chunk for brick, but this feature has been disabled by the host.",5);
							return;
						}
					}
					else
					{
						%Chunk.add(%brick);
						%brick.inPTGChunk = true;
						
						//If chunks are set to be "edited" if a brick planted inside, or "static" if a spawn brick planted inside
						if($PTG.chEditBrPlant)
							%Chunk.ChEditedPTG = true;
						if($PTG.chStcBrSpwnPlnt && strStr(%brick.getDataBlock().getName(),"Spawn") != -1)
							%Chunk.ChStaticPTG = true;
					}
				}
			}
		}

		
		//////////////////////////////////////////////////
		
		//If brick is a source or cascading stream brick 
			//run script below even if brick is ".PTG" / planted by generator, encase using streams as details
		if(%brDB.PTGStreamSrc || %brick.PTGStreamBr) //"if(strStr(%brDB.getName(),"StreamPTGData") > -1)" breaks streams
		{
			//If streams are enabled
			if($PTG.enabStreams)
			{
				//Stream distance
				if(%brDB.PTGStreamSrc) //remove?
				{
					//If a stream source brick is planted, make sure client exists (for permission checks), player is a super admin, and (if host only is enab) if host
					if(!isObject(%cl))
					{
						echo("\c2>>P\c1T\c4G \c2ERROR: Packaged func \"fxDtsBrick::onPlant\" handling dynamic streams failed; client \"" @ %cl @ "\" not found! Stream cascade aborted. [!] \c0->" SPC getWord(getDateTime(),1));
						%streamsFail = true;
					}
					if(!%cl.isAdmin)
					{
						commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Only admins can plant stream bricks.",5);
						%streamsFail = true;
					}
					if($PTG.StreamsHostOnly && !PTG_HostCheck(%cl))
					{
						commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Stream bricks have been set to host-only.",5);
						%StreamsFail = true;
					}
					
					if(%StreamsFail)
					{
						%brick.schedule(100,delete); //killBrick? //schedule prevents issues with "servercmdPlantBrick" function
						return %parent;
					}
					
					//If checks above pass, setup necessary fields
					%brick.PTGStreamBr = true;
					%brick.PTGDetailBr = true;
					%dist = %brick.PTGStreamDist = mFloor(mClamp($PTG.streamsMaxDist,0,8));
				}
				else
					%dist = %brick.PTGStreamDist - 1;

				//%BrObjBox = %brick.getObjectBox();
					//%BrWrldBox = %brick.getWorldBox();
				%BrWB = %brick.getWorldBox();
				%BrPosX = getWord(%BrPos,0);
				%BrPosY = getWord(%BrPos,1);
				%BrPosZ = getWord(%BrPos,2);
					%BrPosZb = %BrPosZ - 0.1; //position of bottom of brick
				
				//Determine brick size to use for container search below
				%BrWBx = getWord(%BrWB,3) - getWord(%BrWB,0);
				%BrWBy = getWord(%BrWB,4) - getWord(%BrWB,1);
				%BrWBz = 0.2; //getWord(%BrWB,5) - getWord(%BrWB,2);

				if(isObject(%cl))
					%BG = getBrickGroupFromObject(%cl).getname();
				else
					%BG = "BrickGroup_888888";
				
				//%brick.PTGPlayerBr = true;
				//%brick.PTGBrType = "PlayerBr";
				//%brick.PTG = true;
				
				
				//If floating streams is disabled, prevent them from generating if the source brick is floating
					//"%brDB.PTGStreamSrc" prevents issues with non-source streams
					//Setup check here to access brick position var above and to run streams enabled check prior
				if(%brDB.PTGStreamSrc && !$PTG.floatStreams && !isObject(%brick.getDownBrick(0)) && mFloatLength(%BrPosZb,1) != 0) //don't use ".hasPathtoGround" since terrain is rarely connected to ground and won't allow streams to plant
				{
					if(isObject(%cl))
						commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Stream generation for floating source bricks is currently disabled.",5);
					
					return %parent;
				}
				
				//Container search below old stream brick
				%BrSizeXYZ = %BrWBx - 0.1 SPC %BrWBy - 0.1 SPC 0.1; //"%BrWBz - 0.1" or "0.2 - 0.1"
				%BrPosXYZ = getWords(%BrPos,0,1) SPC %BrPosZ - 0.2;
				initContainerBoxSearch(%BrPosXYZ,%BrSizeXYZ,$TypeMasks::FxBrickObjectType | $Typemasks::PhysicalZoneObjectType); // | $Typemasks::StaticObjectType);
				
				//////////////////////////////////////////////////

				//Clear detail bricks in stream's path, if feature enabled by user (also allow stream to continue flowing downward if detail brick removed below)
				if(isObject(%objB = containerSearchNext()) && $PTG.streamsClrDetails && %objB.getClassName() $= "fxDTSBrick" && %objB.PTGDetailBr)
				{
					if(!%objB.getDataBlock().PTGStreamSrc && !%objB.PTGStreamBr)
					{
						//If a brick is planted above this detail brick, remove top brick first
						if(isObject(%tmpObjB = %objB.getUpBrick(0)))
							%tmpObjB.schedule(250,delete);
						
						//Remove detail brick
						%objB.delete();
						%objB = "";
						
						%appAmB = 500;
					}
				}

				//Generate new streams bricks adjacent to old brick (N,E,S,W) - if search below prev brick fails (i.e. brick present)
				if(isObject(%objB) || mFloatLength(%BrPosZb,1) <= 0) //(%BrPosz - getWord(%BrOB,5) - 0.1) <= 0) //getMax(0,%wpPos) //%tempZ - getWord(%BrOB,5) //(%BrPosz - getWord(%BrOB,5)) <= 0
				{
					//Don't generate new stream brick if stream dist > amount set by user nor if water brick below old stream brick
					if(%dist > 0 && !%objB.isWaterBrick)
					{
						%appAmC = mClamp($PTG.streamsTickMS,33,2013);
						
						//Check old stream brick adjacent quadrants to plant new stream bricks
						for(%tempY = %BrPosY - %BrWBy; %tempY <= (%BrPosY + %BrWBy); %tempY += %BrWBy)
						{
							for(%tempX = %BrPosX - %BrWBx; %tempX <= (%BrPosX + %BrWBx); %tempX += %BrWBx)
							{
								//Adjacent edgse check (make sure new brick's position not in corner of grid and not overlapping old brick's position)
								if((%vd = VectorDist(%tempX SPC %tempY,%BrPosX SPC %BrPosY)) <= %BrWBx && %vd > 0) //either %BrWBx or %BrWBy can be used
								{
									//N,E,S,W container searches (to prevent overlapping bricks)
									%BrPosXYZ = %tempX SPC %tempY SPC %BrPosZ; // - getWord(%BrOB,5) + 0.1;
									initContainerBoxSearch(%BrPosXYZ,%BrSizeXYZ,$TypeMasks::FxBrickObjectType | $Typemasks::PhysicalZoneObjectType); // | $Typemasks::StaticObjectType);
									
									//Clear detail bricks in stream's path, if feature enabled by user
									if(isObject(%objC = containerSearchNext()) && $PTG.streamsClrDetails && %objC.getClassName() $= "fxDTSBrick" && %objC.PTGDetailBr)
									{
										if(!%objC.getDataBlock().PTGStreamSrc && !%objC.PTGStreamBr)
										{
											//If a brick is planted above this detail brick, remove top brick first
											if(isObject(%tmpObjC = %objC.getUpBrick(0)))
												%tmpObjC.schedule(250,delete);
											
											//Remove detail brick
											%objC.delete();
											%objC = "";
											
											%appAmC += 500;
										}
									}

									//Don't attempt to generate stream brick if there is an adjacent brick at that location, unless adj brick meets certain conditions
									if(!%objC)
									{
										%CHPosX = mFloor(%tempX / %ChSize) * %ChSize;
										%CHPosY = mFloor(%tempY / %ChSize) * %ChSize;
										%Chunk = "Chunk_" @ %CHPosX @ "_" @ %CHPosY; //what if chunk object doesn't exist?

										//Check if chunk object exists for this location, or attempt to create new chunk object if "force bricks into chunks" is enabled
										if(!$PTG.frcBrIntoChunks) //isObject(%Chunk) ||
										{
											schedule(%appAmC += mClamp($PTG.streamsTickMS,33,2013),0,PTG_Chunk_PlantBrick,%newBrDB,%BrposXYZ,%brick.colorID,0,0,0,%brick.printID,%cl,%BG,%Chunk,"StreamBr",%dist);
										
											//Set chunk to "edited" if stream generates inside if "brick plant -> chunk edited" feature is enabled
											if($PTG.chEditBrPlant && isObject(%Chunk))
												%Chunk.ChEditedPTG = true;
										}
										else //if($PTG.frcBrIntoChunks)
										{
											if(!isObject(%Chunk))
											{
												//If automatic chunk creation is enabled
												if($PTG.AutoCreateChunks) //"$PTG.frcBrIntoChunks" would also be enabled
												{
													if(!isFile(PTG_GetFP("Chunk-Norm",%Chunk,"","")) && !isFile(PTG_GetFP("Chunk-Perm",%Chunk,"","")))
													{
														BrickGroup_Chunks.add(new SimSet(%Chunk));
														%Chunk.PTGChSize = mClamp($PTGm.chSize,16,256);
														schedule(%appAmC += mClamp($PTG.streamsTickMS,33,2013),0,PTG_Chunk_PlantBrick,%newBrDB,%BrposXYZ,%brick.colorID,0,0,0,%brick.printID,%cl,%BG,%Chunk,"StreamBr",%dist);
													
														//Set chunk to "edited" if stream generates inside if "brick plant -> chunk edited" feature is enabled
														if($PTG.chEditBrPlant)
															%Chunk.ChEditedPTG = true;
													}
													else
													{
														if(isObject(%cl))
															commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ERROR: <color:ffffff>Attempting to create new chunk for brick, but a chunk save exists for this location (\"Force Bricks Into Chunks\" is enabled)",7);
														if(isObject(%brick))
															%brick.schedule(100,delete); //"delete" to prevent destroying brings below using "killBrick" //schedule prevents issues with returning parent
													}
												}
												
												//If automatic chunk creation is disabled
												else
												{
													if(isObject(%cl))
														commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ERROR: <color:ffffff>Attempting to create new chunk for brick, but this feature has been disabled by the host.",5);
													if(isObject(%brick))
														%brick.schedule(100,delete); //schedule prevents issues with returning parent
													
													return %parent;
												}
											}
											else
											{
												schedule(%appAmC += mClamp($PTG.streamsTickMS,33,2013),0,PTG_Chunk_PlantBrick,%newBrDB,%BrposXYZ,%brick.colorID,0,0,0,%brick.printID,%cl,%BG,%Chunk,"StreamBr",%dist);
											
												//Set chunk to "edited" if stream generates inside if "brick plant -> chunk edited" feature is enabled
												if($PTG.chEditBrPlant)
													%Chunk.ChEditedPTG = true;
											}
										}
									}
								}
							}
						}
					}
					else
						return %parent;
				}
				
				//////////////////////////////////////////////////
				
				//Generate new stream brick below old brick
				else
				{
					//If WaterPlane object exists, find height
					if(isObject(WaterPlane))
						%wpPos = getWord(WaterPlane.getPosition(),2);
					
					//Make sure new stream brick isn't generated below environment WaterPlane level, if set by user
					if(%BrPosZb > %wpPos) //(%BrPosz - getWord(%BrOB,5))
					{
						%CHPosX = mFloor(%BrPosX / %ChSize) * %ChSize;
						%CHPosY = mFloor(%BrPosY / %ChSize) * %ChSize;
						%Chunk = "Chunk_" @ %CHPosX @ "_" @ %CHPosY; //what if chunk object doesn't exist?
						
						//"mFloor(mClamp($PTG.streamsMaxDist,0,8))+1" prevents flow distance from being off by 1 ("%dist = %brick.PTGStreamDist - 1" above) once a column ends and streams flow horizontally
				
						//Check if chunk object exists for this location, or attempt to create new chunk object if "force bricks into chunks" is enabled
						if(!$PTG.frcBrIntoChunks) //isObject(%Chunk) || 
							schedule(mClamp($PTG.streamsTickMS,33,2013) + %appAmB,0,PTG_Chunk_PlantBrick,%newBrDB,%BrposXYZ,%brick.colorID,0,0,0,%brick.printID,%cl,%BG,%Chunk,"StreamBr",mFloor(mClamp($PTG.streamsMaxDist,0,8))+1);
						else //if($PTG.frcBrIntoChunks)
						{
							//Chunk should already exist since stream brick passed check above, 
								//but chunk could still be deleted before bottom steam brick is planted
							if(!isObject(%Chunk))
							{
								//If automatic chunk creation is enabled
								if($PTG.AutoCreateChunks) //"$PTG.frcBrIntoChunks" would also be enabled
								{
									//Chunk save chunk for this location not really necessary (since streams are flowing downward not horizontally), but added as precaution
									if(!isFile(PTG_GetFP("Chunk-Norm",%Chunk,"","")) && !isFile(PTG_GetFP("Chunk-Perm",%Chunk,"","")))
									{
										BrickGroup_Chunks.add(new SimSet(%Chunk));
										%Chunk.PTGChSize = mClamp($PTGm.chSize,16,256);
										schedule(mClamp($PTG.streamsTickMS,33,2013) + %appAmB,0,PTG_Chunk_PlantBrick,%newBrDB,%BrposXYZ,%brick.colorID,0,0,0,%brick.printID,%cl,%BG,%Chunk,"StreamBr",mFloor(mClamp($PTG.streamsMaxDist,0,8))+1);
									}
									else
									{
										if(isObject(%cl))
											commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ERROR: <color:ffffff>Attempting to create new chunk for brick, but a chunk save exists for this location (\"Force Bricks Into Chunks\" is enabled)",7);
										if(isObject(%brick))
											%brick.schedule(100,delete); //"delete" to prevent destroying bricks below using "killBrick" //schedule prevents issues with "servercmdPlantBrick" function
									}
								}
								
								//If automatic chunk creation is disabled
								else
								{
									if(isObject(%cl))
										commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ERROR: <color:ffffff>Attempting to create new chunk for brick, but this feature has been disabled by the host.",5);
									if(isObject(%brick))
										%brick.schedule(100,delete); //schedule prevents issues with returning parent
									
									return %parent;
								}
							}
							else
								schedule(mClamp($PTG.streamsTickMS,33,2013) + %appAmB,0,PTG_Chunk_PlantBrick,%newBrDB,%BrposXYZ,%brick.colorID,0,0,0,%brick.printID,%cl,%BG,%Chunk,"StreamBr",mFloor(mClamp($PTG.streamsMaxDist,0,8))+1);
						}
					}
					
					//////////////////////////////////////////////////
					
					//don't remove most recent stream brick since it needs to be referenced by next one

					//Attempt to join similar stream bricks together to keep brick count down - by using vertically-expanding zone checks
					schedule(mClamp($PTG.streamsTickMS,33,2013) + mClamp($PTG.streamsTickMS,33,2013) + %appAmB,0,PTG_GroupStreamBr_Recurs,%BrPosX,%BrPosY,%BrPosZ,%BrWBx,%BrWBy,%BrWBx * 2,0,%cl,%BG,%Chunk);
				}
			}
			
			//If streams are disabled in general (for host and super admins as well)
			else
			{
				if(isObject(%cl))
					commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Stream bricks have been disabled by the host.",5);
				if(isObject(%brick))
					%brick.schedule(100,delete); //don't use killBrick as it may destroy an entire column of streams (and could crash server) //schedule prevents issues with "servercmdPlantBrick" function
			
				return %parent;
			}
		}
		
		//Highlight chunks and adjust highest chunk position
			//don't gen highlight chunk objects here for generated bricks (Chunks.cs takes care of that condition)
		if((!%brick.PTG || %tempAllowBr || %brDB.PTGStreamSrc || %brick.PTGStreamBr) && isObject(%Chunk))
		{
			if(isObject(%brick.getUpBrick(0)) || isObject(%brick.getDownBrick(0)) || getWord(%BrWB,2) == 0)
			{
				//If chunks are highlighted, add new highlight object for newly created chunk
				%tmpZ = mCeil((%BrZ + 16) / 32) * 32;
				if(isObject(BrickGroup_HighlightChunks) && %tmpZ > %Chunk.PTGHighestZpos)
				{
					%newChZ = mCeil((%BrZ + 16) / 32);
					%pos = %CHPosX+(%ChSize / 2) SPC %CHPosY+(%ChSize / 2) SPC ((%newChZ / 2) * 32);//$PTGm.boundsHLevel+2;
					%scale = (%ChSize / 16) SPC (%ChSize / 16) SPC %newChZ;
					
					if(isObject(%HLChunk = strReplace(%Chunk.getName(),"Chunk","HLChunk")))
					{
						%HLChunk.setTransform(%pos);
						%HLChunk.setScale(%scale);
					}
					else
					{
						//Spawn static shape (add to chunk itself) and add to highlight chunk brickgroup
						%tmpStcObj = PTG_Chunk_SpawnStatic(%Chunk,%pos,%scale,"HL-NonStatic");
						BrickGroup_HighlightChunks.add(%tmpStcObj);
						%tmpStcObj.setName(strReplace(%Chunk.getName(),"Chunk","HLChunk")); //already displays name???
					}
				}
				
				//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
				if(%tmpZ > %Chunk.PTGHighestZpos) //"+ 16" is to make sure rel boundary level is above brick
					%Chunk.PTGHighestZpos = %tmpZ;
			}
		}
		
		return %parent;
	}
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//// REMOVE PHYSICAL WATER ZONE FOR STREAMS IF PRESENT AND ON BRICK DEATH
	function fxDTSBrick::onDeath(%this)
	{
		%parent = parent::onDeath(%this); 

		if((%this.PTGStreamBr || %this.PTGStreamBrAux || %this.PTGStreamBrTert) && %this.PhysicalZone && isObject(%this.PhysicalZone))
		{
			%this.PhysicalZone.delete();
			%this.PhysicalZone = "";
		}
		
		return %parent;
	}
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//// REMOVE PHYSICAL WATER ZONE FOR STREAMS IF PRESENT AND ON BRICK DELETION
	function fxDTSBrick::onRemove(%this)
	{
		%parent = parent::onRemove(%this); 

		if((%this.PTGStreamBr || %this.PTGStreamBrAux || %this.PTGStreamBrTert) && %this.PhysicalZone && isObject(%this.PhysicalZone))
		{
			%this.PhysicalZone.delete();
			%this.PhysicalZone = "";
		}
		
		return %parent;
	}
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//// FLOATING BRICK SUPPORT ////
	function fxDTSBrick::killBrick(%this)
	{
		///Void function
		//"parent::killBrick(%this);" isn't necessary here, otherwise breaks new function below
		
		//Remove physical water zone, if added to (fill) stream brick
		if((%this.PTGStreamBr || %this.PTGStreamBrAux || %this.PTGStreamBrTert) && %this.PhysicalZone && isObject(%this.PhysicalZone))
		{
			%this.PhysicalZone.delete();
			%this.PhysicalZone = "";
		}

		//Check if brick is a generated detail, was planted by a player or is a stream source
		//Also, check if brick is not connected to the ground nor is already being destroyed using custom removal method ("!%this.PTGNullBr")
		if((%this.PTGDetailBr || %this.PTGPlayerBr || %this.getDataBlock().PTGStreamSrc) && !%this.PTGNullBr && !%this.hasPathToGround())
		{
			//If brick will destroy other bricks (otherwise use default function)
			if(%this.willCauseChainKill())
			{
				//don't check if chunk should be edited here, only for relative item funcs (some PTG functions use "killBrick" and "delete")
				
				//Custom removal of brick
				ServerPlay3D(brickBreakSound,%this.getPosition());
				%this.fakeKillBrick(getRandom(-10,10) SPC getRandom(-10,10) SPC getRandom(0,10),3); //"0 0 8"
				%this.PTGNullBr = true; //PTGNullBr check is to prevent object from being constantly fake killed before it's actually deleted
				%this.scheduleNoQuota(500,delete);

				return;
			}
		}

		parent::killBrick(%this); 
		//return;
    }
	
	
	 ////////////////////////////////////////////////////////////////////////////////////////////////////
	 
	//// FLOATING BRICK SUPPORT ////
	function HammerImage::onHitObject(%this,%obj,%slot,%col,%d,%e)//,%f) //%f appears to be unused
	{
		//%parent = parent::onHitObject(%this,%obj,%slot,%col,%d,%e);//,%f);
		
		//"if(%col.willCauseChainKill" isn't necessary
		//%this = hammer
		//%obj = player or bot using hammer
		//%col = collision object / most of the time is a brick
		//%d = position of particle impact?
		//%e = angle of impact?
		
		//Prevent re-destroying same brick (while it's being removed using custom method)
		if((isObject(%col) && %col.PTGNullBrAux))
			return;

		//%BrTypePass = %col.PTGDetailBr || %col.PTGPlayerBr || %col.PTGStreamBrTert || %col.StreamBr || %col.StreamBrAux; // 
		%BrClPass = %col.getClassName() $= "fxDTSBrick";
		
		if(%BrClPass)
		{
			if(isObject(%downBr = %col.getDownBrick(0)))
				%downBrPTG = %downBr.PTGgenerated;
			if(isObject(%upBr = %col.getUpBrick(0)))
				%upBrPTG = %upBr.PTGgenerated;
		}
		
		//hammerHitSound doesn't play when the hammer is used on static shapes

		if(isObject(%ObjBrGroup = getBrickGroupFromObject(%col)))
			%ObjBrGroupisPub = %ObjBrGroup.getName() $= "BrickGroup_888888";
		
		//Use parent function only if col is not brick, if brick is attached to the ground or if a brick is planted above / below this brick (if not a detail)
			//don't return parent if brick is a boundary brick (to prevent being destroyed by hammer, unless allowed below)
		if(!isObject(%col) || !%BrClPass || (%col.hasPathToGround() && !col.PTGgenerated && (!%downBrPTG && !%upBrPTG))) //(!%BrTypePass || (%col.PTGPlayerBr && %col.hasPathToGround()) && !%col.ChBoundsPTG && !%col.PTGTerrainBr) || (!%col.PTGDetailBr && isObject(%col.getUpBrick(0)) && isObject(%col.getDownBrick(0))))
		{
			if(!%col.ChBoundsPTG)
				return parent::onHitObject(%this,%obj,%slot,%col,%d,%e);//,%f);
		}
		
		//////////////////////////////////////////////////
		
		//If object hit by hammer projectile is a brick
			//".PTGNullBr" check is to prevent object from being constantly hammered / fake-killed before it's actually deleted
	 	if(%brClPass && !%col.PTGNullBr && (!%downBr || !%upBr || !%col.PTGPlayerBr))
		{
			//%cl = %col.client; //this will get the client from the brick / object hammered
			%cl = %obj.client;

			if(!%col.PTGTerrainBr && !%col.ChBoundsPTG) //if(%BrTypePass)
			{
				//If brick was planted by player
				if(%col.PTGPlayerBr)
				{
					//If brick was planted by a player, saved to chunk file, then reloaded by generator
					if(%col.PTGGenerated)
					{
						//If brick is public
						if(%ObjBrGroupisPub)
						{
							ServerPlay3D(hammerHitSound,%col.getPosition());
							if(isobject(%cl) && %obj.getClassName() $= "Player") commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Player bricks (loaded from chunk saves) are under public ownership and can only be destroyed using the admin / desctructo wand",5);
							
							return;
						}
						
						//Otherwise, do trust check for loaded brick
						else if(isObject(%ObjBrGroup) && getTrustLevel(getBrickGroupFromObject(%cl),%ObjBrGroup) < 2)
						{
							%plyrN = %ObjBrGroup.name;
							%plyrID = %ObjBrGroup.bl_id;
							
							ServerPlay3D(hammerHitSound,%col.getPosition());
							if(isobject(%cl) && %obj.getClassName() $= "Player") commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0Insufficient Trust: <color:ffffff>\"" @ %plyrN @ "\" (ID:" @ %plyrID @ ") doesn't trust you enough to do that",5);
							return;
						}
					}
					
					//If brick was planted by a player
					else
					{
						%ClBrGroup = getBrickGroupFromObject(%cl);
						
						//Trust level check
						if(isObject(%ObjBrGroup) && getTrustLevel(%ClBrGroup,%ObjBrGroup) < 2)
						{
							%plyrN = %ObjBrGroup.name;
							%plyrID = %ObjBrGroup.bl_id;
							
							ServerPlay3D(hammerHitSound,%col.getPosition());
							if(isobject(%cl) && %obj.getClassName() $= "Player") commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0Insufficient Trust: <color:ffffff>\"" @ %plyrN @ "\" (ID:" @ %plyrID @ ") doesn't trust you enough to do that",5);
							
							return;
						}
					}
				}
				
				if(%col.PTGDetailBr)
				{
					//If option to allow destruction of details bricks under public ownership is disabled ("%obj.getClassName() $= "Player"" filters out bots / AIPlayer)
					if(!$PTG.DestroyPublicBr && %ObjBrGroupisPub)
					{
						ServerPlay3D(hammerHitSound,%col.getPosition());
						if(isobject(%cl) && %obj.getClassName() $= "Player") commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Detail bricks are under public ownership and can only be destroyed using the admin / desctructo wand",5);
						
						return;
					}
					
					//If detail bricks set to be destroyed by admin / desctructo wand only ("%obj.getClassName() $= "Player"" filters out bots / AIPlayer)
					if($PTG.PreventDestDetail)
					{
						ServerPlay3D(hammerHitSound,%col.getPosition());
						if(isobject(%cl) && %obj.getClassName() $= "Player") commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Detail bricks have been set to only be destroyed using the admin / desctructo wand",5);
						
						return;
					}
				}
				
				if(%col.PTGGenerated && %col.PTGPlayerBr && %ObjBrGroupisPub)
				{
					ServerPlay3D(hammerHitSound,%col.getPosition());
					if(isobject(%cl) && %obj.getClassName() $= "Player") commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Player bricks (loaded from chunk saves) are under public ownership and can only be destroyed using the admin / desctructo wand",5);
					
					return;
				}
				
				//Call onToolBreak method
				if(isObject(%ObjBrGroup) && isFunction(%col.onToolBreak(%col,%ObjBrGroup)))
					%col.onToolBreak(%col,%ObjBrGroup);
			
				//////////////////////////////////////////////////
			
				//If set up to set current chunk to "edited" when brick is hammered
				if(isObject(PTG_GlobalSO) && isObject(PTG_MainSO) && $PTG.chEditBrPPD && isObject(BrickGroup_Chunks))
				{
					%ChSize = mClamp($PTGm.chSize,16,256);
					%CHPosX = mFloor(getWord(%col.position,0) / %ChSize) * %ChSize;
					%CHPosY = mFloor(getWord(%col.position,1) / %ChSize) * %ChSize;
					%Chunk = "Chunk_" @ %CHPosX @ "_" @ %CHPosY;
					
					if(isObject(%Chunk))
						%Chunk.ChEditedPTG = true;
				}
				
				//Custom removal of brick
				ServerPlay3D(brickBreakSound,%col.getPosition());
				%col.fakeKillBrick(getRandom(-10,10) SPC getRandom(-10,10) SPC getRandom(0,10),3); //"0 0 8"
				%col.PTGNullBr = true;
				%col.scheduleNoQuota(500,delete);
				
				//if brick is planted on top of a detail, remove that too (for details only - for easily clearing details from terrain for building)
				if(%col.PTGDetailBr && isObject(%upDetBr = %col.getUpBrick(0)))
				{
					//don't have to play "brickBreakSound" for this brick
					%upDetBr.fakeKillBrick(getRandom(-10,10) SPC getRandom(-10,10) SPC getRandom(0,10),3); //"0 0 8"
					%upDetBr.PTGNullBr = true;
					%upDetBr.scheduleNoQuota(500,delete);
				}
				
				//return %parent;
			}
			
			//////////////////////////////////////////////////
			
			else
			{
				//If detail bricks set to be destroyed by admin / desctructo wand only
				if(%col.PTGTerrainBr)// && $PTG.PreventDestTerrain)
				{
					ServerPlay3D(hammerHitSound,%col.getPosition());
					
					if(isobject(%cl) && %obj.getClassName() $= "Player")
					{
						if(%ObjBrGroupisPub)
							commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Terrain bricks are under public ownership and can only be destroyed using the admin / desctructo wand",5);
						else
						{
							if($PTG.PreventDestTerrain)
								commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Terrain bricks have been set to only be destroyed using the admin / desctructo wand",5);
							else
								commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Terrain bricks can only be destroyed using the wand",5);
						}
					}
					return;
				}
				
				//If boundary bricks set to be destroyed by admin / desctructo wand only
				if(%col.ChBoundsPTG)// && $PTG.PreventDestBounds)
				{
					ServerPlay3D(hammerHitSound,%col.getPosition());
					
					if(isobject(%cl) && %obj.getClassName() $= "Player")
					{
						if(%ObjBrGroupisPub)
							commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Boundary bricks are under public ownership and can only be destroyed using the admin / desctructo wand",5);
						else
						{
							if($PTG.PreventDestBounds)
								commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Boundary bricks have been set to only be destroyed using the admin / desctructo wand",5);
							else
								commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Boundary bricks can only be destroyed using the wand",5);
						}
					}
					return;
				}
			}
		}
		else if(!%col.PTGNullBr)
			ServerPlay3D(hammerHitSound,%col.getPosition());

		//return parent::onHitObject(%this,%obj,%slot,%col,%d,%e);//,%f);
	 }
	 
	 
	 ////////////////////////////////////////////////////////////////////////////////////////////////////
	 
	//// FLOATING BRICK SUPPORT ////
	function WandImage::onHitObject(%this,%obj,%slot,%col,%d,%e)//,%f) //%f appears to be unused
	{
		//%parent = parent::onHitObject(%this,%obj,%slot,%col,%d,%e);//,%f);
		
		//%this = wand
		//%obj = player / bot using wand(?)
		//%col = collision object / most of the time is a brick
		//%d = position of particle impact?
		//%e = angle of impact?
		
		//Prevent redestroying same brick (while it's being removed using custom method)
		if(isObject(%col) && %col.PTGNullBrAux)
			return;
		
		//%BrTypePass = %col.PTGDetailBr || %col.PTGPlayerBr || %col.PTGStreamBrTert || %col.StreamBr || %col.StreamBrAux;
		%BrClPass = %col.getClassName() $= "fxDTSBrick";
		
		if(%BrClPass)
		{
			if(isObject(%downBr = %col.getDownBrick(0)))
				%downBrPTG = %downBr.PTGgenerated;
			if(isObject(%upBr = %col.getUpBrick(0)))
				%upBrPTG = %upBr.PTGgenerated;
		}
		
		if(isObject(%ObjBrGroup = getBrickGroupFromObject(%col)))
			%ObjBrGroupisPub = %ObjBrGroup.getName() $= "BrickGroup_888888";
		
		//Use parent function only if col is not brick, or if brick is attached to the ground
		if(!isObject(%col) || !%BrClPass || (%col.hasPathToGround() && !%col.PTGgenerated && (!%downBrPTG && !%upBrPTG))) //%col.hasPathToGround()) // && !%BrTypePass && !%col.ChBoundsPTG && !%col.PTGTerrainBr))
		{
			if(!%col.ChBoundsPTG) //don't return parent if brick is a boundary brick (to prevent being destroyed by wand, unless allowed below)
				return parent::onHitObject(%this,%obj,%slot,%col,%d,%e);//,%f);
		}

		//////////////////////////////////////////////////
		
		//Clear inital brick using custom script (".PTGNullBrAux" is to prevent issues with ".PTGNullBr" set by another script before this one runs)
		if(%BrClPass && !%col.PTGNullBrAux)
		{
			//%cl = %col.client; //this will get the client from the brick / object hammered
			%cl = %obj.client;

			//If brick isn't generated terrain or boundaries
			if(!%col.PTGTerrainBr && !%col.ChBoundsPTG) //if(%BrTypePass)
			{
				//If brick was planted by player
				if(%col.PTGPlayerBr)
				{
					//If brick was planted by a player, saved to chunk file, then reloaded by generator
					if(%col.PTGGenerated)
					{
						//If brick is public
						if(%ObjBrGroupisPub)
						{
							//ServerPlay3D(hammerHitSound,%col.getPosition());
							if(isobject(%cl) && %obj.getClassName() $= "Player") commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Player bricks (loaded from chunk saves) are under public ownership and can only be destroyed using the admin / desctructo wand",5);
							return;
						}
						
						//Otherwise, do trust check for loaded brick
						else if(isObject(%ObjBrGroup) && getTrustLevel(getBrickGroupFromObject(%cl),%ObjBrGroup) < 2)
						{
							%plyrN = %ObjBrGroup.name;
							%plyrID = %ObjBrGroup.bl_id;
							
							//ServerPlay3D(hammerHitSound,%col.getPosition());
							if(isobject(%cl) && %obj.getClassName() $= "Player") commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0Insufficient Trust: <color:ffffff>\"" @ %plyrN @ "\" (ID:" @ %plyrID @ ") doesn't trust you enough to do that",5);
							return;
						}
					}
					
					//If brick was planted by a player
					else
					{
						%ClBrGroup = getBrickGroupFromObject(%cl);
						
						//Trust level check
						if(isObject(%ObjBrGroup) && getTrustLevel(%ClBrGroup,%ObjBrGroup) < 2)
						{
							%plyrN = %ObjBrGroup.name;
							%plyrID = %ObjBrGroup.bl_id;
							
							//ServerPlay3D(hammerHitSound,%col.getPosition());
							if(isobject(%cl) && %obj.getClassName() $= "Player") commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0Insufficient Trust: <color:ffffff>\"" @ %plyrN @ "\" (ID:" @ %plyrID @ ") doesn't trust you enough to do that",5);
							return;
						}
					}
				}
				
				//If brick is a biome detail
				if(%col.PTGDetailBr)
				{
					//If option to allow destruction of details bricks under public ownership is disabled
					if(!$PTG.DestroyPublicBr && getBrickGroupFromObject(%col).getName() $= "BrickGroup_888888")
					{
						if(isobject(%cl) && %obj.getClassName() $= "Player") commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Detail bricks are under public ownership and can only be destroyed using the admin / desctructo wand",5);
						return;
					}

					//If detail bricks set to be destroyed by admin / desctructo wand only ("%obj.getClassName() $= "Player"" filters out bots)
					if($PTG.PreventDestDetail)
					{
						if(isobject(%cl) && %obj.getClassName() $= "Player") commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Detail bricks have been set to only be destroyed using the admin / desctructo wand",5);
						return;
					}
				}
			}
			
			//If brick is generated terrain or boundaries
			else
			{			
				//If detail bricks set to be destroyed by admin / desctructo wand only
				if(%col.PTGTerrainBr && $PTG.PreventDestTerrain)
				{
					if(isobject(%cl) && %obj.getClassName() $= "Player")
					{
						if(getBrickGroupFromObject(%col).getName() $= "BrickGroup_888888")
							commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Terrain bricks are under public ownership and can only be destroyed using the admin / desctructo wand",5);
						else
							commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Terrain bricks have been set to only be destroyed using the admin / desctructo wand",5);
					}
					return;
				}
				
				//If detail bricks set to be destroyed by admin / desctructo wand only
				if(%col.ChBoundsPTG && $PTG.PreventDestBounds)
				{
					if(isobject(%cl) && %obj.getClassName() $= "Player")
					{
						if(getBrickGroupFromObject(%col).getName() $= "BrickGroup_888888")
							commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Boundary bricks are under public ownership and can only be destroyed using the admin / desctructo wand",5);
						else
							commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>Boundary bricks have been set to only be destroyed using the admin / desctructo wand",5);
					}
					return;
				}
			}
			
			//Call onToolBreak Method
			if(isObject(%ObjBrGroup) && isFunction(%col.onToolBreak(%col,%ObjBrGroup)))
				%col.onToolBreak(%col,%ObjBrGroup);
			
			//////////////////////////////////////////////////
			
			//If set up to set current chunk to "edited" when brick is destroyed by wand
			if(isObject(PTG_GlobalSO) && isObject(PTG_MainSO) && $PTG.chEditBrPPD && isObject(BrickGroup_Chunks))
			{
				%ChSize = mClamp($PTGm.chSize,16,256);
				%CHPosX = mFloor(getWord(%col.position,0) / %ChSize) * %ChSize;
				%CHPosY = mFloor(getWord(%col.position,1) / %ChSize) * %ChSize;
				%Chunk = "Chunk_" @ %CHPosX @ "_" @ %CHPosY;
				
				if(isObject(%Chunk))
					%Chunk.ChEditedPTG = true;
			}
			
			//Custom removal of brick
			ServerPlay3D(brickBreakSound,%col.getPosition());
			%col.fakeKillBrick(getRandom(-10,10) SPC getRandom(-10,10) SPC getRandom(0,10),3); //"0 0 8"
			%col.PTGNullBrAux = true;
			%col.scheduleNoQuota(%app = 500,delete);
			
			%nextBr = %col.getUpBrick(0);
			
			//Clear all bricks above initial using custom removal script
			while(isObject(%nextBr) && %failSafe++ < 1000)
			{
				%oldBr = %nextBr;
				%nextBr = %oldBr.getUpBrick(0);
				
				ServerPlay3D(brickBreakSound,%oldBr.getPosition());
				%oldBr.fakeKillBrick(getRandom(-10,10) SPC getRandom(-10,10) SPC getRandom(0,10),3); //"0 0 8"
				%oldBr.PTGNullBrAux = true;
				%oldBr.scheduleNoQuota(%app += mClamp($PTG.delay_subFuncMS,0,50),delete); //make sure lag isn't an issue here
			}
		}
		
		//return parent::onHitObject(%this,%obj,%slot,%col,%d,%e);//,%f);
	}
	 
	 
	 ////////////////////////////////////////////////////////////////////////////////////////////////////
	 
	 //// FLOATING BRICK SUPPORT ////
	function AdminWandImage::onHitObject(%this,%obj,%slot,%col,%d,%e)//,%f) //%f appears to be unused
	{
		//%parent = parent::onHitObject(%this,%obj,%slot,%col,%d,%e);//,%f);
		//%this = wand
		//%obj = player / bot using wand(?)
		//%d = position of particle impact?
		//%e = angle of impact?
		
		//Prevent redestroying same brick (while it's being removed using custom method)
		if(isObject(%col) && %col.PTGNullBrAux)
			return;
		
		//%BrTypePass = %col.PTGDetailBr || %col.PTGPlayerBr || %col.PTGStreamBrTert || %col.StreamBr || %col.StreamBrAux;
		%BrClPass = %col.getClassName() $= "fxDTSBrick";
		
		if(%BrClPass)
		{
			if(isObject(%downBr = %col.getDownBrick(0)))
				%downBrPTG = %downBr.PTGgenerated;
			if(isObject(%upBr = %col.getUpBrick(0)))
				%upBrPTG = %upBr.PTGgenerated;
		}
		
		if(isObject(%ObjBrGroup = getBrickGroupFromObject(%col)))
			%ObjBrGroupisPub = %ObjBrGroup.getName() $= "BrickGroup_888888";
		
		//Use parent function only if col is not brick, or if brick is attached to the ground
		if(!isObject(%col) || !%BrClPass || (%col.hasPathToGround() && !%col.PTGgenerated && (!%downBrPTG && !%upBrPTG))) //%col.hasPathToGround()) // && !%BrTypePass && !%col.ChBoundsPTG && !%col.PTGTerrainBr))
		{
			if(!%col.ChBoundsPTG) //don't return parent if brick is a boundary brick (to prevent being destroyed by wand, unless allowed below)
				return parent::onHitObject(%this,%obj,%slot,%col,%d,%e);//,%f);
		}

		//Clear inital brick using custom script (".PTGNullBrAux" is to prevent issues with ".PTGNullBr" set by another script before this one runs)
		if(%BrClPass && !%col.PTGNullBrAux)
		{
			%cl = %obj.client;
			
			//If attempting to remove a public (PTG generated) brick and public brick destruction is disabled for wand
			if(%ObjBrGroupisPub && !%cl.destroyPublicBricks)
			{
				if(isobject(%cl) && %obj.getClassName() $= "Player") commandToClient(%cl,'centerPrint',"\c0P\c3T\c1G \c0ACTION DENIED: <color:ffffff>This brick is under public ownership, but the Desctructo Wand has public brick destruction disabled. Use the \c0/dpb <color:ffffff>chat command to re-enable it",5);
			}
			else
			{
				//If set up to set current chunk to "edited" when brick is destroyed by wand
				if(isObject(PTG_GlobalSO) && isObject(PTG_MainSO) && $PTG.chEditBrPPD && isObject(BrickGroup_Chunks))
				{
					%ChSize = mClamp($PTGm.chSize,16,256);
					%CHPosX = mFloor(getWord(%col.position,0) / %ChSize) * %ChSize;
					%CHPosY = mFloor(getWord(%col.position,1) / %ChSize) * %ChSize;
					%Chunk = "Chunk_" @ %CHPosX @ "_" @ %CHPosY;
					
					if(isObject(%Chunk))
						%Chunk.ChEditedPTG = true;
				}

				//Custom removal of brick
				ServerPlay3D(brickBreakSound,%col.getPosition());
				%col.fakeKillBrick(getRandom(-10,10) SPC getRandom(-10,10) SPC getRandom(0,10),3); //"0 0 8"
				%col.PTGNullBrAux = true;
				%col.scheduleNoQuota(%app = 500,delete);
				
				%nextBr = %col.getUpBrick(0);
			
				//Clear all bricks above initial using custom removal script
				while(isObject(%nextBr) && %failSafe++ < 1000)
				{
					%oldBr = %nextBr;
					%nextBr = %oldBr.getUpBrick(0);
					
					ServerPlay3D(brickBreakSound,%oldBr.getPosition());
					%oldBr.fakeKillBrick(getRandom(-10,10) SPC getRandom(-10,10) SPC getRandom(0,10),3); //"0 0 8"
					%oldBr.PTGNullBrAux = true;
					%oldBr.scheduleNoQuota(%app += mClamp($PTG.delay_subFuncMS,0,50),delete); //make sure lag isn't an issue here
				}
			}
		}
		
		//if /dpb option is disabled
		
		//remove stacked details
		
		//prevent chain-kill if brick underneath (for wand too)
		
		//return parent::onHitObject(%this,%obj,%slot,%col,%d,%e);//,%f);		
			
		//	%nextBr = %col.getUpBrick(0);
			
		//	ServerPlay3D(brickBreakSound,%col.getPosition());
		//	%col.fakeKillBrick(getRandom(-10,10) SPC getRandom(-10,10) SPC getRandom(0,10),3); //"0 0 8"
		//	%col.PTGNullBrAux = true;
		//	%col.scheduleNoQuota(%app = 500,delete);
			
			//Clear all bricks above initial using custom removal script
		//	while(isObject(%nextBr) && %failSafe++ < 1000)
		//	{
		//		%oldBr = %nextBr;
		//		%nextBr = %oldBr.getUpBrick(0);
				
		//		ServerPlay3D(brickBreakSound,%oldBr.getPosition());
		//		%oldBr.fakeKillBrick(getRandom(-10,10) SPC getRandom(-10,10) SPC getRandom(0,10),3); //"0 0 8"
		//		%oldBr.PTGNullBrAux = true;
		//		%oldBr.scheduleNoQuota(%app += mClamp($PTG.delay_subFuncMS,0,50),delete); //make sure lag isn't an issue here
		//	}
		//}
		
		//return parent::onHitObject(%this,%obj,%slot,%col,%d,%e);//,%f);
	}
	 
	 
	 ////////////////////////////////////////////////////////////////////////////////////////////////////
	 
	//// CHUNK-EDITED SUPPORT ////
	function SERVERCMDSetPrint(%cl,%printID)
	{
		//Find brick before including function parent, otherwise print brick is cleared
		%brick = %cl.printBrick;
		%parent = parent::SERVERCMDSetPrint(%cl,%printID);

		if(isObject(PTG_GlobalSO) && isObject(PTG_MainSO) && isObject(%brick) && $PTG.chEditBrPPD && isObject(BrickGroup_Chunks))
		{
			%ChSize = mClamp($PTGm.chSize,16,256);
			%CHPosX = mFloor(getWord(%brick.position,0) / %ChSize) * %ChSize;
			%CHPosY = mFloor(getWord(%brick.position,1) / %ChSize) * %ChSize;
			%Chunk = "Chunk_" @ %CHPosX @ "_" @ %CHPosY;
			
			if(isObject(%Chunk))
				%Chunk.ChEditedPTG = true;
		}

		return %parent;
	}
	 
	 
	////////////////////////////////////////////////////////////////////////////////////////////////////
	 
	//// CHUNK-EDITED SUPPORT ////
	function SERVERCMDSetWrenchData(%cl,%data)
	{
		%parent = parent::serverCmdsetwrenchdata(%cl,%data);

		if(isObject(PTG_GlobalSO) && isObject(PTG_MainSO) && $PTG.chEditOnWrenchData && isObject(BrickGroup_Chunks))
		{
			%brick = %cl.wrenchBrick;

			%ChSize = mClamp($PTGm.chSize,16,256);
			%CHPosX = mFloor(getWord(%brick.position,0) / %ChSize) * %ChSize;
			%CHPosY = mFloor(getWord(%brick.position,1) / %ChSize) * %ChSize;
			%Chunk = "Chunk_" @ %CHPosX @ "_" @ %CHPosY;
			
			if(isObject(%Chunk))
				%Chunk.ChEditedPTG = true;
		}

		return %parent;
	}
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	 
	//// CHUNK-EDITED SUPPORT ////
	function fxDTSBrick::onColorChange(%brick) //!!! test for dedicated servers !!!
	{
		%parent = parent::onColorChange(%brick);
		
		//"%brick.client != -1" prevents setting chunk to "edited" if color is changed for player's ghost brick - should only apply to planted bricks
		if(isObject(PTG_GlobalSO) && isObject(PTG_MainSO) && %brick.client != -1 && $PTG.chEditBrPPD && isObject(BrickGroup_Chunks))
		{
			%ChSize = mClamp($PTGm.chSize,16,256);
			%CHPosX = mFloor(getWord(%brick.position,0) / %ChSize) * %ChSize;
			%CHPosY = mFloor(getWord(%brick.position,1) / %ChSize) * %ChSize;
			%Chunk = "Chunk_" @ %CHPosX @ "_" @ %CHPosY;
			
			if(isObject(%Chunk))
				%Chunk.ChEditedPTG = true;
		}

		return %parent;
	}
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//// CHUNK-EDITED SUPPORT ////
	function SERVERCMDClearEvents(%cl)
	{
		%parent = parent::serverCmdClearEvents(%cl);
		
		if(isObject(PTG_GlobalSO) && isObject(PTG_MainSO) && $PTG.chEditOnWrenchData && isObject(BrickGroup_Chunks))
		{
			%brick = %cl.wrenchBrick;

			%ChSize = mClamp($PTGm.chSize,16,256);
			%CHPosX = mFloor(getWord(%brick.position,0) / %ChSize) * %ChSize;
			%CHPosY = mFloor(getWord(%brick.position,1) / %ChSize) * %ChSize;
			%Chunk = "Chunk_" @ %CHPosX @ "_" @ %CHPosY;
			
			if(isObject(%Chunk))
				%Chunk.ChEditedPTG = true;
		}

		return %parent;
	}
	

	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//// PREVENTS ISSUE W/ BOT_HOLE ADDON AND ADDS INTEGRATED EVENTS TO DETAILS (ONPLANT) EVEN IF PUBLIC OWNERSHIP IS ENABLED ////
	function fxDTSBrick::addEvent(%this,%enabled,%delay,%input,%target,%output,%param1,%param2,%param3,%param4)
	{
		//Make sure client wrench brick exists
			//Line 845 of "SERVERCMDAddEvent" in "Packages.cs" in "Bot_Hole.zip" doesn't check for the client's wrench brick, and echos errors in the console.
			//Public bricks don't set the wrench brick because you can't add events to them, which causes this problem.
			//Therefore, this function "fxDTSBrick::addEvent" prevents "SERVERCMDAddEvent" from being called if the wrench brick isn't found (this function is called before)
		if(!isObject(%this.client.wrenchBrick))
		{
			//Set up events manually since events aren't added to public bricks
			if(isObject(%this) && isObject(PTG_GlobalSO) && $PTG.publicBricks)
			{
				//Thanks to the duplicator for the following code (slightly modified)
				if((%num = %this.numEvents) == 0)
					%num = "0"; //prevents issue with setting first event
				
				%this.eventEnabled[%num] = %enabled;
				%this.eventInput[%num] = %input;
				%this.eventInputIdx[%num] = inputEvent_GetInputEventIdx(%this.eventInput[%num]);
				%this.eventDelay[%num] = %delay;
				%this.eventTarget[%num] = getField(%target,0);
				%this.eventTargetIdx[%num] = inputEvent_GetTargetIndex("fxDtsBrick",%this.eventInputIdx[%num],%this.eventTarget[%num]);
					%this.eventNT[%num] = getField(%target,1); //shouldn't be used, but added just encase target is a named brick
				%outputClass = %this.eventTargetIdx[%num] == -1 ? "fxDtsBrick" : inputEvent_GetTargetClass("fxDtsBrick",%this.eventInputIdx[%num],%this.eventTargetIdx[%num]);
				%this.eventOutput[%num] = %output;
				%this.eventOutputIdx[%num] = outputEvent_GetOutputEventIdx(%outputClass,%this.eventOutput[%num]);
				%this.eventOutputAppendClient[%num] = $OutputEvent_AppendClient[%outputClass,%this.eventOutputIdx[%num]];

				%params = %param1 TAB %param2 TAB %param3 TAB %param4;
				
				//Output Parameters
				for(%c = 0; %c < 4; %c++)
				{
					%eventParamType = getField($OutputEvent_parameterList[%outputClass,%this.eventOutputIdx[%num]],%c);
					
					if(getWord(%eventParamType,0) $= "dataBlock" && isObject(getField(%params,%c)))
						%this.eventOutputParameter[%num,%c + 1] = getField(%params,%c).getId();
					else
						%this.eventOutputParameter[%num,%c + 1] = getField(%params,%c);
				}

				%this.numEvents++;
				%this.implicitCancelEvents = 0;
			}
			
			return;
		}

		return parent::addEvent(%this,%enabled,%delay,%input,%target,%output,%param1,%param2,%param3,%param4);
	}
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//// CHUNK-EDITED SUPPORT ////
	function SERVERCMDAddEvent(%cl,%enabled,%inputID,%delay,%targetID,%targetNameID,%outputID,%param1,%param2,%param3,%param4)
	{
		if(!isObject(%brick = %cl.wrenchBrick))
			return;
		
		%parent = parent::SERVERCMDAddEvent(%cl,%enabled,%inputID,%delay,%targetID,%targetNameID,%outputID,%param1,%param2,%param3,%param4);

		if(isObject(PTG_GlobalSO) && isObject(PTG_MainSO) && $PTG.chEditOnWrenchData && isObject(BrickGroup_Chunks))
		{
			//Don't set chunk to "edited" (if option is enabled) if brick is planted by the generator and events added automatically via "onPlant"
				//When wrench is used on a brick, it first sets up ".PTGAllowEventEdit" field, which allows chunk to be "edited" after applying events (if that also is enabled)
			if(!%brick.PTG || %brick.PTGAllowEventEdit)
			{
				%ChSize = mClamp($PTGm.chSize,16,256);
				%CHPosX = mFloor(getWord(%brick.position,0) / %ChSize) * %ChSize;
				%CHPosY = mFloor(getWord(%brick.position,1) / %ChSize) * %ChSize;
				%Chunk = "Chunk_" @ %CHPosX @ "_" @ %CHPosY;

				if(isObject(%Chunk))
					%Chunk.ChEditedPTG = true;
			}
		}

		return %parent;
	}
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//// CHUNK-EDITED SUPPORT ////
	function WrenchImage::onHitObject(%this,%obj,%slot,%col,%d,%e)
	{
		//%d = position of particle impact?
		//%e = angle of impact?

		//Prevent chunks from being set to "edited" (if option enabled) when events are added automatically to bricks planted by the generator
		%col.PTGAllowEventEdit = true;
		
		return parent::onHitObject(%this,%obj,%slot,%col,%d,%e);
	}
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	////
	//function startGame(%a,%b,%c,%d)
	function startGame()
	{
		%parent = parent::startGame();
		
		//Set up default preset settings for server
			//set "$Pref::Server::PTG::DontLoadDefault = 1;" to disable loading default settings (this will apply to all game instances)
			//set "$PTG_DontLoadDef = 1;" (i.e. in your gamemode "Server.cs" file) to disable loading default settings for this game instance only (this var isn't saved)
		if(!$Pref::Server::PTG::DontLoadDefault && !$PTG_DontLoadDef)
			PTG_LoadServerPreset("",false,false,"None",false,"");
		
		return %parent;
	}
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//// DISABLE FALL AND COLLISION DAMAGE WHILE A NEW PRESET IS LOADING (i.e. encase terrain is cleared) ////
	function player::damage(%obj,%sourceObject,%position,%damage,%damageType)
	{
		if($DamageType_Array[%damageType] $= "Fall" && isObject(PTG_MainSO_SPtmp) && PTG_MainSO_SPtmp.presetLoadFallRes)
			%damage = 0;
		
		parent::damage(%obj,%sourceObject,%position,%damage,%damageType);
	}
};

activatePackage(PTG_Main_ServerPackage);