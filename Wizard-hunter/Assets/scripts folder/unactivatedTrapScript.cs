using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unactivatedTrapScript : MonoBehaviour
{
    // Start is called before the first frame update

    private SpriteRenderer m_SpriteRenderer;
    private Color trapColor;
    void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        trapColor = m_SpriteRenderer.GetComponent<SpriteRenderer>().color;

    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.layer == 9) {
            GetComponent<SpriteRenderer>().color = new Color(trapColor.r, trapColor.g, trapColor.b, trapColor.r);//makes trap visible.
        }
    }
}
