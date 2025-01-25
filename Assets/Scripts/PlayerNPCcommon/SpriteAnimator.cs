using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator
{
    SpriteRenderer spriteRenderer;
    List<Sprite> frames;
    float frameRate;

    int currentFrame;
    float timer;

    public List<Sprite> Frames {
        get {return frames;}
    }

    public SpriteAnimator(SpriteRenderer spriteRenderer, List<Sprite> frames, float frameRate = 0.16f){
        this.spriteRenderer = spriteRenderer;
        this.frames = frames;
        this.frameRate = frameRate;
    }

    public void Start(){
        currentFrame = 1;
        timer = 0f;
        spriteRenderer.sprite = frames[currentFrame];
    }

    public void HandleUpdate(){
        timer = timer + Time.deltaTime;
        if (timer > frameRate){
            currentFrame = (currentFrame + 1) % frames.Count;
            spriteRenderer.sprite = frames[currentFrame];
            timer = timer - frameRate;
        }
    }
}
