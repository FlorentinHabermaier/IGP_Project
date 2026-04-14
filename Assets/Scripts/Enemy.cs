using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField] private Transform tower;

    //unterschied zwischen Types
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField]private float hp = 10;

    private bool gettingAttacked;
    private float timer = 0f;

    //muss man mit getter vom player holen
    [SerializeField]private float hitInterval;
    [SerializeField]private float damagePerHit;

    private void Update()
    {
        if (tower == null)
        {
            Debug.Log("no tower");
             return;
        }
        hitInterval = 1 / Mastermind.instance.getHitSpeed();
        damagePerHit = Mastermind.instance.getHitDmg();

        Vector3 direction = (tower.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (!gettingAttacked)
        {
            timer = 0f;
            return;
        }
        timer += Time.deltaTime;

        if (timer >= hitInterval)
        {
            hp -= damagePerHit;
            timer -= hitInterval;

            if (hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player"))
        {
            gettingAttacked = true;
            Debug.Log("Entered by rsaus" + other);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gettingAttacked = false;
        }
    }
}
