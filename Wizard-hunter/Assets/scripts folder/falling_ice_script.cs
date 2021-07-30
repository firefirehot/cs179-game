using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class falling_ice_script : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private LayerMask thickPlatformLayerMask;
    private BoxCollider2D boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D collidedPlatform = Physics2D.OverlapBox(boxCollider.bounds.center, boxCollider.bounds.size, 0f, thickPlatformLayerMask);
        if (collidedPlatform != null)
            Destroy(gameObject,0.1f);
    }
}
