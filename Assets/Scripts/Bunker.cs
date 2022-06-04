using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunker : MonoBehaviour
{
    private int health = 10;
    private int topHits = 0, bottomHits = 0;
    private int width, height , heightStep;
    private SpriteRenderer spriteRenderer;
    private Texture2D texture;

    void Start() 
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        Texture2D originalTexture = spriteRenderer.sprite.texture;
        Texture2D copyTexture = new Texture2D(originalTexture.width, originalTexture.height);

        copyTexture.SetPixels(originalTexture.GetPixels());
        copyTexture.Apply();

        spriteRenderer.sprite = Sprite.Create(copyTexture,
            new Rect(0, 0, originalTexture.width, originalTexture.height), new Vector2(0.5f, 0.5f));

        texture = spriteRenderer.sprite.texture;

        width = texture.width;
        height = texture.height;
        heightStep = height / health;
    }
    
    void OnTriggerEnter2D(Collider2D col)
    {

        bool isInvader = col.GetComponent<Laser>().isFromInvader;
        
        if (isInvader) {
            int h = height - topHits * heightStep;
            for (int i = 0; i < width; i++) {
                for (int j = h; j > h - heightStep; j--) {
                    if (Random.Range(0,2) == 0)
                        texture.SetPixel(i,j, Color.black);
                }
            }

            topHits++;
        } else {
            int h = 0 + bottomHits * heightStep;
            for (int i = 0; i < width; i++) {
                for (int j = h; j < h + heightStep; j++) {
                    if (Random.Range(0,2) == 1)
                        texture.SetPixel(i,j, Color.black);
                }
            }

            bottomHits++;
        }

        texture.Apply();

        health--;

        if (health == 0) 
            Destroy(gameObject);
    }
}
