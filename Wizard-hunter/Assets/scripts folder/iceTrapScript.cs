using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class iceTrapScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private float detectionLength;
    [SerializeField] private float detectionWidth;
    [SerializeField] private Sprite triggeredImage;
    [SerializeField] private float spawnY;

    [SerializeField] private GameObject fallingIceObject;

    private SpriteRenderer myImage;
    private BoxCollider2D boxCollider;
    private SpriteRenderer m_SpriteRenderer;
    //private Sprite triggeredImage;
    private bool notBroken = true;

    void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer> ();
        myImage = m_SpriteRenderer.GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        //triggeredImage = Resources.Load<Sprite>("ice_trap_images/triggeredIceTrap");
        
    }
   
    // Update is called once per frame
    void Update()
    {
        if (notBroken)
        {
            RaycastHit2D raycastHitTurnOn1 = Physics2D.Raycast(new Vector2(transform.position.x - detectionWidth, transform.position.y),
                Vector2.down, detectionLength, playerLayerMask);
            RaycastHit2D raycastHitTurnOn2 = Physics2D.Raycast(new Vector2(transform.position.x + detectionWidth, transform.position.y),
                Vector2.down, detectionLength, playerLayerMask);

            if (raycastHitTurnOn1.collider != null || raycastHitTurnOn2.collider != null)
            {
                Instantiate(fallingIceObject, new Vector3(transform.position.x, transform.position.y + spawnY, transform.position.z), Quaternion.Euler(0f, 0f, 0f));
                myImage.sprite = triggeredImage;
                notBroken = false;

            }
        }

    }
}
