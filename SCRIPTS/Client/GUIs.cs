function PTG_GUI_ConfirmMsg(%type)
{
	PTG_Confirm_ChSavesWndw.setVisible(false);
	PTG_Confirm_PresetOWWndw.setVisible(false);
	PTG_Confirm_ClrPresetWndw.setVisible(false);
	PTG_Confirm_SetUploadByPassWndw.setVisible(false);
	PTG_Confirm_PrevSizeErrWndw.setVisible(false);
			
	switch$(%type)
	{
		case "ClearChSaves":
			PTG_Confirm_ChSavesWndw.setVisible(true);
		case "OverWritePres":
			PTG_Confirm_PresetOWWndw.setVisible(true);
		case "RemovePres":
			PTG_Confirm_ClrPresetWndw.setVisible(true);
		case "UploadPriByPass":
			PTG_Confirm_SetUploadByPassWndw.setVisible(true);
		case "PrevSizeErr":
			PTG_Confirm_PrevSizeErrWndw.setVisible(true);
	}
	
	if(getRandom(0,1))
		PTG_Confirm_BGa.setVisible(1);
	else
		PTG_Confirm_BGa.setVisible(0);
	if(getRandom(0,1))
		PTG_Confirm_BGb.setVisible(1);
	else
		PTG_Confirm_BGb.setVisible(0);
	if(getRandom(0,1))
		PTG_Confirm_BGc.setVisible(1);
	else
		PTG_Confirm_BGc.setVisible(0);
	
	canvas.pushDialog(PTG_Confirm);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//// FUNCTION FOR SWITCHING / RESIZING VARIOUS GUI WINDOWS ////
function PTG_GUI_SwitchWndw(%wndw,%wndwType)
{
	switch$(%wndwType)
	{
		case "Main":
		
			for(%c = 0; %c < 8; %c++) //replace .getCount()
				PTG_Cmplx_BmpWndwGroupBG.getObject(%c).setVisible(false);
			
			switch$(%wndw)
			{
				case "Preview":
					PTG_Cmplx_WndwPrev.setVisible(true);
					PTG_Cmplx_MainWndw.setText("PTG Complex GUI -> Preview");
				case "Setup":
					PTG_Cmplx_WndwStp.setVisible(true);
					PTG_Cmplx_MainWndw.setText("PTG Complex GUI -> Setup");
				case "Features":
					PTG_Cmplx_WndwFtrs.setVisible(true);
					PTG_Cmplx_MainWndw.setText("PTG Complex GUI -> Features");
				case "Biomes":
					PTG_Cmplx_WndwBio.setVisible(true);
					PTG_Cmplx_MainWndw.setText("PTG Complex GUI -> Biomes");
				case "BuildLoading":
					PTG_Cmplx_WndwBldL.setVisible(true);
					PTG_Cmplx_MainWndw.setText("PTG Complex GUI -> Build Loading");
				case "Presets":
					PTG_Cmplx_WndwPres.setVisible(true);
					PTG_Cmplx_MainWndw.setText("PTG Complex GUI -> Presets");
				case "Advanced":
					PTG_Cmplx_WndwAdv.setVisible(true);
					PTG_Cmplx_MainWndw.setText("PTG Complex GUI -> Advanced");
				case "Routines":
					PTG_Cmplx_WndwRtns.setVisible(true);
					PTG_Cmplx_MainWndw.setText("PTG Complex GUI -> Routines");
			}
	
		//////////////////////////////////////////////////
		//Biome SubWindow Selection
	
		case "Biomes":
		
			for(%c = 0; %c < PTG_Cmplx_ScrollBioSubWndws.getCount(); %c++) //replace .getCount()
			{
				%tmpWndw = PTG_Cmplx_ScrollBioSubWndws.getObject(%c);
				%tmpWndw.setVisible(false);
			}
			
			switch$(%wndw)
			{
				case "Default Biome":
					PTG_Cmplx_SubWndwBioDef.setVisible(true);
					PTG_Cmplx_TitleBioOpts.setText("<font:impact:33><color:ffffff>Default Biome");
				case "Shore":
					PTG_Cmplx_SubWndwBioShore.setVisible(true);
					PTG_Cmplx_TitleBioOpts.setText("<font:impact:33><color:ffffff>Shore Biome");
				case "SubMarine":
					PTG_Cmplx_SubWndwBioSubM.setVisible(true);
					PTG_Cmplx_TitleBioOpts.setVisible(true);
					PTG_Cmplx_TitleBioOpts.setText("<font:impact:33><color:ffffff>Submarine Biome");
				case "Custom Biome A":
					PTG_Cmplx_SubWndwBioCustA.setVisible(true);
					PTG_Cmplx_TitleBioOpts.setText("<font:impact:33><color:ffffff>Custom Biome A");
				case "Custom Biome B":
					PTG_Cmplx_SubWndwBioCustB.setVisible(true);
					PTG_Cmplx_TitleBioOpts.setText("<font:impact:33><color:ffffff>Custom Biome B");
				case "Custom Biome C":
					PTG_Cmplx_SubWndwBioCustC.setVisible(true);
					PTG_Cmplx_TitleBioOpts.setText("<font:impact:33><color:ffffff>Custom Biome C");
				case "Cave Top":
					PTG_Cmplx_SubWndwBioCaveT.setVisible(true);
					PTG_Cmplx_TitleBioOpts.setText("<font:impact:33><color:ffffff>Cave Top Biome");
				case "Cave Bottom":
					PTG_Cmplx_SubWndwBioCaveB.setVisible(true);
					PTG_Cmplx_TitleBioOpts.setText("<font:impact:33><color:ffffff>Cave Bottom Biome");
				case "Mountains":
					PTG_Cmplx_SubWndwBioMntn.setVisible(true);
					PTG_Cmplx_TitleBioOpts.setText("<font:impact:33><color:ffffff>Mountains Biome");
			}
			
		//////////////////////////////////////////////////
		//Details SubWindow Selection
			
		case "Details":

			for(%c = 0; %c < PTG_DetBrSelect_SwMainSubWndw.getCount(); %c++) //replace .getCount()
			{
				%tmpWndw = PTG_DetBrSelect_SwMainSubWndw.getObject(%c);
				
				if(%tmpWndw.getName() !$= "PTGNoneSel") //check!!!
					%tmpWndw.setVisible(false);
			}

			if(isObject(%wndw))
				%wndw.setVisible(true);
		
		//////////////////////////////////////////////////
		//Build-Loading SubWindow Selection
		
		case "BuildLoad":
		
			switch$(%wndw)
			{
				case "Options":
					PTG_Cmplx_SubWndw_Options.setVisible(1);
					PTG_Cmplx_SubWndw_PlyrSaves.setVisible(0);
					PTG_Cmplx_SubWndw_LoadedBuilds.setVisible(0);
				case "PlyrBuilds":
					PTG_Cmplx_SubWndw_Options.setVisible(0);
					PTG_Cmplx_SubWndw_PlyrSaves.setVisible(1);
					PTG_Cmplx_SubWndw_LoadedBuilds.setVisible(0);
				case "LoadedBuilds":
					PTG_Cmplx_SubWndw_Options.setVisible(0);
					PTG_Cmplx_SubWndw_PlyrSaves.setVisible(0);
					PTG_Cmplx_SubWndw_LoadedBuilds.setVisible(1);
			}
		
		//////////////////////////////////////////////////
		//Shrink / Enlarge Miniature Control Window (In Complex GUI)
		
		case "MiniCtrlWndw":
		
			//Expand
			if(PTG_Cmplx_WndwCtrlList_TxtMore.bitmap $= "add-ons/system_ptg/guis/more")
			{
				PTG_Cmplx_WndwCtrlList_BtnClear.setVisible(0);
				PTG_Cmplx_WndwCtrlList.resize(getWord(PTG_Cmplx_WndwCtrlList.position,0),getWord(PTG_Cmplx_WndwCtrlList.position,1),404,234);
				PTG_Cmplx_WndwCtrlList_TxtMore.setBitmap("add-ons/system_ptg/guis/less");
				PTG_Cmplx_SwCtrlWndw.resize(4,26,396,204);
				PTG_Cmplx_WndwCtrlList_BtnHelpA.setVisible(0);
				PTG_Cmplx_WndwCtrlList_BtnHelpB.setVisible(1);
				PTG_Cmplx_WndwCtrlList_BtnApplyStart.setVisible(1);
			}
			
			//Contract
			else
			{
				PTG_Cmplx_WndwCtrlList_BtnClear.setVisible(1);
				PTG_Cmplx_WndwCtrlList.resize(getWord(PTG_Cmplx_WndwCtrlList.position,0),getWord(PTG_Cmplx_WndwCtrlList.position,1),154,192);
				PTG_Cmplx_WndwCtrlList_TxtMore.setBitmap("add-ons/system_ptg/guis/more");
				PTG_Cmplx_SwCtrlWndw.resize(4,26,146,162);
				PTG_Cmplx_WndwCtrlList_BtnHelpA.setVisible(1);
				PTG_Cmplx_WndwCtrlList_BtnHelpB.setVisible(0);
				PTG_Cmplx_WndwCtrlList_BtnApplyStart.setVisible(0);
			}
		
		//////////////////////////////////////////////////
		//Shrink / Enlarge Chunk Manager
		
		case "ChunkMngr":
		
			%posX = getWord(ptg_chmngr_wndw.position,0);
			%posY = getWord(ptg_chmngr_wndw.position,1);
				
			if(%wndw == 1)
			{
				ptg_chmngr_wndw.resize(%posX,%posY,300,398);
				PTG_ChMngr_SwBG.resize(4,26,292,368);
				
				PTG_ChMngr_BtnMoreOpts.setVisible(false);
			}
			else
			{
				ptg_chmngr_wndw.resize(%posX,%posY,300,232);
				PTG_ChMngr_SwBG.resize(4,26,292,202);
				
				PTG_ChMngr_BtnMoreOpts.setVisible(true);
			}
		
		//////////////////////////////////////////////////
		//Mass Detail GUI Biome List
		
		case "MassDetailList":
		
			//Hide all biome checkboxes and scroll windows / text lists
			for(%c = 0; %c < 9; %c++)
			{
				PTG_DetLst_SwTxtLstGroup.getObject(%c).visible = false;
				PTG_DetLst_SwChkGroup.getObject(%c).visible = false;
			}
			
			//Unhide checkbox and scroll window / text list if relative to selected biome
			switch$(PTG_DetLst_PopUpBiomeSel.getValue())
			{
				case "Default Biome":
					PTG_DetLst_ScrollWndw_DefBio.visible = true;
					PTG_DetLst_ChkEnab_DefBio.visible = true;
				case "Shore":
					PTG_DetLst_ScrollWndw_ShoreBio.visible = true;
					PTG_DetLst_ChkEnab_ShoreBio.visible = true;
				case "SubMarine":
					PTG_DetLst_ScrollWndw_SubMBio.visible = true;
					PTG_DetLst_ChkEnab_SubMBio.visible = true;
				case "Custom Biome A":
					PTG_DetLst_ScrollWndw_CustABio.visible = true;
					PTG_DetLst_ChkEnab_CustBioA.visible = true;
				case "Custom Biome B":
					PTG_DetLst_ScrollWndw_CustBBio.visible = true;
					PTG_DetLst_ChkEnab_CustBioB.visible = true;
				case "Custom Biome C":
					PTG_DetLst_ScrollWndw_CustCBio.visible = true;
					PTG_DetLst_ChkEnab_CustBioC.visible = true;
				case "Cave Top":
					PTG_DetLst_ScrollWndw_CaveTBio.visible = true;
					PTG_DetLst_ChkEnab_CaveTBio.visible = true;
				case "Cave Bottom":
					PTG_DetLst_ScrollWndw_CaveBBio.visible = true;
					PTG_DetLst_ChkEnab_CaveBBio.visible = true;
				case "Mountains":
					PTG_DetLst_ScrollWndw_MntnBio.visible = true;
					PTG_DetLst_ChkEnab_MntnBio.visible = true;
			}
			
		//////////////////////////////////////////////////
		//Expand Preview Window (complex GUI)
		
		//finish floating island layers... (add secondary layer for flt islds A & B toggle options)
		//take default previews layer size into account if window expanded (when prev reset)(also lag graph layers)
			//update prev funcs and toggle layer funcs as well
		//switch windows (grabbing of window pos) update
		
		case "ToggleSize":
		
			switch$(%wndw)
			{
				case "ExpandA" or "ExpandAaux":
					
					PTG_Cmplx_BtnExpand.command = "PTG_GUI_SwitchWndw(\"ExpandB\",\"ToggleSize\");PTG_Complex.btnResize = true;";
					PTG_Cmplx_BtnExpand.setBitmap("Add-Ons/System_PTG/GUIs/expandicon");
					
					if(%wndw !$= "ExpandAaux")
					{
						//PTG_Cmplx_WndwMain_BtnHelp.resize(434,2,20,20);
						PTG_Cmplx_MainWndw.resize(getWord(PTG_Cmplx_MainWndw.position,0),getWord(PTG_Cmplx_MainWndw.position,1),503,612);
					}
					
					//Build Load Window
					PTG_Cmplx_BmpBuildLAllowance.resize(208,170,287,22);
					PTG_Cmplx_ScrlBuildLoadList.resize(0,194,496,350);
					PTG_Cmplx_BmpBuildLBG.resize(-1,192,496,350);
					PTG_Cmplx_BmpBuildLBG.setBitmap("Add-Ons/System_PTG/GUIs/bg_buildload_medium");
					PTG_Cmplx_TxtListLoadedBuilds.columns = "0 22 207 229 251 273 295 317 339 361 383 405 431";
					PTG_Cmplx_BmpBuildsExpndMsgH.resize(120,358,256,72);
					PTG_Cmplx_BmpBuildsExpndMsgV.resize(508,48,72,256);
					
					PTG_Cmplx_ScrlPlyrSavesList.resize(0,194,496,350);
					PTG_Cmplx_BmpPlyrSavesBG.resize(-1,192,496,350);
					PTG_Cmplx_BtnSortByNum.resize(351,170,96,20);
					PTG_Cmplx_TxtListPlyrSaves.columns = "0 342";
					PTG_Cmplx_BmpSavesExpndMsgH.resize(120,358,256,72);
					PTG_Cmplx_BmpSavesExpndMsgV.resize(508,48,72,256);
					
					//Slider Ticks Adjustment
					PTG_Cmplx_SldrWaterLevel.ticks = 799;
					PTG_Cmplx_SldrSandLevel.ticks = 799;
					//PTG_Cmplx_SldrTerZOffset.ticks = 799;
					PTG_Cmplx_SldrMntnSnowLevel.ticks = 799;
					//PTG_Cmplx_SldrCaveZOffset.ticks = 799;
					PTG_Cmplx_SldrCloudZOffset.ticks = 799;
					PTG_Cmplx_SldrFltIsldsAZOffset.ticks = 799;
					PTG_Cmplx_SldrFltIsldsBZOffset.ticks = 799;
					
				case "ExpandB" or "ExpandBaux":
				
					PTG_Cmplx_BtnExpand.command = "PTG_GUI_SwitchWndw(\"Retract\",\"ToggleSize\");PTG_Complex.btnResize = true;";
					PTG_Cmplx_BtnExpand.setBitmap("Add-Ons/System_PTG/GUIs/retracticon");
					
					if(%wndw !$= "ExpandBaux")
					{
						//PTG_Cmplx_WndwMain_BtnHelp.resize(490,2,20,20);
						PTG_Cmplx_MainWndw.resize(getWord(PTG_Cmplx_MainWndw.position,0),getWord(PTG_Cmplx_MainWndw.position,1),558,680);
					}
					
					//Build Load Window
					PTG_Cmplx_BmpBuildLAllowance.resize(264,170,287,22);
					PTG_Cmplx_ScrlBuildLoadList.resize(0,194,550,416);
					PTG_Cmplx_BmpBuildLBG.resize(-1,192,550,416);
					PTG_Cmplx_BmpBuildLBG.setBitmap("Add-Ons/System_PTG/GUIs/bg_buildload_large");
					PTG_Cmplx_TxtListLoadedBuilds.columns = "0 22 262 284 306 328 350 372 394 416 438 460 486";
					PTG_Cmplx_BmpBuildsExpndMsgH.resize(146,426,256,72);
					PTG_Cmplx_BmpBuildsExpndMsgV.resize(562,82,72,256);
					
					PTG_Cmplx_ScrlPlyrSavesList.resize(0,194,550,418);
					PTG_Cmplx_BmpPlyrSavesBG.resize(-1,192,550,418);
					PTG_Cmplx_BtnSortByNum.resize(406,170,96,20);
					PTG_Cmplx_TxtListPlyrSaves.columns = "0 380";
					PTG_Cmplx_BmpSavesExpndMsgH.resize(146,426,256,72);
					PTG_Cmplx_BmpSavesExpndMsgV.resize(562,82,72,256);
					
					//Slider Ticks Adjustment
					PTG_Cmplx_SldrWaterLevel.ticks = 799;
					PTG_Cmplx_SldrSandLevel.ticks = 799;
					//PTG_Cmplx_SldrTerZOffset.ticks = 799;
					PTG_Cmplx_SldrMntnSnowLevel.ticks = 799;
					//PTG_Cmplx_SldrCaveZOffset.ticks = 799;
					PTG_Cmplx_SldrCloudZOffset.ticks = 799;
					PTG_Cmplx_SldrFltIsldsAZOffset.ticks = 799;
					PTG_Cmplx_SldrFltIsldsBZOffset.ticks = 799;
				
				case "Retract" or "RetractAux":
				
					PTG_Cmplx_BtnExpand.command = "PTG_GUI_SwitchWndw(\"ExpandA\",\"ToggleSize\");PTG_Complex.btnResize = true;";
					PTG_Cmplx_BtnExpand.setBitmap("Add-Ons/System_PTG/GUIs/expandicon");
					
					if(%wndw !$= "RetractAux")
					{
						//PTG_Cmplx_WndwMain_BtnHelp.resize(380,2,20,20);
						PTG_Cmplx_MainWndw.resize(getWord(PTG_Cmplx_MainWndw.position,0),getWord(PTG_Cmplx_MainWndw.position,1),448,544);
					}
					
					//Build Load Window
					PTG_Cmplx_BmpBuildLAllowance.resize(151,170,287,22);
					PTG_Cmplx_ScrlBuildLoadList.resize(0,194,440,280);
					PTG_Cmplx_BmpBuildLBG.resize(-1,192,428,282);
					PTG_Cmplx_BmpBuildLBG.setBitmap("Add-Ons/System_PTG/GUIs/bg_buildload");
					PTG_Cmplx_TxtListLoadedBuilds.columns = "0 22 150 172 194 216 240 260 282 304 326 348 372";
					PTG_Cmplx_BmpBuildsExpndMsgH.resize(90,290,256,72);
					PTG_Cmplx_BmpBuildsExpndMsgV.resize(450,14,72,256);
					
					PTG_Cmplx_ScrlPlyrSavesList.resize(0,194,440,282);
					PTG_Cmplx_BmpPlyrSavesBG.resize(-1,192,428,282);
					PTG_Cmplx_BtnSortByNum.resize(296,170,96,20);
					PTG_Cmplx_TxtListPlyrSaves.columns = "0 296";
					PTG_Cmplx_BmpSavesExpndMsgH.resize(90,290,256,72);
					PTG_Cmplx_BmpSavesExpndMsgV.resize(450,14,72,256);
					
					//Slider Ticks Adjustment
					PTG_Cmplx_SldrWaterLevel.ticks = 399;
					PTG_Cmplx_SldrSandLevel.ticks = 399;
					//PTG_Cmplx_SldrTerZOffset.ticks = 399;
					PTG_Cmplx_SldrMntnSnowLevel.ticks = 399;
					//PTG_Cmplx_SldrCaveZOffset.ticks = 399;
					PTG_Cmplx_SldrCloudZOffset.ticks = 399;
					PTG_Cmplx_SldrFltIsldsAZOffset.ticks = 399;
					PTG_Cmplx_SldrFltIsldsBZOffset.ticks = 399;
					
				case "Update":
				
					//
			}
			
			//Force Sliders To Update
			PTG_Cmplx_SldrChRadP.setValue(PTG_Cmplx_SldrChRadP.getValue());
			PTG_Cmplx_SldrChRadSA.setValue(PTG_Cmplx_SldrChRadSA.getValue());
			PTG_Cmplx_SldrWaterLevel.setValue(PTG_Cmplx_SldrWaterLevel.getValue());
			PTG_Cmplx_SldrSandLevel.setValue(PTG_Cmplx_SldrSandLevel.getValue());
			PTG_Cmplx_SldrTerZOffset.setValue(PTG_Cmplx_SldrTerZOffset.getValue());
			PTG_Cmplx_SldrDetailFreq.setValue(PTG_Cmplx_SldrDetailFreq.getValue());
			PTG_Cmplx_SldrMntnSnowLevel.setValue(PTG_Cmplx_SldrMntnSnowLevel.getValue());
			PTG_Cmplx_SldrMntnZSnapMult.setValue(PTG_Cmplx_SldrMntnZSnapMult.getValue());
			PTG_Cmplx_SldrMntnZMult.setValue(PTG_Cmplx_SldrMntnZMult.getValue());
			PTG_Cmplx_SldrCaveZOffset.setValue(PTG_Cmplx_SldrCaveZOffset.getValue());
			PTG_Cmplx_SldrCloudZOffset.setValue(PTG_Cmplx_SldrCloudZOffset.getValue());
			PTG_Cmplx_SldrFltIsldsAZOffset.setValue(PTG_Cmplx_SldrFltIsldsAZOffset.getValue());
			PTG_Cmplx_SldrFltIsldsBZOffset.setValue(PTG_Cmplx_SldrFltIsldsBZOffset.getValue());
			PTG_Cmplx_SldrBndsHAboveTer.setValue(PTG_Cmplx_SldrBndsHAboveTer.getValue());
			PTG_Cmplx_SldrBndsH.setValue(PTG_Cmplx_SldrBndsH.getValue());
			PTG_Cmplx_SldrTerHMod_CustA.setValue(PTG_Cmplx_SldrTerHMod_CustA.getValue());
			PTG_Cmplx_SldrTerHMod_CustB.setValue(PTG_Cmplx_SldrTerHMod_CustB.getValue());
			PTG_Cmplx_SldrTerHMod_CustC.setValue(PTG_Cmplx_SldrTerHMod_CustC.getValue());
			PTG_Cmplx_SldrFlatAreaFreq.setValue(PTG_Cmplx_SldrFlatAreaFreq.getValue());
			PTG_Cmplx_SldrCaveTopZMult.setValue(PTG_Cmplx_SldrCaveTopZMult.getValue());
			PTG_Cmplx_SldrZMod.setValue(PTG_Cmplx_SldrZMod.getValue());
			PTG_Cmplx_SldrCnctLakesStrt.setValue(PTG_Cmplx_SldrCnctLakesStrt.getValue());
			
			PTG_Routines_SldrStreamsTick.setValue(PTG_Routines_SldrStreamsTick.getValue());
			PTG_Routines_SldrStreamsDist.setValue(PTG_Routines_SldrStreamsDist.getValue());
			PTG_Routines_SldrBrLmtBuffer.setValue(PTG_Routines_SldrBrLmtBuffer.getValue());
			PTG_Routines_SldrPingLmtBuffer.setValue(PTG_Routines_SldrPingLmtBuffer.getValue());
			PTG_Routines_SldrDedSrvrFuncBuffer.setValue(PTG_Routines_SldrDedSrvrFuncBuffer.getValue());
			PTG_Routines_SldrChObjLimitBuffer.setValue(PTG_Routines_SldrChObjLimitBuffer.getValue());
			PTG_Routines_SldrChSavesSeedMax.setValue(PTG_Routines_SldrChSavesSeedMax.getValue());
			PTG_Routines_SldrChSavesTotalMax.setValue(PTG_Routines_SldrChSavesTotalMax.getValue());
			PTG_Routines_SldrBuildsMax.setValue(PTG_Routines_SldrBuildsMax.getValue());
			PTG_Routines_SldrPauseTick.setValue(PTG_Routines_SldrPauseTick.getValue());
			PTG_Routines_SldrAutoSaveDelay.setValue(PTG_Routines_SldrAutoSaveDelay.getValue());
			PTG_Routines_SldrPriGenDelay.setValue(PTG_Routines_SldrPriGenDelay.getValue());
			PTG_Routines_SldrSecGenDelay.setValue(PTG_Routines_SldrSecGenDelay.getValue());
			PTG_Routines_SldrPriCalcDelay.setValue(PTG_Routines_SldrPriCalcDelay.getValue());
			PTG_Routines_SldrSecCalcDelay.setValue(PTG_Routines_SldrSecCalcDelay.getValue());
			PTG_Routines_SldrBrGenDelay.setValue(PTG_Routines_SldrBrGenDelay.getValue());
			PTG_Routines_SldrBrRemDelay.setValue(PTG_Routines_SldrBrRemDelay.getValue());
			PTG_Routines_SldrFontSize.setValue(PTG_Routines_SldrFontSize.getValue());
			
			//Force Text Lists To Update
			PTG_Cmplx_TxtListLoadedBuilds.addrow(PTG_Cmplx_TxtListLoadedBuilds.rowCount(),"temp");
			PTG_Cmplx_TxtListLoadedBuilds.removerow(PTG_Cmplx_TxtListLoadedBuilds.rowCount()-1);
			PTG_Cmplx_TxtListPlyrSaves.addrow(PTG_Cmplx_TxtListPlyrSaves.rowCount(),"temp");
			PTG_Cmplx_TxtListPlyrSaves.removerow(PTG_Cmplx_TxtListPlyrSaves.rowCount()-1);
			PTG_Cmplx_TxtListPresets.addrow(PTG_Cmplx_TxtListPresets.rowCount(),"temp"); //force text list to update width to parent width by updating list itself
			PTG_Cmplx_TxtListPresets.removerow(PTG_Cmplx_TxtListPresets.rowCount()-1);
			
			//Force Preview Window Layers To Update
			//%mainWndwExtY = getWord(PTG_Cmplx_ScrlPrevWndw.extent,1);
			
			//for(%c = 0; %c < 18; %c++)
			//{
			//	%tmpObj = PTG_Cmplx_SwLayerGroup.getObject(%c);
			//	%prevWndwExY = getWord(%tmpObj.extent,1);
			//	%resExtY = %mainWndwExtY - %prevWndwExY;
				
			//	%tmpObj.position = 0 SPC %resExtY;
			//}
			
		//////////////////////////////////////////////////
		
		case "OverviewGUI":
		
			PTG_Overview_ScrlWndwA.setVisible(0);
			PTG_Overview_ScrlWndwB.setVisible(0);
			PTG_Overview_ScrlWndwC.setVisible(0);
			PTG_Overview_ScrlWndwD.setVisible(0);
			PTG_Overview_ScrlWndwE.setVisible(0);
			
			switch$(%wndw)
			{
				case "Page1":
					PTG_Overview_ScrlWndwA.setVisible(1);
				case "Page2":
					PTG_Overview_ScrlWndwB.setVisible(1);
				case "Page3":
					PTG_Overview_ScrlWndwC.setVisible(1);
				case "Page4":
					PTG_Overview_ScrlWndwD.setVisible(1);
				case "Page5":
					PTG_Overview_ScrlWndwE.setVisible(1);
			}
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//// LIST ALL PRINTS FOR SELECTED CATEGORY ////
function PTG_GUI_LoadPrintCat(%cat)
{
	if(%cat $= "Choose Category")
		return;

	PTG_PriSelect_SwPriBtns.clear();
	PTG_PriSelect_SwPriBtns.resize(0,0,284,352); //PTG_PriSelect_SwPriBtns.extent = "284 352";
	%relCol = PTG_PrintSelect.relColID; //getColorIDtable(PTG_PrintSelect.relColID); //!!! PTG_PrintSelect.relColID Setup !!!
	
	//Black or White Print Name Text
	if(getWord(%relCol,1) < 128 || getWord(%relCol,3) < 64)
	{
		%textMod = "<font:impact:16><just:center><color:ffffff>";
		%maxChars = 53;
	}
	else
	{
		%textMod = "<font:impact:16><just:center>";
		%maxChars = 39;
	}
	
	//////////////////////////////////////////////////
	//No Print Selection Button
	
	%priBtn = new GuiBitmapCtrl()
	{
		profile = "GuiDefaultProfile";
		horizSizing = "right";
		vertSizing = "bottom";
		position = "4 4";
		extent = "64 64";
		minExtent = "8 2";
		enabled = "1";
		visible = "1";
		clipToParent = "1";
		bitmap = "Add-Ons/System_PTG/GUIs/deselect.jpg";
		wrap = "0";
		lockAspectRatio = "1";
		alignLeft = "0";
		alignTop = "0";
		overflowImage = "0";
		keepCached = "0";
		mColor = "255 255 255 255";
		mMultiply = "0";

		new GuiMLTextCtrl()
		{
			 profile = "GuiMLTextProfile";
			 horizSizing = "right";
			 vertSizing = "bottom";
			 position = "0 48";
			 extent = "64 16";
			 minExtent = "8 2";
			 enabled = "1";
			 visible = "1";
			 clipToParent = "1";
			 lineSpacing = "2";
			 allowColorChars = "0";
			 maxChars = "53";
			 text = "<font:impact:16><just:center><color:ffffff>NONE";
			 maxBitmapHeight = "-1";
			 selectable = "0";
			 autoResize = "1";

			new GuiSwatchCtrl()
			{
				profile = "GuiDefaultProfile";
				horizSizing = "right";
				vertSizing = "bottom";
				position = "17 2";
				extent = "30 12";
				minExtent = "8 2";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				color = "0 0 0 200";
			};
		};
		new GuiBitmapButtonCtrl()
		{
			profile = "GuiDefaultProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "0 0";
			extent = "64 64";
			minExtent = "8 2";
			enabled = "1";
			visible = "1";
			clipToParent = "1";
			command = "PTG_GUI_BPCDSelect(\"Print\",\"PTGnone\",\"\");";
			text = " ";
			groupNum = "-1";
			buttonType = "PushButton";
			bitmap = "base/client/ui/btnColor";
			lockAspectRatio = "0";
			alignLeft = "0";
			alignTop = "0";
			overflowImage = "0";
			mKeepCached = "0";
			mColor = "0 0 0 255";
		};
	};
	
	PTG_PriSelect_SwPriBtns.add(%priBtn);
	
	//////////////////////////////////////////////////

	%x = 1;
	
	for(%c = 0; %c < $PTG_MaxPrints; %c++)
	{
		if(strStr(%priFP = getField($PTG_PrintArr[%c],1),%cat) != -1)
		{
			%priN = fileBase(%priFP);
			%pos = 4 + (70 * %x) SPC 4 + (70 * %y);
			
			//Adjust scroll window swatch on print overflow
			if((4 + (70 * %y) + 70) > (%SwY = getWord(PTG_PriSelect_SwPriBtns.extent,1)))
				PTG_PriSelect_SwPriBtns.resize(0,0,getWord(PTG_PriSelect_SwPriBtns.extent,0),%SwY + 70);

			//Load Print / Selection Button
			%priBtn = new GuiSwatchCtrl()
			{
				profile = "GuiDefaultProfile";
				horizSizing = "right";
				vertSizing = "bottom";
				position = %pos;
				extent = "64 64";
				minExtent = "8 2";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				color = %relCol;

				new GuiBitmapCtrl()
				{
					profile = "GuiDefaultProfile";
					horizSizing = "right";
					vertSizing = "bottom";
					position = "0 0";
					extent = "64 64";
					minExtent = "8 2";
					enabled = "1";
					visible = "1";
					clipToParent = "1";
					bitmap = strReplace(%priFP,"prints","icons");
					wrap = "0";
					lockAspectRatio = "1";
					alignLeft = "0";
					alignTop = "0";
					overflowImage = "0";
					keepCached = "0";
					mColor = "255 255 255 255";
					mMultiply = "0";

					new GuiMLTextCtrl()
					{
						profile = "GuiMLTextProfile";
						horizSizing = "right";
						vertSizing = "bottom";
						position = "0 48";
						extent = "64 16";
						minExtent = "8 2";
						enabled = "1";
						visible = "1";
						clipToParent = "1";
						lineSpacing = "2";
						allowColorChars = "0";
						maxChars = %maxChars;
						text = %textMod @ %priN;
						maxBitmapHeight = "-1";
						selectable = "0";
						autoResize = "1";
					};
					new GuiBitmapButtonCtrl()
					{
						profile = "GuiDefaultProfile";
						horizSizing = "right";
						vertSizing = "bottom";
						position = "0 0";
						extent = "64 64";
						minExtent = "8 2";
						enabled = "1";
						visible = "1";
						clipToParent = "1";
						command = "PTG_GUI_BPCDSelect(\"Print\",\"" @ %c @ "\",\"\");";
						text = " ";
						groupNum = "-1";
						buttonType = "PushButton";
						bitmap = "base/client/ui/btnColor";
						lockAspectRatio = "0";
						alignLeft = "0";
						alignTop = "0";
						overflowImage = "0";
						mKeepCached = "0";
						mColor = "0 0 0 255";
					};
				};
			};
			
			PTG_PriSelect_SwPriBtns.add(%priBtn);
			
			//Append
			if(%x++ > 3)
			{
				%x = 0;
				%y++;
			}
			%priNum++;
		}
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_ColorPalette(%colX,%rowY,%colID,%colStr)
{
	//Reset GUI when re-requesting colors
	if(%colID == 0) //TEST
	{
		PTG_ColorSelect_SwPalette.clear();
		PTG_ColorSelect.returnObjBr = "";
		PTG_ColorSelect.returnObjPri = "";
		PTG_ColorSelect.returnObjCol = "";
	}
	
	%pos = 4 + (32 * %colX) SPC 4 + (32 * %rowY);
	%colStr = getColorI(%colStr);
	%alpha = getWord(%colStr,3);
	
	if(%colID == 63)
		PTG_ColorSelect.PaletteSetup = true;

	if(%alpha <= 0)
		return;
	
	//Opaque Colors
	if(%alpha == 255)
	{
		%colObj = new GuiSwatchCtrl()
		{
			profile = "GuiDefaultProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = %pos;
			extent = "32 32";
			minExtent = "8 2";
			enabled = "1";
			visible = "1";
			clipToParent = "1";
			color = %colStr;

			new GuiBitmapButtonCtrl()
			{
				profile = "GuiDefaultProfile";
				horizSizing = "right";
				vertSizing = "bottom";
				position = "0 0";
				extent = "32 32";
				minExtent = "8 2";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				command = "PTG_GUI_BPCDSelect(\"Color\",\"" @ %colID @ "\",\"" @ %colStr @ "\");";
				text = " ";
				groupNum = "-1";
				buttonType = "PushButton";
				bitmap = "base/client/ui/btnColor";
				lockAspectRatio = "0";
				alignLeft = "0";
				alignTop = "0";
				overflowImage = "0";
				mKeepCached = "0";
				mColor = "0 0 0 255";
			};
		};
	}
	
	//Transparent Colors
	else
	{
		%colObj = new GuiBitmapCtrl()
		{
			profile = "GuiDefaultProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = %pos;
			extent = "32 32";
			minExtent = "8 2";
			enabled = "1";
			visible = "1";
			clipToParent = "1";
			bitmap = "Add-Ons/System_PTG/GUIs/TransSw";
			wrap = "0";
			lockAspectRatio = "0";
			alignLeft = "0";
			alignTop = "0";
			overflowImage = "0";
			keepCached = "0";
			mColor = "255 255 255 255";
			mMultiply = "0";

			new GuiSwatchCtrl()
			{
				profile = "GuiDefaultProfile";
				horizSizing = "right";
				vertSizing = "bottom";
				position = "0 0";
				extent = "32 32";
				minExtent = "8 2";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				color = %colStr;
			};
			new GuiBitmapButtonCtrl()
			{
				profile = "GuiDefaultProfile";
				horizSizing = "right";
				vertSizing = "bottom";
				position = "0 0";
				extent = "32 32";
				minExtent = "8 2";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				command = "PTG_GUI_BPCDSelect(\"Color\",\"" @ %colID @ "\",\"" @ %colStr @ "\");";
				text = " ";
				groupNum = "-1";
				buttonType = "PushButton";
				bitmap = "base/client/ui/btnColor";
				lockAspectRatio = "0";
				alignLeft = "0";
				alignTop = "0";
				overflowImage = "0";
				mKeepCached = "0";
				mColor = "0 0 0 255";
			};
		};
	}
	
	if(isObject(%colObj))
		PTG_ColorSelect_SwPalette.add(%colObj);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_BPCDSelect(%type,%obj,%aux)
{
	switch$(%type)
	{
		case "Brick":
		
			if(isObject(PTG_MainBrSelect.returnObjBr))
			{
				if(isObject(%obj))
				{
					PTG_MainBrSelect.returnObjBr.setBitmap(%obj.iconName);
					PTG_MainBrSelect.returnObjBr.BrickID = %obj;
					PTG_MainBrSelect.returnObjBr.ModTer = %aux;
				}
				else
					PTG_MainBrSelect.returnObjBr.setBitmap("base/client/ui/brickicons/Unknown");
			}
			
			PTG_MainBrSelect.returnObjBr = "";
			
			Canvas.PopDialog(PTG_MainBrSelect);
		
		//////////////////////////////////////////////////
			
		case "Print":
		
			if(%obj $= "PTGnone")
			{
				PTG_PrintSelect.returnObjPri.setBitmap("Add-Ons/System_PTG/GUIs/NoPrint");
				PTG_PrintSelect.returnObjPri.PrintID = "";
			}
			else if(isObject(PTG_PrintSelect.returnObjPri))
			{
				PTG_PrintSelect.returnObjPri.setBitmap(%str = getField($PTG_PrintArr[%obj],1)); //printTexture and color functions client-sided, server-sided or both?????????????
				PTG_PrintSelect.returnObjPri.PrintID = %obj; //getField($PTG_PrintArr[%obj],0); //keep uiNames in array? Necessary?
			}
			
			PTG_PrintSelect.returnObjPri = "";
			PTG_PrintSelect.relColID = "";
			
			Canvas.PopDialog(PTG_PrintSelect);
			
		//////////////////////////////////////////////////
		
		case "Color":

			PTG_ColorSelect.returnObjCol.colorID = %obj;

			if(isObject(PTG_ColorSelect.returnObjBr))
				PTG_ColorSelect.returnObjBr.mColor = %aux; //.setColor doesn't seem to work correctly with GUIBitmapCtrl, only w/ swatches, so use "mColor =" instead
			if(isObject(PTG_ColorSelect.returnObjPri))
				PTG_ColorSelect.returnObjPri.setColor(%aux);
			if(isObject(PTG_ColorSelect.returnObjCol))
				PTG_ColorSelect.returnObjCol.setColor(%aux);
			
			PTG_ColorSelect.returnObjBr = "";
			PTG_ColorSelect.returnObjPri = "";
			PTG_ColorSelect.returnObjCol = "";
			
			Canvas.PopDialog(PTG_ColorSelect);
			
		//////////////////////////////////////////////////
			
		case "Detail":
		
			if(%obj $= "PTGnone")
			{
				PTG_DetailBrSelect.returnObjBr.setBitmap("base/client/ui/brickicons/Unknown");
				PTG_DetailBrSelect.returnObjBr.BrickID = "";
			}
			else if(isObject(PTG_DetailBrSelect.returnObjBr))
			{
				if(isObject(%obj))
				{
					PTG_DetailBrSelect.returnObjBr.setBitmap(%obj.iconName);
					PTG_DetailBrSelect.returnObjBr.BrickID = %obj;
				}
				else
					PTG_DetailBrSelect.returnObjBr.setBitmap("base/client/ui/brickicons/Unknown");
			}
			
			PTG_DetailBrSelect.returnObjBr = "";
			
			Canvas.PopDialog(PTG_DetailBrSelect);
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_BPCDOpenGui(%gui,%rtnBr,%rtnPri,%rtnCol,%syncClr)
{
	switch$(%gui)
	{
		case "Brick":
		
			Canvas.PushDialog(PTG_MainBrSelect);
			
			PTG_MainBrSelect.returnObjBr = %rtnBr;
			
			//Recolor Normal Brick Bitmap Ctrls
			for(%c = 0; %c < 7; %c++)
			{
				%objList = "PTG_BrSelect_SwPillar PTG_BrSelect_SwCube PTG_BrSelect_SwHalf PTG_BrSelect_SwQuarter PTG_BrSelect_SwEighth PTG_BrSelect_SwNormal PTG_BrSelect_SwPlate";
				%relObj = getWord(%objList,%c);
				%relObjMax = getWord("6 5 4 2 1 6 6",%c);
				
				for(%d = 0; %d < %relObjMax; %d++)
					%relObj.getObject(%d).mColor = %syncClr; //setColor doesn't work correctly
			}
			
			//Recolor ModTer Brick Bitmap Ctrls
			for(%c = 0; %c < 5; %c++)
			{
				%objList = "PTG_BrSelect_SwModTerSteep PTG_BrSelect_SwModTerCube PTG_BrSelect_SwModTerThreeFourth PTG_BrSelect_SwModTerHalf PTG_BrSelect_SwModTerQuarter";
				%relObj = getWord(%objList,%c);
				%relObjMax = getWord("3 4 3 3 3",%c);
				
				for(%d = 0; %d < %relObjMax; %d++)
					%relObj.getObject(%d).mColor = %syncClr; //setColor doesn't work correctly
			}
			
		//////////////////////////////////////////////////
			
		case "Print":
		
			if(!PTG_PrintSelect.rcvdPrintData)
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG ERROR: No Print Data","Print data has not yet been received from the server, which should be sent after you first spawn.");
				return;
			}
			Canvas.PushDialog(PTG_PrintSelect);
			
			PTG_PrintSelect.relColID = %syncClr; //"\"" @ %syncClr @ "\"";
			PTG_GUI_LoadPrintCat(PTG_PriSelect_PopUpPri.getvalue()); //refreshes print colors
			PTG_PrintSelect.returnObjPri = %rtnPri;
			
		//////////////////////////////////////////////////
			
		case "Color":
		
			//if(!PTG_ColorSelect.PaletteSetup)
			//{
			//	CLIENTCMDPTG_ReceiveMsg("Failed","PTG ERROR: No Color Data","Color data has not yet been received from the server, which should be sent after you first spawn.");
			//	return;
			//}
			Canvas.PushDialog(PTG_ColorSelect);
			
			PTG_ColorSelect.returnObjBr = %rtnBr;
			PTG_ColorSelect.returnObjPri = %rtnPri;
			PTG_ColorSelect.returnObjCol = %rtnCol;
			
		//////////////////////////////////////////////////
			
		case "Detail":
		
			Canvas.PushDialog(PTG_DetailBrSelect);
			
			PTG_DetailBrSelect.returnObjBr = %rtnBr;
			
			//Recolor All Brick Bitmap Ctrls
			for(%c = 0; %c < PTG_DetBrSelect_SwMainSubWndw.getCount(); %c++) //Replace .getCount()
			{
				%tmpWndw = PTG_DetBrSelect_SwMainSubWndw.getObject(%c).getObject(0);
				
				for(%d = 0; %d < %tmpWndw.getCount(); %d++)
				{
					%tmpSw = %tmpWndw.getObject(%d);
					
					if(%tmpSw.getClassName() $= "GuiBitmapCtrl" && %tmpSw.getName() !$= "PTGNoneSel")
						%tmpSw.mColor = %syncClr;
				}
			}
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_BuildLoadFuncs(%action)
{
	//.bls file names are no longer kept under 30 chars (since it caused issues w/ previews, etc.) server checks char length during upload

	switch$(%action)
	{
		//Load list of player builds (name with brick count)
		case "LoadPlyrBuilds":
		
			//PTG_Cmplx_WndwSavesPlsWait.visible = true;
			PTG_Cmplx_TxtListPlyrSaves.clear();
			%count = getFileCount(%fp = "saves/*.bls"); //necessary to store %count as a variable, otherwise loop is constantly reset and will always return first file that is found only
			
			if(%count == 0)
			{
				CLIENTCMDPTG_ReceiveMsg("Error","PTG Saves List Error","No brick saves exist!");
				return;
			}
			
			for(%c = 0; %c < %count; %c++)
			{
				%tmpFile = findNextFile(%fp);
				%tmpFileN = fileName(%tmpFile);
				
				%file = new FileObject();
				%file.openForRead(%tmpFile);
				
				%readLn = "";
				while(!%file.isEOF() && firstWord(%readLn) !$= "Linecount")
					%readLn = %file.readLine();
				%brCount = getWord(%readLn,1);
				
				%file.close();
				%file.delete();
				
				if(%brCount >= 20000 || strLen(strReplace(%tmpFileN,".bls","")) >= 30) //current brick limit per save is under 20,000
					%mod = "\c4"; //red color - let user know that this save is too large and can't be uploaded (?)
				else
					%mod = "\c5"; //blue color, 0 = black, others = greyscale
				
				PTG_Cmplx_TxtListPlyrSaves.addRow(%c,%mod @ %tmpFileN TAB %brCount,%c); //echo("\x95");
			}
			
			PTG_Cmplx_TxtListPlyrSaves.sort(0,1); //sort alphabetically
			PTG_Cmplx_WndwSavesPlsWait.visible = false;
		
		//////////////////////////////////////////////////
		
		case "SetBuildPrefs":

			if(PTG_Cmplx_TxtListPlyrSaves.getSelectedID() == -1)
				CLIENTCMDPTG_ReceiveMsg("Error","PTG Build Upload Error","Please choose a brick save below to upload; nothing is currently selected.");
			else
			{
				if(getField(PTG_Cmplx_TxtListPlyrSaves.getValue(),1) >= 20000)
				{
					CLIENTCMDPTG_ReceiveMsg("Error","PTG Build Upload Error","You can only upload saves that contain less than 20,000 bricks.");
					return;
				}
				canvas.pushDialog(PTG_BuildUploadSet);
				
				PTG_BuildUploadSet.buildName = strReplace(stripChars(getField(PTG_Cmplx_TxtListPlyrSaves.getValue(),0),"^"),".bls","");
				PTG_BuildUploadSet.buildNameAdj = getSubStr(PTG_BuildUploadSet.buildName,1,strLen(PTG_BuildUploadSet.buildName)); //getSubStr(PTG_BuildUploadSet.buildName,1,31); //uploaded name (adjusted to < 30 chars) //use getSubStr to remove color codes at beginning of string, and prevent string name errors (i.e. \c4 is treated as one char)
				PTG_BldUpldSet_ChkBldUse.setValue(1); //auto-enabled by default
				PTG_BldUpldSet_SldrBldFreq.setValue(100);
				
				PTG_BldUpldSet_ChkBldAllowTer.setValue(1);
				PTG_BldUpldSet_ChkBldAllowShore.setValue(1);
				PTG_BldUpldSet_ChkBldAllowSubM.setValue(0);
				PTG_BldUpldSet_ChkBldAllowCBioA.setValue(1);
				PTG_BldUpldSet_ChkBldAllowCBioB.setValue(1);
				PTG_BldUpldSet_ChkBldAllowCBioC.setValue(1);
				PTG_BldUpldSet_ChkBldAllowWat.setValue(0);
				PTG_BldUpldSet_ChkBldAllowMntns.setValue(1);
				PTG_BldUpldSet_ChkBldAllowFltIslds.setValue(1);

				PTG_BldUpldSet_ChkBldAutoRot.setValue(1);
				
				PTG_BldUpldSet_BtnBldUpload.setText("Upload To Server");
				PTG_BldUpldSet_BtnBldUpload.mColor = "0 222 0 255";
				PTG_BldUpldSet_BtnBldUpload.command = "PTG_GUI_BuildLoadFuncs(\"UploadBuildFile\");";
			}
			
		//////////////////////////////////////////////////
		
		case "EditBuildPrefs":

			if(PTG_Cmplx_TxtListLoadedBuilds.getSelectedID() == -1)
				CLIENTCMDPTG_ReceiveMsg("Error","PTG Build Edit Error","Please choose a brick save below to edit; nothing is currently selected.");
			else
			{
				canvas.pushDialog(PTG_BuildUploadSet);
				%data = strReplace(PTG_Cmplx_TxtListLoadedBuilds.getValue(),"x",true);
				
				PTG_BuildUploadSet.buildName = stripChars(getField(%data,1),"^");
				PTG_BuildUploadSet.buildNameAdj = PTG_BuildUploadSet.buildName; //getSubStr(PTG_BuildUploadSet.buildName,0,30); //uploaded name (adjusted to < 30 chars) //don't have to remove color chars here
				PTG_BldUpldSet_ChkBldUse.setValue(PTG_Cmplx_TxtListLoadedBuilds.getSelectedID());
				PTG_BldUpldSet_SldrBldFreq.setValue(getField(%data,11));
				
				PTG_BldUpldSet_ChkBldAllowTer.setValue(getField(%data,2));
				PTG_BldUpldSet_ChkBldAllowShore.setValue(getField(%data,3));
				PTG_BldUpldSet_ChkBldAllowSubM.setValue(getField(%data,4));
				PTG_BldUpldSet_ChkBldAllowCBioA.setValue(getField(%data,5));
				PTG_BldUpldSet_ChkBldAllowCBioB.setValue(getField(%data,6));
				PTG_BldUpldSet_ChkBldAllowCBioC.setValue(getField(%data,7));
				PTG_BldUpldSet_ChkBldAllowWat.setValue(getField(%data,8));
				PTG_BldUpldSet_ChkBldAllowMntns.setValue(getField(%data,9));
				PTG_BldUpldSet_ChkBldAllowFltIslds.setValue(getField(%data,10));
				
				if(strStr(getField(%data,1),"^") != -1)
					PTG_BldUpldSet_ChkBldAutoRot.setValue(1);
				else
					PTG_BldUpldSet_ChkBldAutoRot.setValue(0);
				
				PTG_BldUpldSet_BtnBldUpload.setText("Apply Changes");
				PTG_BldUpldSet_BtnBldUpload.mColor = "255 100 0 255";
				PTG_BldUpldSet_BtnBldUpload.command = "PTG_GUI_BuildLoadFuncs(\"ApplyBuildEdit\");";
			}
		
		//////////////////////////////////////////////////
		//Save Previewing
		
		case "PlyrSavePreview" or "SrvrBuildPreview":
		
			if(%action $= "PlyrSavePreview")
			{
				%lstObj = PTG_Cmplx_TxtListPlyrSaves;
				%bmpObj = PTG_Cmplx_BmpPlyrPrev;
				%tmpN = getField(%lstObj.getValue(),0);
				%saveName = getSubStr(%tmpN,1,strLen(%tmpN)); //getSubStr(getField(%lstObj.getValue(),0),1,31);
			}
			else
			{
				%lstObj = PTG_Cmplx_TxtListLoadedBuilds;
				%bmpObj = PTG_Cmplx_BmpBuildPrev;
				%saveName = getField(%lstObj.getValue(),1); //getSubStr(getField(%lstObj.getValue(),1),0,30);
			}
			%saveName = strReplace(stripChars(%saveName,"^"),".bls","");
			%fpJPG = "saves/" @ %saveName @ ".jpg";
			%fpPNG = "saves/" @ %saveName @ ".png";

			if(isFile(%fpJPG))
				%bmpObj.setBitmap(%fpJPG);
			else if(isFile(%fpPNG))
				%bmpObj.setBitmap(%fpPNG);
			else
				%bmpObj.setBitmap("Add-Ons/System_PTG/GUIs/Unknown");
		
		//////////////////////////////////////////////////
		//Client-to-Server
		
		//Request Server Build List ("Reload List" button for Loaded-Builds window takes case of this case)
		
		case "UploadBuildFile" or "ApplyBuildEdit": //when a single file is sent to the file and converted (depending on file brick count)

			%allowTer = PTG_BldUpldSet_ChkBldAllowTer.getValue();
			%allowShore = PTG_BldUpldSet_ChkBldAllowShore.getValue();
			%allowSubM = PTG_BldUpldSet_ChkBldAllowSubM.getValue();
			%allowBioA = PTG_BldUpldSet_ChkBldAllowCBioA.getValue();
			%allowBioB = PTG_BldUpldSet_ChkBldAllowCBioB.getValue();
			%allowBioC = PTG_BldUpldSet_ChkBldAllowCBioC.getValue();
			%allowWat = PTG_BldUpldSet_ChkBldAllowWat.getValue();
			%allowMntn = PTG_BldUpldSet_ChkBldAllowMntns.getValue();
			%allowFltIslds = PTG_BldUpldSet_ChkBldAllowFltIslds.getValue();
			
			%enabCheck = %allowTer + %allowShore + %allowSubM + %allowBioA + %allowBioB + %allowBioC + %allowWat + %allowMntn + %allowFltIslds;
			
			if(%enabCheck == 0)
			{
				CLIENTCMDPTG_ReceiveMsg("Error","PTG Build Edit Error","You have not selected any areas of the landscape to allow this build! If you wish to disable this build, you can do so by deselecting \"Allow this build to be used by server\".");
				return;
			}
			
			%enab = PTG_BldUpldSet_ChkBldUse.getValue();
			%name = PTG_BuildUploadSet.buildNameAdj;
			%rot = PTG_BldUpldSet_ChkBldAutoRot.getValue();
			%freq = PTG_BldUpldSet_SldrBldFreq.getValue(); //clamp ranges when received by server			
			%data = %enab TAB %name TAB %rot TAB %allowTer TAB %allowShore TAB %allowSubM TAB %allowBioA TAB %allowBioB TAB %allowBioC TAB %allowWat TAB %allowMntn TAB %allowFltIslds TAB %freq; //TAB ownerID (found by server)
			
			canvas.popDialog(PTG_BuildUploadSet);
			
			if(%action $= "UploadBuildFile")
				CLIENTCMDPTG_BuildUploadRelay("Stage1-Save",%data);
			else
				commandToServer('PTG_BuildLoadSrvrFuncs',"ApplyBuildEdit",%data);
			
		case "RemoveBuildFile": //when a single save file is removed from the server (depending if they are the owner of that file or the host)
		
			if(PTG_Cmplx_TxtListLoadedBuilds.getSelectedID() == -1)
				CLIENTCMDPTG_ReceiveMsg("Error","PTG Build Removal Error","Please choose a brick save below to toggle enabled / disabled; nothing is currently selected.");
			else
				commandToServer('PTG_BuildLoadSrvrFuncs',"RemoveBuildFile",stripChars(getField(PTG_Cmplx_TxtListLoadedBuilds.getValue(),1),"^"));
		
		case "QuickToggle":
		
			if(PTG_Cmplx_TxtListLoadedBuilds.getSelectedID() == -1)
				CLIENTCMDPTG_ReceiveMsg("Error","PTG Build Toggle Error","Please choose a brick save below to toggle enabled / disabled; nothing is currently selected.");
			else
				commandToServer('PTG_BuildLoadSrvrFuncs',"QuickToggle",stripChars(getField(PTG_Cmplx_TxtListLoadedBuilds.getValue(),1),"^"));
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_SetupDetBrickCat()
{
	//Local Connection
	if((%count = DatablockGroup.getCount()) > 0)
		%group = DatablockGroup;
	
	//Non-Local Connection
	else
	{
		if(isObject(ServerConnection))
		{
			%group = ServerConnection;
			%count = %group.getCount();
		}
		else
		{
			CLIENTCMDPTG_ReceiveMsg("Failed","PTG ERROR: Detail Setup Fail","\"ServerConnection\" group not found, detail brick category and list setup failed.");
			return;
		}
	}
	
	PTG_DetBrSelect_SwMainSubWndw.clear();
	
	//No Brick Selection Button
	%brBtn = new GuiBitmapCtrl("PTGNoneSel")
	{
		profile = "GuiDefaultProfile";
		//horizSizing = "right";
		//vertSizing = "bottom";
		position = "4 4";
		extent = "64 64";
		//minExtent = "8 2";
		//enabled = "1";
		//visible = "1";
		//clipToParent = "1";
		bitmap = "Add-Ons/System_PTG/GUIs/deselect.jpg";
		//wrap = "0";
		//lockAspectRatio = "1";
		//alignLeft = "0";
		//alignTop = "0";
		//overflowImage = "0";
		//keepCached = "0";
		mColor = "255 255 255 255";
		//mMultiply = "0";

		new GuiMLTextCtrl()
		{
			 profile = "GuiMLTextProfile";
			 //horizSizing = "right";
			 //vertSizing = "bottom";
			 position = "0 48";
			 extent = "64 16";
			 //minExtent = "8 2";
			 //enabled = "1";
			 //visible = "1";
			 //clipToParent = "1";
			 //lineSpacing = "2";
			 //allowColorChars = "0";
			 maxChars = "53";
			 text = "<font:impact:16><just:center><color:ffffff>NONE";
			 //maxBitmapHeight = "-1";
			 //selectable = "0";
			 //autoResize = "1";

			new GuiSwatchCtrl()
			{
				profile = "GuiDefaultProfile";
				//horizSizing = "right";
				//vertSizing = "bottom";
				position = "17 2";
				extent = "30 12";
				//minExtent = "8 2";
				//enabled = "1";
				//visible = "1";
				//clipToParent = "1";
				color = "0 0 0 200";
			};
		};
		new GuiBitmapButtonCtrl()
		{
			profile = "GuiDefaultProfile";
			//horizSizing = "right";
			//vertSizing = "bottom";
			position = "0 0";
			extent = "64 64";
			//minExtent = "8 2";
			//enabled = "1";
			//visible = "1";
			//clipToParent = "1";
			command = "PTG_GUI_BPCDSelect(\"Detail\",\"PTGnone\",\"\");";
			text = " ";
			//groupNum = "-1";
			//buttonType = "PushButton";
			bitmap = "base/client/ui/btnColor";
			//lockAspectRatio = "0";
			//alignLeft = "0";
			//alignTop = "0";
			//overflowImage = "0";
			//mKeepCached = "0";
			mColor = "0 0 0 255";
		};
	};
	
	PTG_DetBrSelect_SwMainSubWndw.add(%brBtn);

	PTG_DetBrSelect_PopUpPri.clear();
	PTG_DetBrSelect_PopUpPri.setText("Choose Category");
	
	//////////////////////////////////////////////////
	//Create List of Brick Categories, SubCategories and Bricks Per SubCategory
	
	//list of datablocks sent from server (reduces the amount of objects this script needs to search through)
	if((%maxDB = $PTG_MaxDatablocks) == 0)
		%maxDB = %count;
	
	//Search all brick datablocks for category and subcategory names
	for(%c = 0; %c < %count && %c < %maxDB; %c++)
	{
		%brick = %group.getObject(%c);
		
		if(%brick.getClassName() $= "fxDTSBrickData" && %brick.category !$= "" && %brick.subCategory !$= "")
		{
			//Brick Category List (make sure cat name hasn't already been added to list, otherwise add)
			%tmpBrCat = %brick.category;

			if(strStr(%List_BrCat,%tmpBrCat) == -1)
			{
				%List_BrCat = setField(%List_BrCat,%CountBrCat,%tmpBrCat);
				%CountBrCat++;
			}

			//Brick SubCategory List-Array (make sure subcat name hasn't already been added to list, otherwise add to list along with it's pos in string for array)
			%tmpBrSubCat = %brick.subCategory;

			if((%tmpStrPos = strStr(%ListArr_BrSubCat[%tmpBrCat],%tmpBrSubCat)) == -1 || %tmpStrPos != getWord(%StrPosArr_BrSubCat[%tmpBrCat],%tmpStrPos))
			{
				%ListArr_BrSubCat[%tmpBrCat] = setField(%ListArr_BrSubCat[%tmpBrCat],%CountArr_BrSubCat[%tmpBrCat],%tmpBrSubCat); //subcat name list
				%StrPosArr_BrSubCat[%tmpBrCat] = setWord(%StrPosArr_BrSubCat[%tmpBrCat],strPos(%ListArr_BrSubCat[%tmpBrCat],%tmpBrSubCat),strPos(%ListArr_BrSubCat[%tmpBrCat],%tmpBrSubCat)); //subcat str pos int list
				%CountArr_BrSubCat[%tmpBrCat]++; //subcat count
			}
			
			//Brick Count Per SubCategory
			%brArr_SubCat[%tmpBrCat,%tmpBrSubCat] = setField(%brArr_SubCat[%tmpBrCat,%tmpBrSubCat],%brArr_SubCatCount[%tmpBrCat,%tmpBrSubCat],%brick.getID()); //brickIDs
			%brArr_SubCatCount[%tmpBrCat,%tmpBrSubCat]++; //brickcount for subcat
		}
	}
	
	//////////////////////////////////////////////////
	//Setup Categories List In PopUp Menu (And Create References to New SubWindows For Categories)
	
	for(%c = 0; %c < %CountBrCat; %c++)
	{
		//New SubCategory SubWindow
		%tmp = new GuiScrollCtrl()
		{
			profile = "ImpactScrollProfile";
			//horizSizing = "right";
			//vertSizing = "bottom";
			position = "0 0";
			extent = "434 352";
			//minExtent = "8 2";
			//enabled = "1";
			visible = "0";
			//clipToParent = "1";
			//willFirstRespond = "0";
			hScrollBar = "alwaysOff";
			vScrollBar = "alwaysOn";
			//constantThumbHeight = "0";
			//childMargin = "0 0";
			//rowHeight = "40";
			//columnWidth = "30";
		};
		%tmpSubWndw = new GuiSwatchCtrl()
		{
			profile = "GuiDefaultProfile";
			//horizSizing = "right";
			//vertSizing = "bottom";
			position = "0 0";
			extent = "434 352";
			//minExtent = "8 2";
			//enabled = "1";
			//visible = "1";
			//clipToParent = "1";
			color = "255 255 255 255";
		};
		
		%tmp.add(%tmpSubWndw);
		PTG_DetBrSelect_SwMainSubWndw.add(%tmp);
		
		%tmpBrCat = getField(%List_BrCat,%c);
		%wndwID = %tmp.getID();
		PTG_DetBrSelect_PopUpPri.add(%tmpBrCat,%wndwID); //or setup relative, specific names for subwindows, instead of accessing IDs?
		
		//////////////////////////////////////////////////
		
		//No Brick Selection Button
		%brBtn = new GuiBitmapCtrl("PTGNoneSel")
		{
			profile = "GuiDefaultProfile";
			//horizSizing = "right";
			//vertSizing = "bottom";
			position = "4 4";
			extent = "64 64";
			//minExtent = "8 2";
			//enabled = "1";
			//visible = "1";
			//clipToParent = "1";
			bitmap = "Add-Ons/System_PTG/GUIs/deselect.jpg";
			//wrap = "0";
			//lockAspectRatio = "1";
			//alignLeft = "0";
			//alignTop = "0";
			//overflowImage = "0";
			//keepCached = "0";
			//mColor = "255 255 255 255";
			//mMultiply = "0";

			new GuiMLTextCtrl()
			{
				 profile = "GuiMLTextProfile";
				 //horizSizing = "right";
				 //vertSizing = "bottom";
				 position = "0 48";
				 extent = "64 16";
				 //minExtent = "8 2";
				 //enabled = "1";
				 //visible = "1";
				 //clipToParent = "1";
				 //lineSpacing = "2";
				 //allowColorChars = "0";
				 maxChars = "53";
				 text = "<font:impact:16><just:center><color:ffffff>NONE";
				 //maxBitmapHeight = "-1";
				 selectable = "0";
				 //autoResize = "1";

				new GuiSwatchCtrl()
				{
					profile = "GuiDefaultProfile";
					//horizSizing = "right";
					//vertSizing = "bottom";
					position = "17 2";
					extent = "30 12";
					//minExtent = "8 2";
					//enabled = "1";
					//visible = "1";
					//clipToParent = "1";
					color = "0 0 0 200";
				};
			};
			new GuiBitmapButtonCtrl()
			{
				profile = "GuiDefaultProfile";
				//horizSizing = "right";
				//vertSizing = "bottom";
				position = "0 0";
				extent = "64 64";
				//minExtent = "8 2";
				//enabled = "1";
				//visible = "1";
				//clipToParent = "1";
				command = "PTG_GUI_BPCDSelect(\"Detail\",\"PTGnone\",\"\");";
				text = " ";
				//groupNum = "-1";
				//buttonType = "PushButton";
				bitmap = "base/client/ui/btnColor";
				//lockAspectRatio = "0";
				//alignLeft = "0";
				//alignTop = "0";
				//overflowImage = "0";
				//mKeepCached = "0";
				mColor = "0 0 0 255";
			};
		};
		
		%tmpSubWndw.add(%brBtn);

		//////////////////////////////////////////////////
		//Populate Category SubWindows With Bricks and SubCategories

		%yStart = 76; //8;
		
		//SubCategories (Per Category)
		for(%d = 0; %d < %CountArr_BrSubCat[%tmpBrCat]; %d++)
		{
			%tmpBrSubCat = getField(%ListArr_BrSubCat[%tmpBrCat],%d);
			%posTxt = 4 SPC %yStart;
			
			//SubCategory Title
			%txtObj = new GuiMLTextCtrl()
			{
				profile = "GuiMLTextProfile";
				//horizSizing = "right";
				//vertSizing = "bottom";
				position = %posTxt;
				extent = "414 16";
				//minExtent = "8 2";
				//enabled = "1";
				//visible = "1";
				//clipToParent = "1";
				//lineSpacing = "2";
				//allowColorChars = "0";
				//maxChars = "255";
				text = "<font:impact:16>" @ %tmpBrSubCat;
				//maxBitmapHeight = "-1";
				selectable = "1";
				//autoResize = "1";
			};
			%tmpSubWndw.add(%txtObj);
			
			if(%tmpBrSubCat $= "Sylvanor's Trees")
			{
				//%txtObj.extent = "414 16";
				%txtObj.setText("<font:impact:16>Sylvanor's Trees <color:ff0000>(choosing a leaf brick will plant both the base and leaves)");
			}
			
			%x = 0;
			%y = 0;

			//Bricks (Per SubCategory)
			for(%e = 0; %e < %brArr_SubCatCount[%tmpBrCat,%tmpBrSubCat]; %e++)
			{
				%brickID = getField(%brArr_SubCat[%tmpBrCat,%tmpBrSubCat],%e);
				%pos = 4 + (70 * %x) SPC %yStart + 16 + (70 * %y);
				
				//Brick Selection
				%btnObj = new GuiBitmapCtrl()
				{
					profile = "GuiDefaultProfile";
					//horizSizing = "right";
					//vertSizing = "bottom";
					position = %pos;
					extent = "64 64";
					//minExtent = "8 2";
					//enabled = "1";
					//visible = "1";
					//clipToParent = "1";
					bitmap = %brickID.iconName;
					//wrap = "0";
					//lockAspectRatio = "0";
					//alignLeft = "0";
					//alignTop = "0";
					//overflowImage = "0";
					//keepCached = "0";
					mColor = "255 0 0 255";
					//mMultiply = "0";

					new GuiMLTextCtrl()
					{
						profile = "GuiMLTextProfile";
						//horizSizing = "right";
						//vertSizing = "bottom";
						position = "0 48";
						extent = "64 16";
						//minExtent = "8 2";
						//enabled = "1";
						//visible = "1";
						//clipToParent = "1";
						//lineSpacing = "2";
						//allowColorChars = "0";
						maxChars = "39";
						text = "<font:impact:16><just:center>" @ %brickID.uiName;
						//maxBitmapHeight = "-1";
						//selectable = "0";
						//autoResize = "1";
					};
					new GuiBitmapButtonCtrl()
					{
						profile = "GuiDefaultProfile";
						//horizSizing = "right";
						//vertSizing = "bottom";
						position = "0 0";
						extent = "64 64";
						//minExtent = "8 2";
						//enabled = "1";
						//visible = "1";
						//clipToParent = "1";
						command = "PTG_GUI_BPCDSelect(\"Detail\",\"" @ %brickID @ "\",\"\");";
						text = " ";
						//groupNum = "-1";
						//buttonType = "PushButton";
						bitmap = "base/client/ui/btnColor";
						//lockAspectRatio = "0";
						//alignLeft = "0";
						//alignTop = "0";
						//overflowImage = "0";
						//mKeepCached = "0";
						mColor = "0 0 0 255";
					};
				};
				%tmpSubWndw.add(%btnObj);
				
				if(%x++ > 5)
				{
					%x = 0;
					%y++;
				}
			}
			
			%rowNum = mFloor((%e + 5) / 6);
			
			%yStart = %yStart + 4 + (4 + 16 + (70 * %rowNum));
		}
		
		//Adjust window size on overflow
		if(%yStart > 352)
			%tmpSubWndw.resize(0,0,434,%yStart);
	}
	
	PTG_DetailBrSelect.DetBrListSetup = true;
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//// WHEN ADDING OR SUBTRACTING CHUNK SIZE TO START / END GRID POS IN GUI (USING + / - BUTTONS) ////
function PTG_GUI_StartEndMod(%type,%data)
{
	%chSize = getWord("16 32 64 128 256",mClamp(PTG_Cmplx_PopupChSize.getSelected(),0,4));
	
	switch$(%type)
	{
		case "AppChXStart":
		
			PTG_Cmplx_EdtGridXStart.setValue((mFloor(PTG_Cmplx_EdtGridXStart.getValue() / %chSize) * %chSize) + (%data * %chSize));
			
		case "AppChXEnd":
		
			PTG_Cmplx_EdtGridXEnd.setValue((mFloor(PTG_Cmplx_EdtGridXEnd.getValue() / %chSize) * %chSize) + (%data * %chSize));
			
		case "AppChYStart":
		
			PTG_Cmplx_EdtGridYStart.setValue((mFloor(PTG_Cmplx_EdtGridYStart.getValue() / %chSize) * %chSize) + (%data * %chSize));
			
		case "AppChYEnd":
		
			PTG_Cmplx_EdtGridYEnd.setValue((mFloor(PTG_Cmplx_EdtGridYEnd.getValue() / %chSize) * %chSize) + (%data * %chSize));

	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_ToggleWndwSize()
{
	if(PTG_EnlgImg_Wndw.extent $= "776 542")
	{
		%tmpCnvExt = canvas.getExtent();
		PTG_EnlgImg_Wndw.tmpPos = PTG_EnlgImg_Wndw.getPosition();
		
		PTG_EnlgImg_Wndw.resize(0,0,getWord(%tmpCnvExt,0),getWord(%tmpCnvExt,1));
		PTG_EnlgImg_Sw.resize(4,26,getWord(%tmpCnvExt,0)-8,getWord(%tmpCnvExt,1)-30); //test with diff window sizes for game!
		PTG_EnlgImg_BmpCtrl.resize(4,26,getWord(%tmpCnvExt,0)-8,getWord(%tmpCnvExt,1)-30); //test with diff window sizes for game!
		PTG_EnlgImg_BtnWndwResize.setText("minimize");
	}
	else
	{
		%tmpWndwPos = PTG_EnlgImg_Wndw.tmpPos;
		
		PTG_EnlgImg_Wndw.resize(getWord(%tmpWndwPos,0),getWord(%tmpWndwPos,1),776,542);
		PTG_EnlgImg_Sw.resize(4,26,768,512); //test with diff window sizes for game!
		PTG_EnlgImg_BmpCtrl.resize(4,26,768,512); //test with diff window sizes for game!
		PTG_EnlgImg_BtnWndwResize.setText("Maximize");
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_SaveLoadRoutine(%action)
{
	%fp = "Config/Client/PTGv3/Routines.txt";
	
	switch$(%action)
	{
		case "Save":
		
			%file = new FileObject();
			%file.openForWrite(%fp);
			%file.writeLine(">>Saved settings for the Complex GUI Routines window");
			
				//Chunk Highlight Colors (grab)
				%ChunkHLColA = PTG_Cmplx_SwChunkHLACol.colorID;
				%ChunkHLColB = PTG_Cmplx_SwChunkHLBCol.colorID;
				%file.writeLine(getColorIDTable(%ChunkHLColA));
				%file.writeLine(getColorIDTable(%ChunkHLColB));
				
			//Streams
			%file.writeLine(">Streams");
			%file.writeLine(PTG_Routines_BtnEnabStreams.getValue());
			%file.writeLine(PTG_Routines_SldrStreamsTick.getValue());
			%file.writeLine(PTG_Routines_ChkStreamsHostOnly.getValue());
			%file.writeLine(PTG_Routines_ChkStreamsColl.getValue());
			%file.writeLine(PTG_Routines_ChkStreamsClrDet.getValue());
			%file.writeLine(PTG_Routines_SldrStreamsDist.getValue());
			%file.writeLine(PTG_Routines_ChkGenStreamZones.getValue());
			%file.writeLine(PTG_Routines_ChkFloatStreams.getValue());
			
			//Generator Limits
			%file.writeLine(">Generator Limits");
			%file.writeLine(PTG_Routines_SldrBrLmtBuffer.getValue());
			%file.writeLine(PTG_Routines_SldrPingLmtBuffer.getValue());
			%file.writeLine(PTG_Routines_SldrDedSrvrFuncBuffer.getValue());
			%file.writeLine(PTG_Routines_SldrChObjLimitBuffer.getValue());
			%file.writeLine(PTG_Routines_SldrChSavesSeedMax.getValue());
			%file.writeLine(PTG_Routines_SldrChSavesTotalMax.getValue());
			%file.writeLine(PTG_Routines_SldrBuildsMax.getValue());
			%file.writeLine(PTG_Routines_ChkDisBrBuffer.getValue());
			%file.writeLine(PTG_Routines_ChkDisChBuffer.getValue());
			%file.writeLine(PTG_Routines_ChkNormLagCheck.getValue());
			%file.writeLine(PTG_Routines_ChkDedLagCheck.getValue());
			
			//Generator Schedules
			%file.writeLine(">Generator Schedules");
			%file.writeLine(PTG_Routines_SldrPauseTick.getValue());
			%file.writeLine(PTG_Routines_SldrAutoSaveDelay.getValue());
			%file.writeLine(PTG_Routines_SldrPriGenDelay.getValue());
			%file.writeLine(PTG_Routines_SldrSecGenDelay.getValue());
			%file.writeLine(PTG_Routines_SldrPriCalcDelay.getValue());
			%file.writeLine(PTG_Routines_SldrSecCalcDelay.getValue());
			%file.writeLine(PTG_Routines_SldrBrGenDelay.getValue());
			%file.writeLine(PTG_Routines_SldrBrRemDelay.getValue());
			%file.writeLine(PTG_Routines_PopupGenSpeed.getSelected());
			
			//Chunk Options
			%file.writeLine(">Chunk Options");
			%file.writeLine(PTG_Routines_ChkFrcBrIntoChs.getValue());
				%file.writeLine(PTG_Routines_ChkAutoCreateChunks.getValue());
			%file.writeLine(PTG_Routines_ChkChEditBrPlant.getValue());
			%file.writeLine(PTG_Routines_ChkChEditWrenchData.getValue());
			%file.writeLine(PTG_Routines_ChkChEditBrPPD.getValue());
			%file.writeLine(PTG_Routines_ChkChStcBrSpwnPlnt.getValue());
			%file.writeLine(PTG_Routines_ChkLoadChFileStc.getValue());
			%file.writeLine(PTG_Cmplx_PopUpChSave.getSelected());
			%file.writeLine(PTG_Cmplx_PopUpChSaveExceed.getSelected());
			
			//Misc.
			%file.writeLine(">Miscellaneous");
			%file.writeLine(PTG_Routines_ChkPublicBr.getValue());
				%file.writeLine(PTG_Routines_ChkDstryPublicBr.getValue());
				%file.writeLine(PTG_Routines_ChkPublicBrPBL.getValue());
			%file.writeLine(PTG_Routines_ChkHideGhosting.getValue());
			%file.writeLine(PTG_Routines_ChkEnabEchos.getValue());
			%file.writeLine(PTG_Routines_ChkPrvntDstryDet.getValue());
			%file.writeLine(PTG_Routines_ChkPrvntDstryTer.getValue());
			%file.writeLine(PTG_Routines_ChkPrvntDstryBnds.getValue());
			%file.writeLine(PTG_Routines_ChkNHBuildUpload.getValue());
			%file.writeLine(PTG_Routines_ChkNHSetUpload.getValue());
			%file.writeLine(PTG_Routines_ChkNHSrvrCmdEvntUse.getValue());
			%file.writeLine(PTG_Routines_ChkAllowPlyrPosChk.getValue());
			
			%file.writeLine(PTG_Routines_SldrFontSize.getValue());
			
				//Chunk Highlight Colors (save)
				%file.writeLine(%ChunkHLColA);
				%file.writeLine(%ChunkHLColB);
			
			%file.close();
			%file.delete();
			
			if($PTG_DefaultSetupPass !$= "Final") //server-sided (used cross network on local connections only)
				CLIENTCMDPTG_ReceiveMsg("Success","PTG: Saved Successfully!","Your routine settings have been saved to file, and will automatically be loaded into the GUI for the next game instance.");
			
		//////////////////////////////////////////////////
		
		case "Load":
		
			if(!isFile(%fp))
			{
				PTG_GUI_SaveLoadRoutine("Default");
				return;
			}
			
			%file = new FileObject();
			%file.openForRead(%fp);
			
			if(%file.readLine() $= "")
			{
				%file.close();
				%file.delete();
				
				PTG_GUI_SaveLoadRoutine("Default");
				return;
			}
			
				//Chunk Highlight Colors (find)
				%ChunkHLColA = %file.readLine();
				%ChunkHLColB = %file.readLine();
				
			%file.readLine();
				
			//Streams
			PTG_Routines_BtnEnabStreams.setValue(%file.readLine());
			PTG_Routines_SldrStreamsTick.setValue(mClamp(%file.readLine(),33,2013)); //clamps not really necessary since GUI sliders automatically clamp values to their value range
			PTG_Routines_ChkStreamsHostOnly.setValue(%file.readLine());
			PTG_Routines_ChkStreamsColl.setValue(%file.readLine());
			PTG_Routines_ChkStreamsClrDet.setValue(%file.readLine());
			PTG_Routines_SldrStreamsDist.setValue(mClamp(%file.readLine(),0,8));
			PTG_Routines_ChkGenStreamZones.setValue(%file.readLine());
			PTG_Routines_ChkFloatStreams.setValue(%file.readLine());
			%file.readLine();
			
			//Generator Limits
			PTG_Routines_SldrBrLmtBuffer.setValue(mClamp(%file.readLine(),0,20000));
			PTG_Routines_SldrPingLmtBuffer.setValue(mClamp(%file.readLine(),100,1000));
			PTG_Routines_SldrDedSrvrFuncBuffer.setValue(mClamp(%file.readLine(),20,2000));
			PTG_Routines_SldrChObjLimitBuffer.setValue(mClamp(%file.readLine(),20,2000));
			PTG_Routines_SldrChSavesSeedMax.setValue(mClamp(%file.readLine(),0,100000));
			PTG_Routines_SldrChSavesTotalMax.setValue(mClamp(%file.readLine(),0,100000));
			PTG_Routines_SldrBuildsMax.setValue(mClamp(%file.readLine(),0,400));
			PTG_Routines_ChkDisBrBuffer.setValue(mClamp(%file.readLine(),0,1));
			PTG_Routines_ChkDisChBuffer.setValue(mClamp(%file.readLine(),0,1));
			PTG_Routines_ChkNormLagCheck.setValue(mClamp(%file.readLine(),0,1));
			PTG_Routines_ChkDedLagCheck.setValue(mClamp(%file.readLine(),0,1));
			%file.readLine();
			
			//Generator Schedules
			PTG_Routines_SldrPauseTick.setValue(mClamp(%file.readLine(),1,30));
			PTG_Routines_SldrAutoSaveDelay.setValue(mClamp(%file.readLine(),1,60));
			PTG_Routines_SldrPriGenDelay.setValue(mClamp(%file.readLine(),0,100));
			PTG_Routines_SldrSecGenDelay.setValue(mClamp(%file.readLine(),0,100));
			PTG_Routines_SldrPriCalcDelay.setValue(mClamp(%file.readLine(),0,100));
			PTG_Routines_SldrSecCalcDelay.setValue(mClamp(%file.readLine(),0,100));
			PTG_Routines_SldrBrGenDelay.setValue(mClamp(%file.readLine(),0,50));
			PTG_Routines_SldrBrRemDelay.setValue(mClamp(%file.readLine(),0,50));
			PTG_Routines_PopupGenSpeed.setSelected(mClamp(%file.readLine(),0,2));
			%file.readLine();			
			
			//Chunk Options
			PTG_Routines_ChkFrcBrIntoChs.setValue(%file.readLine());
				PTG_Routines_ChkAutoCreateChunks.setValue(%file.readLine());
				PTG_Routines_ChkPublicBrPBL.setValue(%file.readLine());
			PTG_Routines_ChkChEditBrPlant.setValue(%file.readLine());
			PTG_Routines_ChkChEditWrenchData.setValue(%file.readLine());
			PTG_Routines_ChkChEditBrPPD.setValue(%file.readLine());
			PTG_Routines_ChkChStcBrSpwnPlnt.setValue(%file.readLine());
			PTG_Routines_ChkLoadChFileStc.setValue(%file.readLine());
			PTG_Cmplx_PopUpChSave.setSelected(mClamp(%file.readLine(),0,2));
			PTG_Cmplx_PopUpChSaveExceed.setSelected(mClamp(%file.readLine(),0,1));
			%file.readLine();
			
			//Misc.
			PTG_Routines_ChkPublicBr.setValue(%file.readLine());
				PTG_Routines_ChkDstryPublicBr.setValue(%file.readLine());
			PTG_Routines_ChkHideGhosting.setValue(%bool = %file.readLine());
				if(isObject(HUD_Ghosting))
				{
					if(%bool)
						HUD_Ghosting.mColor = "255 255 255 10";
					else
						HUD_Ghosting.mColor = "255 255 255 255";
				}
			PTG_Routines_ChkEnabEchos.setValue(%file.readLine());
			PTG_Routines_ChkPrvntDstryDet.setValue(%file.readLine());
			PTG_Routines_ChkPrvntDstryTer.setValue(%file.readLine());
			PTG_Routines_ChkPrvntDstryBnds.setValue(%file.readLine());
			PTG_Routines_ChkNHBuildUpload.setValue(%file.readLine());
			PTG_Routines_ChkNHSetUpload.setValue(%file.readLine());
			PTG_Routines_ChkNHSrvrCmdEvntUse.setValue(%file.readLine());
			PTG_Routines_ChkAllowPlyrPosChk.setValue(%file.readLine());
			
			PTG_Routines_SldrFontSize.setValue(mClamp(%file.readLine(),1,30));
			
				//Chunk Highlight Colors (set up)
					//set var if colorsets match to "false"
				PTG_Cmplx_SwChunkHLACol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",false TAB 0 TAB %ChunkHLColA);
					PTG_Cmplx_SwChunkHLACol.setColor(%colStr = getColorI(getColorIDTable(%colID)));
				PTG_Cmplx_SwChunkHLBCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",false TAB 1 TAB %ChunkHLColB);
					PTG_Cmplx_SwChunkHLBCol.setColor(%colStr = getColorI(getColorIDTable(%colID)));

				deleteVariables("$PTG_TmpRefArr_Col*");
			
			%file.close();
			%file.delete();
			
			PTG_Complex.LoadedRtnSave = true;
		
		//////////////////////////////////////////////////
		
		case "Default":

			//Streams
			PTG_Routines_BtnEnabStreams.setValue(1);
			PTG_Routines_SldrStreamsTick.setValue(132);
			PTG_Routines_ChkStreamsHostOnly.setValue(1);
			PTG_Routines_ChkStreamsColl.setValue(0);
			PTG_Routines_ChkStreamsClrDet.setValue(1);
			PTG_Routines_SldrStreamsDist.setValue(2);
			PTG_Routines_ChkGenStreamZones.setValue(1);
			PTG_Routines_ChkFloatStreams.setValue(0);
			
			//Generator Limits
			PTG_Routines_SldrBrLmtBuffer.setValue(10000);
			PTG_Routines_SldrPingLmtBuffer.setValue(550);
			PTG_Routines_SldrDedSrvrFuncBuffer.setValue(800);
			PTG_Routines_SldrChObjLimitBuffer.setValue(800);
			PTG_Routines_SldrChSavesSeedMax.setValue(25000);
			PTG_Routines_SldrChSavesTotalMax.setValue(50000);
			PTG_Routines_SldrBuildsMax.setValue(200);
			PTG_Routines_ChkDisBrBuffer.setValue(0);
			PTG_Routines_ChkDisChBuffer.setValue(0);
			PTG_Routines_ChkNormLagCheck.setValue(0);
			PTG_Routines_ChkDedLagCheck.setValue(1);
			
			//Generator Schedules
			PTG_Routines_SldrPauseTick.setValue(3);
			PTG_Routines_SldrAutoSaveDelay.setValue(5);
			PTG_Routines_SldrPriGenDelay.setValue(0);
			PTG_Routines_SldrSecGenDelay.setValue(0);
			PTG_Routines_SldrPriCalcDelay.setValue(0);
			PTG_Routines_SldrSecCalcDelay.setValue(0);
			PTG_Routines_SldrBrGenDelay.setValue(0);
			PTG_Routines_SldrBrRemDelay.setValue(0);
			PTG_Routines_PopupGenSpeed.setSelected(0);

			//Chunk Options
			PTG_Routines_ChkFrcBrIntoChs.setValue(1);
				PTG_Routines_ChkAutoCreateChunks.setValue(1);
			PTG_Routines_ChkChEditBrPlant.setValue(1);
			PTG_Routines_ChkChEditWrenchData.setValue(1);
			PTG_Routines_ChkChEditBrPPD.setValue(0);
			PTG_Routines_ChkChStcBrSpwnPlnt.setValue(0); //???
			PTG_Routines_ChkLoadChFileStc.setValue(1); //???
			PTG_Cmplx_PopUpChSave.setSelected(0);
			PTG_Cmplx_PopUpChSaveExceed.setSelected(0);
			
			//Misc.
			PTG_Routines_ChkPublicBr.setValue(1); //Public ownership (allows building, prevents using events) (removing bricks overrides public ownership, but not PTG server settings)
				PTG_Routines_ChkDstryPublicBr.setValue(1);
				PTG_Routines_ChkPublicBrPBL.setValue(0);
			PTG_Routines_ChkHideGhosting.setValue(0);
				if(isObject(HUD_Ghosting))
					HUD_Ghosting.mColor = "255 255 255 255";
			PTG_Routines_ChkEnabEchos.setValue(1);
			PTG_Routines_ChkPrvntDstryDet.setValue(0);
			PTG_Routines_ChkPrvntDstryTer.setValue(1);
			PTG_Routines_ChkPrvntDstryBnds.setValue(1);
			PTG_Routines_ChkNHBuildUpload.setValue(0);
			PTG_Routines_ChkNHSetUpload.setValue(0);
			PTG_Routines_ChkNHSrvrCmdEvntUse.setValue(0);
			PTG_Routines_ChkAllowPlyrPosChk.setValue(1);
			
			PTG_Routines_SldrFontSize.setValue(18);
			
				//Chunk Highlight Colors (set up)
					//set var if colorsets match to "false", reference default color strings for red and blue highlight colors
				PTG_Cmplx_SwChunkHLACol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",false TAB (%tmpCol = 0) TAB "1.000000 0.000000 0.000000 1.000000");
					PTG_Cmplx_SwChunkHLACol.setColor(%colStr = getColorI(getColorIDTable(%colID)));
				PTG_Cmplx_SwChunkHLBCol.colorID = %colID = PTG_GUI_GetRelObjID("ColorID",false TAB (%tmpCol = 3) TAB "0.000000 0.000000 1.000000 1.000000");
					PTG_Cmplx_SwChunkHLBCol.setColor(%colStr = getColorI(getColorIDTable(%colID)));

				deleteVariables("$PTG_TmpRefArr_Col*");
		
			if(isFile(%fp))
				fileDelete(%fp);
			
			PTG_Complex.LoadedRtnSave = true;
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_UpdateSecCutVals(%cat)
{
	%zMod = PTG_Cmplx_SldrZMod.getValue();

	switch$(%cat)
	{
		case "Caves":
		
			%res = (%zMod * PTG_Cmplx_EdtNosScaleCaveAZ.getValue()) + (%zMod * PTG_Cmplx_EdtNosScaleCaveBZ.getValue()) + (%zMod * PTG_Cmplx_EdtNosScaleCaveCZ.getValue());
			PTG_Complex_TxtRefSec_Caves.setText("if # is <color:ff0000><<color:000000>                       m of <color:ff0000>" @ mFloor(%res) @ "<color:000000>m");
		
		case "FltIslds":
		
			%res = (%zMod * PTG_Cmplx_EdtNosScaleFltIsldAZ.getValue()) + (%zMod * PTG_Cmplx_EdtNosScaleFltIsldBZ.getValue());
			PTG_Complex_TxtRefSec_FltIslds.setText("if # is <color:ff0000>><color:000000>                       m of <color:ff0000>" @ mFloor(%res) @ "<color:000000>m");
			
		case "CustBioA":
		
			%res = %zMod * PTG_Cmplx_EdtNosScaleCustAZ.getValue();
			PTG_Complex_TxtRefSec_CustA.setText("if # is <color:ff0000><<color:000000>                       m of <color:ff0000>" @ mFloor(%res) @ "<color:000000>m");
			
		case "CustBioB":
		
			%res = %zMod * PTG_Cmplx_EdtNosScaleCustBZ.getValue();
			PTG_Complex_TxtRefSec_CustB.setText("if # is <color:ff0000><<color:000000>                       m of <color:ff0000>" @ mFloor(%res) @ "<color:000000>m");
			
		case "CustBioC":
		
			%res = %zMod * PTG_Cmplx_EdtNosScaleCustCZ.getValue();
			PTG_Complex_TxtRefSec_CustC.setText("if # is <color:ff0000><<color:000000>                       m of <color:ff0000>" @ mFloor(%res) @ "<color:000000>m");
			
		case "Clouds":
		
			%res = (%zMod * PTG_Cmplx_EdtNosScaleCloudAZ.getValue()) + (%zMod * PTG_Cmplx_EdtNosScaleCloudBZ.getValue());
			PTG_Complex_TxtRefSec_Clouds.setText("if # is <color:ff0000>><color:000000>                       m of <color:ff0000>" @ mFloor(%res) @ "<color:000000>m");
			
		case "Skylands":
		
			%res = (%zMod * PTG_Cmplx_EdtNosScaleTerAZ.getValue()) + (%zMod * PTG_Cmplx_EdtNosScaleTerBZ.getValue()) + (%zMod * PTG_Cmplx_EdtNosScaleTerCZ.getValue());
			
			if(PTG_Cmplx_ChkEnabMntns.getValue())
				%res += ((((%zMod * PTG_Cmplx_EdtNosScaleMntnAZ.getValue()) + (%zMod * PTG_Cmplx_EdtNosScaleMntnBZ.getValue())) - PTG_Cmplx_EdtSectMntn.getvalue()) * PTG_Cmplx_SldrMntnZMult.getValue());
			
			PTG_Complex_TxtRefSec_SkyLands.setText("if # is <color:ff0000>><color:000000>                       m of <color:ff0000>" @ mFloor(%res) @ "<color:000000>m");
			
		case "Mountains":
		
			%res = (%zMod * PTG_Cmplx_EdtNosScaleMntnAZ.getValue()) + (%zMod * PTG_Cmplx_EdtNosScaleMntnBZ.getValue());
			PTG_Complex_TxtRefSec_Mntns.setText("if # is <color:ff0000>><color:000000>                       m of <color:ff0000>" @ mFloor(%res) @ "<color:000000>m");
			
		case "All":
		
				%res = (%zMod * PTG_Cmplx_EdtNosScaleCaveAZ.getValue()) + (%zMod * PTG_Cmplx_EdtNosScaleCaveBZ.getValue()) + (%zMod * PTG_Cmplx_EdtNosScaleCaveCZ.getValue());
				PTG_Complex_TxtRefSec_Caves.setText("if # is <color:ff0000><<color:000000>                       m of <color:ff0000>" @ mFloor(%res) @ "<color:000000>m");

				%res = (%zMod * PTG_Cmplx_EdtNosScaleFltIsldAZ.getValue()) + (%zMod * PTG_Cmplx_EdtNosScaleFltIsldBZ.getValue());
				PTG_Complex_TxtRefSec_FltIslds.setText("if # is <color:ff0000>><color:000000>                       m of <color:ff0000>" @ mFloor(%res) @ "<color:000000>m");
				
				%res = %zMod * PTG_Cmplx_EdtNosScaleCustAZ.getValue();
				PTG_Complex_TxtRefSec_CustA.setText("if # is <color:ff0000><<color:000000>                       m of <color:ff0000>" @ mFloor(%res) @ "<color:000000>m");
				
				%res = %zMod * PTG_Cmplx_EdtNosScaleCustBZ.getValue();
				PTG_Complex_TxtRefSec_CustB.setText("if # is <color:ff0000><<color:000000>                       m of <color:ff0000>" @ mFloor(%res) @ "<color:000000>m");
				
				%res = %zMod * PTG_Cmplx_EdtNosScaleCustCZ.getValue();
				PTG_Complex_TxtRefSec_CustC.setText("if # is <color:ff0000><<color:000000>                       m of <color:ff0000>" @ mFloor(%res) @ "<color:000000>m");
				
				%res = (%zMod * PTG_Cmplx_EdtNosScaleCloudAZ.getValue()) + (%zMod * PTG_Cmplx_EdtNosScaleCloudBZ.getValue());
				PTG_Complex_TxtRefSec_Clouds.setText("if # is <color:ff0000>><color:000000>                       m of <color:ff0000>" @ mFloor(%res) @ "<color:000000>m");
				
				%res = (%zMod * PTG_Cmplx_EdtNosScaleTerAZ.getValue()) + (%zMod * PTG_Cmplx_EdtNosScaleTerBZ.getValue()) + (%zMod * PTG_Cmplx_EdtNosScaleTerCZ.getValue());
				
				if(PTG_Cmplx_ChkEnabMntns.getValue())
					%res += ((((%zMod * PTG_Cmplx_EdtNosScaleMntnAZ.getValue()) + (%zMod * PTG_Cmplx_EdtNosScaleMntnBZ.getValue())) - PTG_Cmplx_EdtSectMntn.getvalue()) * PTG_Cmplx_SldrMntnZMult.getValue());
			
				PTG_Complex_TxtRefSec_SkyLands.setText("if # is <color:ff0000>><color:000000>                       m of <color:ff0000>" @ mFloor(%res) @ "<color:000000>m");
				
				%res = (%zMod * PTG_Cmplx_EdtNosScaleMntnAZ.getValue()) + (%zMod * PTG_Cmplx_EdtNosScaleMntnBZ.getValue());
				PTG_Complex_TxtRefSec_Mntns.setText("if # is <color:ff0000>><color:000000>                       m of <color:ff0000>" @ mFloor(%res) @ "<color:000000>m");
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_ClearDetails(%biomeName,%detailCat)
{
	//Biome Selection
	switch$(%biomeName)
	{
		case "Default":
			%bioStart = 0;
			%bioEnd = 1;
		case "Shore":
			%bioStart = 1;
			%bioEnd = 2;
		case "SubMarine":
			%bioStart = 2;
			%bioEnd = 3;
		case "CustomA":
			%bioStart = 3;
			%bioEnd = 4;
		case "CustomB":
			%bioStart = 4;
			%bioEnd = 5;
		case "CustomC":
			%bioStart = 5;
			%bioEnd = 6;
		case "CaveTop":
			%bioStart = 6;
			%bioEnd = 7;
		case "CaveBottom":
			%bioStart = 7;
			%bioEnd = 8;
		case "Mountain":
			%bioStart = 8;
			%bioEnd = 9;
		case "All":
			%bioStart = 0;
			%bioEnd = 9;
	}
	
	//Detail Category Selection
	switch$(%detailCat)
	{
		case "Common":
			%detNumStart = 0;
			%detNumEnd = 6;
		case "Uncommon":
			%detNumStart = 6;
			%detNumEnd = 12;
		case "Rare":
			%detNumStart = 12;
			%detNumEnd = 18;
		case "All":
			%detNumStart = 0;
			%detNumEnd = 18;
	}
	
	//////////////////////////////////////////////////
	
	%DefMCol = getColorI(getColorIDTable(0));
	
	//Relative Biome
	for(%c = %bioStart; %c < %bioEnd; %c++)
	{
		%bioAbr = getWord("Def Shore SubM CustA CustB CustC CaveT CaveB Mntn",%c);
		
		//Detail Number
		for(%detNum = %detNumStart; %detNum < %detNumEnd; %detNum++)
		{
			%DetBrNbmp = "PTG_Cmplx_BmpBio" @ %bioAbr @ "Det" @ %detNum @ "Br";
			%DetPriNbmp = "PTG_Cmplx_BmpBio" @ %bioAbr @ "Det" @ %detNum @ "Pri";
			%DetPriNsw = "PTG_Cmplx_SwBio" @ %bioAbr @ "Det" @ %detNum @ "Pri";
			%DetColNsw = "PTG_Cmplx_SwBio" @ %bioAbr @ "Det" @ %detNum @ "Col";

			//Brick
			%DetBrNbmp.BrickID = "";
				%DetBrNbmp.setBitmap("base/client/ui/brickicons/Unknown");
			
			//Print
			%DetPriNbmp.PrintID = "";
				%DetPriNbmp.setBitmap("Add-Ons/System_PTG/GUIs/noprint");
				
			//Color
			%DetColNsw.colorID = "";
				%DetBrNbmp.mColor = %DefMCol;
				%DetPriNsw.setColor(%DefMCol);
				%DetColNsw.setColor(%DefMCol);
		}
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_NoiseScaleAutoFix()
{
	if(PTG_MsgBox.isAwake())
		canvas.popDialog(PTG_MsgBox);
	
	%ChSize = getWord("16 32 64 128 256",mClamp(PTG_Cmplx_PopupChSize.getSelected(),0,4));
	%tmpBrSize = PTG_Cmplx_BmpTerBr.BrickID.brickSizeX * 0.5;
	
	//Chunk Size
	if(%ChSize <= %tmpBrSize)
		PTG_Cmplx_PopupChSize.setValue(mClamp(%ChSize *= 2,16,256)); //if chunk size range is adjusted in gui, also adjust here and in server upload function
	if(%ChSize <= (PTG_Cmplx_BmpCloudBr.BrickID.brickSizeX * 0.5)) //test
		PTG_Cmplx_PopupChSize.setValue(mClamp(%ChSize *= 2,16,256));
	if(%ChSize <= (PTG_Cmplx_BmpFltIsldsBr.BrickID.brickSizeX * 0.5)) //test
		PTG_Cmplx_PopupChSize.setValue(mClamp(%ChSize *= 2,16,256));
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	//XY-Axis
	
	//Terrain
	%ItrAxy = getSubStr(mFloor(PTG_Cmplx_EdtNosScaleTerAXY.getValue()),0,5);
	%ItrBxy = getSubStr(mFloor(PTG_Cmplx_EdtNosScaleTerBXY.getValue()),0,5);
	%ItrCxy = getSubStr(mFloor(PTG_Cmplx_EdtNosScaleTerCXY.getValue()),0,5);
	
	if((%ItrAxy = mFloatLength(%ItrAxy / %ChSize,0) * %ChSize) < %ChSize)
		%ItrAxy = %ChSize;
	if(%ItrBxy > %ItrAxy)
		%ItrBxy = %ItrAxy;
	if((%ItrBxy = mFloatLength(%ItrBxy / %ChSize,0) * %ChSize) < %ChSize)
		%ItrBxy = %ChSize;
	if(%ItrCxy > %ItrBxy)
		%ItrCxy = %ItrBxy;
	if(%ItrCxy > %ChSize)
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

	PTG_Cmplx_EdtNosScaleTerAXY.setValue(mClamp(%ItrAxy,0,16384));
	PTG_Cmplx_EdtNosScaleTerBXY.setValue(mClamp(%ItrBxy,0,16384));
	PTG_Cmplx_EdtNosScaleTerCXY.setValue(mClamp(%ItrCxy,0,16384));
	
	//////////////////////////////////////////////////
	//Mountains
	
	%ItrAxy = getSubStr(mFloor(PTG_Cmplx_EdtNosScaleMntnAXY.getValue()),0,5);
	%ItrBxy = getSubStr(mFloor(PTG_Cmplx_EdtNosScaleMntnBXY.getValue()),0,5);
	
	if((%ItrAxy = mFloatLength(%ItrAxy / %ChSize,0) * %ChSize) < %ChSize)
		%ItrAxy = %ChSize;
	if(%ItrBxy > %ItrAxy)
		%ItrBxy = %ItrAxy;
	if((%ItrBxy = mFloatLength(%ItrBxy / %ChSize,0) * %ChSize) < %ChSize)
		%ItrBxy = %ChSize;

	PTG_Cmplx_EdtNosScaleMntnAXY.setValue(mClamp(%ItrAxy,0,16384));
	PTG_Cmplx_EdtNosScaleMntnBXY.setValue(mClamp(%ItrBxy,0,16384));
	
	//////////////////////////////////////////////////
	//Caves
	
	%ItrAxy = getSubStr(mFloor(PTG_Cmplx_EdtNosScaleCaveAXY.getValue()),0,5);
	%ItrBxy = getSubStr(mFloor(PTG_Cmplx_EdtNosScaleCaveBXY.getValue()),0,5);
	%ItrCxy = getSubStr(mFloor(PTG_Cmplx_EdtNosScaleCaveCXY.getValue()),0,5);
	
	if((%ItrAxy = mFloatLength(%ItrAxy / %ChSize,0) * %ChSize) < %ChSize)
		%ItrAxy = %ChSize;
	if(%ItrBxy > %ItrAxy)
		%ItrBxy = %ItrAxy;
	if((%ItrBxy = mFloatLength(%ItrBxy / %ChSize,0) * %ChSize) < %ChSize)
		%ItrBxy = %ChSize;
	if(%ItrCxy > %ItrBxy)
		%ItrCxy = %ItrBxy;
	if(%ItrCxy > %ChSize)
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

	PTG_Cmplx_EdtNosScaleCaveAXY.setValue(mClamp(%ItrAxy,0,16384));
	PTG_Cmplx_EdtNosScaleCaveBXY.setValue(mClamp(%ItrBxy,0,16384));
	PTG_Cmplx_EdtNosScaleCaveCXY.setValue(mClamp(%ItrCxy,0,16384));
	
	//////////////////////////////////////////////////
	//Cave Height Mod
	
	%ItrAxy = getSubStr(mFloor(PTG_Cmplx_EdtNosScaleCaveHXY.getValue()),0,5);

	if((%ItrAxy = mFloatLength(%ItrAxy / %ChSize,0) * %ChSize) < %ChSize)
		%ItrAxy = %ChSize;

	PTG_Cmplx_EdtNosScaleCaveHXY.setValue(mClamp(%ItrAxy,0,16384));
	
	//////////////////////////////////////////////////
	//Biomes
	
	%tmpItr = PTG_Cmplx_EdtNosScaleTerAXY.getValue();
	
	//Custom Biome A
	%ItrAxy = getSubStr(mFloor(PTG_Cmplx_EdtNosScaleCustAXY.getValue()),0,5);

	if((%ItrAxy = mFloatLength(%ItrAxy / %ChSize,0) * %ChSize) < %ChSize)
		%ItrAxy = %ChSize;
	if(%ItrAxy < %tmpItr) //make sure biome scale is >= terrain scale (ter scale should already be adjusted to chunk size above)
		%ItrAxy = %tmpItr;

	PTG_Cmplx_EdtNosScaleCustAXY.setValue(mClamp(%ItrAxy,0,16384));
	
	//Custom Biome B
	%ItrAxy = getSubStr(mFloor(PTG_Cmplx_EdtNosScaleCustBXY.getValue()),0,5);

	if((%ItrAxy = mFloatLength(%ItrAxy / %ChSize,0) * %ChSize) < %ChSize)
		%ItrAxy = %ChSize;
	if(%ItrAxy < %tmpItr) //make sure biome scale is >= terrain scale (ter scale should already be adjusted to chunk size above)
		%ItrAxy = %tmpItr;

	PTG_Cmplx_EdtNosScaleCustBXY.setValue(mClamp(%ItrAxy,0,16384));
	
	//Custom Biome C
	%ItrAxy = getSubStr(mFloor(PTG_Cmplx_EdtNosScaleCustCXY.getValue()),0,5);

	if((%ItrAxy = mFloatLength(%ItrAxy / %ChSize,0) * %ChSize) < %ChSize)
		%ItrAxy = %ChSize;
	if(%ItrAxy < %tmpItr) //make sure biome scale is >= terrain scale (ter scale should already be adjusted to chunk size above)
		%ItrAxy = %tmpItr;

	PTG_Cmplx_EdtNosScaleCustCXY.setValue(mClamp(%ItrAxy,0,16384));
	
	//////////////////////////////////////////////////
	//Skylands
	
	%ItrAxy = getSubStr(mFloor(PTG_Cmplx_EdtNosScaleSkylandXY.getValue()),0,5);

	if((%ItrAxy = mFloatLength(%ItrAxy / %ChSize,0) * %ChSize) < %ChSize)
		%ItrAxy = %ChSize;

	PTG_Cmplx_EdtNosScaleSkylandXY.setValue(mClamp(%ItrAxy,0,16384));
	
	//////////////////////////////////////////////////
	//Clouds
	
	%ItrAxy = getSubStr(mFloor(PTG_Cmplx_EdtNosScaleCloudAXY.getValue()),0,5);
	%ItrBxy = getSubStr(mFloor(PTG_Cmplx_EdtNosScaleCloudBXY.getValue()),0,5);
	
	if((%ItrAxy = mFloatLength(%ItrAxy / %ChSize,0) * %ChSize) < %ChSize)
		%ItrAxy = %ChSize;
	if(%ItrBxy > %ItrAxy)
		%ItrBxy = %ItrAxy;
	if((%ItrBxy = mFloatLength(%ItrBxy / %ChSize,0) * %ChSize) < %ChSize)
		%ItrBxy = %ChSize;

	PTG_Cmplx_EdtNosScaleCloudAXY.setValue(mClamp(%ItrAxy,0,16384));
	PTG_Cmplx_EdtNosScaleCloudBXY.setValue(mClamp(%ItrBxy,0,16384));
	
	//////////////////////////////////////////////////
	//Floating Islands
	
	%ItrAxy = getSubStr(mFloor(PTG_Cmplx_EdtNosScaleFltIsldAXY.getValue()),0,5);
	%ItrBxy = getSubStr(mFloor(PTG_Cmplx_EdtNosScaleFltIsldBXY.getValue()),0,5);
	
	if((%ItrAxy = mFloatLength(%ItrAxy / %ChSize,0) * %ChSize) < %ChSize)
		%ItrAxy = %ChSize;
	if(%ItrBxy > %ItrAxy)
		%ItrBxy = %ItrAxy;
	if((%ItrBxy = mFloatLength(%ItrBxy / %ChSize,0) * %ChSize) < %ChSize)
		%ItrBxy = %ChSize;

	PTG_Cmplx_EdtNosScaleFltIsldAXY.setValue(mClamp(%ItrAxy,0,16384));
	PTG_Cmplx_EdtNosScaleFltIsldBXY.setValue(mClamp(%ItrBxy,0,16384));
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	//Z-Axis
	
	PTG_Cmplx_EdtNosScaleTerAZ.setValue(mClampF(mAbs(PTG_Cmplx_EdtNosScaleTerAZ.getValue()),0,8));
	PTG_Cmplx_EdtNosScaleTerBZ.setValue(mClampF(mAbs(PTG_Cmplx_EdtNosScaleTerBZ.getValue()),0,8));
	PTG_Cmplx_EdtNosScaleTerCZ.setValue(mClampF(mAbs(PTG_Cmplx_EdtNosScaleTerCZ.getValue()),0,8));
	PTG_Cmplx_EdtNosScaleMntnAZ.setValue(mClampF(mAbs(PTG_Cmplx_EdtNosScaleMntnAZ.getValue()),0,8));
	PTG_Cmplx_EdtNosScaleMntnBZ.setValue(mClampF(mAbs(PTG_Cmplx_EdtNosScaleMntnBZ.getValue()),0,8));
	PTG_Cmplx_EdtNosScaleCaveAZ.setValue(mClampF(mAbs(PTG_Cmplx_EdtNosScaleCaveAZ.getValue()),0,8));
	PTG_Cmplx_EdtNosScaleCaveBZ.setValue(mClampF(mAbs(PTG_Cmplx_EdtNosScaleCaveBZ.getValue()),0,8));
	PTG_Cmplx_EdtNosScaleCaveCZ.setValue(mClampF(mAbs(PTG_Cmplx_EdtNosScaleCaveCZ.getValue()),0,8));
	PTG_Cmplx_EdtNosScaleCaveHZ.setValue(mClampF(mAbs(PTG_Cmplx_EdtNosScaleCaveHZ.getValue()),0,8));
	PTG_Cmplx_EdtNosScaleCustAZ.setValue(mClampF(mAbs(PTG_Cmplx_EdtNosScaleCustAZ.getValue()),0,8));
	PTG_Cmplx_EdtNosScaleCustBZ.setValue(mClampF(mAbs(PTG_Cmplx_EdtNosScaleCustBZ.getValue()),0,8));
	PTG_Cmplx_EdtNosScaleCustCZ.setValue(mClampF(mAbs(PTG_Cmplx_EdtNosScaleCustCZ.getValue()),0,8));
	PTG_Cmplx_EdtNosScaleSkylandZ.setValue(mClampF(mAbs(PTG_Cmplx_EdtNosScaleSkylandZ.getValue()),0,8));
	PTG_Cmplx_EdtNosScaleCloudAZ.setValue(mClampF(mAbs(PTG_Cmplx_EdtNosScaleCloudAZ.getValue()),0,8));
	PTG_Cmplx_EdtNosScaleCloudBZ.setValue(mClampF(mAbs(PTG_Cmplx_EdtNosScaleCloudBZ.getValue()),0,8));
	PTG_Cmplx_EdtNosScaleFltIsldAZ.setValue(mClampF(mAbs(PTG_Cmplx_EdtNosScaleFltIsldAZ.getValue()),0,8));
	PTG_Cmplx_EdtNosScaleFltIsldBZ.setValue(mClampF(mAbs(PTG_Cmplx_EdtNosScaleFltIsldBZ.getValue()),0,8));


	CLIENTCMDPTG_ReceiveMsg("Success","PTG: Noise Scales Fixed","Any issues with noise scale values should now be fixed!");
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_MassDetFuncs(%type)
{
	switch$(PTG_DetLst_PopUpBiomeSel.getValue())
	{
		case "Default Biome":
			%relTextList = PTG_DetLst_TxtLst_DefBio;
		case "Shore":
			%relTextList = PTG_DetLst_TxtLst_ShoreBio;
		case "SubMarine":
			%relTextList = PTG_DetLst_TxtLst_SubMBio;
		case "Custom Biome A":
			%relTextList = PTG_DetLst_TxtLst_CustBioA;
		case "Custom Biome B":
			%relTextList = PTG_DetLst_TxtLst_CustBioB;
		case "Custom Biome C":
			%relTextList = PTG_DetLst_TxtLst_CustBioC;
		case "Cave Top":
			%relTextList = PTG_DetLst_TxtLst_CaveTBio;
		case "Cave Bottom":
			%relTextList = PTG_DetLst_TxtLst_CaveBBio;
		case "Mountains":
			%relTextList = PTG_DetLst_TxtLst_MntnBio;
	}
	
	//////////////////////////////////////////////////
	
	switch$(%type)
	{
		case "Remove":
		
			if((%tmpSID = %relTextList.getSelectedID()) == -1)
			{
				CLIENTCMDPTG_ReceiveMsg("Error","PTG: No Row Selected","No row in list was selected for removal.");
				return;
			}
			
			//"%relTextList.removeRowByID(%tmpSID);" //causes problems with updating detail count number
			
			//Update Detail Number and Selection ID for All Details In List
			%count = %relTextList.rowCount();
			for(%c = 0; %c < %count; %c++)
			{
				if(%c != %tmpSID)
				{
					%relRowText = %relTextList.getRowTextByID(%c);
					
					//Carry over color code, if applicable
					if(!getField(%relRowText,8))
						%colorMod = "\c4";
					else
						%colorMod = "";
					
					%tmpDetArr[%c - %cMod] = %colorMod @ (%c - %cMod)+1 TAB getFields(%relRowText,1,8);
				}
				else
					%cMod = 1;
			}
			%relTextList.clear();
			
			for(%c = 0; %c < %count - %cMod; %c++)
				%relTextList.setRowByID(%c,%tmpDetArr[%c]); //%c+1 TAB 
			
			%relTextList.scrollVisible(%tmpSID);
		
		//////////////////////////////////////////////////
			
		case "Add":
		
			%brID = PTG_DetLst_BmpMassDetBr.BrickID;
			
			if(%relTextList.rowCount() < 400)
			{
				if(%brID !$= "")
				{
					%priID = PTG_DetLst_BmpMassDetPri.PrintID;
					%colID = PTG_DetLst_SwMassDetCol.colorID * 1; //"* 1" ensures a blank character isn't used instead of "0"
					%brData = %brID.uiName TAB %brID.category TAB %brID.subCategory TAB %colID TAB getField($PTG_PrintArr[%priID],0) TAB %brID TAB %priID;
					
					%rCount = %relTextList.rowCount();
					%relTextList.addRow(%rCount,%rCount+1 TAB %brData TAB true,%rCount);
				}
				else
					CLIENTCMDPTG_ReceiveMsg("Error","PTG: No Brick Selected","No detail brick was selected to add; make sure you specify the brick, print and color first.");
			}
			else
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Hard Limit Reached","Limit of 400 details for this biome has been reached; failed to add new brick to list.");
		
		//////////////////////////////////////////////////
		
		case "Clear":
		
			%relTextList.clear();
			
		//////////////////////////////////////////////////
			
		case "ClearAll":
		
			PTG_DetLst_TxtLst_DefBio.clear();
			PTG_DetLst_TxtLst_ShoreBio.clear();
			PTG_DetLst_TxtLst_SubMBio.clear();
			PTG_DetLst_TxtLst_CustBioA.clear();
			PTG_DetLst_TxtLst_CustBioB.clear();
			PTG_DetLst_TxtLst_CustBioC.clear();
			PTG_DetLst_TxtLst_CaveTBio.clear();
			PTG_DetLst_TxtLst_CaveBBio.clear();
			PTG_DetLst_TxtLst_MntnBio.clear();
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_SimplexUpload()
{
	//on upload, find closest colors on the server-side
		//prevent edge falloff if infinite terrain enabled
		//Snap values on server-side
	//Setup default GUI settings on exec
	//Preset brick selection colors on exec?
	//Set default ModTer types and prints
	//Check on server-side if bricks exist(!!!)
	
			//Main Options
	%data = PTG_Smplx_EdtSeed.getValue() SPC
	
			PTG_Smplx_BmpTerBr.BrickID SPC 
			PTG_Smplx_BmpTerBr.ModTer SPC 
			PTG_Smplx_BmpTerBr.ColorID SPC 
			
			PTG_Smplx_BmpCloudBr.BrickID SPC 
			PTG_Smplx_BmpCloudBr.ModTer SPC 
			PTG_Smplx_BmpCloudBr.ColorID SPC 
			
			PTG_Smplx_BmpFltIsldsBr.BrickID SPC 
			PTG_Smplx_BmpFltIsldsBr.ModTer SPC 
			PTG_Smplx_BmpFltIsldsBr.ColorID SPC 
			
			PTG_Smplx_ChkInfTer.getValue() SPC
			PTG_Smplx_ChkEdgeFallOff.getValue() SPC
			PTG_Smplx_SldrTerLengthY.getValue() SPC
			PTG_Smplx_SldrTerWidthX.getValue() SPC
			PTG_Smplx_PopUpTerType.getSelected() SPC
			
			//Additional Features
			PTG_Smplx_ChkEnabMntns.getValue() SPC
			PTG_Smplx_ChkEnabCaves.getValue() SPC
			PTG_Smplx_ChkEnabLakes.getValue() SPC
			PTG_Smplx_ChkEnabClouds.getValue() SPC
			PTG_Smplx_ChkEnabFltIslds.getValue() SPC
			PTG_Smplx_ChkEnabBounds.getValue() SPC
			
			//Optional Biomes
			PTG_Smplx_PopUpBioDef.getSelected() SPC
			PTG_Smplx_PopUpBioCustA.getSelected() SPC
			PTG_Smplx_PopUpBioCustB.getSelected() SPC
			PTG_Smplx_PopUpBioCustC.getSelected();
			
	if(strLen(%data) > 255)
	{
		CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Action Failed","Data string for Simplex GUI is too large, and can't be sent over the network successfully.");
		return;
	}
			
	commandToServer('PTGSimplexStart',%data);
}

