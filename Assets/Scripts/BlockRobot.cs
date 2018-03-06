using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRobot: Robot {

	protected override void getMoveConsequences() {
		if (levelManager.isInLaser(levelPos)) {
			// kill robot and leave block
			Debug.Log("Robot died in laser");
			Transform corpse = Instantiate(corpsePrefab, levelPos, Quaternion.identity, levelManager.transform);
			levelManager.addBlock(levelPos, corpse.GetComponent<Block>());
			Destroy(gameObject);
		}
	}

}
