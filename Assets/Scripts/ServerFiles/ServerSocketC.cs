using UnityEngine;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections;
using System.Runtime.CompilerServices;
using System.IO;
using UnityEditor.PackageManager.Requests;

public class ServerSocketC : MonoBehaviour
{
    private Process pythonServerProcess;

    public static ServerSocketC Instance { get; private set; }

    private void Awake(){
        Instance = this;
        StartCoroutine(startSteps());       
    }

    private IEnumerator startSteps(int retries = 3){
        startPythonServer();
        yield return new WaitForSeconds(5);
        yield return RequestDataFromServer("GetData");
        /////////////////////////////////////////////////////////////////////
        ///Debug statement to see how many connections can be made to server
        /////////////////////////////////////////////////////////////////////

        // for (int i = 0; i <= 10; i++){
        //     yield return new WaitForSeconds(5);
        //     Task requestTask = RequestDataFromServer("GetData");
        //     while (!requestTask.IsCompleted){
        //         yield return null;
        //     }
        //     if (requestTask.IsFaulted){
        //         UnityEngine.Debug.LogError("Error starting server: " + requestTask.Exception.Message);
        //     }
        // }    
    }
    
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start(){
    //     RequestDataFromServer("GetData");
    // }

    void OnApplicationQuit(){
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

    public async Task<TcpClient> connectToServer(int retries, bool doAgain = true){
        try{
            TcpClient client = new TcpClient();
            await client.ConnectAsync("localhost", 25001);
        
            // Configure keep-alive after successful connection
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            
            // Windows-specific keep-alive settings using IOControl
            byte[] inOptionValues = new byte[12];
            BitConverter.GetBytes(1).CopyTo(inOptionValues, 0);                // Enable keep-alive (1=on, 0=off)
            BitConverter.GetBytes(30000).CopyTo(inOptionValues, 4);            // Keep-alive time (30 seconds)
            BitConverter.GetBytes(5000).CopyTo(inOptionValues, 8);             // Keep-alive interval (5 seconds)
            
            client.Client.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);

            UnityEngine.Debug.Log("Connected to server with keep-alive enabled");
            return client;
        }
        catch (Exception e){
            UnityEngine.Debug.LogError("Error connecting to server: " + e.Message);
            //Ideally you would need only a second retry but just in case lets go with 3
            if (retries > 0 && doAgain){
                UnityEngine.Debug.Log("Retrying connection..." + retries);
                OnApplicationQuit();
                StartCoroutine(startSteps(retries - 1));
            }
        }
        return null;
    }

    private async Task RequestDataFromServer(string request){
        TcpClient client = await connectToServer(3);

        NetworkStream stream = client.GetStream();
        if (stream == null) return;

        byte[] data = Encoding.ASCII.GetBytes(request);
        stream.Write(data, 0, data.Length);
        UnityEngine.Debug.Log("Request sent to server");

        await ReceiveResponseFromServer(stream, client);

        stream.Close();

        closeConnection(client);
    }

    public async Task<string> NPCRequest(string request, TcpClient client, NetworkStream stream){
        if (stream == null) return "";

        byte[] data = Encoding.ASCII.GetBytes(request);

        //Todo: send data in chunks, I am just sending the first 1024 bytes for simplicity
        //TCP websocket forcibly closes connection if datalength is greater than whatever i specified in python code
        UnityEngine.Debug.Log("Writing data to stream");
        await stream.WriteAsync(data, 0, Math.Min(data.Length, 1023));
        UnityEngine.Debug.Log("Request sent to server: " + request);

        String resp = await ReceiveResponseFromServer(stream, client);
        return resp;
    }

    public async Task<string> ReceiveResponseFromServer(NetworkStream stream, TcpClient client){
        if (stream == null) return "";

        byte[] data = new byte[1024];
        UnityEngine.Debug.Log("Waiting for response from server...");
        int bytes = await stream.ReadAsync(data, 0, data.Length);

        string response = Encoding.ASCII.GetString(data, 0, bytes);
        UnityEngine.Debug.Log("Response from server: " + response);

        return response;
    }

    void closeConnection(TcpClient client){
        if (client!= null){
            client.Close();
        }

        UnityEngine.Debug.Log("Connection closed");
    }
}

