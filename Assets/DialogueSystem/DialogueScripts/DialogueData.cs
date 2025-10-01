using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
[CreateAssetMenu(fileName = "New Dialogue Data", menuName = "Resources/DialogueData")]
public class DialogueData : ScriptableObject
{
   [System.Serializable]
   public class DialogueEntry {
       public Camera sceneCamera;  // the main camera that is responsible for the main view of the scene. Always enabled no matter what
       public Camera portraitCamera; // the sub camera that only activates when dialogue is happen. Frames the speaker's face in the character portrait view. 
       public GameObject sceneFocus;  // the gameobject that the camera is focused on during a given dialgoue entry. Can sometimes be empty and if so, simply will keep whatever direction it was already facing. (SceneCam)
       public cylCoords sCamPos; // where the camera is in relation to its focus. defined in spherical coordinates. (Scene Cam)
       public dialogueCameraPath sCamPath; // defines a mode of movement for the camera. will be a list of preset movement patterns, from camera pans, rotations, shakes, and zooms. Will mostly just be manipulations of sCoords (SceneCam)

       public GameObject portraitFocus; //the focus of the portrait camera. 99% of the time will be whoever is speaking the current line of text.
      public dialogueCameraPath pCamPath;
       public cylCoords pCamPos; //the position of the portrait camera. defined is spherical coordinates, and can simply in a sphere around the object of focus's focus point (usually the objects head/face if some kind of living creature.)

       public string line; //the line being spoken 

       public bool earlyEnd;
public string[] animation; // a list of animations that game objects in the scene will carry out on dialgoue trigger. Will be a class that takes in a gameobject, an animation, and an order number

public string[] animationTarget; // will be coupled into the animation class. Defines whether or not and what an animation is targeted at. For example, a point animation will be targeted at some specific object/direction

public string[] movementPath; // will be coupled into the animation class. Defines whether the object doing the animation is moving positions while doing it. usually reserved for walking/running animations, but could have other use cases. Simply defines a location to move to, and a speed at which it is done.


public string[] expression; // may or may not be coupled into the animation class. Defines the expression the gameobject has on dialgoue trigger

//all animation based things can either be specifically defined, default, or not defined. If not defined, the object will simply keep the same settings as the previous setting.
//If defined, the animations will execute based on the order numbers defined in the animation class. For example, all animations with order #1 will execute simultaneously, then everything with 2 and so on.
// If default, the object will revert to its default position. Every object will have a resting/default animation.
   
 
   }
   

       private void OnEnable()
    {
        if (entries == null || entries.Length == 0)
        {
            // Initialize the entries array with one default entry
            entries = new DialogueEntry[1];
            entries[0] = new DialogueEntry();
        }

        foreach (var entry in entries)
        {
            if (entry.sceneCamera == null)
            {
                entry.sceneCamera = FindDefaultSceneCamera();
            }
            if (entry.portraitCamera == null)
            {
                entry.portraitCamera = FindDefaultPortraitCamera();
            }
        }
    }

     private Camera FindDefaultSceneCamera()
    {
        // Find the default scene camera by tag or name
        // Example: return Camera.main;
        return GameObject.FindWithTag("MainCamera")?.GetComponent<Camera>();
    }

    private Camera FindDefaultPortraitCamera()
    {
        // Find the default portrait camera by tag or name
        // Example: return GameObject.Find("PortraitCamera")?.GetComponent<Camera>();
        return GameObject.FindWithTag("PortraitCamera")?.GetComponent<Camera>();
    }
   
public string key;
   public DialogueEntry[] entries;
}
