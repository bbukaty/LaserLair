using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : MonoBehaviour {

	// NOTES:
	// robot script is a component, it can ask the level manager if moving is ok, maybe get input there?
	// getComponent<PlayerCharacter> <- name of script
	// public playerCharacter player;

	//prefab references
	private Block[,,] level;

	void Start() {
		getLevelFromScene();
	}

	///<summary>
	///Scans through the children of the levelManager in the editor and populates the level array with them. 
	///</summary>
	private void getLevelFromScene() {
		// Gets cube positions straight from the editor to build a data representation of the level.
		// First get the furthest cube in each direction to create a level array of the right size
		int[] maxPositions = new int[3]{ 0, 0, 0 };
		// for each child block of level manager
		foreach (Transform blockTransform in transform) {
			Debug.Assert(blockTransform.GetComponent<Block>() != null, "Warning: Block in level is missing Block script!");
			for (int i = 0; i < 3; i++) {
				int ithDimension = (int)blockTransform.position[i];
				Debug.Assert((float)ithDimension == blockTransform.position[i], "Warning: Level contains misaligned cube!");
				Debug.Assert(ithDimension >= 0, "Warning: Cube coordinates must be positive!");
				if (ithDimension > maxPositions[i]) {
					maxPositions[i] = ithDimension;
				}
			}
		}
		level = new Block[maxPositions[0] + 1, maxPositions[1] + 1, maxPositions[2] + 1];
		// now populate the level array
		foreach (Transform blockTransform in transform) {
			int[] gridPosition = new int[3];
			for (int i = 0; i < 3; i++) {
				gridPosition[i] = (int)blockTransform.position[i];
			}
			level[gridPosition[0], gridPosition[1], gridPosition[2]] = blockTransform.GetComponent<Block>();
		}
		// debug logs
		for (int x = 0; x < level.GetLength(0); x++) {
			for (int y = 0; y < level.GetLength(1); y++) {
				for (int z = 0; z < level.GetLength(2); z++) {
					if (level[x, y, z] != null) {
						Block block = level[x, y, z].GetComponent<Block>();
						Debug.Log("Cube at " + x.ToString() + ", " + y.ToString() + ", " + z.ToString());
						Debug.Log("Orientation: " + block.getOrientation().ToString());
					}
				}
			}
		}
	}

	///<summary>
	///Returns whether pos is a valid index into the level array. 
	///</summary>
	public bool isInBounds(int[] pos) {
		for (int i = 0; i < 3; i++) {
			if (pos[i] < 0 || pos[i] > level.GetUpperBound(i)) {
				return false;
			}
		}
		return true;
	}

	///<summary>
	///Returns the item at pos in the level array or null if pos is out of bounds.
	///</summary>
	public Block getBlockIn(int[] pos) {
		return (isInBounds(pos)) ? level[pos[0], pos[1], pos[2]] : null;
	}

	///<summary>
	///Try to push the block at pos in the movement direction, propogates push through other blocks.
	///Returns whether push was successful. 
	///</summary>
	public bool tryPush(int[] pos, int[] movement) {
		bool canPush = false;
		int[] adjacentPos = new int[3] {pos[0]+movement[0], pos[1]+movement[1], pos[2]+movement[2]};
		Block blockToPush = getBlockIn(pos);
		if (blockToPush.isPushable) {
			if (isInBounds(adjacentPos)) {
				Block adjacentBlock = getBlockIn(adjacentPos);
				if (adjacentBlock == null) {
					canPush = true;
				} else if (adjacentBlock.isPushable) {
					canPush = tryPush(adjacentPos, movement);
				}
			}
		}
		if (canPush) {
			swapBlocks(pos, adjacentPos);
			blockToPush.moveModel(movement);
		}
		
		return canPush;
	}

	private void swapBlocks(int[] pos1, int[] pos2) {
		Block temp = getBlockIn(pos1);
		level[pos1[0], pos1[1], pos1[2]] = getBlockIn(pos2);
		level[pos2[0], pos2[1], pos2[2]] = temp;
	}

	public bool isInLaser(int[] pos) {
		bool result = false;
		for (int sign = -1; sign <= 1; sign += 2) {
			for (int i = 0; i < 3; i++) {
				int[] searchOrientation = new int[3] {0, 0, 0};
				searchOrientation[i] = sign;
				if (isLaserInDirection(searchOrientation, pos)) {
					result = true;
				}
			}
		}
		return result;
	}

	private bool isLaserInDirection(int[] orientation, int[] pos) {
		int[] adjacentPos = new int[3] {pos[0]+orientation[0], pos[1]+orientation[1], pos[2]+orientation[2]};
		if (!isInBounds(adjacentPos)) {
			return false;
		}
		Block adjacentBlock = getBlockIn(adjacentPos);
		if (adjacentBlock == null) {
			// there's no block in the way, keep searching in this direction
			return isLaserInDirection(orientation, adjacentPos);
		} else if (adjacentBlock is LaserBlock && adjacentBlock.orientationIsReverseOf(orientation)) {
			// there's a laser pointing towards pos
			return true;
		} else {
			// there's a non-laser block in the way of any potential beams in this direction
			return false;
		}
	}
}

