using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticBlock: CubeObject {
    void Start() {
        int axis = Random.Range(0,3);
        Vector3Int newOrientation = new Vector3Int();
        newOrientation[axis] = Random.Range(0,2) == 0 ? 1 : -1;
        Debug.Log(newOrientation.ToString());
        updateOrientation(newOrientation);
    }

}
