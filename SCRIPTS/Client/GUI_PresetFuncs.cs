function PTG_GUI_PresetFuncs(%action)
{
	switch$(%action)
	{
		case "Save":
		
			%fileName = strReplace(PTG_Cmplx_EdtPresetName.getValue(),"/","-"); //prevent issues w/ forward slash char
			//%selectedText = PTG_Cmplx_TxtListPresets.getrowtextbyid(PTG_Cmplx_TxtListPresets.getSelectedID());
			
			if(%fileName $= "Preset Name" || strReplace(%fileName," ","") $= "")
			{
				CLIENTCMDPTG_ReceiveMsg("Error","PTG: Preset Save Failed","Please enter a valid name and description for your preset save.");
				return;
			}
			if(!isWriteableFileName(%fp = "Config/Client/PTGv3/Presets/" @ %fileName @ ".txt"))
			{
				CLIENTCMDPTG_ReceiveMsg("Error","PTG: Preset Save Failed","New filename contains invalid text characters for saving!");
				return;
			}
			if(%fileName $= "Default Settings") //if(PTG_Cmplx_TxtListPresets.getValue() $= "<<Default Settings>>" || PTG_Cmplx_TxtListPresets.getSelectedID() == 0)
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Preset Save Failed","Can't overwrite default settings!");
				return;
			}
			
			//Prevent overwriting nested files
			if(!isFile("Config/Client/PTGv3/Presets/" @ %fileName @ ".txt"))
			{
				%fileCount = getFileCount(%tmpFP = "Config/Client/PTGv3/Presets/*.txt");
				
				for(%c = 0; %c < %fileCount; %c++)
				{
					if(fileBase(%tmpFile = findNextFile(%tmpFP)) $= %fileName && filePath(%tmpFile) !$= "Config/Client/PTGv3/Presets/")
					{
						CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Preset Overwrite Failed","Couldn't overwrite preset file because it's nested in a folder or .zip file. Please move the file to the \"Config/Client/PTGv3/Presets/\" directory.");
						return;
					}
				}
			}

			%file = new FileObject();
			%file.openForWrite(%fp);
			
			%file.writeLine(">>Saved preset settings for the Main GUI");
			%file.writeLine(getSubStr(PTG_Cmplx_EdtPresetDesc.getValue(),0,255)); //desc
			%file.writeLine("");
			
			//Save Colorset
			for(%c = 0; %c < 64; %c++)
				%file.writeLine(getColorIDTable(%c));
			%file.writeLine("");
			
			%file.writeLine(">Setup"); //Setup
			%file.writeLine(PTG_Cmplx_EdtSeed.getValue());
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpTerBr.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
			{
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
				%file.writeLine(PTG_Cmplx_BmpTerBr.ModTer);
			}
			else
			{
				%file.writeLine("");
				%file.writeLine("");
			}
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpTerPri.PrintID],0)); //eventually prevent prints and color saving if brick datablock doesn't exist
			%file.writeLine(PTG_Cmplx_SwTerCol.colorID);
			
			%file.writeLine(PTG_Cmplx_PopUpTerType.getSelected());
			%file.writeLine(PTG_Cmplx_ChkRadialGrid.getValue());
			%file.writeLine(PTG_Cmplx_PopUpModTerType.getSelected());
			%file.writeLine(PTG_Cmplx_ChkGradualGen.getValue());
			%file.writeLine(PTG_Cmplx_EnabAutoSave.getValue());
			
			%file.writeLine(PTG_Cmplx_EdtGridXStart.getValue());
			%file.writeLine(PTG_Cmplx_EdtGridYStart.getValue());
			%file.writeLine(PTG_Cmplx_EdtGridXEnd.getValue());
			%file.writeLine(PTG_Cmplx_EdtGridYEnd.getValue());
			%file.writeLine(PTG_Cmplx_ChkEdgeFallOff.getValue());
			%file.writeLine(PTG_Cmplx_EdtEdgeFallOffDist.getValue());
			
			%file.writeLine(PTG_Cmplx_EnabInfiniteTer.getValue());
			%file.writeLine(PTG_Cmplx_SldrChRadP.getValue());
			%file.writeLine(PTG_Cmplx_SldrChRadSA.getValue());
			%file.writeLine(PTG_Cmplx_ChkClrDistChunks.getValue());
			
			
			%file.writeLine(">Features"); //Features
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpTerDirtPri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwTerDirtCol.colorID);
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpTerStonePri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwTerStoneCol.colorID);

			%file.writeLine(PTG_Cmplx_SldrWaterLevel.getValue());
			%file.writeLine(PTG_Cmplx_SldrSandLevel.getValue());
			%file.writeLine(PTG_Cmplx_SldrTerZOffset.getValue());
			%file.writeLine(PTG_Cmplx_ChkEnabCnctLakes.getValue());
			%file.writeLine(PTG_Cmplx_ChkEnabPlateCap.getValue());
			%file.writeLine(PTG_Cmplx_ChkDirtSameBioTer.getValue());
			%file.writeLine(PTG_Cmplx_ChkShoreSameCustBiome.getValue());
			%file.writeLine(PTG_Cmplx_ChkDisWater.getValue());
			
			%file.writeLine(PTG_Cmplx_ChkEnabDetails.getValue());
			%file.writeLine(PTG_Cmplx_SldrDetailFreq.getValue());
			%file.writeLine(PTG_Cmplx_ChkEnabCustABio.getValue());
			%file.writeLine(PTG_Cmplx_ChkEnabCustBBio.getValue());
			%file.writeLine(PTG_Cmplx_ChkEnabCustCBio.getValue());
			%file.writeLine(PTG_Cmplx_ChkAutoHideSpawns.getValue());
			%file.writeLine(PTG_Cmplx_ChkEnabFltIsldDetails.getValue());
			
			%file.writeLine(PTG_Cmplx_ChkEnabMntns.getValue());
			%file.writeLine(PTG_Cmplx_ChkEnabMntnSnow.getValue());
			%file.writeLine(PTG_Cmplx_SldrMntnSnowLevel.getValue());
			%file.writeLine(PTG_Cmplx_SldrMntnZSnapMult.getValue());
			%file.writeLine(PTG_Cmplx_SldrMntnZMult.getValue());
			
			%file.writeLine(PTG_Cmplx_ChkEnabCaves.getValue());
			%file.writeLine(PTG_Cmplx_SldrCaveZOffset.getValue());
			
			%file.writeLine(PTG_Cmplx_ChkEnabClouds.getValue());
			if(isObject(%tmpBr = PTG_Cmplx_BmpCloudBr.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
			{
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
				%file.writeLine(PTG_Cmplx_BmpCloudBr.ModTer);
			}
			else
			{
				%file.writeLine("");
				%file.writeLine("");
			}
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpCloudPri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwCloudCol.colorID);
			%file.writeLine(PTG_Cmplx_SldrCloudZOffset.getValue());
			%file.writeLine(PTG_Cmplx_ChkEnabCloudColl.getValue());
			%file.writeLine(PTG_Cmplx_PopUpModTerType_clouds.getSelected());
			
			%file.writeLine(PTG_Cmplx_ChkEnabFltIslds.getValue());
			if(isObject(%tmpBr = PTG_Cmplx_BmpFltIsldsBr.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
			{
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
				%file.writeLine(PTG_Cmplx_BmpFltIsldsBr.ModTer);
			}
			else
			{
				%file.writeLine("");
				%file.writeLine("");
			}
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpFltIsldsPri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwFltIsldsCol.colorID);
			%file.writeLine(PTG_Cmplx_SldrFltIsldsAZOffset.getValue());
			%file.writeLine(PTG_Cmplx_SldrFltIsldsBZOffset.getValue());
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpFltIsldsDirtPri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwFltIsldsDirtCol.colorID);
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpFltIsldsStonePri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwFltIsldsStoneCol.colorID);
			%file.writeLine(PTG_Cmplx_PopUpModTerType_fltislds.getSelected());
			
			%file.writeLine(PTG_Cmplx_ChkEnabBnds.getValue());
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBndsWallPri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBndsWallCol.colorID);
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBndsCeilPri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBndsCeilCol.colorID);
			%file.writeLine(PTG_Cmplx_SldrBndsHAboveTer.getValue());
			%file.writeLine(PTG_Cmplx_SldrBndsH.getValue());
			%file.writeLine(PTG_Cmplx_ChkBndsRelToTer.getValue());
			%file.writeLine(PTG_Cmplx_ChkBndsStrtRelOffset.getValue());
			%file.writeLine(PTG_Cmplx_ChkEnabBndsCeil.getValue());
			%file.writeLine(PTG_Cmplx_ChkBndsUseStatic.getValue());
			%file.writeLine(PTG_Cmplx_ChkBndsInvisStatic.getValue());
			
			
			%file.writeLine(">Build-Loading"); //Default Biome
			%file.writeLine(PTG_Cmplx_ChkEnabBuildLoad.getValue());
			%file.writeLine(PTG_Cmplx_ChkAllowFlatAreas.getValue());
			%file.writeLine(PTG_Cmplx_ChkGenDetFlatAreas.getValue());
			%file.writeLine(PTG_Cmplx_ChkBldLdUseTerHMax.getValue());
			%file.writeLine(PTG_Cmplx_SldrFlatAreaFreq.getValue());
			%file.writeLine(PTG_Cmplx_PopUpGridSizeSmall.getSelected());
			%file.writeLine(PTG_Cmplx_PopUpGridSizeLarge.getSelected());
			
			
			%file.writeLine(">Default Biome"); //Default Biome
			//%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioDefTerPri.PrintID],0));
			//%file.writeLine(PTG_Cmplx_SwBioDefTerCol.colorID);
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioDefWatPri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioDefWatCol.colorID);
			//%file.writeLine(PTG_Cmplx_CheckAllMntnsDefBio.getValue());
			%file.writeLine(PTG_Cmplx_PopUpWaterType_Def.getSelected());
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioDefDet0Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioDefDet0Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioDefDet0Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioDefDet1Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioDefDet1Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioDefDet1Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioDefDet2Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioDefDet2Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioDefDet2Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioDefDet3Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioDefDet3Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioDefDet3Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioDefDet4Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioDefDet4Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioDefDet4Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioDefDet5Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioDefDet5Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioDefDet5Col.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioDefDet6Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioDefDet6Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioDefDet6Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioDefDet7Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioDefDet7Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioDefDet7Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioDefDet8Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioDefDet8Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioDefDet8Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioDefDet9Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioDefDet9Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioDefDet9Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioDefDet10Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioDefDet10Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioDefDet10Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioDefDet11Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioDefDet11Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioDefDet11Col.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioDefDet12Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioDefDet12Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioDefDet12Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioDefDet13Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioDefDet13Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioDefDet13Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioDefDet14Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioDefDet14Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioDefDet14Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioDefDet15Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioDefDet15Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioDefDet15Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioDefDet16Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioDefDet16Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioDefDet16Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioDefDet17Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioDefDet17Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioDefDet17Col.colorID);
			
			
			%file.writeLine(">Shore Biome"); //Shore Biome
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioShoreTerPri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioShoreTerCol.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioShoreDet0Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioShoreDet0Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioShoreDet0Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioShoreDet1Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioShoreDet1Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioShoreDet1Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioShoreDet2Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioShoreDet2Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioShoreDet2Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioShoreDet3Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioShoreDet3Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioShoreDet3Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioShoreDet4Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioShoreDet4Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioShoreDet4Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioShoreDet5Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioShoreDet5Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioShoreDet5Col.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioShoreDet6Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioShoreDet6Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioShoreDet6Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioShoreDet7Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioShoreDet7Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioShoreDet7Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioShoreDet8Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioShoreDet8Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioShoreDet8Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioShoreDet9Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioShoreDet9Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioShoreDet9Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioShoreDet10Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioShoreDet10Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioShoreDet10Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioShoreDet11Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioShoreDet11Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioShoreDet11Col.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioShoreDet12Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioShoreDet12Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioShoreDet12Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioShoreDet13Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioShoreDet13Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioShoreDet13Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioShoreDet14Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioShoreDet14Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioShoreDet14Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioShoreDet15Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioShoreDet15Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioShoreDet15Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioShoreDet16Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioShoreDet16Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioShoreDet16Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioShoreDet17Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioShoreDet17Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioShoreDet17Col.colorID);
		
		
			%file.writeLine(">Submarine Biome"); //Submarine Biome
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioSubMTerPri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioSubMTerCol.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioSubMDet0Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioSubMDet0Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioSubMDet0Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioSubMDet1Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioSubMDet1Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioSubMDet1Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioSubMDet2Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioSubMDet2Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioSubMDet2Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioSubMDet3Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioSubMDet3Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioSubMDet3Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioSubMDet4Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioSubMDet4Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioSubMDet4Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioSubMDet5Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioSubMDet5Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioSubMDet5Col.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioSubMDet6Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioSubMDet6Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioSubMDet6Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioSubMDet7Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioSubMDet7Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioSubMDet7Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioSubMDet8Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioSubMDet8Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioSubMDet8Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioSubMDet9Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioSubMDet9Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioSubMDet9Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioSubMDet10Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioSubMDet10Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioSubMDet10Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioSubMDet11Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioSubMDet11Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioSubMDet11Col.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioSubMDet12Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioSubMDet12Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioSubMDet12Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioSubMDet13Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioSubMDet13Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioSubMDet13Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioSubMDet14Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioSubMDet14Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioSubMDet14Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioSubMDet15Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioSubMDet15Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioSubMDet15Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioSubMDet16Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioSubMDet16Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioSubMDet16Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioSubMDet17Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioSubMDet17Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioSubMDet17Col.colorID);
			
			
			%file.writeLine(">Custom Biome A"); //Custom Biome A
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustATerPri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustATerCol.colorID);
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustAWatPri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustAWatCol.colorID);
			%file.writeLine(PTG_Cmplx_SldrTerHMod_CustA.getValue());
			//%file.writeLine(PTG_Cmplx_CheckAllMntnsCustBioA.getValue());
			%file.writeLine(PTG_Cmplx_PopUpWaterType_CustA.getSelected());
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustADet0Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustADet0Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustADet0Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustADet1Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustADet1Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustADet1Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustADet2Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustADet2Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustADet2Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustADet3Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustADet3Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustADet3Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustADet4Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustADet4Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustADet4Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustADet5Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustADet5Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustADet5Col.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustADet6Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustADet6Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustADet6Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustADet7Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustADet7Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustADet7Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustADet8Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustADet8Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustADet8Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustADet9Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustADet9Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustADet9Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustADet10Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustADet10Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustADet10Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustADet11Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustADet11Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustADet11Col.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustADet12Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustADet12Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustADet12Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustADet13Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustADet13Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustADet13Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustADet14Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustADet14Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustADet14Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustADet15Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustADet15Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustADet15Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustADet16Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustADet16Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustADet16Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustADet17Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustADet17Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustADet17Col.colorID);
			
			
			%file.writeLine(">Custom Biome B"); //Custom Biome B
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustBTerPri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustBTerCol.colorID);
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustBWatPri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustBWatCol.colorID);
			%file.writeLine(PTG_Cmplx_SldrTerHMod_CustB.getValue());
			//%file.writeLine(PTG_Cmplx_CheckAllMntnsCustBioB.getValue());
			%file.writeLine(PTG_Cmplx_PopUpWaterType_CustB.getSelected());
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustBDet0Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustBDet0Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustBDet0Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustBDet1Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustBDet1Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustBDet1Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustBDet2Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustBDet2Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustBDet2Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustBDet3Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustBDet3Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustBDet3Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustBDet4Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustBDet4Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustBDet4Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustBDet5Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustBDet5Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustBDet5Col.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustBDet6Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustBDet6Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustBDet6Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustBDet7Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustBDet7Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustBDet7Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustBDet8Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustBDet8Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustBDet8Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustBDet9Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustBDet9Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustBDet9Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustBDet10Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustBDet10Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustBDet10Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustBDet11Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustBDet11Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustBDet11Col.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustBDet12Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustBDet12Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustBDet12Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustBDet13Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustBDet13Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustBDet13Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustBDet14Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustBDet14Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustBDet14Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustBDet15Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustBDet15Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustBDet15Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustBDet16Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustBDet16Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustBDet16Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustBDet17Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustBDet17Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustBDet17Col.colorID);
			
			
			%file.writeLine(">Custom Biome C"); //Custom Biome C
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustCTerPri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustCTerCol.colorID);
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustCWatPri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustCWatCol.colorID);
			%file.writeLine(PTG_Cmplx_SldrTerHMod_CustC.getValue());
			//%file.writeLine(PTG_Cmplx_CheckAllMntnsCustBioC.getValue());
			%file.writeLine(PTG_Cmplx_PopUpWaterType_CustC.getSelected());
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustCDet0Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustCDet0Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustCDet0Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustCDet1Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustCDet1Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustCDet1Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustCDet2Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustCDet2Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustCDet2Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustCDet3Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustCDet3Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustCDet3Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustCDet4Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustCDet4Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustCDet4Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustCDet5Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustCDet5Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustCDet5Col.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustCDet6Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustCDet6Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustCDet6Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustCDet7Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustCDet7Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustCDet7Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustCDet8Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustCDet8Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustCDet8Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustCDet9Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustCDet9Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustCDet9Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustCDet10Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustCDet10Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustCDet10Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustCDet11Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustCDet11Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustCDet11Col.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustCDet12Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustCDet12Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustCDet12Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustCDet13Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustCDet13Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustCDet13Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustCDet14Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustCDet14Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustCDet14Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustCDet15Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustCDet15Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustCDet15Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustCDet16Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustCDet16Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustCDet16Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCustCDet17Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCustCDet17Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCustCDet17Col.colorID);
			
			
			%file.writeLine(">Cave Bottom Biome"); //Cave Bottom Biome
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveTTerPri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveTTerCol.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveTDet0Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveTDet0Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveTDet0Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveTDet1Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveTDet1Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveTDet1Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveTDet2Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveTDet2Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveTDet2Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveTDet3Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveTDet3Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveTDet3Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveTDet4Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveTDet4Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveTDet4Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveTDet5Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveTDet5Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveTDet5Col.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveTDet6Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveTDet6Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveTDet6Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveTDet7Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveTDet7Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveTDet7Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveTDet8Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveTDet8Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveTDet8Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveTDet9Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveTDet9Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveTDet9Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveTDet10Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveTDet10Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveTDet10Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveTDet11Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveTDet11Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveTDet11Col.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveTDet12Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveTDet12Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveTDet12Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveTDet13Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveTDet13Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveTDet13Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveTDet14Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveTDet14Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveTDet14Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveTDet15Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveTDet15Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveTDet15Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveTDet16Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveTDet16Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveTDet16Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveTDet17Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveTDet17Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveTDet17Col.colorID);
			
			
			%file.writeLine(">Cave Top Biome"); //Cave Top Biome
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveBTerPri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveBTerCol.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveBDet0Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveBDet0Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveBDet0Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveBDet1Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveBDet1Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveBDet1Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveBDet2Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveBDet2Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveBDet2Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveBDet3Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveBDet3Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveBDet3Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveBDet4Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveBDet4Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveBDet4Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveBDet5Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveBDet5Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveBDet5Col.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveBDet6Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveBDet6Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveBDet6Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveBDet7Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveBDet7Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveBDet7Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveBDet8Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveBDet8Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveBDet8Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveBDet9Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveBDet9Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveBDet9Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveBDet10Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveBDet10Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveBDet10Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveBDet11Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveBDet11Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveBDet11Col.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveBDet12Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveBDet12Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveBDet12Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveBDet13Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveBDet13Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveBDet13Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveBDet14Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveBDet14Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveBDet14Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveBDet15Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveBDet15Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveBDet15Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveBDet16Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveBDet16Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveBDet16Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioCaveBDet17Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioCaveBDet17Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioCaveBDet17Col.colorID);
			
			
			%file.writeLine(">Mountain Biome"); //Mountain Biome
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioMntnRockPri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioMntnRockCol.colorID);
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioMntnSnowPri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioMntnSnowCol.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioMntnDet0Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioMntnDet0Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioMntnDet0Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioMntnDet1Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioMntnDet1Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioMntnDet1Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioMntnDet2Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioMntnDet2Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioMntnDet2Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioMntnDet3Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioMntnDet3Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioMntnDet3Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioMntnDet4Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioMntnDet4Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioMntnDet4Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioMntnDet5Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioMntnDet5Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioMntnDet5Col.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioMntnDet6Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioMntnDet6Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioMntnDet6Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioMntnDet7Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioMntnDet7Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioMntnDet7Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioMntnDet8Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioMntnDet8Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioMntnDet8Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioMntnDet9Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioMntnDet9Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioMntnDet9Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioMntnDet10Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioMntnDet10Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioMntnDet10Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioMntnDet11Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioMntnDet11Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioMntnDet11Col.colorID);
			
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioMntnDet12Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioMntnDet12Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioMntnDet12Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioMntnDet13Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioMntnDet13Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioMntnDet13Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioMntnDet14Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioMntnDet14Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioMntnDet14Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioMntnDet15Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioMntnDet15Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioMntnDet15Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioMntnDet16Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioMntnDet16Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioMntnDet16Col.colorID);
			if(isObject(%tmpBr = PTG_Cmplx_BmpBioMntnDet17Br.BrickID) && %tmpBr.getClassName() $= "fxDtsBrickData")
				%file.writeLine(%tmpBr.uiName TAB %tmpBr.category TAB %tmpBr.subCategory TAB %tmpBr.getName());
			else
				%file.writeLine("");
			%file.writeLine(getField($PTG_PrintArr[PTG_Cmplx_BmpBioMntnDet17Pri.PrintID],0));
			%file.writeLine(PTG_Cmplx_SwBioMntnDet17Col.colorID);
			

			%file.writeLine(">Advanced");  //Advanced
			%file.writeLine(PTG_Cmplx_PopupChSize.getSelected());
			%file.writeLine(PTG_Cmplx_SldrCaveTopZMult.getValue());
			%file.writeLine(PTG_Cmplx_SldrZMod.getValue());
			%file.writeLine(PTG_Cmplx_SldrCnctLakesStrt.getValue());
			%file.writeLine(PTG_Cmplx_SwTreeBaseACol.colorID);
			%file.writeLine(PTG_Cmplx_SwTreeBaseBCol.colorID);
			%file.writeLine(PTG_Cmplx_SwTreeBaseCCol.colorID);
			%file.writeLine(PTG_Cmplx_ChkFIFOclrChunks.getValue());
			%file.writeLine(PTG_Routines_ChkSeamlessModTer.getValue());
			%file.writeLine(PTG_Routines_ChkSeamlessBuildL.getValue());
			
			%file.writeLine(PTG_Cmplx_EdtTerNosOffX.getValue());
			%file.writeLine(PTG_Cmplx_EdtTerNosOffY.getValue());
			%file.writeLine(PTG_Cmplx_EdtMntnNosOffX.getValue());
			%file.writeLine(PTG_Cmplx_EdtMntnNosOffY.getValue());
			%file.writeLine(PTG_Cmplx_EdtCustANosOffX.getValue());
			%file.writeLine(PTG_Cmplx_EdtCustANosOffY.getValue());
			%file.writeLine(PTG_Cmplx_EdtCustBNosOffX.getValue());
			%file.writeLine(PTG_Cmplx_EdtCustBNosOffY.getValue());
			%file.writeLine(PTG_Cmplx_EdtCustCNosOffX.getValue());
			%file.writeLine(PTG_Cmplx_EdtCustCNosOffY.getValue());
			%file.writeLine(PTG_Cmplx_EdtCaveNosOffX.getValue());
			%file.writeLine(PTG_Cmplx_EdtCaveNosOffY.getValue());
			%file.writeLine(PTG_Cmplx_EdtCaveHNosOffX.getValue());
			%file.writeLine(PTG_Cmplx_EdtCaveHNosOffY.getValue());
			%file.writeLine(PTG_Cmplx_EdtFltIsldANosOffX.getValue());
			%file.writeLine(PTG_Cmplx_EdtFltIsldANosOffY.getValue());
			%file.writeLine(PTG_Cmplx_EdtFltIsldBNosOffX.getValue());
			%file.writeLine(PTG_Cmplx_EdtFltIsldBNosOffY.getValue());
			%file.writeLine(PTG_Cmplx_EdtSkylandNosOffX.getValue());
			%file.writeLine(PTG_Cmplx_EdtSkylandNosOffY.getValue());
			%file.writeLine(PTG_Cmplx_EdtDetNosOffX.getValue());
			%file.writeLine(PTG_Cmplx_EdtDetNosOffY.getValue());
			%file.writeLine(PTG_Cmplx_EdtCloudNosOffX.getValue());
			%file.writeLine(PTG_Cmplx_EdtCloudNosOffY.getValue());
			%file.writeLine(PTG_Cmplx_EdtBldLoadNosOffX.getValue());
			%file.writeLine(PTG_Cmplx_EdtBldLoadNosOffY.getValue());
			
			%file.writeLine(PTG_Cmplx_EdtSectCave.getValue());
			%file.writeLine(PTG_Cmplx_EdtSectSkyland.getValue());
			%file.writeLine(PTG_Cmplx_EdtSectFltIsld.getValue());
			%file.writeLine(PTG_Cmplx_EdtSectCustA.getValue());
			%file.writeLine(PTG_Cmplx_EdtSectCustB.getValue());
			%file.writeLine(PTG_Cmplx_EdtSectCustC.getValue());
			%file.writeLine(PTG_Cmplx_EdtSectCloud.getValue());
			//%file.writeLine(PTG_Cmplx_EdtSectCnctLakes.getValue());
			%file.writeLine(PTG_Cmplx_EdtSectMntn.getValue());
			
			%file.writeLine(PTG_Cmplx_EdtNosScaleTerAXY.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleTerAZ.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleTerBXY.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleTerBZ.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleTerCXY.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleTerCZ.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleMntnAXY.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleMntnAZ.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleMntnBXY.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleMntnBZ.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleCaveAXY.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleCaveAZ.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleCaveBXY.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleCaveBZ.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleCaveCXY.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleCaveCZ.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleCaveHXY.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleCaveHZ.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleCustAXY.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleCustAZ.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleCustBXY.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleCustBZ.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleCustCXY.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleCustCZ.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleSkylandXY.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleSkylandZ.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleCloudAXY.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleCloudAZ.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleCloudBXY.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleCloudBZ.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleFltIsldAXY.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleFltIsldAZ.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleFltIsldBXY.getValue());
			%file.writeLine(PTG_Cmplx_EdtNosScaleFltIsldBZ.getValue());
			
			%file.writeLine(PTG_Cmplx_ChkEnabPseudoEqtr.getValue());
			%file.writeLine(PTG_Cmplx_EdtEquatorCustAY.getValue());
			%file.writeLine(PTG_Cmplx_EdtEquatorCustBY.getValue());
			%file.writeLine(PTG_Cmplx_EdtEquatorCustCY.getValue());
			%file.writeLine(PTG_Cmplx_EdtEquatorCaveY.getValue());
			%file.writeLine(PTG_Cmplx_EdtEquatorMntnY.getValue());
			%file.writeLine(PTG_Cmplx_EdtEquatorCloudY.getValue());
			%file.writeLine(PTG_Cmplx_EdtEquatorFltIsldY.getValue());
			
			
			
			//Save Mass Detail List
			
			%file.writeLine(">Mass Biome Detail List");
			
			for(%c = 0; %c < 9; %c++)
			{
				%BiomeNameStr = "Default Shore SubMarine CustomA CustomB CustomC CaveTop CaveBottom Mountains";
				%BioName = getWord(%BiomeNameStr,%c);
				
				switch$(%BioName)
				{
					case "Default":
						%relBioList = PTG_DetLst_TxtLst_DefBio;
						%relBioEnab = PTG_DetLst_ChkEnab_DefBio.getValue();
					case "Shore":
						%relBioList = PTG_DetLst_TxtLst_ShoreBio;
						%relBioEnab = PTG_DetLst_ChkEnab_ShoreBio.getValue();
					case "SubMarine":
						%relBioList = PTG_DetLst_TxtLst_SubMBio;
						%relBioEnab = PTG_DetLst_ChkEnab_SubMBio.getValue();
					case "CustomA":
						%relBioList = PTG_DetLst_TxtLst_CustBioA;
						%relBioEnab = PTG_DetLst_ChkEnab_CustBioA.getValue();
					case "CustomB":
						%relBioList = PTG_DetLst_TxtLst_CustBioB;
						%relBioEnab = PTG_DetLst_ChkEnab_CustBioB.getValue();
					case "CustomC":
						%relBioList = PTG_DetLst_TxtLst_CustBioC;
						%relBioEnab = PTG_DetLst_ChkEnab_CustBioC.getValue();
					case "CaveTop":
						%relBioList = PTG_DetLst_TxtLst_CaveTBio;
						%relBioEnab = PTG_DetLst_ChkEnab_CaveTBio.getValue();
					case "CaveBottom":
						%relBioList = PTG_DetLst_TxtLst_CaveBBio;
						%relBioEnab = PTG_DetLst_ChkEnab_CaveBBio.getValue();
					case "Mountains":
						%relBioList = PTG_DetLst_TxtLst_MntnBio;
						%relBioEnab = PTG_DetLst_ChkEnab_MntnBio.getValue();
				}
				
				%file.writeLine(">>" @ %BioName TAB %relBioEnab);
				
				for(%d = 0; %d < %relBioList.rowCount() && %d < 400; %d++)
				{
					%relDetail = %relBioList.getRowTextByID(%d);
					%relDetailData = getFields(%relDetail,1,5); //0 to 4
					
					%file.writeLine(%relDetailData);
				}
			}
			
			
			
			%file.writeLine(">End");
			%file.close();
			%file.delete();

			if($PTG_DefaultSetupPass !$= "Final") //server-sided (used cross network on local connections only)
			{
				if(PlayGUI.isAwake())
					%hideHud = true;

				canvas.setContent(NoHudGUI);
				scheduleNoQuota(22,0,ScreenShot,%fpBmp = "Config/Client/PTGv3/Presets/" @ %fileName @ ".jpg","JPG",1); //delay is necessary, otherwise screenshot won't show up in preview window, even after restart
				
				if(%hideHud)
					canvas.scheduleNoQuota(33,setContent,PlayGUI); //other functions must follow delay
				
				scheduleNoQuota(44,0,discoverFile,%fp);
				scheduleNoQuota(44,0,discoverFile,%fpBmp);

				canvas.scheduleNoQuota(44,pushDialog,PTG_Complex);
				scheduleNoQuota(55,0,PTG_GUI_PresetFuncs,"List");
				PTG_Cmplx_BmpPresetPrev.scheduleNoQuota(55,setBitmap,"Add-Ons/System_PTG/GUIs/NoPrint");
				scheduleNoQuota(55,0,CLIENTCMDPTG_ReceiveMsg,"Success","PTG: Preset Save Successful","Preset file \"" @ %fileName @ "\" and preview image saved successfully!");
			}
		
		
		//////////////////////////////////////////////////

		
		case "Load" or "LoadDefault":

			if((isObject(DatablockGroup) && DatablockGroup.getCount() > 0) && (!isObject(ServerConnection) || ServerConnection.getCount() == 0)) //prevent loading a preset if no datablock objects exist (player is probably not on a server, thus no reason to attempt to load objects)
			{
				CLIENTCMDPTG_ReceiveMsg("Error","PTG: Preset Load Failed","Presets can only be loaded into the GUI when you are on a server.");
				return;
			}
			
			if(PTG_Cmplx_TxtListPresets.getValue() $= "<<Default Settings>>" || PTG_Cmplx_TxtListPresets.getSelectedID() == 0 || %action $= "LoadDefault")
			{
				%def = true;
				
				if(!isFile(%fp = "config/client/PTGv3/Presets/Default.txt"))
					%fp = "Add-Ons/System_PTG/Presets/Default.txt";
			}
			else
			{
				if((%fileName = PTG_Cmplx_TxtListPresets.getValue()) $= "")
				{
					CLIENTCMDPTG_ReceiveMsg("Error","PTG: No Preset Selected","No preset selected!");
					return;
				}
				
				if(isFile(%fp = findFirstFile("Config/Client/PTGv3/Presets/" @ (%fileName = PTG_Cmplx_TxtListPresets.getValue()) @ ".txt")))
					%fileExists = true;
				else if(isFile(%fp = findFirstFile("Config/Client/PTGv3/Presets/" @ "*/" @ %fileName @ ".txt")))
					%fileExists = true;
				
				if(!%fileExists)
				{
					CLIENTCMDPTG_ReceiveMsg("Error","PTG: Preset Load Failed","Preset file \"" @ %fileName @ "\" doesn't exist!");
					return;
				}
			}
			
			%file = new FileObject();
			%file.openForRead(%fp);
			
			%file.readLine();
			%file.readLine();
			%file.readLine();
			%colorSetsMatch = true;
			
			for(%c = 0; %c < 64; %c++)
			{
				%tmpColorArr[%c] = %currCol = %file.readLine();
				
				if(%currCol !$= getColorIDTable(%c))
					%colorSetsMatch = false;
			}
			
			%file.readLine();
			%DefMCol = getColorI(getColorIDTable(0)); //default red color
			
			
			%file.readLine(); //Setup
			PTG_Cmplx_EdtSeed.setValue(%file.readLine()); 
			
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpTerBr.BrickID = %brID;
				PTG_Cmplx_BmpTerBr.setBitmap(%brID.iconName);
				PTG_Cmplx_BmpTerBr.ModTer = %file.readLine();
				
				PTG_Cmplx_BmpTerPri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpTerPri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwTerCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpTerBr.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwTerPri.setColor(%colStr);
					PTG_Cmplx_SwTerCol.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpTerBr.BrickID = "";
				PTG_Cmplx_BmpTerBr.setBitmap("base/client/ui/brickicons/Unknown");
				PTG_Cmplx_BmpTerBr.ModTer = false;
				
				PTG_Cmplx_BmpTerPri.PrintID = "";
					PTG_Cmplx_BmpTerPri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwTerCol.colorID = "";
					PTG_Cmplx_BmpTerBr.mColor = %DefMCol;
					PTG_Cmplx_SwTerPri.setColor(%DefMCol);
					PTG_Cmplx_SwTerCol.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
				%file.readLine();
			}
			
			PTG_Cmplx_PopUpTerType.setSelected(%file.readLine()); //mClamp
			PTG_Cmplx_ChkRadialGrid.setValue(%file.readLine()); 
			PTG_Cmplx_PopUpModTerType.setSelected(%file.readLine()); //mClamp
			PTG_Cmplx_ChkGradualGen.setValue(%file.readLine());
			PTG_Cmplx_EnabAutoSave.setValue(%file.readLine());
			
			PTG_Cmplx_EdtGridXStart.setValue(%file.readLine()); 
			PTG_Cmplx_EdtGridYStart.setValue(%file.readLine()); 
			PTG_Cmplx_EdtGridXEnd.setValue(%file.readLine()); 
			PTG_Cmplx_EdtGridYEnd.setValue(%file.readLine());
			PTG_Cmplx_ChkEdgeFallOff.setValue(%file.readLine()); 
			PTG_Cmplx_EdtEdgeFallOffDist.setValue(%file.readLine()); 
			
			PTG_Cmplx_EnabInfiniteTer.setValue(%file.readLine());
			PTG_Cmplx_SldrChRadP.setValue(%file.readLine());
			PTG_Cmplx_SldrChRadSA.setValue(%file.readLine());
			PTG_Cmplx_ChkClrDistChunks.setValue(%file.readLine());
			
			
			%file.readLine(); //Features
			PTG_Cmplx_BmpTerDirtPri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
				PTG_Cmplx_BmpTerDirtPri.setBitmap(getField($PTG_PrintArr[%priID],1));
			PTG_Cmplx_SwTerDirtCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwTerDirtPri.setColor(%colStr = getColorI(getColorIDTable(%colID)));
				PTG_Cmplx_SwTerDirtCol.setColor(%colStr);
			PTG_Cmplx_BmpTerStonePri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
				PTG_Cmplx_BmpTerStonePri.setBitmap(getField($PTG_PrintArr[%priID],1));
			PTG_Cmplx_SwTerStoneCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwTerStonePri.setColor(%colStr = getColorI(getColorIDTable(%colID)));
				PTG_Cmplx_SwTerStoneCol.setColor(%colStr);

			PTG_Cmplx_SldrWaterLevel.setValue(%file.readLine());
			PTG_Cmplx_SldrSandLevel.setValue(%file.readLine());
			PTG_Cmplx_SldrTerZOffset.setValue(%file.readLine());
			PTG_Cmplx_ChkEnabCnctLakes.setValue(%file.readLine());
			PTG_Cmplx_ChkEnabPlateCap.setValue(%file.readLine());
			PTG_Cmplx_ChkDirtSameBioTer.setValue(%file.readLine());
			PTG_Cmplx_ChkShoreSameCustBiome.setValue(%file.readLine());
			PTG_Cmplx_ChkDisWater.setValue(%file.readLine());
			
			PTG_Cmplx_ChkEnabDetails.setValue(%file.readLine());
			PTG_Cmplx_SldrDetailFreq.setValue(%file.readLine());
			PTG_Cmplx_ChkEnabCustABio.setValue(%file.readLine());
			PTG_Cmplx_ChkEnabCustBBio.setValue(%file.readLine());
			PTG_Cmplx_ChkEnabCustCBio.setValue(%file.readLine());
			PTG_Cmplx_ChkAutoHideSpawns.setValue(%file.readLine());
			PTG_Cmplx_ChkEnabFltIsldDetails.setValue(%file.readLine());
			
			PTG_Cmplx_ChkEnabMntns.setValue(%file.readLine());
			PTG_Cmplx_ChkEnabMntnSnow.setValue(%file.readLine());
			PTG_Cmplx_SldrMntnSnowLevel.setValue(%file.readLine());
			PTG_Cmplx_SldrMntnZSnapMult.setValue(%file.readLine());
			PTG_Cmplx_SldrMntnZMult.setValue(%file.readLine());
			
			PTG_Cmplx_ChkEnabCaves.setValue(%file.readLine()); 
			PTG_Cmplx_SldrCaveZOffset.setValue(%file.readLine()); 
			
			
			PTG_Cmplx_ChkEnabClouds.setValue(%file.readLine());
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpCloudBr.BrickID = %brID;
				PTG_Cmplx_BmpCloudBr.setBitmap(%brID.iconName);
				PTG_Cmplx_BmpCloudBr.ModTer = %file.readLine();
				
				PTG_Cmplx_BmpCloudPri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpCloudPri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwCloudCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpCloudBr.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwCloudPri.setColor(%colStr);
					PTG_Cmplx_SwCloudCol.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpCloudBr.BrickID = "";
				PTG_Cmplx_BmpCloudBr.setBitmap("base/client/ui/brickicons/Unknown");
				PTG_Cmplx_BmpCloudBr.ModTer = false;
				
				PTG_Cmplx_BmpCloudPri.PrintID = "";
					PTG_Cmplx_BmpCloudPri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwCloudCol.colorID = "";
					PTG_Cmplx_BmpCloudBr.mColor = %DefMCol;
					PTG_Cmplx_SwCloudPri.setColor(%DefMCol);
					PTG_Cmplx_SwCloudCol.setColor(%DefMCol);
					
				PTG_Cmplx_ChkEnabClouds.setValue(0);
				
				%file.readLine();
				%file.readLine();
				%file.readLine();
			}
			PTG_Cmplx_SldrCloudZOffset.setValue(%file.readLine());
			PTG_Cmplx_ChkEnabCloudColl.setValue(%file.readLine());
			PTG_Cmplx_PopUpModTerType_clouds.setSelected(%file.readLine());
			
			PTG_Cmplx_ChkEnabFltIslds.setValue(%file.readLine());
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpFltIsldsBr.BrickID = %brID;
				PTG_Cmplx_BmpFltIsldsBr.setBitmap(%brID.iconName);
				PTG_Cmplx_BmpFltIsldsBr.ModTer = %file.readLine();
				
				PTG_Cmplx_BmpFltIsldsPri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpFltIsldsPri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwFltIsldsCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpFltIsldsBr.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwFltIsldsPri.setColor(%colStr);
					PTG_Cmplx_SwFltIsldsCol.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpFltIsldsBr.BrickID = "";
				PTG_Cmplx_BmpFltIsldsBr.setBitmap("base/client/ui/brickicons/Unknown");
				PTG_Cmplx_BmpFltIsldsBr.ModTer = false;
				
				PTG_Cmplx_BmpFltIsldsPri.PrintID = "";
					PTG_Cmplx_BmpFltIsldsPri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwFltIsldsCol.colorID = "";
					PTG_Cmplx_BmpFltIsldsBr.mColor = %DefMCol;
					PTG_Cmplx_SwFltIsldsPri.setColor(%DefMCol);
					PTG_Cmplx_SwFltIsldsCol.setColor(%DefMCol);
					
				PTG_Cmplx_ChkEnabFltIslds.setValue(0);
				
				%file.readLine();
				%file.readLine();
				%file.readLine();
			}
			PTG_Cmplx_SldrFltIsldsAZOffset.setValue(%file.readLine()); 
			PTG_Cmplx_SldrFltIsldsBZOffset.setValue(%file.readLine());
			PTG_Cmplx_BmpFltIsldsDirtPri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
				PTG_Cmplx_BmpFltIsldsDirtPri.setBitmap(getField($PTG_PrintArr[%priID],1));
			PTG_Cmplx_SwFltIsldsDirtCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwFltIsldsDirtPri.setColor(%colStr = getColorI(getColorIDTable(%colID)));
				PTG_Cmplx_SwFltIsldsDirtCol.setColor(%colStr);
			PTG_Cmplx_BmpFltIsldsStonePri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
				PTG_Cmplx_BmpFltIsldsStonePri.setBitmap(getField($PTG_PrintArr[%priID],1));
			PTG_Cmplx_SwFltIsldsStoneCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwFltIsldsStonePri.setColor(%colStr = getColorI(getColorIDTable(%colID)));
				PTG_Cmplx_SwFltIsldsStoneCol.setColor(%colStr);
			PTG_Cmplx_PopUpModTerType_fltislds.setSelected(%file.readLine());
			
			PTG_Cmplx_ChkEnabBnds.setValue(%file.readLine()); 
			PTG_Cmplx_BmpBndsWallPri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
				PTG_Cmplx_BmpBndsWallPri.setBitmap(getField($PTG_PrintArr[%priID],1));
			PTG_Cmplx_SwBndsWallCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwBndsWallPri.setColor(%colStr = getColorI(getColorIDTable(%colID)));
				PTG_Cmplx_SwBndsWallCol.setColor(%colStr);
			PTG_Cmplx_BmpBndsCeilPri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
				PTG_Cmplx_BmpBndsCeilPri.setBitmap(getField($PTG_PrintArr[%priID],1));
			PTG_Cmplx_SwBndsCeilCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwBndsCeilPri.setColor(%colStr = getColorI(getColorIDTable(%colID)));
				PTG_Cmplx_SwBndsCeilCol.setColor(%colStr);
			PTG_Cmplx_SldrBndsHAboveTer.setValue(%file.readLine()); 
			PTG_Cmplx_SldrBndsH.setValue(%file.readLine()); 
			PTG_Cmplx_ChkBndsRelToTer.setValue(%file.readLine());
			PTG_Cmplx_ChkBndsStrtRelOffset.setValue(%file.readLine());
			PTG_Cmplx_ChkEnabBndsCeil.setValue(%file.readLine());
			PTG_Cmplx_ChkBndsUseStatic.setValue(%file.readLine());
			PTG_Cmplx_ChkBndsInvisStatic.setValue(%file.readLine());
			
			
			%file.readLine(); //Build-Loading
			PTG_Cmplx_ChkEnabBuildLoad.setValue(%file.readLine()); 
			PTG_Cmplx_ChkAllowFlatAreas.setValue(%file.readLine()); 
			PTG_Cmplx_ChkGenDetFlatAreas.setValue(%file.readLine()); 
			PTG_Cmplx_ChkBldLdUseTerHMax.setValue(%file.readLine()); 
			PTG_Cmplx_SldrFlatAreaFreq.setValue(%file.readLine()); 
			PTG_Cmplx_PopUpGridSizeSmall.setSelected(%file.readLine()); 
			PTG_Cmplx_PopUpGridSizeLarge.setSelected(%file.readLine()); 
			
			
			%file.readLine(); //Default Biome
			//PTG_Cmplx_BmpBioDefTerPri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
			//	PTG_Cmplx_BmpBioDefTerPri.setBitmap(getField($PTG_PrintArr[%priID],1));
			//PTG_Cmplx_SwBioDefTerCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
			//	PTG_Cmplx_SwBioDefTerPri.setColor(%colStr = getColorI(getColorIDTable(%colID)));
			//	PTG_Cmplx_SwBioDefTerCol.setColor(%colStr);
			PTG_Cmplx_BmpBioDefWatPri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
				PTG_Cmplx_BmpBioDefWatPri.setBitmap(getField($PTG_PrintArr[%priID],1));
			PTG_Cmplx_SwBioDefWatCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwBioDefWatPri.setColor(%colStr = getColorI(getColorIDTable(%colID)));
				PTG_Cmplx_SwBioDefWatCol.setColor(%colStr);
			//PTG_Cmplx_CheckAllMntnsDefBio.setValue(%file.readLine()); 
			PTG_Cmplx_PopUpWaterType_Def.setSelected(%file.readLine()); //mClamp
			
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioDefDet0Br.BrickID = %brID;
				PTG_Cmplx_BmpBioDefDet0Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioDefDet0Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioDefDet0Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioDefDet0Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioDefDet0Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioDefDet0Pri.setColor(%colStr);
					PTG_Cmplx_SwBioDefDet0Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioDefDet0Br.BrickID = "";
				PTG_Cmplx_BmpBioDefDet0Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioDefDet0Pri.PrintID = "";
					PTG_Cmplx_BmpBioDefDet0Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioDefDet0Col.colorID = "";
					PTG_Cmplx_BmpBioDefDet0Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioDefDet0Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioDefDet0Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioDefDet1Br.BrickID = %brID;
				PTG_Cmplx_BmpBioDefDet1Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioDefDet1Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioDefDet1Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioDefDet1Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioDefDet1Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioDefDet1Pri.setColor(%colStr);
					PTG_Cmplx_SwBioDefDet1Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioDefDet1Br.BrickID = "";
				PTG_Cmplx_BmpBioDefDet1Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioDefDet1Pri.PrintID = "";
					PTG_Cmplx_BmpBioDefDet1Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioDefDet1Col.colorID = "";
					PTG_Cmplx_BmpBioDefDet1Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioDefDet1Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioDefDet1Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioDefDet2Br.BrickID = %brID;
				PTG_Cmplx_BmpBioDefDet2Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioDefDet2Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioDefDet2Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioDefDet2Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioDefDet2Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioDefDet2Pri.setColor(%colStr);
					PTG_Cmplx_SwBioDefDet2Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioDefDet2Br.BrickID = "";
				PTG_Cmplx_BmpBioDefDet2Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioDefDet2Pri.PrintID = "";
					PTG_Cmplx_BmpBioDefDet2Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioDefDet2Col.colorID = "";
					PTG_Cmplx_BmpBioDefDet2Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioDefDet2Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioDefDet2Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioDefDet3Br.BrickID = %brID;
				PTG_Cmplx_BmpBioDefDet3Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioDefDet3Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioDefDet3Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioDefDet3Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioDefDet3Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioDefDet3Pri.setColor(%colStr);
					PTG_Cmplx_SwBioDefDet3Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioDefDet3Br.BrickID = "";
				PTG_Cmplx_BmpBioDefDet3Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioDefDet3Pri.PrintID = "";
					PTG_Cmplx_BmpBioDefDet3Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioDefDet3Col.colorID = "";
					PTG_Cmplx_BmpBioDefDet3Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioDefDet3Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioDefDet3Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioDefDet4Br.BrickID = %brID;
				PTG_Cmplx_BmpBioDefDet4Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioDefDet4Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioDefDet4Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioDefDet4Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioDefDet4Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioDefDet4Pri.setColor(%colStr);
					PTG_Cmplx_SwBioDefDet4Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioDefDet4Br.BrickID = "";
				PTG_Cmplx_BmpBioDefDet4Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioDefDet4Pri.PrintID = "";
					PTG_Cmplx_BmpBioDefDet4Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioDefDet4Col.colorID = "";
					PTG_Cmplx_BmpBioDefDet4Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioDefDet4Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioDefDet4Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioDefDet5Br.BrickID = %brID;
				PTG_Cmplx_BmpBioDefDet5Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioDefDet5Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioDefDet5Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioDefDet5Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioDefDet5Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioDefDet5Pri.setColor(%colStr);
					PTG_Cmplx_SwBioDefDet5Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioDefDet5Br.BrickID = "";
				PTG_Cmplx_BmpBioDefDet5Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioDefDet5Pri.PrintID = "";
					PTG_Cmplx_BmpBioDefDet5Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioDefDet5Col.colorID = "";
					PTG_Cmplx_BmpBioDefDet5Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioDefDet5Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioDefDet5Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioDefDet6Br.BrickID = %brID;
				PTG_Cmplx_BmpBioDefDet6Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioDefDet6Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioDefDet6Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioDefDet6Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioDefDet6Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioDefDet6Pri.setColor(%colStr);
					PTG_Cmplx_SwBioDefDet6Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioDefDet6Br.BrickID = "";
				PTG_Cmplx_BmpBioDefDet6Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioDefDet6Pri.PrintID = "";
					PTG_Cmplx_BmpBioDefDet6Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioDefDet6Col.colorID = "";
					PTG_Cmplx_BmpBioDefDet6Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioDefDet6Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioDefDet6Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioDefDet7Br.BrickID = %brID;
				PTG_Cmplx_BmpBioDefDet7Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioDefDet7Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioDefDet7Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioDefDet7Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioDefDet7Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioDefDet7Pri.setColor(%colStr);
					PTG_Cmplx_SwBioDefDet7Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioDefDet7Br.BrickID = "";
				PTG_Cmplx_BmpBioDefDet7Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioDefDet7Pri.PrintID = "";
					PTG_Cmplx_BmpBioDefDet7Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioDefDet7Col.colorID = "";
					PTG_Cmplx_BmpBioDefDet7Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioDefDet7Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioDefDet7Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioDefDet8Br.BrickID = %brID;
				PTG_Cmplx_BmpBioDefDet8Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioDefDet8Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioDefDet8Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioDefDet8Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioDefDet8Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioDefDet8Pri.setColor(%colStr);
					PTG_Cmplx_SwBioDefDet8Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioDefDet8Br.BrickID = "";
				PTG_Cmplx_BmpBioDefDet8Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioDefDet8Pri.PrintID = "";
					PTG_Cmplx_BmpBioDefDet8Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioDefDet8Col.colorID = "";
					PTG_Cmplx_BmpBioDefDet8Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioDefDet8Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioDefDet8Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioDefDet9Br.BrickID = %brID;
				PTG_Cmplx_BmpBioDefDet9Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioDefDet9Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioDefDet9Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioDefDet9Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioDefDet9Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioDefDet9Pri.setColor(%colStr);
					PTG_Cmplx_SwBioDefDet9Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioDefDet9Br.BrickID = "";
				PTG_Cmplx_BmpBioDefDet9Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioDefDet9Pri.PrintID = "";
					PTG_Cmplx_BmpBioDefDet9Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioDefDet9Col.colorID = "";
					PTG_Cmplx_BmpBioDefDet9Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioDefDet9Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioDefDet9Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioDefDet10Br.BrickID = %brID;
				PTG_Cmplx_BmpBioDefDet10Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioDefDet10Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioDefDet10Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioDefDet10Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioDefDet10Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioDefDet10Pri.setColor(%colStr);
					PTG_Cmplx_SwBioDefDet10Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioDefDet10Br.BrickID = "";
				PTG_Cmplx_BmpBioDefDet10Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioDefDet10Pri.PrintID = "";
					PTG_Cmplx_BmpBioDefDet10Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioDefDet10Col.colorID = "";
					PTG_Cmplx_BmpBioDefDet10Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioDefDet10Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioDefDet10Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioDefDet11Br.BrickID = %brID;
				PTG_Cmplx_BmpBioDefDet11Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioDefDet11Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioDefDet11Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioDefDet11Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioDefDet11Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioDefDet11Pri.setColor(%colStr);
					PTG_Cmplx_SwBioDefDet11Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioDefDet11Br.BrickID = "";
				PTG_Cmplx_BmpBioDefDet11Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioDefDet11Pri.PrintID = "";
					PTG_Cmplx_BmpBioDefDet11Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioDefDet11Col.colorID = "";
					PTG_Cmplx_BmpBioDefDet11Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioDefDet11Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioDefDet11Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioDefdet12Br.BrickID = %brID;
				PTG_Cmplx_BmpBioDefdet12Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioDefdet12Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioDefdet12Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioDefdet12Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioDefdet12Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioDefdet12Pri.setColor(%colStr);
					PTG_Cmplx_SwBioDefdet12Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioDefdet12Br.BrickID = "";
				PTG_Cmplx_BmpBioDefdet12Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioDefdet12Pri.PrintID = "";
					PTG_Cmplx_BmpBioDefdet12Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioDefdet12Col.colorID = "";
					PTG_Cmplx_BmpBioDefdet12Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioDefdet12Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioDefdet12Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioDefdet13Br.BrickID = %brID;
				PTG_Cmplx_BmpBioDefdet13Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioDefdet13Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioDefdet13Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioDefdet13Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioDefdet13Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioDefdet13Pri.setColor(%colStr);
					PTG_Cmplx_SwBioDefdet13Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioDefdet13Br.BrickID = "";
				PTG_Cmplx_BmpBioDefdet13Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioDefdet13Pri.PrintID = "";
					PTG_Cmplx_BmpBioDefdet13Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioDefdet13Col.colorID = "";
					PTG_Cmplx_BmpBioDefdet13Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioDefdet13Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioDefdet13Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioDefdet14Br.BrickID = %brID;
				PTG_Cmplx_BmpBioDefdet14Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioDefdet14Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioDefdet14Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioDefdet14Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioDefdet14Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioDefdet14Pri.setColor(%colStr);
					PTG_Cmplx_SwBioDefdet14Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioDefdet14Br.BrickID = "";
				PTG_Cmplx_BmpBioDefdet14Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioDefdet14Pri.PrintID = "";
					PTG_Cmplx_BmpBioDefdet14Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioDefdet14Col.colorID = "";
					PTG_Cmplx_BmpBioDefdet14Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioDefdet14Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioDefdet14Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioDefdet15Br.BrickID = %brID;
				PTG_Cmplx_BmpBioDefdet15Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioDefdet15Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioDefdet15Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioDefdet15Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioDefdet15Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioDefdet15Pri.setColor(%colStr);
					PTG_Cmplx_SwBioDefdet15Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioDefdet15Br.BrickID = "";
				PTG_Cmplx_BmpBioDefdet15Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioDefdet15Pri.PrintID = "";
					PTG_Cmplx_BmpBioDefdet15Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioDefdet15Col.colorID = "";
					PTG_Cmplx_BmpBioDefdet15Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioDefdet15Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioDefdet15Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioDefdet16Br.BrickID = %brID;
				PTG_Cmplx_BmpBioDefdet16Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioDefdet16Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioDefdet16Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioDefdet16Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioDefdet16Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioDefdet16Pri.setColor(%colStr);
					PTG_Cmplx_SwBioDefdet16Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioDefdet16Br.BrickID = "";
				PTG_Cmplx_BmpBioDefdet16Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioDefdet16Pri.PrintID = "";
					PTG_Cmplx_BmpBioDefdet16Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioDefdet16Col.colorID = "";
					PTG_Cmplx_BmpBioDefdet16Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioDefdet16Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioDefdet16Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioDefdet17Br.BrickID = %brID;
				PTG_Cmplx_BmpBioDefdet17Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioDefdet17Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioDefdet17Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioDefdet17Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioDefdet17Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioDefdet17Pri.setColor(%colStr);
					PTG_Cmplx_SwBioDefdet17Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioDefdet17Br.BrickID = "";
				PTG_Cmplx_BmpBioDefdet17Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioDefdet17Pri.PrintID = "";
					PTG_Cmplx_BmpBioDefdet17Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioDefdet17Col.colorID = "";
					PTG_Cmplx_BmpBioDefdet17Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioDefdet17Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioDefdet17Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			
			
			%file.readLine(); //Shore Biome
			PTG_Cmplx_BmpBioShoreTerPri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
				PTG_Cmplx_BmpBioShoreTerPri.setBitmap(getField($PTG_PrintArr[%priID],1));
			PTG_Cmplx_SwBioShoreTerCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwBioShoreTerPri.setColor(%colStr = getColorI(getColorIDTable(%colID)));
				PTG_Cmplx_SwBioShoreTerCol.setColor(%colStr);
			
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioShoreDet0Br.BrickID = %brID;
				PTG_Cmplx_BmpBioShoreDet0Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioShoreDet0Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioShoreDet0Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioShoreDet0Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioShoreDet0Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioShoreDet0Pri.setColor(%colStr);
					PTG_Cmplx_SwBioShoreDet0Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioShoreDet0Br.BrickID = "";
				PTG_Cmplx_BmpBioShoreDet0Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioShoreDet0Pri.PrintID = "";
					PTG_Cmplx_BmpBioShoreDet0Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioShoreDet0Col.colorID = "";
					PTG_Cmplx_BmpBioShoreDet0Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioShoreDet0Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioShoreDet0Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioShoreDet1Br.BrickID = %brID;
				PTG_Cmplx_BmpBioShoreDet1Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioShoreDet1Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioShoreDet1Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioShoreDet1Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioShoreDet1Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioShoreDet1Pri.setColor(%colStr);
					PTG_Cmplx_SwBioShoreDet1Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioShoreDet1Br.BrickID = "";
				PTG_Cmplx_BmpBioShoreDet1Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioShoreDet1Pri.PrintID = "";
					PTG_Cmplx_BmpBioShoreDet1Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioShoreDet1Col.colorID = "";
					PTG_Cmplx_BmpBioShoreDet1Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioShoreDet1Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioShoreDet1Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioShoreDet2Br.BrickID = %brID;
				PTG_Cmplx_BmpBioShoreDet2Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioShoreDet2Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioShoreDet2Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioShoreDet2Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioShoreDet2Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioShoreDet2Pri.setColor(%colStr);
					PTG_Cmplx_SwBioShoreDet2Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioShoreDet2Br.BrickID = "";
				PTG_Cmplx_BmpBioShoreDet2Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioShoreDet2Pri.PrintID = "";
					PTG_Cmplx_BmpBioShoreDet2Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioShoreDet2Col.colorID = "";
					PTG_Cmplx_BmpBioShoreDet2Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioShoreDet2Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioShoreDet2Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioShoreDet3Br.BrickID = %brID;
				PTG_Cmplx_BmpBioShoreDet3Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioShoreDet3Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioShoreDet3Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioShoreDet3Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioShoreDet3Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioShoreDet3Pri.setColor(%colStr);
					PTG_Cmplx_SwBioShoreDet3Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioShoreDet3Br.BrickID = "";
				PTG_Cmplx_BmpBioShoreDet3Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioShoreDet3Pri.PrintID = "";
					PTG_Cmplx_BmpBioShoreDet3Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioShoreDet3Col.colorID = "";
					PTG_Cmplx_BmpBioShoreDet3Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioShoreDet3Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioShoreDet3Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioShoreDet4Br.BrickID = %brID;
				PTG_Cmplx_BmpBioShoreDet4Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioShoreDet4Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioShoreDet4Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioShoreDet4Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioShoreDet4Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioShoreDet4Pri.setColor(%colStr);
					PTG_Cmplx_SwBioShoreDet4Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioShoreDet4Br.BrickID = "";
				PTG_Cmplx_BmpBioShoreDet4Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioShoreDet4Pri.PrintID = "";
					PTG_Cmplx_BmpBioShoreDet4Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioShoreDet4Col.colorID = "";
					PTG_Cmplx_BmpBioShoreDet4Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioShoreDet4Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioShoreDet4Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioShoreDet5Br.BrickID = %brID;
				PTG_Cmplx_BmpBioShoreDet5Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioShoreDet5Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioShoreDet5Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioShoreDet5Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioShoreDet5Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioShoreDet5Pri.setColor(%colStr);
					PTG_Cmplx_SwBioShoreDet5Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioShoreDet5Br.BrickID = "";
				PTG_Cmplx_BmpBioShoreDet5Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioShoreDet5Pri.PrintID = "";
					PTG_Cmplx_BmpBioShoreDet5Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioShoreDet5Col.colorID = "";
					PTG_Cmplx_BmpBioShoreDet5Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioShoreDet5Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioShoreDet5Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioShoreDet6Br.BrickID = %brID;
				PTG_Cmplx_BmpBioShoreDet6Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioShoreDet6Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioShoreDet6Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioShoreDet6Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioShoreDet6Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioShoreDet6Pri.setColor(%colStr);
					PTG_Cmplx_SwBioShoreDet6Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioShoreDet6Br.BrickID = "";
				PTG_Cmplx_BmpBioShoreDet6Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioShoreDet6Pri.PrintID = "";
					PTG_Cmplx_BmpBioShoreDet6Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioShoreDet6Col.colorID = "";
					PTG_Cmplx_BmpBioShoreDet6Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioShoreDet6Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioShoreDet6Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioShoreDet7Br.BrickID = %brID;
				PTG_Cmplx_BmpBioShoreDet7Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioShoreDet7Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioShoreDet7Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioShoreDet7Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioShoreDet7Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioShoreDet7Pri.setColor(%colStr);
					PTG_Cmplx_SwBioShoreDet7Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioShoreDet7Br.BrickID = "";
				PTG_Cmplx_BmpBioShoreDet7Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioShoreDet7Pri.PrintID = "";
					PTG_Cmplx_BmpBioShoreDet7Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioShoreDet7Col.colorID = "";
					PTG_Cmplx_BmpBioShoreDet7Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioShoreDet7Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioShoreDet7Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioShoreDet8Br.BrickID = %brID;
				PTG_Cmplx_BmpBioShoreDet8Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioShoreDet8Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioShoreDet8Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioShoreDet8Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioShoreDet8Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioShoreDet8Pri.setColor(%colStr);
					PTG_Cmplx_SwBioShoreDet8Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioShoreDet8Br.BrickID = "";
				PTG_Cmplx_BmpBioShoreDet8Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioShoreDet8Pri.PrintID = "";
					PTG_Cmplx_BmpBioShoreDet8Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioShoreDet8Col.colorID = "";
					PTG_Cmplx_BmpBioShoreDet8Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioShoreDet8Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioShoreDet8Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioShoreDet9Br.BrickID = %brID;
				PTG_Cmplx_BmpBioShoreDet9Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioShoreDet9Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioShoreDet9Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioShoreDet9Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioShoreDet9Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioShoreDet9Pri.setColor(%colStr);
					PTG_Cmplx_SwBioShoreDet9Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioShoreDet9Br.BrickID = "";
				PTG_Cmplx_BmpBioShoreDet9Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioShoreDet9Pri.PrintID = "";
					PTG_Cmplx_BmpBioShoreDet9Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioShoreDet9Col.colorID = "";
					PTG_Cmplx_BmpBioShoreDet9Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioShoreDet9Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioShoreDet9Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioShoreDet10Br.BrickID = %brID;
				PTG_Cmplx_BmpBioShoreDet10Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioShoreDet10Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioShoreDet10Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioShoreDet10Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioShoreDet10Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioShoreDet10Pri.setColor(%colStr);
					PTG_Cmplx_SwBioShoreDet10Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioShoreDet10Br.BrickID = "";
				PTG_Cmplx_BmpBioShoreDet10Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioShoreDet10Pri.PrintID = "";
					PTG_Cmplx_BmpBioShoreDet10Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioShoreDet10Col.colorID = "";
					PTG_Cmplx_BmpBioShoreDet10Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioShoreDet10Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioShoreDet10Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioShoreDet11Br.BrickID = %brID;
				PTG_Cmplx_BmpBioShoreDet11Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioShoreDet11Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioShoreDet11Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioShoreDet11Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioShoreDet11Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioShoreDet11Pri.setColor(%colStr);
					PTG_Cmplx_SwBioShoreDet11Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioShoreDet11Br.BrickID = "";
				PTG_Cmplx_BmpBioShoreDet11Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioShoreDet11Pri.PrintID = "";
					PTG_Cmplx_BmpBioShoreDet11Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioShoreDet11Col.colorID = "";
					PTG_Cmplx_BmpBioShoreDet11Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioShoreDet11Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioShoreDet11Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioShoredet12Br.BrickID = %brID;
				PTG_Cmplx_BmpBioShoredet12Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioShoredet12Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioShoredet12Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioShoredet12Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioShoredet12Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioShoredet12Pri.setColor(%colStr);
					PTG_Cmplx_SwBioShoredet12Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioShoredet12Br.BrickID = "";
				PTG_Cmplx_BmpBioShoredet12Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioShoredet12Pri.PrintID = "";
					PTG_Cmplx_BmpBioShoredet12Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioShoredet12Col.colorID = "";
					PTG_Cmplx_BmpBioShoredet12Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioShoredet12Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioShoredet12Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioShoredet13Br.BrickID = %brID;
				PTG_Cmplx_BmpBioShoredet13Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioShoredet13Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioShoredet13Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioShoredet13Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioShoredet13Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioShoredet13Pri.setColor(%colStr);
					PTG_Cmplx_SwBioShoredet13Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioShoredet13Br.BrickID = "";
				PTG_Cmplx_BmpBioShoredet13Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioShoredet13Pri.PrintID = "";
					PTG_Cmplx_BmpBioShoredet13Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioShoredet13Col.colorID = "";
					PTG_Cmplx_BmpBioShoredet13Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioShoredet13Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioShoredet13Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioShoredet14Br.BrickID = %brID;
				PTG_Cmplx_BmpBioShoredet14Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioShoredet14Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioShoredet14Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioShoredet14Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioShoredet14Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioShoredet14Pri.setColor(%colStr);
					PTG_Cmplx_SwBioShoredet14Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioShoredet14Br.BrickID = "";
				PTG_Cmplx_BmpBioShoredet14Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioShoredet14Pri.PrintID = "";
					PTG_Cmplx_BmpBioShoredet14Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioShoredet14Col.colorID = "";
					PTG_Cmplx_BmpBioShoredet14Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioShoredet14Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioShoredet14Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioShoredet15Br.BrickID = %brID;
				PTG_Cmplx_BmpBioShoredet15Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioShoredet15Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioShoredet15Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioShoredet15Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioShoredet15Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioShoredet15Pri.setColor(%colStr);
					PTG_Cmplx_SwBioShoredet15Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioShoredet15Br.BrickID = "";
				PTG_Cmplx_BmpBioShoredet15Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioShoredet15Pri.PrintID = "";
					PTG_Cmplx_BmpBioShoredet15Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioShoredet15Col.colorID = "";
					PTG_Cmplx_BmpBioShoredet15Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioShoredet15Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioShoredet15Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioShoredet16Br.BrickID = %brID;
				PTG_Cmplx_BmpBioShoredet16Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioShoredet16Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioShoredet16Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioShoredet16Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioShoredet16Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioShoredet16Pri.setColor(%colStr);
					PTG_Cmplx_SwBioShoredet16Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioShoredet16Br.BrickID = "";
				PTG_Cmplx_BmpBioShoredet16Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioShoredet16Pri.PrintID = "";
					PTG_Cmplx_BmpBioShoredet16Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioShoredet16Col.colorID = "";
					PTG_Cmplx_BmpBioShoredet16Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioShoredet16Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioShoredet16Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioShoredet17Br.BrickID = %brID;
				PTG_Cmplx_BmpBioShoredet17Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioShoredet17Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioShoredet17Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioShoredet17Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioShoredet17Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioShoredet17Pri.setColor(%colStr);
					PTG_Cmplx_SwBioShoredet17Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioShoredet17Br.BrickID = "";
				PTG_Cmplx_BmpBioShoredet17Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioShoredet17Pri.PrintID = "";
					PTG_Cmplx_BmpBioShoredet17Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioShoredet17Col.colorID = "";
					PTG_Cmplx_BmpBioShoredet17Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioShoredet17Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioShoredet17Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			
			
			%file.readLine(); //Submarine Biome
			PTG_Cmplx_BmpBioSubMTerPri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
				PTG_Cmplx_BmpBioSubMTerPri.setBitmap(getField($PTG_PrintArr[%priID],1));
			PTG_Cmplx_SwBioSubMTerCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwBioSubMTerPri.setColor(%colStr = getColorI(getColorIDTable(%colID)));
				PTG_Cmplx_SwBioSubMTerCol.setColor(%colStr);
			
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioSubMDet0Br.BrickID = %brID;
				PTG_Cmplx_BmpBioSubMDet0Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioSubMDet0Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioSubMDet0Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioSubMDet0Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioSubMDet0Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioSubMDet0Pri.setColor(%colStr);
					PTG_Cmplx_SwBioSubMDet0Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioSubMDet0Br.BrickID = "";
				PTG_Cmplx_BmpBioSubMDet0Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioSubMDet0Pri.PrintID = "";
					PTG_Cmplx_BmpBioSubMDet0Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioSubMDet0Col.colorID = "";
					PTG_Cmplx_BmpBioSubMDet0Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioSubMDet0Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioSubMDet0Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioSubMDet1Br.BrickID = %brID;
				PTG_Cmplx_BmpBioSubMDet1Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioSubMDet1Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioSubMDet1Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioSubMDet1Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioSubMDet1Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioSubMDet1Pri.setColor(%colStr);
					PTG_Cmplx_SwBioSubMDet1Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioSubMDet1Br.BrickID = "";
				PTG_Cmplx_BmpBioSubMDet1Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioSubMDet1Pri.PrintID = "";
					PTG_Cmplx_BmpBioSubMDet1Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioSubMDet1Col.colorID = "";
					PTG_Cmplx_BmpBioSubMDet1Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioSubMDet1Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioSubMDet1Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioSubMDet2Br.BrickID = %brID;
				PTG_Cmplx_BmpBioSubMDet2Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioSubMDet2Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioSubMDet2Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioSubMDet2Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioSubMDet2Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioSubMDet2Pri.setColor(%colStr);
					PTG_Cmplx_SwBioSubMDet2Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioSubMDet2Br.BrickID = "";
				PTG_Cmplx_BmpBioSubMDet2Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioSubMDet2Pri.PrintID = "";
					PTG_Cmplx_BmpBioSubMDet2Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioSubMDet2Col.colorID = "";
					PTG_Cmplx_BmpBioSubMDet2Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioSubMDet2Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioSubMDet2Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioSubMDet3Br.BrickID = %brID;
				PTG_Cmplx_BmpBioSubMDet3Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioSubMDet3Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioSubMDet3Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioSubMDet3Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioSubMDet3Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioSubMDet3Pri.setColor(%colStr);
					PTG_Cmplx_SwBioSubMDet3Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioSubMDet3Br.BrickID = "";
				PTG_Cmplx_BmpBioSubMDet3Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioSubMDet3Pri.PrintID = "";
					PTG_Cmplx_BmpBioSubMDet3Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioSubMDet3Col.colorID = "";
					PTG_Cmplx_BmpBioSubMDet3Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioSubMDet3Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioSubMDet3Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioSubMDet4Br.BrickID = %brID;
				PTG_Cmplx_BmpBioSubMDet4Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioSubMDet4Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioSubMDet4Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioSubMDet4Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioSubMDet4Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioSubMDet4Pri.setColor(%colStr);
					PTG_Cmplx_SwBioSubMDet4Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioSubMDet4Br.BrickID = "";
				PTG_Cmplx_BmpBioSubMDet4Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioSubMDet4Pri.PrintID = "";
					PTG_Cmplx_BmpBioSubMDet4Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioSubMDet4Col.colorID = "";
					PTG_Cmplx_BmpBioSubMDet4Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioSubMDet4Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioSubMDet4Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioSubMDet5Br.BrickID = %brID;
				PTG_Cmplx_BmpBioSubMDet5Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioSubMDet5Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioSubMDet5Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioSubMDet5Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioSubMDet5Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioSubMDet5Pri.setColor(%colStr);
					PTG_Cmplx_SwBioSubMDet5Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioSubMDet5Br.BrickID = "";
				PTG_Cmplx_BmpBioSubMDet5Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioSubMDet5Pri.PrintID = "";
					PTG_Cmplx_BmpBioSubMDet5Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioSubMDet5Col.colorID = "";
					PTG_Cmplx_BmpBioSubMDet5Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioSubMDet5Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioSubMDet5Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioSubMDet6Br.BrickID = %brID;
				PTG_Cmplx_BmpBioSubMDet6Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioSubMDet6Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioSubMDet6Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioSubMDet6Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioSubMDet6Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioSubMDet6Pri.setColor(%colStr);
					PTG_Cmplx_SwBioSubMDet6Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioSubMDet6Br.BrickID = "";
				PTG_Cmplx_BmpBioSubMDet6Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioSubMDet6Pri.PrintID = "";
					PTG_Cmplx_BmpBioSubMDet6Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioSubMDet6Col.colorID = "";
					PTG_Cmplx_BmpBioSubMDet6Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioSubMDet6Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioSubMDet6Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioSubMDet7Br.BrickID = %brID;
				PTG_Cmplx_BmpBioSubMDet7Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioSubMDet7Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioSubMDet7Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioSubMDet7Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioSubMDet7Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioSubMDet7Pri.setColor(%colStr);
					PTG_Cmplx_SwBioSubMDet7Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioSubMDet7Br.BrickID = "";
				PTG_Cmplx_BmpBioSubMDet7Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioSubMDet7Pri.PrintID = "";
					PTG_Cmplx_BmpBioSubMDet7Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioSubMDet7Col.colorID = "";
					PTG_Cmplx_BmpBioSubMDet7Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioSubMDet7Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioSubMDet7Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioSubMDet8Br.BrickID = %brID;
				PTG_Cmplx_BmpBioSubMDet8Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioSubMDet8Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioSubMDet8Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioSubMDet8Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioSubMDet8Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioSubMDet8Pri.setColor(%colStr);
					PTG_Cmplx_SwBioSubMDet8Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioSubMDet8Br.BrickID = "";
				PTG_Cmplx_BmpBioSubMDet8Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioSubMDet8Pri.PrintID = "";
					PTG_Cmplx_BmpBioSubMDet8Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioSubMDet8Col.colorID = "";
					PTG_Cmplx_BmpBioSubMDet8Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioSubMDet8Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioSubMDet8Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioSubMDet9Br.BrickID = %brID;
				PTG_Cmplx_BmpBioSubMDet9Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioSubMDet9Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioSubMDet9Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioSubMDet9Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioSubMDet9Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioSubMDet9Pri.setColor(%colStr);
					PTG_Cmplx_SwBioSubMDet9Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioSubMDet9Br.BrickID = "";
				PTG_Cmplx_BmpBioSubMDet9Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioSubMDet9Pri.PrintID = "";
					PTG_Cmplx_BmpBioSubMDet9Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioSubMDet9Col.colorID = "";
					PTG_Cmplx_BmpBioSubMDet9Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioSubMDet9Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioSubMDet9Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioSubMDet10Br.BrickID = %brID;
				PTG_Cmplx_BmpBioSubMDet10Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioSubMDet10Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioSubMDet10Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioSubMDet10Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioSubMDet10Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioSubMDet10Pri.setColor(%colStr);
					PTG_Cmplx_SwBioSubMDet10Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioSubMDet10Br.BrickID = "";
				PTG_Cmplx_BmpBioSubMDet10Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioSubMDet10Pri.PrintID = "";
					PTG_Cmplx_BmpBioSubMDet10Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioSubMDet10Col.colorID = "";
					PTG_Cmplx_BmpBioSubMDet10Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioSubMDet10Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioSubMDet10Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioSubMDet11Br.BrickID = %brID;
				PTG_Cmplx_BmpBioSubMDet11Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioSubMDet11Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioSubMDet11Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioSubMDet11Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioSubMDet11Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioSubMDet11Pri.setColor(%colStr);
					PTG_Cmplx_SwBioSubMDet11Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioSubMDet11Br.BrickID = "";
				PTG_Cmplx_BmpBioSubMDet11Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioSubMDet11Pri.PrintID = "";
					PTG_Cmplx_BmpBioSubMDet11Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioSubMDet11Col.colorID = "";
					PTG_Cmplx_BmpBioSubMDet11Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioSubMDet11Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioSubMDet11Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioSubMdet12Br.BrickID = %brID;
				PTG_Cmplx_BmpBioSubMdet12Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioSubMdet12Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioSubMdet12Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioSubMdet12Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioSubMdet12Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioSubMdet12Pri.setColor(%colStr);
					PTG_Cmplx_SwBioSubMdet12Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioSubMdet12Br.BrickID = "";
				PTG_Cmplx_BmpBioSubMdet12Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioSubMdet12Pri.PrintID = "";
					PTG_Cmplx_BmpBioSubMdet12Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioSubMdet12Col.colorID = "";
					PTG_Cmplx_BmpBioSubMdet12Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioSubMdet12Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioSubMdet12Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioSubMdet13Br.BrickID = %brID;
				PTG_Cmplx_BmpBioSubMdet13Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioSubMdet13Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioSubMdet13Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioSubMdet13Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioSubMdet13Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioSubMdet13Pri.setColor(%colStr);
					PTG_Cmplx_SwBioSubMdet13Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioSubMdet13Br.BrickID = "";
				PTG_Cmplx_BmpBioSubMdet13Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioSubMdet13Pri.PrintID = "";
					PTG_Cmplx_BmpBioSubMdet13Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioSubMdet13Col.colorID = "";
					PTG_Cmplx_BmpBioSubMdet13Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioSubMdet13Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioSubMdet13Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioSubMdet14Br.BrickID = %brID;
				PTG_Cmplx_BmpBioSubMdet14Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioSubMdet14Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioSubMdet14Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioSubMdet14Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioSubMdet14Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioSubMdet14Pri.setColor(%colStr);
					PTG_Cmplx_SwBioSubMdet14Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioSubMdet14Br.BrickID = "";
				PTG_Cmplx_BmpBioSubMdet14Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioSubMdet14Pri.PrintID = "";
					PTG_Cmplx_BmpBioSubMdet14Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioSubMdet14Col.colorID = "";
					PTG_Cmplx_BmpBioSubMdet14Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioSubMdet14Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioSubMdet14Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioSubMdet15Br.BrickID = %brID;
				PTG_Cmplx_BmpBioSubMdet15Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioSubMdet15Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioSubMdet15Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioSubMdet15Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioSubMdet15Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioSubMdet15Pri.setColor(%colStr);
					PTG_Cmplx_SwBioSubMdet15Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioSubMdet15Br.BrickID = "";
				PTG_Cmplx_BmpBioSubMdet15Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioSubMdet15Pri.PrintID = "";
					PTG_Cmplx_BmpBioSubMdet15Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioSubMdet15Col.colorID = "";
					PTG_Cmplx_BmpBioSubMdet15Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioSubMdet15Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioSubMdet15Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioSubMdet16Br.BrickID = %brID;
				PTG_Cmplx_BmpBioSubMdet16Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioSubMdet16Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioSubMdet16Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioSubMdet16Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioSubMdet16Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioSubMdet16Pri.setColor(%colStr);
					PTG_Cmplx_SwBioSubMdet16Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioSubMdet16Br.BrickID = "";
				PTG_Cmplx_BmpBioSubMdet16Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioSubMdet16Pri.PrintID = "";
					PTG_Cmplx_BmpBioSubMdet16Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioSubMdet16Col.colorID = "";
					PTG_Cmplx_BmpBioSubMdet16Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioSubMdet16Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioSubMdet16Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioSubMdet17Br.BrickID = %brID;
				PTG_Cmplx_BmpBioSubMdet17Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioSubMdet17Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioSubMdet17Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioSubMdet17Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioSubMdet17Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioSubMdet17Pri.setColor(%colStr);
					PTG_Cmplx_SwBioSubMdet17Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioSubMdet17Br.BrickID = "";
				PTG_Cmplx_BmpBioSubMdet17Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioSubMdet17Pri.PrintID = "";
					PTG_Cmplx_BmpBioSubMdet17Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioSubMdet17Col.colorID = "";
					PTG_Cmplx_BmpBioSubMdet17Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioSubMdet17Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioSubMdet17Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			
			
			%file.readLine(); //Custom Biome A
			PTG_Cmplx_BmpBioCustATerPri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
				PTG_Cmplx_BmpBioCustATerPri.setBitmap(getField($PTG_PrintArr[%priID],1));
			PTG_Cmplx_SwBioCustATerCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwBioCustATerPri.setColor(%colStr = getColorI(getColorIDTable(%colID)));
				PTG_Cmplx_SwBioCustATerCol.setColor(%colStr);
			PTG_Cmplx_BmpBioCustAWatPri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
				PTG_Cmplx_BmpBioCustAWatPri.setBitmap(getField($PTG_PrintArr[%priID],1));
			PTG_Cmplx_SwBioCustAWatCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwBioCustAWatPri.setColor(%colStr = getColorI(getColorIDTable(%colID)));
				PTG_Cmplx_SwBioCustAWatCol.setColor(%colStr);
			PTG_Cmplx_SldrTerHMod_CustA.setValue(%file.readLine());
			//PTG_Cmplx_CheckAllMntnsCustBioA.setValue(%file.readLine()); 
			PTG_Cmplx_PopUpWaterType_CustA.setSelected(%file.readLine()); //mClamp
			
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustADet0Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustADet0Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustADet0Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustADet0Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustADet0Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustADet0Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustADet0Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustADet0Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustADet0Br.BrickID = "";
				PTG_Cmplx_BmpBioCustADet0Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustADet0Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustADet0Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustADet0Col.colorID = "";
					PTG_Cmplx_BmpBioCustADet0Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustADet0Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustADet0Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustADet1Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustADet1Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustADet1Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustADet1Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustADet1Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustADet1Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustADet1Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustADet1Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustADet1Br.BrickID = "";
				PTG_Cmplx_BmpBioCustADet1Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustADet1Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustADet1Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustADet1Col.colorID = "";
					PTG_Cmplx_BmpBioCustADet1Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustADet1Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustADet1Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustADet2Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustADet2Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustADet2Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustADet2Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustADet2Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustADet2Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustADet2Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustADet2Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustADet2Br.BrickID = "";
				PTG_Cmplx_BmpBioCustADet2Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustADet2Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustADet2Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustADet2Col.colorID = "";
					PTG_Cmplx_BmpBioCustADet2Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustADet2Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustADet2Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustADet3Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustADet3Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustADet3Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustADet3Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustADet3Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustADet3Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustADet3Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustADet3Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustADet3Br.BrickID = "";
				PTG_Cmplx_BmpBioCustADet3Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustADet3Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustADet3Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustADet3Col.colorID = "";
					PTG_Cmplx_BmpBioCustADet3Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustADet3Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustADet3Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustADet4Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustADet4Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustADet4Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustADet4Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustADet4Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustADet4Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustADet4Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustADet4Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustADet4Br.BrickID = "";
				PTG_Cmplx_BmpBioCustADet4Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustADet4Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustADet4Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustADet4Col.colorID = "";
					PTG_Cmplx_BmpBioCustADet4Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustADet4Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustADet4Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustADet5Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustADet5Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustADet5Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustADet5Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustADet5Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustADet5Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustADet5Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustADet5Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustADet5Br.BrickID = "";
				PTG_Cmplx_BmpBioCustADet5Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustADet5Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustADet5Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustADet5Col.colorID = "";
					PTG_Cmplx_BmpBioCustADet5Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustADet5Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustADet5Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustADet6Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustADet6Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustADet6Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustADet6Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustADet6Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustADet6Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustADet6Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustADet6Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustADet6Br.BrickID = "";
				PTG_Cmplx_BmpBioCustADet6Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustADet6Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustADet6Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustADet6Col.colorID = "";
					PTG_Cmplx_BmpBioCustADet6Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustADet6Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustADet6Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustADet7Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustADet7Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustADet7Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustADet7Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustADet7Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustADet7Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustADet7Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustADet7Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustADet7Br.BrickID = "";
				PTG_Cmplx_BmpBioCustADet7Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustADet7Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustADet7Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustADet7Col.colorID = "";
					PTG_Cmplx_BmpBioCustADet7Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustADet7Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustADet7Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustADet8Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustADet8Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustADet8Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustADet8Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustADet8Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustADet8Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustADet8Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustADet8Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustADet8Br.BrickID = "";
				PTG_Cmplx_BmpBioCustADet8Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustADet8Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustADet8Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustADet8Col.colorID = "";
					PTG_Cmplx_BmpBioCustADet8Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustADet8Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustADet8Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustADet9Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustADet9Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustADet9Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustADet9Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustADet9Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustADet9Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustADet9Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustADet9Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustADet9Br.BrickID = "";
				PTG_Cmplx_BmpBioCustADet9Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustADet9Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustADet9Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustADet9Col.colorID = "";
					PTG_Cmplx_BmpBioCustADet9Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustADet9Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustADet9Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustADet10Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustADet10Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustADet10Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustADet10Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustADet10Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustADet10Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustADet10Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustADet10Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustADet10Br.BrickID = "";
				PTG_Cmplx_BmpBioCustADet10Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustADet10Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustADet10Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustADet10Col.colorID = "";
					PTG_Cmplx_BmpBioCustADet10Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustADet10Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustADet10Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustADet11Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustADet11Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustADet11Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustADet11Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustADet11Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustADet11Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustADet11Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustADet11Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustADet11Br.BrickID = "";
				PTG_Cmplx_BmpBioCustADet11Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustADet11Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustADet11Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustADet11Col.colorID = "";
					PTG_Cmplx_BmpBioCustADet11Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustADet11Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustADet11Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustAdet12Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustAdet12Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustAdet12Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustAdet12Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustAdet12Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustAdet12Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustAdet12Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustAdet12Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustAdet12Br.BrickID = "";
				PTG_Cmplx_BmpBioCustAdet12Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustAdet12Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustAdet12Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustAdet12Col.colorID = "";
					PTG_Cmplx_BmpBioCustAdet12Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustAdet12Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustAdet12Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustAdet13Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustAdet13Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustAdet13Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustAdet13Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustAdet13Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustAdet13Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustAdet13Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustAdet13Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustAdet13Br.BrickID = "";
				PTG_Cmplx_BmpBioCustAdet13Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustAdet13Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustAdet13Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustAdet13Col.colorID = "";
					PTG_Cmplx_BmpBioCustAdet13Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustAdet13Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustAdet13Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustAdet14Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustAdet14Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustAdet14Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustAdet14Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustAdet14Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustAdet14Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustAdet14Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustAdet14Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustAdet14Br.BrickID = "";
				PTG_Cmplx_BmpBioCustAdet14Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustAdet14Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustAdet14Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustAdet14Col.colorID = "";
					PTG_Cmplx_BmpBioCustAdet14Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustAdet14Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustAdet14Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustAdet15Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustAdet15Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustAdet15Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustAdet15Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustAdet15Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustAdet15Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustAdet15Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustAdet15Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustAdet15Br.BrickID = "";
				PTG_Cmplx_BmpBioCustAdet15Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustAdet15Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustAdet15Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustAdet15Col.colorID = "";
					PTG_Cmplx_BmpBioCustAdet15Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustAdet15Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustAdet15Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustAdet16Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustAdet16Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustAdet16Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustAdet16Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustAdet16Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustAdet16Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustAdet16Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustAdet16Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustAdet16Br.BrickID = "";
				PTG_Cmplx_BmpBioCustAdet16Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustAdet16Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustAdet16Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustAdet16Col.colorID = "";
					PTG_Cmplx_BmpBioCustAdet16Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustAdet16Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustAdet16Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustAdet17Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustAdet17Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustAdet17Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustAdet17Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustAdet17Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustAdet17Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustAdet17Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustAdet17Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustAdet17Br.BrickID = "";
				PTG_Cmplx_BmpBioCustAdet17Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustAdet17Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustAdet17Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustAdet17Col.colorID = "";
					PTG_Cmplx_BmpBioCustAdet17Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustAdet17Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustAdet17Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			
			
			%file.readLine(); //Custom Biome B
			PTG_Cmplx_BmpBioCustBTerPri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
				PTG_Cmplx_BmpBioCustBTerPri.setBitmap(getField($PTG_PrintArr[%priID],1));
			PTG_Cmplx_SwBioCustBTerCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwBioCustBTerPri.setColor(%colStr = getColorI(getColorIDTable(%colID)));
				PTG_Cmplx_SwBioCustBTerCol.setColor(%colStr);
			PTG_Cmplx_BmpBioCustBWatPri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
				PTG_Cmplx_BmpBioCustBWatPri.setBitmap(getField($PTG_PrintArr[%priID],1));
			PTG_Cmplx_SwBioCustBWatCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwBioCustBWatPri.setColor(%colStr = getColorI(getColorIDTable(%colID)));
				PTG_Cmplx_SwBioCustBWatCol.setColor(%colStr);
			PTG_Cmplx_SldrTerHMod_CustB.setValue(%file.readLine());
			//PTG_Cmplx_CheckAllMntnsCustBioB.setValue(%file.readLine()); 
			PTG_Cmplx_PopUpWaterType_CustB.setSelected(%file.readLine()); //mClamp
			
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustBDet0Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustBDet0Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustBDet0Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustBDet0Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustBDet0Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustBDet0Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustBDet0Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustBDet0Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustBDet0Br.BrickID = "";
				PTG_Cmplx_BmpBioCustBDet0Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustBDet0Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustBDet0Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustBDet0Col.colorID = "";
					PTG_Cmplx_BmpBioCustBDet0Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustBDet0Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustBDet0Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustBDet1Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustBDet1Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustBDet1Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustBDet1Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustBDet1Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustBDet1Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustBDet1Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustBDet1Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustBDet1Br.BrickID = "";
				PTG_Cmplx_BmpBioCustBDet1Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustBDet1Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustBDet1Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustBDet1Col.colorID = "";
					PTG_Cmplx_BmpBioCustBDet1Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustBDet1Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustBDet1Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustBDet2Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustBDet2Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustBDet2Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustBDet2Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustBDet2Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustBDet2Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustBDet2Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustBDet2Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustBDet2Br.BrickID = "";
				PTG_Cmplx_BmpBioCustBDet2Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustBDet2Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustBDet2Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustBDet2Col.colorID = "";
					PTG_Cmplx_BmpBioCustBDet2Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustBDet2Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustBDet2Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustBDet3Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustBDet3Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustBDet3Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustBDet3Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustBDet3Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustBDet3Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustBDet3Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustBDet3Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustBDet3Br.BrickID = "";
				PTG_Cmplx_BmpBioCustBDet3Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustBDet3Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustBDet3Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustBDet3Col.colorID = "";
					PTG_Cmplx_BmpBioCustBDet3Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustBDet3Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustBDet3Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustBDet4Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustBDet4Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustBDet4Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustBDet4Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustBDet4Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustBDet4Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustBDet4Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustBDet4Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustBDet4Br.BrickID = "";
				PTG_Cmplx_BmpBioCustBDet4Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustBDet4Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustBDet4Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustBDet4Col.colorID = "";
					PTG_Cmplx_BmpBioCustBDet4Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustBDet4Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustBDet4Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustBDet5Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustBDet5Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustBDet5Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustBDet5Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustBDet5Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustBDet5Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustBDet5Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustBDet5Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustBDet5Br.BrickID = "";
				PTG_Cmplx_BmpBioCustBDet5Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustBDet5Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustBDet5Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustBDet5Col.colorID = "";
					PTG_Cmplx_BmpBioCustBDet5Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustBDet5Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustBDet5Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustBDet6Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustBDet6Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustBDet6Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustBDet6Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustBDet6Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustBDet6Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustBDet6Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustBDet6Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustBDet6Br.BrickID = "";
				PTG_Cmplx_BmpBioCustBDet6Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustBDet6Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustBDet6Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustBDet6Col.colorID = "";
					PTG_Cmplx_BmpBioCustBDet6Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustBDet6Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustBDet6Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustBDet7Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustBDet7Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustBDet7Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustBDet7Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustBDet7Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustBDet7Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustBDet7Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustBDet7Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustBDet7Br.BrickID = "";
				PTG_Cmplx_BmpBioCustBDet7Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustBDet7Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustBDet7Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustBDet7Col.colorID = "";
					PTG_Cmplx_BmpBioCustBDet7Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustBDet7Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustBDet7Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustBDet8Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustBDet8Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustBDet8Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustBDet8Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustBDet8Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustBDet8Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustBDet8Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustBDet8Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustBDet8Br.BrickID = "";
				PTG_Cmplx_BmpBioCustBDet8Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustBDet8Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustBDet8Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustBDet8Col.colorID = "";
					PTG_Cmplx_BmpBioCustBDet8Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustBDet8Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustBDet8Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustBDet9Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustBDet9Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustBDet9Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustBDet9Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustBDet9Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustBDet9Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustBDet9Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustBDet9Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustBDet9Br.BrickID = "";
				PTG_Cmplx_BmpBioCustBDet9Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustBDet9Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustBDet9Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustBDet9Col.colorID = "";
					PTG_Cmplx_BmpBioCustBDet9Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustBDet9Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustBDet9Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustBDet10Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustBDet10Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustBDet10Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustBDet10Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustBDet10Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustBDet10Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustBDet10Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustBDet10Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustBDet10Br.BrickID = "";
				PTG_Cmplx_BmpBioCustBDet10Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustBDet10Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustBDet10Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustBDet10Col.colorID = "";
					PTG_Cmplx_BmpBioCustBDet10Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustBDet10Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustBDet10Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustBDet11Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustBDet11Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustBDet11Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustBDet11Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustBDet11Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustBDet11Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustBDet11Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustBDet11Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustBDet11Br.BrickID = "";
				PTG_Cmplx_BmpBioCustBDet11Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustBDet11Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustBDet11Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustBDet11Col.colorID = "";
					PTG_Cmplx_BmpBioCustBDet11Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustBDet11Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustBDet11Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustBdet12Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustBdet12Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustBdet12Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustBdet12Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustBdet12Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustBdet12Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustBdet12Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustBdet12Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustBdet12Br.BrickID = "";
				PTG_Cmplx_BmpBioCustBdet12Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustBdet12Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustBdet12Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustBdet12Col.colorID = "";
					PTG_Cmplx_BmpBioCustBdet12Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustBdet12Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustBdet12Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustBdet13Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustBdet13Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustBdet13Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustBdet13Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustBdet13Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustBdet13Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustBdet13Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustBdet13Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustBdet13Br.BrickID = "";
				PTG_Cmplx_BmpBioCustBdet13Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustBdet13Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustBdet13Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustBdet13Col.colorID = "";
					PTG_Cmplx_BmpBioCustBdet13Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustBdet13Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustBdet13Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustBdet14Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustBdet14Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustBdet14Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustBdet14Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustBdet14Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustBdet14Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustBdet14Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustBdet14Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustBdet14Br.BrickID = "";
				PTG_Cmplx_BmpBioCustBdet14Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustBdet14Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustBdet14Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustBdet14Col.colorID = "";
					PTG_Cmplx_BmpBioCustBdet14Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustBdet14Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustBdet14Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustBdet15Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustBdet15Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustBdet15Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustBdet15Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustBdet15Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustBdet15Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustBdet15Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustBdet15Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustBdet15Br.BrickID = "";
				PTG_Cmplx_BmpBioCustBdet15Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustBdet15Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustBdet15Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustBdet15Col.colorID = "";
					PTG_Cmplx_BmpBioCustBdet15Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustBdet15Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustBdet15Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustBdet16Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustBdet16Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustBdet16Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustBdet16Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustBdet16Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustBdet16Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustBdet16Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustBdet16Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustBdet16Br.BrickID = "";
				PTG_Cmplx_BmpBioCustBdet16Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustBdet16Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustBdet16Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustBdet16Col.colorID = "";
					PTG_Cmplx_BmpBioCustBdet16Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustBdet16Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustBdet16Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustBdet17Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustBdet17Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustBdet17Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustBdet17Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustBdet17Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustBdet17Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustBdet17Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustBdet17Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustBdet17Br.BrickID = "";
				PTG_Cmplx_BmpBioCustBdet17Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustBdet17Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustBdet17Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustBdet17Col.colorID = "";
					PTG_Cmplx_BmpBioCustBdet17Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustBdet17Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustBdet17Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			
			
			%file.readLine(); //Custom Biome C
			PTG_Cmplx_BmpBioCustCTerPri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
				PTG_Cmplx_BmpBioCustCTerPri.setBitmap(getField($PTG_PrintArr[%priID],1));
			PTG_Cmplx_SwBioCustCTerCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwBioCustCTerPri.setColor(%colStr = getColorI(getColorIDTable(%colID)));
				PTG_Cmplx_SwBioCustCTerCol.setColor(%colStr);
			PTG_Cmplx_BmpBioCustCWatPri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
				PTG_Cmplx_BmpBioCustCWatPri.setBitmap(getField($PTG_PrintArr[%priID],1));
			PTG_Cmplx_SwBioCustCWatCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwBioCustCWatPri.setColor(%colStr = getColorI(getColorIDTable(%colID)));
				PTG_Cmplx_SwBioCustCWatCol.setColor(%colStr);
			PTG_Cmplx_SldrTerHMod_CustC.setValue(%file.readLine());
			//PTG_Cmplx_CheckAllMntnsCustBioC.setValue(%file.readLine()); 
			PTG_Cmplx_PopUpWaterType_CustC.setSelected(%file.readLine()); //mClamp
			
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustCDet0Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustCDet0Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustCDet0Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustCDet0Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustCDet0Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustCDet0Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustCDet0Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustCDet0Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustCDet0Br.BrickID = "";
				PTG_Cmplx_BmpBioCustCDet0Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustCDet0Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustCDet0Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustCDet0Col.colorID = "";
					PTG_Cmplx_BmpBioCustCDet0Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustCDet0Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustCDet0Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustCDet1Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustCDet1Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustCDet1Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustCDet1Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustCDet1Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustCDet1Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustCDet1Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustCDet1Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustCDet1Br.BrickID = "";
				PTG_Cmplx_BmpBioCustCDet1Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustCDet1Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustCDet1Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustCDet1Col.colorID = "";
					PTG_Cmplx_BmpBioCustCDet1Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustCDet1Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustCDet1Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustCDet2Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustCDet2Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustCDet2Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustCDet2Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustCDet2Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustCDet2Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustCDet2Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustCDet2Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustCDet2Br.BrickID = "";
				PTG_Cmplx_BmpBioCustCDet2Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustCDet2Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustCDet2Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustCDet2Col.colorID = "";
					PTG_Cmplx_BmpBioCustCDet2Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustCDet2Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustCDet2Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustCDet3Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustCDet3Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustCDet3Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustCDet3Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustCDet3Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustCDet3Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustCDet3Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustCDet3Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustCDet3Br.BrickID = "";
				PTG_Cmplx_BmpBioCustCDet3Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustCDet3Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustCDet3Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustCDet3Col.colorID = "";
					PTG_Cmplx_BmpBioCustCDet3Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustCDet3Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustCDet3Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustCDet4Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustCDet4Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustCDet4Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustCDet4Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustCDet4Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustCDet4Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustCDet4Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustCDet4Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustCDet4Br.BrickID = "";
				PTG_Cmplx_BmpBioCustCDet4Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustCDet4Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustCDet4Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustCDet4Col.colorID = "";
					PTG_Cmplx_BmpBioCustCDet4Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustCDet4Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustCDet4Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustCDet5Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustCDet5Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustCDet5Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustCDet5Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustCDet5Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustCDet5Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustCDet5Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustCDet5Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustCDet5Br.BrickID = "";
				PTG_Cmplx_BmpBioCustCDet5Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustCDet5Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustCDet5Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustCDet5Col.colorID = "";
					PTG_Cmplx_BmpBioCustCDet5Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustCDet5Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustCDet5Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustCDet6Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustCDet6Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustCDet6Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustCDet6Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustCDet6Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustCDet6Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustCDet6Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustCDet6Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustCDet6Br.BrickID = "";
				PTG_Cmplx_BmpBioCustCDet6Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustCDet6Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustCDet6Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustCDet6Col.colorID = "";
					PTG_Cmplx_BmpBioCustCDet6Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustCDet6Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustCDet6Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustCDet7Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustCDet7Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustCDet7Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustCDet7Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustCDet7Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustCDet7Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustCDet7Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustCDet7Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustCDet7Br.BrickID = "";
				PTG_Cmplx_BmpBioCustCDet7Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustCDet7Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustCDet7Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustCDet7Col.colorID = "";
					PTG_Cmplx_BmpBioCustCDet7Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustCDet7Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustCDet7Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustCDet8Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustCDet8Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustCDet8Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustCDet8Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustCDet8Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustCDet8Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustCDet8Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustCDet8Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustCDet8Br.BrickID = "";
				PTG_Cmplx_BmpBioCustCDet8Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustCDet8Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustCDet8Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustCDet8Col.colorID = "";
					PTG_Cmplx_BmpBioCustCDet8Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustCDet8Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustCDet8Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustCDet9Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustCDet9Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustCDet9Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustCDet9Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustCDet9Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustCDet9Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustCDet9Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustCDet9Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustCDet9Br.BrickID = "";
				PTG_Cmplx_BmpBioCustCDet9Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustCDet9Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustCDet9Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustCDet9Col.colorID = "";
					PTG_Cmplx_BmpBioCustCDet9Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustCDet9Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustCDet9Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustCDet10Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustCDet10Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustCDet10Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustCDet10Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustCDet10Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustCDet10Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustCDet10Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustCDet10Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustCDet10Br.BrickID = "";
				PTG_Cmplx_BmpBioCustCDet10Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustCDet10Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustCDet10Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustCDet10Col.colorID = "";
					PTG_Cmplx_BmpBioCustCDet10Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustCDet10Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustCDet10Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustCDet11Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustCDet11Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustCDet11Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustCDet11Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustCDet11Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustCDet11Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustCDet11Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustCDet11Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustCDet11Br.BrickID = "";
				PTG_Cmplx_BmpBioCustCDet11Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustCDet11Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustCDet11Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustCDet11Col.colorID = "";
					PTG_Cmplx_BmpBioCustCDet11Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustCDet11Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustCDet11Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustCdet12Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustCdet12Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustCdet12Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustCdet12Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustCdet12Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustCdet12Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustCdet12Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustCdet12Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustCdet12Br.BrickID = "";
				PTG_Cmplx_BmpBioCustCdet12Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustCdet12Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustCdet12Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustCdet12Col.colorID = "";
					PTG_Cmplx_BmpBioCustCdet12Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustCdet12Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustCdet12Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustCdet13Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustCdet13Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustCdet13Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustCdet13Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustCdet13Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustCdet13Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustCdet13Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustCdet13Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustCdet13Br.BrickID = "";
				PTG_Cmplx_BmpBioCustCdet13Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustCdet13Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustCdet13Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustCdet13Col.colorID = "";
					PTG_Cmplx_BmpBioCustCdet13Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustCdet13Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustCdet13Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustCdet14Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustCdet14Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustCdet14Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustCdet14Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustCdet14Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustCdet14Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustCdet14Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustCdet14Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustCdet14Br.BrickID = "";
				PTG_Cmplx_BmpBioCustCdet14Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustCdet14Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustCdet14Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustCdet14Col.colorID = "";
					PTG_Cmplx_BmpBioCustCdet14Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustCdet14Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustCdet14Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustCdet15Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustCdet15Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustCdet15Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustCdet15Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustCdet15Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustCdet15Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustCdet15Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustCdet15Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustCdet15Br.BrickID = "";
				PTG_Cmplx_BmpBioCustCdet15Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustCdet15Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustCdet15Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustCdet15Col.colorID = "";
					PTG_Cmplx_BmpBioCustCdet15Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustCdet15Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustCdet15Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustCdet16Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustCdet16Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustCdet16Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustCdet16Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustCdet16Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustCdet16Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustCdet16Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustCdet16Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustCdet16Br.BrickID = "";
				PTG_Cmplx_BmpBioCustCdet16Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustCdet16Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustCdet16Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustCdet16Col.colorID = "";
					PTG_Cmplx_BmpBioCustCdet16Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustCdet16Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustCdet16Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCustCdet17Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCustCdet17Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCustCdet17Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCustCdet17Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCustCdet17Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCustCdet17Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCustCdet17Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCustCdet17Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCustCdet17Br.BrickID = "";
				PTG_Cmplx_BmpBioCustCdet17Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCustCdet17Pri.PrintID = "";
					PTG_Cmplx_BmpBioCustCdet17Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCustCdet17Col.colorID = "";
					PTG_Cmplx_BmpBioCustCdet17Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCustCdet17Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCustCdet17Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			
			
			%file.readLine(); //Cave Top Biome
			PTG_Cmplx_BmpBioCaveTTerPri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
				PTG_Cmplx_BmpBioCaveTTerPri.setBitmap(getField($PTG_PrintArr[%priID],1));
			PTG_Cmplx_SwBioCaveTTerCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwBioCaveTTerPri.setColor(%colStr = getColorI(getColorIDTable(%colID)));
				PTG_Cmplx_SwBioCaveTTerCol.setColor(%colStr);
			
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveTDet0Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveTDet0Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveTDet0Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveTDet0Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveTDet0Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveTDet0Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveTDet0Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveTDet0Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveTDet0Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveTDet0Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveTDet0Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveTDet0Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveTDet0Col.colorID = "";
					PTG_Cmplx_BmpBioCaveTDet0Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveTDet0Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveTDet0Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveTDet1Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveTDet1Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveTDet1Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveTDet1Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveTDet1Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveTDet1Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveTDet1Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveTDet1Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveTDet1Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveTDet1Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveTDet1Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveTDet1Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveTDet1Col.colorID = "";
					PTG_Cmplx_BmpBioCaveTDet1Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveTDet1Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveTDet1Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveTDet2Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveTDet2Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveTDet2Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveTDet2Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveTDet2Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveTDet2Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveTDet2Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveTDet2Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveTDet2Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveTDet2Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveTDet2Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveTDet2Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveTDet2Col.colorID = "";
					PTG_Cmplx_BmpBioCaveTDet2Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveTDet2Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveTDet2Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveTDet3Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveTDet3Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveTDet3Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveTDet3Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveTDet3Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveTDet3Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveTDet3Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveTDet3Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveTDet3Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveTDet3Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveTDet3Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveTDet3Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveTDet3Col.colorID = "";
					PTG_Cmplx_BmpBioCaveTDet3Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveTDet3Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveTDet3Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveTDet4Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveTDet4Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveTDet4Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveTDet4Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveTDet4Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveTDet4Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveTDet4Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveTDet4Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveTDet4Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveTDet4Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveTDet4Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveTDet4Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveTDet4Col.colorID = "";
					PTG_Cmplx_BmpBioCaveTDet4Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveTDet4Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveTDet4Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveTDet5Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveTDet5Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveTDet5Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveTDet5Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveTDet5Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveTDet5Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveTDet5Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveTDet5Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveTDet5Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveTDet5Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveTDet5Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveTDet5Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveTDet5Col.colorID = "";
					PTG_Cmplx_BmpBioCaveTDet5Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveTDet5Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveTDet5Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveTDet6Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveTDet6Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveTDet6Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveTDet6Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveTDet6Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveTDet6Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveTDet6Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveTDet6Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveTDet6Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveTDet6Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveTDet6Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveTDet6Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveTDet6Col.colorID = "";
					PTG_Cmplx_BmpBioCaveTDet6Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveTDet6Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveTDet6Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveTDet7Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveTDet7Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveTDet7Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveTDet7Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveTDet7Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveTDet7Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveTDet7Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveTDet7Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveTDet7Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveTDet7Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveTDet7Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveTDet7Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveTDet7Col.colorID = "";
					PTG_Cmplx_BmpBioCaveTDet7Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveTDet7Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveTDet7Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveTDet8Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveTDet8Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveTDet8Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveTDet8Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveTDet8Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveTDet8Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveTDet8Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveTDet8Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveTDet8Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveTDet8Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveTDet8Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveTDet8Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveTDet8Col.colorID = "";
					PTG_Cmplx_BmpBioCaveTDet8Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveTDet8Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveTDet8Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveTDet9Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveTDet9Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveTDet9Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveTDet9Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveTDet9Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveTDet9Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveTDet9Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveTDet9Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveTDet9Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveTDet9Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveTDet9Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveTDet9Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveTDet9Col.colorID = "";
					PTG_Cmplx_BmpBioCaveTDet9Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveTDet9Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveTDet9Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveTDet10Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveTDet10Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveTDet10Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveTDet10Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveTDet10Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveTDet10Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveTDet10Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveTDet10Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveTDet10Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveTDet10Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveTDet10Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveTDet10Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveTDet10Col.colorID = "";
					PTG_Cmplx_BmpBioCaveTDet10Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveTDet10Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveTDet10Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveTDet11Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveTDet11Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveTDet11Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveTDet11Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveTDet11Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveTDet11Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveTDet11Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveTDet11Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveTDet11Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveTDet11Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveTDet11Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveTDet11Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveTDet11Col.colorID = "";
					PTG_Cmplx_BmpBioCaveTDet11Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveTDet11Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveTDet11Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveTdet12Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveTdet12Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveTdet12Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveTdet12Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveTdet12Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveTdet12Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveTdet12Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveTdet12Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveTdet12Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveTdet12Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveTdet12Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveTdet12Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveTdet12Col.colorID = "";
					PTG_Cmplx_BmpBioCaveTdet12Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveTdet12Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveTdet12Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveTdet13Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveTdet13Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveTdet13Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveTdet13Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveTdet13Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveTdet13Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveTdet13Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveTdet13Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveTdet13Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveTdet13Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveTdet13Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveTdet13Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveTdet13Col.colorID = "";
					PTG_Cmplx_BmpBioCaveTdet13Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveTdet13Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveTdet13Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveTdet14Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveTdet14Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveTdet14Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveTdet14Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveTdet14Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveTdet14Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveTdet14Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveTdet14Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveTdet14Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveTdet14Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveTdet14Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveTdet14Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveTdet14Col.colorID = "";
					PTG_Cmplx_BmpBioCaveTdet14Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveTdet14Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveTdet14Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveTdet15Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveTdet15Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveTdet15Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveTdet15Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveTdet15Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveTdet15Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveTdet15Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveTdet15Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveTdet15Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveTdet15Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveTdet15Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveTdet15Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveTdet15Col.colorID = "";
					PTG_Cmplx_BmpBioCaveTdet15Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveTdet15Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveTdet15Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveTdet16Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveTdet16Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveTdet16Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveTdet16Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveTdet16Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveTdet16Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveTdet16Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveTdet16Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveTdet16Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveTdet16Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveTdet16Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveTdet16Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveTdet16Col.colorID = "";
					PTG_Cmplx_BmpBioCaveTdet16Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveTdet16Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveTdet16Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveTdet17Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveTdet17Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveTdet17Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveTdet17Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveTdet17Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveTdet17Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveTdet17Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveTdet17Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveTdet17Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveTdet17Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveTdet17Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveTdet17Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveTdet17Col.colorID = "";
					PTG_Cmplx_BmpBioCaveTdet17Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveTdet17Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveTdet17Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			
			
			%file.readLine(); //Cave Bottom Biome
			PTG_Cmplx_BmpBioCaveBTerPri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
				PTG_Cmplx_BmpBioCaveBTerPri.setBitmap(getField($PTG_PrintArr[%priID],1));
			PTG_Cmplx_SwBioCaveBTerCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwBioCaveBTerPri.setColor(%colStr = getColorI(getColorIDTable(%colID)));
				PTG_Cmplx_SwBioCaveBTerCol.setColor(%colStr);
			
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveBDet0Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveBDet0Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveBDet0Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveBDet0Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveBDet0Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveBDet0Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveBDet0Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveBDet0Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveBDet0Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveBDet0Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveBDet0Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveBDet0Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveBDet0Col.colorID = "";
					PTG_Cmplx_BmpBioCaveBDet0Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveBDet0Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveBDet0Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveBDet1Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveBDet1Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveBDet1Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveBDet1Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveBDet1Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveBDet1Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveBDet1Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveBDet1Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveBDet1Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveBDet1Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveBDet1Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveBDet1Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveBDet1Col.colorID = "";
					PTG_Cmplx_BmpBioCaveBDet1Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveBDet1Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveBDet1Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveBDet2Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveBDet2Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveBDet2Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveBDet2Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveBDet2Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveBDet2Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveBDet2Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveBDet2Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveBDet2Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveBDet2Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveBDet2Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveBDet2Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveBDet2Col.colorID = "";
					PTG_Cmplx_BmpBioCaveBDet2Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveBDet2Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveBDet2Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveBDet3Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveBDet3Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveBDet3Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveBDet3Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveBDet3Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveBDet3Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveBDet3Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveBDet3Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveBDet3Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveBDet3Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveBDet3Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveBDet3Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveBDet3Col.colorID = "";
					PTG_Cmplx_BmpBioCaveBDet3Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveBDet3Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveBDet3Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveBDet4Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveBDet4Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveBDet4Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveBDet4Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveBDet4Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveBDet4Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveBDet4Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveBDet4Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveBDet4Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveBDet4Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveBDet4Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveBDet4Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveBDet4Col.colorID = "";
					PTG_Cmplx_BmpBioCaveBDet4Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveBDet4Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveBDet4Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveBDet5Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveBDet5Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveBDet5Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveBDet5Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveBDet5Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveBDet5Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveBDet5Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveBDet5Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveBDet5Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveBDet5Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveBDet5Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveBDet5Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveBDet5Col.colorID = "";
					PTG_Cmplx_BmpBioCaveBDet5Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveBDet5Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveBDet5Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveBDet6Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveBDet6Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveBDet6Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveBDet6Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveBDet6Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveBDet6Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveBDet6Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveBDet6Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveBDet6Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveBDet6Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveBDet6Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveBDet6Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveBDet6Col.colorID = "";
					PTG_Cmplx_BmpBioCaveBDet6Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveBDet6Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveBDet6Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveBDet7Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveBDet7Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveBDet7Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveBDet7Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveBDet7Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveBDet7Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveBDet7Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveBDet7Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveBDet7Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveBDet7Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveBDet7Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveBDet7Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveBDet7Col.colorID = "";
					PTG_Cmplx_BmpBioCaveBDet7Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveBDet7Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveBDet7Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveBDet8Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveBDet8Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveBDet8Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveBDet8Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveBDet8Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveBDet8Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveBDet8Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveBDet8Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveBDet8Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveBDet8Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveBDet8Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveBDet8Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveBDet8Col.colorID = "";
					PTG_Cmplx_BmpBioCaveBDet8Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveBDet8Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveBDet8Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveBDet9Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveBDet9Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveBDet9Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveBDet9Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveBDet9Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveBDet9Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveBDet9Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveBDet9Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveBDet9Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveBDet9Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveBDet9Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveBDet9Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveBDet9Col.colorID = "";
					PTG_Cmplx_BmpBioCaveBDet9Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveBDet9Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveBDet9Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveBDet10Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveBDet10Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveBDet10Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveBDet10Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveBDet10Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveBDet10Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveBDet10Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveBDet10Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveBDet10Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveBDet10Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveBDet10Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveBDet10Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveBDet10Col.colorID = "";
					PTG_Cmplx_BmpBioCaveBDet10Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveBDet10Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveBDet10Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveBDet11Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveBDet11Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveBDet11Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveBDet11Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveBDet11Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveBDet11Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveBDet11Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveBDet11Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveBDet11Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveBDet11Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveBDet11Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveBDet11Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveBDet11Col.colorID = "";
					PTG_Cmplx_BmpBioCaveBDet11Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveBDet11Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveBDet11Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveBdet12Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveBdet12Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveBdet12Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveBdet12Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveBdet12Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveBdet12Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveBdet12Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveBdet12Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveBdet12Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveBdet12Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveBdet12Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveBdet12Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveBdet12Col.colorID = "";
					PTG_Cmplx_BmpBioCaveBdet12Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveBdet12Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveBdet12Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveBdet13Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveBdet13Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveBdet13Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveBdet13Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveBdet13Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveBdet13Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveBdet13Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveBdet13Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveBdet13Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveBdet13Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveBdet13Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveBdet13Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveBdet13Col.colorID = "";
					PTG_Cmplx_BmpBioCaveBdet13Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveBdet13Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveBdet13Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveBdet14Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveBdet14Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveBdet14Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveBdet14Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveBdet14Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveBdet14Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveBdet14Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveBdet14Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveBdet14Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveBdet14Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveBdet14Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveBdet14Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveBdet14Col.colorID = "";
					PTG_Cmplx_BmpBioCaveBdet14Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveBdet14Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveBdet14Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveBdet15Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveBdet15Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveBdet15Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveBdet15Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveBdet15Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveBdet15Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveBdet15Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveBdet15Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveBdet15Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveBdet15Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveBdet15Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveBdet15Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveBdet15Col.colorID = "";
					PTG_Cmplx_BmpBioCaveBdet15Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveBdet15Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveBdet15Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveBdet16Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveBdet16Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveBdet16Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveBdet16Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveBdet16Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveBdet16Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveBdet16Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveBdet16Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveBdet16Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveBdet16Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveBdet16Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveBdet16Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveBdet16Col.colorID = "";
					PTG_Cmplx_BmpBioCaveBdet16Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveBdet16Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveBdet16Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioCaveBdet17Br.BrickID = %brID;
				PTG_Cmplx_BmpBioCaveBdet17Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioCaveBdet17Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioCaveBdet17Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioCaveBdet17Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioCaveBdet17Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioCaveBdet17Pri.setColor(%colStr);
					PTG_Cmplx_SwBioCaveBdet17Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioCaveBdet17Br.BrickID = "";
				PTG_Cmplx_BmpBioCaveBdet17Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioCaveBdet17Pri.PrintID = "";
					PTG_Cmplx_BmpBioCaveBdet17Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioCaveBdet17Col.colorID = "";
					PTG_Cmplx_BmpBioCaveBdet17Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioCaveBdet17Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioCaveBdet17Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			
			
			%file.readLine(); //Mountain Biome
			PTG_Cmplx_BmpBioMntnRockPri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
				PTG_Cmplx_BmpBioMntnRockPri.setBitmap(getField($PTG_PrintArr[%priID],1));
			PTG_Cmplx_SwBioMntnRockCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwBioMntnRockPri.setColor(%colStr = getColorI(getColorIDTable(%colID)));
				PTG_Cmplx_SwBioMntnRockCol.setColor(%colStr);
			PTG_Cmplx_BmpBioMntnSnowPri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
				PTG_Cmplx_BmpBioMntnSnowPri.setBitmap(getField($PTG_PrintArr[%priID],1));
			PTG_Cmplx_SwBioMntnSnowCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwBioMntnSnowPri.setColor(%colStr = getColorI(getColorIDTable(%colID)));
				PTG_Cmplx_SwBioMntnSnowCol.setColor(%colStr);
			
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioMntnDet0Br.BrickID = %brID;
				PTG_Cmplx_BmpBioMntnDet0Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioMntnDet0Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioMntnDet0Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioMntnDet0Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioMntnDet0Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioMntnDet0Pri.setColor(%colStr);
					PTG_Cmplx_SwBioMntnDet0Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioMntnDet0Br.BrickID = "";
				PTG_Cmplx_BmpBioMntnDet0Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioMntnDet0Pri.PrintID = "";
					PTG_Cmplx_BmpBioMntnDet0Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioMntnDet0Col.colorID = "";
					PTG_Cmplx_BmpBioMntnDet0Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioMntnDet0Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioMntnDet0Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioMntnDet1Br.BrickID = %brID;
				PTG_Cmplx_BmpBioMntnDet1Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioMntnDet1Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioMntnDet1Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioMntnDet1Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioMntnDet1Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioMntnDet1Pri.setColor(%colStr);
					PTG_Cmplx_SwBioMntnDet1Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioMntnDet1Br.BrickID = "";
				PTG_Cmplx_BmpBioMntnDet1Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioMntnDet1Pri.PrintID = "";
					PTG_Cmplx_BmpBioMntnDet1Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioMntnDet1Col.colorID = "";
					PTG_Cmplx_BmpBioMntnDet1Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioMntnDet1Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioMntnDet1Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioMntnDet2Br.BrickID = %brID;
				PTG_Cmplx_BmpBioMntnDet2Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioMntnDet2Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioMntnDet2Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioMntnDet2Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioMntnDet2Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioMntnDet2Pri.setColor(%colStr);
					PTG_Cmplx_SwBioMntnDet2Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioMntnDet2Br.BrickID = "";
				PTG_Cmplx_BmpBioMntnDet2Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioMntnDet2Pri.PrintID = "";
					PTG_Cmplx_BmpBioMntnDet2Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioMntnDet2Col.colorID = "";
					PTG_Cmplx_BmpBioMntnDet2Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioMntnDet2Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioMntnDet2Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioMntnDet3Br.BrickID = %brID;
				PTG_Cmplx_BmpBioMntnDet3Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioMntnDet3Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioMntnDet3Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioMntnDet3Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioMntnDet3Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioMntnDet3Pri.setColor(%colStr);
					PTG_Cmplx_SwBioMntnDet3Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioMntnDet3Br.BrickID = "";
				PTG_Cmplx_BmpBioMntnDet3Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioMntnDet3Pri.PrintID = "";
					PTG_Cmplx_BmpBioMntnDet3Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioMntnDet3Col.colorID = "";
					PTG_Cmplx_BmpBioMntnDet3Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioMntnDet3Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioMntnDet3Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioMntnDet4Br.BrickID = %brID;
				PTG_Cmplx_BmpBioMntnDet4Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioMntnDet4Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioMntnDet4Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioMntnDet4Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioMntnDet4Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioMntnDet4Pri.setColor(%colStr);
					PTG_Cmplx_SwBioMntnDet4Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioMntnDet4Br.BrickID = "";
				PTG_Cmplx_BmpBioMntnDet4Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioMntnDet4Pri.PrintID = "";
					PTG_Cmplx_BmpBioMntnDet4Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioMntnDet4Col.colorID = "";
					PTG_Cmplx_BmpBioMntnDet4Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioMntnDet4Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioMntnDet4Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioMntnDet5Br.BrickID = %brID;
				PTG_Cmplx_BmpBioMntnDet5Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioMntnDet5Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioMntnDet5Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioMntnDet5Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioMntnDet5Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioMntnDet5Pri.setColor(%colStr);
					PTG_Cmplx_SwBioMntnDet5Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioMntnDet5Br.BrickID = "";
				PTG_Cmplx_BmpBioMntnDet5Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioMntnDet5Pri.PrintID = "";
					PTG_Cmplx_BmpBioMntnDet5Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioMntnDet5Col.colorID = "";
					PTG_Cmplx_BmpBioMntnDet5Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioMntnDet5Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioMntnDet5Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioMntnDet6Br.BrickID = %brID;
				PTG_Cmplx_BmpBioMntnDet6Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioMntnDet6Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioMntnDet6Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioMntnDet6Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioMntnDet6Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioMntnDet6Pri.setColor(%colStr);
					PTG_Cmplx_SwBioMntnDet6Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioMntnDet6Br.BrickID = "";
				PTG_Cmplx_BmpBioMntnDet6Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioMntnDet6Pri.PrintID = "";
					PTG_Cmplx_BmpBioMntnDet6Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioMntnDet6Col.colorID = "";
					PTG_Cmplx_BmpBioMntnDet6Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioMntnDet6Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioMntnDet6Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioMntnDet7Br.BrickID = %brID;
				PTG_Cmplx_BmpBioMntnDet7Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioMntnDet7Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioMntnDet7Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioMntnDet7Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioMntnDet7Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioMntnDet7Pri.setColor(%colStr);
					PTG_Cmplx_SwBioMntnDet7Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioMntnDet7Br.BrickID = "";
				PTG_Cmplx_BmpBioMntnDet7Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioMntnDet7Pri.PrintID = "";
					PTG_Cmplx_BmpBioMntnDet7Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioMntnDet7Col.colorID = "";
					PTG_Cmplx_BmpBioMntnDet7Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioMntnDet7Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioMntnDet7Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioMntnDet8Br.BrickID = %brID;
				PTG_Cmplx_BmpBioMntnDet8Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioMntnDet8Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioMntnDet8Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioMntnDet8Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioMntnDet8Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioMntnDet8Pri.setColor(%colStr);
					PTG_Cmplx_SwBioMntnDet8Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioMntnDet8Br.BrickID = "";
				PTG_Cmplx_BmpBioMntnDet8Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioMntnDet8Pri.PrintID = "";
					PTG_Cmplx_BmpBioMntnDet8Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioMntnDet8Col.colorID = "";
					PTG_Cmplx_BmpBioMntnDet8Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioMntnDet8Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioMntnDet8Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioMntnDet9Br.BrickID = %brID;
				PTG_Cmplx_BmpBioMntnDet9Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioMntnDet9Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioMntnDet9Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioMntnDet9Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioMntnDet9Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioMntnDet9Pri.setColor(%colStr);
					PTG_Cmplx_SwBioMntnDet9Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioMntnDet9Br.BrickID = "";
				PTG_Cmplx_BmpBioMntnDet9Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioMntnDet9Pri.PrintID = "";
					PTG_Cmplx_BmpBioMntnDet9Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioMntnDet9Col.colorID = "";
					PTG_Cmplx_BmpBioMntnDet9Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioMntnDet9Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioMntnDet9Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioMntnDet10Br.BrickID = %brID;
				PTG_Cmplx_BmpBioMntnDet10Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioMntnDet10Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioMntnDet10Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioMntnDet10Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioMntnDet10Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioMntnDet10Pri.setColor(%colStr);
					PTG_Cmplx_SwBioMntnDet10Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioMntnDet10Br.BrickID = "";
				PTG_Cmplx_BmpBioMntnDet10Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioMntnDet10Pri.PrintID = "";
					PTG_Cmplx_BmpBioMntnDet10Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioMntnDet10Col.colorID = "";
					PTG_Cmplx_BmpBioMntnDet10Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioMntnDet10Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioMntnDet10Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioMntnDet11Br.BrickID = %brID;
				PTG_Cmplx_BmpBioMntnDet11Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioMntnDet11Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioMntnDet11Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioMntnDet11Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioMntnDet11Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioMntnDet11Pri.setColor(%colStr);
					PTG_Cmplx_SwBioMntnDet11Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioMntnDet11Br.BrickID = "";
				PTG_Cmplx_BmpBioMntnDet11Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioMntnDet11Pri.PrintID = "";
					PTG_Cmplx_BmpBioMntnDet11Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioMntnDet11Col.colorID = "";
					PTG_Cmplx_BmpBioMntnDet11Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioMntnDet11Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioMntnDet11Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioMntndet12Br.BrickID = %brID;
				PTG_Cmplx_BmpBioMntndet12Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioMntndet12Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioMntndet12Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioMntndet12Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioMntndet12Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioMntndet12Pri.setColor(%colStr);
					PTG_Cmplx_SwBioMntndet12Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioMntndet12Br.BrickID = "";
				PTG_Cmplx_BmpBioMntndet12Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioMntndet12Pri.PrintID = "";
					PTG_Cmplx_BmpBioMntndet12Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioMntndet12Col.colorID = "";
					PTG_Cmplx_BmpBioMntndet12Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioMntndet12Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioMntndet12Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioMntndet13Br.BrickID = %brID;
				PTG_Cmplx_BmpBioMntndet13Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioMntndet13Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioMntndet13Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioMntndet13Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioMntndet13Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioMntndet13Pri.setColor(%colStr);
					PTG_Cmplx_SwBioMntndet13Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioMntndet13Br.BrickID = "";
				PTG_Cmplx_BmpBioMntndet13Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioMntndet13Pri.PrintID = "";
					PTG_Cmplx_BmpBioMntndet13Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioMntndet13Col.colorID = "";
					PTG_Cmplx_BmpBioMntndet13Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioMntndet13Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioMntndet13Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioMntndet14Br.BrickID = %brID;
				PTG_Cmplx_BmpBioMntndet14Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioMntndet14Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioMntndet14Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioMntndet14Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioMntndet14Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioMntndet14Pri.setColor(%colStr);
					PTG_Cmplx_SwBioMntndet14Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioMntndet14Br.BrickID = "";
				PTG_Cmplx_BmpBioMntndet14Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioMntndet14Pri.PrintID = "";
					PTG_Cmplx_BmpBioMntndet14Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioMntndet14Col.colorID = "";
					PTG_Cmplx_BmpBioMntndet14Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioMntndet14Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioMntndet14Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioMntndet15Br.BrickID = %brID;
				PTG_Cmplx_BmpBioMntndet15Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioMntndet15Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioMntndet15Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioMntndet15Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioMntndet15Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioMntndet15Pri.setColor(%colStr);
					PTG_Cmplx_SwBioMntndet15Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioMntndet15Br.BrickID = "";
				PTG_Cmplx_BmpBioMntndet15Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioMntndet15Pri.PrintID = "";
					PTG_Cmplx_BmpBioMntndet15Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioMntndet15Col.colorID = "";
					PTG_Cmplx_BmpBioMntndet15Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioMntndet15Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioMntndet15Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioMntndet16Br.BrickID = %brID;
				PTG_Cmplx_BmpBioMntndet16Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioMntndet16Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioMntndet16Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioMntndet16Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioMntndet16Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioMntndet16Pri.setColor(%colStr);
					PTG_Cmplx_SwBioMntndet16Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioMntndet16Br.BrickID = "";
				PTG_Cmplx_BmpBioMntndet16Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioMntndet16Pri.PrintID = "";
					PTG_Cmplx_BmpBioMntndet16Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioMntndet16Col.colorID = "";
					PTG_Cmplx_BmpBioMntndet16Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioMntndet16Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioMntndet16Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",%tmpBrDB = %file.readLine()),"Brick",%tmpBrDB)) != -1)
			{
				PTG_Cmplx_BmpBioMntndet17Br.BrickID = %brID;
				PTG_Cmplx_BmpBioMntndet17Br.setBitmap(%brID.iconName);
				
				PTG_Cmplx_BmpBioMntndet17Pri.PrintID = %priID = $PTG_PermRefArr_Pri[PTG_GUI_PresObjCheck(%tmpPrntUIN = %file.readLine(),"Print",%tmpPrntUIN)];
					PTG_Cmplx_BmpBioMntndet17Pri.setBitmap(getField($PTG_PrintArr[%priID],1));
				PTG_Cmplx_SwBioMntndet17Col.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
					PTG_Cmplx_BmpBioMntndet17Br.mColor = %colStr = getColorI(getColorIDTable(%colID));
					PTG_Cmplx_SwBioMntndet17Pri.setColor(%colStr);
					PTG_Cmplx_SwBioMntndet17Col.setColor(%colStr);
			}
			else
			{
				PTG_Cmplx_BmpBioMntndet17Br.BrickID = "";
				PTG_Cmplx_BmpBioMntndet17Br.setBitmap("base/client/ui/brickicons/Unknown");
				
				PTG_Cmplx_BmpBioMntndet17Pri.PrintID = "";
					PTG_Cmplx_BmpBioMntndet17Pri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_Cmplx_SwBioMntndet17Col.colorID = "";
					PTG_Cmplx_BmpBioMntndet17Br.mColor = %DefMCol;
					PTG_Cmplx_SwBioMntndet17Pri.setColor(%DefMCol);
					PTG_Cmplx_SwBioMntndet17Col.setColor(%DefMCol);
				
				%file.readLine();
				%file.readLine();
			}
			

			%file.readLine(); //Advanced
			PTG_Cmplx_PopupChSize.setSelected(%file.readLine()); //.setValue(getField("16x16 m" TAB "32x32 m" TAB "64x64 m" TAB "128x128 m" TAB "256x256 m",%file.readLine()));
			PTG_Cmplx_SldrCaveTopZMult.setValue(%file.readLine());
			PTG_Cmplx_SldrZMod.setValue(%file.readLine());
			PTG_Cmplx_SldrCnctLakesStrt.setValue(%file.readLine());
			PTG_Cmplx_SwTreeBaseACol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwTreeBaseACol.setColor(%colStr = getColorI(getColorIDTable(%colID)));
			PTG_Cmplx_SwTreeBaseBCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwTreeBaseBCol.setColor(%colStr = getColorI(getColorIDTable(%colID)));
			PTG_Cmplx_SwTreeBaseCCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = (%file.readLine() % 64)) TAB %tmpColorArr[%tmpCol]);
				PTG_Cmplx_SwTreeBaseCCol.setColor(%colStr = getColorI(getColorIDTable(%colID)));
			PTG_Cmplx_ChkFIFOclrChunks.setValue(%file.readLine());
			PTG_Routines_ChkSeamlessModTer.setValue(%file.readLine());
			PTG_Routines_ChkSeamlessBuildL.setValue(%file.readLine());
			
			PTG_Cmplx_EdtTerNosOffX.setValue(%file.readLine()); 
			PTG_Cmplx_EdtTerNosOffY.setValue(%file.readLine());
			PTG_Cmplx_EdtMntnNosOffX.setValue(%file.readLine()); 
			PTG_Cmplx_EdtMntnNosOffY.setValue(%file.readLine());
			PTG_Cmplx_EdtCustANosOffX.setValue(%file.readLine()); 
			PTG_Cmplx_EdtCustANosOffY.setValue(%file.readLine());
			PTG_Cmplx_EdtCustBNosOffX.setValue(%file.readLine()); 
			PTG_Cmplx_EdtCustBNosOffY.setValue(%file.readLine());
			PTG_Cmplx_EdtCustCNosOffX.setValue(%file.readLine()); 
			PTG_Cmplx_EdtCustCNosOffY.setValue(%file.readLine());
			PTG_Cmplx_EdtCaveNosOffX.setValue(%file.readLine()); 
			PTG_Cmplx_EdtCaveNosOffY.setValue(%file.readLine());
			PTG_Cmplx_EdtCaveHNosOffX.setValue(%file.readLine()); 
			PTG_Cmplx_EdtCaveHNosOffY.setValue(%file.readLine());
			PTG_Cmplx_EdtFltIsldANosOffX.setValue(%file.readLine()); 
			PTG_Cmplx_EdtFltIsldANosOffY.setValue(%file.readLine());
			PTG_Cmplx_EdtFltIsldBNosOffX.setValue(%file.readLine()); 
			PTG_Cmplx_EdtFltIsldBNosOffY.setValue(%file.readLine());
			PTG_Cmplx_EdtSkylandNosOffX.setValue(%file.readLine()); 
			PTG_Cmplx_EdtSkylandNosOffY.setValue(%file.readLine());
			PTG_Cmplx_EdtDetNosOffX.setValue(%file.readLine()); 
			PTG_Cmplx_EdtDetNosOffY.setValue(%file.readLine());
			PTG_Cmplx_EdtCloudNosOffX.setValue(%file.readLine()); 
			PTG_Cmplx_EdtCloudNosOffY.setValue(%file.readLine());
			PTG_Cmplx_EdtBldLoadNosOffX.setValue(%file.readLine()); 
			PTG_Cmplx_EdtBldLoadNosOffY.setValue(%file.readLine());
			
			PTG_Cmplx_EdtSectCave.setValue(%file.readLine());
			PTG_Cmplx_EdtSectSkyland.setValue(%file.readLine());
			PTG_Cmplx_EdtSectFltIsld.setValue(%file.readLine());
			PTG_Cmplx_EdtSectCustA.setValue(%file.readLine());
			PTG_Cmplx_EdtSectCustB.setValue(%file.readLine());
			PTG_Cmplx_EdtSectCustC.setValue(%file.readLine());
			PTG_Cmplx_EdtSectCloud.setValue(%file.readLine());
			//PTG_Cmplx_EdtSectCnctLakes.setValue(%file.readLine());
			PTG_Cmplx_EdtSectMntn.setValue(%file.readLine());
			
			PTG_Cmplx_EdtNosScaleTerAXY.setValue(%file.readLine()); 
			PTG_Cmplx_EdtNosScaleTerAZ.setValue(%file.readLine());
			PTG_Cmplx_EdtNosScaleTerBXY.setValue(%file.readLine()); 
			PTG_Cmplx_EdtNosScaleTerBZ.setValue(%file.readLine());
			PTG_Cmplx_EdtNosScaleTerCXY.setValue(%file.readLine()); 
			PTG_Cmplx_EdtNosScaleTerCZ.setValue(%file.readLine());
			PTG_Cmplx_EdtNosScaleMntnAXY.setValue(%file.readLine()); 
			PTG_Cmplx_EdtNosScaleMntnAZ.setValue(%file.readLine());
			PTG_Cmplx_EdtNosScaleMntnBXY.setValue(%file.readLine()); 
			PTG_Cmplx_EdtNosScaleMntnBZ.setValue(%file.readLine());
			PTG_Cmplx_EdtNosScaleCaveAXY.setValue(%file.readLine()); 
			PTG_Cmplx_EdtNosScaleCaveAZ.setValue(%file.readLine());
			PTG_Cmplx_EdtNosScaleCaveBXY.setValue(%file.readLine()); 
			PTG_Cmplx_EdtNosScaleCaveBZ.setValue(%file.readLine());
			PTG_Cmplx_EdtNosScaleCaveCXY.setValue(%file.readLine()); 
			PTG_Cmplx_EdtNosScaleCaveCZ.setValue(%file.readLine());
			PTG_Cmplx_EdtNosScaleCaveHXY.setValue(%file.readLine()); 
			PTG_Cmplx_EdtNosScaleCaveHZ.setValue(%file.readLine());
			PTG_Cmplx_EdtNosScaleCustAXY.setValue(%file.readLine()); 
			PTG_Cmplx_EdtNosScaleCustAZ.setValue(%file.readLine());
			PTG_Cmplx_EdtNosScaleCustBXY.setValue(%file.readLine()); 
			PTG_Cmplx_EdtNosScaleCustBZ.setValue(%file.readLine());
			PTG_Cmplx_EdtNosScaleCustCXY.setValue(%file.readLine()); 
			PTG_Cmplx_EdtNosScaleCustCZ.setValue(%file.readLine());
			PTG_Cmplx_EdtNosScaleSkylandXY.setValue(%file.readLine()); 
			PTG_Cmplx_EdtNosScaleSkylandZ.setValue(%file.readLine());
			PTG_Cmplx_EdtNosScaleCloudAXY.setValue(%file.readLine()); 
			PTG_Cmplx_EdtNosScaleCloudAZ.setValue(%file.readLine());
			PTG_Cmplx_EdtNosScaleCloudBXY.setValue(%file.readLine()); 
			PTG_Cmplx_EdtNosScaleCloudBZ.setValue(%file.readLine());
			PTG_Cmplx_EdtNosScaleFltIsldAXY.setValue(%file.readLine()); 
			PTG_Cmplx_EdtNosScaleFltIsldAZ.setValue(%file.readLine());
			PTG_Cmplx_EdtNosScaleFltIsldBXY.setValue(%file.readLine()); 
			PTG_Cmplx_EdtNosScaleFltIsldBZ.setValue(%file.readLine());
			
			PTG_Cmplx_ChkEnabPseudoEqtr.setValue(%file.readLine());
			PTG_Cmplx_EdtEquatorCustAY.setValue(%file.readLine());
			PTG_Cmplx_EdtEquatorCustBY.setValue(%file.readLine());
			PTG_Cmplx_EdtEquatorCustCY.setValue(%file.readLine());
			PTG_Cmplx_EdtEquatorCaveY.setValue(%file.readLine());
			PTG_Cmplx_EdtEquatorMntnY.setValue(%file.readLine());
			PTG_Cmplx_EdtEquatorCloudY.setValue(%file.readLine());
			PTG_Cmplx_EdtEquatorFltIsldY.setValue(%file.readLine());
			
			
			
			//Mass Detail List
			
			%file.readLine();
			
			//Clear data stored in brick, print and color buttons
			PTG_DetLst_BmpMassDetBr.BrickID = "";
				PTG_DetLst_BmpMassDetBr.setBitmap("base/client/ui/brickicons/Unknown");
			PTG_DetLst_BmpMassDetPri.PrintID = "";
				PTG_DetLst_BmpMassDetPri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
			PTG_DetLst_SwMassDetCol.colorID = "";
				PTG_DetLst_BmpMassDetBr.mColor = %DefMCol;
				PTG_DetLst_SwMassDetPri.setColor(%DefMCol);
				PTG_DetLst_SwMassDetCol.setColor(%DefMCol);
			
			//Clear past GUI list and checkbox data
			for(%c = 0; %c < 9; %c++)
			{
				PTG_DetLst_SwTxtLstGroup.getObject(%c).getObject(0).clear();
				PTG_DetLst_SwChkGroup.getObject(%c).setValue(0);
			}
			
			while(!%file.isEOF && %failSafe++ <= 3609) //3609 = (400 * 9) + 9
			{
				%readLn = %file.readLine();
				%BioName = getField(%readLn,0);
				
				switch$(%BioName)
				{
					case ">>Default":
						%relBioList = "PTG_DetLst_TxtLst_DefBio";
						PTG_DetLst_ChkEnab_DefBio.setValue(getField(%readLn,1));
						%rCount = 0;
					case ">>Shore":
						%relBioList = "PTG_DetLst_TxtLst_ShoreBio";
						PTG_DetLst_ChkEnab_ShoreBio.setValue(getField(%readLn,1));
						%rCount = 0;
					case ">>SubMarine":
						%relBioList = "PTG_DetLst_TxtLst_SubMBio";
						PTG_DetLst_ChkEnab_SubMBio.setValue(getField(%readLn,1));
						%rCount = 0;
					case ">>CustomA":
						%relBioList = "PTG_DetLst_TxtLst_CustBioA";
						PTG_DetLst_ChkEnab_CustBioA.setValue(getField(%readLn,1));
						%rCount = 0;
					case ">>CustomB":
						%relBioList = "PTG_DetLst_TxtLst_CustBioB";
						PTG_DetLst_ChkEnab_CustBioB.setValue(getField(%readLn,1));
						%rCount = 0;
					case ">>CustomC":
						%relBioList = "PTG_DetLst_TxtLst_CustBioC";
						PTG_DetLst_ChkEnab_CustBioC.setValue(getField(%readLn,1));
						%rCount = 0;
					case ">>CaveTop":
						%relBioList = "PTG_DetLst_TxtLst_CaveTBio";
						PTG_DetLst_ChkEnab_CaveTBio.setValue(getField(%readLn,1));
						%rCount = 0;
					case ">>CaveBottom":
						%relBioList = "PTG_DetLst_TxtLst_CaveBBio";
						PTG_DetLst_ChkEnab_CaveBBio.setValue(getField(%readLn,1));
						%rCount = 0;
					case ">>Mountains":
						%relBioList = "PTG_DetLst_TxtLst_MntnBio";
						PTG_DetLst_ChkEnab_MntnBio.setValue(getField(%readLn,1));
						%rCount = 0;
					case ">End":
						break;
					default:
						if(isObject(%relBioList))
						{
							%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID",getFields(%readLn,0,2)),"Brick",%tmpBrDB);
							//
							//PTG_GUI_PresObjCheck($PTG_PermRefArr_Pri[%tmpPrntUIN = getField(%readLn,4)],"Print",%tmpPrntUIN)
							
							if(%brID == -1 || strReplace(getField(%readLn,0)," ","") $= "")
							{
								%colMod = "\c4"; //red text if brick doesn't exist on server
								%detExists = false;
							}
							else
							{
								%colMod = ""; 
								%detExists = true; //simplifies detail removal from list
							}
							
							%readLn = setField(%readLn,3,PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB (%tmpCol = getField(%readLn,3) % 64) TAB %tmpColorArr[%tmpCol]));
							%priID = PTG_GUI_PresObjCheck($PTG_PermRefArr_Pri[%tmpPrntUIN = getField(%readLn,4)],"Print",%tmpPrntUIN); //$PTG_PermRefArr_Pri[getField(%readLn,4)];
							%relBioList.addRow(%rCount,%colMod @ %rCount+1 TAB %readLn TAB %brID TAB %priID TAB %detExists,%rCount);
							%rCount++;
						}
				}
			}
				
			
			
			//Finalize / End Function
			%file.close();
			%file.delete();
			
			deleteVariables("$PTG_TmpRefArr_Br*");
			deleteVariables("$PTG_TmpRefArr_Col*");
			
			PTG_GUI_UpdateSecCutVals("All"); //update section cut text under advanced settings in GUI

			
			if(%action !$= "LoadDefault") //no need to message player when GUI first loads default settings
			{
				if($PTG_PresErrPrntCnt > 0 || $PTG_PresErrBrCnt > 0)
					PTG_GUI_PresObjCheck("","End"); //preset load error report (also delete reference global vars used above)
				else
				{
					if(%def)
						CLIENTCMDPTG_ReceiveMsg("Success","PTG: Preset Load Successful","Default settings loaded successfully into the GUI.");
					else
						CLIENTCMDPTG_ReceiveMsg("Success","PTG: Preset Load Successful","Preset file \"" @ %fileName @ "\" loaded successfully into the GUI.");
				}
			}
			else
			{
				//If loading default settings, also reset Mass Detail List GUI and Simplex GUI
				PTG_DetLst_BmpMassDetBr.BrickID = "";
				PTG_DetLst_BmpMassDetBr.setBitmap("base/client/ui/brickicons/Unknown");
				PTG_DetLst_BmpMassDetBr.mColor = %DefMCol;
				
				PTG_DetLst_SwMassDetPri.color = %DefMCol;
				PTG_DetLst_BmpMassDetPri.setBitmap("Add-Ons/System_PTG/GUIs/noprint");
				
				PTG_DetLst_SwMassDetCol.color = %DefMCol;
				PTG_DetLst_SwMassDetCol.colorID = "";
				
				
				//////////////////////////////////////////////////
				//Simplex GUI
				
				
				PTG_Smplx_EdtSeed.setValue(1237);
				
				//Terrain Brick
				if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID","8x8" TAB "Bricks" TAB "8x" TAB "brick8x8Data"),"Brick","8x8" TAB "Bricks" TAB "8x" TAB "brick8x8Data")) != -1) //Brick8x8Data
				{
					PTG_Smplx_BmpTerBr.BrickID = %brID;
					PTG_Smplx_BmpTerBr.setBitmap(%brID.iconName);
				}
				else
				{
					PTG_Smplx_BmpTerBr.BrickID = "";
					PTG_Smplx_BmpTerBr.setBitmap("base/client/ui/brickicons/Unknown");
					echo("\c2>>PTG ERROR: Terrain brick \"8x8\" for Simplex GUI not found!");
				}
				PTG_Smplx_BmpTerBr.ModTer = false;
				
				PTG_Smplx_BmpTerBr.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB 38 TAB %tmpColorArr[38]);
					PTG_Smplx_BmpTerBr.mColor = getColorI(getColorIDTable(%colID));
		
				//Cloud Brick
				if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID","8x Cube 1/2h" TAB "Baseplates" TAB "ModTer 8x" TAB "brick8Cube3Data"),"Brick","8x Cube 1/2h" TAB "Baseplates" TAB "ModTer 8x" TAB "brick8Cube3Data")) != -1)
				{
					PTG_Smplx_BmpCloudBr.BrickID = %brID;
					PTG_Smplx_BmpCloudBr.setBitmap(%brID.iconName);
					PTG_Smplx_BmpCloudBr.ModTer = true;
				}
				else
				{
					if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID","8x Cube" TAB "Baseplates" TAB "Cube" TAB "brick8xCubeData"),"Brick","8x Cube" TAB "Baseplates" TAB "Cube" TAB "brick8xCubeData")) != -1)
					{
						PTG_Smplx_BmpCloudBr.BrickID = %brID;
						PTG_Smplx_BmpCloudBr.setBitmap(%brID.iconName);
						PTG_Smplx_BmpCloudBr.ModTer = true;
						echo("\c4>>\c2P\c1T\c4G: \c0Cloud brick \"8x Cube 1/2h\" wasn't found for Simplex GUI, using \"8x Cube\" brick instead for clouds.");
					}
					else
					{
						PTG_Smplx_BmpCloudBr.BrickID = "";
						PTG_Smplx_BmpCloudBr.setBitmap("base/client/ui/brickicons/Unknown");
						PTG_Smplx_BmpCloudBr.ModTer = false;
						echo("\c2>>PTG ERROR: Cloud brick \"8x Cube\" for Simplex GUI not found!");
					}
				}
				
				PTG_Smplx_BmpCloudBr.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB 45 TAB %tmpColorArr[45]);
					PTG_Smplx_BmpCloudBr.mColor = getColorI(getColorIDTable(%colID));
					
				//Floating Islands Brick
				if((%brID = PTG_GUI_PresObjCheck(PTG_GUI_GetRelObjID("BrickID","16x16" TAB "Special" TAB "PTG Custom" TAB "brick16x16Data"),"Brick","16x16" TAB "Special" TAB "PTG Custom" TAB "brick16x16Data")) != -1)
				{
					PTG_Smplx_BmpFltIsldsBr.BrickID = %brID;
					PTG_Smplx_BmpFltIsldsBr.setBitmap(%brID.iconName);
				}
				else
				{
					PTG_Smplx_BmpFltIsldsBr.BrickID = "";
					PTG_Smplx_BmpFltIsldsBr.setBitmap("base/client/ui/brickicons/Unknown");
					echo("\c2>>PTG ERROR: Floating Island brick \"16x16\" for Simplex GUI not found!");
				}
				PTG_Smplx_BmpFltIsldsBr.ModTer = false;
				
				PTG_Smplx_BmpFltIsldsBr.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",%colorSetsMatch TAB 38 TAB %tmpColorArr[38]);
					PTG_Smplx_BmpFltIsldsBr.mColor = getColorI(getColorIDTable(%colID));
					
				//Popup Menus
				PTG_Smplx_ChkInfTer.setValue(0);
				PTG_Smplx_ChkEdgeFallOff.setValue(0);
				PTG_Smplx_SldrTerLengthY.setValue(256);
				PTG_Smplx_SldrTerWidthX.setValue(256);
				PTG_Smplx_PopUpTerType.setSelected(0);

				PTG_Smplx_ChkEnabMntns.setValue(0);
				PTG_Smplx_ChkEnabCaves.setValue(0);
				PTG_Smplx_ChkEnabLakes.setValue(1);
				PTG_Smplx_ChkEnabClouds.setValue(0);
				PTG_Smplx_ChkEnabFltIslds.setValue(0);
				PTG_Smplx_ChkEnabBounds.setValue(0);
			
				PTG_Smplx_PopUpBioDef.setSelected(0); //Forest
				PTG_Smplx_PopUpBioCustA.setSelected(0); //None
				PTG_Smplx_PopUpBioCustB.setSelected(0); //None
				PTG_Smplx_PopUpBioCustC.setSelected(0); //None
			}
			
			//Delete error reference vars (even if loading default settings - vars are still set up, but error report won't show)
			deleteVariables("$PTG_PresErrPrntArr_Chk*");
			deleteVariables("$PTG_PresErrPrntArr*");
			deleteVariables("$PTG_PresErrPrntCnt");
			
			deleteVariables("$PTG_PresErrBrDBArr_Chk*");
			deleteVariables("$PTG_PresErrBrDBArr*");
			deleteVariables("$PTG_PresErrBrCnt");
		
		//////////////////////////////////////////////////
		
		case "List":
		
			//setModPaths(getModPaths()); //Check for new preset files added at runtime? Could cause client lag...
			
			//if((%count = getFileCount("Config/Client/PTGv3/Presets/" @ "*.txt")) == 0 && !PTG_Complex.noPresMsg)
			//{
			//	CLIENTCMDPTG_ReceiveMsg("Info","PTG: No Presets Found","No custom preset saves were found. You can create a new preset by first typing a name and description in the text boxes. Then, select the \"Save New\" button to save your GUI settings.");
			//	PTG_Complex.noPresMsg = true;
				
			//	//return; (don't end function so that default settings can be added to list)
			//}
			
			%count = getFileCount("Config/Client/PTGv3/Presets/" @ "*.txt");
			PTG_Cmplx_TxtListPresets.clear();
			PTG_Cmplx_BmpPresetPrev.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
			
			PTG_Cmplx_TxtListPresets.addRow(0,"<<Default Settings>>",0);
			
			for(%c = 0; %c < %count; %c++)
				PTG_Cmplx_TxtListPresets.addRow(%c+1,fileBase(findNextFile("Config/Client/PTGv3/Presets/*.txt")),%c+1);
			
			PTG_Cmplx_TxtListPresets.sort(0,1);
			PTG_Cmplx_BtnPresSort.sorted = 0;
			
		//////////////////////////////////////////////////
		
		case "Remove":
		
			if(strReplace((%fileName = PTG_Cmplx_TxtListPresets.getValue())," ","") $= "")
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: No Preset Selected","No preset selected!");
				return;
			}
			if(PTG_Cmplx_TxtListPresets.getValue() $= "<<Default Settings>>")
			{
				CLIENTCMDPTG_ReceiveMsg("Error","PTG: Preset Removal Failed","Can't remove default settings!");
				return;
			}
			
			if(isFile(%fp = "Config/Client/PTGv3/Presets/" @ (%fileName = PTG_Cmplx_TxtListPresets.getValue()) @ ".txt"))
				%fileExists = true;
			
			if(%fileExists)
			{
				if(!%inFldr)
				{
					fileDelete(%fp);
					
					if(isFile(%fpBmp = strReplace(%fp,".txt",".jpg")))
						fileDelete(%fpBmp);
					
					if(!isFile(%fp))
						%fileRem = true;
				}
				
				if(%fileRem)
					CLIENTCMDPTG_ReceiveMsg("Success","PTG: Preset Remove Successful","Preset file \"" @ %fileName @ "\" removed successfully.");
				else
					CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Preset Removal Failed","Preset file \"" @ %fileName @ "\" couldn't be deleted. You might need to remove this file manually by navigating to the directory: Config/Client/PTGv3/Presets/");
				
				PTG_GUI_PresetFuncs("List"); //update list
				PTG_Cmplx_BmpPresetPrev.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
			}
			else
				CLIENTCMDPTG_ReceiveMsg("Error","PTG: Preset Removal Error","Preset file \"" @ %fileName @ "\" either doesn't exist, or is nested in a folder / .zip file and can't be removed through the Preset Manager.");
			
		//////////////////////////////////////////////////
		
		case "Rename":

			%fileNameA = PTG_Cmplx_TxtListPresets.getValue();
			%fileNameB = strReplace(PTG_Cmplx_EdtPresetName.getValue(),"/","-"); //prevent issues w/ forward slash char //PTG_Cmplx_EdtPresetName.getValue();
			%fpA = "Config/Client/PTGv3/Presets/" @ %fileNameA @ ".txt";
			%fpB = strReplace(%fpA,%fileNameA,%fileNameB);
			
			if(strReplace(%fileNameA," ","") $= "")
			{
				CLIENTCMDPTG_ReceiveMsg("Error","PTG: No Preset Selected","No preset selected!");
				return;
			}
			if(%fileNameB $= "Preset Name" || strReplace(%fileNameB," ","") $= "")
			{
				CLIENTCMDPTG_ReceiveMsg("Error","PTG: Preset Rename Failed","No valid name entered!");
				return;
			}
			if(%fileNameA $= "<<Default Settings>>")
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Preset Rename Failed","Can't rename default preset!");
				return;
			}
			if(!isWriteableFileName(%fpB))
			{
				CLIENTCMDPTG_ReceiveMsg("Error","PTG: Preset Rename Failed","New file name contains invalid text characters for saving!");
				return;
			}
			if(%fileNameB $= %fileNameA)
			{
				CLIENTCMDPTG_ReceiveMsg("Error","PTG: Preset Rename Failed","File names are exactly the same!");
				return;
			}
			
			if(isFile(%fpA))
			{
				fileCopy(%fpA,%fpB);
				fileDelete(%fpA);
				discoverFile(%fpB);  //file was created at runtime, so tell game to include the new file - also can use "getModPaths(setModPaths());" but it might cause serious lag
				
				if(isFile(%fpAbmp = strReplace(%fpA,".txt",".jpg")))
				{
					fileCopy(%fpAbmp,%fpBbmp = "Config/Client/PTGv3/Presets/" @ %fileNameB @ ".jpg");
					fileDelete(%fpAbmp);
					discoverFile(%fpBbmp);
				}

				PTG_GUI_PresetFuncs("List"); //update list
				
				if(isFile(%fpBbmp))
					CLIENTCMDPTG_ReceiveMsg("Success","PTG: Preset Rename Successful","Preset file \"" @ %fileNameA @ "\" renamed successfully to \"" @ %fileNameB @ "\"");
				else
					CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Preset Rename Failed","Unknown issue with renaming the preset \"" @ %fileNameA @ "\"; rename failed.");
			}
			else
				CLIENTCMDPTG_ReceiveMsg("Error","PTG: Preset Rename Error","Preset file \"" @ %fileNameA @ "\" either doesn't exist, or is in a .zip file or folder and can't be renamed through the Preset Manager.");
			
		//////////////////////////////////////////////////
		
		case "LoadDesc":
		
			if(PTG_Cmplx_TxtListPresets.getValue() $= "<<Default Settings>>")
			{
				PTG_Cmplx_EdtPresetName.setText("Default Settings");
				PTG_Cmplx_EdtPresetDesc.setText("Standard settings for the PTG Main GUI.");
				PTG_Cmplx_BmpPresetPrev.setBitmap("Add-Ons/System_PTG/Presets/Default");
				
				return;
			}
			
			if(isFile(%fp = findFirstFile("Config/Client/PTGv3/Presets/" @ (%fileName = PTG_Cmplx_TxtListPresets.getValue()) @ ".txt")))
				%fileExists = true;
			else if(isFile(%fp = findFirstFile("Config/Client/PTGv3/Presets/" @ "*/" @ %fileName @ ".txt")))
				%fileExists = true;

			if(%fileExists)
			{
				%file = new FileObject();
				%file.openForRead(%fp);
				
				%file.readLine();
				%desc = getSubStr(%file.readLine(),0,255);
				
				PTG_Cmplx_EdtPresetName.setText(%fileName);
				PTG_Cmplx_EdtPresetDesc.setText(%desc);
				
				if(isFile(%fpBmp = strReplace(%fp,".txt",".jpg")))
					PTG_Cmplx_BmpPresetPrev.setBitmap(%fpBmp);
				else
					PTG_Cmplx_BmpPresetPrev.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				
				%file.close();
				%file.delete();
			}
	}
}


