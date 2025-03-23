using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Hasher : MonoBehaviour
{
    Dictionary<GUID, TcpClient> npcHash = new Dictionary<GUID, TcpClient>();
    public static Hasher Instance { get; private set; }
    public void Awake(){
        Instance = this;
    }

    public bool containsNPC(GUID npcID){
        return npcHash.ContainsKey(npcID);
    }   

    public TcpClient getNPC(GUID npcID){
        if(containsNPC(npcID)){
            return npcHash[npcID];
        }
        return null;
    }

    private void displayHashedNPCs(){
        foreach (KeyValuePair<GUID, TcpClient> kvp in npcHash){
            Debug.Log("Key: " + kvp.Key + " Value: " + kvp.Value.Connected);
        }
    }

    void OnApplicationQuit(){
        foreach (KeyValuePair<GUID, TcpClient> kvp in npcHash){
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
            npcHash[npcID] = clientID;
            return true;
        }
        return false;
    }
}
