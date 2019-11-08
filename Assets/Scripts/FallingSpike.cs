using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingSpike : MonoBehaviour
{

    public float maxDist;

    Vector2 startPos;
    Vector2 pos, size;
    PlayerController player;

    private bool droppedSpike;

    void Awake(){
        startPos = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        pos = (Vector2)transform.position + box.offset * transform.localScale;
        size = box.size * transform.localScale;

        player = PlayerController.instance;
    }

    //If player is below the spikes, drop them
    void Update()
    {
        Vector2 playerPos = player.GetPosition();
        BoxCollider2D playerBox = player.GetBox();
        if(playerPos.x + playerBox.size.x / 2 >= pos.x - size.x / 2 && playerPos.x - playerBox.size.x / 2 <= pos.x + size.x / 2){
            if(playerPos.y + size.y / 2 >= (pos.y - size.y / 2) - maxDist && playerPos.y < pos.y && !droppedSpike) StartCoroutine(Drop());
        }
    }

    //Drop the spikes, then after 5 seconds disable this game object
    IEnumerator Drop(){
        float timer = 5.0f;
        while(timer > 0){
            pos.y -= 1 * Time.fixedDeltaTime;
            transform.position = pos;
            timer -= Time.fixedDeltaTime;
            yield return null;
        }
        this.gameObject.SetActive(false);
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.down * maxDist);
    }
}
