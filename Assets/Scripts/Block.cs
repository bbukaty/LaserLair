using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block: CubeObject {

    public bool isPushable;
    public bool isDestructible;

    void Awake() {
        initOrientation();
		initLevelPos();
    }
}
