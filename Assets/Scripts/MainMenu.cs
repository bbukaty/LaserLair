using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

public Transform menuCamera;

public void showPrologue()
{
	menuCamera.transform.position = new Vector3(17.58f, 1f, -10f);
}

public void loadFirstLevel()
{
	SceneManager.LoadScene("level1");
}

public void loadLevelSelect() 
{
	SceneManager.LoadScene("Level Select");
}

public void quit()
{
	Application.Quit();
}

}
