using System;
using System.Collections;
using Unity.Multiplayer.Center.Common.Analytics;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerController:MonoBehaviour
{
    private Vector2 input;
    
    private CharacterMove charMove;

    private void Awake(){
        charMove = GetComponent<CharacterMove>();
        GetComponent<PlayerInit>().init();
    }

    public void HandleUpdate(){
        if (!charMove.moving){
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //Remove diagonal movement
            if (input.x != 0){
                input.y = 0;
            }

            if (input != Vector2.zero){
                StartCoroutine(charMove.Move(input, true));
            }
        }

        charMove.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.E)){
            Interact();
        }
    }

    void Interact(){
        var facingDir = new Vector3(charMove.Animator.MoveX, charMove.Animator.MoveY);
        var interactPos = transform.position + facingDir;

        // Debug.DrawLine(transform.position, interactPos, Color.black, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
        if (collider != null){
            charMove.moving = false;
            if (collider.GetComponent<Interactable_intf>() != null){
                collider.GetComponent<Interactable_intf>()?.Interact(transform);
            }
        }
    }
}
