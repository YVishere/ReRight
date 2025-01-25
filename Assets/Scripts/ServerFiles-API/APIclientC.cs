using UnityEngine;
using System;
using System.Diagnostics;
using System.Text;
using System.Collections;
using UnityEngine.Networking;

public class APIclientC : MonoBehaviour
{
    private Process pythonServerProcess;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPythonServer();

        StartCoroutine(RequestDataFromServer("GetData"));
    }

    void OnApplicationQuit()
    {
        stopPythonServer();
    }

    void startPythonServer()
    {
        try
        {
            pythonServerProcess = new Process();
            pythonServerProcess.StartInfo.FileName = "python";
            pythonServerProcess.StartInfo.Arguments = "ServerSocketPython.py"; // Ensure this is the correct path to your FastAPI script

            //Somehow unity messes up same directory files so this line is important
            pythonServerProcess.StartInfo.WorkingDirectory = System.IO.Path.Combine(Application.dataPath, "Scripts/ServerFiles-API");

            pythonServerProcess.StartInfo.CreateNoWindow = true;
            pythonServerProcess.StartInfo.UseShellExecute = false;

            pythonServerProcess.Start();
            UnityEngine.Debug.Log("Python server started");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Error starting python server: " + e.Message);
        }
    }

    void stopPythonServer()
    {
        try
        {
            if (pythonServerProcess != null && !pythonServerProcess.HasExited)
            {
                endServer();
                pythonServerProcess.Kill();
                UnityEngine.Debug.Log("Python server stopped");
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Error stopping python server: " + e.Message);
        }
    }

    void endServer(){
        if (pythonServerProcess != null && !pythonServerProcess.HasExited)
        {
            string url = "http://localhost:25001/stop";

            UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
            webRequest.SetRequestHeader("Content-Type", "application/json");

            webRequest.SendWebRequest();
        }
    }

    IEnumerator RequestDataFromServer(string request)
    {
        string url = "http://localhost:25001/process";
        string jsonData = "{\"data\":\"" + request + "\"}";

        UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            UnityEngine.Debug.LogError("Error: " + webRequest.error);
        }
        else
        {
            UnityEngine.Debug.Log("Response from server: " + webRequest.downloadHandler.text);
        }
    }
}