/* Title:			PlayerSpawn.cs
 * Author:			Larry Grant - http://www.3dmuve.com/3dmblog/?p=201
 * Edited by:		Graham Gaylor
 * 
 * Function: 		Called when a player is spawned both locally and remotely.
 * 
 * Game objects: 	Attached to Player prefab.
 * 
 */

using UnityEngine;
//using System.Collections;

public class PlayerSpawn : MonoBehaviour {

	void OnNetworkInstantiate (NetworkMessageInfo msg) 
	{
		// I like to setup a local variable to our local gameObject so the code is a little more readable
		GameObject localplayer = this.gameObject;
		//networkView.group = 1; // send all RPC messages for group 1, so only the player instantiate is on group 0
		
		/* If the player being spawned is ME, then I need to enable a whole bunch of attached components I initally disabled because I didn't want them
		 * enabled for remote players, just myself, so I'm doing that now */
		if (networkView.isMine) 
		{
			// On the original player since we control the player with the keyboard controls
			// so we don't want to use the NetworkRigidbody script which was specific for the prediction and smoothing
			// of remote networked character avatars.  Therefore, let's find that component on the new player and
			// disable it
			NetworkRigidbody _NetworkRigidbody = (NetworkRigidbody) localplayer.GetComponent("NetworkRigidbody");	
			_NetworkRigidbody.enabled = false;
			
			// Since this is the local player and not the remote avatar we do want to ensure the player controls 
			// are active.  
			CharacterController cc = (CharacterController)localplayer.GetComponent<CharacterController>();
			cc.enabled = true;
			
			CharacterMotor cm = (CharacterMotor)localplayer.GetComponent<CharacterMotor>();
			cm.enabled = true;
			
//			AudioSource source = (AudioSource)localplayer.GetComponent<AudioSource>();
//			source.enabled = true;
//			
			// We don't have a Temp Camera setup in the scene, but if we did we would need to disable before activating
			// the players camera
			//GameObject.Find("TempCamera").SetActiveRecursively(false);
			
			// Since this is the local player, we want our local player camera enabled as the main camera for the scene
			// Since the camera component is in a sub object called Main Camera under the Player Prefab we can use
			// GetComponentInChildren looking specifically for our "Camera" component. There should only be one, so we 
			// don't need to loop through multiple components
			Camera ca = (Camera)localplayer.GetComponentInChildren(typeof(Camera));
			ca.enabled = true; // activate our camera
			
			// Since this is the local player, we want our local player audio enabled. Since you can only have one audio
			// enabled in a scene, we need to make sure it's not enabled for remote avatars.  Therefore we set it disabled
			// be default and enable it when we spawn the player
			AudioListener al = (AudioListener)localplayer.GetComponentInChildren(typeof(AudioListener));
			al.enabled = true;
			
			VoiceChatGUI vcg = (VoiceChatGUI)localplayer.GetComponent<VoiceChatGUI>();
			vcg.enabled = true;
			
			TextEditorGUI teg = (TextEditorGUI)localplayer.GetComponent<TextEditorGUI>();
			teg.enabled = true;
					
		}
		/* If the networkView isn't mine, then the player being spawned is some remote player so we'll enable their NetworkRigidBody component.
		 * We'll also disable their CharacterController because otherwise we'd be able to control them!! 
		 */
		else
		{
			Debug.Log("NetworkView is Remote");
			Debug.Log ("Remote NetworkView.owner = " + networkView.owner);
			name += "Remote";
			
			// Since this player object is a remote avatar, we wont be performing any manual controls to this avatar.  
			// Instead we want all updates to come from network updates.  The NetworkView will send those updates 
			// automatically.  Therefore we want to enable "NetworkRigidbody" component which will process all
			// the prediction and smoothing for our avatar
			NetworkRigidbody _NetworkRigidbody = (NetworkRigidbody) localplayer.GetComponent("NetworkRigidbody");
			_NetworkRigidbody.enabled = true;
			
			// Since this is a player avatar for a remote player, we need to make sure its camera is disabled.  We already 
			// have our local player camera so we don't need this one
			CharacterController cc = (CharacterController)localplayer.GetComponent("CharacterController");
			cc.enabled = false;

			
		}
	}

}
