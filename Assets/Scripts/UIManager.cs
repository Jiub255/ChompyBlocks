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
        GM.onScoreChanged += UpdateUI;
        Chomp.onScoreChanged += UpdateUI;
        GM.onWin += Win;
    }

    private void OnDisable()
    {
        GM.onScoreChanged -= UpdateUI;
        Chomp.onScoreChanged -= UpdateUI;
        GM.onWin -= Win;
    }

    private void UpdateUI()
    {
		topScoreText.text = "Score: " + GM.topScore.ToString();
		bottomScoreText.text = "Score: " + GM.bottomScore.ToString();

        topLinesText.text = "Lines completed: " + GM.topLines.ToString();
        bottomLinesText.text = "Lines completed: " + GM.bottomLines.ToString();

        topTreatsText.text = "Cat Treats: " + GM.topTreats.ToString();
        bottomTreatsText.text = "Cat Treats: " + GM.bottomTreats.ToString();
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