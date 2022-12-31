using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum WhichPlayersBlock 
{
    Top,
    Bottom
}

public class BlockController : MonoBehaviour
{
    [SerializeField]
    private GameObject currentBlock;

    [SerializeField]
    private GameObject[] blockPrefabs;

    public float timeBetweenMoves = 1f;
    private float timer = 0f;

    [SerializeField]
    private GameObject background;
    private int backgroundWidth;
    private int backgroundHeight;
    private int halfHeight;

    [SerializeField]
    private LayerMask brickLayer;

    private bool wait = false;
    private float waitTimer;

    public static event Action onScoreChanged;
    public static event Action<List<int>> onChomp;
    private List<int> chompYValues = new List<int>();

    private int linesDeleted = 0;

    // Top or bottom player stuff
    [SerializeField]
    private WhichPlayersBlock player = WhichPlayersBlock.Top;

    [SerializeField]
    private Vector2 blockSpawnPosition;

    private Vector3 upOrDown = Vector3.down;

    private KeyCode rotateRight = KeyCode.UpArrow;
    private KeyCode moveDownOrUp = KeyCode.DownArrow;
    private KeyCode moveLeft = KeyCode.LeftArrow;
    private KeyCode moveRight = KeyCode.RightArrow;

    private void Awake()
    {
        backgroundWidth = Mathf.RoundToInt(background.transform.localScale.x);
        backgroundHeight = Mathf.RoundToInt(background.transform.localScale.y);
        halfHeight = Mathf.FloorToInt(backgroundHeight / 2);

        if (player == WhichPlayersBlock.Bottom)
        {
            upOrDown = Vector3.up;
            blockSpawnPosition = new Vector2(blockSpawnPosition.x, -blockSpawnPosition.y);
            rotateRight = KeyCode.S;
            moveDownOrUp = KeyCode.W;
            moveLeft = KeyCode.A;
            moveRight = KeyCode.D;
            wait = true;
            waitTimer = timeBetweenMoves / 2;
        }
    }

    private void Start()
    {
        if (player == WhichPlayersBlock.Top)
        {
            CreateNewBlock();
        }
    }

