using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

public void playGame()
{
	SceneManager.LoadScene("level1");
}

public void loadLevelSelect() 
{
	SceneManager.LoadScene("Level Select");
}

public void QuitGame() 
{
	Application.Quit();
}

}
