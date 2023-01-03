using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Chomp : MonoBehaviour
{
    /*	[SerializeField]
        private GameObject chomper;*/
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        BlockController.onChomp += ChompLines;
    }

    private void OnDisable()
    {
        BlockController.onChomp -= ChompLines;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TurnAround();
        }

        if (Input.GetKey(KeyCode.Y))
        {
            rb.MovePosition(transform.position + Vector3.left * transform.localScale.x * Time.deltaTime * 50);
            //transform.position += Vector3.left * transform.localScale.x * Time.deltaTime * 5;
        }
    }

    private void ChompLines(List<int> lineYValues)
    {
        // Pause Game until chomping done


       // bool goingLeft = false;

        lineYValues.Sort((x, y) => x.CompareTo(y));

        StartCoroutine(MoveAcrossLines(lineYValues));

/*        for (int i = 0; i < lineYValues.Count; i++)
        {
            // move (lerp) across row at lineYValues[i] using rb.MovePosition (so collisions work)
            if (goingLeft)
            {

            }

            // Move up if not last row

            // turn around if not last row
        }*/
    }

    private IEnumerator MoveAcrossLines(List<int> yValues)
    {
        yield return null;

        // Move to left side of first line

        // Move across line until at other side

        //if (notLastLine)
        //{
            // Move up to next yValue

            // Turn around
        //}
        //else
        //{
            // Hide chomp?

            // Call MoveBricksTowardCenter(yValue, number of lines)
                // not fully sure on parameters, list of yValues instead maybe?
    }

/*    private IEnumerator MoveAcrossLine(int YValue, bool goingLeft, bool isLastLine)
    {
        if (goingLeft)
        {

        }
    }*/

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);

        Destroy(collision.gameObject);
    }

    private void TurnAround()
    {
        transform.localScale = new Vector3(
            -transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}