using UnityEngine;

public class RankeDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null)
        {
            return;
        }

        enemy.TakeDamage(Mastermind.instance.getRankenDamage());
    }
}
