using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryShip : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        int reward = Random.Range(10,50);
        Camera.main.GetComponent<Game>().KillMysteryShip(reward);
    }
}
