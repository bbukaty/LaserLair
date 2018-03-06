using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

	public Transform[] robotPrefabs;
	public Vector3Int spawnLoc;
	
	// Update is called once per frame
	void Update () {
		//TODO: switch between robot prefabs in robotPrefabs array
		//have icons for each type of robot that you click on to change selected robot
		if (Input.GetButtonDown("Jump")) {
			Robot newRobot = Instantiate(robotPrefabs[0], spawnLoc, Quaternion.identity).GetComponent<Robot>();
			newRobot.rotateModelTo(new Vector3Int(0,0,1));
			newRobot.setLevelPos(spawnLoc);
		}
	}


}
