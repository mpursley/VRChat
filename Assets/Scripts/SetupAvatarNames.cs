/* Title:			SetupAvatarNames.cs
 * Author: 			Graham Gaylor
 * 
 * Function: 		Called everytime a player spawns. We ned to set up everybody's avatar name above their head.
 * 					It took me a while to get this working, and because I'm fairly new to Unity scripitng, I'm doing this very inefficiently.
 * 					It works fine for a small number of users, but I can see it getting out of hand for larger groups.
 * 					I invite anyone to make it faster :)
 * 					
 * Game objects: 	Attached to Player prefab.
 * 
 */

using UnityEngine;
using System.Collections;

public class SetupAvatarNames : MonoBehaviour {
	public string username;

	// Use this for initialization
	/* REMEMBER THIS IS EXECUTED FOR EVERY PLAYER IN THE SCENE ON EVERY NODE!!!!!! */
	void Start () {
		/* If I've just joined the game, I need to set my username from the global variable I made in NetworkMasterServer
		 * Then I'm sending out an RPC to every current user and telling them to add my name to the newly joined player object's username variable.
		 * At this point we still haven't update everybody's HUD */
		if(networkView.isMine) {
			username = NetworkMasterServer.gMyUsername;
			networkView.RPC ("setJoinedAvatarUserName", RPCMode.AllBuffered, username, networkView.viewID);

		}
		
		/* Once the new player has loaded in everybody's game and has had it's username variable updated, it's time to update our GUIs */
		networkView.RPC("updateGUIHandles", RPCMode.All);
	}
	
	
	/* Called when a new user joins by that user. Tells all connected players to add new player's name to a variable in the remote player game object.
	 * We still don't see anything on our screen at this point.
	 */
	[RPC]
	void setJoinedAvatarUserName(string joinedAvatarName, NetworkViewID joinedAvatarNVID) {
		foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
			if(player.networkView.viewID == joinedAvatarNVID) {
				player.GetComponent<SetupAvatarNames>().username = joinedAvatarName;	
			}
		}

	}
	
	/* This is where my inefficiency kicks in.
	 * Once we've set the username variable on the added remote player gameobject, we delete all the previously added player names on our HUD (GUITexts with ObjectLabel Components),
	 * and loop through every player in the scene, create a gameObject called "userHandle", add a GUIText component to it, set the GUIText to the player's name, add an ObjectLabel component (script) to it, 
	 * set a few properties on both components, and we're done.
	 * 
	 * As you can see, every time a single new player joins, instead of just adding that new player to their HUD, I'm having everybody delete all the GUIText names they already have, and readd everybody, which
	 * include the new player. I could fix this myself, and probably will in the future, but am too lazy to do it now :P
	 */
	[RPC]
	void updateGUIHandles() {
		GameObject localplayer = this.gameObject;
		foreach (Transform child in localplayer.transform) {
            if(child.name == "userHandle") {
				Destroy(child.gameObject);	
			}
        }
		
		foreach(GameObject _player in GameObject.FindGameObjectsWithTag("Player")) {
			if ( _player != localplayer && networkView.isMine) {
				GameObject go = new GameObject("userHandle");
				go.AddComponent<GUIText>();
				go.guiText.text = _player.GetComponent<SetupAvatarNames>().username;
				go.guiText.anchor = TextAnchor.LowerCenter;
				go.AddComponent<ObjectLabel>();
				ObjectLabel ol = go.GetComponent<ObjectLabel>();
				ol.target = _player.transform;
				ol.offset = new Vector3(0,2,0);
//				ol.clampToScreen = true; // Set this if you always want the username on the edge of the screen even if you can't see the other player. 
				ol.useMainCamera = false;
				ol.cameraToUse = (Camera)localplayer.GetComponentInChildren(typeof(Camera));
				go.transform.parent = localplayer.transform;
			} 
			
		}
	}
		
}
