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

   camSpiralCWNorm1,
   camSpiralCWFar1,
   camSpiralCWClose1,

   camSpiralCWNorm2,
   camSpiralCWFar2,
   camSpiralCWClose2,

   camSpiralCWNorm3,
   camSpiralCWFar3,
   camSpiralCWClose3,

   camSpiralCCWNorm1,
   camSpiralCCWFar1,
   camSpiralCCWClose1,

   camSpiralCCWNorm2,
   camSpiralCCWFar2,
   camSpiralCCWClose2,

   camSpiralCCWNorm3,
   camSpiralCCWFar3,
   camSpiralCCWClose3,

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

    camLoopSpiralCWNorm1,
   camLoopSpiralCWFar1,
   camLoopSpiralCWClose1,

   camLoopSpiralCWNorm2,
   camLoopSpiralCWFar2,
   camLoopSpiralCWClose2,

   camLoopSpiralCWNorm3,
   camLoopSpiralCWFar3,
   camLoopSpiralCWClose3,

   camLoopSpiralCCWNorm1,
   camLoopSpiralCCWFar1,
   camLoopSpiralCCWClose1,

   camLoopSpiralCCWNorm2,
   camLoopSpiralCCWFar2,
   camLoopSpiralCCWClose2,

   camLoopSpiralCCWNorm3,
   camLoopSpiralCCWFar3,
   camLoopSpiralCCWClose3,

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
public string dialogueDataPath = "Assets/DialogueSystem/DialogueData";

[OdinSerialize, ReadOnly] public Dictionary<string, DialogueData> allGameDialogue = new Dictionary<string, DialogueData>();
 GameState state;
public List<GameObject> objectsInScene = new List<GameObject>(); 

private DialogueData currentDialogue;
private byte entryAmount;
private byte entryIndex;

[SerializeField] private Canvas dialogueBox;
[SerializeField] private TextMeshProUGUI textField;




void gatherAllObjects() {  //This method gathers all the current gameObjects in the scene that have the capacity for dialogue

}

void Awake() {
    foreach(DialogueData dia in Resources.LoadAll<DialogueData>(dialogueDataPath)) {
        allGameDialogue.Add(dia.key, dia);

    }
}

void Start() {
    foreach(KeyValuePair<string,DialogueData> dia in allGameDialogue) {
        Debug.Log(dia.Key);
    }
}


    private void StartDialogue(DialogueData dia) {
        //This Method changes the game state to the dialogue state and process the first dialogueEntry while setting everything else up.

        state = GameState.Dialogue;
        currentDialogue = dia;
        entryAmount = (byte)dia.entries.Length;
        entryIndex = 0;

        //Process Scene Camera
        

        //Process Portrait Camera

        //Process Line
        textField.text = dia.entries[entryIndex].line;
        Debug.Log(dia.entries[entryIndex].line);
    }

    private void NextLine() {


    }

    private void EndDialogue() {

    }

    void Update() {
        while(state == GameState.Dialogue) {
            
        }
    }


}
