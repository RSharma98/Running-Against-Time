using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBlade : MonoBehaviour
{

    public float rotationSpeed;

    public Vector2[] targetPositions;
    public float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = targetPositions[0];
        if(targetPositions.Length > 0) StartCoroutine(Move());
    }

    // Update is called once per frame
    void Update()
    {
        //Rotate the saw blade
        transform.Rotate(0, 0, rotationSpeed);
    }

    //Move the saw blade to the target positions and then loop it
    IEnumerator Move(){
        int i = 0;
        Vector2 pos = transform.localPosition;
        Vector2 target = targetPositions[i];
        while(true){
            if(pos == target){
                i = (i == targetPositions.Length - 1) ? 0 : i + 1;
                target = targetPositions[i];
            }
            pos = Vector2.MoveTowards(pos, target, moveSpeed * Time.deltaTime);
            transform.localPosition = pos;
            yield return null;
        }
    }
}
