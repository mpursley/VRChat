VRChat
======
VRChatroom designed for the Oculus Rift.


Github and Unity
================
After doing some research online, I was finally able to get Github and Unity to play nice.
For those curious, I enabled Meta Files Version control mode in Unity's Editor Settings (Edit/Project Settings/Editor). 
This creates a text file for every asset in the Assets directory containing the necessary bookkeeping for Unity.
The only two tracked folders are /ProjectSettings and /Library. 


Getting Things Up and Running
=============================
Getting the project up and running on your own should be pretty straight forward.
I'm assuming the user is familiar with github and uses the command line interface.
There are github GUIs availabe, but I wouldn't reccomend it. Go learn real github!

1. "git clone git@github.com:gg67/VRChat.git" in whatever directory you want the folder to be created in.
2. The folder you just downloaded is the Unity Project. Open Unity, navigate to File/Open Project and navigate to the VRChat folder.
3. It'll take a little while to load. There are a lot of assets to import and lots of files to create.
4. Once things load, you're probably looking at the MasterGameServerLobby scene. This is the scene (or level) you will be launching from.
5. Go ahead and press the Play button, and wait. I don't know why, but the first time you try to run this it's going to take awhile (at least it did for me). My mac even said Unity was "Not Responding" but just let it do it's thing. It'll evetually play. 
6. You should now be in the "Lobby". There are two text fields, one is what you want your server to be called, the other is your player username.
7. If anybody else has created a server, it should be listed underneath all that. If you want to join the server, enter your player username and click on the server. This project is still a very early proof of concept. Sometimes joining games don't work. I haven't tested on different LANS yet.
8. From there you should be loaded into the Default chatroom.
9. Features (as of now)
    - Voicechat (select a mic on the top right and either hold "P" to talk, "O" to continously send or select auto detect (this is more CPU intensive).
    - TextEditor/Messageboard (Press 'T' to open the TextEditor. You have to click inside to start typing. To send the text to the messageboard, click outside the text box and press 'M'. To exit without sending to the messageboard, click outside the messageboard and press 'esc'. 
    - Messageboard - Walk up to the white box to see what's been posted to the messageboad. 'Esc' to exit.
    - Arcade Portal - Walk up to get a "Enter game xxxxx". Doesn't actually do anything yet.

I'll add a lot more to the README soon! Including resources, my code, best ways to work together on this, etc...

If there are any questions, feel free to email me at graham.gaylor@gmail.com



