/* Title:			ObjectLabel.cs
 * 
 * Function: 		Keeps a GUIText label above a gameobject. Used to display avatar usernames above their heads.
 * 
 * Game objects: 	Used in SetupAvatarNames.cs
 * 
 * NOTE: 			http://wiki.unity3d.com/index.php?title=ObjectLabel
 */

using UnityEngine;
using System.Collections;
 
[RequireComponent (typeof (GUIText))]
public class ObjectLabel : MonoBehaviour {
 
public Transform target;  // Object that this label should follow
public Vector3 offset = Vector3.up;    // Units in world space to offset; 1 unit above object by default
public bool clampToScreen = false;  // If true, label will be visible even if object is off screen
public float clampBorderSize = 0.05f;  // How much viewport space to leave at the borders when a label is being clamped
public bool useMainCamera = true;   // Use the camera tagged MainCamera
public Camera cameraToUse ;   // Only use this if useMainCamera is false
Camera cam ;
Transform thisTransform;
Transform camTransform;
Vector3 screenPos;
 
	void Start () 
    {
	    thisTransform = transform;
    if (useMainCamera)
        cam = Camera.main;
    else
        cam = cameraToUse;
    camTransform = cam.transform;
	}
 
 
    void Update()
    {
 
        if (clampToScreen)
        {
            Vector3 relativePosition = camTransform.InverseTransformPoint(target.position);
            relativePosition.z =  Mathf.Max(relativePosition.z, 1.0f);
            thisTransform.position = cam.WorldToViewportPoint(camTransform.TransformPoint(relativePosition + offset));
            thisTransform.position = new Vector3(Mathf.Clamp(thisTransform.position.x, clampBorderSize, 1.0f - clampBorderSize),
                                             Mathf.Clamp(thisTransform.position.y, clampBorderSize, 1.0f - clampBorderSize),
                                             thisTransform.position.z);
 
        }
        else
        {
			screenPos = cam.WorldToViewportPoint(target.position + offset);
			
			/* The following if statement is a hack to fix bug where label was appearing twice in the world. 
			 * Once above the remote player and another if the "my player" turned around 180 degrees after looking at the remote player.
			 * Remove the if to see what I'm talking about and maybe a better solution can be found :)
			 */
			if(screenPos.z > 0) { 
				thisTransform.position = screenPos;
			}
        }
    }
}