using UnityEngine;

public enum GameState{freeRoam, dialogMode};
public class GameController : MonoBehaviour{
    [SerializeField] PlayerController playerController;
    //[SerializeField] somethingelse somethingelse;
    [SerializeField] Camera worldCamera;
    GameState state;

    private void Start(){

        DialogManager.Instance.OnShowDialog += () => {
            state = GameState.dialogMode;
        };

        DialogManager.Instance.OnCloseDialog += () => {
            if (state == GameState.dialogMode){
                state = GameState.freeRoam;
            }
        };
    }
    private void Update(){
        if (state == GameState.freeRoam){
            playerController.HandleUpdate();
        }
        else if (state == GameState.dialogMode){
            DialogManager.Instance.HandleUpdate();
        }
    }
}
