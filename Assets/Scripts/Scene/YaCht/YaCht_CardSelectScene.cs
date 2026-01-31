using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class YaCht_CardSelectScene : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button m_johnCenaSetButton;
    [SerializeField] private Button m_undertakerSetButton;
    [SerializeField] private Button m_testSetButton;

    [SerializeField] private TextMeshProUGUI m_setNameText;
    [SerializeField] private TextMeshProUGUI m_setDescriptionText;

    [SerializeField] private Button m_startGameButton;
    [SerializeField] private Button m_cancelButton;
    [SerializeField] private Button m_titleButton;

    private YaCht_CardSetData m_currentSelectedSet;

    void Start()
    {
        m_johnCenaSetButton.onClick.AddListener(() => SelectCardSet(YaCht_CardSetDatabase.JohnCenaSet));
        m_undertakerSetButton.onClick.AddListener(() => SelectCardSet(YaCht_CardSetDatabase.UndertakerSet));
        m_testSetButton.onClick.AddListener(() => SelectCardSet(YaCht_CardSetDatabase.TestSet));

        m_startGameButton.onClick.AddListener(() =>
        {
            if (m_currentSelectedSet != null)
                SceneManager.LoadScene("YaCht_GameScene");
            
        });

        m_cancelButton.onClick.AddListener(() =>
        {
             m_currentSelectedSet = null;
            m_startGameButton.gameObject.SetActive(false);
        });

        m_titleButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("YaCht_TitleScene");
        });

        m_startGameButton.gameObject.SetActive(false);
    }

    private void UpdateSetInfo(YaCht_CardSetData cardSet)
    {
        if (m_setNameText != null)
        {
            m_setNameText.text = cardSet.setName;
        }

        if (m_setDescriptionText != null)
        {
            m_setDescriptionText.text = cardSet.setDescription;
        }
    }

    private void SelectCardSet(YaCht_CardSetData selectedSet)
    {
        YaCht_GameManager.nowPlayerData.SetPlayerDeck(selectedSet.cards, selectedSet.wrestlerType);
        m_currentSelectedSet = selectedSet;
        UpdateSetInfo(selectedSet);
        m_startGameButton.gameObject.SetActive(true);

        Debug.Log($"선택된 카드 세트: {selectedSet.setName}, 레슬러: {selectedSet.wrestlerType}");
    }
}
