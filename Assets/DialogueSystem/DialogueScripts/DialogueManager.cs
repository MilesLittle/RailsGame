using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using TMPro;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor.Build.Content;

public enum GameState
{
    Playing,
    Dialogue
}

public enum dialogueCameraPath {
   camFrontFaceNorm,
   camFrontFaceFar,
   camFrontFaceClose,

   camBackFaceNorm,
   camBackFaceFar,
   camBackFaceClose,

   camRotateCWNorm1,
   camRotateCWFar1,
   camRotateCWClose1,

   camRotateCWNorm2,
   camRotateCWFar2,
   camRotateCWClose2,

   camRotateCWNorm3,
   camRotateCWFar3,
   camRotateCWClose3,

   camRotateCCWNorm1,
   camRotateCCWFar1,
   camRotateCCWClose1,

   camRotateCCWNorm2,
   camRotateCCWFar2,
   camRotateCCWClose2,

   camRotateCCWNorm3,
   camRotateCCWFar3,
   camRotateCCWClose3,

    camSpiralUpCWNorm1,
   camSpiralUpCWFar1,
   camSpiralUpCWClose1,

   camSpiralUpCWNorm2,
   camSpiralUpCWFar2,
   camSpiralUpCWClose2,

   camSpiralUpCWNorm3,
   camSpiralUpCWFar3,
   camSpiralUpCWClose3,

    camSpiralUpCCWNorm1,
   camSpiralUpCCWFar1,
   camSpiralUpCCWClose1,

   camSpiralUpCCWNorm2,
   camSpiralUpCCWFar2,
   camSpiralUpCCWClose2,

   camSpiralUpCCWNorm3,
   camSpiralUpCCWFar3,
   camSpiralUpCCWClose3,

    camSpiralDownCWNorm1,
   camSpiralDownCWFar1,
   camSpiralDownCWClose1,

   camSpiralDownCWNorm2,
   camSpiralDownCWFar2,
   camSpiralDownCWClose2,

   camSpiralDownCWNorm3,
   camSpiralDownCWFar3,
   camSpiralDownCWClose3,

    camSpiralDownCCWNorm1,
   camSpiralDownCCWFar1,
   camSpiralDownCCWClose1,

   camSpiralDownCCWNorm2,
   camSpiralDownCCWFar2,
   camSpiralDownCCWClose2,

   camSpiralDownCCWNorm3,
   camSpiralDownCCWFar3,
   camSpiralDownCCWClose3,
   
   camZoomFarToNorm1,
   camZoomFarToClose1,
   camZoomNormToClose1,
   
   camZoomFarToNorm2,
   camZoomFarToClose2,
   camZoomNormToClose2,

   camZoomFarToNorm3,
   camZoomFarToClose3,
   camZoomNormToClose3,

    camZoomCloseToFar1,
    camZoomCloseToNorm1,
    camZoomNormToFar1,

    camZoomCloseToFar2,
    camZoomCloseToNorm2,
    camZoomNormToFar2,

    camZoomCloseToFar3,
    camZoomCloseToNorm3,
    camZoomNormToFar3,

    camLoopRotateCWNorm1,
   camLoopRotateCWFar1,
   camLoopRotateCWClose1,

   camLoopRotateCWNorm2,
   camLoopRotateCWFar2,
   camLoopRotateCWClose2,

   camLoopRotateCWNorm3,
   camLoopRotateCWFar3,
   camLoopRotateCWClose3,

   camLoopRotateCCWNorm1,
   camLoopRotateCCWFar1,
   camLoopRotateCCWClose1,

   camLoopRotateCCWNorm2,
   camLoopRotateCCWFar2,
   camLoopRotateCCWClose2,

   camLoopRotateCCWNorm3,
   camLoopRotateCCWFar3,
   camLoopRotateCCWClose3,

    camLoopSpiralUpCWNorm1,
   camLoopSpiralUpCWFar1,
   camLoopSpiralUpCWClose1,

   camLoopSpiralUpCWNorm2,
   camLoopSpiralUpCWFar2,
   camLoopSpiralUpCWClose2,

   camLoopSpiralUpCWNorm3,
   camLoopSpiralUpCWFar3,
   camLoopSpiralUpCWClose3,

