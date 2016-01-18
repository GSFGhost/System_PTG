//exec("Add-Ons/System_PTG/Client.cs");

//Scripts
exec("Add-Ons/System_PTG/SCRIPTS/Client/GUIs.cs");
exec("Add-Ons/System_PTG/SCRIPTS/Client/GUIs_Support.cs");
exec("Add-Ons/System_PTG/SCRIPTS/Client/GUI_PresetFuncs.cs");
exec("Add-Ons/System_PTG/SCRIPTS/Client/GUI_PreviewFuncs.cs");
exec("Add-Ons/System_PTG/SCRIPTS/Client/GUI_PreviewFuncs_Support.cs");
exec("Add-Ons/System_PTG/SCRIPTS/Client/GUI_HelpFuncs.cs");
exec("Add-Ons/System_PTG/SCRIPTS/Client/GUI_OverviewFuncs.cs");
exec("Add-Ons/System_PTG/SCRIPTS/Client/UpdaterDL.cs");

//Fundamental GUIs
exec("Add-Ons/System_PTG/GUIs/PTG_ChunkManager.gui");
exec("Add-Ons/System_PTG/GUIs/PTG_Complex.gui");
exec("Add-Ons/System_PTG/GUIs/PTG_Simplex.gui");
exec("Add-Ons/System_PTG/GUIs/PTG_Overview.gui");

//Secondary / Selection and Specification GUIs
exec("Add-Ons/System_PTG/GUIs/PTG_MainBrSelect.gui");
exec("Add-Ons/System_PTG/GUIs/PTG_PrintSelect.gui");
exec("Add-Ons/System_PTG/GUIs/PTG_ColorSelect.gui");
exec("Add-Ons/System_PTG/GUIs/PTG_DetailBrSelect.gui");
exec("Add-Ons/System_PTG/GUIs/PTG_DetailList.gui");
exec("Add-Ons/System_PTG/GUIs/PTG_BuildUploadSet.gui");

//Tertiary / Notification GUIs
exec("Add-Ons/System_PTG/GUIs/PTG_MsgBox.gui");
exec("Add-Ons/System_PTG/GUIs/PTG_Confirm.gui");
exec("Add-Ons/System_PTG/GUIs/PTG_EnlargeImg.gui");
exec("Add-Ons/System_PTG/GUIs/PTG_ErrorReport.gui");
exec("Add-Ons/System_PTG/GUIs/PTG_Help.gui");

