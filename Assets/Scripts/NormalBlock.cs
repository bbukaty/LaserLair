using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBlock : CubeObject {

	public override void tryExplode() {
		levelManager.explodeOutwards(levelPos);
		Destroy(gameObject);
	}
}
