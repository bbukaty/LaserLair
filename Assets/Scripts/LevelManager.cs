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
		// First get the furthest cube in each direction to create a level array of the right size
		Vector3Int levelBounds = new Vector3Int(0,0,0);
		// for each child block of level manager
		foreach (Transform blockTransform in transform) {
			Debug.Assert(blockTransform.GetComponent<Block>() != null, "Warning: Block in level is missing Block script!");
			Vector3Int blockPosition = Vector3Int.RoundToInt(blockTransform.position);
			Debug.Assert((Vector3)blockPosition == blockTransform.position, "Warning: Improperly aligned block in editor!");
			for (int i = 0; i < 3; i++) {
				if (blockPosition[i] > levelBounds[i]) {
					levelBounds[i] = blockPosition[i];
				}
			}
		}

		levelBounds = levelBounds + new Vector3Int(1,1,1); // account for zero-indexing
		level = new Block[levelBounds.x, levelBounds.y, levelBounds.z];
		// now populate the level array
		foreach (Transform blockTransform in transform) {
			Vector3Int blockPosition = Vector3Int.RoundToInt(blockTransform.position);
			level[blockPosition.x, blockPosition.y, blockPosition.z] = blockTransform.GetComponent<Block>();
		}
		//printLevel();
	}


	public void printLevel() {
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
	public bool isInBounds(Vector3Int pos) {
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
	public Block getBlockIn(Vector3Int pos) {
		return (isInBounds(pos)) ? level[pos[0], pos[1], pos[2]] : null;
	}

	///<summary>
	///Try to push the block at pos in the movement direction, propogates push through other blocks.
	///Returns whether push was successful. 
	///</summary>
	public bool tryPush(Vector3Int pos, Vector3Int movement) {
		bool canPush = false;
		Block blockToPush = getBlockIn(pos);
		Vector3Int adjacentPos = pos + movement;
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

	private void swapBlocks(Vector3Int pos1, Vector3Int pos2) {
		Block temp = getBlockIn(pos1);
		level[pos1.x, pos1.y, pos1.z] = getBlockIn(pos2);
		level[pos2.x, pos2.y, pos2.z] = temp;
	}

	///<summary>
	///Returns whether or not a laser passes through position pos.
	///</summary>
	public bool isInLaser(Vector3Int pos) {
		bool result = false;
		for (int sign = -1; sign <= 1; sign += 2) {
			for (int i = 0; i < 3; i++) {
				Vector3Int searchOrientation = new Vector3Int(0, 0, 0);
				searchOrientation[i] = sign;
				if (isLaserInDirection(searchOrientation, pos)) {
					result = true;
				}
			}
		}
		return result;
	}

	private bool isLaserInDirection(Vector3Int direction, Vector3Int pos) {
		Vector3Int adjacentPos = pos + direction;
		if (!isInBounds(adjacentPos)) {
			return false;
		}
		Block adjacentBlock = getBlockIn(adjacentPos);
		if (adjacentBlock == null) {
			// there's no block in the way, keep searching in this direction
			return isLaserInDirection(direction, adjacentPos);
		} else if (adjacentBlock is LaserBlock && adjacentBlock.getOrientation() == direction * -1) {
			// there's a laser pointing towards pos
			return true;
		} else {
			// there's a non-laser block in the way of any potential beams in this direction
			return false;
		}
	}

	public void addBlock(Vector3Int pos, Block blockToAdd) {
		Debug.Assert(level[pos.x, pos.y, pos.z] == null, "Warning: Adding block into an occupied position in the level!");
		level[pos.x, pos.y, pos.z] = blockToAdd;
	}
}

