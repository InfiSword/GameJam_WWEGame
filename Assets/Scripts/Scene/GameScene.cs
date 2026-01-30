using UnityEngine;

public class GameScene : MonoBehaviour
{    
    void Start()
    {
        GameManager.Instance.EnterGameScene();
        GameManager.CardManager.Init();
        GameManager.CardManager.StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
