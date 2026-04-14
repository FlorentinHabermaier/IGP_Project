using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private float TowerHP = 100;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            return;
        }
        else if (other.CompareTag("Enemy1"))
        {
            TakeDMG(10);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Enemy2"))
        {
            TakeDMG(100);
            Destroy(other.gameObject);
        }
    }
    public void TakeDMG(float DMG)
    {
        TowerHP -= DMG;
        Debug.Log(TowerHP);
        if(TowerHP <= 0)
        {
            Mastermind.instance.GameOver();
        }
    }
}
