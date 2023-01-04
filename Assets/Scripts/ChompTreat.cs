using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ChompTreat : MonoBehaviour
{
    [SerializeField]
    private GameObject catTreatPrefab;

    [SerializeField]
    private float speed = 50f;

    public static event Action onScoreChanged;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    private void Update()
    {
        if (!GM.paused)
        {
            // For top player
            if (Input.GetKeyDown(KeyCode.RightShift) && GM.topTreats > 0)
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

                if (bottomBricks.Count > 0)
                {
                    // Pause gameplay
                    GM.paused = true;

                    GM.topTreats--;
                    onScoreChanged?.Invoke();

                    // Choose a random brick from list
                    int randomInt = UnityEngine.Random.Range(0, bottomBricks.Count);
                    GameObject randomBrick = bottomBricks[randomInt];
                   // Debug.Log(randomBrick.transform.position);

                    // Throw cat treat onto brick
                    GameObject treatInstance = Instantiate(catTreatPrefab);
                    treatInstance.transform.position = new Vector3(GM.backgroundWidth + 3, 0, 0);
                    StartCoroutine(MoveTreatToBrick(treatInstance.transform, randomBrick.transform.position));

                    // Have chompy come eat that brick and cat treat
                    // Run from end of MoveTreatToBrick coroutine?
                }
            }

            // For bottom player
            if (Input.GetKeyDown(KeyCode.LeftShift) && GM.bottomTreats > 0)
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

                if (topBricks.Count > 0)
                {
                    // Pause gameplay
                    GM.paused = true;

                    GM.bottomTreats--;
                    onScoreChanged?.Invoke();

                    // Choose a random brick from list
                    int randomInt = UnityEngine.Random.Range(0, topBricks.Count);
                    GameObject randomBrick = topBricks[randomInt];
                   // Debug.Log(randomBrick.transform.position);

                    // Throw cat treat onto brick
                    GameObject treatInstance = Instantiate(catTreatPrefab);
                    treatInstance.transform.position = new Vector3(-3, 0, 0);
                    StartCoroutine(MoveTreatToBrick(treatInstance.transform, randomBrick.transform.position));

                    // Have chompy come eat that brick and cat treat
                    // Run from end of MoveTreatToBrick coroutine?
                }
            }
        }
    }

    private IEnumerator MoveTreatToBrick(Transform treat, Vector2 targetPosition)
    {
/*        // Pause gameplay, maybe set timeScale to 0?
        GM.paused = true;*/
       // Time.timeScale = 0f;

        // Move treat to brick
        while (Vector2.Distance(targetPosition, treat.position) > 0.5f)
        {
            treat.position = Vector3.MoveTowards(treat.position, targetPosition, Time.deltaTime * speed);

            yield return null;
        }

        treat.position = targetPosition;

        StartCoroutine(ChompTreatAndBrick(targetPosition));
    }

    private IEnumerator ChompTreatAndBrick(Vector2 treatPosition)
    {
        // Chomp goes to treat

        spriteRenderer.enabled = true;
        
        // Bottom Player
        if (treatPosition.y > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            transform.position = new Vector3(-3, 0, 0);
        }
        // Top Player
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.position = new Vector3(GM.backgroundWidth + 3, 0, 0);
        }

        while (Vector2.Distance(treatPosition, transform.position) > 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, treatPosition, Time.deltaTime * speed);

            yield return null;
        }

        transform.position = treatPosition;

        //Debug.Log("Moved chomp to treat");

        // Chomp eats treat and brick
        Collider2D[] hits = Physics2D.OverlapBoxAll(treatPosition, Vector2.one * 0.1f, 0, GM.brickLayer);
        foreach (Collider2D hit in hits)
        {
            //Debug.Log(hit.gameObject.name);
            Destroy(hit.gameObject);
        }

        // Chomp goes to other side of board
        // Bottom Player
        Vector2 homeTarget;

        if (treatPosition.y > 0)
        {
            homeTarget = new Vector3(GM.backgroundWidth + 3, 0, 0);
        }
        // Top Player
        else
        {
            homeTarget = new Vector3(-3, 0, 0);
        }

        while (Vector2.Distance(homeTarget, transform.position) > 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, homeTarget, Time.deltaTime * speed);

            yield return null;
        }

        transform.position = homeTarget;

        spriteRenderer.enabled = false;

        // Unpause gameplay, set timeScale back to 1?
        GM.paused = false;
       // Time.timeScale = 1f;
    }
}