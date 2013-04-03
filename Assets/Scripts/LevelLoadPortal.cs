/* Title:			LoadLevelPortal.cs
 * Author:			Mary Young
 * Edited by: 		Graham Gaylor
 * 
 * Function: 		Used to load user into another level. Isn't complete.
 * 
 * Game objects: 	Attached to any object acting as a "portal" to other realms :)
 * 
 */

using UnityEngine;
using System.Collections;

public class LevelLoadPortal : MonoBehaviour {
	bool displayMessage;
	
	void OnTriggerEnter(Collider other)
	{
		displayMessage = true;
	}
	
	void OnTriggerExit(Collider other) {
		displayMessage = false;
	}

	void OnGUI()
    {
        if(displayMessage){
			GUILayout.Window(1, new Rect(100, 100, 150, 150), Window, "", GUIStyle.none);
			GUI.Box (new Rect(100, 100, 150, 150), "");
		}
    }

    void Window(int id)
    {
		    GUILayout.Label("Would you like to go into the arcade portal?");
            if (GUILayout.Button("Yes"))
            {
				Debug.Log ("Pressed yes");
             	Application.LoadLevel ("WorldChatroom");
				displayMessage = false;   
            }
		
			if (GUILayout.Button ("No"))
			{
				Debug.Log ("Pressed no");
				displayMessage = false;
			}
   
    }
	
}

