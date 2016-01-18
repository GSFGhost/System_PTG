if(!$PTGv3_KeyBind)
{
	$remapDivision[$remapCount] = "PTG";
	
	//Chunk Manager
	$remapName[$remapCount] = "Chunk Manager GUI";
	$remapCmd[$remapCount] = "PTG_ToggleGUI_ChunkManager";
	$remapCount++;
	
	//Simplex GUI
	$remapName[$remapCount] = "Simplex (Easy-To-Use) GUI";
	$remapCmd[$remapCount] = "PTG_ToggleGUI_SimplexGUI";
	$remapCount++;
	
	//Main Complex GUI
	$remapName[$remapCount] = "Complex (Main) GUI";
	$remapCmd[$remapCount] = "PTG_ToggleGUI_ComplexGUI";
	$remapCount++;
	
	//Overview GUI
	$remapName[$remapCount] = "Overview GUI";
	$remapCmd[$remapCount] = "PTG_ToggleGUI_OverviewGUI";
	$remapCount++;
	
	$PTGv3_KeyBind = true;
}

function PTG_ToggleGUI_ChunkManager(%toggle)
{
	if(%toggle) //specifies if keybind is pressed up or down (otherwise both true and false are sent, since keybind is pushed down then released)
	{
		if(isObject(PTG_ChunkManager))
		{
			if(PTG_ChunkManager.isAwake())
				canvas.popDialog(PTG_ChunkManager);
			else
			{
				if($PTG_SrvHasPTGv3)
					commandToServer('PTG_Request',"ChunkManagerGUI");
				else
					CLIENTCMDPTG_ReceiveMsg("Failed","PTG ERROR: GUI Open Failed","Either you have no spawned yet on the server, or the server is not running v3 of PTG.");
			}
		}
	}
}

function PTG_ToggleGUI_SimplexGUI(%toggle)
{
	if(%toggle)
	{
		if(isObject(PTG_Simplex))
		{
			if(PTG_Simplex.isAwake())
				canvas.popDialog(PTG_Simplex);
			else
			{
				if($PTG_SrvHasPTGv3)
					commandToServer('PTG_Request',"SimplexGUI");
				else
					CLIENTCMDPTG_ReceiveMsg("Failed","PTG ERROR: GUI Open Failed","Either you have no spawned yet on the server, or the server is not running v3 of PTG.");
			}
		}
	}
}

function PTG_ToggleGUI_ComplexGUI(%toggle)
{
	if(%toggle)
	{
		if(isObject(PTG_Complex))
		{
			if(PTG_Complex.isAwake())
				canvas.popDialog(PTG_Complex);
			else
			{
				if($PTG_SrvHasPTGv3)
					commandToServer('PTG_Request',"ComplexGUI");
				else
					CLIENTCMDPTG_ReceiveMsg("Failed","PTG ERROR: GUI Open Failed","Either you have no spawned yet on the server, or the server is not running v3 of PTG.");
			}
		}
	}
}

function PTG_ToggleGUI_OverviewGUI(%toggle)
{
	if(%toggle)
	{
		if(isObject(PTG_Overview))
		{
			if(PTG_Overview.isAwake())
				canvas.popDialog(PTG_Overview);
			else
				canvas.pushDialog(PTG_Overview);
		}
	}
}


//Setup Custom GUI Profiles
new GuiControlProfile(PTGCustomCheckProfile : ImpactCheckProfile)
{
	bitmap = "Add-Ons/System_PTG/GUIs/PTGRadio.png";
};

new GuiControlProfile(PTGCustomScrollProfile : ImpactScrollProfile) //for Preview window only
{
	bitmap = "Add-Ons/System_PTG/GUIs/PTGScroll.png";
};

