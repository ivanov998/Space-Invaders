using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public GameObject laser;
    
    bool fireCooldown;

    private float movementSpeed = 3.0f;

    private Vector3 playerInitialPosition;

    private float leftLimit, rightLimit;

    private bool isMovingLeft, isMovingRight;

    void Start() 
    {
        SetInitialPlayerPosition();
        GetMovementLimits();
    }

    void SetInitialPlayerPosition() 
    {
        playerInitialPosition = this.transform.position;
    }

    void GetMovementLimits() 
    {
        leftLimit = Camera.main.GetComponent<Game>().CalculateLeftWorldBorder();
        rightLimit = Mathf.Abs(leftLimit);
    }

    void Update() 
    {
        if(isMovingLeft && this.transform.position.x > leftLimit + 0.25f) {
            this.transform.position += Vector3.left * movementSpeed * Time.deltaTime;
        }

        if(isMovingRight && this.transform.position.x < rightLimit - 0.25f) {
            this.transform.position += Vector3.right * movementSpeed * Time.deltaTime;
        }

    }

    void OnTriggerEnter2D(Collider2D collision) 
    {
        Camera.main.GetComponent<Game>().RemoveLife();
        Invoke("RespawnPlayer",2.0f);
        gameObject.SetActive(false);
    }

    void RespawnPlayer() 
    {
        this.transform.position = playerInitialPosition;
        gameObject.SetActive(true);
    }
    
    public void Fire() 
    {
        if (fireCooldown || !gameObject.activeSelf) return;
        Vector3 pos = this.transform.position + Vector3.up * 0.3f;
        Instantiate(laser, pos, Quaternion.identity);
        fireCooldown = true;
        Invoke("RemoveFireCooldown", 0.4f);
    }

    void RemoveFireCooldown() 
    {
        fireCooldown = false;
    }

    public void LeftMove() 
    {
        isMovingLeft = true;
    }

    public void StopLeftMove() 
    {
        isMovingLeft = false;
    }

    public void RightMove() 
    {
        isMovingRight = true;
    }

    public void StopRightMove() 
    {
        isMovingRight = false;
    }

}
