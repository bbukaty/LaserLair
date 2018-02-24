using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : MonoBehaviour {

	// NOTES:
	// robot script is a component, it can ask the level manager if moving is ok, maybe get input there?
	// getComponent<PlayerCharacter> <- name of script
	// public playerCharacter player;

	//prefab references
	public Transform blockRobotPrefab;

	public Transform[,,] level;

	void Start() {
		getLevelFromScene();
	}

	private void getLevelFromScene() {
		// Gets cube positions straight from the editor to build a data representation of the level.
		// First get the furthest cube in each direction to create a level array of the right size
		int[] maxPositions = new int[3]{ 0, 0, 0 };
		// for each child block of level manager
		foreach (Transform block in transform) {
			for (int i = 0; i < 3; i++) {
				int ithDimension = (int)block.position[i];
				Debug.Assert((float)ithDimension == block.position[i], "Error: Level contains misaligned cube!");
				Debug.Assert(ithDimension >= 0, "Error: Cube coordinates must be positive!");
				if (ithDimension > maxPositions[i]) {
					maxPositions[i] = ithDimension;
				}
			}
		}
		level = new Transform[maxPositions[0] + 1, maxPositions[1] + 1, maxPositions[2] + 1];
		// now populate the level array
		foreach (Transform block in transform) {
			int[] gridPosition = new int[3];
			for (int i = 0; i < 3; i++) {
				gridPosition[i] = (int)block.position[i];
			}
			level[gridPosition[0], gridPosition[1], gridPosition[2]] = block;
		}
		// debug logs
		for (int x = 0; x < level.GetLength(0); x++) {
			for (int y = 0; y < level.GetLength(1); y++) {
				for (int z = 0; z < level.GetLength(2); z++) {
					if (level[x, y, z] != null) {
						Debug.Log("Cube at " + x.ToString() + ", " + y.ToString() + ", " + z.ToString());
					}
				}
			}
		}
	}
}

