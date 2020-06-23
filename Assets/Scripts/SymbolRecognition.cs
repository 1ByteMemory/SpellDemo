using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDollarGestureRecognizer;
using PDollarDemo;

public class SymbolRecognition : MonoBehaviour
{

    List<PDollarGestureRecognizer.Point> points = new List<PDollarGestureRecognizer.Point>();   // mouse points acquired from the user
    Gesture[] trainingSet = null;   // training set loaded from XML files




    // Start is called before the first frame update
    void Start()
    {
        trainingSet = LoadTrainingSet();
    }

    // Update is called once per frame
    void Update()
    {
        
    }




    /// <summary>
    /// Loads training gesture samples from XML files
    /// </summary>
    /// <returns></returns>
    private Gesture[] LoadTrainingSet()
    {
        List<Gesture> gestures = new List<Gesture>();
        string[] gestureFolders = Directory.GetDirectories(Application.StartupPath + "\\GestureSet");
        foreach (string folder in gestureFolders)
        {
            string[] gestureFiles = Directory.GetFiles(folder, "*.xml");
            foreach (string file in gestureFiles)
                gestures.Add(GestureIO.ReadGesture(file));
        }
        return gestures.ToArray();
    }


    /// <summary>
    /// Save gesture points to file
    /// </summary>
    /// <param name="fileName"></param>
    private void SaveGesture(string gestureName)
    {
        if (points.Count == 0)
            return;
        if (!Directory.Exists(Application.StartupPath + "\\GestureSet\\NewGestures"))
            Directory.CreateDirectory(Application.StartupPath + "\\GestureSet\\NewGestures");
        GestureIO.WriteGesture(
            points.ToArray(),
            gestureName,
            String.Format("{0}\\GestureSet\\NewGestures\\{1}-{2}.xml", Application.StartupPath, gestureName, DateTime.Now.ToFileTime())
        );

        // reload the training set
        this.trainingSet = LoadTrainingSet();
    }




}
