function PTG_Noise_PsuedoEquatorCheck(%ChHValStr,%ChPosStr,%relOffset,%biomeMax)
{
	if($PTGm.enabPseudoEqtr)
	{
		if(mAbs(getWord(%ChPosStr,0)) < %relOffset)
		{
			%ChHValStr = setWord(%ChHValStr,0,%biomeMax); //set to %biomeMax to prevent biome from generating, if biome is outside the Y-axis offset (biomes generate if < section value).
			%ChHValStr = setWord(%ChHValStr,2,%biomeMax);
		}
		if(mAbs(getWord(%ChPosStr,1)) < %relOffset)
		{
			%ChHValStr = setWord(%ChHValStr,1,%biomeMax);
			%ChHValStr = setWord(%ChHValStr,3,%biomeMax);
		}
	}
	
	return %ChHValStr;
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Noise_EdgeFallOff(%PosX,%PosY,%PosZ)
{
	%newZ = %PosZ;
	
	if($PTGm.enabEdgeFallOff && $PTGm.genType $= "Finite")
	{
		if(%PosX < ($PTGm.gridStartX + $PTGm.edgeFOffDist))
		{
			%Co = (%PosX - $PTGm.gridStartX) / $PTGm.edgeFOffDist;
			%Co = mClampF(%Co,0.0,1.0);
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%newZ = %newZ * %Sm;
		}
		if(%PosX > ($PTGm.gridEndX - $PTGm.edgeFOffDist))
		{
			%Co = (%PosX - ($PTGm.gridEndX - $PTGm.edgeFOffDist)) / $PTGm.edgeFOffDist;
			%Co = mAbs(mClampF(%Co,0.0,1.0) - 1); //invert
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%newZ = %newZ * %Sm;
		}
		if(%PosY < ($PTGm.gridStartY + $PTGm.edgeFOffDist))
		{
			%Co = (%PosY - $PTGm.gridStartY) / $PTGm.edgeFOffDist;
			%Co = mClampF(%Co,0.0,1.0);
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%newZ = %newZ * %Sm;
		}
		if(%PosY > ($PTGm.gridEndY - $PTGm.edgeFOffDist))
		{
			%Co = (%PosY - ($PTGm.gridEndY - $PTGm.edgeFOffDist)) / $PTGm.edgeFOffDist;
			%Co = mAbs(mClampF(%Co,0.0,1.0) - 1); //invert
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%newZ = %newZ * %Sm;
		}
	}
	
	return %newZ;
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Noise_BiomeTerHMod(%ChPosX,%ChPosY,%ChHVItr)
{
	//Prevent issue with returning pass and fail conditions string if height value array is blank
	if(%ChHVItr $= "" || %ChHVItr $= " ")
		%ChHVItr = "0 0 0 0";
		
	%ChHVItrMod = %ChHVItr;
	%bioAPass = false;
	%bioBPass = false;
	%bioCPass = false;
	%bioAFail = true;
	%bioBFail = true;
	%bioCFail = true;
	
	if($PTGm.enabBio_CustA) //Custom Biome A
	{
		%bioCheckStr = PTG_Noise_Perlin_BiomeCheck(%ChPosX,%ChPosY,$PTGm.bio_CustA_itrA_XY,$PTGm.bio_CustA_itrA_Z,$PTGm.bio_CustAOff_X,$PTGm.bio_CustAOff_Y,$PTGm.Bio_CustA_YStart);
		
		if((getWord(%bioCheckStr,0)) < $PTGm.bio_CustASecZ) %ChHVItrMod = setWord(%ChHVItrMod, 0, getWord(%ChHVItr, 0) * $PTGbio.Bio_CustA_TerHMod);
		if((getWord(%bioCheckStr,2)) < $PTGm.bio_CustASecZ) %ChHVItrMod = setWord(%ChHVItrMod, 1, getWord(%ChHVItr, 1) * $PTGbio.Bio_CustA_TerHMod);
		if((getWord(%bioCheckStr,1)) < $PTGm.bio_CustASecZ) %ChHVItrMod = setWord(%ChHVItrMod, 2, getWord(%ChHVItr, 2) * $PTGbio.Bio_CustA_TerHMod);
		if((getWord(%bioCheckStr,3)) < $PTGm.bio_CustASecZ) %ChHVItrMod = setWord(%ChHVItrMod, 3, getWord(%ChHVItr, 3) * $PTGbio.Bio_CustA_TerHMod);
		
		if($StrArrayHV_CustomBiomeA[%ChPosX,%ChPosY] < $PTGm.bio_CustASecZ)
			%bioAPass = true;
		else
			%bioAFail = true;
	}
	else
		%bioAFail = true;
	
	if($PTGm.enabBio_CustB) //Custom Biome B
	{
		%bioCheckStr = PTG_Noise_Perlin_BiomeCheck(%ChPosX,%ChPosY,$PTGm.bio_CustB_itrA_XY,$PTGm.bio_CustB_itrA_Z,$PTGm.bio_CustBOff_X,$PTGm.bio_CustBOff_Y,$PTGm.Bio_CustB_YStart);
		
		if((getWord(%bioCheckStr,0)) < $PTGm.bio_CustBSecZ) %ChHVItrMod = setWord(%ChHVItrMod, 0, getWord(%ChHVItr, 0) * $PTGbio.Bio_CustB_TerHMod);
		if((getWord(%bioCheckStr,2)) < $PTGm.bio_CustBSecZ) %ChHVItrMod = setWord(%ChHVItrMod, 1, getWord(%ChHVItr, 1) * $PTGbio.Bio_CustB_TerHMod);
		if((getWord(%bioCheckStr,1)) < $PTGm.bio_CustBSecZ) %ChHVItrMod = setWord(%ChHVItrMod, 2, getWord(%ChHVItr, 2) * $PTGbio.Bio_CustB_TerHMod);
		if((getWord(%bioCheckStr,3)) < $PTGm.bio_CustBSecZ) %ChHVItrMod = setWord(%ChHVItrMod, 3, getWord(%ChHVItr, 3) * $PTGbio.Bio_CustB_TerHMod);
		
		if($StrArrayHV_CustomBiomeB[%ChPosX,%ChPosY] < $PTGm.bio_CustBSecZ)
			%bioBPass = true;
		else
			%bioBFail = true;
	}
	else
		%bioBFail = true;
	
	if($PTGm.enabBio_CustC) //Custom Biome B
	{
		%bioCheckStr = PTG_Noise_Perlin_BiomeCheck(%ChPosX,%ChPosY,$PTGm.bio_CustC_itrA_XY,$PTGm.bio_CustC_itrA_Z,$PTGm.bio_CustCOff_X,$PTGm.bio_CustCOff_Y,$PTGm.Bio_CustC_YStart);
		
		if((getWord(%bioCheckStr,0)) < $PTGm.bio_CustCSecZ) %ChHVItrMod = setWord(%ChHVItrMod, 0, getWord(%ChHVItr, 0) * $PTGbio.Bio_CustC_TerHMod);
		if((getWord(%bioCheckStr,2)) < $PTGm.bio_CustCSecZ) %ChHVItrMod = setWord(%ChHVItrMod, 1, getWord(%ChHVItr, 1) * $PTGbio.Bio_CustC_TerHMod);
		if((getWord(%bioCheckStr,1)) < $PTGm.bio_CustCSecZ) %ChHVItrMod = setWord(%ChHVItrMod, 2, getWord(%ChHVItr, 2) * $PTGbio.Bio_CustC_TerHMod);
		if((getWord(%bioCheckStr,3)) < $PTGm.bio_CustCSecZ) %ChHVItrMod = setWord(%ChHVItrMod, 3, getWord(%ChHVItr, 3) * $PTGbio.Bio_CustC_TerHMod);
		
		if($StrArrayHV_CustomBiomeC[%ChPosX,%ChPosY] < $PTGm.bio_CustCSecZ)
			%bioCPass = true;
		else
			%bioCFail = true;
	}
	else
		%bioCFail = true;
	
	%ChHVItr = %ChHVItrMod;
	
	return %ChHVItr SPC %bioAPass SPC %bioAFail SPC %bioBPass SPC %bioBFail SPC %bioCPass SPC %bioCFail;
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Noise_Perlin_BiomeCheck(%CHPosX,%CHPosY,%ItrxyScale,%ItrZScale,%OffX,%OffY,%biomeOff)
{
	//This func only works if the biome scale is >= terrain scale, otherwise skips func
	
	//%BrXYhSize = $PTGm.brTer_XYsize / 2;
	%ItrxyScaleTer =  $PTGm.ter_ItrA_XY; //check
	%ChPxActItrTer = (mFloor(%CHPosX / %ItrxyScaleTer)) * %ItrxyScaleTer;
	%ChPyActItrTer = (mFloor(%CHPosY / %ItrxyScaleTer)) * %ItrxyScaleTer;
	
	%ChPxRelItr = (mFloor(%CHPosX / %ItrxyScale)) * %ItrxyScale;
	%ChPyRelItr = (mFloor(%CHPosY / %ItrxyScale)) * %ItrxyScale;
	%ChHVItr = PTG_RandNumGen_Chunk(%CHPxRelItr+%offX,%CHPyRelItr+%OffY,%ItrxyScale,12192683,83059231,10007,54973);
	
	%ChPosStr = %ChPyRelItr SPC %ChPyRelItr+%ItrxyScale;
	%ChHVItr = PTG_Noise_PsuedoEquatorCheck(%ChHVItr,%ChPosStr,%biomeOff,99999);

	%wrdCount = 0;
	
	for(%BrPosY = 0; %BrPosY <= 1; %BrPosY++)
	{
		for(%BrPosX = 0; %BrPosX <= 1; %BrPosX++)
		{
			%Co = ((%ChPyActItrTer % %ItrxyScale) + (%BrPosY * %ItrxyScaleTer)) / %ItrxyScale;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrColL = getWord(%ChHVItr,0) - ((getWord(%ChHVItr,0) - getWord(%ChHVItr,1)) * %Sm);
			%ItrColR = getWord(%ChHVItr,2) - ((getWord(%ChHVItr,2) - getWord(%ChHVItr,3)) * %Sm);
		
			%Co = ((%ChPxActItrTer % %ItrxyScale) + (%BrPosX * %ItrxyScaleTer)) / %ItrxyScale;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrRow = %ItrColL - ((%ItrColL - %ItrColR) * %Sm);

			%valStr = setWord(%valStr,%wrdCount,%ItrRow * %ItrZScale);
			%wrdCount++;
		}
	}
	
	return %valStr;
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Noise_BuildGridHRef(%CHPosX_init,%CHPosY_init,%relGrid,%bnds) //%BrXYhSize? 
{
	%xMin = getWord(%bnds,0);
	%xMax = getWord(%bnds,1);
	%yMin = getWord(%bnds,2);
	%yMax = getWord(%bnds,3);
	%zMin = getWord(%bnds,4);
	%zMax = getWord(%bnds,5);
	
	%ChPosX_rel = mFloor(%CHPosX_init / %relGrid) * %relGrid;
	%ChPosY_rel = mFloor(%CHPosY_init / %relGrid) * %relGrid;
	%MinBrZSnap = $PTGm.brTer_Zsize; // $PTGm.brTer_FillXYZSize + $PTGm.brTer_Zsize;
	%MinBrZSnapFI = $PTGm.brFltIslds_Zsize; //%BrXYhSize_FI = $PTGm.brFltIslds_XYsize / 2;
	%BrXYhSize = $PTGm.brTer_XYsize / 2;
	%relGridH = %relGrid / 2;
	%relGridSnap = mClamp(%relGrid,mClamp($PTGm.chSize,16,256),256);
	//%ter_itcCsnap = $PTGm.ter_itrB_XY;//getMin($PTGm.ter_itrB_XY,%relGridSnap);
	

	//Use chunk size as reference since itr A & B must be >= the chunk size, and it can be snapped to all various grid sizes below
	for(%tempY = 0; %tempY <= %relGridSnap; %tempY += mClamp($PTGm.chSize,16,256)) //%BrXYhSize or 0???
	{
		for(%tempX = 0; %tempX <= %relGridSnap; %tempX += mClamp($PTGm.chSize,16,256)) //%BrXYhSize or 0???
		{
			%CHPosXb = %ChPosX_rel + %tempX;
			%CHPosYb = %ChPosY_rel + %tempY;

			if(%tempX < (%relGridH + %xMax) && (%tempX + mClamp($PTGm.chSize,16,256)) >= (%relGridH + %xMin)) //was this what was wrong with paths? max should be before min!
			{
				if(%tempY < (%relGridH + %yMax) && (%tempY + mClamp($PTGm.chSize,16,256)) >= (%relGridH + %yMin))
				{
					////////////////////////////////////////////////////////////////////////////////////////////////////
					//Terrain
									
					for(%tempYb = 0; %tempYb <= mClamp($PTGm.chSize,16,256); %tempYb += $PTGm.ter_itrC_XY) //%BrXYhSize or 0???
					{
						for(%tempXb = 0; %tempXb <= mClamp($PTGm.chSize,16,256); %tempXb += $PTGm.ter_itrC_XY) //%BrXYhSize or 0???
						{
							%CHPosXc = %CHPosXb + %tempXb;
							%CHPosYc = %CHPosYb + %tempYb;

							if(%tempXb < (%relGridH + %xMax) && (%tempXb + $PTGm.ter_itrC_XY) >= (%relGridH + %xMin)) //was this what was wrong with paths? max should be before min!
							{
								if(%tempYb < (%relGridH + %yMax) && (%tempYb + $PTGm.ter_itrC_XY) >= (%relGridH + %yMin))
								{
									////////////////////////////////////////////////////////////////////////////////////////////////////
									//Default Terrain
									
									
									if($PTGm.terType $= "FlatLands")
										%TerH = getMax(mFloor(PTG_Noise_EdgeFallOff(%ChPosXc,%ChPosYc,$PTGm.terHLevel) / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize,$PTGm.brTer_Zsize);
									else
									{
										%ChPxRelItrA = (mFloor(%CHPosXc / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrB_XY;
										%ChPyRelItrA = (mFloor(%ChPosYc / $PTGm.ter_itrA_XY)) * $PTGm.ter_itrB_XY;
										%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.terOff_X,%CHPyRelItrA+$PTGm.terOff_Y,$PTGm.ter_itrB_XY,13333227,49979687,13313,15551);
										
										%HVnBioStr = PTG_Noise_BiomeTerHMod(%CHPosXc,%ChPosYc,%ChHVItrA);
										%ChHVItrA = getWords(%HVnBioStr,0,3);
										
										%ChPxRelItrB = (mFloor(%CHPosXc / $PTGm.ter_itrB_XY)) * $PTGm.ter_itrB_XY; //RelItrB == ActItrB???
										%CHPyRelItrB = (mFloor(%ChPosYc / $PTGm.ter_itrB_XY)) * $PTGm.ter_itrB_XY;
										%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.terOff_X,%CHPyRelItrB+$PTGm.terOff_Y,$PTGm.ter_itrB_XY,13333227,49979687,13313,15551);
										
										//itr c necessary?
										%ChPxRelItrC = (mFloor((%CHPosXc + %tempXb) / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrB_XY;
										%ChPyRelItrC = (mFloor((%ChPosYc + %tempYb) / $PTGm.ter_itrC_XY)) * $PTGm.ter_itrB_XY;
										%ChHVItrC = PTG_RandNumGen_Chunk(%CHPxRelItrC+$PTGm.terOff_X,%CHPyRelItrC+$PTGm.terOff_Y,$PTGm.ter_itrB_XY,13333227,49979687,13313,15551);

										%Co = ((%ChPosYc) % $PTGm.ter_itrA_XY) / $PTGm.ter_itrA_XY;
										%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
										%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
										%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

										%Co = ((%ChPosYc) % $PTGm.ter_itrB_XY) / $PTGm.ter_itrB_XY;
										%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
										%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
										%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);

										%Co = ((%ChPosYc) % $PTGm.ter_itrC_XY) / $PTGm.ter_itrC_XY;
										%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
										%ItrCColL = getWord(%ChHVItrC,0) - ((getWord(%ChHVItrC,0) - getWord(%ChHVItrC,1)) * %Sm);
										%ItrCColR = getWord(%ChHVItrC,2) - ((getWord(%ChHVItrC,2) - getWord(%ChHVItrC,3)) * %Sm);

										%Co = ((%CHPosXc) % $PTGm.ter_itrA_XY) / $PTGm.ter_itrA_XY;
										%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
										%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
										
										%Co = ((%CHPosXc) % $PTGm.ter_itrB_XY) / $PTGm.ter_itrB_XY;
										%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
										%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

										%Co = ((%CHPosXc) % $PTGm.ter_itrC_XY) / $PTGm.ter_itrC_XY;
										%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
										%ItrCRow = %ItrCColL - ((%ItrCColL - %ItrCColR) * %Sm);

										%relZ = (%ItrARow*$PTGm.ter_itrA_Z) + (%ItrBRow*$PTGm.ter_itrB_Z) + (%ItrCRow*$PTGm.ter_itrC_Z) + $PTGm.terHLevel;

										//Lakes / Connect Lakes option
										if($PTGm.enabCnctLakes && (%relZ - $PTGm.terHLevel) < ($PTGm.lakesHLevel+$PTGm.cnctLakesStrt) && $PTGm.terType !$= "SkyLands")
											%relZ -= (($PTGm.lakesHLevel+$PTGm.cnctLakesStrt) - %relZ);
										
										//Edge-FallOff
										%relZ = PTG_Noise_EdgeFallOff(%CHPosXc,%ChPosYc,%relZ); //necessary? since only calculating height values edges?

										%TerH = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
										%TerH = getMax(%TerH,%MinBrZSnap);
									}
									
									
									////////////////////////////////////////////////////////////////////////////////////////////////////
									//Mountains
					
										
									if(!$PTGm.enabMntns)
									{
										%mntnsFail = true;
										
										//if((%TerH < %BrH_build && %TerH > 0) || %BrH_build == 0)
										//	%BrH_build = %TerH;
									
										//Only check biomes if mountains are disabled or don't generated for this location
										%HVnBioStr = PTG_Noise_BiomeTerHMod(%ChPosXc,%ChPosYb,"");
										//%ChHVItrA = getWords(%HVnBioStr,0,3);
										%bioAPass = getWord(%HVnBioStr,4);
										%bioAFail = getWord(%HVnBioStr,5);
										%bioBPass = getWord(%HVnBioStr,6);
										%bioBFail = getWord(%HVnBioStr,7);
										%bioCPass = getWord(%HVnBioStr,8);
										%bioCFail = getWord(%HVnBioStr,9);
										
										if(!$PTGm.BldLdUseMaxHTer)
										{
											if((%TerH < %BrH_build && %TerH > 0) || %BrH_build == 0)
												%BrH_build = %TerH;
										}
										else
										{
											if((%TerH > %BrH_build && %TerH > 0) || %BrH_build == 0)
												%BrH_build = %TerH;
										}
									}
									else
									{
										%ChPxRelItrA = (mFloor((%ChPosXc) / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrB_XY;
										%ChPyRelItrA = (mFloor((%ChPosYc) / $PTGm.mntn_itrA_XY)) * $PTGm.mntn_itrB_XY;
										%ChHVItrA = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.terOff_X,%CHPyRelItrA+$PTGm.terOff_Y,$PTGm.mntn_itrB_XY,27177289,71234567,14549,54163);
										
										%ChPosStr = %ChPyActItrA SPC %ChPyActItrA+$PTGm.mntn_itrA_XY;
										%ChHVItrA = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPosStr,$PTGm.Mntn_YStart,0);
	
										%ChPxRelItrB = (mFloor((%ChPosXc) / $PTGm.mntn_itrB_XY)) * $PTGm.mntn_itrB_XY;
										%CHPyRelItrB = (mFloor((%ChPosYc) / $PTGm.mntn_itrB_XY)) * $PTGm.mntn_itrB_XY;
										%ChHVItrB = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.terOff_X,%CHPyRelItrB+$PTGm.terOff_Y,$PTGm.mntn_itrB_XY,27177289,71234567,14549,54163);

										%ChPosStr = %CHPyRelItrB SPC %CHPyRelItrB+$PTGm.mntn_itrB_XY;
										%ChHVItrB = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB,%ChPosStr,$PTGm.Mntn_YStart,0);
										
										%Co = (((%ChPosYc) % $PTGm.mntn_itrA_XY)) / $PTGm.mntn_itrA_XY;
										%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
										%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
										%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

										%Co = (((%ChPosYc) % $PTGm.mntn_itrB_XY)) / $PTGm.mntn_itrB_XY;
										%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
										%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
										%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
										
										%Co = (((%ChPosXc) % $PTGm.mntn_itrA_XY)) / $PTGm.mntn_itrA_XY;
										%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
										%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
										
										%Co = (((%ChPosXc) % $PTGm.mntn_itrB_XY)) / $PTGm.mntn_itrB_XY;
										%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
										%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);
									
										//Mountains (offsets initial terrain height)
										%relZ = (%ItrARow * $PTGm.mntn_itrA_Z) + (%ItrBRow * $PTGm.mntn_itrB_Z);
										%MntnH = getMax(%relZ,%MinBrZSnap);

										if(%MntnH > $PTGm.mntnsStrtSecZ)
										{
											%TerHb = mFloor(%TerH + ((%MntnH - $PTGm.mntnsStrtSecZ) * $PTGm.mntnsZMult));
											%relBrH = ($PTGm.brTer_Zsize * $PTGm.mntnsZSnap);
											
											//Edge-FallOff
											%TerHb = PTG_Noise_EdgeFallOff(%ChPosXc,%ChPosYc,%TerHb);
											%TerHb = mFloor(%TerHb / %relBrH) * %relBrH;
											
											if(%TerHb < %TerH)
												%TerHb = %TerH;
											%TerH = %TerHb;
											
											
											if(%TerHb > $PTGm.lakesHLevel && %TerH > $PTGm.mntnsStrtSecZ) //take edge falloff into account here for mntns
											{
												%mntnsPass = true; //%bioDefFail = true;
												%relMntnPass = true;
												
												//adjust heighest / lowest pos if mountains are above water level
												if(!$PTGm.BldLdUseMaxHTer)
												{
													if((%TerHb < %BrH_build && %TerHb > 0) || %BrH_build == 0)
														%BrH_build = %TerHb;
												}
												else
												{
													if((%TerHb > %BrH_build && %TerHb > 0) || %BrH_build == 0)
														%BrH_build = %TerHb;
												}
											}
											else
											{
												%mntnsFail = true;

												if(!$PTGm.BldLdUseMaxHTer)
												{
													if((%TerH < %BrH_build && %TerH > 0) || %BrH_build == 0)
														%BrH_build = %TerH;
												}
												else
												{
													if((%TerH > %BrH_build && %TerH > 0) || %BrH_build == 0)
														%BrH_build = %TerH;
												}
											}
										}
										else
										{
											%mntnsFail = true;

											if(!$PTGm.BldLdUseMaxHTer)
											{
												if((%TerH < %BrH_build && %TerH > 0) || %BrH_build == 0)
													%BrH_build = %TerH;
											}
											else
											{
												if((%TerH > %BrH_build && %TerH > 0) || %BrH_build == 0)
													%BrH_build = %TerH;
											}
										}
									}
									
									
									////////////////////////////////////////////////////////////////////////////////////////////////////
									//Shore, Water Level, SubMarine, Biomes
									
									
									//SubMarine (Water level is independent of sand level / shore)
									if(%TerH < $PTGm.lakesHLevel) //zMax??? (to prevent top of build from sticking out above water for subM)
									{
										//Default Terrain, Shore and Submarine
										%bioDefFail = true; //mountains still gen?
										%bioShoreFail = true;
										%bioSubMPass = true; //if((%BrH_build + %zMax) < $PTGm.lakesHLevel) moved to other func
										
										%bioAFail = true;
										%bioBFail = true;
										%bioCFail = true;
										
										%watPass = true; //%mntnsFail = true;
									}
									else
									{
										//Default Terrain
										if(%TerH > $PTGm.sandHLevel)
										{
											//Biome checks (if terrain check passes and if mntns don't gen at this location)
											if(!%relMntnPass || !%relSkyLPass)
											{
												%bioDefPass = true;
												
												%HVnBioStr = PTG_Noise_BiomeTerHMod(%ChPosXb,%ChPosYb,""); //%ChHVItrA = getWords(%HVnBioStr,0,3);
												%bioAPass = getWord(%HVnBioStr,4);
												%bioAFail = getWord(%HVnBioStr,5);
												%bioBPass = getWord(%HVnBioStr,6);
												%bioBFail = getWord(%HVnBioStr,7);
												%bioCPass = getWord(%HVnBioStr,8);
												%bioCFail = getWord(%HVnBioStr,9);
											}
											else
											{
												%bioDefFail = true;

												%bioAFail = true;
												%bioBFail = true;
												%bioCFail = true;
											}
											
											%bioShoreFail = true;
										}
										
										//Shore
										else
										{
											%bioDefFail = true;
											
											if(!%relMntnPass)// || !%relSkyLPass)
												%bioShorePass = true;
											else
											{
												%bioShoreFail = true;
												
												//biomes
											}
											
											%bioAFail = true;
											%bioBFail = true;
											%bioCFail = true;
										}

										//Submarine and Water Level
										%bioSubMFail = true;
										%watFail = true;
									}
									
									////////////////////////////////////////////////////////////////////////////////////////////////////
									//Skylands
					
									if($PTGm.terType $= "SkyLands")
									{
										%BrH_act = %TerH - $PTGm.terHLevel;
										
										if(%BrH_act > $PTGm.skyLndsSecZ)
										{
											%SkyLandPass = true;
											
											%ChPxActItrA = (mFloor(%CHPosXc / $PTGm.skyLnds_itrA_XY)) * $PTGm.skyLnds_itrA_XY;
											%ChPyActItrA = (mFloor(%CHPosYc / $PTGm.skyLnds_itrA_XY)) * $PTGm.skyLnds_itrA_XY;
											%ChHVItrA = PTG_RandNumGen_Chunk(%ChPxActItrA+$PTGm.skyLandsOff_X,%ChPyActItrA+$PTGm.skyLandsOff_Y,$PTGm.skyLnds_itrA_XY,13466917,7649689,14107,79561);

											%Co = ((%CHPosYc-%ChPyActItrA)) / $PTGm.skyLnds_itrA_XY; //% ?
											%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
											%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
											%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);
											
											%Co = ((%CHPosXc-%ChPxActItrA)) / $PTGm.skyLnds_itrA_XY; //% ?
											%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
											%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
											
											%relZ = (%ItrARow*$PTGm.skyLnds_itrA_Z);
											%tempZ = mFloor(%relZ / $PTGm.brTer_Zsize) * $PTGm.brTer_Zsize;
											%BrH_SL = getMax(%tempZ,%MinBrZSnap);

											//%TerHc = getMax(%tempZ,%MinBrZSnap);
											//%relX = mFloor((%tempX+%tempXb) / mClamp($PTGm.chSize,16,256)) * mClamp($PTGm.chSize,16,256);
											//%relY = mFloor((%tempY+%tempYb) / mClamp($PTGm.chSize,16,256)) * mClamp($PTGm.chSize,16,256);
											//%xAdj = (%tempX+%tempXb) - %relX;
											//%YAdj = (%tempY+%tempYb) - %relY;
											//%BrH_act = $StrArrayHV_Mountains[%xAdj+%BrXYhSize,%YAdj+%BrXYhSize] - $PTGm.terHLevel;

											%TerHc = ($PTGm.terHLevel + (%BrH_act - $PTGm.skyLndsSecZ)) + %BrH_SL;
											
											if(!$PTGm.BldLdUseMaxHTer)
											{
												if((%TerHc < %BrH_skld_build && %TerHc > 0) || %BrH_skld_build == 0)
													%BrH_skld_build = %TerHc;
											}
											else
											{
												if((%TerHc > %BrH_skld_build && %TerHc > 0) || %BrH_skld_build == 0)
													%BrH_skld_build = %TerHc;
											}
										}
										else
											%SkyLandFail = true;
									}
									else
										%SkyLandFail = true;
								}
							}
						}
					}

					////////////////////////////////////////////////////////////////////////////////////////////////////
					//Floating Islands

					if(!$PTGm.enabFltIslds)
					{
						%fltIsldsAFail = true;
						%fltIsldsBFail = true;
						%fltIsldsAPass = false;
						%fltIsldsBPass = false;
					}
					else
					{
						//Both iteration A and B for floating islands should be at least >= the chunk size, thus included under "for" loops above
						
						%ChPxRelItrA = (mFloor(%CHPosXb / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrB_XY;
						%ChPyRelItrA = (mFloor(%CHPosYb / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrB_XY;
						%ChHVItrA_A = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.fltIsldsOff_X,%CHPyRelItrA+$PTGm.fltIsldsOff_Y,$PTGm.fltIslds_itrB_XY,55555333,87889091,22273,33773);
						%ChHVItrA_B = PTG_RandNumGen_Chunk(%CHPxRelItrA+$PTGm.fltIsldsBOff_X,%CHPyRelItrA+$PTGm.fltIsldsBOff_Y,$PTGm.fltIslds_itrB_XY,24710753,60000607,22273,33773);
						%ChPxActItrA = (mFloor(%CHPosXb / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrA_XY;
						%ChPyActItrA = (mFloor(%CHPosYb / $PTGm.fltIslds_itrA_XY)) * $PTGm.fltIslds_itrA_XY;
						
						%ChHVItrA_A = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA_A,%ChPyActItrA SPC %ChPyActItrA+$PTGm.fltIslds_itrA_XY,$PTGm.FltIsld_YStart,0);
						%ChHVItrA_B = PTG_Noise_PsuedoEquatorCheck(%ChHVItrA_B,%ChPyActItrA SPC %ChPyActItrA+$PTGm.fltIslds_itrA_XY,$PTGm.FltIsld_YStart,0);
						
						%CHPyRelItrB = (mFloor(%CHPosYb / $PTGm.fltIslds_itrB_XY)) * $PTGm.fltIslds_itrB_XY;
						%ChPxRelItrB = (mFloor(%CHPosXb / $PTGm.fltIslds_itrB_XY)) * $PTGm.fltIslds_itrB_XY;
						%ChHVItrB_A = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.fltIsldsOff_X,%CHPyRelItrB+$PTGm.fltIsldsOff_Y,$PTGm.fltIslds_itrB_XY,55555333,87889091,22273,33773);
						%ChHVItrB_B = PTG_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.fltIsldsBOff_X,%CHPyRelItrB+$PTGm.fltIsldsBOff_Y,$PTGm.fltIslds_itrB_XY,24710753,60000607,22273,33773);
						
						%ChHVItrB_A = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB_A,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.fltIslds_itrB_XY,$PTGm.FltIsld_YStart,0);
						%ChHVItrB_B = PTG_Noise_PsuedoEquatorCheck(%ChHVItrB_B,%CHPyRelItrB SPC %CHPyRelItrB+$PTGm.fltIslds_itrB_XY,$PTGm.FltIsld_YStart,0);

						//////////////////////////////////////////////////
						//Layer A
						
						%Co = (%CHPosYb - %ChPyActItrA) / $PTGm.fltIslds_itrA_XY;
						%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
						%ItrAColL = getWord(%ChHVItrA_A,0) - ((getWord(%ChHVItrA_A,0) - getWord(%ChHVItrA_A,1)) * %Sm);
						%ItrAColR = getWord(%ChHVItrA_A,2) - ((getWord(%ChHVItrA_A,2) - getWord(%ChHVItrA_A,3)) * %Sm);

						%Co = (%CHPosYb - %CHPyRelItrB) / $PTGm.fltIslds_itrB_XY;
						%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
						%ItrBColL = getWord(%ChHVItrB_A,0) - ((getWord(%ChHVItrB_A,0) - getWord(%ChHVItrB_A,1)) * %Sm);
						%ItrBColR = getWord(%ChHVItrB_A,2) - ((getWord(%ChHVItrB_A,2) - getWord(%ChHVItrB_A,3)) * %Sm);
						
						%Co = (%CHPosXb - %ChPxActItrA) / $PTGm.fltIslds_itrA_XY;
						%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
						%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) *%Sm);
						
						%Co = (%CHPosXb - %ChPxRelItrB) / $PTGm.fltIslds_itrB_XY;
						%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
						%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

						%relZ = (%ItrARow * $PTGm.fltIslds_itrA_Z) + (%ItrBRow * $PTGm.fltIslds_itrB_Z);
						
						//Edge-FallOff
						%relZ = PTG_Noise_EdgeFallOff(%CHPosXb,%ChPosYb,%relZ);
						%tempZ = mFloor(%relZ / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize;

						if(%tempZ > $PTGm.fltIsldsSecZ)// && ((%tempZ - $PTGm.fltIsldsSecZ) > 16))
						{
							//echo("A:" @ %tempZ SPC $PTGm.fltIsldsSecZ);
							//%FIaH = getMax(%tempZ,%MinBrZSnap_FI);
							%FIaH = $PTGm.fltIsldsAHLevel + (mFloor((getMax(%tempZ,%MinBrZSnapFI) - $PTGm.fltIsldsSecZ) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
							%fltIsldsAPass = true;
							
							if(!$PTGm.BldLdUseMaxHTer)
							{
								if((%FIaH < %BrH_FIa_build && %FIaH > 0) || %BrH_FIa_build == 0)
									%BrH_FIa_build = %FIaH;
							}
							else
							{
								if((%FIaH > %BrH_FIa_build && %FIaH > 0) || %BrH_FIa_build == 0)
									%BrH_FIa_build = %FIaH;
							}
						}
						else
						{
							%fltIsldsAFail = true;
							%FISecCutA = true;
						}
						
						//////////////////////////////////////////////////
						//Layer B
						
						%Co = (%CHPosYb - %ChPyActItrA) / $PTGm.fltIslds_itrA_XY;
						%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
						%ItrAColL = getWord(%ChHVItrA_B,0) - ((getWord(%ChHVItrA_B,0) - getWord(%ChHVItrA_B,1)) * %Sm);
						%ItrAColR = getWord(%ChHVItrA_B,2) - ((getWord(%ChHVItrA_B,2) - getWord(%ChHVItrA_B,3)) * %Sm);

						%Co = (%CHPosYb - %CHPyRelItrB) / $PTGm.fltIslds_itrB_XY;
						%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
						%ItrBColL = getWord(%ChHVItrB_B,0) - ((getWord(%ChHVItrB_B,0) - getWord(%ChHVItrB_B,1)) * %Sm);
						%ItrBColR = getWord(%ChHVItrB_B,2) - ((getWord(%ChHVItrB_B,2) - getWord(%ChHVItrB_B,3)) * %Sm);
						
						%Co = (%CHPosXb - %ChPxActItrA) / $PTGm.fltIslds_itrA_XY;
						%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
						%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) *%Sm);
						
						%Co = (%CHPosXb - %ChPxRelItrB) / $PTGm.fltIslds_itrB_XY;
						%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
						%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

						%relZB = (%ItrARow * $PTGm.fltIslds_itrA_Z) + (%ItrBRow * $PTGm.fltIslds_itrB_Z);
						
						//Edge-FallOff
						%relZB = PTG_Noise_EdgeFallOff(%CHPosXb,%ChPosYb,%relZB);
						%tempZB = mFloor(%relZB / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize;
						
						if(%tempZB > $PTGm.fltIsldsSecZ)// && ((%tempZB - $PTGm.fltIsldsSecZ) > 16))
						{
							//echo("B:" @ %tempZB SPC $PTGm.fltIsldsSecZ);
							//%FIbH = getMax(%tempZB,%MinBrZSnap_FI);
							%FIbH = $PTGm.fltIsldsBHLevel + (mFloor((getMax(%tempZB,%MinBrZSnapFI) - $PTGm.fltIsldsSecZ) / $PTGm.brFltIslds_Zsize) * $PTGm.brFltIslds_Zsize);
							%fltIsldsBPass = true;
							
							if(!$PTGm.BldLdUseMaxHTer)
							{
								if((%FIbH < %BrH_FIb_build && %FIbH > 0) || %BrH_FIb_build == 0)
									%BrH_FIb_build = %FIbH;
							}
							else
							{
								if((%FIbH > %BrH_FIb_build && %FIbH > 0) || %BrH_FIb_build == 0)
									%BrH_FIb_build = %FIbH;
							}
						}
						else
						{
							%fltIsldsBFail = true;
							%FISecCutB = true;
						}
					}
				}
			}
		}
	}
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////

	
	%ter = %BrH_build SPC %bioDefPass SPC %bioDefFail;
	%shore = %bioShorePass SPC %bioShoreFail;
	%subm = %bioSubMPass SPC %bioSubMFail;
	%biomes = %bioAPass SPC %bioAFail SPC %bioBPass SPC %bioBFail SPC %bioCPass SPC %bioCFail;
	%wtr = %watPass SPC %watFail;
	%mntns = %mntnsPass SPC %mntnsFail;
	%fltilds = %fltIsldsAPass SPC %fltIsldsAFail SPC %fltIsldsBPass SPC %fltIsldsBFail SPC %BrH_FIa_build SPC %BrH_FIb_build SPC %FISecCutA SPC %FISecCutB;
	%rtnStr = %ter SPC %shore SPC %subm SPC %biomes SPC %wtr SPC %mntns SPC %fltilds SPC %BrH_skld_build SPC %SkyLandPass SPC %SkyLandFail;

	return %rtnStr;
}