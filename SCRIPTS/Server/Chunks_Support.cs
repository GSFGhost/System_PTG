function PTG_Chunk_FigureColPri(%BrH_init,%BrH_cf,%BrH_fnl,%Type,%BrPosX,%BrPosY)
{
	//UnderWater
	if(%BrH_fnl < $PTGm.lakesHLevel)
	{
		%col = $PTGbio.Bio_SubM_TerCol;
		%pri = $PTGbio.Bio_SubM_TerPri;
	}

	//Shore
	else if(%BrH_fnl <= $PTGm.sandHLevel)
	{
		%col = $PTGbio.Bio_Shore_TerCol;
		%pri = $PTGbio.Bio_Shore_TerPri;
		
		//If shore set to be same as custom biomes
		if($PTGm.shoreSameCustBiome)
		{
			if($PTGm.enabBio_CustA && ($StrArrayHV_CustomBiomeA[%BrPosX,%BrPosY] < $PTGm.bio_CustASecZ)) //Custom Biome A
			{
				%col = $PTGbio.Bio_CustA_TerCol;
				%pri = $PTGbio.Bio_CustA_TerPri;
			}
			if($PTGm.enabBio_CustB && ($StrArrayHV_CustomBiomeB[%BrPosX,%BrPosY] < $PTGm.bio_CustBSecZ)) //Custom Biome B
			{
				%col = $PTGbio.Bio_CustB_TerCol;
				%pri = $PTGbio.Bio_CustB_TerPri;
			}
			if($PTGm.enabBio_CustC && ($StrArrayHV_CustomBiomeC[%BrPosX,%BrPosY] < $PTGm.bio_CustCSecZ)) //Custom Biome C
			{
				%col = $PTGbio.Bio_CustC_TerCol;
				%pri = $PTGbio.Bio_CustC_TerPri;
			}
		}
	}
		
	//////////////////////////////////////////////////
	//Terrain

	else
	{
		switch$(%Type)
		{
			case "Terrain":
			
				if(%BrH_cf > %BrH_init && $PTGm.enabMntns) //"$PTGm.enabMntns" is to prevent issues with flat areas
				{
					if(!$PTGm.enabMntnSnow || %BrH_fnl < $PTGm.MntnsSnowHLevel)
					{
						%col = $PTGbio.Bio_Mntn_RockCol;
						%pri = $PTGbio.Bio_Mntn_RockPri;
					}
					else
					{
						%col = $PTGbio.Bio_Mntn_SnowCol;
						%pri = $PTGbio.Bio_Mntn_SnowPri;
					}
				}
				
				else
				{
					%col = $PTGm.Bio_Def_TerCol;
					%pri = $PTGm.Bio_Def_TerPri;

					if($PTGm.enabBio_CustA && ($StrArrayHV_CustomBiomeA[%BrPosX,%BrPosY] < $PTGm.bio_CustASecZ)) //Custom Biome A
					{
						%col = $PTGbio.Bio_CustA_TerCol; //!!! Prevent exceeding player's max colors. Otherwise, if a color ID exceeds it, certain bricks using that color won't load.
						%pri = $PTGbio.Bio_CustA_TerPri;
					}
					if($PTGm.enabBio_CustB && ($StrArrayHV_CustomBiomeB[%BrPosX,%BrPosY] < $PTGm.bio_CustBSecZ)) //Custom Biome B
					{
						%col = $PTGbio.Bio_CustB_TerCol; //!!! Prevent exceeding player's max colors. Otherwise, if a color ID exceeds it, certain bricks using that color won't load.
						%pri = $PTGbio.Bio_CustB_TerPri;
					}
					if($PTGm.enabBio_CustC && ($StrArrayHV_CustomBiomeC[%BrPosX,%BrPosY] < $PTGm.bio_CustCSecZ)) //Custom Biome C
					{
						%col = $PTGbio.Bio_CustC_TerCol; //!!! Prevent exceeding player's max colors. Otherwise, if a color ID exceeds it, certain bricks using that color won't load.
						%pri = $PTGbio.Bio_CustC_TerPri;
					}
				}
			
			//////////////////////////////////////////////////
			
			case "Dirt":
			
				if($PTGm.dirtSameTer) //if dirt layer is set to be same color as terrain / biomes
				{
					if(%BrH_cf > %BrH_init && $PTGm.enabMntns) //"$PTGm.enabMntns" is to prevent issues with flat areas
					{
						if(!$PTGm.enabMntnSnow || %BrH_fnl < $PTGm.MntnsSnowHLevel)
						{
							%col = $PTGbio.Bio_Mntn_RockCol;
							%pri = $PTGbio.Bio_Mntn_RockPri;
						}
						else
						{
							%col = $PTGbio.Bio_Mntn_SnowCol;
							%pri = $PTGbio.Bio_Mntn_SnowPri;
						}
					}
					
					else
					{
						%col = $PTGm.Bio_Def_TerCol;
						%pri = $PTGm.Bio_Def_TerPri;
				
						if($PTGm.enabBio_CustA && ($StrArrayHV_CustomBiomeA[%BrPosX,%BrPosY] < $PTGm.bio_CustASecZ)) //Custom Biome A
						{
							%col = $PTGbio.Bio_CustA_TerCol; //!!! Prevent exceeding player's max colors. Otherwise, if a color ID exceeds it, certain bricks using that color won't load.
							%pri = $PTGbio.Bio_CustA_TerPri;
						}
						if($PTGm.enabBio_CustB && ($StrArrayHV_CustomBiomeB[%BrPosX,%BrPosY] < $PTGm.bio_CustBSecZ)) //Custom Biome B
						{
							%col = $PTGbio.Bio_CustB_TerCol; //!!! Prevent exceeding player's max colors. Otherwise, if a color ID exceeds it, certain bricks using that color won't load.
							%pri = $PTGbio.Bio_CustB_TerPri;
						}
						if($PTGm.enabBio_CustC && ($StrArrayHV_CustomBiomeC[%BrPosX,%BrPosY] < $PTGm.bio_CustCSecZ)) //Custom Biome C
						{
							%col = $PTGbio.Bio_CustC_TerCol; //!!! Prevent exceeding player's max colors. Otherwise, if a color ID exceeds it, certain bricks using that color won't load.
							%pri = $PTGbio.Bio_CustC_TerPri;
						}
					}
				}
				else
				{
					if(%BrH_cf > %BrH_init && $PTGm.enabMntns) //"$PTGm.enabMntns" is to prevent issues with flat areas
					{
						if(!$PTGm.enabMntnSnow || %BrH_fnl < $PTGm.MntnsSnowHLevel)
						{
							%col = $PTGbio.Bio_Mntn_RockCol;
							%pri = $PTGbio.Bio_Mntn_RockPri;
						}
						else
						{
							%col = $PTGbio.Bio_Mntn_SnowCol;
							%pri = $PTGbio.Bio_Mntn_SnowPri;
						}
					}
					
					else
					{
						%col = $PTGm.dirtCol;
						%pri = $PTGm.dirtPri;
					}
				}
				
			default:
				return "0 0";
		}
	}
	
	return %col SPC %pri;
}


////////////////////////////////////////////////////////////////////////////////////////////////////

