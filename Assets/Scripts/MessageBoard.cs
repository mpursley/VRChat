/* Title:			MessageBoard.cs
 * Author:			Mary Young
 * Edited by: 		Graham Gaylor
 * 
 * Function: 		Opens an non-editable text window showing the "contents" of the message board object you've touched.
 * 
 * Game objects: 	Attached to any object you want to act as a "message board"
 * 
 */

using UnityEngine;
using System.Collections;


public class MessageBoard : MonoBehaviour {
	public string Message = "";
	bool displayGUI;
	// Use this for initialization
	void Start () {
		displayGUI = false;
	}
	
	// Update is called once per frame
	void Update () {
	   	if (Input.GetKeyDown (KeyCode.Escape))
   		{
			displayGUI = false;
   		} 
	}
	
	void OnTriggerEnter(Collider other)
	{
		displayGUI = true;
	}
	
	
	void OnGUI()
	{
		if(displayGUI)
		{
			string dummy = GUI.TextArea(new Rect(Screen.width/8, Screen.height/8, 6*Screen.width/8, 6*Screen.height/8), Message);	
		}
	}
	
	public void updateMessageBoard(string message) {
		Debug.Log ("Local updateMessageBoard Called. About to call RPC.");
		networkView.RPC ("updateNetworkedMessageBoard", RPCMode.AllBuffered, message);
	}
	
	[RPC]
	void updateNetworkedMessageBoard(string message) {
		Debug.Log ("RPC updateMessageBoard called. Setting message to " + message);
		Message = message;
	}
}
