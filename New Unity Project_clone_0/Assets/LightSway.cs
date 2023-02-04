using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSway : MonoBehaviour
{
    public Transform ceilLamp;

    Vector3 startAngle;   //Reference to the object's original angle values

    float rotationSpeed = .001f;  //Speed variable used to control the animation

    float rotationOffset = 5f; //Rotate by 50 units

    float finalAngle;  //Keeping track of final angle to keep code cleaner

    void Start()
    {
        startAngle = transform.eulerAngles;  // Get the start position
    }

    void Update()
    {
        finalAngle = (float)(startAngle.z + Math.Sin(Environment.TickCount * rotationSpeed) * rotationOffset);  //Calculate animation angle
        ceilLamp.eulerAngles = new Vector3(startAngle.x, startAngle.y, finalAngle); //Apply new angle to object
    }
}
