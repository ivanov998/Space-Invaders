using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invader : MonoBehaviour
{
    public int pointsReward;
    public Sprite defaultSprite, altSprite;
    private SpriteRenderer _spriteRenderer;
    public float altTime;

    void Start() 
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = _spriteRenderer.sprite;
        InvokeRepeating("AnimateSprite", altTime, altTime);
    }

    void AnimateSprite() 
    {
        if (_spriteRenderer.sprite == defaultSprite) {
            _spriteRenderer.sprite = altSprite;
        } else {
            _spriteRenderer.sprite = defaultSprite;
        }
    }

    void OnTriggerEnter2D(Collider2D col) 
    {
        if (col.GetComponent<Laser>().isFromInvader) return;
        Destroy(gameObject);
        OnDestroyed();
    }

    void OnDestroyed() 
    {
        Camera.main.GetComponent<Game>().KillInvader(gameObject, pointsReward);
    }
}
