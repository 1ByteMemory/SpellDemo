using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;
using Valve.VR.Extras;

public class SpellInput : MonoBehaviour
{
    public Text boo;
    public Text vib;
    public Text foo;

    [Header("")]
    public Hand hand;
    public SteamVR_Action_Boolean action_Boolean;
    public SteamVR_Action_Boolean action_grab;
    public SteamVR_Action_Vibration action_vibration;
    public SteamVR_Input_Sources input_Sources;

    [Header("")]
    public Transform spellHand;
    RaycastHit hit;


    // Update is called once per frame
    void Update()
    {
        vib.text = action_grab.active.ToString();
        boo.text = action_Boolean.state.ToString();

        Ray raycast = new Ray(hand.skeleton.indexTip.position, hand.skeleton.indexTip.right);
        Debug.DrawRay(hand.skeleton.indexTip.position, hand.skeleton.indexTip.right);
        
        

        if (!action_Boolean.state && action_grab.state)
        {
            if (Physics.Raycast(raycast, out hit, 0.05f))
		    {
                foo.text = hit.transform.name;
                if (hit.transform.CompareTag("Box"))
			    {
                    if (transform.position.y < 10)
                    {
                        DrawSymbol(hit.point);
                    }
                }
            }
		}
    }

    void DrawSymbol(Vector3 drawPosition)
	{

	}

}