new GuiControlProfile(PTGCustomPopUpMenuProfile : GUIPopUpMenuProfile)
{
	fontType = "Arial Bold";
	fontSize = "14";
	bitmap =  "base/client/ui/blockWindow";
};


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//// RETRIEVE BRICK IDs FOR CLIENT FOR BRICK SELECT GUI ////
function PTG_GUI_BrExistCheck()
{
	//Normal Bricks
	for(%c = 0; %c < PTG_BrSelect_SwNormBr.getCount(); %c++) //replace .getcount()
	{
		//Swatches
		%tmpSW = PTG_BrSelect_SwNormBr.getObject(%c);

		//Avoid Header-Swatches
		if(getSubStr(%tmpSw.getName(),0,13) $= "PTG_BrSelect_")
		{
			for(%d = 0; %d < %tmpSW.getCount(); %d++) //replace .getcount()
			{
				//Bitmaps
				%tmpBmp = %tmpSW.getObject(%d);

				for(%e = 0; %e < 2; %e++) //replace .getcount()
				{
					%tmpBtn = %tmpBmp.getObject(%e);

					//Get Button Only
					if(%tmpBtn.getClassName() $= "GuiBitmapButtonCtrl")
					{
						if((%brID = PTG_GUI_GetRelObjID("BrickID",%tmpBtn.NonLocalRef TAB %tmpBtn.LocalRef)) != -1)
							%tmpBtn.command = "PTG_GUI_BPCDSelect(\"Brick\"," @ %brID @ ",false);";
					}
				}
			}
		}
	}
	
	//ModTer Bricks
	for(%c = 0; %c < PTG_BrSelect_SwModTerBr.getCount(); %c++) //replace .getcount()
	{
		//Swatches
		%tmpSW = PTG_BrSelect_SwModTerBr.getObject(%c);

		//Avoid Header-Swatches
		if(getSubStr(%tmpSw.getName(),0,13) $= "PTG_BrSelect_")
		{
			for(%d = 0; %d < %tmpSW.getCount(); %d++) //replace .getcount()
			{
				//Bitmaps
				%tmpBmp = %tmpSW.getObject(%d);

				for(%e = 0; %e < 2; %e++) //replace .getcount()
				{
					%tmpBtn = %tmpBmp.getObject(%e);

					//Get Button Only
					if(%tmpBtn.getClassName() $= "GuiBitmapButtonCtrl")
					{
						if((%brID = PTG_GUI_GetRelObjID("BrickID",%tmpBtn.NonLocalRef TAB %tmpBtn.LocalRef)) != -1)
							%tmpBtn.command = "PTG_GUI_BPCDSelect(\"Brick\"," @ %brID @ ",true);";
					}
				}
			}
		}
	}
	
	deleteVariables("$PTG_TmpRefArr_Br*");
	PTG_MainBrSelect.brIDSetup = true;
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//// FIND IDs FOR BRICKS, PRINTS AND COLORS - FOR LOCAL AND NON-LOCAL CONNECTIONS ////
function PTG_GUI_GetRelObjID(%objType,%data)
{
	//Note: Datablock names can't be found for non-local connections, thus reference object IDs instead
	//Note: Make sure to delete brick and color array variables after function (that references this function) is finished - to prevent future issues

	switch$(%objType)
	{
		//////////////////////////////////////////////////
		
		case "BrickID":

			%uiName = getField(%data,0);
			%cat = getField(%data,1);
			%subCat = getField(%data,2);
			
			//Do quick lookup first (encase brick data was saved previously in temporary array)
			if((%tmpBr = $PTG_TmpRefArr_Br[%uiName,%cat,%subCat]) > 0 && %tmpBr.getClassName() $= "fxDTSBrickData") //if((%tmpBr = $PTG_TmpRefArr_Br[%tmpObj.uiName,%tmpObj.category,%tmpObj.subCategory]) > 0 && %tmpBr.getClassName() $= "fxDTSBrickData")
				return %tmpBr;
			
			//Local / non-local relative group setup
			if((%count = DatablockGroup.getCount()) > 0) //If local connection, no need to continue rest of script (since obj IDs can be referenced from datablock names themselves)]
			{
				%group = DatablockGroup; //local connection
				
				if(isObject(%obj = getField(%data,3)))
					return %obj.getID(); //Since player is locally connected, try accessing saved datablock name first from save. If that fails, do normal object check.
			}
			else
				%group = ServerConnection; //non-local connection

			if(isObject(%group) && %uiName !$= "" && %cat !$= "" && %subCat !$= "")
			{
				if($PTG_MaxDatablocks == 0)
					$PTG_MaxDatablocks = 99999;
				
				for(%c = 0; %c < %group.getCount() && $PTG_MaxDatablocks; %c++) //$PTG_MaxDatablocks prevents this For Loop from searching through newly created objects, thus it will only search through datablocks instead
				{
					%tmpObj = %group.getObject(%c);
					
					if(%tmpObj.getClassName() $= "fxDTSBrickData" && %tmpObj.category $= %cat && %tmpObj.subCategory $= %subCat)
					{
						if(%tmpObj.uiName $= %uiName)
						{
							$PTG_TmpRefArr_Br[%tmpObj.uiName,%tmpObj.category,%tmpObj.subCategory] = %tmpObj;
							return %tmpObj;
						}
					}
				}
			}

			return -1;
		
		//////////////////////////////////////////////////
		
		case "BrickID-NoCat": //lookup brick data with only uiname give (mainly for sending brick saves to server)

			//Do quick lookup first (encase brick data was saved previously in temporary array)
			if((%tmpBr = $PTG_TmpRefArr_Br[%uiName = %data]) > 0 && %tmpBr.getClassName() $= "fxDTSBrickData") //if(%uiName !$= "") ???
				return %tmpBr;
				
			//Local / non-local relative group setup
			if((%count = DatablockGroup.getCount()) > 0)
				%group = DatablockGroup; //local connection
			else
				%group = ServerConnection; //non-local connection

			if(isObject(%group) && %uiName !$= "")
			{
				if($PTG_MaxDatablocks == 0)
					$PTG_MaxDatablocks = 99999;
				
				for(%c = 0; %c < %group.getCount() && $PTG_MaxDatablocks; %c++) //$PTG_MaxDatablocks prevents this For Loop from searching through newly created objects, thus it will only search through datablocks instead
				{
					%tmpObj = %group.getObject(%c);
					
					if(%tmpObj.getClassName() $= "fxDTSBrickData" && %tmpObj.category !$= "" && %tmpObj.subCategory !$= "")
					{
						if(%tmpObj.uiName $= %uiName)
						{
							$PTG_TmpRefArr_Br[%tmpObj.uiName] = %tmpObj;
							return %tmpObj;
						}
					}
				}
			}

			return -1;
			
		//////////////////////////////////////////////////
		
		case "ColorID":
		
			%colorSetsMatch = getField(%data,0);
			%colorID = getField(%data,1);
			%colorStr = getField(%data,2);

			if(%colorSetsMatch)
				return %colorID;
			
			else
			{
				if($PTG_TmpRefArr_Col[%colorID] !$= "")
					return $PTG_TmpRefArr_Col[%colorID];
				
				//Find closest color and store value for temp future reference
				else
				{
					%resMin = 2.0;
					
					for(%c = 0; %c < 64; %c++)
					{
						%tmpColStr = getColorIDTable(%c);
						%tmpMin = vectorDist(%tmpColStr,%colorStr);
						%transCond = mabs(getWord(%tmpColStr, 3) - getWord(%colorStr, 3)) < 0.3;  //credit to SpaceGuy's script for this part; slightly modified
						
						if(%tmpMin < %resMin && %transCond)
						{
							%resMin = %tmpMin;
							
							if(%colorID != 65)
								%rtrnCol = $PTG_TmpRefArr_Col[%colorID] = %c;
							else
								%rtrnCol = %c;
						}
					}
					if(%c == 0 && %colorID != 65)
						%rtrnCol = $PTG_TmpRefArr_Col[%colorID] = "0";

					return %rtrnCol; //$PTG_TmpRefArr_Col[%colorID];
				}
			}
			
		//////////////////////////////////////////////////
		
		//case "ColorID-NoRef": //don't store or access values in array (since each color is checked only once - arrays in objType "ColorID" above are meant for preset-loading)

		//	%colorStr = %data;
		//	%rtnClr = %colorStr;
		//	%resMin = 2.0;
			
			//echo("ColorStart:" @ %colorStr);
			
			//Find closest color
		//	for(%c = 0; %c < 64; %c++)
		//	{
		//		%tmpColStr = getColorIDTable(%c);
		//		%tmpMin = vectorDist(%tmpColStr,%colorStr);
		//		%transCond = mabs(getWord(%tmpColStr, 3) - getWord(%colorStr, 3)) < 0.3;  //credit to SpaceGuy's script for this part; slightly modified
				
		//		if(%tmpMin < %resMin && %transCond)
		//		{
					//echo(%colorStr @ " -> " @ %tmpColStr);
		//			%rtnClr = %tmpColStr;
		//			%resMin = %tmpMin;
		//		}
		//	}				

		//	return %rtnClr; //returns color string, not id
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_onServerCheck()
{
	if((isObject(DatablockGroup) && DatablockGroup.getCount() > 0) || (isObject(ServerConnection) && ServerConnection.getCount() > 0))
		return true;
	else
		return false;
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUIObjsUpdate(%wndwExtX,%wndwExtY)
{
	if(PTG_Complex.isAwake()) //isObject(PTG_Complex) &&  //if((isObject(DatablockGroup) && DatablockGroup.getCount() > 0) || (isObject(ServerConnection) && ServerConnection.getCount() > 0))
	{
		%wndwCurrExt = PTG_Cmplx_MainWndw.extent;
		%wndwCurrExtX = getWord(%wndwCurrExt,0);
		%wndwCurrExtY = getWord(%wndwCurrExt,1);
			
		if(!PTG_Complex.btnResize) //prevents calling this function twice when the window is resized using the title bar button
		{
			if(%wndwCurrExtX != %wndwExtX || %wndwCurrExtY != %wndwExtY)
			{
				if(%wndwCurrExtX >= 558 && %wndwCurrExtY >= 680)
					PTG_GUI_SwitchWndw("ExpandBaux","ToggleSize");
				else
				{
					if(%wndwCurrExtX >= 503 && %wndwCurrExtY >= 612)
						PTG_GUI_SwitchWndw("ExpandAaux","ToggleSize");
					else
						PTG_GUI_SwitchWndw("RetractAux","ToggleSize");
				}
			}
		}
		else
			PTG_Complex.btnResize = false;
		
		schedule(1000,0,PTG_GUIObjsUpdate,%wndwCurrExtX,%wndwCurrExtY);
	}
	else
		PTG_Complex.PTGAutoObjUpdate = false;
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_OverviewGUI(%action)
{
	switch$(%action)
	{
		case "Check":

			if(!isFile("Config/Client/PTGv3/OVguiDisable.txt"))
			{
				canvas.pushDialog(PTG_Overview);
				PTG_Overview_ChkEnabInitShow.setValue(1);
			}
			
			PTG_Overview.initCheck = true;
		
		case "Update":
		
			//If closing Overview GUI and initial popup option is disabled, remember decision for next inital popup check
			if(!PTG_Overview_ChkEnabInitShow.getValue())
			{
				if(!isFile(%fp = "Config/Client/PTGv3/OVguiDisable.txt"))
				{
					%file = new FileObject();
					%file.openForWrite(%fp);
					%file.writeLine(">>This file prevents the Overview GUI from appearing on initial PTG GUI use.");
					
					%file.close();
					%file.delete();
				}
			}
			else
			{
				if(isFile(%fp = "Config/Client/PTGv3/OVguiDisable.txt"))
					fileDelete(%fp);
			}
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//// SETUP GUIs FOR SERVER ONWAKE, IF NOT ALREADY SETUP ////
function PTG_MainBrSelect::onWake()
{
	if(!PTG_MainBrSelect.brIDSetup && PTG_GUI_onServerCheck())
		PTG_GUI_BrExistCheck();
}

function PTG_DetailBrSelect::onWake()
{
	if(!PTG_DetailBrSelect.DetBrListSetup && PTG_GUI_onServerCheck())
		PTG_GUI_SetupDetBrickCat();
}

function PTG_ColorSelect::onWake()
{
	if(!PTG_ColorSelect.PaletteSetup && PTG_GUI_onServerCheck())
	{
		for(%rowY = 0; %rowY < 8; %rowY++)
		{
			for(%colX = 0; %colX < 8; %colX++)
			{
				%colStr = getColorIDtable(%colID);
				PTG_GUI_ColorPalette(%colX,%rowY,%colID,%colStr);
				%colID++;
			}
		}
	}
}

function PTG_Complex::onWake()
{
	if(!PTG_Complex.LoadedRtnSave && PTG_GUI_onServerCheck())
		PTG_GUI_SaveLoadRoutine("Load");
	
	//if(!PTG_Complex.LoadedPresetList) //loading preset list (in addition to routine settings and default preset) could cause client lag issues in extreme cases, so commented out
	//{
	//	PTG_GUI_PresetFuncs("List");
	//	PTG_Complex.LoadedPresetList = true;
	//}
			
	if(!PTG_Complex.LoadDefault && PTG_GUI_onServerCheck())
	{
		PTG_GUI_PresetFuncs("LoadDefault");
		PTG_Complex.LoadDefault = true;
	}
	
	if(!PTG_Complex.PTGAutoObjUpdate && PTG_GUI_onServerCheck())
	{
		PTG_Complex.PTGAutoObjUpdate = true;
		PTG_GUIObjsUpdate(getWord(PTG_Cmplx_MainWndw.extent,0),getWord(PTG_Cmplx_MainWndw.extent,1));
	}
	
	//PTG_ChMngr_EditSpamID.setText("Spammer ID"); //not necessary
	
	if(!PTG_Overview.initCheck)
		PTG_GUI_OverviewGUI("Check");
}

function PTG_Simplex::onWake()
{
	//Whether Complex or Simplex GUI is opened first by the user, default settings for both are loaded - if not previously set up
	
	if(!PTG_Complex.LoadedRtnSave && PTG_GUI_onServerCheck())
		PTG_GUI_SaveLoadRoutine("Load");
			
	if(!PTG_Complex.LoadDefault && PTG_GUI_onServerCheck())
	{
		PTG_GUI_PresetFuncs("LoadDefault");
		PTG_Complex.LoadDefault = true;
	}
	
	if(!PTG_Overview.initCheck)
		PTG_GUI_OverviewGUI("Check");
}

function PTG_ChunkManager::onWake()
{
	if(!PTG_Overview.initCheck)
		PTG_GUI_OverviewGUI("Check");
}

function PTG_Overview::onWake()
{
	if(!PTG_Overview.textSetup)
	{
		PTG_GUI_OverviewText();
		PTG_Overview.textSetup = true;
	}
	
	if(!PTG_Overview.initCheck)
		PTG_GUI_OverviewGUI("Check");
}