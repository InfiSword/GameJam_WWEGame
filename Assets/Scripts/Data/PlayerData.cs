using UnityEngine;

public class PlayerData
{
    // 플레이어 스탯
    public float maxHealth = 100;
    public float currentHealth = 100;
    
    // 생성자
    public PlayerData()
    {
        currentHealth = maxHealth;
    }
}
