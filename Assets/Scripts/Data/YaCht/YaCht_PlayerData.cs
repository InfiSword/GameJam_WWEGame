using System.Collections.Generic;
using UnityEngine;

public class YaCht_PlayerData
{
    // 플레이어 정보
    public float maxHealth = 100;
    public float currentHealth = 100;
    
    // 플레이 보유한 카드 덱
    public List<YaCht_CardData> playerDeck = new List<YaCht_CardData>();
    public const int MAX_DECK_SIZE = 20;
    // 생성자
    public YaCht_PlayerData()
    {
        currentHealth = maxHealth;
        InitializeDefaultDeck();
    }
    
    // 기본 덱 초기화 (8장)
    private void InitializeDefaultDeck()
    {
        playerDeck.Clear();
        playerDeck.Add(YaCht_CardDatabase.Chop);
        playerDeck.Add(YaCht_CardDatabase.Jab);
        playerDeck.Add(YaCht_CardDatabase.RearNakedChoke);
        playerDeck.Add(YaCht_CardDatabase.Headbutt);
        playerDeck.Add(YaCht_CardDatabase.HeartPunch);
        playerDeck.Add(YaCht_CardDatabase.Superkick);
        playerDeck.Add(YaCht_CardDatabase.RKO);
        playerDeck.Add(YaCht_CardDatabase.LowKick);
    }
}
