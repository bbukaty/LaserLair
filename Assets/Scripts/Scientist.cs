using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scientist: CubeObject {

	public override void checkIfDead() {
		base.checkIfDead();
		if (levelManager.getCubeObjIn(levelPos + Vector3Int.down) is GoalBlock) {
			levelManager.win();
		}
	}
	
	private void die() {
		levelManager.animateExplosion(levelPos);
		Destroy(gameObject);
	}

	public override void tryBurn() {
		die();
	}

	public override void tryExplode() {
		die();
	}
}
