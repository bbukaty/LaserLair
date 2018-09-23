using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

public Transform menuCamera;

public void showPrologue()
{
	menuCamera.transform.position = new Vector3(8.65f, 1f, -10f);
}

public void showInstructions()
{
	menuCamera.transform.position = new Vector3(17.3f, 1f, -10f);
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
