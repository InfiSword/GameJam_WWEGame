using UnityEngine;

public class YaCht_GameScene : MonoBehaviour
{
    [SerializeField] private YaCht_WWEMainGame wwe;
    void Start()
    {
        YaCht_GameManager.CardManager.StartGame();
        wwe.Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
