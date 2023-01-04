using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDeleter : MonoBehaviour
{
	public static List<int> CheckLines(Vector3 upOrDown)
    {
        List<int> yValues = new List<int>();

        for (int y = 0;
            Mathf.Abs(y) < GM.halfHeight;
            y -= Mathf.RoundToInt(upOrDown.y))
        {
            if (CheckLine(y))
            {
                yValues.Add(y);
            }
        }

        return yValues;
    }

    private static bool CheckLine(int yCoordinate)
    {
        List<Transform> blockTransforms = new List<Transform>();

        for (int i = 0; i < GM.backgroundWidth; i++)
        {
            if (Physics2D.OverlapBox(new Vector2(i, yCoordinate), Vector2.one * 0.1f, 0f) != null)
            {
                Transform blockTransform = Physics2D.OverlapBox(
                    new Vector2(i, yCoordinate), Vector2.one * 0.1f, 0f, GM.brickLayer).transform;

                blockTransforms.Add(blockTransform);
            }
        }

        if (blockTransforms.Count < GM.backgroundWidth)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}