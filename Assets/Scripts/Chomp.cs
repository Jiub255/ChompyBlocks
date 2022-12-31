using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chomp : MonoBehaviour
{
	[SerializeField]
	private GameObject chomper;

    private void OnEnable()
    {
        BlockController.onChomp += ChompLines;
    }

    private void OnDisable()
    {
        BlockController.onChomp -= ChompLines;
    }

    private void ChompLines(List<int> lineYValues)
    {
        lineYValues.Sort((x, y) => x.CompareTo(y));

        foreach (int yValue in lineYValues)
        {

        }
    }
}