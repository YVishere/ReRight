using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog{
    [SerializeField] List<string> lines;

    public bool isEmpty(){
        if (lines.Count == 0){
            return true;
        }
        return false;
    }

    public List<string> Lines{
        get {return lines;}
    }
}
