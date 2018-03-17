using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackedBlock: CubeObject {
    
    public override void tryExplode() {
        levelManager.animateExplosion(levelPos);
        Destroy(gameObject);
    }
}
