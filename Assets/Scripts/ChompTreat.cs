using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChompTreat : MonoBehaviour
{
    [SerializeField]
    private GameObject catTreatPrefab;

    [SerializeField]
    private float speed = 50f;

    private void Update()
    {
        if (!PieceController.paused)
        {
            // For top player
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                // Get all bottom player's bricks in a list (except the current piece ones)
                List<GameObject> bottomBricks = new List<GameObject>();

                for (int y = -1; y >= -GM.halfHeight; y--)
                {
                    for (int x = 0; x < GM.backgroundWidth; x++)
                    {
                        if (Physics2D.OverlapBox(new Vector2(x, y), Vector2.one * 0.1f, 0f, GM.brickLayer) != null)
                        {
                            GameObject brick = Physics2D.OverlapBox(
                                new Vector2(x, y), Vector2.one * 0.1f, 0f).gameObject;

                            bottomBricks.Add(brick);
                        }
                    }
                }

                // Choose a random brick from list
                int randomInt = Random.Range(0, bottomBricks.Count);
                GameObject randomBrick = bottomBricks[randomInt];
                Debug.Log(randomBrick.transform.position);

                // Throw cat treat onto brick
                GameObject treatInstance = Instantiate(catTreatPrefab);
                treatInstance.transform.position = new Vector2(GM.backgroundWidth + 3, 0);
                StartCoroutine(MoveTreatToBrick(treatInstance.transform, randomBrick.transform.position));

                // Have chompy come eat that brick and cat treat
                // Run from end of MoveTreatToBrick coroutine?
            }

            // For bottom player
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                // Get all top player's bricks in a list (except the current piece ones)
                List<GameObject> topBricks = new List<GameObject>();

                for (int y = 1; y <= GM.halfHeight; y++)
                {
                    for (int x = 0; x < GM.backgroundWidth; x++)
                    {
                        if (Physics2D.OverlapBox(new Vector2(x, y), Vector2.one * 0.1f, 0f, GM.brickLayer) != null)
                        {
                            GameObject brick = Physics2D.OverlapBox(
                                new Vector2(x, y), Vector2.one * 0.1f, 0f).gameObject;

                            topBricks.Add(brick);
                        }
                    }
                }

                // Choose a random brick from list
                int randomInt = Random.Range(0, topBricks.Count);
                GameObject randomBrick = topBricks[randomInt];
                Debug.Log(randomBrick.transform.position);

                // Throw cat treat onto brick

                // Have chompy come eat that brick and cat treat

            }
        }
    }

    private IEnumerator MoveTreatToBrick(Transform treat, Vector2 targetPosition)
    {
        while (Vector2.Distance(targetPosition, treat.transform.position) > 0.1f)
        {
            treat.transform.position = Vector3.MoveTowards(treat.transform.position, targetPosition, Time.deltaTime * speed);

            yield return null;
        }
    }
}