//Auto-create presets file path (to avoid confusion when installing presets if file path hasn't been created yet)
createPath("Config/Client/PTGv3/Presets/");


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function CLIENTCMDPTG_GUI_OpenGUI(%gui,%Enab_BasicPack,%Enab_4xPack)
{
	switch$(%gui)
	{
		case "SimplexGUI":
			Canvas.PushDialog(PTG_Simplex);
		case "ComplexGUI":
			Canvas.PushDialog(PTG_Complex);
	}
	
	//If ModTer Packs Are Enabled
	if(%Enab_BasicPack)
		PTG_BrSelect_BmpModTerBasicDis.setVisible(false);
	else
		PTG_BrSelect_BmpModTerBasicDis.setVisible(true);
	switch$(%Enab_4xPack)
	{
		case "Disabled":
			PTG_BrSelect_BmpModTer4xDis.setVisible(true);
			PTG_BrSelect_BmpModTer4xDis.setBitMap("Add-Ons/System_PTG/GUIs/Disabled_ModTer4x");
		case "NotFound":
			PTG_BrSelect_BmpModTer4xDis.setVisible(true);
			PTG_BrSelect_BmpModTer4xDis.setBitMap("Add-Ons/System_PTG/GUIs/NotFound_ModTer4x");
		case "Enabled":
			PTG_BrSelect_BmpModTer4xDis.setVisible(false);
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function CLIENTCMDPTG_RequestClVer(%secKey)
{
	commandToServer('PTG_ReceiveClVer',3,%secKey);
	$PTG_SrvHasPTGv3 = true;
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function CLIENTCMDPTG_ReceiveGUIData(%type,%data)
{
	%chSize = getWord("16 32 64 128 256",mClamp(PTG_Cmplx_PopupChSize.getSelected(),0,4));
	
	switch$(%type)
	{
		case "PosAsStart":

			PTG_Cmplx_EdtGridXStart.setValue(mFloor(getWord(%data,0) / %ChSize) * %ChSize);
			PTG_Cmplx_EdtGridYStart.setValue(mFloor(getWord(%data,1) / %ChSize) * %ChSize);
			
			if(PTG_Cmplx_EdtGridXEnd.getValue() <= (%xsPos = PTG_Cmplx_EdtGridXStart.getValue()))
				PTG_Cmplx_EdtGridXEnd.setValue((mFloor(%xsPos / %ChSize) * %ChSize) + %ChSize);
			if(PTG_Cmplx_EdtGridYEnd.getValue() <= (%ysPos = PTG_Cmplx_EdtGridYStart.getValue()))
				PTG_Cmplx_EdtGridYEnd.setValue((mFloor(%ysPos / %ChSize) * %ChSize) + %ChSize);
		
		//////////////////////////////////////////////////
			
		case "PosAsCenter":

			%offX = mAbs(PTG_Cmplx_EdtGridXEnd.getValue() - PTG_Cmplx_EdtGridXStart.getValue());
			%offX = getMax((mFloor((%offX / 2) / %ChSize) * %ChSize),%ChSize);
			%offY = mAbs(PTG_Cmplx_EdtGridYEnd.getValue() - PTG_Cmplx_EdtGridYStart.getValue());
			%offY = getMax((mFloor(%offY / 2) / %ChSize) * %ChSize,%ChSize);
			
			%posX = mFloor(getWord(%data,0) / %ChSize) * %ChSize;
			%posY = mFloor(getWord(%data,1) / %ChSize) * %ChSize;
			
			PTG_Cmplx_EdtGridXStart.setValue(%posX - %offX);
			PTG_Cmplx_EdtGridXEnd.setValue(%posX + %offX);
			PTG_Cmplx_EdtGridYStart.setValue(%posY - %offY);
			PTG_Cmplx_EdtGridYEnd.setValue(%posY + %offY);
			
		//////////////////////////////////////////////////
			
		case "DatablockCount":
		
			$PTG_MaxDatablocks = %data;

		//////////////////////////////////////////////////
		
		case "ListServerBuilds":
		
			%rowCount = PTG_Cmplx_TxtListLoadedBuilds.rowCount() + 1;
			%enabled = getField(%data,0);
			%buildName = stripChars(getSubStr(getField(%data,1),0,30),"^");
			%rot = getField(%data,2);
			%allow = getFields(%data,3,11);
			%allow = strReplace(strReplace(%allow,"1","  x"),"0","  ");
			%freq = mClamp(getField(%data,12),0,100); //otherwise may replace boolean values meant for %allow
			%owner = getField(%data,13);

			if(%enabled)
				%colVal = "\c5";
			else
				%colVal = "\c4";
			if(%rot) 
				%buildName = %buildName @ "^";
			
			PTG_Cmplx_TxtListLoadedBuilds.addRow(%enabled,%colVal @ %rowCount TAB %buildName TAB %allow TAB %freq TAB %owner); //PTG_Cmplx_TxtListLoadedBuilds.addRow(0,%data,getField(%data,0)); //leave last var blank to add new builds to bottom of list
		
		//////////////////////////////////////////////////

		case "RemoveFromBuildList":
		
			%buildName = stripChars(getSubStr(%data,0,30),"^");
			
			for(%c = 0; %c <= PTG_Cmplx_TxtListLoadedBuilds.rowCount(); %c++) //<=???
			{
				if(stripChars(getField(PTG_Cmplx_TxtListLoadedBuilds.getRowText(%c),1),"^") $= %buildName) //don't have to remove color char since it's at beginning of string
				{
					PTG_Cmplx_TxtListLoadedBuilds.removeRow(%c); //.removeRowByID(%c);
					break;
				}
			}
		
		//////////////////////////////////////////////////

		case "UpdateBuildList":

			%enab = getField(%data,0); //blank for "Remove" action
			%buildName = stripChars(getSubStr(getField(%data,1),0,30),"^");
			%rot = getField(%data,2);
			%allow = getFields(%data,3,11);
				%allow = strReplace(strReplace(%allow,"1","  x"),"0","  ");
			%freq = mClamp(getField(%data,12),0,100); //otherwise may replace boolean values meant for %allow
			%owner = getField(%data,13);
			
			//Placement Allowance
			for(%c = 0; %c <= (%count = PTG_Cmplx_TxtListLoadedBuilds.rowCount()) && !%fileFound; %c++) //<=???
			{
				if(stripChars(getField(%rowText = PTG_Cmplx_TxtListLoadedBuilds.getRowText(%c),1),"^") $= %buildName)
				{
					PTG_Cmplx_TxtListLoadedBuilds.removeRow(%c); //.removeRowByID(%c);
					%fileFound = true;
					
					if((%newStr = getSubStr(%tmp = getField(%rowText,0),1,strLen(%tmp))) !$= %tmp)
						%prevCount = %newStr;
					else
						%prevCount = %tmp;					
					if(%enab)
						%colVal = "\c5";
					else
						%colVal = "\c4";
					if(%rot)
						%buildName = %buildName @ "^";
			
					PTG_Cmplx_TxtListLoadedBuilds.addRow(%enab,%colVal @ %prevCount TAB %buildName TAB %allow TAB %freq TAB %owner); //last var for getFields not necessary (getFields(%data,3,12)); defaults to infinity
					PTG_Cmplx_TxtListLoadedBuilds.setSelectedRow(%count-1);
					break;
				}
			}
		
		//////////////////////////////////////////////////

		case "BldConvProgBar":
		
			%currProg = getWord(%data,0);
			%totalProg = getWord(%data,1);
			
			%perc = mFloatLength((%currProg / %totalProg),2) * 240;
			%percAct = mFloatLength((%currProg / %totalProg) * 100,2);
			PTG_Cmplx_TxtBldConvProg.setText("Conversion Progress: (" @ %percAct @ "%)");
			PTG_Cmplx_SwBldConvProg.extent = %perc SPC 12;
		
		//////////////////////////////////////////////////

		case "CloseBldProgWndw":

			if(PTG_Cmplx_WndwBldUpldProg.visible)
				PTG_Cmplx_WndwBldUpldProg.visible = false;
			
			PTG_Complex.bldUpldRunning = false; //to be safe
		
		//////////////////////////////////////////////////
		
		case "NotifyBldListUpdate":

			%plName = getField(%data,0);
			%plID = getField(%data,1);
			
			PTG_Cmplx_WndwBldListNotify.visible = true;
			PTG_Cmplx_TxtBldListNotify.setText("Player \"" @ %plName @ "\" (ID:" @ %plID @ ") recently modified server builds; it\'s recommended to reload your list above to see the changes.");
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function CLIENTCMDPTG_ReceiveMsg(%icon,%title,%text)
{
	PTG_MsgBox_Wndw.setText(%title);
	PTG_MsgBox_Text.setText("<font:Verdana Bold:14>" @ %text);
	
	switch$(%icon)
	{
		case "Denied": 
			PTG_MsgBox_Icon.setBitmap("Add-Ons/System_PTG/GUIs/lock");
		case "Failed": 
			PTG_MsgBox_Icon.setBitmap("Add-Ons/System_PTG/GUIs/caution_red");
		case "Error": 
			PTG_MsgBox_Icon.setBitmap("Add-Ons/System_PTG/GUIs/caution_yellow");
		case "Success": 
			PTG_MsgBox_Icon.setBitmap("Add-Ons/System_PTG/GUIs/checkmark");
		case "Info": 
			PTG_MsgBox_Icon.setBitmap("Add-Ons/System_PTG/GUIs/info");
		case "SuccessError":
			PTG_MsgBox_Icon.setBitmap("Add-Ons/System_PTG/GUIs/checkmarkError");
	}
	
	if((%title = strReplace(%title,"Preview ","")) $= "PTG ERROR: Incorrect Biome XY Scale" || %title $= "PTG ERROR: Incorrect Noise Scales")
		PTG_MsgBox_BtnAutoFix.setVisible(1);
	else
		PTG_MsgBox_BtnAutoFix.setVisible(0);
	
	//Random BG Layers
	if(getRandom(0,1))
		PTG_MsgBox_BGa.setVisible(1);
	else
		PTG_MsgBox_BGa.setVisible(0);
	if(getRandom(0,1))
		PTG_MsgBox_BGb.setVisible(1);
	else
		PTG_MsgBox_BGb.setVisible(0);
	if(getRandom(0,1))
		PTG_MsgBox_BGc.setVisible(1);
	else
		PTG_MsgBox_BGc.setVisible(0);
	if(getRandom(0,1))
		PTG_MsgBox_BGd.setVisible(1);
	else
		PTG_MsgBox_BGd.setVisible(0);
	
	Canvas.PushDialog("PTG_MsgBox");
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function CLIENTCMDPTG_ReceiveChunkInfo(%exists,%info,%chCount,%saveCount)
{
	//%info = ChunkPos, PlyrPos, Static, Untable, Edited, BrCount, SavePres
	
	%chX = getWord(getField(%info,0),0);
	%chY = getWord(getField(%info,0),1);
		
	if(%exists)
	{
		//1st Title: Chunk Exists & Icon
		PTG_ChMngr_TitleChExst.setText("<font:arial bold:24><color:ffffff>This Chunk (Exists)"); //<color:009900>
		PTG_ChMngr_Icon.setBitmap("Add-Ons/System_PTG/GUIs/chunk_exists");
		
		//Chunk Pos
		PTG_ChMngr_TextChPos.setText("<font:arial bold:16>Chunk Pos: X:<color:009900>" @ %chX @ " <color:000000>Y:<color:009900>" @ %chY);
	}
	else
	{
		//1st Title: Chunk Exists & Icon
		PTG_ChMngr_TitleChExst.setText("<font:arial bold:24><color:ffffff>This Chunk (Doesn\'t Exist)"); //<color:ff0000>
		PTG_ChMngr_Icon.setBitmap("Add-Ons/System_PTG/GUIs/chunk_dexist");
		
		//Chunk Pos
		PTG_ChMngr_TextChPos.setText("<font:arial bold:16>Chunk Pos: X:<color:ff0000>" @ %chX @ " <color:000000>Y:<color:ff0000>" @ %chY);
	}
	
	//Plyr Pos
	%plX = getWord(getField(%info,1),0);
	%plY = getWord(getField(%info,1),1);
	PTG_ChMngr_TextYourPos.setText("<font:arial bold:16>Your Pos: X:<color:009900>" @ %plX @ " <color:000000>Y:<color:009900>" @ %plY);
	
	//Static
	if(getField(%info,2))
	{
		PTG_ChMngr_TextChStc.setText("<font:arial bold:16>Chunk Is Static: <color:009900>TRUE");
		PTG_ChMngr_BtnAllStc.static = false; //set opposite static value for next time static toggle button is selected (in Chunk Manager GUI)
	}
	else
	{
		PTG_ChMngr_TextChStc.setText("<font:arial bold:16>Chunk Is Static: <color:0000ff>FALSE");
		PTG_ChMngr_BtnAllStc.static = true; //set opposite static value for next time static toggle button is selected (in Chunk Manager GUI)
	}
	
	//Stable
	if(!getField(%info,3))
		PTG_ChMngr_TextChStb.setText("<font:arial bold:16>Chunk Is Stable: <color:009900>TRUE");
	else
		PTG_ChMngr_TextChStb.setText("<font:arial bold:16>Chunk Is Stable: <color:ff0000>FALSE");
	
	//Edited
	if(getField(%info,4))
		PTG_ChMngr_TextChEdt.setText("<font:arial bold:16>Chunk Is Edited: <color:009900>TRUE");
	else
		PTG_ChMngr_TextChEdt.setText("<font:arial bold:16>Chunk Is Edited: <color:0000ff>FALSE");
	
	//Brick Count
	if((%brCount = getField(%info,5)) > 0)
		PTG_ChMngr_TextBrCnt.setText("<font:arial bold:16>Bricks In Chunk: <color:009900>" SPC %brCount);
	else
		PTG_ChMngr_TextBrCnt.setText("<font:arial bold:16>Bricks In Chunk: <color:ff0000> None");
	
	//Save Present
	if(getField(%info,6))
		PTG_ChMngr_TextSvPres.setText("<font:arial bold:16>Chunk Save Present: <color:009900>YES");
	else
		PTG_ChMngr_TextSvPres.setText("<font:arial bold:16>Chunk Save Present: <color:ff0000>NO");
	
	//2nd Title: Chunk Count
	if(%chCount > 0)
		PTG_ChMngr_TitleChPres.setText("<font:arial bold:24><color:ffffff>All Chunks (" @ %chCount @ " Present)"); //<color:009900>
	else
		PTG_ChMngr_TitleChPres.setText("<font:arial bold:24><color:ffffff>All Chunks (None Present)"); //<color:ff0000>
	
	if(%saveCount $= "N/A")
		PTG_ChMngr_TextTotalSvPres.setText("<font:arial bold:16>Total saves for seed / chunk size: <color:ff0000>N/A");
	else
	{
		if(%saveCount > 0)
			PTG_ChMngr_TextTotalSvPres.setText("<font:arial bold:16>Total saves for seed / chunk size: <color:009900>" @ %saveCount);
		else
			PTG_ChMngr_TextTotalSvPres.setText("<font:arial bold:16>Total saves for seed / chunk size: <color:ff0000>0");
	}
	
	Canvas.PushDialog("PTG_ChunkManager");
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//// Populate Prints List From Server ////
function CLIENTCMDPTG_ReceiveGUIPrints(%action,%data)
{
	switch$(%action)
	{
		case "PrintCount":

			$PTG_MaxPrints = %data;
			
			//Reset GUI
			PTG_PriSelect_SwPriBtns.clear();
				PTG_PriSelect_SwPriBtns.resize(0,0,284,352); //PTG_PriSelect_SwPriBtns.extent = "284 352";
			PTG_PriSelect_PopUpPri.clear();
				PTG_PriSelect_PopUpPri.setValue("Choose Category");
			PTG_PrintSelect.relColID = "";
			
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
		
		case "CreateList":

			%str = %priFP = getField(%data,1);
			%print = filebase(%str);
			%str = strReplace(%str,"_"," ");
			%printUIN = getWord(%str,1) @ "/" @ %print;
			
			$PTG_PrintArr[%priID = getField(%data,0)] = %printUIN TAB %priFP; //keep uiNames?
			$PTG_PermRefArr_Pri[%printUIN] = %priID;
			
		//////////////////////////////////////////////////

		case "SetupCatList":
		
			//Get category list string
			for(%c = 0; %c < $PTG_MaxPrints; %c++) //$PTG_MaxPrints is server-sided (take ded serv into account)
			{
				%priCat = strReplace(getField($PTG_PrintArr[%c],1),"/"," "); //getPrintTexture(%c) is server-sided (take ded serv into account)
				%priCat = getWord(%priCat,1);
				%priCat = strReplace(%priCat,"Print_","");
				
				if(strStr(%catStr,%priCat) == -1)
				{
					%catStr = setWord(%catStr,%catNum,%priCat);
					%catNum++;
				}
			}

			//List categories in popup menu
			for(%c = 0; %c < %catNum; %c++)
				PTG_PriSelect_PopUpPri.add(getWord(%catStr,%c),%c);
			
			//PTG_PriSelect_PopUpPri.add("ALL PRINTS",%c + 1);

			PTG_PrintSelect.rcvdPrintData = true;
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function CLIENTCMDPTG_SetUploadRelay(%pass,%detData,%secKey,%autoStart)
{
	switch$(%pass)
	{
		case "Initialize":
			
			//If no print is setup when using ModTer bricks for Terrain, Clouds or Floating Islands, notify user
			if(!PTG_Confirm_ChkSUBPdontshow.getValue() && ((PTG_Cmplx_BmpTerPri.PrintID $= "" && PTG_Cmplx_BmpTerBr.ModTer) || (PTG_Cmplx_BmpCloudPri.PrintID $= "" && PTG_Cmplx_BmpCloudBr.ModTer) || (PTG_Cmplx_BmpFltIsldsPri.PrintID $= "" && PTG_Cmplx_BmpFltIsldsBr.ModTer)))
			{
				PTG_GUI_ConfirmMsg("UploadPriByPass");
				return;
			}				
			
			commandToServer('PTG_SetUploadRelay',"Initialize","","","","","",%autoStart);
			
		//////////////////////////////////////////////////
		
		case "Setup":
		
			%strA = PTG_Cmplx_EdtSeed.getValue() SPC 
			
			PTG_Cmplx_BmpTerBr.BrickID SPC 
			PTG_Cmplx_BmpTerBr.ModTer SPC 
			PTG_Cmplx_BmpTerPri.PrintID SPC 
			PTG_Cmplx_SwTerCol.colorID SPC 
			//	PTG_Complex.treeBaseCol SPC //automatically calculates nearest brown color for tree base (for Sylvanor tree support, if those bricks are used)
			
			PTG_Cmplx_PopUpTerType.getSelected() SPC
			PTG_Cmplx_ChkRadialGrid.getValue() SPC
			PTG_Cmplx_PopUpModTerType.getSelected() SPC
			PTG_Cmplx_ChkGradualGen.getValue() SPC
			PTG_Cmplx_EnabAutoSave.getValue() SPC
			
			PTG_Cmplx_EdtGridXStart.getValue() SPC 
			PTG_Cmplx_EdtGridYStart.getValue() SPC 
			PTG_Cmplx_EdtGridXEnd.getValue() SPC 
			PTG_Cmplx_EdtGridYEnd.getValue() SPC 
			PTG_Cmplx_ChkEdgeFallOff.getValue() SPC 
			PTG_Cmplx_EdtEdgeFallOffDist.getValue() SPC 
			
			PTG_Cmplx_EnabInfiniteTer.getValue() SPC
			PTG_Cmplx_SldrChRadP.getValue() SPC
			PTG_Cmplx_SldrChRadSA.getValue() SPC 
			PTG_Cmplx_ChkClrDistChunks.getValue();
			
			
			if(strLen(%strA) > 255)
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Action Failed","Data string for settings in Setup is too large, and can't be sent over the network successfully.");
				commandToServer('PTG_SetUploadRelay',"Clear","","","","",%secKey,"");
				return;
			}
			
			commandToServer('PTG_SetUploadRelay',"Setup",%strA,"","","",%secKey,%autoStart);
			
		//////////////////////////////////////////////////
			
		case "Features":

			%strA = PTG_Cmplx_BmpTerDirtPri.PrintID SPC 
			PTG_Cmplx_SwTerDirtCol.colorID SPC 
			PTG_Cmplx_BmpTerStonePri.PrintID SPC 
			PTG_Cmplx_SwTerStoneCol.colorID SPC 

			PTG_Cmplx_SldrWaterLevel.getValue() SPC 
			PTG_Cmplx_SldrSandLevel.getValue() SPC 
			PTG_Cmplx_SldrTerZOffset.getValue() SPC 
			PTG_Cmplx_ChkEnabCnctLakes.getValue() SPC 
			PTG_Cmplx_ChkEnabPlateCap.getValue() SPC 
			PTG_Cmplx_ChkDirtSameBioTer.getValue() SPC
			PTG_Cmplx_ChkShoreSameCustBiome.getValue() SPC
			PTG_Cmplx_ChkDisWater.getValue() SPC
			
			PTG_Cmplx_ChkEnabDetails.getValue() SPC
			PTG_Cmplx_SldrDetailFreq.getValue() SPC
			PTG_Cmplx_ChkEnabCustABio.getValue() SPC
			PTG_Cmplx_ChkEnabCustBBio.getValue() SPC
			PTG_Cmplx_ChkEnabCustCBio.getValue() SPC
			PTG_Cmplx_ChkAutoHideSpawns.getValue() SPC
			PTG_Cmplx_ChkEnabFltIsldDetails.getValue() SPC
			
			PTG_Cmplx_ChkEnabMntns.getValue() SPC 
			PTG_Cmplx_ChkEnabMntnSnow.getValue() SPC 
			PTG_Cmplx_SldrMntnSnowLevel.getValue() SPC 
			PTG_Cmplx_SldrMntnZSnapMult.getValue() SPC 
			PTG_Cmplx_SldrMntnZMult.getValue() SPC
			
			PTG_Cmplx_ChkEnabCaves.getValue() SPC 
			PTG_Cmplx_SldrCaveZOffset.getValue() SPC
			
			PTG_Cmplx_ChkEnabClouds.getValue() SPC 
			PTG_Cmplx_BmpCloudBr.BrickID SPC 
			PTG_Cmplx_BmpCloudBr.ModTer SPC 
			PTG_Cmplx_BmpCloudPri.PrintID SPC 
			PTG_Cmplx_SwCloudCol.colorID SPC 
			PTG_Cmplx_SldrCloudZOffset.getValue() SPC 
			PTG_Cmplx_ChkEnabCloudColl.getValue() SPC
			PTG_Cmplx_PopUpModTerType_clouds.getSelected();
			
			%strB = PTG_Cmplx_ChkEnabFltIslds.getValue() SPC 
			PTG_Cmplx_BmpFltIsldsBr.BrickID SPC 
			PTG_Cmplx_BmpFltIsldsBr.ModTer SPC 
			PTG_Cmplx_BmpFltIsldsPri.PrintID SPC 
			PTG_Cmplx_SwFltIsldsCol.colorID SPC 
			PTG_Cmplx_SldrFltIsldsAZOffset.getValue() SPC 
			PTG_Cmplx_SldrFltIsldsBZOffset.getValue() SPC 
			PTG_Cmplx_BmpFltIsldsDirtPri.PrintID SPC 
			PTG_Cmplx_SwFltIsldsDirtCol.colorID SPC 
			PTG_Cmplx_BmpFltIsldsStonePri.PrintID SPC 
			PTG_Cmplx_SwFltIsldsStoneCol.colorID SPC 
			PTG_Cmplx_PopUpModTerType_fltislds.getSelected() SPC
			
			PTG_Cmplx_ChkEnabBnds.getValue() SPC 
			PTG_Cmplx_BmpBndsWallPri.PrintID SPC 
			PTG_Cmplx_SwBndsWallCol.colorID SPC 
			PTG_Cmplx_BmpBndsCeilPri.PrintID SPC 
			PTG_Cmplx_SwBndsCeilCol.colorID SPC 
			PTG_Cmplx_SldrBndsHAboveTer.getValue() SPC 
			PTG_Cmplx_SldrBndsH.getValue() SPC 
			PTG_Cmplx_ChkBndsRelToTer.getValue() SPC
			PTG_Cmplx_ChkBndsStrtRelOffset.getValue() SPC
			PTG_Cmplx_ChkEnabBndsCeil.getValue() SPC 
			PTG_Cmplx_ChkBndsUseStatic.getValue() SPC 
			PTG_Cmplx_ChkBndsInvisStatic.getValue();
			
			
			if(strLen(%strA) > 255 || strLen(%strB) > 255)
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Action Failed","One of the data strings for settings in Features is too large, and can't be sent over the network successfully.");
				commandToServer('PTG_SetUploadRelay',"Clear","","","","",%secKey,"");
				return;
			}
			
			commandToServer('PTG_SetUploadRelay',"Features",%strA,%strB,"","",%secKey,%autoStart);
			
		//////////////////////////////////////////////////
		
		case "BuildLoad":
		
			%strA = PTG_Cmplx_ChkEnabBuildLoad.getValue() SPC
			PTG_Cmplx_ChkAllowFlatAreas.getValue() SPC
			PTG_Cmplx_ChkGenDetFlatAreas.getValue() SPC
			PTG_Cmplx_ChkBldLdUseTerHMax.getValue() SPC
			PTG_Cmplx_SldrFlatAreaFreq.getValue() SPC
			PTG_Cmplx_PopUpGridSizeSmall.getSelected() SPC //.getValue()
			PTG_Cmplx_PopUpGridSizeLarge.getSelected();// SPC //.getValue()
			//	PTG_Cmplx_TxtListLoadedBuilds.rowCount();
			
			
			if(strLen(%strA) > 255) //not really needed, but safer to include
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Action Failed","The data string for settings in Build-Loading is too large, and can't be sent over the network successfully.");
				commandToServer('PTG_SetUploadRelay',"Clear","","","","",%secKey,"");
				return;
			}
			
			commandToServer('PTG_SetUploadRelay',"BuildLoad",%strA,"","","",%secKey,%autoStart);
			
		//////////////////////////////////////////////////
			
		case "BiomeDef":
		
			%strA =  "" SPC  "" SPC 
			PTG_Cmplx_BmpBioDefWatPri.PrintID SPC 
			PTG_Cmplx_SwBioDefWatCol.colorID SPC "" SPC "" SPC
			//PTG_Cmplx_CheckAllMntnsDefBio.getValue() SPC 
			PTG_Cmplx_PopUpWaterType_Def.getSelected();
			
			if(!PTG_DetLst_ChkEnab_DefBio.getValue())
			{
				%strB = PTG_Cmplx_BmpBioDefDet0Br.brickID SPC 
				PTG_Cmplx_BmpBioDefDet0Pri.PrintID SPC 
				PTG_Cmplx_SwBioDefDet0Col.colorID SPC
				PTG_Cmplx_BmpBioDefDet1Br.brickID SPC 
				PTG_Cmplx_BmpBioDefDet1Pri.PrintID SPC 
				PTG_Cmplx_SwBioDefDet1Col.colorID SPC
				PTG_Cmplx_BmpBioDefDet2Br.brickID SPC 
				PTG_Cmplx_BmpBioDefDet2Pri.PrintID SPC 
				PTG_Cmplx_SwBioDefDet2Col.colorID SPC 
				PTG_Cmplx_BmpBioDefDet3Br.brickID SPC 
				PTG_Cmplx_BmpBioDefDet3Pri.PrintID SPC 
				PTG_Cmplx_SwBioDefDet3Col.colorID SPC 
				PTG_Cmplx_BmpBioDefDet4Br.brickID SPC 
				PTG_Cmplx_BmpBioDefDet4Pri.PrintID SPC 
				PTG_Cmplx_SwBioDefDet4Col.colorID SPC 
				PTG_Cmplx_BmpBioDefDet5Br.brickID SPC 
				PTG_Cmplx_BmpBioDefDet5Pri.PrintID SPC 
				PTG_Cmplx_SwBioDefDet5Col.colorID;
				
				%strC = PTG_Cmplx_BmpBioDefDet6Br.brickID SPC 
				PTG_Cmplx_BmpBioDefDet6Pri.PrintID SPC 
				PTG_Cmplx_SwBioDefDet6Col.colorID SPC 
				PTG_Cmplx_BmpBioDefDet7Br.brickID SPC 
				PTG_Cmplx_BmpBioDefDet7Pri.PrintID SPC 
				PTG_Cmplx_SwBioDefDet7Col.colorID SPC 
				PTG_Cmplx_BmpBioDefDet8Br.brickID SPC 
				PTG_Cmplx_BmpBioDefDet8Pri.PrintID SPC 
				PTG_Cmplx_SwBioDefDet8Col.colorID SPC 
				PTG_Cmplx_BmpBioDefDet9Br.brickID SPC 
				PTG_Cmplx_BmpBioDefDet9Pri.PrintID SPC 
				PTG_Cmplx_SwBioDefDet9Col.colorID SPC
				PTG_Cmplx_BmpBioDefDet10Br.brickID SPC 
				PTG_Cmplx_BmpBioDefDet10Pri.PrintID SPC 
				PTG_Cmplx_SwBioDefDet10Col.colorID SPC 
				PTG_Cmplx_BmpBioDefDet11Br.brickID SPC 
				PTG_Cmplx_BmpBioDefDet11Pri.PrintID SPC 
				PTG_Cmplx_SwBioDefDet11Col.colorID;
				
				%strD = PTG_Cmplx_BmpBioDefDet12Br.brickID SPC 
				PTG_Cmplx_BmpBioDefDet12Pri.PrintID SPC 
				PTG_Cmplx_SwBioDefDet12Col.colorID SPC 
				PTG_Cmplx_BmpBioDefDet13Br.brickID SPC 
				PTG_Cmplx_BmpBioDefDet13Pri.PrintID SPC 
				PTG_Cmplx_SwBioDefDet13Col.colorID SPC 
				PTG_Cmplx_BmpBioDefDet14Br.brickID SPC 
				PTG_Cmplx_BmpBioDefDet14Pri.PrintID SPC 
				PTG_Cmplx_SwBioDefDet14Col.colorID SPC 
				PTG_Cmplx_BmpBioDefDet15Br.brickID SPC 
				PTG_Cmplx_BmpBioDefDet15Pri.PrintID SPC 
				PTG_Cmplx_SwBioDefDet15Col.colorID SPC
				PTG_Cmplx_BmpBioDefDet16Br.brickID SPC 
				PTG_Cmplx_BmpBioDefDet16Pri.PrintID SPC 
				PTG_Cmplx_SwBioDefDet16Col.colorID SPC 
				PTG_Cmplx_BmpBioDefDet17Br.brickID SPC 
				PTG_Cmplx_BmpBioDefDet17Pri.PrintID SPC 
				PTG_Cmplx_SwBioDefDet17Col.colorID;
			}
			
			
			if(strLen(%strA) > 255 || strLen(%strB) > 255 || strLen(%strC) > 255 || strLen(%strD) > 255)
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Action Failed","One of the data strings for Default Biome settings is too large, and can't be sent over the network successfully.");
				commandToServer('PTG_SetUploadRelay',"Clear","","","","",%secKey,"");
				return;
			}
			
			commandToServer('PTG_SetUploadRelay',"BiomeDef",%strA,%strB,%strC,%strD,%secKey,%autoStart);
			
		//////////////////////////////////////////////////
			
		case "BiomeShore":
		
			%strA = PTG_Cmplx_BmpBioShoreTerPri.PrintID SPC 
			PTG_Cmplx_SwBioShoreTerCol.colorID SPC "" SPC "" SPC "" SPC "" SPC "";
			
			if(!PTG_DetLst_ChkEnab_ShoreBio.getValue())
			{
				%strB = PTG_Cmplx_BmpBioShoreDet0Br.brickID SPC 
				PTG_Cmplx_BmpBioShoreDet0Pri.PrintID SPC 
				PTG_Cmplx_SwBioShoreDet0Col.colorID SPC
				PTG_Cmplx_BmpBioShoreDet1Br.brickID SPC 
				PTG_Cmplx_BmpBioShoreDet1Pri.PrintID SPC 
				PTG_Cmplx_SwBioShoreDet1Col.colorID SPC
				PTG_Cmplx_BmpBioShoreDet2Br.brickID SPC 
				PTG_Cmplx_BmpBioShoreDet2Pri.PrintID SPC 
				PTG_Cmplx_SwBioShoreDet2Col.colorID SPC 
				PTG_Cmplx_BmpBioShoreDet3Br.brickID SPC 
				PTG_Cmplx_BmpBioShoreDet3Pri.PrintID SPC 
				PTG_Cmplx_SwBioShoreDet3Col.colorID SPC 
				PTG_Cmplx_BmpBioShoreDet4Br.brickID SPC 
				PTG_Cmplx_BmpBioShoreDet4Pri.PrintID SPC 
				PTG_Cmplx_SwBioShoreDet4Col.colorID SPC 
				PTG_Cmplx_BmpBioShoreDet5Br.brickID SPC 
				PTG_Cmplx_BmpBioShoreDet5Pri.PrintID SPC 
				PTG_Cmplx_SwBioShoreDet5Col.colorID;
				
				%strC = PTG_Cmplx_BmpBioShoreDet6Br.brickID SPC 
				PTG_Cmplx_BmpBioShoreDet6Pri.PrintID SPC 
				PTG_Cmplx_SwBioShoreDet6Col.colorID SPC 
				PTG_Cmplx_BmpBioShoreDet7Br.brickID SPC 
				PTG_Cmplx_BmpBioShoreDet7Pri.PrintID SPC 
				PTG_Cmplx_SwBioShoreDet7Col.colorID SPC 
				PTG_Cmplx_BmpBioShoreDet8Br.brickID SPC 
				PTG_Cmplx_BmpBioShoreDet8Pri.PrintID SPC 
				PTG_Cmplx_SwBioShoreDet8Col.colorID SPC 
				PTG_Cmplx_BmpBioShoreDet9Br.brickID SPC 
				PTG_Cmplx_BmpBioShoreDet9Pri.PrintID SPC 
				PTG_Cmplx_SwBioShoreDet9Col.colorID SPC
				PTG_Cmplx_BmpBioShoreDet10Br.brickID SPC 
				PTG_Cmplx_BmpBioShoreDet10Pri.PrintID SPC 
				PTG_Cmplx_SwBioShoreDet10Col.colorID SPC 
				PTG_Cmplx_BmpBioShoreDet11Br.brickID SPC 
				PTG_Cmplx_BmpBioShoreDet11Pri.PrintID SPC 
				PTG_Cmplx_SwBioShoreDet11Col.colorID;
				
				%strD = PTG_Cmplx_BmpBioShoreDet12Br.brickID SPC 
				PTG_Cmplx_BmpBioShoreDet12Pri.PrintID SPC 
				PTG_Cmplx_SwBioShoreDet12Col.colorID SPC 
				PTG_Cmplx_BmpBioShoreDet13Br.brickID SPC 
				PTG_Cmplx_BmpBioShoreDet13Pri.PrintID SPC 
				PTG_Cmplx_SwBioShoreDet13Col.colorID SPC 
				PTG_Cmplx_BmpBioShoreDet14Br.brickID SPC 
				PTG_Cmplx_BmpBioShoreDet14Pri.PrintID SPC 
				PTG_Cmplx_SwBioShoreDet14Col.colorID SPC 
				PTG_Cmplx_BmpBioShoreDet15Br.brickID SPC 
				PTG_Cmplx_BmpBioShoreDet15Pri.PrintID SPC 
				PTG_Cmplx_SwBioShoreDet15Col.colorID SPC
				PTG_Cmplx_BmpBioShoreDet16Br.brickID SPC 
				PTG_Cmplx_BmpBioShoreDet16Pri.PrintID SPC 
				PTG_Cmplx_SwBioShoreDet16Col.colorID SPC 
				PTG_Cmplx_BmpBioShoreDet17Br.brickID SPC 
				PTG_Cmplx_BmpBioShoreDet17Pri.PrintID SPC 
				PTG_Cmplx_SwBioShoreDet17Col.colorID;
			}
			
			
			if(strLen(%strA) > 255 || strLen(%strB) > 255 || strLen(%strC) > 255 || strLen(%strD) > 255)
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Action Failed","One of the data strings for Shore Biome settings is too large, and can't be sent over the network successfully.");
				commandToServer('PTG_SetUploadRelay',"Clear","","","","",%secKey,"");
				return;
			}
			
			commandToServer('PTG_SetUploadRelay',"BiomeShore",%strA,%strB,%strC,%strD,%secKey,%autoStart);
			
		//////////////////////////////////////////////////
			
		case "BiomeSubM":
		
			%strA = PTG_Cmplx_BmpBioSubMTerPri.PrintID SPC 
			PTG_Cmplx_SwBioSubMTerCol.colorID SPC "" SPC "" SPC "" SPC "" SPC "";
			
			if(!PTG_DetLst_ChkEnab_SubMBio.getValue())
			{
				%strB = PTG_Cmplx_BmpBioSubMDet0Br.brickID SPC 
				PTG_Cmplx_BmpBioSubMDet0Pri.PrintID SPC 
				PTG_Cmplx_SwBioSubMDet0Col.colorID SPC
				PTG_Cmplx_BmpBioSubMDet1Br.brickID SPC 
				PTG_Cmplx_BmpBioSubMDet1Pri.PrintID SPC 
				PTG_Cmplx_SwBioSubMDet1Col.colorID SPC
				PTG_Cmplx_BmpBioSubMDet2Br.brickID SPC 
				PTG_Cmplx_BmpBioSubMDet2Pri.PrintID SPC 
				PTG_Cmplx_SwBioSubMDet2Col.colorID SPC 
				PTG_Cmplx_BmpBioSubMDet3Br.brickID SPC 
				PTG_Cmplx_BmpBioSubMDet3Pri.PrintID SPC 
				PTG_Cmplx_SwBioSubMDet3Col.colorID SPC 
				PTG_Cmplx_BmpBioSubMDet4Br.brickID SPC 
				PTG_Cmplx_BmpBioSubMDet4Pri.PrintID SPC 
				PTG_Cmplx_SwBioSubMDet4Col.colorID SPC 
				PTG_Cmplx_BmpBioSubMDet5Br.brickID SPC 
				PTG_Cmplx_BmpBioSubMDet5Pri.PrintID SPC 
				PTG_Cmplx_SwBioSubMDet5Col.colorID;
				
				%strC = PTG_Cmplx_BmpBioSubMDet6Br.brickID SPC 
				PTG_Cmplx_BmpBioSubMDet6Pri.PrintID SPC 
				PTG_Cmplx_SwBioSubMDet6Col.colorID SPC 
				PTG_Cmplx_BmpBioSubMDet7Br.brickID SPC 
				PTG_Cmplx_BmpBioSubMDet7Pri.PrintID SPC 	
				PTG_Cmplx_SwBioSubMDet7Col.colorID SPC 
				PTG_Cmplx_BmpBioSubMDet8Br.brickID SPC 
				PTG_Cmplx_BmpBioSubMDet8Pri.PrintID SPC 
				PTG_Cmplx_SwBioSubMDet8Col.colorID SPC 
				PTG_Cmplx_BmpBioSubMDet9Br.brickID SPC 
				PTG_Cmplx_BmpBioSubMDet9Pri.PrintID SPC 
				PTG_Cmplx_SwBioSubMDet9Col.colorID SPC
				PTG_Cmplx_BmpBioSubMDet10Br.brickID SPC 
				PTG_Cmplx_BmpBioSubMDet10Pri.PrintID SPC 
				PTG_Cmplx_SwBioSubMDet10Col.colorID SPC 
				PTG_Cmplx_BmpBioSubMDet11Br.brickID SPC 
				PTG_Cmplx_BmpBioSubMDet11Pri.PrintID SPC 
				PTG_Cmplx_SwBioSubMDet11Col.colorID;
				
				%strD = PTG_Cmplx_BmpBioSubMDet12Br.brickID SPC 
				PTG_Cmplx_BmpBioSubMDet12Pri.PrintID SPC 
				PTG_Cmplx_SwBioSubMDet12Col.colorID SPC 
				PTG_Cmplx_BmpBioSubMDet13Br.brickID SPC 
				PTG_Cmplx_BmpBioSubMDet13Pri.PrintID SPC 
				PTG_Cmplx_SwBioSubMDet13Col.colorID SPC 
				PTG_Cmplx_BmpBioSubMDet14Br.brickID SPC 
				PTG_Cmplx_BmpBioSubMDet14Pri.PrintID SPC 
				PTG_Cmplx_SwBioSubMDet14Col.colorID SPC 
				PTG_Cmplx_BmpBioSubMDet15Br.brickID SPC 
				PTG_Cmplx_BmpBioSubMDet15Pri.PrintID SPC 
				PTG_Cmplx_SwBioSubMDet15Col.colorID SPC
				PTG_Cmplx_BmpBioSubMDet16Br.brickID SPC 
				PTG_Cmplx_BmpBioSubMDet16Pri.PrintID SPC 
				PTG_Cmplx_SwBioSubMDet16Col.colorID SPC 
				PTG_Cmplx_BmpBioSubMDet17Br.brickID SPC 
				PTG_Cmplx_BmpBioSubMDet17Pri.PrintID SPC 
				PTG_Cmplx_SwBioSubMDet17Col.colorID;
			}
			
			
			if(strLen(%strA) > 255 || strLen(%strB) > 255 || strLen(%strC) > 255 || strLen(%strD) > 255)
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Action Failed","One of the data strings for SubMarine Biome settings is too large, and can't be sent over the network successfully.");
				commandToServer('PTG_SetUploadRelay',"Clear","","","","",%secKey,"");
				return;
			}
			
			commandToServer('PTG_SetUploadRelay',"BiomeSubM",%strA,%strB,%strC,%strD,%secKey,%autoStart);
			
		//////////////////////////////////////////////////
			
		case "BiomeCustA":
		
			%strA = PTG_Cmplx_BmpBioCustATerPri.PrintID SPC 
			PTG_Cmplx_SwBioCustATerCol.colorID SPC 
			PTG_Cmplx_BmpBioCustAWatPri.PrintID SPC 
			PTG_Cmplx_SwBioCustAWatCol.colorID SPC 
			PTG_Cmplx_SldrTerHMod_CustA.getValue() SPC "" SPC
			//PTG_Cmplx_CheckAllMntnsCustBioA.getValue() SPC 
			PTG_Cmplx_PopUpWaterType_CustA.getSelected();
			
			if(!PTG_DetLst_ChkEnab_CustBioA.getValue())
			{
				%strB = PTG_Cmplx_BmpBioCustADet0Br.brickID SPC 
				PTG_Cmplx_BmpBioCustADet0Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustADet0Col.colorID SPC
				PTG_Cmplx_BmpBioCustADet1Br.brickID SPC 
				PTG_Cmplx_BmpBioCustADet1Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustADet1Col.colorID SPC
				PTG_Cmplx_BmpBioCustADet2Br.brickID SPC 
				PTG_Cmplx_BmpBioCustADet2Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustADet2Col.colorID SPC 
				PTG_Cmplx_BmpBioCustADet3Br.brickID SPC 
				PTG_Cmplx_BmpBioCustADet3Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustADet3Col.colorID SPC 
				PTG_Cmplx_BmpBioCustADet4Br.brickID SPC 
				PTG_Cmplx_BmpBioCustADet4Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustADet4Col.colorID SPC 
				PTG_Cmplx_BmpBioCustADet5Br.brickID SPC 
				PTG_Cmplx_BmpBioCustADet5Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustADet5Col.colorID;
				
				%strC = PTG_Cmplx_BmpBioCustADet6Br.brickID SPC 
				PTG_Cmplx_BmpBioCustADet6Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustADet6Col.colorID SPC 
				PTG_Cmplx_BmpBioCustADet7Br.brickID SPC 
				PTG_Cmplx_BmpBioCustADet7Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustADet7Col.colorID SPC 
				PTG_Cmplx_BmpBioCustADet8Br.brickID SPC 
				PTG_Cmplx_BmpBioCustADet8Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustADet8Col.colorID SPC 
				PTG_Cmplx_BmpBioCustADet9Br.brickID SPC 
				PTG_Cmplx_BmpBioCustADet9Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustADet9Col.colorID SPC
				PTG_Cmplx_BmpBioCustADet10Br.brickID SPC 
				PTG_Cmplx_BmpBioCustADet10Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustADet10Col.colorID SPC 
				PTG_Cmplx_BmpBioCustADet11Br.brickID SPC 
				PTG_Cmplx_BmpBioCustADet11Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustADet11Col.colorID;
				
				%strD = PTG_Cmplx_BmpBioCustADet12Br.brickID SPC 
				PTG_Cmplx_BmpBioCustADet12Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustADet12Col.colorID SPC 
				PTG_Cmplx_BmpBioCustADet13Br.brickID SPC 
				PTG_Cmplx_BmpBioCustADet13Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustADet13Col.colorID SPC 
				PTG_Cmplx_BmpBioCustADet14Br.brickID SPC 
				PTG_Cmplx_BmpBioCustADet14Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustADet14Col.colorID SPC 
				PTG_Cmplx_BmpBioCustADet15Br.brickID SPC 
				PTG_Cmplx_BmpBioCustADet15Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustADet15Col.colorID SPC
				PTG_Cmplx_BmpBioCustADet16Br.brickID SPC 
				PTG_Cmplx_BmpBioCustADet16Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustADet16Col.colorID SPC 
				PTG_Cmplx_BmpBioCustADet17Br.brickID SPC 
				PTG_Cmplx_BmpBioCustADet17Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustADet17Col.colorID;
			}
			
			
			if(strLen(%strA) > 255 || strLen(%strB) > 255 || strLen(%strC) > 255 || strLen(%strD) > 255)
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Action Failed","One of the data strings for Custom A Biome settings is too large, and can't be sent over the network successfully.");
				commandToServer('PTG_SetUploadRelay',"Clear","","","","",%secKey,"");
				return;
			}
			
			commandToServer('PTG_SetUploadRelay',"BiomeCustA",%strA,%strB,%strC,%strD,%secKey,%autoStart);
			
		//////////////////////////////////////////////////
			
		case "BiomeCustB":
		
			%strA = PTG_Cmplx_BmpBioCustBTerPri.PrintID SPC 
			PTG_Cmplx_SwBioCustBTerCol.colorID SPC 
			PTG_Cmplx_BmpBioCustBWatPri.PrintID SPC 
			PTG_Cmplx_SwBioCustBWatCol.colorID SPC 
			PTG_Cmplx_SldrTerHMod_CustB.getValue() SPC "" SPC
			//PTG_Cmplx_CheckAllMntnsCustBioB.getValue() SPC 
			PTG_Cmplx_PopUpWaterType_CustB.getSelected();
			
			if(!PTG_DetLst_ChkEnab_CustBioB.getValue())
			{
				%strB = PTG_Cmplx_BmpBioCustBDet0Br.brickID SPC 
				PTG_Cmplx_BmpBioCustBDet0Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustBDet0Col.colorID SPC
				PTG_Cmplx_BmpBioCustBDet1Br.brickID SPC 
				PTG_Cmplx_BmpBioCustBDet1Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustBDet1Col.colorID SPC
				PTG_Cmplx_BmpBioCustBDet2Br.brickID SPC 
				PTG_Cmplx_BmpBioCustBDet2Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustBDet2Col.colorID SPC 
				PTG_Cmplx_BmpBioCustBDet3Br.brickID SPC 
				PTG_Cmplx_BmpBioCustBDet3Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustBDet3Col.colorID SPC 
				PTG_Cmplx_BmpBioCustBDet4Br.brickID SPC 
				PTG_Cmplx_BmpBioCustBDet4Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustBDet4Col.colorID SPC 
				PTG_Cmplx_BmpBioCustBDet5Br.brickID SPC 
				PTG_Cmplx_BmpBioCustBDet5Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustBDet5Col.colorID;
				
				%strC = PTG_Cmplx_BmpBioCustBDet6Br.brickID SPC 
				PTG_Cmplx_BmpBioCustBDet6Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustBDet6Col.colorID SPC 
				PTG_Cmplx_BmpBioCustBDet7Br.brickID SPC 
				PTG_Cmplx_BmpBioCustBDet7Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustBDet7Col.colorID SPC 
				PTG_Cmplx_BmpBioCustBDet8Br.brickID SPC 
				PTG_Cmplx_BmpBioCustBDet8Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustBDet8Col.colorID SPC 
				PTG_Cmplx_BmpBioCustBDet9Br.brickID SPC 
				PTG_Cmplx_BmpBioCustBDet9Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustBDet9Col.colorID SPC
				PTG_Cmplx_BmpBioCustBDet10Br.brickID SPC 
				PTG_Cmplx_BmpBioCustBDet10Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustBDet10Col.colorID SPC 
				PTG_Cmplx_BmpBioCustBDet11Br.brickID SPC 
				PTG_Cmplx_BmpBioCustBDet11Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustBDet11Col.colorID;
				
				%strD = PTG_Cmplx_BmpBioCustBDet12Br.brickID SPC 
				PTG_Cmplx_BmpBioCustBDet12Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustBDet12Col.colorID SPC 
				PTG_Cmplx_BmpBioCustBDet13Br.brickID SPC 
				PTG_Cmplx_BmpBioCustBDet13Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustBDet13Col.colorID SPC 
				PTG_Cmplx_BmpBioCustBDet14Br.brickID SPC 
				PTG_Cmplx_BmpBioCustBDet14Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustBDet14Col.colorID SPC 
				PTG_Cmplx_BmpBioCustBDet15Br.brickID SPC 
				PTG_Cmplx_BmpBioCustBDet15Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustBDet15Col.colorID SPC
				PTG_Cmplx_BmpBioCustBDet16Br.brickID SPC 
				PTG_Cmplx_BmpBioCustBDet16Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustBDet16Col.colorID SPC 
				PTG_Cmplx_BmpBioCustBDet17Br.brickID SPC 
				PTG_Cmplx_BmpBioCustBDet17Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustBDet17Col.colorID;
			}
			
			
			if(strLen(%strA) > 255 || strLen(%strB) > 255 || strLen(%strC) > 255 || strLen(%strD) > 255)
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Action Failed","One of the data strings for Custom B Biome settings is too large, and can't be sent over the network successfully.");
				commandToServer('PTG_SetUploadRelay',"Clear","","","","",%secKey,"");
				return;
			}
			
			commandToServer('PTG_SetUploadRelay',"BiomeCustB",%strA,%strB,%strC,%strD,%secKey,%autoStart);
			
		//////////////////////////////////////////////////
			
		case "BiomeCustC":
		
			%strA = PTG_Cmplx_BmpBioCustCTerPri.PrintID SPC 
			PTG_Cmplx_SwBioCustCTerCol.colorID SPC 
			PTG_Cmplx_BmpBioCustCWatPri.PrintID SPC 
			PTG_Cmplx_SwBioCustCWatCol.colorID SPC 
			PTG_Cmplx_SldrTerHMod_CustC.getValue() SPC "" SPC
			//PTG_Cmplx_CheckAllMntnsCustBioC.getValue() SPC 
			PTG_Cmplx_PopUpWaterType_CustC.getSelected();
			
			if(!PTG_DetLst_ChkEnab_CustBioC.getValue())
			{
				%strB = PTG_Cmplx_BmpBioCustCDet0Br.brickID SPC 
				PTG_Cmplx_BmpBioCustCDet0Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustCDet0Col.colorID SPC
				PTG_Cmplx_BmpBioCustCDet1Br.brickID SPC 
				PTG_Cmplx_BmpBioCustCDet1Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustCDet1Col.colorID SPC
				PTG_Cmplx_BmpBioCustCDet2Br.brickID SPC 
				PTG_Cmplx_BmpBioCustCDet2Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustCDet2Col.colorID SPC 
				PTG_Cmplx_BmpBioCustCDet3Br.brickID SPC 
				PTG_Cmplx_BmpBioCustCDet3Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustCDet3Col.colorID SPC 
				PTG_Cmplx_BmpBioCustCDet4Br.brickID SPC 
				PTG_Cmplx_BmpBioCustCDet4Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustCDet4Col.colorID SPC 
				PTG_Cmplx_BmpBioCustCDet5Br.brickID SPC 
				PTG_Cmplx_BmpBioCustCDet5Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustCDet5Col.colorID;
				
				%strC = PTG_Cmplx_BmpBioCustCDet6Br.brickID SPC 
				PTG_Cmplx_BmpBioCustCDet6Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustCDet6Col.colorID SPC 
				PTG_Cmplx_BmpBioCustCDet7Br.brickID SPC 
				PTG_Cmplx_BmpBioCustCDet7Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustCDet7Col.colorID SPC 
				PTG_Cmplx_BmpBioCustCDet8Br.brickID SPC 
				PTG_Cmplx_BmpBioCustCDet8Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustCDet8Col.colorID SPC 
				PTG_Cmplx_BmpBioCustCDet9Br.brickID SPC 
				PTG_Cmplx_BmpBioCustCDet9Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustCDet9Col.colorID SPC
				PTG_Cmplx_BmpBioCustCDet10Br.brickID SPC 
				PTG_Cmplx_BmpBioCustCDet10Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustCDet10Col.colorID SPC 
				PTG_Cmplx_BmpBioCustCDet11Br.brickID SPC 
				PTG_Cmplx_BmpBioCustCDet11Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustCDet11Col.colorID;
				
				%strD = PTG_Cmplx_BmpBioCustCDet12Br.brickID SPC 
				PTG_Cmplx_BmpBioCustCDet12Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustCDet12Col.colorID SPC 
				PTG_Cmplx_BmpBioCustCDet13Br.brickID SPC 
				PTG_Cmplx_BmpBioCustCDet13Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustCDet13Col.colorID SPC 
				PTG_Cmplx_BmpBioCustCDet14Br.brickID SPC 
				PTG_Cmplx_BmpBioCustCDet14Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustCDet14Col.colorID SPC 
				PTG_Cmplx_BmpBioCustCDet15Br.brickID SPC 
				PTG_Cmplx_BmpBioCustCDet15Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustCDet15Col.colorID SPC
				PTG_Cmplx_BmpBioCustCDet16Br.brickID SPC 
				PTG_Cmplx_BmpBioCustCDet16Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustCDet16Col.colorID SPC 
				PTG_Cmplx_BmpBioCustCDet17Br.brickID SPC 
				PTG_Cmplx_BmpBioCustCDet17Pri.PrintID SPC 
				PTG_Cmplx_SwBioCustCDet17Col.colorID;
			}
			
			
			if(strLen(%strA) > 255 || strLen(%strB) > 255 || strLen(%strC) > 255 || strLen(%strD) > 255)
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Action Failed","One of the data strings for Custom C Biome settings is too large, and can't be sent over the network successfully.");
				commandToServer('PTG_SetUploadRelay',"Clear","","","","",%secKey,"");
				return;
			}
			
			commandToServer('PTG_SetUploadRelay',"BiomeCustC",%strA,%strB,%strC,%strD,%secKey,%autoStart);
			
		//////////////////////////////////////////////////
			
		case "BiomeCaveTop":
		
			%strA = PTG_Cmplx_BmpBioCaveTTerPri.PrintID SPC 
			PTG_Cmplx_SwBioCaveTTerCol.colorID SPC "" SPC "" SPC "" SPC "" SPC "";
			
			if(!PTG_DetLst_ChkEnab_CaveTBio.getValue())
			{
				%strB = PTG_Cmplx_BmpBioCaveTDet0Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveTDet0Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveTDet0Col.colorID SPC
				PTG_Cmplx_BmpBioCaveTDet1Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveTDet1Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveTDet1Col.colorID SPC
				PTG_Cmplx_BmpBioCaveTDet2Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveTDet2Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveTDet2Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveTDet3Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveTDet3Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveTDet3Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveTDet4Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveTDet4Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveTDet4Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveTDet5Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveTDet5Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveTDet5Col.colorID;
				
				%strC = PTG_Cmplx_BmpBioCaveTDet6Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveTDet6Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveTDet6Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveTDet7Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveTDet7Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveTDet7Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveTDet8Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveTDet8Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveTDet8Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveTDet9Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveTDet9Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveTDet9Col.colorID SPC
				PTG_Cmplx_BmpBioCaveTDet10Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveTDet10Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveTDet10Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveTDet11Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveTDet11Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveTDet11Col.colorID;
				
				%strD = PTG_Cmplx_BmpBioCaveTDet12Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveTDet12Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveTDet12Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveTDet13Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveTDet13Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveTDet13Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveTDet14Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveTDet14Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveTDet14Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveTDet15Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveTDet15Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveTDet15Col.colorID SPC
				PTG_Cmplx_BmpBioCaveTDet16Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveTDet16Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveTDet16Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveTDet17Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveTDet17Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveTDet17Col.colorID;
			}
			
			
			if(strLen(%strA) > 255 || strLen(%strB) > 255 || strLen(%strC) > 255 || strLen(%strD) > 255)
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Action Failed","One of the data strings for Cave Bottom Top settings is too large, and can't be sent over the network successfully.");
				commandToServer('PTG_SetUploadRelay',"Clear","","","","",%secKey,"");
				return;
			}
			
			commandToServer('PTG_SetUploadRelay',"BiomeCaveTop",%strA,%strB,%strC,%strD,%secKey,%autoStart);
			
		//////////////////////////////////////////////////
			
		case "BiomeCaveBtm":
		
			%strA = PTG_Cmplx_BmpBioCaveBTerPri.PrintID SPC 
			PTG_Cmplx_SwBioCaveBTerCol.colorID SPC "" SPC "" SPC "" SPC "" SPC "";
			
			if(!PTG_DetLst_ChkEnab_CaveBBio.getValue())
			{
				%strB = PTG_Cmplx_BmpBioCaveBDet0Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveBDet0Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveBDet0Col.colorID SPC
				PTG_Cmplx_BmpBioCaveBDet1Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveBDet1Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveBDet1Col.colorID SPC
				PTG_Cmplx_BmpBioCaveBDet2Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveBDet2Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveBDet2Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveBDet3Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveBDet3Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveBDet3Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveBDet4Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveBDet4Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveBDet4Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveBDet5Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveBDet5Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveBDet5Col.colorID;
				
				%strC = PTG_Cmplx_BmpBioCaveBDet6Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveBDet6Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveBDet6Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveBDet7Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveBDet7Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveBDet7Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveBDet8Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveBDet8Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveBDet8Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveBDet9Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveBDet9Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveBDet9Col.colorID SPC
				PTG_Cmplx_BmpBioCaveBDet10Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveBDet10Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveBDet10Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveBDet11Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveBDet11Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveBDet11Col.colorID;
				
				%strD = PTG_Cmplx_BmpBioCaveBDet12Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveBDet12Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveBDet12Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveBDet13Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveBDet13Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveBDet13Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveBDet14Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveBDet14Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveBDet14Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveBDet15Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveBDet15Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveBDet15Col.colorID SPC
				PTG_Cmplx_BmpBioCaveBDet16Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveBDet16Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveBDet16Col.colorID SPC 
				PTG_Cmplx_BmpBioCaveBDet17Br.brickID SPC 
				PTG_Cmplx_BmpBioCaveBDet17Pri.PrintID SPC 
				PTG_Cmplx_SwBioCaveBDet17Col.colorID;
			}
			
			
			if(strLen(%strA) > 255 || strLen(%strB) > 255 || strLen(%strC) > 255 || strLen(%strD) > 255)
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Action Failed","One of the data strings for Cave Bottom Biome settings is too large, and can't be sent over the network successfully.");
				commandToServer('PTG_SetUploadRelay',"Clear","","","","",%secKey,"");
				return;
			}
			
			commandToServer('PTG_SetUploadRelay',"BiomeCaveBtm",%strA,%strB,%strC,%strD,%secKey,%autoStart);
			
		//////////////////////////////////////////////////
			
		case "BiomeMntn":
		
			%strA = PTG_Cmplx_BmpBioMntnRockPri.PrintID SPC
			PTG_Cmplx_SwBioMntnRockCol.colorID SPC
			PTG_Cmplx_BmpBioMntnSnowPri.PrintID SPC
			PTG_Cmplx_SwBioMntnSnowCol.colorID SPC "" SPC "" SPC "";
			
			if(!PTG_DetLst_ChkEnab_MntnBio.getValue())
			{
				%strB = PTG_Cmplx_BmpBioMntnDet0Br.brickID SPC 
				PTG_Cmplx_BmpBioMntnDet0Pri.PrintID SPC 
				PTG_Cmplx_SwBioMntnDet0Col.colorID SPC
				PTG_Cmplx_BmpBioMntnDet1Br.brickID SPC 
				PTG_Cmplx_BmpBioMntnDet1Pri.PrintID SPC 
				PTG_Cmplx_SwBioMntnDet1Col.colorID SPC
				PTG_Cmplx_BmpBioMntnDet2Br.brickID SPC 
				PTG_Cmplx_BmpBioMntnDet2Pri.PrintID SPC 
				PTG_Cmplx_SwBioMntnDet2Col.colorID SPC 
				PTG_Cmplx_BmpBioMntnDet3Br.brickID SPC 
				PTG_Cmplx_BmpBioMntnDet3Pri.PrintID SPC 
				PTG_Cmplx_SwBioMntnDet3Col.colorID SPC 
				PTG_Cmplx_BmpBioMntnDet4Br.brickID SPC 
				PTG_Cmplx_BmpBioMntnDet4Pri.PrintID SPC 
				PTG_Cmplx_SwBioMntnDet4Col.colorID SPC 
				PTG_Cmplx_BmpBioMntnDet5Br.brickID SPC 
				PTG_Cmplx_BmpBioMntnDet5Pri.PrintID SPC 
				PTG_Cmplx_SwBioMntnDet5Col.colorID;
				
				%strC = PTG_Cmplx_BmpBioMntnDet6Br.brickID SPC 
				PTG_Cmplx_BmpBioMntnDet6Pri.PrintID SPC 
				PTG_Cmplx_SwBioMntnDet6Col.colorID SPC 
				PTG_Cmplx_BmpBioMntnDet7Br.brickID SPC 
				PTG_Cmplx_BmpBioMntnDet7Pri.PrintID SPC 
				PTG_Cmplx_SwBioMntnDet7Col.colorID SPC 
				PTG_Cmplx_BmpBioMntnDet8Br.brickID SPC 
				PTG_Cmplx_BmpBioMntnDet8Pri.PrintID SPC 
				PTG_Cmplx_SwBioMntnDet8Col.colorID SPC 
				PTG_Cmplx_BmpBioMntnDet9Br.brickID SPC 
				PTG_Cmplx_BmpBioMntnDet9Pri.PrintID SPC 
				PTG_Cmplx_SwBioMntnDet9Col.colorID SPC
				PTG_Cmplx_BmpBioMntnDet10Br.brickID SPC 
				PTG_Cmplx_BmpBioMntnDet10Pri.PrintID SPC 
				PTG_Cmplx_SwBioMntnDet10Col.colorID SPC 
				PTG_Cmplx_BmpBioMntnDet11Br.brickID SPC 
				PTG_Cmplx_BmpBioMntnDet11Pri.PrintID SPC 
				PTG_Cmplx_SwBioMntnDet11Col.colorID;
				
				%strD = PTG_Cmplx_BmpBioMntnDet12Br.brickID SPC 
				PTG_Cmplx_BmpBioMntnDet12Pri.PrintID SPC 
				PTG_Cmplx_SwBioMntnDet12Col.colorID SPC 
				PTG_Cmplx_BmpBioMntnDet13Br.brickID SPC 
				PTG_Cmplx_BmpBioMntnDet13Pri.PrintID SPC 
				PTG_Cmplx_SwBioMntnDet13Col.colorID SPC 
				PTG_Cmplx_BmpBioMntnDet14Br.brickID SPC 
				PTG_Cmplx_BmpBioMntnDet14Pri.PrintID SPC 
				PTG_Cmplx_SwBioMntnDet14Col.colorID SPC 
				PTG_Cmplx_BmpBioMntnDet15Br.brickID SPC 
				PTG_Cmplx_BmpBioMntnDet15Pri.PrintID SPC 
				PTG_Cmplx_SwBioMntnDet15Col.colorID SPC
				PTG_Cmplx_BmpBioMntnDet16Br.brickID SPC 
				PTG_Cmplx_BmpBioMntnDet16Pri.PrintID SPC 
				PTG_Cmplx_SwBioMntnDet16Col.colorID SPC 
				PTG_Cmplx_BmpBioMntnDet17Br.brickID SPC 
				PTG_Cmplx_BmpBioMntnDet17Pri.PrintID SPC 
				PTG_Cmplx_SwBioMntnDet17Col.colorID;
			}
			
			
			if(strLen(%strA) > 255 || strLen(%strB) > 255 || strLen(%strC) > 255 || strLen(%strD) > 255)
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Action Failed","One of the data strings for Mountain Biome settings is too large, and can't be sent over the network successfully.");
				commandToServer('PTG_SetUploadRelay',"Clear","","","","",%secKey,"");
				return;
			}
			
			commandToServer('PTG_SetUploadRelay',"BiomeMntn",%strA,%strB,%strC,%strD,%secKey,%autoStart);
			
		//////////////////////////////////////////////////
		
		case "Advanced":
		
			%strA = PTG_Cmplx_PopupChSize.getSelected() SPC
			PTG_Cmplx_SldrCaveTopZMult.getValue() SPC
			PTG_Cmplx_SldrZMod.getValue() SPC
			PTG_Cmplx_SldrCnctLakesStrt.getValue() SPC
			PTG_Cmplx_SwTreeBaseACol.colorID SPC 
			PTG_Cmplx_SwTreeBaseBCol.colorID SPC 
			PTG_Cmplx_SwTreeBaseCCol.colorID SPC
			PTG_Cmplx_ChkFIFOclrChunks.getValue() SPC
			PTG_Routines_ChkSeamlessModTer.getValue() SPC
			PTG_Routines_ChkSeamlessBuildL.getValue() SPC
			
			PTG_Cmplx_EdtSectCave.getValue() SPC
			PTG_Cmplx_EdtSectSkyland.getValue() SPC
			PTG_Cmplx_EdtSectFltIsld.getValue() SPC
			PTG_Cmplx_EdtSectCustA.getValue() SPC
			PTG_Cmplx_EdtSectCustB.getValue() SPC
			PTG_Cmplx_EdtSectCustC.getValue() SPC
			PTG_Cmplx_EdtSectCloud.getValue() SPC
			PTG_Cmplx_EdtSectMntn.getValue();
			
			
			%strB = PTG_Cmplx_EdtTerNosOffX.getValue() SPC 
			PTG_Cmplx_EdtTerNosOffY.getValue() SPC
			PTG_Cmplx_EdtMntnNosOffX.getValue() SPC 
			PTG_Cmplx_EdtMntnNosOffY.getValue() SPC
			PTG_Cmplx_EdtCustANosOffX.getValue() SPC 
			PTG_Cmplx_EdtCustANosOffY.getValue() SPC
			PTG_Cmplx_EdtCustBNosOffX.getValue() SPC 
			PTG_Cmplx_EdtCustBNosOffY.getValue() SPC
			PTG_Cmplx_EdtCustCNosOffX.getValue() SPC 
			PTG_Cmplx_EdtCustCNosOffY.getValue() SPC
			PTG_Cmplx_EdtCaveNosOffX.getValue() SPC 
			PTG_Cmplx_EdtCaveNosOffY.getValue() SPC
			PTG_Cmplx_EdtCaveHNosOffX.getValue() SPC 
			PTG_Cmplx_EdtCaveHNosOffY.getValue() SPC
			PTG_Cmplx_EdtFltIsldANosOffX.getValue() SPC 
			PTG_Cmplx_EdtFltIsldANosOffY.getValue() SPC
			PTG_Cmplx_EdtFltIsldBNosOffX.getValue() SPC 
			PTG_Cmplx_EdtFltIsldBNosOffY.getValue() SPC
			PTG_Cmplx_EdtSkylandNosOffX.getValue() SPC 
			PTG_Cmplx_EdtSkylandNosOffY.getValue() SPC
			PTG_Cmplx_EdtDetNosOffX.getValue() SPC 
			PTG_Cmplx_EdtDetNosOffY.getValue() SPC
			PTG_Cmplx_EdtCloudNosOffX.getValue() SPC 
			PTG_Cmplx_EdtCloudNosOffY.getValue() SPC
			PTG_Cmplx_EdtBldLoadNosOffX.getValue() SPC 
			PTG_Cmplx_EdtBldLoadNosOffY.getValue();
			
			
			%strC = PTG_Cmplx_EdtNosScaleTerAXY.getValue() SPC 
			PTG_Cmplx_EdtNosScaleTerAZ.getValue() SPC
			PTG_Cmplx_EdtNosScaleTerBXY.getValue() SPC 
			PTG_Cmplx_EdtNosScaleTerBZ.getValue() SPC
			PTG_Cmplx_EdtNosScaleTerCXY.getValue() SPC 
			PTG_Cmplx_EdtNosScaleTerCZ.getValue() SPC
			PTG_Cmplx_EdtNosScaleMntnAXY.getValue() SPC 
			PTG_Cmplx_EdtNosScaleMntnAZ.getValue() SPC
			PTG_Cmplx_EdtNosScaleMntnBXY.getValue() SPC 
			PTG_Cmplx_EdtNosScaleMntnBZ.getValue() SPC
			PTG_Cmplx_EdtNosScaleCaveAXY.getValue() SPC 
			PTG_Cmplx_EdtNosScaleCaveAZ.getValue() SPC
			PTG_Cmplx_EdtNosScaleCaveBXY.getValue() SPC 
			PTG_Cmplx_EdtNosScaleCaveBZ.getValue() SPC
			PTG_Cmplx_EdtNosScaleCaveCXY.getValue() SPC 
			PTG_Cmplx_EdtNosScaleCaveCZ.getValue() SPC
			PTG_Cmplx_EdtNosScaleCaveHXY.getValue() SPC 
			PTG_Cmplx_EdtNosScaleCaveHZ.getValue() SPC
			PTG_Cmplx_EdtNosScaleCustAXY.getValue() SPC 
			PTG_Cmplx_EdtNosScaleCustAZ.getValue() SPC
			PTG_Cmplx_EdtNosScaleCustBXY.getValue() SPC 
			PTG_Cmplx_EdtNosScaleCustBZ.getValue() SPC
			PTG_Cmplx_EdtNosScaleCustCXY.getValue() SPC 
			PTG_Cmplx_EdtNosScaleCustCZ.getValue() SPC
			PTG_Cmplx_EdtNosScaleSkylandXY.getValue() SPC 
			PTG_Cmplx_EdtNosScaleSkylandZ.getValue();
			
			
			%strD = PTG_Cmplx_EdtNosScaleCloudAXY.getValue() SPC 
			PTG_Cmplx_EdtNosScaleCloudAZ.getValue() SPC
			PTG_Cmplx_EdtNosScaleCloudBXY.getValue() SPC 
			PTG_Cmplx_EdtNosScaleCloudBZ.getValue() SPC
			PTG_Cmplx_EdtNosScaleFltIsldAXY.getValue() SPC 
			PTG_Cmplx_EdtNosScaleFltIsldAZ.getValue() SPC
			PTG_Cmplx_EdtNosScaleFltIsldBXY.getValue() SPC 
			PTG_Cmplx_EdtNosScaleFltIsldBZ.getValue() SPC
			
			PTG_Cmplx_ChkEnabPseudoEqtr.getValue() SPC
			PTG_Cmplx_EdtEquatorCustAY.getValue() SPC
			PTG_Cmplx_EdtEquatorCustBY.getValue() SPC
			PTG_Cmplx_EdtEquatorCustCY.getValue() SPC
			PTG_Cmplx_EdtEquatorCaveY.getValue() SPC
			PTG_Cmplx_EdtEquatorMntnY.getValue() SPC
			PTG_Cmplx_EdtEquatorCloudY.getValue() SPC
			PTG_Cmplx_EdtEquatorFltIsldY.getValue();
			
			
			if(strLen(%strA) > 255 || strLen(%strB) > 255 || strLen(%strC) > 255 || strLen(%strD) > 255)
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Action Failed","One of the data strings for Advanced settings is too large, and can't be sent over the network successfully.");
				commandToServer('PTG_SetUploadRelay',"Clear","","","","",%secKey,"");
				return;
			}
			
			commandToServer('PTG_SetUploadRelay',"Advanced",%strA,%strB,%strC,%strD,%secKey,%autoStart);
		
		//////////////////////////////////////////////////
		
		case "MassDetails":

			%relBiome = getWord(%detData,0);
			%relDetNum = getWord(%detData,1);
			
			switch$(%relBiome)
			{
				case "Default":
					%relBioList = "PTG_DetLst_TxtLst_DefBio";
					%ChkEnab = PTG_DetLst_ChkEnab_DefBio.getValue();
				case "Shore":
					%relBioList = "PTG_DetLst_TxtLst_ShoreBio";
					%ChkEnab = PTG_DetLst_ChkEnab_ShoreBio.getValue();
				case "SubMarine":
					%relBioList = "PTG_DetLst_TxtLst_SubMBio";
					%ChkEnab = PTG_DetLst_ChkEnab_SubMBio.getValue();
				case "CustomA":
					%relBioList = "PTG_DetLst_TxtLst_CustBioA";
					%ChkEnab = PTG_DetLst_ChkEnab_CustBioA.getValue();
				case "CustomB":
					%relBioList = "PTG_DetLst_TxtLst_CustBioB";
					%ChkEnab = PTG_DetLst_ChkEnab_CustBioB.getValue();
				case "CustomC":
					%relBioList = "PTG_DetLst_TxtLst_CustBioC";
					%ChkEnab = PTG_DetLst_ChkEnab_CustBioC.getValue();
				case "CaveTop":
					%relBioList = "PTG_DetLst_TxtLst_CaveTBio";
					%ChkEnab = PTG_DetLst_ChkEnab_CaveTBio.getValue();
				case "CaveBottom":
					%relBioList = "PTG_DetLst_TxtLst_CaveBBio";
					%ChkEnab = PTG_DetLst_ChkEnab_CaveBBio.getValue();
				case "Mountains":
					%relBioList = "PTG_DetLst_TxtLst_MntnBio";
					%ChkEnab = PTG_DetLst_ChkEnab_MntnBio.getValue();
				default:
					%relBioList = "PTG_DetLst_TxtLst_DefBio";
					%ChkEnab = PTG_DetLst_ChkEnab_DefBio.getValue();
			}
			
			
			//Skip biome if mess details are disabled for it
			if(%ChkEnab)
			{
				%relTxtLstCount = %relBioList.rowCount(); //for server: "scriptobject.detailListArray[%Number]???"
				
				for(%c = 0; %c < 56; %c += 14)
				{
					%relCount = (%c + 14);

					for(%d = 0; %d < 14; %d++)
					{
						%dMod = %d * 3; //(%relDetNum + %d + (%c * 14)) * 3;
						
						%testA = %relTxtLstCount > 0;
						%testB = %relCount < (%relTxtLstCount + 14); //%relTxtLstCount >= (%relCount + 14);
						%testC = %relCount <= 400;

						if(%relTxtLstCount > 0 && (%relDetNum + %relCount) < (%relTxtLstCount + 14) && %relCount <= 400) 
						{
							%rowText = %relBioList.getRowTextByID(%relDetNum + %d + %c); //???
							
							//make sure is object when uploading (i.e. brick != -1)
							switch(%c)
							{
								case 0:
									%strA = setWord(%strA,%dMod,getField(%rowText,6)); //Brick ID
									%strA = setWord(%strA,%dMod + 1,getField(%rowText,7)); //Print ID
									%strA = setWord(%strA,%dMod + 2,getField(%rowText,4)); //Color ID
								case 14:
									%strB = setWord(%strB,%dMod,getField(%rowText,6)); //Brick ID
									%strB = setWord(%strB,%dMod + 1,getField(%rowText,7)); //Print ID
									%strB = setWord(%strB,%dMod + 2,getField(%rowText,4)); //Color ID
								case 28:
									%strC = setWord(%strC,%dMod,getField(%rowText,6)); //Brick ID
									%strC = setWord(%strC,%dMod + 1,getField(%rowText,7)); //Print ID
									%strC = setWord(%strC,%dMod + 2,getField(%rowText,4)); //Color ID
								case 42:
									%strD = setWord(%strD,%dMod,getField(%rowText,6)); //Brick ID
									%strD = setWord(%strD,%dMod + 1,getField(%rowText,7)); //Print ID
									%strD = setWord(%strD,%dMod + 2,getField(%rowText,4)); //Color ID
							}
						}
						else
						{
							switch(%c)
							{
								case 0: %strA = "NULL";
								case 14: %strB = "NULL";
								case 28: %strC = "NULL";
								case 42: %strD = "NULL";
							}
						}
					}
				}
			}
			else
			{
				%strA = "NULL";
				%strB = "NULL";
				%strC = "NULL";
				%strD = "NULL";
			}
		
		
			if(strLen(%strA) > 255 || strLen(%strB) > 255 || strLen(%strC) > 255 || strLen(%strD) > 255)
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Action Failed","One of the data strings for Mass Biome Details is too large, and can't be sent over the network successfully.");
				commandToServer('PTG_SetUploadRelay',"Clear","","","","",%secKey,"");
				return;
			}
			
			commandToServer('PTG_SetUploadRelay',"MassDetails",%strA,%strB,%strC,%strD,%secKey,%autoStart);
		
		//////////////////////////////////////////////////
		
		case "Routines" or "RoutinesOnly": //Host Only
		
			%strA = PTG_Routines_BtnEnabStreams.getValue() SPC
			PTG_Routines_SldrStreamsTick.getValue() SPC
			PTG_Routines_ChkStreamsHostOnly.getValue() SPC
			PTG_Routines_ChkStreamsColl.getValue() SPC
			PTG_Routines_ChkStreamsClrDet.getValue() SPC
			PTG_Routines_SldrStreamsDist.getValue() SPC
			PTG_Routines_ChkGenStreamZones.getValue() SPC
			PTG_Routines_ChkFloatStreams.getValue() SPC

			PTG_Routines_SldrBrLmtBuffer.getValue() SPC
			PTG_Routines_SldrPingLmtBuffer.getValue() SPC
			PTG_Routines_SldrDedSrvrFuncBuffer.getValue() SPC
			PTG_Routines_SldrChObjLimitBuffer.getValue() SPC
			PTG_Routines_SldrChSavesSeedMax.getValue() SPC
			PTG_Routines_SldrChSavesTotalMax.getValue() SPC
			PTG_Routines_SldrBuildsMax.getValue() SPC 
			PTG_Routines_ChkDisBrBuffer.getValue() SPC
			PTG_Routines_ChkDisChBuffer.getValue() SPC
			PTG_Routines_ChkNormLagCheck.getValue() SPC
			PTG_Routines_ChkDedLagCheck.getValue();
			
			%strB = PTG_Routines_SldrPauseTick.getValue() SPC
			PTG_Routines_SldrAutoSaveDelay.getValue() SPC
			PTG_Routines_SldrPriGenDelay.getValue() SPC
			PTG_Routines_SldrSecGenDelay.getValue() SPC
			PTG_Routines_SldrPriCalcDelay.getValue() SPC
			PTG_Routines_SldrSecCalcDelay.getValue() SPC
			PTG_Routines_SldrBrGenDelay.getValue() SPC
			PTG_Routines_SldrBrRemDelay.getValue() SPC
			PTG_Routines_PopupGenSpeed.getSelected() SPC
			
			PTG_Routines_ChkFrcBrIntoChs.getValue() SPC
			PTG_Routines_ChkAutoCreateChunks.getValue() SPC
			PTG_Routines_ChkChEditBrPlant.getValue() SPC
			PTG_Routines_ChkChEditWrenchData.getValue() SPC
			PTG_Routines_ChkChEditBrPPD.getValue() SPC
			PTG_Routines_ChkChStcBrSpwnPlnt.getValue() SPC
			PTG_Routines_ChkLoadChFileStc.getValue() SPC
			PTG_Cmplx_PopUpChSave.getSelected() SPC
			PTG_Cmplx_PopUpChSaveExceed.getSelected() SPC
			
			PTG_Routines_ChkPublicBr.getValue() SPC 
			PTG_Routines_ChkDstryPublicBr.getValue() SPC
			PTG_Routines_ChkPublicBrPBL.getValue() SPC
			//"" SPC //PTG_Routines_ChkHideGhosting.getValue() //client-sided only (don't send to server)
			PTG_Routines_ChkEnabEchos.getValue() SPC
			PTG_Routines_ChkPrvntDstryDet.getValue() SPC
			PTG_Routines_ChkPrvntDstryTer.getValue() SPC
			PTG_Routines_ChkPrvntDstryBnds.getValue() SPC
			PTG_Routines_ChkNHBuildUpload.getValue() SPC
			PTG_Routines_ChkNHSetUpload.getValue() SPC
			PTG_Routines_ChkNHSrvrCmdEvntUse.getValue() SPC
			PTG_Routines_ChkAllowPlyrPosChk.getValue() SPC
			PTG_Routines_SldrFontSize.getValue() SPC
			PTG_Cmplx_SwChunkHLACol.colorID SPC 
			PTG_Cmplx_SwChunkHLBCol.colorID;
			
			
			if(strLen(%strA) > 255 || strLen(%strB) > 255)
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Action Failed","One of the data strings for Routine settings is too large, and can't be sent over the network successfully.");
				commandToServer('PTG_SetUploadRelay',"Clear","","","","",%secKey,"");
				return;
			}
			
			if(%pass $= "RoutinesOnly")
				commandToServer('PTG_SetUploadRelay',"RoutinesOnly",%strA,%strB,"","","",false);
			else
				commandToServer('PTG_SetUploadRelay',"Routines",%strA,%strB,"","",%secKey,%autoStart);
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function CLIENTCMDPTG_BuildUploadRelay(%stage,%data)
{
	switch$(%stage)
	{
		case "Stage1-Save": //save build data to array
		
			//send / save ownership data for file? (a build uploaded by a player may actually be owned by someone else)

			if(PTG_Complex.bldUpldRunning)
			{
				CLIENTCMDPTG_ReceiveMsg("Error","PTG: Build Upload Error","You are already uploading a save!");
				return;
			}
			
			PTG_Complex.bldUpldRunning = true;
			%buildName = getField(%data,1);
			%relColorID = 0; //???
			%fp = "saves/" @ %buildName @ ".bls";
			
			for(%c = 0; %c < strLen(%buildName); %c++)
				%str = %str @ getSubStr(%buildName,%c,1);			
			
			//Initial checks
			if(!isFile(%fp)) //don't include .bls extension
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Build Upload Failed","Brick save \"" @ %buildName @ ".bls\" doesn't exist!");
				CLIENTCMDPTG_BuildUploadRelay("Stage3-Clear",""); //???
				return;
			}
			if(%buildName $= "Fort Papa")
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Build Upload Failed","The brick save \"" @ %buildName @ ".bls\" is known to have distant, floating bricks, and thus is blocked from being uploaded. Please remove the bricks and retry under a different name.");
				CLIENTCMDPTG_BuildUploadRelay("Stage3-Clear",""); //???
				return;
			}
			
			%file = new FileObject();
			%file.openForRead(%fp);
			%file.readLine();
			%descLen = %file.readLine();
			%clrsMatch = true;
			
			//ignore description and colorset data (also check if colorsets match)
			for(%c = 0; %c < %descLen; %c++)
				%file.readLine();
			for(%c = 0; %c < 64; %c++)
			{
				%tmpCol = %tmpColArr[%c] = %file.readLine();
				
				if(%tmpCol !$= getColorIDTable(%c))
					%clrsMatch = false;
			}
			
			%xMin = 100000;
			%xMax = -100000;
			%yMin = 100000;
			%yMax = -100000;
			%zMin = 100000;
			%zMax = -100000;
			%count = 0;
			
			while(!%file.isEOF())
			{
				%readLn = %file.readLine();
				%frstWrd = firstWord(%readLn);
				
				//Data checks
				if(%count > 20100)
				{
					CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Build Upload Failed","Excessive amount of data for save file \"" @ %buildName @ ".bls\"; limit is 20,100 lines.");
					CLIENTCMDPTG_BuildUploadRelay("Stage3-Clear",""); //don't use force-clear %stage here (only during upload) because it could close progress wndw for other upload in progress (if trying to upload two builds at once)
					return;
				}
				if(strLen(%readLn) > 255)
				{
					if(%count == 0)
						%count = "0";
					
					CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Build Upload Failed","Data string on line \"" @ %count @ "\" for save file \"" @ %buildName @ ".bls\" is too large; limit is 255 characters.");
					CLIENTCMDPTG_BuildUploadRelay("Stage3-Clear","");
					return;
				}
				
				//Check brick count (on server-side too as additional precaution)
				if(%frstWrd $= "Linecount")
				{
					if((%brCount = getWord(%readLn,1)) >= 20000)
					{
						CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Build Upload Failed","Brick count for file \"" @ %buildName @ ".bls\" is too large! It must be under 20,000 bricks to be uploaded.");
						CLIENTCMDPTG_BuildUploadRelay("Stage3-Clear","");
						return;
					}
					else if(%brCount <= 0) //"<=" is a precaution, only "==" is necessary
					{
						CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Build Upload Failed","Brick count for file \"" @ %buildName @ ".bls\" doesn't contain any bricks!");
						CLIENTCMDPTG_BuildUploadRelay("Stage3-Clear","");
						return;
					}
					
					$PTG_TmpClBuildArr[%count] = %readLn;
					$PTG_TmpClBuildCount = %count++;
				}
				else
				{
					//Brick Data
					if(getSubStr(%readLn,0,2) !$= "+-")
					{
						%remPos = stripos(%readLn,"\"",0); //remove brick UI name (and space after)
						%strLen = strLen(%readLn);
						%newString = getSubStr(%readLn,%remPos + 2,%strLen);

						%brUIN = getSubStr(%readLn,0,%remPos);
						%brObj = PTG_GUI_GetRelObjID("BrickID-NoCat",%brUIN);
						
						//Setup new brick color id
						%brOldCol = getWord(%newString,5);
						%brNewCol = PTG_GUI_GetRelObjID("ColorID",%clrsMatch TAB %brOldCol TAB %tmpColArr[%brOldCol]);
						%newString = setWord(%newString,5,%brNewCol);
						
						if(%brObj != -1) // && isObject(%brObj)) ???
						{
							%brSzXh = %brObj.brickSizeX * 0.25; //* 0.5 * 0.5
							%brSzYh = %brObj.brickSizeY * 0.25;
							%brSzZh = %brObj.brickSizeZ * 0.1; //* 0.2 * 0.5
							%brPosX = getWord(%newString,0);
							%brPosY = getWord(%newString,1);
							%brPosZ = getWord(%newString,2);
							
							if(mAbs(%brPosX) > 100000 || mAbs(%brPosY) > 100000 || mAbs(%brPosZ) > 100000)
							{
								CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Build Upload Failed","Bricks are out of bounds! The position of bricks (within a save file you wish to upload) must be within 100,000 meters of the origin for X, Y & Z.");
								CLIENTCMDPTG_BuildUploadRelay("Stage3-Clear","");
								return;
							}
							if((%brPosZ - %brSzZh) < 0)
							{
								CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Build Upload Failed","Bricks must rest on or be above the ground in order for a save to be uploaded.");
								CLIENTCMDPTG_BuildUploadRelay("Stage3-Clear","");
								return;
							}
							
							//Bounds check
							if((%brPosX - %brSzXh) < %xMin)
								%xMin = (%brPosX - %brSzXh);
							if((%brPosX + %brSzXh) > %xMax)
								%xMax = (%brPosX + %brSzXh);
							if((%brPosY - %brSzYh) < %yMin)
								%yMin = (%brPosY - %brSzYh);
							if((%brPosY + %brSzYh) > %yMax)
								%yMax = (%brPosY + %brSzYh);
							if((%brPosZ - %brSzZh) < %zMin)
								%zMin = (%brPosZ - %brSzZh);
							if((%brPosZ + %brSzZh) > %zMax)
								%zMax = (%brPosZ + %brSzZh);
							
							if(mAbs(%xMax - %xMin) > 256 || mAbs(%yMax - %yMin) > 256  || mAbs(%zMax - %zMin) > 256)
							{
								CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Build Upload Failed","Save bounds is too large! Saves must fit within a 8x8x8 grid of 64x cubes in order to be uploaded.");
								CLIENTCMDPTG_BuildUploadRelay("Stage3-Clear","");
								return;
							}
							
							$PTG_TmpClBuildArr[%count] = "BRICKDATA" SPC %brObj SPC %newString; //make sure to convert brick obj ID to datablock name on server-side
							$PTG_TmpClBuildCount = %count++;
						}
						else if(!%PTG_TmpClBuildFailArr[%brUIN])
						{
							%PTG_TmpClBuildFailArr[%brUIN] = true;
							warn(">>PTG Save Upload Error: Datablock not found for brick \"" @ %brUIN @ "\"; excluded from converted build. [!]");
						}
					}
					
					//Other Data: Events, Owner, Brick-Name, etc.
					else
					{
						$PTG_TmpClBuildArr[%count] = %readLn;
						$PTG_TmpClBuildCount = %count++;
					}
				}
			}
			
			%file.close();
			%file.delete();
			
			//Upload progress window setup
			PTG_Cmplx_WndwBldUpldProg.visible = true;
			PTG_Cmplx_TxtBldUpldProg.setText("Upload Progress: (0%)");
			PTG_Cmplx_SwBldUpldProg.extent = "0 12";
			PTG_Cmplx_TxtBldConvProg.setText("Conversion Progress: (0%)");
			PTG_Cmplx_SwBldConvProg.extent = "0 12";
			
			deleteVariables("$PTG_TmpRefArr_Br*");
			deleteVariables("$PTG_TmpRefArr_Col*");
			commandToServer('PTG_BuildLoadSrvrFuncs',"InitializeBuildUpload",%data); //%data TAB %count
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		
		case "Stage2-GradUpload":

			if(%data < 20100)
			{
				if(%data < $PTG_TmpClBuildCount) //server hard-limit too
				{
					%perc = mFloatLength((%data / $PTG_TmpClBuildCount),2) * 240;
					%percAct = mFloatLength((%data / $PTG_TmpClBuildCount) * 100,2);
					PTG_Cmplx_TxtBldUpldProg.setText("Upload Progress: (" @ %percAct @ "%)");
					PTG_Cmplx_SwBldUpldProg.extent = %perc SPC 12;
					
					commandToServer('PTG_BuildLoadSrvrFuncs',"UploadBuildFile",$PTG_TmpClBuildArr[%data]); //scheduleNoQuota(33,0,
				}
				else
				{
					PTG_Cmplx_TxtBldUpldProg.setText("Upload Progress: (100%)");
					CLIENTCMDPTG_BuildUploadRelay("Stage3-Clear","");
					commandToServer('PTG_BuildLoadSrvrFuncs',"ConvertBuildFile","");
				}
			}
			else
			{
				commandToServer('PTG_BuildLoadSrvrFuncs',"CancelBuildUpload","");
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG: Build Upload Failed","Too many lines of data for save file \"" @ %buildName @ ".bls\"; limit is 20,100.");
				CLIENTCMDPTG_BuildUploadRelay("ForceClear","");
				
				return;
			}
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		
		case "Stage3-Clear" or "ForceClear":

			if(%stage $= "ForceClear")
				PTG_Cmplx_WndwBldUpldProg.visible = false;

			deleteVariables("$PTG_TmpClBuildArr*");
			deleteVariables("$PTG_TmpClBuildCount");
			PTG_Complex.bldUpldRunning = false;
	}
}


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


