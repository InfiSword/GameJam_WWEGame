using Unity.VisualScripting;
using UnityEngine;

public class YaCht_GameManager : MonoBehaviour
{
    #region Manager
    static YaCht_GameManager m_instance;
    public static YaCht_GameManager Instance { get { Init(); return m_instance; } }

    public YaCht_CardManager m_cardManager;
    public static YaCht_CardManager CardManager { get { return Instance.m_cardManager; } }

    #endregion

    public static YaCht_PlayerData nowPlayerData = new YaCht_PlayerData();
    static void Init()
    {
        if (m_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            GameObject cardObj = GameObject.Find("@CardManager");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<YaCht_GameManager>();

                if (cardObj == null)
                {
                    cardObj = new GameObject { name = "@CardManager" };
                    cardObj.AddComponent<YaCht_CardManager>();
                }          
            }            
            
            DontDestroyOnLoad(go);
            m_instance = go.GetComponent<YaCht_GameManager>();
            m_instance.m_cardManager = cardObj.GetComponent<YaCht_CardManager>();
            cardObj.transform.SetParent(m_instance.transform);
            m_instance.m_cardManager.Init();
        }
    }

    private void Update()
    {

    }

    public static void Clear()
    {

    }
}
