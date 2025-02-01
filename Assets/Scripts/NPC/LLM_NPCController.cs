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

    public async Task<string> getDialog(string userSpeech){
        Debug.Log("To be implemented -- LLM_NPCController.getDialog");
        try{
            string dialog = await ServerSocketC.Instance.NPCRequest(userSpeech);
            return dialog;
        }catch (System.Exception e){
            Debug.Log(e.Message);
            throw e;
        }
    }

    public string generatePersonality(bool custom = false, string customPersonality = ""){
        Debug.Log("To be implemented -- LLM_NPCController.generatePersonality");
        return "You are an npc in a video game. You are a generic character.";
    }
}
