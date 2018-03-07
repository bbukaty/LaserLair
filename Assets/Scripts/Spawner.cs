using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour {

	public Transform robotButton;
	public Transform[] robotPrefabs;
	public Vector3Int spawnLoc;
	
	void Start() {

		foreach (Transform robotPrefab in robotPrefabs) {
			Transform buttonTransform = Instantiate(robotButton, transform);
			buttonTransform.GetComponent<Button>().onClick.AddListener(delegate{spawn(robotPrefab);});
		}
	}

	public void spawn(Transform robotPrefab) {
		Robot newRobot = Instantiate(robotPrefab, spawnLoc, Quaternion.identity).GetComponent<Robot>();
		newRobot.setLevelPos(spawnLoc);
	}

}
