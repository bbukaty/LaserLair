using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTrigger : MonoBehaviour {

	public GameObject previousText;
	public GameObject nextText;
	
	void OnTriggerEnter(Collider other) {
		previousText.SetActive(false);
		nextText.SetActive(true);
	}
}