    camLoopSpiralUpCCWNorm1,
   camLoopSpiralUpCCWFar1,
   camLoopSpiralUpCCWClose1,

   camLoopSpiralUpCCWNorm2,
   camLoopSpiralUpCCWFar2,
   camLoopSpiralUpCCWClose2,

   camLoopSpiralUpCCWNorm3,
   camLoopSpiralUpCCWFar3,
   camLoopSpiralUpCCWClose3,

    camLoopSpiralDownCWNorm1,
   camLoopSpiralDownCWFar1,
   camLoopSpiralDownCWClose1,

   camLoopSpiralDownCWNorm2,
   camLoopSpiralDownCWFar2,
   camLoopSpiralDownCWClose2,

   camLoopSpiralDownCWNorm3,
   camLoopSpiralDownCWFar3,
   camLoopSpiralDownCWClose3,

    camLoopSpiralDownCCWNorm1,
   camLoopSpiralDownCCWFar1,
   camLoopSpiralDownCCWClose1,

   camLoopSpiralDownCCWNorm2,
   camLoopSpiralDownCCWFar2,
   camLoopSpiralDownCCWClose2,

   camLoopSpiralDownCCWNorm3,
   camLoopSpiralDownCCWFar3,
   camLoopSpiralDownCCWClose3

};
   

  
public class DialogueManager : MonoBehaviour
{
public string dialogueDataPath = "DialogueData";

[OdinSerialize, ReadOnly] public Dictionary<string, DialogueData> allGameDialogue = new Dictionary<string, DialogueData>();


public List<GameObject> objectsInScene = new List<GameObject>(); 

private DialogueData currentDialogue;
private byte entryAmount;
private sbyte entryIndex;

[SerializeField] private Canvas dialogueBox;
[SerializeField] private TextMeshProUGUI textField;

public Camera sceneCamera;
public  Camera portraitCamera;
private CameraMovement moveCam;
private Movement movementScript;
   [SerializeField] private GameController gameController;




/*void gatherAllObjects() {  //This method gathers all the current gameObjects in the scene that have the capacity for dialogue
    foreach(GameObject obj in FindObjectsOfType<GameObject>()) {

        objectsInScene.Add(obj);
    }
} */

void Awake() {
    foreach(DialogueData dia in Resources.LoadAll<DialogueData>(dialogueDataPath)) {
        
        allGameDialogue.Add(dia.key, dia);
            Debug.Log("Loaded Dialogue: " + dia.key);

        }

    dialogueBox.enabled = false;
    textField.enabled = false;
    moveCam = this.GetComponent<CameraMovement>();
    movementScript = this.GetComponent<Movement>();
}

void Start() {
   
   
    
}

    
    public void StartDialogue(DialogueData dia) {

        //This Method changes the game state to the dialogue state and process the first dialogueEntry while setting everything else up.
        portraitCamera.enabled = true;
        dialogueBox.enabled = true;
        textField.enabled = true;
        textField.text = "";
      
        currentDialogue = dia;
        entryAmount = (byte)dia.entries.Length;
        Debug.Log(entryAmount);
        entryIndex = -1;


        

        
        gameController.previousState = gameController.gameState;
        gameController.gameState = GamePlayState.Dialogue;

    }

    private void NextLine() {
        textField.text = "";
        entryIndex += 1;
         sceneCamera = currentDialogue.entries[entryIndex].sceneCamera;
            if(currentDialogue.entries[entryIndex].sceneFocus != null) {
                processCameraPath(currentDialogue.entries[entryIndex].sCamPath, sceneCamera, currentDialogue.entries[entryIndex].sceneFocus);
            } 

             portraitCamera = currentDialogue.entries[entryIndex].portraitCamera;
        
       if(currentDialogue.entries[entryIndex].portraitFocus != null) {
                processCameraPath(currentDialogue.entries[entryIndex].pCamPath, portraitCamera,currentDialogue.entries[entryIndex].portraitFocus);
            } 
textField.text = currentDialogue.entries[entryIndex].line;
    }

    private void EndDialogue() {
       gameController.gameState = gameController.previousState;
        portraitCamera.enabled = false;
        dialogueBox.enabled = false;
        textField.text = "";
        textField.enabled = false;
    }

