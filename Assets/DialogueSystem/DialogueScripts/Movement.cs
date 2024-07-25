using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct cylCoords 
{
   //sCoords stands for spherical coordinates. Just indicates a position of an object given a center. 
   // This is indicated with the coordinate system (r, t, p) or radius, theta, and phi
   // Radius indicates the distance from the center, aka the radius of the sphere.
   // theta indicates the horizontal rotation around the center. While phi represents the vertical rotation around the center.
   // In other words, theta is the rotation around the y axis, and phi is the rotation around the z axis.
   // For a good example of this in real life, imagine a coin on a string tied around your finger.
   // the radius would be the length of the string, aka how far away is the coin from your finger.
   // Theta would then be the rotational direction parallel to the ground, while Phi would be the rotational direction parallel to the wall.

  public float radius;
  public float theta;
   public float height;

   

   public cylCoords(float radius, float theta, float height) {
     this.radius = radius;
    this.theta = theta;
    this.height = height;
   }

   
}
public class Movement : MonoBehaviour
{

 float pi_over_180 = Mathf.PI/180;
    

   public cylCoords carToCyl(Vector3 currentPos, Vector3 focusPos) {
        Vector3 posDif = new Vector3((currentPos.x - focusPos.x), (currentPos.y - focusPos.y), (currentPos.z - focusPos.z));

        float radius = (Mathf.Pow(posDif.x,2) + Mathf.Pow(posDif.z,2));
        float theta = Mathf.Atan((posDif.x * posDif.z));
        float height = posDif.y;

        return(new cylCoords(radius, theta, height));

   }

   public Vector3 cylToCar(cylCoords coords, Vector3 focus) {
    float x = (coords.radius * Mathf.Cos(coords.theta)) + focus.x;
    float z = (coords.radius * Mathf.Sin(coords.theta)) + focus.z;
    float y = coords.height + focus.y;

    return (new Vector3(x,y,z));
   }

   




public float degree_to_radians(float d) {
    return d * pi_over_180;
}

public float radians_to_degree(float r) {
    return r/ pi_over_180;
}


}
