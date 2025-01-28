using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class NPCController : MonoBehaviour, Interactable_intf
{
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> walkPoints;
    [SerializeField] float timerBetweenPattern = 1f;

    public enum NPCState{ Idle, Walking, Speaking}
    CharacterMove charMove;

    NPCState state;
    float idleTimer = 0f;
    int currentPattern = 0;
    public event Action OnNPCInteractStart;
    public event Action OnNPCInteractEnd;

    private CharacterAnimator animator;

    private InteractManager[] interactManagers;

    private bool dialogGenerate = false;
       
    public void Interact(Transform initiator){
        //Disable interact unless the player is in the interaction zone

        var canInteract = false;

        foreach (var interactManager in interactManagers){
            if (interactManager.canInteract){
                canInteract = true;
            }
        }

        if (!canInteract){
            return;
        }

        //To account for floating point inaccuracies
        if ((float)Math.Round(Mathf.Abs(initiator.position.x - transform.position.x), 3) == 1f){
            transform.position = new Vector3(transform.position.x, initiator.position.y, transform.position.z);
        }

        if ((float)Math.Round(Mathf.Abs(initiator.position.y - transform.position.y),3) == 1f){
            transform.position = new Vector3(initiator.position.x, transform.position.y, transform.position.z);
        }

        var oldState = state;
        OnNPCInteractStart?.Invoke();
        state = NPCState.Speaking;
        charMove.lookTowards(initiator.position);

        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () => {
            idleTimer = 0f;
            state = oldState; //return to whatever the npc was doing before
            OnNPCInteractEnd?.Invoke();
            charMove.restoreOldLookTowards();
        }));
    }

    private void Awake(){
        charMove = GetComponent<CharacterMove>();
        animator = GetComponent<CharacterAnimator>();
        GetComponent<NpcInit>()?.Init();
        interactManagers = GetComponentsInChildren<InteractManager>();
        if (dialog.isEmpty()){
            dialogGenerate = true;
        }
    }
    
    private void Update(){
        charMove.HandleUpdate();
        if (state == NPCState.Idle){
            idleTimer += Time.deltaTime;
            if (idleTimer >= timerBetweenPattern){
                idleTimer = 0f;
                if (walkPoints.Count > 0){
                    StartCoroutine(Walk());
                }
            }
        }
    }

    IEnumerator Walk(){
        state = NPCState.Walking;

        var targetPos = new Vector3(walkPoints[currentPattern].x, walkPoints[currentPattern].y, 0);
        targetPos += transform.position;

        yield return charMove.Move(walkPoints[currentPattern]);

        while (transform.position != targetPos){
            var adjust = targetPos - transform.position;
            yield return charMove.Move(new Vector2(adjust.x, adjust.y));
        }
        currentPattern = (currentPattern + 1) % walkPoints.Count;

        state = NPCState.Idle;
    }
}
