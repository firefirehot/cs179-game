using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    public float threshold;
    public float closeDistance = Mathf.Infinity;
    private Rigidbody2D rb;
    my_hp_script hp_scriptObject;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        hp_scriptObject = GameObject.FindGameObjectWithTag("HP Bar").GetComponent<my_hp_script>();

    }

    Vector3 FindClosestRespawn()
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("RespawnSpot");

        Vector3 closest = new Vector3(Mathf.Infinity, Mathf.Infinity, transform.position.z);
        closeDistance = Mathf.Infinity;

        for (int i = 0; i < taggedObjects.Length; i++)
        {
            float currDistance = Vector3.Distance(transform.position, taggedObjects[i].transform.position);
            if (currDistance <= closeDistance)
            {
                closest = new Vector3(taggedObjects[i].transform.position.x, taggedObjects[i].transform.position.y, transform.position.z);
                closeDistance = currDistance;
            }
        }
        return closest;
    }

    void FixedUpdate()
    {
        if (transform.position.y < threshold)
        {
            transform.position = FindClosestRespawn();
            hp_scriptObject.halfHeartDamage();
            rb.velocity = new Vector2(0, 0);
        }
    }
}
