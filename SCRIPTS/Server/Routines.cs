//"scheduleNoQuota" is used in most cases to prevent routines from being able to continue if over quota (otherwise can't halt / restart PTG routines);
//Normal schedules are used otherwise (such as for clearing boundaries, removing columns of bricks for the packaged wand func, etc.)

//// PAUSE / HALT APPEND OR CLEAR ROUTINES IF LAG DETECTED, NEAR BRICK LIMIT OR HALT IF ROUTINES HALTED ////
function PTG_Routine_Append_LimitsChk(%cl,%type)//,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{	
	//Check if routine halted
	if($PTG.routine_isHalting)
	{
		if($PTG.routine_ProcessAm-- <= 0) //%type $= "Append" && 
		{
			messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6Routine halted successfully. \c0[X]");
			if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0Routine halted successfully. \c4[X] \c0->" SPC getWord(getDateTime(),1));
			
			$PTG.routine_Process = "None";
			$PTG.routine_isHalting = false;
		}
		
		return "Halt";
	}
	
	//If lag check is enabled
	if((isObject(ServerConnection) && !$PTG.disNormLagCheck) || (!isObject(ServerConnection) && !$PTG.disDedLagCheck)) //if(!$PTG.DisableLagCheck)
	{
		if((%delay = mClamp($PTG.delay_PauseTickS,1,30)) > 1)
			%plur = "s";

		//Lag buffer check
		if(isObject(ServerConnection))
		{
			if(ServerConnection.getPing() > mClamp($PTG.pingMaxBuffer,100,1000))
			{
				messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G \c0WARNING: Lag detected, pausing routine for \c6" @ %delay @ " \c0second" @ %plur @ "... [!]");
				if($PTG.allowEchos) echo("\c2>>P\c1T\c4G: \c2WARNING: \c0Lag detected, pausing routine for \c2" @ %delay @ " \c0second" @ %plur @ "... \c2[!] \c0->" SPC getWord(getDateTime(),1));
				
				//$PTG.dedSrvrFuncCheckTime = getSimTime() + (%delay * 1000); //prevent failing lag check for dedicated server (even if not running ded server)
				//scheduleNoQuota(%delay * 1000,0,PTG_Routine_Append,%cl,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
				
				return "Pause"; //wait for lag to diminish before continuing
			}
		}
		else
		{
			if(getMax(getSimTime() - $PTG.dedSrvrFuncCheckTime,0) > mClamp($PTG.DedSrvrFuncBuffer,20,2000))
			{
				messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G \c0WARNING: Dedicated Server lag detected, pausing routine for \c6" @ %delay @ " \c0second" @ %plur @ "... [!]");
				if($PTG.allowEchos) echo("\c2>>P\c1T\c4G: \c2WARNING: \c0Dedicated Server lag detected, pausing routine for \c2" @ %delay @ " \c0second" @ %plur @ "... \c2[!] \c0->" SPC getWord(getDateTime(),1));
				
				//$PTG.dedSrvrFuncCheckTime = getSimTime() + (%delay * 1000); //take pause into account when checking function lag times
				//scheduleNoQuota(%delay * 1000,0,PTG_Routine_Append,%cl,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
				
				return "Pause"; //wait for lag to diminish before continuing
			}
		}
	}
		
	//Bricklimit buffer check
	if(%type !$= "Clear" && !$PTG.disBrBuffer)
	{
		if(getBrickCount() > (getBrickLimit() - mClamp($PTG.brLimitBuffer,0,20000)))
		{
			if($PTGm.genType $= "Infinite")
			{
				messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G \c0WARNING: Exceeded brick limit buffer! Pausing routine for \c6" @ %delay @ " \c0second" @ %plur @ "and attempting to reduce chunk radius / grid for all players... [!]");
				if($PTG.allowEchos) echo("\c2>>P\c1T\c4G: \c2WARNING: \c0Exceeded brick limit buffer! Pausing routine for \c2" @ %delay @ " \c0second" @ %plur @ "and attempting to reduce chunk radius / grid for all players... \c2[!] \c0->" SPC getWord(getDateTime(),1));
				
				//$PTGm.chrad_P = mClamp($PTGm.chrad_P--, 1, 8); //??????? //should work for both radial and square grids
				//$PTGm.chrad_SA = mClamp($PTGm.chrad_SA--, 1, 8); //??????? //limit of 16
				
				//$PTG.dedSrvrFuncCheckTime = getSimTime();
				//scheduleNoQuota(%delay * 1000,0,PTG_Routine_Append,%cl,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}
			else //use chat message for fintie terrain (since routine ends after)?
			{
				messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G \c0WARNING: Exceeded brick limit buffer! Finite routine halted. [!]");
				if($PTG.allowEchos) echo("\c2>>P\c1T\c4G: \c2WARNING: \c0Exceeded brick limit buffer! Finite routine halted. \c2[!] \c0->" SPC getWord(getDateTime(),1));
				
				$PTG.routine_isHalting = true; //halt finite routines
				//$PTG.dedSrvrFuncCheckTime = getSimTime();
				
				//scheduleNoQuota(0,0,PTG_Routine_Append,%cl,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}
			
			if($PTGm.genType $= "Infinite" && $PTGm.remDistChs) //if infininte terrain and chunk culling are both enabled, pause so bricks can be removed
				return "Pause";
			else
				return "Halt";
		}
	}
	
	return "Continue";
}


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//// APPEND CHUNK GRID FOR CHECKING / GENERATING CHUNKS ////
function PTG_Routine_Append(%cl,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{
	if((%lagCheck = PTG_Routine_Append_LimitsChk(%cl,"Append")) !$= "Continue")//,%CHPosX,%CHPosY,%xmod,%ymod,%clNum))
	{
		if(%lagCheck $= "Pause")
		{
			%delay = mClamp($PTG.delay_PauseTickS,1,30) * 1000;
			$PTG.dedSrvrFuncCheckTime += (%delay + mClamp($PTG.DedSrvrFuncBuffer,20,2000));
			
			scheduleNoQuota(%delay,0,PTG_Routine_Append,%cl,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
		}
		else if(isObject(PTG_MainSO_SPtmp) && PTG_MainSO_SPtmp.presetLoadInProg)
				PTG_LoadServerPreset(getField(PTG_MainSO_SPtmp.srvrPresData,0),getField(PTG_MainSO_SPtmp.srvrPresData,1),getField(PTG_MainSO_SPtmp.srvrPresData,2),getField(PTG_MainSO_SPtmp.srvrPresData,3),getField(PTG_MainSO_SPtmp.srvrPresData,4),"Clear");
			
		return;
	}

	//%delay = //mClamp($PTG.brDelay_remMS,0,50);???

	////////////////////////////////////////////////////////////////////////////////////////////////////
	//INFINTIE / FINITE GEN CONFIGURATION
	
	//If finite terrain is enabled, use grid start / end pos, otherwise reference player(s) pos
	
	//FINITE TERRAIN
	if($PTGm.genType $= "finite")
	{
		%tempCl = %cl;
		
		%startPosX = $PTGm.gridStartX;
		%startPosY = $PTGm.gridStartY;
		%endPosX = $PTGm.gridEndX;
		%endPosY = $PTGm.gridEndY;
	}
	
	//INFINITE TERRAIN
	else
	{
		%clCount = clientgroup.getCount();
		
		//If no clients (i.e. an empty dedicated server), check again in 1 sec
		if(%clCount == 0)
		{
			$PTG.dedSrvrFuncCheckTime = getSimTime();// + 1; //getSimTime() + %delay;z
			scheduleNoQuota(0,0,PTG_Routine_Append,%cl,%CHPosX,%CHPosY,%xmod,%ymod,%clNum); //scheduleNoQuota(%delay,0
			
			return;
		}

		//Prevent issue with exceeding total amount of clients or referencing non-existent client
		if(%clNum < %clCount)
		{
			%tempCL = clientgroup.getObject(%clNum);
			
			if(!isObject(%tempCL) || !isObject(%tempCL.player)) //if client object doesn't exist (shouldn't happen - normally filtered out)
			{
				if(%clNum++ >= %clCount) 
					%clNum = 0;
				
				$PTG.dedSrvrFuncCheckTime = getSimTime();// + %delay;
				scheduleNoQuota(0,0,PTG_Routine_Append,%cl,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
				
				return;
			}
		}
		else //client number overflow (shouldn't happen - should automatically be avoided)
		{
			if(%clNum++ >= %clCount) 
				%clNum = 0;
			
			$PTG.dedSrvrFuncCheckTime = getSimTime();// + %delay;
			scheduleNoQuota(0,0,PTG_Routine_Append,%cl,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			
			return;
		}

		 //always check player's position for each chunk for infinite terrain, to keep generation-grid moving with player
		
		if(isObject(%tmpPl = %tempCL.player))
		{
			%pos = %tmpPl.position;
			%posX = mFloor(getWord(%pos,0) / mClamp($PTGm.chSize,16,256)) * mClamp($PTGm.chSize,16,256);
			%posY = mFloor(getWord(%pos,1) / mClamp($PTGm.chSize,16,256)) * mClamp($PTGm.chSize,16,256);
		}
		else
		{
			if(%clNum++ >= %clCount) //???
				%clNum = 0; //???
			
			$PTG.dedSrvrFuncCheckTime = getSimTime();// + %delay;
			scheduleNoQuota(0,0,PTG_Routine_Append,%cl,%CHPosX,%CHPosY,%xmod,%ymod,%clNum); //???
			
			return; //???
		}
		
		//Chunk radius depending on if temp client is super admin or not
		if(%tempCL.isSuperAdmin)
			%ChRad = mClamp($PTGm.chrad_SA,0,8);
		else
			%ChRad = mClamp($PTGm.chrad_P,0,8);
		
		%ChProx = mClamp($PTGm.chSize,16,256) * %ChRad;
		%startPosX = %posX - %ChProx;
		%startPosY = %posY - %ChProx;
		%endPosX = %posX + %ChProx;
		%endPosY = %posY + %ChProx;
	}
	
	%CHPosX = %startPosX + (mClamp($PTGm.chSize,16,256) * %xmod);
	%CHPosY = %startPosY + (mClamp($PTGm.chSize,16,256) * %ymod);

	////////////////////////////////////////////////////////////////////////////////////////////////////
	//RADIAL GRID GEN
	
	if($PTGm.gridType $= "Radial")
	{
		//Append chunk grid relative to player on X-axis
		if(%CHPosX > %endPosX) //(%posX + (mClamp($PTGm.chSize,16,256) * %ChRad)))
		{
			if($PTGm.genType $= "Infinite")
				%xmod = (%ChRad * -1);
			else
				%xmod = 0;
			
			%CHPosX = %startPosX + (mClamp($PTGm.chSize,16,256) * %xmod); //adjust
			%ymod++;
			
			//Append chunk grid relative to player on Y-axis
			if(%CHPosY > %endPosY)
			{
				if($PTGm.genType $= "Infinite")
					%ymod = (%ChRad * -1);
				else
					%ymod = 0;
				
				%CHPosY = %startPosY + (mClamp($PTGm.chSize,16,256) * %ymod); //adjust
				
				//If finite terrain enabled, end routine
				if($PTGm.genType $= "finite")
				{
					%totalTime = (getSimTime() - $PTG.routine_StartMS) / 1000;
					
					messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 Finite terrain generation complete,<color:00ff00>" SPC %totalTime SPC "\c6seconds elapsed. <color:00ff00>[DONE]");
					if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0Finite terrain generation complete, \c4" @ %totalTime SPC "\c0seconds elapsed. \c4[DONE] \c0->" SPC getWord(getDateTime(),1));

					if($PTG.routine_ProcessAm-- > 0)
					{
						if($PTGm.enabAutoSave)
						{
							messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 AutoSave routine will continue running... \c1[AS]");
							if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0AutoSave routine will continue running... \c4[AS] \c0->" SPC getWord(getDateTime(),1));
						}
						else
						{
							//messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 ???");
							if($PTG.allowEchos) echo(">>\c2P\c1T\c4G \c2WARNING: \c0Slight issue detected with finite routine halt; force-halting. \c2[!] \c0->" SPC getWord(getDateTime(),1));
							
							$PTG.routine_ProcessAm = 0;
							$PTG.routine_Process = "None";
						}
					}						
					else
					{
						$PTG.routine_Process = "None";
						
						if(isObject(PTG_MainSO_SPtmp) && PTG_MainSO_SPtmp.presetLoadInProg)
							PTG_LoadServerPreset(getField(PTG_MainSO_SPtmp.srvrPresData,0),getField(PTG_MainSO_SPtmp.srvrPresData,1),getField(PTG_MainSO_SPtmp.srvrPresData,2),getField(PTG_MainSO_SPtmp.srvrPresData,3),getField(PTG_MainSO_SPtmp.srvrPresData,4),"Clear");
					}

					return;
				}
				
				//Otherwise, if infinite terrain enabled, find next client and continue
				else
				{
					if(%clNum++ >= %clCount)
						%clNum = 0; //!!! what if dedicated server and no clients exist? make sure keeps running and checking until someone joins...
					
					$PTG.dedSrvrFuncCheckTime = getSimTime();// + %delay;
					scheduleNoQuota(0,0,PTG_Routine_Append,%cl,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
					
					return;
				}
			}
		}
		
		//If current chunk is within chunk radius set by host, check if chunk already present at that location
		
		//If infinite (check chunk relative to player chunk-radius)
		if($PTGm.genType $= "Infinite")
		{	
			if(VectorDist(%CHPosX SPC %CHPosY,%startPosX+((%endPosX-%startPosX) / 2) SPC %startPosY+((%endPosY-%startPosY) / 2)) <= %ChProx)
				PTG_Routine_Detect(%cl,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			else
			{
				$PTG.dedSrvrFuncCheckTime = getSimTime();// + %delay;
				scheduleNoQuota(0,0,PTG_Routine_Append,%cl,%CHPosX,%CHPosY,%xmod++,%ymod,%clNum);
			}
		}
		
		//If finite (check chunk relative to grid chunk-radius)
		else
		{
			%getminX = ((%endPosX-%startPosX) / 2);
			%getminY = ((%endPosY-%startPosY) / 2);
			%ChProxAux = getMin(%getMinX,%getMinY); //X or Y axis can be used to find radius - since it's symmetrical

			if(VectorDist(%CHPosX SPC %CHPosY,%startPosX+((%endPosX-%startPosX) / 2) SPC %startPosY+((%endPosY-%startPosY) / 2)) <= %ChProxAux)
				PTG_Routine_Detect(%cl,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			else
			{
				$PTG.dedSrvrFuncCheckTime = getSimTime();// + %delay;
				scheduleNoQuota(0,0,PTG_Routine_Append,%cl,%CHPosX,%CHPosY,%xmod++,%ymod,%clNum);
			}
		}
	}
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	//SQUARE GRID GEN
	
	else
	{
		//Append chunk grid relative to player on X-axis
		if(%CHPosX >= %endPosX)
		{
			%xmod = 0;
			%CHPosX = %startPosX;
			%ymod++;
			%CHPosY = %startPosY + (mClamp($PTGm.chSize,16,256) * %ymod);	
			
			//Append chunk grid relative to player on Y-axis
			if(%CHPosY >= %endPosY)
			{
				%ymod = 0;
				%CHPosY = %startPosY;
				
				//If finite terrain enabled, end routine
				if($PTGm.genType $= "finite")
				{
					%totalTime = (getSimTime() - $PTG.routine_StartMS) / 1000;
					
					messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 Finite terrain generation complete,<color:00ff00>" SPC %totalTime SPC "\c6seconds elapsed. <color:00ff00>[DONE]");
					if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0Finite terrain generation complete, \c4" @ %totalTime SPC "\c0seconds elapsed. \c4[DONE] \c0->" SPC getWord(getDateTime(),1));

					if($PTG.routine_ProcessAm-- > 0)
					{
						if($PTGm.enabAutoSave)
						{
							messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 AutoSave routine will continue running... \c1[AS]");
							if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0AutoSave routine will continue running... \c4[AS] \c0->" SPC getWord(getDateTime(),1));
						}
						else
						{
							//messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G:\c6 ???");
							if($PTG.allowEchos) echo(">>\c2P\c1T\c4G \c2WARNING: \c0Slight issue detected with finite routine halt; force-halting. \c2[!] \c0->" SPC getWord(getDateTime(),1));
							
							$PTG.routine_ProcessAm = 0;
							$PTG.routine_Process = "None";
						}
					}						
					else
					{
						$PTG.routine_Process = "None";
						
						if(isObject(PTG_MainSO_SPtmp) && PTG_MainSO_SPtmp.presetLoadInProg)
							PTG_LoadServerPreset(getField(PTG_MainSO_SPtmp.srvrPresData,0),getField(PTG_MainSO_SPtmp.srvrPresData,1),getField(PTG_MainSO_SPtmp.srvrPresData,2),getField(PTG_MainSO_SPtmp.srvrPresData,3),getField(PTG_MainSO_SPtmp.srvrPresData,4),"Clear");
					}
					
					return;
				}
				
				//Otherwise, if infinite terrain enabled, find next client and continue
				else
				{
					if(%clNum++ >= %clCount) 
						%clNum = 0;
					
					$PTG.dedSrvrFuncCheckTime = getSimTime();// + %delay;
					scheduleNoQuota(0,0,PTG_Routine_Append,%cl,%CHPosX,%CHPosY,%xmod++,%ymod,%clNum);
					
					return;
				}
			}
		}

		PTG_Routine_Detect(%cl,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//// CHECK IF A CHUNK IS ALREADY PRESENT AT A SPECIFIC LOCATION ////
function PTG_Routine_Detect(%cl,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{
	//Function-relative variables
	%ChHalfSize = mClamp($PTGm.chSize,16,256) / 2;
	%tempSSN = "Chunk_" @ %CHPosX @ "_" @ %CHPosY;
	%delay = mClamp($PTG.delay_PauseTickS,1,30) * 1000; //"* 1000"?
	%delay_act = mClamp($PTG.delay_PauseTickS,1,30);
	%delayB = mClamp($PTG.brDelay_remMS,0,50);
	
	//If chunk object doesn't exist, create and generate
	if(!isObject(%tempSSN))
	{
		//Chunk object limit check / pause (pauses to allow culling routine to catch up)
		if(!$PTG.disChBuffer && (BrickGroup_Chunks.getCount() + 1) > (%chLimit = mClamp($PTG.chObjLimit,20,4000)))
		{
			if(%delay > 1)
				%plur = "s";

			if($PTGm.genType $= "Infinite" && $PTGm.remDistChs)
			{
				bottomPrintAll("\c0P\c3T\c1G \c0WARNING: <color:ffffff>Exceeded max of " @ %chLimit @ " total chunk objects; no more chunks can be loaded / generated! Pausing routine for \c6" @ %delay_act @ " \c0second" @ %plur @ "...",5); //use bottomprint encase of multiple server-wide messages
				if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c2WARNING: \c0Exceeded max of \c2" @ %chLimit @ " \c0total chunk objects; no more chunks can be loaded / generated! Pausing routine for \c6" @ %delay_act @ " \c0second" @ %plur @ "... \c2[!] \c0->" SPC getWord(getDateTime(),1));

				$PTG.dedSrvrFuncCheckTime = getSimTime() + %delay;
				scheduleNoQuota(%delay,0,PTG_Routine_Append,%cl,%CHPosX,%CHPosY,%xmod,%ymod,%clNum); //or return to PTG_Routine_Detect function? //%delay * 1000
			}
			else //use chat message for fintie terrain (since routine ends after)?
			{
				bottomPrintAll("\c0P\c3T\c1G \c0WARNING: <color:ffffff>Exceeded max of " @ %chLimit @ " total chunk objects; no more chunks can be loaded / generated! Finite routine halted.",5); //use bottomprint encase of multiple server-wide messages
				if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c2WARNING: \c0Exceeded max of \c2" @ %chLimit @ " \c0total chunk objects; no more chunks can be loaded / generated! Finite routine halted. \c2[!] \c0->" SPC getWord(getDateTime(),1));
				
				$PTG.routine_isHalting = true; //halt finite routines
				
				$PTG.dedSrvrFuncCheckTime = getSimTime();// + %delayB;
				PTG_Routine_Append(%cl,%CHPosX,%CHPosY,%xmod,%ymod,%clNum); //next func delay was used prev
			}
			
			return;
		}
		
		//////////////////////////////////////////////////

		//If no chunk save exists, generate instead
		if(!isFile(PTG_GetFP("Chunk-Norm",%tempSSN,"","")) && !isFile(PTG_GetFP("Chunk-Perm",%tempSSN,"","")))
		{
			if($PTGm.terType !$= "NoTerrain") //If chunk save doesn't exist, use algorithms to generate bricks
			{
				//If under server chunk object-limit, continue
				BrickGroup_Chunks.add(%Chunk = new SimSet(%tempSSN));
					//BrickGroup_Chunks.pushToBack(%Chunk);
				%Chunk.ChUnstablePTG = true;

				//Clear boundary bricks if within area where chunk will be generated / loaded		//Also clears conflicting player bricks???
				%CBposXYZ = %CHPosX+%ChHalfSize SPC %CHPosY+%ChHalfSize SPC mClamp($PTG.zMax,0,4000) / 2;
				%CBsizeXYZ = mClamp($PTGm.chSize,16,256)-0.1 SPC mClamp($PTGm.chSize,16,256)-0.1 SPC mClamp($PTG.zMax,0,4000)-0.1;
				initContainerBoxSearch(%CBposXYZ,%CBsizeXYZ,$TypeMasks::FxBrickObjectType | $Typemasks::StaticObjectType);
				
				//Clear boundary bricks if within area where chunk will be generated / loaded		//Also clears conflicting player bricks???
				while((%obj = containerSearchNext()) && %failSafe++ <= 10000)
					if(isObject(%obj) && %obj.ChBoundsPTG) //%obj.setcolor(0);
						%obj.schedule(%app += %delayB,delete); //just clear boundary bricks, not previous player or loaded bricks...
				
				//%delayC = %delayC + mClamp($PTG.delay_priFuncMS,0,100) + 11;
				%delayC = %app + %delayB + mClamp($PTG.delay_priFuncMS,0,100);
				$PTG.dedSrvrFuncCheckTime = getSimTime() + %delayC;
				scheduleNoQuota(%delayC,0,PTG_Routine_Calc,%cl,"Caves",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum); //%app + 11
			}
			else //if Terrain disabled, then skip and gen empty chunk (i.e. for saving / loading chunks only for freebuilds, or only loading terrain instead of generating)
			{
				$PTG.dedSrvrFuncCheckTime = getSimTime() + %delayB;
				scheduleNoQuota(%delayB,0,PTG_Routine_Append,%cl,%CHPosX,%CHPosY,%xmod++,%ymod,%clNum); //!!! Make sure empty chunk object exsits first !!!
			}
		}
		
		//////////////////////////////////////////////////
		
		//If chunk save does exist, load from save (generate bounds first if enabled)
		else
		{
			//If under server chunk object-limit, continue
			BrickGroup_Chunks.add(%Chunk = new SimSet(%tempSSN));
				//BrickGroup_Chunks.pushToBack(%Chunk);
			%Chunk.ChUnstablePTG = true;

			%CBposXYZ = %CHPosX+%ChHalfSize SPC %CHPosY+%ChHalfSize SPC mClamp($PTG.zMax,0,4000) / 2;
			%CBsizeXYZ = mClamp($PTGm.chSize,16,256)-0.1 SPC mClamp($PTGm.chSize,16,256)-0.1 SPC mClamp($PTG.zMax,0,4000)-0.1;
			initContainerBoxSearch(%CBposXYZ,%CBsizeXYZ,$TypeMasks::FxBrickObjectType | $Typemasks::StaticObjectType);
			
			//Clear boundary bricks if within area where chunk will be generated / loaded		//Also clears conflicting player bricks???
			while((%obj = containerSearchNext()) && %failSafe++ <= 10000)
				if(isObject(%obj) && %obj.ChBoundsPTG)
					%obj.schedule(%app += %delayB,delete); //just clear boundary bricks, not previous player or loaded bricks...
			
			%delayC = %app + %delayB + mClamp($PTG.delay_priFuncMS,0,100);
			$PTG.dedSrvrFuncCheckTime = getSimTime() + %delayC;
			scheduleNoQuota(%delayC,0,PTG_Chunk_ChunkLoad,%cl,%Chunk.getName(),"","",%CHPosX SPC %CHPosY SPC %xmod SPC %ymod SPC %clNum); //%tempSSN //%app + 11
		}
	}
	
	//If chunk already present, return to Append function to look for next chunk
	else
	{
		$PTG.dedSrvrFuncCheckTime = getSimTime() + %delayB;
		scheduleNoQuota(%delayB,0,PTG_Routine_Append,%cl,%CHPosX,%CHPosY,%xmod++,%ymod,%clNum);
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//// CALCULATE STRING HEIGHT-VALUES FOR CHUNK ////
function PTG_Routine_Calc(%cl,%pass,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{
	%delayPri = mClamp($PTG.calcDelay_priFuncMS,0,100);
	%delaySec = mClamp($PTG.calcDelay_secFuncMS,0,100);

	switch$(%pass)
	{
		case "Caves":
		
			if($PTGm.enabCaves && $PTGm.terType !$= "SkyLands")
			{
				$PTG.dedSrvrFuncCheckTime += %delayPri;
				scheduleNoQuota(%delayPri,0,PTG_Noise_3ItrFractal_Caves,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}
			else
			{
				if($PTG.genSpeed == 0)
				{
					$PTG.dedSrvrFuncCheckTime += %delaySec;
					scheduleNoQuota(%delaySec,0,PTG_Routine_Calc,%cl,"CustomBiomeA",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
				}
				else
					PTG_Routine_Calc(%cl,"CustomBiomeA",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}
				
		case "CustomBiomeA":

			if($PTGm.enabBio_CustA)
			{
				$PTG.dedSrvrFuncCheckTime += %delayPri;
				scheduleNoQuota(%delayPri,0,PTG_Noise_Perlin_CustomBiomeA,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}
			else
			{
				if($PTG.genSpeed == 0)
				{
					$PTG.dedSrvrFuncCheckTime += %delaySec;
					scheduleNoQuota(%delaySec,0,PTG_Routine_Calc,%cl,"CustomBiomeB",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
				}
				else
					PTG_Routine_Calc(%cl,"CustomBiomeB",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}

		case "CustomBiomeB":
		
			if($PTGm.enabBio_CustB)
			{
				$PTG.dedSrvrFuncCheckTime += %delayPri;
				scheduleNoQuota(%delayPri,0,PTG_Noise_Perlin_CustomBiomeB,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}
			else
			{
				if($PTG.genSpeed == 0)
				{
					$PTG.dedSrvrFuncCheckTime += %delaySec;
					scheduleNoQuota(%delaySec,0,PTG_Routine_Calc,%cl,"CustomBiomeC",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
				}
				else
					PTG_Routine_Calc(%cl,"CustomBiomeC",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}

		case "CustomBiomeC":
		
			if($PTGm.enabBio_CustC)
			{
				$PTG.dedSrvrFuncCheckTime += %delayPri;
				scheduleNoQuota(%delayPri,0,PTG_Noise_Perlin_CustomBiomeC,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}
			else
			{
				if($PTG.genSpeed == 0)
				{
					$PTG.dedSrvrFuncCheckTime += %delaySec;
					scheduleNoQuota(%delaySec,0,PTG_Routine_Calc,%cl,"Clouds",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
				}
				else
					PTG_Routine_Calc(%cl,"Clouds",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}

		case "Clouds":
		
			if($PTGm.enabClouds)
			{
				$PTG.dedSrvrFuncCheckTime += %delayPri;
				scheduleNoQuota(%delayPri,0,PTG_Noise_2ItrFractal_Clouds,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}
			else
			{
				if($PTG.genSpeed == 0)
				{
					$PTG.dedSrvrFuncCheckTime += %delaySec;
					scheduleNoQuota(%delaySec,0,PTG_Routine_Calc,%cl,"Terrain",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
				}
				else
					PTG_Routine_Calc(%cl,"Terrain",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}

		case "Terrain":
		
			switch$($PTGm.terType)
			{
				case "Normal":
				
					$PTG.dedSrvrFuncCheckTime += %delayPri;
					scheduleNoQuota(%delayPri,0,PTG_Noise_3ItrFractal_Terrain,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
					
				case "SkyLands": //calculate SkyLands in separate case below, but calculate normal terrain here first
				
					$PTG.dedSrvrFuncCheckTime += %delayPri;
					scheduleNoQuota(%delayPri,0,PTG_Noise_3ItrFractal_Terrain,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
					
				case "FlatLands":
				
					$PTG.dedSrvrFuncCheckTime += %delayPri;
					scheduleNoQuota(%delayPri,0,PTG_Noise_FlatLands,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum); //doesn't require noise calc
					
				default: //speed check not necessary here

					$PTG.dedSrvrFuncCheckTime += %delaySec;
					scheduleNoQuota(%delaySec,0,PTG_Routine_Calc,%cl,"Mountains",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}
		
		case "Mountains":
		
			if($PTGm.enabMntns)
			{
				$PTG.dedSrvrFuncCheckTime += %delayPri;
				scheduleNoQuota(%delayPri,0,PTG_Noise_2ItrFractal_Mntns,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}
			else
			{
				if($PTG.genSpeed == 0)
				{
					$PTG.dedSrvrFuncCheckTime += %delaySec;
					scheduleNoQuota(%delaySec,0,PTG_Routine_Calc,%cl,"SkyLands",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
				}
				else
					PTG_Routine_Calc(%cl,"SkyLands",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}
		
		case "SkyLands": //Skylands terrain type requires Mountains to be calc first, so routine calc adjusted...
		
			if($PTGm.terType $= "SkyLands")
			{
				$PTG.dedSrvrFuncCheckTime += %delayPri;
				scheduleNoQuota(%delayPri,0,PTG_Noise_Perlin_SkyLands,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}
			else
			{
				if($PTG.genSpeed == 0)
				{
					$PTG.dedSrvrFuncCheckTime += %delaySec;
					scheduleNoQuota(%delaySec,0,PTG_Routine_Calc,%cl,"FltIslds",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
				}
				else
					PTG_Routine_Calc(%cl,"FltIslds",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}

		case "FltIslds":
		
			if($PTGm.enabFltIslds)
			{
				$PTG.dedSrvrFuncCheckTime += %delayPri;
				scheduleNoQuota(%delayPri,0,PTG_Noise_2ItrFractal_FltIslds,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}
			else
			{
				if($PTG.genSpeed == 0)
				{
					$PTG.dedSrvrFuncCheckTime += %delaySec;
					scheduleNoQuota(%delaySec,0,PTG_Routine_Calc,%cl,"BuildLoading",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
				}
				else
					PTG_Routine_Calc(%cl,"BuildLoading",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}

		case "BuildLoading":
		
			if($PTGm.enabBuildLoad || $PTGm.allowFlatAreas)
			{
				if($PTGm.seamlessBuildL)
				{
					$PTG.dedSrvrFuncCheckTime += (%delayPri * 4);

					scheduleNoQuota(0,0,PTG_BuildLoadCheck,%cl,%Chunk,%CHPosX-$PTGm.chSize,%CHPosY,%xmod,%ymod,%clNum,true,-$PTGm.chSize,0);
					scheduleNoQuota(%delayPri,0,PTG_BuildLoadCheck,%cl,%Chunk,%CHPosX+$PTGm.chSize,%CHPosY,%xmod,%ymod,%clNum,true,$PTGm.chSize,0);
					scheduleNoQuota(%delayPri * 2,0,PTG_BuildLoadCheck,%cl,%Chunk,%CHPosX,%CHPosY-$PTGm.chSize,%xmod,%ymod,%clNum,true,0,-$PTGm.chSize);
					scheduleNoQuota(%delayPri * 3,0,PTG_BuildLoadCheck,%cl,%Chunk,%CHPosX,%CHPosY+$PTGm.chSize,%xmod,%ymod,%clNum,true,0,$PTGm.chSize);
				
					scheduleNoQuota(%delayPri * 4,0,PTG_BuildLoadCheck,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,false,0,0);
				}
				else
				{
					$PTG.dedSrvrFuncCheckTime += %delayPri ;
					scheduleNoQuota(%delayPri,0,PTG_BuildLoadCheck,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,false,0,0);
				}
			}
			else
			{
				if($PTG.genSpeed == 0)
				{
					$PTG.dedSrvrFuncCheckTime += %delaySec;
					scheduleNoQuota(%delaySec,0,PTG_Routine_Calc,%cl,"Final",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
				}
				else
					PTG_Routine_Calc(%cl,"Final",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}

		case "Final":

			if($PTG.publicBricks || $PTG.lastClientID == 0 || !isObject("BrickGroup_" @ $PTG.lastClientID)) //Setup brickgroup for bricks then continue to relay function
				%BG = "BrickGroup_888888"; //if public bricks enabled
			else
				%BG = "BrickGroup_" @ $PTG.lastClientID;
			
			%delay = mClamp($PTG.delay_priFuncMS,0,100);
			$PTG.dedSrvrFuncCheckTime = getSimTime() + %delay;
			scheduleNoQuota(%delay,0,PTG_Chunk_Gen_Relay,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"Terrain",%BG);
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//// REMOVE DISTANT CHUNKS ////
function PTG_Routine_ChunkCull(%cl,%CHcount,%CHmax)
{
	if($PTG.routine_isHalting) //Check if routine halted
	{
		if($PTG.routine_ProcessAm-- <= 0)
		{
			messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6Routine halted successfully. \c0[X]");
			if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0Routine halted successfully. \c4[X] \c0->" SPC getWord(getDateTime(),1));
			
			$PTG.routine_Process = "None";
			$PTG.routine_isHalting = false;
			
			if(isObject(PTG_MainSO_SPtmp) && PTG_MainSO_SPtmp.presetLoadInProg)
				PTG_LoadServerPreset(getField(PTG_MainSO_SPtmp.srvrPresData,0),getField(PTG_MainSO_SPtmp.srvrPresData,1),getField(PTG_MainSO_SPtmp.srvrPresData,2),getField(PTG_MainSO_SPtmp.srvrPresData,3),getField(PTG_MainSO_SPtmp.srvrPresData,4),"Clear");
		}
		
		return;
	}
	
	//Make sure chunk-count doesn't exceed actual, total amount of chunks present
	if(%CHcount >= %CHmax) //(%CHTotalC = BrickGroup_Chunks.getCount()))
	{
		scheduleNoQuota(0,0,PTG_Routine_ChunkCull,%cl,0,BrickGroup_Chunks.getCount());
		return;
	}
	else
		%tempSS = BrickGroup_Chunks.getObject(%CHcount);
	
	//Check if chunk exists and isn't currently being generated / saved / removed
	if(!isObject(%tempSS) || %tempSS.ChUnstablePTG || %tempSS.ChStaticPTG)
	{
		scheduleNoQuota(0,0,PTG_Routine_ChunkCull,%cl,%CHcount++,%CHmax);
		return;
	}
	
	
	//Function-relative variables (continued if chunk exists)
	%tempSSN = strReplace(%tempSS.getName(),"_"," ");
	%x = getWord(%tempSSN,1);
	%y = getWord(%tempSSN,2);
	
	//Check if chunk is within set radius for at least one player
	%CLcount = clientGroup.getCount();

	for(%c = 0; %c < %CLcount; %c++)
	{
		%tempCl = clientgroup.getobject(%c);
		
		if(isObject(%tmpPl = %tempCL.player))
		{
			%ChSize = mClamp($PTGm.chSize,16,256);
			%ChRad_SA = mClamp($PTGm.chrad_SA,0,8);
			%ChRad_NP = mClamp($PTGm.chrad_P,0,8);
			
			%pos = %tmpPl.position;	
			%posX = mFloor(getWord(%pos,0) / %ChSize) * %ChSize;
			%posY = mFloor(getWord(%pos,1) / %ChSize) * %ChSize;
		
			if(%tempCL.isSuperAdmin)
				%ChRad = %ChRad_SA;
			else
				%ChRad = %ChRad_NP;
			
			%relSzeSA = (%ChSize * %ChRad);
			
			if($PTGm.gridType $= "Radial")	//Radial Grid
			{
				if(VectorDist(%x SPC %y,%posX SPC %posY) > %relSzeSA)
					%proxyCount++;
			}
			else							//Square Grid
			{
				if(mAbs(%posX - %x) > %relSzeSA || mAbs(%posY - %y) > %relSzeSA)
					%proxyCount++;
			}
		}
	}
	
	//Check if chunk is outside set radius for all players, remove chunk (if not set to static)
	if(!%tempSS.ChStaticPTG)
	{
		if(%proxyCount >= %CLcount) //on dedicated servers with no players, this will also clear all chunks until someone joins //">=" is a failsafe, only requires "=="
		{
			%Chunk = %tempSS.getName();
			%VARS = %CHcount;// SPC %CHtotalC;
			
			if($PTGm.enabBounds)
				%nextFunc = "Bounds";
			else
				%nextFunc = "Cull";
			
			
			if($PTG.chSaveMethod !$= "Never")
			{
				if($PTG.chSaveMethod $= "IfEdited")
				{
					if(%chunk.ChEditedPTG)
						PTG_Chunk_SaveRemoveBricks(%cl,%chunk,%chunk.getCount(),0,"",true,%nextFunc,%VARS);
					else
						PTG_Chunk_RemoveBricks(%cl,%chunk,%chunk.getCount(),0,%nextFunc,%VARS);
				}
				else
					PTG_Chunk_SaveRemoveBricks(%cl,%chunk,%chunk.getCount(),0,"",true,%nextFunc,%VARS);
			}
			else
				PTG_Chunk_RemoveBricks(%cl,%chunk,%chunk.getCount(),0,%nextFunc,%VARS);

			return;
		}
	}

	//%CHcount++ here and check for exceeding count of chunks in group? (like above in start of function)
	scheduleNoQuota(mClamp($PTG.delay_secFuncMS,0,100),0,PTG_Routine_ChunkCull,%cl,%CHcount++,%CHmax); //subfunc or nextfunc delay here?
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//// AUTOSAVE CHUNKS ////
function PTG_Routine_AutoSaveChunks(%cl,%CHcount,%CHTotalC,%incr,%saveStart)
{
	if($PTG.routine_isHalting) //Check if routine halted
	{
		if($PTG.routine_ProcessAm-- <= 0)
		{
			messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6Routine halted successfully. \c0[X]");
			if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0Routine halted successfully. \c4[X] \c0->" SPC getWord(getDateTime(),1));
			
			$PTG.routine_Process = "None";
			$PTG.routine_isHalting = false;
			
			if(isObject(PTG_MainSO_SPtmp) && PTG_MainSO_SPtmp.presetLoadInProg)
				PTG_LoadServerPreset(getField(PTG_MainSO_SPtmp.srvrPresData,0),getField(PTG_MainSO_SPtmp.srvrPresData,1),getField(PTG_MainSO_SPtmp.srvrPresData,2),getField(PTG_MainSO_SPtmp.srvrPresData,3),getField(PTG_MainSO_SPtmp.srvrPresData,4),"Clear");
		}
		
		return;
	}
	
	%delay = (mClamp($PTG.schedM_autosave,1,60) * 60);
	
	//Autosave tick = 1 sec; autosave schedule set by host / superadmin
	if(%incr++ <= %delay)
	{
		scheduleNoQuota(1000,0,PTG_Routine_AutoSaveChunks,%cl,0,BrickGroup_Chunks.getCount(),%incr,%saveStart);
		return;
	}
	else if(%incr >= %delay && %saveStart) //message everyone in chat once per schedule that chunks are being saved
	{
		messageAll('',"<font:Verdana Bold:" @ $PTG.fontSize @ ">\c0P\c3T\c1G: \c6AutoSaving Chunks... \c1[AS]");
		if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c0AutoSaving Chunks... \c4[AS] \c0->" SPC getWord(getDateTime(),1));
	}
	
	//Make sure chunk-count doesn't exceed total amount of chunks present, otherwise reset schedule
	if(%CHcount >= %CHTotalC)
	{
		scheduleNoQuota(0,0,PTG_Routine_AutoSaveChunks,%cl,%CHcount++,%CHtotalC,0,true);
		return;
	}
	else
	{
		%tempSS = BrickGroup_Chunks.getObject(%CHcount);
		//%saveStart = false;
	}
	
	//Check if chunk exists and isn't currently being generated / saved / removed
	if(!isObject(%tempSS) || %tempSS.ChUnstablePTG)
	{
		scheduleNoQuota(0,0,PTG_Routine_AutoSaveChunks,%cl,%CHcount++,%CHtotalC,%inc,false);
		return;
	}
	
	//////////////////////////////////////////////////
	
	//Save chunk if it meets certain conditions, or continue checking.
	if($PTG.chSaveMethod !$= "Never") //(don't run auto-save routine if "never" selected???
	{
		%Chunk = %tempSS.getName();
		%VARS = %CHcount SPC %CHtotalC SPC %incr;
		
		if($PTG.chSaveMethod $= "IfEdited")
		{
			if(%Chunk.ChEditedPTG)
			{
				scheduleNoQuota(0,0,PTG_Chunk_SaveRemoveBricks,%cl,%Chunk,%Chunk.getCount(),0,"",false,"AutoSave",%VARS);
				return;
			}
		}
		else
		{
			scheduleNoQuota(0,0,PTG_Chunk_SaveRemoveBricks,%cl,%Chunk,%Chunk.getCount(),0,"",false,"AutoSave",%VARS);
			return;
		}
	}
		
	scheduleNoQuota(mClamp($PTG.delay_secFuncMS,0,100),0,PTG_Routine_AutoSaveChunks,%cl,%CHcount++,%CHtotalC,%incr,false);
}