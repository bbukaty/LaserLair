using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticBlock: CubeObject {
    void Start() {
        transform.Rotate(Vector3.up*90*Random.Range(0,4), Space.World);
        if (levelPos.y != 0) {
            transform.Rotate(Vector3.right*90*Random.Range(0,4), Space.World);
        }
    }
}
