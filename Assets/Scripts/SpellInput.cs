using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Valve.VR.Extras;
using FreeDraw;
using UnityEditorInternal;
using UnityEngine.UI;
//using System.Numerics;
using UnityEngine.SocialPlatforms;

public class SpellInput : SymbolRecognition
{

    [Header("STuff")]
    [Tooltip("The spell drawing range in cm")]
    [Range(1f, 10f)]
    public float drawRangeInCM = 5f; // This is in cm to make it easier to visualise

    public Hand hand;
    public SteamVR_Action_Boolean action_pinch;
    public SteamVR_Action_Boolean action_grab;

    [Header("")]
    RaycastHit hit;

    public Drawable drawable;

    Sprite sprite; 
    bool newLine = true;

	
	// Update is called once per frame
	void Update()
    {
        Debug.DrawRay(hand.skeleton.indexTip.position, hand.skeleton.indexTip.right);

        Vector3 forward = drawable.transform.TransformDirection(Vector3.back);
        Vector3 toOther = hand.skeleton.middleMetacarpal.position - drawable.transform.position;

        
        if (!action_pinch.state && action_grab.state)
        {
            Ray raycast = new Ray(hand.skeleton.indexTip.position, hand.skeleton.indexTip.right);
            if (Physics.Raycast(raycast, out hit, drawRangeInCM / 100)) // Divided by 100 to convert it to meters
		    {
                if (hit.transform.CompareTag("Box"))
			    {
                    sprite = hit.transform.GetComponent<SpriteRenderer>().sprite;
                    
                    DrawSymbol(hit.transform.InverseTransformPoint(hit.point));    
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
        drawable.isDrawing = false;
        drawable.isInDrawSpace = false;
    }

}
