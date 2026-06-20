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
            Mastermind.instance.TakeDamage(5);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Enemy2"))
        {
            Mastermind.instance.TakeDamage(25);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Enemy3"))
        {
            Mastermind.instance.TakeDamage(50);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Enemy4"))
        {
            Mastermind.instance.TakeDamage(80);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Enemy5"))
        {
            Mastermind.instance.TakeDamage(200);
            Destroy(other.gameObject);
        }
    }
   
}
