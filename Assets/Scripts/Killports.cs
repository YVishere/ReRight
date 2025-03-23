using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

public class Killports : MonoBehaviour
{
    int port = 25001;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnApplicationQuit()
    {
        UnityEngine.Debug.Log("Killing processes on port " + port);
        try
        {
            // Use netstat to find processes using this port
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c netstat -ano | findstr :{port}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            
            UnityEngine.Debug.Log($"Netstat output: {output}");
            
            // Extract PIDs using regex - only match lines where 25001 is the first port
            Regex pidRegex = new Regex(@"TCP\s+\d+\.\d+\.\d+\.\d+:25001\s+\d+\.\d+\.\d+\.\d+:\d+\s+\w+\s+(\d+)", RegexOptions.Multiline);
            MatchCollection matches = pidRegex.Matches(output);
            
            UnityEngine.Debug.Log($"Found {matches.Count} listener processes on port {port}");
            
            // Kill each process found
            foreach (Match match in matches)
            {
                // The PID is in the first capture group
                string pidString = match.Groups[1].Value.Trim();
                if (int.TryParse(pidString, out int pid))
                {
                    try
                    {
                        Process.GetProcessById(pid).Kill();
                        UnityEngine.Debug.Log($"Killed process with PID {pid} hosting port {port}");
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"Failed to kill process {pid}: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error killing processes on port {port}: {ex.Message}");
        }
    }
}
