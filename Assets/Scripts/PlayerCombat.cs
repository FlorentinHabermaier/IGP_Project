using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;

    private readonly List<Enemy> enemiesInRange = new();
    private float attackTimer;
    private bool isAttackInProgress;

    private void Awake()
    {
        if (playerMovement == null)
        {
            playerMovement = GetComponentInParent<PlayerMovement>();
        }
    }

    private void Update()
    {
        if (Mastermind.instance == null)
        {
            return;
        }

        CleanupEnemies();

        if (enemiesInRange.Count == 0)
        {
            attackTimer = 0f;
            return;
        }

        attackTimer -= Time.deltaTime;

        if (!isAttackInProgress && attackTimer <= 0f)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        float attackSpeed = Mathf.Max(0.01f, Mastermind.instance.getHitSpeed());
        attackTimer = GetAttackInterval();
        isAttackInProgress = true;

        float attackDelay = 0f;
        if (playerMovement != null)
        {
            attackDelay = playerMovement.Attack(attackSpeed);
        }

        StartCoroutine(DealDamageAfterDelay(attackDelay));
    }

    private IEnumerator DealDamageAfterDelay(float delay)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        DealDamageToEnemiesInRange();
        isAttackInProgress = false;
    }

    private void DealDamageToEnemiesInRange()
    {
        if (Mastermind.instance == null)
        {
            return;
        }

        float damage = Mastermind.instance.getHitDmg();

        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            Enemy enemy = enemiesInRange[i];
            if (enemy == null)
            {
                enemiesInRange.RemoveAt(i);
                continue;
            }

            enemy.TakeDamage(damage);
        }
    }

    private float GetAttackInterval()
    {
        float attackSpeed = Mathf.Max(0.01f, Mastermind.instance.getHitSpeed());
        return 1f / attackSpeed;
    }

    private void CleanupEnemies()
    {
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            if (enemiesInRange[i] == null)
            {
                enemiesInRange.RemoveAt(i);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null || enemiesInRange.Contains(enemy))
        {
            return;
        }

        enemiesInRange.Add(enemy);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null)
        {
            return;
        }

        enemiesInRange.Remove(enemy);
    }
}
