using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
  
   

  

  
  public Movement movementScript;

  sbyte i = 0;



  




// Method that places the cam at a given position. EX: the cam being at the character's feet, then immediately shifting to the character's head




public void frontFace(Camera cam, GameObject Focus, float radius) {
i = 2;
  cam.transform.position = movementScript.cylToCar(new cylCoords(radius, Mathf.PI/2 , 0f), Focus.transform);
  cam.transform.LookAt(Focus.transform.position);
        Debug.Log("moving to position");

    }

public void behindFace(Camera cam, GameObject Focus, float radius) {
   i = 2;
    cam.transform.position = movementScript.cylToCar(new cylCoords(radius, (3/2)*Mathf.PI , 0f), Focus.transform);
  cam.transform.LookAt(Focus.transform.position);

}

public void snapCamera(Camera cam, cylCoords pos, Vector3 focus) {
  i = 2;
//cam.transform.position = movementScript.cylToCar(pos, focus);
//cam.transform.LookAt(focus);
}


/*public IEnumerator moveCamera(Camera cam, cylCoords currentPos, cylCoords endPos, Vector3 focus, float spinSpeed, float heightSpeed, float radiusSpeed) {
i = 1;
cam.transform.position = movementScript.cylToCar(currentPos, f);


int j = i;
while (j == i) {
if (Vector3.Distance(movementScript.cylToCar(currentPos,focus), movementScript.cylToCar(endPos, focus)) >= .001f) {
  currentPos.radius = Mathf.Lerp(currentPos.radius, endPos.radius, radiusSpeed);
  currentPos.theta = Mathf.Lerp(currentPos.theta, endPos.theta, spinSpeed);
  currentPos.height = Mathf.Lerp(currentPos.height, endPos.height, heightSpeed);
  cam.transform.position = movementScript.cylToCar(currentPos, focus);
  cam.transform.LookAt(new Vector3(focus.x, cam.transform.position.y, focus.z));
  
  yield return null;
}
cam.transform.position = movementScript.cylToCar(endPos, focus);
cam.transform.LookAt(focus);
}
}

IEnumerator moveCameraLoop(Camera cam, cylCoords currentPos, Vector3 focus, float spinSpeed, float heightSpeed, float radiusSpeed, float radiusRange, float heightRange) {
i = 0;

cam.transform.position = movementScript.cylToCar(currentPos, focus);
cylCoords endPos = new cylCoords(currentPos.radius + radiusRange, currentPos.theta + (2 * Mathf.PI), currentPos.height + heightRange);

int j = i;
float radiusInterval = 0;
float heightInterval = 0;
if(radiusRange != 0 ) {
radiusInterval = radiusRange * (radiusSpeed * Time.deltaTime);
 }
if (heightRange != 0) {
 heightInterval = heightRange * (heightSpeed * Time.deltaTime);
}
radiusRange = radiusRange - (2 * radiusRange);
heightRange = heightRange - (2 * heightRange);
Debug.Log(heightInterval);
while (i == j) {
  if (Mathf.Abs(currentPos.radius - endPos.radius) <= radiusInterval) {
    currentPos.radius = endPos.radius;
    endPos.radius = endPos.radius + radiusRange;
    radiusRange = radiusRange - (2 * radiusRange);
    radiusInterval = radiusInterval - (2 * radiusInterval);
  }
   if (Mathf.Abs(currentPos.height - endPos.height) <= heightInterval) {
    currentPos.height = endPos.height;
    endPos.height = endPos.height + heightRange;
    heightRange = heightRange - (2 * heightRange);
    heightInterval = heightInterval - (2 * heightInterval);
    Debug.Log(heightInterval);
  }
   if (Mathf.Abs(currentPos.theta - endPos.theta) < .0001f) {

    currentPos.theta = 0;
  }
  
  currentPos.radius += radiusInterval;  
  currentPos.height += heightInterval;
  currentPos.theta +=  Mathf.PI/500;

  cam.transform.position = movementScript.cylToCar(currentPos, focus);
  cam.transform.LookAt(new Vector3(focus.x, cam.transform.position.y, focus.z));
  
  yield return null;
  

} } */








}

// Method that checks the direction of the focus. EX: Being able to tell the direction the character is facing in order to use that for cam placement

   
// Methods that move the cam in some way. EX: The cam starting off behind the character then rotating to in front of the character. 
  

 // public void HorizontalArc(GameObject focus = null, float Speed = 10, float arcDistance = 360, sCoords? startingLocation = null) {

 // }


  

