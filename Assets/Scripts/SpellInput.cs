﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Valve.VR.Extras;
using FreeDraw;
using UnityEditorInternal;

public class SpellInput : SymbolRecognition
{
   
    [Header("")]
    public Hand hand;
    public SteamVR_Action_Boolean action_Boolean;
    public SteamVR_Action_Boolean action_grab;
    public SteamVR_Action_Vibration action_vibration;
    public SteamVR_Input_Sources input_Sources;

    [Header("")]
    public Transform spellHand;
    RaycastHit hit;

    public Drawable drawable;

    bool newLine = true;

	
	// Update is called once per frame
	void Update()
    {
        if (!action_Boolean.state && action_grab.state)
        {
            Ray raycast = new Ray(hand.skeleton.indexTip.position, hand.skeleton.indexTip.right);
            if (Physics.Raycast(raycast, out hit, 0.05f))
		    {
                if (hit.transform.CompareTag("Box"))
			    {
                    DrawSymbol(hit.point);    
                }
                else
				{
                    NotDrawing();
                }
			}
			else
			{
                NotDrawing();
            }
		}
        else if (!action_Boolean.state && !action_grab.state)
		{
            Ray raycast = new Ray(hand.skeleton.indexTip.position, hand.skeleton.indexTip.forward);
            if (Physics.Raycast(raycast, out hit, 0.1f))
            {
                if (hit.transform.CompareTag("CastSpell"))
                {
                    newLine = true;
                    drawable.ResetCanvas();
                    NotDrawing();
                    DrawingInputStart(GestureInput.Cast);
                }
            }

        }
        else
        {
            NotDrawing();
        }
    }

    void DrawSymbol(Vector3 drawPosition)
	{
        // Make the drawing visiable
        drawable.isDrawing = true;
        drawable.isInDrawSpace = true;
		drawable.drawCoords = (Vector2)drawPosition;

        // Check what symbol has been drawn
        if (newLine)
		{
            newLine = false;
            DrawingInputStart(GestureInput.Draw);
		}
		else
		{
            DrawingInputMove(drawPosition);
		}


	}
    void NotDrawing()
	{
        drawable.isDrawing = false;
        drawable.isInDrawSpace = false;
    }

}
