using UnityEngine;
using TMPro;

public class Tower : MonoBehaviour
{
    [SerializeField] private float TowerHP = 100;
    [SerializeField] private TextMeshProUGUI HPtext;
    void Start()
    {
        HPtext.text = TowerHP.ToString();
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
        else if (other.CompareTag("Enemy3"))
        {
            TakeDMG(100);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Enemy4"))
        {
            TakeDMG(100);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Enemy5"))
        {
            TakeDMG(100);
            Destroy(other.gameObject);
        }
    }
    public void TakeDMG(float DMG)
    {
        TowerHP -= DMG;
        HPtext.text = TowerHP.ToString();
        Debug.Log(TowerHP);
        if(TowerHP <= 0)
        {
            Mastermind.instance.GameOver();
        }
    }
}
