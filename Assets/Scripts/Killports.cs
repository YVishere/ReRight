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
            if (Application.platform == RuntimePlatform.WindowsEditor || 
                Application.platform == RuntimePlatform.WindowsPlayer) {
                // Windows netstat command
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = $"/c netstat -ano | findstr :{port}";
            }
            else{
                // Unix netstat command
                process.StartInfo.FileName = "/bin/bash";
                process.StartInfo.Arguments = $"-c netstat -ano | grep {port}";
            }
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
                    // Skip PID 0 and other system processes
                    if (pid == 0 || pid == 4) // PID 4 is the System process on Windows
                    {
                        UnityEngine.Debug.Log($"Skipping system process with PID {pid}");
                        continue;
                    }

                    // Also good to check against current process
                    if (pid == Process.GetCurrentProcess().Id)
                    {
                        UnityEngine.Debug.Log($"Skipping current process with PID {pid}");
                        continue;
                    }
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
