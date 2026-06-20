using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField] private Tower tower;

    //unterschied zwischen Types
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField]private float hp = 10;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private int goldReward = 1;
    private bool isDead;
    private bool isBoss;
    private int bossNumber;
    private int bossEnemyIndex;
    private bool bossHealthApplied;

    private void Awake()
    {
        tower = FindFirstObjectByType<Tower>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        
        if (tower == null)
        {
            Debug.Log("no tower");
             return;
        }

        Vector3 direction = (tower.transform.position - transform.position).normalized;

        // Sprite nach links spiegeln wenn der Turm links ist
        spriteRenderer.flipX = direction.x < 0;

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }

        hp -= damage;
        if (hp <= 0)
        {
            isDead = true;
            if (goldReward > 0)
            {
                GoldPopup.Spawn(transform.position, goldReward, spriteRenderer.sortingLayerID, spriteRenderer.sortingOrder);
            }
            if (isBoss)
            {
                Mastermind.instance.BossDefeated(bossEnemyIndex);
            }
            Mastermind.instance.GoldGained(goldReward);
            if (Mastermind.instance.getLifesteal())
            {
                Mastermind.instance.healing();
            }
            Destroy(gameObject);
        }
    }

    public void SetBoss(int number, int enemyIndex, float healthMultiplier)
    {
        isBoss = true;
        bossNumber = number;
        bossEnemyIndex = enemyIndex;

        if (!bossHealthApplied)
        {
            hp *= Mathf.Max(1f, healthMultiplier);
            bossHealthApplied = true;
        }
    }
}
