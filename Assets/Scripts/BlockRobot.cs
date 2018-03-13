using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRobot: CubeObject {

	public override void die() {
		Debug.Log("Robot died in laser");
		// Destroy robot and leave corpse object in its place, parented to level manager
		CubeObject corpse = Instantiate(corpsePrefab, levelPos, Quaternion.Euler(90,0,0), levelManager.transform).GetComponent<CubeObject>();
		levelManager.addBlock(corpse);
		levelManager.animateExplosion(levelPos); // can't see this because of the block
		corpse.getMoveConsequences();
		Destroy(gameObject);
	}

}
