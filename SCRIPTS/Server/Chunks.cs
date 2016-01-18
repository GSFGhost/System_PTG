function PTG_Chunk_Gen_Relay(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%function,%BG)
{
	%delayPri = mClamp($PTG.delay_priFuncMS,0,100);
	%delaySec = mClamp($PTG.delay_secFuncMS,0,100);
	%BrXYhSize = $PTGm.brTer_XYsize / 2;

	switch$(%function)
	{
		//Generate Terrain
		case "Terrain":
		
			switch$($PTGm.terType)
			{
				case "Normal" or "FlatLands":
				
					$PTG.dedSrvrFuncCheckTime += %delayPri;
					scheduleNoQuota(%delayPri,0,PTG_Chunk_Gen_Terrain_Normal,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrXYhSize,%BrXYhSize);
					
				case "SkyLands":
				
					$PTG.dedSrvrFuncCheckTime += %delayPri;
					scheduleNoQuota(%delayPri,0,PTG_Chunk_Gen_Terrain_SkyLands,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrXYhSize,%BrXYhSize);
					
				default:
				
					$PTG.dedSrvrFuncCheckTime += %delaySec;
					scheduleNoQuota(%delaySec,0,PTG_Chunk_Gen_Relay,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"Caves",%BG);
			}

		//Generate Caves
		case "Caves":
		
			if($PTGm.enabCaves && $PTGm.terType !$= "SkyLands")
			{
				//if($PTGm.terType $= "SkyLands") //caves for Skylands is disabled for this version
				//	scheduleNoQuota(mClamp($PTG.delay_priFuncMS,0,100),0,PTG_Chunk_Gen_Caves_SkyLands,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrXYhSize,%BrXYhSize);
				//else
					$PTG.dedSrvrFuncCheckTime += %delayPri;
					scheduleNoQuota(%delayPri,0,PTG_Chunk_Gen_Caves_Normal,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrXYhSize,%BrXYhSize);
			}
			else 
			{
				$PTG.dedSrvrFuncCheckTime += %delaySec;
				scheduleNoQuota(%delaySec,0,PTG_Chunk_Gen_Relay,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"FltIslds",%BG);
			}

		//Generate Floating Islands
		case "FltIslds":
		
			%BrXYhSize = $PTGm.brFltIslds_XYsize / 2;
		
			if($PTGm.enabFltIslds)
			{
				$PTG.dedSrvrFuncCheckTime += %delayPri;
				scheduleNoQuota(%delayPri,0,PTG_Chunk_Gen_FltIslds,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrXYhSize,%BrXYhSize);
			}
			else
			{
				$PTG.dedSrvrFuncCheckTime += %delaySec;
				scheduleNoQuota(%delaySec,0,PTG_Chunk_Gen_Relay,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"Lakes",%BG);
			}
		
		//Generate Lakes
		case "Lakes":
		
			if($PTGm.lakesHLevel > 0 && $PTGm.terType !$= "SkyLands" && !$PTGm.DisableWatGen)
			{
				$PTG.dedSrvrFuncCheckTime += %delayPri;
				scheduleNoQuota(%delayPri,0,PTG_Chunk_Gen_Lakes,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,8,8);
			}
			else
			{
				$PTG.dedSrvrFuncCheckTime += %delaySec;
				scheduleNoQuota(%delaySec,0,PTG_Chunk_Gen_Relay,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"Details",%BG);
			}
		
		//Generate Details
		case "Details":
		
			if($PTGm.enabDetails)
			{
				switch$($PTGm.terType)
				{
					case "Normal" or "FlatLands":
					
						$PTG.dedSrvrFuncCheckTime += %delayPri;
						scheduleNoQuota(%delayPri,0,PTG_Chunk_Gen_Details_Normal,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrXYhSize,%BrXYhSize,0);
						
					case "SkyLands":
					
						$PTG.dedSrvrFuncCheckTime += %delayPri;
						scheduleNoQuota(%delayPri,0,PTG_Chunk_Gen_Details_SkyLands,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrXYhSize,%BrXYhSize,0);
						
					default:
					
						$PTG.dedSrvrFuncCheckTime += %delaySec;
						scheduleNoQuota(%delaySec,0,PTG_Chunk_Gen_Relay,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"FltIsldsDetails",%BG);
				}
			}
			else
			{
				$PTG.dedSrvrFuncCheckTime += %delaySec;
				scheduleNoQuota(%delaySec,0,PTG_Chunk_Gen_Relay,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"FltIsldsDetails",%BG);
			}
		
		//Generate Details for Floating Islands (based on terrain details)
		case "FltIsldsDetails":

			%BrXYhSize = $PTGm.brFltIslds_XYsize / 2;
			
			if($PTGm.enabDetails && $PTGm.enabFltIslds && $PTGm.enabFltIsldDetails)
			{
				$PTG.dedSrvrFuncCheckTime += %delayPri;
				scheduleNoQuota(%delayPri,0,PTG_Chunk_Gen_Details_FltIslds,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrXYhSize,%BrXYhSize,0); //doesn't exist yet
			}
			else
			{
				$PTG.dedSrvrFuncCheckTime += %delaySec;
				scheduleNoQuota(%delaySec,0,PTG_Chunk_Gen_Relay,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"Clouds",%BG);
			}
		
		//Generate Clouds
		case "Clouds":

			%BrXYhSize = $PTGm.brClouds_XYsize / 2;
			
			if($PTGm.enabClouds)
			{
				$PTG.dedSrvrFuncCheckTime += %delayPri;
				scheduleNoQuota(%delayPri,0,PTG_Chunk_Gen_Clouds,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrXYhSize,%BrXYhSize);
			}
			else
			{
				$PTG.dedSrvrFuncCheckTime += %delaySec;
				scheduleNoQuota(%delaySec,0,PTG_Chunk_Gen_Relay,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"Builds",%BG);
			}

		//Generate Builds
		case "Builds":

			if($PTGm.enabBuildLoad && strReplace(getField($StrArrayData_Builds,2)," ","") !$= "")
			{
				$PTG.dedSrvrFuncCheckTime += %delayPri;
				scheduleNoQuota(%delayPri,0,PTG_Chunk_BuildLoad,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,"","");
			}
			else
			{
				$PTG.dedSrvrFuncCheckTime += %delaySec;
				scheduleNoQuota(%delaySec,0,PTG_Chunk_Gen_Relay,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"Bounds",%BG);
			}

		//Generate Boundaries (gen last to allow highest chunk Z pos to be calculated by other funcs above)
		case "Bounds":

			if($PTGm.enabBounds) // && $PTGm.genType $= "Finite")
			{
				$PTG.dedSrvrFuncCheckTime += %delayPri;
				scheduleNoQuota(%delayPri,0,PTG_Chunk_Gen_Boundaries,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,"Relay");
			}
			else
			{
				$PTG.dedSrvrFuncCheckTime += %delaySec;
				scheduleNoQuota(%delaySec,0,PTG_Chunk_Gen_Relay,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"Final",%BG);
			}
		
		//Finalize(clear global string arrays and tell chunk culling routine that gen is complete), Next Function
		case "Final":

			%Chunk.ChUnstablePTG = false;
			%Chunk.PTGChSize = mClamp($PTGm.chSize,16,256);
			
			//If chunks are highlighted, add new highlight object for newly created chunk
			%tmpChZ = %Chunk.PTGHighestZpos = mCeil((%Chunk.PTGHighestZpos + 16) / 32) * 32;
			if(isObject(BrickGroup_HighlightChunks))
			{
				//If highest point isn't stored in chunk (i.e. if created for planted / loaded brick), run through all objects in chunk and find highest pos
				if(%tmpChZ == 0)
				{
					for(%c = 0; %c < %Chunk.getCount(); %c++)
					{
						%tmpObj = %Chunk.getObject(%c);
						%tmpObjPosZ = getWord(%tmpObj.getPosition(),2);
						
						//Update highlighted chunk color (if revealed)
						if(%tmpObj.getClassName() $= "fxDTSBrick" && %tmpObjPosZ > %tmpChZ)
						{
							%tmpChZ = %tmpObjPosZ + ((%tmpObjPosZ.brickSizeZ * 0.2) * 0.5);
							%tmpChZ = mCeil((%tmpChZ + 16) / 32) * 32;
						}
					}
				}

				%newChZ = mCeil(%tmpChZ / 32);
				%pos = %CHPosX+(mClamp($PTGm.chSize,16,256) / 2) SPC %CHPosY+(mClamp($PTGm.chSize,16,256) / 2) SPC ((%newChZ / 2) * 32);//$PTGm.boundsHLevel+2;
				%scale = (mClamp($PTGm.chSize,16,256) / 16) SPC (mClamp($PTGm.chSize,16,256) / 16) SPC %newChZ;
				
				//Spawn static shape (add to chunk itself) and add to highlight chunk brickgroup
				%tmpStcObj = PTG_Chunk_SpawnStatic(%Chunk,%pos,%scale,"HL-NonStatic");
				BrickGroup_HighlightChunks.add(%tmpStcObj);
				%tmpStcObj.setName(strReplace(%Chunk.getName(),"Chunk","HLChunk")); //already displays name???
			}
			
			
			//Landscape height value arrays
			deleteVariables("$StrArrayHV_Terrain*");
			//deleteVariables("$StrArrayHV_SkyLands*");
			deleteVariables("$StrArrayHV_SkyLandsTop*");
			deleteVariables("$StrArrayHV_SkyLandsBtm*");
			deleteVariables("$StrArrayHV_CavesA*");
			deleteVariables("$StrArrayHV_CavesB*");
			deleteVariables("$StrArrayHV_CustomBiomeA*");
			deleteVariables("$StrArrayHV_CustomBiomeB*");
			deleteVariables("$StrArrayHV_CustomBiomeC*");
			deleteVariables("$StrArrayHV_Clouds*");
			deleteVariables("$StrArrayHV_Mountains*");
			deleteVariables("$StrArrayHV_FltIsldsA*");
			deleteVariables("$StrArrayHV_FltIsldsB*");
			
			//Build loading height value arrays
			deleteVariables("$StrArrayHV_MountainsAux*");
			deleteVariables("$StrArrayHV_FltIsldsATop*");
			deleteVariables("$StrArrayHV_FltIsldsBTop*");
			deleteVariables("$StrArrayHV_FltIsldsABtm*");
			deleteVariables("$StrArrayHV_FltIsldsBBtm*");
			//deleteVariables("$StrArrayHV_FltIsldsAaux*");
			//deleteVariables("$StrArrayHV_FltIsldsBaux*");
			//deleteVariables("$StrArrayHV_SkylandsAux*");
			deleteVariables("$StrArrayData_Builds");
			
			
			$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.delay_priFuncMS,0,100)); //= getSimTime() +

			//Gradual or Entire grid generation per player (reset chunk pos on grid - for next player - or append next chunk for gradual loading?)
			if($PTGm.genMethod $= "Complete")
				scheduleNoQuota(%delay,0,PTG_Routine_Append,%cl,%CHPosX,%CHPosY,%xmod++,%ymod,%clNum);
			
			else
			{
				if(%clNum++ >= clientgroup.getCount())
					%clNum = 0; //Make sure client exists? Or allow append function to check?
				
				scheduleNoQuota(%delay,0,PTG_Routine_Append,%cl,%CHPosX,%CHPosY,0,0,%clNum);
			}
	}
}


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////// LANDSCAPE GENERATION ////////


