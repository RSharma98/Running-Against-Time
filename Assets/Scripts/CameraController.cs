using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private float orthoSize, height, width;

    private Vector3 pos;
    private bool moveCamera;
    private bool hasMovedCamera = false;

    private List<LevelBounds> levelBounds;
    private LevelBounds currentLevel, nextLevel;

    PlayerController player;
    GameManager gameManager;

    public static CameraController instance;
    void Awake(){
        if(instance == null) instance = this;
        else Destroy(this);

        Camera cam = GetComponent<Camera>();
        float aspectRatio = (float)Screen.width / (float)Screen.height;   //Get the aspect ratio
        float targetRatio = 1280.0f / 720.0f;
        Rect camRect = cam.rect;

        //Set the viewport to match the target aspect ratio
        if(!aspectRatio.Equals(targetRatio)){
            float scaleHeight = aspectRatio / targetRatio;
            float scaleWidth = 1.0f / scaleHeight;
            if(scaleHeight < 1.0f){
                camRect.width = 1.0f;
                camRect.height = scaleHeight;
                camRect.x = 0;
                camRect.y = (1.0f - scaleHeight) / 2.0f;
            } else{
                camRect.width = scaleWidth;
                camRect.height = 1.0f;
                camRect.x = (1.0f - scaleWidth) / 2.0f;
                camRect.y = 0;
            }
            cam.rect = camRect;
        }

        pos = transform.position;

        //Get the level bounds in the game
        levelBounds = new List<LevelBounds>();
        foreach(GameObject gm in GameObject.FindGameObjectsWithTag("Level")){
            levelBounds.Add(gm.GetComponent<LevelBounds>());
            gm.SetActive(false);
        }
    }

    void Start(){
        Camera cam = GetComponent<Camera>();       //Get the camera
        float aspectRatio = (float)Screen.width / (float)Screen.height;   //Get the aspect ratio
        
        orthoSize = cam.orthographicSize;   //Get the orthographic size of the camera
        height = orthoSize * 2;             //Set the height
        width = height * aspectRatio;       //Set the width

        player = PlayerController.instance;
        gameManager = GameManager.instance;

        hasMovedCamera = false;
        FindLevel(false);
    }

    //If the player is not in the current level, find the new level they are in
    void FixedUpdate(){
        if(!PlayerInLevel()) FindLevel(true);
    }

    //Function to determine if player is within bounds of a certain level
    private bool PlayerInLevel(){
        Vector2 playerPos = player.GetPosition();
        if(currentLevel != null){
            if(playerPos.x >= currentLevel.pos.x - currentLevel.size.x / 2 && playerPos.x <= currentLevel.pos.x + currentLevel.size.x / 2){
                if(playerPos.y >= currentLevel.pos.y - currentLevel.size.y / 2 && playerPos.y <= currentLevel.pos.y + currentLevel.size.y / 2){
                    return true;
                }
            }
        }
        return false;
    }

    //A function to find the level the player is in
    void FindLevel(bool lerp){
        foreach(LevelBounds bounds in levelBounds){
            if(player.GetPosition().x >= bounds.pos.x - bounds.size.x / 2 && player.GetPosition().x <= bounds.pos.x + bounds.size.x / 2){
                if(player.GetPosition().y >= bounds.pos.y - bounds.size.y / 2 && player.GetPosition().y <= bounds.pos.y + bounds.size.y / 2){
                    if((pos.x - width / 2 < bounds.pos.x - bounds.size.x || pos.x + width / 2 > bounds.pos.x + bounds.size.x || 
                    pos.y - height / 2 < bounds.pos.y - bounds.size.y || pos.y + height / 2 > bounds.pos.y + bounds.size.y) && !hasMovedCamera){
                        StartCoroutine(MoveTo(bounds, lerp));
                        break;
                    }
                }
            }
        }
    }

    //A couroutine to smoothly lerp the camera towards the target level
    public IEnumerator MoveTo(LevelBounds target, bool lerp = true, bool moveCamera = false){
        nextLevel = target;
        nextLevel.gameObject.SetActive(true);
        if(!nextLevel.startLevel) gameManager.ShowTutorial(false);
        Debug.Log("Moving camera");
        hasMovedCamera = true;
        if(lerp){
            while((Vector2)pos != target.pos){
                gameManager.SetMultiplier(0);
                pos = Vector2.MoveTowards(pos, target.pos, 50 * Time.deltaTime);
                pos.z = -10;
                transform.position = pos;
                yield return null;
            }
        }
        hasMovedCamera = false;
        this.moveCamera = moveCamera;
        if(currentLevel != null) currentLevel.gameObject.SetActive(false);
        currentLevel = nextLevel;
        pos = target.pos;
        pos.z = -10;
        transform.position = pos;
        if(currentLevel.startLevel) gameManager.ShowTutorial(true);
        else gameManager.ShowTutorial(false);
        if(currentLevel.endLevel) gameManager.GameOver(true, false);
        gameManager.SetMultiplier(1);
    }
}
