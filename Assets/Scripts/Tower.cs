    using UnityEngine;
using TMPro;

public class Tower : MonoBehaviour
{
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            return;
        }
        else if (other.CompareTag("Enemy1"))
        {
            Mastermind.instance.TakeDamage(10);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Enemy2"))
        {
            Mastermind.instance.TakeDamage(100);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Enemy3"))
        {
            Mastermind.instance.TakeDamage(100);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Enemy4"))
        {
            Mastermind.instance.TakeDamage(100);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Enemy5"))
        {
            Mastermind.instance.TakeDamage(100);
            Destroy(other.gameObject);
        }
    }
   
}
