using System.Collections.Generic;
using UnityEngine;

public class YaCht_PlayerData
{
    // 플레이어 소유한 카드 덱
    public List<YaCht_CardData> playerDeck = new List<YaCht_CardData>();
    public const int MAX_DECK_SIZE = 10;
    
    // 플레이어 선택한 레슬러 타입
    public YaCht_WrestlerType wrestlerType = YaCht_WrestlerType.None;

    public YaCht_PlayerData()
    {
    }
    
    // 카드 세트 설정하면 플레이어 덱 저장
    public void SetPlayerDeck(List<YaCht_CardData> selectedCards, YaCht_WrestlerType wrestler)
    {
        playerDeck.Clear();
        playerDeck.AddRange(selectedCards);
        wrestlerType = wrestler;
        Debug.Log($"플레이어 덱이 설정되었습니다. 총 {playerDeck.Count}장의 카드, 레슬러: {wrestler}");
    }
    
    // 레슬러 타입 가져오기
    public YaCht_WrestlerType GetWrestlerType()
    {
        return wrestlerType;
    }
}
