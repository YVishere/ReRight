using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class LLM_NPCController : MonoBehaviour, LLM_NPC_intf
{
    public static LLM_NPCController Instance { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake(){
        Instance = this;
    }

    private string reformatDialog(List<string> dialog){
        string formatted = "Invoke::: " + dialog[^1] + " ::: ";

        if (dialog.Count > 1){
            formatted += "Context::: ";
            //Indices not included are doodoo for context
            //0 is the dialog to be displayed and the last is the user input
            for (int i = 1; i < dialog.Count-1; i++){
                //What AI says gets appended first
                if (i%2 != 0)
                    formatted += ". You --> " + dialog[i];
                else
                    formatted += ". Player --> " + dialog[i];
            }
        }

        return formatted;
    }

    public async Task<string> getDialog(List<string> userSpeech){
        Debug.Log("To be implemented -- LLM_NPCController.getDialog");
        string conversation = reformatDialog(userSpeech);
        try{
            string dialog = await ServerSocketC.Instance.NPCRequest(conversation);
            return dialog;
        }catch (System.Exception e){
            Debug.Log(e.Message);
            throw e;
        }
    }

    public string generatePersonality(bool custom = false, string customPersonality = "You are the first npc in this game who is connected to an LLM"){
        Debug.Log("To be implemented -- LLM_NPCController.generatePersonality");
        // return "You are an npc in a video game. You are a generic character.";
        if (custom){
            return customPersonality;
        }
        else{
            return "You are an npc in a video game. You are a generic character.";
        }
    }
}
