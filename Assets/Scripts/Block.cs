using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block: MonoBehaviour {

    private int[] orientation;
    public bool isPushable;
    public string type;

    public int[] getOrientation() {
        return orientation;
    }

    public bool orientationIsReverseOf(int[] inputOrientation) {
        for (int i = 0; i < 3; i++) {
            if (orientation[i] * -1 != inputOrientation[i]) {
                return false;
            }
        }
        return true;
    }
    
    void Start() {
        orientation = new int[3];
        for (int i = 0; i < 3; i++) {
            orientation[i] = (int)transform.up[i];
            Debug.Assert(Mathf.Abs((float)orientation[i] - transform.up[i]) < 0.001, "Warning: Level contains improperly oriented cube!");
        }
    }

    public void moveModel(int[] movement) {
        transform.Translate(new Vector3(movement[0], movement[1], movement[2]));
    }
}
