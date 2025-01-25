using UnityEngine;

public class PlayerInit : MonoBehaviour
{
    public void init(){
        createRigidbody();
        createHitbox();
        createInteractionPointer();
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

    private void createInteractionPointer(){
        var interactionPointer = new GameObject("Interaction_Point");

        interactionPointer.transform.parent = transform;
        interactionPointer.transform.localPosition = new Vector3(0, 0, 0);

        interactionPointer.tag = "Player_Interaction_Point";
        interactionPointer.layer = LayerMask.NameToLayer("Player");

        var circleCollider = interactionPointer.AddComponent<CircleCollider2D>();

        circleCollider.isTrigger = true;
        circleCollider.offset = new Vector2(0f, -0.3274525f);
        circleCollider.radius = 0.1725475f;
    }
}
