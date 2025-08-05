using UnityEngine;
using Python.Runtime;

public class PythonManager : MonoBehaviour
{
    void Start()
    {
        // Point to embedded Python DLL
        Runtime.PythonDLL = Application.dataPath + "/StreamingAssets/embeddedPython/python313.dll";
        PythonEngine.Initialize();

        using (Py.GIL())
        {
            dynamic sys = Py.Import("sys");
            // Add site‑packages path inside embedded interpreter
            sys.path.insert(0, $"{Application.streamingAssetsPath}\\embeddedPython\\Lib\\site‑packages");

            dynamic np = Py.Import("numpy");
            Debug.Log($"pi: {np.pi}");
        }
    }

    void OnApplicationQuit()
    {
        if (PythonEngine.IsInitialized) PythonEngine.Shutdown();
    }
}