using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRobot: Character {

	public override void die() {
		Debug.Log("Robot died in laser");
		// Destroy robot and leave corpse object in its place, parented to level manager
		CubeObject corpse = Instantiate(corpsePrefab, levelPos, transform.rotation, levelManager.transform).GetComponent<CubeObject>();
		levelManager.addBlock(corpse);
		corpse.getMoveConsequences();
		Destroy(gameObject);
	}

}
