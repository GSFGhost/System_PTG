function PTG_GUI_ErrorCheck()
{
	%ChSize = getWord("16 32 64 128 256",mClamp(PTG_Cmplx_PopupChSize.getSelected(),0,4));
	%BrXYSize = getMax(PTG_Cmplx_BmpTerBr.BrickID.brickSizeX * 0.5,1);
	%startPosX = mFloor(PTG_Cmplx_EdtGridXStart.getValue() / %ChSize) * %ChSize;
	%startPosY = mFloor(PTG_Cmplx_EdtGridYStart.getValue() / %ChSize) * %ChSize;
	%endPosX = mFloor(PTG_Cmplx_EdtGridXEnd.getValue() / %ChSize) * %ChSize;
	%endPosY = mFloor(PTG_Cmplx_EdtGridYEnd.getValue() / %ChSize) * %ChSize;
		
	//Grid Size Check (Finite Terrain)
	//if(%so.genType $= "Finite")
	//{
		if(%startPosX >= %endPosX)
		{
			CLIENTCMDPTG_ReceiveMsg("Failed","PTG Preview ERROR: Incorrect Grid Size","The Grid start position for the X-Axis must be < the grid end position for X.");
			%fail = true;
		}
		if(%startPosY >= %endPosY)
		{
			CLIENTCMDPTG_ReceiveMsg("Failed","PTG Preview ERROR: Incorrect Grid Size","The Grid start position for the Y-Axis must be < the grid end position for Y.");
			%fail = true;
		}
	//}
	
	//Datablocks Check
	if(!%fail)
	{
		if(!isObject(PTG_Cmplx_BmpTerBr.BrickID))
		{
			if(PTG_Cmplx_BmpTerBr.ModTer)
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG Preview ERROR: Invalid Brick Datablock","Brick datablock for \"Terrain\" doesn't exist! Either the 4x ModTer pack or both the Basic and Inv ModTer packs are disabled.");
			else
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG Preview ERROR: Invalid Brick Datablock","Brick datablock for \"Terrain\" doesn't exist!");
			
			%fail = true;
		}
		if(PTG_Cmplx_ChkEnabClouds.getValue() && !isObject(PTG_Cmplx_BmpCloudBr.BrickID))
		{
			if(PTG_Cmplx_BmpCloudBr.ModTer)
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG Preview ERROR: Invalid Brick Datablock","Brick datablock for \"Clouds\" doesn't exist! Either the 4x ModTer pack or both the Basic and Inv ModTer packs are disabled.");
			else
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG Preview ERROR: Invalid Brick Datablock","Brick datablock for \"Clouds\" doesn't exist!");
			
			%fail = true;
		}
		if(PTG_Cmplx_ChkEnabFltIslds.getValue() && !isObject(PTG_Cmplx_BmpFltIsldsBr.BrickID))
		{
			if(PTG_Cmplx_BmpFltIsldsBr.ModTer)
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG Preview ERROR: Invalid Brick Datablock","Brick datablock for \"Floating Islands\" doesn't exist! Either the 4x ModTer pack or both the Basic and Inv ModTer packs are disabled.");
			else
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG Preview ERROR: Invalid Brick Datablock","Brick datablock for \"Floating Islands\" doesn't exist!");
			
			%fail = true;
		}
	}
	
	//Biome (Relative to Terrain) Scales Check
	if(!%fail)
	{
		if(PTG_Cmplx_ChkEnabCustABio.getValue() && PTG_Cmplx_EdtNosScaleCustAXY.getValue() < %ChSize) //custom biome A
		{
			CLIENTCMDPTG_ReceiveMsg("Failed","PTG Preview ERROR: Incorrect Biome XY Scale","The biome XY noise scale for \"Custom Biome A\" must be >= the chunk size!");
			%fail = true;
		}
		if(PTG_Cmplx_ChkEnabCustBBio.getValue() && PTG_Cmplx_EdtNosScaleCustBXY.getValue() < %ChSize) //custom biome B
		{
			CLIENTCMDPTG_ReceiveMsg("Failed","PTG Preview ERROR: Incorrect Biome XY Scale","The biome XY noise scale for \"Custom Biome B\" must be >= the chunk size!");
			%fail = true;
		}
		if(PTG_Cmplx_ChkEnabCustCBio.getValue() && PTG_Cmplx_EdtNosScaleCustCXY.getValue() < %ChSize) //custom biome C
		{
			CLIENTCMDPTG_ReceiveMsg("Failed","PTG Preview ERROR: Incorrect Biome XY Scale","The biome XY noise scale for \"Custom Biome C\" must be >= the chunk size!");
			%fail = true;
		}
	}
	
	//Noise Scales Check
	if(!%fail)
	{
		if(PTG_Cmplx_EdtNosScaleTerAXY.getValue() < PTG_Cmplx_EdtNosScaleTerBXY.getValue() || PTG_Cmplx_EdtNosScaleTerAXY.getValue() < %ChSize || PTG_Cmplx_EdtNosScaleTerBXY.getValue() < %ChSize || PTG_Cmplx_EdtNosScaleTerCXY.getValue() > %ChSize) //terrain
		{
			CLIENTCMDPTG_ReceiveMsg("Failed","PTG Preview ERROR: Incorrect Noise Scales","XY noise scales for \"Terrain\" must follow the following rule: <color:ff0000>Iteration A >= Iteration B >= Chunk Size >= Iteration C<color:000000>.<br><br>Example: 256 >= 64 >= 32 >= 16");
			%fail = true;
		}
		if(PTG_Cmplx_ChkEnabMntns.getValue() && (PTG_Cmplx_EdtNosScaleMntnAXY.getValue() < PTG_Cmplx_EdtNosScaleMntnBXY.getValue() || PTG_Cmplx_EdtNosScaleMntnAXY.getValue() < %ChSize || PTG_Cmplx_EdtNosScaleMntnBXY.getValue() < %ChSize)) //mountains
		{
			CLIENTCMDPTG_ReceiveMsg("Failed","PTG Preview ERROR: Incorrect Noise Scales","XY noise scales for \"Mountains\" must follow the following rule: <color:ff0000>Iteration A >= Iteration B >= Chunk Size<color:000000>.<br><br>Example: 256 >= 64 >= 32");
			%fail = true;
		}
		if(PTG_Cmplx_ChkEnabCaves.getValue())
		{
			if(PTG_Cmplx_EdtNosScaleCaveAXY.getValue() < PTG_Cmplx_EdtNosScaleCaveBXY.getValue() || PTG_Cmplx_EdtNosScaleCaveAXY.getValue() < %ChSize || PTG_Cmplx_EdtNosScaleCaveBXY.getValue() < %ChSize || PTG_Cmplx_EdtNosScaleCaveCXY.getValue() > %ChSize) //caves
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG Preview ERROR: Incorrect Noise Scales","XY noise scales for \"Caves\" must follow the following rule: <color:ff0000>Iteration A >= Iteration B >= Chunk Size >= Iteration C<color:000000>.<br><br>Example: 128 >= 64 >= 32 >= 32");
				%fail = true;
			}
			if(PTG_Cmplx_EdtNosScaleCaveHXY.getValue() < %ChSize) //caves height mod
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG Preview ERROR: Incorrect Noise Scales","XY noise scales for \"Caves Height Mod\" must follow the following rule: <color:ff0000>Iteration A >= Chunk Size<color:000000>.<br><br>Example: 256 >= 32");
				%fail = true;
			}
		}
		if((PTG_Cmplx_ChkEnabCustABio.getValue() && PTG_Cmplx_EdtNosScaleCustAXY.getValue() < PTG_Cmplx_EdtNosScaleTerAXY.getValue()) || (PTG_Cmplx_ChkEnabCustBBio.getValue() && PTG_Cmplx_EdtNosScaleCustBXY.getValue() < PTG_Cmplx_EdtNosScaleTerAXY.getValue()) || (PTG_Cmplx_ChkEnabCustCBio.getValue() && PTG_Cmplx_EdtNosScaleCustCXY.getValue() < PTG_Cmplx_EdtNosScaleTerAXY.getValue())) //custom biomes
		{
			CLIENTCMDPTG_ReceiveMsg("Failed","PTG Preview ERROR: Incorrect Noise Scales","XY noise scales for \"Custom Biomes\" must follow the following rule: <color:ff0000>Biome Iteration >= Terrain Iteration A<color:000000>.<br><br>Example: 512 >= 256");
			%fail = true;
		}
		if(PTG_Cmplx_PopUpTerType.getValue() $= "Skylands" && PTG_Cmplx_EdtNosScaleSkylandXY.getValue() < %ChSize) //skylands terrain height mod
		{
			CLIENTCMDPTG_ReceiveMsg("Failed","PTG Preview ERROR: Incorrect Noise Scales","XY noise scales for \"Skylands Height Mod\" must follow the following rule: <color:ff0000>Iteration A >= Chunk Size<color:000000>.<br><br>Example: 128 >= 32");
			%fail = true;
		}
		if(PTG_Cmplx_ChkEnabClouds.getValue() && (PTG_Cmplx_EdtNosScaleCloudAXY.getValue() < PTG_Cmplx_EdtNosScaleCloudBXY.getValue() || PTG_Cmplx_EdtNosScaleCloudAXY.getValue() < %ChSize || PTG_Cmplx_EdtNosScaleCloudBXY.getValue() < %ChSize)) //clouds
		{
			CLIENTCMDPTG_ReceiveMsg("Failed","PTG Preview ERROR: Incorrect Noise Scales","XY noise scales for \"Clouds\" must follow the following rule: <color:ff0000>Iteration A >= Iteration B >= Chunk Size<color:000000>.<br><br>Example: 128 >= 32 >= 32");
			%fail = true;
		}
		if(PTG_Cmplx_ChkEnabFltIslds.getValue() && (PTG_Cmplx_EdtNosScaleFltIsldAXY.getValue() < PTG_Cmplx_EdtNosScaleFltIsldBXY.getValue() || PTG_Cmplx_EdtNosScaleFltIsldAXY.getValue() < %ChSize || PTG_Cmplx_EdtNosScaleFltIsldBXY.getValue() < %ChSize)) //floating islands
		{
			CLIENTCMDPTG_ReceiveMsg("Failed","PTG Preview ERROR: Incorrect Noise Scales","XY noise scales for \"Floating Islands\" must follow the following rule: <color:ff0000>Iteration A >= Iteration B >= Chunk Size<color:000000>.<br><br>Example: 64 >= 32 >= 32");
			%fail = true;
		}		
	}
	
	if(%fail)
		return false;
	else
		return true;
}	


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_Noise_PsuedoEquatorCheck(%ChHValStr,%ChPosStr,%relOffset,%biomeMax)
{
	if(PTG_Cmplx_ChkEnabPseudoEqtr.getValue())
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


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_Noise_BiomeCheck(%CHPosX,%CHPosY,%ItrxyScale,%ItrZScale,%OffX,%OffY,%biomeOff)
{
	//This func only works if the biome scale is >= terrain scale, otherwise skips func

	%ItrxyScaleTer =  PTG_Cmplx_EdtNosScaleTerAXY.getValue(); //check
	%ChPxActItrTer = (mFloor(%CHPosX / %ItrxyScaleTer)) * %ItrxyScaleTer;
	%ChPyActItrTer = (mFloor(%CHPosY / %ItrxyScaleTer)) * %ItrxyScaleTer;
	
	%ChPxRelItr = (mFloor(%CHPosX / %ItrxyScale)) * %ItrxyScale;
	%ChPyRelItr = (mFloor(%CHPosY / %ItrxyScale)) * %ItrxyScale;
	%ChHVItr = PTG_GUI_RandNumGen_Chunk(%CHPxRelItr+%offX,%CHPyRelItr+%OffY,%ItrxyScale,12192683,83059231,10007,54973);
	
	%ChPosStr = %ChPyRelItr SPC %ChPyRelItr+%ItrxyScale;
	%ChHVItr = PTG_Noise_PsuedoEquatorCheck(%ChHVItr,%ChPosStr,%biomeOff,99999);

	%wrdCount = 0;
	
	//
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


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_Noise_EdgeFallOff(%PosX,%PosY,%PosZ)
{
	%newZ = %PosZ;
	%fallOffDist = PTG_Cmplx_EdtEdgeFallOffDist.getValue();
	
	if(PTG_Cmplx_ChkEdgeFallOff.getValue() && !PTG_Cmplx_EnabInfiniteTer.getValue())
	{
		if(%PosX < (PTG_Cmplx_EdtGridXStart.getValue() + %fallOffDist))
		{
			%Co = (%PosX - PTG_Cmplx_EdtGridXStart.getValue()) / %fallOffDist;
			%Co = mClampF(%Co,0.0,1.0);
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%newZ = %newZ * %Sm;
		}
		if(%PosX > (PTG_Cmplx_EdtGridXEnd.getValue() - %fallOffDist))
		{
			%Co = (%PosX - (PTG_Cmplx_EdtGridXEnd.getValue() - %fallOffDist)) / %fallOffDist;
			%Co = mAbs(mClampF(%Co,0.0,1.0) - 1); //invert
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%newZ = %newZ * %Sm;
		}
		if(%PosY < (PTG_Cmplx_EdtGridYStart.getValue() + %fallOffDist))
		{
			%Co = (%PosY - PTG_Cmplx_EdtGridYStart.getValue()) / %fallOffDist;
			%Co = mClampF(%Co,0.0,1.0);
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%newZ = %newZ * %Sm;
		}
		if(%PosY > (PTG_Cmplx_EdtGridYEnd.getValue() - %fallOffDist))
		{
			%Co = (%PosY - (PTG_Cmplx_EdtGridYEnd.getValue() - %fallOffDist)) / %fallOffDist;
			%Co = mAbs(mClampF(%Co,0.0,1.0) - 1); //invert
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%newZ = %newZ * %Sm;
		}
	}
	
	return %newZ;
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_GenGUIObj(%pos,%ext,%aux,%type)
{	
	switch$(%type)
	{
		case "Swatch":

			%sw = new GuiSwatchCtrl()
			{
				profile = "GuiDefaultProfile";
				horizSizing = "right";
				vertSizing = "bottom";
				position = %pos;
				extent = %ext;
				minExtent = "1 1";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				color = %aux;
			};
			
			return %sw;
		
		//////////////////////////////////////////////////
	
		case "BmpCtrl":
		
			%bmpCtrl = new GuiBitmapCtrl()
			{
				profile = "GuiDefaultProfile";
				horizSizing = "right";
				vertSizing = "bottom";
				position = %pos;
				extent = %ext;
				minExtent = "1 1";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				bitmap = %aux;
			};
			
			return %bmpCtrl;
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_Chunk_FigureColPri(%BrH_init,%BrH_cf,%BrH,%Type,%BrH_bioA,%BrH_bioB,%BrH_bioC)
{
	if(%Type !$= "Water" && %BrH <= PTG_Cmplx_SldrSandLevel.getValue()) //sand level could be == 0 while water level could be > 0 (or other way around)
	{
		if(%BrH < PTG_Cmplx_SldrWaterLevel.getValue())
			%col = PTG_Cmplx_SwBioSubMTerCol.color;
		else
		{
			%col = PTG_Cmplx_SwBioShoreTerCol.color;
			
			//If shore set to be same as custom biomes
			if(PTG_Cmplx_ChkShoreSameCustBiome.getValue())
			{
				if(PTG_Cmplx_ChkEnabCustABio.getValue() && (%BrH_bioA < PTG_Cmplx_EdtSectCustA.getValue())) //Custom Biome A
					%col = PTG_Cmplx_SwBioCustATerCol.color;
				if(PTG_Cmplx_ChkEnabCustBBio.getValue() && (%BrH_bioB < PTG_Cmplx_EdtSectCustB.getValue())) //Custom Biome B
					%col = PTG_Cmplx_SwBioCustBTerCol.color;
				if(PTG_Cmplx_ChkEnabCustCBio.getValue() && (%BrH_bioC < PTG_Cmplx_EdtSectCustC.getValue())) //Custom Biome C
					%col = PTG_Cmplx_SwBioCustCTerCol.color;
			}
		}
	}
		
	//////////////////////////////////////////////////
	
	else
	{
		switch$(%Type)
		{
			case "Terrain":

				if(%BrH < PTG_Cmplx_SldrWaterLevel.getValue()) //encase water level > sand level, %BrH > sand level
					%col = PTG_Cmplx_SwBioSubMTerCol.color;
				else
				{
					if(PTG_Cmplx_ChkEnabMntns.getValue() && %BrH_cf > %BrH_init)
					{
						if(%BrH_cf < PTG_Cmplx_SldrMntnSnowLevel.getValue() || !PTG_Cmplx_ChkEnabMntnSnow.getValue())
							%col = PTG_Cmplx_SwBioMntnRockCol.color;
						else
							%col = PTG_Cmplx_SwBioMntnSnowCol.color;
					}
					
					else
					{
						%col = PTG_Cmplx_SwTerCol.color;

						if(PTG_Cmplx_ChkEnabCustABio.getValue() && (%BrH_bioA < PTG_Cmplx_EdtSectCustA.getValue())) //Custom Biome A
							%col = PTG_Cmplx_SwBioCustATerCol.color;
						if(PTG_Cmplx_ChkEnabCustBBio.getValue() && (%BrH_bioB < PTG_Cmplx_EdtSectCustB.getValue())) //Custom Biome B
							%col = PTG_Cmplx_SwBioCustBTerCol.color;
						if(PTG_Cmplx_ChkEnabCustCBio.getValue() && (%BrH_bioC < PTG_Cmplx_EdtSectCustC.getValue())) //Custom Biome C
							%col = PTG_Cmplx_SwBioCustCTerCol.color;
					}
				}
			
			//////////////////////////////////////////////////
			
			case "Dirt": //Never used?
			
				if(PTG_Cmplx_ChkDirtSameBioTer.getValue()) //if dirt layer is set to be same color as terrain / biomes
				{
					if(%BrH_cf > %BrH_init)
					{
						if(%BrH_cf < PTG_Cmplx_SldrMntnSnowLevel.getValue() || !PTG_Cmplx_ChkEnabMntnSnow.getValue())
							%col = PTG_Cmplx_SwBioMntnRockCol.color;
						else
							%col = PTG_Cmplx_SwBioMntnSnowCol.color;
					}
					
					else
					{
						%col = PTG_Cmplx_SwTerCol.color;
				
						if(PTG_Cmplx_ChkEnabCustABio.getValue() && (%BrH_bioA < PTG_Cmplx_EdtSectCustA.getValue())) //Custom Biome A
							%col = PTG_Cmplx_SwBioCustATerCol.color;
						if(PTG_Cmplx_ChkEnabCustBBio.getValue() && (%BrH_bioB < PTG_Cmplx_EdtSectCustB.getValue())) //Custom Biome B
							%col = PTG_Cmplx_SwBioCustBTerCol.color;
						if(PTG_Cmplx_ChkEnabCustCBio.getValue() && (%BrH_bioC < PTG_Cmplx_EdtSectCustC.getValue())) //Custom Biome C
							%col = PTG_Cmplx_SwBioCustCTerCol.color;
					}
				}
				else
				{
					if(%BrH_cf > %BrH_init)
					{
						if(%BrH_cf < PTG_Cmplx_SldrMntnSnowLevel.getValue() || !PTG_Cmplx_ChkEnabMntnSnow.getValue())
							%col = PTG_Cmplx_SwBioMntnRockCol.color;
						else
							%col = PTG_Cmplx_SwBioMntnSnowCol.color;
					}
					else
						%col = PTG_Cmplx_SwTerDirtCol.color; //Take dirt / shore color options into account
				}
		
			//////////////////////////////////////////////////
		
			case "Water":
			
				%col = PTG_Cmplx_SwBioDefWatCol.color;
		
				if(PTG_Cmplx_ChkEnabCustABio.getValue() && (%BrH_bioA < PTG_Cmplx_EdtSectCustA.getValue())) //Custom Biome A
					%col = PTG_Cmplx_SwBioCustAWatCol.color;
				if(PTG_Cmplx_ChkEnabCustBBio.getValue() && (%BrH_bioB < PTG_Cmplx_EdtSectCustB.getValue())) //Custom Biome B
					%col = PTG_Cmplx_SwBioCustBWatCol.color;
				if(PTG_Cmplx_ChkEnabCustCBio.getValue() && (%BrH_bioC < PTG_Cmplx_EdtSectCustC.getValue())) //Custom Biome C
					%col = PTG_Cmplx_SwBioCustCWatCol.color;
				
			default:
				return "0 0 0 0";
		}
	}
	
	return %col;
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_ApplyFogDepth(%colStr,%BrH)
{
	if(PTG_Cmplx_ChkPrevGenFog.getValue())
	{
		%maxHeight = PTG_Cmplx_EdtPrevFogMax.getValue();
		%res = 1 - (%BrH / %maxHeight); //find percentage of distance between brick height and max distance set in GUI, then invert percentage (bricks further away / closer to ground are more saturated, while higher bricks are darker)
		%snapZ = 8; //snap range (to make terrain height displacement easier to notice)

		%tmpColA = getWord(%colStr,0);
		%tmpColB = getWord(%colStr,1);
		%tmpColC = getWord(%colStr,2);
		%tmpColD = getWord(%colStr,3);
		
		%newColA = mClamp(%tmpColA + ((255 - %tmpColA) * %res),0,255);
		%newColB = mClamp(%tmpColB + ((255 - %tmpColB) * %res),0,255);
		%newColC = mClamp(%tmpColC + ((255 - %tmpColC) * %res),0,255);
		%newColD = mClamp(%tmpColD + ((255 - %tmpColD) * %res),0,255);
		
		%newColA = mFloor(%newColA / %snapZ) * %snapZ;
		%newColB = mFloor(%newColB / %snapZ) * %snapZ;
		%newColC = mFloor(%newColC / %snapZ) * %snapZ;
		%newColD = mFloor(%newColD / %snapZ) * %snapZ;

		return %newColA SPC %newColB SPC %newColC SPC %tmpColD;
	}
	else
		return %colStr;
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_GapFillCount(%gapDist,%type,%BrXYSize)
{
	if(%type $= "Cube")
	{
		%gapDist = mCeil(%gapDist / %BrXYSize) * %BrXYSize;
		
		%brRemA = mFloor(%gapDist / (%BrXYSize * 4)); //x4
			%gapDist -= (%brRemA * (%BrXYSize * 4));
		%brRemB = mFloor(%gapDist / (%BrXYSize * 2)); //x2
			%gapDist -= (%brRemB * (%BrXYSize * 2));
		%brRemC = mFloor(%gapDist / %BrXYSize); //normal fill size
		
		return getMax(%brRemA + %brRemB + %brRemC,0);
	}
	
	//////////////////////////////////////////////////
	
	else
	{
		%brRemA = mFloor(%gapDist / (%BrXYSize * 4)); //x4
			%gapDist -= (%brRemA * (%BrXYSize * 4));
		%brRemB = mFloor(%gapDist / (%BrXYSize * 2)); //x2
			%gapDist -= (%brRemB * (%BrXYSize * 2));
		%brRemC = mFloor(%gapDist / %BrXYSize); //cube fill
			%gapDist -= (%brRemC * %BrXYSize);

		if(%BrXYSize >= 2) //4x
		{
			%brRemD = mFloor(%gapDist / (%BrXYSize * 0.5)); //1/2 cube
				%gapDist -= (%brRemD * (%BrXYSize * 0.5));
			
			if(%BrXYSize >= 8) //16x
			{
				%brRemE = mFloor(%gapDist / (%BrXYSize * 0.25)); //1/4 cube
					%gapDist -= (%brRemE * (%BrXYSize * 0.25));
				
				if(%BrXYSize >= 16) //32x
				{
					%brRemF = mFloor(%gapDist / (%BrXYSize * 0.125)); //1/8 cube
						%gapDist -= (%brRemF * (%BrXYSize * 0.125));
				}
			}
		}
		//if(%BrXYSize > 0.5) //1x or greater
		//{
			%brRemG = mFloor(%gapDist * 0.6); //normal fill
				%gapDist -= (%brRemG / 0.6);
			%brRemH = mFloor(%gapDist * 0.2); //plate fill
		//}
		
		return getMax(%brRemA + %brRemB + %brRemC + %brRemD + %brRemE + %brRemF + %brRemG + %brRemH,0);
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_RandNumGen_Chunk(%CHPosX,%CHPosY,%RelItrSize,%IncA,%IncB,%MultA,%MultB)
{
	%ChPosX = getSubStr(%ChPosX,0,8);
	%ChPosY = getSubStr(%ChPosY,0,8);
	%ChPosXb = %ChPosX+%RelItrSize;
	%ChPosYb = %ChPosY+%RelItrSize;
	
	%ZMod = PTG_Cmplx_SldrZMod.getValue();
	%Seed = getSubStr(PTG_Cmplx_EdtSeed.getValue(),0,8);

	//Generate random initial values using a custom LCG method
	%PosXa = (((%ChPosX + %IncA + %Seed) % 99999) * %MultA) % %ZMod; // "% 99999" is to prevent a known issue with discrepancies of scientific notation between the engine on Macs and PCs
	%PosXb = (((%ChPosXb + %IncA + %Seed) % 99999) * %MultA) % %ZMod;
	%PosYa = (((%ChPosY + %IncB + %Seed) % 99999) * %MultB) % %ZMod;
	%PosYb = (((%ChPosYb + %IncB + %Seed) % 99999) * %MultB) % %ZMod;

	//Interpolate initial values to find final height values; return string
	return mAbs(%PosYa-%PosXa) SPC mAbs(%PosYb-%PosXa) SPC mAbs(%PosYa-%PosXb) SPC mAbs(%PosYb-%PosXb);
}

