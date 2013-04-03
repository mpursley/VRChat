/* Title:			TextEditorGUI.cs
 * Author:			Mary Young
 * Edited by: 		Graham Gaylor
 * 
 * Function: 		Used to open the TextEditor screen on a player HUD.
 * 
 * Game objects: 	Attached to Player prefab.
 * 
 * NOTE:			A friend of mine wrote this script and I haven't taken the time to really figure out what's going on.
 * 					I know she used the TextEditor Object hidden in Unity's API (there's like no documention on it), but that's about it.
 * 					The resolution of the v1 Rift may be too low for us to use a "message board" type function, but I think it's definitely worth a try.
 * 					If anybody is interested in expanding the text editor, check out http://primerlabs.com/texteditor
 */

using UnityEngine;
using System.Collections;
using System;


public class TextEditorGUI : MonoBehaviour {
	
	public Rect windowRect;
	public string text = "";
	public GUIStyle richTextEditorPlainHiddenStyle;
	TextEditor editor = new TextEditor();
	public String plainText = ""; 
	public String withLineNumbers;
	bool display = false;
	private GameObject mBoardGO;
	MessageBoard mBoard;
	
	void Start() {
    	windowRect = new Rect (0, 0, 3*Screen.width/4, 3*Screen.height/4);   
		mBoardGO = GameObject.FindWithTag ("MessageBoard");
		mBoard = mBoardGO.GetComponent<MessageBoard>();
		
    }
	
	/* Currently the key to access the text editor is T */
	void Update() {
   		if (Input.GetKeyDown (KeyCode.T))
		{
			Debug.Log ("T pressed. ");
			display = true;	
		}
	}
	
    void OnGUI()
    {
		if(display)
		{
			windowRect = GUI.Window(0, windowRect, Window, "TextEditor");
		}
    }

    void Window(int id)
    {
  		if (Input.GetKeyDown (KeyCode.Escape))
   		{
			display = false;
   		}
		/* Currently M is the key to send a message to the "Message board" */
		else if(Input.GetKeyDown (KeyCode.M))
		{
			Debug.Log ("Pressed M.");
			string message =  mBoard.Message + plainText + "\n\n" ; 
			mBoard.updateMessageBoard(message);
			plainText = "";
			display = false;

			
		}
		else {
		
			plainText = GUI.TextArea(new Rect(20, 20, 3*Screen.width/4 -40, 3*Screen.height/4 - 40), plainText);
			editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
   			if ( Event.current.Equals( Event.KeyboardEvent("tab") ) )
   			{
     			Debug.Log("hit tab");
      			InsertTab ();
      			Event.current.Use();
   			} 		
		}

	
        GUI.DragWindow(new Rect (0,0, 10000, 10000));
		
		//editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
	}
	
	void InsertTab ()
	{
   		Debug.Log ("InsertTab");
   		if ( plainText.Length >= editor.pos )
   		{
      		plainText = plainText.Insert(editor.pos, "\t");
      		editor.pos++;
      		editor.selectPos = editor.pos; // if there's a gap between pos and selectPos, it selects what is between them
   		}
   		else
   		{
      		Debug.Log ("ERROR: Can't InsertTab, plainText.Length < editor.pos");
   		}
	}
	
}
