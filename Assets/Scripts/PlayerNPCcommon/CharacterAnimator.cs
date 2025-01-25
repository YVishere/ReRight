using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{

    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> walkUpSprites;

    //Parameters
    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }

    SpriteAnimator walkDownAnim;
    SpriteAnimator walkLeftAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkUpAnim;

    bool wasMoving;
    SpriteAnimator currentAnim;
    SpriteAnimator previousAnim;

    SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(spriteRenderer, walkDownSprites);
        walkLeftAnim = new SpriteAnimator(spriteRenderer, walkLeftSprites);
        walkRightAnim = new SpriteAnimator(spriteRenderer, walkRightSprites);
        walkUpAnim = new SpriteAnimator(spriteRenderer, walkUpSprites);

        currentAnim = walkDownAnim;
    }

    // Update is called once per frame
    void Update()
    {
        previousAnim = currentAnim;
        if (MoveX == 1){
            currentAnim = walkRightAnim;
        }
        else if (MoveX == -1){
            currentAnim = walkLeftAnim;
        }
        else if (MoveY == 1){
            currentAnim = walkUpAnim;
        }
        else if (MoveY == -1){
            currentAnim = walkDownAnim;
        }

        if (currentAnim != previousAnim || wasMoving != IsMoving){
            currentAnim.Start();
        }

        if (IsMoving){
            currentAnim.HandleUpdate();
        }
        else{
            spriteRenderer.sprite = currentAnim.Frames[0];
        }

        wasMoving = IsMoving;
    }
}
