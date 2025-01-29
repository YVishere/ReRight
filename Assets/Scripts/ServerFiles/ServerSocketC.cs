using UnityEngine;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections;

public class ServerSocketC : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private Process pythonServerProcess;

    public static ServerSocketC Instance { get; private set; }

    private void Awake(){
        Instance = this;
        StartCoroutine(startSteps());       
    }

    private IEnumerator startSteps(int retries = 3){
        startPythonServer();
        yield return new WaitForSeconds(5);
        connectToServer(3);
        RequestDataFromServer("GetData");
    }
    
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start(){
    //     RequestDataFromServer("GetData");
    // }

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
            // pythonServerProcess.StartInfo.RedirectStandardOutput = true; // Redirect standard output

            pythonServerProcess.Start();

            // // Read the standard error output asynchronously
            // pythonServerProcess.BeginErrorReadLine();
            // pythonServerProcess.ErrorDataReceived += (sender, args) =>
            // {
            //     if (!string.IsNullOrEmpty(args.Data))
            //     {
            //         UnityEngine.Debug.LogError("Python server error: " + args.Data);
            //     }
            // };

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

    void connectToServer(int retries){
        try{
            client = new TcpClient("localhost", 25001);
            stream = client.GetStream();
            UnityEngine.Debug.Log("Connected to server");
        }
        catch (Exception e){
            UnityEngine.Debug.LogError("Error connecting to server: " + e.Message);
            //Ideally you would need only a second retry but just in case lets go with 3
            if (retries > 0){
                UnityEngine.Debug.Log("Retrying connection..." + retries);
                OnApplicationQuit();
                StartCoroutine(startSteps(retries - 1));
            }
        }
    }

    void RequestDataFromServer(string request){
        if (stream == null) return;

        byte[] data = Encoding.ASCII.GetBytes(request);
        stream.Write(data, 0, data.Length);
        UnityEngine.Debug.Log("Request sent to server");

        ReceiveResponseFromServer();
    }

    public async Task ReceiveResponseFromServer(){
        if (stream == null) return;

        byte[] data = new byte[1024];
        int bytes = await stream.ReadAsync(data, 0, data.Length);

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
