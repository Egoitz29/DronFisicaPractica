using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Transform target;
    private MissionManager mission;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        mission = FindObjectOfType<MissionManager>();
    }

    void Update()
    {
        if (!target) return;
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            mission.EnemyDestroyed();
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
