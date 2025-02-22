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

    async Task<TcpClient> connectToServer(int retries){
        try{
            TcpClient client = new TcpClient();
            await client.ConnectAsync("localhost", 25001);
            UnityEngine.Debug.Log("Connected to server");
            return client;
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
    }

    public async Task<string> NPCRequest(string request){
        TcpClient client = await connectToServer(3);

        NetworkStream stream = client.GetStream();
        if (stream == null) return "";

        byte[] data = Encoding.ASCII.GetBytes(request);

        //Todo: send data in chunks, I am just sending the first 1024 bytes for simplicity
        //TCP websocket forcibly closes connection if datalength is greater than whatever i specified in python code
        stream.Write(data, 0, Math.Min(data.Length, 1023));
        UnityEngine.Debug.Log("Request sent to server: " + request);

        return await ReceiveResponseFromServer(stream, client);
    }

    public async Task<string> ReceiveResponseFromServer(NetworkStream stream, TcpClient client){
        if (stream == null) return "";

        byte[] data = new byte[1024];
        UnityEngine.Debug.Log("Waiting for response from server...");
        int bytes = await stream.ReadAsync(data, 0, data.Length);

        string response = Encoding.ASCII.GetString(data, 0, bytes);
        UnityEngine.Debug.Log("Response from server: " + response);
        closeConnection(stream, client);
        return response;
    }

    void closeConnection(NetworkStream stream, TcpClient client){
        if (stream != null){
            stream.Close();
        }
        if (client!= null){
            client.Close();
        }

        UnityEngine.Debug.Log("Connection closed");
    }
}
