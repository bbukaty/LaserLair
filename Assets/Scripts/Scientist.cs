using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scientist: Character {

	public override void getMoveConsequences() {
		base.getMoveConsequences();
		if (levelManager.getCubeObjIn(levelPos + Vector3Int.down) is GoalBlock) {
			levelManager.win();
		}
	}

	protected override void tryJump(Vector3Int movement) {
		return;
	}
}
