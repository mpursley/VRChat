/* Title:			NetworkLoadLevel.cs
 * Author:			Larry Grant - http://www.3dmuve.com/3dmblog/?p=201
 * 
 * Function: 		Loads level on all clients. 
 * 
 * Game objects: 	NetworkMasterServer.cs is attached to the MasterServerMenu game object in the MasterGameServerLobby scene.
 * 
 */

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class NetworkLoadLevel : MonoBehaviour {
	/* Step 1 (out of 2) for setting up voicechat using basic Unity Networking.
	 * We'll actually set this to something in a bit. 
	 */
	VoiceChatNetworkProxy proxy;



	// Keep track of the last level prefix (increment each time a new level loads)
	private int lastLevelPrefix = 0;
	private NetworkMasterServer mgs;
	private bool hasCreatedProxy = false;

	void Awake () {
		// Network level loading is done in a seperate channel.
		DontDestroyOnLoad(this);
		networkView.group = 1;
		//Application.LoadLevel("EmptyScene");
		mgs = GameObject.Find("MasterServerMenu").GetComponent<NetworkMasterServer>() as NetworkMasterServer;
	}

	//void OnGUI () {
	void Update() {		
		// When the server is started then issue a "LoadLevel" RPC call for the local GS client to load his own level
		// then also buffer the LoadLevel request so any new client that connects will also receive the "LoadLevel"
		if (Network.peerType != NetworkPeerType.Disconnected && 
			mgs.gamemenustate == NetworkMasterServer.menustate.networklobby &&
			Network.isServer )
		{
			// Make sure no old RPC calls are buffered and then send load level command
			Network.RemoveRPCsInGroup(0);
			Network.RemoveRPCsInGroup(1);
			// Load level with incremented level prefix (for view IDs)
			// NOTE: The RPCMode.AllBuffered sends the request to all clients, including the client sending the
			//       request.  Also, the Buffered command tells the game server to hold onto the RPC command and also
			//       send it to any new clients that connect.  This is important for level loading or player instantiation 
			//       so we make sure all objects are correctly created on all clients, but would not be used for example 
			//       to indicate a death as the new client would never have seen the player in the first place and 
			//       therefore doesn't need to see the death.
			
			/* This is the network call that every player who starts/joins a server.
			 * Change "DefaultChatroom" to the name of the scene you want the player to load at first */
			networkView.RPC( "LoadLevel", RPCMode.AllBuffered, "DefaultChatroom", lastLevelPrefix + 1);
		}
		
		else if (Network.peerType != NetworkPeerType.Disconnected && !hasCreatedProxy) {
			proxy = VoiceChatUtils.CreateProxy();
			hasCreatedProxy = true;
		}
	}

	[RPC]
	IEnumerator LoadLevel (string level, int levelPrefix) 
	{
		// If we already loaded the level don't load it again
		if ( mgs.gamemenustate != NetworkMasterServer.menustate.ingame )
		{
			mgs.gamemenustate = NetworkMasterServer.menustate.ingame;
			//Debug.Log("Loading level " + level + " with prefix " + levelPrefix);
			lastLevelPrefix = levelPrefix;
			// There is no reason to send any more data over the network on the default channel,
			// because we are about to load the level, because all those objects will get deleted anyway
			Network.SetSendingEnabled(0, false);
			// We need to stop receiving because first the level must be loaded.
			// Once the level is loaded, RPC's and other state update attached to objects in the level are allowed to fire
			Network.isMessageQueueRunning = false;
			
			// All network views loaded from a level will get a prefix into their NetworkViewID.
			// This will prevent old updates from clients leaking into a newly created scene.
			Network.SetLevelPrefix(levelPrefix);
			Application.LoadLevel(level);
			yield return 0; //new WaitForSeconds (1);
			yield return 0; //new WaitForSeconds (1);
			
			// Allow receiving data again
			Network.isMessageQueueRunning = true;
			// Now the level has been loaded and we can start sending out data
			Network.SetSendingEnabled(0, true);

			// Once the level is loaded we then need to instantiate our local player.  To do that we 
			// could use Network.Instantiate, but that only lets us send 
			GameObject go = GameObject.Find("PlayerSpawn");
			InstantiatePlayer io = (InstantiatePlayer)go.GetComponent(typeof(InstantiatePlayer));
			io.SendMessage("InstantiatePlayerOnNetworkLoadedLevel", SendMessageOptions.RequireReceiver);
		
		}

	}
	

	void OnDisconnectedFromServer () {
		// If we lose the connection to the server then return to the Master Game Server Lobby
		Application.LoadLevel("MasterGameServerLobby");
	}

}
