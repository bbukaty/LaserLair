using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeRobot : Character {

	public override void die() {
		// Debug.Log("Robot died in laser");
		// for (int x = -1; x <= 1; x++){
		// 	for (int y = -1; y <= 1; y++){
		// 		for (int z = -1; z <= 1; z++) {
		// 			Vector3Int newPos = levelPos + new Vector3Int(x,y,z);
		// 			// Debug.Log(levelManager.getBlockIn(pos));
		// 			levelManager.tryExplodeBlock(newPos);
		// 		}
		// 	}
		// }
		levelManager.explodePos(levelPos);
		Destroy(gameObject);
	}

}
