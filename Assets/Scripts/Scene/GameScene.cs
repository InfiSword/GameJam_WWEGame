using UnityEngine;

public class GameScene : MonoBehaviour
{
    [SerializeField] private WWEMainGame wwe;
    void Start()
    {
        GameManager.Instance.EnterGameScene();
        GameManager.CardManager.Init();
        GameManager.CardManager.StartGame();
        wwe.Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
