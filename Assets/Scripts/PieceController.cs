using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum WhichPlayer 
{
    Top/* = 1*/,
    Bottom/* = -1*/
}

public class PieceController : MonoBehaviour
{
    [SerializeField]
    private GameObject currentPiece;

    [SerializeField]
    private GameObject[] piecePrefabs;

    public float timeBetweenMoves = 1f;
    private float timer = 0f;

    public static event Action<List<int>, Vector3> onChomp;

    // Top or bottom player stuff
    [SerializeField]
    private WhichPlayer player = WhichPlayer.Top;

    private Vector2 spawnPosition;

    private Vector3 upOrDown = Vector3.down;

    private KeyCode rotateRight = KeyCode.UpArrow;
    private KeyCode moveDownOrUp = KeyCode.DownArrow;
    private KeyCode moveLeft = KeyCode.LeftArrow;
    private KeyCode moveRight = KeyCode.RightArrow;

    private void Start()
    {
        spawnPosition = new Vector2(Mathf.RoundToInt(GM.backgroundWidth / 2), GM.halfHeight);

        if (player == WhichPlayer.Bottom)
        {
            upOrDown = Vector3.up;
            spawnPosition = new Vector2(spawnPosition.x, -spawnPosition.y);
            rotateRight = KeyCode.S;
            moveDownOrUp = KeyCode.W;
            moveLeft = KeyCode.A;
            moveRight = KeyCode.D;
        }

        CreateNewPiece();
    }

    private void Update()
    {
        if (!GM.paused)
        {
            MovePiece();
            RotatePiece();
        }
    }

    public void CreateNewPiece()
    {
        GameObject randomBlock = piecePrefabs[UnityEngine.Random.Range(0, piecePrefabs.Length)];

        currentPiece = Instantiate(randomBlock, spawnPosition, Quaternion.identity);

        currentPiece.layer = LayerMask.NameToLayer("Default");

        foreach (Transform brick in currentPiece.transform)
        {
            brick.gameObject.layer = LayerMask.NameToLayer("Default");

            if (Physics2D.OverlapBox(brick.transform.position, Vector3.one * 0.1f, 0f, GM.brickLayer) != null)
            {
                GM.RoundOver(player);
                return;
            }
        }
    }

    private void MovePiece()
    {
        // Move side to side with controls
        if (Input.GetKeyDown(moveLeft) && CanMove(Vector3.left))
        {
            currentPiece.transform.position += Vector3.left;
        }
        if (Input.GetKeyDown(moveRight) && CanMove(Vector3.right))
        {
            currentPiece.transform.position += Vector3.right;
        }

        // Move down (or up) automatically or with key press
        timer += Time.deltaTime;
        if (timer > timeBetweenMoves || Input.GetKeyDown(moveDownOrUp))
        {
            timer = 0;

            if (CanMove(upOrDown))
            {
                currentPiece.transform.position += upOrDown;
            }
            // Else the piece has landed. Check for full lines, delete (and shift remaining bricks)
                // if necessary, then spawn new piece. 
            else
            {
                // Change piece and children bricks to "Brick" layer to differentiate them
                // from the "Default" layer that currentPiece and children bricks are in.
                currentPiece.layer = LayerMask.NameToLayer("Brick");
                foreach (Transform brick in currentPiece.transform)
                {
                    brick.gameObject.layer = LayerMask.NameToLayer("Brick");
                }

                // Check for lines with LineDeleter
                List<int> linesToDelete = LineChecker.CheckLines(upOrDown);
                
                // If there are full lines, start deleting/shifting process
                if (linesToDelete.Count > 0)
                {
                    onChomp?.Invoke(linesToDelete, upOrDown);
                }
                // If no full lines, just create new piece
                else
                {
                    CreateNewPiece();
                }
            }
        }
    }

    private bool CanMove(Vector3 movement, bool checkBoundaries = true)
    {
        foreach (Transform blockTransform in currentPiece.transform)
        {
            Vector3 targetPosition = blockTransform.position + movement;

            // If outside of boundaries, return false.
            if (checkBoundaries)
            {
                if (-upOrDown.y * targetPosition.y < -0.1f ||
                    targetPosition.x < -0.1f ||
                    targetPosition.x > GM.backgroundWidth - 0.9f)
                {
                    return false;
                }
            }

            // If space already taken by another block, return false.
                // Don't think I should use brick layermask here, since piece should never hit itself, 
                // and it should detect the rare collision where you move into the other players piece
                // on the middle line before either have "landed" and become brick layered GO's.
            if (Physics2D.OverlapBox(targetPosition, Vector2.one * 0.1f, 0f))
            {
                // Ignore collisions with self
                if (Physics2D.OverlapBox(targetPosition, Vector2.one * 0.1f, 0f).
                    transform.parent.GetInstanceID() != currentPiece.transform.GetInstanceID())
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void RotatePiece()
    {
        if (Input.GetKeyDown(rotateRight))
        {
            // Rotate piece 90 degrees clockwise.
            currentPiece.transform.Rotate(Vector3.forward * 90);

            // Move this to the end of RotateBlock method? Or not necessary?
            // If space already occupied, rotate back.
            if (!CanMove(Vector3.zero, false))
            {
                currentPiece.transform.Rotate(Vector3.back * 90);
            }

            // Get min and max x values
            List<float> xValues = new List<float>();
            List<float> yValues = new List<float>();
            foreach (Transform t in currentPiece.transform)
            {
                xValues.Add(t.position.x);
                yValues.Add(t.position.y);
            }
            if (player == WhichPlayer.Bottom)
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
                    currentPiece.transform.position += Vector3.right * -xMin;
                }
                // If blocked, undo rotation instead.
                else
                {
                    currentPiece.transform.Rotate(Vector3.back * 90);
                }
            }
            // If off the right side of the screen, move left...
            else if (xMax > GM.backgroundWidth - 1)
            {
                // ...unless blocked by other bricks
                if (CanMove(Vector3.left * (xMax - (GM.backgroundWidth - 1)), false))
                {
                    currentPiece.transform.position += Vector3.left * (xMax - (GM.backgroundWidth - 1));
                }
                // If blocked, undo rotation instead.
                else
                {
                    currentPiece.transform.Rotate(Vector3.back * 90);
                }
            }
            // If past the middle, move back up or back down...
            if (yMin < 0)
            {
                // ...unless blocked by other bricks
                if (CanMove(upOrDown * yMin, false))
                {
                    currentPiece.transform.position += (upOrDown * yMin);
                }
                // If blocked, undo rotation instead.
                else
                {
                    currentPiece.transform.Rotate(Vector3.back * 90);
                }
            }
        }
    }
}