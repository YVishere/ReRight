using System.Collections;
using UnityEngine;

public class NpcInit : MonoBehaviour
{

    private Vector2[] boxDim = {new Vector2(3f, 0.4f), new Vector2(0.4f, 3f)};

    private Vector2[] boxOffset = {new Vector2(0f, -0.2f), new Vector2(0, 0)};
    public void Init(){
        createRigidbody();
        createHitbox();
        createInteractObjects();
    }

    private void createRigidbody(){
        var rigidbody = gameObject.AddComponent<Rigidbody2D>();

        rigidbody.bodyType = RigidbodyType2D.Kinematic;
        rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
    }

    private void createHitbox(){
        var capsuleCollider = gameObject.AddComponent<CapsuleCollider2D>();

        capsuleCollider.size = new Vector2(0.859f, 1.192f);
        capsuleCollider.offset = new Vector2(-0.0128f, -0.0448f);
        capsuleCollider.isTrigger = true;

        capsuleCollider.direction = CapsuleDirection2D.Vertical;
    }

    // Update is called once per frame
    private void createInteractObjects(int numberOfInteracts = 2){
        for (int i = 0; i < numberOfInteracts; i++){
            var interactObject = new GameObject("InteractObject" + i);

            interactObject.transform.parent = transform;
            interactObject.transform.localPosition = new Vector3(0, 0, 0);

            var boxCollider = interactObject.AddComponent<BoxCollider2D>();

            boxCollider.size = boxDim[i];
            boxCollider.offset = boxOffset[i];
            boxCollider.isTrigger = true;

            interactObject.AddComponent<InteractManager>();

            interactObject.tag = "Interactable";
            interactObject.layer = LayerMask.NameToLayer("Default");
        }
    }
}