//Terrain Details Check (DataBlock, Color and Print rel to biome)
function PTG_Chunk_FigureColPri_Details(%BrH,%detailNum,%BrH_cf,%BrH_init,%BrPosX,%BrPosY,%CHPosX,%CHPosY)
{
	//%BrH_cf and %BrH_init are for checking mountains biome
	
	%str = "|(){}[]<>\"\'\\/~`!@#$%^&*_+=,?:;"; //evals in this file don't require same level of security as in Server.cs (be careful who you make super admin however)

	//Underwater Details
	if(%BrH < $PTGm.lakesHLevel)// && $PTGm.terType !$= "SkyLands") //shouldn't use SubM biome in skylands though...
	{
		eval("%tempBr = stripChars($PTGbio.Bio_SubM_Det" @ %detailNum @ "_BrDB,%str);"); //evals are secure (stripChars prevents any malicious code)
		%tempH = %BrH + (%tempBr.brickSizeZ * 0.2);
		
		if(%tempH < $PTGm.lakesHLevel)
		{
			if(!isObject(PTG_MassBioDetails_SubMarine) || PTG_MassBioDetails_SubMarine.totalDetAm <= 0)
			{
				eval("%col = stripChars($PTGbio.Bio_SubM_Det" @ %detailNum @ "_Col,%str);");
				eval("%pri = stripChars($PTGbio.Bio_SubM_Det" @ %detailNum @ "_Pri,%str);");
				%detailDB = %tempBr; //???
			}
			else
			{
				%massDetAm = PTG_MassBioDetails_SubMarine.totalDetAm;
				%massDetBio = PTG_MassBioDetails_SubMarine;
			}
		}
	}
	
	//Shore Details
	else if(%BrH <= $PTGm.sandHLevel)// && %BrH > $PTGm.lakesHLevel)
	{
		if(!isObject(PTG_MassBioDetails_Shore) || PTG_MassBioDetails_Shore.totalDetAm <= 0)
		{
			//Shore Details eval("%tempBr = stripChars($PTGbio.Bio_SubM_Det" @ %detailNum @ "_BrDB,\"abcdefghijklmnopqrstuvwxyz(){}[]\"\'\\/~`!@#$%^&*()_+=,?:;\");");
			eval("%col = stripChars($PTGbio.Bio_Shore_Det" @ %detailNum @ "_Col,%str);");
			eval("%pri = stripChars($PTGbio.Bio_Shore_Det" @ %detailNum @ "_Pri,%str);");
			eval("%detailDB = stripChars($PTGbio.Bio_Shore_Det" @ %detailNum @ "_BrDB,%str);");
		}
		else
		{
			%massDetAm = PTG_MassBioDetails_Shore.totalDetAm;
			%massDetBio = PTG_MassBioDetails_Shore;
		}
		
		
		//If shore set to same as custom biomes
		if($PTGm.shoreSameCustBiome)
		{
			if($PTGm.enabBio_CustA && ($StrArrayHV_CustomBiomeA[%BrPosX,%BrPosY] < $PTGm.bio_CustASecZ)) //Custom Biome A
			{
				if(!isObject(PTG_MassBioDetails_CustomA) || PTG_MassBioDetails_CustomA.totalDetAm <= 0)
				{
					eval("%col = stripChars($PTGbio.Bio_CustA_Det" @ %detailNum @ "_Col,%str);");
					eval("%pri = stripChars($PTGbio.Bio_CustA_Det" @ %detailNum @ "_Pri,%str);");
					eval("%detailDB = stripChars($PTGbio.Bio_CustA_Det" @ %detailNum @ "_BrDB,%str);");
				}
				else
				{
					%massDetAm = PTG_MassBioDetails_CustomA.totalDetAm;
					%massDetBio = PTG_MassBioDetails_CustomA;
				}
			}
			if($PTGm.enabBio_CustB && ($StrArrayHV_CustomBiomeB[%BrPosX,%BrPosY] < $PTGm.bio_CustBSecZ)) //Custom Biome B
			{
				if(!isObject(PTG_MassBioDetails_CustomB) || PTG_MassBioDetails_CustomB.totalDetAm <= 0)
				{
					eval("%col = stripChars($PTGbio.Bio_CustB_Det" @ %detailNum @ "_Col,%str);");
					eval("%pri = stripChars($PTGbio.Bio_CustB_Det" @ %detailNum @ "_Pri,%str);");
					eval("%detailDB = stripChars($PTGbio.Bio_CustB_Det" @ %detailNum @ "_BrDB,%str);");
				}
				else
				{
					%massDetAm = PTG_MassBioDetails_CustomB.totalDetAm;
					%massDetBio = PTG_MassBioDetails_CustomB;
				}
			}
			if($PTGm.enabBio_CustC && ($StrArrayHV_CustomBiomeC[%BrPosX,%BrPosY] < $PTGm.bio_CustCSecZ)) //Custom Biome C
			{
				if(!isObject(PTG_MassBioDetails_CustomC) || PTG_MassBioDetails_CustomC.totalDetAm <= 0)
				{
					eval("%col = stripChars($PTGbio.Bio_CustC_Det" @ %detailNum @ "_Col,%str);");
					eval("%pri = stripChars($PTGbio.Bio_CustC_Det" @ %detailNum @ "_Pri,%str);");
					eval("%detailDB = stripChars($PTGbio.Bio_CustC_Det" @ %detailNum @ "_BrDB,%str);");
				}
				else
				{
					%massDetAm = PTG_MassBioDetails_CustomC.totalDetAm;
					%massDetBio = PTG_MassBioDetails_CustomC;
				}
			}
		}
	}
	
	//////////////////////////////////////////////////
	//Terrain

	else
	{
		//Mountain Details
		if(%BrH_cf > %BrH_init)
		{
			if(!isObject(PTG_MassBioDetails_Mountains) || PTG_MassBioDetails_Mountains.totalDetAm <= 0)
			{
				if(!$PTGm.enabMntnSnow || %BrH < $PTGm.MntnsSnowHLevel) //details in mountain snow layer change to snow layer color
					eval("%col = stripChars($PTGbio.Bio_Mntn_Det" @ %detailNum @ "_Col,%str);");
				else
					%col = $PTGbio.Bio_Mntn_SnowCol;
				eval("%pri = stripChars($PTGbio.Bio_Mntn_Det" @ %detailNum @ "_Pri,%str);"); //print stays the same, even if in snow layer
				eval("%detailDB = stripChars($PTGbio.Bio_Mntn_Det" @ %detailNum @ "_BrDB,%str);");
			}
			else
			{
				%massDetAm = PTG_MassBioDetails_Mountains.totalDetAm;
				%massDetBio = PTG_MassBioDetails_Mountains;
				%massDetBioMntn = true;
			}
		}
		
		else
		{
			if(!isObject(PTG_MassBioDetails_Default) || PTG_MassBioDetails_Default.totalDetAm <= 0)
			{
				//Default Details
				eval("%col = stripChars($PTGbio.Bio_Def_Det" @ %detailNum @ "_Col,%str);");
				eval("%pri = stripChars($PTGbio.Bio_Def_Det" @ %detailNum @ "_Pri,%str);");
				eval("%detailDB = stripChars($PTGbio.Bio_Def_Det" @ %detailNum @ "_BrDB,%str);");
			}
			else
			{
				%massDetAm = PTG_MassBioDetails_Default.totalDetAm;
				%massDetBio = PTG_MassBioDetails_Default;
			}
		
			//Biome Details
			if($PTGm.enabBio_CustA && ($StrArrayHV_CustomBiomeA[%BrPosX,%BrPosY] < $PTGm.bio_CustASecZ)) //Custom Biome A
			{
				if(!isObject(PTG_MassBioDetails_CustomA) || PTG_MassBioDetails_CustomA.totalDetAm <= 0)
				{
					eval("%col = stripChars($PTGbio.Bio_CustA_Det" @ %detailNum @ "_Col,%str);");
					eval("%pri = stripChars($PTGbio.Bio_CustA_Det" @ %detailNum @ "_Pri,%str);");
					eval("%detailDB = stripChars($PTGbio.Bio_CustA_Det" @ %detailNum @ "_BrDB,%str);");
				}
				else
				{
					%massDetAm = PTG_MassBioDetails_CustomA.totalDetAm;
					%massDetBio = PTG_MassBioDetails_CustomA;
				}
			}
			if($PTGm.enabBio_CustB && ($StrArrayHV_CustomBiomeB[%BrPosX,%BrPosY] < $PTGm.bio_CustBSecZ)) //Custom Biome B
			{
				if(!isObject(PTG_MassBioDetails_CustomB) || PTG_MassBioDetails_CustomB.totalDetAm <= 0)
				{
					eval("%col = stripChars($PTGbio.Bio_CustB_Det" @ %detailNum @ "_Col,%str);");
					eval("%pri = stripChars($PTGbio.Bio_CustB_Det" @ %detailNum @ "_Pri,%str);");
					eval("%detailDB = stripChars($PTGbio.Bio_CustB_Det" @ %detailNum @ "_BrDB,%str);");
				}
				else
				{
					%massDetAm = PTG_MassBioDetails_CustomB.totalDetAm;
					%massDetBio = PTG_MassBioDetails_CustomB;
				}
			}
			if($PTGm.enabBio_CustC && ($StrArrayHV_CustomBiomeC[%BrPosX,%BrPosY] < $PTGm.bio_CustCSecZ)) //Custom Biome C
			{
				if(!isObject(PTG_MassBioDetails_CustomC) || PTG_MassBioDetails_CustomC.totalDetAm <= 0)
				{
					eval("%col = stripChars($PTGbio.Bio_CustC_Det" @ %detailNum @ "_Col,%str);");
					eval("%pri = stripChars($PTGbio.Bio_CustC_Det" @ %detailNum @ "_Pri,%str);");
					eval("%detailDB = stripChars($PTGbio.Bio_CustC_Det" @ %detailNum @ "_BrDB,%str);");
				}
				else
				{
					%massDetAm = PTG_MassBioDetails_CustomC.totalDetAm;
					%massDetBio = PTG_MassBioDetails_CustomC;
				}
			}
		}
	}
	
	//////////////////////////////////////////////////
	
	//If using mass details list instead
	if(%massDetAm > 0 && %massDetBio !$= "")
	{
		%rand = (%CHPosX+%BrPosX+$PTGm.detailsOff_X+347) + (%CHPosY+%BrPosY+$PTGm.detailsOff_Y+907) + %BrH;
		%detNum = (((%rand + 10212011 + getSubStr($PTGm.seed,0,8)) % 99999) * 77171) % (%massDetAm * 100);
		%detNum = mFloor(%detNum / 100);
		%selDet = %massDetBio.detail[%detNum];
		
		if(!%massDetBioMntn || !$PTGm.enabMntnSnow || %BrH < $PTGm.MntnsSnowHLevel) //details in mountain snow layer change to snow layer color
			%col = getWord(%selDet,2);
		else
			%col = $PTGbio.Bio_Mntn_SnowCol;
		%pri = getWord(%selDet,1);
		%detailDB = getWord(%selDet,0);
	}

	return %col SPC %pri SPC %detailDB;
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Chunk_ModTerCheck_Details(%genType,%str_LURD,%str_LuRuRdLd)
{
	switch$(%genType)
	{
		case "Cubes":
		
			%ModTerPass = true;
			
		case "CubesWedges":

			switch$(%str_LURD)
			{
				case "0000":
					%ModTerPass = true;
				case "1000" or "0100" or "0010" or "0001":
					%ModTerPass = false;
				case "1100" or "0110" or "0011" or "1001":
					%ModTerPass = false;
				case "1111" or "1110" or "0111" or "1011" or "1101" or "1010" or "0101":
					%ModTerPass = false;
				default:
					%ModTerPass = true;
			}
			
		case "CubesRamps":

			if(%str_LURD $= "0000" && %str_LuRuRdLd $= "0000")
				%ModTerPass = true;
			else
				%ModTerPass = false;
	}
	
	return %ModTerPass;
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Chunk_PlantBrick(%db,%pos,%col,%colFX,%shpFX,%ang,%pri,%cl,%BG,%Chunk,%BrType,%BrMod)
{
	//Angle ID and Rotation
	switch(%ang)
	{
		case 0: %rot = "1 0 0 0";
		case 1: %rot = "0 0 1 90.0002";
		case 2: %rot = "0 0 1 180";
		case 3: %rot = "0 0 -1 90.0002";
	}
	
	//If a stream source brick is about to be planted, replace datablock with plate brick based on brick's size 
	//(prevents issues with dynamic stream updating if loaded from chunk save later on)
	if(%db.PTGStreamSrc)
	{
		%tempSizeXY = strReplace(strReplace(%db,"xStreamPTGData",""),"Brick","");
		%db = "brick" @ %tempSizeXY @ "x" @ %tempSizeXY @ "fData";
		
		if(!isObject(%db))
			return;
	}
	
	//if(%db.isBotHole && PTG_ObjLimitCheck("Bots"))
	//	return;
							
	//Plant Brick
	%brick = new fxDTSbrick()
	{
		datablock = %db;
		position = %pos;
		rotation = %rot;
		scale = "1 1 1";
		colorID = %col;
		colorFXID = %colFX;
		shapefxID = %shpFX; 
		angleID = %ang;
		printID = %pri;
		client = %cl;
		stackBL_ID = $PTG.lastClientID;
		isPlanted = true;
		
		PTG = true; //used to determine if a chunk is "edited" or not, depending on if the brick is planted by a player or by the generator itself
		PTGgenerated = true; //third party support (this field is only added in this function, unlike the ".PTG" field)
	};
	

	if(isObject(%brick))
	{
		switch$(%brick.PTGBrType = %BrType) //!!!test when loading bricks from chunk saves!!!
		{
			case "TerrainBr":
			
				%brick.PTGTerrainBr = true; //to prevent being destroyed by hammer
				
			case "StreamBr": //Saved as TerrainBr (source and normal stream bricks)
			
				%brick.PTGStreamBr = true;
				%brick.PTGStreamDist = %BrMod;
				%brick.PTGDetailBr = true;
				
				//If streams set to colliding / non-colliding
				if(!$PTG.solidStreams)
					%brick.setColliding(0);
				else
					%brick.setColliding(1);
				
			case "StreamBrAux": //Saved as TerrainBr (case when smaller stream bricks are merged into larger ones)
			
				%brick.PTGStreamBrAux = true;
				%brick.PTGDetailBr = true;
				
				//If streams set to colliding / non-colliding
				if(!$PTG.solidStreams)
					%brick.setColliding(0);
				else
					%brick.setColliding(1);
				
				//If creation of water zones is enabled for stream bricks (only apply to larger bricks, and set up below after brick is planted)
				if(%BrMod)
					%createWatZone = true;
				
			case "StreamBrTert": //
			
				%brick.PTGStreamBrTert = true;
				%brick.PTGDetailBr = true;
				
				//If streams set to colliding / non-colliding
				if(!$PTG.solidStreams)
					%brick.setColliding(0);
				else
					%brick.setColliding(1);
				
				//If creation of water zones is enabled for stream bricks (only apply to larger bricks, and set up below after brick is planted)
				if(%BrMod)
					%createWatZone = true;
				
			case "DetailBr":
			
				%brick.PTGDetailBr = true;
				
				if(%db.isWaterBrick)
					%brick.createWaterZone();
				else
				{
					if(%db.isBotHole)
					{
						if(!PTG_ObjLimitCheck("Bots"))
						{
							%brick.isBotHole = true;
							%brick.hBotType = %db.holeBot; //!!! FIX !!!
							
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
				
			case "PlayerBr":
			
				%brick.PTGPlayerBr = true;
				
			case "WaterIceBr" or "WaterLavaBr" or "WaterQuickSandBr" or "WaterBr":
			
				%brick.PTGTerrainBr = true;
				%brick.createWaterZone();
				
			case "BoundsCeilBr":
			
				//%brick.PTGTerrainBr = true;
				%brick.ChBndsCeilPTG = true;
				%brick.ChBoundsPTG = true; //???
				
			case "BoundsWallBr":
			
				//%brick.PTGTerrainBr = true;
				%brick.ChBoundsPTG = true;
				
			case "CloudBr":
			
				%brick.PTGCloudBr = true;
				%brick.PTGTerrainBr = true; //???
				
				if(!$PTGm.cloudsColl) 
					%brick.setColliding(0);
				
			case "FltIsldsBr":
			
				%brick.PTGFltIsldsBr = true;
				%brick.PTGTerrainBr = true;
				
			default:
			
				%brick.PTGPlayerBr = true; //if can't locate brick type (i.e. if loaded from file and saved to chunks), default to player brick type
		}

		if(isObject(%BG) && !$PTG.publicBricks)
			%BG.add(%brick);
		else
			BrickGroup_888888.add(%brick); //if client or brickgroup not found; dedicated servers w/o host == public terrain bricks generated (??? test)
		if(isObject(%Chunk))
		{
			%Chunk.add(%brick); //Chunk object should always exist, but added check just encase (i.e. if "force bricks into chunks" is disabled)
			%brick.inPTGChunk = true;
		}
		
		%brick.setTrusted(1);
		%brick.plant();
		
		if(%createWatZone)
		{
			%brick.createWaterZone();
			%brick.PhysicalZone.setWaterColor(getColorIDTable(%col));
			%brick.PhysicalZone.waterDensity = 0.5;
		}

		return %brick; //encase previous function needs to modify this brick afterwards
	}
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Chunk_ModTer(%inv,%posStr,%relPos,%brData,%cl,%BG,%Chunk) //If flatlands enabled, avoid this function and just use ModTer Cubes (what if caves enabled though?)
{
	//take terrain, cloud and floating island ModTer-enabled into account

	//%MinBrZSnap = %BrHalfZSize;
	%BrPosX = getWord(%relPos,0);
	%BrPosY = getWord(%relPos,1);

	%BrH = mFloatLength(getWord(%posStr,2),1);
	%BrH_Linit = %BrH_L = mFloatLength(getWord(%posStr,3),1);
	%BrH_Rinit = %BrH_R = mFloatLength(getWord(%posStr,4),1);
	%BrH_Dinit = %BrH_D = mFloatLength(getWord(%posStr,5),1);
	%BrH_Uinit = %BrH_U = mFloatLength(getWord(%posStr,6),1);
	%BrH_LU = mFloatLength(getWord(%posStr,7),1);
	%BrH_RU = mFloatLength(getWord(%posStr,8),1);
	%BrH_LD = mFloatLength(getWord(%posStr,9),1);
	%BrH_RD = mFloatLength(getWord(%posStr,10),1);
	
	//init vars avoid this adjustment (to check for caves edge section cuts before planting brick below)
	if(%BrH_L == 0) %BrH_L = %BrH+%BrZSize;
	if(%BrH_R == 0) %BrH_R = %BrH+%BrZSize;
	if(%BrH_D == 0) %BrH_D = %BrH+%BrZSize;
	if(%BrH_U == 0) %BrH_U = %BrH+%BrZSize;
	
	if(%BrH_LU == 0) %BrH_LU = %BrH+%BrZSize;
	if(%BrH_RU == 0) %BrH_RU = %BrH+%BrZSize;
	if(%BrH_LD == 0) %BrH_LD = %BrH+%BrZSize;
	if(%BrH_RD == 0) %BrH_RD = %BrH+%BrZSize;
	
	%col = getWord(%brData,0);
	%pri = getWord(%brData,1);
	%colFX = getWord(%brData,2);
	%shpFX = getWord(%brData,3);
	%relDB = getWord(%brData,4);
	%BrZSize = getWord(%brData,5);
	%BrHalfZSize = %BrZSize / 2;
	%FillBrXYZSize = getWord(%brData,6);
	%type = getWord(%brData,7);
	
	//ModTer Type
	switch$(%type)
	{
		case "Clouds":
			%modTerType = $PTGm.modTerGenType_clouds;
			%brType = "CloudBr";
		case "FltIslds":
			%modTerType = $PTGm.modTerGenType_fltislds;
			%brType = "FltIsldsBr";
		case "Terrain" or "CaveA" or "CaveB":
			%modTerType = $PTGm.modTerGenType;
			%brType = "TerrainBr";
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	switch$(%modTerType)
	{
		case "Cubes":

			%pos = getWords(%posStr,0,1) SPC %BrH-%BrHalfZSize;
			PTG_Chunk_PlantBrick(%relDB,%pos,%col,%colFX,%shpFX,%ang = 0,%pri,%cl,%BG,%Chunk,%brType,"");
			
		////////////////////////////////////////////////////////////////////////////////////////////////////
		
		case "CubesWedges":

			if((%type !$= "CaveA" && %type !$= "CaveB") || (%BrH_Linit != 0 && %BrH_Rinit != 0 && %BrH_Dinit != 0 && %BrH_Uinit != 0))
			{
				if(!%inv)
				{
					%BrH += %BrZSize;
					
					if(%BrH_L >= %BrH) %state++;
					if(%BrH_R >= %BrH) %state++;
					if(%BrH_D >= %BrH) %state++;
					if(%BrH_U >= %BrH) %state++;
					
					%str_LURD = (%BrH_L >= %BrH) @ (%BrH_U >= %BrH) @ (%BrH_R >= %BrH) @ (%BrH_D >= %BrH);
					%cond = "<=";
					%MaxH = mFloatLength(getMax(%BrH_L,getMax(%BrH_R,getMax(%BrH_D,%BrH_U))),1);//+0.1;
					%BrHb = %BrH - %BrZSize; //reset to start pos
					%pos = getWords(%posStr,0,1) SPC %BrH-%BrHalfZSize;
				}
				else
				{
					%BrH -= %BrZSize;
					
					if(%BrH_L <= %BrH) %state++;
					if(%BrH_R <= %BrH) %state++;
					if(%BrH_D <= %BrH) %state++;
					if(%BrH_U <= %BrH) %state++;
					
					%str_LURD = (%BrH_L <= %BrH) @ (%BrH_U <= %BrH) @ (%BrH_R <= %BrH) @ (%BrH_D <= %BrH);
					%cond = ">=";
					%MaxH = mFloatLength(getMin(%BrH_L,getMin(%BrH_R,getMin(%BrH_D,%BrH_U))),1);//-0.1;
					%BrHb = %BrH + %BrZSize; //reset to start pos
					%pos = getWords(%posStr,0,1) SPC %BrH+%BrHalfZSize;
				}
					
				%dbWedge = strReplace(%relDB,"Cube","Wedge");
				%fillType = "Cube";
				
				switch$(%str_LURD)
				{
					case "1100":
						%db = %dbWedge;
						%ang = 3;
						%fillType = "Wedge";
					case "0110":
						%db = %dbWedge;
						%ang = 0;
						%fillType = "Wedge";
					case "0011":
						%db = %dbWedge;
						%ang = 1;
						%fillType = "Wedge";
					case "1001":
						%db = %dbWedge;
						%ang = 2;
						%fillType = "Wedge";
					case "1111" or "1110" or "0111" or "1011" or "1101" or "1010" or "0101":
						%db = %relDB;
						%ang = 0;
				}
				
				if(%db $= %dbWedge)// && %type $= "Terrain")
				{
					//If two sides are the same height (otherwise looks odd when generated)
					%checkA = %BrH_L == %MaxH && (%BrH_L == %BrH_R || %BrH_L == %BrH_D || %BrH_L == %BrH_U);
					%checkB = %BrH_R == %MaxH && (%BrH_R == %BrH_L || %BrH_R == %BrH_D || %BrH_R == %BrH_U);
					%checkC = %BrH_D == %MaxH && (%BrH_D == %BrH_R || %BrH_D == %BrH_L || %BrH_D == %BrH_U);
					%checkD = %BrH_U == %MaxH && (%BrH_U == %BrH_R || %BrH_U == %BrH_D || %BrH_U == %BrH_L);
					
					if(%checkA || %checkB || %checkC || %checkD)
					{
						PTG_Chunk_SubDivideFill(%cl,%Chunk,getWord(%posStr,0),getWord(%posStr,1),%BG,%BrHb SPC %MaxH SPC "PlateFill",%col SPC 0 SPC 0 SPC %ang SPC %pri,%cond,%fillType,%brType);
						return;
					}
				}
				
				if(isObject(%db))
					PTG_Chunk_PlantBrick(%db,%pos,%col,%colFX,%shpFX,%ang,%pri,%cl,%BG,%Chunk,%brType,"");
			}
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		
		case "CubesRamps":
		
			//Setup brick datablocks (use inverted ModTer bricks if required)
			%dbRamp = strReplace(%relDB,"Cube","Ramp");
			%dbWedge = strReplace(%relDB,"Cube","Wedge");
			%dbCornerA = strReplace(%relDB,"Cube","CornerA");
			%dbCornerB = strReplace(%relDB,"Cube","CornerB");
			%dbCornerC = strReplace(%relDB,"Cube","CornerC");
			%dbCornerD = strReplace(%relDB,"Cube","CornerD");
	
			//Determine number of adjacent bricks to determine which rules to use
			if(!%inv)
			{
				%BrH += %BrZSize;
				
				if(%BrH_L >= %BrH) %state++;
				if(%BrH_R >= %BrH) %state++;
				if(%BrH_D >= %BrH) %state++;
				if(%BrH_U >= %BrH) %state++;

				%pos = getWords(%posStr,0,1) SPC %BrH-%BrHalfZSize;
				%pos_aux = getWords(%posStr,0,1) SPC %BrH+%BrZSize-%BrHalfZSize;
				%BrH_inc = %BrH+%BrZSize;
				
				%str_LURD = (%BrH_L >= %BrH) @ (%BrH_U >= %BrH) @ (%BrH_R >= %BrH) @ (%BrH_D >= %BrH);
				%str_LuRuRdLd = (%BrH_LU >= %BrH) @ (%BrH_RU >= %BrH) @ (%BrH_RD >= %BrH) @ (%BrH_LD >= %BrH);
				%strInc_LURD = (%BrH_L >= %BrH_inc) @ (%BrH_U >= %BrH_inc) @ (%BrH_R >= %BrH_inc) @ (%BrH_D >= %BrH_inc);
				%strInc_LuRuRdLd = (%BrH_LU >= %BrH_inc) @ (%BrH_RU >= %BrH_inc) @ (%BrH_RD >= %BrH_inc) @ (%BrH_LD >= %BrH_inc);
				
				%cond = "<=";
				%MaxH = getMax(%BrH_L,getMax(%BrH_R,getMax(%BrH_D,%BrH_U)));
			}
			else
			{
				%BrH -= %BrZSize;
				
				if(%BrH_L <= %BrH) %state++;
				if(%BrH_R <= %BrH) %state++;
				if(%BrH_D <= %BrH) %state++;
				if(%BrH_U <= %BrH) %state++;
				
				%pos = getWords(%posStr,0,1) SPC %BrH+%BrHalfZSize;
				%pos_aux = getWords(%posStr,0,1) SPC %BrH-%BrZSize+%BrHalfZSize;
				%BrH_inc = %BrH-%BrZSize;
				%BrH_incB = %BrH_inc-%BrZSize;
				
				%str_LURD = (%BrH_L <= %BrH) @ (%BrH_U <= %BrH) @ (%BrH_R <= %BrH) @ (%BrH_D <= %BrH);
				%str_LuRuRdLd = (%BrH_LU <= %BrH) @ (%BrH_RU <= %BrH) @ (%BrH_RD <= %BrH) @ (%BrH_LD <= %BrH);
				%strInc_LURD = (%BrH_L <= %BrH_inc) @ (%BrH_U <= %BrH_inc) @ (%BrH_R <= %BrH_inc) @ (%BrH_D <= %BrH_inc);
				%strInc_LuRuRdLd = (%BrH_LU <= %BrH_inc) @ (%BrH_RU <= %BrH_inc) @ (%BrH_RD <= %BrH_inc) @ (%BrH_LD <= %BrH_inc);
				%strIncB_LuRuRdLd = (%BrH_LU <= %BrH_incB) @ (%BrH_RU <= %BrH_incB) @ (%BrH_RD <= %BrH_incB) @ (%BrH_LD <= %BrH_incB);

				%dbRamp = strReplace(%dbRamp,"Data","InvData");
				%dbCornerA = strReplace(%dbCornerA,"Data","InvData");
				%dbCornerB = strReplace(%dbCornerB,"Data","InvData");
				%dbCornerC = strReplace(%dbCornerC,"Data","InvData");
				%dbCornerD = strReplace(%dbCornerD,"Data","InvData");
				
				%cond = ">=";
				%MaxH = getMin(%BrH_L,getMin(%BrH_R,getMin(%BrH_D,%BrH_U)));
			}
			
			//////////////////////////////////////////////////
			
			switch(%state)
			{
				case 0:
						
					switch$(%str_LuRuRdLd)
					{
						case "1100":
							%db = %dbRamp;
							%ang = 3;
						case "0110":
							%db = %dbRamp;
							%ang = 0;
						case "0011":
							%db = %dbRamp;
							%ang = 1;
						case "1001":
							%db = %dbRamp;
							%ang = 2;
								
						case "1000" or "1101":
							%db = %dbCornerA;
							%ang = 3;
						case "0100" or "1110":
							%db = %dbCornerA;
							%ang = 0;
						case "0010" or "0111":
							%db = %dbCornerA;
							%ang = 1;
						case "0001" or "1011":
							%db = %dbCornerA;
							%ang = 2;
								
						case "0101" or "1010":
							%db = %dbCornerC;
							%ang = 0;
							
					}
				
				case 1:
				
					switch$(%str_LURD)
					{
						case "1000":
						
							switch$(%str_LuRuRdLd)
							{
								case "1100" or "1101":
									%db = %dbCornerD;
									%ang = 3;
								case "0011" or "1011":
									%db = %dbCornerD;
									%ang = 2;									
								case "1111":
									%db = %relDB;
									%ang = 0;
								default:
									%db = %dbRamp;
									%ang = 2;
							}
							
						case "0100":
						
							switch$(%str_LuRuRdLd)
							{
								case "0110" or "1110":
									%db = %dbCornerD;
									%ang = 0;
								case "1001" or "1101":
									%db = %dbCornerD;
									%ang = 3;
								case "1111":
									%db = %relDB;
									%ang = 0;
								default:
									%db = %dbRamp;
									%ang = 3;
							}
							
						case "0010":
						
							switch$(%str_LuRuRdLd)
							{
								case "0011" or "0111":
									%db = %dbCornerD;
									%ang = 1;
								case "1100" or "1110":
									%db = %dbCornerD;
									%ang = 0;
								case "1111":
									%db = %relDB;
									%ang = 0;
								default:
									%db = %dbRamp;
									%ang = 0;
							}
							
						case "0001":
							
							switch$(%str_LuRuRdLd)
							{
								case "1001" or "1011":
									%db = %dbCornerD;
									%ang = 2;
								case "0110" or "0111":
									%db = %dbCornerD;
									%ang = 1;
								case "1111":
									%db = %relDB;
									%ang = 0;
								default:
									%db = %dbRamp;
									%ang = 1;
							}
					}
				
				case 2:
				
					switch$(%str_LURD)
					{
						case "1010" or "0101":
							%db = %relDB;
							%ang = 0;
								
							switch$(%strInc_LURD)
							{
								case "1010" or "0101":
									%db = %dbCornerC;
									%ang_aux = 0;
								case "1110" or "0111" or "1011" or "1101":
									%db_aux = %relDB;
									%ang_aux = 0;
								case "1111":
									%db_aux = %relDB;
									%ang_aux = 0;
								
								case "1000":
									%db_aux = %dbRamp;
									%ang_aux = 2;
								case "0100":
									%db_aux = %dbRamp;
									%ang_aux = 3;
								case "0010":
									%db_aux = %dbRamp;
									%ang_aux = 0;
								case "0001":
									%db_aux = %dbRamp;
									%ang_aux = 1;
								
								default:
								
									switch$(%strInc_LuRuRdLd)
									{
										case "1000":
											%db_aux = %dbCornerC;
											%ang_aux = 3;
										case "0100":
											%db_aux = %dbCornerC;
											%ang_aux = 0;
										case "0010":
											%db_aux = %dbCornerC;
											%ang_aux = 1;
										case "0001":
											%db_aux = %dbCornerC;
											%ang_aux = 2;
									}
							}
						
						case "1100":
							%db = %dbCornerB;
							%ang = 3;
							%chkUp = true;
						case "0110":
							%db = %dbCornerB;
							%ang = 0;
							%chkUp = true;
						case "0011":
							%db = %dbCornerB;
							%ang = 1;
							%chkUp = true;
						case "1001":
							%db = %dbCornerB;
							%ang = 2;
							%chkUp = true;
					}
					
					if(%chkUp)
					{
						if(%inv && %BrZSize >= %FillBrXYZSize) //Switch to Wedge as secondary brick for inverted cube or pillar bricks
						{
							switch$(%strInc_LURD) //add tertiary check below wedge? (which is below corner brick above)
							{
								case "1100":
									%db_aux = %dbWedge;
									%ang_aux = 3;
									%chkUpB = true;
								case "0110":
									%db_aux = %dbWedge;
									%ang_aux = 0;
									%chkUpB = true;
								case "0011":
									%db_aux = %dbWedge;
									%ang_aux = 1;
									%chkUpB = true;
								case "1001":
									%db_aux = %dbWedge;
									%ang_aux = 2;
									%chkUpB = true;
									
								case "1000":
									%db_aux = %dbCornerA;
									%ang_aux = %ang;
								case "0100":
									%db_aux = %dbCornerA;
									%ang_aux = %ang;
								case "0010":
									%db_aux = %dbCornerA;
									%ang_aux = %ang;
								case "0001":
									%db_aux = %dbCornerA;
									%ang_aux = %ang;
									
								default:

									switch$(%strInc_LuRuRdLd)
									{
										case "1000":
											%db_aux = %dbCornerA;
											%ang_aux = 3;
										case "0100":
											%db_aux = %dbCornerA;
											%ang_aux = 0;
										case "0010":
											%db_aux = %dbCornerA;
											%ang_aux = 1;
										case "0001":
											%db_aux = %dbCornerA;
											%ang_aux = 2;
									}
									
									if(%ang_aux != %ang)
										%db_aux = "";
							}
							
							if(%chkUpB)
							{
								switch$(%strIncB_LuRuRdLd)
								{
									case "1000":
										%db_aux = %dbCornerA;
										%ang_aux = %ang;
									case "0100":
										%db_aux = %dbCornerA;
										%ang_aux = %ang;
									case "0010":
										%db_aux = %dbCornerA;
										%ang_aux = %ang;
									case "0001":
										%db_aux = %dbCornerA;
										%ang_aux = %ang;
								}
							}
						}
						else
						{
							switch$(%strInc_LURD)
							{
								case "1100":
									%db_aux = %dbCornerA;
									%ang_aux = 3;
								case "0110":
									%db_aux = %dbCornerA;
									%ang_aux = 0;
								case "0011":
									%db_aux = %dbCornerA;
									%ang_aux = 1;
								case "1001":
									%db_aux = %dbCornerA;
									%ang_aux = 2;
										
								case "1000":
									%db_aux = %dbCornerA;
									%ang_aux = %ang;
								case "0100":
									%db_aux = %dbCornerA;
									%ang_aux = %ang;
								case "0010":
									%db_aux = %dbCornerA;
									%ang_aux = %ang;
								case "0001":
									%db_aux = %dbCornerA;
									%ang_aux = %ang;
								
								//1000 0100 0010 0001
								
								default:
								
									switch$(%strInc_LuRuRdLd)
									{
										case "1000":
											%db_aux = %dbCornerA;
											%ang_aux = 3;
										case "0100":
											%db_aux = %dbCornerA;
											%ang_aux = 0;
										case "0010":
											%db_aux = %dbCornerA;
											%ang_aux = 1;
										case "0001":
											%db_aux = %dbCornerA;
											%ang_aux = 2;
										
										//Wedge
										case "1101": // or "0101":
											%db_aux = %dbCornerA;
											%ang_aux = 3;
										case "1110":
											%db_aux = %dbCornerA; //%dbWedge;
											%ang_aux = 0;
										case "0111":
											%db_aux = %dbCornerA; //%dbWedge;
											%ang_aux = 1;
										case "1011":
											%db_aux = %dbCornerA; //%dbWedge;
											%ang_aux = 2;
												
										//1001 1100 0110 0011
										//set angle to same as cornerB
										case "1001":
											%db_aux = %dbCornerA;
											%ang_aux = 3;
										case "1100":
											%db_aux = %dbCornerA;
											%ang_aux = 0;
										case "0110":
											%db_aux = %dbCornerA;
											%ang_aux = 1;
										case "0011":
											%db_aux = %dbCornerA;
											%ang_aux = 2;
									}
							}
							
							if(%ang != %ang_aux)
								%db_aux = "";
						}
					}
				
				case 3 or 4:
				
					%db = %relDB; //cube
					%ang = 0;

					switch$(%strInc_LURD)
					{
						case "1110":
							%db_aux = %relDB;
							%ang_aux = 3;
						case "0111":
							%db_aux = %relDB;
							%ang_aux = 0;
						case "1011":
							%db_aux = %relDB;
							%ang_aux = 1;
						case "1101":
							%db_aux = %relDB;
							%ang_aux = 2;
						case "1111" or "1010" or "0101":
							%db_aux = %relDB;
							%ang_aux = 0;
								
						case "0110":
							%db_aux = %dbCornerD;
							%ang_aux = 0;
						case "0011":
							%db_aux = %dbCornerD;
							%ang_aux = 1;
						case "1001":
							%db_aux = %dbCornerD;
							%ang_aux = 2;
						case "1100":
							%db_aux = %dbCornerD;
							%ang_aux = 3;
								
						case "1000":
							%db_aux = %dbRamp;
							%ang_aux = 2;
						case "0100":
							%db_aux = %dbRamp;
							%ang_aux = 3;
						case "0010":
							%db_aux = %dbRamp;
							%ang_aux = 0;
						case "0001":
							%db_aux = %dbRamp;
							%ang_aux = 1;
								
						default:
						
							switch$(%strInc_LuRuRdLd)
							{
								case "1100":
									%db_aux = %dbRamp;
									%ang_aux = 3;
								case "0110":
									%db_aux = %dbRamp;
									%ang_aux = 0;
								case "0011":
									%db_aux = %dbRamp;
									%ang_aux = 1;
								case "1001":
									%db_aux = %dbRamp;
									%ang_aux = 2;
										
								case "1000":
									%db_aux = %dbCornerC;
									%ang_aux = 3;
								case "0100":
									%db_aux = %dbCornerC;
									%ang_aux = 0;
								case "0010":
									%db_aux = %dbCornerC;
									%ang_aux = 1;
								case "0001":
									%db_aux = %dbCornerC;
									%ang_aux = 2;
										
								case "1101":
									%db_aux = %relDB;
									%ang_aux = 0;
								case "1110":
									%db_aux = %relDB;
									%ang_aux = 0;
								case "0111":
									%db_aux = %relDB;
									%ang_aux = 0;
								case "1011":
									%db_aux = %relDB;
									%ang_aux = 0;
							}
					}
			}
			
			//Only generate ModTer bricks if location is not in a cave or if location in cave is not adjacent to cave section cut (otherwise fill top / btm layers back at parent function)
			if((%type !$= "CaveA" && %type !$= "CaveB") || (%BrH_Linit != 0 && %BrH_Rinit != 0 && %BrH_Dinit != 0 && %BrH_Uinit != 0))
			{
				//Main ModTer brick gen
				if(isObject(%db))
					PTG_Chunk_PlantBrick(%db,%pos,%col,%colFX,%shpFX,%ang,%pri,%cl,%BG,%Chunk,%brType,"");

				if(isObject(%db_aux))
				{
					//Aux brick
					switch$(%db_aux)
					{
						case %relDB: 
							//Aux fill gen (only; no tertiary cap brick)
							PTG_Chunk_SubDivideFill(%cl,%Chunk,getWord(%pos,0),getWord(%pos,1),%BG,%BrH SPC %MaxH SPC "PlateFill",%col SPC 0 SPC 0 SPC %ang SPC %pri,%cond,"Cube",%brType);
						case %dbWedge:
							//Aux brick gen
							PTG_Chunk_PlantBrick(%db_aux,%pos_aux,%col,%colFX,%shpFX,%ang_aux,%pri,%cl,%BG,%Chunk,%brType,"");
						default:
							//Aux brick gen
							PTG_Chunk_PlantBrick(%db_aux,%pos_aux,%col,%colFX,%shpFX,%ang_aux,%pri,%cl,%BG,%Chunk,%brType,""); //%col_aux
					}
				}
			}
	}
}


////////////////////////////////////////////////////////////////////////////////////////////////////

//// SPAWN STATIC SHAPE FOR BOUNDARIES OR HIGHLIGHTED CHUNKS ////
function PTG_Chunk_SpawnStatic(%chunk,%pos,%scale,%type)
{
	switch$(%type)
	{
		case "Wall":
			%db = "PTGBoundsWallData";
			%col = $PTGm.boundsWallCol;
		case "Ceil":
			%db = "PTGBoundsCeilData";
			%col = $PTGm.boundsCeilCol;
		case "HL-Static":
			%db = "PTGHighlightData";
			%col = $PTG.ChunkHLACol;
		case "HL-NonStatic":
			%db = "PTGHighlightData";
			%col = $PTG.ChunkHLBCol;
		case "HL-Removal":
			%db = "PTGHighlightRmvData";
			%col = 0; //colorID not necessary		//PTG_FindClosestColor("1.000000 0.000000 0.000000 1.000000","RGBA-RGBA"); //red
		default:
			return;
	}
	
	%static = new StaticShape()
   {
      datablock = %db;
      position = %pos;
      scale = %scale;
   };
   
	%col = getColorIDTable(%col);
	%static.setNodeColor(ColMain,%col); //set color of main, rendered mesh / node
	
	if(%type $= "Wall" || %type $= "Ceil")
	{
		//If boundary (wall or ceiling)
		%static.ChBoundsPTG = true;
		%static.hideNode(col); //exported shape adds collision mesh to detail node for unknown reason (causing it to be rendered), so collision mesh is hidden (hide node instead?)
		
		//If boundary static shape is set to invisible
		if($PTGm.boundsInvisStatic)
			%static.hideNode(ColMain);
	}
	if(isObject(%chunk))
		%chunk.add(%static);
	
	return %static;
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_Chunk_SubDivideFill(%cl,%Chunk,%posX,%posY,%BG,%startEndStr,%BrInfo,%condition,%relModTer,%brType)
{
	//Take ModTer-enabled into account relative to terrain, clouds and floating islands
	
	%startPos = getWord(%startEndStr,0);
	%endPos = getWord(%startEndStr,1);
	%fillType = getWord(%startEndStr,2);
	
	%col = getWord(%BrInfo,0);
	%colFX = getWord(%BrInfo,1);
	%shpFX = getWord(%BrInfo,2);
	%ang = getWord(%BrInfo,3);
	%pri = getWord(%BrInfo,4);

	switch$(%brType) //choose relative brick size (either for terrain, clouds or floating islands
	{
		case "CloudBr":
			%relBrSize = $PTGm.brClouds_FillXYZSize;
		case "FltIsldsBr":
			%relBrSize = $PTGm.brFltIslds_FillXYZSize;
		default:
			%relBrSize = $PTGm.brTer_FillXYZSize;
	}
	%tempSize =	%relBrSize * 2;
	%tempSizeB = %tempSize * 2;
	%tempSizeC = %tempSize * 4;
		
	switch$(%relModTer)
	{
		case "Disabled":
		
			%BrSubSizes =	%relBrSize * 4 SPC 
							%relBrSize * 2 SPC 
							%relBrSize SPC 
							%relBrSize / 2 SPC 
							%relBrSize / 4 SPC 
							%relBrSize / 8 SPC 
							0.6 SPC 0.2;

			if(%tempSize > 8) 
				%ptgNMod = "PTG";
			else if(%tempSize < 4) 
				%ptgCMod = "PTG";
							
			%tempA = "brick" @ %tempSize @ "x" @ %tempSize @ "x" @ %tempSizeC @ "PTGData";
			%tempB = "brick" @ %tempSize @ "x" @ %tempSize @ "x" @ %tempSizeB @ "PTGData";
			%tempC = "brick" @ %tempSize @ "xCube" @ %ptgCMod @ "Data";
			%tempD = "brick" @ %tempSize @ "xHalfHCubePTGData";
			%tempE = "brick" @ %tempSize @ "xQuarterHCubePTGData";
			%tempF = "brick" @ %tempSize @ "xEighthHCubePTGData";
			%tempG = "brick" @ %tempSize @ "x" @ %tempSize @ %ptgNMod @ "Data";
			%tempH = "brick" @ %tempSize @ "x" @ %tempSize @ "fData";

			switch$(%fillType)
			{
				case "CubeFill":
					%cStart = 0;
					%cEnd = 3; //Cube Scale
				//case "PlateFill":
				//	%cStart = 0;
				//	%cEnd = 6; //???
				case "PlateFill":
					%cStart = 0;
					%cEnd = 8;
				default:
					return;
			}
			
		//////////////////////////////////////////////////

		case "Cube" or "Wedge":

			%BrSubSizes =	%relBrSize * 2 SPC 
							%relBrSize SPC 
							%relBrSize * 0.75 SPC 
							%relBrSize / 2 SPC 
							%relBrSize / 4;
							
			%tempA = "brick" @ %tempSize @ %relModTer @ "5Data"; //Steep
			%tempB = "brick" @ %tempSize @ %relModTer @ "1Data"; //Full
			%tempC = "brick" @ %tempSize @ %relModTer @ "2Data"; //3/4
			%tempD = "brick" @ %tempSize @ %relModTer @ "3Data"; //1/2
			%tempE = "brick" @ %tempSize @ %relModTer @ "4Data"; //1/4
			
			switch$(%fillType)
			{
				case "CubeFill":
					%cStart = 0;
					%cEnd = 1; //Full Scale
				//case "PlateFill":
				//	%cStart = 0;
				//	%cEnd = 2;
				case "PlateFill":
					%cStart = 0;
					%cEnd = 8;
				default: 
					return;
			}
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	if(%tempSize == 32)
		%cStart = getMax(%cStart,1); //prevent using a 32x32x128 brick
	
	for(%c = %cStart; %c < %cEnd && %c < 8; %c++)
	{
		%temp = getWord(%BrSubSizes,%c);
		%tempHalf = %temp / 2;
		%tempDB = getWord(%tempA SPC %tempB SPC %tempC SPC %tempD SPC %tempE SPC %tempF SPC %tempG SPC %tempH,%c);
		
		if(isObject(%tempDB))
		{
			switch$(%condition)
			{
				case ">":
				
					for(%d = %startPos; (%d - %temp) > %endPos && %d > 0; %d -= %temp)
					{
						if((%d - %temp) < 0)// && %fillType $= "CubeFill")
						{
							%startEndStr = %d SPC 0 SPC "PlateFill";
							PTG_Chunk_SubDivideFill(%cl,%Chunk,%posX,%posY,%BG,%startEndStr,%BrInfo,">=",%relModTer,%brType);
							
							return;
						}
						else //prevent bricks not generating if underground
						{
							%pos = %posX SPC %posY SPC (%d - %tempHalf);
							PTG_Chunk_PlantBrick(%tempDB,%pos,%col,%colFX,%shpFX,%ang,%pri,%cl,%BG,%Chunk,%brType,"");
						}
					}
					
				case ">=":
				
					for(%d = %startPos; (%d - %temp) >= %endPos && %d > 0; %d -= %temp)
					{
						if((%d - %temp) < 0)// && %fillType $= "CubeFill")
						{
							%startEndStr = %d SPC 0 SPC "PlateFill";
							PTG_Chunk_SubDivideFill(%cl,%Chunk,%posX,%posY,%BG,%startEndStr,%BrInfo,">=",%relModTer,%brType);
							
							return;
						}
						else //prevent bricks not generating if underground
						{
							%pos = %posX SPC %posY SPC (%d - %tempHalf);
							PTG_Chunk_PlantBrick(%tempDB,%pos,%col,%colFX,%shpFX,%ang,%pri,%cl,%BG,%Chunk,%brType,"");
						}
					}
					
				case "<":
				
					for(%d = %startPos; (%d + %temp) < %endPos && %d > 0; %d += %temp)
					{
						%pos = %posX SPC %posY SPC (%d + %tempHalf);
						PTG_Chunk_PlantBrick(%tempDB,%pos,%col,%colFX,%shpFX,%ang,%pri,%cl,%BG,%Chunk,%brType,"");
					}
					
				case "<=":
				
					for(%d = %startPos; (%d + %temp) <= %endPos && %d > 0; %d += %temp)
					{
						%pos = %posX SPC %posY SPC (%d + %tempHalf);
						PTG_Chunk_PlantBrick(%tempDB,%pos,%col,%colFX,%shpFX,%ang,%pri,%cl,%BG,%Chunk,%brType,"");
					}
					
				default:
					return;
			}

			%startPos = %d;
		}
	}
}

