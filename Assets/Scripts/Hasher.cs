using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Hasher : MonoBehaviour
{
    Dictionary<GUID, ConnectionInfo> npcHash = new Dictionary<GUID, ConnectionInfo>();
    public static Hasher Instance { get; private set; }
    public void Awake(){
        Instance = this;
    }

    public bool containsNPC(GUID npcID){
        return npcHash.ContainsKey(npcID);
    }   

    public ConnectionInfo getNPCConnection(GUID npcID){
        
        if(containsNPC(npcID)){
            return npcHash[npcID];
        }
        return null;
    }

    private void displayHashedNPCs(){
        foreach (KeyValuePair<GUID, ConnectionInfo> kvp in npcHash){
            Debug.Log("Key: " + kvp.Key + " Value: " + kvp.Value.IsConnected);
        }
    }

    void OnApplicationQuit(){
        foreach (KeyValuePair<GUID, ConnectionInfo> kvp in npcHash){
            kvp.Value.Close();
        }
        npcHash.Clear();
        Debug.Log("Hasher cleared");
    }

    // private void Update()
    // {
    // }

    public bool HashNPC(GUID npcID, TcpClient clientID){
        //Establish connection and then hash the NPC with clientID

        if(!containsNPC(npcID)){
            Debug.Log("Hashing NPC with ID: " + npcID);
            Debug.Log("Client connected: " + clientID.Connected);
            var connInfo = new ConnectionInfo {
                Client = clientID,
                Stream = clientID.Connected ? clientID.GetStream() : null
            };
            
            npcHash[npcID] = connInfo;
            return true;
        }
        return false;
    }
}
