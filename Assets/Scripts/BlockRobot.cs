using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRobot: CubeObject {

    public Transform corpsePrefab;

	public void spawnCorpse() {
		CubeObject corpse = Instantiate(corpsePrefab, levelPos, Quaternion.Euler(90,0,0), levelManager.transform).GetComponent<CubeObject>();
		levelManager.addBlock(corpse);
	}
}
