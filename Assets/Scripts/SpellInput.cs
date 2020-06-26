using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Valve.VR.Extras;
using FreeDraw;

public class SpellInput : SymbolRecognition
{

    [Header("STuff")]
    [Tooltip("The spell drawing range in cm")]
    [Range(1f, 10f)]
    public float drawRangeInCM = 5f; // This is in cm to make it easier to visualise

    public Hand handR;
    public Hand handL;

    bool handRDrawing;
    bool handLDrawing;

    [Header("")]
    RaycastHit hitR;
    RaycastHit hitL;

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


	// Update is called once per frame
	void Update()
    {

        Vector3 forward = drawable.transform.TransformDirection(Vector3.back);
        Vector3 toOtherR = handR.skeleton.middleMetacarpal.position - drawable.transform.position;
        Vector3 toOtherL = handL.skeleton.middleMetacarpal.position - drawable.transform.position;

        bool LeftGrapGrip = handL.grabGripAction.state;
        bool LeftGrapPinch = handL.grabPinchAction.state;

        bool RightGrapGrip = handR.grabGripAction.state;
        bool RightGrapPinch = handR.grabPinchAction.state;
        
        if (!RightGrapPinch && RightGrapGrip)
        {
            Ray raycastR = new Ray(handR.skeleton.indexTip.position, handR.skeleton.indexTip.right);
			

            // Right Hand
            if (Physics.Raycast(raycastR, out hitR, drawRangeInCM / 100)) // Divided by 100 to convert it to meters
            {
                if (hitR.transform.CompareTag("Box"))
			    {
                    handRDrawing = true;
					if (!sound[1].isPlaying)
						sound[1].Play();

					sprite = hitR.transform.GetComponent<SpriteRenderer>().sprite;
                    DrawSymbol(hitR.transform.InverseTransformPoint(hitR.point));    

                }
                else
				{
                    handRDrawing = false;
                    NotDrawing();
                }
			}
			else
			{
                handRDrawing = false;
                NotDrawing();
			}
        }

        if (!LeftGrapPinch && LeftGrapGrip)
		{
            Ray raycastL = new Ray(handL.skeleton.indexTip.position, -handL.skeleton.indexTip.right);

            if (Physics.Raycast(raycastL, out hitL, drawRangeInCM / 100))
            {
                if (hitL.transform.CompareTag("Box"))
                {
                    handLDrawing = true;

                    if (!sound[1].isPlaying)
                        sound[1].Play();

                    sprite = hitL.transform.GetComponent<SpriteRenderer>().sprite;
                    DrawSymbol(hitL.transform.InverseTransformPoint(hitL.point));

                }
                else
                {
                    handLDrawing = false;
                    NotDrawing();
                }
            }
            else
            {
                handLDrawing = false;
                NotDrawing();
            }
        }

        if (Vector3.Dot(forward, toOtherR) < 0 || Vector3.Dot(forward, toOtherL) < 0)
        {
            if ((!RightGrapPinch && !RightGrapGrip) || (!LeftGrapPinch && !LeftGrapGrip))
            {
                if (!newLine)
                {
                    newLine = true;
                    drawable.ResetCanvas();

                    sound[0].Play();

                    handRDrawing = false;
                    handLDrawing = false;

                    NotDrawing();
                    DrawingInputStart(GestureInput.Cast);
                }
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
        if (!handLDrawing && !handRDrawing)
		{
            if (sound[1].isPlaying)
                sound[1].Stop();

            drawable.isDrawing = false;
            drawable.isInDrawSpace = false;
		}
    }

}
