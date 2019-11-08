using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{

    PlayerController player;
    BoxCollider2D playerBox;

    BoxCollider2D box;
    Vector2 pos, size;

    void Start(){
        player = PlayerController.instance;
        playerBox = player.GetBox();

        box = GetComponent<BoxCollider2D>();
        pos = (Vector2)transform.position + box.offset * transform.localScale;
        size = box.size * transform.localScale;
        box.enabled = false;
    }

    //Check if the player is above the object. If they are, enable the collider so they can stand on it
    void Update()
    {
        Vector2 playerPos = player.GetPosition();
        if(playerPos.x - playerBox.size.x / 2f <= pos.x + size.x / 2f && playerPos.x + playerBox.size.x / 2f >= pos.x - size.x / 2f){
            if(playerPos.y + playerBox.size.y / 2f >= pos.y - size.y / 2f && playerPos.y - playerBox.size.y / 2f <= pos.y + size.y / 2f){
                box.enabled = true;
                Vector2 newPos = new Vector2(playerPos.x, (pos.y + size.y / 2) + (playerBox.size.y));
                if(player.GetVelocity().y <= 0) player.SetPosition(newPos);
            }
        }
    }
}
