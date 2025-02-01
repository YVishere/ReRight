using UnityEngine;

public class CollisionDetector : MonoBehaviour
{

    private CharacterAnimator characterAnim;
    public bool collided { get; private set; }

    private void Start(){
        characterAnim = GetComponent<CharacterAnimator>();
    }

    public void OnTriggerEnter2D(Collider2D other){
        if (transform.gameObject.CompareTag("NPC") || transform.gameObject.CompareTag("NPC_AI")){
            collided = checkContinue();
        }
    }

    public bool checkContinue(){
        if (characterAnim == null){
            // characterAnim = GetComponentInParent<CharacterAnimator>();
            return true;
        }
        var dir = new Vector3(characterAnim.MoveX, characterAnim.MoveY, 0);
        if (!Physics2D.BoxCast(transform.position + dir, new Vector2(0.5f, 0.5f), 0f, dir, 1, GameLayers.i.SolidObjects | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer)){
            return false;
        }
        return true;
    }

    public void OnTriggerExit2D(Collider2D other){
        collided = false;
    }
}
