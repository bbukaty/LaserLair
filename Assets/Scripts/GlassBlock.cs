﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBlock: CubeObject {

    public override bool tryPush(Vector3Int movement, bool justChecking = false) {
        return false;
    }
}
