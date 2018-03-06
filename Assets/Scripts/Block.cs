using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block: MonoBehaviour {

    public Vector3Int orientation;
    public bool isPushable;

    void Start() {
        orientation = new Vector3Int();
        for (int i = 0; i < 3; i++) {
            Debug.Assert(Mathf.Abs((float)(int)transform.up[i] - transform.up[i]) < 0.001, "Warning: Level contains improperly oriented cube!");
            orientation[i] = (int)transform.up[i];
        }
    }

    public void moveModel(Vector3Int movement) {
        transform.Translate(movement);
    }
    
}
