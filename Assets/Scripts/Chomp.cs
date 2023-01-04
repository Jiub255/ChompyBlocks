using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Chomp : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private float speed = 150f;

    [SerializeField]
    private PieceController topBlockController;
    [SerializeField]
    private PieceController bottomBlockController;

    public static event Action onScoreChanged;

    // "Collision" detection with bricks using raycast stuff
    Vector3 lastPosition;
    bool chomping = false;

    private void Awake()
    {
        lastPosition = transform.position + Vector3.right * 0.01f;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    private void OnEnable()
    {
        PieceController.onChomp += ChompLines;
    }

    private void OnDisable()
    {
        PieceController.onChomp -= ChompLines;
    }

    // Use this instead of colliders? Collisions not detected when speed too high.
    private void Update()
    {
        if (chomping)
        {
            Vector3 direction = transform.position - lastPosition;
            //Ray ray = new Ray(lastPosition, direction);
            RaycastHit2D[] hits = Physics2D.RaycastAll(lastPosition, direction, direction.magnitude, GM.brickLayer);
            if (hits.Length > 0)
            {
                foreach (RaycastHit2D hit in hits)
                {
                    //Debug.Log(hit.collider.gameObject.name);
                    Destroy(hit.collider.gameObject);
                }
                /*Debug.Log(hit.collider.gameObject.name);
                Destroy(hit.collider.gameObject);*/
            }
            lastPosition = transform.position;
        }
    }

    private void ChompLines(List<int> lineYValues, Vector3 upOrDown)
    {
        // Pause Game until chomping done
        GM.paused = true;
     
        lineYValues.Sort();

        // If bottom player, reverse list
        if (upOrDown.y > 0)
        {
            lineYValues.Reverse();
        }

        StartCoroutine(MoveAcrossLines(lineYValues, upOrDown));
    }

    private IEnumerator MoveAcrossLines(List<int> yValues, Vector3 upOrDown)
    {
        // Set chomping to true
        chomping = true;

        // Enable sprite renderer
        spriteRenderer.enabled = true;

        // Move to yValue (on the left side of the row)
        transform.position = new Vector3(-2, yValues[0], 0);

        // Runs through yValues from closest to center to furthest out
        for (int i = 0; i < yValues.Count; i++)
        {
            // Move to yValue on left/right edge
            transform.position = new Vector3(transform.position.x, yValues[i], 0);

            // Flip facing direction
            TurnAround();

            // Alternate left to right
            if (i % 2 == 0) // if i is even
            {
                // Move across until at other side
                while (transform.position.x < GM.backgroundWidth + 2)
                {
                    rb.MovePosition(transform.position + 
                        speed * Time.deltaTime * transform.localScale.x * Vector3.left);

                    yield return null;
                }
            }
            else // if i is odd
            {
                while (transform.position.x > -2)
                {
                    rb.MovePosition(transform.position +
                        speed * Time.deltaTime * transform.localScale.x * Vector3.left);

                    yield return null;
                }
            }

            // is this one necessary? Only makes a yValues.count frame difference I think.
            yield return null;
        }

        // Disable sprite renderer
        spriteRenderer.enabled = false;

        // Face sprite to the left
        transform.localScale = Vector3.one;

        // Move chomp out of the way so collider doesn't interfere with gameplay
        transform.position += Vector3.right * 50;

        // Set chomping to false
        chomping = false;

        // Run ShiftLines
        ShiftLines(yValues, upOrDown);
    }

    private void ShiftLines(List<int> yValues, Vector3 upOrDown)
    {
        // Sort list by int value (least to highest)
        yValues.Sort();

        // If top player, reverse list
        if (upOrDown.y < 0)
        {
            yValues.Reverse();
        }

        // Runs through for loop from furthest y value to closest to center
        for (int i = 0; i < yValues.Count; i++)
        {
            // for each x,y that is further out from center than i,
            // if there's a brick at x,y, move it one unit toward center.
            for (int y = yValues[i] - Mathf.RoundToInt(upOrDown.y);
                Mathf.Abs(y) <= GM.halfHeight;
                y -= Mathf.RoundToInt(upOrDown.y))
            {
                for (int x = 0; x < GM.backgroundWidth; x++)
                {
                    if (Physics2D.OverlapBox(new Vector2(x, y), Vector2.one * 0.1f, 0f) != null)
                    {
                        GameObject brick = Physics2D.OverlapBox(
                            new Vector2(x, y), Vector2.one * 0.1f, 0f).gameObject;

                        brick.transform.position += upOrDown; 
                    }
                }
            }
        }

        int treatsToAdd = TriangleNumber(yValues.Count);

        // Top Player
        if (upOrDown.y < 0)
        {
            GM.topLines += yValues.Count;
            GM.topTreats += treatsToAdd;
            topBlockController.CreateNewPiece();
        }
        // Bottom Player
        else 
        {
            GM.bottomLines += yValues.Count;
            GM.bottomTreats += treatsToAdd;
            bottomBlockController.CreateNewPiece();
        }

        onScoreChanged?.Invoke();

        // Unpause game
        GM.paused = false;
    }

    private void TurnAround()
    {
        transform.localScale = new Vector3(
            -transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private int TriangleNumber(int n)
    {
        return (n * (n + 1)) / 2;
    }
}