using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform tower;
    [SerializeField] private float moveSpeed = 3f;

    private void Update()
    {
        if (tower == null) return;

        Vector3 direction = (tower.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}