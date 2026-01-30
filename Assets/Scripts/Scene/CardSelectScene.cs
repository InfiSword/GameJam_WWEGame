using UnityEngine;
using UnityEngine.UI;

public class CardSelectScene : MonoBehaviour
{
    [SerializeField] private Button m_startButton;
    void Start()
    {
        m_startButton.onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