    private void Update()
    {
        if (!wait)
        {
            MoveBlock();
            RotateBlock();
        }
        else
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer < 0)
            {
               // Debug.Log("Done waiting");
                wait = false;
                CreateNewBlock();
            }
        }
    }

    private void CreateNewBlock()
    {
        if (Physics2D.OverlapBox(blockSpawnPosition, Vector3.one * 0.1f, 0f) != null)
        {
            GameOver(player);
        }

        GameObject randomBlock = blockPrefabs[UnityEngine.Random.Range(0, blockPrefabs.Length)];

        currentBlock = Instantiate(randomBlock, blockSpawnPosition, Quaternion.identity);

/*        Debug.Log("New block from " + player.ToString() +
            " at " + currentBlock.transform.position.ToString());*/
    }

    private void GameOver(WhichPlayersBlock player)
    {
        if (player == WhichPlayersBlock.Top)
        {
            GameManager.bottomScore++;
        }
        else
        {
            GameManager.topScore++;
        }
        Debug.Log(GameManager.topScore + " vs " + GameManager.bottomScore);

        GameManager.topLines = 0;
        GameManager.bottomLines = 0;

        onScoreChanged?.Invoke();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void RotateBlock()
    {
        if (Input.GetKeyDown(rotateRight))
        {
            // Rotate piece 90 degrees clockwise.
            currentBlock.transform.Rotate(Vector3.forward * 90);

            // If space already occupied, rotate back.
            if (!CanMove(Vector3.zero, false))
            {
                currentBlock.transform.Rotate(Vector3.back * 90);
            }

            // Get min and max x values
            List<float> xValues = new List<float>();
            List<float> yValues = new List<float>();
            foreach (Transform t in currentBlock.transform)
            {
                xValues.Add(t.position.x);
                yValues.Add(t.position.y);
            }
            if (player == WhichPlayersBlock.Bottom)
            {
                for (int i = 0; i < yValues.Count; i++)
                {
                    yValues[i] = -yValues[i];
                }
            }
            float xMin = xValues.Min();
            float xMax = xValues.Max();
            float yMin = yValues.Min();

            // If off the left side of the screen, move right...
            if (xMin < 0)
            {
                // ...unless blocked by other bricks
                if (CanMove(Vector3.right * -xMin, false))
                {
                    currentBlock.transform.position += Vector3.right * -xMin;
                }
                // If blocked, undo rotation instead.
                else
                {
                    currentBlock.transform.Rotate(Vector3.back * 90);
                }
            }
            // If off the right side of the screen, move left...
            else if (xMax > backgroundWidth - 1)
            {
                // ...unless blocked by other bricks
                if (CanMove(Vector3.left * (xMax - (backgroundWidth - 1)), false))
                {
                    currentBlock.transform.position += Vector3.left * (xMax - (backgroundWidth - 1));
                }
                // If blocked, undo rotation instead.
                else
                {
                    currentBlock.transform.Rotate(Vector3.back * 90);
                }
            }
            // If past the middle, move back up or back down...
            if (yMin < 0)
            {
                // ...unless blocked by other bricks
                if (CanMove(upOrDown * yMin, false))
                {
                    currentBlock.transform.position += (upOrDown * yMin);
                }
                // If blocked, undo rotation instead.
                else
                {
                    currentBlock.transform.Rotate(Vector3.back * 90);
                }
            }
        }
    }

    private void MoveDirectionIfPossible(Vector3 direction)
    {
        if (CanMove(direction))
        {
            currentBlock.transform.position += direction;
        }
    }

    private void MoveBlock()
    {
        // Move side to side with controls
        if (Input.GetKeyDown(moveLeft))
        {
            MoveDirectionIfPossible(Vector3.left);

        }
        if (Input.GetKeyDown(moveRight))
        {
            MoveDirectionIfPossible(Vector3.right);
        }

        // Move down (or up) automatically or with key press
        timer += Time.deltaTime;
        if (timer > timeBetweenMoves || Input.GetKeyDown(moveDownOrUp))
        {
            timer = 0;

            if (CanMove(upOrDown))
            {
                currentBlock.transform.position += upOrDown;
            }
            else
            {
                // Checks for full lines. If found, deletes them
                    // and moves "outside" blocks toward the center line.
                List<Transform> transforms = CheckForFullLines();
                if (transforms != null)
                {
                    // DeleteLine runs itself recursively until all full lines are deleted.
                    // Then it runs MoveBricksTowardCenter, then DeleteLines again until no lines are left.
                    DeleteLine(transforms);
                }

                CreateNewBlock();
            }
        }
    }

    private bool CanMove(Vector3 movement, bool checkBoundaries = true)
    {
        foreach (Transform blockTransform in currentBlock.transform)
        {
            Vector3 targetPosition = blockTransform.position + movement;

            // If outside of boundaries, return false.
            if (checkBoundaries)
            {
                if (/*Mathf.Abs(targetPosition.y) > halfHeight ||*/
                    -upOrDown.y * targetPosition.y < -0.1f ||
                    targetPosition.x < -0.1f ||
                    targetPosition.x > backgroundWidth - 0.9f)
                {
                    Debug.Log(blockTransform.name + " outside of boundaries: " + targetPosition);
                    return false;
                }
            }

            // If space already taken by another block, return false.
            if (Physics2D.OverlapBox(targetPosition, Vector2.one * 0.1f, 0f))
            {
                // Ignore collisions with self
                if (Physics2D.OverlapBox(targetPosition, Vector2.one * 0.1f, 0f).
                    transform.parent.GetInstanceID() != currentBlock.transform.GetInstanceID())
                {
/*                    Debug.Log(Physics2D.OverlapBox(targetPosition, Vector2.one * 0.1f, 0f).ToString() 
                        + " at " + targetPosition.ToString());*/
                    Debug.Log(targetPosition + " already occupied by another piece: " +
                        Physics2D.OverlapBox(targetPosition, Vector2.one * 0.1f, 0f).transform.parent.name);
                    return false;
                }
            }
        }

        return true;
    }

    private List<Transform> CheckForFullLines()
    {
        for (int i = 0;
            Mathf.Abs(i) < halfHeight;
            i -= Mathf.RoundToInt(upOrDown.y))
        {
            if (CheckLine(i) != null)
            {
                return CheckLine(i);
            }
        }

        return null;
    }

    private List<Transform> CheckLine(int yValue)
    {
        List<Transform> blockTransforms = new List<Transform>();

        for (int i = 0; i < backgroundWidth; i++)
        {
        if (Physics2D.OverlapBox(new Vector2(i, yValue), Vector2.one * 0.1f, 0f) != null)
            {
                Debug.Log(Physics2D.OverlapBox(new Vector2(i, yValue), Vector2.one * 0.1f, 0f)
                    + " at (" + i + ", " + yValue + ")");

                Transform blockTransform = Physics2D.OverlapBox(
                    new Vector2(i, yValue), Vector2.one * 0.1f, 0f).transform;

                blockTransforms.Add(blockTransform);
            }
            else
            {
                return null;
            }
        }

        Debug.Log("Block Transforms in row " + yValue.ToString() + ": " + blockTransforms.Count);

        return blockTransforms;
    }

    private void DeleteLine(List<Transform> blockTransforms)
    {
        int yValue = Mathf.RoundToInt(blockTransforms[0].position.y);
        Debug.Log("Deleting line " + yValue);

        // Have Chomp do the actual destroying by moving Chomp game object across each line
        // Chomp GO has trigger collider that destroys the blocks.
        foreach (Transform blockTransform in blockTransforms)
        {
            Destroy(blockTransform.gameObject);
        }

        linesDeleted++;

        if (player == WhichPlayersBlock.Top)
        {
            GameManager.topLines++;
        }
        else
        {
            GameManager.bottomLines++;
        }

        chompYValues.Add(yValue);

        onScoreChanged?.Invoke();

        // Recursively run through DeleteLine until all full lines are deleted
        List<Transform> transforms = CheckForFullLines();
        if (transforms != null)
        {
            DeleteLine(transforms);
        }
        // If no more full lines before moving bricks toward center
        else
        {
            // Chomp animation
            onChomp?.Invoke(chompYValues);
            chompYValues.Clear();

            MoveBricksTowardCenter(yValue);
        }
    }

    private void MoveBricksTowardCenter(int yValue)
    {
        // Run through deleteLine recursively until done, then move bricks.
            // Can do the animation better this way too.

        Debug.Log("Moving bricks from line " + yValue + " outward");

        List<GameObject> remainingBricks = new List<GameObject>();

        for (int i = 0; i < backgroundWidth; i++)
        {
            for (int j = yValue; 
                Mathf.Abs(j) < halfHeight;
                j -= Mathf.RoundToInt(upOrDown.y))
            {
                if (Physics2D.OverlapBox(new Vector2(i, j), Vector2.one * 0.1f, 0f) != null)
                {
                    GameObject brick = Physics2D.OverlapBox(
                        new Vector2(i, j), Vector2.one * 0.1f, 0f).gameObject;

                    remainingBricks.Add(brick);
                }
            }
        }

        foreach (GameObject remainingBrick in remainingBricks)
        {
            remainingBrick.transform.position += upOrDown;
        }

        // Recursively run through DeleteLine -> MoveBricksTowardCenter until all full lines are deleted
        List<Transform> transforms = CheckForFullLines();
        if (transforms != null)
        {
            DeleteLine(transforms);
        }
        else
        {
            // send linesDeleted to UI/GameManager?
            // popup animation?

            // Add treats for multiple lines deleted
            if (player == WhichPlayersBlock.Top)
            {
                GameManager.topTreats += linesDeleted - 1;
                linesDeleted = 0;
            }
            else
            {
                GameManager.bottomTreats += linesDeleted - 1;
                linesDeleted = 0;
            }
        }
    }
}