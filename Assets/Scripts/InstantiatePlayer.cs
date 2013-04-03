using UnityEngine;
//using System.Collections;

public class InstantiatePlayer : MonoBehaviour {

	public Transform PlayerAvatar;
	public Transform[] spawn;
	
	
	/****************************************************************
	 * 
	 * 		All Code Below Here is only executed on the SERVER
	 * 
	 * 
	 *****************************************************************/
	
	// When a new player connects the GS will receive this callback. At that time we'll
	// instantiate the player on all clients, with it facing the block in the center of the level
	// at a randomly select spawn point
	void InstantiatePlayerOnNetworkLoadedLevel () 
	{
		/* Setup voice chat*/
//		if(Network.isClient) {
//			VoiceChatRecorder.Instance.Device = null;
//			VoiceChatRecorder.Instance.StartRecording();
//			Debug.Log("VoiceChatRecorder device set to default and started recording...");
//		}
		
		Transform spawnarea;
		
		// Let's randomize where we spawn from between the two available spawn areas (0 and 1)
		int spawnpoint = (int)Mathf.Round(Random.Range( 0.0f, 1.0f));
		Debug.Log("Spawning at : " + spawnpoint.ToString()) ;
		
		// using the generated random #, use the spawn array to find which spawn point to use
		spawnarea = spawn[spawnpoint];
//		GameObject go = GameObject.Find("Cube");
		
//		Debug.Log("Instantiate a new player");
		// Instantiating Player when Level is Loaded
		
		// First turn the spawn area to be facing the cube.  We could have pre done this in the 
		// level editor, but we'll do it hear just because.
		// Note: we only use the CUBE's X and Z position, but keep our spawn's Y position so we don't affect the 
		// pitch orientation of our player, keeping it level to the scene.
//		spawnarea.LookAt( new Vector3( go.transform.position.x , spawnarea.position.y, go.transform.position.z ) ); 
		
		// Now Instantiate the player at the spawn area, facing the cube
        // Internally this is a BufferedAll RPC call to instantiate the player on all current and
        // future clients
//		Debug.Log("About to instantiate player...");
		Network.Instantiate(PlayerAvatar, spawnarea.position, spawnarea.rotation, 0);
//		Debug.Log("Finished instantiating player...");

	}

	// Called on the GS when a remote client connects
	// Technically we could probably spawn the player here, or spawn the balls above, 
	// but I'm breaking it out to show various techniques 
	void OnPlayerConnected (NetworkPlayer player)  {
//		Debug.Log("OnPlayerConnected called");

	}
	
	// Called on the GS when a remote client disconnects
	void OnPlayerDisconnected (NetworkPlayer player) 
	{
		// GameObject _playerGO = null;

		// Removing player if Network is disconnected
		Debug.Log("Server destroying player");
		
		// When a player disconnects we need to cleanup the player from all other clients.  But before we do that we need to make
		// sure we reset the state of everything back correctly.
		// If the Player was holding a ball, then we want to player to drop the ball before cleaning up  To do that we need to first
		// associate the NetworkPlayer to the ingame object that represents that player.  To do so we'll loop through all Player objects and match 
		// the player passed in to the GameObject.networkView.owner
		//
		// Loop through all GameObjects of type Player
		foreach(GameObject _player in GameObject.FindGameObjectsWithTag("Player"))
		{
			// Match the player to the NetworkPlayer and if we have a match then this is the player we're cleaning up
			if ( _player.networkView.owner == player )
			{
				Debug.Log ("Found player");
				// _playerGO = _player;
				// Get the players Ball Controller Component
//				BallController _bc = (BallController)_player.GetComponent(typeof(BallController));
//				if (_bc.pickedup) // If the Player is holding the ball then let's drop/respawn the ball
//				{
//					// To find the ball being held, we have the pickedupBall_SpawnID identifier held by the player.  This is the ball that originated
//					// from one of the spawn areas, which is managed by the BallSpawnManager.  Therefore, we find the root level PickupSpawn
//					// then get the BallSpawnManager Component which then is mapped to the specific spawn area using the players "pickedupBall_SpawnID"
//					// which gives us the ball object to be dropped/respawned.
//					//
//					BallSpawnManager _bsm = (BallSpawnManager)GameObject.Find("PickupSpawn").GetComponent(typeof(BallSpawnManager));
//					BallManager _bm = _bsm.spawnedBalls[_bc.pickedupBall_SpawnID];
//
//					// Call the init Ball RPC to update the ball on all clients
//					_bsm.networkView.RPC("OnInitBall", RPCMode.All,
//						_bm.ballSpawnElementID, 
//						_bsm.spawnedBalls[_bm.ballSpawnElementID].transform.position, 
//						_bsm.spawnedBalls[_bm.ballSpawnElementID].transform.rotation, 
//						false, 
//						false, 
//						Vector3.zero, 
//						Vector3.zero );
//					
//				}
				break; // Done cleaning up, so get out
			}
		}
		
		// remove any pending Buffered RPC's left from the disconnected player for group 0 
		Network.RemoveRPCs( player, 0); 
		
		// Cleanup just the player and all the objects the player might have instantiated
		Network.DestroyPlayerObjects( player  ); // send a request to all the remaining clients to have them remove the disconnected player
		
		// Cleanup just the player but not all the objects the player might have instantiated
		//Network.Destroy( _playerGO  ); // send a request to all the remaining clients to have them remove the disconnected player
	}
	
}