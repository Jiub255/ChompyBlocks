using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour
{
	public static int topScore = 0;
	public static int bottomScore = 0;

	public static int topLines = 0;
	public static int bottomLines = 0;

	public static int topTreats = 3;
	public static int bottomTreats = 3;

	[SerializeField]
	private GameObject background;
	public static int backgroundWidth;
	public static int backgroundHeight;
	public static int halfHeight;

    public static event Action onScoreChanged;
    public static event Action<WhichPlayer> onWin;

    [SerializeField]
    private int winsNeededInspector = 3;
    public static int winsNeeded = 3;

    [SerializeField]
    private LayerMask brickLayerInspector;
    public static LayerMask brickLayer;

    public static bool paused = false;

    private void Awake()
    {
		backgroundWidth = Mathf.RoundToInt(background.transform.localScale.x);
		backgroundHeight = Mathf.RoundToInt(background.transform.localScale.y);
		halfHeight = Mathf.FloorToInt(backgroundHeight / 2);

        winsNeeded = winsNeededInspector;
        brickLayer = brickLayerInspector;
	}

    private void Update()
    {
       // if (Time.timeScale < 0.5f && Input.GetKeyDown(KeyCode.Any)
        {
			//PlayAgain();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        //Time.timeScale = (Time.timeScale + 1f) % 2;
        paused = !paused;
    }

    public static void RoundOver(WhichPlayer player)
    {
        if (player == WhichPlayer.Top)
        {
            GM.bottomScore++;
            if (GM.bottomScore >= winsNeeded)
            {
                GameOver(WhichPlayer.Bottom);
            }
        }
        else
        {
            GM.topScore++;
            if (GM.topScore >= winsNeeded)
            {
                GameOver(WhichPlayer.Top);
            }
        }
        //Debug.Log(GameManager.topScore + " vs " + GameManager.bottomScore);

        GM.topLines = 0;
        GM.bottomLines = 0;

        onScoreChanged?.Invoke();

        if (GM.bottomScore < winsNeeded && GM.topScore < winsNeeded)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void GameOver(WhichPlayer player)
    {
        onWin?.Invoke(player);
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