using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
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

    private bool isAI = false;

    public bool isShowing { get; private set; }
    private void Awake(){
        Instance = this;
    }

    public void HandleUpdate(){
        if (isAI){
            if (Input.GetKeyDown(KeyCode.Return) && !isTyping){
                if (userInput.text == ""){
                    currentLine = 0;
                    dialogBox.SetActive(false);
                    userInput.gameObject.SetActive(false);
                    isShowing = false;
                    onFinishSpeaking?.Invoke();
                    OnCloseDialog?.Invoke();
                }
            }
            return;
        }
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return)) && !isTyping){
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
            dialogText.text = "...";
            yield return null;
        }

        userInput.text = "";
        if (asyncFunc.IsFaulted){
            Debug.LogError(asyncFunc.Exception);
        }
        else{
            callback(asyncFunc.Result);
        }
    }

    public IEnumerator ShowDialog(Dialog dialog, GUID npcID, Action onFinish = null, bool isAi = false){

        //Wait a frame
        yield return new WaitForEndOfFrame();

        onFinishSpeaking = onFinish;

        isAI = isAi;
        OnShowDialog?.Invoke();

        isShowing = true;
        dialogBox.SetActive(true);
        if (isAi){
            userInput.gameObject.SetActive(true);
        }

        currentDialog = dialog;
        if (isAi){
            yield return StartCoroutine(waitForDialog(LLM_NPCController.Instance.getDialog(dialog.Lines, npcID),
                (result) => {
                    dialog.append(result);
                    dialog.replaceFirst(result); //Replace the first line with the AI response in accordance with displaying algorithm
                }));
        }
        StartCoroutine(TypeDialog(dialog.Lines[0], isAi, npcID, onFinish, dialog));
    }

    private void clearInputField(){
        userInput.text = "";
    }

    private IEnumerator waitForInput(KeyCode key = KeyCode.Return){
        while (!Input.GetKeyDown(key)){
            yield return null;
        }
    }

    public IEnumerator TypeDialog(string dialog, bool isAi, GUID npcID = default, Action onFinish = null, Dialog dialogObj = null){
        isTyping = true;
        clearInputField();
        dialogText.text = "";
        foreach(var letter in dialog.ToCharArray()){
            dialogText.text += letter;
            yield return new WaitForSeconds(1f/lettersPerSecond);
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return)){
                dialogText.text = dialog;
                break;
            }
        }
        isTyping = false;
        //Halt
        if (isAi){
            yield return StartCoroutine(waitForInput());
            string userText = userInput.text;
            dialogObj.append(userText); //Stacking user text
            yield return StartCoroutine(ShowDialog(dialogObj, npcID, onFinish, isAi));
        }
    }
}