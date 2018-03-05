using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block: MonoBehaviour {

    private intTrio orientation;
    public bool isPushable;

    public intTrio getOrientation() {
        return orientation;
    }

    void Start() {
        orientation = new intTrio(transform.up);
    }

    public void moveModel(intTrio movement) {
        transform.Translate(new Vector3(movement.x, movement.y, movement.z));
    }
}
