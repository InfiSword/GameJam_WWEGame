using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class YaCht_TitleScene : MonoBehaviour
{
    [SerializeField] private Button endButton;

    [Header("Blink Settings")]
    [SerializeField] private GameObject blinkObject;
    [SerializeField] private float blinkSpeed = 0.05f;
    [SerializeField] private float blinkPause = 0.5f;
    [SerializeField] private bool autoStartBlink = true;

    private Color orgColor;
    private bool isBlinking;
    private TextMeshProUGUI blinkText;

    private void Awake()
    {
        isBlinking = false;
        orgColor = new Color(1f, 1f, 1f, 1f);
        
        if (blinkObject != null)
        {
            blinkText = blinkObject.GetComponent<TextMeshProUGUI>();
            if (blinkText != null)
            {
                orgColor = blinkText.color;
            }
        }

        endButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    private void Start()
    {
        // 타이틀 씬 로드 시 모든 공격 이펙트 정리 (WWEMainGame이 있으면 호출)
        YaCht_WWEMainGame wweMainGame = FindFirstObjectByType<YaCht_WWEMainGame>();
        if (wweMainGame != null)
        {
            wweMainGame.ClearAllAttackEffects();
        }
        
        // 타이틀 BGM 재생
        YaCht_BGMManager.Instance.PlayTitleBGM();

        // 자동으로 깜빡임 시작
        if (autoStartBlink && blinkObject != null && !isBlinking)
        {
            StartCoroutine(BlinkCoroutine());
            isBlinking = true;
        }
    }

    private IEnumerator BlinkCoroutine()
    {
        if (blinkText == null) yield break;

        Color tempColor = orgColor;
        blinkText.color = tempColor;

        while (true)
        {
            while (tempColor.a > 0f)
            {
                tempColor.a -= 0.1f;
                tempColor.a = Mathf.Max(0f, tempColor.a); 
                blinkText.color = tempColor;
                yield return new WaitForSeconds(blinkSpeed);
            }

            yield return new WaitForSeconds(blinkPause);

            while (tempColor.a < 1f)
            {
                tempColor.a += 0.1f;
                tempColor.a = Mathf.Min(1f, tempColor.a); 
                blinkText.color = tempColor;
                yield return new WaitForSeconds(blinkSpeed);
            }
            yield return new WaitForSeconds(blinkPause);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("YaCht_CardSelectScene");
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
