using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class icewall_script : MonoBehaviour
{

    private SpriteRenderer m_SpriteRenderer;
    private Color trapColor;
    private BoxCollider2D boxCollider;
    void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        trapColor = m_SpriteRenderer.GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(trapColor.r, trapColor.g, trapColor.b, 0);//makes trap invisible at the staart
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.layer == 9)
        {
            GetComponent<SpriteRenderer>().color = new Color(trapColor.r, trapColor.g, trapColor.b, trapColor.r);//makes trap visible.
            boxCollider.isTrigger = false;
            gameObject.layer = 11;//changes laayer to ground layer
        }
    }
}
