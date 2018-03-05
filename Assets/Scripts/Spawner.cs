using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

	public Transform[] robotPrefabs;
	public intTrio spawnLoc;
	public int x;
	public int y;
	public int z;
	// Use this for initialization
	void Start () {
		spawnLoc = new intTrio(x, y, z);
	}
	
	// Update is called once per frame
	void Update () {
		//TODO: switch between robot prefabs in robotPrefabs array
		//have icons for each type of robot that you click on to change selected robot
		if (Input.GetButtonDown("Jump")) {
			Transform newRobot = Instantiate(robotPrefabs[0], new Vector3(spawnLoc[0], spawnLoc[1], spawnLoc[2]), Quaternion.identity);
			newRobot.GetComponent<BlockRobot>().initLevelPos(spawnLoc);
		}
	}

}
