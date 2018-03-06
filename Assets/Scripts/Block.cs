using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block: CubeObject {

    public bool isPushable;

    void Start() {
        initOrientation();
    }
}
