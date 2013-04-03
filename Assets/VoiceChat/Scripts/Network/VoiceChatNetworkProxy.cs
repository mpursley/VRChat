using UnityEngine;
using System.Collections.Generic;

public class VoiceChatNetworkProxy : MonoBehaviour
{
    static int networkIdCounter = 0;

    int assignedNetworkId = -1;
    VoiceChatPlayer player = null;
    Queue<VoiceChatPacket> packets = new Queue<VoiceChatPacket>(16);

    void Start()
    {
		// If im the owner of the proxy, send my voice data
        if (networkView.isMine)
        {
			Debug.Log ("Network view isMine. Creating VoiceChatPackets.");
            VoiceChatRecorder.Instance.NewSample += new System.Action<VoiceChatPacket>(OnNewSample);
        }
		
		// if im the server, set the proxy owner's voiceChatRecorder's networkId
        if (Network.isServer)
        {
			Debug.Log ("Network.isServer");
			Debug.Log("networkView.owner = " + networkView.owner);
            assignedNetworkId = ++networkIdCounter;
			
			/* If I'm the server, I still need to get a networkID. We have to handle this separately bc 
			 * the RPC call apparently won't call to itself or something */
			if(networkView.isMine) {
				SetNetworkId(assignedNetworkId);	
			} else {
				networkView.RPC("SetNetworkId", networkView.owner, assignedNetworkId);
				gameObject.AddComponent<AudioSource>();
            	player = gameObject.AddComponent<VoiceChatPlayer>();
			}
        }
		
		// im a client other than the owner of the proxy, add an audio source so I can hear the proxy owner's voice data
        if(Network.isClient && !networkView.isMine)
        {
			Debug.Log ("VoiceChatNetworkProxy - isClient and addingVoiceChatPlayer");
            gameObject.AddComponent<AudioSource>();
            player = gameObject.AddComponent<VoiceChatPlayer>();
        }
    }

    void OnNewSample(VoiceChatPacket packet)
    {
        packets.Enqueue(packet);
    }

    [RPC]
    void SetNetworkId(int networkId)
    {
		Debug.Log ("Setting networkId to " + networkId);
        VoiceChatRecorder.Instance.NetworkId = networkId;
    }
	

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        int count = packets.Count;

        if (stream.isWriting)
        {
			Debug.Log ("Stream is writing");
            stream.Serialize(ref count);

            while (packets.Count > 0)
            {
                VoiceChatPacket packet = packets.Dequeue();
                stream.WritePacket(packet);

                // If this packet is the same size as the sample size, we can return it
                if (packet.Data.Length == VoiceChatSettings.Instance.SampleSize)
                {
                    VoiceChatBytePool.Instance.Return(packet.Data);
                }
            }
        }
        else
        {
            if (Network.isServer)
            {
				Debug.Log ("Inside stream is not writing and isServer");
                stream.Serialize(ref count);

				for (int i = 0; i < count; ++i)
                {
                    var packet = stream.ReadPacket();

                    if (player != null)
                    {
                        player.OnNewSample(packet);
                    }
                }
				
				for (int i = 0; i < count; ++i)
                {
                    packets.Enqueue(stream.ReadPacket());

                    if (Network.connections.Length < 2)
                    {
                        packets.Dequeue();
                    }
                }
				
				
            }
            else
            {
				Debug.Log ("Inside stream is not writing and NOT isServer");
                stream.Serialize(ref count);

                for (int i = 0; i < count; ++i)
                {
                    var packet = stream.ReadPacket();

                    if (player != null)
                    {
                        player.OnNewSample(packet);
                    }
                }
            }
        }
    }
}