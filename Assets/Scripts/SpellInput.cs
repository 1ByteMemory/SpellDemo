using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Valve.VR.Extras;
using FreeDraw;
using System;

public class SpellInput : SymbolRecognition
{

    [Header("STuff")]
    [Tooltip("The spell drawing range in cm")]
    [Range(1f, 10f)]
    public float drawRangeInCM = 5f; // This is in cm to make it easier to visualise

    public Hand handR;
    public Hand handL;

    public SteamVR_Action_Boolean actionRightGrip = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "GrabGripRight");
    public SteamVR_Action_Boolean actionLeftGrip = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "GrabGripLeft");
    public SteamVR_Action_Boolean actionRightPinch = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "GrabPinchRight");
    public SteamVR_Action_Boolean actionLeftPinch = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "GrabPinchLeft");

    bool handDrawing;
    bool isLeftDrawing;
    bool isrightDrawing;

    enum HandAction
	{
        drawing,
        casting,
        none
	}

    HandAction rightHandAction;
    HandAction leftHandAction;

    bool isLIndexOpen,
        isRIndexOpen,
        isLPalmOpen,
        isRPalmOpen;


    [Header("")]
    RaycastHit hit;

    public Drawable drawable;

    public AudioSource[] sound;

    Sprite sprite; 
    bool newLine = true;

	public override void Start()
	{
        base.Start();
        sound[0] = GetComponentsInParent<AudioSource>()[0];
        sound[1] = GetComponentsInParent<AudioSource>()[1];
	}

	private void OnEnable()
	{
        actionLeftGrip.AddOnChangeListener(HandActionChange, handL.handType);
        actionLeftPinch.AddOnChangeListener(HandActionChange, handL.handType);

		actionRightGrip.AddOnChangeListener(HandActionChange, handR.handType);
        actionRightPinch.AddOnChangeListener(HandActionChange, handR.handType);

    }

	private void OnDisable()
	{
        if (actionLeftGrip != null)
            actionLeftGrip.RemoveOnChangeListener(HandActionChange, handL.handType);

        if (actionLeftPinch != null)
            actionLeftPinch.RemoveOnChangeListener(HandActionChange, handL.handType);

        if (actionRightGrip != null)
            actionRightGrip.RemoveOnChangeListener(HandActionChange, handR.handType);

        if (actionRightPinch != null)
            actionRightPinch.RemoveOnChangeListener(HandActionChange, handR.handType);


	}

	public void HandActionChange(SteamVR_Action_Boolean actionIn, SteamVR_Input_Sources inputSource, bool newValue)
	{
        if (actionIn.GetPath() == actionLeftGrip.GetPath())
        {
            //Debug.Log("lg: " + newValue);
            isLPalmOpen = !newValue;
        }
        else if (actionIn.GetPath() == actionLeftPinch.GetPath())
        {
            //Debug.Log("lp: " + newValue);
            isLIndexOpen = !newValue;
        }
        else if (actionIn.GetPath() == actionRightGrip.GetPath())
        {
            //Debug.Log("rg: " + newValue);
            isRPalmOpen = !newValue;
        }
        else if (actionIn.GetPath() == actionRightPinch.GetPath())
        {
            //Debug.Log("rp: " + newValue);
            isRIndexOpen = !newValue;
        }
		
	}

	// Update is called once per frame
	void Update()
    {
        Vector3 forward = drawable.transform.TransformDirection(Vector3.back);
        
        if (handL != null)
		{
            if (isLIndexOpen && !isLPalmOpen)
                HandDrawing(SteamVR_Input_Sources.LeftHand, handL);

            if (isLIndexOpen && isLPalmOpen)
                SpellCastingCheck(forward, handL.skeleton.middleMetacarpal.position - drawable.transform.position);
		}

        if (handR != null)
		{
            if (isRIndexOpen && !isRPalmOpen)
                HandDrawing(SteamVR_Input_Sources.RightHand, handR);

            if (isRIndexOpen && isRPalmOpen)
                SpellCastingCheck(forward, handR.skeleton.middleMetacarpal.position - drawable.transform.position);
        }
    }

    void HandDrawing(SteamVR_Input_Sources sourceHand, Hand hand)
    {
        
        //Debug.Log(string.Format("grip {0}; pinch {1}", GrapGrip, GrapPinch));

        Ray raycast;

        if (sourceHand == SteamVR_Input_Sources.RightHand)
            raycast = new Ray(hand.skeleton.indexTip.position, hand.skeleton.indexTip.right);
        else 
            raycast = new Ray(hand.skeleton.indexTip.position, -hand.skeleton.indexTip.right);


        // Right Hand
        if (Physics.Raycast(raycast, out hit, drawRangeInCM / 100)) // Divided by 100 to convert it to meters
		{
            
            if (hit.transform.CompareTag("Box"))
            {
                //Debug.Log("drawing");

                handDrawing = true;
                if (!sound[1].isPlaying)
                    sound[1].Play();

                sprite = hit.transform.GetComponent<SpriteRenderer>().sprite;
                DrawSymbol(hit.transform.InverseTransformPoint(hit.point));

            }
            else
            {
                handDrawing = false;
                NotDrawing();
            }
        }
        else
        {
            handDrawing = false;
            NotDrawing();
        }
        
        if (!handDrawing)
		{
            if (sourceHand == SteamVR_Input_Sources.RightHand)
                isrightDrawing = false;
            else
                isLeftDrawing = false;
		}
		else
		{

            if (sourceHand == SteamVR_Input_Sources.RightHand)
                isrightDrawing = true;
            else
                isLeftDrawing = true;
		}
    }

    void SpellCastingCheck(Vector3 forward, Vector3 toOther)
	{
        if (Vector3.Dot(forward, toOther) < 0)
        {
            if (!newLine)
            {
                newLine = true;
                drawable.ResetCanvas();

                sound[0].Play();

                handDrawing = false;

                NotDrawing();
                DrawingInputStart(GestureInput.Cast);

                Instantiate(spells[spellIndex], castPosition.position, Quaternion.LookRotation(transform.forward));


            }
        }
    }

    public Vector2 TextureSpaceCoord(Vector3 worldPos)
    {
        float ppu = sprite.pixelsPerUnit;

        // Local position on the sprite in pixels.
        Vector2 localPos = transform.InverseTransformPoint(worldPos) * ppu;

        // When the sprite is part of an atlas, the rect defines its offset on the texture.
        // When the sprite is not part of an atlas, the rect is the same as the texture (x = 0, y = 0, width = tex.width, ...)
        var texSpacePivot = new Vector2(sprite.rect.x, sprite.rect.y) + sprite.pivot;
        Vector2 texSpaceCoord = texSpacePivot + localPos;

       return texSpaceCoord;
    }

    public Vector2 TextureSpaceUV(Vector3 worldPos)
    {
        Texture2D tex = sprite.texture;
        Vector2 texSpaceCoord = TextureSpaceCoord(worldPos);

        // Pixels to UV(0-1) conversion.
        Vector2 uvs = texSpaceCoord;
        uvs.x /= tex.width;
        uvs.y /= tex.height;


        return uvs;
    }

    void DrawSymbol(Vector3 drawPosition)
	{
        /*
        Texture2D tex = rend.material.mainTexture as Texture2D;
        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= tex.width;
        pixelUV.y *= tex.height;

        tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.black);
        tex.Apply();
        */
        
        
        // Make the drawing visiable
        drawable.isDrawing = true;
        drawable.isInDrawSpace = true;

		//Vector2 localPoint = hit.transform.InverseTransformPoint(drawPosition);
        
        drawable.drawCoords = (Vector2)drawPosition;


        // Check what symbol has been drawn
        if (newLine)
		{
            newLine = false;
            DrawingInputStart(GestureInput.Draw);
		}
		else
		{
            DrawingInputMove((Vector2)drawPosition);
		}

    }
    void NotDrawing()
	{
        if (!isLeftDrawing && !isrightDrawing)
		{
            if (sound[1].isPlaying)
                sound[1].Stop();

            drawable.isDrawing = false;
            drawable.isInDrawSpace = false;
		}
    }

}
