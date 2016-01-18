// NOTE: The following noise functions are custom made, and are the result of over 2-3 years of work. Please give credit if using these or any other PTG
//scripts / functions in your own projects - thanks.
	

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////// INCOHERENT NOISE / INITIAL COHERENT NOISE CALC. (NON-VOID) ////////

//// GENERATE PSEUDO-RANDOM INTEGER CORNER HEIGHT-VALUES FOR CHUNK ////
function PTG_RandNumGen_Chunk(%CHPosX,%CHPosY,%RelItrSize,%IncA,%IncB,%MultA,%MultB)
{
	%ChPosX = getSubStr(%CHPosX,0,8);
	%ChPosY = getSubStr(%CHPosY,0,8);
	%ChPosXb = %ChPosX+%RelItrSize;
	%ChPosYb = %ChPosY+%RelItrSize;
	
	$PTGm.zMod = mClamp($PTGm.zMod,0,200);

	//Generate random initial values using a custom LCG method
	%PosXa = ((((%ChPosX + %IncA + getSubStr($PTGm.seed,0,8)) % 99999) * %MultA) % $PTGm.zMod); // "% 99999" is to prevent a known issue with discrepancies of scientific notation between the engine on Macs and PCs
	%PosXb = ((((%ChPosXb + %IncA + getSubStr($PTGm.seed,0,8)) % 99999) * %MultA) % $PTGm.zMod);
	%PosYa = ((((%ChPosY + %IncB + getSubStr($PTGm.seed,0,8)) % 99999) * %MultB) % $PTGm.zMod);
	%PosYb = ((((%ChPosYb + %IncB + getSubStr($PTGm.seed,0,8)) % 99999) * %MultB) % $PTGm.zMod);

	//Interpolate initial values to find final height values; return string
	return mAbs(%PosYa-%PosXa) SPC mAbs(%PosYb-%PosXa) SPC mAbs(%PosYa-%PosXb) SPC mAbs(%PosYb-%PosXb);
}


////////////////////////////////////////////////////////////////////////////////////////////////////

//// GENERATE PSEUDO-RANDOM INTEGER (TYPE AND FREQUENCY) VALUES FOR DETAILS ////
function PTG_RandNumGen_Details(%value)
{
	%value = getSubStr(%value,0,8);
	
	%detNum = (((%value + 48205429 + getSubStr($PTGm.seed,0,8)) % 99999) * 50153) % 114; //Common, Uncommon and Rare details -> (6*16)+(4*4)+(2*1) = 114
	%freq = (((%value + 10212011 + getSubStr($PTGm.seed,0,8)) % 99999) * 64577) % 100; //0% to 100% gen frequency

	return %detNum SPC %freq;	
}


////////////////////////////////////////////////////////////////////////////////////////////////////

//// GENERATE PSEUDO-RANDOM INTEGER (TYPE AND FREQUENCY) VALUES FOR DETAILS ////
function PTG_RandNumGen_Builds(%ChPosX,%ChPosY)
{
	%ChPosX = getSubStr(%CHPosX,0,8);
	%ChPosY = getSubStr(%CHPosY,0,8);
	%freq_FltAr = $PTGm.flatAreaFreq;
	%FltArA = mClamp($PTGm.BLfaGridSizeSmall,2,256);
	%FltArB = mClamp($PTGm.BLfaGridSizeLarge,2,256);
	%calcFltAr = $PTGm.allowFlatAreas; //$PTGm.enabBuildLoad && 
	%calcBuildL = $PTGm.enabBuildLoad;
	
	for(%c = 256; %c >= 2 ; %c /= 2)
	{
		%ChPosX_rel = (mFloor(%ChPosX / %c) * %c) + $PTGm.buildLoad_X;
		%ChPosY_rel = (mFloor(%ChPosY / %c) * %c) + $PTGm.buildLoad_Y;
		%ChPos_res = mFloor(%ChPosX_rel * %ChPosY_rel * %c); //negative results give different modulo values than positive results (i)
		%pass = (((%ChPos_res + 40544951 + getSubStr($PTGm.seed,0,8)) % 99999) * 72253) % 8000;
		
		if(%pass < 4000)
		{
			%freq = (((%ChPos_res + 75372313 + getSubStr($PTGm.seed,0,8)) % 99999) * 53453) % 100; ////%freqSel_FltAr = (((%ChPos_res + 31622777 + getSubStr($PTGm.seed,0,8)) % 99999) * 59281) % 100;
			
			if(%calcFltAr && %freq < %freq_FltAr && %c >= %FltArA && %c <= %FltArB) //Flat Areas (add noise pass to find grid size between %FltArA and %FltArB?)
				return %c SPC "" SPC "" SPC "" SPC "" SPC true;
			if(%calcBuildL && (%fileCount = getFileCount(PTG_GetFP("RelGridFldr","","",%c))) > 0) //?
			{
				%relGridSz = %c;
				break;
			}
		}
	}
	if(%relGridSz == 0)
		%relGridSz = mClamp($PTGm.chSize,16,256); //Use 2 instead?

	%rot = (((%ChPos_res + 42638597 + getSubStr($PTGm.seed,0,8)) % 99999) * 77171) % 4000; //make sure rot file exists!
	%rot = mFloor(%rot / 1000);

	%fileNum = (((%ChPos_res + 67898771 + getSubStr($PTGm.seed,0,8)) % 99999) * 21319) % (%fileCount * 1000);
	%fileNum = mFloor(%fileNum / 1000);
	
	//if(%fileCount == 1 && %fileNum == 0)
	//	%fileNum = 1;

	return %relGridSz SPC %freq SPC %rot SPC %fileCount SPC %fileNum; // SPC %genFltAr;
}


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////// COHERENT NOISE CALC. (VOID) ////////

