using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {
	
	public float followSpeed;

	private Transform player;
	private Vector3 startPos;
	private Vector3 offset;

	void Start () {
		startPos = transform.position;
		player = null;
	}
	
	public void follow(Transform newPlayer) {
		player = newPlayer;
		offset = startPos - newPlayer.position;
	}

	void Update () {
		Vector3 destination;
		if (player != null) {
			destination = new Vector3(startPos.x, player.position.y + offset.y, player.position.z + offset.z);
		} else {
			destination = startPos;
			// no player assigned yet, or player dead: return to startpos
		}
		transform.position = Vector3.Lerp(transform.position, destination, followSpeed*Time.deltaTime);
	}


}
