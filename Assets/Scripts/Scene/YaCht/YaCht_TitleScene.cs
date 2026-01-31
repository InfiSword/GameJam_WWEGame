using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class YaCht_TitleScene : MonoBehaviour
{
    public Button m_startButton;
  
    void Start()
    {
        m_startButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("YaCht_CardSelectScene");
        });
    }

    void Update()
    {
        
    }
    
}
