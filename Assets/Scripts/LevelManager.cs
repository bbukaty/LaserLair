using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour {

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
			Block block = blockTransform.GetComponent<Block>();
			for (int i = 0; i < 3; i++) {
				if (block.levelPos[i] > levelBounds[i]) {
					levelBounds[i] = block.levelPos[i];
				}
			}
		}

		levelBounds = levelBounds + new Vector3Int(1,1,1); // account for zero-indexing
		level = new Block[levelBounds.x, levelBounds.y, levelBounds.z];
		
		// now populate the level array
		foreach (Transform blockTransform in transform) {
			addBlock(blockTransform.GetComponent<Block>());
		}
		// printLevel();
	}

	public void printLevel() {
		for (int x = 0; x < level.GetLength(0); x++) {
			for (int y = 0; y < level.GetLength(1); y++) {
				for (int z = 0; z < level.GetLength(2); z++) {
					if (level[x, y, z] != null) {
						Block block = level[x, y, z].GetComponent<Block>();
						Debug.Log("Cube at " + block.levelPos.ToString());
						Debug.Log("Orientation: " + block.orientation.ToString());
					}
				}
			}
		}
	}

	public void win() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
	///Returns the item 1 y-unit below pos in the level array or null if that is out of bounds.
	///</summary>
	public Block getBlockUnder(Vector3Int pos) {
		Vector3Int under = pos + new Vector3Int(0,-1,0);
		return (isInBounds(under)) ? level[under[0], under[1], under[2]] : null;
	}

	public Block getBlockAbove(Vector3Int pos) {
		Vector3Int above = pos + new Vector3Int(0,1,0);
		return (isInBounds(above)) ? level[above[0], above[1], above[2]] : null;
	}

	///<summary>
	///Try to push the block at pos in the movement direction, propogates push through other blocks.
	///Returns whether push was successful. 
	///</summary>
	public bool tryPush(Vector3Int pos, Vector3Int movement, bool justChecking = true) {
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
		if (canPush && !justChecking) {
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
		if (adjacentBlock == null || adjacentBlock is GlassBlock) {
			// there's no block in the way, keep searching in this direction
			return isLaserInDirection(direction, adjacentPos);
		} else if (adjacentBlock is LaserBlock && adjacentBlock.orientation == direction * -1) {
			// there's a laser pointing towards pos
			return true;
		} else {
			// there's a non-laser block in the way of any potential beams in this direction
			return false;
		}
	}

	public void addBlock(Block blockToAdd) {
		Vector3Int pos = blockToAdd.levelPos;
		Debug.Assert(level[pos.x, pos.y, pos.z] == null, "Warning: Adding block into an occupied position in the level!");
		level[pos.x, pos.y, pos.z] = blockToAdd;
	}

	public void explodePos(Vector3Int pos) {
		for (int sign = -1; sign <= 1; sign += 2) {
			for (int i = 0; i < 3; i++) {
				Vector3Int searchOrientation = new Vector3Int(0, 0, 0);
				searchOrientation[i] = sign;
				explodeDirection(searchOrientation, pos);
			}
		}
	}

	public void explodeDirection(Vector3Int direction, Vector3Int pos) {
		Vector3Int adjacentPos = pos + direction;
		if (!isInBounds(adjacentPos)) {
			return;
		}
		if (getBlockIn(adjacentPos) == null) {
			// play explode animation on empty block, continue
			explodeDirection(direction, adjacentPos);
		} else if (tryExplodeBlock(adjacentPos)) {
			// adjacent pos block was destructible, continue
			explodeDirection(direction, adjacentPos);
			// return; // if you want to only blow up one block in a direction
		} else {
			// there's a non-laser block in the way of any potential beams in this direction
			return;
		}
	}

	public bool tryExplodeBlock(Vector3Int pos){
		Block toExplode = getBlockIn(pos);  
		if (toExplode != null && toExplode.isDestructible) {
			Destroy(toExplode.gameObject);
			return true;
		}
		return false;
	}
}

