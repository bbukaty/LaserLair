using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRobot: Character {

	public override void die() {
		Debug.Log("Robot died in laser");
		// Destroy robot and leave corpse object in its place, parented to level manager
		Transform corpse = Instantiate(corpsePrefab, levelPos, Quaternion.identity, levelManager.transform);
		levelManager.addBlock(corpse.GetComponent<CubeObject>());
		Destroy(gameObject);
	}

}
