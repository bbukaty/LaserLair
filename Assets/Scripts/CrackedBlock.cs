﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackedBlock: CubeObject {

    public override bool tryPush(Vector3Int movement, bool justChecking = false) {
        return false;
    }
    
    public override void tryExplode() {
        levelManager.animateExplosion(levelPos);
        Destroy(gameObject);
    }
}