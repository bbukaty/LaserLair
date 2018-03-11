using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scientist: Character {

	protected override void die() {
		Destroy(gameObject);
	}

	protected override void getMoveConsequences() {
		base.getMoveConsequences();
		if (levelManager.getBlockIn(levelPos + Vector3Int.down) is GoalBlock) {
			levelManager.win();
		}
	}

	protected override void tryJump(Vector3Int movement) {
		return;
	}
}
