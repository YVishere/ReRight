using UnityEngine;
using System.Collections;
using System;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Composites;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Rendering.Universal.Internal;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine.Rendering.Universal;
using System.Security.Cryptography;

public class CharacterMove : MonoBehaviour
{
    CharacterAnimator animator;
    CollisionDetector collisionDetector;

    public float speed;

    public bool moving;

    private bool stopMoveFlag = false;
    private bool oldMove;

    private float oldMoveX;
    private float oldMoveY;

    private Sprite ogSprite;

    private void Awake(){
        animator = GetComponent<CharacterAnimator>();
        collisionDetector = GetComponent<CollisionDetector>();

        NPCController npc = GetComponent<NPCController>();

        ogSprite = GetComponent<SpriteRenderer>().sprite;

        if (npc != null){
            npc.OnNPCInteractStart += pauseMovement;
            npc.OnNPCInteractEnd += resumeMovement; 
        }
    }

    public IEnumerator Move(Vector2 moveVec, bool isPlayer = false){

        animator.MoveX = Mathf.Clamp(moveVec.x, -1.0f, 1.0f);
        animator.MoveY = Mathf.Clamp(moveVec.y, -1.0f, 1.0f);

        var targetPos = transform.position;
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;
        
        if (!isPathClear(targetPos) && isPlayer){
            yield break; 
        }

        moving = true;

        while ((targetPos - transform.position).sqrMagnitude > float.Epsilon){

            while (stopMoveFlag){
                moving = false;
                yield break;
            }

            if (collisionDetector != null){
                while (collisionDetector.collided){
                    moving = false;
                    yield break;    //Memory consumption very high with yield return null
                }
                moving = true;
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        moving = false;
    }
    public void HandleUpdate(){
        animator.IsMoving = moving;
    }

    private void pauseMovement(){
        stopMoveFlag = true;
        oldMove = moving;
        moving = false;
    }

    private void resumeMovement(){
        stopMoveFlag = false;
        moving = oldMove;
    }

    // private bool isWalkable(Vector3 targetPos){
    //     if (Physics2D.OverlapCircle(targetPos, 0.1f, GameLayers.i.SolidObjects | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer) != null){
    //         return false;
    //     }
    //     return true;
    // }

    private bool isPathClear(Vector3 targetPos){
        var diff  = targetPos - transform.position;
        var dir = diff.normalized;

        if (Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude - 1, GameLayers.i.SolidObjects | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer)){
            return false;
        }
        return true;
    }

    public void lookTowards(Vector3 targetPos){

        oldMoveX = animator.MoveX;
        oldMoveY = animator.MoveY;

        var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if (xdiff == 0 || ydiff == 0){
            animator.MoveX = Mathf.Clamp(xdiff, -1.0f, 1.0f);
            animator.MoveY = Mathf.Clamp(ydiff, -1.0f, 1.0f);  
        }
        else{
            Debug.LogError("How did bro turn diagonal");
        }
    }

    public void restoreOldLookTowards(){
        animator.MoveX = oldMoveX;
        animator.MoveY = oldMoveY;
    }

    public CharacterAnimator Animator{
        get => animator;
    }
}
