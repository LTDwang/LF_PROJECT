using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerCha : MonoBehaviour
{
    [SerializeField]
    private PlayerConfigData playerConfig;
    [SerializeField]
    private int maxHP;
    [SerializeField]
    private float maxEnergy;
    [SerializeField]
    private int currentHP;
    [SerializeField]
    private float currentEnergy;
    [SerializeField]
    private float energyRecoverRate;
    public ChaUI chaUI;
    public void InitializeFromConfig(PlayerConfigData configData)
    {
        playerConfig = configData;
        maxHP = playerConfig.maxHP;
        maxEnergy = playerConfig.maxEnergy;
        currentHP = maxHP;
        currentEnergy = maxEnergy;
        energyRecoverRate = playerConfig.recoverRate;
        chaUI.maxEnergy = maxEnergy;
        chaUI.SetHPUI(currentHP);
        chaUI.SetTargetEnergy(currentEnergy);
        chaUI.nowEnergy = currentEnergy;
    }
    public void ChangeHP(int count)
    {
        currentHP += count;
        if (currentHP>maxHP)
        {
            currentHP = maxHP;
        }
        if (currentHP<=0)
        {
            currentHP = 0;
        }
        chaUI.SetHPUI(currentHP);
    }
    public void ChangeEnergy(float count)
    {
        currentEnergy += count;
        if (currentEnergy>maxEnergy)
        {
            currentEnergy = maxEnergy;
        }
        if (currentEnergy<=0)
        {
            currentEnergy = 0;
        }
        chaUI.SetTargetEnergy(currentEnergy);
    }

    public bool IsDead()
    {
        return currentHP <= 0;
    }    
    public bool EnergyOut()
    {
        return currentEnergy <= 0.1f;
    }
    public void EnergyRecover()
    {
        ChangeEnergy(energyRecoverRate*Time.deltaTime);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        EnergyRecover();
    }
}