function PTG_Chunk_Gen_Terrain_Normal(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY)
{
	%BrH_init = $StrArrayHV_Terrain[%BrPosX,%BrPosY];	//make sure to start below generated terrain brick
	%BrH_cf = $StrArrayHV_Mountains[%BrPosX,%BrPosY];
	if((%BrH_cf_aux = $StrArrayHV_MountainsAux[%BrPosX,%BrPosY]) == 0) //mountainsAux for colors/prints only?
		%BrH_cf_aux = %BrH_cf;
	%CaveH_btm = $StrArrayHV_CavesA[%BrPosX,%BrPosY];
	%CaveH_top = $StrArrayHV_CavesB[%BrPosX,%BrPosY];
	%BrH_fnl = %BrH_cf;
	%ChSize = mClamp($PTGm.chSize,16,256);
	
	%BrZSize = $PTGm.brTer_Zsize;
	%BrZhSize = $PTGm.brTer_Zsize / 2;
	%BrXYSize = $PTGm.brTer_XYsize;
	%BrXYhSize = %BrXYSize / 2;
	%FillBrXYZSize = $PTGm.brTer_FillXYZSize;

	if(%FillBrXYZSize < 1 || $PTGm.enabModTer)
		%genType = "PlateFill";
	else
		%genType = "CubeFill";
	
	//////////////////////////////////////////////////

	%mntnLm = $StrArrayHV_Mountains[%BrPosX-$PTGm.brTer_XYsize,%BrPosY];
	%mntnRm = $StrArrayHV_Mountains[%BrPosX+$PTGm.brTer_XYsize,%BrPosY];
	%mntnDm = $StrArrayHV_Mountains[%BrPosX,%BrPosY-$PTGm.brTer_XYsize];
	%mntnUm = $StrArrayHV_Mountains[%BrPosX,%BrPosY+$PTGm.brTer_XYsize];
	
	%CaveH_btm_LU = $StrArrayHV_CavesA[%BrPosX-$PTGm.brTer_XYsize,%BrPosY+$PTGm.brTer_XYsize];
	%CaveH_btm_RU = $StrArrayHV_CavesA[%BrPosX+$PTGm.brTer_XYsize,%BrPosY+$PTGm.brTer_XYsize];
	%CaveH_btm_LD = $StrArrayHV_CavesA[%BrPosX-$PTGm.brTer_XYsize,%BrPosY-$PTGm.brTer_XYsize];
	%CaveH_btm_RD = $StrArrayHV_CavesA[%BrPosX+$PTGm.brTer_XYsize,%BrPosY-$PTGm.brTer_XYsize];
	
	%CaveH_top_LU = $StrArrayHV_CavesB[%BrPosX-$PTGm.brTer_XYsize,%BrPosY+$PTGm.brTer_XYsize];
	%CaveH_top_RU = $StrArrayHV_CavesB[%BrPosX+$PTGm.brTer_XYsize,%BrPosY+$PTGm.brTer_XYsize];
	%CaveH_top_LD = $StrArrayHV_CavesB[%BrPosX-$PTGm.brTer_XYsize,%BrPosY-$PTGm.brTer_XYsize];
	%CaveH_top_RD = $StrArrayHV_CavesB[%BrPosX+$PTGm.brTer_XYsize,%BrPosY-$PTGm.brTer_XYsize];
	
	%CaveH_btm_Ltmp = %CaveH_btm_L = $StrArrayHV_CavesA[%BrPosX-$PTGm.brTer_XYsize,%BrPosY];
	%CaveH_btm_Rtmp = %CaveH_btm_R = $StrArrayHV_CavesA[%BrPosX+$PTGm.brTer_XYsize,%BrPosY];
	%CaveH_btm_Dtmp = %CaveH_btm_D = $StrArrayHV_CavesA[%BrPosX,%BrPosY-$PTGm.brTer_XYsize];
	%CaveH_btm_Utmp = %CaveH_btm_U = $StrArrayHV_CavesA[%BrPosX,%BrPosY+$PTGm.brTer_XYsize];
	
	if(%CaveH_btm_L > %mntnLm || %CaveH_btm_L <= 0) %CaveH_btm_L = %mntnLm;
	if(%CaveH_btm_R > %mntnRm || %CaveH_btm_R <= 0) %CaveH_btm_R = %mntnRm;
	if(%CaveH_btm_D > %mntnDm || %CaveH_btm_D <= 0) %CaveH_btm_D = %mntnDm;
	if(%CaveH_btm_U > %mntnUm || %CaveH_btm_U <= 0) %CaveH_btm_U = %mntnUm;
	
	%CaveH_top_L = $StrArrayHV_CavesB[%BrPosX-$PTGm.brTer_XYsize,%BrPosY];
	%CaveH_top_R = $StrArrayHV_CavesB[%BrPosX+$PTGm.brTer_XYsize,%BrPosY];
	%CaveH_top_D = $StrArrayHV_CavesB[%BrPosX,%BrPosY-$PTGm.brTer_XYsize];
	%CaveH_top_U = $StrArrayHV_CavesB[%BrPosX,%BrPosY+$PTGm.brTer_XYsize];
	
	if((%CaveH_top_L+%BrZSize) > (%mntnLm-%BrZSize) && %CaveH_btm_L <= (%mntnLm-%BrZSize)) %mntnLm = %CaveH_btm_L;
	if((%CaveH_top_R+%BrZSize) > (%mntnRm-%BrZSize) && %CaveH_btm_R <= (%mntnRm-%BrZSize)) %mntnRm = %CaveH_btm_R;
	if((%CaveH_top_D+%BrZSize) > (%mntnDm-%BrZSize) && %CaveH_btm_D <= (%mntnDm-%BrZSize)) %mntnDm = %CaveH_btm_D;
	if((%CaveH_top_U+%BrZSize) > (%mntnUm-%BrZSize) && %CaveH_btm_U <= (%mntnUm-%BrZSize)) %mntnUm = %CaveH_btm_U;

	%minAdjTer = getMin(%mntnLm,getMin(%mntnRm,getMin(%mntnDm,%mntnUm))) - %BrZSize;
	%maxAdjCave = getMax(%CaveH_top_L,getMax(%CaveH_top_R,getMax(%CaveH_top_D,%CaveH_top_U)));
	%minAdjCave_btm = getMin(%CaveH_btm_L,getMin(%CaveH_btm_R,getMin(%CaveH_btm_D,%CaveH_btm_U)));

	%caveTerPass = !$PTGm.enabCaves || %CaveH_top == 0 || mFloatLength(%CaveH_top+%BrZSize,1) <= mFloatLength(%BrH_fnl-%BrZSize,1) || %CaveH_btm > (%BrH_fnl-%BrZSize);
	%caveAdjCheck = (((%CaveH_top_L+%BrZSize) > (%mntnLm-%BrZSize)) && (mFloatLength(%CaveH_btm_Ltmp+%BrZSize,1) <= mFloatLength(%mntnLm-%BrZSize,1))) || 
					(((%CaveH_top_R+%BrZSize) > (%mntnRm-%BrZSize)) && (mFloatLength(%CaveH_btm_Rtmp+%BrZSize,1) <= mFloatLength(%mntnRm-%BrZSize,1))) || 
					(((%CaveH_top_D+%BrZSize) > (%mntnDm-%BrZSize)) && (mFloatLength(%CaveH_btm_Dtmp+%BrZSize,1) <= mFloatLength(%mntnDm-%BrZSize,1))) || 
					(((%CaveH_top_U+%BrZSize) > (%mntnUm-%BrZSize)) && (mFloatLength(%CaveH_btm_Utmp+%BrZSize,1) <= mFloatLength(%mntnUm-%BrZSize,1))); //mFloatLength is necessary to fix issue with engine and "<=" condition
	%caveTerGapCheck = !$PTGm.enabCaves || %CaveH_top == 0 || %CaveH_btm > (%BrH_fnl - %BrZSize); // || %CaveH_btm <= (%minAdjTer-%FillBrXYZSize);
	%CaveTerFillPass = (%maxAdjCave + %BrZSize + %FillBrXYZSize) <= (%minAdjTer - %FillBrXYZSize);
	%adjTerGap = (%BrH_fnl - %BrZSize) > (%minAdjTer + %BrZSize);
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	if(%caveTerPass)
	{
		//Set terrain main brick to color / print for terrain or dirt, depending if place-capping is enabled
		if($PTGm.enabPlateCap && (!$PTGm.enabModTer || $PTGm.modTerGenType $= "Cubes"))
			%colPriType = "Dirt";
		else
			%colPriType = "Terrain";
		
		//Main Terrain Brick
		%tmpColPri = PTG_Chunk_FigureColPri(%BrH_init,%BrH_cf_aux,%BrH_fnl,%colPriType,%BrPosX,%BrPosY);
		%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %BrH_fnl-%BrZhSize;
		PTG_Chunk_PlantBrick($PTGm.brTer_DB,%pos,getWord(%tmpColPri,0),0,0,0,getWord(%tmpColPri,1),%cl,%BG,%Chunk,"TerrainBr","");
		
		//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
		if(isObject(%Chunk) && ((%tempZMax = %BrH_fnl) > %Chunk.PTGHighestZpos))
			%Chunk.PTGHighestZpos = %tempZMax;
		
		
		//Modular Terrain
		if($PTGm.enabModTer)
		{
			%mntnLUm = $StrArrayHV_Mountains[%BrPosX-%BrXYSize,%BrPosY+%BrXYSize];
			%mntnRUm = $StrArrayHV_Mountains[%BrPosX+%BrXYSize,%BrPosY+%BrXYSize];
			%mntnLDm = $StrArrayHV_Mountains[%BrPosX-%BrXYSize,%BrPosY-%BrXYSize];
			%mntnRDm = $StrArrayHV_Mountains[%BrPosX+%BrXYSize,%BrPosY-%BrXYSize];
			
			if((%CaveH_top_L+%BrZSize) > (%mntnLm-%BrZSize) && %CaveH_btm_L <= (%mntnLm-%BrZSize)) %mntnLm = %CaveH_btm_L;
			if((%CaveH_top_R+%BrZSize) > (%mntnRm-%BrZSize) && %CaveH_btm_R <= (%mntnRm-%BrZSize)) %mntnRm = %CaveH_btm_R;
			if((%CaveH_top_D+%BrZSize) > (%mntnDm-%BrZSize) && %CaveH_btm_D <= (%mntnDm-%BrZSize)) %mntnDm = %CaveH_btm_D;
			if((%CaveH_top_U+%BrZSize) > (%mntnUm-%BrZSize) && %CaveH_btm_U <= (%mntnUm-%BrZSize)) %mntnUm = %CaveH_btm_U;
			
			if((%CaveH_top_LU+%BrZSize) > (%mntnLUm-%BrZSize) && %CaveH_btm_LU <= (%mntnLUm-%BrZSize)) %mntnLUm = %CaveH_btm_LU;
			if((%CaveH_top_RU+%BrZSize) > (%mntnRUm-%BrZSize) && %CaveH_btm_RU <= (%mntnRUm-%BrZSize)) %mntnRUm = %CaveH_btm_RU;
			if((%CaveH_top_LD+%BrZSize) > (%mntnLDm-%BrZSize) && %CaveH_btm_LD <= (%mntnLDm-%BrZSize)) %mntnLDm = %CaveH_btm_LD;
			if((%CaveH_top_RD+%BrZSize) > (%mntnRDm-%BrZSize) && %CaveH_btm_RD <= (%mntnRDm-%BrZSize)) %mntnRDm = %CaveH_btm_RD;

			%posB = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %BrH_fnl;//-%BrZhSize;
			%posStr = %mntnLm SPC %mntnRm SPC %mntnDm SPC %mntnUm SPC %mntnLUm SPC %mntnRUm SPC %mntnLDm SPC %mntnRDm;
			%brData = getWord(%tmpColPri,0) SPC getWord(%tmpColPri,1) SPC 0 SPC 0 SPC $PTGm.brTer_DB SPC %BrZSize SPC %FillBrXYZSize SPC "Terrain";
			%modTerStr = PTG_Chunk_ModTer(false,%posB SPC %posStr,%BrPosX SPC %BrPosY,%brData,%cl,%BG,%Chunk);

			//(top terrain brick is planted in "PTG_Chunk_ModTer" function)
			if(%CaveTerFillPass || %caveTerGapCheck)
			{
				if((%BrH_fnl-%BrZSize) > (%minAdjTer-%FillBrXYZSize-0.1))
				{
					%tmpColPriB = PTG_Chunk_FigureColPri(%BrH_init,%BrH_cf_aux,(%BrH_fnl-%BrZSize),"Dirt",%BrPosX,%BrPosY);
					PTG_Chunk_SubDivideFill(%cl,%Chunk,%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BG,(%BrH_fnl-%BrZSize) SPC (%minAdjTer-%FillBrXYZSize-0.1) SPC %genType,getWord(%tmpColPriB,0) SPC 0 SPC 0 SPC 0 SPC getWord(%tmpColPriB,1),">","Cube","TerrainBr");
				}
			}
		}
		
		//Normal Terrain
		else
		{
			//Dirt / Fill Layer (Merging of terrain and caves takes place in caves gen function)
			if((%CaveTerFillPass || %caveTerGapCheck))
			{
				%tmpColPriB = PTG_Chunk_FigureColPri(%BrH_init,%BrH_cf_aux,(%BrH_fnl-%BrZSize),"Dirt",%BrPosX,%BrPosY);

				if(%caveTerGapCheck && %caveAdjCheck)
				{
					if((%BrH_fnl-%BrZSize) > (%minAdjCave_btm-%BrZSize-%FillBrXYZSize))
						PTG_Chunk_SubDivideFill(%cl,%Chunk,%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BG,(%BrH_fnl-%BrZSize) SPC (%minAdjCave_btm-%BrZSize-%FillBrXYZSize) SPC %genType,getWord(%tmpColPriB,0) SPC 0 SPC 0 SPC 0 SPC getWord(%tmpColPriB,1),">","Disabled","TerrainBr");
				}
				else
				{
					if((%BrH_fnl-%BrZSize) > (%minAdjTer-%FillBrXYZSize))
						PTG_Chunk_SubDivideFill(%cl,%Chunk,%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BG,(%BrH_fnl-%BrZSize) SPC (%minAdjTer-%FillBrXYZSize) SPC %genType,getWord(%tmpColPriB,0) SPC 0 SPC 0 SPC 0 SPC getWord(%tmpColPriB,1),">","Disabled","TerrainBr");
				}
			}
		}
		
		//Plate-Capping (top plate)
		if($PTGm.enabPlateCap && (!$PTGm.enabModTer || $PTGm.modTerGenType $= "Cubes")) //gen if ModTer enabled or using Cubes ModTer type only
		{
			%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %BrH_fnl+0.1;
			%tmpColPri = PTG_Chunk_FigureColPri(%BrH_init,%BrH_cf_aux,%BrH_fnl,"Terrain",%BrPosX,%BrPosY);
			%BrDBp = "Brick" @ (%BrXYSize*2) @ "x" @ (%BrXYSize*2) @ "fData"; //plate-capping datablock

			PTG_Chunk_PlantBrick(%BrDBp,%pos,getWord(%tmpColPri,0),0,0,0,getWord(%tmpColPri,1),%cl,%BG,%Chunk,"TerrainBr","");
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	if((%BrPosX += %BrXYSize) >= %ChSize)
	{
		%BrPosX = %BrXYhSize;
		
		if((%BrPosY += %BrXYSize) >= %ChSize)
		{
			PTG_Chunk_Gen_Relay(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"Caves",%BG);
			return;
		}
		else
		{
			if($PTG.genSpeed < 2)
			{
				$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_genMS,0,50));
				scheduleNoQuota(%delay,0,PTG_Chunk_Gen_Terrain_Normal,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
			}
			else
				PTG_Chunk_Gen_Terrain_Normal(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
			return;
		}
	}
	
	if($PTG.genSpeed == 0)
	{
		$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_genMS,0,50));
		scheduleNoQuota(%delay,0,PTG_Chunk_Gen_Terrain_Normal,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
	}
	else
		PTG_Chunk_Gen_Terrain_Normal(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Chunk_Gen_Terrain_SkyLands(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY)
{
	%BrH_SL = $StrArrayHV_SkyLands[%BrPosX,%BrPosY];
	%BrH_ter = $StrArrayHV_Terrain[%BrPosX,%BrPosY];
	%BrH_init = $StrArrayHV_Mountains[%BrPosX,%BrPosY];	//make sure to start below generated terrain brick
	%BrH_cf_aux = $StrArrayHV_MountainsAux[%BrPosX,%BrPosY];
	%BrH_act = %BrH_init - $PTGm.terHLevel;
	%BrH_build = getWord($StrArrayData_Builds,0);
	%layerName = getField($StrArrayData_Builds,4);
	//%flatArPass = ($PTGm.enabBuildLoad || $PTGm.allowFlatAreas) && %layerName $= "Terrain";
	%ChSize = mClamp($PTGm.chSize,16,256);
	
	%BrZSize = $PTGm.brTer_Zsize;
	%BrZhSize = %BrZSize / 2;
	%BrXYSize = $PTGm.brTer_XYsize;
	%BrXYhSize = %BrXYSize / 2;
	%FillBrXYZSize = $PTGm.brTer_FillXYZSize;
	
	if(%FillBrXYZSize < 1 || $PTGm.enabModTer)
		%genType = "PlateFill";
	else
		%genType = "CubeFill";
	
	if($PTGm.enabPlateCap)
	{
		%relBrSz = %BrXYSize * 2;
		%BrDBp = "brick" @ %relBrSz @ "x" @ %relBrSz @ "fData";
	}
	
	//////////////////////////////////////////////////

	if(%BrH_act > $PTGm.skyLndsSecZ)
	{
		%BrH_SL_top = $StrArrayHV_SkyLandsTop[%BrPosX,%BrPosY];
		%BrH_SL_btm = $StrArrayHV_SkyLandsBtm[%BrPosX,%BrPosY];

		//Adjacent sides (skylands top)
		%SL_Lm = $StrArrayHV_SkyLandsTop[%BrPosX-%BrXYSize,%BrPosY];
		%SL_Rm = $StrArrayHV_SkyLandsTop[%BrPosX+%BrXYSize,%BrPosY];
		%SL_Dm = $StrArrayHV_SkyLandsTop[%BrPosX,%BrPosY-%BrXYSize];
		%SL_Um = $StrArrayHV_SkyLandsTop[%BrPosX,%BrPosY+%BrXYSize];

		//Adjacent sides (skylands bottom)
		%SL_LmB = $StrArrayHV_SkyLandsBtm[%BrPosX-%BrXYSize,%BrPosY];
		%SL_RmB = $StrArrayHV_SkyLandsBtm[%BrPosX+%BrXYSize,%BrPosY];
		%SL_DmB = $StrArrayHV_SkyLandsBtm[%BrPosX,%BrPosY-%BrXYSize];
		%SL_UmB = $StrArrayHV_SkyLandsBtm[%BrPosX,%BrPosY+%BrXYSize];
		
		//Max / Min adjacent values
		%minAdjSL_top = getMin(%SL_Lm,getMin(%SL_Rm,getMin(%SL_Dm,%SL_Um)));
		%maxAdjSL_btm = getMax(%SL_LmB,getMax(%SL_RmB,getMax(%SL_DmB,%SL_UmB)));
		%adjSLCut = ($StrArrayHV_Mountains[%BrPosX-%BrXYSize,%BrPosY] - $PTGm.terHLevel) <= $PTGm.skyLndsSecZ ||
					($StrArrayHV_Mountains[%BrPosX+%BrXYSize,%BrPosY] - $PTGm.terHLevel) <= $PTGm.skyLndsSecZ ||
					($StrArrayHV_Mountains[%BrPosX,%BrPosY-%BrXYSize] - $PTGm.terHLevel) <= $PTGm.skyLndsSecZ ||
					($StrArrayHV_Mountains[%BrPosX,%BrPosY+%BrXYSize] - $PTGm.terHLevel) <= $PTGm.skyLndsSecZ;

		////////////////////////////////////////////////////////////////////////////////////////////////////
		
		if((%BrH_SL_top - %BrZSize) >= (%BrH_SL_btm + %BrZSize))
		{
			//Top Layer (Plate-Capping)
			if($PTGm.enabPlateCap) //if ModTer disable or using Cubes only
			{
				%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %BrH_SL_top+0.1;
				%tmpColPri = PTG_Chunk_FigureColPri(%BrH_ter,%BrH_cf_aux,%BrH_SL_top,"Terrain",%BrPosX,%BrPosY);
				PTG_Chunk_PlantBrick(%BrDBp,%pos,getWord(%tmpColPri,0),0,0,0,getWord(%tmpColPri,1),%cl,%BG,%Chunk,"TerrainBr","");
			}
			
			//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
			if(isObject(%Chunk) && ((%tempZMax = %BrH_SL_top) > %Chunk.PTGHighestZpos))
				%Chunk.PTGHighestZpos = %tempZMax;
			
			////////////////////////////////////////////////////////////////////////////////////////////////////

			//If Modular Terrain is enabled
			if($PTGm.enabModTer)
			{
				//Top Layer
				%SL_LUm = $StrArrayHV_SkyLandsTop[%BrPosX-%BrXYSize,%BrPosY+%BrXYSize];
				%SL_RUm = $StrArrayHV_SkyLandsTop[%BrPosX+%BrXYSize,%BrPosY+%BrXYSize];
				%SL_LDm = $StrArrayHV_SkyLandsTop[%BrPosX-%BrXYSize,%BrPosY-%BrXYSize];
				%SL_RDm = $StrArrayHV_SkyLandsTop[%BrPosX+%BrXYSize,%BrPosY-%BrXYSize];
				
				%posB = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %BrH_SL_top; //-%BrZhSize;
				%posStr = %SL_Lm SPC %SL_Rm SPC %SL_Dm SPC %SL_Um SPC %SL_LUm SPC %SL_RUm SPC %SL_LDm SPC %SL_RDm;
				%tmpColPri = PTG_Chunk_FigureColPri(%BrH_ter,%BrH_cf_aux,%BrH_SL_top,"Terrain",%BrPosX,%BrPosY);
				%brData = getWord(%tmpColPri,0) SPC getWord(%tmpColPri,1) SPC 0 SPC 0 SPC $PTGm.brTer_DB SPC %BrZSize SPC %BrXYSize SPC "Terrain";
				//%brData = $PTGm.Bio_Def_TerCol SPC $PTGm.Bio_Def_TerPri SPC 0 SPC 0 SPC $PTGm.brTer_DB SPC %BrZSize SPC %BrXYSize SPC "Terrain";
				%modTerStr = PTG_Chunk_ModTer(false,%posB SPC %posStr,%BrPosX SPC %BrPosY,%brData,%cl,%BG,%Chunk);
				
				//Bottom Layer
				%SL_LUmB = $StrArrayHV_SkyLandsBtm[%BrPosX-%BrXYSize,%BrPosY+%BrXYSize];
				%SL_RUmB = $StrArrayHV_SkyLandsBtm[%BrPosX+%BrXYSize,%BrPosY+%BrXYSize];
				%SL_LDmB = $StrArrayHV_SkyLandsBtm[%BrPosX-%BrXYSize,%BrPosY-%BrXYSize];
				%SL_RDmB = $StrArrayHV_SkyLandsBtm[%BrPosX+%BrXYSize,%BrPosY-%BrXYSize];
				
				%posB = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %BrH_SL_btm; //-%BrZhSize;
				%posStr = %SL_LmB SPC %SL_RmB SPC %SL_DmB SPC %SL_UmB SPC %SL_LUmB SPC %SL_RUmB SPC %SL_LDmB SPC %SL_RDmB;
				%brData = $PTGm.skylandsBtmCol SPC $PTGm.skylandsBtmPri SPC 0 SPC 0 SPC $PTGm.brTer_DB SPC %BrZSize SPC %BrXYSize SPC "Terrain";
				%modTerStr = PTG_Chunk_ModTer(true,%posB SPC %posStr,%BrPosX SPC %BrPosY,%brData,%cl,%BG,%Chunk); //$PTGm.fltIsldsBtmCol

				//Subdivide Fill References
				//%condA = ">=";
				//%condB = "<=";
				//%condC = "<=";
				//%genTypeA = "PlateFill";
				%fillType = "Cube";
			}
			
			//If Modular Terrain is disabled
			else
			{
				//Subdivide Fill References
				//%condA = ">";
				//%condB = "<";
				//%condC = "<=";
				//%genTypeA = "PlateFill"; //CubeFill
				%fillType = "Disabled";
			}
			
			
			//Top Layer Brick
			if($PTGm.enabPlateCap)
				%tmpColPri = PTG_Chunk_FigureColPri(%BrH_ter-%BrZhSize,%BrH_cf_aux-%BrZhSize,%BrH_SL_top-%BrZhSize,"Dirt",%BrPosX,%BrPosY);
			else
				%tmpColPri = PTG_Chunk_FigureColPri(%BrH_ter,%BrH_cf_aux,%BrH_SL_top,"Terrain",%BrPosX,%BrPosY);
			
			%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %BrH_SL_top-%BrZhSize;
			PTG_Chunk_PlantBrick($PTGm.brTer_DB,%pos,getWord(%tmpColPri,0),0,0,0,getWord(%tmpColPri,1),%cl,%BG,%Chunk,"TerrainBr","");

			////////////////////////////////////////////////////////////////////////////////////////////////////
			//Gap-Fill
			
			%startPos = (%BrH_SL_top - %BrZSize);
			%tmpColPri = PTG_Chunk_FigureColPri(%BrH_ter-%BrZhSize,%BrH_cf_aux-%BrZhSize,%BrH_SL_top-%BrZhSize,"Dirt",%BrPosX,%BrPosY);
			
			//prevent gap fill if no gap exists (check prior to subdivide func)
			
			if((%minAdjSL_top - %BrZSize - %FillBrXYZSize) > (%maxAdjSL_btm + %FillBrXYZSize) && !%adjSLCut) //if edge of skylands not cut and gap-fills don't interfere
			{
				//Top Gap-Fill Layer			
				if(((%BrH_SL_top - %BrZSize) > (%minAdjSL_top + 0.1)))  //prevent unnecessary fill-bricks
				{
					%endPos = (%minAdjSL_top - %BrZSize - %FillBrXYZSize);
					
					if(%startPos > %endPos)
						PTG_Chunk_SubDivideFill(%cl,%Chunk,%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BG,%startPos SPC %endPos SPC %genType,getWord(%tmpColPri,0) SPC 0 SPC 0 SPC 0 SPC getWord(%tmpColPri,1),">",%fillType,"TerrainBr");
				}
				
				//////////////////////////////////////////////////
				
				//Bottom Gap-Fill Layer
				%startPos = %BrH_SL_Btm;
				%endPos = (%maxAdjSL_btm + %FillBrXYZSize); //no brZ size here

				//Make sure bottom gap-fill allows at least one fill-brick to be planted, otherwise use smaller bricks (without interfering with caves when intersecting gap-pillars)
				if(%BrH_SL_Btm < %maxAdjSL_btm)
					PTG_Chunk_SubDivideFill(%cl,%Chunk,%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BG,%startPos SPC %endPos SPC %genType,$PTGm.skylandsBtmCol SPC 0 SPC 0 SPC 0 SPC $PTGm.skylandsBtmPri,"<",%fillType,"TerrainBr");
				else
				{
					%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %BrH_SL_btm + %BrZhSize;
					PTG_Chunk_PlantBrick($PTGm.brTer_DB,%pos,$PTGm.skylandsBtmCol,0,0,0,$PTGm.skylandsBtmPri,%cl,%BG,%Chunk,"TerrainBr","");
				}				
			}
			
			////////////////////////////////////////////////////////////////////////////////////////////////////
			//Gap-fill between Floating Island A top and bottom layers to fill seams
			
			else
				PTG_Chunk_SubDivideFill(%cl,%Chunk,%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BG,%BrH_SL_btm SPC (%BrH_SL_top - %BrZSize + 0.1) SPC "PlateFill",$PTGm.skylandsBtmCol SPC 0 SPC 0 SPC 0 SPC $PTGm.skylandsBtmPri,"<=",%fillType,"TerrainBr");
		}
	}
	
	//////////////////////////////////////////////////
	
	if((%BrPosX += %BrXYSize) >= %ChSize)
	{
		%BrPosX = %BrXYhSize;
		
		if((%BrPosY += %BrXYSize) >= %ChSize)
		{
			PTG_Chunk_Gen_Relay(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"Caves",%BG);
			return;
		}
		else
		{
			if($PTG.genSpeed < 2)
			{
				$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_genMS,0,50));
				scheduleNoQuota(%delay,0,PTG_Chunk_Gen_Terrain_SkyLands,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
			}
			else
				PTG_Chunk_Gen_Terrain_SkyLands(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
			return;
		}
	}
	
	if($PTG.genSpeed == 0)
	{
		$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_genMS,0,50));
		scheduleNoQuota(%delay,0,PTG_Chunk_Gen_Terrain_SkyLands,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
	}
	else
		PTG_Chunk_Gen_Terrain_SkyLands(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Chunk_Gen_Caves_Normal(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY)
{
	%BrH_init = $StrArrayHV_Terrain[%BrPosX,%BrPosY];
	%BrH_cf = $StrArrayHV_Mountains[%BrPosX,%BrPosY];
	%CaveH_btm = $StrArrayHV_CavesA[%BrPosX,%BrPosY];
	%CaveH_top = $StrArrayHV_CavesB[%BrPosX,%BrPosY];
	%ChSize = mClamp($PTGm.chSize,16,256);
	
	%BrZSize = $PTGm.brTer_Zsize;
	%BrZhSize = %BrZSize / 2;
	%BrXYSize = $PTGm.brTer_XYsize;
	%BrXYhSize = %BrXYSize / 2;
	%FillBrXYZSize = $PTGm.brTer_FillXYZSize;
	%BrH_fnl = %BrH_cf - %BrZSize;
	
	if(%FillBrXYZSize < 1 || $PTGm.enabModTer)
		%genType = "PlateFill";
	else
		%genType = "CubeFill";
	
	%terLm = $StrArrayHV_Mountains[%BrPosX-%BrXYSize,%BrPosY];
	%terRm = $StrArrayHV_Mountains[%BrPosX+%BrXYSize,%BrPosY];
	%terDm = $StrArrayHV_Mountains[%BrPosX,%BrPosY-%BrXYSize];
	%terUm = $StrArrayHV_Mountains[%BrPosX,%BrPosY+%BrXYSize];
	
	%CaveH_btm_Lrel = %CaveH_btm_Ltmp = %CaveH_btm_L = $StrArrayHV_CavesA[%BrPosX-%BrXYSize,%BrPosY]; //include tmp vars for selecting details as well!
	%CaveH_btm_Rrel = %CaveH_btm_Rtmp = %CaveH_btm_R = $StrArrayHV_CavesA[%BrPosX+%BrXYSize,%BrPosY];
	%CaveH_btm_Drel = %CaveH_btm_Dtmp = %CaveH_btm_D = $StrArrayHV_CavesA[%BrPosX,%BrPosY-%BrXYSize];
	%CaveH_btm_Urel = %CaveH_btm_Utmp = %CaveH_btm_U = $StrArrayHV_CavesA[%BrPosX,%BrPosY+%BrXYSize];
	
	if(%CaveH_btm_L > %terLm || %CaveH_btm_L <= 0) %CaveH_btm_L = %terLm;
	if(%CaveH_btm_R > %terRm || %CaveH_btm_R <= 0) %CaveH_btm_R = %terRm;
	if(%CaveH_btm_D > %terDm || %CaveH_btm_D <= 0) %CaveH_btm_D = %terDm;
	if(%CaveH_btm_U > %terUm || %CaveH_btm_U <= 0) %CaveH_btm_U = %terUm;
	
	%CaveH_top_L = $StrArrayHV_CavesB[%BrPosX-%BrXYSize,%BrPosY];
	%CaveH_top_R = $StrArrayHV_CavesB[%BrPosX+%BrXYSize,%BrPosY];
	%CaveH_top_D = $StrArrayHV_CavesB[%BrPosX,%BrPosY-%BrXYSize];
	%CaveH_top_U = $StrArrayHV_CavesB[%BrPosX,%BrPosY+%BrXYSize];
	
	//Min / Max Adjacent Height Reference Values
	%tempFMinHTer = getMin(%terLm, getMin(%terRm, getMin(%terDm,%terUm)));
	%tempFMinHCave = getMax(%CaveH_top_L, getMax(%CaveH_top_R, getMax(%CaveH_top_D,%CaveH_top_U))); //tempFMaxH since using getMax (fix others)		
	%tempFMinHCave_btm = getMin(%CaveH_btm_L, getMin(%CaveH_btm_R, getMin(%CaveH_btm_D,%CaveH_btm_U)));
	
	//////////////////////////////////////////////////
	
	//check bottom layer first because if CaveA > terrain layer, don't gen caves (they would be floating above terrain)
	if(%CaveH_top > 0 && %CaveH_btm <= %BrH_fnl) //use mFloatLength to avoid strange issue with "<=" condition in the Torque Engine
	{
		//Bottom Layer
		if(%CaveH_btm > 0)
		{
			//Main Brick
			%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %CaveH_btm-%BrZhSize;
			PTG_Chunk_PlantBrick($PTGm.brTer_DB,%pos,$PTGbio.Bio_CaveBtm_TerCol,0,0,0,$PTGbio.Bio_CaveBtm_TerPri,%cl,%BG,%Chunk,"TerrainBr","");
			
			//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
			if(isObject(%Chunk) && ((%tempZMax = %CaveH_btm) > %Chunk.PTGHighestZpos))
				%Chunk.PTGHighestZpos = %tempZMax;


			//If Modular Terrain Enabled
			if($PTGm.enabModTer)
			{
				%CaveH_btm_LU = $StrArrayHV_CavesA[%BrPosX-%BrXYSize,%BrPosY+%BrXYSize];
				%CaveH_btm_RU = $StrArrayHV_CavesA[%BrPosX+%BrXYSize,%BrPosY+%BrXYSize];
				%CaveH_btm_LD = $StrArrayHV_CavesA[%BrPosX-%BrXYSize,%BrPosY-%BrXYSize];
				%CaveH_btm_RD = $StrArrayHV_CavesA[%BrPosX+%BrXYSize,%BrPosY-%BrXYSize];
				
				%CaveH_top_LU = $StrArrayHV_CavesB[%BrPosX-%BrXYSize,%BrPosY+%BrXYSize];
				%CaveH_top_RU = $StrArrayHV_CavesB[%BrPosX+%BrXYSize,%BrPosY+%BrXYSize];
				%CaveH_top_LD = $StrArrayHV_CavesB[%BrPosX-%BrXYSize,%BrPosY-%BrXYSize];
				%CaveH_top_RD = $StrArrayHV_CavesB[%BrPosX+%BrXYSize,%BrPosY-%BrXYSize];
				
				%terLUm = $StrArrayHV_Mountains[%BrPosX-%BrXYSize,%BrPosY+%BrXYSize];
				%terRUm = $StrArrayHV_Mountains[%BrPosX+%BrXYSize,%BrPosY+%BrXYSize];
				%terLDm = $StrArrayHV_Mountains[%BrPosX-%BrXYSize,%BrPosY-%BrXYSize];
				%terRDm = $StrArrayHV_Mountains[%BrPosX+%BrXYSize,%BrPosY-%BrXYSize];
				
				//For making bottom cave layer revert to terrain height when cave layer is cut by section height (seamless transition between caves / terrain for ModTer placement)
				if(%CaveH_btm_Ltmp == 0 && (%terLm-%CaveH_top) < %BrZSize) %CaveH_btm_Lrel = %terLm;
				if(%CaveH_btm_Rtmp == 0 && (%terRm-%CaveH_top) < %BrZSize) %CaveH_btm_Rrel = %terRm;
				if(%CaveH_btm_Dtmp == 0 && (%terDm-%CaveH_top) < %BrZSize) %CaveH_btm_Drel = %terDm;
				if(%CaveH_btm_Utmp == 0 && (%terUm-%CaveH_top) < %BrZSize) %CaveH_btm_Urel = %terUm;
				
				if(%CaveH_btm_LU == 0 && (%terLUm-%CaveH_top) < %BrZSize) %CaveH_btm_LU = %terLUm;
				if(%CaveH_btm_RU == 0 && (%terRUm-%CaveH_top) < %BrZSize) %CaveH_btm_RU = %terRUm;
				if(%CaveH_btm_LD == 0 && (%terLDm-%CaveH_top) < %BrZSize) %CaveH_btm_LD = %terLDm;
				if(%CaveH_btm_RD == 0 && (%terRDm-%CaveH_top) < %BrZSize) %CaveH_btm_RD = %terRDm;
				
				%posB = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %CaveH_btm;
				%posStr = %CaveH_btm_Lrel SPC %CaveH_btm_Rrel SPC %CaveH_btm_Drel SPC %CaveH_btm_Urel SPC %CaveH_btm_LU SPC %CaveH_btm_RU SPC %CaveH_btm_LD SPC %CaveH_btm_RD; //tmp vars are used to prevent wedges from being used for edges of caves, when top and btm layers are merged
				%brData = $PTGbio.Bio_CaveBtm_TerCol SPC $PTGbio.Bio_CaveBtm_TerPri SPC 0 SPC 0 SPC $PTGm.brTer_DB SPC %BrZSize SPC %FillBrXYZSize SPC "CaveA";
				%modTerStr = PTG_Chunk_ModTer(false,%posB SPC %posStr,%BrPosX SPC %BrPosY,%brData,%cl,%BG,%Chunk);
				
				//ModTer Bottom Fill (always use "PlateFill" cond below for SubDivideFill otherwise there will be gaps in terrain (due to ModTer brick Z size scales and how subdivide fill works)
				PTG_Chunk_SubDivideFill(%cl,%Chunk,getWord(%posB,0),getWord(%posB,1),%BG,(%CaveH_btm-%BrZSize) SPC (%tempFMinHCave_btm-%BrZSize-%FillBrXYZSize) SPC %genType,$PTGbio.Bio_CaveBtm_TerCol SPC 0 SPC 0 SPC 0 SPC $PTGbio.Bio_CaveBtm_TerPri,">","Cube","TerrainBr");
			}
			
			//If Modular Terrain Disabled
			else
			{
				//Cave A (Bottom Layer) Gap Fill (Don't gen fill brick if no gap in adjacent bricks)
				if((%CaveH_btm - %BrZSize) > %tempFMinHCave_btm) //Don't gen fill brick if adjacent bricks are at same height (for-loop doesn't prevent this on first loop for some reason, so prevent initially instead)
					PTG_Chunk_SubDivideFill(%cl,%Chunk,%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BG,(%CaveH_btm-%BrZSize) SPC ((%tempFMinHCave_btm-%BrZSize-%FillBrXYZSize)+0.1) SPC %genType,$PTGbio.Bio_CaveBtm_TerCol SPC 0 SPC 0 SPC 0 SPC $PTGbio.Bio_CaveBtm_TerPri,">","Disabled","TerrainBr");
			}
		}
	
		//////////////////////////////////////////////////
		//Top Layer

		if((%CaveH_top + %BrZSize) <= %BrH_fnl)
		{
			//Main Brick
			%startPos_CaveTop = %CaveH_top+%BrZSize;
			%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %CaveH_top+%BrZhSize;
			PTG_Chunk_PlantBrick($PTGm.brTer_DB,%pos,$PTGbio.Bio_CaveTop_TerCol,0,0,0,$PTGbio.Bio_CaveTop_TerPri,%cl,%BG,%Chunk,"TerrainBr","");
			
				//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
				//if(isObject(%Chunk) && ((%tempZMax = %CaveH_btm) > %Chunk.PTGHighestZpos))
				//	%Chunk.PTGHighestZpos = %tempZMax; //shouldn't be necessary for top layer of caves
			
			//If Modular Terrain Enabled
			if($PTGm.enabModTer)
			{
				%posB = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %CaveH_top;
				%posStr = %CaveH_top_L SPC %CaveH_top_R SPC %CaveH_top_D SPC %CaveH_top_U SPC %CaveH_top_LU SPC %CaveH_top_RU SPC %CaveH_top_LD SPC %CaveH_top_RD; //tmp vars are used to prevent wedges from being used for edges of caves, when top and btm layers are merged
				%brData = $PTGbio.Bio_CaveTop_TerCol SPC $PTGbio.Bio_CaveTop_TerPri SPC 0 SPC 0 SPC $PTGm.brTer_DB SPC %BrZSize SPC %FillBrXYZSize SPC "CaveB";
				%modTerStrB = PTG_Chunk_ModTer(true,%posB SPC %posStr,%BrPosX SPC %BrPosY,%brData,%cl,%BG,%Chunk); //inv?

				//ModTer Bottom Fill (always use "PlateFill" cond below for SubDivideFill otherwise there will be gaps in terrain (due to ModTer brick Z size scales and how subdivide fill works)
				if((%tempFMinHCave + %BrZSize + %FillBrXYZSize) <= (%tempFMinHTer - %BrZSize - %FillBrXYZSize))
				{
					if(%startPos_CaveTop < (%tempFMinHCave + %BrZSize + %FillBrXYZSize)) //$PTGbio.Bio_CaveTop_TerCol
						PTG_Chunk_SubDivideFill(%cl,%Chunk,getWord(%posB,0),getWord(%posB,1),%BG,%startPos_CaveTop SPC (%tempFMinHCave+%BrZSize+%FillBrXYZSize) SPC %genType,$PTGbio.Bio_CaveTop_TerCol SPC 0 SPC 0 SPC 0 SPC $PTGbio.Bio_CaveTop_TerPri,"<","Cube","TerrainBr");
				}
				else
					PTG_Chunk_SubDivideFill(%cl,%Chunk,getWord(%posB,0),getWord(%posB,1),%BG,%startPos_CaveTop SPC (%BrH_fnl+0.1) SPC "PlateFill",$PTGbio.Bio_CaveTop_TerCol SPC 0 SPC 0 SPC 0 SPC $PTGbio.Bio_CaveTop_TerPri,"<=","Cube","TerrainBr");
			}
			
			//If Modular Terrain Disabled
			else
			{
				if((%tempFMinHCave + %BrZSize + %FillBrXYZSize) <= (%tempFMinHTer - %BrZSize - %FillBrXYZSize))
				{
					if(%startPos_CaveTop < (%tempFMinHCave+%BrZSize+%FillBrXYZSize))
						PTG_Chunk_SubDivideFill(%cl,%Chunk,getWord(%pos,0),getWord(%pos,1),%BG,%startPos_CaveTop SPC (%tempFMinHCave+%BrZSize+%FillBrXYZSize) SPC %genType,$PTGbio.Bio_CaveTop_TerCol SPC 0 SPC 0 SPC 0 SPC $PTGbio.Bio_CaveTop_TerPri,"<","Disabled","TerrainBr");
				}
				else
					PTG_Chunk_SubDivideFill(%cl,%Chunk,getWord(%pos,0),getWord(%pos,1),%BG,%startPos_CaveTop SPC (%BrH_fnl+0.1) SPC "PlateFill",$PTGbio.Bio_CaveTop_TerCol SPC 0 SPC 0 SPC 0 SPC $PTGbio.Bio_CaveTop_TerPri,"<=","Disabled","TerrainBr");
			}

			//////////////////////////////////////////////////
			//Top and Bottom Cave Layer Merge
			
			//Merge cave A&B layers to fill seam if one adjacent side is exposed due to cave section cut (==0 means section cut where cave not present, for Cave_top only)
			if(((%CaveH_top_L == 0) || (%CaveH_top_R == 0) || (%CaveH_top_D == 0) || (%CaveH_top_U == 0)))
			{
				//Merge layers normally
				if(!$PTGm.enabModTer)
					PTG_Chunk_SubDivideFill(%cl,%Chunk,%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BG,%CaveH_btm SPC (%CaveH_top+0.1) SPC "PlateFill",$PTGbio.Bio_CaveTop_TerCol SPC 0 SPC 0 SPC 0 SPC $PTGbio.Bio_CaveTop_TerPri,"<=","Disabled","TerrainBr");
				else
					PTG_Chunk_SubDivideFill(%cl,%Chunk,%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BG,%CaveH_btm SPC (%CaveH_top+0.1) SPC "PlateFill",$PTGbio.Bio_CaveTop_TerCol SPC 0 SPC 0 SPC 0 SPC $PTGbio.Bio_CaveTop_TerPri,"<=","Cube","TerrainBr");
			}
		}
	}
	
	//////////////////////////////////////////////////
	
	if((%BrPosX += %BrXYSize) >= %ChSize)
	{
		%BrPosX = %BrXYhSize;
		
		if((%BrPosY += %BrXYSize) >= %ChSize)
		{
			PTG_Chunk_Gen_Relay(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"FltIslds",%BG);
			return;
		}
		else
		{
			if($PTG.genSpeed < 2)
			{
				$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_genMS,0,50));
				scheduleNoQuota(%delay,0,PTG_Chunk_Gen_Caves_Normal,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
			}
			else
				PTG_Chunk_Gen_Caves_Normal(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
			return;
		}
	}
	
	if($PTG.genSpeed == 0)
	{
		$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_genMS,0,50));
		scheduleNoQuota(%delay,0,PTG_Chunk_Gen_Caves_Normal,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
	}
	else
		PTG_Chunk_Gen_Caves_Normal(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Chunk_Gen_FltIslds(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY)
{
	%BrH_build = getWord($StrArrayData_Builds,0);
	%relGrid = getWord($StrArrayData_Builds,2);
	%relGridHSz = %relGrid / 2;
	%bnds_build = getField($StrArrayData_Builds,1);
	%layerName = getField($StrArrayData_Builds,4);
	%ChSize = mClamp($PTGm.chSize,16,256);
		
	//%BrH_FIa = $StrArrayHV_FltIsldsATop[%BrPosX,%BrPosY];
	//%BrH_FIb = $StrArrayHV_FltIsldsBTop[%BrPosX,%BrPosY];
	%BrXYSize = $PTGm.brFltIslds_XYsize;
	%BrXYhSize = %BrXYSize / 2;
	%BrZSize = $PTGm.brFltIslds_Zsize;
	%BrZhSize = %BrZSize / 2;
	%FillBrXYZSize = $PTGm.brFltIslds_XYsize;
	
	if(%FillBrXYZSize < 1 || $PTGm.enabModTer_FltIslds)
		%genType = "PlateFill";
	else
		%genType = "CubeFill";

	%BrH_FIa_top = $StrArrayHV_FltIsldsATop[%BrPosX,%BrPosY];
	%BrH_FIb_top = $StrArrayHV_FltIsldsBTop[%BrPosX,%BrPosY];
	%BrH_FIa_btm = $StrArrayHV_FltIsldsABtm[%BrPosX,%BrPosY];
	%BrH_FIb_btm = $StrArrayHV_FltIsldsBBtm[%BrPosX,%BrPosY];
	
	%FI_A_Lm = $StrArrayHV_FltIsldsATop[%BrPosX-%BrXYSize,%BrPosY];
	%FI_A_Rm = $StrArrayHV_FltIsldsATop[%BrPosX+%BrXYSize,%BrPosY];
	%FI_A_Dm = $StrArrayHV_FltIsldsATop[%BrPosX,%BrPosY-%BrXYSize];
	%FI_A_Um = $StrArrayHV_FltIsldsATop[%BrPosX,%BrPosY+%BrXYSize];
	
	%FI_B_Lm = $StrArrayHV_FltIsldsBTop[%BrPosX-%BrXYSize,%BrPosY];
	%FI_B_Rm = $StrArrayHV_FltIsldsBTop[%BrPosX+%BrXYSize,%BrPosY];
	%FI_B_Dm = $StrArrayHV_FltIsldsBTop[%BrPosX,%BrPosY-%BrXYSize];
	%FI_B_Um = $StrArrayHV_FltIsldsBTop[%BrPosX,%BrPosY+%BrXYSize];
	
	%FI_A_LmB = $StrArrayHV_FltIsldsABtm[%BrPosX-%BrXYSize,%BrPosY];
	%FI_A_RmB = $StrArrayHV_FltIsldsABtm[%BrPosX+%BrXYSize,%BrPosY];
	%FI_A_DmB = $StrArrayHV_FltIsldsABtm[%BrPosX,%BrPosY-%BrXYSize];
	%FI_A_UmB = $StrArrayHV_FltIsldsABtm[%BrPosX,%BrPosY+%BrXYSize];
	
	%FI_B_LmB = $StrArrayHV_FltIsldsBBtm[%BrPosX-%BrXYSize,%BrPosY];
	%FI_B_RmB = $StrArrayHV_FltIsldsBBtm[%BrPosX+%BrXYSize,%BrPosY];
	%FI_B_DmB = $StrArrayHV_FltIsldsBBtm[%BrPosX,%BrPosY-%BrXYSize];
	%FI_B_UmB = $StrArrayHV_FltIsldsBBtm[%BrPosX,%BrPosY+%BrXYSize];

	%minAdjFIa_top = getMin(%FI_A_Lm,getMin(%FI_A_Rm,getMin(%FI_A_Dm,%FI_A_Um))) - %BrZSize;
	%minAdjFIb_top = getMin(%FI_B_Lm,getMin(%FI_B_Rm,getMin(%FI_B_Dm,%FI_B_Um))) - %BrZSize;
	%minAdjFIa_btm = getMax(%FI_A_LmB,getMax(%FI_A_RmB,getMax(%FI_A_DmB,%FI_A_UmB)));
	%minAdjFIb_btm = getMax(%FI_B_LmB,getMax(%FI_B_RmB,getMax(%FI_B_DmB,%FI_B_UmB)));

	%adjFIaCut = %FI_A_Lm <= 0 || %FI_A_Rm <= 0 || %FI_A_Dm <= 0 || %FI_A_Um <= 0;
	%adjFIbCut = %FI_B_Lm <= 0 || %FI_B_Rm <= 0 || %FI_B_Dm <= 0 || %FI_B_Um <= 0;

	//////////////////////////////////////////////////

	//!!! Add check to prevent fill brick from sticking in between btm and top layers (see screenshot) !!! (might have been due to using 0.5 as flt islds btm mult)
	
	//Floating Island A
	if(%BrH_FIa_top > 0 && (%BrH_FIa_top - %BrZSize) >= (%BrH_FIa_btm + %BrZSize))
	{
		//Top layer
		%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %BrH_FIa_top - %BrZhSize;
		PTG_Chunk_PlantBrick($PTGm.brFltIslds_DB,%pos,$PTGm.fltIsldsTerCol,0,0,0,$PTGm.fltIsldsTerPri,%cl,%BG,%Chunk,"FltIsldsBr","");
		
		//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
		if(isObject(%Chunk) && ((%tempZMax = %BrH_FIa_top) > %Chunk.PTGHighestZpos))
			%Chunk.PTGHighestZpos = %tempZMax;

		
		//If Modular Terrain is enabled
		if($PTGm.enabModTer_FltIslds)
		{
			%FI_A_LUm = $StrArrayHV_FltIsldsATop[%BrPosX-%BrXYSize,%BrPosY+%BrXYSize];
			%FI_A_RUm = $StrArrayHV_FltIsldsATop[%BrPosX+%BrXYSize,%BrPosY+%BrXYSize];
			%FI_A_LDm = $StrArrayHV_FltIsldsATop[%BrPosX-%BrXYSize,%BrPosY-%BrXYSize];
			%FI_A_RDm = $StrArrayHV_FltIsldsATop[%BrPosX+%BrXYSize,%BrPosY-%BrXYSize];

			%posB = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %BrH_FIa_top;
			%posStr = %FI_A_Lm SPC %FI_A_Rm SPC %FI_A_Dm SPC %FI_A_Um SPC %FI_A_LUm SPC %FI_A_RUm SPC %FI_A_LDm SPC %FI_A_RDm;
			%brData = $PTGm.fltIsldsTerCol SPC $PTGm.fltIsldsTerPri SPC 0 SPC 0 SPC $PTGm.brFltIslds_DB SPC %BrZSize SPC %FillBrXYZSize SPC "FltIslds";
			%modTerStr = PTG_Chunk_ModTer(false,%posB SPC %posStr,%BrPosX SPC %BrPosY,%brData,%cl,%BG,%Chunk);

			//Bottom Layer
			%FI_A_LUmB = $StrArrayHV_FltIsldsABtm[%BrPosX-%BrXYSize,%BrPosY+%BrXYSize];
			%FI_A_RUmB = $StrArrayHV_FltIsldsABtm[%BrPosX+%BrXYSize,%BrPosY+%BrXYSize];
			%FI_A_LDmB = $StrArrayHV_FltIsldsABtm[%BrPosX-%BrXYSize,%BrPosY-%BrXYSize];
			%FI_A_RDmB = $StrArrayHV_FltIsldsABtm[%BrPosX+%BrXYSize,%BrPosY-%BrXYSize];

			%posB = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %BrH_FIa_btm;
			%posStr = %FI_A_LmB SPC %FI_A_RmB SPC %FI_A_DmB SPC %FI_A_UmB SPC %FI_A_LUmB SPC %FI_A_RUmB SPC %FI_A_LDmB SPC %FI_A_RDmB;
			%brData = $PTGm.fltIsldsBtmCol SPC $PTGm.fltIsldsBtmPri SPC 0 SPC 0 SPC $PTGm.brFltIslds_DB SPC %BrZSize SPC %FillBrXYZSize SPC "FltIslds";
			%modTerStr = PTG_Chunk_ModTer(true,%posB SPC %posStr,%BrPosX SPC %BrPosY,%brData,%cl,%BG,%Chunk);
			
			//%condA = ">=";
			//%condB = "<=";
			//%condC = "<=";
			//%genTypeA = "PlateFill";
			%fillType = "Cube";
		}
		
		//If Modular Terrain is disabled
		else
		{
			//%condA = ">";
			//%condB = "<";
			//%condC = "<=";
			//%genTypeA = "PlateFill"; //CubeFill
			%fillType = "Disabled";
		}
		
		//Gap Fill
		if((%minAdjFIa_top - %FillBrXYZSize) > (%minAdjFIa_btm + %FillBrXYZSize) && !%adjFIaCut)
		{
			%endPos = mFloor((%minAdjFIa_top - %FillBrXYZSize) / %BrZSize) * %BrZSize;
			
			//Top Layer
			if((%BrH_FIa_top - %BrZSize) > (%minAdjFIa_top + %BrZSize))
			{
				if($PTGm.dirtSameTer)
				{
					%dirtCol = $PTGm.fltIsldsTerCol;
					%dirtPri = $PTGm.fltIsldsTerPri;
				}
				else
				{
					%dirtCol = $PTGm.fltIsldsDirtCol;
					%dirtPri = $PTGm.fltIsldsDirtPri;
				}
				
				if((%BrH_FIa_top - %BrZSize) > %endPos)
					PTG_Chunk_SubDivideFill(%cl,%Chunk,%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BG,(%BrH_FIa_top - %BrZSize) SPC %endPos SPC %genType,%dirtCol SPC 0 SPC 0 SPC 0 SPC %dirtPri,">",%fillType,"FltIsldsBr");
			}
			
			//Bottom Layer
			if(((%minAdjFIa_btm + %FillBrXYZSize) - %BrH_FIa_btm) > %FillBrXYZSize)
			{
				if(%BrH_FIa_btm < (%minAdjFIa_btm + %FillBrXYZSize))
					PTG_Chunk_SubDivideFill(%cl,%Chunk,%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BG,%BrH_FIa_btm SPC (%minAdjFIa_btm + %FillBrXYZSize) SPC %genType,$PTGm.fltIsldsBtmCol SPC 0 SPC 0 SPC 0 SPC $PTGm.fltIsldsBtmPri,"<",%fillType,"FltIsldsBr");
			}
			else if(%BrH_FIa_btm > 0)
			{
				%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %BrH_FIa_btm + %BrZhSize;
				PTG_Chunk_PlantBrick($PTGm.brFltIslds_DB,%pos,$PTGm.fltIsldsBtmCol,0,0,0,$PTGm.fltIsldsBtmPri,%cl,%BG,%Chunk,"FltIsldsBr","");
			}
		}
		else //Gap-fill between Floating Island A top and bottom layers
			PTG_Chunk_SubDivideFill(%cl,%Chunk,%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BG,%BrH_FIa_btm SPC(%BrH_FIa_top - %BrZSize + 0.1) SPC "PlateFill",$PTGm.fltIsldsBtmCol SPC 0 SPC 0 SPC 0 SPC $PTGm.fltIsldsBtmPri,"<=",%fillType,"FltIsldsBr");
	}
	
	//////////////////////////////////////////////////
	
	//Floating Island B
	if(%BrH_FIb_top > 0 && (%BrH_FIb_top - %BrZSize) >= (%BrH_FIb_btm + %BrZSize))
	{
		//Top layer
		%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %BrH_FIb_top - %BrZhSize;
		PTG_Chunk_PlantBrick($PTGm.brFltIslds_DB,%pos,$PTGm.fltIsldsTerCol,0,0,0,$PTGm.fltIsldsTerPri,%cl,%BG,%Chunk,"FltIsldsBr","");
		
		//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
		if(isObject(%Chunk) && ((%tempZMax = %BrH_FIb_top) > %Chunk.PTGHighestZpos))
			%Chunk.PTGHighestZpos = %tempZMax;

		
		//If Modular Terrain is enabled
		if($PTGm.enabModTer_FltIslds)
		{
			%FI_B_LUm = $StrArrayHV_FltIsldsBTop[%BrPosX-%BrXYSize,%BrPosY+%BrXYSize];
			%FI_B_RUm = $StrArrayHV_FltIsldsBTop[%BrPosX+%BrXYSize,%BrPosY+%BrXYSize];
			%FI_B_LDm = $StrArrayHV_FltIsldsBTop[%BrPosX-%BrXYSize,%BrPosY-%BrXYSize];
			%FI_B_RDm = $StrArrayHV_FltIsldsBTop[%BrPosX+%BrXYSize,%BrPosY-%BrXYSize];

			%posB = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %BrH_FIb_top; //-%BrZhSize;
			%posStr = %FI_B_Lm SPC %FI_B_Rm SPC %FI_B_Dm SPC %FI_B_Um SPC %FI_B_LUm SPC %FI_B_RUm SPC %FI_B_LDm SPC %FI_B_RDm;
			%brData = $PTGm.fltIsldsTerCol SPC $PTGm.fltIsldsTerPri SPC 0 SPC 0 SPC $PTGm.brFltIslds_DB SPC %BrZSize SPC %FillBrXYZSize SPC "FltIslds";
			%modTerStr = PTG_Chunk_ModTer(false,%posB SPC %posStr,%BrPosX SPC %BrPosY,%brData,%cl,%BG,%Chunk);
			
			//Bottom Layer
			%FI_B_LUmB = $StrArrayHV_FltIsldsBBtm[%BrPosX-%BrXYSize,%BrPosY+%BrXYSize];
			%FI_B_RUmB = $StrArrayHV_FltIsldsBBtm[%BrPosX+%BrXYSize,%BrPosY+%BrXYSize];
			%FI_B_LDmB = $StrArrayHV_FltIsldsBBtm[%BrPosX-%BrXYSize,%BrPosY-%BrXYSize];
			%FI_B_RDmB = $StrArrayHV_FltIsldsBBtm[%BrPosX+%BrXYSize,%BrPosY-%BrXYSize];

			%posB = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %BrH_FIb_btm; //-%BrZhSize;
			%posStr = %FI_B_LmB SPC %FI_B_RmB SPC %FI_B_DmB SPC %FI_B_UmB SPC %FI_B_LUmB SPC %FI_B_RUmB SPC %FI_B_LDmB SPC %FI_B_RDmB;
			%brData = $PTGm.fltIsldsBtmCol SPC $PTGm.fltIsldsBtmPri SPC 0 SPC 0 SPC $PTGm.brFltIslds_DB SPC %BrZSize SPC %FillBrXYZSize SPC "FltIslds";
			%modTerStr = PTG_Chunk_ModTer(true,%posB SPC %posStr,%BrPosX SPC %BrPosY,%brData,%cl,%BG,%Chunk);
			
			//%condA = ">=";
			//%condB = "<=";
			//%condC = "<=";
			//%genTypeA = "PlateFill";
			%fillType = "Cube";
		}
		
		//If Modular Terrain is disabled
		else
		{
			//%condA = ">";
			//%condB = "<";
			//%condC = "<=";
			//%genTypeA = "PlateFill"; //CubeFill
			%fillType = "Disabled";
		}
		
		if((%minAdjFIb_top - %FillBrXYZSize) > (%minAdjFIb_btm + %FillBrXYZSize) && !%adjFIbCut)
		{
			%endPos = mFloor((%minAdjFIb_top - %FillBrXYZSize) / %BrZSize) * %BrZSize;
			
			//Gap-fill top layer
			if((%BrH_FIb_top - %BrZSize) > (%minAdjFIb_top + %BrZSize))
			{
				if($PTGm.dirtSameTer)
				{
					%dirtCol = $PTGm.fltIsldsTerCol;
					%dirtPri = $PTGm.fltIsldsTerPri;
				}
				else
				{
					%dirtCol = $PTGm.fltIsldsDirtCol;
					%dirtPri = $PTGm.fltIsldsDirtPri;
				}
				PTG_Chunk_SubDivideFill(%cl,%Chunk,%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BG,(%BrH_FIb_top - %BrZSize) SPC %endPos SPC %genType,%dirtCol SPC 0 SPC 0 SPC 0 SPC %dirtPri,">",%fillType,"FltIsldsBr");
			}
			
			//Gap-fill bottom layer
			if(((%minAdjFIb_btm + %FillBrXYZSize) - %BrH_FIb_btm) > %FillBrXYZSize)
			{
				if(%BrH_FIb_btm < (%minAdjFIb_btm + %FillBrXYZSize))
					PTG_Chunk_SubDivideFill(%cl,%Chunk,%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BG,%BrH_FIb_btm SPC (%minAdjFIb_btm + %FillBrXYZSize) SPC %genType,$PTGm.fltIsldsBtmCol SPC 0 SPC 0 SPC 0 SPC $PTGm.fltIsldsBtmPri,"<",%fillType,"FltIsldsBr");
			}
			else if(%BrH_FIb_btm > 0)
			{
				%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %BrH_FIb_btm + %BrZhSize;
				PTG_Chunk_PlantBrick($PTGm.brFltIslds_DB,%pos,$PTGm.fltIsldsBtmCol,0,0,0,$PTGm.fltIsldsBtmPri,%cl,%BG,%Chunk,"FltIsldsBr","");
			}
		}
		else //Gap-fill between Floating Island B top and bottom layers
			PTG_Chunk_SubDivideFill(%cl,%Chunk,%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BG,%BrH_FIb_btm SPC (%BrH_FIb_top - %BrZSize + 0.1) SPC "PlateFill",$PTGm.fltIsldsBtmCol SPC 0 SPC 0 SPC 0 SPC $PTGm.fltIsldsBtmPri,"<=",%fillType,"FltIsldsBr");
	}
	
	//////////////////////////////////////////////////
	
	if((%BrPosX += %BrXYSize) >= %ChSize)
	{
		%BrPosX = %BrXYhSize;
		
		if((%BrPosY += %BrXYSize) >= %ChSize)
		{
			PTG_Chunk_Gen_Relay(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"Lakes",%BG);
			return;
		}
		else
		{
			if($PTG.genSpeed < 2)
			{
				$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_genMS,0,50));
				scheduleNoQuota(%delay,0,PTG_Chunk_Gen_FltIslds,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
			}
			else
				PTG_Chunk_Gen_FltIslds(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
			return;
		}
	}
	
	if($PTG.genSpeed == 0)
	{
		$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_genMS,0,50));
		scheduleNoQuota(%delay,0,PTG_Chunk_Gen_FltIslds,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
	}
	else
		PTG_Chunk_Gen_FltIslds(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Chunk_Gen_Lakes(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY) //add cave check //!!!make sure chunk size isn't < water brick size!!!
{
	%genWater = false;
	%genForCaves = false;
	%BrXYSize = $PTGm.brTer_XYsize;
	%BrXYhSize = %BrXYSize / 2;
	%Level_Water = $PTGm.lakesHLevel;
	%lowestHV = $StrArrayHV_Mountains[%BrPosX-8+%BrXYhSize,%BrPosY-8+%BrXYhSize]; //set initial lowest point for water gen
	%ChSize = mClamp($PTGm.chSize,16,256);
	
	//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
	if(isObject(%Chunk) && ((%tempZMax = %Level_Water) > %Chunk.PTGHighestZpos))
		%Chunk.PTGHighestZpos = %tempZMax;

	
	for(%ScanPosY = %BrXYhSize; %ScanPosY < 16; %ScanPosY += %BrXYSize)
	{
		for(%ScanPosX = %BrXYhSize; %ScanPosX < 16; %ScanPosX += %BrXYSize)
		{
			%BrH_cf = $StrArrayHV_Mountains[%BrPosX-8+%ScanPosX,%BrPosY-8+%ScanPosY];
			%CaveH_btm = $StrArrayHV_CavesA[%BrPosX-8+%ScanPosX,%BrPosY-8+%ScanPosY];
			%CaveH_top = $StrArrayHV_CavesB[%BrPosX-8+%ScanPosX,%BrPosY-8+%ScanPosY];
			%caveTerPass = ((%CaveH_top+$PTGm.brTer_Zsize) > (%BrH_cf-$PTGm.brTer_Zsize)) && (%CaveH_btm <= (%BrH_cf-$PTGm.brTer_Zsize));

			//Check for Terrain
			if(%BrH_cf < %lowestHV)
				%lowestHV = %BrH_cf; //adjust lowest height value point for water gen, if lower point found
			if(%BrH_cf < %Level_Water) //!!! issue w/ 130.2 as water level !!!
				%genWater = true;
				
			//Check for Caves
			if($PTGm.enabCaves && %CaveH_top > 0 && ((%CaveH_btm < %Level_Water) && ((%CaveH_top > %Level_Water) || %caveTerPass)) || %genForCaves)
			{
				%genWater = true;
				%genForCaves = true;

				if(%CaveH_btm < %lowestHV && %CaveH_btm > 0) //%CaveH_top > 0 &&  //%CaveH_btm > 0
					%lowestHV = %CaveH_btm; //test!!!
			}	
		}
	}

	//Gen water bricks
	if(%genWater)
	{
		%setRen = 0;
		%startPos = %Level_Water;
		
		%col = $PTGbio.Bio_Def_Wat_Col;
		%pri = $PTGbio.Bio_Def_Wat_Pri;

		switch$($PTGbio.Bio_Def_Wat_Type)
		{
			case "Ice": %brType = "WaterIceBr";
			case "Lava": %brType = "WaterLavaBr";
			case "QuickSand": %brType = "WaterQuickSandBr";
			default: %brType = "WaterBr";
		}

		if($PTGm.enabBio_CustA && $StrArrayHV_CustomBiomeA[%BrPosX-8+%BrXYhSize,%BrPosY-8+%BrXYhSize] < $PTGm.bio_CustASecZ) //Custom Biome A    //!!! Prevent exceeding player's max colors. Otherwise, if a color ID exceeds it, certain bricks using that color won't load.
		{
			%col = $PTGbio.Bio_CustA_Wat_Col;
			%pri = $PTGbio.Bio_CustA_Wat_Pri;

			switch$($PTGbio.Bio_CustA_Wat_Type)
			{
				case "Ice": %brType = "WaterIceBr";
				case "Lava": %brType = "WaterLavaBr";
				case "QuickSand": %brType = "WaterQuickSandBr";
				default: %brType = "WaterBr";
			}
		}
		if($PTGm.enabBio_CustB && $StrArrayHV_CustomBiomeB[%BrPosX-8+%BrXYhSize,%BrPosY-8+%BrXYhSize] < $PTGm.bio_CustBSecZ) //Custom Biome B
		{
			%col = $PTGbio.Bio_CustB_Wat_Col;
			%pri = $PTGbio.Bio_CustB_Wat_Pri;

			switch$($PTGbio.Bio_CustB_Wat_Type)
			{
				case "Ice": %brType = "WaterIceBr";
				case "Lava": %brType = "WaterLavaBr";
				case "QuickSand": %brType = "WaterQuickSandBr";
				default: %brType = "WaterBr";
			}
		}
		if($PTGm.enabBio_CustC && $StrArrayHV_CustomBiomeC[%BrPosX-8+%BrXYhSize,%BrPosY-8+%BrXYhSize] < $PTGm.bio_CustCSecZ) //Custom Biome C
		{
			%col = $PTGbio.Bio_CustC_Wat_Col;
			%pri = $PTGbio.Bio_CustC_Wat_Pri;

			switch$($PTGbio.Bio_CustC_Wat_Type)
			{
				case "Ice": %brType = "WaterIceBr";
				case "Lava": %brType = "WaterLavaBr";
				case "QuickSand": %brType = "WaterQuickSandBr";
				default: %brType = "WaterBr";
			}
		}
		
		for(%waterZrel = 32; %waterZrel >= 4; %waterZrel /= 2)
		{
			switch($PTGm.enabModTer)
			{
				case false:
				
					switch(%waterZrel)
					{
						case 4: %BrDB = "brick32xWaterData";
						case 16: %BrDB = "brick32xCubeWaterPTGData";
						case 32: %BrDB = "brick32xLPillarWaterPTGData";
					}
					
				case true:
				
					switch(%waterZrel)
					{
						case 4: %BrDB = "brick32xWaterModTerData"; //test modter bricks (don't work...)
						case 16: %BrDB = "brick32xCubeWaterModTerPTGData";
						case 32: %BrDB = "brick32xLPillarWaterModTerPTGData"; //lags? disable?
					}
			}
			
			//prevent using large modter water bricks (lags)
			if(%waterZrel != 8) //no datablock for this water brick size
			{
				%waterZrelHalf = %waterZrel / 2;

				for(%waterZ = %startPos; ((%lowestHV < 0 && (%waterZ - %waterZrel) >= 0) || (%waterZ - %waterZrel) > %lowestHV || (%waterZrel == 4 && %waterZ > %lowestHV)) && %waterZ > 0; %waterZ -= %waterZrel)
				{
					%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %waterZ-%waterZrelHalf; //make sure to snap water level to relative brick height grid when routine is started!
					%brick = PTG_Chunk_PlantBrick(%BrDB,%pos,%col,0,2,0,%pri,%cl,%BG,%Chunk,%brType,""); //handled as terrain / indestructible?
					
					if(isObject(%brick))
					{
						switch$(%brType)
						{
							case "WaterIceBr":
							
								%brick.setColliding(1); //doesn't work when only set in PlantBrick function
								%brick.setShapeFX(0);
								
								%brick.setEmitter("FogEmitterA"); //doesn't work when only set in PlantBrick function
									%watPos = getWord(%pos,0) SPC getWord(%pos,1) SPC getWord(%pos,2) + (%waterZrel / 2);
									%brick.emitter.setTransform(%watPos @ " 1 0 0 0");
									%brick.emitter.setScale("16 16 0.1");

							case "WaterLavaBr":
							
								%brick.setColorFX(3); //glow
								%brick.setShapeFX(1);
								%brick.PhysicalZone.waterType = 8; //lava (burns players within zone)
								
								%brick.setEmitter("BurnEmitterA");
									%watPos = getWord(%pos,0) SPC getWord(%pos,1) SPC getWord(%pos,2) + (%waterZrel / 2);
									%brick.emitter.setTransform(%watPos @ " 1 0 0 0");
									%brick.emitter.setScale("16 16 0.1");
							
							case "WaterQuickSandBr":

								%brick.setShapeFX(0);
								%brick.PhysicalZone.extraDrag = 8;
								%brick.PhysicalZone.waterViscosity = 120;
								%brick.PhysicalZone.waterDensity = 0.5;
						}
						
						if(%setRen) 
							%brick.setRendering(0); //set water bricks below water surface to invisible
						%setRen = 1;
					}
				}
				
				%startPos = %waterZ;
			}
		}
	}
	
	//////////////////////////////////////////////////
	
	if((%BrPosX += 16) >= %ChSize)
	{
		%BrPosX = 8;
		
		if((%BrPosY += 16) >= %ChSize)
		{
			PTG_Chunk_Gen_Relay(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"Details",%BG);
			return;
		}
		else
		{
			if($PTG.genSpeed < 2)
			{
				$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_genMS,0,50));// + 0; //(%delay = mClamp($PTG.delay_priFuncMS,0,100));
				scheduleNoQuota(%delay,0,PTG_Chunk_Gen_Lakes,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
			}
			else
				PTG_Chunk_Gen_Lakes(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
			return;
		}
	}
	
	if($PTG.genSpeed == 0)
	{
		$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_genMS,0,50));// + 0; //(%delay = mClamp($PTG.delay_priFuncMS,0,100));
		scheduleNoQuota(%delay,0,PTG_Chunk_Gen_Lakes,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
	}
	else
		PTG_Chunk_Gen_Lakes(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Chunk_Gen_Details_Normal(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY,%ang)
{
	%BrH_build = getWord($StrArrayData_Builds,0); //same as "getField(%var,0)" (?)
	//%rot_build = getWord($StrArrayData_Builds,1); //rot file (check if rot enabled in advance to this func)
	%relGrid = getWord($StrArrayData_Builds,2);
	%relGridHSz = %relGrid / 2;
	%bnds_build = getField($StrArrayData_Builds,1);
	%buildName = getField($StrArrayData_Builds,2);
	//%clrSetsMatch = getField($StrArrayData_Builds,3);
	%flatArPass = ($PTGm.enabBuildLoad || $PTGm.allowFlatAreas) && $StrArrayData_Builds !$= "" && getField($StrArrayData_Builds,4) $= "Terrain"; // || $PTGm.allowFlatAreas
	%ChSize = mClamp($PTGm.chSize,16,256);
	%seed = getSubStr($PTGm.seed,0,8);
	%str = "|(){}[]<>\"\'\\/~`!@#$%^&*_+=,?:;"; //evals in this file don't require same level of security as in Server.cs (be careful who you make super admin however)

	%ChPosX_rel = mFloor(%ChPosX / %relGrid) * %relGrid;
	%ChPosY_rel = mFloor(%ChPosY / %relGrid) * %relGrid;
	%ChPosX_rem = %ChPosX - %ChPosX_rel; //%ChPosX_rel + %relGridHSz;
	%ChPosY_rem = %ChPosY - %ChPosY_rel; //%ChPosY_rel + %relGridHSz;
	
	%BrXYSize = $PTGm.brTer_XYsize; //replace all "$PTGm.brTer_XYsize"
	%BrXYhSize = %BrXYSize / 2;
	%BrZSize = $PTGm.brTer_Zsize;
	%ModTerCheck = !$PTGm.enabModTer || $PTGm.modTerGenType $= "Cubes" || %BrXYSize > 2;
	
	//already rotates boundaries in noise function
	%xMin = getWord(%bnds_build,0);
	%xMax = getWord(%bnds_build,1);
	%yMin = getWord(%bnds_build,2);
	%yMax = getWord(%bnds_build,3);
	//%zMin = getWord(%bnds_build,4);
	//%zMax = getWord(%bnds_build,5);
	
	if($PTGm.enabPlateCap && (!$PTGm.enabModTer || $PTGm.modTerGenType $= "Cubes")) //apply everywhere else where Plate-Capping height offset is used
		%plateOff = 0.2; //Plate-Capping
	
	//Details
	%BrH_init = $StrArrayHV_Terrain[%BrPosX,%BrPosY];
	%BrH_cf = $StrArrayHV_Mountains[%BrPosX,%BrPosY];
	%BrH_cf_aux = $StrArrayHV_MountainsAux[%BrPosX,%BrPosY];
	%BrH_fnl = %BrH_cf;
	%CaveH_btm = $StrArrayHV_CavesA[%BrPosX,%BrPosY];
	%CaveH_top = $StrArrayHV_CavesB[%BrPosX,%BrPosY];
	
	if($PTGm.enabMntns && %BrH_cf > %BrH_init)
		%BrH_fnl = %BrH_cf;
	
	//////////////////////////////////////////////////

	if(!%flatArPass || ($PTGm.allowDetFlatAreas && %buildName $= "") || ((%ChPosX_rem + %BrPosX - %BrXYhSize) >= (%relGridHSz + %xMax)) || ((%ChPosX_rem + %BrPosX + %BrXYhSize) < (%relGridHSz + %xMin))) //was this what was wrong with paths? max should be before min!
	{
		if(!%flatArPass || ($PTGm.allowDetFlatAreas && %buildName $= "") || ((%ChPosY_rem + %BrPosY - %BrXYhSize) >= (%relGridHSz + %yMax)) || ((%ChPosY_rem + %BrPosY + %BrXYhSize) < (%relGridHSz + %yMin))) // //- / + %brHalfSize_terXY
		{			
			//Generate Detail (if within
			if(!$PTGm.enabCaves || %CaveH_top == 0 || ((%CaveH_top + %BrZSize) <= (%BrH_fnl - %BrZSize) || %CaveH_btm > %BrH_fnl)) //use cave section check when actually generating caves (?)
			{
				if($PTGm.enabModTer)
				{
					//ModTer check
					%BrH_L = $StrArrayHV_Mountains[%BrPosX-%BrXYSize,%BrPosY];
					%BrH_R = $StrArrayHV_Mountains[%BrPosX+%BrXYSize,%BrPosY];
					%BrH_D = $StrArrayHV_Mountains[%BrPosX,%BrPosY-%BrXYSize];
					%BrH_U = $StrArrayHV_Mountains[%BrPosX,%BrPosY+%BrXYSize];
					
					%BrH_LU = $StrArrayHV_Mountains[%BrPosX-%BrXYSize,%BrPosY+%BrXYSize];
					%BrH_RU = $StrArrayHV_Mountains[%BrPosX+%BrXYSize,%BrPosY+%BrXYSize];
					%BrH_LD = $StrArrayHV_Mountains[%BrPosX-%BrXYSize,%BrPosY-%BrXYSize];
					%BrH_RD = $StrArrayHV_Mountains[%BrPosX+%BrXYSize,%BrPosY-%BrXYSize];
					
					%BrH = %BrH_fnl;
					%str_LURD = (%BrH_L > %BrH) @ (%BrH_U > %BrH) @ (%BrH_R > %BrH) @ (%BrH_D > %BrH);
					%str_LuRuRdLd = (%BrH_LU > %BrH) @ (%BrH_RU > %BrH) @ (%BrH_RD > %BrH) @ (%BrH_LD > %BrH);
					%ModTerPass = PTG_Chunk_ModTerCheck_Details($PTGm.modTerGenType,%str_LURD,%str_LuRuRdLd);
				}
				else
					%ModTerPass = true;


				%detailStr = PTG_RandNumGen_Details(%randA = (((%CHPosX + %BrPosX + $PTGm.detailsOff_X + 307) * (%CHPosY + %BrPosY + $PTGm.detailsOff_Y + 839) * %BrH_fnl) % 100000)); //remove modulo? //10000
				%detailNum = getWord(%detailStr,0);
				%freq = getWord(%detailStr,1);
				
				if(%freq <= $PTGm.detailFreq)
				{
					//Detail Freq Adjust (common, uncommon and rare gen.)
					if((%detailNum = mClamp(%detailNum,0,83)) < 72)
					{
						if(%detailNum < 48)
							%detailNum = mFloor(%detailNum / 8);
						else
							%detailNum = mFloor(%detailNum / 4) - 6;
					}
					else
						%detailNum -= 60;

					//Choose detail brick datablock, color and print
					%detTmp = PTG_Chunk_FigureColPri_Details(%BrH_fnl,%detailNum,%BrH_cf_aux,%BrH_init,%BrPosX,%BrPosY,%CHPosX,%CHPosY);
					%col = getWord(%detTmp,0);
					%pri = getWord(%detTmp,1);
					%detailDB = getWord(%detTmp,2);
					
					//Generate Detail Brick
					//Terrain Detail (!!!case where terrain not generated?)
					if(%detailDB !$= "" && isObject(%detailDB) && %detailDB.getclassname() $= "fxDTSBrickData")
					{
						switch$(%detailDB.getName())
						{
							//Sylvanor's Tree Support
							case "brickTreeTop11Data" or "brickTreeTop12Data" or "brickTreeTop13aData" or "brickTreeTop13bData" or "brickTreeTop14Data" or "brickTreeTop14FatData" or "brickTreeTop15Data" or "brickTreeTop16Data" or "brickTreeTop17Data":
								
								%randB = (((%randA + 10212011 + %seed) % 99999) * 64577) % 3000;
								%randB = mFloor(%randB / 1000);
								
								switch(%randB)
								{
									case 0:
										%detailDBb = "brickTree1Data";
										%offtrunk = 2.5;
										%offleaves = 5.1;
									case 1:
										%detailDBb = "brickTree2Data";
										%offtrunk = 4.7;
										%offleaves = 9.5;
									case 2:
										%detailDBb = "brickTree3Data";
										%offtrunk = 1.2;
										%offleaves = 2.5;
									default:
										%detailDBb = "brickTree3Data";
										%offtrunk = 1.2;
										%offleaves = 2.5;
								}

								if(%ModTerPass || (((%detailDBb.brickSizeZ * 0.2) + %offleaves + %offtrunk) > (%BrZSize * 3) && %ModTerCheck))
								{
									%brActSizeZ = %detailDB.brickSizeZ * 0.2;
									%DbrH = %BrH_fnl + %offleaves + %plateOff;
									%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %DbrH;
									PTG_Chunk_PlantBrick(%detailDB,%pos,%col,0,0,%ang,%pri,%cl,%BG,%Chunk,"DetailBr",""); //tree leaves
									
									//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
									if(isObject(%Chunk) && ((%tempZMax = %DbrH + (%brActSizeZ / 2)) > %Chunk.PTGHighestZpos))
										%Chunk.PTGHighestZpos = %tempZMax;
			
									
									%DbrH = %BrH_fnl + %offtrunk + %plateOff;
									%randC = (%CHPosX+%BrPosX+$PTGm.detailsOff_X+307) + (%CHPosY+%BrPosY+$PTGm.detailsOff_Y+839) + %DbrH;
									%colTB = (((%randC + 10212011 + %seed) % 99999) * 64577) % 3000;
									%colTB = getWord($PTGm.TreeBaseACol SPC $PTGm.TreeBaseBCol SPC $PTGm.TreeBaseCCol,%colTB / 1000);
									
									%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %DbrH;
									PTG_Chunk_PlantBrick(%detailDBb,%pos,%colTB,0,0,%ang,%pri,%cl,%BG,%Chunk,"DetailBr",""); //tree base
								}
								
							default:
							
								if(%ModTerPass || ((%detailDB.brickSizeZ * 0.2) > (%BrZSize * 3) && %ModTerCheck))
								{
									%brActSizeZ = %detailDB.brickSizeZ * 0.2;
									%DbrH = %BrH_fnl + (%brActSizeZ / 2) + %plateOff;
									%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %DbrH;
									%brick = PTG_Chunk_PlantBrick(%detailDB,%pos,%col,0,0,%ang,%pri,%cl,%BG,%Chunk,"DetailBr","");
									
									//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
									if(isObject(%Chunk) && ((%tempZMax = %DbrH + (%brActSizeZ / 2)) > %Chunk.PTGHighestZpos))
										%Chunk.PTGHighestZpos = %tempZMax;

									
									if(isObject(%brick) && $PTGm.autoHideSpawns && strStr(%brick.getDataBlock().getName(),"Spawn") != -1) //not necessary for Sylvanor's brick above
									{
										%brick.setRendering(0);
										%brick.setColliding(0);
									}
								}
						}
					}
					
					//Increment Pseudo Random Detail Brick Rotation
					if(%ang++ > 3)
						%ang = 0;
				}
			}
		}
	}
	
	//////////////////////////////////////////////////
	//Cave Details
	
	%CaveLyrPass =	$StrArrayHV_CavesB[%BrPosX-%BrXYSize,%BrPosY] > 0 && 
					$StrArrayHV_CavesB[%BrPosX+%BrXYSize,%BrPosY] > 0 && 
					$StrArrayHV_CavesB[%BrPosX,%BrPosY-%BrXYSize] > 0 &&
					$StrArrayHV_CavesB[%BrPosX,%BrPosY+%BrXYSize];

	//CaveA (Bottom) Layer Details
	%caveTerPass = $PTGm.enabCaves && %CaveH_top > 0 && %CaveH_btm <= (%BrH_fnl - %BrZSize); //what if cave detail brick height > terrain?
	
	if(%caveTerPass && %CaveLyrPass) //if($PTGm.enabCaves == true && %CaveH < %Section_Caves && (%CaveH_B + %BrZSize) <= (%BrH - %BrZSize))
	{
		if(%CaveH_btm < %BrH_fnl && %CaveH_btm > 0)
		{
			//ModTer check
			if($PTGm.enabModTer)
			{
				%BrH_L = $StrArrayHV_CavesA[%BrPosX-%BrXYSize,%BrPosY];
				%BrH_R = $StrArrayHV_CavesA[%BrPosX+%BrXYSize,%BrPosY];
				%BrH_D = $StrArrayHV_CavesA[%BrPosX,%BrPosY-%BrXYSize];
				%BrH_U = $StrArrayHV_CavesA[%BrPosX,%BrPosY+%BrXYSize];
				
				%BrH_LU = $StrArrayHV_CavesA[%BrPosX-%BrXYSize,%BrPosY+%BrXYSize];
				%BrH_RU = $StrArrayHV_CavesA[%BrPosX+%BrXYSize,%BrPosY+%BrXYSize];
				%BrH_RD = $StrArrayHV_CavesA[%BrPosX+%BrXYSize,%BrPosY-%BrXYSize];
				%BrH_LD = $StrArrayHV_CavesA[%BrPosX-%BrXYSize,%BrPosY-%BrXYSize];
				
				%BrH = %CaveH_btm;// + %BrZSize;
				%str_LURD = (%BrH_L > %BrH) @ (%BrH_U > %BrH) @ (%BrH_R > %BrH) @ (%BrH_D > %BrH);
				%str_LuRuRdLd = (%BrH_LU > %BrH) @ (%BrH_RU > %BrH) @ (%BrH_RD > %BrH) @ (%BrH_LD > %BrH);
				%ModTerPass = PTG_Chunk_ModTerCheck_Details($PTGm.modTerGenType,%str_LURD,%str_LuRuRdLd);
			}
			else
				%ModTerPass = true;
			
			%detailStr = PTG_RandNumGen_Details(((%CHPosX + %BrPosX + $PTGm.detailsOff_X + 307) * (%CHPosY + %BrPosY + $PTGm.detailsOff_Y + 839) * %CaveH_btm)% 100000); //10000
			%detailNum = getWord(%detailStr,0);
			%freq = getWord(%detailStr,1);
			
			if(%freq <= $PTGm.detailFreq)
			{
				//Detail Freq Adjust (common, uncommon and rare gen.)
				if((%detailNum = mClamp(%detailNum,0,83)) < 72)
				{
					if(%detailNum < 48)
						%detailNum = mFloor(%detailNum / 8);
					else
						%detailNum = mFloor(%detailNum / 4) - 6;
				}
				else
					%detailNum -= 60;

				if(isObject(PTG_MassBioDetails_CaveBottom) && PTG_MassBioDetails_CaveBottom.totalDetAm > 0)
				{
					%rand = (%CHPosX+%BrPosX+$PTGm.detailsOff_X+347) + (%CHPosY+%BrPosY+$PTGm.detailsOff_Y+907) + %CaveH_btm; //change random values???
					%detNum = (((%rand + 10212011 + %seed) % 99999) * 77171) % (PTG_MassBioDetails_CaveBottom.totalDetAm * 100); //mFloor(%rand)
					%detNum = mFloor(%detNum / 100);
					%selDet = PTG_MassBioDetails_CaveBottom.detail[%detNum];

					%col = getWord(%selDet,2);
					%pri = getWord(%selDet,1);
					%detailDBcb = getWord(%selDet,0);
				}
				else
				{
					eval("%col = stripChars($PTGbio.Bio_CaveBtm_Det" @ %detailNum @ "_Col,%str);"); //function characters are removed before evaluation, so these cmds are secure
					eval("%pri = stripChars($PTGbio.Bio_CaveBtm_Det" @ %detailNum @ "_Pri,%str);");
					eval("%detailDBcb = stripChars($PTGbio.Bio_CaveBtm_Det" @ %detailNum @ "_BrDB,%str);");
				}
				
				
				//Cave Detail Bottom Layer
				if(%detailDBcb !$= "" && isObject(%detailDBcb) && %detailDBcb.getclassname() $= "fxDTSBrickData" && (%CaveH_btm + (%detailDBcb.brickSizeZ * 0.1)) <= %CaveH_top)
				{
					switch$(%detailDBcb.getName())
					{
						//Sylvanor's Tree Support
						case "brickTreeTop11Data" or "brickTreeTop12Data" or "brickTreeTop13aData" or "brickTreeTop13bData" or "brickTreeTop14Data" or "brickTreeTop14FatData" or "brickTreeTop15Data" or "brickTreeTop16Data" or "brickTreeTop17Data":
							
							%randB = (((%randA + 10212011 + %seed) % 99999) * 64577) % 3000;
							%randB = mFloor(%randB / 1000);
							
							switch(%randB)
							{
								case 0:
									%detailDBb = "brickTree1Data";
									%offtrunk = 2.5;
									%offleaves = 5.1;
								case 1:
									%detailDBb = "brickTree2Data";
									%offtrunk = 4.7;
									%offleaves = 9.5;
								case 2:
									%detailDBb = "brickTree3Data";
									%offtrunk = 1.2;
									%offleaves = 2.5;
								default:
									%detailDBb = "brickTree3Data";
									%offtrunk = 1.2;
									%offleaves = 2.5;
							}
							
							if(%ModTerPass || (((%detailDBb.brickSizeZ * 0.2) + %offleaves + %offtrunk) > (%BrZSize * 3) && %ModTerCheck))
							{
								%brActSizeZ = %detailDBcb.brickSizeZ * 0.2;
								%DbrH = %CaveH_btm + %offleaves;// + %plateOff;
								%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %DbrH;
								PTG_Chunk_PlantBrick(%detailDBcb,%pos,%col,0,0,%ang,%pri,%cl,%BG,%Chunk,"DetailBr",""); //tree leaves
								
								//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
								if(isObject(%Chunk) && ((%tempZMax = %DbrH + (%brActSizeZ / 2)) > %Chunk.PTGHighestZpos))
									%Chunk.PTGHighestZpos = %tempZMax;
										
								
								%DbrH = %CaveH_btm + %offtrunk;// + %plateOff;
								%randC = (%CHPosX+%BrPosX+$PTGm.detailsOff_X+307) + (%CHPosY+%BrPosY+$PTGm.detailsOff_Y+839) + %DbrH;
								%colTB = (((%randC + 10212011 + %seed) % 99999) * 64577) % 3000;
								%colTB = getWord($PTGm.TreeBaseACol SPC $PTGm.TreeBaseBCol SPC $PTGm.TreeBaseCCol,%colTB / 1000);
								
								%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %DbrH;
								PTG_Chunk_PlantBrick(%detailDBb,%pos,%colTB,0,0,%ang,%pri,%cl,%BG,%Chunk,"DetailBr",""); //tree base
							}
							
						default:
						
							if(%ModTerPass || ((%detailDBcb.brickSizeZ * 0.2) > (%BrZSize * 3) && %ModTerCheck))
							{
								%brActSizeZ = %detailDBcb.brickSizeZ * 0.2;
								%DbrH = %CaveH_btm + (%brActSizeZ / 2);// + %plateOff;
								%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %DbrH;
								%brick = PTG_Chunk_PlantBrick(%detailDBcb,%pos,%col,0,0,%ang,%pri,%cl,%BG,%Chunk,"DetailBr","");
								
								//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
								if(isObject(%Chunk) && ((%tempZMax = %DbrH + (%brActSizeZ / 2)) > %Chunk.PTGHighestZpos))
									%Chunk.PTGHighestZpos = %tempZMax;
										
								
								if(isObject(%brick) && $PTGm.autoHideSpawns && strStr(%brick.getDataBlock().getName(),"Spawn") != -1) //not necessary for Sylvanor's brick above
								{
									%brick.setRendering(0);
									%brick.setColliding(0);
								}
							}
					}
				}
				
				//Increment Pseudo Random Detail Brick Rotation
				if(%ang++ > 3)
					%ang = 0;
			}
		}
		
		
		//CaveB (Top) Layer Details
		if((%CaveH_top + %BrZSize) <= (%BrH_fnl - %BrZSize)) //if(%CaveH_B < %BrH)
		{
			//ModTer check
			if($PTGm.enabModTer)
			{
				%BrH_L = $StrArrayHV_CavesB[%BrPosX-%BrXYSize,%BrPosY];
				%BrH_R = $StrArrayHV_CavesB[%BrPosX+%BrXYSize,%BrPosY];
				%BrH_D = $StrArrayHV_CavesB[%BrPosX,%BrPosY-%BrXYSize];
				%BrH_U = $StrArrayHV_CavesB[%BrPosX,%BrPosY+%BrXYSize];
				
				%BrH_LU = $StrArrayHV_CavesB[%BrPosX-%BrXYSize,%BrPosY+%BrXYSize];
				%BrH_RU = $StrArrayHV_CavesB[%BrPosX+%BrXYSize,%BrPosY+%BrXYSize];
				%BrH_RD = $StrArrayHV_CavesB[%BrPosX+%BrXYSize,%BrPosY-%BrXYSize];
				%BrH_LD = $StrArrayHV_CavesB[%BrPosX-%BrXYSize,%BrPosY-%BrXYSize];
				
				%BrH = %CaveH_top;// + %BrZSize;
				%str_LURD = (%BrH_L < %BrH) @ (%BrH_U < %BrH) @ (%BrH_R < %BrH) @ (%BrH_D < %BrH); //inverted
				%str_LuRuRdLd = (%BrH_LU < %BrH) @ (%BrH_RU < %BrH) @ (%BrH_RD < %BrH) @ (%BrH_LD < %BrH); //inverted
				%ModTerPass = PTG_Chunk_ModTerCheck_Details($PTGm.modTerGenType,%str_LURD,%str_LuRuRdLd);
			}
			else
				%ModTerPass = true;
			
			%detailStr = PTG_RandNumGen_Details(((%CHPosX + %BrPosX + $PTGm.detailsOff_X + 307) * (%CHPosY + %BrPosY + $PTGm.detailsOff_Y + 839) * %CaveH_top) % 100000); //10000
			%detailNum = getWord(%detailStr,0);
			%freq = getWord(%detailStr,1);
			
			if(%freq <= $PTGm.detailFreq)
			{
				//Detail Freq Adjust (common, uncommon and rare gen.)
				if((%detailNum = mClamp(%detailNum,0,83)) < 72)
				{
					if(%detailNum < 48)
						%detailNum = mFloor(%detailNum / 8);
					else
						%detailNum = mFloor(%detailNum / 4) - 6;
				}
				else
					%detailNum -= 60;
				
				if(isObject(PTG_MassBioDetails_CaveTop) && PTG_MassBioDetails_CaveTop.totalDetAm > 0)
				{
					%rand = (%CHPosX+%BrPosX+$PTGm.detailsOff_X+347) + (%CHPosY+%BrPosY+$PTGm.detailsOff_Y+907) + %CaveH_top; //change random values???
					%detNum = (((%rand + 10212011 + %seed) % 99999) * 77171) % (PTG_MassBioDetails_CaveTop.totalDetAm * 100); //mFloor(%rand)
					%detNum = mFloor(%detNum / 100);
					%selDet = PTG_MassBioDetails_CaveTop.detail[%detNum];

					%col = getWord(%selDet,2);
					%pri = getWord(%selDet,1);
					%detailDBct = getWord(%selDet,0);
				}
				else
				{
					eval("%col = stripChars($PTGbio.Bio_CaveTop_Det" @ %detailNum @ "_Col,%str);"); //function characters are removed before evaluation, so these cmds are secure
					eval("%pri = stripChars($PTGbio.Bio_CaveTop_Det" @ %detailNum @ "_Pri,%str);");
					eval("%detailDBct = stripChars($PTGbio.Bio_CaveTop_Det" @ %detailNum @ "_BrDB,%str);");
				}
				
				//Cave Detail Top Layer
				if(%detailDBct !$= "" && isObject(%detailDBct) && %detailDBct.getclassname() $= "fxDTSBrickData" && (%CaveH_top - (%detailDBct.brickSizeZ * 0.1)) >= %CaveH_btm)
				{
					if(%ModTerPass || ((%detailDBct.brickSizeZ * 0.2) > (%BrZSize * 3) && %ModTerCheck))
					{
						%brActSizeZ = %detailDBct.brickSizeZ * 0.2;
						%DbrH = %CaveH_top - (%brActSizeZ / 2);
						%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %DbrH;
						
						%brick = PTG_Chunk_PlantBrick(%detailDBct,%pos,%col,%colFX=0,%shpFX=0,%ang,%pri,%cl,%BG,%Chunk,"DetailBr","");
						
						if(isObject(%brick) && $PTGm.autoHideSpawns && strStr(%brick.getDataBlock().getName(),"Spawn") != -1)
						{
							%brick.setRendering(0);
							%brick.setColliding(0);
						}
					}
				}
				
				//Increment Pseudo Random Detail Brick Rotation
				if(%ang++ > 3)
					%ang = 0;
			}
		}
	}

	//////////////////////////////////////////////////
	
	if((%BrPosX += %BrXYSize) >= %ChSize)
	{
		%BrPosX = %BrXYhSize;
		
		if((%BrPosY += %BrXYSize) >= %ChSize)
		{
			PTG_Chunk_Gen_Relay(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"FltIsldsDetails",%BG);
			return;
		}
		else
		{
			if($PTG.genSpeed < 2)
			{
				$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_genMS,0,50));
				scheduleNoQuota(%delay,0,PTG_Chunk_Gen_Details_Normal,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY,%ang);
			}
			else
				PTG_Chunk_Gen_Details_Normal(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY,%ang);	
			return;
		}
	}

	if($PTG.genSpeed == 0)
	{
		$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_genMS,0,50));
		scheduleNoQuota(%delay,0,PTG_Chunk_Gen_Details_Normal,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY,%ang);
	}
	else
		PTG_Chunk_Gen_Details_Normal(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY,%ang);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Chunk_Gen_Details_SkyLands(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY,%ang)
{
	%BrH_build = getWord($StrArrayData_Builds,0);
	%relGrid = getWord($StrArrayData_Builds,2);
	%relGridHSz = %relGrid / 2;
	%bnds_build = getField($StrArrayData_Builds,1);
	%buildName = getField($StrArrayData_Builds,2);
	%layerName = getField($StrArrayData_Builds,4);
	%flatArPass = ($PTGm.enabBuildLoad || $PTGm.allowFlatAreas) && %layerName $= "Terrain";
	
	%ChPosX_rel = mFloor(%ChPosX / %relGrid) * %relGrid;
	%ChPosY_rel = mFloor(%ChPosY / %relGrid) * %relGrid;
	%ChPosX_rem = %ChPosX - %ChPosX_rel; //%ChPosX_rel + %relGridHSz;
	%ChPosY_rem = %ChPosY - %ChPosY_rel; //%ChPosY_rel + %relGridHSz;
	%BrXYSize = $PTGm.brTer_XYsize;
	%BrXYhSize = %BrXYSize / 2;
	%BrZSize = $PTGm.brTer_Zsize;
	%Level_TerOff = $PTGm.terHLevel;
	%SecZ = $PTGm.skyLndsSecZ;
	%ChSize = mClamp($PTGm.chSize,16,256);
	%seed = getSubStr($PTGm.seed,0,8);
	%str = "|(){}[]<>\"\'\\/~`!@#$%^&*_+=,?:;"; //evals in this file don't require same level of security as in Server.cs (be careful who you make super admin however)
	%ModTerCheck = !$PTGm.enabModTer || $PTGm.modTerGenType $= "Cubes" || %BrXYSize > 2;
	
	%xMin = getWord(%bnds_build,0);
	%xMax = getWord(%bnds_build,1);
	%yMin = getWord(%bnds_build,2);
	%yMax = getWord(%bnds_build,3);
	
	//%Section_SkyLands_act = $PTGm.skyLndsSecZ + $PTGm.terHLevel;
	//%Section_SkyLands_nom = $PTGm.skyLndsSecZ;
	%MinBrZSnap = $PTGm.brTer_Zsize; //$PTGm.brTer_FillXYZSize + $PTGm.brTer_Zsize; //%MinBrZSnap = getWord($PTGMAIN,7) / 2;
	
	//Plate-Capping
	if($PTGm.enabPlateCap) 
		%plateOff = 0.2;
	
	%BrH = $StrArrayHV_Terrain[%BrPosX,%BrPosY];
	//%BrH_SL = $StrArrayHV_SkyLandsTop[%BrPosX,%BrPosY];
	%BrH_SL_top = $StrArrayHV_SkyLandsTop[%BrPosX,%BrPosY];
	%BrH_SL_btm = $StrArrayHV_SkyLandsBtm[%BrPosX,%BrPosY];
	%BrH_cf = $StrArrayHV_Mountains[%BrPosX,%BrPosY];
	%BrH_cf_aux = $StrArrayHV_MountainsAux[%BrPosX,%BrPosY];
	//%BrH_act = %BrH_cf - %Level_TerOff; //%BrH_SL_top - %Level_TerOff; //? //brh_init? //if error replace brhfl with brh_init as before
	//%BrH_Res = %BrH_cf + %BrH_SL_top;
	
	//////////////////////////////////////////////////

	if(!%flatArPass || ($PTGm.allowDetFlatAreas && %buildName $= "") || ((%ChPosX_rem + %BrPosX - %BrXYhSize) >= (%relGridHSz + %xMax)) || ((%ChPosX_rem + %BrPosX + %BrXYhSize) < (%relGridHSz + %xMin))) //was this what was wrong with paths? max should be before min!
	{
		if(!%flatArPass || ($PTGm.allowDetFlatAreas && %buildName $= "") || ((%ChPosY_rem + %BrPosY - %BrXYhSize) >= (%relGridHSz + %yMax)) || ((%ChPosY_rem + %BrPosY + %BrXYhSize) < (%relGridHSz + %yMin))) // //- / + %brHalfSize_terXY
		{
			//Generate Detail Bricks (if terrain brick generated under or cave present)
			if((%BrH_cf - %Level_TerOff) > %SecZ && (%BrH_SL_top - $PTGm.brTer_Zsize) >= (%BrH_SL_btm + $PTGm.brTer_Zsize))
			{
				if($PTGm.enabModTer)
				{
					//ModTer check (Edges)
					%SL_Lm = $StrArrayHV_SkyLandsTop[%BrPosX-%BrXYSize,%BrPosY];
					%SL_Rm = $StrArrayHV_SkyLandsTop[%BrPosX+%BrXYSize,%BrPosY];
					%SL_Dm = $StrArrayHV_SkyLandsTop[%BrPosX,%BrPosY-%BrXYSize];
					%SL_Um = $StrArrayHV_SkyLandsTop[%BrPosX,%BrPosY+%BrXYSize];
					
					%SL_LUm = $StrArrayHV_SkyLandsTop[%BrPosX-%BrXYSize,%BrPosY+%BrXYSize];
					%SL_RUm = $StrArrayHV_SkyLandsTop[%BrPosX+%BrXYSize,%BrPosY+%BrXYSize];
					%SL_LDm = $StrArrayHV_SkyLandsTop[%BrPosX-%BrXYSize,%BrPosY-%BrXYSize];
					%SL_RDm = $StrArrayHV_SkyLandsTop[%BrPosX+%BrXYSize,%BrPosY-%BrXYSize];

					%BrH = %BrH_SL_top; //%BrH_fnl;// + %BrZSize;
					%str_LURD = (%SL_Lm > %BrH) @ (%SL_Um > %BrH) @ (%SL_Rm > %BrH) @ (%SL_Um > %BrH);
					%str_LuRuRdLd = (%SL_LUm > %BrH) @ (%SL_RUm > %BrH) @ (%SL_RDm > %BrH) @ (%SL_LDm > %BrH);
					%ModTerPass = PTG_Chunk_ModTerCheck_Details($PTGm.modTerGenType,%str_LURD,%str_LuRuRdLd); //floating islands modter gen type
				}
				else
					%ModTerPass = true;
			
				%detailStr = PTG_RandNumGen_Details(((%CHPosX + %BrPosX + $PTGm.detailsOff_X + 307) * (%CHPosY + %BrPosY + $PTGm.detailsOff_Y + 839) * %BrH_SL_top) % 100000); //10000 //%detailStr = PTG_RandNumGen_Details((%BrPosX * %BrPosY) + %BrH);
				%detailNum = getWord(%detailStr,0);
				%freq = getWord(%detailStr,1);
				
				//Detail Freq Adjust (common, uncommon and rare gen.)
				if((%detailNum = mClamp(%detailNum,0,83)) < 72)
				{
					if(%detailNum < 48)
						%detailNum = mFloor(%detailNum / 8);
					else
						%detailNum = mFloor(%detailNum / 4) - 6;
				}
				else
					%detailNum -= 60;
				
				%tempH = %BrH_SL_top;//getMax(%BrH_Res + (%BrH_act - %SecZ),%MinBrZSnap);

				//Choose detail brick datablock, color and print
				%detTmp = PTG_Chunk_FigureColPri_Details(%tempH,%detailNum,%BrH_cf_aux,%BrH,%BrPosX,%BrPosY,%CHPosX,%CHPosY); //%BrH_ter,%BrH_cf_aux,%BrH_init,
				%col = getWord(%detTmp,0);
				%pri = getWord(%detTmp,1);
				%detailDB = getWord(%detTmp,2);
				
				//Terrain Detail (!!!case where terrain not generated?)
				if(%freq <= $PTGm.detailFreq)
				{
					if(%detailDB !$= "" && isObject(%detailDB) && %detailDB.getclassname() $= "fxDTSBrickData")
					{
						switch$(%detailDB.getName())
						{
							//Sylvanor's Tree Support
							case "brickTreeTop11Data" or "brickTreeTop12Data" or "brickTreeTop13aData" or "brickTreeTop13bData" or "brickTreeTop14Data" or "brickTreeTop14FatData" or "brickTreeTop15Data" or "brickTreeTop16Data" or "brickTreeTop17Data":
								
								%randB = (((%randA + 10212011 + %seed) % 99999) * 64577) % 3000;
								%randB = mFloor(%randB / 1000);
								
								switch(%randB)
								{
									case 0:
										%detailDBb = "brickTree1Data";
										%offtrunk = 2.5;
										%offleaves = 5.1;
									case 1:
										%detailDBb = "brickTree2Data";
										%offtrunk = 4.7;
										%offleaves = 9.5;
									case 2:
										%detailDBb = "brickTree3Data";
										%offtrunk = 1.2;
										%offleaves = 2.5;
									default:
										%detailDBb = "brickTree3Data";
										%offtrunk = 1.2;
										%offleaves = 2.5;
								}
								
								if(%ModTerPass || ((%detailDBb.brickSizeZ * 0.2) > (%BrZSize * 3) && %ModTerCheck))
								{
									%brActSizeZ = %detailDB.brickSizeZ * 0.2;
									%DbrH = %tempH + %offleaves + %plateOff;
									%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %DbrH;
									PTG_Chunk_PlantBrick(%detailDB,%pos,%col,0,0,%ang,%pri,%cl,%BG,%Chunk,"DetailBr",""); //tree leaves
									
									//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
									if(isObject(%Chunk) && ((%tempZMax = %DbrH + (%brActSizeZ / 2)) > %Chunk.PTGHighestZpos))
										%Chunk.PTGHighestZpos = %tempZMax;
										

									%DbrH = %tempH + %offtrunk + %plateOff;
									%randC = (%CHPosX+%BrPosX+$PTGm.detailsOff_X+307) + (%CHPosY+%BrPosY+$PTGm.detailsOff_Y+839) + %DbrH;
									%colTB = (((%randC + 10212011 + %seed) % 99999) * 64577) % 3000;
									%colTB = getWord($PTGm.TreeBaseACol SPC $PTGm.TreeBaseBCol SPC $PTGm.TreeBaseCCol,%colTB / 1000);
									
									%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %DbrH;
									PTG_Chunk_PlantBrick(%detailDBb,%pos,%colTB,0,0,%ang,%pri,%cl,%BG,%Chunk,"DetailBr",""); //tree base
								}
								
							default:
							
								if(%ModTerPass || ((%detailDB.brickSizeZ * 0.2) > (%BrZSize * 3) && %ModTerCheck))
								{
									%brActSizeZ = %detailDB.brickSizeZ * 0.2;
									%DbrH = %tempH + (%brActSizeZ / 2) + %plateOff;
									%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %DbrH;
									%brick = PTG_Chunk_PlantBrick(%detailDB,%pos,%col,0,0,%ang,%pri,%cl,%BG,%Chunk,"DetailBr","");
									
									//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
									if(isObject(%Chunk) && ((%tempZMax = %DbrH + (%brActSizeZ / 2)) > %Chunk.PTGHighestZpos))
										%Chunk.PTGHighestZpos = %tempZMax;
										
									
									if(isObject(%brick) && $PTGm.autoHideSpawns && strStr(%brick.getDataBlock().getName(),"Spawn") != -1) //not necessary for Sylvanor's brick above
									{
										%brick.setRendering(0);
										%brick.setColliding(0);
									}
								}
						}
					}
				}
			}
		}
	}
	
	//Increment Pseudo Random Detail Brick Rotation
	if(%ang++ > 3)
		%ang = 0;

	//////////////////////////////////////////////////
	
	if((%BrPosX += %BrXYSize) >= %ChSize)
	{
		%BrPosX = %BrXYSize / 2;
		
		if((%BrPosY += %BrXYSize) >= %ChSize)
		{
			PTG_Chunk_Gen_Relay(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"FltIsldsDetails",%BG);
			return;
		}
		else
		{
			if($PTG.genSpeed < 2)
			{
				$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_genMS,0,50));
				scheduleNoQuota(%delay,0,PTG_Chunk_Gen_Details_SkyLands,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY,%ang);
			}
			else
				PTG_Chunk_Gen_Details_SkyLands(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY,%ang);
			return;
		}
	}
	
	if($PTG.genSpeed == 0)
	{
		$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_genMS,0,50));
		scheduleNoQuota(%delay,0,PTG_Chunk_Gen_Details_SkyLands,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY,%ang);
	}
	else
		PTG_Chunk_Gen_Details_SkyLands(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY,%ang);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Chunk_Gen_Details_FltIslds(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY,%ang)
{
	%BrH_build = getWord($StrArrayData_Builds,0);
	%relGrid = getWord($StrArrayData_Builds,2);
	%relGridHSz = %relGrid / 2;
	%bnds_build = getField($StrArrayData_Builds,1);
	%buildName = getField($StrArrayData_Builds,2);
	%layerName = getField($StrArrayData_Builds,4);
	%flatArPass = ($PTGm.enabBuildLoad || $PTGm.allowFlatAreas) && (%layerName $= "FltIsldA" || %layerName $= "FltIsldB");
	%FIMDpass = isObject(PTG_MassBioDetails_Default) && PTG_MassBioDetails_Default.totalDetAm > 0;
		%massDetAm = PTG_MassBioDetails_Default.totalDetAm;
	%str = "|(){}[]<>\"\'\\/~`!@#$%^&*_+=,?:;"; //evals in this file don't require same level of security as in Server.cs (be careful who you make super admin however)

	%ChPosX_rel = mFloor(%ChPosX / %relGrid) * %relGrid;
	%ChPosY_rel = mFloor(%ChPosY / %relGrid) * %relGrid;
	%ChPosX_rem = %ChPosX - %ChPosX_rel; //%ChPosX_rel + %relGridHSz;
	%ChPosY_rem = %ChPosY - %ChPosY_rel; //%ChPosY_rel + %relGridHSz;
	%BrXYSize = $PTGm.brFltIslds_XYsize;
	%BrXYhSize = %BrXYSize / 2;
	%BrZSize = $PTGm.brFltIslds_Zsize;
	%ChSize = mClamp($PTGm.chSize,16,256);
	%seed = getSubStr($PTGm.seed,0,8);
	
	%ModTerCheck = !$PTGm.enabModTer_FltIslds || $PTGm.modTerGenType_fltislds $= "Cubes" || %BrXYSize > 2;
	
	//Boundaries (already rotates boundaries in noise function)
	%xMin = getWord(%bnds_build,0);
	%xMax = getWord(%bnds_build,1);
	%yMin = getWord(%bnds_build,2);
	%yMax = getWord(%bnds_build,3);
	
	//if($PTGm.enabPlateCap) 
	//	%plateOff = 0.2; //Plate-Capping (doesn't apply to floating islands)
	
	//////////////////////////////////////////////////

	if(!%flatArPass || ($PTGm.allowDetFlatAreas && %buildName $= "") || ((%ChPosX_rem + %BrPosX - %BrXYhSize) >= (%relGridHSz + %xMax)) || ((%ChPosX_rem + %BrPosX + %BrXYhSize) < (%relGridHSz + %xMin))) //was this what was wrong with paths? max should be before min!
	{
		if(!%flatArPass || ($PTGm.allowDetFlatAreas && %buildName $= "") || ((%ChPosY_rem + %BrPosY - %BrXYhSize) >= (%relGridHSz + %yMax)) || ((%ChPosY_rem + %BrPosY + %BrXYhSize) < (%relGridHSz + %yMin))) // //- / + %brHalfSize_terXY
		{
			%BrH_FIa_top = $StrArrayHV_FltIsldsATop[%BrPosX,%BrPosY];
			%BrH_FIa_btm = $StrArrayHV_FltIsldsABtm[%BrPosX,%BrPosY];
	
			//Floating Island A
			if(%BrH_FIa_top > 0 && (%BrH_FIa_top - %BrZSize) >= (%BrH_FIa_btm + %BrZSize))
			{
				//ModTer check (Edges)
				if($PTGm.enabModTer_FltIslds)
				{
					%FI_A_Lm = $StrArrayHV_FltIsldsATop[%BrPosX-%BrXYSize,%BrPosY];
					%FI_A_Rm = $StrArrayHV_FltIsldsATop[%BrPosX+%BrXYSize,%BrPosY];
					%FI_A_Dm = $StrArrayHV_FltIsldsATop[%BrPosX,%BrPosY-%BrXYSize];
					%FI_A_Um = $StrArrayHV_FltIsldsATop[%BrPosX,%BrPosY+%BrXYSize];
					
					%FI_A_LUm = $StrArrayHV_FltIsldsATop[%BrPosX-%BrXYSize,%BrPosY+%BrXYSize];
					%FI_A_RUm = $StrArrayHV_FltIsldsATop[%BrPosX+%BrXYSize,%BrPosY+%BrXYSize];
					%FI_A_LDm = $StrArrayHV_FltIsldsATop[%BrPosX-%BrXYSize,%BrPosY-%BrXYSize];
					%FI_A_RDm = $StrArrayHV_FltIsldsATop[%BrPosX+%BrXYSize,%BrPosY-%BrXYSize];
					
					%BrH = %BrH_FIa_top;
					%str_LURD = (%FI_A_Lm > %BrH) @ (%FI_A_Um > %BrH) @ (%FI_A_Rm > %BrH) @ (%FI_A_Dm > %BrH);
					%str_LuRuRdLd = (%FI_A_LUm > %BrH) @ (%FI_A_RUm > %BrH) @ (%FI_A_RDm > %BrH) @ (%FI_A_LDm > %BrH);
					%ModTerPassA = PTG_Chunk_ModTerCheck_Details($PTGm.modTerGenType_fltislds,%str_LURD,%str_LuRuRdLd);
				}
				else
					%ModTerPassA = true;
				

				%detailStr = PTG_RandNumGen_Details(%randA = (((%CHPosX + %BrPosX + $PTGm.detailsOff_X + 307) * (%CHPosY + %BrPosY + $PTGm.detailsOff_Y + 839) * %BrH_FIa_top) % 100000)); //remove modulo? //10000
				%detailNum = getWord(%detailStr,0);
				%freq = getWord(%detailStr,1);

				if(%freq <= $PTGm.detailFreq)
				{
					//Detail Freq Adjust (common, uncommon and rare gen.)
					if((%detailNum = mClamp(%detailNum,0,83)) < 72)
					{
						if(%detailNum < 48)
							%detailNum = mFloor(%detailNum / 8);
						else
							%detailNum = mFloor(%detailNum / 4) - 6;
					}
					else
						%detailNum -= 60;
					
					if(!%FIMDpass)
					{
						//Choose detail brick datablock, color and print
						eval("%col = stripChars($PTGbio.Bio_Def_Det" @ %detailNum @ "_Col,%str);");
						eval("%pri = stripChars($PTGbio.Bio_Def_Det" @ %detailNum @ "_Pri,%str);");
						eval("%detailDB = stripChars($PTGbio.Bio_Def_Det" @ %detailNum @ "_BrDB,%str);");
					}
					else
					{
						%rand = (%CHPosX+%BrPosX+$PTGm.detailsOff_X+347) + (%CHPosY+%BrPosY+$PTGm.detailsOff_Y+907) + %BrH_FIa_top; //change random values???
						%detNum = (((%rand + 10212011 + %seed) % 99999) * 77171) % (%massDetAm * 100); //mFloor(%rand)
						%detNum = mFloor(%detNum / 100);
						%selDet = PTG_MassBioDetails_Default.detail[%detNum];

						%col = getWord(%selDet,2);
						%pri = getWord(%selDet,1);
						%detailDB = getWord(%selDet,0);
					}


					//Generate Detail Brick
					//Terrain Detail (!!!case where terrain not generated?)
					if(%detailDB !$= "" && isObject(%detailDB) && %detailDB.getclassname() $= "fxDTSBrickData")
					{
						switch$(%detailDB.getName())
						{
							//Sylvanor's Tree Support
							case "brickTreeTop11Data" or "brickTreeTop12Data" or "brickTreeTop13aData" or "brickTreeTop13bData" or "brickTreeTop14Data" or "brickTreeTop14FatData" or "brickTreeTop15Data" or "brickTreeTop16Data" or "brickTreeTop17Data":
								
								%randB = (((%randA + 10212011 + %seed) % 99999) * 64577) % 3000;
								%randB = mFloor(%randB / 1000);
								
								switch(%randB)
								{
									case 0:
										%detailDBb = "brickTree1Data";
										%offtrunk = 2.5;
										%offleaves = 5.1;
									case 1:
										%detailDBb = "brickTree2Data";
										%offtrunk = 4.7;
										%offleaves = 9.5;
									case 2:
										%detailDBb = "brickTree3Data";
										%offtrunk = 1.2;
										%offleaves = 2.5;
									default:
										%detailDBb = "brickTree3Data";
										%offtrunk = 1.2;
										%offleaves = 2.5;
								}
								
								if(%ModTerPassA || ((%detailDBb.brickSizeZ * 0.2) > (%BrZSize * 3) && %ModTerCheck))
								{
									%brActSizeZ = %detailDB.brickSizeZ * 0.2;
									%DbrH = %BrH_FIa_top + %offleaves + %plateOff;
									%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %DbrH;
									PTG_Chunk_PlantBrick(%detailDB,%pos,%col,0,0,%ang,%pri,%cl,%BG,%Chunk,"DetailBr",""); //tree leaves
									
									//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
									if(isObject(%Chunk) && ((%tempZMax = %DbrH + (%brActSizeZ / 2)) > %Chunk.PTGHighestZpos))
										%Chunk.PTGHighestZpos = %tempZMax;
										
									
									%DbrH = %BrH_FIa_top + %offtrunk + %plateOff;
									%randC = (%CHPosX+%BrPosX+$PTGm.detailsOff_X+307) + (%CHPosY+%BrPosY+$PTGm.detailsOff_Y+839) + %DbrH;
									%colTB = (((%randC + 10212011 + %seed) % 99999) * 64577) % 3000;
									%colTB = getWord($PTGm.TreeBaseACol SPC $PTGm.TreeBaseBCol SPC $PTGm.TreeBaseCCol,%colTB / 1000);

									%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %DbrH;
									PTG_Chunk_PlantBrick(%detailDBb,%pos,%colTB,0,0,%ang,%pri,%cl,%BG,%Chunk,"DetailBr",""); //tree base
								}
								
							default:

								if(%ModTerPassA || ((%detailDB.brickSizeZ * 0.2) > (%BrZSize * 3) && %ModTerCheck))
								{
									%brActSizeZ = %detailDB.brickSizeZ * 0.2;
									%DbrH = %BrH_FIa_top + (%brActSizeZ / 2) + %plateOff;
									%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %DbrH;
									%brick = PTG_Chunk_PlantBrick(%detailDB,%pos,%col,0,0,%ang,%pri,%cl,%BG,%Chunk,"DetailBr","");
									
									//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
									if(isObject(%Chunk) && ((%tempZMax = %DbrH + (%brActSizeZ / 2)) > %Chunk.PTGHighestZpos))
										%Chunk.PTGHighestZpos = %tempZMax;
										
									
									if(isObject(%brick) && $PTGm.autoHideSpawns && strStr(%brick.getDataBlock().getName(),"Spawn") != -1) //not necessary for Sylvanor's brick above
									{
										%brick.setRendering(0);
										%brick.setColliding(0);
									}
								}
						}
					}
				}
			}
			
			//Increment Pseudo Random Detail Brick Rotation
			if(%ang++ > 3)
				%ang = 0; //two of these in this function, check if works correctly / well enough
			
			//////////////////////////////////////////////////
			
			%BrH_FIb_top = $StrArrayHV_FltIsldsBTop[%BrPosX,%BrPosY];
			%BrH_FIb_btm = $StrArrayHV_FltIsldsBBtm[%BrPosX,%BrPosY];
			
			//Floating Island B
			if(%BrH_FIb_top > 0 && (%BrH_FIb_top - %BrZSize) >= (%BrH_FIb_btm + %BrZSize))
			{
				//ModTer check (Edges)
				if($PTGm.enabModTer_FltIslds)
				{
					%FI_B_Lm = $StrArrayHV_FltIsldsBTop[%BrPosX-%BrXYSize,%BrPosY];
					%FI_B_Rm = $StrArrayHV_FltIsldsBTop[%BrPosX+%BrXYSize,%BrPosY];
					%FI_B_Dm = $StrArrayHV_FltIsldsBTop[%BrPosX,%BrPosY-%BrXYSize];
					%FI_B_Um = $StrArrayHV_FltIsldsBTop[%BrPosX,%BrPosY+%BrXYSize];
					
					%FI_B_LUm = $StrArrayHV_FltIsldsBTop[%BrPosX-%BrXYSize,%BrPosY+%BrXYSize];
					%FI_B_RUm = $StrArrayHV_FltIsldsBTop[%BrPosX+%BrXYSize,%BrPosY+%BrXYSize];
					%FI_B_LDm = $StrArrayHV_FltIsldsBTop[%BrPosX-%BrXYSize,%BrPosY-%BrXYSize];
					%FI_B_RDm = $StrArrayHV_FltIsldsBTop[%BrPosX+%BrXYSize,%BrPosY-%BrXYSize];
					
					%BrH = %BrH_FIb_top;
					%str_LURD = (%FI_B_Lm > %BrH) @ (%FI_B_Um > %BrH) @ (%FI_B_Rm > %BrH) @ (%FI_B_Dm > %BrH);
					%str_LuRuRdLd = (%FI_B_LUm > %BrH) @ (%FI_B_RUm > %BrH) @ (%FI_B_RDm > %BrH) @ (%FI_B_LDm > %BrH);					
					%ModTerPassB = PTG_Chunk_ModTerCheck_Details($PTGm.modTerGenType_fltislds,%str_LURD,%str_LuRuRdLd);
				}
				else
					%ModTerPassB = true;
				
				
				%detailStr = PTG_RandNumGen_Details(%randA = (((%CHPosX + %BrPosX + $PTGm.detailsOff_X + 307) * (%CHPosY + %BrPosY + $PTGm.detailsOff_Y + 839) * %BrH_FIb_top) % 100000)); //remove modulo? //10000
				%detailNum = getWord(%detailStr,0);
				%freq = getWord(%detailStr,1);
				%detailDB = ""; //make sure script doesn't accidentally carry over past detail datablock from island A

				if(%freq <= $PTGm.detailFreq)
				{
					//Detail Freq Adjust (common, uncommon and rare gen.)
					if((%detailNum = mClamp(%detailNum,0,83)) < 72)
					{
						if(%detailNum < 48)
							%detailNum = mFloor(%detailNum / 8);
						else
							%detailNum = mFloor(%detailNum / 4) - 6;
					}
					else
						%detailNum -= 60;


					if(!%FIMDpass)
					{
						//Choose detail brick datablock, color and print
						eval("%col = stripChars($PTGbio.Bio_Def_Det" @ %detailNum @ "_Col,%str);");
						eval("%pri = stripChars($PTGbio.Bio_Def_Det" @ %detailNum @ "_Pri,%str);");
						eval("%detailDB = stripChars($PTGbio.Bio_Def_Det" @ %detailNum @ "_BrDB,%str);");
					}
					else
					{
						%rand = (%CHPosX+%BrPosX+$PTGm.detailsOff_X+347) + (%CHPosY+%BrPosY+$PTGm.detailsOff_Y+907) + %BrH_FIb_top;
						%detNum = (((%rand + 10212011 + %seed) % 99999) * 77171) % (%massDetAm * 100);
						%detNum = mFloor(%detNum / 100);
						%selDet = PTG_MassBioDetails_Default.detail[%detNum];

						%col = getWord(%selDet,2);
						%pri = getWord(%selDet,1);
						%detailDB = getWord(%selDet,0);
					}


					//Generate Detail Brick
					//Terrain Detail (!!!case where terrain not generated?)
					if(%detailDB !$= "" && isObject(%detailDB) && %detailDB.getclassname() $= "fxDTSBrickData")
					{
						switch$(%detailDB.getName())
						{
							//Sylvanor's Tree Support
							case "brickTreeTop11Data" or "brickTreeTop12Data" or "brickTreeTop13aData" or "brickTreeTop13bData" or "brickTreeTop14Data" or "brickTreeTop14FatData" or "brickTreeTop15Data" or "brickTreeTop16Data" or "brickTreeTop17Data":
								
								%randB = (((%randA + 10212011 + %seed) % 99999) * 64577) % 3000;
								%randB = mFloor(%randB / 1000);
								
								switch(%randB)
								{
									case 0:
										%detailDBb = "brickTree1Data";
										%offtrunk = 2.5;
										%offleaves = 5.1;
									case 1:
										%detailDBb = "brickTree2Data";
										%offtrunk = 4.7;
										%offleaves = 9.5;
									case 2:
										%detailDBb = "brickTree3Data";
										%offtrunk = 1.2;
										%offleaves = 2.5;
									default:
										%detailDBb = "brickTree3Data";
										%offtrunk = 1.2;
										%offleaves = 2.5;
								}
								
								if(%ModTerPassB || ((%detailDBb.brickSizeZ * 0.2) > (%BrZSize * 3) && %ModTerCheck))
								{
									%brActSizeZ = %detailDB.brickSizeZ * 0.2;
									%DbrH = %BrH_FIb_top + %offleaves + %plateOff;
									%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %DbrH;
									PTG_Chunk_PlantBrick(%detailDB,%pos,%col,0,0,%ang,%pri,%cl,%BG,%Chunk,"DetailBr",""); //tree leaves
									
									//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
									if(isObject(%Chunk) && ((%tempZMax = %DbrH + (%brActSizeZ / 2)) > %Chunk.PTGHighestZpos))
										%Chunk.PTGHighestZpos = %tempZMax;
										
									
									%DbrH = %BrH_FIb_top + %offtrunk + %plateOff;
									%randC = (%CHPosX+%BrPosX+$PTGm.detailsOff_X+307) + (%CHPosY+%BrPosY+$PTGm.detailsOff_Y+839) + %DbrH;
									%colTB = (((%randC + 10212011 + %seed) % 99999) * 64577) % 3000;
									%colTB = getWord($PTGm.TreeBaseACol SPC $PTGm.TreeBaseBCol SPC $PTGm.TreeBaseCCol,%colTB / 1000);

									%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %DbrH;
									PTG_Chunk_PlantBrick(%detailDBb,%pos,%colTB,0,0,%ang,%pri,%cl,%BG,%Chunk,"DetailBr",""); //tree base
								}
								
							default:
							
								if(%ModTerPassB || ((%detailDB.brickSizeZ * 0.2) > (%BrZSize * 3) && %ModTerCheck))
								{
									%brActSizeZ = %detailDB.brickSizeZ * 0.2;
									%DbrH = %BrH_FIb_top + (%brActSizeZ / 2) + %plateOff;
									%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %DbrH;
									%brick = PTG_Chunk_PlantBrick(%detailDB,%pos,%col,0,0,%ang,%pri,%cl,%BG,%Chunk,"DetailBr","");
									
									//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
									if(isObject(%Chunk) && ((%tempZMax = %BrH_FIb_top + (%brActSizeZ / 2)) > %Chunk.PTGHighestZpos))
										%Chunk.PTGHighestZpos = %tempZMax;
										
									
									if(isObject(%brick) && $PTGm.autoHideSpawns && strStr(%brick.getDataBlock().getName(),"Spawn") != -1) //not necessary for Sylvanor's brick above
									{
										%brick.setRendering(0);
										%brick.setColliding(0);
									}
								}
						}
					}
				}
			}
			
			//Increment Pseudo Random Detail Brick Rotation
			if(%ang++ > 3)
				%ang = 0; //two of these in this function, check if works correctly / well enough
		}
	}

	//////////////////////////////////////////////////
	
	if((%BrPosX += %BrXYSize) >= %ChSize)
	{
		%BrPosX = %BrXYhSize;
		
		if((%BrPosY += %BrXYSize) >= %ChSize)
		{
			PTG_Chunk_Gen_Relay(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"Clouds",%BG);
			return;
		}
		else
		{
			if($PTG.genSpeed < 2)
			{
				$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_genMS,0,50));
				scheduleNoQuota(%delay,0,PTG_Chunk_Gen_Details_FltIslds,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY,%ang);
			}
			else
				PTG_Chunk_Gen_Details_FltIslds(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY,%ang);
			return;
		}
	}
	
	if($PTG.genSpeed == 0)
	{
		$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_genMS,0,50));
		scheduleNoQuota(%delay,0,PTG_Chunk_Gen_Details_FltIslds,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY,%ang);
	}
	else
		PTG_Chunk_Gen_Details_FltIslds(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY,%ang);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Chunk_Gen_Clouds(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY) //Add Terrain Offset
{
	%CloudH = $StrArrayHV_Clouds[%BrPosX,%BrPosY];
	%BrZSize = $PTGm.brClouds_Zsize;
	%BrZhSize = %BrZSize / 2;
	%BrXYSize = $PTGm.brClouds_XYsize;
	%BrXYhSize = %BrXYSize / 2;
	%SecZ = $PTGm.cloudsSecZ;
	%Level_CloudOff = $PTGm.cloudsHLevel;
	%ChSize = mClamp($PTGm.chSize,16,256);
	%FillBrXYZSize = $PTGm.brClouds_FillXYZSize;
	
	if(%FillBrXYZSize < 1 || $PTGm.enabModTer_Clouds)
		%genType = "PlateFill";
	else
		%genType = "CubeFill";

	//////////////////////////////////////////////////
	
	if(%CloudH > %SecZ)
	{
		//Cloud Brick (Bottom)
		%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %Level_CloudOff+%BrZhSize;
		
		%CloudHLm = (mFloor(($StrArrayHV_Clouds[%BrPosX-%BrXYSize,%BrPosY] - %SecZ) / %BrZSize) * %BrZSize) + %Level_CloudOff;
		%CloudHRm = (mFloor(($StrArrayHV_Clouds[%BrPosX+%BrXYSize,%BrPosY] - %SecZ) / %BrZSize) * %BrZSize) + %Level_CloudOff;
		%CloudHDm = (mFloor(($StrArrayHV_Clouds[%BrPosX,%BrPosY-%BrXYSize] - %SecZ) / %BrZSize) * %BrZSize) + %Level_CloudOff;
		%CloudHUm = (mFloor(($StrArrayHV_Clouds[%BrPosX,%BrPosY+%BrXYSize] - %SecZ) / %BrZSize) * %BrZSize) + %Level_CloudOff;
		%minAdjCloud = getMin(getMin(getMin(%CloudHDm,%CloudHUm),%CloudHRm),%CloudHLm);
		
		PTG_Chunk_PlantBrick($PTGm.brClouds_DB,%pos,$PTGm.cloudsCol,0,0,0,$PTGm.cloudsPri,%cl,%BG,%Chunk,"CloudBr","");
		
		//Store highest point for chunk relative to top of each brick (for boundaries, if enabled) 
			//sometimes only bottom cloud layer will generate (i.e. using cube or pillar bricks), thus two height checks are included for clouds
		if(isObject(%Chunk) && ((%tempZMax = %Level_CloudOff) > %Chunk.PTGHighestZpos))
			%Chunk.PTGHighestZpos = %tempZMax;
	

		//Cloud Brick (Top)
		%clTopH = (mFloor((%CloudH - %SecZ) / %BrZSize) * %BrZSize) + %Level_CloudOff; //mFloor((%Level_CloudOff + (%CloudH - %SecZ)) / %BrZSize) * %BrZSize;
		
		if((%clTopH - %BrZSize) >= %BrZSize && %clTopH > (%Level_CloudOff+%BrZSize)) //if((%clTopH - %BrZSize) >= (%Level_CloudOff + %BrZSize))
		{
			%secPass = true;
			
			%pos = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %clTopH-%BrZhSize;
			PTG_Chunk_PlantBrick($PTGm.brClouds_DB,%pos,$PTGm.cloudsCol,0,0,0,$PTGm.cloudsPri,%cl,%BG,%Chunk,"CloudBr","");
			
			//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
			if(isObject(%Chunk) && ((%tempZMax = %clTopH) > %Chunk.PTGHighestZpos))
				%Chunk.PTGHighestZpos = %tempZMax;
		}

		//Modular Terrain
		if($PTGm.enabModTer_Clouds)
		{
			%AdjSecPass = 	$StrArrayHV_Clouds[%BrPosX-%BrXYSize,%BrPosY] > %SecZ ||
							$StrArrayHV_Clouds[%BrPosX+%BrXYSize,%BrPosY] > %SecZ ||
							$StrArrayHV_Clouds[%BrPosX,%BrPosY-%BrXYSize] > %SecZ ||
							$StrArrayHV_Clouds[%BrPosX,%BrPosY+%BrXYSize] > %SecZ;
			
			//Generate ModTer if above cloud section, or if adjacent cloud brick is above cloud section
			if(%secPass || %AdjSecPass)
			{
				%CloudHLUm = (mFloor(($StrArrayHV_Clouds[%BrPosX-%BrXYSize,%BrPosY+%BrXYSize] - %SecZ) / %BrZSize) * %BrZSize) + %Level_CloudOff;
				%CloudHRUm = (mFloor(($StrArrayHV_Clouds[%BrPosX+%BrXYSize,%BrPosY+%BrXYSize] - %SecZ) / %BrZSize) * %BrZSize) + %Level_CloudOff;
				%CloudHLDm = (mFloor(($StrArrayHV_Clouds[%BrPosX-%BrXYSize,%BrPosY-%BrXYSize] - %SecZ) / %BrZSize) * %BrZSize) + %Level_CloudOff;
				%CloudHRDm = (mFloor(($StrArrayHV_Clouds[%BrPosX+%BrXYSize,%BrPosY-%BrXYSize] - %SecZ) / %BrZSize) * %BrZSize) + %Level_CloudOff;

				%posB = %CHPosX+%BrPosX SPC %CHPosY+%BrPosY SPC %clTopH;//-%BrZhSize;
				%posStr = %CloudHLm SPC %CloudHRm SPC %CloudHDm SPC %CloudHUm SPC %CloudHLUm SPC %CloudHRUm SPC %CloudHLDm SPC %CloudHRDm;
				%brData = $PTGm.cloudsCol SPC $PTGm.cloudsPri SPC 0 SPC 0 SPC $PTGm.brClouds_DB SPC %BrZSize SPC %FillBrXYZSize SPC "Clouds";
				%modTerStr = PTG_Chunk_ModTer(false,%posB SPC %posStr,%BrPosX SPC %BrPosY,%brData,%cl,%BG,%Chunk);
				
				if((%minAdjCloud-%FillBrXYZSize-%BrZSize) < (%Level_CloudOff + %BrZSize))
					PTG_Chunk_SubDivideFill(%cl,%Chunk,%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BG,%clTopH-%BrZSize SPC %Level_CloudOff+%BrZSize-0.1 SPC "PlateFill",$PTGm.cloudsCol SPC 0 SPC 0 SPC 0 SPC $PTGm.cloudsPri,">=","Cube","CloudBr");
				else
					PTG_Chunk_SubDivideFill(%cl,%Chunk,%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BG,%clTopH-%BrZSize SPC %minAdjCloud-%FillBrXYZSize-%BrZSize SPC "PlateFill",$PTGm.cloudsCol SPC 0 SPC 0 SPC 0 SPC $PTGm.cloudsPri,">=","Cube","CloudBr");
			}
		}
		
		
		//Gap-Fill
		else
		{
			if((%minAdjCloud-%FillBrXYZSize-%BrZSize) < (%Level_CloudOff + %BrZSize))
				PTG_Chunk_SubDivideFill(%cl,%Chunk,%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BG,%clTopH-%BrZSize SPC %Level_CloudOff+%BrZSize-0.1 SPC "PlateFill",$PTGm.cloudsCol SPC 0 SPC 0 SPC 0 SPC $PTGm.cloudsPri,">=","Disabled","CloudBr");
			else
			{
				if(%clTopH-%BrZSize > %minAdjCloud-%FillBrXYZSize-%BrZSize)
					PTG_Chunk_SubDivideFill(%cl,%Chunk,%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BG,%clTopH-%BrZSize SPC %minAdjCloud-%FillBrXYZSize-%BrZSize SPC %genType,$PTGm.cloudsCol SPC 0 SPC 0 SPC 0 SPC $PTGm.cloudsPri,">","Disabled","CloudBr");
			}
		}
	}

	//////////////////////////////////////////////////
	
	if((%BrPosX += %BrXYSize) >= %ChSize)
	{
		%BrPosX = %BrXYhSize;
		
		if((%BrPosY += %BrXYSize) >= %ChSize)
		{
			PTG_Chunk_Gen_Relay(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"Builds",%BG);
			return;
		}
		else
		{
			if($PTG.genSpeed < 2)
			{
				$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_genMS,0,50));
				scheduleNoQuota(%delay,0,PTG_Chunk_Gen_Clouds,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
			}
			else
				PTG_Chunk_Gen_Clouds(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
			return;
		}
	}
	
	if($PTG.genSpeed == 0)
	{
		$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_genMS,0,50));
		scheduleNoQuota(%delay,0,PTG_Chunk_Gen_Clouds,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
	}
	else
		PTG_Chunk_Gen_Clouds(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%BrPosX,%BrPosY);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Chunk_Gen_Boundaries(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%rtnFunc)
{
	%BrXYhSize = $PTGm.brTer_XYsize / 2;
	%delay = mClamp($PTG.brDelay_genMS,0,50);
	%delaySec = mClamp($PTG.delay_secFuncMS,0,100);
	%ZMax = mClamp($PTG.zMax,0,4000);
		%ZMaxH = %ZMax / 2;
	%ChSize = mClamp($PTGm.chSize,16,256);
	%Level_BoundsH = $PTGm.boundsHLevel;
	
	%GridStartX = $PTGm.gridStartX;
	%GridStartY = $PTGm.gridStartY;
	%GridEndX = $PTGm.gridEndX;
	%GridEndY = $PTGm.gridEndY;
	
	%ChPosXb = %ChPosX + %ChSize;
	%ChPosYb = %ChPosY + mClamp($PTGm.chSize,16,256);
	
	//!!! make sure bounds ceiling is > highest terrain point (clouds and floating islands too?) !!!
	//Add all boundary bricks to this chunk, even though outside / adjacent to chunk

	//If highest point already stored in chunk, don't calculate but access instead
	if($PTGm.boundsH_RelToTer && !$PTGm.boundsCeil)// && isObject(%Chunk) && %Chunk.PTGHighestZpos > 0)
		%BrHmax = %Chunk.PTGHighestZpos; //getMax(%Chunk.PTGHighestZpos,$PTGm.lakesHLevel);
	else
		%BrHmax = %Level_BoundsH - 8; //"- 8" to account for "+ 8" below (otherwise boundaries will generate above standard boundary level)
	
	//Bounds (Wall and Ceiling) Non-Static Datablocks
	switch($PTGm.enabModTer) //based on if terrain is using ModTer (not clouds or floating islands)
	{
		case true:
		
			%dbCeil = "brick32Cube4Data";
			%dbWall = "brick32Cube5Data";
			
		case false:
		
			%dbCeil = "brick32xQuarterHCubePTGData";
			%dbWall = "brick32x32x64PTGData";
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	//CEILING (if enabled)
	
	if($PTGm.boundsCeil)
	{
		//If static shape boundaries are disabled
		if(!$PTGm.boundsStatic)
		{
			for(%posX = 8; %posX < %ChSize; %posX += 16)
			{
				for(%posY = 8; %posY < %ChSize; %posY += 16)
				{
					%pos = %CHPosX+%posX SPC %CHPosY+%posY SPC %Level_BoundsH+2;
					scheduleNoQuota(%app += %delaySec,0,PTG_Chunk_PlantBrick,%dbCeil,%pos,$PTGm.boundsCeilCol,0,0,0,$PTGm.boundsCeilPri,%cl,%BG,%Chunk,"BoundsCeilBr","");
				}
			}
		}
		
		//Stretch static shapes to fill area while reducing amount of static objects
		else
		{
			%pos = %CHPosX+(%ChSize / 2) SPC %CHPosY+(%ChSize / 2) SPC %Level_BoundsH+2;
			%scale = (%ChSize / 16) SPC (%ChSize / 16) SPC 1;
			
			PTG_Chunk_SpawnStatic(%Chunk,%pos,%scale,"Ceil");
		}
	}
	
	//////////////////////////////////////////////////
	//BOUNDARY WALLS

	//%gridHalf = getMin((%GridEndX - %GridStartX) / 2,(%GridEndY - %GridStarty) / 2);
	//%vectDistX = vectorDist(%ChPosX SPC %ChPosY,(%ChPosX-%ChSize) SPC %ChPosY);
	//%vectDistXb = vectorDist(%ChPosX SPC %ChPosY,(%ChPosX+%ChSize) SPC %ChPosY);
	//%vectDistY = vectorDist(%ChPosX SPC %ChPosY,%ChPosX SPC (%ChPosY-%ChSize));
	//%vectDistYb = vectorDist(%ChPosX SPC %ChPosY,%ChPosX SPC (%ChPosY+%ChSize));
	
	//If boundary start height set to be relative to terrain height offset
	if($PTGm.boundsH_RefTerOff)
		%BndsHFloorRel = mFloor($PTGm.terHLevel / 32) * 32;
	
	%tmpBrHsnap = mCeil(((%BrHmax - %BndsHFloorRel) + 8 + ($PTGm.boundsHTerRel * 32)) / 32); ////"+ 16" to ensure bounds are above %BrHmax value
	%tmpBrHsnap_act = mCeil((%BrHmax + 8 + ($PTGm.boundsHTerRel * 32)) / 32); ////"+ 16" to ensure bounds are above %BrHmax value
	%BrHmax = mClamp(%tmpBrHsnap_act * 32,32,%ZMax); // / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
	%BrHMax_rel = mClamp(%tmpBrHsnap,1,31);
	
	%checkX = $PTGm.genType $= "Finite" && (($PTGm.gridType $= "Square" && %ChPosX == %GridStartX) || ($PTGm.gridType $= "Radial"));// && %vectDistX <= %gridHalf));
	%checkXb = $PTGm.genType $= "Finite" && (($PTGm.gridType $= "Square" && %ChPosXb == %GridEndX) || ($PTGm.gridType $= "Radial"));// && %vectDistXb <= %gridHalf));
	%checkY = $PTGm.genType $= "Finite" && (($PTGm.gridType $= "Square" && %ChPosY == %GridStartY) || ($PTGm.gridType $= "Radial"));// && %vectDistY <= %gridHalf));
	%checkYb = $PTGm.genType $= "Finite" && (($PTGm.gridType $= "Square" && %ChPosYb == %GridEndY) || ($PTGm.gridType $= "Radial"));// && %vectDistYb <= %gridHalf));
	
	
	//South
	if((%checkY || $PTGm.genType $= "Infinite" || $PTGm.terType $= "NoTerrain") && !isObject(%tmpChunk = "Chunk_" @ %ChPosX @ "_" @ (%ChPosY-%ChSize)))
	{
		//If static shape boundaries are disabled
		if(!$PTGm.boundsStatic)
		{
			for(%posX = 8; %posX < %ChSize; %posX += 16)
			{
				for(%posZ = (16 + %BndsHFloorRel); %posZ < %BrHMax; %posZ += 32) //"%posZ < %BrHMax" instead of using "<=" because centroid of boundary is being referenced, not top
				{
					%CBposXYZ = %CHPosX+%posX SPC %CHPosY-8 SPC %posZ;//%ZMaxH;//32;
					%CBsizeXYZ = 15.9 SPC 15.9 SPC 31.9;//%ZMax;//31.9;
					initContainerBoxSearch(%CBposXYZ,%CBsizeXYZ,$TypeMasks::FxBrickObjectType);
			
					if(isObject(%obj = containerSearchNext()))
					{
						%obj.delete();
						%tmpDelay = 10;
					}
					else
						%tmpDelay = 0;
				
					%pos = %CHPosX+%posX SPC %CHPosY-8 SPC %posZ;
					scheduleNoQuota(%app += (%delaySec + %tmpDelay),0,PTG_Chunk_PlantBrick,%dbWall,%pos,$PTGm.boundsWallCol,0,0,0,$PTGm.boundsWallPri,%cl,%BG,%Chunk,"BoundsWallBr","");
				}
			}
		}
		
		//Stretch static shapes to fill area while reducing amount of static objects
		else
		{
			//%posZ = mClamp(%BrHMax_rel,1,9);
			%pos = %CHPosX+(%ChSize / 2) SPC %CHPosY-8 SPC %BndsHFloorRel+((%BrHMax_rel * 32) / 2);
			%scale = (%ChSize / 16) SPC 1 SPC %BrHMax_rel;
			
			//%CBposXYZ = %pos;
			//%CBsizeXYZ = 15.9 SPC 15.9 SPC (%BrHMax_rel*32)-0.1;
			//initContainerBoxSearch(%CBposXYZ,%CBsizeXYZ,$Typemasks::StaticObjectType);
	
			//if(isObject(%obj = containerSearchNext())) //static shape boundaries are designed to overlap, so don't remove 
			//	%obj.delete();
			
			PTG_Chunk_SpawnStatic(%Chunk,%pos,%scale,"Wall");
		}
	}
	
	
	//North
	if((%checkYb || $PTGm.genType $= "Infinite" || $PTGm.terType $= "NoTerrain") && !isObject(%tmpChunk = "Chunk_" @ %ChPosX @ "_" @ %ChPosYb))
	{
		//If static shape boundaries are disabled
		if(!$PTGm.boundsStatic)
		{
			for(%posX = 8; %posX < %ChSize; %posX += 16)
			{
				for(%posZ = (16 + %BndsHFloorRel); %posZ < %BrHMax; %posZ += 32)
				{
					%CBposXYZ = %CHPosX+%posX SPC %CHPosY+%ChSize+8 SPC %posZ;//%ZMaxH;//32;
					%CBsizeXYZ = 15.9 SPC 15.9 SPC 31.9;//%ZMax;//31.9;
					initContainerBoxSearch(%CBposXYZ,%CBsizeXYZ,$TypeMasks::FxBrickObjectType);
			
					if(isObject(%obj = containerSearchNext()))
					{
						%obj.delete();
						%tmpDelay = 10;
					}
					else
						%tmpDelay = 0;
					
					%pos = %CHPosX+%posX SPC %CHPosY+%ChSize+8 SPC %posZ;
					scheduleNoQuota(%app += (%delaySec + %tmpDelay),0,PTG_Chunk_PlantBrick,%dbWall,%pos,$PTGm.boundsWallCol,0,0,0,$PTGm.boundsWallPri,%cl,%BG,%Chunk,"BoundsWallBr","");
				}
			}
		}
		
		//Stretch static shapes to fill area while reducing amount of static objects
		else
		{
			//%posZ = mClamp(%BrHMax_rel,1,9);
			%pos = %CHPosX+(%ChSize / 2) SPC %CHPosY+%ChSize+8 SPC %BndsHFloorRel+((%BrHMax_rel * 32) / 2);
			%scale = (%ChSize / 16) SPC 1 SPC %BrHMax_rel;
			
			//%CBposXYZ = %pos;
			//%CBsizeXYZ = 15.9 SPC 15.9 SPC (%BrHMax_rel*32)-0.1;
			//initContainerBoxSearch(%CBposXYZ,%CBsizeXYZ,$Typemasks::StaticObjectType);
	
			//if(isObject(%obj = containerSearchNext()))
			//	%obj.delete();
			
			PTG_Chunk_SpawnStatic(%Chunk,%pos,%scale,"Wall");
		}
	}
	
	
	//West
	if((%checkX || $PTGm.genType $= "Infinite" || $PTGm.terType $= "NoTerrain") && !isObject(%tmpChunk = "Chunk_" @ (%ChPosX-%ChSize) @ "_" @ %ChPosY))
	{
		//If static shape boundaries are disabled
		if(!$PTGm.boundsStatic)
		{
			for(%posY = 8; %posY < %ChSize; %posY += 16)
			{
				for(%posZ = (16 + %BndsHFloorRel); %posZ < %BrHMax; %posZ += 32)
				{
					%CBposXYZ = %CHPosX-8 SPC %CHPosY+%posY SPC %posZ;//%ZMaxH;//32;
					%CBsizeXYZ = 15.9 SPC 15.9 SPC 31.9;//%ZMax;//31.9;
					initContainerBoxSearch(%CBposXYZ,%CBsizeXYZ,$TypeMasks::FxBrickObjectType);
			
					if(isObject(%obj = containerSearchNext()))
					{
						%obj.delete();
						%tmpDelay = 10;
					}
					else
						%tmpDelay = 0;
					
					%pos = %CHPosX-8 SPC %CHPosY+%posY SPC %posZ;
					scheduleNoQuota(%app += (%delaySec + %tmpDelay),0,PTG_Chunk_PlantBrick,%dbWall,%pos,$PTGm.boundsWallCol,0,0,0,$PTGm.boundsWallPri,%cl,%BG,%Chunk,"BoundsWallBr","");
				}
			}
		}
		
		//Stretch static shapes to fill area while reducing amount of static objects
		else
		{
			//%posZ = mClamp(%BrHMax_rel,1,9);
			%pos = %CHPosX-8 SPC %CHPosY+(%ChSize / 2) SPC %BndsHFloorRel+((%BrHMax_rel * 32) / 2);
			%scale = 1 SPC (%ChSize / 16) SPC %BrHMax_rel;
			
			//%CBposXYZ = %pos;
			//%CBsizeXYZ = 15.9 SPC 15.9 SPC (%BrHMax_rel*32)-0.1;
			//initContainerBoxSearch(%CBposXYZ,%CBsizeXYZ,$Typemasks::StaticObjectType);
	
			//if(isObject(%obj = containerSearchNext()))
			//	%obj.delete();
			
			PTG_Chunk_SpawnStatic(%Chunk,%pos,%scale,"Wall");
		}
	}

	
	//East
	if((%checkXb || $PTGm.genType $= "Infinite" || $PTGm.terType $= "NoTerrain") && !isObject(%tmpChunk = "Chunk_" @ %ChPosXb @ "_" @ %ChPosY))
	{		
		//If static shape boundaries are disabled
		if(!$PTGm.boundsStatic)
		{
			for(%posY = 8; %posY < %ChSize; %posY += 16)
			{
				for(%posZ = (16 + %BndsHFloorRel); %posZ < %BrHMax; %posZ += 32)
				{
					%CBposXYZ = %CHPosX+%ChSize+8 SPC %CHPosY+%posY SPC %posZ;//%ZMaxH;//32;
					%CBsizeXYZ = 15.9 SPC 15.9 SPC 31.9;//%ZMax;//31.9;
					initContainerBoxSearch(%CBposXYZ,%CBsizeXYZ,$TypeMasks::FxBrickObjectType);
			
					if(isObject(%obj = containerSearchNext()))
					{
						%obj.delete();
						%tmpDelay = 10;
					}
					else
						%tmpDelay = 0;
					
					%pos = %CHPosX+%ChSize+8 SPC %CHPosY+%posY SPC %posZ;
					scheduleNoQuota(%app += (%delaySec + %tmpDelay),0,PTG_Chunk_PlantBrick,%dbWall,%pos,$PTGm.boundsWallCol,0,0,0,$PTGm.boundsWallPri,%cl,%BG,%Chunk,"BoundsWallBr","");
				}
			}
		}
		
		//Stretch static shapes to fill area while reducing amount of static objects
		else
		{
			//%posZ = mClamp(%BrHMax_rel,1,9);
			%pos = %CHPosX+%ChSize+8 SPC %CHPosY+(%ChSize / 2) SPC %BndsHFloorRel+((%BrHMax_rel * 32) / 2);
			%scale = 1 SPC (%ChSize / 16) SPC %BrHMax_rel;
			
			//%CBposXYZ = %pos;
			//%CBsizeXYZ = 15.9 SPC 15.9 SPC (%BrHMax_rel*32)-0.1;
			//initContainerBoxSearch(%CBposXYZ,%CBsizeXYZ,$Typemasks::StaticObjectType);
	
			//if(isObject(%obj = containerSearchNext()))
			//	%obj.delete();
			
			PTG_Chunk_SpawnStatic(%Chunk,%pos,%scale,"Wall");
		}
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//Return function
	switch$(%rtnFunc)
	{
		case "FileLoad":
		
			$PTG.dedSrvrFuncCheckTime += %delaySec;
			scheduleNoQuota(%app += %delaySec,0,PTG_Chunk_ChunkLoad,%cl,%Chunk,"","",%CHPosX SPC %CHPosY SPC %xmod SPC %ymod SPC %clNum);
			
		case "Append":
		
			//Gradual or Entire grid generation per player (reset chunk pos on grid - for next player - or append next chunk for gradual loading?)
			if($PTGm.genMethod $= "Complete")
			{
				$PTG.dedSrvrFuncCheckTime = getSimTime();
				scheduleNoQuota(%app += mClamp($PTG.delay_subFuncMS,0,50),0,PTG_Routine_Append,%cl,%CHPosX,%CHPosY,%xmod++,%ymod,%clNum);
			}
			else
			{
				if(%clNum++ >= clientgroup.getCount())
					%clNum = 0; //Make sure client exists? Or allow append function to check?
				
				$PTG.dedSrvrFuncCheckTime = getSimTime();
				scheduleNoQuota(%app + mClamp($PTG.delay_subFuncMS,0,50),0,PTG_Routine_Append,%cl,%CHPosX,%CHPosY,0,0,%clNum);
			}
		
			//scheduleNoQuota(0,0,PTG_Routine_Append,%cl,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			
		default: //Relay

			$PTG.dedSrvrFuncCheckTime += %delaySec;
			scheduleNoQuota(%app + %delaySec,0,PTG_Chunk_Gen_Relay,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"Final",%BG);
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Chunk_Gen_ChunkReCalcBounds(%cl,%Chunk,%VARS) //,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%rtnFunc
{
	//Calculate boundaries for each adjacent chunk instead of center, existing chunk like above function
	//use schedules for planting bricks?
	
	//%Chunk doesn't exist anymore at this point (chunk was culled), but can still access name
	%ChunkN = strReplace(%Chunk,"_"," ");
	%CHPosX = getWord(%ChunkN,1);
	%CHPosY = getWord(%ChunkN,2);
	%delay = mClamp($PTG.brDelay_genMS,0,50);
	//%altPos = false;
	%Level_BoundsTerH = $PTGm.boundsHTerRel;
	%ChSize = mClamp($PTGm.chSize,16,256);
	%Level_TerOff = $PTGm.terHLevel;
	%zMax = mClamp($PTG.zMax,0,4000);
	
	%tmpChunk_South = "Chunk_" @ %ChPosX @ "_" @ (%ChPosY-%ChSize);
	%tmpChunk_North = "Chunk_" @ %ChPosX @ "_" @ (%ChPosY+%ChSize);
	%tmpChunk_West = "Chunk_" @ (%ChPosX-%ChSize) @ "_" @ %ChPosY;
	%tmpChunk_East = "Chunk_" @ (%ChPosX+%ChSize) @ "_" @ %ChPosY;
	
	//Check if boundaries are enabled before continuing and if at least one adj chunk exists (otherwise skip and return to cull function)
	if(!$PTGm.enabBounds || (!isObject(%tmpChunk_South) && !isObject(%tmpChunk_North) && !isObject(%tmpChunk_West) && !isObject(%tmpChunk_East)))
	{
		%chCount = getWord(%VARS,0);
		%CHTotalC = getWord(%VARS,1);
	
		PTG_Routine_ChunkCull(%cl,%chCount++,%CHTotalC);
		return;
	}

	if($PTG.publicBricks || !isObject(%BGb = "BrickGroup_" @ $PTG.lastClientID)) //%c.bl_id
		%BG = "BrickGroup_888888";
	else
		%BG = %BGb;
	
	//Bounds (Wall and Ceiling) Non-Static Datablocks
	switch($PTGm.enabModTer) //based on if terrain is using ModTer (not clouds or floating islands)
	{
		case true:
		
			%dbCeil = "brick32Cube4Data";
			%dbWall = "brick32Cube5Data";
			
		case false:
		
			%dbCeil = "brick32xQuarterHCubePTGData";
			%dbWall = "brick32x32x64PTGData";
	}	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//South
	if(isObject(%tmpChunk_South)) //%Chunk.PTGHighestZpos
	{
		if($PTGm.boundsH_RelToTer && !$PTGm.boundsCeil)
			%BrHMax = %tmpChunk_South.PTGHighestZpos; //getMax(%tmpChunk_South.PTGHighestZpos,$PTGm.lakesHLevel);
		else
			%BrHMax = $PTGm.boundsHLevel - 8;
		
		//%tmpBrHsnap = mCeil((%BrHmax + 8 + (%Level_BoundsTerH * 32)) / 32); ////"+ 16" to ensure bounds are above %BrHmax value
		
		//If boundary start height set to be relative to terrain height offset
		if($PTGm.boundsH_RefTerOff)
		{
			%BndsHFloorRel = mFloor((%tmpBrHsnap - %Level_TerOff) / 32) * 32;
			//%tmpBrHsnap = %tmpBrHsnap - %BndsHFloorRel;
		}
	
		//%BrHmax = %tmpBrHsnap * 32;
		//%BrHMax_rel = mClamp(mCeil(%BrHMax / 32),1,31);
		
		%tmpBrHsnap = mCeil(((%BrHmax - %BndsHFloorRel) + 8 + (%Level_BoundsTerH * 32)) / 32); //"+ 8" to ensure bounds are above %BrHmax value
		%tmpBrHsnap_act = mCeil((%BrHmax + 8 + (%Level_BoundsTerH * 32)) / 32);
		%BrHmax = mClamp(%tmpBrHsnap_act * 32,32,%zMax); // / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
		%BrHMax_rel = mClamp(%tmpBrHsnap,1,31);
		
		//If static shape boundaries are disabled
		if(!$PTGm.boundsStatic)
		{		
			for(%posX = 8; %posX < %ChSize; %posX += 16)
			{
				for(%posZ = (16+%BndsHFloorRel); %posZ <= %BrHMax; %posZ += 32)
				{
					%CBposXYZ = %CHPosX+%posX SPC %CHPosY+8 SPC %posZ;//32;
					%CBsizeXYZ = 15.9 SPC 15.9 SPC 31.9;
					initContainerBoxSearch(%CBposXYZ,%CBsizeXYZ,$TypeMasks::FxBrickObjectType);
			
					if(isObject(%obj = containerSearchNext()))
					{
						%obj.delete();
						%tmpDelay = 10;
					}
					else
						%tmpDelay = 0;
					
					%pos = %CHPosX+%posX SPC %CHPosY+8 SPC %posZ;
					scheduleNoQuota(%app += (%delay + %tmpDelay),0,PTG_Chunk_PlantBrick,%dbWall,%pos,$PTGm.boundsWallCol,0,0,0,$PTGm.boundsWallPri,%cl,%BG,%tmpChunk_South,"BoundsWallBr","");
				}
			}
		}
		
		//Stretch static shapes to fill area while reducing amount of static objects
		else
		{
			%posZ = mClamp(%BrHMax_rel,1,9);
			%pos = %CHPosX+(%ChSize / 2) SPC %CHPosY+8 SPC %BndsHFloorRel+((%BrHMax_rel * 32) / 2);
			%scale = (%ChSize / 16) SPC 1 SPC %posZ;
			
			//%CBposXYZ = %pos;
			//%CBsizeXYZ = 15.9 SPC 15.9 SPC (%posZ*32)-0.1;
			//initContainerBoxSearch(%CBposXYZ,%CBsizeXYZ,$Typemasks::StaticObjectType);
	
			//if(isObject(%obj = containerSearchNext())) //static shape boundaries are designed to overlap, so don't remove 
			//	%obj.delete();
			
			PTG_Chunk_SpawnStatic(%tmpChunk_South,%pos,%scale,"Wall");
		}
	}
	
	//////////////////////////////////////////////////
	
	//North
	if(isObject(%tmpChunk_North))
	{
		if($PTGm.boundsH_RelToTer && !$PTGm.boundsCeil)
			%BrHMax = %tmpChunk_North.PTGHighestZpos; //getMax(%tmpChunk_North.PTGHighestZpos,$PTGm.lakesHLevel);
		else
			%BrHMax = $PTGm.boundsHLevel - 8;
		
		//%tmpBrHsnap = mCeil((%BrHmax + 8 + (%Level_BoundsTerH * 32)) / 32); ////"+ 16" to ensure bounds are above %BrHmax value
		
		//If boundary start height set to be relative to terrain height offset
		if($PTGm.boundsH_RefTerOff)
		{
			%BndsHFloorRel = mFloor((%tmpBrHsnap - %Level_TerOff) / 32) * 32;
			//%tmpBrHsnap = %tmpBrHsnap - %BndsHFloorRel;
		}
	
		//%BrHmax = %tmpBrHsnap * 32;
		//%BrHMax_rel = mClamp(mCeil(%BrHMax / 32),1,31);
		
		%tmpBrHsnap = mCeil(((%BrHmax - %BndsHFloorRel) + 8 + (%Level_BoundsTerH * 32)) / 32); //"+ 8" to ensure bounds are above %BrHmax value
		%tmpBrHsnap_act = mCeil((%BrHmax + 8 + (%Level_BoundsTerH * 32)) / 32);
		%BrHmax = mClamp(%tmpBrHsnap_act * 32,32,%zMax); // / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
		%BrHMax_rel = mClamp(%tmpBrHsnap,1,31);
		
		//If static shape boundaries are disabled
		if(!$PTGm.boundsStatic)
		{
			for(%posX = 8; %posX < %ChSize; %posX += 16)
			{
				for(%posZ = (16+%BndsHFloorRel); %posZ <= %BrHMax; %posZ += 32)
				{
					%CBposXYZ = %CHPosX+%posX SPC %CHPosY+%ChSize-8 SPC %posZ;
					%CBsizeXYZ = 15.9 SPC 15.9 SPC 31.9;
					initContainerBoxSearch(%CBposXYZ,%CBsizeXYZ,$TypeMasks::FxBrickObjectType);
			
					if(isObject(%obj = containerSearchNext()))
					{
						%obj.delete();
						%tmpDelay = 10;
					}
					else
						%tmpDelay = 0;
					
					%pos = %CHPosX+%posX SPC %CHPosY+%ChSize-8 SPC %posZ;
					scheduleNoQuota(%app += (%delay + %tmpDelay),0,PTG_Chunk_PlantBrick,%dbWall,%pos,$PTGm.boundsWallCol,0,0,0,$PTGm.boundsWallPri,%cl,%BG,%tmpChunk_North,"BoundsWallBr","");
				}
			}
		}
		
		//Stretch static shapes to fill area while reducing amount of static objects
		else
		{
			%posZ = mClamp(%BrHMax_rel,1,9);
			%pos = %CHPosX+(%ChSize / 2) SPC %CHPosY+%ChSize-8 SPC %BndsHFloorRel+((%BrHMax_rel * 32) / 2);
			%scale = (%ChSize / 16) SPC 1 SPC %posZ;
			
			//%CBposXYZ = %pos;
			//%CBsizeXYZ = 15.9 SPC 15.9 SPC (%posZ*32)-0.1;
			//initContainerBoxSearch(%CBposXYZ,%CBsizeXYZ,$Typemasks::StaticObjectType);
	
			//if(isObject(%obj = containerSearchNext()))
			//	%obj.delete();
			
			PTG_Chunk_SpawnStatic(%tmpChunk_North,%pos,%scale,"Wall");
		}
	}
	
	//////////////////////////////////////////////////
	
	//West
	if(isObject(%tmpChunk_West))
	{
		if($PTGm.boundsH_RelToTer && !$PTGm.boundsCeil)
			%BrHMax = %tmpChunk_West.PTGHighestZpos; //getMax(%tmpChunk_West.PTGHighestZpos,$PTGm.lakesHLevel);
		else
			%BrHMax = $PTGm.boundsHLevel - 8;
		
		//%tmpBrHsnap = mCeil((%BrHmax + 8 + (%Level_BoundsTerH * 32)) / 32); ////"+ 16" to ensure bounds are above %BrHmax value
		
		//If boundary start height set to be relative to terrain height offset
		if($PTGm.boundsH_RefTerOff)
		{
			%BndsHFloorRel = mFloor((%tmpBrHsnap - %Level_TerOff) / 32) * 32;
			//%tmpBrHsnap = %tmpBrHsnap - %BndsHFloorRel;
		}
	
		//%BrHmax = %tmpBrHsnap * 32;
		//%BrHMax_rel = mClamp(mCeil(%BrHMax / 32),1,31);
		
		%tmpBrHsnap = mCeil(((%BrHmax - %BndsHFloorRel) + 8 + (%Level_BoundsTerH * 32)) / 32); //"+ 8" to ensure bounds are above %BrHmax value
		%tmpBrHsnap_act = mCeil((%BrHmax + 8 + (%Level_BoundsTerH * 32)) / 32);
		%BrHmax = mClamp(%tmpBrHsnap_act * 32,32,%zMax); // / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
		%BrHMax_rel = mClamp(%tmpBrHsnap,1,31);
		
		//If static shape boundaries are disabled
		if(!$PTGm.boundsStatic)
		{
			for(%posY = 8; %posY < %ChSize; %posY += 16)
			{
				for(%posZ = (16+%BndsHFloorRel); %posZ <= %BrHMax; %posZ += 32)
				{
					%CBposXYZ = %CHPosX+8 SPC %CHPosY+%posY SPC %posZ;
					%CBsizeXYZ = 15.9 SPC 15.9 SPC 31.9;
					initContainerBoxSearch(%CBposXYZ,%CBsizeXYZ,$TypeMasks::FxBrickObjectType);
			
					if(isObject(%obj = containerSearchNext()))
					{
						%obj.delete();
						%tmpDelay = 10;
					}
					else
						%tmpDelay = 0;
					
					%pos = %CHPosX+8 SPC %CHPosY+%posY SPC %posZ;
					scheduleNoQuota(%app += (%delay + %tmpDelay),0,PTG_Chunk_PlantBrick,%dbWall,%pos,$PTGm.boundsWallCol,0,0,0,$PTGm.boundsWallPri,%cl,%BG,%tmpChunk_West,"BoundsWallBr","");
				}
			}
		}
		
		//Stretch static shapes to fill area while reducing amount of static objects
		else
		{
			%posZ = mClamp(%BrHMax_rel,1,9);
			%pos = %CHPosX+8 SPC %CHPosY+(%ChSize / 2) SPC %BndsHFloorRel+((%BrHMax_rel * 32) / 2);
			%scale = 1 SPC (%ChSize / 16) SPC %posZ;
			
			//%CBposXYZ = %pos;
			//%CBsizeXYZ = 15.9 SPC 15.9 SPC (%posZ*32)-0.1;
			//initContainerBoxSearch(%CBposXYZ,%CBsizeXYZ,$Typemasks::StaticObjectType);
	
			//if(isObject(%obj = containerSearchNext()))
			//	%obj.delete();
			
			PTG_Chunk_SpawnStatic(%tmpChunk_West,%pos,%scale,"Wall");
		}
	}
	
	//////////////////////////////////////////////////
	
	//East
	if(isObject(%tmpChunk_East))
	{
		if($PTGm.boundsH_RelToTer && !$PTGm.boundsCeil)
			%BrHMax = %tmpChunk_East.PTGHighestZpos; //getMax(%tmpChunk_East.PTGHighestZpos,$PTGm.lakesHLevel);
		else
			%BrHMax = $PTGm.boundsHLevel - 8;
		
		//%tmpBrHsnap = mCeil((%BrHmax + 8 + (%Level_BoundsTerH * 32)) / 32); ////"+ 16" to ensure bounds are above %BrHmax value
		
		//If boundary start height set to be relative to terrain height offset
		if($PTGm.boundsH_RefTerOff)
		{
			%BndsHFloorRel = mFloor((%tmpBrHsnap - %Level_TerOff) / 32) * 32;
			//%tmpBrHsnap = %tmpBrHsnap - %BndsHFloorRel;
		}
	
		//%BrHmax = %tmpBrHsnap * 32;
		//%BrHMax_rel = mClamp(mCeil(%BrHMax / 32),1,31);
		
		%tmpBrHsnap = mCeil(((%BrHmax - %BndsHFloorRel) + 8 + (%Level_BoundsTerH * 32)) / 32); //"+ 8" to ensure bounds are above %BrHmax value
		%tmpBrHsnap_act = mCeil((%BrHmax + 8 + (%Level_BoundsTerH * 32)) / 32);
		%BrHmax = mClamp(%tmpBrHsnap_act * 32,32,%zMax); // / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
		%BrHMax_rel = mClamp(%tmpBrHsnap,1,31);
		
		//If static shape boundaries are disabled
		if(!$PTGm.boundsStatic)
		{
			for(%posY = 8; %posY < %ChSize; %posY += 16)
			{
				for(%posZ = (16+%BndsHFloorRel); %posZ <= %BrHMax; %posZ += 32)
				{
					%CBposXYZ = %CHPosX+%ChSize-8 SPC %CHPosY+%posY SPC %posZ;
					%CBsizeXYZ = 15.9 SPC 15.9 SPC 31.9;
					initContainerBoxSearch(%CBposXYZ,%CBsizeXYZ,$TypeMasks::FxBrickObjectType); // | $Typemasks::StaticObjectType);
			
					if(isObject(%obj = containerSearchNext()))
					{
						%obj.delete();
						%tmpDelay = 10;
					}
					else
						%tmpDelay = 0;
					
					%pos = %CHPosX+%ChSize-8 SPC %CHPosY+%posY SPC %posZ;
					scheduleNoQuota(%app += (%delay + %tmpDelay),0,PTG_Chunk_PlantBrick,%dbWall,%pos,$PTGm.boundsWallCol,0,0,0,$PTGm.boundsWallPri,%cl,%BG,%tmpChunk_East,"BoundsWallBr","");
				}
			}
		}
		
		//Stretch static shapes to fill area while reducing amount of static objects
		else
		{
			%posZ = mClamp(%BrHMax_rel,1,9);
			%pos = %CHPosX+%ChSize-8 SPC %CHPosY+(%ChSize / 2) SPC %BndsHFloorRel+((%BrHMax_rel * 32) / 2);
			%scale = 1 SPC (%ChSize / 16) SPC %posZ;
			
			//%CBposXYZ = %pos;
			//%CBsizeXYZ = 15.9 SPC 15.9 SPC (%posZ*32)-0.1;
			//initContainerBoxSearch(%CBposXYZ,%CBsizeXYZ,$Typemasks::StaticObjectType);
	
			//if(isObject(%obj = containerSearchNext()))
			//	%obj.delete();
			
			PTG_Chunk_SpawnStatic(%tmpChunk_East,%pos,%scale,"Wall");
		}
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//Return to cull function
	%chCount = getWord(%VARS,0);
	%CHTotalC = getWord(%VARS,1);
	
	//Ensure FIFO option
	if($PTG.FIFOchClr)
	{
		%lowestIDCh = 0;
		
		for(%c = 1; %c < BrickGroup_Chunks.getCount(); %c++)
		{
			if((%tmpCh = BrickGroup_Chunks.getObject(%c).getID()) < %lowestIDCh)
			{
				%lowestIDCh = %tmpCh;
				%lowestNumCh = %c;
			}
		}
		%chCount = %lowestNumCh;
	}
	else
		%chCount++;
	
	//$PTG.dedSrvrFuncCheckTime += %delay; //doesn't apply to dedicated server lag check
	scheduleNoQuota(%app + mClamp($PTG.delay_secFuncMS,0,100),0,PTG_Routine_ChunkCull,%cl,%chCount,%CHTotalC);
}


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////// SAVING, LOADING & REMOVAL FUNCTIONS ////////


function PTG_Chunk_RemoveBricks(%cl,%Chunk,%count,%objNum,%rtnFunc,%VARS)
{
	if(%ObjNum == 0)
	{
		if(%Chunk.ChUnstablePTG) // || %Chunk.ChStaticPTG)
		{
			switch$(%rtnFunc)
			{
				case "Cull" or "Bounds":
				
					%chCount = getWord(%VARS,0);
					%CHTotalC = getWord(%VARS,1);
					
					//if($PTG.FIFOchClr) //don't return to beginning of chunk group if there's an issue accessing chunk (meant for "Cull" only)
					
					scheduleNoQuota(0,0,PTG_Routine_ChunkCull,%cl,%chCount++,%CHTotalC);
					
				case "Purge" or "Clear" or "ClearAll":
				
					%chCount = getWord(%VARS,0);
					%brCount = getWord(%VARS,1);// + %objNum; //add new brick count to existing count //kept %objNum just encase, even though it should always be null at the beginning of this function
					%CHTotalC = getWord(%VARS,2);
					%clearStatic = getWord(%VARS,3);
					%dontSave = getWord(%VARS,4);
					%chPassCount = getWord(%VARS,5);
					
					scheduleNoQuota(0,0,PTGClear_Recurs,%cl,%chCount++,%brCount,%CHTotalC,%clearStatic,%dontSave,%rtnFunc,%chPassCount);
					
				case "RemoveChunk":
				
					%chCount = getWord(%VARS,0);
					%brCount = 0; //%objNum;
					%removeSave = getWord(%VARS,1);
					
					scheduleNoQuota(0,0,PTGRemoveChunk_Recurs,%cl,%Chunk,%chCount++,%brCount,%removeSave,true);
					
				default:
					return;
			}
			
			return;
		}

		if(!%Chunk.ChUnstablePTG)
			%Chunk.ChUnstablePTG = true;
	}

	%brick = %Chunk.getObject(0);
	
	//Adjust total brick count if an object in chunk isn't a planted brick (ghost brick or highlight obj) or is a boundary brick
	if(%brick.ChBoundsPTG || %brick.getClassName() !$= "fxDTSBrick" || !%brick.isPlanted) //".isPlanted" filters out temp / ghost bricks
	{
		%objNum--;
		%count--;
	}
	
	%brick.delete();
	
	if(%objNum++ >= %count)
	{
		%Chunk.delete(); //%Chunk.ChUnstablePTG = false;

		switch$(%rtnFunc)
		{
			case "Cull":
			
				%chCount = getWord(%VARS,0);
				%CHTotalC = getWord(%VARS,1);
				
				//Ensure FIFO option
				if($PTG.FIFOchClr)
				{
					%lowestIDCh = 0;
					
					for(%c = 1; %c < BrickGroup_Chunks.getCount(); %c++)
					{
						if((%tmpCh = BrickGroup_Chunks.getObject(%c).getID()) < %lowestIDCh)
						{
							%lowestIDCh = %tmpCh;
							%lowestNumCh = %c;
						}
					}
					%chCount = %lowestNumCh;
				}
				else
					%chCount++;
				
				scheduleNoQuota(0,0,PTG_Routine_ChunkCull,%cl,%chCount,%CHTotalC);
				
			case "Bounds":
			
				scheduleNoQuota(0,0,PTG_Chunk_Gen_ChunkReCalcBounds,%cl,%Chunk,%VARS);
				
			case "Purge" or "Clear" or "ClearAll":
			
				%chCount = getWord(%VARS,0);
				%brCount = getWord(%VARS,1) + %objNum; //add new brick count to existing count //kept %objNum just encase, even though it should always be null at the beginning of this function
				%CHTotalC = getWord(%VARS,2);
				%clearStatic = getWord(%VARS,3);
				%dontSave = getWord(%VARS,4);
				%chPassCount = getWord(%VARS,5);
				
				if(%rtnFunc $= "Purge" && isFile(%file = PTG_GetFP("Chunk-Norm",%Chunk,"","")) && strStr(%file,"Permanent_Saves") == -1)
					fileDelete(%file);
								
				scheduleNoQuota(0,0,PTGClear_Recurs,%cl,%chCount++,%brCount,%CHTotalC,%clearStatic,%dontSave,%rtnFunc,%chPassCount);
				
			case "RemoveChunk":
			
				%chCount = getWord(%VARS,0);
				%brCount = %objNum;
				%removeSave = getWord(%VARS,1);
				
				scheduleNoQuota(0,0,PTGRemoveChunk_Recurs,%cl,%Chunk,%chCount++,%brCount,%removeSave,true);
				
			default:
				return;
		}
	}
	else
	{
		//$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_remMS,0,50));// + 0; //(%delay = mClamp($PTG.delay_priFuncMS,0,100));
		
		if($PTG.genSpeed < 2)
			scheduleNoQuota(mClamp($PTG.brDelay_remMS,0,50),0,PTG_Chunk_RemoveBricks,%cl,%Chunk,%count,%objNum,%rtnFunc,%VARS);
		else
			PTG_Chunk_RemoveBricks(%cl,%Chunk,%count,%objNum,%rtnFunc,%VARS);
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Chunk_SaveRemoveBricks(%cl,%Chunk,%count,%objNum,%file,%rmv,%rtnFunc,%VARS)
{
	if(!isObject(%file))
	{
		//Create Info file for permanent saves folder - if it doesn't already exist
		if(!isFile(%fp_ps = PTG_GetFP("Info-PermFldr","","","")))
		{
			%file = new FileObject();
			%file.openForWrite(%fp_ps);
			
			%file.writeLine(">>The \"Permanent_Saves\" folder is for any chunk saves (for this seed and chunk size) which will never be deleted by the generator, under any conditions.");
			%file.writeLine(">>To set up a chunk save as \"permanent\", just move that file from the \"Normal_Saves\" folder to the \"Permanent_Saves\" folder.");
			%file.writeLine("Note: It's not recommended to copy saves from one folder to the next, nor to move or delete files while the game is running.");
			
			%file.close();
			%file.delete();
		}
		
		//If a new chunk save file is being created, check file save limits
		if(!isFile(PTG_GetFP("Chunk",%Chunk,"","")) && !isFile(PTG_GetFP("Chunk",%Chunk,"","")))
		{
			$PTG.chSaveLimit_TotalFiles = mClamp($PTG.chSaveLimit_TotalFiles,0,100000);
			$PTG.chSaveLimit_FilesPerSeed = mClamp($PTG.chSaveLimit_FilesPerSeed,0,100000);

			%fileCheckA = getFileCount(PTG_GetFP("ChunkCache","","","")) <= $PTG.chSaveLimit_TotalFiles;
			%fileCheckB = getFileCount(PTG_GetFP("ChunkSeed","","","")) <= $PTG.chSaveLimit_FilesPerSeed;

			if(!%fileCheckA) //check total save files limit first, then limit for seed
			{
				//If option to remove old saves is enabled, attempt to make room for new save by clearing an old save file
				if($PTG.chSaveExcdResp $= "RemoveOld")
				{
					if(isFile(%tmpFile = findFirstFile(PTG_GetFP("ChunkSeed-NonPerm","","","")))) //even though total limit exceeded, try removing file for current seed and chunk size
						fileDelete(%tmpFile);
					else
					{
						bottomPrintAll("\c0P\c3T\c1G \c0WARNING: <color:ffffff>Exceeded max of " @ $PTG.chSaveLimit_TotalFiles @ " total chunk saves; no more chunks can be saved! Attempt to remove old save for seed failed.",7); //use bottomprint encase of multiple server-wide messages
						if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c2WARNING: \c0Exceeded max of \c2" @ $PTG.chSaveLimit_TotalFiles @ " \c0total chunk saves; no more chunks can be saved! Attempt to remove old save for seed failed. \c2[!] \c0->" SPC getWord(getDateTime(),1));
					
						%fileLimitFail = true;
					}
				}
				else
				{
					bottomPrintAll("\c0P\c3T\c1G \c0WARNING: <color:ffffff>Exceeded max of " @ $PTG.chSaveLimit_TotalFiles @ " total chunk saves; no more chunks can be saved!",7); //use bottomprint encase of multiple server-wide messages
					if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c2WARNING: \c0Exceeded max of \c2" @ $PTG.chSaveLimit_TotalFiles @ " \c0total chunk saves; no more chunks can be saved! \c2[!] \c0->" SPC getWord(getDateTime(),1));
					
					%fileLimitFail = true;
				}
			}
			else if(!%fileCheckB)
			{
				if($PTG.chSaveExcdResp $= "RemoveOld")
				{
					if(isFile(%tmpFile = findFirstFile(PTG_GetFP("ChunkSeed-NonPerm","","","")))) //even though total limit exceeded, try removing file for current seed and chunk size
						fileDelete(%tmpFile);
					else
					{
						bottomPrintAll("\c0P\c3T\c1G \c0WARNING: <color:ffffff>Exceeded max of " @ $PTG.chSaveLimit_FilesPerSeed @ " chunk saves for this seed; no more chunks can be saved! Attempt to remove old save for seed failed.",7);  //use bottomprint encase of multiple server-wide messages
						if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c2WARNING: \c0Exceeded max of \c2" @ $PTG.chSaveLimit_FilesPerSeed @ " \c0chunk saves for this seed; no more chunks can be saved! Attempt to remove old save for seed failed. \c2[!] \c0->" SPC getWord(getDateTime(),1));
						
						%fileLimitFail = true;
					}
				}
				else
				{
					bottomPrintAll("\c0P\c3T\c1G \c0WARNING: <color:ffffff>Exceeded max of " @ $PTG.chSaveLimit_FilesPerSeed @ " chunk saves for this seed; no more chunks can be saved!",7);  //use bottomprint encase of multiple server-wide messages
					if($PTG.allowEchos) echo("\c4>>\c2P\c1T\c4G: \c2WARNING: \c0Exceeded max of \c2" @ $PTG.chSaveLimit_FilesPerSeed @ " \c0chunk saves for this seed; no more chunks can be saved! \c2[!] \c0->" SPC getWord(getDateTime(),1));
					
					%fileLimitFail = true;
				}
			}
		}

		if(%Chunk.ChUnstablePTG || %fileLimitFail) // || %Chunk.ChStaticPTG)
		{
			switch$(%rtnFunc) //getWord(%readLn,0);
			{
				case "Cull" or "Bounds":
				
					%chCount = getWord(%VARS,0);
					%CHtotalC = getWord(%VARS,1);
					
					//if($PTG.FIFOchClr) //don't return to beginning of chunk group if there's an issue accessing chunk (meant for "Cull" only)

					PTG_Routine_ChunkCull(%cl,%chCount++,%CHtotalC);
					
				case "AutoSave":
				
					%chCount = getWord(%VARS,0);
					%CHtotalC = getWord(%VARS,1);
					%incr = getWord(%VARS,2);
					
					PTG_Routine_AutoSaveChunks(%cl,%chCount++,%CHtotalC,%incr,false);
					
				case "Clear" or "Purge" or "ClearAll":
				
					%chCount = getWord(%VARS,0);
					%brCount = getWord(%VARS,1);// + %objNum; //add new brick count to existing count //kept %objNum just encase, even though it should always be null at the beginning of this function
					%CHTotalC = getWord(%VARS,2);
					%clearStatic = getWord(%VARS,3);
					%dontSave = getWord(%VARS,4);
					%chPassCount = getWord(%VARS,5);
					
					PTGClear_Recurs(%cl,%chCount++,%brCount,%CHTotalC,%clearStatic,%dontSave,%rtnFunc,%chPassCount);
					
				case "Save":
				
					%chNum = getWord(%VARS,0);
					%chTotal = getWord(%VARS,1);
					%chSaveAm = getWord(%VARS,2);
					%chPotSaveAm = getWord(%VARS,3);
					
					PTGSave_Recurs(%cl,%chNum++,%chTotal,%chSaveAm,%chPotSaveAm++);
			}
			
			return;
		}
		
		if(!%Chunk.ChUnstablePTG)
			%Chunk.ChUnstablePTG = true;
		
		%file = new FileObject();
		
		//Normal / Permanent File Determination
		if(isFile(%fp = PTG_GetFP("Chunk-Perm",%Chunk,"","")))
			%file.openForWrite(%fp);
		else
		{
			%fp = PTG_GetFP("Chunk-Norm",%Chunk,"","");
			%file.openForWrite(%fp);
		}
		
		if(%Chunk.ChStaticPTG)
			%chStc = "true"; //test
		else
			%chStc = "false";
		
		%file.writeLine(">>PTGChunk:" SPC %Chunk SPC "Static:" SPC %chStc); //save if chunk is static or not //%ChName
		
		for(%c = 0; %c < 64; %c++)
			%file.writeLine(getColorIDTable(%c));
	}
	
	if(%rmv)
		%brick = %Chunk.getObject(0);
	else
		%brick = %Chunk.getObject(%objNum);
	
	//Don't save boundary bricks or highlighted static chunk objects (remove though if set to remove after saving)
	if(!%brick.ChBoundsPTG && %brick.getClassName() $= "fxDTSBrick" && %brick.isPlanted) //".isPlanted" filters out temp / ghost bricks  //%brick.getDataBlock().getClassName() $= "fxDTSBrickData")
	{
		%brName = %brick.getName();
		
		if(%brick.getDataBlock().hasPrint && %brick.printID !$= "") //%brick.getDataBlock().hasPrint
		{
			%str = getPrintTexture(%brick.printID); //.getPrintId()
			%print = filebase(%str);
			%str = strReplace(%str,"_"," ");
			%printUIN = getWord(%str,1) @ "/" @ %print;
		}

		%file.writeLine("BRICKDATA " @ %brick.getDataBlock().getName() SPC %brick.getPosition() SPC %brick.angleID SPC %brick.isBasePlate SPC %brick.colorID SPC %printUIN SPC %brick.colorFxID SPC %brick.shapeFxID SPC %brick.isRayCasting() SPC %brick.isColliding() SPC %brick.isRendering() SPC %brick.PTGBrType); //%brick.position includes angleID (?)
		%file.writeLine("+-OWNER " @ getBrickGroupFromObject(%brick).bl_id);//or use stackBL_ID???    //if(%ownership && %brick.isBasePlate() && !$Server::LAN)
		
		if(%brName !$= "") //if(%brick.name)
			%file.writeLine("+-NTOBJECTNAME " @ %brName);
		
		for(%d = 0; %d < %brick.numEvents; %d++) 
			%file.writeLine("+-EVENT" TAB %brick.eventEnabled[%d] TAB %brick.eventDelay[%d] TAB %brick.eventInput[%d] TAB %brick.eventTarget[%d] TAB %brick.eventNT[%d] TAB %brick.eventOutput[%d] TAB "\"" @ %brick.eventoutputparameter[%d,1] @ "\"" TAB "\"" @ %brick.eventoutputparameter[%d,2] @ "\"" TAB "\"" @ %brick.eventoutputparameter[%d,3] @ "\"" TAB "\"" @ %brick.eventoutputparameter[%d,4] @ "\""); //thanks to Randy's saveBricks function for part of this line
		
		if(%brick.emitter)
			%file.writeLine("+-EMITTER " @ %brick.emitter.emitter SPC %brick.emitterDirection); //%brick.emitter.getEmitterDataBlock().uiName
		
		if(%brick.light)
			%file.writeLine("+-LIGHT " @ %brick.light.getDataBlock().getName()); // SPC 1); //1???
		
		if(%brick.item)
			%file.writeLine("+-ITEM " @ %brick.item.getDataBlock().getName() SPC %brick.itemPosition SPC %brick.itemDirection SPC %brick.itemRespawnTime);
		
		if(%brick.AudioEmitter)
			%file.writeLine("+-AUDIOEMITTER " @ %brick.audioEmitter.getProfileID().getName()); //%brick.audioEmitter.getProfileID().uiName @ "\"");
		
		if(%brick.Vehicle)
			%file.writeLine("+-VEHICLE " @ %brick.vehicleDataBlock.getName() SPC %brick.reColorVehicle);
	}
	
	//Adjust total brick count if an object in chunk isn't a planted brick (ghost brick or highlight obj) or is a boundary brick
	else // if(%brick.getDataBlock() !$= "fxDTSBrickData")
	{
		%objNum--;
		%count--;
	}
	
	if(%rmv)
		%brick.delete();
	
	if(%objNum++ >= %count)
	{
		if(%rmv)
			%Chunk.delete();
		else
		{
			%Chunk.ChEditedPTG = false;
			%Chunk.ChUnstablePTG = false;
		}
		
		%file.writeLine(">>END"); //prevent issues with not reading last line when loading chunks from file
		%file.close();
		%file.delete();
		
		switch$(%rtnFunc) //getWord(%readLn,0);
		{
			case "Cull":
			
				%chCount = getWord(%VARS,0);
				%CHtotalC = getWord(%VARS,1);
				
				//Ensure FIFO option
				if($PTG.FIFOchClr)
				{
					%lowestIDCh = 0;
					
					for(%c = 1; %c < BrickGroup_Chunks.getCount(); %c++)
					{
						if((%tmpCh = BrickGroup_Chunks.getObject(%c).getID()) < %lowestIDCh)
						{
							%lowestIDCh = %tmpCh;
							%lowestNumCh = %c;
						}
					}
					%chCount = %lowestNumCh;
				}
				else
					%chCount++;
				
				PTG_Routine_ChunkCull(%cl,%chCount,%CHtotalC);
				
			case "Bounds":
			
				PTG_Chunk_Gen_ChunkReCalcBounds(%cl,%Chunk,%VARS);
				
			case "AutoSave":
			
				%chCount = getWord(%VARS,0);
				%CHtotalC = getWord(%VARS,1);
				%incr = getWord(%VARS,2);
				
				PTG_Routine_AutoSaveChunks(%cl,%chCount++,%CHtotalC,%incr,false);
				
			case "Clear" or "Purge" or "ClearAll":
			
				%chCount = getWord(%VARS,0);
				%brCount = getWord(%VARS,1) + %objNum; //add new brick count to existing count //kept %objNum just encase, even though it should always be null at the beginning of this function
				%CHTotalC = getWord(%VARS,2);
				%clearStatic = getWord(%VARS,3);
				%dontSave = getWord(%VARS,4);
				%chPassCount = getWord(%VARS,5);
				
				PTGClear_Recurs(%cl,%chCount++,%brCount,%CHTotalC,%clearStatic,%dontSave,%rtnFunc,%chPassCount);
				
			case "Save":
			
				%chNum = getWord(%VARS,0);
				%chTotal = getWord(%VARS,1);
				%chSaveAm = getWord(%VARS,2);
				%chPotSaveAm = getWord(%VARS,3);
				
				PTGSave_Recurs(%cl,%chNum++,%chTotal,%chSaveAm++,%chPotSaveAm++);
		}
		
		return;
	}
	else
	{
		//$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_remMS,0,50));// + 0; //(%delay = mClamp($PTG.delay_priFuncMS,0,100));
		
		if($PTG.genSpeed < 2)
			scheduleNoQuota(mClamp($PTG.brDelay_remMS,0,50),0,PTG_Chunk_SaveRemoveBricks,%cl,%Chunk,%count,%objNum,%file,%rmv,%rtnFunc,%VARS);
		else
			PTG_Chunk_SaveRemoveBricks(%cl,%Chunk,%count,%objNum,%file,%rmv,%rtnFunc,%VARS);
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//test setting highest chunk pos via build loading
function PTG_Chunk_BuildLoad(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%file,%brick)
{
	%BrH_build = getWord($StrArrayData_Builds,0); //same as "getField(%var,0)" (?)
	%rot_build = getWord($StrArrayData_Builds,1); //rot file (check if rot enabled in advance to this func)
	%relGrid = getWord($StrArrayData_Builds,2);
	%relGridH = %relGrid / 2;
	%bnds_build = getField($StrArrayData_Builds,1);
	%buildName = getField($StrArrayData_Builds,2);
	%clrSetsMatch = getField($StrArrayData_Builds,3);
	
	%ChPosX_rel = mFloor(%ChPosX / %relGrid) * %relGrid;
	%ChPosY_rel = mFloor(%ChPosY / %relGrid) * %relGrid;
	%ChPosX_rem = %ChPosX_rel + %relGridH;
	%ChPosY_rem = %ChPosY_rel + %relGridH;
	
	//already rotates boundaries in noise function
	%xMin = getWord(%bnds_build,0);
	%xMax = getWord(%bnds_build,1);
	%yMin = getWord(%bnds_build,2);
	%yMax = getWord(%bnds_build,3);
	%zMin = getWord(%bnds_build,4);
	%zMax = getWord(%bnds_build,5);
		
	if(!isObject(%file))
	{
		//check if routines scriptobject ($PTG) exists? (for direct func exec in console)
		
		if(!isFile(%fp = PTG_GetFP("Build",%buildName,%rot_build,%relGrid)))
		{
			PTG_Chunk_Gen_Relay(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"Bounds",%BG);
			return;
		}
		
		%file = new FileObject();
		%file.openForRead(%fp);
		
		//////////////////////////////////////////////////
		//Colorset conversion, if necessary
		
		if(isFile(%fpB = PTG_GetFP("BuildInfo",%buildName,"",%relGrid)))
		{
			%fileB = new FileObject();
			%fileB.openForRead(%fpB);
		
			if(!%clrSetsMatch)
			{
				for(%c = 0; %c < 5; %c++)
					%fileB.readLine();
				
				//ColorSet Data and Conversion
				for(%c = 0; %c < 64; %c++)
				{
					%oldColorStr = %fileB.readLine();
					$colRefArr[%c] = PTG_FindClosestColor(%oldColorStr,"RGBA-RGBA"); //delete array when function is finished
				}
			}
			
			%fileB.close();
			%fileB.delete();
		}
		else
		{
			echo("\c2>>PTG ERROR -> \"PTG_Chunk_BuildLoad\": couldn't access Info.txt file for build \"" @ %buildName @ "\".");
			PTG_Chunk_Gen_Relay(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"Bounds",%BG);
			
			%fileAccFail = true;
		}
	}
	
	%readLn = %file.readLine();
	
	//////////////////////////////////////////////////

	if(!%file.isEOF() && !%fileAccFail)
	{
		%frstWrd = firstWord(%readLn);
		
		if(getSubStr(%readLn,0,7) $= "+-EVENT") 
			%frstWrd = "+-EVENT";
			
		switch$(%frstWrd)
		{			
			case "BRICKDATA":

				%db = getWord(%readLn,1);
				%pos = getWords(%readLn,2,4);
				%ang = getWord(%readLn,5);
				if(%clrSetsMatch)
					%col = getWord(%readLn,7);
				else
					%col = $colRefArr[getWord(%readLn,7)];
				%pri = getWord(%readLn,8);
				%colFX = getWord(%readLn,9);
				%shpFX = getWord(%readLn,10);
				%BrType = getWord(%readLn,14);
				
				%BrPosX = getWord(%pos,0);
				%BrPosY = getWord(%pos,1);
				%BrPosZ = getWord(%pos,2);

				%skip = true;
				
				if((%BrPosX + %ChPosX_rem) >= %ChPosX && (%BrPosX + %ChPosX_rem) < (%ChPosX + mClamp($PTGm.chSize,16,256))) //make sure chunk size is setup!
					if((%BrPosY + %ChPosY_rem) >= %ChPosY && (%BrPosY + %ChPosY_rem) < (%ChPosY + mClamp($PTGm.chSize,16,256)))
						%skip = false;
				
				
				if(isObject(%db) && !%skip)
				{
					%pos = setWord(%pos,0,%BrPosX+%ChPosX_rem);
					%pos = setWord(%pos,1,%BrPosY+%ChPosY_rem);
					%pos = setWord(%pos,2,%BrH_build + %BrPosZ);
					
					//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
					if(isObject(%Chunk) && ((%tempZMax = getWord(%pos,2) + (%db.brickSizeZ * 0.2 * 0.5)) > %Chunk.PTGHighestZpos))
						%Chunk.PTGHighestZpos = %tempZMax;
					
					//Print ID
					if(%pri !$= "")
						%pri = $printNameTable[%pri];
					else
						%pri = 0;

					
					//Plant Brick
					%brick = PTG_Chunk_PlantBrick(%db,%pos,%col,%colFX,%shpFX,%ang,%pri,%cl,%BG,%Chunk,%BrType); //brtype always playerbr?
					
					if(isObject(%brick))
					{
						%brick.setRayCasting(getWord(%readLn,11));
						%brick.setColliding(getWord(%readLn,12));
						%brick.setRendering(getWord(%readLn,13));
						%brick.isBasePlate = getWord(%readLn,6);
						
						if(%db.isWaterBrick)
							%brick.createWaterZone(); //If Water Brick
						else
						{
							if(%db.isBotHole)
							{
								if(!PTG_ObjLimitCheck("Bots"))
								{
									%brick.isBotHole = true;
									%brick.hBotType = %db.holeBot; //???
									
									if(!%brick.hBotType.hManualSpawn)
									{
										%brick.scheduleNoQuota(1000,spawnHoleBot); //.schedule //%brick.spawnHoleBot();
										%brick.hLastSpawnTime = getSimTime();
										//%brick.scheduleNoQuota(1000,onHoleSpawnPlanted); //%brick.onHoleSpawnPlanted(); //If Bot Hole
									}
									
									if(!isObject(mainHoleBotBrickSet))
										MissionCleanup.add(new SimSet("mainHoleBotBrickSet"));
									mainHoleBotBrickSet.add(%brick);
								}
							}
							else if(%db $= "brickSpawnPointData")
								%BG.addSpawnBrick(%BG,%brick); //register spawn brick as a player spawn
						}
					}
					else
						%brick = "";
				}
				else
					%brick = "";
			
			case "+-OWNER":
			
				if(%brick !$= "" && isObject(%brick))
				{
					%ownerID = getWord(%readLn,1);
					%brick.stackBL_ID = %ownerID;
					%BG = "BrickGroup_" @ %ownerID;
					%BGb = "BrickGroup_" @ $PTG.lastClientID;
					
					//If public bricks or load ownership option enabled
					if(!$PTG.publicBricksPBLs)
					{
						if(isObject(%BG))
							%BG.add(%brick);
						else if(isObject(%BGb))
							%BGb.add(%brick);
						else
							BrickGroup_888888.add(%brick);
					}
					else
						BrickGroup_888888.add(%brick);
				}
				
			case "+-NTOBJECTNAME":
			
				if(%brick !$= "" && isObject(%brick))
					%brick.setNTObjectName(getWord(%readLn,1));
				
			case "+-EVENT": //credit to duplicator script for following code-blocks (%brick.addEvent() requires a client (issue w/ ded servers) and doesn't seem to apply the output target brick-name):
			
				if(%brick !$= "" && isObject(%brick))
				{
					%num = getField(%readLn,1);
					%brick.eventEnabled[%num] = getField(%readLn, 2);
					%brick.eventInput[%num] = getField(%readLn, 3);
					%brick.eventInputIdx[%num] = inputEvent_GetInputEventIdx(%brick.eventInput[%num]);
					%brick.eventDelay[%num] = getField(%readLn, 4);
					%brick.eventTarget[%num] = getField(%readLn, 5);
					%brick.eventTargetIdx[%num] = inputEvent_GetTargetIndex("fxDtsBrick",%brick.eventInputIdx[%num],%brick.eventTarget[%num]);
					%brick.eventNT[%num] = getField(%readLn, 6);
					%outputClass = %brick.eventTargetIdx[%num] == -1 ? "fxDtsBrick" : inputEvent_GetTargetClass("fxDtsBrick",%brick.eventInputIdx[%num],%brick.eventTargetIdx[%num]);
					%brick.eventOutput[%num] = getField(%readLn, 7);
					%brick.eventOutputIdx[%num] = outputEvent_GetOutputEventIdx(%outputClass,%brick.eventOutput[%num]);
					%brick.eventOutputAppendClient[%num] = $OutputEvent_AppendClient[%outputClass,%brick.eventOutputIdx[%num]];
					
					for(%c = 1; %c <= 4; %c++)
					{
						%eventParamType = getField($OutputEvent_parameterList[%outputClass,%brick.eventOutputIdx[%num]],%c - 1);
						if(getWord(%eventParamType, 0) $= "dataBlock" && isObject(getField(%readLn,%c + 7)))
							%brick.eventOutputParameter[%num,%c] = getField(%readLn,%c + 7).getId();
						else
							%brick.eventOutputParameter[%num,%c] = getField(%readLn,%c + 7);
					}
				
					%brick.numEvents++;
					%brick.implicitCancelEvents = 0;
				}
				
			case "+-EMITTER":
			
				if(%brick !$= "" && isObject(%brick) && !PTG_ObjLimitCheck("PlyrEmitter"))
				{
					%brick.setEmitter(getWord(%readLn,1));
					%brick.setEmitterDirection(getWord(%readLn,2));
				}
				
			case "+-LIGHT":
			
				if(%brick !$= "" && isObject(%brick) && !PTG_ObjLimitCheck("PlyrLight"))
					%brick.setLight(getWord(%readLn,1));
			
			case "+-ITEM":
			
				if(%brick !$= "" && isObject(%brick) && !PTG_ObjLimitCheck("PlyrItems"))
				{
					%brick.setItem(getWord(%readLn,1));
					%brick.setItemPosition(getWord(%readLn,2));
					%brick.setItemDirection(getWord(%readLn,3));
					%brick.setItemRespawnTime(getWord(%readLn,4));
				}
				
			case "+-AUDIOEMITTER":
			
				if(%brick !$= "" && isObject(%brick))
					%brick.setMusic(getWord(%readLn,1));
			
			case "+-VEHICLE":
			
				//spawned vehicles float in the air for some reason, rather than resting on their spawn; disabled for now
				
				//if(%brick !$= "" && isObject(%brick)) // && !PTG_ObjLimitCheck("Vehicles")
				//{
					//%brick.scheduleNoQuota(1000,setVehicle,getWord(%readLn,1).getID(),%cl); //%brick.setVehicle(getWord(%readLn,1).getID(),%cl);
						//%brick.scheduleNoQuota(1000,respawnVehicle); // to fix issue w/ spawning (first is to update vehicle count for objlimitchecks)
					//%brick.scheduleNoQuota(33,setReColorVehicle,getWord(%readLn,2)); //%brick.setReColorVehicle(getWord(%readLn,2)); //check
				//}
		}
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//If reached end of file
	else
	{
		%file.close(); //if function cancelled, will not closing/deleting the function cause problems?
		%file.delete();
		
		deleteVariables("$colRefArr*");
		deleteVariables("$PTG_BldLoadFailSafe");

		PTG_Chunk_Gen_Relay(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,"Bounds",%BG);
		return;
	}
	
	if($PTG.genSpeed < 2)
	{
		$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_genMS,0,50));
		scheduleNoQuota(%delay,0,PTG_Chunk_BuildLoad,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%file,%brick);
	}
	else
	{
		if($PTG_BldLoadFailSafe++ > 250)
		{
			$PTG_BldLoadFailSafe = 0;
			$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.delay_priFuncMS,0,100));
			scheduleNoQuota(%delay,0,PTG_Chunk_BuildLoad,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%file,%brick);
		}
		else
			PTG_Chunk_BuildLoad(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,%file,%brick);
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Chunk_ChunkLoad(%cl,%Chunk,%file,%brick,%VARS) //,%rtnFunc
{
	if(!isObject(%file))
	{
		if(!isFile(%fp = PTG_GetFP("Chunk-Norm",%Chunk,"",""))) //PTG_Routine_Detect already checks if chunk file exists, but recheck here just encase
		{
			if(!isFile(%fp = PTG_GetFP("Chunk-Perm",%Chunk,"","")))
			{
				%Chunk.ChUnstablePTG = false;
				$PTG.dedSrvrFuncCheckTime = getSimTime();// + 0; //(%delay = mClamp($PTG.delay_priFuncMS,0,100));
				
				%CHPosX = getWord(%VARS,0);
				%CHPosY = getWord(%VARS,1);
				%xmod = getWord(%VARS,2);
				%ymod = getWord(%VARS,3);
				%clNum = getWord(%VARS,4);
				
				if($PTGm.genMethod $= "Complete")
					PTG_Routine_Append(%cl,%CHPosX,%CHPosY,%xmod++,%ymod,%clNum);
				else
				{
					if(%clNum++ >= clientgroup.getCount()) 
						%clNum = 0; //Make sure client exists? Or allow append function to check?
					
					PTG_Routine_Append(%cl,%CHPosX,%CHPosY,0,0,%clNum);
				}

				return;
			}
		}

		%file = new FileObject();
		%file.openForRead(%fp); //%file.openForRead(%fp = $PTG::FilePathCh @ "ChunksCache/" @ %Chunk @ ".txt");
	}

	%readLn = %file.readLine();

	if(!%file.isEOF())
	{
		%frstWrd = firstWord(%readLn);
		
		if((%tmpWrd = getField(%readLn,0)) $= "+-EVENT")
			%frstWrd = "+-EVENT";
		
		switch$(%frstWrd) //getWord(%readLn,0);
		{
			case ">>PTGChunk:":
				
				if($PTG.LoadChFileStc)
				{
					if((%stcV = getWord(%readLn,3)) $= "true") 
						%stcV = true;
					else 
						%stcV = false;
					
					%Chunk.ChStaticPTG = %stcV; //if chunk was saved as static or not, apply to new chunk
				}
				else
					%Chunk.ChStaticPTG = false;
				
				//ColorSet Data and Conversion
				for(%c = 0; %c < 64; %c++)
				{
					%oldColorStr = %file.readLine();
					$colRefArr[%c] = PTG_FindClosestColor(%oldColorStr,"RGBA-RGBA"); //delete array when function is finished
				}
				
			case "BRICKDATA":

				%db = getWord(%readLn,1); //check?
				%pos = getWords(%readLn,2,5);
				%ang = getWord(%readLn,5);
				%col = $colRefArr[getWord(%readLn,7)];
				%pri = getWord(%readLn,8);
				%colFX = getWord(%readLn,9);
				%shpFX = getWord(%readLn,10);
				%BrType = getWord(%readLn,14);
				
				//If is stream brick, prevent from dynamically updating when loaded from chunk save
				if(%BrType $= "StreamBr" || %BrType $= "StreamBrAux")
				{
					%BrType = "StreamBrTert"; //"DetailBr" identifying added along with "StreamBr" / "StreamBrAux" in "PTG_Chunk_PlantBrick" function instead
					%StreamBr = true;
				}
				
				if(isObject(%db))
				{
					//Store highest point for chunk relative to top of each brick (for boundaries, if enabled)
					if(isObject(%Chunk) && ((%tempZMax = getWord(%pos,2) + (%db.brickSizeZ * 0.2 * 0.5)) > %Chunk.PTGHighestZpos))
						%Chunk.PTGHighestZpos = %tempZMax;
				
					//Print ID
					if(%pri !$= "")	
						%pri = $printNameTable[%pri];
					else 
						%pri = 0;

					%brick = PTG_Chunk_PlantBrick(%db,%pos,%col,%colFX,%shpFX,%ang,%pri,%cl,%BG,%Chunk,%BrType,""); //!!! update other plantbrick functions to nullify %BrMod at end like here
					
					if(%brick !$= "" && isObject(%brick))
					{
						%brick.setRayCasting(getWord(%readLn,11));
						%brick.setRendering(getWord(%readLn,13));
						%brick.isBasePlate = getWord(%readLn,6);
						
						//If is stream brick and if streams are set to be colliding or not (not loaded from file, but rather based on current settings under Routines)
						if(!%StreamBr)
							%brick.setColliding(getWord(%readLn,12));
						else
						{
							if($PTG.solidStreams)
								%brick.setColliding(1);
							else
								%brick.setColliding(0);
												
							//If physical water zone create is enabled (and if stream brick isn't a plate)
							if($PTG.genStreamZones && %db.brickSizeZ > 1)
							{
								%brick.createWaterZone();
								%brick.PhysicalZone.setWaterColor(getColorIDTable(%col));
								%brick.PhysicalZone.waterDensity = 0.5;
							}
						}
						
						if(%db.isWaterBrick)
						{
							//%brick.createWaterZone(); //If Water Brick //Should be done automatically within PlantBrick function
							
							switch$(%brType)
							{
								case "WaterIceBr":
								
									%brick.setColliding(1); //doesn't work when only set in PlantBrick function
									%brick.setShapeFX(0);
									
									%brick.setEmitter("FogEmitterA"); //doesn't work when only set in PlantBrick function
										%watPos = getWord(%pos,0) SPC getWord(%pos,1) SPC getWord(%pos,2) + (%waterZrel / 2);
										%brick.emitter.setTransform(%watPos @ " 1 0 0 0");
										%brick.emitter.setScale("16 16 0.1");

								case "WaterLavaBr":
								
									%brick.setColorFX(3); //glow
									%brick.setShapeFX(1);
									%brick.PhysicalZone.waterType = 8; //lava (burns players within zone)
									
									%brick.setEmitter("BurnEmitterA");
										%watPos = getWord(%pos,0) SPC getWord(%pos,1) SPC getWord(%pos,2) + (%waterZrel / 2);
										%brick.emitter.setTransform(%watPos @ " 1 0 0 0");
										%brick.emitter.setScale("16 16 0.1");
										
								case "WaterQuickSandBr":

									%brick.setShapeFX(0);
									%brick.PhysicalZone.extraDrag = 8;
									%brick.PhysicalZone.waterViscosity = 120;
									%brick.PhysicalZone.waterDensity = 0.5;
							}
						}
						else
						{
							if(%db.isBotHole)
							{
								if(!PTG_ObjLimitCheck("Bots"))
								{
									%brick.isBotHole = true;
									%brick.hBotType = %db.holeBot; //???
									
									if(!%brick.hBotType.hManualSpawn)
									{
										%brick.scheduleNoQuota(1000,spawnHoleBot); //.schedule //%brick.spawnHoleBot();
										%brick.hLastSpawnTime = getSimTime();
										//%brick.scheduleNoQuota(1000,onHoleSpawnPlanted); //%brick.onHoleSpawnPlanted(); //If Bot Hole
									}
									
									if(!isObject(mainHoleBotBrickSet))
										MissionCleanup.add(new SimSet("mainHoleBotBrickSet"));
									mainHoleBotBrickSet.add(%brick);
								}
							}
							else if(%db $= "brickSpawnPointData")
								%BG.addSpawnBrick(%BG,%brick); //register spawn brick as a player spawn
						}
					}
					else
						%brick = "";
				}
				else
					%brick = "";
			
			case "+-OWNER":
			
				if(%brick !$= "" && isObject(%brick))
				{
					%ownerID = getWord(%readLn,1);
					%brick.stackBL_ID = %ownerID; //? brick already set up...
					%BG = "BrickGroup_" @ %ownerID;
					%BGb = "BrickGroup_" @ $PTG.lastClientID;
					
					//If public bricks or load ownership option enabled
					if(!$PTG.publicBricksPBLs)
					{
						if(isObject(%BG))
							%BG.add(%brick);
						else if(isObject(%BGb))
							%BGb.add(%brick);
						else
							BrickGroup_888888.add(%brick);
					}
					else
						BrickGroup_888888.add(%brick);
				}
				
			case "+-NTOBJECTNAME":
			
				if(%brick !$= "" && isObject(%brick)) 
					%brick.setNTObjectName(getWord(%readLn,1));
				
			case "+-EVENT":
			
				if(%brick !$= "" && isObject(%brick))
				{
					%readLn = stripChars(%readLn,"\"");
					
					//Thanks to the duplicator for the following code (slightly modified) //!!!!!!!!! Also check for build loading !!!!!!!!!
					if((%num = %brick.numEvents) == 0)
						%num = "0"; //prevents issue with setting first event
					
					%brick.eventEnabled[%num] = getField(%readLn, 1);
					%brick.eventInput[%num] = getField(%readLn, 3);
					%brick.eventInputIdx[%num] = inputEvent_GetInputEventIdx(%brick.eventInput[%num]);
					%brick.eventDelay[%num] = getField(%readLn, 2);
					%brick.eventTarget[%num] = getField(%readLn, 4);
					%brick.eventTargetIdx[%num] = inputEvent_GetTargetIndex("fxDtsBrick",%brick.eventInputIdx[%num],%brick.eventTarget[%num]);
					%brick.eventNT[%num] = getField(%readLn, 5);
					%outputClass = %brick.eventTargetIdx[%num] == -1 ? "fxDtsBrick" : inputEvent_GetTargetClass("fxDtsBrick",%brick.eventInputIdx[%num],%brick.eventTargetIdx[%num]);
					%brick.eventOutput[%num] = getField(%readLn, 6);
					%brick.eventOutputIdx[%num] = outputEvent_GetOutputEventIdx(%outputClass,%brick.eventOutput[%num]);
					%brick.eventOutputAppendClient[%num] = $OutputEvent_AppendClient[%outputClass,%brick.eventOutputIdx[%num]];

					for(%c = 0; %c < 4; %c++)
					{
						%eventParamType = getField($OutputEvent_parameterList[%outputClass,%brick.eventOutputIdx[%num]],%c); //%c - 1 //make sure this works (should)
						if(getWord(%eventParamType,0) $= "dataBlock" && isObject(getField(%readLn,%c + 7)))
							%brick.eventOutputParameter[%num,%c + 1] = getField(%readLn,%c + 7).getId();
						else
							%brick.eventOutputParameter[%num,%c + 1] = getField(%readLn,%c + 7);
					}

					%brick.numEvents++;
					%brick.implicitCancelEvents = 0;
				}
				
			case "+-EMITTER":
			
				if(%brick !$= "" && isObject(%brick) && !PTG_ObjLimitCheck("PlyrEmitter"))
				{
					%brick.setEmitter(getWord(%readLn,1));
					%brick.setEmitterDirection(getWord(%readLn,2));
				}
				
			case "+-LIGHT":
			
				if(%brick !$= "" && isObject(%brick) && !PTG_ObjLimitCheck("PlyrLight")) 
					%brick.setLight(getWord(%readLn,1));
			
			case "+-ITEM":
			
				if(%brick !$= "" && isObject(%brick) && !PTG_ObjLimitCheck("PlyrItems"))
				{
					%brick.setItem(getWord(%readLn,1));
					%brick.setItemPosition(getWord(%readLn,2));
					%brick.setItemDirection(getWord(%readLn,3));
					%brick.setItemRespawnTime(getWord(%readLn,4));
				}
				
			case "+-AUDIOEMITTER":
			
				if(%brick !$= "" && isObject(%brick))
					%brick.setMusic(getWord(%readLn,1));
			
			case "+-VEHICLE":
			
				//spawned vehicles float in the air for some reason, rather than resting on their spawn; disabled for now
				//if(%brick !$= "" && isObject(%brick) && !PTG_ObjLimitCheck("Vehicles"))
				//{
				//	%brick.setVehicle(getWord(%readLn,1).getID(),%cl);
				//	%brick.setReColorVehicle(getWord(%readLn,2));
				//}
		}
	}
	
	
	//If reached end of file
	else
	{
		%file.close(); //if function cancelled, will not closing/deleting the function cause problems?
		%file.delete();
		
		deleteVariables("$colRefArr*");
		deleteVariables("$PTG_ChLoadFailSafe");
		%Chunk.ChUnstablePTG = false;
		%Chunk.PTGChSize = mClamp($PTGm.chSize,16,256);

		%CHPosX = getword(%VARS,0);
		%CHPosY = getword(%VARS,1);
		%xmod = getword(%VARS,2);
		%ymod = getword(%VARS,3);
		%clNum = getword(%VARS,4);
		
		if($PTG.publicBricks || !isObject(%BGb = "BrickGroup_" @ $PTG.lastClientID))
			%BG = "BrickGroup_888888";
		else
			%BG = %BGb;
		
		
		//If chunks are highlighted, add new highlight object for newly created chunk
		%Chunk.PTGHighestZpos = mCeil((%Chunk.PTGHighestZpos + 16) / 32) * 32;
		if(isObject(BrickGroup_HighlightChunks))
		{
			//If highest point isn't stored in chunk (i.e. if created for planted / loaded brick), run through all objects in chunk and find highest
			%newChZ = mCeil((%Chunk.PTGHighestZpos + 16) / 32);
			%pos = %CHPosX+(mClamp($PTGm.chSize,16,256) / 2) SPC %CHPosY+(mClamp($PTGm.chSize,16,256) / 2) SPC ((%newChZ / 2) * 32); //$PTGm.boundsHLevel+2;
			%scale = (mClamp($PTGm.chSize,16,256) / 16) SPC (mClamp($PTGm.chSize,16,256) / 16) SPC %newChZ;
			
			if(%Chunk.ChStaticPTG)
				%HLCtype = "HL-Static";
			else
				%HLCtype = "HL-NonStatic";
			
			//Spawn static shape (add to chunk itself) and add to highlight chunk brickgroup
			%tmpStcObj = PTG_Chunk_SpawnStatic(%Chunk,%pos,%scale,%HLCtype);
			BrickGroup_HighlightChunks.add(%tmpStcObj);
			%tmpStcObj.setName(strReplace(%Chunk.getName(),"Chunk","HLChunk")); //already displays name???
		}
		

		//Boundaries (if enabled)
		if($PTGm.enabBounds)// && $PTGm.genType !$= "Infinite")
			PTG_Chunk_Gen_Boundaries(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%BG,"Append");
		
		//Gradual or Entire grid generation per player (reset chunk pos on grid - for next player - or append next chunk for gradual loading?)
		else
		{
			$PTG.dedSrvrFuncCheckTime = getSimTime();// + 0; //(%delay = mClamp($PTG.delay_priFuncMS,0,100));
			
			if($PTGm.genMethod $= "Complete")
				PTG_Routine_Append(%cl,%CHPosX,%CHPosY,%xmod++,%ymod,%clNum);
			else
			{
				if(%clNum++ >= clientgroup.getCount()) 
					%clNum = 0; //Make sure client exists? Or allow append function to check?
				
				PTG_Routine_Append(%cl,%CHPosX,%CHPosY,0,0,%clNum);
			}
		}

		return;
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////

	if($PTG.genSpeed < 2)
	{
		$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.brDelay_genMS,0,50));
		scheduleNoQuota(%delay,0,PTG_Chunk_ChunkLoad,%cl,%Chunk,%file,%brick,%VARS);
	}
	else
	{
		if($PTG_ChLoadFailSafe++ > 250)
		{
			$PTG_ChLoadFailSafe = 0;
			$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.delay_priFuncMS,0,100));
			scheduleNoQuota(%delay,0,PTG_Chunk_ChunkLoad,%cl,%Chunk,%file,%brick,%VARS);
		}
		else
			PTG_Chunk_ChunkLoad(%cl,%Chunk,%file,%brick,%VARS);
	}
}