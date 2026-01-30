using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    // 플레이어 스탯
    public float maxHealth = 100;
    public float currentHealth = 100;
    
    // 플레이어가 소유한 카드 덱
    public List<CardData> playerDeck = new List<CardData>();
    public const int MAX_DECK_SIZE = 20;
    // 생성자
    public PlayerData()
    {
        currentHealth = maxHealth;
        InitializeDefaultDeck();
    }
    
    // 기본 덱 초기화 (8장)
    private void InitializeDefaultDeck()
    {
        playerDeck.Clear();
        playerDeck.Add(CardDatabase.Chop);
        playerDeck.Add(CardDatabase.Jab);
        playerDeck.Add(CardDatabase.RearNakedChoke);
        playerDeck.Add(CardDatabase.Headbutt);
        playerDeck.Add(CardDatabase.HeartPunch);
        playerDeck.Add(CardDatabase.Superkick);
        playerDeck.Add(CardDatabase.RKO);
        playerDeck.Add(CardDatabase.LowKick);
    }
}
