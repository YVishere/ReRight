using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] TMP_Text dialogText;
    [SerializeField] TMP_InputField userInput;
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
                StartCoroutine(TypeDialog(currentDialog.Lines[currentLine], false));
            }
            else{
                currentLine = 0;
                dialogBox.SetActive(false);
                userInput.gameObject.SetActive(false);
                isShowing = false;
                onFinishSpeaking?.Invoke();
                OnCloseDialog?.Invoke();
            }
        }
    }

    private IEnumerator waitForDialog(Task<string> asyncFunc,Action<string> callback){
        while (!asyncFunc.IsCompleted){
            Debug.Log("Waiting for dialog...");
            yield return null;
        }

        if (asyncFunc.IsFaulted){
            Debug.LogError(asyncFunc.Exception);
        }
        else{
            callback(asyncFunc.Result);
        }
    }

    public IEnumerator ShowDialog(Dialog dialog, Action onFinish = null, bool isAi = false){

        //Wait a frame
        yield return new WaitForEndOfFrame();

        onFinishSpeaking = onFinish;

        currentDialog = dialog;
        if (isAi){
            yield return StartCoroutine(waitForDialog(LLM_NPCController.Instance.getDialog(dialog.Lines[0]),
                (result) => {
                    dialog.Lines[0] = result;
                }));
        }
        OnShowDialog?.Invoke();

        isShowing = true;
        dialogBox.SetActive(true);
        if (isAi){
            userInput.gameObject.SetActive(true);
        }
        StartCoroutine(TypeDialog(dialog.Lines[0], isAi, onFinish, dialog));
    }

    private void clearInputField(){
        userInput.text = "";
    }

    private IEnumerator waitForInput(KeyCode key = KeyCode.Return){
        while (!Input.GetKeyDown(key)){
            yield return null;
        }
    }

    public IEnumerator TypeDialog(string dialog, bool isAi, Action onFinish = null, Dialog dialogObj = null){
        isTyping = true;
        clearInputField();
        dialogText.text = "";
        foreach(var letter in dialog.ToCharArray()){
            dialogText.text += letter;
            yield return new WaitForSeconds(1f/lettersPerSecond);
            if (Input.GetKeyDown(KeyCode.E)){
                dialogText.text = dialog;
                break;
            }
        }
        if (isAi){
            yield return StartCoroutine(waitForInput());
            string userText = userInput.text;
            if (userText != ""){
                dialogObj.replaceFirst(userText);
                yield return StartCoroutine(ShowDialog(dialogObj, onFinish, isAi));
            }
        }
        isTyping = false;
    }
}