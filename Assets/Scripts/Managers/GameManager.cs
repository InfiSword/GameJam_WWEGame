using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Manager
    static GameManager m_instance;
    public static GameManager Instance { get { Init(); return m_instance; } }
  
    public CardManager m_cardManager;
    public static CardManager CardManager { get { return Instance.m_cardManager; } }

    #endregion

    public static PlayerData nowPlayerData = new PlayerData();
    static void Init()
    {
        if (m_instance == null)
        {           
            GameObject go = GameObject.Find("@Managers");          
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<GameManager>();              
            }
            DontDestroyOnLoad(go);
            m_instance = go.GetComponent<GameManager>();           
        }
    }

    public void EnterGameScene()
    {
        if(CardManager == null)
        {
            GameObject cardObj = GameObject.Find("@CardManager");
            if(cardObj == null)
            {
                cardObj = new GameObject { name = "@CardManager" };
                cardObj.AddComponent<CardManager>();
            }
            m_cardManager = cardObj.GetComponent<CardManager>();
            cardObj.transform.SetParent(this.transform);
        }
    }

    private void Update()
    {

    }

    public static void Clear()
    {

    }
}
