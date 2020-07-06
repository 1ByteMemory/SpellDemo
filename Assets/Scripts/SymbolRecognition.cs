
using System.Collections.Generic;
using UnityEngine;
using PDollarGestureRecognizer;
using PDollarDemo;
using System.Xml.Schema;
using System.IO;
using System.Xml;

public enum GestureInput
{
    Draw,
    Cast,
    Erase
}

public class SymbolRecognition : MonoBehaviour
{

    List<PDollarGestureRecognizer.Point> points = new List<PDollarGestureRecognizer.Point>();   // mouse points acquired from the user
    Gesture[] trainingSet = null;   // training set loaded from XML files

    bool symbolRecognised = true;

    public GameObject Test;

    public string gestureClass;
    public Transform castPosition;
    public GameObject[] spells;

    // Start is called before the first frame update
    public virtual void Start()
    {
        trainingSet = LoadTrainingSet();
    }


    bool isDrawing;
    int strokeIndex = -1;

    /// <summary>
    /// Call this for each new line drawn
    /// </summary>
    /// <param name="gestureInput"></param>
    public void DrawingInputStart(GestureInput gestureInput)
	{
		switch (gestureInput)
		{
            case GestureInput.Draw:
                if (strokeIndex == -1)
                    points = new List<Point>();
                isDrawing = true;
                strokeIndex++;
                break;
            case GestureInput.Cast:
                RecognizeGesture();
                strokeIndex = -1;
                break;
            case GestureInput.Erase:
                strokeIndex = -1;
                isDrawing = false;
                // clear drawing canvas

                break;
        }
	}

    /// <summary>
    /// Call this while drawing
    /// </summary>
    /// <param name="position"></param>
    public void DrawingInputMove(Vector2 position)
	{
        if (!isDrawing)
            return;
        points.Add(new Point(position.x, position.y, strokeIndex));

	}
    
    private void RecognizeGesture()
    {
        Gesture candidate = new Gesture(points.ToArray());
        gestureClass = PointCloudRecognizer.Classify(candidate, trainingSet);
        
        if (Test != null)
            Test.transform.position += Vector3.up * 5;

        ///////////////////////////////////////////////////////
        /// Make a check to see if symbol wasn't regocnised ///
        ///////////////////////////////////////////////////////
        symbolRecognised = true;


        Debug.Log("Recognized as: " + gestureClass);
        SymbolAction();
    }

    /// <summary>
    /// The actions to take based on what symbol was drawn
    /// </summary>
    void SymbolAction()
	{
        if (symbolRecognised)
		{
            
			// Switch case on action to take based on symbol
			switch (gestureClass)
			{
				case "Fire":
                    spellIndex = 0;
					break;
                case "Ice":
                    spellIndex = 1;
                    break;
                case "Earth":
                    spellIndex = 2;
                    break;
			}
            gestureClass = "";
		}
	}
    public int spellIndex;





    #region Load and Save gestures

    /// <summary>
    /// Loads training gesture samples from XML files
    /// </summary>
    /// <returns></returns>
    private Gesture[] LoadTrainingSet()
    {
        Debug.Log(string.Format("loading traing set from: {0}", Application.dataPath));
        
        List<Gesture> gestures = new List<Gesture>();
        
        Object[] gestureFolders = Resources.LoadAll("", typeof (TextAsset));
        
        foreach (var folder in gestureFolders)
        {

            string filename = folder.name;
            TextAsset binary = Resources.Load<TextAsset>(filename);
            
            gestures.Add(GestureIO.ReadGesture(binary.text));
        }
        Debug.Log(gestures);
        return gestures.ToArray();
    }


    /*
    /// <summary>
    /// Save gesture points to file
    /// </summary>
    /// <param name="fileName"></param>
    private void SaveGesture(string gestureName)
    {
        if (points.Count == 0)
            return;
        if (!Directory.Exists(Application.dataPath + "\\GestureSet\\NewGestures"))
            Directory.CreateDirectory(Application.dataPath + "\\GestureSet\\NewGestures");
        GestureIO.WriteGesture(
            points.ToArray(),
            gestureName,
            String.Format("{0}\\GestureSet\\NewGestures\\{1}-{2}.xml", Application.dataPath, gestureName, DateTime.Now.ToFileTime())
        );

        // reload the training set
        this.trainingSet = LoadTrainingSet();
    }
    */
	#endregion


}
