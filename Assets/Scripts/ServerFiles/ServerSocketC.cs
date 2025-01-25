using UnityEngine;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Xml.Serialization;

public class ServerSocketC : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private Process pythonServerProcess;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        startPythonServer();

        connectToServer();
        RequestDataFromServer("GetData");
        ReceiveResponseFromServer();
    }

    void OnApplicationQuit(){
        closeConnection();
        stopPythonServer();
    }

    void startPythonServer(){
        try{
            pythonServerProcess = new Process();
            pythonServerProcess.StartInfo.FileName = "python";
            pythonServerProcess.StartInfo.Arguments = "ServerSocketPython.py";

            //Somehow unity messes up same directory files so this line is important
            pythonServerProcess.StartInfo.WorkingDirectory = System.IO.Path.Combine(Application.dataPath, "Scripts/ServerFiles");

            pythonServerProcess.StartInfo.CreateNoWindow = true;
            pythonServerProcess.StartInfo.UseShellExecute = false;

            pythonServerProcess.Start();
            UnityEngine.Debug.Log("Python server started");
        }
        catch (Exception e){
            UnityEngine.Debug.LogError("Error starting python server: " + e.Message);
        }
    }

    void stopPythonServer(){
        try{
            if (pythonServerProcess != null && !pythonServerProcess.HasExited){
                pythonServerProcess.Kill();
                UnityEngine.Debug.Log("Python server stopped");
            }
        }
        catch(Exception e){
            UnityEngine.Debug.LogError("Error stopping python server: " + e.Message);
        }
    }

    void connectToServer(){
        try{
            client = new TcpClient("localhost", 25001);
            stream = client.GetStream();
            UnityEngine.Debug.Log("Connected to server");
        }
        catch (Exception e){
            UnityEngine.Debug.LogError("Error connecting to server: " + e.Message);
        }
    }

    void RequestDataFromServer(string request){
        if (stream == null) return;

        byte[] data = Encoding.ASCII.GetBytes(request);
        stream.Write(data, 0, data.Length);
        UnityEngine.Debug.Log("Request sent to server");
    }

    void ReceiveResponseFromServer(){
        if (stream == null) return;

        byte[] data = new byte[1024];
        int bytes = stream.Read(data, 0, data.Length);

        string response = Encoding.ASCII.GetString(data, 0, bytes);
        UnityEngine.Debug.Log("Response from server: " + response);
    }

    void closeConnection(){
        if (stream != null){
            stream.Close();
        }
        if (client!= null){
            client.Close();
        }

        UnityEngine.Debug.Log("Connection closed");
    }
}
