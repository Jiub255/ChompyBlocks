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

    private void Awake()
    {
        UpdateUI();
    }

    private void OnEnable()
    {
        BlockController.onScoreChanged += UpdateUI;
    }

    private void OnDisable()
    {
        BlockController.onScoreChanged -= UpdateUI;
    }

    private void UpdateUI()
    {
		topScoreText.text = "Top: " + GameManager.topScore.ToString();
		bottomScoreText.text = "Bottom: " + GameManager.bottomScore.ToString();

        topLinesText.text = "Lines completed: " + GameManager.topLines.ToString();
        bottomLinesText.text = "Lines completed: " + GameManager.bottomLines.ToString();
    }
}