using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Valve.VR.Extras;
using FreeDraw;
using UnityEditorInternal;
using UnityEngine.UI;

public class SpellInput : SymbolRecognition
{
    [Header("DEBUG INFO")]
    public Text text;
    public Text pinch;
    public Text grab;


    [Header("STuff")]
    public Hand hand;
    public SteamVR_Action_Boolean action_pinch;
    public SteamVR_Action_Boolean action_grab;

    [Header("")]
    RaycastHit hit;

    public Drawable drawable;

    bool newLine = true;

	
	// Update is called once per frame
	void Update()
    {
        Debug.DrawRay(hand.skeleton.indexTip.position, hand.skeleton.indexTip.right);

        Vector3 forward = drawable.transform.TransformDirection(Vector3.back);
        Vector3 toOther = hand.skeleton.middleMetacarpal.position - drawable.transform.position;

        pinch.text = action_pinch.state.ToString();
        grab.text = action_grab.state.ToString();

        if (!action_pinch.state && action_grab.state)
        {
            text.text = "drawing";
            Ray raycast = new Ray(hand.skeleton.indexTip.position, hand.skeleton.indexTip.right);
            if (Physics.Raycast(raycast, out hit, 0.01f))
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
        else if (Vector3.Dot(forward, toOther) < 0)
        {
            if (!action_pinch.state && !action_grab.state)
            {
                text.text = "Casting";
                if (!newLine)
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
