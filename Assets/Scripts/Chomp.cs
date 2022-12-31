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
/*        lineYValues.Sort((x, y) => x.CompareTo(y));

        foreach (int yValue in lineYValues)
        {

        }*/
    }

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