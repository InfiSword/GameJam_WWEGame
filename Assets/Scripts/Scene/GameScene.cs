using UnityEngine;
using UnityEngine.EventSystems;

public class GameScene : MonoBehaviour
{    
    void Start()
    {
        // EventSystem 확인 및 생성
        EnsureEventSystem();
        
        GameManager.Instance.EnterGameScene();
        //GameManager.CardManager.Init();
        //GameManager.CardManager.StartGame();
    }

    void Update()
    {
        
    }
    
    private void EnsureEventSystem()
    {
        // EventSystem이 없으면 생성
        if (EventSystem.current == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
            Debug.Log("[GameScene] EventSystem 자동 생성 완료");
        }
        else
        {
            Debug.Log("[GameScene] EventSystem 이미 존재함");
        }
    }
}
