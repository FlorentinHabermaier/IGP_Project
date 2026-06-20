using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;

    private readonly List<Enemy> enemiesInRange = new();
    private float attackTimer;

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

        float attackInterval = GetAttackInterval();
        attackTimer += Time.deltaTime;

        while (attackTimer >= attackInterval)
        {
            attackTimer -= attackInterval;
            PerformAttack();
            attackInterval = GetAttackInterval();
        }
    }

    private void PerformAttack()
    {
        float damage = Mastermind.instance.getHitDmg();
        float attackSpeed = Mastermind.instance.getHitSpeed();

        if (playerMovement != null)
        {
            playerMovement.Attack(attackSpeed);
        }

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
