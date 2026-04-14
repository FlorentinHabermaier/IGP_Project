using UnityEngine;

public class Mastermind : MonoBehaviour
{
    public static Mastermind instance;
   

    private void Awake()
    {
        instance = this;
    }
    public void GameOver()
    {
        Debug.Log("Game Over");
    }
    private void Victory()
    {
        Debug.Log("Victory");
    }
}
