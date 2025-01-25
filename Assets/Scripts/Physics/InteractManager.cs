using Unity.VisualScripting;
using UnityEngine;

public class InteractManager : MonoBehaviour
{

    public bool canInteract { get; private set; }
    private void OnTriggerEnter2D(Collider2D other){
        if (other.gameObject.CompareTag("Player_Interaction_Point")){
            canInteract = true;
        }
        else{
            canInteract = false;
        }
    }

    // private void OnTriggerExit2D(Collider2D other){
    //     canInteract = false;
    // }

    private void Start(){
        canInteract = false;
    }
}
