using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface LLM_NPC_intf
{
    Task<string> getDialog(List<string> userSpeech);

    string generatePersonality(bool custom, string customPersonality);
}
