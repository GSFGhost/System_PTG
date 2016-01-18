PTG_Complex.prevRunning = false;
PTG_Complex.prevHalting = false;
PTG_Complex.prevLagChkTime = 0;
PTG_Complex.prevLagChkDiff = 0;
PTG_Complex.prevLagChkXPos = 0;
PTG_Complex.segSize = 0;

function PTG_GUI_PrevCmds(%command)
{
	//%wndwExtY = getWord(PTG_Cmplx_ScrlPrevWndw.extent,1);
	//%wndwExtX = getWord(PTG_Cmplx_SwLayerGroup.extent,0);
	//%wndwExtY = getWord(PTG_Cmplx_SwLayerGroup.extent,1);
	%wndwScrlExtX = getWord(PTG_Cmplx_ScrlPrevWndw.extent,0);
	%wndwScrlExtY = getWord(PTG_Cmplx_ScrlPrevWndw.extent,1);
	%ChSize = getWord("16 32 64 128 256",mClamp(PTG_Cmplx_PopupChSize.getSelected(),0,4));
	
	switch$(%command)
	{
		case "Start":

			//Check if preview routine is already running or currently ending
			if(PTG_Complex.prevRunning)
			{
				CLIENTCMDPTG_ReceiveMsg("Error","PTG Preview Error","A preview routine has already started.");
				return;
			}
			if(PTG_Complex.prevHalting)
			{
				CLIENTCMDPTG_ReceiveMsg("Error","PTG Preview Error","The preview routine is currently halting, please wait before starting a new preview.");
				return;
			}
			
			//Clear last preview
			PTG_Cmplx_SwPrevTopWndw_FltIsldD_B.clear();
			PTG_Cmplx_SwPrevTopWndw_FltIsldD_A.clear();
			PTG_Cmplx_SwPrevTopWndw_CldB.clear();
			PTG_Cmplx_SwPrevTopWndw_FltIsldC_B.clear();
			PTG_Cmplx_SwPrevTopWndw_FltIsldC_A.clear();
			PTG_Cmplx_SwPrevTopWndw_CaveC.clear();
			PTG_Cmplx_SwPrevTopWndw_TerB.clear();
			PTG_Cmplx_SwPrevTopWndw_CaveB.clear();
			PTG_Cmplx_SwPrevTopWndw_Wat.clear();
			PTG_Cmplx_SwPrevTopWndw_TerA.clear();
			PTG_Cmplx_SwPrevTopWndw_CaveA.clear();
			PTG_Cmplx_SwPrevTopWndw_CaveWat.clear();
			PTG_Cmplx_SwPrevTopWndw_FltIsldB_B.clear();
			PTG_Cmplx_SwPrevTopWndw_FltIsldB_A.clear();
			PTG_Cmplx_SwPrevTopWndw_CldA.clear();
			PTG_Cmplx_SwPrevTopWndw_FltIsldA_B.clear();
			PTG_Cmplx_SwPrevTopWndw_FltIsldA_A.clear();
			PTG_Cmplx_SwPrevTopWndw_ChGrid.clear();

			PTG_Cmplx_SwPrevBtmWndwB_LagEst.clear();
			PTG_Complex.prevLagChkTime = 0;
			PTG_Complex.prevLagChkDiff = 0;
			PTG_Complex.prevLagChkXPos = 0;
			PTG_Complex.segSize = 0;
			
			//Prevent large previews, unless allowed by player
			if(!PTG_Cmplx_BmpTerBr.BrickID || (PTG_Cmplx_ChkEnabClouds.getValue() && !PTG_Cmplx_BmpCloudBr.BrickID) || (PTG_Cmplx_ChkEnabFltIslds.getValue() && !PTG_Cmplx_BmpFltIsldsBr.BrickID))
			{
				CLIENTCMDPTG_ReceiveMsg("Error","PTG Preview Error","No brick has been set up for either terrain, clouds or floating islands; can't access brick size for preview.");
				return;
			}
			else
			{
				%ChSize = getWord("16 32 64 128 256",mClamp(PTG_Cmplx_PopupChSize.getSelected(),0,4));
				%terBrAm = mPow(%chSize / (getMax(PTG_Cmplx_BmpTerBr.BrickID.brickSizeX,2) / 2),2);
				
				if(PTG_Cmplx_ChkEnabClouds.getValue())
					%cldBrAm = mPow(%chSize / (getMax(PTG_Cmplx_BmpCloudBr.BrickID.brickSizeX,2) / 2),2);
				if(PTG_Cmplx_ChkEnabFltIslds.getValue())
					%fiBrAm = mPow(%chSize / (getMax(PTG_Cmplx_BmpFltIsldsBr.BrickID.brickSizeX,2) / 2),2);
				
				//find approximate amount of bricks per chunk for all chunks
				%chTotCountX = mFloor((PTG_Cmplx_EdtGridXEnd.getValue() - PTG_Cmplx_EdtGridXStart.getValue()) / %chSize);
				%chTotCountY = mFloor((PTG_Cmplx_EdtGridYEnd.getValue() - PTG_Cmplx_EdtGridYStart.getValue()) / %chSize);
				%brTotCount = (%terBrAm + %cldBrAm + %fiBrAm) * %chTotCountX * %chTotCountY;
				
				//If aprox brick count is > 10000 bricks, notify player of potentiality for lag (due to stress on the client to handle numerous GUI objects)
				if(%brTotCount > 10000 && !PTG_Confirm.PrevSizeErrByPass && !PTG_Confirm_ChkPrevSizeErrdontshow.getValue())
				{
					PTG_GUI_ConfirmMsg("PrevSizeErr");
					return;
				}
			}
			PTG_Confirm.PrevSizeErrByPass = false; //field for bypassing preview size lag-error above
			
			//Lag Graph Setup
			%parentObjW = getWord(PTG_Cmplx_ScrlPrevWndwB.extent,0) - 16;
				
			if(PTG_Cmplx_ChkEnabLagGraph.getValue())
			{
				%chCountX = mFloor(PTG_Cmplx_EdtGridXEnd.getValue() - PTG_Cmplx_EdtGridXStart.getValue()) / %chSize;
				%chCountY = mFloor(PTG_Cmplx_EdtGridYEnd.getValue() - PTG_Cmplx_EdtGridYStart.getValue()) / %chSize;
				%chTotCount = (%chCountX * %chCountY);
				
				if(%chTotCount > 42)
				{
					PTG_Complex.segSize = 10;
					%tmpAm = %chTotCount * 10;
					PTG_Cmplx_SwPrevBtmWndwB_LagEst.resize(0,0,%tmpAm,420);
					PTG_Cmplx_BmpPrevBtmWndwB_Bg.resize(0,0,%tmpAm,420);
				}
				else
				{
					PTG_Complex.segSize = mFloor(420 / %chTotCount);
					PTG_Cmplx_SwPrevBtmWndwB_LagEst.resize(0,0,%parentObjW,420);
					PTG_Cmplx_BmpPrevBtmWndwB_Bg.resize(0,0,%parentObjW,420);
				}
				
				PTG_Cmplx_ScrlPrevWndwB.scrollToBottom();
				PTG_Cmplx_BmpPrevBtmWndwB_BgTxt.resize(0,getWord(PTG_Cmplx_BmpPrevBtmWndwB_Bg.position,1),436,420);
				PTG_Complex.prevLagChkXPos -= PTG_Complex.segSize;
			}
			else
			{
				PTG_Cmplx_SwPrevBtmWndwB_LagEst.resize(0,0,%parentObjW,420);
				PTG_Cmplx_BmpPrevBtmWndwB_Bg.resize(0,0,%parentObjW,420);
				PTG_Cmplx_BmpPrevBtmWndwB_BgTxt.resize(0,getWord(PTG_Cmplx_BmpPrevBtmWndwB_Bg.position,1),436,420);
				PTG_Cmplx_ScrlPrevWndwB.scrollToBottom();
			}
			
			//Initialize
			PTG_Complex.prevRunning = true;
			PTG_Complex.prevHalting = false;

			//Error checks
			if(!PTG_GUI_ErrorCheck())
			{
				PTG_Complex.prevRunning = false;
				//if(PTG_Routines_ChkEnabEchos.getValue()) echo("\c2P\c1T\c4G \c2Client Preview Error: Error check for GUI settings failed; preview aborted.");
				
				return;
			}

			
			%startPosX = mFloor(PTG_Cmplx_EdtGridXStart.getValue() / %ChSize) * %ChSize;
			%startPosY = mFloor(PTG_Cmplx_EdtGridYStart.getValue() / %ChSize) * %ChSize;
			%endPosX = mFloor(PTG_Cmplx_EdtGridXEnd.getValue() / %ChSize) * %ChSize;
			%endPosY = mFloor(PTG_Cmplx_EdtGridYEnd.getValue() / %ChSize) * %ChSize;
			
			%gridSizeX = %endPosX - %startPosX;
			%gridSizeY = %endPosY - %startPosY;
			
			if(PTG_Cmplx_ChkRadialGrid.getValue()) //offset size of layers for X and Y for radial grids (takes top and right sides of chunk into account)
				%adjust = %ChSize;

			//Resize windows
			if(%gridSizeX > 420) //437
			{
				%wndwSzX = %gridSizeX + %adjust;
				PTG_Cmplx_ScrlPrevWndw.hScrollBar = "AlwaysOn";
			}
			else
			{
				%wndwSzX = 420;  //437
				PTG_Cmplx_ScrlPrevWndw.hScrollBar = "AlwaysOff";
			}
			if(%gridSizeY > %wndwExtY) //319
			{
				%wndwSzY = %gridSizeY + %adjust + 16;
				PTG_Cmplx_ScrlPrevWndw.vScrollBar = "AlwaysOn";
			}
			else
			{
				%wndwSzY = %wndwExtY + 16; //319
				PTG_Cmplx_ScrlPrevWndw.vScrollBar = "AlwaysOff";
			}
			
			//PTG_Cmplx_SwLayerGroup.resize(0,0,getMax(%wndwScrlExtX,%wndwSzX),getMax(%wndwScrlExtY,%wndwSzY));
			PTG_Cmplx_SwPrevTopWndw_FltIsldD_B.resize(0,0,%wndwSzX,%wndwSzY);
			PTG_Cmplx_SwPrevTopWndw_FltIsldD_A.resize(0,0,%wndwSzX,%wndwSzY);
			PTG_Cmplx_SwPrevTopWndw_CldB.resize(0,0,%wndwSzX,%wndwSzY);
			PTG_Cmplx_SwPrevTopWndw_FltIsldC_B.resize(0,0,%wndwSzX,%wndwSzY);
			PTG_Cmplx_SwPrevTopWndw_FltIsldC_A.resize(0,0,%wndwSzX,%wndwSzY);
			PTG_Cmplx_SwPrevTopWndw_CaveC.resize(0,0,%wndwSzX,%wndwSzY);
			PTG_Cmplx_SwPrevTopWndw_TerB.resize(0,0,%wndwSzX,%wndwSzY);
			PTG_Cmplx_SwPrevTopWndw_CaveB.resize(0,0,%wndwSzX,%wndwSzY);
			PTG_Cmplx_SwPrevTopWndw_Wat.resize(0,0,%wndwSzX,%wndwSzY);
			PTG_Cmplx_SwPrevTopWndw_TerA.resize(0,0,%wndwSzX,%wndwSzY);
			PTG_Cmplx_SwPrevTopWndw_CaveA.resize(0,0,%wndwSzX,%wndwSzY);
			PTG_Cmplx_SwPrevTopWndw_CaveWat.resize(0,0,%wndwSzX,%wndwSzY);
			PTG_Cmplx_SwPrevTopWndw_FltIsldB_B.resize(0,0,%wndwSzX,%wndwSzY);
			PTG_Cmplx_SwPrevTopWndw_FltIsldB_A.resize(0,0,%wndwSzX,%wndwSzY);
			PTG_Cmplx_SwPrevTopWndw_CldA.resize(0,0,%wndwSzX,%wndwSzY);
			PTG_Cmplx_SwPrevTopWndw_FltIsldA_B.resize(0,0,%wndwSzX,%wndwSzY);
			PTG_Cmplx_SwPrevTopWndw_FltIsldA_A.resize(0,0,%wndwSzX,%wndwSzY);
			PTG_Cmplx_SwPrevTopWndw_ChGrid.resize(0,0,%wndwSzX,%wndwSzY);
			PTG_Cmplx_ScrlPrevWndw.scrollToBottom(); //after resizing preview layers above, otherwise won't work
		
			//PTG_Cmplx_SwPrevTopWndw_Wat.setColor(PTG_Cmplx_SwBioDefWatCol.color);
			//if(PTG_Cmplx_ChkPrevGenFog.getValue())

			PTG_GUI_Routine_Append(0,0,0,0,0,"Next");
		
		////////////////////////////////////////////////////////////////////////////////
			
		case "Halt":
		
			if(!PTG_Complex.prevRunning)
			{
				CLIENTCMDPTG_ReceiveMsg("Error","PTG Preview Error","No preview routine is running; can't halt.");
				return;
			}
			if(PTG_Complex.prevHalting)
			{
				CLIENTCMDPTG_ReceiveMsg("Error","PTG Preview Error","The preview routine is already halting.");
				return;
			}
			
			//PTG_Complex.prevRunning = false;
			PTG_Complex.prevHalting = true;
		
		////////////////////////////////////////////////////////////////////////////////
		
		case "Clear":
		
			if(PTG_Complex.prevRunning)
			{
				CLIENTCMDPTG_ReceiveMsg("Error","PTG Preview Error","A preview routine is currently running, please halt it before clearing the preview.");
				return;
			}
			if(PTG_Complex.prevHalting)
			{
				CLIENTCMDPTG_ReceiveMsg("Error","PTG Preview Error","A preview routine is currently halting, please wait before clearing the preview.");
				return;
			}

			PTG_Cmplx_SwPrevTopWndw_FltIsldD_B.clear();
			PTG_Cmplx_SwPrevTopWndw_FltIsldD_A.clear();
			PTG_Cmplx_SwPrevTopWndw_CldB.clear();
			PTG_Cmplx_SwPrevTopWndw_FltIsldC_B.clear();
			PTG_Cmplx_SwPrevTopWndw_FltIsldC_A.clear();
			PTG_Cmplx_SwPrevTopWndw_CaveC.clear();
			PTG_Cmplx_SwPrevTopWndw_TerB.clear();
			PTG_Cmplx_SwPrevTopWndw_CaveB.clear();
			PTG_Cmplx_SwPrevTopWndw_Wat.clear();
			PTG_Cmplx_SwPrevTopWndw_TerA.clear();
			PTG_Cmplx_SwPrevTopWndw_CaveA.clear();
			PTG_Cmplx_SwPrevTopWndw_CaveWat.clear();
			PTG_Cmplx_SwPrevTopWndw_FltIsldB_B.clear();
			PTG_Cmplx_SwPrevTopWndw_FltIsldB_A.clear();
			PTG_Cmplx_SwPrevTopWndw_CldA.clear();
			PTG_Cmplx_SwPrevTopWndw_FltIsldA_B.clear();
			PTG_Cmplx_SwPrevTopWndw_FltIsldA_A.clear();
			PTG_Cmplx_SwPrevTopWndw_ChGrid.clear();
			
			PTG_Cmplx_SwPrevBtmWndwB_LagEst.clear();
			PTG_Complex.prevLagChkTime = 0;
			PTG_Complex.prevLagChkDiff = 0;
			PTG_Complex.prevLagChkXPos = 0;
			PTG_Complex.segSize = 0;
			
			%parentObjW = getWord(PTG_Cmplx_ScrlPrevWndwB.extent,0) - 16;
			
			PTG_Cmplx_SwPrevBtmWndwB_LagEst.resize(0,0,%parentObjW,420);
			PTG_Cmplx_BmpPrevBtmWndwB_Bg.resize(0,0,%parentObjW,420);
			PTG_Cmplx_BmpPrevBtmWndwB_BgTxt.resize(0,getWord(PTG_Cmplx_BmpPrevBtmWndwB_Bg.position,1),436,420);
			PTG_Cmplx_ScrlPrevWndwB.scrollToBottom();

			PTG_Cmplx_ScrlPrevWndw.hScrollBar = "AlwaysOff";
			PTG_Cmplx_ScrlPrevWndw.vScrollBar = "AlwaysOff";
			
			//PTG_Cmplx_SwLayerGroup.resize(0,0,%wndwScrlExtX,%wndwScrlExtY);
			PTG_Cmplx_SwPrevTopWndw_FltIsldD_B.resize(0,0,420,%wndwExtY);
			PTG_Cmplx_SwPrevTopWndw_FltIsldD_A.resize(0,0,420,%wndwExtY);
			PTG_Cmplx_SwPrevTopWndw_CldB.resize(0,0,420,%wndwExtY);
			PTG_Cmplx_SwPrevTopWndw_FltIsldC_B.resize(0,0,420,%wndwExtY);
			PTG_Cmplx_SwPrevTopWndw_FltIsldC_A.resize(0,0,420,%wndwExtY);
			PTG_Cmplx_SwPrevTopWndw_CaveC.resize(0,0,420,%wndwExtY);
			PTG_Cmplx_SwPrevTopWndw_TerB.resize(0,0,420,%wndwExtY); //437,319
			PTG_Cmplx_SwPrevTopWndw_CaveB.resize(0,0,420,%wndwExtY); //437,319
			PTG_Cmplx_SwPrevTopWndw_Wat.resize(0,0,420,%wndwExtY); //437,319
			PTG_Cmplx_SwPrevTopWndw_TerA.resize(0,0,420,%wndwExtY); //437,319
			PTG_Cmplx_SwPrevTopWndw_CaveA.resize(0,0,420,%wndwExtY); //437,319
			PTG_Cmplx_SwPrevTopWndw_CaveWat.resize(0,0,420,%wndwExtY);
			PTG_Cmplx_SwPrevTopWndw_FltIsldB_B.resize(0,0,420,%wndwExtY);
			PTG_Cmplx_SwPrevTopWndw_FltIsldB_A.resize(0,0,420,%wndwExtY);
			PTG_Cmplx_SwPrevTopWndw_CldA.resize(0,0,420,%wndwExtY);
			PTG_Cmplx_SwPrevTopWndw_FltIsldA_B.resize(0,0,420,%wndwExtY);
			PTG_Cmplx_SwPrevTopWndw_FltIsldA_A.resize(0,0,420,%wndwExtY);
			PTG_Cmplx_SwPrevTopWndw_ChGrid.resize(0,0,420,%wndwExtY); //437,319
			PTG_Cmplx_ScrlPrevWndw.scrollToBottom(); //after resizing preview layers above, otherwise won't work

		////////////////////////////////////////////////////////////////////////////////
		
		case "ToggleTer":
		
			PTG_Cmplx_SwPrevTopWndw_TerA.setVisible(%v = PTG_Cmplx_ChkTerLayer.getValue());
			PTG_Cmplx_SwPrevTopWndw_TerB.setVisible(%v);
			
		case "ToggleWat":
		
			PTG_Cmplx_SwPrevTopWndw_Wat.setVisible(%v = PTG_Cmplx_ChkWatLayer.getValue());
			PTG_Cmplx_SwPrevTopWndw_CaveWat.setVisible(%v);
			
		case "ToggleCaves":
		
			PTG_Cmplx_SwPrevTopWndw_CaveA.setVisible(%v = PTG_Cmplx_ChkCavesLayer.getValue());
			PTG_Cmplx_SwPrevTopWndw_CaveB.setVisible(%v);
			PTG_Cmplx_SwPrevTopWndw_CaveC.setVisible(%v);
			
		case "ToggleClds":
		
			PTG_Cmplx_SwPrevTopWndw_CldA.setVisible(%v = PTG_Cmplx_ChkCldsLayer.getValue()); //two layers encase > or < terrain layer
			PTG_Cmplx_SwPrevTopWndw_CldB.setVisible(%v);
			
		case "ToggleFltIsldsA":
		
			PTG_Cmplx_SwPrevTopWndw_FltIsldA_A.setVisible(%v = PTG_Cmplx_ChkFltIsldsLayerA.getValue()); //four layers (what if < clouds and or terrain?)
			PTG_Cmplx_SwPrevTopWndw_FltIsldB_A.setVisible(%v);
			PTG_Cmplx_SwPrevTopWndw_FltIsldC_A.setVisible(%v);
			PTG_Cmplx_SwPrevTopWndw_FltIsldD_A.setVisible(%v);
			
		case "ToggleFltIsldsB":
		
			PTG_Cmplx_SwPrevTopWndw_FltIsldA_B.setVisible(%v = PTG_Cmplx_ChkFltIsldsLayerB.getValue()); //four layers (what if < clouds and or terrain?)
			PTG_Cmplx_SwPrevTopWndw_FltIsldB_B.setVisible(%v);
			PTG_Cmplx_SwPrevTopWndw_FltIsldC_B.setVisible(%v);
			PTG_Cmplx_SwPrevTopWndw_FltIsldD_B.setVisible(%v);
			
		case "ToggleChGrid":
		
			PTG_Cmplx_SwPrevTopWndw_ChGrid.setVisible(%v = PTG_Cmplx_ChkChGridLayer.getValue()); //always in front
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_Routine_Append(%ChPosX,%ChPosY,%xmod,%ymod,%BrCount,%type)
{
	if(%type !$= "Delete")
	{
		%ChSize = getWord("16 32 64 128 256",mClamp(PTG_Cmplx_PopupChSize.getSelected(),0,4));
		%startPosX = mFloor(PTG_Cmplx_EdtGridXStart.getValue() / %ChSize) * %ChSize;
		%startPosY = mFloor(PTG_Cmplx_EdtGridYStart.getValue() / %ChSize) * %ChSize;
		%endPosX = mFloor(PTG_Cmplx_EdtGridXEnd.getValue() / %ChSize) * %ChSize;
		%endPosY = mFloor(PTG_Cmplx_EdtGridYEnd.getValue() / %ChSize) * %ChSize;
		
		//%BrXYSize = getMax(PTG_Cmplx_BmpTerBr.BrickID.brickSizeX * 0.5,1);

		%ChPosX = %startPosX + (%ChSize * %xmod);
		%ChPosY = %startPosY + (%ChSize * %ymod);// - %ChSize;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	switch$(%type)
	{
		case "Next":

			//Lag Graph (if enabled)
			if(PTG_Cmplx_ChkEnabLagGraph.getValue())
			{
				%currTime = mFloor(getSimTime());
				%prevTime = PTG_Complex.prevLagChkTime;
					if(%prevTime == 0) %prevTime = mClamp(%currTime,0,420);
				
				%currDiff = mClamp(mAbs(%currTime - %prevTime),0,420);
				%prevDiff = PTG_Complex.prevLagChkDiff;
				%resDiff = mClamp(mAbs(%currDiff - %prevDiff),0,420);
				
				PTG_Complex.prevLagChkTime = %currTime;
				PTG_Complex.prevLagChkDiff = %currDiff;
				
				
				//Reference highest point (last time or current) for segment color selection
				%lagMax = PTG_Cmplx_EdtPrevLagMax.getValue();
				%resInit = (getMax(%currDiff,%prevDiff) / %lagMax) * 100;
				%resMax = mFloatLength(%resInit / 10,0) * 10; //mClamp(mFloor(%resInit / 10) * 10,0,100);
					
				switch(%resMax)
				{
					case 0 or 10 or 20:
						%clr = "0 0 255 255"; //blue
					case 30 or 40:
						%clr = "0 255 0 255"; //green
					case 50 or 60:
						%clr = "255 255 0 255"; //yellow
					case 70 or 80:
						%clr = "220 220 0 255"; //dark yellow
					case 90 or 100:
						%clr = "255 0 0 255"; //red
					case 110 or 120:
						%clr = "220 0 0 255"; //dark red
					default:
						%clr = "180 0 0 255"; //darker red //%clr = "0 0 0 255"; //black(error)
				}
				
				
				//If past and present time are different
				if(%resDiff > 0)
				{
					%ext = PTG_Complex.segSize SPC %resDiff;
					%pos = (PTG_Complex.prevLagChkXPos += PTG_Complex.segSize) SPC (420-(getMax(%currDiff,%prevDiff)));
				
					if(%currDiff > %prevDiff)
						%bmp = "add-ons/system_ptg/guis/laggraph_inc";
					else if(%currDiff < %prevDiff)
						%bmp = "add-ons/system_ptg/guis/laggraph_dec";
				}
				
				//If past time and present time are the same (no difference)(don't use slope image if no slope exists - since slope is flat)
				else
				{
					%pos = (PTG_Complex.prevLagChkXPos += PTG_Complex.segSize) SPC (420-(%currDiff));
					%ext = PTG_Complex.segSize SPC (%currDiff);
					%bmp = "add-ons/system_ptg/guis/laggraph_full";
				}

				PTG_Cmplx_SwPrevBtmWndwB_LagEst.add(%bmpObj = PTG_GUI_GenGUIObj(%pos,%ext,%bmp,"BmpCtrl"));
				%bmpObj.mColor = %clr;
				
				//Bottom Rectangle (filler, if necessary)
				if(%prevDiff > 0 && %currDiff > 0 && %currDiff != %prevDiff)
				{
					%pos = PTG_Complex.prevLagChkXPos SPC mFloor(420-(getMin(%prevDiff,%currDiff)));
					%ext = PTG_Complex.segSize SPC (getMin(%prevDiff,%currDiff));
					%bmp = "add-ons/system_ptg/guis/laggraph_full";
					
					PTG_Cmplx_SwPrevBtmWndwB_LagEst.add(%bmpObj = PTG_GUI_GenGUIObj(%pos,%ext,%bmp,"BmpCtrl"));
					%bmpObj.mColor = %clr;
				}
			}

			//Brick Count - Estimate
			if(%BrCount == 0)
				%tmpBrCount = "0";
			else if(%BrCount > 999999)
				%tmpBrCount = "999999+";
			else
				%tmpBrCount = mFloatLength((%BrCount / 100),0) * 100; //1000
			
			PTG_Cmplx_TxtPrevBrTotal.setText("<font:arial bold:14>Total Bricks: <font:arial:14>~ " @ %tmpBrCount);
			
			//If Routine Halted
			if(PTG_Complex.prevHalting)
			{
				PTG_Complex.prevRunning = false;
				PTG_Complex.prevHalting = false;
				PTG_GUI_Routine_Append("","","","","","Delete");
				
				return;
			}
			
			//%startPosX = PTG_Cmplx_EdtGridXStart.getValue();
			//%startPosY = PTG_Cmplx_EdtGridYStart.getValue();
		
		
			//RADIAL GRID GEN
			if(PTG_Cmplx_ChkRadialGrid.getValue())
			{
				//Append chunk grid relative to player on X-axis
				if(%ChPosX > %endPosX)
				{
					%xmod = 0;
					%ChPosX = %startPosX + (%ChSize * %xmod);
					
					%ymod++;
					%ChPosY = %startPosY + (%ChSize * %ymod);	
					
					//Append chunk grid relative to player on Y-axis
					if(%ChPosY > %endPosY)
					{

						//%totalTime = (getSimTime() - $PTG.routine_StartMS) / 1000;
						PTG_Complex.prevRunning = false;
						PTG_Complex.prevHalting = false;
						PTG_GUI_Routine_Append("","","","","","Delete");
						
						return;
					}
				}

				%getminX = ((%endPosX-%startPosX) / 2);
				%getminY = ((%endPosY-%startPosY) / 2);
				%ChProxAux = getMin(%getMinX,%getMinY);
				%vd = VectorDist(%ChPosX SPC %ChPosY,%startPosX+%getminX SPC %startPosY+%getminY);

				if(%vd <= %ChProxAux)
				{
					%gridStartY = getWord(PTG_Cmplx_SwPrevTopWndw_ChGrid.extent,1);
					PTG_Cmplx_SwPrevTopWndw_ChGrid.add(PTG_GUI_GenGUIObj(%ChPosX-%startPosX SPC (%gridStartY-%ChPosY-%ChSize)+%startPosY,%ChSize SPC %ChSize,"","BmpCtrl"));
					
					//for development only
					//PTG_Cmplx_SwPrevTopWndw_ChGrid.add(PTG_GUI_GenGUIObj(%ChPosX-%startPosX SPC (%gridStartY-%ChPosY-%ChSize)+%startPosY,%ChSize SPC %ChSize,"0 255 0 128","Swatch"));
					//PTG_Cmplx_SwPrevTopWndw_ChGrid.add(PTG_GUI_GenGUIObj(%getminX-2 SPC %getminY-2,"4 4","255 0 0 255","Swatch"));

					schedule(0,0,PTG_GUI_Routine_Append,%ChPosX,%ChPosY,%xmod,%ymod,%BrCount,"Caves");
				}
				
				//for development only
				else
				{
					//%gridStartY = getWord(PTG_Cmplx_SwPrevTopWndw_ChGrid.extent,1);
					//PTG_Cmplx_SwPrevTopWndw_ChGrid.add(PTG_GUI_GenGUIObj(%ChPosX-%startPosX SPC (%gridStartY-%ChPosY-%ChSize)+%startPosY,%ChSize SPC %ChSize,"255 0 0 128","Swatch"));
					schedule(0,0,PTG_GUI_Routine_Append,%ChPosX,%ChPosY,%xmod++,%ymod,%BrCount,"Next");
				}
				
				return;
			}

			//SQUARE GRID GEN
			else
			{
				//Append chunk grid relative to player on X-axis
				if(%ChPosX >= %endPosX)
				{
					%xmod = 0;
					%ChPosX = %startPosX;
					
					%ymod++;
					%ChPosY = %startPosY + (%ChSize * %ymod);	
					
					//Append chunk grid relative to player on Y-axis
					if(%ChPosY >= %endPosY)
					{
						//%totalTime = (getSimTime() - $PTG.routine_StartMS) / 1000;
						PTG_Complex.prevRunning = false;
						PTG_Complex.prevHalting = false;
						PTG_GUI_Routine_Append("","","","","","Delete");

						return;	
					}
				}
				
				%gridStartY = getWord(PTG_Cmplx_SwPrevTopWndw_ChGrid.extent,1);
				PTG_Cmplx_SwPrevTopWndw_ChGrid.add(PTG_GUI_GenGUIObj(%ChPosX-%startPosX SPC %gridStartY+%startPosY-%ChPosY-%ChSize,%ChSize SPC %ChSize,"","BmpCtrl"));

				schedule(0,0,PTG_GUI_Routine_Append,%ChPosX,%ChPosY,%xmod,%ymod,%BrCount,"Caves");
				
			}
		
		//////////////////////////////////////////////////

		case "Caves":
		
			if(PTG_Cmplx_ChkEnabCaves.getValue() && PTG_Cmplx_PopUpTerType.getValue() !$= "SkyLands")
				schedule(0,0,PTG_GUI_Noise_Caves,%ChPosX,%ChPosY,%xmod,%ymod,0,0,%BrCount);
			else
				schedule(0,0,PTG_GUI_Routine_Append,%ChPosX,%ChPosY,%xmod,%ymod,%BrCount,"Biomes");
			
		case "Biomes":
			
			if(PTG_Cmplx_ChkEnabCustABio.getValue() || PTG_Cmplx_ChkEnabCustBBio.getValue() || PTG_Cmplx_ChkEnabCustCBio.getValue())
				schedule(0,0,PTG_GUI_Noise_CustomBiomes,%ChPosX,%ChPosY,%xmod,%ymod,0,0,%BrCount);
			else
				schedule(0,0,PTG_GUI_Routine_Append,%ChPosX,%ChPosY,%xmod,%ymod,%BrCount,"Mountains");
		
		case "Mountains":

			if(PTG_Cmplx_ChkEnabMntns.getValue())
				schedule(0,0,PTG_GUI_Noise_Mountains,%ChPosX,%ChPosY,%xmod,%ymod,0,0,%BrCount);
			else
				schedule(0,0,PTG_GUI_Routine_Append,%ChPosX,%ChPosY,%xmod,%ymod,%BrCount,"Terrain");
		
		case "Terrain":

			switch$(PTG_Cmplx_PopUpTerType.getValue())
			{
				case "Normal Terrain" or "FlatLands":
					schedule(0,0,PTG_GUI_Noise_Terrain,%ChPosX,%ChPosY,%xmod,%ymod,0,0,%BrCount);
				case "SkyLands":
					schedule(0,0,PTG_GUI_Noise_SkyLands,%ChPosX,%ChPosY,%xmod,%ymod,0,0,%BrCount);
				default:
					schedule(0,0,PTG_GUI_Routine_Append,%ChPosX,%ChPosY,%xmod,%ymod,%BrCount,"Water");
			}
		
		case "Water":

			if(PTG_Cmplx_SldrWaterLevel.getValue() > 0 && PTG_Cmplx_PopUpTerType.getValue() !$= "SkyLands" && !PTG_Cmplx_ChkDisWater.getValue())
				schedule(0,0,PTG_GUI_Chunk_Lakes,%ChPosX,%ChPosY,%xmod,%ymod,0,0,%BrCount);
			else
				schedule(0,0,PTG_GUI_Routine_Append,%ChPosX,%ChPosY,%xmod,%ymod,%BrCount,"Clouds");

		case "Clouds":

			if(PTG_Cmplx_ChkEnabClouds.getValue())
				schedule(0,0,PTG_GUI_Noise_Clouds,%ChPosX,%ChPosY,%xmod,%ymod,0,0,%BrCount);
			else
				schedule(0,0,PTG_GUI_Routine_Append,%ChPosX,%ChPosY,%xmod,%ymod,%BrCount,"FltIslds");
		
		case "FltIslds":

			if(PTG_Cmplx_ChkEnabFltIslds.getValue())
				schedule(0,0,PTG_GUI_Noise_FltIslds,%ChPosX,%ChPosY,%xmod,%ymod,0,0,%BrCount);
			else
			{
				PTG_GUI_Routine_Append("","","","","","Delete"); //
				schedule(0,0,PTG_GUI_Routine_Append,%ChPosX,%ChPosY,%xmod++,%ymod,%BrCount,"Next");
			}
		
		//////////////////////////////////////////////////
		
		case "Delete":

			//deleteVariables("$StrPrevArrayHV_SkyLands*");
			deleteVariables("$StrPrevArrayHV_CavesBtm*");
			deleteVariables("$StrPrevArrayHV_CavesTop*");
			deleteVariables("$StrPrevArrayHV_CustomBiomeA*");
			deleteVariables("$StrPrevArrayHV_CustomBiomeB*");
			deleteVariables("$StrPrevArrayHV_CustomBiomeC*");
			deleteVariables("$StrPrevArrayHV_Clouds*");
			deleteVariables("$StrPrevArrayHV_Mountains*");
			deleteVariables("$StrPrevArrayHV_FltIsldsATop*");
			deleteVariables("$StrPrevArrayHV_FltIsldsABtm*");
			deleteVariables("$StrPrevArrayHV_FltIsldsBTop*");
			deleteVariables("$StrPrevArrayHV_FltIsldsBBtm*");
			deleteVariables("$StrPrevArrayHV_SkylandsTop*");
			deleteVariables("$StrPrevArrayHV_SkylandsBtm*");
			
		//delete arrays before Next case; go to Next case also from floating islands function - if enabled
	}
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_Noise_Caves(%CHPosX,%CHPosY,%xmod,%ymod,%BrPosX,%BrPosY,%BrCount) //also Perlin!
{
	%BrXYSize = getMax(PTG_Cmplx_BmpTerBr.BrickID.brickSizeX * 0.5,1); //2x2 (or 1x1 meters) is the smallest brick size possible / allowed for previews
	%BrZSize = PTG_Cmplx_BmpTerBr.BrickID.brickSizeZ * 0.2;
	%ChSize = getWord("16 32 64 128 256",mClamp(PTG_Cmplx_PopupChSize.getSelected(),0,4));
	
	%CaveA_ItrA_XY = PTG_Cmplx_EdtNosScaleCaveAXY.getValue();
	%CaveA_ItrB_XY = PTG_Cmplx_EdtNosScaleCaveBXY.getValue();
	%CaveA_ItrC_XY = PTG_Cmplx_EdtNosScaleCaveCXY.getValue();
	%CaveA_ItrA_Z = PTG_Cmplx_EdtNosScaleCaveAZ.getValue();
	%CaveA_ItrB_Z = PTG_Cmplx_EdtNosScaleCaveBZ.getValue();
	%CaveA_ItrC_Z = PTG_Cmplx_EdtNosScaleCaveCZ.getValue();
	%CaveAOff_X = PTG_Cmplx_EdtCaveNosOffX.getValue();
	%CaveAOff_Y = PTG_Cmplx_EdtCaveNosOffY.getValue();
	%CaveAOff_Z = PTG_Cmplx_SldrCaveZOffset.getValue();
	%CaveSecZ = PTG_Cmplx_EdtSectCave.getValue();
	%CaveA_YStart = PTG_Cmplx_EdtEquatorCaveY.getValue();
	%ZMult = PTG_Cmplx_SldrCaveTopZMult.getValue();
	
	%CaveB_ItrA_XY = PTG_Cmplx_EdtNosScaleCaveHXY.getValue();
	%CaveB_ItrA_Z = PTG_Cmplx_EdtNosScaleCaveHZ.getValue();
	%CaveBOff_X = PTG_Cmplx_EdtCaveHNosOffX.getValue();
	%CaveBOff_Y = PTG_Cmplx_EdtCaveHNosOffY.getValue();
	
	//Caves Main Calc.
	
	%ChPxRelItrA = (mFloor(%CHPosX / %CaveA_ItrA_XY)) * %CaveA_ItrB_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / %CaveA_ItrA_XY)) * %CaveA_ItrB_XY;
	%ChHVItrA = PTG_GUI_RandNumGen_Chunk(%CHPxRelItrA+%CaveAOff_X,%CHPyRelItrA+%CaveAOff_Y,%CaveA_ItrB_XY, 110000223, 53781811, 35801, 72727);
	%ChPxActItrA = (mFloor(%CHPosX / %CaveA_ItrA_XY)) * %CaveA_ItrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / %CaveA_ItrA_XY)) * %CaveA_ItrA_XY;
	
	%ChHVItrA = PTG_GUI_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPyActItrA SPC %ChPyActItrA+%CaveA_ItrA_XY,%CaveA_YStart,99999);
	
	%CHPyRelItrB = (mFloor(%CHPosY / %CaveA_ItrB_XY)) * %CaveA_ItrB_XY;
	%ChPxRelItrB = (mFloor(%CHPosX / %CaveA_ItrB_XY)) * %CaveA_ItrB_XY;
	%ChHVItrB = PTG_GUI_RandNumGen_Chunk(%CHPxRelItrB+%CaveAOff_X,%CHPyRelItrB+%CaveAOff_Y,%CaveA_ItrB_XY, 110000223, 53781811, 35801, 72727);
	
	%ChHVItrB = PTG_GUI_Noise_PsuedoEquatorCheck(%ChHVItrB,%CHPyRelItrB SPC %CHPyRelItrB+%CaveA_ItrB_XY,%CaveA_YStart,99999);
	
	%ChPyRelItrC = (mFloor((%CHPosY + %BrPosY) / %CaveA_ItrC_XY)) * %CaveA_ItrB_XY;
	%ChPxRelItrC = (mFloor((%CHPosX + %BrPosX) / %CaveA_ItrC_XY)) * %CaveA_ItrB_XY;
	%ChHVItrC = PTG_GUI_RandNumGen_Chunk(%CHPxRelItrC+%CaveAOff_X,%CHPyRelItrC+%CaveAOff_Y,%CaveA_ItrB_XY, 110000223, 53781811, 35801, 72727);
	%BrPyActItrC = (mFloor(%BrPosY / %CaveA_ItrC_XY)) * %CaveA_ItrC_XY;
	%BrPxActItrC = (mFloor(%BrPosX / %CaveA_ItrC_XY)) * %CaveA_ItrC_XY;
	
	%ChPyRelItrCtmp = (mFloor((%CHPosY + %BrPosY) / %CaveA_ItrC_XY)) * %CaveA_ItrC_XY;
	%ChHVItrC = PTG_GUI_Noise_PsuedoEquatorCheck(%ChHVItrC,%ChPyRelItrCtmp SPC %ChPyRelItrCtmp+%CaveA_ItrC_XY,%CaveA_YStart,99999);

	%Co = ((%CHPosY - %ChPyActItrA) + %BrPosY) / %CaveA_ItrA_XY;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
	%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

	%Co = ((%CHPosY - %CHPyRelItrB) + %BrPosY) / %CaveA_ItrB_XY;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
	%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
	
	%Co = (%BrPosY - %BrPyActItrC) / %CaveA_ItrC_XY;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrCColL = getWord(%ChHVItrC,0) - ((getWord(%ChHVItrC,0) - getWord(%ChHVItrC,1)) * %Sm);
	%ItrCColR = getWord(%ChHVItrC,2) - ((getWord(%ChHVItrC,2) - getWord(%ChHVItrC,3)) * %Sm);
	
	%Co = ((%CHPosX - %ChPxActItrA) + %BrPosX) / %CaveA_ItrA_XY;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
	
	%Co = ((%CHPosX - %CHPxRelItrB) + %BrPosX) / %CaveA_ItrB_XY;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);
	
	%Co = (%BrPosX - %BrPxActItrC) / %CaveA_ItrC_XY;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrCRow = %ItrCColL - ((%ItrCColL - %ItrCColR) * %Sm);
	
	%relZ = (%ItrARow * %CaveA_ItrA_Z) + (%ItrBRow * %CaveA_ItrB_Z) + (%ItrCRow * %CaveA_ItrC_Z);
	%tempZ = mFloor(%relZ / %BrZSize) * %BrZSize;
	%CaveH = getMax(%tempZ,%BrZSize);
	
	//////////////////////////////////////////////////
	//Caves Height Mod
	
	%ChPxActItrAb = (mFloor(%CHPosX / %CaveB_ItrA_XY)) * %CaveB_ItrA_XY;
	%ChPyActItrAb = (mFloor(%CHPosY / %CaveB_ItrA_XY)) * %CaveB_ItrA_XY;
	%ChHVItrA_B = PTG_GUI_RandNumGen_Chunk(%ChPxActItrAb+%CaveBOff_X,%ChPyActItrAb+%CaveBOff_Y,%CaveB_ItrA_XY,15141163,77777377,17977,44741);
	
	%Co = ((%CHPosY-%ChPyActItrAb)+%BrPosY) / %CaveB_ItrA_XY;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrAColL = getWord(%ChHVItrA_B,0) - ((getWord(%ChHVItrA_B,0) - getWord(%ChHVItrA_B,1)) * %Sm);
	%ItrAColR = getWord(%ChHVItrA_B,2) - ((getWord(%ChHVItrA_B,2) - getWord(%ChHVItrA_B,3)) * %Sm);
	
	%Co = ((%CHPosX-%ChPxActItrAb)+%BrPosX) / %CaveB_ItrA_XY;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
	
	%relZ = (%ItrARow * %CaveB_ItrA_Z);
	%tempZ = mFloor(%relZ / %BrZSize) * %BrZSize;
	%CaveH_Mod = getMax(%tempZ,%BrZSize);
	
	if(%CaveH < %CaveSecZ)
	{
		$StrPrevArrayHV_CavesBtm[%BrPosX,%BrPosY] = mFloor(((%CaveAOff_Z - (%CaveSecZ - %CaveH)) + %CaveH_Mod) / %BrZSize) * %BrZSize;
		$StrPrevArrayHV_CavesTop[%BrPosX,%BrPosY] = mFloor((%CaveAOff_Z + ((%CaveSecZ - %CaveH) * %ZMult) + %CaveH_Mod) / %BrZSize) * %BrZSize;
	}
	else
	{
		$StrPrevArrayHV_CavesBtm[%BrPosX,%BrPosY] = 0;
		$StrPrevArrayHV_CavesTop[%BrPosX,%BrPosY] = 0;
	}
	
	//////////////////////////////////////////////////
	
	if((%BrPosX += %BrXYSize) >= %ChSize)
	{
		%BrPosX = 0; //%BrPosX = %BrXYSize / 2;
		
		if((%BrPosY += %BrXYSize) >= %ChSize)
		{
			schedule(0,0,PTG_GUI_Routine_Append,%ChPosX,%ChPosY,%xmod,%ymod,%BrCount,"Biomes");
			return;
		}
	}

	schedule(0,0,PTG_GUI_Noise_Caves,%CHPosX,%CHPosY,%xmod,%ymod,%BrPosX,%BrPosY,%BrCount);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_Noise_CustomBiomes(%CHPosX,%CHPosY,%xmod,%ymod,%BrPosX,%BrPosY,%BrCount)
{
	%BrXYSize = getMax(PTG_Cmplx_BmpTerBr.BrickID.brickSizeX * 0.5,1);
	%BrZSize = PTG_Cmplx_BmpTerBr.BrickID.brickSizeZ * 0.2;
	%ChSize = getWord("16 32 64 128 256",mClamp(PTG_Cmplx_PopupChSize.getSelected(),0,4));
	
	//Custom Biome A
	if(PTG_Cmplx_ChkEnabCustABio.getValue())
	{
		%BioA_ItrA_XY = PTG_Cmplx_EdtNosScaleCustAXY.getValue();
		%BioA_ItrA_Z = PTG_Cmplx_EdtNosScaleCustAZ.getValue();
		%BioAOff_X = PTG_Cmplx_EdtCustANosOffX.getValue();
		%BioAOff_Y = PTG_Cmplx_EdtCustANosOffY.getValue();
		//%BioASecZ = PTG_Cmplx_EdtSectCustA.getValue();
		%BioA_YStart = PTG_Cmplx_EdtEquatorCustAY.getValue();
		
		%ChPxRelItrA = (mFloor(%CHPosX / %BioA_ItrA_XY)) * %BioA_ItrA_XY;
		%ChPyRelItrA = (mFloor(%CHPosY / %BioA_ItrA_XY)) * %BioA_ItrA_XY;
		%ChHVItrA = PTG_GUI_RandNumGen_Chunk(%CHPxRelItrA+%BioAOff_X,%CHPyRelItrA+%BioAOff_Y,%BioA_ItrA_XY,12192683,83059231,10007,54973);
		
		%ChPosStr = %ChPyRelItrA SPC %ChPyRelItrA+%BioA_ItrA_XY;
		%ChHVItrA = PTG_GUI_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPosStr,%BioA_YStart,99999);
		
		%Co = ((%CHPosY-%ChPyRelItrA)+%BrPosY) / %BioA_ItrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);
		
		%Co = ((%CHPosX-%ChPxRelItrA)+%BrPosX) / %BioA_ItrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
		
		$StrPrevArrayHV_CustomBiomeA[%BrPosX,%BrPosY] = %ItrARow * %BioA_ItrA_Z;
	}
	
	//Custom Biome B
	if(PTG_Cmplx_ChkEnabCustBBio.getValue())
	{
		%BioB_ItrA_XY = PTG_Cmplx_EdtNosScaleCustBXY.getValue();
		%BioB_ItrA_Z = PTG_Cmplx_EdtNosScaleCustBZ.getValue();
		%BioBOff_X = PTG_Cmplx_EdtCustBNosOffX.getValue();
		%BioBOff_Y = PTG_Cmplx_EdtCustBNosOffY.getValue();
		//%BioBSecZ = PTG_Cmplx_EdtSectCustB.getValue();
		%BioB_YStart = PTG_Cmplx_EdtEquatorCustBY.getValue();
		
		%ChPxRelItrA = (mFloor(%CHPosX / %BioB_ItrA_XY)) * %BioB_ItrA_XY;
		%ChPyRelItrA = (mFloor(%CHPosY / %BioB_ItrA_XY)) * %BioB_ItrA_XY;
		%ChHVItrA = PTG_GUI_RandNumGen_Chunk(%CHPxRelItrA+%BioBOff_X,%CHPyRelItrA+%BioBOff_Y,%BioB_ItrA_XY,13233757,55555333,21397,50153);
		
		%ChPosStr = %ChPyRelItrA SPC %ChPyRelItrA+%BioB_ItrA_XY;
		%ChHVItrA = PTG_GUI_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPosStr,%BioB_YStart,99999);
		
		%Co = ((%CHPosY-%ChPyRelItrA)+%BrPosY) / %BioB_ItrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);
		
		%Co = ((%CHPosX-%ChPxRelItrA)+%BrPosX) / %BioB_ItrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
		
		$StrPrevArrayHV_CustomBiomeB[%BrPosX,%BrPosY] = %ItrARow * %BioB_ItrA_Z;
	}
	
	//Custom Biome C
	if(PTG_Cmplx_ChkEnabCustCBio.getValue())
	{
		%BioC_ItrA_XY = PTG_Cmplx_EdtNosScaleCustCXY.getValue();
		%BioC_ItrA_Z = PTG_Cmplx_EdtNosScaleCustCZ.getValue();
		%BioCOff_X = PTG_Cmplx_EdtCustCNosOffX.getValue();
		%BioCOff_Y = PTG_Cmplx_EdtCustCNosOffY.getValue();
		//%BioCSecZ = PTG_Cmplx_EdtSectCustC.getValue();
		%BioC_YStart = PTG_Cmplx_EdtEquatorCustCY.getValue();
		
		%ChPxRelItrA = (mFloor(%CHPosX / %BioC_ItrA_XY)) * %BioC_ItrA_XY;
		%ChPyRelItrA = (mFloor(%CHPosY / %BioC_ItrA_XY)) * %BioC_ItrA_XY;
		%ChHVItrA = PTG_GUI_RandNumGen_Chunk(%CHPxRelItrA+%BioCOff_X,%CHPyRelItrA+%BioCOff_Y,%BioC_ItrA_XY,14151617,71111111,32831,80557);
		
		%ChPosStr = %ChPyRelItrA SPC %ChPyRelItrA+%BioC_ItrA_XY;
		%ChHVItrA = PTG_GUI_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPosStr,%BioC_YStart,99999);
		
		%Co = ((%CHPosY-%ChPyRelItrA)+%BrPosY) / %BioC_ItrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);
		
		%Co = ((%CHPosX-%ChPxRelItrA)+%BrPosX) / %BioC_ItrA_XY;
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
		
		$StrPrevArrayHV_CustomBiomeC[%BrPosX,%BrPosY] = %ItrARow * %BioC_ItrA_Z;
	}
	
	//////////////////////////////////////////////////
	
	if((%BrPosX += %BrXYSize) >= %ChSize)
	{
		%BrPosX = 0;
		
		if((%BrPosY += %BrXYSize) >= %ChSize)
		{
			schedule(0,0,PTG_GUI_Routine_Append,%ChPosX,%ChPosY,%xmod,%ymod,%BrCount,"Mountains");
			return;
		}
	}

	schedule(0,0,PTG_GUI_Noise_CustomBiomes,%CHPosX,%CHPosY,%xmod,%ymod,%BrPosX,%BrPosY,%BrCount);
	
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_Noise_Mountains(%CHPosX,%CHPosY,%xmod,%ymod,%BrPosX,%BrPosY,%BrCount)
{
	%BrXYSize = getMax(PTG_Cmplx_BmpTerBr.BrickID.brickSizeX * 0.5,1);
	%BrZSize = PTG_Cmplx_BmpTerBr.BrickID.brickSizeZ * 0.2;
	%ChSize = getWord("16 32 64 128 256",mClamp(PTG_Cmplx_PopupChSize.getSelected(),0,4));
	//%Level_Water = PTG_Cmplx_SldrWaterLevel.getValue();
	//%Level_CnctLakes = PTG_Cmplx_SldrCnctLakesStrt.getValue();
	
	%Mntn_ItrA_XY = PTG_Cmplx_EdtNosScaleMntnAXY.getValue();
	%Mntn_ItrB_XY = PTG_Cmplx_EdtNosScaleMntnBXY.getValue();
	%Mntn_ItrA_Z = PTG_Cmplx_EdtNosScaleMntnAZ.getValue();
	%Mntn_ItrB_Z = PTG_Cmplx_EdtNosScaleMntnBZ.getValue();
	%MntnOff_X = PTG_Cmplx_EdtMntnNosOffX.getValue();
	%MntnOff_Y = PTG_Cmplx_EdtMntnNosOffY.getValue();
	%Mntn_YStart = PTG_Cmplx_EdtEquatorMntnY.getValue();
	
	%ChPxRelItrA = (mFloor(%CHPosX / %Mntn_ItrA_XY)) * %Mntn_ItrB_XY;
	%ChPyRelItrA = (mFloor(%CHPosY / %Mntn_ItrA_XY)) * %Mntn_ItrB_XY;
	%ChHVItrA = PTG_GUI_RandNumGen_Chunk(%CHPxRelItrA+%MntnOff_X,%CHPyRelItrA+%MntnOff_Y,%Mntn_ItrB_XY,27177289,71234567,14549,54163);
	%ChPxActItrA = (mFloor(%CHPosX / %Mntn_ItrA_XY)) * %Mntn_ItrA_XY;
	%ChPyActItrA = (mFloor(%CHPosY / %Mntn_ItrA_XY)) * %Mntn_ItrA_XY;

	%ChPosStr = %ChPyActItrA SPC %ChPyActItrA+%Mntn_ItrA_XY;
	%ChHVItrA = PTG_GUI_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPosStr,%Mntn_YStart,0);
	
	%CHPyRelItrB = (mFloor(%CHPosY / %Mntn_ItrB_XY)) * %Mntn_ItrB_XY;
	%ChPxRelItrB = (mFloor(%CHPosX / %Mntn_ItrB_XY)) * %Mntn_ItrB_XY;
	%ChHVItrB = PTG_GUI_RandNumGen_Chunk(%CHPxRelItrB+%MntnOff_X,%CHPyRelItrB+%MntnOff_Y,%Mntn_ItrB_XY,27177289,71234567,14549,54163);
	
	%ChPosStr = %CHPyRelItrB SPC %CHPyRelItrB+%Mntn_ItrB_XY;
	%ChHVItrB = PTG_GUI_Noise_PsuedoEquatorCheck(%ChHVItrB,%ChPosStr,%Mntn_YStart,0);
	
	//for(%BrPosY = 0; %BrPosY < $PTGm.chSize; %BrPosY += $PTGm.brTer_XYsize)
	//{
	//	for(%BrPosX = 0; %BrPosX < $PTGm.chSize; %BrPosX += $PTGm.brTer_XYsize)
	//	{			
			%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / %Mntn_ItrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
			%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

			%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY) / %Mntn_ItrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
			%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
			
			%Co = ((%CHPosX-%ChPxActItrA)+%BrPosX) / %Mntn_ItrA_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) *%Sm);
			
			%Co = ((%CHPosX-%CHPxRelItrB)+%BrPosX) / %Mntn_ItrB_XY;
			%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
			%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

			%relZ = (%ItrARow * %Mntn_ItrA_Z) + (%ItrBRow * %Mntn_ItrB_Z);
			$StrPrevArrayHV_Mountains[%BrPosX,%BrPosY] = getMax(%relZ,%BrZSize);
	//	}
	//}
	
	//////////////////////////////////////////////////
	
	if((%BrPosX += %BrXYSize) >= %ChSize)
	{
		%BrPosX = 0; //%BrPosX = %BrXYSize / 2;
		
		if((%BrPosY += %BrXYSize) >= %ChSize)
		{
			schedule(0,0,PTG_GUI_Routine_Append,%ChPosX,%ChPosY,%xmod,%ymod,%BrCount,"Terrain");
			return;
		}
	}

	schedule(0,0,PTG_GUI_Noise_Mountains,%CHPosX,%CHPosY,%xmod,%ymod,%BrPosX,%BrPosY,%BrCount);
	
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_Noise_Terrain(%CHPosX,%CHPosY,%xmod,%ymod,%BrPosX,%BrPosY,%BrCount)
{
	%BrXYSize = getMax(PTG_Cmplx_BmpTerBr.BrickID.brickSizeX * 0.5,1); //can access brick size when not locally connected?
	%BrZSize = PTG_Cmplx_BmpTerBr.BrickID.brickSizeZ * 0.2;
	%ChSize = getWord("16 32 64 128 256",mClamp(PTG_Cmplx_PopupChSize.getSelected(),0,4));
	%Level_Water = PTG_Cmplx_SldrWaterLevel.getValue();
	%Level_CnctLakes = PTG_Cmplx_SldrCnctLakesStrt.getValue();
	%startPosX = PTG_Cmplx_EdtGridXStart.getValue();
	%startPosY = PTG_Cmplx_EdtGridYStart.getValue();
	
	%Ter_ItrA_XY = PTG_Cmplx_EdtNosScaleTerAXY.getValue();
	%Ter_ItrB_XY = PTG_Cmplx_EdtNosScaleTerBXY.getValue();
	%Ter_ItrC_XY = PTG_Cmplx_EdtNosScaleTerCXY.getValue();
	%Ter_ItrA_Z = PTG_Cmplx_EdtNosScaleTerAZ.getValue();
	%Ter_ItrB_Z = PTG_Cmplx_EdtNosScaleTerBZ.getValue();
	%Ter_ItrC_Z = PTG_Cmplx_EdtNosScaleTerCZ.getValue();
	%TerOff_X = PTG_Cmplx_EdtTerNosOffX.getValue();
	%TerOff_Y = PTG_Cmplx_EdtTerNosOffY.getValue();
	%TerOff_Z = PTG_Cmplx_SldrTerZOffset.getValue();

	%ChPxRelItrA = (mFloor(%ChPosX / %Ter_ItrA_XY)) * %Ter_ItrB_XY;
	%ChPyRelItrA = (mFloor(%ChPosY / %Ter_ItrA_XY)) * %Ter_ItrB_XY;
	%ChHVItrA = PTG_GUI_RandNumGen_Chunk(%CHPxRelItrA+%TerOff_X,%CHPyRelItrA+%TerOff_Y,%Ter_ItrB_XY,13333227,49979687,13313,15551);
	%ChPxActItrA = (mFloor(%ChPosX / %Ter_ItrA_XY)) * %Ter_ItrA_XY;
	%ChPyActItrA = (mFloor(%ChPosY / %Ter_ItrA_XY)) * %Ter_ItrA_XY;

	%ChHVItrAmod = %ChHVItrA;
	if(PTG_Cmplx_ChkEnabCustABio.getValue()) //Custom Biome A
	{
		%bioCheckStr = PTG_GUI_Noise_BiomeCheck(%ChPosX,%ChPosY,$PTGm.bio_CustA_itrA_XY,$PTGm.bio_CustA_itrA_Z,$PTGm.bio_CustAOff_X,$PTGm.bio_CustAOff_Y,$PTGm.Bio_CustA_YStart);
		
		if((getWord(%bioCheckStr,0)) < PTG_Cmplx_EdtSectCustA.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * PTG_Cmplx_SldrTerHMod_CustA.getValue());
		if((getWord(%bioCheckStr,2)) < PTG_Cmplx_EdtSectCustA.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * PTG_Cmplx_SldrTerHMod_CustA.getValue());
		if((getWord(%bioCheckStr,1)) < PTG_Cmplx_EdtSectCustA.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * PTG_Cmplx_SldrTerHMod_CustA.getValue());
		if((getWord(%bioCheckStr,3)) < PTG_Cmplx_EdtSectCustA.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * PTG_Cmplx_SldrTerHMod_CustA.getValue());
	}
	if(PTG_Cmplx_ChkEnabCustBBio.getValue()) //Custom Biome B
	{
		%bioCheckStr = PTG_GUI_Noise_BiomeCheck(%ChPosX,%ChPosY,$PTGm.bio_CustB_itrA_XY,$PTGm.bio_CustB_itrA_Z,$PTGm.bio_CustBOff_X,$PTGm.bio_CustBOff_Y,$PTGm.Bio_CustB_YStart);
		
		if((getWord(%bioCheckStr,0)) < PTG_Cmplx_EdtSectCustB.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * PTG_Cmplx_SldrTerHMod_CustB.getValue());
		if((getWord(%bioCheckStr,2)) < PTG_Cmplx_EdtSectCustB.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * PTG_Cmplx_SldrTerHMod_CustB.getValue());
		if((getWord(%bioCheckStr,1)) < PTG_Cmplx_EdtSectCustB.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * PTG_Cmplx_SldrTerHMod_CustB.getValue());
		if((getWord(%bioCheckStr,3)) < PTG_Cmplx_EdtSectCustB.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * PTG_Cmplx_SldrTerHMod_CustB.getValue());
	}
	if(PTG_Cmplx_ChkEnabCustCBio.getValue()) //Custom Biome C
	{
		%bioCheckStr = PTG_GUI_Noise_BiomeCheck(%ChPosX,%ChPosY,$PTGm.bio_CustC_itrA_XY,$PTGm.bio_CustC_itrA_Z,$PTGm.bio_CustCOff_X,$PTGm.bio_CustCOff_Y,$PTGm.Bio_CustC_YStart);
		
		if((getWord(%bioCheckStr,0)) < PTG_Cmplx_EdtSectCustC.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * PTG_Cmplx_SldrTerHMod_CustC.getValue());
		if((getWord(%bioCheckStr,2)) < PTG_Cmplx_EdtSectCustC.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * PTG_Cmplx_SldrTerHMod_CustC.getValue());
		if((getWord(%bioCheckStr,1)) < PTG_Cmplx_EdtSectCustC.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * PTG_Cmplx_SldrTerHMod_CustC.getValue());
		if((getWord(%bioCheckStr,3)) < PTG_Cmplx_EdtSectCustC.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * PTG_Cmplx_SldrTerHMod_CustC.getValue());
	}
	%ChHVItrA = %ChHVItrAmod;

	%CHPyRelItrB = (mFloor(%ChPosY / %Ter_ItrB_XY)) * %Ter_ItrB_XY;
	%ChPxRelItrB = (mFloor(%ChPosX / %Ter_ItrB_XY)) * %Ter_ItrB_XY;
	%ChHVItrB = PTG_GUI_RandNumGen_Chunk(%CHPxRelItrB+%TerOff_X,%CHPyRelItrB+%TerOff_Y,%Ter_ItrB_XY,13333227,49979687,13313,15551);

	%ChPyRelItrC = (mFloor((%ChPosY+%BrPosY) / %Ter_ItrC_XY)) * %Ter_ItrB_XY;
	%ChPxRelItrC = (mFloor((%ChPosX+%BrPosX) / %Ter_ItrC_XY)) * %Ter_ItrB_XY;
	%ChHVItrC = PTG_GUI_RandNumGen_Chunk(%CHPxRelItrC+%TerOff_X,%CHPyRelItrC+%TerOff_Y,%Ter_ItrB_XY,13333227,49979687,13313,15551);
	%BrPyActItrC = (mFloor(%BrPosY / %Ter_ItrC_XY)) * %Ter_ItrC_XY; //%ChPosY + %BrPosY? Then dont' have to use %BrPosY-%BrPyActITrC below
	%BrPxActItrC = (mFloor(%BrPosX / %Ter_ItrC_XY)) * %Ter_ItrC_XY;
	
	if(PTG_Cmplx_PopUpTerType.getvalue() $= "FlatLands")
	{
		%ChHVItrA = "0 0 0 0";
		%ChHVItrB = "0 0 0 0";
		%ChHVItrC = "0 0 0 0";
	}

	%Co = ((%ChPosY-%ChPyActItrA)+%BrPosY) / %Ter_ItrA_XY;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
	%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

	%Co = ((%ChPosY-%CHPyRelItrB)+%BrPosY) / %Ter_ItrB_XY;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
	%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
	
	%Co = (%BrPosY-%BrPyActItrC) / %Ter_ItrC_XY;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrCColL = getWord(%ChHVItrC,0) - ((getWord(%ChHVItrC,0) - getWord(%ChHVItrC,1)) * %Sm);
	%ItrCColR = getWord(%ChHVItrC,2) - ((getWord(%ChHVItrC,2) - getWord(%ChHVItrC,3)) * %Sm);
	
	%Co = ((%ChPosX-%ChPxActItrA)+%BrPosX) / %Ter_ItrA_XY;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
	
	%Co = ((%ChPosX-%CHPxRelItrB)+%BrPosX) / %Ter_ItrB_XY;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);
	
	%Co = (%BrPosX-%BrPxActItrC) / %Ter_ItrC_XY;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrCRow = %ItrCColL - ((%ItrCColL - %ItrCColR) * %Sm);
	
	%relZ = (%ItrARow*%Ter_ItrA_Z) + (%ItrBRow*%Ter_ItrB_Z) + (%ItrCRow*%Ter_ItrC_Z) + %TerOff_Z;

	//Lakes / Connect Lakes option
	if(PTG_Cmplx_ChkEnabCnctLakes.getValue() && (%relZ - %TerOff_Z) < (%Level_Water+%Level_CnctLakes) && PTG_Cmplx_PopUpTerType.getValue() !$= "SkyLands")
		%relZ -= ((%Level_Water+%Level_CnctLakes) - %relZ);

	//Edge-FallOff
	%relZ = PTG_GUI_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%relZ);
	
	%tempZ = mFloor(%relZ / %BrZSize) * %BrZSize;
	%BrH = getMax(%tempZ,%BrZSize);
	
	//////////////////////////////////////////////////
	
	//Mountains
	if(!PTG_Cmplx_ChkEnabMntns.getValue())
		%BrH_init = %BrH_cf = $StrPrevArrayHV_Mountains[%BrPosX,%BrPosY] = %BrH;
	else
	{
		if((%BrH_cf = $StrPrevArrayHV_Mountains[%BrPosX,%BrPosY]) == 0)
			%BrH_init = %BrH_cf = %BrH;
		else
		{
			if(%BrH_cf > (%MntnSecZ = PTG_Cmplx_EdtSectMntn.getValue()))
			{
				%Mntn_ZSnap = PTG_Cmplx_SldrMntnZSnapMult.getValue();
				%Mntn_ZMult = PTG_Cmplx_SldrMntnZMult.getValue();
	
				%BrH_cf = mFloor(%BrH + ((%BrH_cf - %MntnSecZ) * %Mntn_ZMult));
				%relBrH = %BrZSize * %Mntn_ZSnap;
				
				%BrH_cf = PTG_GUI_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%BrH_cf);
				%BrH_cf = mFloor(%BrH_cf / %relBrH) * %relBrH;
				
				if(%BrH_cf < %BrH) 
					%BrH_cf = %BrH;
				
				%BrH_init = %BrH; //ter height unmodified by mntn height
				%BrH = %BrH_cf;

				%BrH_cf = $StrPrevArrayHV_Mountains[%BrPosX,%BrPosY] = %BrH_cf;
			}
			else
				%BrH_init = %BrH_cf = $StrPrevArrayHV_Mountains[%BrPosX,%BrPosY] = %BrH;
		}
	}
	
	//////////////////////////////////////////////////
	//Caves
	
	if(PTG_Cmplx_ChkEnabCaves.getValue() && PTG_Cmplx_PopUpTerType.getValue() !$= "SkyLands")
	{
		%CaveH_top = $StrPrevArrayHV_CavesTop[%BrPosX,%BrPosY];
		%CaveH_btm = $StrPrevArrayHV_CavesBtm[%BrPosX,%BrPosY];
		
		//Take fill bricks into account for total brick count
		%CaveH_top_L = $StrPrevArrayHV_CavesTop[%BrPosX-%BrXYSize,%BrPosY];
		%CaveH_top_R = $StrPrevArrayHV_CavesTop[%BrPosX+%BrXYSize,%BrPosY];
		%CaveH_top_D = $StrPrevArrayHV_CavesTop[%BrPosX,%BrPosY-%BrXYSize];
		%CaveH_top_U = $StrPrevArrayHV_CavesTop[%BrPosX,%BrPosY+%BrXYSize];
		
		%CaveH_btm_L = $StrPrevArrayHV_CavesBtm[%BrPosX-%BrXYSize,%BrPosY];
		%CaveH_btm_R = $StrPrevArrayHV_CavesBtm[%BrPosX+%BrXYSize,%BrPosY];
		%CaveH_btm_D = $StrPrevArrayHV_CavesBtm[%BrPosX,%BrPosY-%BrXYSize];
		%CaveH_btm_U = $StrPrevArrayHV_CavesBtm[%BrPosX,%BrPosY+%BrXYSize];
		
		%caveSecCut = %CaveH_top_L == 0 || %CaveH_top_R == 0 || %CaveH_top_D == 0 || %CaveH_top_U == 0;
		
		if(%CaveH_top_L == 0)
		{
			%CaveH_top_L = %CaveH_top;
			%CaveH_btm_L = %CaveH_btm;
		}
		if(%CaveH_top_R == 0)
		{
			%CaveH_top_R = %CaveH_top;
			%CaveH_btm_R = %CaveH_btm;
		}
		if(%CaveH_top_D == 0)
		{
			%CaveH_top_D = %CaveH_top;
			%CaveH_btm_D = %CaveH_btm;
		}
		if(%CaveH_top_U == 0)
		{
			%CaveH_top_U = %CaveH_top;
			%CaveH_btm_U = %CaveH_btm;
		}

		%CaveH_top_max = getMax(%CaveH_top_L,getMax(%CaveH_top_R,getMax(%CaveH_top_D,%CaveH_top_U)));					
		%CaveH_btm_min = getMin(%CaveH_btm_L,getMin(%CaveH_btm_R,getMin(%CaveH_btm_D,%CaveH_btm_U)));
		
		if(%CaveH_top > 0 && %CaveH_btm <= (%BrH - %BrZSize)) //make sure this stays in sync w/ actual terrain gen - esp. once seams issue with caves is fixed
		{
			%col = PTG_GUI_ApplyFogDepth(PTG_Cmplx_SwBioCaveBTerCol.color,%CaveH_btm);
			%gridStartY = getWord(PTG_Cmplx_SwPrevTopWndw_ChGrid.extent,1);
			%caveBtmBrGen = true;
			%BrCount++;
			
			if(PTG_Cmplx_BmpTerBr.ModTer && PTG_Cmplx_PopUpModTerType.getValue() !$= "Cubes") //ModTer
				%BrCount++;

			if((%CaveH_top + %BrZSize) > (%BrH - %BrZSize))
			{
				//if(%CaveH_btm >= %Level_Water || PTG_Cmplx_ChkDisWater.getValue())
				//{
					%sw = PTG_GUI_GenGUIObj(%ChPosX-%startPosX+%BrPosX SPC %gridStartY-(%ChPosY-%startPosY+%BrPosY)-%BrXYSize,%BrXYSize SPC %BrXYSize,%col,"Swatch");
					PTG_Cmplx_SwPrevTopWndw_CaveA.add(%sw);
				//}
				
				//If cave cuts terrain layer, and water generates for cave
				if(%CaveH_btm < %Level_Water && !PTG_Cmplx_ChkDisWater.getValue())
				{
					%col = PTG_GUI_Chunk_FigureColPri("","","","Water",$StrPrevArrayHV_CustomBiomeA[%BrPosX,%BrPosY],$StrPrevArrayHV_CustomBiomeB[%BrPosX,%BrPosY],$StrPrevArrayHV_CustomBiomeC[%BrPosX,%BrPosY]);
					%col = PTG_GUI_ApplyFogDepth(%col,%Level_Water);
					%sw = PTG_GUI_GenGUIObj(%ChPosX-%startPosX+%BrPosX SPC %gridStartY-(%ChPosY-%startPosY+%BrPosY)-%BrXYSize,%BrXYSize SPC %BrXYSize,%col,"Swatch");
					//PTG_Cmplx_SwPrevTopWndw_CaveA.add(%sw); //avoid using "PTG_Cmplx_SwPrevTopWndw_CaveB" (causes problems with rendering)
					PTG_Cmplx_SwPrevTopWndw_CaveWat.add(%sw);
				}
			}
			else
			{
				%sw = PTG_GUI_GenGUIObj(%ChPosX-%startPosX+%BrPosX SPC %gridStartY-(%ChPosY-%startPosY+%BrPosY)-%BrXYSize,%BrXYSize SPC %BrXYSize,%col,"Swatch");
				PTG_Cmplx_SwPrevTopWndw_CaveC.add(%sw);
				%caveTopBrGen = true;
				%BrCount++;
				
				if(PTG_Cmplx_BmpTerBr.ModTer && PTG_Cmplx_PopUpModTerType.getValue() !$= "Cubes") //ModTer
					%BrCount++;
			}
		}
		
		//Adjacent gap fill (between cave top and cave btm layers)
		if(%caveBtmBrGen && %caveTopBrGen && %caveSecCut)
		{
			//Adjacent gap fill (cave bottom)
			if(%CaveH_top > %CaveH_btm) //%tmpBrZSize_caveTopBtm > 0.5
				%BrCount += PTG_GUI_GapFillCount(%CaveH_top - %CaveH_btm,"Plate",%BrXYSize);
		}
		
		//Normal cave layers gap fill
		else
		{
			//Cave bottom gap fill
			if(%caveBtmBrGen && ((%CaveH_btm - %BrZSize) > %CaveH_btm_min))
				%BrCount += PTG_GUI_GapFillCount(((%CaveH_btm - %BrZSize) - %CaveH_btm_min) + %BrXYSize,"Cube",%BrXYSize);
		
			//Cave top gap fill
			if(%caveTopBrGen)
			{
				%TerH = $StrPrevArrayHV_Mountains[%BrPosX,%BrPosY];
				
				%TerH_L = $StrPrevArrayHV_Mountains[%BrPosX-%BrXYSize,%BrPosY];
				%TerH_R = $StrPrevArrayHV_Mountains[%BrPosX+%BrXYSize,%BrPosY];
				%TerH_D = $StrPrevArrayHV_Mountains[%BrPosX,%BrPosY-%BrXYSize];
				%TerH_U = $StrPrevArrayHV_Mountains[%BrPosX,%BrPosY+%BrXYSize];
				
				//Edges of chunks aren't calculated, so take this into account
				if(%TerH_L == 0) %TerH_L = %TerH;
				if(%TerH_R == 0) %TerH_R = %TerH;
				if(%TerH_D == 0) %TerH_D = %TerH;
				if(%TerH_U == 0) %TerH_U = %TerH;
				
				%TerH_min = getMin(%TerH_L,getMin(%TerH_R,getMin(%TerH_D,%TerH_U)));
				
				//If cave top gap fill exceeds terrain btm gap fill
				if((%CaveH_top_max + %BrXYSize) > (%TerH_min - %BrXYSize))
				{
					//Adjacent gap fill (between cave top and terrain)
					if((%TerH - %BrZSize) > (%CaveH_top + %BrZSize))
						%BrCount += PTG_GUI_GapFillCount((%TerH - %BrZSize) - (%CaveH_top + %BrZSize),"Plate",%BrXYSize);
				}
				
				//Normal cave top gap fill
				else if(%CaveH_top_max > (%CaveH_top + %BrZSize))
					%BrCount += PTG_GUI_GapFillCount((%CaveH_top_max - (%CaveH_top + %BrZSize)) + %BrXYSize,"Cube",%BrXYSize);
			}
		}
	}
	
	//////////////////////////////////////////////////
	//Terrain 
	
	if(PTG_Cmplx_ChkEnabMntns.getValue() && %BrH_cf > %BrH_init) //don't apply biome color if mntns enabled and above mntn section
		%col = PTG_GUI_Chunk_FigureColPri(%BrH_init,%BrH_cf,%BrH,"Terrain",0,0,0);
	else
		%col = PTG_GUI_Chunk_FigureColPri(%BrH_init,%BrH_init,%BrH,"Terrain",$StrPrevArrayHV_CustomBiomeA[%BrPosX,%BrPosY],$StrPrevArrayHV_CustomBiomeB[%BrPosX,%BrPosY],$StrPrevArrayHV_CustomBiomeC[%BrPosX,%BrPosY]);
	
	%col = PTG_GUI_ApplyFogDepth(%col,%BrH);
	%gridStartY = getWord(PTG_Cmplx_SwPrevTopWndw_ChGrid.extent,1);
	%sw = PTG_GUI_GenGUIObj(%ChPosX-%startPosX+%BrPosX SPC %gridStartY-(%ChPosY-%startPosY+%BrPosY)-%BrXYSize,%BrXYSize SPC %BrXYSize,%col,"Swatch");

	//Terrain / Mountains (mountains are intertwined with terrain, and thus can't be a separate layer - due to mountains affecting terrain height)
	if(%BrH < %Level_Water)
		PTG_Cmplx_SwPrevTopWndw_TerB.add(%sw);
	else
		PTG_Cmplx_SwPrevTopWndw_TerA.add(%sw);
	
	//////////////////////////////////////////////////
	
	if((%BrPosX += %BrXYSize) >= %ChSize)
	{
		%BrPosX = 0; //%BrPosX = %BrXYSize / 2;
		
		if((%BrPosY += %BrXYSize) >= %ChSize)
		{
			//Terrain Brick Count (terrain, unlike other features for preview, are generated while being calculated, which would throw off brick count if calculated above)
			for(%BrPosY = 0; %BrPosY < %ChSize; %BrPosY += %BrXYSize)
			{
				for(%BrPosX = 0; %BrPosX < %ChSize; %BrPosX += %BrXYSize)
				{
					%TerH = $StrPrevArrayHV_Mountains[%BrPosX,%BrPosY];
					%CaveH_top = $StrPrevArrayHV_CavesTop[%BrPosX,%BrPosY];
					%CaveH_btm = $StrPrevArrayHV_CavesBtm[%BrPosX,%BrPosY];
					
					if(%CaveH_top == 0 || %CaveH_btm > (%BrH - %BrZSize) || (%CaveH_top + %BrZSize) <= (%BrH - %BrZSize))
					{
						%TerH_L = $StrPrevArrayHV_Mountains[%BrPosX-%BrXYSize,%BrPosY];
						%TerH_R = $StrPrevArrayHV_Mountains[%BrPosX+%BrXYSize,%BrPosY];
						%TerH_D = $StrPrevArrayHV_Mountains[%BrPosX,%BrPosY-%BrXYSize];
						%TerH_U = $StrPrevArrayHV_Mountains[%BrPosX,%BrPosY+%BrXYSize];
						
						%CaveH_top_L = $StrPrevArrayHV_CavesTop[%BrPosX-%BrXYSize,%BrPosY];
						%CaveH_top_R = $StrPrevArrayHV_CavesTop[%BrPosX+%BrXYSize,%BrPosY];
						%CaveH_top_D = $StrPrevArrayHV_CavesTop[%BrPosX,%BrPosY-%BrXYSize];
						%CaveH_top_U = $StrPrevArrayHV_CavesTop[%BrPosX,%BrPosY+%BrXYSize];
						
						//Edges of chunks aren't calculated, so take this into account
						if(%TerH_L == 0) %TerH_L = %TerH;
						if(%TerH_R == 0) %TerH_R = %TerH;
						if(%TerH_D == 0) %TerH_D = %TerH;
						if(%TerH_U == 0) %TerH_U = %TerH;
							
						%TerH_min = getMin(%TerH_L,getMin(%TerH_R,getMin(%TerH_D,%TerH_U)));
						%CaveT_max = getMax(%CaveH_top_L,getMax(%CaveH_top_R,getMax(%CaveH_top_D,%CaveH_top_U)));
						//%tmpBrZSize_ter = mCeil((%TerH + %BrXYSize - %TerH_min) / %BrXYSize) * %BrXYSize;
						%BrCount++;
						
						//Adjacent gap fill
						if(((%TerH - %BrZSize) > %TerH_min) && (%TerH_min > %CaveT_max))
							%BrCount += PTG_GUI_GapFillCount(((%TerH - %BrZSize) - %TerH_min) + %BrXYSize,"Cube",%BrXYSize);

						if(PTG_Cmplx_ChkEnabPlateCap.getValue() && (!PTG_Cmplx_BmpTerBr.ModTer || PTG_Cmplx_PopUpModTerType.getValue() $= "Cubes")) //plate-capping
							%BrCount++;
						else if(PTG_Cmplx_BmpTerBr.ModTer && PTG_Cmplx_PopUpModTerType.getValue() !$= "Cubes") //ModTer
							%BrCount++;
					}
				}
			}

			schedule(0,0,PTG_GUI_Routine_Append,%ChPosX,%ChPosY,%xmod,%ymod,%BrCount,"Water");
			return;
		}
	}

	schedule(0,0,PTG_GUI_Noise_Terrain,%CHPosX,%CHPosY,%xmod,%ymod,%BrPosX,%BrPosY,%BrCount);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_Noise_SkyLands(%CHPosX,%CHPosY,%xmod,%ymod,%BrPosX,%BrPosY,%BrCount)
{
	%BrXYSize = getMax(PTG_Cmplx_BmpTerBr.BrickID.brickSizeX * 0.5,1);
	%BrZSize = PTG_Cmplx_BmpTerBr.BrickID.brickSizeZ * 0.2;
	%ChSize = getWord("16 32 64 128 256",mClamp(PTG_Cmplx_PopupChSize.getSelected(),0,4));
	%Level_Water = PTG_Cmplx_SldrWaterLevel.getValue();
	%Level_CnctLakes = PTG_Cmplx_SldrCnctLakesStrt.getValue();
	
	%Ter_ItrA_XY = PTG_Cmplx_EdtNosScaleTerAXY.getValue();
	%Ter_ItrB_XY = PTG_Cmplx_EdtNosScaleTerBXY.getValue();
	%Ter_ItrC_XY = PTG_Cmplx_EdtNosScaleTerCXY.getValue();
	%Ter_ItrA_Z = PTG_Cmplx_EdtNosScaleTerAZ.getValue();
	%Ter_ItrB_Z = PTG_Cmplx_EdtNosScaleTerBZ.getValue();
	%Ter_ItrC_Z = PTG_Cmplx_EdtNosScaleTerCZ.getValue();
	%TerOff_X = PTG_Cmplx_EdtTerNosOffX.getValue();
	%TerOff_Y = PTG_Cmplx_EdtTerNosOffY.getValue();
	%TerOff_Z = PTG_Cmplx_SldrTerZOffset.getValue();

	%ChPxRelItrA = (mFloor(%ChPosX / %Ter_ItrA_XY)) * %Ter_ItrB_XY;
	%ChPyRelItrA = (mFloor(%ChPosY / %Ter_ItrA_XY)) * %Ter_ItrB_XY;
	%ChHVItrA = PTG_GUI_RandNumGen_Chunk(%CHPxRelItrA+%TerOff_X,%CHPyRelItrA+%TerOff_Y,%Ter_ItrB_XY,13333227,49979687,13313,15551);
	%ChPxActItrA = (mFloor(%ChPosX / %Ter_ItrA_XY)) * %Ter_ItrA_XY;
	%ChPyActItrA = (mFloor(%ChPosY / %Ter_ItrA_XY)) * %Ter_ItrA_XY;

	%ChHVItrAmod = %ChHVItrA;
	if(PTG_Cmplx_ChkEnabCustABio.getValue()) //Custom Biome A
	{
		%bioCheckStr = PTG_GUI_Noise_BiomeCheck(%ChPosX,%ChPosY,$PTGm.bio_CustA_itrA_XY,$PTGm.bio_CustA_itrA_Z,$PTGm.bio_CustAOff_X,$PTGm.bio_CustAOff_Y,$PTGm.Bio_CustA_YStart);
		
		if((getWord(%bioCheckStr,0)) < PTG_Cmplx_EdtSectCustA.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * PTG_Cmplx_SldrTerHMod_CustA.getValue());
		if((getWord(%bioCheckStr,2)) < PTG_Cmplx_EdtSectCustA.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * PTG_Cmplx_SldrTerHMod_CustA.getValue());
		if((getWord(%bioCheckStr,1)) < PTG_Cmplx_EdtSectCustA.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * PTG_Cmplx_SldrTerHMod_CustA.getValue());
		if((getWord(%bioCheckStr,3)) < PTG_Cmplx_EdtSectCustA.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * PTG_Cmplx_SldrTerHMod_CustA.getValue());
	}
	if(PTG_Cmplx_ChkEnabCustBBio.getValue()) //Custom Biome B
	{
		%bioCheckStr = PTG_GUI_Noise_BiomeCheck(%ChPosX,%ChPosY,$PTGm.bio_CustB_itrA_XY,$PTGm.bio_CustB_itrA_Z,$PTGm.bio_CustBOff_X,$PTGm.bio_CustBOff_Y,$PTGm.Bio_CustB_YStart);
		
		if((getWord(%bioCheckStr,0)) < PTG_Cmplx_EdtSectCustB.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * PTG_Cmplx_SldrTerHMod_CustB.getValue());
		if((getWord(%bioCheckStr,2)) < PTG_Cmplx_EdtSectCustB.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * PTG_Cmplx_SldrTerHMod_CustB.getValue());
		if((getWord(%bioCheckStr,1)) < PTG_Cmplx_EdtSectCustB.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * PTG_Cmplx_SldrTerHMod_CustB.getValue());
		if((getWord(%bioCheckStr,3)) < PTG_Cmplx_EdtSectCustB.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * PTG_Cmplx_SldrTerHMod_CustB.getValue());
	}
	if(PTG_Cmplx_ChkEnabCustCBio.getValue()) //Custom Biome C
	{
		%bioCheckStr = PTG_GUI_Noise_BiomeCheck(%ChPosX,%ChPosY,$PTGm.bio_CustC_itrA_XY,$PTGm.bio_CustC_itrA_Z,$PTGm.bio_CustCOff_X,$PTGm.bio_CustCOff_Y,$PTGm.Bio_CustC_YStart);
		
		if((getWord(%bioCheckStr,0)) < PTG_Cmplx_EdtSectCustC.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 0, getWord(%ChHVItrA, 0) * PTG_Cmplx_SldrTerHMod_CustC.getValue());
		if((getWord(%bioCheckStr,2)) < PTG_Cmplx_EdtSectCustC.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 1, getWord(%ChHVItrA, 1) * PTG_Cmplx_SldrTerHMod_CustC.getValue());
		if((getWord(%bioCheckStr,1)) < PTG_Cmplx_EdtSectCustC.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 2, getWord(%ChHVItrA, 2) * PTG_Cmplx_SldrTerHMod_CustC.getValue());
		if((getWord(%bioCheckStr,3)) < PTG_Cmplx_EdtSectCustC.getValue()) %ChHVItrAmod = setWord(%ChHVItrAmod, 3, getWord(%ChHVItrA, 3) * PTG_Cmplx_SldrTerHMod_CustC.getValue());
	}
	%ChHVItrA = %ChHVItrAmod;

	%CHPyRelItrB = (mFloor(%ChPosY / %Ter_ItrB_XY)) * %Ter_ItrB_XY;
	%ChPxRelItrB = (mFloor(%ChPosX / %Ter_ItrB_XY)) * %Ter_ItrB_XY;
	%ChHVItrB = PTG_GUI_RandNumGen_Chunk(%CHPxRelItrB+%TerOff_X,%CHPyRelItrB+%TerOff_Y,%Ter_ItrB_XY,13333227,49979687,13313,15551);

	//////////////////////////////////////////////////
	
	%ChPyRelItrC = (mFloor((%ChPosY+%BrPosY) / %Ter_ItrC_XY)) * %Ter_ItrB_XY;
	%ChPxRelItrC = (mFloor((%ChPosX+%BrPosX) / %Ter_ItrC_XY)) * %Ter_ItrB_XY;
	%ChHVItrC = PTG_GUI_RandNumGen_Chunk(%CHPxRelItrC+%TerOff_X,%CHPyRelItrC+%TerOff_Y,%Ter_ItrB_XY,13333227,49979687,13313,15551);
	%BrPyActItrC = (mFloor(%BrPosY / %Ter_ItrC_XY)) * %Ter_ItrC_XY; //%ChPosY + %BrPosY? Then dont' have to use %BrPosY-%BrPyActITrC below!!!
	%BrPxActItrC = (mFloor(%BrPosX / %Ter_ItrC_XY)) * %Ter_ItrC_XY;

	%Co = ((%ChPosY-%ChPyActItrA)+%BrPosY) / %Ter_ItrA_XY;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
	%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

	%Co = ((%ChPosY-%CHPyRelItrB)+%BrPosY) / %Ter_ItrB_XY;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
	%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
	
	%Co = (%BrPosY-%BrPyActItrC) / %Ter_ItrC_XY;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrCColL = getWord(%ChHVItrC,0) - ((getWord(%ChHVItrC,0) - getWord(%ChHVItrC,1)) * %Sm);
	%ItrCColR = getWord(%ChHVItrC,2) - ((getWord(%ChHVItrC,2) - getWord(%ChHVItrC,3)) * %Sm);
	
	%Co = ((%ChPosX-%ChPxActItrA)+%BrPosX) / %Ter_ItrA_XY;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
	
	%Co = ((%ChPosX-%CHPxRelItrB)+%BrPosX) / %Ter_ItrB_XY;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);
	
	%Co = (%BrPosX-%BrPxActItrC) / %Ter_ItrC_XY;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrCRow = %ItrCColL - ((%ItrCColL - %ItrCColR) * %Sm);
	
	%relZ = (%ItrARow*%Ter_ItrA_Z) + (%ItrBRow*%Ter_ItrB_Z) + (%ItrCRow*%Ter_ItrC_Z) + %TerOff_Z;

	//Lakes / Connect Lakes option
	if(PTG_Cmplx_ChkEnabCnctLakes.getValue() && (%relZ - %TerOff_Z) < (%Level_Water+%Level_CnctLakes) && PTG_Cmplx_PopUpTerType.getValue() !$= "SkyLands")
		%relZ -= ((%Level_Water+%Level_CnctLakes) - %relZ);

	//Edge-FallOff
	%relZ = PTG_GUI_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%relZ);
	
	%tempZ = mFloor(%relZ / %BrZSize) * %BrZSize;
	%BrH = getMax(%tempZ,%BrZSize);
	%BrH_init = %BrH;
	
	//////////////////////////////////////////////////
	
	//Mountains
	if(!PTG_Cmplx_ChkEnabMntns.getValue())
	{
		%BrH_cf = %BrH;
		$StrPrevArrayHV_Mountains[%BrPosX,%BrPosY] = %BrH_cf;
	}
	else
	{
		if((%BrH_cf = $StrPrevArrayHV_Mountains[%BrPosX,%BrPosY]) == 0)
			%BrH_cf = %BrH;
		else
		{
			if(%BrH_cf > (%MntnSecZ = PTG_Cmplx_EdtSectMntn.getValue()))
			{
				%Mntn_ZSnap = PTG_Cmplx_SldrMntnZSnapMult.getValue();
				%Mntn_ZMult = PTG_Cmplx_SldrMntnZMult.getValue();
	
				%BrHb = mFloor(%BrH + ((%BrH_cf - %MntnSecZ) * %Mntn_ZMult));
				%relBrH = %BrZSize * %Mntn_ZSnap;
				
				//Edge-FallOff
				%relBrH = PTG_GUI_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%relBrH);
	
				%BrHb = mFloor(%BrHb / %relBrH) * %relBrH;
				
				if(%BrHb < %BrH) 
					%BrHb = %BrH;
				%BrH = %BrHb;

				%BrH_cf = $StrPrevArrayHV_Mountains[%BrPosX,%BrPosY] = %BrHb;
			}
			else
				%BrH_cf = $StrPrevArrayHV_Mountains[%BrPosX,%BrPosY] = %BrH;
		}
	}
	
	//////////////////////////////////////////////////

	%Section_SkyLands = PTG_Cmplx_EdtSectSkyland.getValue();
	
	if((%BrH - %TerOff_Z) > %Section_SkyLands)
	{
		%ChPxActItrA = (mFloor(%CHPosX / PTG_Cmplx_EdtNosScaleSkylandXY.getValue())) * PTG_Cmplx_EdtNosScaleSkylandXY.getValue();
		%ChPyActItrA = (mFloor(%CHPosY / PTG_Cmplx_EdtNosScaleSkylandXY.getValue())) * PTG_Cmplx_EdtNosScaleSkylandXY.getValue();
		%ChHVItrA = PTG_GUI_RandNumGen_Chunk(%ChPxActItrA+$PTGm.skyLandsOff_X,%ChPyActItrA+$PTGm.skyLandsOff_Y,PTG_Cmplx_EdtNosScaleSkylandXY.getValue(),13466917,7649689,14107,79561);

		%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / PTG_Cmplx_EdtNosScaleSkylandXY.getValue();
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
		%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);
		
		%Co = ((%CHPosX-%ChPxActItrA)+%BrPosX) / PTG_Cmplx_EdtNosScaleSkylandXY.getValue();
		%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
		%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) * %Sm);
		
		%relZ = (%ItrARow * PTG_Cmplx_EdtNosScaleSkylandZ.getValue());
		%tempZ = mFloor(%relZ / %BrZSize) * %BrZSize;
		%BrH_SL = getMax(%tempZ,%BrZSize);
		
			//$StrPrevArrayHV_SkylandsTop[%BrPosX,%BrPosY] = %BrH = (%BrH - %Section_SkyLands) + %tempZ;
			//$StrPrevArrayHV_SkylandsBtm[%BrPosX,%BrPosY]
		//%BrH = ((%BrH - %Section_SkyLands)) + %tempZ;
		//$StrPrevArrayHV_SkyLands[%BrPosX,%BrPosY] = %BrH; //???
		
		%BrH_act = $StrPrevArrayHV_Mountains[%BrPosX,%BrPosY] - %TerOff_Z;
		%BrH_fnl = $StrPrevArrayHV_SkyLandsTop[%BrPosX,%BrPosY] = (%TerOff_Z + (%BrH_act - %Section_SkyLands)) + %BrH_SL;
		$StrPrevArrayHV_SkyLandsBtm[%BrPosX,%BrPosY] = (%TerOff_Z - ((%BrH_act - %Section_SkyLands) * 2)) + %BrH_SL;

		%col = PTG_GUI_Chunk_FigureColPri(%BrH_init,%BrH_cf,%BrH_fnl,"Terrain",$StrPrevArrayHV_CustomBiomeA[%BrPosX,%BrPosY],$StrPrevArrayHV_CustomBiomeB[%BrPosX,%BrPosY],$StrPrevArrayHV_CustomBiomeC[%BrPosX,%BrPosY]);
		%col = PTG_GUI_ApplyFogDepth(%col,%BrH_cf);
		%gridStartY = getWord(PTG_Cmplx_SwPrevTopWndw_ChGrid.extent,1);
		%startPosX = PTG_Cmplx_EdtGridXStart.getValue();
		%startPosY = PTG_Cmplx_EdtGridYStart.getValue();
		%sw = PTG_GUI_GenGUIObj(%ChPosX-%startPosX+%BrPosX SPC %gridStartY-(%ChPosY-%startPosY+%BrPosY)-%BrXYSize,%BrXYSize SPC %BrXYSize,%col,"Swatch");

		//Terrain / Mountains (mountains are intertwined with terrain, and thus can't be a separate layer - due to mountains affecting terrain height)
			//Add relative to water layer, even though water doesn't generate for SkyLands
		if(%BrH_cf < PTG_Cmplx_SldrWaterLevel.getValue())
			PTG_Cmplx_SwPrevTopWndw_TerB.add(%sw);
		else
			PTG_Cmplx_SwPrevTopWndw_TerA.add(%sw);
	}
	
	//////////////////////////////////////////////////
	
	//Fix brickcount for skylands
	//skylands array?
	
	if((%BrPosX += %BrXYSize) >= %ChSize)
	{
		%BrPosX = 0; //%BrPosX = %BrXYSize / 2;
		
		if((%BrPosY += %BrXYSize) >= %ChSize)
		{
			//Terrain Brick Count (terrain, unlike other features for preview, are generated while being calculated, which would throw off brick count if calculated above)
			for(%BrPosY = 0; %BrPosY < %ChSize; %BrPosY += %BrXYSize)
			{
				for(%BrPosX = 0; %BrPosX < %ChSize; %BrPosX += %BrXYSize)
				{
					%TerH_top = $StrPrevArrayHV_SkyLandsTop[%BrPosX,%BrPosY];
					%TerH_btm = $StrPrevArrayHV_SkyLandsBtm[%BrPosX,%BrPosY];
					
					if(%TerH_top > 0 && %TerH_btm > 0)
					{
						%TerH_L = $StrPrevArrayHV_SkyLandsTop[%BrPosX-%BrXYSize,%BrPosY];
						%TerH_R = $StrPrevArrayHV_SkyLandsTop[%BrPosX+%BrXYSize,%BrPosY];
						%TerH_D = $StrPrevArrayHV_SkyLandsTop[%BrPosX,%BrPosY-%BrXYSize];
						%TerH_U = $StrPrevArrayHV_SkyLandsTop[%BrPosX,%BrPosY+%BrXYSize];
						
						%TerH_Lb = $StrPrevArrayHV_SkyLandsBtm[%BrPosX-%BrXYSize,%BrPosY];
						%TerH_Rb = $StrPrevArrayHV_SkyLandsBtm[%BrPosX+%BrXYSize,%BrPosY];
						%TerH_Db = $StrPrevArrayHV_SkyLandsBtm[%BrPosX,%BrPosY-%BrXYSize];
						%TerH_Ub = $StrPrevArrayHV_SkyLandsBtm[%BrPosX,%BrPosY+%BrXYSize];
						
						%skylandSecCut = %TerH_L == 0 || %TerH_R == 0 || %TerH_D == 0 || %TerH_U == 0;
						
						if(%TerH_L == 0) %TerH_L = %TerH_top;
						if(%TerH_R == 0) %TerH_R = %TerH_top;
						if(%TerH_D == 0) %TerH_D = %TerH_top;
						if(%TerH_U == 0) %TerH_U = %TerH_top;
						
						if(%TerH_Lb == 0) %TerH_Lb = %TerH_btm;
						if(%TerH_Rb == 0) %TerH_Rb = %TerH_btm;
						if(%TerH_Db == 0) %TerH_Db = %TerH_btm;
						if(%TerH_Ub == 0) %TerH_Ub = %TerH_btm;
							
						%TerH_min = getMin(%TerH_L,getMin(%TerH_R,getMin(%TerH_D,%TerH_U)));
						%TerH_max = getMax(%TerH_Lb,getMax(%TerH_Rb,getMax(%TerH_Db,%TerH_Ub)));
						%BrCount++;

						if(PTG_Cmplx_ChkEnabPlateCap.getValue() && (!PTG_Cmplx_BmpTerBr.ModTer || PTG_Cmplx_PopUpModTerType.getValue() $= "Cubes")) //plate-capping
							%BrCount++;
						else if(PTG_Cmplx_BmpTerBr.ModTer && PTG_Cmplx_PopUpModTerType.getValue() !$= "Cubes")
							%BrCount++;
						
						
						//Adjacent gap fill
						if((%TerH_min < %TerH_max) || %skylandSecCut)
						{
							if((%TerH_top - %BrZSize) > %TerH_btm)
								%BrCount += PTG_GUI_GapFillCount((%TerH_top - %BrZSize) - %TerH_btm,"Plate",%BrXYSize);
						}
						else
						{
							if((%TerH_top - %TerH_min) > 0)
								%BrCount += PTG_GUI_GapFillCount(((%TerH_top - %BrZSize) - %TerH_min) + %BrXYSize,"Cube",%BrXYSize);
							if((%TerH_max - %TerH_btm) > 0)
								%BrCount += PTG_GUI_GapFillCount((%TerH_max - %TerH_btm) + %BrXYSize,"Cube",%BrXYSize);
						}
					}
				}
			}

			schedule(0,0,PTG_GUI_Routine_Append,%ChPosX,%ChPosY,%xmod,%ymod,%BrCount,"Water");
			return;
		}
	}

	schedule(0,0,PTG_GUI_Noise_SkyLands,%CHPosX,%CHPosY,%xmod,%ymod,%BrPosX,%BrPosY,%BrCount);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_Noise_Clouds(%CHPosX,%CHPosY,%xmod,%ymod,%BrPosX,%BrPosY,%BrCount)
{
	%BrZSize = PTG_Cmplx_BmpTerBr.BrickID.brickSizeZ * 0.2;
	%BrXYSize = getMax(PTG_Cmplx_BmpCloudBr.BrickID.brickSizeX * 0.5,1);
	%ChSize = getWord("16 32 64 128 256",mClamp(PTG_Cmplx_PopupChSize.getSelected(),0,4));
	%ItrA_Clouds = PTG_Cmplx_EdtNosScaleCloudAXY.getValue();
	%ItrB_Clouds = PTG_Cmplx_EdtNosScaleCloudBXY.getValue();
	%OffX_Clouds = PTG_Cmplx_EdtCloudNosOffX.getValue();
	%OffY_Clouds = PTG_Cmplx_EdtCloudNosOffY.getValue();
	%YStrt_Clouds = PTG_Cmplx_EdtEquatorCloudY.getValue();
	%SecZ_Clouds = PTG_Cmplx_EdtSectCloud.getValue();
	%OffZ_Clouds = mFloor(PTG_Cmplx_SldrCloudZOffset.getValue() / %BrZSize) * %BrZSize;
	
	%ChPxRelItrA = (mFloor(%CHPosX / %ItrA_Clouds)) * %ItrB_Clouds;
	%ChPyRelItrA = (mFloor(%CHPosY / %ItrA_Clouds)) * %ItrB_Clouds;
	%ChHVItrA = PTG_GUI_RandNumGen_Chunk(%CHPxRelItrA+%OffX_Clouds,%CHPyRelItrA+%OffY_Clouds,%ItrB_Clouds,27177289,753266429,14549,54163);
	%ChPxActItrA = (mFloor(%CHPosX / %ItrA_Clouds)) * %ItrA_Clouds;
	%ChPyActItrA = (mFloor(%CHPosY / %ItrA_Clouds)) * %ItrA_Clouds;
	
	%ChHVItrA = PTG_GUI_Noise_PsuedoEquatorCheck(%ChHVItrA,%ChPyActItrA SPC %ChPyActItrA+%ItrA_Clouds,%YStrt_Clouds,0);
	
	%CHPyRelItrB = (mFloor(%CHPosY / %ItrB_Clouds)) * %ItrB_Clouds;
	%ChPxRelItrB = (mFloor(%CHPosX / %ItrB_Clouds)) * %ItrB_Clouds;
	%ChHVItrB = PTG_GUI_RandNumGen_Chunk(%CHPxRelItrB+%OffX_Clouds,%CHPyRelItrB+%OffY_Clouds,%ItrB_Clouds,27177289,753266429,14549,54163);
	
	%ChHVItrB = PTG_GUI_Noise_PsuedoEquatorCheck(%ChHVItrB,%CHPyRelItrB SPC %CHPyRelItrB+%ItrB_Clouds,%YStrt_Clouds,0);
	
	%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / %ItrA_Clouds;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrAColL = getWord(%ChHVItrA,0) - ((getWord(%ChHVItrA,0) - getWord(%ChHVItrA,1)) * %Sm);
	%ItrAColR = getWord(%ChHVItrA,2) - ((getWord(%ChHVItrA,2) - getWord(%ChHVItrA,3)) * %Sm);

	%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY) / %ItrB_Clouds;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrBColL = getWord(%ChHVItrB,0) - ((getWord(%ChHVItrB,0) - getWord(%ChHVItrB,1)) * %Sm);
	%ItrBColR = getWord(%ChHVItrB,2) - ((getWord(%ChHVItrB,2) - getWord(%ChHVItrB,3)) * %Sm);
	
	%Co = ((%CHPosX-%ChPxActItrA)+%BrPosX) / %ItrA_Clouds;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) *%Sm);
	
	%Co = ((%CHPosX-%CHPxRelItrB)+%BrPosX) / %ItrB_Clouds;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

	%relZ = (%ItrARow * PTG_Cmplx_EdtNosScaleCloudAZ.getValue()) + (%ItrBRow * PTG_Cmplx_EdtNosScaleCloudBZ.getValue());
	
	//Edge-FallOff
	%relZ = PTG_GUI_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,getMax(%relZ,%BrZSize));
	
	if(%relZ > %SecZ_Clouds)
	{
		%relZ = (mFloor((%relZ - %SecZ_Clouds) / %BrZSize) * %BrZSize) + PTG_Cmplx_SldrCloudZOffset.getValue();
		%BrCount++;
		%relZ = $StrPrevArrayHV_Clouds[%BrPosX,%BrPosY] = getMax(%relZ,%BrZSize);
		
		%col = PTG_GUI_ApplyFogDepth(PTG_Cmplx_SwCloudCol.color,%relZ);
		%gridStartY = getWord(PTG_Cmplx_SwPrevTopWndw_ChGrid.extent,1);
		%startPosX = PTG_Cmplx_EdtGridXStart.getValue();
		%startPosY = PTG_Cmplx_EdtGridYStart.getValue();
		%sw = PTG_GUI_GenGUIObj(%ChPosX-%startPosX+%BrPosX SPC %gridStartY-(%ChPosY-%startPosY+%BrPosY)-%BrXYSize,%BrXYSize SPC %BrXYSize,%col,"Swatch");

		//
		if(PTG_Cmplx_PopUpTerType.getValue() $= "SkyLands")
		{
			if(%relZ < $StrPrevArrayHV_SkyLands[%BrPosX,%BrPosY])
				PTG_Cmplx_SwPrevTopWndw_CldB.add(%sw);
			else
				PTG_Cmplx_SwPrevTopWndw_CldA.add(%sw);
		}
		else
		{
			if(%relZ < $StrPrevArrayHV_Mountains[%BrPosX,%BrPosY])
				PTG_Cmplx_SwPrevTopWndw_CldB.add(%sw);
			else
				PTG_Cmplx_SwPrevTopWndw_CldA.add(%sw);
		}
	}
	
	//////////////////////////////////////////////////
	
	if((%BrPosX += %BrXYSize) >= %ChSize)
	{
		%BrPosX = 0;
		
		if((%BrPosY += %BrXYSize) >= %ChSize)
		{
			for(%BrPosY = 0; %BrPosY < %ChSize; %BrPosY += %BrXYSize)
			{
				for(%BrPosX = 0; %BrPosX < %ChSize; %BrPosX += %BrXYSize)
				{
					%CldH = $StrPrevArrayHV_Clouds[%BrPosX,%BrPosY];
					
					if(%CldH > (%OffZ_Clouds + %BrZSize))
					{
						%BrCount++;
						
						if(PTG_Cmplx_BmpCloudBr.ModTer && PTG_Cmplx_PopUpModTerType_clouds.getValue() !$= "Cubes") //ModTer
							%BrCount++;
						
						%CldH_L = $StrPrevArrayHV_Clouds[%BrPosX-%BrXYSize,%BrPosY];
						%CldH_R = $StrPrevArrayHV_Clouds[%BrPosX+%BrXYSize,%BrPosY];
						%CldH_D = $StrPrevArrayHV_Clouds[%BrPosX,%BrPosY-%BrXYSize];
						%CldH_U = $StrPrevArrayHV_Clouds[%BrPosX,%BrPosY+%BrXYSize];
						
						//Edges of chunks aren't calculated, so take this into account
						if(%CldH_L == 0) %CldH_L = %CldH;
						if(%CldH_R == 0) %CldH_R = %CldH;
						if(%CldH_D == 0) %CldH_D = %CldH;
						if(%CldH_U == 0) %CldH_U = %CldH;
						
						%CldH_min = getMin(%CldH_L,getMin(%CldH_R,getMin(%CldH_D,%CldH_U)));
						
						//Adjacent gap fill
						if((%CldH - %CldH_min) > 0)
						{
							if((%CldH_min - %BrXYSize - %BrZSize) < (%OffZ_Clouds + %BrZSize)) //%clTopH-%BrZSize SPC %Level_CloudOff+%BrZSize
								%BrCount += PTG_GUI_GapFillCount(((%CldH - %BrZSize) - (%OffZ_Clouds + %BrZSize)) + %BrXYSize,"Plate",%BrXYSize);
							else if((%CldH - %BrZSize) > (%CldH_min - %BrXYSize - %BrZSize))
								%BrCount += PTG_GUI_GapFillCount(((%CldH - %BrZSize) - %CldH_min) + %BrXYSize,"Cube",%BrXYSize);
						}
					}
				}
			}
			
			
			schedule(0,0,PTG_GUI_Routine_Append,%ChPosX,%ChPosY,%xmod,%ymod,%BrCount,"FltIslds");
			return;
		}
	}

	schedule(0,0,PTG_GUI_Noise_Clouds,%CHPosX,%CHPosY,%xmod,%ymod,%BrPosX,%BrPosY,%BrCount);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_Noise_FltIslds(%CHPosX,%CHPosY,%xmod,%ymod,%BrPosX,%BrPosY,%BrCount)
{
	//%MinBrZSnap = $PTGm.brFltIslds_Zsize; // $PTGm.brFltIslds_FillXYZSize + $PTGm.brFltIslds_Zsize;
	//%BrXYhSize = $PTGm.brFltIslds_XYsize / 2;
	
	%BrZSize = PTG_Cmplx_BmpFltIsldsBr.BrickID.brickSizeZ * 0.2;
	%BrXYSize = getMax(PTG_Cmplx_BmpFltIsldsBr.BrickID.brickSizeX * 0.5,1);
	%ChSize = getWord("16 32 64 128 256",mClamp(PTG_Cmplx_PopupChSize.getSelected(),0,4));
	%ItrA_FI = PTG_Cmplx_EdtNosScaleFltIsldAXY.getValue();
	%ItrB_FI = PTG_Cmplx_EdtNosScaleFltIsldBXY.getValue();
	%OffX_FIa = PTG_Cmplx_EdtFltIsldANosOffX.getValue();
	%OffY_FIa = PTG_Cmplx_EdtFltIsldANosOffY.getValue();
	%OffX_FIb = PTG_Cmplx_EdtFltIsldBNosOffX.getValue();
	%OffY_FIb = PTG_Cmplx_EdtFltIsldBNosOffY.getValue();
	%YStrt_FI = PTG_Cmplx_EdtEquatorFltIsldY.getValue();
	%SecZ_FI = PTG_Cmplx_EdtSectFltIsld.getValue();
	%FIa_Off = PTG_Cmplx_SldrFltIsldsAZOffset.getValue();
	%FIb_Off = PTG_Cmplx_SldrFltIsldsBZOffset.getValue();
	%MinBrZSnap = %BrZSize;

	%ChPxRelItrA = (mFloor(%CHPosX / %ItrA_FI)) * %ItrB_FI;
	%ChPyRelItrA = (mFloor(%CHPosY / %ItrA_FI)) * %ItrB_FI;
	%ChHVItrA_A = PTG_GUI_RandNumGen_Chunk(%CHPxRelItrA+%OffX_FIa,%CHPyRelItrA+%OffY_FIa,%ItrB_FI,55555333,87889091,22273,33773);
	%ChHVItrA_B = PTG_GUI_RandNumGen_Chunk(%CHPxRelItrA+%OffX_FIa,%CHPyRelItrA+%OffY_FIb,%ItrB_FI,24710753,60000607,22273,33773);
	%ChPxActItrA = (mFloor(%CHPosX / %ItrA_FI)) * %ItrA_FI;
	%ChPyActItrA = (mFloor(%CHPosY / %ItrA_FI)) * %ItrA_FI;
	
	%ChHVItrA_A = PTG_GUI_Noise_PsuedoEquatorCheck(%ChHVItrA_A,%ChPyActItrA SPC %ChPyActItrA+%ItrA_FI,%YStrt_FI,0);
	%ChHVItrA_B = PTG_GUI_Noise_PsuedoEquatorCheck(%ChHVItrA_B,%ChPyActItrA SPC %ChPyActItrA+%ItrA_FI,%YStrt_FI,0);
	
	%CHPyRelItrB = (mFloor(%CHPosY / %ItrB_FI)) * %ItrB_FI;
	%ChPxRelItrB = (mFloor(%CHPosX / %ItrB_FI)) * %ItrB_FI;
	%ChHVItrB_A = PTG_GUI_RandNumGen_Chunk(%CHPxRelItrB+$PTGm.fltIsldsOff_X,%CHPyRelItrB+%OffY_FIa,%ItrB_FI,55555333,87889091,22273,33773);
	%ChHVItrB_B = PTG_GUI_RandNumGen_Chunk(%CHPxRelItrB+%OffX_FIa,%CHPyRelItrB+%OffY_FIb,%ItrB_FI,24710753,60000607,22273,33773);
	
	%ChHVItrB_A = PTG_GUI_Noise_PsuedoEquatorCheck(%ChHVItrB_A,%CHPyRelItrB SPC %CHPyRelItrB+%ItrB_FI,%YStrt_FI,0);
	%ChHVItrB_B = PTG_GUI_Noise_PsuedoEquatorCheck(%ChHVItrB_B,%CHPyRelItrB SPC %CHPyRelItrB+%ItrB_FI,%YStrt_FI,0);
	
	//////////////////////////////////////////////////
	
	%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / %ItrA_FI;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrAColL = getWord(%ChHVItrA_A,0) - ((getWord(%ChHVItrA_A,0) - getWord(%ChHVItrA_A,1)) * %Sm);
	%ItrAColR = getWord(%ChHVItrA_A,2) - ((getWord(%ChHVItrA_A,2) - getWord(%ChHVItrA_A,3)) * %Sm);

	%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY) / %ItrB_FI;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrBColL = getWord(%ChHVItrB_A,0) - ((getWord(%ChHVItrB_A,0) - getWord(%ChHVItrB_A,1)) * %Sm);
	%ItrBColR = getWord(%ChHVItrB_A,2) - ((getWord(%ChHVItrB_A,2) - getWord(%ChHVItrB_A,3)) * %Sm);
	
	%Co = ((%CHPosX-%ChPxActItrA)+%BrPosX) / %ItrA_FI;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) *%Sm);
	
	%Co = ((%CHPosX-%CHPxRelItrB)+%BrPosX) / %ItrB_FI;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

	%relZ = (%ItrARow * PTG_Cmplx_EdtNosScaleFltIsldAZ.getValue()) + (%ItrBRow * PTG_Cmplx_EdtNosScaleFltIsldBZ.getValue());
	
	//Edge-FallOff
	%relZ = PTG_GUI_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%relZ);

	%tempZ = mFloor(%relZ / %BrZSize) * %BrZSize;

	if(%tempZ > %SecZ_FI)
	{
		%fltIsldPass = true;
		%BrH_FIa = $StrPrevArrayHV_FltIsldsATop[%BrPosX,%BrPosY] = %FIa_Off + (mFloor((getMax(%tempZ,%MinBrZSnap) - %SecZ_FI) / %BrZSize) * %BrZSize);
		$StrPrevArrayHV_FltIsldsABtm[%BrPosX,%BrPosY] = %FIa_Off - (mFloor(((getMax(%tempZ,%MinBrZSnap) - %SecZ_FIZ) * 1.5) / %BrZSize) * %BrZSize);
	}
	
	//////////////////////////////////////////////////
	
	%Co = ((%CHPosY-%ChPyActItrA)+%BrPosY) / %ItrA_FI;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrAColL = getWord(%ChHVItrA_B,0) - ((getWord(%ChHVItrA_B,0) - getWord(%ChHVItrA_B,1)) * %Sm);
	%ItrAColR = getWord(%ChHVItrA_B,2) - ((getWord(%ChHVItrA_B,2) - getWord(%ChHVItrA_B,3)) * %Sm);

	%Co = ((%CHPosY-%CHPyRelItrB)+%BrPosY) / %ItrB_FI;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrBColL = getWord(%ChHVItrB_B,0) - ((getWord(%ChHVItrB_B,0) - getWord(%ChHVItrB_B,1)) * %Sm);
	%ItrBColR = getWord(%ChHVItrB_B,2) - ((getWord(%ChHVItrB_B,2) - getWord(%ChHVItrB_B,3)) * %Sm);
	
	%Co = ((%CHPosX-%ChPxActItrA)+%BrPosX) / %ItrA_FI;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrARow = %ItrAColL - ((%ItrAColL - %ItrAColR) *%Sm);
	
	%Co = ((%CHPosX-%CHPxRelItrB)+%BrPosX) / %ItrB_FI;
	%Sm = (6 * mPow(%Co,5)) - (15 * mPow(%Co,4)) + (10 * mPow(%Co,3));
	%ItrBRow = %ItrBColL - ((%ItrBColL - %ItrBColR) * %Sm);

	%relZ = (%ItrARow * PTG_Cmplx_EdtNosScaleFltIsldAZ.getValue()) + (%ItrBRow * PTG_Cmplx_EdtNosScaleFltIsldBZ.getValue());
	
	//Edge-FallOff
	%relZ = PTG_GUI_Noise_EdgeFallOff(%ChPosX+%BrPosX,%ChPosY+%BrPosY,%relZ);

	%tempZ = mFloor(%relZ / %BrZSize) * %BrZSize;
	
	if(%tempZ > %SecZ_FI)
	{
		%fltIsldPass = true;
		%BrH_FIb = $StrPrevArrayHV_FltIsldsBTop[%BrPosX,%BrPosY] = %FIb_Off + (mFloor((getMax(%tempZ,%MinBrZSnap) - %SecZ_FI) / %BrZSize) * %BrZSize);
		$StrPrevArrayHV_FltIsldsBBtm[%BrPosX,%BrPosY] = %FIb_Off - (mFloor(((getMax(%tempZ,%MinBrZSnap) - %SecZ_FIZ) * 1.5) / %BrZSize) * %BrZSize);
	}
	
	//////////////////////////////////////////////////
	
	if(%fltIsldPass)
	{
		if(%BrH_FIa > %BrH_FIb)
			%BrH_FI = %BrH_FIa;
		else
			%BrH_FI = %BrH_FIb;
		%BrH_Cl = $StrPrevArrayHV_Clouds[%BrPosX,%BrPosY];
		
		//%BrCount += 4;
			
		%col = PTG_GUI_ApplyFogDepth(PTG_Cmplx_SwFltIsldsCol.color,%BrH_FI);
		%gridStartY = getWord(PTG_Cmplx_SwPrevTopWndw_ChGrid.extent,1);
		%startPosX = PTG_Cmplx_EdtGridXStart.getValue();
		%startPosY = PTG_Cmplx_EdtGridYStart.getValue();
		%sw = PTG_GUI_GenGUIObj(%ChPosX-%startPosX+%BrPosX SPC %gridStartY-(%ChPosY-%startPosY+%BrPosY)-%BrXYSize,%BrXYSize SPC %BrXYSize,%col,"Swatch");

		if(PTG_Cmplx_PopUpTerType.getValue() $= "SkyLands")
			%BrH_rel = $StrPrevArrayHV_SkyLands[%BrPosX,%BrPosY];
		else
			%BrH_rel = $StrPrevArrayHV_Mountains[%BrPosX,%BrPosY];
		
		if(%BrH_FI < %BrH_rel)
		{
			if(%BrH_FI < %BrH_Cl)
			{
				if(%BrH_FIa > %BrH_FIb)
					PTG_Cmplx_SwPrevTopWndw_FltIsldD_A.add(%sw);
				else
					PTG_Cmplx_SwPrevTopWndw_FltIsldD_B.add(%sw);
			}
			else
			{
				if(%BrH_Cl > 0)
				{
					if(%BrH_FIa > %BrH_FIb)
						PTG_Cmplx_SwPrevTopWndw_FltIsldC_A.add(%sw);
					else
						PTG_Cmplx_SwPrevTopWndw_FltIsldC_B.add(%sw);
				}
				else
				{
					if(%BrH_FIa > %BrH_FIb)
						PTG_Cmplx_SwPrevTopWndw_FltIsldD_A.add(%sw);
					else
						PTG_Cmplx_SwPrevTopWndw_FltIsldD_B.add(%sw);
				}
			}
		}
		else
		{
			if(%BrH_FI < %BrH_Cl)
			{
				if(%BrH_FIa > %BrH_FIb)
					PTG_Cmplx_SwPrevTopWndw_FltIsldB_A.add(%sw);
				else
					PTG_Cmplx_SwPrevTopWndw_FltIsldB_B.add(%sw);
			}
			else
			{
				if(%BrH_Cl > 0)
				{
					if(%BrH_FIa > %BrH_FIb)
						PTG_Cmplx_SwPrevTopWndw_FltIsldA_A.add(%sw);
					else
						PTG_Cmplx_SwPrevTopWndw_FltIsldA_B.add(%sw);
				}
				else
				{
					if(%BrH_FIa > %BrH_FIb)
						PTG_Cmplx_SwPrevTopWndw_FltIsldB_A.add(%sw);
					else
						PTG_Cmplx_SwPrevTopWndw_FltIsldB_B.add(%sw);
				}
			}
		}
	}
	
	//////////////////////////////////////////////////
	
	if((%BrPosX += %BrXYSize) >= %ChSize)
	{
		%BrPosX = 0;
		
		if((%BrPosY += %BrXYSize) >= %ChSize)
		{
			for(%BrPosY = 0; %BrPosY < %ChSize; %BrPosY += %BrXYSize)
			{
				for(%BrPosX = 0; %BrPosX < %ChSize; %BrPosX += %BrXYSize)
				{
					//Layer A
					%FIaH = $StrPrevArrayHV_FltIsldsATop[%BrPosX,%BrPosY];
					%FIaHb = $StrPrevArrayHV_FltIsldsABtm[%BrPosX,%BrPosY];
					
					//Adjacent gap fill
					if(%FIaH > 0 && %FIaHb > 0)
					{
						%FIaH_L = $StrPrevArrayHV_FltIsldsATop[%BrPosX-%BrXYSize,%BrPosY];
						%FIaH_R = $StrPrevArrayHV_FltIsldsATop[%BrPosX+%BrXYSize,%BrPosY];
						%FIaH_D = $StrPrevArrayHV_FltIsldsATop[%BrPosX,%BrPosY-%BrXYSize];
						%FIaH_U = $StrPrevArrayHV_FltIsldsATop[%BrPosX,%BrPosY+%BrXYSize];
						
						%FIaH_Lb = $StrPrevArrayHV_FltIsldsABtm[%BrPosX-%BrXYSize,%BrPosY];
						%FIaH_Rb = $StrPrevArrayHV_FltIsldsABtm[%BrPosX+%BrXYSize,%BrPosY];
						%FIaH_Db = $StrPrevArrayHV_FltIsldsABtm[%BrPosX,%BrPosY-%BrXYSize];
						%FIaH_Ub = $StrPrevArrayHV_FltIsldsABtm[%BrPosX,%BrPosY+%BrXYSize];
						
						%fltIsldASecCut = %FIaH_L == 0 || %FIaH_R == 0 || %FIaH_D == 0 || %FIaH_U == 0;
			
						//Edges of chunks aren't calculated, so take this into account
						if(%FIaH_L == 0) %FIaH_L = %FIaH;
						if(%FIaH_R == 0) %FIaH_R = %FIaH;
						if(%FIaH_D == 0) %FIaH_D = %FIaH;
						if(%FIaH_U == 0) %FIaH_U = %FIaH;
						
						if(%FIaH_Lb == 0) %FIaH_Lb = %FIaHb;
						if(%FIaH_Rb == 0) %FIaH_Rb = %FIaHb;
						if(%FIaH_Db == 0) %FIaH_Db = %FIaHb;
						if(%FIaH_Ub == 0) %FIaH_Ub = %FIaHb;
						
						%FIaH_min = getMin(%FIaH_L,getMin(%FIaH_R,getMin(%FIaH_D,%FIaH_U)));
						%FIaH_maxB = getMax(%FIaH_Lb,getMax(%FIaH_Rb,getMax(%FIaH_Db,%FIaH_Ub)));
						%BrCount++;
						
						if(PTG_Cmplx_BmpFltIsldsBr.ModTer && PTG_Cmplx_PopUpModTerType_fltislds.getValue() !$= "Cubes") //ModTer
							%BrCount++;

						if((%FIaH_min < %FIaH_maxB) || %fltIsldASecCut)
						{
							if((%FIaH - %BrZSize) > %FIaHb)
								%BrCount += PTG_GUI_GapFillCount((%FIaH - %BrZSize) - %FIaHb,"Plate",%BrXYSize);
						}
						else
						{
							if((%FIaH - %FIaH_min) > 0)
								%BrCount += PTG_GUI_GapFillCount(((%FIaH - %BrZSize) - %FIaH_min) + %BrXYSize,"Cube",%BrXYSize);
							if((%FIaH_maxB - %FIaH) > 0)
								%BrCount += PTG_GUI_GapFillCount((%FIaH_maxB - %FIaH) + %BrXYSize,"Cube",%BrXYSize);
						}
					}
					
					//////////////////////////////////////////////////
					
					//Layer B
					%FIbH = $StrPrevArrayHV_FltIsldsBTop[%BrPosX,%BrPosY];
					%FIbHb = $StrPrevArrayHV_FltIsldsBBtm[%BrPosX,%BrPosY];
					
					//Adjacent gap fill
					if(%FIbH > 0 && %FIbHb > 0)
					{
						%FIbH_L = $StrPrevArrayHV_FltIsldsBTop[%BrPosX-%BrXYSize,%BrPosY];
						%FIbH_R = $StrPrevArrayHV_FltIsldsBTop[%BrPosX+%BrXYSize,%BrPosY];
						%FIbH_D = $StrPrevArrayHV_FltIsldsBTop[%BrPosX,%BrPosY-%BrXYSize];
						%FIbH_U = $StrPrevArrayHV_FltIsldsBTop[%BrPosX,%BrPosY+%BrXYSize];
						
						%FIbH_Lb = $StrPrevArrayHV_FltIsldsBBtm[%BrPosX-%BrXYSize,%BrPosY];
						%FIbH_Rb = $StrPrevArrayHV_FltIsldsBBtm[%BrPosX+%BrXYSize,%BrPosY];
						%FIbH_Db = $StrPrevArrayHV_FltIsldsBBtm[%BrPosX,%BrPosY-%BrXYSize];
						%FIbH_Ub = $StrPrevArrayHV_FltIsldsBBtm[%BrPosX,%BrPosY+%BrXYSize];
						
						%fltIsldBSecCut = %FIbH_L == 0 || %FIbH_R == 0 || %FIbH_D == 0 || %FIbH_U == 0;
						
						//Edges of chunks aren't calculated, so take this into account
						if(%FIbH_L == 0) %FIbH_L = %FIbH;
						if(%FIbH_R == 0) %FIbH_R = %FIbH;
						if(%FIbH_D == 0) %FIbH_D = %FIbH;
						if(%FIbH_U == 0) %FIbH_U = %FIbH;
						
						if(%FIbH_Lb == 0) %FIbH_Lb = %FIbHb;
						if(%FIbH_Rb == 0) %FIbH_Rb = %FIbHb;
						if(%FIbH_Db == 0) %FIbH_Db = %FIbHb;
						if(%FIbH_Ub == 0) %FIbH_Ub = %FIbHb;
						
						%FIbH_min = getMin(%FIbH_L,getMin(%FIbH_R,getMin(%FIbH_D,%FIbH_U)));
						%FIbH_maxB = getMax(%FIbH_Lb,getMax(%FIbH_Rb,getMax(%FIbH_Db,%FIbH_Ub)));
						%BrCount++;
						
						if(PTG_Cmplx_BmpFltIsldsBr.ModTer && PTG_Cmplx_PopUpModTerType_fltislds.getValue() !$= "Cubes") //ModTer
							%BrCount++;
						
						if((%FIbH_min < %FIbH_maxB) || %fltIsldBSecCut)
						{
							if((%FIbH - %BrZSize) > %FIbHb)
								%BrCount += PTG_GUI_GapFillCount((%FIbH - %BrZSize) - %FIbHb,"Plate",%BrXYSize);
						}
						else
						{
							if((%FIbH - %FIbH_min) > 0)
								%BrCount += PTG_GUI_GapFillCount(((%FIbH - %BrZSize) - %FIbH_min) + %BrXYSize,"Cube",%BrXYSize);
							if((%FIbH_maxB - %FIbH) > 0)
								%BrCount += PTG_GUI_GapFillCount((%FIbH_maxB - %FIbH) + %BrXYSize,"Cube",%BrXYSize);
						}
					}
				}
			}
			
			
			PTG_GUI_Routine_Append("","","","","","Delete"); //
			schedule(0,0,PTG_GUI_Routine_Append,%ChPosX,%ChPosY,%xmod++,%ymod,%BrCount,"Next");
			return;
		}
	}

	schedule(0,0,PTG_GUI_Noise_FltIslds,%CHPosX,%CHPosY,%xmod,%ymod,%BrPosX,%BrPosY,%BrCount);
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function PTG_GUI_Chunk_Lakes(%CHPosX,%CHPosY,%xmod,%ymod,%BrPosX,%BrPosY,%BrCount) //add cave check //!!!make sure chunk size isn't < water brick size!!!
{
	%genWater = false;
	%genForCaves = false;
	%lowestHV = $StrPrevArrayHV_Mountains[%BrPosX-8,%BrPosY-8]; //set initial lowest point for water gen
	%LakesH = PTG_Cmplx_SldrWaterLevel.getValue();
	%ChSize = getWord("16 32 64 128 256",mClamp(PTG_Cmplx_PopupChSize.getSelected(),0,4));
	%BrXYSize = getMax(PTG_Cmplx_BmpTerBr.BrickID.brickSizeX * 0.5,1);
	%BrZSize = PTG_Cmplx_BmpTerBr.BrickID.brickSizeZ * 0.2;
	
	for(%ScanPosY = 0; %ScanPosY < 16; %ScanPosY += %BrXYSize)
	{
		for(%ScanPosX = 0; %ScanPosX < 16; %ScanPosX += %BrXYSize)
		{
			%BrH_cf = $StrPrevArrayHV_Mountains[%BrPosX+%ScanPosX,%BrPosY+%ScanPosY];
			%CaveH_btm = $StrPrevArrayHV_CavesBtm[%BrPosX+%ScanPosX,%BrPosY+%ScanPosY];
			%CaveH_top = $StrPrevArrayHV_CavesTop[%BrPosX+%ScanPosX,%BrPosY+%ScanPosY];

			if((%BrH_cf+%BrZSize) < %lowestHV)
				%lowestHV = %BrH_cf;
			
			if((%BrH_cf+%BrZSize) < %LakesH)
				%genWater = true;
			
			if(PTG_Cmplx_ChkEnabCaves.getValue() && %CaveH_top > 0 && ((%CaveH_btm < %LakesH && (%CaveH_top + %BrZSize) > %LakesH) || %genForCaves))
			{
				%genWater = true;
				%genForCaves = true;

				if(%CaveH_btm > 0 && %CaveH_btm < %lowestHV)
					%lowestHV = %CaveH_btm;
			}	
		}
	}

	//////////////////////////////////////////////////
	
	if(%genWater)
	{
		%gridStartY = getWord(PTG_Cmplx_SwPrevTopWndw_ChGrid.extent,1);
		%col = PTG_GUI_Chunk_FigureColPri("","","","Water",$StrPrevArrayHV_CustomBiomeA[%BrPosX,%BrPosY],$StrPrevArrayHV_CustomBiomeB[%BrPosX,%BrPosY],$StrPrevArrayHV_CustomBiomeC[%BrPosX,%BrPosY]);
		%col = PTG_GUI_ApplyFogDepth(%col,%LakesH);
		%startPosX = PTG_Cmplx_EdtGridXStart.getValue();
		%startPosY = PTG_Cmplx_EdtGridYStart.getValue();
		%sw = PTG_GUI_GenGUIObj(%ChPosX-%startPosX+%BrPosX SPC %gridStartY-(%ChPosY-%startPosY+%BrPosY)-16,16 SPC 16,%col,"Swatch");
		
		PTG_Cmplx_SwPrevTopWndw_Wat.add(%sw);
		
		//Brick Count
		//%lowestHV = mCeil(%lowestHV / 4) * 4;
		
		//Adjacent gap fill
			%gapDist = %lowestHV + %BrZSize; //"+ %BrZSize"???
		%brRemA = mFloor(%gapDist / 32); //x32
			%gapDist -= (%brRemA * 32);
		%brRemB = mFloor(%gapDist / 16); //x16
			%gapDist -= (%brRemA * 16);
		%brRemC = mFloor(%gapDist / 4); //normal fill size
		%BrCount += getMax(%brRemA + %brRemB + %brRemC,0);
	}
	
	//////////////////////////////////////////////////
	
	if((%BrPosX += 16) >= %ChSize)
	{
		%BrPosX = 0;
		
		if((%BrPosY += 16) >= %ChSize)
		{
			schedule(0,0,PTG_GUI_Routine_Append,%ChPosX,%ChPosY,%xmod,%ymod,%BrCount,"Clouds");
			return;
		}
	}

	schedule(0,0,PTG_GUI_Chunk_Lakes,%CHPosX,%CHPosY,%xmod,%ymod,%BrPosX,%BrPosY,%BrCount);
}

