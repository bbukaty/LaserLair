using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRobot: CubeObject {

    public Transform corpsePrefab;

	public override void tryBurn() {
		Debug.Log("Robot died in laser");
		// Destroy robot and leave corpse object in its place, parented to level manager
		CubeObject corpse = Instantiate(corpsePrefab, levelPos, Quaternion.Euler(90,0,0), levelManager.transform).GetComponent<CubeObject>();
		levelManager.addBlock(corpse);
		corpse.getMoveConsequences();
		Destroy(gameObject);
	}

	public override void tryExplode() {
		Destroy(gameObject);
	}

}
