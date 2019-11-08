using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{

    PlayerController player;

    BoxCollider2D box;
    private Vector2 pos, size;
    
    void Start()
    {
        player = PlayerController.instance;
        box = GetComponent<BoxCollider2D>();
        pos = (Vector2)transform.position + box.offset * transform.localScale;
        size = box.size * transform.localScale;
    }

    //If the player is standing on top of the platform, drop it
    void Update()
    {
        if(player.GetPosition().y - player.GetBox().size.y / 2 >= pos.y + size.y / 2){
            if(player.GetPosition().x + player.GetBox().size.x / 2 >= pos.x - size.x / 2 && player.GetPosition().x - player.GetBox().size.x / 2 <= pos.x + size.x / 2){
                StartCoroutine(Drop());
            }
        }
    }

    IEnumerator Drop(){
        yield return new WaitForSeconds(0.5f);
        float timer = 5.0f;
        while(timer > 0){
            pos.y -= 1 * Time.fixedDeltaTime;
            transform.position = pos;
            timer -= Time.fixedDeltaTime;
            yield return null;
        }
        this.gameObject.SetActive(false);
    }
}
