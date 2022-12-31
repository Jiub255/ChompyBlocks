using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static int topScore = 0;
	public static int bottomScore = 0;

	public static int topLines = 0;
	public static int bottomLines = 0;

	public static int topTreats = 0;
	public static int bottomTreats = 0;

    private void Update()
    {
       // if (Time.timeScale < 0.5f && Input.GetKeyDown(KeyCode.Any)
        {
			//PlayAgain();
        }
    }

    public void PlayAgain()
    {
		topScore = 0;
		bottomScore = 0;
		topLines = 0;
		bottomLines = 0;
		topTreats = 0;
		bottomTreats = 0;

		Time.timeScale = 1f;
		
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}