//// GENERATE PSEUDO-RANDOM INTEGER (TYPE AND FREQUENCY) VALUES FOR DETAILS ////
function PTG_BuildLoadCheck(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum,%dntRtrn,%relXMod,%relYMod)
{
	//!!! reference var instead of $PTGm.brTer_XYsize !!!
	
	//Figure random grid size, frequency, rotation and build to load
	%tmpVars = PTG_RandNumGen_Builds(%ChPosX,%ChPosY);
	%relGridSz = getWord(%tmpVars,0);
	%relGridHSz = %relGridSz / 2;
	%freqRef = getWord(%tmpVars,1);
	%rotRef = getWord(%tmpVars,2);
	%fileCount = getWord(%tmpVars,3);
	%fileNum = mClamp(getWord(%tmpVars,4),0,400); //hard-limit of 400
	//%freqSel_FltAr = getWord(%tmpVars,5); //for flat building areas (if enabled)
	%genFltAr = getWord(%tmpVars,5); //,6
	
	%ChPosX_rel = mFloor(%ChPosX / %relGridSz) * %relGridSz;
	%ChPosY_rel = mFloor(%ChPosY / %relGridSz) * %relGridSz;
	%ChPosX_rem = %ChPosX - %ChPosX_rel;
	%ChPosY_rem = %ChPosY - %ChPosY_rel;
	
	%brHalfSize_terXY = $PTGm.brTer_XYsize / 2;
	%brHalfSize_fltIsldsXY = $PTGm.brFltIslds_XYsize / 2;
	%enabSkylands = $PTGm.terType $= "SkyLands";
	
	//Build Loading (if enabled)
	if(!%genFltAr && $PTGm.enabBuildLoad)
	{
		//Find / access file data
		%tmpFP = PTG_GetFP("RelGridFldr","","",%relGridSz);
		
		if(%fileCount > 0)
		{
			//if(%fileNum > 0)
				for(%c = 0; %c < (%fileNum + 1); %c++)
					%tmpFile = findNextFile(%tmpFP);
			//else 
			//	%tmpFile = findFirstFile(%tmpFP); //findNextFile?
		}
		if(!isFile(%tmpFile) || %fileCount == 0) //take into account when attempting to load build later
		{
			if(!%dntRtrn)
			{
				$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_secFuncMS,0,20));
				scheduleNoQuota(%delay,0,PTG_Routine_Calc,%cl,"Final",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}
			return;
		}
		
		//Check build conditions for file
		%file = new FileObject(); //make sure rot file exists first !!!
		%file.openForRead(%tmpFile);
		
		%file.readLine();
		%conds = %file.readLine();
		%bnds = getWords(%file.readLine(),1,6);
		%file.readLine(); //owner
		%buildName = getSubStr(getField(%file.readLine(),1),0,30);
		
		%enab = getWord(%conds,1); //if enabled
		%freq = getWord(%conds,3);
		if(%rot = getWord(%conds,5)) //if rotation enabled
			%rot = %rotRef;
		else
			%rot = 0;
		%bioDef = getWord(%conds,7);
		%bioShore = getWord(%conds,9);
		%bioSubM = getWord(%conds,11);
		%bioCustA = getWord(%conds,13);
		%bioCustB = getWord(%conds,15);
		%bioCustC = getWord(%conds,17);
		%wat = getWord(%conds,19);
		%mntns = getWord(%conds,21);
		%fltIslds = getWord(%conds,23);
		
		//check if colorsets match
		%clrSetsMatc = true;
		for(%c = 0; %c < 64 && %clrSetsMatc; %c++)
		{
			if(%file.readLine() !$= getColorIDTable(%c))
				%clrSetsMatc = false;
		}
		
		%file.close();
		%file.delete();

		//Frequency and file check
		
		if(%freq < %freqRef || !%enab) //don't use <= since freq is based on modulus value (freq % # is != #)
		{
			if(!%dntRtrn)
			{
				$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_secFuncMS,0,20));
				scheduleNoQuota(%delay,0,PTG_Routine_Calc,%cl,"Final",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}
			return;
		}
		if(!isFile(%tmpFile = strReplace(%tmpFile,"Info",%rot))) //find / verify rotation file
		{
			if(!%dntRtrn)
			{
				$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_secFuncMS,0,20));
				scheduleNoQuota(%delay,0,PTG_Routine_Calc,%cl,"Final",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}
			return;
		}

		//Apply conditions
		switch(%rot)
		{
			case 0 or 2:
				%xMin = getWord(%bnds,0);
				%xMax = getWord(%bnds,1);
				%yMin = getWord(%bnds,2);
				%yMax = getWord(%bnds,3);
			case 1 or 3:
				%xMin = getWord(%bnds,2);
				%xMax = getWord(%bnds,3);
				%yMin = getWord(%bnds,0);
				%yMax = getWord(%bnds,1);
		}
		%zMin = getWord(%bnds,4);
		%zMax = getWord(%bnds,5);
		%bnds = %xMin SPC %xMax SPC %yMin SPC %yMax SPC %zMin SPC %zMax;
	}
	
	//////////////////////////////////////////////////
	//Flat Area Generation (if enabled)
	
	else
	{
		if(%genFltAr)
		{
			//%bnds = -%relGridHSz+$PTGm.brTer_XYsize SPC %relGridHSz-$PTGm.brTer_XYsize SPC -%relGridHSz+$PTGm.brTer_XYsize SPC %relGridHSz-$PTGm.brTer_XYsize SPC 0 SPC 1; //%BrH_build = getWord(PTG_Noise_BuildGridHRef(%ChPosX,%ChPosY,%relGridSz,%bnds),0);
			%bnds = -%relGridSz SPC %relGridSz SPC -%relGridSz SPC %relGridSz SPC 0 SPC 1;
			
			%xMin = -%relGridHSz;//+$PTGm.brTer_XYsize+$PTGm.brTer_XYsize; //what about the floating islands brick size???
			%xMax = %relGridHSz;//-$PTGm.brTer_XYsize-$PTGm.brTer_XYsize;
			%yMin =  -%relGridHSz;//+$PTGm.brTer_XYsize+$PTGm.brTer_XYsize;
			%yMax = %relGridHSz;//-$PTGm.brTer_XYsize-$PTGm.brTer_XYsize;
			
			%bioDef = true;
			%bioShore = true;
			%bioSubM = false;
			%bioCustA = true;
			%bioCustB = true;
			%bioCustC = true;
				%wat = false;
			%mntns = true;
			%fltIslds = true;
		}
		else
		{
			if(!%dntRtrn)
			{
				$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_secFuncMS,0,20));
				scheduleNoQuota(%delay,0,PTG_Routine_Calc,%cl,"Final",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}
			return;
		}
	}
	
	//////////////////////////////////////////////////
	//Check landscape relative to build size and conditions
	
	%buildPassCheck = PTG_Noise_BuildGridHRef(%ChPosX,%ChPosY,%relGridSz,%bnds);
	%BrH_build = getWord(%buildPassCheck,0);
	%bioDefPass = getWord(%buildPassCheck,1);
	%bioDefFail = getWord(%buildPassCheck,2);
	%bioShorePass = getWord(%buildPassCheck,3);
	%bioShoreFail = getWord(%buildPassCheck,4);
	%bioSubMPass = getWord(%buildPassCheck,5);
	%bioSubMFail = getWord(%buildPassCheck,6);
	%bioCustAPass = getWord(%buildPassCheck,7);
	%bioCustAFail = getWord(%buildPassCheck,8);
	%bioCustBPass = getWord(%buildPassCheck,9);
	%bioCustBFail = getWord(%buildPassCheck,10);
	%bioCustCPass = getWord(%buildPassCheck,11);
	%bioCustCFail = getWord(%buildPassCheck,12);
	%watPass = getWord(%buildPassCheck,13);
	%watFail = getWord(%buildPassCheck,14);
	%mntnsPass = getWord(%buildPassCheck,15);
	%mntnsFail = getWord(%buildPassCheck,16);
	%fltIsldsAPass = getWord(%buildPassCheck,17);
	%fltIsldsAFail = getWord(%buildPassCheck,18);
	%fltIsldsBPass = getWord(%buildPassCheck,19);
	%fltIsldsBFail = getWord(%buildPassCheck,20);
	%BrH_FIa_build = getWord(%buildPassCheck,21);
	%BrH_FIb_build = getWord(%buildPassCheck,22);
	%FISecCutA = getWord(%buildPassCheck,23);
	%FISecCutB = getWord(%buildPassCheck,24);
	%BrH_skld_build = getWord(%buildPassCheck,25);
	%SkyLandPass = getWord(%buildPassCheck,26);
	%SkyLandFail = getWord(%buildPassCheck,27);
	
	//add another random noise check to choose either floating islands A or B, water, or terrain, and to attempt to gen on anything if all else fails (?)
	//conditions: for var names, "Pass" means at least one instance check passed, "Fail" means at least one instance check failed.

	////////////////////////////////////////////////////////////////////////////////////////////////////
	//Floating Islands Check
	
	if(%fltIslds)
	{
		if(%fltIsldsAPass && !%fltIsldsAFail && %BrH_FIa_build > 0 && %BrH_FIa_build > %BrH_FIb_build)
		{
			if(!%FISecCutA)
			{
				%genFltIsldsA = true; //%BrH_FIa_build+$PTGm.fltIsldsAHLevel-$PTGm.fltIsldsSecZ
				if(!%dntRtrn) $StrArrayData_Builds = %BrH_FIa_build SPC %rot SPC %relGridSz TAB %bnds TAB %buildName TAB %clrSetsMatc TAB "FltIsldA";
			}
		}
		if(%fltIsldsBPass && !%fltIsldsBFail && %BrH_FIb_build > 0 && %BrH_FIa_build < %BrH_FIb_build)
		{
			if(!%FISecCutB)
			{
				%genFltIsldsB = true; //%BrH_FIb_build+$PTGm.fltIsldsBHLevel-$PTGm.fltIsldsSecZ
				if(!%dntRtrn) $StrArrayData_Builds = %BrH_FIb_build SPC %rot SPC %relGridSz TAB %bnds TAB %buildName TAB %clrSetsMatc TAB "FltIsldB";
			}
		}
		
		if(%FISecCutA && %FISecCutA)
		{
			if(!%dntRtrn)
			{
				$StrArrayData_Builds = "";
				$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_secFuncMS,0,20));
				scheduleNoQuota(%delay,0,PTG_Routine_Calc,%cl,"Final",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
			}
			return;
		}

		if(%genFltIsldsA || %genFltIsldsB)
		{
			for(%BrPosY = %brHalfSize_fltIsldsXY; %BrPosY < mClamp($PTGm.chSize,16,256); %BrPosY += $PTGm.brFltIslds_XYsize)
			{
				for(%BrPosX = %brHalfSize_fltIsldsXY; %BrPosX < mClamp($PTGm.chSize,16,256); %BrPosX += $PTGm.brFltIslds_XYsize)
				{
					if(((%ChPosX_rem + %BrPosX - %brHalfSize_fltIsldsXY) < (%relGridHSz + %xMax)) && ((%ChPosX_rem + %BrPosX + %brHalfSize_fltIsldsXY) >= (%relGridHSz + %xMin))) //was this what was wrong with paths? max should be before min!
					{
						if(((%ChPosY_rem + %BrPosY - %brHalfSize_fltIsldsXY) < (%relGridHSz + %yMax)) && ((%ChPosY_rem + %BrPosY + %brHalfSize_fltIsldsXY) >= (%relGridHSz + %yMin))) //- / + %brHalfSize_fltIsldsXY
						{
							if(%genFltIsldsA)
							{
								$StrArrayHV_FltIsldsATop[%BrPosX+%relXMod,%BrPosY+%relYMod] = %BrH_FIa_build;
								//$StrArrayHV_FltIsldsABtm[%BrPosX+%relXMod,%BrPosY+%relYMod] = getMin($StrArrayHV_FltIsldsABtm[%BrPosX+%relXMod,%BrPosY+%relYMod],%BrH_FIa_build-$PTGm.brFltIslds_XYsize);
							}
							else if(%genFltIsldsB)
							{
								$StrArrayHV_FltIsldsBTop[%BrPosX+%relXMod,%BrPosY+%relYMod] = %BrH_FIb_build;
								//$StrArrayHV_FltIsldsBBtm[%BrPosX+%relXMod,%BrPosY+%relYMod] = getMin($StrArrayHV_FltIsldsBBtm[%BrPosX+%relXMod,%BrPosY+%relYMod],%BrH_FIb_build-$PTGm.brFltIslds_XYsize);
							}
						}
					}
				}
			}
		}
	}
	
	//////////////////////////////////////////////////
	
	if($PTGm.terType !$= "SkyLands" || (%SkyLandPass && !%SkyLandFail))
	{
		//Water Level Check
		if(!%genFltIsldsA && !%genFltIsldsB && %wat)
		{
			if(%wat && %watPass && !%watFail && $PTGm.terType !$= "SkyLands") // && !%terCheck) //(make sure terrain doesn't intersect water level where build will be loaded)
			{
				//%subMCheck = (!%bioSubM && !%bioSubMPass && %bioSubMFail) || (%bioSubM && ((%BrH_build + %zMax) < $PTGm.lakesHLevel || %terrainCheck || %shoreCheck || %mntnsCheck));
				if((!%bioSubM || ((%BrH_build + %zMax) >= $PTGm.lakesHLevel)) && %bioShorePass && !%bioShoreFail)
				{
					if(!%dntRtrn) $StrArrayData_Builds = $PTGm.lakesHLevel SPC %rot SPC %relGridSz TAB %bnds TAB %buildName TAB %clrSetsMatc;
					%genWat = true;
				}
			}
		}
		
		//////////////////////////////////////////////////
		//Terrain or SkyLands - Biomes, SubMarine, Mountains, etc.
		
		if(!%genFltIsldsA && !%genFltIsldsB && !%genWat)
		{
			//If normal terrain check passes, also check shore, submarine, biomes and mountains (default to true for a condition if enabled - not necessary to check)
			%terrainCheck = (!%bioDef && !%bioDefPass && %bioDefFail) || (%bioDef) || (%mntns && %mntnsPass && !%mntnsFail);
			%shoreCheck = (!%bioShore && !%bioShorePass && %bioShoreFail) || (%bioShore);
			%biomesCheckA = (!%bioCustA && !%bioCustAPass && %bioCustAFail) || (%bioCustA && %terrainCheck);
			%biomesCheckB = (!%bioCustB && !%bioCustBPass && %bioCustBFail) || (%bioCustB && %terrainCheck);
			%biomesCheckC = (!%bioCustC && !%bioCustCPass && %bioCustCFail) || (%bioCustC && %terrainCheck);
			%mntnsCheck = (!%mntns && !%mntnsPass && %mntnsFail) || (%mntns); //(!%bioSubMPass && %bioSubMFail && %bioDefPass && !%bioDefFail)
			//%subMCheck = (!%bioSubM && !%bioSubMPass && %bioSubMFail) || (%bioSubM && (((%BrH_build + %zMax) < $PTGm.lakesHLevel) || %terrainCheck || %shoreCheck || %mntnsCheck));
			%subMCheck = (%bioSubM && %bioSubMPass && !%bioSubMFail && ((%BrH_build + %zMax) < $PTGm.lakesHLevel));
			
			%mainPass = ((%terrainCheck && %mntnsCheck && %shoreCheck) && (%biomesCheckA && %biomesCheckB && %biomesCheckC) && (!%bioSubMPass || %bioSubMFail)) || %subMCheck;
			
			if(%mainPass)
			{
				//Set up global reference var for chunk scripts (if not calculating corner chunks)
				if(!%dntRtrn) 
				{
					if(%enabSkylands)
					{
						//%tmpBrH = ($PTGm.terHLevel + (%BrH_build - $PTGm.terHLevel - $PTGm.skyLndsSecZ)) + %BrH_skld_build;
						//%tmpBrH = mFloor(%tmpBrH / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
						$StrArrayData_Builds = %BrH_skld_build SPC %rot SPC %relGridSz TAB %bnds TAB %buildName TAB %clrSetsMatc TAB "Terrain";
					}
					else
						$StrArrayData_Builds = %BrH_build SPC %rot SPC %relGridSz TAB %bnds TAB %buildName TAB %clrSetsMatc TAB "Terrain";
				}
				
				//Adjust height reference arrays
				for(%BrPosY = %brHalfSize_terXY; %BrPosY < mClamp($PTGm.chSize,16,256); %BrPosY += $PTGm.brTer_XYsize)
				{
					for(%BrPosX = %brHalfSize_terXY; %BrPosX < mClamp($PTGm.chSize,16,256); %BrPosX += $PTGm.brTer_XYsize)
					{
						if(((%ChPosX_rem + %BrPosX - %brHalfSize_terXY) < (%relGridHSz + %xMax)) && ((%ChPosX_rem + %BrPosX + %brHalfSize_terXY) >= (%relGridHSz + %xMin))) //was this what was wrong with paths? max should be before min!
						{
							if(((%ChPosY_rem + %BrPosY - %brHalfSize_terXY) < (%relGridHSz + %yMax)) && ((%ChPosY_rem + %BrPosY + %brHalfSize_terXY) >= (%relGridHSz + %yMin))) // //- / + %brHalfSize_terXY
							{
								//SkyLands
								if(%enabSkylands)
									$StrArrayHV_SkyLandsTop[%BrPosX+%relXMod,%BrPosY+%relYMod] = %BrH_skld_build;
								
								//Normal Terrain (and caves - SkyLands don't generate caves)
								else
								{
									$StrArrayHV_Mountains[%BrPosX+%relXMod,%BrPosY+%relYMod] = %BrH_build; //take edges into account? (for caves below also if so)
								
									//Adjust cave top / btm layer heights, if necessary (i.e. if they conflict with flattened terrain)
									if(($StrArrayHV_CavesB[%BrPosX,%BrPosY] + $PTGm.brTer_Zsize) > (%BrH_build - $PTGm.brTer_Zsize) && $StrArrayHV_CavesA[%BrPosX,%BrPosY] <= %BrH_build)
									{
										$StrArrayHV_CavesB[%BrPosX+%relXMod,%BrPosY+%relYMod] = %BrH_build - $PTGm.brTer_FillXYZSize; //or just set both A&B to 0 no matter what
										
										if($StrArrayHV_CavesA[%BrPosX,%BrPosY] > (%BrH_build - $PTGm.brTer_FillXYZSize))
										{
											$StrArrayHV_CavesA[%BrPosX+%relXMod,%BrPosY+%relYMod] = 0;
											$StrArrayHV_CavesB[%BrPosX+%relXMod,%BrPosY+%relYMod] = 0;
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
	

	if(!%dntRtrn)
	{
		$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_secFuncMS,0,20));
		scheduleNoQuota(%delay,0,PTG_Routine_Calc,%cl,"Final",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
	}
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Noise_3ItrFractal_Terrain(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{
	%MinBrZSnap = $PTGm.brTer_Zsize; // $PTGm.brTer_FillXYZSize + $PTGm.brTer_Zsize;
	%BrXYSize = $PTGm.brTer_XYsize;
	%BrXYhSize = %BrXYSize / 2;
	%itrCSize = $PTGm.ter_itrC_XY;
	%ChSize = mClamp($PTGm.chSize,16,256);
	
	//if($PTGm.enabEdgeFallOff && $PTGm.genType $= "Finite") 
	//	%edgeFallOff = true;

	%ChPxRelItrA = (mFloor(%CHPosX / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrB_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrB_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.terOff_X,%CHPyRelItrA+$PTGm.terOff_Y,$PTGm.ter_itrB_XY,13333227,49979687,13313,15551);
	%ChPxActItrA = (mFloor(%CHPosX / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrA_XY;

	%ChHVItrAmod = %ChHVItrA;
	if($PTGm.enabBio_CustA) //Custom Biome A
	{
		%bioCheckStr = PTG_Noise_Perlin_BiomeCheck(%CHPosX,%CHPosY,$PTGm.bio_CustA_itrA_XY,$PTGm.bio_CustA_itrA_Z,$PTGm.bio_CustAOff_X,$PTGm.bio_CustAOff_Y,$PTGm.Bio_CustA_YStart);
		
		if((getWord(%bioCheckStr,0)) < $PTGm.bio_CustASecZ) %ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * $PTGbio.Bio_CustA_TerHMod);
		if((getWord(%bioCheckStr,2)) < $PTGm.bio_CustASecZ) %ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * $PTGbio.Bio_CustA_TerHMod);
		if((getWord(%bioCheckStr,1)) < $PTGm.bio_CustASecZ) %ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * $PTGbio.Bio_CustA_TerHMod);
		if((getWord(%bioCheckStr,3)) < $PTGm.bio_CustASecZ) %ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * $PTGbio.Bio_CustA_TerHMod);
	}
	if($PTGm.enabBio_CustB) //Custom Biome B
	{
		%bioCheckStr = PTG_Noise_Perlin_BiomeCheck(%CHPosX,%CHPosY,$PTGm.bio_CustB_itrA_XY,$PTGm.bio_CustB_itrA_Z,$PTGm.bio_CustBOff_X,$PTGm.bio_CustBOff_Y,$PTGm.Bio_CustB_YStart);
		
		if((getWord(%bioCheckStr,0)) < $PTGm.bio_CustBSecZ) %ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * $PTGbio.Bio_CustB_TerHMod);
		if((getWord(%bioCheckStr,2)) < $PTGm.bio_CustBSecZ) %ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * $PTGbio.Bio_CustB_TerHMod);
		if((getWord(%bioCheckStr,1)) < $PTGm.bio_CustBSecZ) %ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * $PTGbio.Bio_CustB_TerHMod);
		if((getWord(%bioCheckStr,3)) < $PTGm.bio_CustBSecZ) %ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * $PTGbio.Bio_CustB_TerHMod);
	}
	if($PTGm.enabBio_CustC) //Custom Biome C
	{
		%bioCheckStr = PTG_Noise_Perlin_BiomeCheck(%CHPosX,%CHPosY,$PTGm.bio_CustC_itrA_XY,$PTGm.bio_CustC_itrA_Z,$PTGm.bio_CustCOff_X,$PTGm.bio_CustCOff_Y,$PTGm.Bio_CustC_YStart);
		
		if((getWord(%bioCheckStr,0)) < $PTGm.bio_CustCSecZ) %ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * $PTGbio.Bio_CustC_TerHMod);
		if((getWord(%bioCheckStr,2)) < $PTGm.bio_CustCSecZ) %ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * $PTGbio.Bio_CustC_TerHMod);
		if((getWord(%bioCheckStr,1)) < $PTGm.bio_CustCSecZ) %ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * $PTGbio.Bio_CustC_TerHMod);
		if((getWord(%bioCheckStr,3)) < $PTGm.bio_CustCSecZ) %ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * $PTGbio.Bio_CustC_TerHMod);
	}
	%ChHVItrA = %ChHVItrAmod;

	%CHPyRelItrB = (mFloor(%CHPosY / $PTGm.ter_itrB_XY)) * $PTGm.ter_itrB_XY;
	%ChPxRelItrB = (mFloor(%CHPosX / $PTGm.ter_itrB_XY)) * $PTGm.ter_itrB_XY;
	%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.terOff_X,%CHPyRelItrB+$PTGm.terOff_Y,$PTGm.ter_itrB_XY,13333227,49979687,13313,15551);

	for(%BrPosY = %BrXYhSize; %BrPosY < %ChSize; %BrPosY += %itrCSize)
	{
		for(%BrPosX = %BrXYhSize; %BrPosX < %ChSize; %BrPosX += %itrCSize)
		{
			%ChPyRelItrC = (mFloor((%CHPosY+%BrPosY) / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrB_XY;
			%ChPxRelItrC = (mFloor((%CHPosX+%BrPosX) / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrB_XY;
			%ChHVItrC = PTG_RandNumGen_Chunk(%CHPxRelItrC+$PTGm.terOff_X,%CHPyRelItrC+$PTGm.terOff_Y,$PTGm.ter_itrB_XY,13333227,49979687,13313,15551);
			%BrPyActItrC = (mFloor(%BrPosY / %itrCSize)) * %itrCSize; //%ChPosY + %BrPosY? Then dont' have to use %BrPosY-%BrPyActITrC below!!!
			%BrPxActItrC = (mFloor(%BrPosX / %itrCSize)) * %itrCSize;

			for(%BrPosYb = 0; %BrPosYb < %itrCSize; %BrPosYb += %BrXYSize)
			{
				for(%BrPosXb = 0; %BrPosXb < %itrCSize; %BrPosXb += %BrXYSize)
				{
					%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY+%BrPosYb) / $PTGm.ter_itrA_XY;
					%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
					%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
					%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

					%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY+%BrPosYb) / $PTGm.ter_itrB_XY;
					%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
					%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
					%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
					
					%Co = (((%BrPosY+%BrPosYb)-%BrPyActItrC)) / %itrCSize;
					%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
					%ItrCColL = getWord(%ChHVItrC,0) - ((getWord(%ChHVItrC,0) - getWord(%ChHVItrC,1)) * %Sm);
					%ItrCColR = getWord(%ChHVItrC,2) - ((getWord(%ChHVItrC,2) - getWord(%ChHVItrC,3)) * %Sm);
					
					%Co = ((%CHPosX-%ChPxActItrA)+%BrPosX+%BrPosXb) / $PTGm.ter_itrA_XY;
					%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
					%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
					
					%Co = ((%CHPosX-%CHPxRelItrB)+%BrPosX+%BrPosXb) / $PTGm.ter_itrB_XY;
					%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
					%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);
					
					%Co = (((%BrPosX+%BrPosXb)-%BrPxActItrC)) / $PTGm.ter_itrC_XY;
					%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
					%ItrCRow = %ItrCColL - ((%ItrCColL - %ItrCColR) * %Sm);

					%relZ = (%ItrARow*$PTGm.ter_itrA_Z) + (%ItrBRow*$PTGm.ter_itrB_Z) + (%ItrCRow*$PTGm.ter_itrC_Z) + $PTGm.terHLevel;

					//Lakes / Connect Lakes option
					if($PTGm.enabCnctLakes && (%relZ - $PTGm.terHLevel) < ($PTGm.lakesHLevel+$PTGm.cnctLakesStrt) && $PTGm.terType !$= "SkyLands")
						%relZ -= (($PTGm.lakesHLevel+$PTGm.cnctLakesStrt) - %relZ);
				
					//Edge-FallOff
					%relZ = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX+%BrPosXb,%ChPosY+%BrPosY+%BrPosYb,%relZ);
					
					%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
					%BrH = getMax(%tempZ,%MinBrZSnap);
					$StrArrayHV_Terrain[%BrPosX+%BrPosXb,%BrPosY+%BrPosYb] = %BrH;
					
					
					//%relZstr = %relZstr SPC %ItrARow SPC %ItrBRow SPC %ItrCRow @ " |"; //unused?
					//%terStr = %terStr SPC %BrH; //unused?
					
					
					if(!$PTGm.enabMntns) //If mountains are disabled, set up height value array to reference terrain instead (since some functions still use it)
						$StrArrayHV_Mountains[%BrPosX+%BrPosXb,%BrPosY+%BrPosYb] = %BrH;
				}
			}
		}
	}

	$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_priFuncMS,0,100));
	scheduleNoQuota(%delay,0,PTG_Noise_3ItrFractal_Terrain_Edges,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Noise_3ItrFractal_Terrain_Edges(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{
	%MinBrZSnap = $PTGm.brTer_Zsize; // $PTGm.brTer_FillXYZSize + $PTGm.brTer_Zsize;
	%BrXYSize = $PTGm.brTer_XYsize;
	%BrXYhSize = %BrXYSize / 2;
	%itrCSize = $PTGm.ter_itrC_XY;
	%ChSize = mClamp($PTGm.chSize,16,256);
	
	//if($PTGm.enabEdgeFallOff && $PTGm.genType $= "Finite") 
	//	%edgeFallOff = true;
	
	//////////////////////////////////////////////////
	//Left Side

	%tempCHPosX = %ChPosX - %ChSize;
	%tempBrPosX = mClamp($PTGm.chSize,16,256) - %BrXYhSize;//$PTGm.brTer_XYsize;
	%BrPosX = -1 * %BrXYhSize;//$PTGm.brTer_XYsize;
	
	%ChPxRelItrA = (mFloor(%tempCHPosX / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrB_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrB_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.terOff_X,%CHPyRelItrA+$PTGm.terOff_Y,$PTGm.ter_itrB_XY,13333227,49979687,13313,15551);
	%ChPxActItrA = (mFloor(%tempCHPosX / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrA_XY;
	
	%ChHVItrAmod = %ChHVItrA;
	if($PTGm.enabBio_CustA) //Custom Biome A
	{
		%bioCheckStr = PTG_Noise_Perlin_BiomeCheck(%tempCHPosX,%CHPosY,$PTGm.bio_CustA_itrA_XY,$PTGm.bio_CustA_itrA_Z,$PTGm.bio_CustAOff_X,$PTGm.bio_CustAOff_Y,$PTGm.Bio_CustA_YStart);
		%Section_Biome = $PTGm.bio_CustASecZ;
		%Hmod = $PTGbio.Bio_CustA_TerHMod;
		
		if((getWord(%bioCheckStr,0)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * %Hmod);
		if((getWord(%bioCheckStr,2)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * %Hmod);
		if((getWord(%bioCheckStr,1)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * %Hmod);
		if((getWord(%bioCheckStr,3)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * %Hmod);
	}
	if($PTGm.enabBio_CustB) //Custom Biome B
	{
		%bioCheckStr = PTG_Noise_Perlin_BiomeCheck(%tempCHPosX,%CHPosY,$PTGm.bio_CustB_itrA_XY,$PTGm.bio_CustB_itrA_Z,$PTGm.bio_CustBOff_X,$PTGm.bio_CustBOff_Y,$PTGm.Bio_CustB_YStart);
		%Section_Biome = $PTGm.bio_CustBSecZ;
		%Hmod = $PTGbio.Bio_CustB_TerHMod;
		
		if((getWord(%bioCheckStr,0)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * %Hmod);
		if((getWord(%bioCheckStr,2)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * %Hmod);
		if((getWord(%bioCheckStr,1)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * %Hmod);
		if((getWord(%bioCheckStr,3)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * %Hmod);
	}
	if($PTGm.enabBio_CustC) //Custom Biome C
	{
		%bioCheckStr = PTG_Noise_Perlin_BiomeCheck(%tempCHPosX,%CHPosY,$PTGm.bio_CustC_itrA_XY,$PTGm.bio_CustC_itrA_Z,$PTGm.bio_CustCOff_X,$PTGm.bio_CustCOff_Y,$PTGm.Bio_CustC_YStart);
		%Section_Biome = $PTGm.bio_CustCSecZ;
		%Hmod = $PTGbio.Bio_CustC_TerHMod;
		
		if((getWord(%bioCheckStr,0)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * %Hmod);
		if((getWord(%bioCheckStr,2)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * %Hmod);
		if((getWord(%bioCheckStr,1)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * %Hmod);
		if((getWord(%bioCheckStr,3)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * %Hmod);
	}
	%ChHVItrA = %ChHVItrAmod;
	
	%CHPyRelItrB = (mFloor(%CHPosY / $PTGm.ter_itrB_XY)) * $PTGm.ter_itrB_XY;
	%ChPxRelItrB = (mFloor(%tempCHPosX / $PTGm.ter_itrB_XY)) * $PTGm.ter_itrB_XY;
	%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.terOff_X,%CHPyRelItrB+$PTGm.terOff_Y,$PTGm.ter_itrB_XY,13333227,49979687,13313,15551);
	
	for(%BrPosY = %BrXYhSize; %BrPosY < %ChSize; %BrPosY += %itrCSize)
	{
		%ChPyRelItrC = (mFloor((%CHPosY+%BrPosY) / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrB_XY;
		%ChPxRelItrC = (mFloor((%tempCHPosX+%tempBrPosX) / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrB_XY;
		%ChHVItrC = PTG_RandNumGen_Chunk(%CHPxRelItrC+$PTGm.terOff_X,%CHPyRelItrC+$PTGm.terOff_Y,$PTGm.ter_itrB_XY,13333227,49979687,13313,15551);
		%BrPyActItrC = (mFloor(%BrPosY / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrC_XY;
		%BrPxActItrC = (mFloor(%tempBrPosX / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrC_XY;
			
		for(%BrPosYb = 0; %BrPosYb < %itrCSize; %BrPosYb += %BrXYSize)
		{
			%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY+%BrPosYb) / $PTGm.ter_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

			%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY+%BrPosYb) / $PTGm.ter_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
			%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
			
			%Co = ((%BrPosY+%BrPosYb)-%BrPyActItrC) / $PTGm.ter_itrC_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrCColL = getWord(%ChHVItrC,0) - ((getWord(%ChHVItrC,0) - getWord(%ChHVItrC,1)) * %Sm);
			%ItrCColR = getWord(%ChHVItrC,2) - ((getWord(%ChHVItrC,2) - getWord(%ChHVItrC,3)) * %Sm);
			
			%Co = ((%tempCHPosX-%ChPxActItrA)+%tempBrPosX) / $PTGm.ter_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
			
			%Co = ((%tempCHPosX-%CHPxRelItrB)+%tempBrPosX) / $PTGm.ter_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);
			
			%Co = (%tempBrPosX-%BrPxActItrC) / $PTGm.ter_itrC_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrCRow = %ItrCColL - ((%ItrCColL - %ItrCColR) * %Sm);
			
			%relZ = (%ItrARow*$PTGm.ter_itrA_Z) + (%ItrBRow*$PTGm.ter_itrB_Z) + (%ItrCRow*$PTGm.ter_itrC_Z) + $PTGm.terHLevel;
			
			//Lakes / Connect Lakes option
			if($PTGm.enabCnctLakes && (%relZ - $PTGm.terHLevel) < ($PTGm.lakesHLevel+$PTGm.cnctLakesStrt) && $PTGm.terType !$= "SkyLands")
				%relZ -= (($PTGm.lakesHLevel+$PTGm.cnctLakesStrt) - %relZ);

			//Edge-FallOff
			%relZ = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY+%BrPosYb,%relZ);
				
			%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
			%BrH = getMax(%tempZ,%MinBrZSnap);
			$StrArrayHV_Terrain[%BrPosX,%BrPosY+%BrPosYb] = %BrH;
			
			if(!$PTGm.enabMntns)
				$StrArrayHV_Mountains[%BrPosX,%BrPosY+%BrPosYb] = %BrH;
		}
	}
	
	//////////////////////////////////////////////////
	//Right Side

	%tempCHPosX = %ChPosX + %ChSize;
	%tempBrPosX = %BrXYhSize;//0;
	%BrPosX = %ChSize+%BrXYhSize;
	
	%ChPxRelItrA = (mFloor(%tempCHPosX / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrB_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrB_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.terOff_X,%CHPyRelItrA+$PTGm.terOff_Y,$PTGm.ter_itrB_XY,13333227,49979687,13313,15551);
	%ChPxActItrA = (mFloor(%tempCHPosX / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrA_XY;
	
	%ChHVItrAmod = %ChHVItrA;
	if($PTGm.enabBio_CustA) //Custom Biome A
	{
		%bioCheckStr = PTG_Noise_Perlin_BiomeCheck(%tempCHPosX,%CHPosY,$PTGm.bio_CustA_itrA_XY,$PTGm.bio_CustA_itrA_Z,$PTGm.bio_CustAOff_X,$PTGm.bio_CustAOff_Y,$PTGm.Bio_CustA_YStart);
		%Section_Biome = $PTGm.bio_CustASecZ;
		%Hmod = $PTGbio.Bio_CustA_TerHMod;
		
		if((getWord(%bioCheckStr,0)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * %Hmod);
		if((getWord(%bioCheckStr,2)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * %Hmod);
		if((getWord(%bioCheckStr,1)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * %Hmod);
		if((getWord(%bioCheckStr,3)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * %Hmod);
	}
	if($PTGm.enabBio_CustB) //Custom Biome B
	{
		%bioCheckStr = PTG_Noise_Perlin_BiomeCheck(%tempCHPosX,%CHPosY,$PTGm.bio_CustB_itrA_XY,$PTGm.bio_CustB_itrA_Z,$PTGm.bio_CustBOff_X,$PTGm.bio_CustBOff_Y,$PTGm.Bio_CustB_YStart);
		%Section_Biome = $PTGm.bio_CustBSecZ;
		%Hmod = $PTGbio.Bio_CustB_TerHMod;
		
		if((getWord(%bioCheckStr,0)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * %Hmod);
		if((getWord(%bioCheckStr,2)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * %Hmod);
		if((getWord(%bioCheckStr,1)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * %Hmod);
		if((getWord(%bioCheckStr,3)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * %Hmod);
	}
	if($PTGm.enabBio_CustC) //Custom Biome C
	{
		%bioCheckStr = PTG_Noise_Perlin_BiomeCheck(%tempCHPosX,%CHPosY,$PTGm.bio_CustC_itrA_XY,$PTGm.bio_CustC_itrA_Z,$PTGm.bio_CustCOff_X,$PTGm.bio_CustCOff_Y,$PTGm.Bio_CustC_YStart);
		%Section_Biome = $PTGm.bio_CustCSecZ;
		%Hmod = $PTGbio.Bio_CustC_TerHMod;
		
		if((getWord(%bioCheckStr,0)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * %Hmod);
		if((getWord(%bioCheckStr,2)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * %Hmod);
		if((getWord(%bioCheckStr,1)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * %Hmod);
		if((getWord(%bioCheckStr,3)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * %Hmod);
	}
	%ChHVItrA = %ChHVItrAmod;
	
	
	%CHPyRelItrB = (mFloor(%CHPosY / $PTGm.ter_itrB_XY)) * $PTGm.ter_itrB_XY;
	%ChPxRelItrB = (mFloor(%tempCHPosX / $PTGm.ter_itrB_XY)) * $PTGm.ter_itrB_XY;
	%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.terOff_X,%CHPyRelItrB+$PTGm.terOff_Y,$PTGm.ter_itrB_XY,13333227,49979687,13313,15551);
	
	for(%BrPosY = %BrXYhSize; %BrPosY < %ChSize; %BrPosY += $PTGm.brTer_XYsize)
	{
		%ChPyRelItrC = (mFloor((%CHPosY+%BrPosY) / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrB_XY;
		%ChPxRelItrC = (mFloor((%tempCHPosX+%tempBrPosX) / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrB_XY;
		%ChHVItrC = PTG_RandNumGen_Chunk(%CHPxRelItrC+$PTGm.terOff_X,%CHPyRelItrC+$PTGm.terOff_Y,$PTGm.ter_itrB_XY,13333227,49979687,13313,15551);
		%BrPyActItrC = (mFloor(%BrPosY / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrC_XY;
		%BrPxActItrC = (mFloor(%tempBrPosX / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrC_XY;
			
		for(%BrPosYb = 0; %BrPosYb < %itrCSize; %BrPosYb += %BrXYSize)
		{
			%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY+%BrPosYb) / $PTGm.ter_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

			%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY+%BrPosYb) / $PTGm.ter_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
			%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
			
			%Co = ((%BrPosY+%BrPosYb)-%BrPyActItrC) / $PTGm.ter_itrC_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrCColL = getWord(%ChHVItrC,0) - ((getWord(%ChHVItrC,0) - getWord(%ChHVItrC,1)) * %Sm);
			%ItrCColR = getWord(%ChHVItrC,2) - ((getWord(%ChHVItrC,2) - getWord(%ChHVItrC,3)) * %Sm);
			
			%Co = ((%tempCHPosX-%ChPxActItrA)+%tempBrPosX) / $PTGm.ter_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
			
			%Co = ((%tempCHPosX-%CHPxRelItrB)+%tempBrPosX) / $PTGm.ter_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);
			
			%Co = (%tempBrPosX-%BrPxActItrC) / $PTGm.ter_itrC_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrCRow = %ItrCColL - ((%ItrCColL - %ItrCColR) * %Sm);
			
			%relZ = (%ItrARow*$PTGm.ter_itrA_Z) + (%ItrBRow*$PTGm.ter_itrB_Z) + (%ItrCRow*$PTGm.ter_itrC_Z) + $PTGm.terHLevel;
			
			//Lakes / Connect Lakes option
			if($PTGm.enabCnctLakes && (%relZ - $PTGm.terHLevel) < ($PTGm.lakesHLevel+$PTGm.cnctLakesStrt) && $PTGm.terType !$= "SkyLands")
				%relZ -= (($PTGm.lakesHLevel+$PTGm.cnctLakesStrt) - %relZ);

			//Edge-FallOff
			%relZ = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY+%BrPosYb,%relZ);
			
			%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
			%BrH = getMax(%tempZ,%MinBrZSnap);
			$StrArrayHV_Terrain[%BrPosX,%BrPosY+%BrPosYb] = %BrH;
			
			if(!$PTGm.enabMntns)
				$StrArrayHV_Mountains[%BrPosX,%BrPosY+%BrPosYb] = %BrH;
		}
	}
	
	//////////////////////////////////////////////////
	//Bottom Side

	%tempCHPosY = %ChPosY - %ChSize;
	%tempBrPosY = %ChSize - %BrXYhSize;//$PTGm.brTer_XYsize;
	%BrPosY = -1 * %BrXYhSize;//$PTGm.brTer_XYsize;
	
	%ChPxRelItrA = (mFloor(%ChPosX / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrB_XY;
	%ChPyRelItrA = (mFloor(%tempCHPosY / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrB_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.terOff_X,%CHPyRelItrA+$PTGm.terOff_Y,$PTGm.ter_itrB_XY,13333227,49979687,13313,15551);
	%ChPxActItrA = (mFloor(%ChPosX / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrA_XY;
	%ChPyActItrA = (mFloor(%tempCHPosY / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrA_XY;
	
	%ChHVItrAmod = %ChHVItrA;
	if($PTGm.enabBio_CustA) //Custom Biome A
	{
		%bioCheckStr = PTG_Noise_Perlin_BiomeCheck(%CHPosX,%tempCHPosY,$PTGm.bio_CustA_itrA_XY,$PTGm.bio_CustA_itrA_Z,$PTGm.bio_CustAOff_X,$PTGm.bio_CustAOff_Y,$PTGm.Bio_CustA_YStart);
		%Section_Biome = $PTGm.bio_CustASecZ;
		%Hmod = $PTGbio.Bio_CustA_TerHMod;
		
		if((getWord(%bioCheckStr,0)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * %Hmod);
		if((getWord(%bioCheckStr,2)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * %Hmod);
		if((getWord(%bioCheckStr,1)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * %Hmod);
		if((getWord(%bioCheckStr,3)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * %Hmod);
	}
	if($PTGm.enabBio_CustB) //Custom Biome B
	{
		%bioCheckStr = PTG_Noise_Perlin_BiomeCheck(%CHPosX,%tempCHPosY,$PTGm.bio_CustB_itrA_XY,$PTGm.bio_CustB_itrA_Z,$PTGm.bio_CustBOff_X,$PTGm.bio_CustBOff_Y,$PTGm.Bio_CustB_YStart);
		%Section_Biome = $PTGm.bio_CustBSecZ;
		%Hmod = $PTGbio.Bio_CustB_TerHMod;
		
		if((getWord(%bioCheckStr,0)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * %Hmod);
		if((getWord(%bioCheckStr,2)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * %Hmod);
		if((getWord(%bioCheckStr,1)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * %Hmod);
		if((getWord(%bioCheckStr,3)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * %Hmod);
	}
	if($PTGm.enabBio_CustC) //Custom Biome C
	{
		%bioCheckStr = PTG_Noise_Perlin_BiomeCheck(%CHPosX,%tempCHPosY,$PTGm.bio_CustC_itrA_XY,$PTGm.bio_CustC_itrA_Z,$PTGm.bio_CustCOff_X,$PTGm.bio_CustCOff_Y,$PTGm.Bio_CustC_YStart);
		%Section_Biome = $PTGm.bio_CustCSecZ;
		%Hmod = $PTGbio.Bio_CustC_TerHMod;
		
		if((getWord(%bioCheckStr,0)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * %Hmod);
		if((getWord(%bioCheckStr,2)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * %Hmod);
		if((getWord(%bioCheckStr,1)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * %Hmod);
		if((getWord(%bioCheckStr,3)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * %Hmod);
	}
	%ChHVItrA = %ChHVItrAmod;
	
	
	%CHPyRelItrB = (mFloor(%tempCHPosY / $PTGm.ter_itrB_XY)) * $PTGm.ter_itrB_XY;
	%ChPxRelItrB = (mFloor(%ChPosX / $PTGm.ter_itrB_XY)) * $PTGm.ter_itrB_XY;
	%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.terOff_X,%CHPyRelItrB+$PTGm.terOff_Y,$PTGm.ter_itrB_XY,13333227,49979687,13313,15551);
	
	for(%BrPosX = %BrXYhSize; %BrPosX < %ChSize; %BrPosX += $PTGm.brTer_XYsize)
	{
		for(%BrPosXb = 0; %BrPosXb < %itrCSize; %BrPosXb += %BrXYSize)
		{
			%ChPyRelItrC = (mFloor((%tempCHPosY+%tempBrPosY) / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrB_XY;
			%ChPxRelItrC = (mFloor((%ChPosX+%BrPosX) / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrB_XY;
			%ChHVItrC = PTG_RandNumGen_Chunk(%CHPxRelItrC+$PTGm.terOff_X,%CHPyRelItrC+$PTGm.terOff_Y,$PTGm.ter_itrB_XY,13333227,49979687,13313,15551);
			%BrPyActItrC = (mFloor(%tempBrPosY / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrC_XY;
			%BrPxActItrC = (mFloor(%BrPosX / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrC_XY;
		
			%Co = ((%tempCHPosY-%ChPyActItrA)+%tempBrPosY) / $PTGm.ter_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

			%Co = ((%tempCHPosY-%CHPyRelItrB)+%tempBrPosY) / $PTGm.ter_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
			%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
			
			%Co = (%tempBrPosY-%BrPyActItrC) / $PTGm.ter_itrC_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrCColL = getWord(%ChHVItrC,0) - ((getWord(%ChHVItrC,0) - getWord(%ChHVItrC,1)) * %Sm);
			%ItrCColR = getWord(%ChHVItrC,2) - ((getWord(%ChHVItrC,2) - getWord(%ChHVItrC,3)) * %Sm);
			
			%Co = ((%ChPosX-%ChPxActItrA)+%BrPosX+%BrPosXb) / $PTGm.ter_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
			
			%Co = ((%ChPosX-%CHPxRelItrB)+%BrPosX+%BrPosXb) / $PTGm.ter_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);
			
			%Co = ((%BrPosX+%BrPosXb)-%BrPxActItrC) / $PTGm.ter_itrC_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrCRow = %ItrCColL - ((%ItrCColL - %ItrCColR) * %Sm);
			
			%relZ = (%ItrARow*$PTGm.ter_itrA_Z) + (%ItrBRow*$PTGm.ter_itrB_Z) + (%ItrCRow*$PTGm.ter_itrC_Z) + $PTGm.terHLevel;
			
			//Lakes / Connect Lakes option
			if($PTGm.enabCnctLakes && (%relZ - $PTGm.terHLevel) < ($PTGm.lakesHLevel+$PTGm.cnctLakesStrt) && $PTGm.terType !$= "SkyLands")
				%relZ -= (($PTGm.lakesHLevel+$PTGm.cnctLakesStrt) - %relZ);
			
			//Edge-FallOff
			%relZ = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX+%BrPosXb,%ChPosY+%BrPosY,%relZ);
			
			%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
			%BrH = getMax(%tempZ,%MinBrZSnap);
			$StrArrayHV_Terrain[%BrPosX+%BrPosXb,%BrPosY] = %BrH;
			
			if(!$PTGm.enabMntns)
				$StrArrayHV_Mountains[%BrPosX+%BrPosXb,%BrPosY] = %BrH;
		}
	}
	
	//////////////////////////////////////////////////
	//Top Side

	%tempCHPosY = %ChPosY + %ChSize;
	%tempBrPosY = %BrXYhSize;//0;
	%BrPosY = %ChSize+%BrXYhSize;
	
	%ChPxRelItrA = (mFloor(%ChPosX / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrB_XY;
	%ChPyRelItrA = (mFloor(%tempCHPosY / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrB_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.terOff_X,%CHPyRelItrA+$PTGm.terOff_Y,$PTGm.ter_itrB_XY,13333227,49979687,13313,15551);
	%ChPxActItrA = (mFloor(%ChPosX / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrA_XY;
	%ChPyActItrA = (mFloor(%tempCHPosY / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrA_XY;
	
	%ChHVItrAmod = %ChHVItrA;
	if($PTGm.enabBio_CustA) //Custom Biome A
	{
		%bioCheckStr = PTG_Noise_Perlin_BiomeCheck(%CHPosX,%tempCHPosY,$PTGm.bio_CustA_itrA_XY,$PTGm.bio_CustA_itrA_Z,$PTGm.bio_CustAOff_X,$PTGm.bio_CustAOff_Y,$PTGm.Bio_CustA_YStart);
		%Section_Biome = $PTGm.bio_CustASecZ;
		%Hmod = $PTGbio.Bio_CustA_TerHMod;
		
		if((getWord(%bioCheckStr,0)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * %Hmod);
		if((getWord(%bioCheckStr,2)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * %Hmod);
		if((getWord(%bioCheckStr,1)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * %Hmod);
		if((getWord(%bioCheckStr,3)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * %Hmod);
	}
	if($PTGm.enabBio_CustB) //Custom Biome B
	{
		%bioCheckStr = PTG_Noise_Perlin_BiomeCheck(%CHPosX,%tempCHPosY,$PTGm.bio_CustB_itrA_XY,$PTGm.bio_CustB_itrA_Z,$PTGm.bio_CustBOff_X,$PTGm.bio_CustBOff_Y,$PTGm.Bio_CustB_YStart);
		%Section_Biome = $PTGm.bio_CustBSecZ;
		%Hmod = $PTGbio.Bio_CustB_TerHMod;
		
		if((getWord(%bioCheckStr,0)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * %Hmod);
		if((getWord(%bioCheckStr,2)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * %Hmod);
		if((getWord(%bioCheckStr,1)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * %Hmod);
		if((getWord(%bioCheckStr,3)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * %Hmod);
	}
	if($PTGm.enabBio_CustC) //Custom Biome C
	{
		%bioCheckStr = PTG_Noise_Perlin_BiomeCheck(%CHPosX,%tempCHPosY,$PTGm.bio_CustC_itrA_XY,$PTGm.bio_CustC_itrA_Z,$PTGm.bio_CustCOff_X,$PTGm.bio_CustCOff_Y,$PTGm.Bio_CustC_YStart);
		%Section_Biome = $PTGm.bio_CustCSecZ;
		%Hmod = $PTGbio.Bio_CustC_TerHMod;
		
		if((getWord(%bioCheckStr,0)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * %Hmod);
		if((getWord(%bioCheckStr,2)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * %Hmod);
		if((getWord(%bioCheckStr,1)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * %Hmod);
		if((getWord(%bioCheckStr,3)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * %Hmod);
	}
	%ChHVItrA = %ChHVItrAmod;
	
	
	%CHPyRelItrB = (mFloor(%tempCHPosY / $PTGm.ter_itrB_XY)) * $PTGm.ter_itrB_XY;
	%ChPxRelItrB = (mFloor(%ChPosX / $PTGm.ter_itrB_XY)) * $PTGm.ter_itrB_XY;
	%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.terOff_X,%CHPyRelItrB+$PTGm.terOff_Y,$PTGm.ter_itrB_XY,13333227,49979687,13313,15551);
	
	for(%BrPosX = %BrXYhSize; %BrPosX < %ChSize; %BrPosX += $PTGm.brTer_XYsize)
	{
		%ChPyRelItrC = (mFloor((%tempCHPosY+%tempBrPosY) / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrB_XY;
		%ChPxRelItrC = (mFloor((%ChPosX+%BrPosX) / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrB_XY;
		%ChHVItrC = PTG_RandNumGen_Chunk(%CHPxRelItrC+$PTGm.terOff_X,%CHPyRelItrC+$PTGm.terOff_Y,$PTGm.ter_itrB_XY,13333227,49979687,13313,15551);
		%BrPyActItrC = (mFloor(%tempBrPosY / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrC_XY;
		%BrPxActItrC = (mFloor(%BrPosX / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrC_XY;
	
		for(%BrPosXb = 0; %BrPosXb < %itrCSize; %BrPosXb += %BrXYSize)
		{
			%Co = ((%tempCHPosY-%ChPyActItrA)+%tempBrPosY) / $PTGm.ter_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

			%Co = ((%tempCHPosY-%CHPyRelItrB)+%tempBrPosY) / $PTGm.ter_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
			%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
			
			%Co = (%tempBrPosY-%BrPyActItrC) / $PTGm.ter_itrC_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrCColL = getWord(%ChHVItrC,0) - ((getWord(%ChHVItrC,0) - getWord(%ChHVItrC,1)) * %Sm);
			%ItrCColR = getWord(%ChHVItrC,2) - ((getWord(%ChHVItrC,2) - getWord(%ChHVItrC,3)) * %Sm);
			
			%Co = ((%ChPosX-%ChPxActItrA)+%BrPosX+%BrPosXb) / $PTGm.ter_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
			
			%Co = ((%ChPosX-%CHPxRelItrB)+%BrPosX+%BrPosXb) / $PTGm.ter_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);
			
			%Co = ((%BrPosX+%BrPosXb)-%BrPxActItrC) / $PTGm.ter_itrC_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrCRow = %ItrCColL - ((%ItrCColL - %ItrCColR) * %Sm);
			
			%relZ = (%ItrARow*$PTGm.ter_itrA_Z) + (%ItrBRow*$PTGm.ter_itrB_Z) + (%ItrCRow*$PTGm.ter_itrC_Z) + $PTGm.terHLevel;
			
			//Lakes / Connect Lakes option
			if($PTGm.enabCnctLakes && (%relZ - $PTGm.terHLevel) < ($PTGm.lakesHLevel+$PTGm.cnctLakesStrt) && $PTGm.terType !$= "SkyLands")
				%relZ -= (($PTGm.lakesHLevel+$PTGm.cnctLakesStrt) - %relZ);
			
			//Edge-FallOff
			%relZ = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX+%BrPosXb,%ChPosY+%BrPosY,%relZ);
			
			%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
			%BrH = getMax(%tempZ,%MinBrZSnap);
			$StrArrayHV_Terrain[%BrPosX+%BrPosXb,%BrPosY] = %BrH;
			
			if(!$PTGm.enabMntns)
				$StrArrayHV_Mountains[%BrPosX+%BrPosXb,%BrPosY] = %BrH;
		}
	}
	
	//////////////////////////////////////////////////
	
	if($PTGm.seamlessModTer)
	{
		for(%c = 0; %c < 4; %c++)
		{
			switch(%c)
			{
				case 0:
				
					%tempCHPosX = %ChPosX - %ChSize;
					%tempCHPosY = %ChPosY + %ChSize;
					%tempBrPosX = %ChSize - %BrXYhSize;
					%tempBrPosY = %BrXYhSize;
					
					%BrPosY = %ChSize + %BrXYhSize;
					%BrPosX = -1 * %BrXYhSize;
				
				case 1:
				
					%tempCHPosX = %ChPosX + %ChSize;
					%tempCHPosY = %ChPosY + %ChSize;
					%tempBrPosX = %BrXYhSize;
					%tempBrPosY = %BrXYhSize;
					
					%BrPosY = %ChSize + %BrXYhSize;
					%BrPosX = %ChSize + %BrXYhSize;
				
				case 2:
				
					%tempCHPosX = %ChPosX - %ChSize;
					%tempCHPosY = %ChPosY - %ChSize;
					%tempBrPosX = %ChSize - %BrXYhSize;
					%tempBrPosY = %ChSize - %BrXYhSize;
					
					%BrPosY = -1 * %BrXYhSize;
					%BrPosX = -1 * %BrXYhSize;
				
				case 3:
				
					%tempCHPosX = %ChPosX + %ChSize;
					%tempCHPosY = %ChPosY - %ChSize;
					%tempBrPosX = %BrXYhSize;
					%tempBrPosY = %ChSize - %BrXYhSize;
					
					%BrPosY = -1 * %BrXYhSize;
					%BrPosX = %ChSize + %BrXYhSize;
			}
			
			%ChPxRelItrA = (mFloor(%tempCHPosX / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrB_XY;
			%ChPyRelItrA = (mFloor(%tempCHPosY / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrB_XY;
			%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.terOff_X,%CHPyRelItrA+$PTGm.terOff_Y,$PTGm.ter_itrB_XY,13333227,49979687,13313,15551);
			%ChPxActItrA = (mFloor(%tempCHPosX / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrA_XY;
			%ChPyActItrA = (mFloor(%tempCHPosY / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrA_XY;
			
			%ChHVItrAmod = %ChHVItrA;
			if($PTGm.enabBio_CustA) //Custom Biome A
			{
				%bioCheckStr = PTG_Noise_Perlin_BiomeCheck(%tempCHPosX,%tempCHPosY,$PTGm.bio_CustA_itrA_XY,$PTGm.bio_CustA_itrA_Z,$PTGm.bio_CustAOff_X,$PTGm.bio_CustAOff_Y,$PTGm.Bio_CustA_YStart);
				%Section_Biome = $PTGm.bio_CustASecZ;
				%Hmod = $PTGbio.Bio_CustA_TerHMod;
				
				if((getWord(%bioCheckStr,0)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * %Hmod);
				if((getWord(%bioCheckStr,2)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * %Hmod);
				if((getWord(%bioCheckStr,1)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * %Hmod);
				if((getWord(%bioCheckStr,3)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * %Hmod);
			}
			if($PTGm.enabBio_CustB) //Custom Biome B
			{
				%bioCheckStr = PTG_Noise_Perlin_BiomeCheck(%tempCHPosX,%tempCHPosY,$PTGm.bio_CustB_itrA_XY,$PTGm.bio_CustB_itrA_Z,$PTGm.bio_CustBOff_X,$PTGm.bio_CustBOff_Y,$PTGm.Bio_CustB_YStart);
				%Section_Biome = $PTGm.bio_CustBSecZ;
				%Hmod = $PTGbio.Bio_CustB_TerHMod;
				
				if((getWord(%bioCheckStr,0)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * %Hmod);
				if((getWord(%bioCheckStr,2)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * %Hmod);
				if((getWord(%bioCheckStr,1)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * %Hmod);
				if((getWord(%bioCheckStr,3)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * %Hmod);
			}
			if($PTGm.enabBio_CustC) //Custom Biome C
			{
				%bioCheckStr = PTG_Noise_Perlin_BiomeCheck(%tempCHPosX,%tempCHPosY,$PTGm.bio_CustC_itrA_XY,$PTGm.bio_CustC_itrA_Z,$PTGm.bio_CustCOff_X,$PTGm.bio_CustCOff_Y,$PTGm.Bio_CustC_YStart);
				%Section_Biome = $PTGm.bio_CustCSecZ;
				%Hmod = $PTGbio.Bio_CustC_TerHMod;
				
				if((getWord(%bioCheckStr,0)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * %Hmod);
				if((getWord(%bioCheckStr,2)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * %Hmod);
				if((getWord(%bioCheckStr,1)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * %Hmod);
				if((getWord(%bioCheckStr,3)) < %Section_Biome)%ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * %Hmod);
			}
			%ChHVItrA = %ChHVItrAmod;
			
			
			%CHPyRelItrB = (mFloor(%tempCHPosY / $PTGm.ter_itrB_XY)) * $PTGm.ter_itrB_XY;
			%ChPxRelItrB = (mFloor(%tempCHPosX / $PTGm.ter_itrB_XY)) * $PTGm.ter_itrB_XY;
			%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.terOff_X,%CHPyRelItrB+$PTGm.terOff_Y,$PTGm.ter_itrB_XY,13333227,49979687,13313,15551);

			%ChPyRelItrC = (mFloor((%tempCHPosY+%tempBrPosY) / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrB_XY;
			%ChPxRelItrC = (mFloor((%tempCHPosX+%tempBrPosX) / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrB_XY;
			%ChHVItrC = PTG_RandNumGen_Chunk(%CHPxRelItrC+$PTGm.terOff_X,%CHPyRelItrC+$PTGm.terOff_Y,$PTGm.ter_itrB_XY,13333227,49979687,13313,15551);
			%BrPyActItrC = (mFloor(%tempBrPosY / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrC_XY;
			%BrPxActItrC = (mFloor(%tempBrPosX / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrC_XY;


			%Co = ((%tempCHPosY-%ChPyActItrA)+%tempBrPosY) / $PTGm.ter_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

			%Co = ((%tempCHPosY-%CHPyRelItrB)+%tempBrPosY) / $PTGm.ter_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
			%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
			
			%Co = (%tempBrPosY-%BrPyActItrC) / $PTGm.ter_itrC_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrCColL = getWord(%ChHVItrC,0) - ((getWord(%ChHVItrC,0) - getWord(%ChHVItrC,1)) * %Sm);
			%ItrCColR = getWord(%ChHVItrC,2) - ((getWord(%ChHVItrC,2) - getWord(%ChHVItrC,3)) * %Sm);
			
			%Co = ((%tempCHPosX-%ChPxActItrA)+%tempBrPosX) / $PTGm.ter_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
			
			%Co = ((%tempCHPosX-%CHPxRelItrB)+%tempBrPosX) / $PTGm.ter_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);
			
			%Co = (%tempBrPosX-%BrPxActItrC) / $PTGm.ter_itrC_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrCRow = %ItrCColL - ((%ItrCColL - %ItrCColR) * %Sm);
			
			%relZ = (%ItrARow*$PTGm.ter_itrA_Z) + (%ItrBRow*$PTGm.ter_itrB_Z) + (%ItrCRow*$PTGm.ter_itrC_Z) + $PTGm.terHLevel;
			
			//Lakes / Connect Lakes option
			if($PTGm.enabCnctLakes && (%relZ - $PTGm.terHLevel) < ($PTGm.lakesHLevel+$PTGm.cnctLakesStrt) && $PTGm.terType !$= "SkyLands")
				%relZ -= (($PTGm.lakesHLevel+$PTGm.cnctLakesStrt) - %relZ);
			
			//Edge-FallOff
			%relZ = PTG_Noise_EdgeFallOff(%tempCHPosX+%tempBrPosX,%tempCHPosY+%tempBrPosY,%relZ);
			
			%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
			%BrH = getMax(%tempZ,%MinBrZSnap);
			$StrArrayHV_Terrain[%BrPosX,%BrPosY] = %BrH;
			
			if(!$PTGm.enabMntns)
				$StrArrayHV_Mountains[%BrPosX,%BrPosY] = %BrH;
		}
	}


	$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_secFuncMS,0,20));
	scheduleNoQuota(%delay,0,PTG_Routine_Calc,%cl,"Mountains",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Noise_2ItrFractal_Clouds(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{
	%MinBrZSnap = $PTGm.brTer_Zsize; // $PTGm.brClouds_FillXYZSize + $PTGm.brClouds_Zsize; //?
	%BrXYhSize = $PTGm.brClouds_XYsize / 2;
	
	%ChPxRelItrA = (mFloor(%CHPosX / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrB_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrB_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.cloudsOff_X,%CHPyRelItrA+$PTGm.cloudsOff_Y,$PTGm.clouds_itrB_XY,27177289,753266429,14549,54163);
	%ChPxActItrA = (mFloor(%CHPosX / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrA_XY;
	
	%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPyActItrA SPC %ChPyActItrA+$PTGm.clouds_itrA_XY,$PTGm.Cloud_YStart,0);
	
	%CHPyRelItrB = (mFloor(%CHPosY / $PTGm.clouds_itrB_XY)) * $PTGm.clouds_itrB_XY;
	%ChPxRelItrB = (mFloor(%CHPosX / $PTGm.clouds_itrB_XY)) * $PTGm.clouds_itrB_XY;
	%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.cloudsOff_X,%CHPyRelItrB+$PTGm.cloudsOff_Y,$PTGm.clouds_itrB_XY,27177289,753266429,14549,54163);
	
	%ChHVItrB = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.clouds_itrB_XY,$PTGm.Cloud_YStart,0);
	
	for(%BrPosY = %BrXYhSize; %BrPosY < mClamp($PTGm.chSize,16,256); %BrPosY += $PTGm.brClouds_XYsize)
	{
		for(%BrPosX = %BrXYhSize; %BrPosX < mClamp($PTGm.chSize,16,256); %BrPosX += $PTGm.brClouds_XYsize)
		{			
			%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.clouds_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

			%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY) / $PTGm.clouds_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
			%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
			
			%Co = ((%CHPosX-%ChPxActItrA)+%BrPosX) / $PTGm.clouds_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) *%Sm);
			
			%Co = ((%CHPosX-%CHPxRelItrB)+%BrPosX) / $PTGm.clouds_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

			%relZ = (%ItrARow*$PTGm.clouds_itrA_Z) + (%ItrBRow*$PTGm.clouds_itrB_Z);
			
			//Edge-FallOff
			%relZ = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%relZ);
			
			$StrArrayHV_Clouds[%BrPosX,%BrPosY] = getMax(%relZ,%MinBrZSnap);
		}
	}

	$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_priFuncMS,0,100));
	scheduleNoQuota(%delay,0,PTG_Noise_2ItrFractal_Cloud_Edges,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Noise_2ItrFractal_Cloud_Edges(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{
	%MinBrZSnap = $PTGm.brTer_Zsize; // $PTGm.brClouds_FillXYZSize + $PTGm.brClouds_Zsize; //?
	%BrXYhSize = $PTGm.brClouds_XYsize / 2;
	%ChSize = mClamp($PTGm.chSize,16,256);

	//////////////////////////////////////////////////
	//Left Side

	%tempCHPosX = %ChPosX - mClamp($PTGm.chSize,16,256);
	%tempBrPosX = mClamp($PTGm.chSize,16,256) - %BrXYhSize;//$PTGm.brClouds_XYsize;
	%BrPosX = -1 * %BrXYhSize;//$PTGm.brClouds_XYsize;
	
	%ChPxRelItrA = (mFloor(%tempCHPosX / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrB_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrB_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.cloudsOff_X,%CHPyRelItrA+$PTGm.cloudsOff_Y,$PTGm.clouds_itrB_XY,27177289,753266429,14549,54163);
	%ChPxActItrA = (mFloor(%tempCHPosX / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrA_XY;
	
	%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPyActItrA SPC %ChPyActItrA+$PTGm.clouds_itrA_XY,$PTGm.Cloud_YStart,0);
	
	%CHPyRelItrB = (mFloor(%CHPosY / $PTGm.clouds_itrB_XY)) * $PTGm.clouds_itrB_XY;
	%ChPxRelItrB = (mFloor(%tempCHPosX / $PTGm.clouds_itrB_XY)) * $PTGm.clouds_itrB_XY;
	%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.cloudsOff_X,%CHPyRelItrB+$PTGm.cloudsOff_Y,$PTGm.clouds_itrB_XY,27177289,753266429,14549,54163);
	
	%ChHVItrB = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.clouds_itrB_XY,$PTGm.Cloud_YStart,0);
	
	for(%BrPosY = %BrXYhSize; %BrPosY < mClamp($PTGm.chSize,16,256); %BrPosY += $PTGm.brClouds_XYsize)
	{
		%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.clouds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

		%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY) / $PTGm.clouds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
		%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
		
		%Co = ((%tempCHPosX-%ChPxActItrA)+%tempBrPosX) / $PTGm.clouds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
		
		%Co = ((%tempCHPosX-%CHPxRelItrB)+%tempBrPosX) / $PTGm.clouds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

		%relZ = (%ItrARow*$PTGm.clouds_itrA_Z) + (%ItrBRow*$PTGm.clouds_itrB_Z);
		
		//Edge-FallOff
		%relZ = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%relZ);
			
		%tempZ = mFloor(%relZ / $PTGm.brClouds_Zsize) * $PTGm.brClouds_Zsize;
		$StrArrayHV_Clouds[%BrPosX,%BrPosY] = getMax(%tempZ,%MinBrZSnap);
	}
	
	//////////////////////////////////////////////////
	//Right Side

	%tempCHPosX = %ChPosX + mClamp($PTGm.chSize,16,256);
	%tempBrPosX = %BrXYhSize;//0;
	%BrPosX = mClamp($PTGm.chSize,16,256) + %BrXYhSize;
	
	%ChPxRelItrA = (mFloor(%tempCHPosX / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrB_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrB_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.cloudsOff_X,%CHPyRelItrA+$PTGm.cloudsOff_Y,$PTGm.clouds_itrB_XY,27177289,753266429,14549,54163);
	%ChPxActItrA = (mFloor(%tempCHPosX / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrA_XY;
	
	%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPyActItrA SPC %ChPyActItrA+$PTGm.clouds_itrA_XY,$PTGm.Cloud_YStart,0);
	
	%CHPyRelItrB = (mFloor(%CHPosY / $PTGm.clouds_itrB_XY)) * $PTGm.clouds_itrB_XY;
	%ChPxRelItrB = (mFloor(%tempCHPosX / $PTGm.clouds_itrB_XY)) * $PTGm.clouds_itrB_XY;
	%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.cloudsOff_X,%CHPyRelItrB+$PTGm.cloudsOff_Y,$PTGm.clouds_itrB_XY,27177289,753266429,14549,54163);
	
	%ChHVItrB = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.clouds_itrB_XY,$PTGm.Cloud_YStart,0);
	
	for(%BrPosY = %BrXYhSize; %BrPosY < mClamp($PTGm.chSize,16,256); %BrPosY += $PTGm.brClouds_XYsize)
	{
		%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.clouds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

		%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY) / $PTGm.clouds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
		%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
		
		%Co = ((%tempCHPosX-%ChPxActItrA)+%tempBrPosX) / $PTGm.clouds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
		
		%Co = ((%tempCHPosX-%CHPxRelItrB)+%tempBrPosX) / $PTGm.clouds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);
		
		%relZ = (%ItrARow*$PTGm.clouds_itrA_Z) + (%ItrBRow*$PTGm.clouds_itrB_Z);
		
		//Edge-FallOff
		%relZ = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%relZ);
		
		%tempZ = mFloor(%relZ / $PTGm.brClouds_Zsize) * $PTGm.brClouds_Zsize;
		$StrArrayHV_Clouds[%BrPosX,%BrPosY] = getMax(%tempZ,%MinBrZSnap);
	}
	
	//////////////////////////////////////////////////
	//Bottom Side

	%tempCHPosY = %ChPosY - mClamp($PTGm.chSize,16,256);
	%tempBrPosY = mClamp($PTGm.chSize,16,256) - %BrXYhSize;//$PTGm.brClouds_XYsize;
	%BrPosY = -1 * %BrXYhSize;//$PTGm.brClouds_XYsize;
	
	%ChPxRelItrA = (mFloor(%ChPosX / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrB_XY;
	%ChPyRelItrA = (mFloor(%tempCHPosY / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrB_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.cloudsOff_X,%CHPyRelItrA+$PTGm.cloudsOff_Y,$PTGm.clouds_itrB_XY,27177289,753266429,14549,54163);
	%ChPxActItrA = (mFloor(%ChPosX / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrA_XY;
	%ChPyActItrA = (mFloor(%tempCHPosY / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrA_XY;
	
	%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPyActItrA SPC %ChPyActItrA+$PTGm.clouds_itrA_XY,$PTGm.Cloud_YStart,0);
	
	%CHPyRelItrB = (mFloor(%tempCHPosY / $PTGm.clouds_itrB_XY)) * $PTGm.clouds_itrB_XY;
	%ChPxRelItrB = (mFloor(%ChPosX / $PTGm.clouds_itrB_XY)) * $PTGm.clouds_itrB_XY;
	%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.cloudsOff_X,%CHPyRelItrB+$PTGm.cloudsOff_Y,$PTGm.clouds_itrB_XY,27177289,753266429,14549,54163);
	
	%ChHVItrB = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.clouds_itrB_XY,$PTGm.Cloud_YStart,0);
	
	for(%BrPosX = %BrXYhSize; %BrPosX < mClamp($PTGm.chSize,16,256); %BrPosX += $PTGm.brClouds_XYsize)
	{	
		%Co = ((%tempCHPosY-%ChPyActItrA)+%tempBrPosY) / $PTGm.clouds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

		%Co = ((%tempCHPosY-%CHPyRelItrB)+%tempBrPosY) / $PTGm.clouds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
		%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
		
		%Co = ((%ChPosX-%ChPxActItrA)+%BrPosX) / $PTGm.clouds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
		
		%Co = ((%ChPosX-%CHPxRelItrB)+%BrPosX) / $PTGm.clouds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);
		
		%relZ = (%ItrARow*$PTGm.clouds_itrA_Z) + (%ItrBRow*$PTGm.clouds_itrB_Z);
		
		//Edge-FallOff
		%relZ = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%relZ);
		
		%tempZ = mFloor(%relZ / $PTGm.brClouds_Zsize) * $PTGm.brClouds_Zsize;
		$StrArrayHV_Clouds[%BrPosX,%BrPosY] = getMax(%tempZ,%MinBrZSnap);
	}
	
	//////////////////////////////////////////////////
	//Top Side

	%tempCHPosY = %ChPosY + mClamp($PTGm.chSize,16,256);
	%tempBrPosY = %BrXYhSize;//0;
	%BrPosY = mClamp($PTGm.chSize,16,256) + %BrXYhSize;//;
	
	%ChPxRelItrA = (mFloor(%ChPosX / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrB_XY;
	%ChPyRelItrA = (mFloor(%tempCHPosY / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrB_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.cloudsOff_X,%CHPyRelItrA+$PTGm.cloudsOff_Y,$PTGm.clouds_itrB_XY,27177289,753266429,14549,54163);
	%ChPxActItrA = (mFloor(%ChPosX / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrA_XY;
	%ChPyActItrA = (mFloor(%tempCHPosY / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrA_XY;
	
	%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPyActItrA SPC %ChPyActItrA+$PTGm.clouds_itrA_XY,$PTGm.Cloud_YStart,0);
	
	%CHPyRelItrB = (mFloor(%tempCHPosY / $PTGm.clouds_itrB_XY)) * $PTGm.clouds_itrB_XY;
	%ChPxRelItrB = (mFloor(%ChPosX / $PTGm.clouds_itrB_XY)) * $PTGm.clouds_itrB_XY;
	%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.cloudsOff_X,%CHPyRelItrB+$PTGm.cloudsOff_Y,$PTGm.clouds_itrB_XY,27177289,753266429,14549,54163);
	
	%ChHVItrB = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.clouds_itrB_XY,$PTGm.Cloud_YStart,0);
	
	for(%BrPosX = %BrXYhSize; %BrPosX < mClamp($PTGm.chSize,16,256); %BrPosX += $PTGm.brClouds_XYsize)
	{
		%Co = ((%tempCHPosY-%ChPyActItrA)+%tempBrPosY) / $PTGm.clouds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

		%Co = ((%tempCHPosY-%CHPyRelItrB)+%tempBrPosY) / $PTGm.clouds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
		%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
		
		%Co = ((%ChPosX-%ChPxActItrA)+%BrPosX) / $PTGm.clouds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
		
		%Co = ((%ChPosX-%CHPxRelItrB)+%BrPosX) / $PTGm.clouds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

		%relZ = (%ItrARow*$PTGm.clouds_itrA_Z) + (%ItrBRow*$PTGm.clouds_itrB_Z);
		
		//Edge-FallOff
		%relZ = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%relZ);
		
		%tempZ = mFloor(%relZ / $PTGm.brClouds_Zsize) * $PTGm.brClouds_Zsize;
		$StrArrayHV_Clouds[%BrPosX,%BrPosY] = getMax(%tempZ,%MinBrZSnap);
	}
	
	//////////////////////////////////////////////////
	
	if($PTGm.seamlessModTer)
	{
		for(%c = 0; %c < 4; %c++)
		{
			switch(%c)
			{
				case 0:
				
					%tempCHPosX = %ChPosX - %ChSize;
					%tempCHPosY = %ChPosY + %ChSize;
					%tempBrPosX = %ChSize - %BrXYhSize;
					%tempBrPosY = %BrXYhSize;
					
					%BrPosY = %ChSize + %BrXYhSize;
					%BrPosX = -1 * %BrXYhSize;
				
				case 1:
				
					%tempCHPosX = %ChPosX + %ChSize;
					%tempCHPosY = %ChPosY + %ChSize;
					%tempBrPosX = %BrXYhSize;
					%tempBrPosY = %BrXYhSize;
					
					%BrPosY = %ChSize + %BrXYhSize;
					%BrPosX = %ChSize + %BrXYhSize;
				
				case 2:
				
					%tempCHPosX = %ChPosX - %ChSize;
					%tempCHPosY = %ChPosY - %ChSize;
					%tempBrPosX = %ChSize - %BrXYhSize;
					%tempBrPosY = %ChSize - %BrXYhSize;
					
					%BrPosY = -1 * %BrXYhSize;
					%BrPosX = -1 * %BrXYhSize;
				
				case 3:
				
					%tempCHPosX = %ChPosX + %ChSize;
					%tempCHPosY = %ChPosY - %ChSize;
					%tempBrPosX = %BrXYhSize;
					%tempBrPosY = %ChSize - %BrXYhSize;
					
					%BrPosY = -1 * %BrXYhSize;
					%BrPosX = %ChSize + %BrXYhSize;
			}

			
			%ChPxRelItrA = (mFloor(%tempCHPosX / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrB_XY;
			%ChPyRelItrA = (mFloor(%tempCHPosY / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrB_XY;
			%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.cloudsOff_X,%CHPyRelItrA+$PTGm.cloudsOff_Y,$PTGm.clouds_itrB_XY,27177289,753266429,14549,54163);
			%ChPxActItrA = (mFloor(%tempCHPosX / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrA_XY;
			%ChPyActItrA = (mFloor(%tempCHPosY / $PTGm.clouds_itrA_XY)) * $PTGm.clouds_itrA_XY;
			
			%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPyActItrA SPC %ChPyActItrA+$PTGm.clouds_itrA_XY,$PTGm.Cloud_YStart,0);
			
			%CHPyRelItrB = (mFloor(%tempCHPosY / $PTGm.clouds_itrB_XY)) * $PTGm.clouds_itrB_XY;
			%ChPxRelItrB = (mFloor(%tempCHPosX / $PTGm.clouds_itrB_XY)) * $PTGm.clouds_itrB_XY;
			%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.cloudsOff_X,%CHPyRelItrB+$PTGm.cloudsOff_Y,$PTGm.clouds_itrB_XY,27177289,753266429,14549,54163);
			
			%ChHVItrB = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.clouds_itrB_XY,$PTGm.Cloud_YStart,0);
			
			
			%Co = ((%tempCHPosY-%ChPyActItrA)+%tempBrPosY) / $PTGm.clouds_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

			%Co = ((%tempCHPosY-%CHPyRelItrB)+%tempBrPosY) / $PTGm.clouds_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
			%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
			
			%Co = ((%tempCHPosX-%ChPxActItrA)+%tempBrPosX) / $PTGm.clouds_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
			
			%Co = ((%tempCHPosX-%CHPxRelItrB)+%tempBrPosX) / $PTGm.clouds_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

			%relZ = (%ItrARow*$PTGm.clouds_itrA_Z) + (%ItrBRow*$PTGm.clouds_itrB_Z);
			
			//Edge-FallOff
			%relZ = PTG_Noise_EdgeFallOff(%tempCHPosX+%tempBrPosX,%tempCHPosY+%tempBrPosY,%relZ);
			
			%tempZ = mFloor(%relZ / $PTGm.brClouds_Zsize) * $PTGm.brClouds_Zsize;
			$StrArrayHV_Clouds[%BrPosX,%BrPosY] = getMax(%tempZ,%MinBrZSnap);
		}
	}
	

	$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_secFuncMS,0,20));
	scheduleNoQuota(%delay,0,PTG_Routine_Calc,%cl,"Terrain",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Noise_3ItrFractal_Caves(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{
	%MinBrZSnap = $PTGm.brTer_Zsize; // $PTGm.brTer_FillXYZSize + $PTGm.brTer_Zsize; // might cause issue with caves?
	%BrXYSize = $PTGm.brTer_XYsize;
	%BrXYhSize = %BrXYSize / 2;
	%itrCSize = $PTGm.ter_itrC_XY;
	%ChSize = mClamp($PTGm.chSize,16,256);

	%ChPxRelItrA = (mFloor(%CHPosX / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrB_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrB_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.caveAOff_X,%CHPyRelItrA+$PTGm.caveAOff_Y, $PTGm.caveA_itrB_XY, 110000223, 53781811, 35801, 72727);
	%ChPxActItrA = (mFloor(%CHPosX / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrA_XY;
	
	%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPyActItrA SPC %ChPyActItrA+$PTGm.caveA_itrA_XY,$PTGm.Cave_YStart,99999);
	
	%CHPyRelItrB = (mFloor(%CHPosY / $PTGm.caveA_itrB_XY)) * $PTGm.caveA_itrB_XY;
	%ChPxRelItrB = (mFloor(%CHPosX / $PTGm.caveA_itrB_XY)) * $PTGm.caveA_itrB_XY;
	%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.caveAOff_X,%CHPyRelItrB+$PTGm.caveAOff_Y, $PTGm.caveA_itrB_XY, 110000223, 53781811, 35801, 72727);
	
	%ChHVItrB = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.caveA_itrB_XY,$PTGm.Cave_YStart,99999);
	
	for(%BrPosY = %BrXYhSize; %BrPosY < %ChSize; %BrPosY += %itrCSize)
	{
		for(%BrPosX = %BrXYhSize; %BrPosX < %ChSize; %BrPosX += %itrCSize)
		{			
			%ChPyRelItrC = (mFloor((%CHPosY + %BrPosY) / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrB_XY;
			%ChPxRelItrC = (mFloor((%CHPosX + %BrPosX) / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrB_XY;
			%ChHVItrC = PTG_RandNumGen_Chunk(%CHPxRelItrC+$PTGm.caveAOff_X,%CHPyRelItrC+$PTGm.caveAOff_Y, $PTGm.caveA_itrB_XY, 110000223, 53781811, 35801, 72727);
			%BrPyActItrC = (mFloor(%BrPosY / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrC_XY;
			%BrPxActItrC = (mFloor(%BrPosX / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrC_XY;
			
			for(%BrPosYb = 0; %BrPosYb < %itrCSize; %BrPosYb += %BrXYSize)
			{
				for(%BrPosXb = 0; %BrPosXb < %itrCSize; %BrPosXb += %BrXYSize)
				{
					%ChPyRelItrCtmp = (mFloor((%CHPosY + %BrPosY) / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrC_XY;
					%ChHVItrC = PTG_Noise_PsuedoEquatorCheck(%ChHVItrC,%ChPyRelItrCtmp SPC %ChPyRelItrCtmp+$PTGm.caveA_itrC_XY,$PTGm.Cave_YStart,99999);
				
					%Co = ((%CHPosY - %ChPyActItrA) + %BrPosY + %BrPosYb) / $PTGm.caveA_itrA_XY;
					%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
					%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
					%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

					%Co = ((%CHPosY - %CHPyRelItrB) + %BrPosY + %BrPosYb) / $PTGm.caveA_itrB_XY;
					%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
					%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
					%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
					
					%Co = ((%BrPosY + %BrPosYb) - %BrPyActItrC) / $PTGm.caveA_itrC_XY;
					%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
					%ItrCColL = getWord(%ChHVItrC,0) - ((getWord(%ChHVItrC,0) - getWord(%ChHVItrC,1)) * %Sm);
					%ItrCColR = getWord(%ChHVItrC,2) - ((getWord(%ChHVItrC,2) - getWord(%ChHVItrC,3)) * %Sm);
					
					%Co = ((%CHPosX - %ChPxActItrA) + %BrPosX + %BrPosXb) / $PTGm.caveA_itrA_XY;
					%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
					%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
					
					%Co = ((%CHPosX - %CHPxRelItrB) + %BrPosX + %BrPosXb) / $PTGm.caveA_itrB_XY;
					%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
					%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);
					
					%Co = ((%BrPosX + %BrPosXb) - %BrPxActItrC) / $PTGm.caveA_itrC_XY;
					%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
					%ItrCRow = %ItrCColL - ((%ItrCColL - %ItrCColR) * %Sm);
					
					%relZ = (%ItrARow * $PTGm.caveA_itrA_Z) + (%ItrBRow * $PTGm.caveA_itrB_Z) + (%ItrCRow * $PTGm.caveA_itrC_Z);
					%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
					$StrArrayHV_CavesA[%BrPosX+%BrPosXb,%BrPosY+%BrPosYb] = getMax(%tempZ,%MinBrZSnap);
				}
			}
		}
	}
	
	$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_priFuncMS,0,100));
	scheduleNoQuota(%delay,0,PTG_Noise_Perlin_Caves,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum); // go to perlin func
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Noise_Perlin_Caves(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{
	%MinBrZSnap = $PTGm.brTer_Zsize; // $PTGm.brTer_FillXYZSize + $PTGm.brTer_Zsize;
	%BrXYhSize = $PTGm.brTer_XYsize / 2;
	
	%ChPxRelItrA = (mFloor(%CHPosX / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.caveBOff_X,%CHPyRelItrA+$PTGm.caveBOff_Y,$PTGm.caveB_itrA_XY,15141163,77777377,17977,44741);
	%ChPxActItrA = (mFloor(%CHPosX / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;
	
	for(%BrPosY = %BrXYhSize; %BrPosY < mClamp($PTGm.chSize,16,256); %BrPosY += $PTGm.brTer_XYsize)
	{
		for(%BrPosX = %BrXYhSize; %BrPosX < mClamp($PTGm.chSize,16,256); %BrPosX += $PTGm.brTer_XYsize)
		{
			%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.caveB_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);
			
			%Co = ((%CHPosX-%ChPxActItrA)+%BrPosX) / $PTGm.caveB_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
			
			%relZ = (%ItrARow * $PTGm.caveB_itrA_Z);
			%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
			%CaveH_Mod = $StrArrayHV_CavesB[%BrPosX,%BrPosY] = getMax(%tempZ,%MinBrZSnap);
			//%CaveH_Mod = $StrArrayHV_CavesB[%BrPosX,%BrPosY] = getMax(%tempZ,%MinBrZSnap);
			
			if((%CaveH = $StrArrayHV_CavesA[%BrPosX,%BrPosY]) < $PTGm.cavesSecZ)
			{
				//%CaveH_Mod = $StrArrayHV_CavesB[%BrPosX,%BrPosY];
				$StrArrayHV_CavesA[%BrPosX,%BrPosY] = mFloor((($PTGm.cavesHLevel - ($PTGm.cavesSecZ - %CaveH)) + %CaveH_Mod) / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
				$StrArrayHV_CavesB[%BrPosX,%BrPosY] = mFloor(($PTGm.cavesHLevel + (($PTGm.cavesSecZ - %CaveH) * mClamp($PTGm.caveTopZMult,1,8)) + %CaveH_Mod) / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
			}
			else
			{
				$StrArrayHV_CavesA[%BrPosX,%BrPosY] = 0;
				$StrArrayHV_CavesB[%BrPosX,%BrPosY] = 0;
			}
		}
	}

	$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_priFuncMS,0,100));
	scheduleNoQuota(%delay,0,PTG_Noise_3ItrFractal_Cave_Edges,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Noise_3ItrFractal_Cave_Edges(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{
	%MinBrZSnap = $PTGm.brTer_Zsize; // $PTGm.brTer_FillXYZSize + $PTGm.brTer_Zsize; // might cause issue with caves?
	%BrXYSize = $PTGm.brTer_XYsize;
	%BrXYhSize = %BrXYSize / 2;
	%itrCSize = $PTGm.ter_itrC_XY;
	%ChSize = mClamp($PTGm.chSize,16,256);
	
	//////////////////////////////////////////////////
	//Left Side

	%tempCHPosX = %ChPosX - mClamp($PTGm.chSize,16,256);
	%tempBrPosX = mClamp($PTGm.chSize,16,256) - %BrXYhSize;//$PTGm.brTer_XYsize;
	%BrPosX = -1 * %BrXYhSize;//$PTGm.brTer_XYsize;
	
	%ChPxRelItrA = (mFloor(%tempCHPosX / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrB_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrB_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.caveAOff_X,%CHPyRelItrA+$PTGm.caveAOff_Y,$PTGm.caveA_itrB_XY, 110000223, 53781811, 35801, 72727);
	%ChPxActItrA = (mFloor(%tempCHPosX / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrA_XY;
	
	%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPyActItrA SPC %ChPyActItrA+$PTGm.caveA_itrA_XY,$PTGm.Cave_YStart,99999);
	
	%CHPyRelItrB = (mFloor(%CHPosY / $PTGm.caveA_itrB_XY)) * $PTGm.caveA_itrB_XY;
	%ChPxRelItrB = (mFloor(%tempCHPosX / $PTGm.caveA_itrB_XY)) * $PTGm.caveA_itrB_XY;
	%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.caveAOff_X,%CHPyRelItrB+$PTGm.caveAOff_Y,$PTGm.caveA_itrB_XY, 110000223, 53781811, 35801, 72727);
	
	%ChHVItrB = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.caveA_itrB_XY,$PTGm.Cave_YStart,99999);
	
	for(%BrPosY = %BrXYhSize; %BrPosY < %ChSize; %BrPosY += %itrCSize)
	{
		%ChPyRelItrC = (mFloor((%CHPosY+%BrPosY) / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrB_XY;
		%ChPxRelItrC = (mFloor((%tempCHPosX+%tempBrPosX) / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrB_XY;
		%ChHVItrC = PTG_RandNumGen_Chunk(%CHPxRelItrC+$PTGm.caveAOff_X,%CHPyRelItrC+$PTGm.caveAOff_Y,$PTGm.caveA_itrB_XY, 110000223, 53781811, 35801, 72727);
		%BrPyActItrC = (mFloor(%BrPosY / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrC_XY;
		%BrPxActItrC = (mFloor(%tempBrPosX / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrC_XY;

		%ChPyRelItrCtmp = (mFloor((%CHPosY+%BrPosY) / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrC_XY;
		%ChHVItrC = PTG_Noise_PsuedoEquatorCheck(%ChHVItrC,%ChPyRelItrCtmp SPC %ChPyRelItrCtmp+$PTGm.caveA_itrC_XY,$PTGm.Cave_YStart,99999);
			
		for(%BrPosYb = 0; %BrPosYb < %itrCSize; %BrPosYb += %BrXYSize)
		{
			%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY+%BrPosYb) / $PTGm.caveA_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

			%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY+%BrPosYb) / $PTGm.caveA_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
			%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
			
			%Co = ((%BrPosY+%BrPosYb)-%BrPyActItrC) / $PTGm.caveA_itrC_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrCColL = getWord(%ChHVItrC,0) - ((getWord(%ChHVItrC,0) - getWord(%ChHVItrC,1)) * %Sm);
			%ItrCColR = getWord(%ChHVItrC,2) - ((getWord(%ChHVItrC,2) - getWord(%ChHVItrC,3)) * %Sm);
			
			%Co = ((%tempCHPosX-%ChPxActItrA)+%tempBrPosX) / $PTGm.caveA_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
			
			%Co = ((%tempCHPosX-%CHPxRelItrB)+%tempBrPosX) / $PTGm.caveA_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);
			
			%Co = (%tempBrPosX-%BrPxActItrC) / $PTGm.caveA_itrC_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrCRow = %ItrCColL - ((%ItrCColL - %ItrCColR) * %Sm);
			
			%relZ = (%ItrARow*$PTGm.caveA_itrA_Z) + (%ItrBRow*$PTGm.caveA_itrB_Z) + (%ItrCRow*$PTGm.caveA_itrC_Z);
			%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
			$StrArrayHV_CavesA[%BrPosX,%BrPosY+%BrPosYb] = getMax(%tempZ,%MinBrZSnap);
		}
	}
	
	//////////////////////////////////////////////////
	//Right Side

	%tempCHPosX = %ChPosX + mClamp($PTGm.chSize,16,256);
	%tempBrPosX = %BrXYhSize;//0;
	%BrPosX = mClamp($PTGm.chSize,16,256) + %BrXYhSize;
	
	%ChPxRelItrA = (mFloor(%tempCHPosX / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrB_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrB_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.caveAOff_X,%CHPyRelItrA+$PTGm.caveAOff_Y,$PTGm.caveA_itrB_XY, 110000223, 53781811, 35801, 72727);
	%ChPxActItrA = (mFloor(%tempCHPosX / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrA_XY;
	
	%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPyActItrA SPC %ChPyActItrA+$PTGm.caveA_itrA_XY,$PTGm.Cave_YStart,99999);
	
	%CHPyRelItrB = (mFloor(%CHPosY / $PTGm.caveA_itrB_XY)) * $PTGm.caveA_itrB_XY;
	%ChPxRelItrB = (mFloor(%tempCHPosX / $PTGm.caveA_itrB_XY)) * $PTGm.caveA_itrB_XY;
	%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.caveAOff_X,%CHPyRelItrB+$PTGm.caveAOff_Y,$PTGm.caveA_itrB_XY, 110000223, 53781811, 35801, 72727);
	
	%ChHVItrB = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.caveA_itrB_XY,$PTGm.Cave_YStart,99999);
	
	for(%BrPosY = %BrXYhSize; %BrPosY < %ChSize; %BrPosY += %itrCSize)
	{
		%ChPyRelItrC = (mFloor((%CHPosY+%BrPosY) / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrB_XY;
		%ChPxRelItrC = (mFloor((%tempCHPosX+%tempBrPosX) / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrB_XY;
		%ChHVItrC = PTG_RandNumGen_Chunk(%CHPxRelItrC+$PTGm.caveAOff_X,%CHPyRelItrC+$PTGm.caveAOff_Y,$PTGm.caveA_itrB_XY, 110000223, 53781811, 35801, 72727);
		%BrPyActItrC = (mFloor(%BrPosY / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrC_XY;
		%BrPxActItrC = (mFloor(%tempBrPosX / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrC_XY;
		
		%ChPyRelItrCtmp = (mFloor((%CHPosY+%BrPosY) / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrC_XY;
		%ChHVItrC = PTG_Noise_PsuedoEquatorCheck(%ChHVItrC,%ChPyRelItrCtmp SPC %ChPyRelItrCtmp+$PTGm.caveA_itrC_XY,$PTGm.Cave_YStart,99999);
			
		for(%BrPosYb = 0; %BrPosYb < %itrCSize; %BrPosYb += %BrXYSize)
		{
			%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY+%BrPosYb) / $PTGm.caveA_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

			%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY+%BrPosYb) / $PTGm.caveA_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
			%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
			
			%Co = ((%BrPosY+%BrPosYb)-%BrPyActItrC) / $PTGm.caveA_itrC_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrCColL = getWord(%ChHVItrC,0) - ((getWord(%ChHVItrC,0) - getWord(%ChHVItrC,1)) * %Sm);
			%ItrCColR = getWord(%ChHVItrC,2) - ((getWord(%ChHVItrC,2) - getWord(%ChHVItrC,3)) * %Sm);
			
			%Co = ((%tempCHPosX-%ChPxActItrA)+%tempBrPosX) / $PTGm.caveA_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
			
			%Co = ((%tempCHPosX-%CHPxRelItrB)+%tempBrPosX) / $PTGm.caveA_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);
			
			%Co = (%tempBrPosX-%BrPxActItrC) / $PTGm.caveA_itrC_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrCRow = %ItrCColL - ((%ItrCColL - %ItrCColR) * %Sm);
			
			%relZ = (%ItrARow*$PTGm.caveA_itrA_Z) + (%ItrBRow*$PTGm.caveA_itrB_Z) + (%ItrCRow*$PTGm.caveA_itrC_Z);
			%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
			$StrArrayHV_CavesA[%BrPosX,%BrPosY+%BrPosYb] = getMax(%tempZ,%MinBrZSnap);
		}
	}
	
	//////////////////////////////////////////////////
	//Bottom Side

	%tempCHPosY = %ChPosY - %ChSize;
	%tempBrPosY = %ChSize - %BrXYhSize;//$PTGm.brTer_XYsize;
	%BrPosY = -1 * %BrXYhSize;//$PTGm.brTer_XYsize;
	
	%ChPxRelItrA = (mFloor(%ChPosX / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrB_XY;
	%ChPyRelItrA = (mFloor(%tempCHPosY / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrB_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.caveAOff_X,%CHPyRelItrA+$PTGm.caveAOff_Y,$PTGm.caveA_itrB_XY, 110000223, 53781811, 35801, 72727);
	%ChPxActItrA = (mFloor(%ChPosX / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrA_XY;
	%ChPyActItrA = (mFloor(%tempCHPosY / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrA_XY;
	
	%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPyActItrA SPC %ChPyActItrA+$PTGm.caveA_itrA_XY,$PTGm.Cave_YStart,99999);
	
	%CHPyRelItrB = (mFloor(%tempCHPosY / $PTGm.caveA_itrB_XY)) * $PTGm.caveA_itrB_XY;
	%ChPxRelItrB = (mFloor(%ChPosX / $PTGm.caveA_itrB_XY)) * $PTGm.caveA_itrB_XY;
	%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.caveAOff_X,%CHPyRelItrB+$PTGm.caveAOff_Y,$PTGm.caveA_itrB_XY, 110000223, 53781811, 35801, 72727);
	
	%ChHVItrB = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.caveA_itrB_XY,$PTGm.Cave_YStart,99999);
	
	for(%BrPosX = %BrXYhSize; %BrPosX < %ChSize; %BrPosX += %itrCSize)
	{
		%ChPyRelItrC = (mFloor((%tempCHPosY+%tempBrPosY) / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrB_XY;
		%ChPxRelItrC = (mFloor((%ChPosX+%BrPosX) / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrB_XY;
		%ChHVItrC = PTG_RandNumGen_Chunk(%CHPxRelItrC+$PTGm.caveAOff_X,%CHPyRelItrC+$PTGm.caveAOff_Y,$PTGm.caveA_itrB_XY, 110000223, 53781811, 35801, 72727);
		%BrPyActItrC = (mFloor(%tempBrPosY / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrC_XY;
		%BrPxActItrC = (mFloor(%BrPosX / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrC_XY;

		%ChHVItrC = PTG_Noise_PsuedoEquatorCheck(%ChHVItrC,%ChPyRelItrC SPC %ChPyRelItrC+$PTGm.caveA_itrC_XY,$PTGm.Cave_YStart,99999);
		
		for(%BrPosXb = 0; %BrPosXb < %itrCSize; %BrPosXb += %BrXYSize)
		{
			%Co = ((%tempCHPosY-%ChPyActItrA)+%tempBrPosY) / $PTGm.caveA_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

			%Co = ((%tempCHPosY-%CHPyRelItrB)+%tempBrPosY) / $PTGm.caveA_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
			%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
			
			%Co = (%tempBrPosY-%BrPyActItrC) / $PTGm.caveA_itrC_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrCColL = getWord(%ChHVItrC,0) - ((getWord(%ChHVItrC,0) - getWord(%ChHVItrC,1)) * %Sm);
			%ItrCColR = getWord(%ChHVItrC,2) - ((getWord(%ChHVItrC,2) - getWord(%ChHVItrC,3)) * %Sm);
			
			%Co = ((%ChPosX-%ChPxActItrA)+%BrPosX+%BrPosXb) / $PTGm.caveA_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
			
			%Co = ((%ChPosX-%CHPxRelItrB)+%BrPosX+%BrPosXb) / $PTGm.caveA_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);
			
			%Co = ((%BrPosX+%BrPosXb)-%BrPxActItrC) / $PTGm.caveA_itrC_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrCRow = %ItrCColL - ((%ItrCColL - %ItrCColR) * %Sm);
				
			%relZ = (%ItrARow*$PTGm.caveA_itrA_Z) + (%ItrBRow*$PTGm.caveA_itrB_Z) + (%ItrCRow*$PTGm.caveA_itrC_Z);
			%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
			$StrArrayHV_CavesA[%BrPosX+%BrPosXb,%BrPosY] = getMax(%tempZ,%MinBrZSnap);
		}
	}
	
	//////////////////////////////////////////////////
	//Top Side

	%tempCHPosY = %ChPosY + %ChSize;
	%tempBrPosY = %BrXYhSize;//0;
	%BrPosY = %ChSize + %BrXYhSize;
	
	%ChPxRelItrA = (mFloor(%ChPosX / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrB_XY;
	%ChPyRelItrA = (mFloor(%tempCHPosY / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrB_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.caveAOff_X,%CHPyRelItrA+$PTGm.caveAOff_Y,$PTGm.caveA_itrB_XY, 110000223, 53781811, 35801, 72727);
	%ChPxActItrA = (mFloor(%ChPosX / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrA_XY;
	%ChPyActItrA = (mFloor(%tempCHPosY / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrA_XY;
	
	%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPyActItrA SPC %ChPyActItrA+$PTGm.caveA_itrA_XY,$PTGm.Cave_YStart,99999);
	
	%CHPyRelItrB = (mFloor(%tempCHPosY / $PTGm.caveA_itrB_XY)) * $PTGm.caveA_itrB_XY;
	%ChPxRelItrB = (mFloor(%ChPosX / $PTGm.caveA_itrB_XY)) * $PTGm.caveA_itrB_XY;
	%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.caveAOff_X,%CHPyRelItrB+$PTGm.caveAOff_Y,$PTGm.caveA_itrB_XY, 110000223, 53781811, 35801, 72727);
	
	%ChHVItrB = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.caveA_itrB_XY,$PTGm.Cave_YStart,99999);
	
	for(%BrPosX = %BrXYhSize; %BrPosX < %ChSize; %BrPosX += %itrCSize)
	{
		%ChPyRelItrC = (mFloor((%tempCHPosY+%tempBrPosY) / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrB_XY;
		%ChPxRelItrC = (mFloor((%ChPosX+%BrPosX) / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrB_XY;
		%ChHVItrC = PTG_RandNumGen_Chunk(%CHPxRelItrC+$PTGm.caveAOff_X,%CHPyRelItrC+$PTGm.caveAOff_Y,$PTGm.caveA_itrB_XY, 110000223, 53781811, 35801, 72727);
		%BrPyActItrC = (mFloor(%tempBrPosY / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrC_XY;
		%BrPxActItrC = (mFloor(%BrPosX / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrC_XY;

		%ChHVItrC = PTG_Noise_PsuedoEquatorCheck(%ChHVItrC,%ChPyRelItrC SPC %ChPyRelItrC+$PTGm.caveA_itrC_XY,$PTGm.Cave_YStart,99999);

		for(%BrPosXb = 0; %BrPosXb < %itrCSize; %BrPosXb += %BrXYSize)
		{
			%Co = ((%tempCHPosY-%ChPyActItrA)+%tempBrPosY) / $PTGm.caveA_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

			%Co = ((%tempCHPosY-%CHPyRelItrB)+%tempBrPosY) / $PTGm.caveA_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
			%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
			
			%Co = (%tempBrPosY-%BrPyActItrC) / $PTGm.caveA_itrC_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrCColL = getWord(%ChHVItrC,0) - ((getWord(%ChHVItrC,0) - getWord(%ChHVItrC,1)) * %Sm);
			%ItrCColR = getWord(%ChHVItrC,2) - ((getWord(%ChHVItrC,2) - getWord(%ChHVItrC,3)) * %Sm);

			%Co = ((%ChPosX-%ChPxActItrA)+%BrPosX+%BrPosXb) / $PTGm.caveA_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
			
			%Co = ((%ChPosX-%CHPxRelItrB)+%BrPosX+%BrPosXb) / $PTGm.caveA_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);
			
			%Co = ((%BrPosX+%BrPosXb)-%BrPxActItrC) / $PTGm.caveA_itrC_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrCRow = %ItrCColL - ((%ItrCColL - %ItrCColR) * %Sm);

			%relZ = (%ItrARow*$PTGm.caveA_itrA_Z) + (%ItrBRow*$PTGm.caveA_itrB_Z) + (%ItrCRow*$PTGm.caveA_itrC_Z);
			%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
			$StrArrayHV_CavesA[%BrPosX+%BrPosXb,%BrPosY] = getMax(%tempZ,%MinBrZSnap);
		}
	}
	
	//////////////////////////////////////////////////

	if($PTGm.seamlessModTer)
	{
		for(%c = 0; %c < 4; %c++)
		{
			switch(%c)
			{
				case 0:
				
					%tempCHPosX = %ChPosX - %ChSize;
					%tempCHPosY = %ChPosY + %ChSize;
					%tempBrPosX = %ChSize - %BrXYhSize;
					%tempBrPosY = %BrXYhSize;
					
					%BrPosY = %ChSize + %BrXYhSize;
					%BrPosX = -1 * %BrXYhSize;
				
				case 1:
				
					%tempCHPosX = %ChPosX + %ChSize;
					%tempCHPosY = %ChPosY + %ChSize;
					%tempBrPosX = %BrXYhSize;
					%tempBrPosY = %BrXYhSize;
					
					%BrPosY = %ChSize + %BrXYhSize;
					%BrPosX = %ChSize + %BrXYhSize;
				
				case 2:
				
					%tempCHPosX = %ChPosX - %ChSize;
					%tempCHPosY = %ChPosY - %ChSize;
					%tempBrPosX = %ChSize - %BrXYhSize;
					%tempBrPosY = %ChSize - %BrXYhSize;
					
					%BrPosY = -1 * %BrXYhSize;
					%BrPosX = -1 * %BrXYhSize;
				
				case 3:
				
					%tempCHPosX = %ChPosX + %ChSize;
					%tempCHPosY = %ChPosY - %ChSize;
					%tempBrPosX = %BrXYhSize;
					%tempBrPosY = %ChSize - %BrXYhSize;
					
					%BrPosY = -1 * %BrXYhSize;
					%BrPosX = %ChSize + %BrXYhSize;
			}

			
			%ChPxRelItrA = (mFloor(%tempCHPosX / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrB_XY;
			%ChPyRelItrA = (mFloor(%tempCHPosY / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrB_XY;
			%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.caveAOff_X,%CHPyRelItrA+$PTGm.caveAOff_Y,$PTGm.caveA_itrB_XY, 110000223, 53781811, 35801, 72727);
			%ChPxActItrA = (mFloor(%tempCHPosX / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrA_XY;
			%ChPyActItrA = (mFloor(%tempCHPosY / $PTGm.caveA_itrA_XY)) * $PTGm.caveA_itrA_XY;
			
			%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPyActItrA SPC %ChPyActItrA+$PTGm.caveA_itrA_XY,$PTGm.Cave_YStart,99999);
			
			%CHPyRelItrB = (mFloor(%tempCHPosY / $PTGm.caveA_itrB_XY)) * $PTGm.caveA_itrB_XY;
			%ChPxRelItrB = (mFloor(%tempCHPosX / $PTGm.caveA_itrB_XY)) * $PTGm.caveA_itrB_XY;
			%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.caveAOff_X,%CHPyRelItrB+$PTGm.caveAOff_Y,$PTGm.caveA_itrB_XY, 110000223, 53781811, 35801, 72727);
			
			%ChHVItrB = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.caveA_itrB_XY,$PTGm.Cave_YStart,99999);
		
			%ChPyRelItrC = (mFloor((%tempCHPosY+%tempBrPosY) / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrB_XY;
			%ChPxRelItrC = (mFloor((%tempCHPosX+%tempBrPosX) / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrB_XY;
			%ChHVItrC = PTG_RandNumGen_Chunk(%CHPxRelItrC+$PTGm.caveAOff_X,%CHPyRelItrC+$PTGm.caveAOff_Y,$PTGm.caveA_itrB_XY, 110000223, 53781811, 35801, 72727);
			%BrPyActItrC = (mFloor(%tempBrPosY / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrC_XY;
			%BrPxActItrC = (mFloor(%tempBrPosX / $PTGm.caveA_itrC_XY)) * $PTGm.caveA_itrC_XY;
			
			%ChHVItrC = PTG_Noise_PsuedoEquatorCheck(%ChHVItrC,%ChPyRelItrC SPC %ChPyRelItrC+$PTGm.caveA_itrC_XY,$PTGm.Cave_YStart,99999);

			%Co = ((%tempCHPosY-%ChPyActItrA)+%tempBrPosY) / $PTGm.caveA_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

			%Co = ((%tempCHPosY-%CHPyRelItrB)+%tempBrPosY) / $PTGm.caveA_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
			%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
			
			%Co = (%tempBrPosY-%BrPyActItrC) / $PTGm.caveA_itrC_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrCColL = getWord(%ChHVItrC,0) - ((getWord(%ChHVItrC,0) - getWord(%ChHVItrC,1)) * %Sm);
			%ItrCColR = getWord(%ChHVItrC,2) - ((getWord(%ChHVItrC,2) - getWord(%ChHVItrC,3)) * %Sm);

			%Co = ((%tempCHPosX-%ChPxActItrA)+%tempBrPosX) / $PTGm.caveA_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
			
			%Co = ((%tempCHPosX-%CHPxRelItrB)+%tempBrPosX) / $PTGm.caveA_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);
			
			%Co = (%tempBrPosX-%BrPxActItrC) / $PTGm.caveA_itrC_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrCRow = %ItrCColL - ((%ItrCColL - %ItrCColR) * %Sm);

			%relZ = (%ItrARow*$PTGm.caveA_itrA_Z) + (%ItrBRow*$PTGm.caveA_itrB_Z) + (%ItrCRow*$PTGm.caveA_itrC_Z);
			%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
			$StrArrayHV_CavesA[%BrPosX,%BrPosY] = getMax(%tempZ,%MinBrZSnap);
		}
	}
	
	
	$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_priFuncMS,0,100));
	scheduleNoQuota(%delay,0,PTG_Noise_Perlin_Cave_Edges,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum); // go to perlin func
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Noise_Perlin_Cave_Edges(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{
	%MinBrZSnap = $PTGm.brTer_Zsize; // $PTGm.brTer_FillXYZSize + $PTGm.brTer_Zsize;
	%BrXYhSize = $PTGm.brTer_XYsize / 2;
	%ChSize = mClamp($PTGm.chSize,16,256);
	
	//////////////////////////////////////////////////
	//Left Side

	%tempCHPosX = %ChPosX - mClamp($PTGm.chSize,16,256);
	%tempBrPosX = mClamp($PTGm.chSize,16,256) - %BrXYhSize;//$PTGm.brTer_XYsize;
	%BrPosX = -1 * %BrXYhSize;//$PTGm.brTer_XYsize;
	
	%ChPxRelItrA = (mFloor(%tempCHPosX / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.caveBOff_X,%CHPyRelItrA+$PTGm.caveBOff_Y,$PTGm.caveB_itrA_XY,15141163,77777377,17977,44741);
	%ChPxActItrA = (mFloor(%tempCHPosX / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;

	for(%BrPosY = %BrXYhSize; %BrPosY < mClamp($PTGm.chSize,16,256); %BrPosY += $PTGm.brTer_XYsize)
	{
		%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.caveB_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);		
		
		%Co = ((%tempCHPosX-%ChPxActItrA)+%tempBrPosX) / $PTGm.caveB_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);		
		
		%relZ = (%ItrARow*$PTGm.caveB_itrA_Z);
		%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
		$StrArrayHV_CavesB[%BrPosX,%BrPosY] = getMax(%tempZ,%MinBrZSnap);
			
		if((%CaveH = $StrArrayHV_CavesA[%BrPosX,%BrPosY]) < $PTGm.cavesSecZ)
		{
			%CaveH_Mod = $StrArrayHV_CavesB[%BrPosX,%BrPosY];
			$StrArrayHV_CavesA[%BrPosX,%BrPosY] = mFloor((($PTGm.cavesHLevel - ($PTGm.cavesSecZ - %CaveH)) + %CaveH_Mod) / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
			$StrArrayHV_CavesB[%BrPosX,%BrPosY] = mFloor(($PTGm.cavesHLevel + (($PTGm.cavesSecZ - %CaveH) * mClamp($PTGm.caveTopZMult,1,8)) + %CaveH_Mod) / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
		}
		else
		{
			$StrArrayHV_CavesA[%BrPosX,%BrPosY] = 0;
			$StrArrayHV_CavesB[%BrPosX,%BrPosY] = 0;
		}
	}
	
	//////////////////////////////////////////////////
	//Right Side

	%tempCHPosX = %ChPosX + mClamp($PTGm.chSize,16,256);
	%tempBrPosX = %BrXYhSize;//0;
	%BrPosX = mClamp($PTGm.chSize,16,256) + %BrXYhSize;
	
	%ChPxRelItrA = (mFloor(%tempCHPosX / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.caveBOff_X,%CHPyRelItrA+$PTGm.caveBOff_Y,$PTGm.caveB_itrA_XY,15141163,77777377,17977,44741);
	%ChPxActItrA = (mFloor(%tempCHPosX / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;
	
	for(%BrPosY = %BrXYhSize; %BrPosY < mClamp($PTGm.chSize,16,256); %BrPosY += $PTGm.brTer_XYsize)
	{
		%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.caveB_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);		
		
		%Co = ((%tempCHPosX-%ChPxActItrA)+%tempBrPosX) / $PTGm.caveB_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);

		%relZ = (%ItrARow*$PTGm.caveB_itrA_Z);
		%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
		$StrArrayHV_CavesB[%BrPosX,%BrPosY] = getMax(%tempZ,%MinBrZSnap);
			
		if((%CaveH = $StrArrayHV_CavesA[%BrPosX,%BrPosY]) < $PTGm.cavesSecZ)
		{
			%CaveH_Mod = $StrArrayHV_CavesB[%BrPosX,%BrPosY];
			$StrArrayHV_CavesA[%BrPosX,%BrPosY] = mFloor((($PTGm.cavesHLevel - ($PTGm.cavesSecZ - %CaveH)) + %CaveH_Mod) / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
			$StrArrayHV_CavesB[%BrPosX,%BrPosY] = mFloor(($PTGm.cavesHLevel + (($PTGm.cavesSecZ - %CaveH) * mClamp($PTGm.caveTopZMult,1,8)) + %CaveH_Mod) / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
		}
		else
		{
			$StrArrayHV_CavesA[%BrPosX,%BrPosY] = 0;
			$StrArrayHV_CavesB[%BrPosX,%BrPosY] = 0;
		}
	}
	
	//////////////////////////////////////////////////
	//Bottom Side

	%tempCHPosY = %ChPosY - mClamp($PTGm.chSize,16,256);
	%tempBrPosY = mClamp($PTGm.chSize,16,256) - %BrXYhSize;//$PTGm.brTer_XYsize;
	%BrPosY = -1 * %BrXYhSize;//$PTGm.brTer_XYsize;
	
	%ChPxRelItrA = (mFloor(%ChPosX / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;
	%ChPyRelItrA = (mFloor(%tempCHPosY / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.caveBOff_X,%CHPyRelItrA+$PTGm.caveBOff_Y,$PTGm.caveB_itrA_XY,15141163,77777377,17977,44741);
	%ChPxActItrA = (mFloor(%ChPosX / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;
	%ChPyActItrA = (mFloor(%tempCHPosY / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;
	
	for(%BrPosX = %BrXYhSize; %BrPosX < mClamp($PTGm.chSize,16,256); %BrPosX += $PTGm.brTer_XYsize)
	{
		%Co = ((%tempCHPosY-%ChPyActItrA)+%tempBrPosY) / $PTGm.caveB_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

		%Co = ((%ChPosX-%ChPxActItrA)+%BrPosX) / $PTGm.caveB_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
		
		%relZ = (%ItrARow*$PTGm.caveB_itrA_Z);
		%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
		$StrArrayHV_CavesB[%BrPosX,%BrPosY] = getMax(%tempZ,%MinBrZSnap);
			
		if((%CaveH = $StrArrayHV_CavesA[%BrPosX,%BrPosY]) < $PTGm.cavesSecZ)
		{
			%CaveH_Mod = $StrArrayHV_CavesB[%BrPosX,%BrPosY];
			$StrArrayHV_CavesA[%BrPosX,%BrPosY] = mFloor((($PTGm.cavesHLevel - ($PTGm.cavesSecZ - %CaveH)) + %CaveH_Mod) / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
			$StrArrayHV_CavesB[%BrPosX,%BrPosY] = mFloor(($PTGm.cavesHLevel + (($PTGm.cavesSecZ - %CaveH) * mClamp($PTGm.caveTopZMult,1,8)) + %CaveH_Mod) / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
		}
		else
		{
			$StrArrayHV_CavesA[%BrPosX,%BrPosY] = 0;
			$StrArrayHV_CavesB[%BrPosX,%BrPosY] = 0;
		}
	}
	
	//////////////////////////////////////////////////
	//Top Side

	%tempCHPosY = %ChPosY + mClamp($PTGm.chSize,16,256);
	%tempBrPosY = %BrXYhSize;//0;
	%BrPosY = mClamp($PTGm.chSize,16,256) + %BrXYhSize;//;
	
	%ChPxRelItrA = (mFloor(%ChPosX / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;
	%ChPyRelItrA = (mFloor(%tempCHPosY / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.caveBOff_X,%CHPyRelItrA+$PTGm.caveBOff_Y,$PTGm.caveB_itrA_XY,15141163,77777377,17977,44741);
	%ChPxActItrA = (mFloor(%ChPosX / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;
	%ChPyActItrA = (mFloor(%tempCHPosY / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;

	for(%BrPosX = %BrXYhSize; %BrPosX < mClamp($PTGm.chSize,16,256); %BrPosX += $PTGm.brTer_XYsize)
	{
		%Co = ((%tempCHPosY-%ChPyActItrA)+%tempBrPosY) / $PTGm.caveB_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

		%Co = ((%ChPosX-%ChPxActItrA)+%BrPosX) / $PTGm.caveB_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
		
		%relZ = (%ItrARow*$PTGm.caveB_itrA_Z);
		%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
		$StrArrayHV_CavesB[%BrPosX,%BrPosY] = getMax(%tempZ,%MinBrZSnap);
			
		if((%CaveH = $StrArrayHV_CavesA[%BrPosX,%BrPosY]) < $PTGm.cavesSecZ)
		{
			%CaveH_Mod = $StrArrayHV_CavesB[%BrPosX,%BrPosY];
			$StrArrayHV_CavesA[%BrPosX,%BrPosY] = mFloor((($PTGm.cavesHLevel - ($PTGm.cavesSecZ - %CaveH)) + %CaveH_Mod) / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
			$StrArrayHV_CavesB[%BrPosX,%BrPosY] = mFloor(($PTGm.cavesHLevel + (($PTGm.cavesSecZ - %CaveH) * mClamp($PTGm.caveTopZMult,1,8)) + %CaveH_Mod) / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
		}
		else
		{
			$StrArrayHV_CavesA[%BrPosX,%BrPosY] = 0;
			$StrArrayHV_CavesB[%BrPosX,%BrPosY] = 0;
		}
	}
	
	//////////////////////////////////////////////////
	
	if($PTGm.seamlessModTer)
	{
		for(%c = 0; %c < 4; %c++)
		{
			switch(%c)
			{
				case 0:
				
					%tempCHPosX = %ChPosX - %ChSize;
					%tempCHPosY = %ChPosY + %ChSize;
					%tempBrPosX = %ChSize - %BrXYhSize;
					%tempBrPosY = %BrXYhSize;
					
					%BrPosY = %ChSize + %BrXYhSize;
					%BrPosX = -1 * %BrXYhSize;
				
				case 1:
				
					%tempCHPosX = %ChPosX + %ChSize;
					%tempCHPosY = %ChPosY + %ChSize;
					%tempBrPosX = %BrXYhSize;
					%tempBrPosY = %BrXYhSize;
					
					%BrPosY = %ChSize + %BrXYhSize;
					%BrPosX = %ChSize + %BrXYhSize;
				
				case 2:
				
					%tempCHPosX = %ChPosX - %ChSize;
					%tempCHPosY = %ChPosY - %ChSize;
					%tempBrPosX = %ChSize - %BrXYhSize;
					%tempBrPosY = %ChSize - %BrXYhSize;
					
					%BrPosY = -1 * %BrXYhSize;
					%BrPosX = -1 * %BrXYhSize;
				
				case 3:
				
					%tempCHPosX = %ChPosX + %ChSize;
					%tempCHPosY = %ChPosY - %ChSize;
					%tempBrPosX = %BrXYhSize;
					%tempBrPosY = %ChSize - %BrXYhSize;
					
					%BrPosY = -1 * %BrXYhSize;
					%BrPosX = %ChSize + %BrXYhSize;
			}

			
			%ChPxRelItrA = (mFloor(%tempCHPosX / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;
			%ChPyRelItrA = (mFloor(%tempCHPosY / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;
			%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.caveBOff_X,%CHPyRelItrA+$PTGm.caveBOff_Y,$PTGm.caveB_itrA_XY,15141163,77777377,17977,44741);
			%ChPxActItrA = (mFloor(%tempCHPosX / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;
			%ChPyActItrA = (mFloor(%tempCHPosY / $PTGm.caveB_itrA_XY)) * $PTGm.caveB_itrA_XY;

			%Co = ((%tempCHPosY-%ChPyActItrA)+%tempBrPosY) / $PTGm.caveB_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

			%Co = ((%tempCHPosX-%ChPxActItrA)+%tempBrPosX) / $PTGm.caveB_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
			
			%relZ = (%ItrARow*$PTGm.caveB_itrA_Z);
			%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
			$StrArrayHV_CavesB[%BrPosX,%BrPosY] = getMax(%tempZ,%MinBrZSnap);
				
			if((%CaveH = $StrArrayHV_CavesA[%BrPosX,%BrPosY]) < $PTGm.cavesSecZ)
			{
				%CaveH_Mod = $StrArrayHV_CavesB[%BrPosX,%BrPosY];
				$StrArrayHV_CavesA[%BrPosX,%BrPosY] = mFloor((($PTGm.cavesHLevel - ($PTGm.cavesSecZ - %CaveH)) + %CaveH_Mod) / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
				$StrArrayHV_CavesB[%BrPosX,%BrPosY] = mFloor(($PTGm.cavesHLevel + (($PTGm.cavesSecZ - %CaveH) * mClamp($PTGm.caveTopZMult,1,8)) + %CaveH_Mod) / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
			}
			else
			{
				$StrArrayHV_CavesA[%BrPosX,%BrPosY] = 0;
				$StrArrayHV_CavesB[%BrPosX,%BrPosY] = 0;
			}
		}
	}

	
	$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_secFuncMS,0,20));
	scheduleNoQuota(%delay,0,PTG_Routine_Calc,%cl,"CustomBiomeA",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Noise_Perlin_CustomBiomeA(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{
	%BrXYhSize = $PTGm.brTer_XYsize / 2;
	%ChPxRelItrA = (mFloor(%CHPosX / $PTGm.bio_CustA_itrA_XY)) * $PTGm.bio_CustA_itrA_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / $PTGm.bio_CustA_itrA_XY)) * $PTGm.bio_CustA_itrA_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.bio_CustAOff_X,%CHPyRelItrA+$PTGm.bio_CustAOff_Y,$PTGm.bio_CustA_itrA_XY,12192683,83059231,10007,54973);
	
	%ChPosStr = %ChPyRelItrA SPC %ChPyRelItrA+$PTGm.bio_CustA_itrA_XY;
	%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPosStr,$PTGm.Bio_CustA_YStart,99999);
	
	for(%BrPosY = %BrXYhSize; %BrPosY < mClamp($PTGm.chSize,16,256); %BrPosY += $PTGm.brTer_XYsize)
	{
		for(%BrPosX = %BrXYhSize; %BrPosX < mClamp($PTGm.chSize,16,256); %BrPosX += $PTGm.brTer_XYsize)
		{			
			%Co = ((%CHPosY-%ChPyRelItrA)+%BrPosY) / $PTGm.bio_CustA_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);
			
			%Co = ((%CHPosX-%ChPxRelItrA)+%BrPosX) / $PTGm.bio_CustA_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
			
			$StrArrayHV_CustomBiomeA[%BrPosX,%BrPosY] = %ItrARow * $PTGm.bio_CustA_itrA_Z;
		}
	}
	
	
	$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_secFuncMS,0,20));
	scheduleNoQuota(%delay,0,PTG_Routine_Calc,%cl,"CustomBiomeB",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Noise_Perlin_CustomBiomeB(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{
	%BrXYhSize = $PTGm.brTer_XYsize / 2;		
	%ChPxRelItrA = (mFloor(%CHPosX / $PTGm.bio_CustB_itrA_XY)) * $PTGm.bio_CustB_itrA_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / $PTGm.bio_CustB_itrA_XY)) * $PTGm.bio_CustB_itrA_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.bio_CustBOff_X,%CHPyRelItrA+$PTGm.bio_CustBOff_Y,$PTGm.bio_CustB_itrA_XY,13233757,55555333,21397,50153);
	
	%ChPosStr = %ChPyRelItrA SPC %ChPyRelItrA+$PTGm.bio_CustB_itrA_XY;
	%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPosStr,$PTGm.Bio_CustB_YStart,99999);
	
	for(%BrPosY = %BrXYhSize; %BrPosY < mClamp($PTGm.chSize,16,256); %BrPosY += $PTGm.brTer_XYsize)
	{
		for(%BrPosX = %BrXYhSize; %BrPosX < mClamp($PTGm.chSize,16,256); %BrPosX += $PTGm.brTer_XYsize)
		{			
			%Co = ((%CHPosY-%ChPyRelItrA)+%BrPosY) / $PTGm.bio_CustB_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);
			
			%Co = ((%CHPosX-%ChPxRelItrA)+%BrPosX) / $PTGm.bio_CustB_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
			
			$StrArrayHV_CustomBiomeB[%BrPosX,%BrPosY] = %ItrARow * $PTGm.bio_CustB_itrA_Z;
		}
	}
	
	
	$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_secFuncMS,0,20));
	scheduleNoQuota(%delay,0,PTG_Routine_Calc,%cl,"CustomBiomeC",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Noise_Perlin_CustomBiomeC(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{
	%BrXYhSize = $PTGm.brTer_XYsize / 2;
	%ChPxRelItrA = (mFloor(%CHPosX / $PTGm.bio_CustC_itrA_XY)) * $PTGm.bio_CustC_itrA_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / $PTGm.bio_CustC_itrA_XY)) * $PTGm.bio_CustC_itrA_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.bio_CustCOff_X,%CHPyRelItrA+$PTGm.bio_CustCOff_Y,$PTGm.bio_CustC_itrA_XY,14151617,71111111,32831,80557);
	
	%ChPosStr = %ChPyRelItrA SPC %ChPyRelItrA+$PTGm.bio_CustC_itrA_XY;
	%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPosStr,$PTGm.Bio_CustC_YStart,99999);
	
	for(%BrPosY = %BrXYhSize; %BrPosY < mClamp($PTGm.chSize,16,256); %BrPosY += $PTGm.brTer_XYsize)
	{
		for(%BrPosX = %BrXYhSize; %BrPosX < mClamp($PTGm.chSize,16,256); %BrPosX += $PTGm.brTer_XYsize)
		{			
			%Co = ((%CHPosY-%ChPyRelItrA)+%BrPosY) / $PTGm.bio_CustC_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);
			
			%Co = ((%CHPosX-%ChPxRelItrA)+%BrPosX) / $PTGm.bio_CustC_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
			
			$StrArrayHV_CustomBiomeC[%BrPosX,%BrPosY] = (%ItrARow * $PTGm.bio_CustC_itrA_Z);
		}
	}

	
	$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_secFuncMS,0,20));
	scheduleNoQuota(%delay,0,PTG_Routine_Calc,%cl,"Clouds",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
}


////////////////////////////////////////////////////////////////////////////////////////////////////

function PTG_Noise_Perlin_SkyLands(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{
	%MinBrZSnap = $PTGm.brTer_Zsize; // $PTGm.brTer_FillXYZSize + $PTGm.brTer_Zsize;
	%BrXYhSize = $PTGm.brTer_XYsize / 2;
	
	%ChPxActItrA = (mFloor(%CHPosX / $PTGm.skyLnds_itrA_XY)) * $PTGm.skyLnds_itrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / $PTGm.skyLnds_itrA_XY)) * $PTGm.skyLnds_itrA_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%ChPxActItrA+$PTGm.skyLandsOff_X,%ChPyActItrA+$PTGm.skyLandsOff_Y,$PTGm.skyLnds_itrA_XY,13466917,7649689,14107,79561);

	for(%BrPosY = %BrXYhSize; %BrPosY < mClamp($PTGm.chSize,16,256); %BrPosY += $PTGm.brTer_XYsize)
	{
		for(%BrPosX = %BrXYhSize; %BrPosX < mClamp($PTGm.chSize,16,256); %BrPosX += $PTGm.brTer_XYsize)
		{			
			%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.skyLnds_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);
			
			%Co = ((%CHPosX-%ChPxActItrA)+%BrPosX) / $PTGm.skyLnds_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
			
			%relZ = (%ItrARow*$PTGm.skyLnds_itrA_Z);
			%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
			%BrH_SL = getMax(%tempZ,%MinBrZSnap);
			
			%BrH_act = $StrArrayHV_Mountains[%BrPosX,%BrPosY] - $PTGm.terHLevel;
			$StrArrayHV_SkyLandsTop[%BrPosX,%BrPosY] = ($PTGm.terHLevel + (%BrH_act - $PTGm.skyLndsSecZ)) + %BrH_SL;
			$StrArrayHV_SkyLandsBtm[%BrPosX,%BrPosY] = ($PTGm.terHLevel - ((%BrH_act - $PTGm.skyLndsSecZ) * 2)) + %BrH_SL;
		}
	}

	
	$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_priFuncMS,0,100));
	scheduleNoQuota(%delay,0,PTG_Noise_Perlin_SkyLands_Edges,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Noise_Perlin_SkyLands_Edges(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{
	%MinBrZSnap = $PTGm.brTer_Zsize;
	%BrXYhSize = $PTGm.brTer_XYsize / 2;
	%ChSize = mClamp($PTGm.chSize,16,256);
	
	//////////////////////////////////////////////////
	//Left Side

	%tempCHPosX = %ChPosX - mClamp($PTGm.chSize,16,256);
	%tempBrPosX = mClamp($PTGm.chSize,16,256) - %BrXYhSize;
	%BrPosX = -1 * %BrXYhSize;
	
	%ChPxActItrA = (mFloor(%tempCHPosX / $PTGm.skyLnds_itrA_XY)) * $PTGm.skyLnds_itrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / $PTGm.skyLnds_itrA_XY)) * $PTGm.skyLnds_itrA_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%ChPxActItrA+$PTGm.skyLandsOff_X,%ChPyActItrA+$PTGm.skyLandsOff_Y,$PTGm.skyLnds_itrA_XY,13466917,7649689,14107,79561);
	
	for(%BrPosY = %BrXYhSize; %BrPosY < mClamp($PTGm.chSize,16,256); %BrPosY += $PTGm.brTer_XYsize)
	{
		%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.skyLnds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);		
		
		%Co = ((%tempCHPosX-%ChPxActItrA)+%tempBrPosX) / $PTGm.skyLnds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);		
		
		%relZ = (%ItrARow*$PTGm.skyLnds_itrA_Z);
		%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
		%BrH_SL = getMax(%tempZ,%MinBrZSnap);
		
		%BrH_act = $StrArrayHV_Mountains[%BrPosX,%BrPosY] - $PTGm.terHLevel;
		$StrArrayHV_SkyLandsTop[%BrPosX,%BrPosY] = ($PTGm.terHLevel + (%BrH_act - $PTGm.skyLndsSecZ)) + %BrH_SL;
		$StrArrayHV_SkyLandsBtm[%BrPosX,%BrPosY] = ($PTGm.terHLevel - ((%BrH_act - $PTGm.skyLndsSecZ) * 2)) + %BrH_SL;
	}
	
	//////////////////////////////////////////////////
	//Right Side

	%tempCHPosX = %ChPosX + mClamp($PTGm.chSize,16,256);
	%tempBrPosX = %BrXYhSize;
	%BrPosX = mClamp($PTGm.chSize,16,256) + %BrXYhSize;
	
	%ChPxActItrA = (mFloor(%tempCHPosX / $PTGm.skyLnds_itrA_XY)) * $PTGm.skyLnds_itrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / $PTGm.skyLnds_itrA_XY)) * $PTGm.skyLnds_itrA_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%ChPxActItrA+$PTGm.skyLandsOff_X,%ChPyActItrA+$PTGm.skyLandsOff_Y,$PTGm.skyLnds_itrA_XY,13466917,7649689,14107,79561);
	
	for(%BrPosY = %BrXYhSize; %BrPosY < mClamp($PTGm.chSize,16,256); %BrPosY += $PTGm.brTer_XYsize)
	{
		%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.skyLnds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);		
		
		%Co = ((%tempCHPosX-%ChPxActItrA)+%tempBrPosX) / $PTGm.skyLnds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);

		%relZ = (%ItrARow*$PTGm.skyLnds_itrA_Z);
		%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
		%BrH_SL = getMax(%tempZ,%MinBrZSnap);
		
		%BrH_act = $StrArrayHV_Mountains[%BrPosX,%BrPosY] - $PTGm.terHLevel;
		$StrArrayHV_SkyLandsTop[%BrPosX,%BrPosY] = ($PTGm.terHLevel + (%BrH_act - $PTGm.skyLndsSecZ)) + %BrH_SL;
		$StrArrayHV_SkyLandsBtm[%BrPosX,%BrPosY] = ($PTGm.terHLevel - ((%BrH_act - $PTGm.skyLndsSecZ) * 2)) + %BrH_SL;
	}
	
	//////////////////////////////////////////////////
	//Bottom Side

	%tempCHPosY = %ChPosY - mClamp($PTGm.chSize,16,256);
	%tempBrPosY = mClamp($PTGm.chSize,16,256) - %BrXYhSize;
	%BrPosY = -1 * %BrXYhSize;
	
	%ChPxActItrA = (mFloor(%ChPosX / $PTGm.skyLnds_itrA_XY)) * $PTGm.skyLnds_itrA_XY;
	%ChPyActItrA = (mFloor(%tempCHPosY / $PTGm.skyLnds_itrA_XY)) * $PTGm.skyLnds_itrA_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%ChPxActItrA+$PTGm.skyLandsOff_X,%ChPyActItrA+$PTGm.skyLandsOff_Y,$PTGm.skyLnds_itrA_XY,13466917,7649689,14107,79561);

	for(%BrPosX = %BrXYhSize; %BrPosX < mClamp($PTGm.chSize,16,256); %BrPosX += $PTGm.brTer_XYsize)
	{
		%Co = ((%tempCHPosY-%ChPyActItrA)+%tempBrPosY) / $PTGm.skyLnds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

		%Co = ((%ChPosX-%ChPxActItrA)+%BrPosX) / $PTGm.skyLnds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
		
		%relZ = (%ItrARow*$PTGm.skyLnds_itrA_Z);
		%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
		%BrH_SL = getMax(%tempZ,%MinBrZSnap);
		
		%BrH_act = $StrArrayHV_Mountains[%BrPosX,%BrPosY] - $PTGm.terHLevel;
		$StrArrayHV_SkyLandsTop[%BrPosX,%BrPosY] = ($PTGm.terHLevel + (%BrH_act - $PTGm.skyLndsSecZ)) + %BrH_SL;
		$StrArrayHV_SkyLandsBtm[%BrPosX,%BrPosY] = ($PTGm.terHLevel - ((%BrH_act - $PTGm.skyLndsSecZ) * 2)) + %BrH_SL;
	}
	
	//////////////////////////////////////////////////
	//Top Side

	%tempCHPosY = %ChPosY + mClamp($PTGm.chSize,16,256);
	%tempBrPosY = %BrXYhSize;//0;
	%BrPosY = mClamp($PTGm.chSize,16,256) + %BrXYhSize;
	
	%ChPxActItrA = (mFloor(%ChPosX / $PTGm.skyLnds_itrA_XY)) * $PTGm.skyLnds_itrA_XY;
	%ChPyActItrA = (mFloor(%tempCHPosY / $PTGm.skyLnds_itrA_XY)) * $PTGm.skyLnds_itrA_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%ChPxActItrA+$PTGm.skyLandsOff_X,%ChPyActItrA+$PTGm.skyLandsOff_Y,$PTGm.skyLnds_itrA_XY,13466917,7649689,14107,79561);

	for(%BrPosX = %BrXYhSize; %BrPosX < mClamp($PTGm.chSize,16,256); %BrPosX += $PTGm.brTer_XYsize)
	{
		%Co = ((%tempCHPosY-%ChPyActItrA)+%tempBrPosY) / $PTGm.skyLnds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

		%Co = ((%ChPosX-%ChPxActItrA)+%BrPosX) / $PTGm.skyLnds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
		
		%relZ = (%ItrARow*$PTGm.skyLnds_itrA_Z);
		%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
		%BrH_SL = getMax(%tempZ,%MinBrZSnap);
		
		%BrH_act = $StrArrayHV_Mountains[%BrPosX,%BrPosY] - $PTGm.terHLevel;
		$StrArrayHV_SkyLandsTop[%BrPosX,%BrPosY] = ($PTGm.terHLevel + (%BrH_act - $PTGm.skyLndsSecZ)) + %BrH_SL;
		$StrArrayHV_SkyLandsBtm[%BrPosX,%BrPosY] = ($PTGm.terHLevel - ((%BrH_act - $PTGm.skyLndsSecZ) * 2)) + %BrH_SL;
	}
	
	//////////////////////////////////////////////////
	
	if($PTGm.seamlessModTer)
	{
		for(%c = 0; %c < 4; %c++)
		{
			switch(%c)
			{
				case 0:
				
					%tempCHPosX = %ChPosX - %ChSize;
					%tempCHPosY = %ChPosY + %ChSize;
					%tempBrPosX = %ChSize - %BrXYhSize;
					%tempBrPosY = %BrXYhSize;
					
					%BrPosY = %ChSize + %BrXYhSize;
					%BrPosX = -1 * %BrXYhSize;
				
				case 1:
				
					%tempCHPosX = %ChPosX + %ChSize;
					%tempCHPosY = %ChPosY + %ChSize;
					%tempBrPosX = %BrXYhSize;
					%tempBrPosY = %BrXYhSize;
					
					%BrPosY = %ChSize + %BrXYhSize;
					%BrPosX = %ChSize + %BrXYhSize;
				
				case 2:
				
					%tempCHPosX = %ChPosX - %ChSize;
					%tempCHPosY = %ChPosY - %ChSize;
					%tempBrPosX = %ChSize - %BrXYhSize;
					%tempBrPosY = %ChSize - %BrXYhSize;
					
					%BrPosY = -1 * %BrXYhSize;
					%BrPosX = -1 * %BrXYhSize;
				
				case 3:
				
					%tempCHPosX = %ChPosX + %ChSize;
					%tempCHPosY = %ChPosY - %ChSize;
					%tempBrPosX = %BrXYhSize;
					%tempBrPosY = %ChSize - %BrXYhSize;
					
					%BrPosY = -1 * %BrXYhSize;
					%BrPosX = %ChSize + %BrXYhSize;
			}

			
			%ChPxActItrA = (mFloor(%tempCHPosX / $PTGm.skyLnds_itrA_XY)) * $PTGm.skyLnds_itrA_XY;
			%ChPyActItrA = (mFloor(%tempCHPosY / $PTGm.skyLnds_itrA_XY)) * $PTGm.skyLnds_itrA_XY;
			%ChHVItrA = PTG_RandNumGen_Chunk(%ChPxActItrA+$PTGm.skyLandsOff_X,%ChPyActItrA+$PTGm.skyLandsOff_Y,$PTGm.skyLnds_itrA_XY,13466917,7649689,14107,79561);

			%Co = ((%tempCHPosY-%ChPyActItrA)+%tempBrPosY) / $PTGm.skyLnds_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

			%Co = ((%tempCHPosX-%ChPxActItrA)+%tempBrPosX) / $PTGm.skyLnds_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
			
			%relZ = (%ItrARow*$PTGm.skyLnds_itrA_Z);
			%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
			%BrH_SL = getMax(%tempZ,%MinBrZSnap);
			
			%BrH_act = $StrArrayHV_Mountains[%BrPosX,%BrPosY] - $PTGm.terHLevel;
			$StrArrayHV_SkyLandsTop[%BrPosX,%BrPosY] = ($PTGm.terHLevel + (%BrH_act - $PTGm.skyLndsSecZ)) + %BrH_SL;
			$StrArrayHV_SkyLandsBtm[%BrPosX,%BrPosY] = ($PTGm.terHLevel - ((%BrH_act - $PTGm.skyLndsSecZ) * 2)) + %BrH_SL;
		}
	}

	
	$PTG.dedSrvrFuncCheckTime += %delaySec;
	scheduleNoQuota(%delaySec,0,PTG_Routine_Calc,%cl,"FltIslds",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Noise_FlatLands(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{
	%MinBrZSnap = $PTGm.brTer_Zsize;
	%BrZSize = %MinBrZSnap;
	%BrXYSize = $PTGm.brTer_XYsize;
	%BrXYhSize = %BrXYSize / 2;
	%ChSize = mClamp($PTGm.chSize,16,256);
	
	%brTer_XYsizeN = -1 * %BrXYhSize;
	%BrH = $PTGm.terHLevel;
	
	//Main chunk height values (automatically calculates edges and corners - whether or not seamless ModTer is enabled)
	for(%BrPosY = -%BrXYhSize; %BrPosY < (%ChSize+%BrXYSize); %BrPosY += %BrXYSize)
	{
		for(%BrPosX = -%BrXYhSize; %BrPosX < (%ChSize+%BrXYSize); %BrPosX += %BrXYSize)
		{
			//if using modter w/ flatlands, don't check corners like other functions do (not needed)
			$StrArrayHV_Terrain[%BrPosX,%BrPosY] = getMax(mFloor(PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BrH) / %BrZSize) * %BrZSize,%MinBrZSnap);
			
			if(!$PTGm.enabMntns) //If mountains are disabled, set up height value array to reference terrain instead (since some functions still use it)
				$StrArrayHV_Mountains[%BrPosX,%BrPosY] = getMax(mFloor(PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BrH) / %BrZSize) * %BrZSize,%MinBrZSnap);
		}
		
	}

	
	$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_secFuncMS,0,20));
	scheduleNoQuota(%delay,0,PTG_Routine_Calc,%cl,"Mountains",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Noise_2ItrFractal_Mntns(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{
	%MinBrZSnap = $PTGm.brTer_Zsize;
	%BrXYhSize = $PTGm.brTer_XYsize / 2;
	
	%ChPxRelItrA = (mFloor(%CHPosX / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrB_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrB_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.mntnsOff_X,%CHPyRelItrA+$PTGm.mntnsOff_Y,$PTGm.mntn_itrB_XY,27177289,71234567,14549,54163);
	%ChPxActItrA = (mFloor(%CHPosX / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrA_XY;

	%ChPosStr = %ChPyActItrA SPC %ChPyActItrA+$PTGm.mntn_itrA_XY;
	%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPosStr,$PTGm.Mntn_YStart,0);
	
	%CHPyRelItrB = (mFloor(%CHPosY / $PTGm.mntn_itrB_XY)) * $PTGm.mntn_itrB_XY;
	%ChPxRelItrB = (mFloor(%CHPosX / $PTGm.mntn_itrB_XY)) * $PTGm.mntn_itrB_XY;
	%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.mntnsOff_X,%CHPyRelItrB+$PTGm.mntnsOff_Y,$PTGm.mntn_itrB_XY,27177289,71234567,14549,54163);
	
	%ChPosStr = %CHPyRelItrB SPC %CHPyRelItrB+$PTGm.mntn_itrB_XY;
	%ChHVItrB = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB,%ChPosStr,$PTGm.Mntn_YStart,0);
	
	for(%BrPosY = %BrXYhSize; %BrPosY < mClamp($PTGm.chSize,16,256); %BrPosY += $PTGm.brTer_XYsize)
	{
		for(%BrPosX = %BrXYhSize; %BrPosX < mClamp($PTGm.chSize,16,256); %BrPosX += $PTGm.brTer_XYsize)
		{
			%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.mntn_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

			%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY) / $PTGm.mntn_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
			%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
			
			%Co = ((%CHPosX-%ChPxActItrA)+%BrPosX) / $PTGm.mntn_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) *%Sm);
			
			%Co = ((%CHPosX-%CHPxRelItrB)+%BrPosX) / $PTGm.mntn_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

			%relZ = (%ItrARow * $PTGm.mntn_itrA_Z) + (%ItrBRow * $PTGm.mntn_itrB_Z);
			%MntnH = getMax(%relZ,%MinBrZSnap);

			//Mountains terrain height offset
			%BrH = $StrArrayHV_Terrain[%BrPosX,%BrPosY];
			
			if(%MntnH > $PTGm.mntnsStrtSecZ)
			{
				%BrHb = mFloor(%BrH + ((%MntnH - $PTGm.mntnsStrtSecZ) * $PTGm.mntnsZMult));
				%relBrH = ($PTGm.brTer_Zsize * $PTGm.mntnsZSnap);
				
				//Edge-FallOff
				%BrHb = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BrHb);
				%BrHb = mFloor(%BrHb / %relBrH) * %relBrH;
				
				if(%BrHb < %BrH) 
					%BrHb = %BrH;
				$StrArrayHV_Mountains[%BrPosX,%BrPosY] = %BrHb;
			}
			else
				$StrArrayHV_Mountains[%BrPosX,%BrPosY] = %BrH;
			
			$StrArrayHV_MountainsAux[%BrPosX,%BrPosY] = $StrArrayHV_Mountains[%BrPosX,%BrPosY]; //always keep Mntns and MntnsAux the same, mainly for choosing brick colors / prints for mountains
		}
	}

	
	$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_priFuncMS,0,100));
	scheduleNoQuota(%delay,0,PTG_Noise_2ItrFractal_Mntn_Edges,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Noise_2ItrFractal_Mntn_Edges(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{	
	%MinBrZSnap = $PTGm.brTer_Zsize; // $PTGm.brTer_FillXYZSize + $PTGm.brTer_Zsize; //?
	%BrXYhSize = $PTGm.brTer_XYsize / 2;
	%ChSize = mClamp($PTGm.chSize,16,256);

	//////////////////////////////////////////////////
	//Left Side

	%tempCHPosX = %ChPosX - mClamp($PTGm.chSize,16,256);
	%tempBrPosX = mClamp($PTGm.chSize,16,256) - %BrXYhSize;//$PTGm.brTer_XYsize;
	%BrPosX = -1 * %BrXYhSize;//$PTGm.brTer_XYsize;
	
	%ChPxRelItrA = (mFloor(%tempCHPosX / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrB_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrB_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.mntnsOff_X,%CHPyRelItrA+$PTGm.mntnsOff_Y,$PTGm.mntn_itrB_XY,27177289,71234567,14549,54163);
	%ChPxActItrA = (mFloor(%tempCHPosX / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrA_XY;
	
	%ChPosStr = %ChPyActItrA SPC %ChPyActItrA+$PTGm.mntn_itrA_XY;
	%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPosStr,$PTGm.Mntn_YStart,0);
	
	%CHPyRelItrB = (mFloor(%CHPosY / $PTGm.mntn_itrB_XY)) * $PTGm.mntn_itrB_XY;
	%ChPxRelItrB = (mFloor(%tempCHPosX / $PTGm.mntn_itrB_XY)) * $PTGm.mntn_itrB_XY;
	%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.mntnsOff_X,%CHPyRelItrB+$PTGm.mntnsOff_Y,$PTGm.mntn_itrB_XY,27177289,71234567,14549,54163);
	
	%ChPosStr = %CHPyRelItrB SPC %CHPyRelItrB+$PTGm.mntn_itrB_XY;
	%ChHVItrB = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB,%ChPosStr,$PTGm.Mntn_YStart,0);
	
	for(%BrPosY = %BrXYhSize; %BrPosY < mClamp($PTGm.chSize,16,256); %BrPosY += $PTGm.brTer_XYsize)
	{
		%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.mntn_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

		%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY) / $PTGm.mntn_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
		%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
		
		%Co = ((%tempCHPosX-%ChPxActItrA)+%tempBrPosX) / $PTGm.mntn_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
		
		%Co = ((%tempCHPosX-%CHPxRelItrB)+%tempBrPosX) / $PTGm.mntn_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

		%relZ = (%ItrARow*$PTGm.mntn_itrA_Z) + (%ItrBRow*$PTGm.mntn_itrB_Z);
		%MntnH = getMax(%relZ,%MinBrZSnap);

		//Mountains terrain height offset
		%BrH = $StrArrayHV_Terrain[%BrPosX,%BrPosY];
		
		if(%MntnH > $PTGm.mntnsStrtSecZ)
		{
			%BrHb = mFloor(%BrH + ((%MntnH - $PTGm.mntnsStrtSecZ) * $PTGm.mntnsZMult));
			%relBrH = $PTGm.brTer_Zsize * $PTGm.mntnsZSnap;
			
			//Edge-FallOff
			%BrHb = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BrHb);
			%BrHb = mFloor(%BrHb / %relBrH) * %relBrH;
			
			if(%BrHb < %BrH)
				%BrHb = %BrH;
			$StrArrayHV_Mountains[%BrPosX,%BrPosY] = %BrHb;
		}
		else
			$StrArrayHV_Mountains[%BrPosX,%BrPosY] = %BrH;
		
		$StrArrayHV_MountainsAux[%BrPosX,%BrPosY] = $StrArrayHV_Mountains[%BrPosX,%BrPosY]; //always keep Mntns and MntnsAux the same, mainly for choosing brick colors / prints for mountains
	}
	
	//////////////////////////////////////////////////
	//Right Side

	%tempCHPosX = %ChPosX + mClamp($PTGm.chSize,16,256);
	%tempBrPosX = %BrXYhSize;//0;
	%BrPosX = mClamp($PTGm.chSize,16,256) + %BrXYhSize;
	
	%ChPxRelItrA = (mFloor(%tempCHPosX / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrB_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrB_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.mntnsOff_X,%CHPyRelItrA+$PTGm.mntnsOff_Y,$PTGm.mntn_itrB_XY,27177289,71234567,14549,54163);
	%ChPxActItrA = (mFloor(%tempCHPosX / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrA_XY;
	
	%ChPosStr = %ChPyActItrA SPC %ChPyActItrA+$PTGm.mntn_itrA_XY;
	%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPosStr,$PTGm.Mntn_YStart,0);
	
	%CHPyRelItrB = (mFloor(%CHPosY / $PTGm.mntn_itrB_XY)) * $PTGm.mntn_itrB_XY;
	%ChPxRelItrB = (mFloor(%tempCHPosX / $PTGm.mntn_itrB_XY)) * $PTGm.mntn_itrB_XY;
	%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.mntnsOff_X,%CHPyRelItrB+$PTGm.mntnsOff_Y,$PTGm.mntn_itrB_XY,27177289,71234567,14549,54163);
	
	%ChPosStr = %CHPyRelItrB SPC %CHPyRelItrB+$PTGm.mntn_itrB_XY;
	%ChHVItrB = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB,%ChPosStr,$PTGm.Mntn_YStart,0);
	
	for(%BrPosY = %BrXYhSize; %BrPosY < mClamp($PTGm.chSize,16,256); %BrPosY += $PTGm.brTer_XYsize)
	{
		%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.mntn_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

		%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY) / $PTGm.mntn_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
		%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
		
		%Co = ((%tempCHPosX-%ChPxActItrA)+%tempBrPosX) / $PTGm.mntn_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
		
		%Co = ((%tempCHPosX-%CHPxRelItrB)+%tempBrPosX) / $PTGm.mntn_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);
		
		%relZ = (%ItrARow*$PTGm.mntn_itrA_Z) + (%ItrBRow*$PTGm.mntn_itrB_Z);
		%MntnH = getMax(%relZ,%MinBrZSnap);

		//Mountains terrain height offset
		%BrH = $StrArrayHV_Terrain[%BrPosX,%BrPosY];
		if(%MntnH > $PTGm.mntnsStrtSecZ)
		{
			%BrHb = mFloor(%BrH + ((%MntnH - $PTGm.mntnsStrtSecZ) * $PTGm.mntnsZMult));
			%relBrH = $PTGm.brTer_Zsize * $PTGm.mntnsZSnap;
			
			//Edge-FallOff
			%BrHb = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BrHb);
			%BrHb = mFloor(%BrHb / %relBrH) * %relBrH;
			
			if(%BrHb < %BrH)
				%BrHb = %BrH;
			$StrArrayHV_Mountains[%BrPosX,%BrPosY] = %BrHb;
		}
		else
			$StrArrayHV_Mountains[%BrPosX,%BrPosY] = %BrH;
		
		$StrArrayHV_MountainsAux[%BrPosX,%BrPosY] = $StrArrayHV_Mountains[%BrPosX,%BrPosY]; //always keep Mntns and MntnsAux the same, mainly for choosing brick colors / prints for mountains
	}
	
	//////////////////////////////////////////////////
	//Bottom Side

	%tempCHPosY = %ChPosY - mClamp($PTGm.chSize,16,256);
	%tempBrPosY = mClamp($PTGm.chSize,16,256) - %BrXYhSize;//$PTGm.brTer_XYsize;
	%BrPosY = -1 * %BrXYhSize;//$PTGm.brTer_XYsize;
	
	%ChPxRelItrA = (mFloor(%ChPosX / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrB_XY;
	%ChPyRelItrA = (mFloor(%tempCHPosY / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrB_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.mntnsOff_X,%CHPyRelItrA+$PTGm.mntnsOff_Y,$PTGm.mntn_itrB_XY,27177289,71234567,14549,54163);
	%ChPxActItrA = (mFloor(%ChPosX / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrA_XY;
	%ChPyActItrA = (mFloor(%tempCHPosY / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrA_XY;
	
	%ChPosStr = %ChPyActItrA SPC %ChPyActItrA+$PTGm.mntn_itrA_XY;
	%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPosStr,$PTGm.Mntn_YStart,0);
	
	%CHPyRelItrB = (mFloor(%tempCHPosY / $PTGm.mntn_itrB_XY)) * $PTGm.mntn_itrB_XY;
	%ChPxRelItrB = (mFloor(%ChPosX / $PTGm.mntn_itrB_XY)) * $PTGm.mntn_itrB_XY;
	%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.mntnsOff_X,%CHPyRelItrB+$PTGm.mntnsOff_Y,$PTGm.mntn_itrB_XY,27177289,71234567,14549,54163);
	
	%ChPosStr = %CHPyRelItrB SPC %CHPyRelItrB+$PTGm.mntn_itrB_XY;
	%ChHVItrB = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB,%ChPosStr,$PTGm.Mntn_YStart,0);
	
	for(%BrPosX = %BrXYhSize; %BrPosX < mClamp($PTGm.chSize,16,256); %BrPosX += $PTGm.brTer_XYsize)
	{	
		%Co = ((%tempCHPosY-%ChPyActItrA)+%tempBrPosY) / $PTGm.mntn_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

		%Co = ((%tempCHPosY-%CHPyRelItrB)+%tempBrPosY) / $PTGm.mntn_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
		%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
		
		%Co = ((%ChPosX-%ChPxActItrA)+%BrPosX) / $PTGm.mntn_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
		
		%Co = ((%ChPosX-%CHPxRelItrB)+%BrPosX) / $PTGm.mntn_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);
		
		%relZ = (%ItrARow*$PTGm.mntn_itrA_Z) + (%ItrBRow*$PTGm.mntn_itrB_Z);
		%MntnH = getMax(%relZ,%MinBrZSnap);

		//Mountains terrain height offset
		%BrH = $StrArrayHV_Terrain[%BrPosX,%BrPosY];
		if(%MntnH > $PTGm.mntnsStrtSecZ)
		{
			%BrHb = mFloor(%BrH + ((%MntnH - $PTGm.mntnsStrtSecZ) * $PTGm.mntnsZMult));
			%relBrH = $PTGm.brTer_Zsize * $PTGm.mntnsZSnap;
			
			//Edge-FallOff
			%BrHb = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BrHb);
			%BrHb = mFloor(%BrHb / %relBrH) * %relBrH;
			
			if(%BrHb < %BrH)
				%BrHb = %BrH;
			$StrArrayHV_Mountains[%BrPosX,%BrPosY] = %BrHb;
		}
		else
			$StrArrayHV_Mountains[%BrPosX,%BrPosY] = %BrH;
		
		$StrArrayHV_MountainsAux[%BrPosX,%BrPosY] = $StrArrayHV_Mountains[%BrPosX,%BrPosY]; //always keep Mntns and MntnsAux the same, mainly for choosing brick colors / prints for mountains
	}
	
	//////////////////////////////////////////////////
	//Top Side

	%tempCHPosY = %ChPosY + mClamp($PTGm.chSize,16,256);
	%tempBrPosY = %BrXYhSize;//0;
	%BrPosY = mClamp($PTGm.chSize,16,256) + %BrXYhSize;
	
	%ChPxRelItrA = (mFloor(%ChPosX / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrB_XY;
	%ChPyRelItrA = (mFloor(%tempCHPosY / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrB_XY;
	%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.mntnsOff_X,%CHPyRelItrA+$PTGm.mntnsOff_Y,$PTGm.mntn_itrB_XY,27177289,71234567,14549,54163);
	%ChPxActItrA = (mFloor(%ChPosX / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrA_XY;
	%ChPyActItrA = (mFloor(%tempCHPosY / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrA_XY;
	
	%ChPosStr = %ChPyActItrA SPC %ChPyActItrA+$PTGm.mntn_itrA_XY;
	%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPosStr,$PTGm.Mntn_YStart,0);
	
	%CHPyRelItrB = (mFloor(%tempCHPosY / $PTGm.mntn_itrB_XY)) * $PTGm.mntn_itrB_XY;
	%ChPxRelItrB = (mFloor(%ChPosX / $PTGm.mntn_itrB_XY)) * $PTGm.mntn_itrB_XY;
	%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.mntnsOff_X,%CHPyRelItrB+$PTGm.mntnsOff_Y,$PTGm.mntn_itrB_XY,27177289,71234567,14549,54163);
	
	%ChPosStr = %CHPyRelItrB SPC %CHPyRelItrB+$PTGm.mntn_itrB_XY;
	%ChHVItrB = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB,%ChPosStr,$PTGm.Mntn_YStart,0);
	
	for(%BrPosX = %BrXYhSize; %BrPosX < mClamp($PTGm.chSize,16,256); %BrPosX += $PTGm.brTer_XYsize)
	{
		%Co = ((%tempCHPosY-%ChPyActItrA)+%tempBrPosY) / $PTGm.mntn_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

		%Co = ((%tempCHPosY-%CHPyRelItrB)+%tempBrPosY) / $PTGm.mntn_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
		%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
		
		%Co = ((%ChPosX-%ChPxActItrA)+%BrPosX) / $PTGm.mntn_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
		
		%Co = ((%ChPosX-%CHPxRelItrB)+%BrPosX) / $PTGm.mntn_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

		%relZ = (%ItrARow*$PTGm.mntn_itrA_Z) + (%ItrBRow*$PTGm.mntn_itrB_Z);
		%MntnH = getMax(%relZ,%MinBrZSnap);

		//Mountains terrain height offset
		%BrH = $StrArrayHV_Terrain[%BrPosX,%BrPosY];
		if(%MntnH > $PTGm.mntnsStrtSecZ)
		{
			%BrHb = mFloor(%BrH + ((%MntnH - $PTGm.mntnsStrtSecZ) * $PTGm.mntnsZMult));
			%relBrH = $PTGm.brTer_Zsize * $PTGm.mntnsZSnap;
			
			//Edge-FallOff
			%BrHb = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BrHb);
			%BrHb = mFloor(%BrHb / %relBrH) * %relBrH;
			
			if(%BrHb < %BrH)
				%BrHb = %BrH;
			$StrArrayHV_Mountains[%BrPosX,%BrPosY] = %BrHb;
		}
		else
			$StrArrayHV_Mountains[%BrPosX,%BrPosY] = %BrH;
		
		$StrArrayHV_MountainsAux[%BrPosX,%BrPosY] = $StrArrayHV_Mountains[%BrPosX,%BrPosY]; //always keep Mntns and MntnsAux the same, mainly for choosing brick colors / prints for mountains
	}
	
	//////////////////////////////////////////////////
	
	if($PTGm.seamlessModTer)
	{
		for(%c = 0; %c < 4; %c++)
		{
			switch(%c)
			{
				case 0:
				
					%tempCHPosX = %ChPosX - %ChSize;
					%tempCHPosY = %ChPosY + %ChSize;
					%tempBrPosX = %ChSize - %BrXYhSize;
					%tempBrPosY = %BrXYhSize;
					
					%BrPosY = %ChSize + %BrXYhSize;
					%BrPosX = -1 * %BrXYhSize;
				
				case 1:
				
					%tempCHPosX = %ChPosX + %ChSize;
					%tempCHPosY = %ChPosY + %ChSize;
					%tempBrPosX = %BrXYhSize;
					%tempBrPosY = %BrXYhSize;
					
					%BrPosY = %ChSize + %BrXYhSize;
					%BrPosX = %ChSize + %BrXYhSize;
				
				case 2:
				
					%tempCHPosX = %ChPosX - %ChSize;
					%tempCHPosY = %ChPosY - %ChSize;
					%tempBrPosX = %ChSize - %BrXYhSize;
					%tempBrPosY = %ChSize - %BrXYhSize;
					
					%BrPosY = -1 * %BrXYhSize;
					%BrPosX = -1 * %BrXYhSize;
				
				case 3:
				
					%tempCHPosX = %ChPosX + %ChSize;
					%tempCHPosY = %ChPosY - %ChSize;
					%tempBrPosX = %BrXYhSize;
					%tempBrPosY = %ChSize - %BrXYhSize;
					
					%BrPosY = -1 * %BrXYhSize;
					%BrPosX = %ChSize + %BrXYhSize;
			}

			
			%ChPxRelItrA = (mFloor(%tempCHPosX / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrB_XY;
			%ChPyRelItrA = (mFloor(%tempCHPosY / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrB_XY;
			%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.mntnsOff_X,%CHPyRelItrA+$PTGm.mntnsOff_Y,$PTGm.mntn_itrB_XY,27177289,71234567,14549,54163);
			%ChPxActItrA = (mFloor(%tempCHPosX / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrA_XY;
			%ChPyActItrA = (mFloor(%tempCHPosY / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrA_XY;
			
			%ChPosStr = %ChPyActItrA SPC %ChPyActItrA+$PTGm.mntn_itrA_XY;
			%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPosStr,$PTGm.Mntn_YStart,0);
			
			%CHPyRelItrB = (mFloor(%tempCHPosY / $PTGm.mntn_itrB_XY)) * $PTGm.mntn_itrB_XY;
			%ChPxRelItrB = (mFloor(%tempCHPosX / $PTGm.mntn_itrB_XY)) * $PTGm.mntn_itrB_XY;
			%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.mntnsOff_X,%CHPyRelItrB+$PTGm.mntnsOff_Y,$PTGm.mntn_itrB_XY,27177289,71234567,14549,54163);
			
			%ChPosStr = %CHPyRelItrB SPC %CHPyRelItrB+$PTGm.mntn_itrB_XY;
			%ChHVItrB = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB,%ChPosStr,$PTGm.Mntn_YStart,0);


			%Co = ((%tempCHPosY-%ChPyActItrA)+%tempBrPosY) / $PTGm.mntn_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

			%Co = ((%tempCHPosY-%CHPyRelItrB)+%tempBrPosY) / $PTGm.mntn_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
			%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
			
			%Co = ((%tempCHPosX-%ChPxActItrA)+%tempBrPosX) / $PTGm.mntn_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
			
			%Co = ((%tempCHPosX-%CHPxRelItrB)+%tempBrPosX) / $PTGm.mntn_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

			%relZ = (%ItrARow*$PTGm.mntn_itrA_Z) + (%ItrBRow*$PTGm.mntn_itrB_Z);
			%MntnH = getMax(%relZ,%MinBrZSnap);

			//Mountains terrain height offset
			%BrH = $StrArrayHV_Terrain[%BrPosX,%BrPosY];
			
			if(%MntnH > $PTGm.mntnsStrtSecZ)
			{
				%BrHb = mFloor(%BrH + ((%MntnH - $PTGm.mntnsStrtSecZ) * $PTGm.mntnsZMult));
				%relBrH = $PTGm.brTer_Zsize * $PTGm.mntnsZSnap;
				
				//Edge-FallOff
				%BrHb = PTG_Noise_EdgeFallOff(%tempCHPosX+%tempBrPosX,%tempCHPosY+%tempBrPosY,%BrHb);
				%BrHb = mFloor(%BrHb / %relBrH) * %relBrH;
				
				if(%BrHb < %BrH)
					%BrHb = %BrH;
				$StrArrayHV_Mountains[%BrPosX,%BrPosY] = %BrHb;
			}
			else
				$StrArrayHV_Mountains[%BrPosX,%BrPosY] = %BrH;
			
			$StrArrayHV_MountainsAux[%BrPosX,%BrPosY] = $StrArrayHV_Mountains[%BrPosX,%BrPosY]; //always keep Mntns and MntnsAux the same, mainly for choosing brick colors / prints for mountains
		}
	}

	
	$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_secFuncMS,0,20));
	scheduleNoQuota(%delay,0,PTG_Routine_Calc,%cl,"SkyLands",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Noise_2ItrFractal_FltIslds(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{
	%MinBrZSnap = $PTGm.brFltIslds_Zsize; // $PTGm.brFltIslds_FillXYZSize + $PTGm.brFltIslds_Zsize;
	%BrXYhSize = $PTGm.brFltIslds_XYsize / 2;
	
	%ChPxRelItrA = (mFloor(%CHPosX / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrB_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrB_XY;
	%ChHVItrA_A = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.fltIsldsOff_X,%CHPyRelItrA+$PTGm.fltIsldsOff_Y,$PTGm.fltIslds_itrB_XY,55555333,87889091,22273,33773);
	%ChHVItrA_B = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.fltIsldsBOff_X,%CHPyRelItrA+$PTGm.fltIsldsBOff_Y,$PTGm.fltIslds_itrB_XY,24710753,60000607,22273,33773);
	%ChPxActItrA = (mFloor(%CHPosX / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrA_XY;
	
	%ChHVItrA_A = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA_A,%ChPyActItrA SPC %ChPyActItrA+$PTGm.fltIslds_itrA_XY,$PTGm.FltIsld_YStart,0);
	%ChHVItrA_B = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA_B,%ChPyActItrA SPC %ChPyActItrA+$PTGm.fltIslds_itrA_XY,$PTGm.FltIsld_YStart,0);
	
	%CHPyRelItrB = (mFloor(%CHPosY / $PTGm.fltIslds_itrB_XY)) * $PTGm.fltIslds_itrB_XY;
	%ChPxRelItrB = (mFloor(%CHPosX / $PTGm.fltIslds_itrB_XY)) * $PTGm.fltIslds_itrB_XY;
	%ChHVItrB_A = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.fltIsldsOff_X,%CHPyRelItrB+$PTGm.fltIsldsOff_Y,$PTGm.fltIslds_itrB_XY,55555333,87889091,22273,33773);
	%ChHVItrB_B = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.fltIsldsBOff_X,%CHPyRelItrB+$PTGm.fltIsldsBOff_Y,$PTGm.fltIslds_itrB_XY,24710753,60000607,22273,33773);
	
	%ChHVItrB_A = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB_A,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.fltIslds_itrB_XY,$PTGm.FltIsld_YStart,0);
	%ChHVItrB_B = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB_B,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.fltIslds_itrB_XY,$PTGm.FltIsld_YStart,0);
	
	for(%BrPosY = %BrXYhSize; %BrPosY < mClamp($PTGm.chSize,16,256); %BrPosY += $PTGm.brFltIslds_XYsize)
	{
		for(%BrPosX = %BrXYhSize; %BrPosX < mClamp($PTGm.chSize,16,256); %BrPosX += $PTGm.brFltIslds_XYsize)
		{
			%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.fltIslds_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA_A,0) - ((getWord(%ChHVItrA_A,0) - getWord(%ChHVItrA_A,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA_A,2) - ((getWord(%ChHVItrA_A,2) - getWord(%ChHVItrA_A,3)) * %Sm);

			%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY) / $PTGm.fltIslds_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBColL = getWord(%ChHVItrB_A,0) - ((getWord(%ChHVItrB_A,0) - getWord(%ChHVItrB_A,1)) * %Sm);
			%ItrBColR = getWord(%ChHVItrB_A,2) - ((getWord(%ChHVItrB_A,2) - getWord(%ChHVItrB_A,3)) * %Sm);
			
			%Co = ((%CHPosX-%ChPxActItrA)+%BrPosX) / $PTGm.fltIslds_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) *%Sm);
			
			%Co = ((%CHPosX-%CHPxRelItrB)+%BrPosX) / $PTGm.fltIslds_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

			%relZ = (%ItrARow * $PTGm.fltIslds_itrA_Z) + (%ItrBRow * $PTGm.fltIslds_itrB_Z);
			
			//Edge-FallOff
			%relZ = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%relZ);
		
			%tempZ = mFloor(%relZ / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize;

			if(%tempZ > $PTGm.fltIsldsSecZ)
			{
				$StrArrayHV_FltIsldsATop[%BrPosX,%BrPosY] = $PTGm.fltIsldsAHLevel + (mFloor((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
				$StrArrayHV_FltIsldsABtm[%BrPosX,%BrPosY] = $PTGm.fltIsldsAHLevel - (mFloor(((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) * 1.5) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
			}
			
			%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.fltIslds_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA_B,0) - ((getWord(%ChHVItrA_B,0) - getWord(%ChHVItrA_B,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA_B,2) - ((getWord(%ChHVItrA_B,2) - getWord(%ChHVItrA_B,3)) * %Sm);

			%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY) / $PTGm.fltIslds_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBColL = getWord(%ChHVItrB_B,0) - ((getWord(%ChHVItrB_B,0) - getWord(%ChHVItrB_B,1)) * %Sm);
			%ItrBColR = getWord(%ChHVItrB_B,2) - ((getWord(%ChHVItrB_B,2) - getWord(%ChHVItrB_B,3)) * %Sm);
			
			%Co = ((%CHPosX-%ChPxActItrA)+%BrPosX) / $PTGm.fltIslds_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) *%Sm);
			
			%Co = ((%CHPosX-%CHPxRelItrB)+%BrPosX) / $PTGm.fltIslds_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

			%relZ = (%ItrARow * $PTGm.fltIslds_itrA_Z) + (%ItrBRow * $PTGm.fltIslds_itrB_Z);
			
			//Edge-FallOff
			%relZ = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%relZ);

			%tempZ = mFloor(%relZ / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize;
			
			if(%tempZ > $PTGm.fltIsldsSecZ)
			{
				$StrArrayHV_FltIsldsBTop[%BrPosX,%BrPosY] = $PTGm.fltIsldsBHLevel + (mFloor((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
				$StrArrayHV_FltIsldsBBtm[%BrPosX,%BrPosY] = $PTGm.fltIsldsBHLevel - (mFloor(((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) * 1.5) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
			}
		}
	}


	$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_priFuncMS,0,100));
	scheduleNoQuota(%delay,0,PTG_Noise_2ItrFractal_FltIslds_Edges,%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Noise_2ItrFractal_FltIslds_Edges(%cl,%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum)
{
	//Left Side

	%MinBrZSnap = $PTGm.brFltIslds_Zsize; // $PTGm.brFltIslds_FillXYZSize + $PTGm.brFltIslds_Zsize;
	%BrXYhSize = $PTGm.brFltIslds_XYsize / 2;
	%ChSize = mClamp($PTGm.chSize,16,256);
	
	%tempCHPosX = %ChPosX - mClamp($PTGm.chSize,16,256);
	%tempBrPosX = mClamp($PTGm.chSize,16,256) - %BrXYhSize;//$PTGm.brFltIslds_XYsize;
	%BrPosX = -1 * %BrXYhSize;//$PTGm.brFltIslds_XYsize;
	
	%ChPxRelItrA = (mFloor(%tempCHPosX / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrB_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrB_XY;
	%ChHVItrA_A = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.fltIsldsOff_X,%CHPyRelItrA+$PTGm.fltIsldsOff_Y,$PTGm.fltIslds_itrB_XY,55555333,87889091,22273,33773);
	%ChHVItrA_B = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.fltIsldsBOff_X,%CHPyRelItrA+$PTGm.fltIsldsBOff_Y,$PTGm.fltIslds_itrB_XY,24710753,60000607,22273,33773);
	%ChPxActItrA = (mFloor(%tempCHPosX / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrA_XY;
	
	%ChHVItrA_A = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA_A,%ChPyActItrA SPC %ChPyActItrA+$PTGm.fltIslds_itrA_XY,$PTGm.FltIsld_YStart,0);
	%ChHVItrA_B = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA_B,%ChPyActItrA SPC %ChPyActItrA+$PTGm.fltIslds_itrA_XY,$PTGm.FltIsld_YStart,0);
	
	%CHPyRelItrB = (mFloor(%CHPosY / $PTGm.fltIslds_itrB_XY)) * $PTGm.fltIslds_itrB_XY;
	%ChPxRelItrB = (mFloor(%tempCHPosX / $PTGm.fltIslds_itrB_XY)) * $PTGm.fltIslds_itrB_XY;
	%ChHVItrB_A = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.fltIsldsOff_X,%CHPyRelItrB+$PTGm.fltIsldsOff_Y,$PTGm.fltIslds_itrB_XY,55555333,87889091,22273,33773);
	%ChHVItrB_B = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.fltIsldsBOff_X,%CHPyRelItrB+$PTGm.fltIsldsBOff_Y,$PTGm.fltIslds_itrB_XY,24710753,60000607,22273,33773);
	
	%ChHVItrB_A = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB_A,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.fltIslds_itrB_XY,$PTGm.FltIsld_YStart,0);
	%ChHVItrB_B = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB_B,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.fltIslds_itrB_XY,$PTGm.FltIsld_YStart,0);

	for(%BrPosY = %BrXYhSize; %BrPosY < mClamp($PTGm.chSize,16,256); %BrPosY += $PTGm.brFltIslds_XYsize)
	{
		%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.fltIslds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA_A,0) - ((getWord(%ChHVItrA_A,0) - getWord(%ChHVItrA_A,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA_A,2) - ((getWord(%ChHVItrA_A,2) - getWord(%ChHVItrA_A,3)) * %Sm);

		%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY) / $PTGm.fltIslds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBColL = getWord(%ChHVItrB_A,0) - ((getWord(%ChHVItrB_A,0) - getWord(%ChHVItrB_A,1)) * %Sm);
		%ItrBColR = getWord(%ChHVItrB_A,2) - ((getWord(%ChHVItrB_A,2) - getWord(%ChHVItrB_A,3)) * %Sm);
		
		%Co = ((%CHPosX-%ChPxActItrA)+%BrPosX) / $PTGm.fltIslds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) *%Sm);
		
		%Co = ((%CHPosX-%CHPxRelItrB)+%BrPosX) / $PTGm.fltIslds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

		%relZ = (%ItrARow * $PTGm.fltIslds_itrA_Z) + (%ItrBRow * $PTGm.fltIslds_itrB_Z);
		
		//Edge-FallOff
		%relZ = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%relZ);
			
		%tempZ = mFloor(%relZ / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize;

		if(%tempZ > $PTGm.fltIsldsSecZ)
		{
			$StrArrayHV_FltIsldsATop[%BrPosX,%BrPosY] = $PTGm.fltIsldsAHLevel + (mFloor((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
			$StrArrayHV_FltIsldsABtm[%BrPosX,%BrPosY] = $PTGm.fltIsldsAHLevel - (mFloor(((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) * 1.5) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
		}
		
		%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.fltIslds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA_B,0) - ((getWord(%ChHVItrA_B,0) - getWord(%ChHVItrA_B,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA_B,2) - ((getWord(%ChHVItrA_B,2) - getWord(%ChHVItrA_B,3)) * %Sm);

		%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY) / $PTGm.fltIslds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBColL = getWord(%ChHVItrB_B,0) - ((getWord(%ChHVItrB_B,0) - getWord(%ChHVItrB_B,1)) * %Sm);
		%ItrBColR = getWord(%ChHVItrB_B,2) - ((getWord(%ChHVItrB_B,2) - getWord(%ChHVItrB_B,3)) * %Sm);
		
		%Co = ((%CHPosX-%ChPxActItrA)+%BrPosX) / $PTGm.fltIslds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) *%Sm);

		%Co = ((%CHPosX-%CHPxRelItrB)+%BrPosX) / $PTGm.fltIslds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

		%relZ = (%ItrARow * $PTGm.fltIslds_itrA_Z) + (%ItrBRow * $PTGm.fltIslds_itrB_Z);
		
		//Edge-FallOff
		%relZ = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%relZ);

		%tempZ = mFloor(%relZ / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize;

		if(%tempZ > $PTGm.fltIsldsSecZ)
		{
			$StrArrayHV_FltIsldsBTop[%BrPosX,%BrPosY] = $PTGm.fltIsldsBHLevel + (mFloor((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
			$StrArrayHV_FltIsldsBBtm[%BrPosX,%BrPosY] = $PTGm.fltIsldsBHLevel - (mFloor(((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) * 1.5) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
		}
	}
	
	//////////////////////////////////////////////////
	//Right Side

	%tempCHPosX = %ChPosX + mClamp($PTGm.chSize,16,256);
	%tempBrPosX = %BrXYhSize;//0;
	%BrPosX = mClamp($PTGm.chSize,16,256) + %BrXYhSize;
	
	%ChPxRelItrA = (mFloor(%tempCHPosX / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrB_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrB_XY;
	%ChHVItrA_A = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.fltIsldsOff_X,%CHPyRelItrA+$PTGm.fltIsldsOff_Y,$PTGm.fltIslds_itrB_XY,55555333,87889091,22273,33773);
	%ChHVItrA_B = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.fltIsldsBOff_X,%CHPyRelItrA+$PTGm.fltIsldsBOff_Y,$PTGm.fltIslds_itrB_XY,24710753,60000607,22273,33773);
	%ChPxActItrA = (mFloor(%tempCHPosX / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrA_XY;
	
	%ChHVItrA_A = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA_A,%ChPyActItrA SPC %ChPyActItrA+$PTGm.fltIslds_itrA_XY,$PTGm.FltIsld_YStart,0);
	%ChHVItrA_B = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA_B,%ChPyActItrA SPC %ChPyActItrA+$PTGm.fltIslds_itrA_XY,$PTGm.FltIsld_YStart,0);
	
	%CHPyRelItrB = (mFloor(%CHPosY / $PTGm.fltIslds_itrB_XY)) * $PTGm.fltIslds_itrB_XY;
	%ChPxRelItrB = (mFloor(%tempCHPosX / $PTGm.fltIslds_itrB_XY)) * $PTGm.fltIslds_itrB_XY;
	%ChHVItrB_A = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.fltIsldsOff_X,%CHPyRelItrB+$PTGm.fltIsldsOff_Y,$PTGm.fltIslds_itrB_XY,55555333,87889091,22273,33773);
	%ChHVItrB_B = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.fltIsldsBOff_X,%CHPyRelItrB+$PTGm.fltIsldsBOff_Y,$PTGm.fltIslds_itrB_XY,24710753,60000607,22273,33773);
	
	%ChHVItrB_A = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB_A,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.fltIslds_itrB_XY,$PTGm.FltIsld_YStart,0);
	%ChHVItrB_B = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB_B,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.fltIslds_itrB_XY,$PTGm.FltIsld_YStart,0);
	
	for(%BrPosY = %BrXYhSize; %BrPosY < mClamp($PTGm.chSize,16,256); %BrPosY += $PTGm.brFltIslds_XYsize)
	{	
		%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.fltIslds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA_A,0) - ((getWord(%ChHVItrA_A,0) - getWord(%ChHVItrA_A,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA_A,2) - ((getWord(%ChHVItrA_A,2) - getWord(%ChHVItrA_A,3)) * %Sm);

		%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY) / $PTGm.fltIslds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBColL = getWord(%ChHVItrB_A,0) - ((getWord(%ChHVItrB_A,0) - getWord(%ChHVItrB_A,1)) * %Sm);
		%ItrBColR = getWord(%ChHVItrB_A,2) - ((getWord(%ChHVItrB_A,2) - getWord(%ChHVItrB_A,3)) * %Sm);
		
		%Co = ((%CHPosX-%ChPxActItrA)+%BrPosX) / $PTGm.fltIslds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) *%Sm);
		
		%Co = ((%CHPosX-%CHPxRelItrB)+%BrPosX) / $PTGm.fltIslds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

		%relZ = (%ItrARow * $PTGm.fltIslds_itrA_Z) + (%ItrBRow * $PTGm.fltIslds_itrB_Z);
		
		//Edge-FallOff
		%relZ = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%relZ);
		
		%tempZ = mFloor(%relZ / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize;
		
		if(%tempZ > $PTGm.fltIsldsSecZ)
		{
			$StrArrayHV_FltIsldsATop[%BrPosX,%BrPosY] = $PTGm.fltIsldsAHLevel + (mFloor((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
			$StrArrayHV_FltIsldsABtm[%BrPosX,%BrPosY] = $PTGm.fltIsldsAHLevel - (mFloor(((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) * 1.5) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
		}
		
		%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.fltIslds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA_B,0) - ((getWord(%ChHVItrA_B,0) - getWord(%ChHVItrA_B,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA_B,2) - ((getWord(%ChHVItrA_B,2) - getWord(%ChHVItrA_B,3)) * %Sm);

		%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY) / $PTGm.fltIslds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBColL = getWord(%ChHVItrB_B,0) - ((getWord(%ChHVItrB_B,0) - getWord(%ChHVItrB_B,1)) * %Sm);
		%ItrBColR = getWord(%ChHVItrB_B,2) - ((getWord(%ChHVItrB_B,2) - getWord(%ChHVItrB_B,3)) * %Sm);
		
		%Co = ((%CHPosX-%ChPxActItrA)+%BrPosX) / $PTGm.fltIslds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) *%Sm);
		
		%Co = ((%CHPosX-%CHPxRelItrB)+%BrPosX) / $PTGm.fltIslds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

		%relZ = (%ItrARow * $PTGm.fltIslds_itrA_Z) + (%ItrBRow * $PTGm.fltIslds_itrB_Z);
		
		//Edge-FallOff
		%relZ = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%relZ);
		
		%tempZ = mFloor(%relZ / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize;
		
		if(%tempZ > $PTGm.fltIsldsSecZ)
		{
			$StrArrayHV_FltIsldsBTop[%BrPosX,%BrPosY] = $PTGm.fltIsldsBHLevel + (mFloor((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
			$StrArrayHV_FltIsldsBBtm[%BrPosX,%BrPosY] = $PTGm.fltIsldsBHLevel - (mFloor(((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) * 1.5) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
		}
	}
	
	//////////////////////////////////////////////////
	//Bottom Side

	%tempCHPosY = %ChPosY - mClamp($PTGm.chSize,16,256);
	%tempBrPosY = mClamp($PTGm.chSize,16,256) - %BrXYhSize;//$PTGm.brFltIslds_XYsize;
	%BrPosY = -1 * %BrXYhSize;//$PTGm.brFltIslds_XYsize;
	
	%ChPxRelItrA = (mFloor(%ChPosX / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrB_XY;
	%ChPyRelItrA = (mFloor(%tempCHPosY / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrB_XY;
	%ChHVItrA_A = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.fltIsldsOff_X,%CHPyRelItrA+$PTGm.fltIsldsOff_Y,$PTGm.fltIslds_itrB_XY,55555333,87889091,22273,33773);
	%ChHVItrA_B = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.fltIsldsBOff_X,%CHPyRelItrA+$PTGm.fltIsldsBOff_Y,$PTGm.fltIslds_itrB_XY,24710753,60000607,22273,33773);
	%ChPxActItrA = (mFloor(%ChPosX / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrA_XY;
	%ChPyActItrA = (mFloor(%tempCHPosY / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrA_XY;
	
	%ChHVItrA_A = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA_A,%ChPyActItrA SPC %ChPyActItrA+$PTGm.fltIslds_itrA_XY,$PTGm.FltIsld_YStart,0);
	%ChHVItrA_B = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA_B,%ChPyActItrA SPC %ChPyActItrA+$PTGm.fltIslds_itrA_XY,$PTGm.FltIsld_YStart,0);
	
	%CHPyRelItrB = (mFloor(%tempCHPosY / $PTGm.fltIslds_itrB_XY)) * $PTGm.fltIslds_itrB_XY;
	%ChPxRelItrB = (mFloor(%ChPosX / $PTGm.fltIslds_itrB_XY)) * $PTGm.fltIslds_itrB_XY;
	%ChHVItrB_A = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.fltIsldsOff_X,%CHPyRelItrB+$PTGm.fltIsldsOff_Y,$PTGm.fltIslds_itrB_XY,55555333,87889091,22273,33773);
	%ChHVItrB_B = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.fltIsldsBOff_X,%CHPyRelItrB+$PTGm.fltIsldsBOff_Y,$PTGm.fltIslds_itrB_XY,24710753,60000607,22273,33773);
	
	%ChHVItrB_A = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB_A,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.fltIslds_itrB_XY,$PTGm.FltIsld_YStart,0);
	%ChHVItrB_B = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB_B,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.fltIslds_itrB_XY,$PTGm.FltIsld_YStart,0);
	
	for(%BrPosX = %BrXYhSize; %BrPosX < mClamp($PTGm.chSize,16,256); %BrPosX += $PTGm.brFltIslds_XYsize)
	{
		%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.fltIslds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA_A,0) - ((getWord(%ChHVItrA_A,0) - getWord(%ChHVItrA_A,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA_A,2) - ((getWord(%ChHVItrA_A,2) - getWord(%ChHVItrA_A,3)) * %Sm);

		%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY) / $PTGm.fltIslds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBColL = getWord(%ChHVItrB_A,0) - ((getWord(%ChHVItrB_A,0) - getWord(%ChHVItrB_A,1)) * %Sm);
		%ItrBColR = getWord(%ChHVItrB_A,2) - ((getWord(%ChHVItrB_A,2) - getWord(%ChHVItrB_A,3)) * %Sm);
		
		%Co = ((%CHPosX-%ChPxActItrA)+%BrPosX) / $PTGm.fltIslds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) *%Sm);
		
		%Co = ((%CHPosX-%CHPxRelItrB)+%BrPosX) / $PTGm.fltIslds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

		%relZ = (%ItrARow * $PTGm.fltIslds_itrA_Z) + (%ItrBRow * $PTGm.fltIslds_itrB_Z);
		
		//Edge-FallOff
		%relZ = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%relZ);
		
		%tempZ = mFloor(%relZ / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize;
		
		if(%tempZ > $PTGm.fltIsldsSecZ)
		{
			$StrArrayHV_FltIsldsATop[%BrPosX,%BrPosY] = $PTGm.fltIsldsAHLevel + (mFloor((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
			$StrArrayHV_FltIsldsABtm[%BrPosX,%BrPosY] = $PTGm.fltIsldsAHLevel - (mFloor(((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) * 1.5) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
		}
		
		%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.fltIslds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA_B,0) - ((getWord(%ChHVItrA_B,0) - getWord(%ChHVItrA_B,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA_B,2) - ((getWord(%ChHVItrA_B,2) - getWord(%ChHVItrA_B,3)) * %Sm);

		%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY) / $PTGm.fltIslds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBColL = getWord(%ChHVItrB_B,0) - ((getWord(%ChHVItrB_B,0) - getWord(%ChHVItrB_B,1)) * %Sm);
		%ItrBColR = getWord(%ChHVItrB_B,2) - ((getWord(%ChHVItrB_B,2) - getWord(%ChHVItrB_B,3)) * %Sm);
		
		%Co = ((%CHPosX-%ChPxActItrA)+%BrPosX) / $PTGm.fltIslds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) *%Sm);
		
		%Co = ((%CHPosX-%CHPxRelItrB)+%BrPosX) / $PTGm.fltIslds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

		%relZ = (%ItrARow * $PTGm.fltIslds_itrA_Z) + (%ItrBRow * $PTGm.fltIslds_itrB_Z);
		
		//Edge-FallOff
		%relZ = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%relZ);
		
		%tempZ = mFloor(%relZ / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize;
		
		if(%tempZ > $PTGm.fltIsldsSecZ)
		{
			$StrArrayHV_FltIsldsBTop[%BrPosX,%BrPosY] = $PTGm.fltIsldsBHLevel + (mFloor((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
			$StrArrayHV_FltIsldsBBtm[%BrPosX,%BrPosY] = $PTGm.fltIsldsBHLevel - (mFloor(((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) * 1.5) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
		}
	}
	
	//////////////////////////////////////////////////
	//Top Side

	%tempCHPosY = %ChPosY + mClamp($PTGm.chSize,16,256);
	%tempBrPosY = %BrXYhSize;//0;
	%BrPosY = mClamp($PTGm.chSize,16,256) + %BrXYhSize;
	
	%ChPxRelItrA = (mFloor(%ChPosX / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrB_XY;
	%ChPyRelItrA = (mFloor(%tempCHPosY / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrB_XY;
	%ChHVItrA_A = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.fltIsldsOff_X,%CHPyRelItrA+$PTGm.fltIsldsOff_Y,$PTGm.fltIslds_itrB_XY,55555333,87889091,22273,33773);
	%ChHVItrA_B = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.fltIsldsBOff_X,%CHPyRelItrA+$PTGm.fltIsldsBOff_Y,$PTGm.fltIslds_itrB_XY,24710753,60000607,22273,33773);
	%ChPxActItrA = (mFloor(%ChPosX / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrA_XY;
	%ChPyActItrA = (mFloor(%tempCHPosY / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrA_XY;
	
	%ChHVItrA_A = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA_A,%ChPyActItrA SPC %ChPyActItrA+$PTGm.fltIslds_itrA_XY,$PTGm.FltIsld_YStart,0);
	%ChHVItrA_B = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA_B,%ChPyActItrA SPC %ChPyActItrA+$PTGm.fltIslds_itrA_XY,$PTGm.FltIsld_YStart,0);
	
	%CHPyRelItrB = (mFloor(%tempCHPosY / $PTGm.fltIslds_itrB_XY)) * $PTGm.fltIslds_itrB_XY;
	%ChPxRelItrB = (mFloor(%ChPosX / $PTGm.fltIslds_itrB_XY)) * $PTGm.fltIslds_itrB_XY;
	%ChHVItrB_A = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.fltIsldsOff_X,%CHPyRelItrB+$PTGm.fltIsldsOff_Y,$PTGm.fltIslds_itrB_XY,55555333,87889091,22273,33773);
	%ChHVItrB_B = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.fltIsldsBOff_X,%CHPyRelItrB+$PTGm.fltIsldsBOff_Y,$PTGm.fltIslds_itrB_XY,24710753,60000607,22273,33773);
	
	%ChHVItrB_A = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB_A,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.fltIslds_itrB_XY,$PTGm.FltIsld_YStart,0);
	%ChHVItrB_B = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB_B,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.fltIslds_itrB_XY,$PTGm.FltIsld_YStart,0);
	
	for(%BrPosX = %BrXYhSize; %BrPosX < mClamp($PTGm.chSize,16,256); %BrPosX += $PTGm.brFltIslds_XYsize)
	{
		%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.fltIslds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA_A,0) - ((getWord(%ChHVItrA_A,0) - getWord(%ChHVItrA_A,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA_A,2) - ((getWord(%ChHVItrA_A,2) - getWord(%ChHVItrA_A,3)) * %Sm);

		%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY) / $PTGm.fltIslds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBColL = getWord(%ChHVItrB_A,0) - ((getWord(%ChHVItrB_A,0) - getWord(%ChHVItrB_A,1)) * %Sm);
		%ItrBColR = getWord(%ChHVItrB_A,2) - ((getWord(%ChHVItrB_A,2) - getWord(%ChHVItrB_A,3)) * %Sm);
		
		%Co = ((%CHPosX-%ChPxActItrA)+%BrPosX) / $PTGm.fltIslds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) *%Sm);
		
		%Co = ((%CHPosX-%CHPxRelItrB)+%BrPosX) / $PTGm.fltIslds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

		%relZ = (%ItrARow * $PTGm.fltIslds_itrA_Z) + (%ItrBRow * $PTGm.fltIslds_itrB_Z);
		
		//Edge-FallOff
		%relZ = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%relZ);
		
		%tempZ = mFloor(%relZ / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize;
		
		if(%tempZ > $PTGm.fltIsldsSecZ)
		{
			$StrArrayHV_FltIsldsATop[%BrPosX,%BrPosY] = $PTGm.fltIsldsAHLevel + (mFloor((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
			$StrArrayHV_FltIsldsABtm[%BrPosX,%BrPosY] = $PTGm.fltIsldsAHLevel - (mFloor(((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) * 1.5) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
		}
		
		%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / $PTGm.fltIslds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA_B,0) - ((getWord(%ChHVItrA_B,0) - getWord(%ChHVItrA_B,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA_B,2) - ((getWord(%ChHVItrA_B,2) - getWord(%ChHVItrA_B,3)) * %Sm);

		%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY) / $PTGm.fltIslds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBColL = getWord(%ChHVItrB_B,0) - ((getWord(%ChHVItrB_B,0) - getWord(%ChHVItrB_B,1)) * %Sm);
		%ItrBColR = getWord(%ChHVItrB_B,2) - ((getWord(%ChHVItrB_B,2) - getWord(%ChHVItrB_B,3)) * %Sm);
		
		%Co = ((%CHPosX-%ChPxActItrA)+%BrPosX) / $PTGm.fltIslds_itrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) *%Sm);
		
		%Co = ((%CHPosX-%CHPxRelItrB)+%BrPosX) / $PTGm.fltIslds_itrB_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

		%relZ = (%ItrARow * $PTGm.fltIslds_itrA_Z) + (%ItrBRow * $PTGm.fltIslds_itrB_Z);
		
		//Edge-FallOff
		%relZ = PTG_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%relZ);
		
		%tempZ = mFloor(%relZ / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize;
		
		if(%tempZ > $PTGm.fltIsldsSecZ)
		{
			$StrArrayHV_FltIsldsBTop[%BrPosX,%BrPosY] = $PTGm.fltIsldsBHLevel + (mFloor((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
			$StrArrayHV_FltIsldsBBtm[%BrPosX,%BrPosY] = $PTGm.fltIsldsBHLevel - (mFloor(((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) * 1.5) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
		}
	}
	
	//////////////////////////////////////////////////
	
	if($PTGm.seamlessModTer)
	{
		for(%c = 0; %c < 4; %c++)
		{
			switch(%c)
			{
				case 0:
				
					%tempCHPosX = %ChPosX - %ChSize;
					%tempCHPosY = %ChPosY + %ChSize;
					%tempBrPosX = %ChSize - %BrXYhSize;
					%tempBrPosY = %BrXYhSize;
					
					%BrPosY = %ChSize + %BrXYhSize;
					%BrPosX = -1 * %BrXYhSize;
				
				case 1:
				
					%tempCHPosX = %ChPosX + %ChSize;
					%tempCHPosY = %ChPosY + %ChSize;
					%tempBrPosX = %BrXYhSize;
					%tempBrPosY = %BrXYhSize;
					
					%BrPosY = %ChSize + %BrXYhSize;
					%BrPosX = %ChSize + %BrXYhSize;
				
				case 2:
				
					%tempCHPosX = %ChPosX - %ChSize;
					%tempCHPosY = %ChPosY - %ChSize;
					%tempBrPosX = %ChSize - %BrXYhSize;
					%tempBrPosY = %ChSize - %BrXYhSize;
					
					%BrPosY = -1 * %BrXYhSize;
					%BrPosX = -1 * %BrXYhSize;
				
				case 3:
				
					%tempCHPosX = %ChPosX + %ChSize;
					%tempCHPosY = %ChPosY - %ChSize;
					%tempBrPosX = %BrXYhSize;
					%tempBrPosY = %ChSize - %BrXYhSize;
					
					%BrPosY = -1 * %BrXYhSize;
					%BrPosX = %ChSize + %BrXYhSize;
			}

			
			%ChPxRelItrA = (mFloor(%tempCHPosX / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrB_XY;
			%ChPyRelItrA = (mFloor(%tempCHPosY / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrB_XY;
			%ChHVItrA_A = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.fltIsldsOff_X,%CHPyRelItrA+$PTGm.fltIsldsOff_Y,$PTGm.fltIslds_itrB_XY,55555333,87889091,22273,33773);
			%ChHVItrA_B = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.fltIsldsBOff_X,%CHPyRelItrA+$PTGm.fltIsldsBOff_Y,$PTGm.fltIslds_itrB_XY,24710753,60000607,22273,33773);
			%ChPxActItrA = (mFloor(%tempCHPosX / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrA_XY;
			%ChPyActItrA = (mFloor(%tempCHPosY / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrA_XY;
			
			%ChHVItrA_A = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA_A,%ChPyActItrA SPC %ChPyActItrA+$PTGm.fltIslds_itrA_XY,$PTGm.FltIsld_YStart,0);
			%ChHVItrA_B = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA_B,%ChPyActItrA SPC %ChPyActItrA+$PTGm.fltIslds_itrA_XY,$PTGm.FltIsld_YStart,0);
			
			%CHPyRelItrB = (mFloor(%tempCHPosY / $PTGm.fltIslds_itrB_XY)) * $PTGm.fltIslds_itrB_XY;
			%ChPxRelItrB = (mFloor(%tempCHPosX / $PTGm.fltIslds_itrB_XY)) * $PTGm.fltIslds_itrB_XY;
			%ChHVItrB_A = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.fltIsldsOff_X,%CHPyRelItrB+$PTGm.fltIsldsOff_Y,$PTGm.fltIslds_itrB_XY,55555333,87889091,22273,33773);
			%ChHVItrB_B = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.fltIsldsBOff_X,%CHPyRelItrB+$PTGm.fltIsldsBOff_Y,$PTGm.fltIslds_itrB_XY,24710753,60000607,22273,33773);
			
			%ChHVItrB_A = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB_A,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.fltIslds_itrB_XY,$PTGm.FltIsld_YStart,0);
			%ChHVItrB_B = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB_B,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.fltIslds_itrB_XY,$PTGm.FltIsld_YStart,0);

			%Co = ((%tempCHPosY-%ChPyActItrA)+%tempBrPosY) / $PTGm.fltIslds_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA_A,0) - ((getWord(%ChHVItrA_A,0) - getWord(%ChHVItrA_A,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA_A,2) - ((getWord(%ChHVItrA_A,2) - getWord(%ChHVItrA_A,3)) * %Sm);

			%Co = ((%tempCHPosY-%CHPyRelItrB)+%tempBrPosY) / $PTGm.fltIslds_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBColL = getWord(%ChHVItrB_A,0) - ((getWord(%ChHVItrB_A,0) - getWord(%ChHVItrB_A,1)) * %Sm);
			%ItrBColR = getWord(%ChHVItrB_A,2) - ((getWord(%ChHVItrB_A,2) - getWord(%ChHVItrB_A,3)) * %Sm);
			
			%Co = ((%tempCHPosX-%ChPxActItrA)+%tempBrPosX) / $PTGm.fltIslds_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) *%Sm);
			
			%Co = ((%tempCHPosX-%CHPxRelItrB)+%tempBrPosX) / $PTGm.fltIslds_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

			%relZ = (%ItrARow * $PTGm.fltIslds_itrA_Z) + (%ItrBRow * $PTGm.fltIslds_itrB_Z);
			
			//Edge-FallOff
			%relZ = PTG_Noise_EdgeFallOff(%tempCHPosX+%tempBrPosX,%tempCHPosY+%tempBrPosY,%relZ);
			
			%tempZ = mFloor(%relZ / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize;
			
			if(%tempZ > $PTGm.fltIsldsSecZ)
			{
				$StrArrayHV_FltIsldsATop[%BrPosX,%BrPosY] = $PTGm.fltIsldsAHLevel + (mFloor((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
				$StrArrayHV_FltIsldsABtm[%BrPosX,%BrPosY] = $PTGm.fltIsldsAHLevel - (mFloor(((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) * 1.5) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
			}
			
			%Co = ((%tempCHPosY-%ChPyActItrA)+%tempBrPosY) / $PTGm.fltIslds_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA_B,0) - ((getWord(%ChHVItrA_B,0) - getWord(%ChHVItrA_B,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA_B,2) - ((getWord(%ChHVItrA_B,2) - getWord(%ChHVItrA_B,3)) * %Sm);

			%Co = ((%tempCHPosY-%CHPyRelItrB)+%tempBrPosY) / $PTGm.fltIslds_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBColL = getWord(%ChHVItrB_B,0) - ((getWord(%ChHVItrB_B,0) - getWord(%ChHVItrB_B,1)) * %Sm);
			%ItrBColR = getWord(%ChHVItrB_B,2) - ((getWord(%ChHVItrB_B,2) - getWord(%ChHVItrB_B,3)) * %Sm);
			
			%Co = ((%tempCHPosX-%ChPxActItrA)+%tempBrPosX) / $PTGm.fltIslds_itrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) *%Sm);
			
			%Co = ((%tempCHPosX-%CHPxRelItrB)+%tempBrPosX) / $PTGm.fltIslds_itrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

			%relZ = (%ItrARow * $PTGm.fltIslds_itrA_Z) + (%ItrBRow * $PTGm.fltIslds_itrB_Z);
			
			//Edge-FallOff
			%relZ = PTG_Noise_EdgeFallOff(%tempCHPosX+%tempBrPosX,%tempCHPosY+%tempBrPosY,%relZ);
			
			%tempZ = mFloor(%relZ / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize;
			
			if(%tempZ > $PTGm.fltIsldsSecZ)
			{
				$StrArrayHV_FltIsldsBTop[%BrPosX,%BrPosY] = $PTGm.fltIsldsBHLevel + (mFloor((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
				$StrArrayHV_FltIsldsBBtm[%BrPosX,%BrPosY] = $PTGm.fltIsldsBHLevel - (mFloor(((getMax(%tempZ,%MinBrZSnap) - $PTGm.fltIsldsSecZ) * 1.5) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
			}
		}
	}

	
	$PTG.dedSrvrFuncCheckTime += (%delay = mClamp($PTG.calcDelay_secFuncMS,0,20));
	scheduleNoQuota(%delay,0,PTG_Routine_Calc,%cl,"BuildLoading",%Chunk,%CHPosX,%CHPosY,%xmod,%ymod,%clNum);
}

