/* Title:			VoiceChatGUI.cs
 * 
 * Function: 		Sets up VoiceChat HUD. Include different microphone inputs, voice chat keys and when you are sending data
 * 
 * Game objects: 	Attachd to player prefab.
 * 
 */
using UnityEngine;
using System.Collections;

public class VoiceChatGUI : MonoBehaviour {

	void Start() {
        Application.RequestUserAuthorization(UserAuthorization.Microphone);
    }

    void OnGUI()
    {
        GUILayout.Window(1, new Rect(Screen.width - 150, 10, 150, 300), Window, "", GUIStyle.none);
    }

    void Window(int id)
    {

        if (VoiceChatRecorder.Instance.IsRecording)
        {
            GUILayout.Label(VoiceChatRecorder.Instance.Device);

            if (GUILayout.Button("Stop Recording"))
            {
                VoiceChatRecorder.Instance.StopRecording();
            }
        }
        else
        {
            GUILayout.Label("Select microphone to start recording");

            foreach (string device in VoiceChatRecorder.Instance.AvailableDevices)
            {
                if (GUILayout.Button(device))
                {
                    VoiceChatRecorder.Instance.Device = device;
                    
                    VoiceChatRecorder.Instance.StartRecording();
                }
            }
        }

        if (VoiceChatRecorder.Instance.Device != null)
        {
			Debug.Log ("Creating VoiceChat button GUI");
            GUILayout.Label("Push-to-talk key: " + VoiceChatRecorder.Instance.PushToTalkKey);
            GUILayout.Label("Toggle-to-talk key: " + VoiceChatRecorder.Instance.ToggleToTalkKey);
            GUILayout.Label("Auto detect speech: " + (VoiceChatRecorder.Instance.AutoDetectSpeech ? "On" : "Off"));

            if (GUILayout.Button("Toggle Auto Detect"))
            {
                VoiceChatRecorder.Instance.AutoDetectSpeech = !VoiceChatRecorder.Instance.AutoDetectSpeech;
            }

            GUI.color = VoiceChatRecorder.Instance.IsTransmitting ? Color.green : Color.red;
            GUILayout.Label(VoiceChatRecorder.Instance.IsTransmitting ? "Transmitting" : "Silent");
        }
    }
}
