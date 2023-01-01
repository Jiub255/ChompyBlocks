using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI topScoreText;
	[SerializeField]
	private TextMeshProUGUI bottomScoreText;

	[SerializeField]
	private TextMeshProUGUI topLinesText;
	[SerializeField]
	private TextMeshProUGUI bottomLinesText;

	[SerializeField]
	private TextMeshProUGUI topTreatsText;
	[SerializeField]
	private TextMeshProUGUI bottomTreatsText;

    [SerializeField]
    private GameObject winPanel;
    [SerializeField]
    private TextMeshProUGUI winnerText;

    private void Awake()
    {
        UpdateUI();
    }

    private void OnEnable()
    {
        BlockController.onScoreChanged += UpdateUI;
        BlockController.onWin += Win;
    }

    private void OnDisable()
    {
        BlockController.onScoreChanged -= UpdateUI;
        BlockController.onWin -= Win;
    }

    private void UpdateUI()
    {
		topScoreText.text = "Score: " + GameManager.topScore.ToString();
		bottomScoreText.text = "Score: " + GameManager.bottomScore.ToString();

        topLinesText.text = "Lines completed: " + GameManager.topLines.ToString();
        bottomLinesText.text = "Lines completed: " + GameManager.bottomLines.ToString();

        topTreatsText.text = "Cat Treats: " + GameManager.topTreats.ToString();
        bottomTreatsText.text = "Cat Treats: " + GameManager.bottomTreats.ToString();
    }

    private void Win(WhichPlayersBlock player)
    {
        Time.timeScale = 0f;

        WhichPlayersBlock otherPlayer;
        if (player == WhichPlayersBlock.Top)
            otherPlayer = WhichPlayersBlock.Bottom;
        else 
            otherPlayer = WhichPlayersBlock.Top;
        winPanel.SetActive(true);
        winnerText.text = player.ToString() + " Player Wins!" + "\n" +
            "Fuck You, " + otherPlayer.ToString() + " Player!";
        // Have chomp go chomp on loser's name or something.
    }
}