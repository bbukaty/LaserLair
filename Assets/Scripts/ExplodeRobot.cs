using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeRobot : Robot {

	protected override void getMoveConsequences() {
		if (levelManager.isInLaser(levelPos)) {
			Debug.Log("Robot died in laser");
			// Destroy robot and leave corpse object in its place, parented to level manager
			for (int x = -1; x < 2; x++){
				for (int y = -1; y < 2; y++){
					for (int z = -1; z < 2; z++) {
						Vector3Int newPos = levelPos + new Vector3Int(x,y,z);
//						Debug.Log(levelManager.getBlockIn(pos));
						removeBlock(newPos);
					}
				}
			}
			Destroy(gameObject);		
		}
	}

	protected override void tryGrab() {
		if (isGrabbing) {
			isGrabbing = false;
		} else if (levelManager.getBlockIn(levelPos + orientation) != null) {
			// toggle grab animation
			isGrabbing = true;
		}
	}
}
