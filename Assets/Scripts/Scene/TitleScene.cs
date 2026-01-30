using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    public Button m_startButton;
  
    void Start()
    {
        m_startButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("CardSelectScene");
        });
    }

    void Update()
    {
        
    }
    
}
