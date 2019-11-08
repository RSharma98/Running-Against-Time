using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeGem : MonoBehaviour
{

    public LayerMask playerLayer;
    public float radius;

    GameManager gameManager;
    
    void Start()
    {
        gameManager = GameManager.instance;
    }

    void Update()
    {
        //If the player collides with this object refill the timer
        if(Physics2D.OverlapCircle(transform.position, radius, playerLayer)){
            gameManager.RefillTimer(transform.position);
            this.gameObject.SetActive(false);
        }
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
