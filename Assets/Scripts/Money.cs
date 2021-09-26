using UnityEngine;

public class Money : MonoBehaviour
{
    private float speed = 100f;

    void Start()
    {
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;
    }
}