////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_PresObjCheck(%tmpObjRef,%type,%tmpObjUIName)
{
	switch$(%type)
	{
		case "Print":
		
			%tmpObjRef = strReplace(%tmpObjRef," ","");
			
			//check if print exists
			if($PTG_PermRefArr_Pri[%tmpObjRef] $= "" && %tmpObjUIName !$= "" && !$PTG_PresErrPrntArr_Chk[%tmpObjUIName])
			{
				$PTG_PresErrPrntArr[$PTG_PresErrPrntCnt++] = %tmpObjUIName;
				$PTG_PresErrPrntArr_Chk[%tmpObjUIName] = true;
			}
			//else
			//	%tmpObjUIName = "";

			return %tmpObjUIName; //return print UI Name for preset loading function

		//////////////////////////////////////////////////

		case "Brick":

			//brick data (such as category, subcategory, etc.) stored as separate fields in preset file; we only need the name below
			%tmpObjUIName = getField(%tmpObjUIName,0);
			
			//check if brick exists
			if(%tmpObjRef == -1 && %tmpObjUIName !$= "" && !$PTG_PresErrBrDBArr_Chk[%tmpObjUIName])
			{
				$PTG_PresErrBrDBArr[$PTG_PresErrBrCnt++] = %tmpObjUIName;
				$PTG_PresErrBrDBArr_Chk[%tmpObjUIName] = true;
			}
			
			return %tmpObjRef; //return brick datablock name for preset loading function

		//////////////////////////////////////////////////

		case "End":
		
			//Don't send error report if loading default settings

			if(!PTG_ErrRprt_ChkAvoidMsg.getValue())
			{
				PTG_ErrRprt_TxtLst.clear();
				
				%errChkA = $PTG_PresErrPrntCnt > 0;
				%errChkB = $PTG_PresErrBrCnt > 0;
				%errChkC = $PTG_PresErrPrntCnt > 0 && $PTG_PresErrBrCnt > 0;
				
				//If some brick and prints couldn't be converted / found
				if(%errChkC)
				{
					//CLIENTCMDPTG_ReceiveMsg("Error","PTG: GUI Preset Loaded","Preset settings were loaded successfully, but some bricks and prints couldn't be loaded because they don't exist on the server; full error report in console.");
					PTG_ErrRprt_TxtLst.addRow(0,"\c5Bricks",0);
						
					for(%c = 1; %c <= $PTG_PresErrBrCnt; %c++)
						PTG_ErrRprt_TxtLst.addRow(0,"     " @ $PTG_PresErrBrDBArr[%c],%c+1);
					
					PTG_ErrRprt_TxtLst.addRow(0,"",%c+1);
					PTG_ErrRprt_TxtLst.addRow(0,"\c5Prints",%c+2);
						
					for(%d = 1; %d <= $PTG_PresErrPrntCnt; %d++)
						PTG_ErrRprt_TxtLst.addRow(0,"     " @ $PTG_PresErrPrntArr[%d],%c+2+%d);
				}
				else
				{
					//If some brick couldn't be converted / found
					if(%errChkB)
					{
						//CLIENTCMDPTG_ReceiveMsg("Error","PTG: GUI Preset Loaded","Preset settings were loaded successfully, but some bricks couldn't be loaded because they don't exist on the server; full error report in console.");
						PTG_ErrRprt_TxtLst.addRow(0,"\c5Bricks",0);
						
						for(%c = 1; %c <= $PTG_PresErrBrCnt; %c++)
							PTG_ErrRprt_TxtLst.addRow(0,"     " @ $PTG_PresErrBrDBArr[%c],%c+1);
					}
					
					//If some prints couldn't be converted / found
					else
					{
						//CLIENTCMDPTG_ReceiveMsg("Error","PTG: GUI Preset Loaded","Preset settings were loaded successfully, but some prints couldn't be loaded because they don't exist on the server; full error report in console.");
						PTG_ErrRprt_TxtLst.addRow(0,"\c5Prints",0);
						
						for(%c = 1; %c <= $PTG_PresErrPrntCnt; %c++)
							PTG_ErrRprt_TxtLst.addRow(0,"     " @ $PTG_PresErrPrntArr[%c],%c+1);
					}
				}
				
				
				canvas.pushDialog(PTG_ErrorReport);
			}
			else
				CLIENTCMDPTG_ReceiveMsg("SuccessError","PTG: Preset Loaded","Preset settings were loaded successfully, but some bricks and / or prints couldn't be loaded because they don't exist on the server.");
			
			
			deleteVariables("$PTG_PresErrPrntArr_Chk*");
			deleteVariables("$PTG_PresErrPrntArr*");
			deleteVariables("$PTG_PresErrPrntCnt");
			
			deleteVariables("$PTG_PresErrBrDBArr_Chk*");
			deleteVariables("$PTG_PresErrBrDBArr*");
			deleteVariables("$PTG_PresErrBrCnt");
	}
}