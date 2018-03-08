using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour {

	public Transform robotButton;
	public Transform[] robotPrefabs;
	public Vector3Int spawnLoc;
	public FollowCamera playerCam;

	private Transform currPlayer;
	
	void Start() {
		currPlayer = null;
		// create a UI spawn button for each robot prefab
		foreach (Transform robotPrefab in robotPrefabs) {
			Transform buttonTransform = Instantiate(robotButton, transform);
			buttonTransform.GetComponent<Button>().onClick.AddListener(delegate{spawn(robotPrefab);});
		}
	}

	public void spawn(Transform robotPrefab) {
		// Reminder: if the previous player died or was deactivated, its gameObject was destroyed and replaced.
		if (currPlayer == null) {
			currPlayer = Instantiate(robotPrefab, spawnLoc, Quaternion.identity);
			playerCam.follow(currPlayer);
		}
	}

}
