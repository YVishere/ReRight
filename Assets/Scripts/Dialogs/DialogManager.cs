using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    public static DialogManager Instance { get; private set; }

    int currentLine = 0;
    bool isTyping;
    Dialog currentDialog; 
    Action onFinishSpeaking;

    public bool isShowing { get; private set; }

    private void Awake(){
        Instance = this;
    }

    public void HandleUpdate(){
        if (Input.GetKeyDown(KeyCode.E) && !isTyping){
            ++currentLine;
            if (currentLine < currentDialog.Lines.Count){
                StartCoroutine(TypeDialog(currentDialog.Lines[currentLine]));
            }
            else{
                currentLine = 0;
                dialogBox.SetActive(false);
                isShowing = false;
                onFinishSpeaking?.Invoke();
                OnCloseDialog?.Invoke();
            }
        }
    }

    public IEnumerator ShowDialog(Dialog dialog, Action onFinish = null){

        //Wait a frame
        yield return new WaitForEndOfFrame();

        onFinishSpeaking = onFinish;

        currentDialog = dialog;
        OnShowDialog?.Invoke();

        isShowing = true;
        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public IEnumerator TypeDialog(string dialog){
        isTyping = true;
        dialogText.text = "";
        foreach(var letter in dialog.ToCharArray()){
            dialogText.text += letter;
            yield return new WaitForSeconds(1f/lettersPerSecond);
            if (Input.GetKeyDown(KeyCode.E)){
                dialogText.text = dialog;
                break;
            }
        }
        isTyping = false;
    }
}