package PTG_ClientPackage
{
	function disconnect(%bool)
	{
		if(isObject(DatablockGroup) && DatablockGroup.getCount() > 0) //If locally connected only (prevent server shutdown while routine is running)
		{
			if($PTG.routine_isHalting)
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG ERROR: Routine Is Ending","Can't disconnect from the server while a PTG routine is ending; please wait for it to finish first.");
				return;
			}
			if($PTG.routine_Process !$= "None" && $PTG.routine_Process !$= "")
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG ERROR: Routine Still Running","Can't disconnect from the server while a PTG routine is still running; please stop the routine first or wait for it to finish.");
				return;
			}
			if($PTG.routine_ProcessAux !$= "None" && $PTG.routine_ProcessAux !$= "")
			{
				CLIENTCMDPTG_ReceiveMsg("Failed","PTG ERROR: Secondary Routine Still Running","Can't disconnect from the server while a PTG secondary routine is still running; please stop the routine first or wait for it to finish.");
				return;
			}
		}
		
		//Reset GUIs and vars on disconnect
		PTG_MainBrSelect.brIDSetup = false;
		PTG_PrintSelect.rcvdPrintData = false;
		PTG_ColorSelect.PaletteSetup = false;
		PTG_DetailBrSelect.DetBrListSetup = false;
		PTG_Complex.LoadDefault = false;
		PTG_Complex.LoadedRtnSave = false;
		PTG_Complex.prevRunning = false;
		PTG_Complex.prevHalting = false;
		PTG_Complex.bldUpldRunning = false;
		PTG_Cmplx_WndwBldUpldProg.visible = false; //???
		PTG_Cmplx_WndwBldListNotify.visible = false; //???
		PTG_Confirm.PrevSizeErrByPass = false;
		
		deleteVariables("$PTG_MaxPrints");
		deleteVariables("$PTG_PrintArr*");
		deleteVariables("$PTG_PermRefArr_Pri*");
		deleteVariables("$PTG_SrvHasPTGv3");
		deleteVariables("$PTG_TmpRefArr_Br*"); //just to be safe
		deleteVariables("$PTG_TmpRefArr_Col*"); //just to be safe
		deleteVariables("$PTG_MaxDatablocks");

		PTG_GUI_PrevCmds("Clear");
		CLIENTCMDPTG_BuildUploadRelay("Stage3-Clear","");
		
		return parent::disconnect(%bool);
	}
};
activatePackage(PTG_ClientPackage);