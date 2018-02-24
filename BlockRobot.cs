using System; //TODO - no exceptions
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRobot: MonoBehaviour {

	private LevelManager levelManager;
	public int[] levelPosition;
	
	void Start() {
		// find the level manager object in the scene to get level data from
		levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Left")) {
			movePlayer(-1, 0, 0);
		}
		if (Input.GetButtonDown ("Right")) {
			movePlayer(1, 0, 0);
		}
		if (Input.GetButtonDown ("Up")) {
			movePlayer(0, 0, 1);
		}
		if (Input.GetButtonDown ("Down")) {
			movePlayer(0, 0, -1);
		}
	}

	bool modelIsMoving() {
		return transform.GetComponentInChildren<Rigidbody>().velocity.magnitude != 0;
	}

	bool movePlayer(int x, int y, int z) {
		if (!modelIsMoving()) {
			Debug.Log("trying to move: " + x.ToString() + y.ToString() + z.ToString());
			int[] newPos = new int[3] {levelPosition[0] + x, levelPosition[1] + y, levelPosition[2] + z};
			try {
				if (levelManager.level[newPos[0], newPos[1], newPos[2]] == null) {
					if (levelManager.level[newPos[0], newPos[1] - 1, newPos[2]] is Transform) {
						levelPosition = newPos;
						transform.Translate (new Vector3 (x, y, z));
					}
				} else {
					levelManager.level[newPos[0], newPos[1], newPos[2]].Translate(new Vector3(x, y, z));
					//TODO: this doesn't change where the blocks are in the data, just visually.
					levelPosition = newPos;
					transform.Translate (new Vector3 (x, y, z));
				}
			}
			catch (IndexOutOfRangeException e) {
				return false;
			}
		}
		return false;
	}

}
