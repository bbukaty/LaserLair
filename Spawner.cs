using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

	public Transform blockRobotPrefab;
	public int[] spawnLoc;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Jump")) {
			Transform newRobot = Instantiate(blockRobotPrefab, new Vector3(spawnLoc[0], spawnLoc[1], spawnLoc[2]), Quaternion.identity);
			newRobot.GetComponent<BlockRobot>().levelPosition = spawnLoc;
		}
	}

}
