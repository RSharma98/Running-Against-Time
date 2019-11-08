using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class sets and draws the bounds of a specific level
public class LevelBounds : MonoBehaviour
{
    
    public Vector2 pos;
    public Vector2 size;

    public bool startLevel;
    public bool endLevel;

    void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pos, size);
    }
}