    void Update() {
        if(gameController.gameState == GamePlayState.Dialogue) {
            if(Input.GetKeyDown(KeyCode.Return)) {
                if(entryIndex == entryAmount -1 ) {
                    EndDialogue();
                } else {
                    NextLine();
                }
            }
        }
    }

    void processCameraPath(dialogueCameraPath path, Camera cam, GameObject focus) {
        switch(path) {
            //Front Facing Cam Statements Begin
            case dialogueCameraPath.camFrontFaceClose:
            moveCam.frontFace(cam, focus, 1f);
            
            break;

             case dialogueCameraPath.camFrontFaceNorm:
             moveCam.frontFace(cam, focus, 1f);
            
            break;

            case dialogueCameraPath.camFrontFaceFar:
            moveCam.frontFace(cam, focus, 10f);
            
            break;

            //Front Facing Cam Statements End

            //Back Facing Cam Statements Begin


             case dialogueCameraPath.camBackFaceClose:
            break;

              case dialogueCameraPath.camBackFaceNorm:
            break;

              case dialogueCameraPath.camBackFaceFar:
            break;

            //Back Facing Cam Statements End

            //Rotate Clockwise Begin


            case dialogueCameraPath.camRotateCWNorm1:
            break;

            case dialogueCameraPath.camRotateCWClose1:
            break;

            case dialogueCameraPath.camRotateCWFar1:
            break;


            case dialogueCameraPath.camRotateCWNorm2:
            break;

            case dialogueCameraPath.camRotateCWClose2:
            break;

            case dialogueCameraPath.camRotateCWFar2:
            break;


            case dialogueCameraPath.camRotateCWNorm3:
            break;
            
            case dialogueCameraPath.camRotateCWClose3:
            break;

            case dialogueCameraPath.camRotateCWFar3:
            break;

            //Rotate Clockwise End
           //Rotate CounterClockwise Begin 


             case dialogueCameraPath.camRotateCCWNorm1:
            break;

            case dialogueCameraPath.camRotateCCWClose1:
            break;

            case dialogueCameraPath.camRotateCCWFar1:
            break;


            case dialogueCameraPath.camRotateCCWNorm2:
            break;

            case dialogueCameraPath.camRotateCCWClose2:
            break;

            case dialogueCameraPath.camRotateCCWFar2:
            break;


            case dialogueCameraPath.camRotateCCWNorm3:
            break;
            
            case dialogueCameraPath.camRotateCCWClose3:
            break;

            case dialogueCameraPath.camRotateCCWFar3:
            break;

            //Rotate CounterClockwise End

            //Spiral Up Clockwise Begin

             case dialogueCameraPath.camSpiralUpCWNorm1:
            break;

            case dialogueCameraPath.camSpiralUpCWClose1:
            break;

            case dialogueCameraPath.camSpiralUpCWFar1:
            break;


            case dialogueCameraPath.camSpiralUpCWNorm2:
            break;

            case dialogueCameraPath.camSpiralUpCWClose2:
            break;

            case dialogueCameraPath.camSpiralUpCWFar2:
            break;


            case dialogueCameraPath.camSpiralUpCWNorm3:
            break;
            
            case dialogueCameraPath.camSpiralUpCWClose3:
            break;

            case dialogueCameraPath.camSpiralUpCWFar3:
            break;

            //Spiral Up ClockWise End

            //Spiral Up CounterClockWise Begin


             case dialogueCameraPath.camSpiralUpCCWNorm1:
            break;

            case dialogueCameraPath.camSpiralUpCCWClose1:
            break;

            case dialogueCameraPath.camSpiralUpCCWFar1:
            break;


            case dialogueCameraPath.camSpiralUpCCWNorm2:
            break;

            case dialogueCameraPath.camSpiralUpCCWClose2:
            break;

            case dialogueCameraPath.camSpiralUpCCWFar2:
            break;


            case dialogueCameraPath.camSpiralUpCCWNorm3:
            break;
            
            case dialogueCameraPath.camSpiralUpCCWClose3:
            break;

            case dialogueCameraPath.camSpiralUpCCWFar3:
            break;

            //Spiral Up CounterClockWise End
              //Spiral Down Clockwise Begin

             case dialogueCameraPath.camSpiralDownCWNorm1:
            break;

            case dialogueCameraPath.camSpiralDownCWClose1:
            break;

            case dialogueCameraPath.camSpiralDownCWFar1:
            break;


            case dialogueCameraPath.camSpiralDownCWNorm2:
            break;

            case dialogueCameraPath.camSpiralDownCWClose2:
            break;

            case dialogueCameraPath.camSpiralDownCWFar2:
            break;


            case dialogueCameraPath.camSpiralDownCWNorm3:
            break;
            
            case dialogueCameraPath.camSpiralDownCWClose3:
            break;

            case dialogueCameraPath.camSpiralDownCWFar3:
            break;

            //Spiral Down ClockWise End

            //Spiral Down CounterClockWise Begin


             case dialogueCameraPath.camSpiralDownCCWNorm1:
            break;

            case dialogueCameraPath.camSpiralDownCCWClose1:
            break;

            case dialogueCameraPath.camSpiralDownCCWFar1:
            break;


            case dialogueCameraPath.camSpiralDownCCWNorm2:
            break;

            case dialogueCameraPath.camSpiralDownCCWClose2:
            break;

            case dialogueCameraPath.camSpiralDownCCWFar2:
            break;


            case dialogueCameraPath.camSpiralDownCCWNorm3:
            break;
            
            case dialogueCameraPath.camSpiralDownCCWClose3:
            break;

            case dialogueCameraPath.camSpiralDownCCWFar3:
            break;

            //Spiral Down CounterClockWise End

            //Zoom In Speed 1 Begin
            case dialogueCameraPath.camZoomFarToNorm1:
            break;
            case dialogueCameraPath.camZoomFarToClose1:
            break;
            case dialogueCameraPath.camZoomNormToClose1:
            break;
            //Zoom In Speed 1 End


            //Zoom In Speed 2 Begin
            case dialogueCameraPath.camZoomFarToNorm2:
            break;

            case dialogueCameraPath.camZoomFarToClose2:
            break;

            case dialogueCameraPath.camZoomNormToClose2:
            break;

            //Zoom In Speed 2 End

            //Zoom In Speed 3 Begin
            case dialogueCameraPath.camZoomFarToNorm3:
            break;
            case dialogueCameraPath.camZoomFarToClose3:
            break;
            case dialogueCameraPath.camZoomNormToClose3:
            break;
            //Zoom In Speed 3 End

            //Zoom Out Speed 1 Begin
            case dialogueCameraPath.camZoomCloseToFar1:
            break;

            case dialogueCameraPath.camZoomCloseToNorm1:
            break;

            case dialogueCameraPath.camZoomNormToFar1:
            break;
            //Zoom Out Speed 1 End

            //Zoom Out Speed 2 Begin
             case dialogueCameraPath.camZoomCloseToFar2:
            break;

            case dialogueCameraPath.camZoomCloseToNorm2:
            break;

            case dialogueCameraPath.camZoomNormToFar2:
            break;
            //Zoom Out Speed 2 End

            //Zoom Out Speed 3 Begin
             case dialogueCameraPath.camZoomCloseToFar3:
            break;

            case dialogueCameraPath.camZoomCloseToNorm3:
            break;

            case dialogueCameraPath.camZoomNormToFar3:
            break;
            //Zoom Out Speed 3 End

            //Looping Methods Start
            //Loop Rotate Clockwise Start
            
            case dialogueCameraPath.camLoopRotateCWNorm1:
            break;

            case dialogueCameraPath.camLoopRotateCWClose1:
            break;

            case dialogueCameraPath.camLoopRotateCWFar1:
            break;


            case dialogueCameraPath.camLoopRotateCWNorm2:
            break;

            case dialogueCameraPath.camLoopRotateCWClose2:
            break;

            case dialogueCameraPath.camLoopRotateCWFar2:
            break;


            case dialogueCameraPath.camLoopRotateCWNorm3:
            break;
            
            case dialogueCameraPath.camLoopRotateCWClose3:
            break;

            case dialogueCameraPath.camLoopRotateCWFar3:
            break;

            //Rotate Clockwise End
           //Rotate CounterClockwise Begin 


             case dialogueCameraPath.camLoopRotateCCWNorm1:
            break;

            case dialogueCameraPath.camLoopRotateCCWClose1:
            break;

            case dialogueCameraPath.camLoopRotateCCWFar1:
            break;


            case dialogueCameraPath.camLoopRotateCCWNorm2:
            break;

            case dialogueCameraPath.camLoopRotateCCWClose2:
            break;

            case dialogueCameraPath.camLoopRotateCCWFar2:
            break;


            case dialogueCameraPath.camLoopRotateCCWNorm3:
            break;
            
            case dialogueCameraPath.camLoopRotateCCWClose3:
            break;

            case dialogueCameraPath.camLoopRotateCCWFar3:
            break;

            //Rotate CounterClockwise End

            //Spiral Up Clockwise Begin

             case dialogueCameraPath.camLoopSpiralUpCWNorm1:
            break;

            case dialogueCameraPath.camLoopSpiralUpCWClose1:
            break;

            case dialogueCameraPath.camLoopSpiralUpCWFar1:
            break;


            case dialogueCameraPath.camLoopSpiralUpCWNorm2:
            break;

            case dialogueCameraPath.camLoopSpiralUpCWClose2:
            break;

            case dialogueCameraPath.camLoopSpiralUpCWFar2:
            break;


            case dialogueCameraPath.camLoopSpiralUpCWNorm3:
            break;
            
            case dialogueCameraPath.camLoopSpiralUpCWClose3:
            break;

            case dialogueCameraPath.camLoopSpiralUpCWFar3:
            break;

            //Spiral Up ClockWise End

            //Spiral Up CounterClockWise Begin


             case dialogueCameraPath.camLoopSpiralUpCCWNorm1:
            break;

            case dialogueCameraPath.camLoopSpiralUpCCWClose1:
            break;

            case dialogueCameraPath.camLoopSpiralUpCCWFar1:
            break;


            case dialogueCameraPath.camLoopSpiralUpCCWNorm2:
            break;

            case dialogueCameraPath.camLoopSpiralUpCCWClose2:
            break;

            case dialogueCameraPath.camLoopSpiralUpCCWFar2:
            break;


            case dialogueCameraPath.camLoopSpiralUpCCWNorm3:
            break;
            
            case dialogueCameraPath.camLoopSpiralUpCCWClose3:
            break;

            case dialogueCameraPath.camLoopSpiralUpCCWFar3:
            break;

            //Spiral Up CounterClockWise End
              //Spiral Down Clockwise Begin

             case dialogueCameraPath.camLoopSpiralDownCWNorm1:
            break;

            case dialogueCameraPath.camLoopSpiralDownCWClose1:
            break;

            case dialogueCameraPath.camLoopSpiralDownCWFar1:
            break;


            case dialogueCameraPath.camLoopSpiralDownCWNorm2:
            break;

            case dialogueCameraPath.camLoopSpiralDownCWClose2:
            break;

            case dialogueCameraPath.camLoopSpiralDownCWFar2:
            break;


            case dialogueCameraPath.camLoopSpiralDownCWNorm3:
            break;
            
            case dialogueCameraPath.camLoopSpiralDownCWClose3:
            break;

            case dialogueCameraPath.camLoopSpiralDownCWFar3:
            break;

            //Spiral Down ClockWise End

            //Spiral Down CounterClockWise Begin


             case dialogueCameraPath.camLoopSpiralDownCCWNorm1:
            break;

            case dialogueCameraPath.camLoopSpiralDownCCWClose1:
            break;

            case dialogueCameraPath.camLoopSpiralDownCCWFar1:
            break;


            case dialogueCameraPath.camLoopSpiralDownCCWNorm2:
            break;

            case dialogueCameraPath.camLoopSpiralDownCCWClose2:
            break;

            case dialogueCameraPath.camLoopSpiralDownCCWFar2:
            break;


            case dialogueCameraPath.camLoopSpiralDownCCWNorm3:
            break;
            
            case dialogueCameraPath.camLoopSpiralDownCCWClose3:
            break;

            case dialogueCameraPath.camLoopSpiralDownCCWFar3:
            break;


            default:
            break;

        



            

            
        }
    }


}
