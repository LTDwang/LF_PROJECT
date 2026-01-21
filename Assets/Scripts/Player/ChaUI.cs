using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EquipmentKind
{
    Short,
    Long
}
public class ChaUI : MonoBehaviour
{
    [Header("武器UI")]
    public Image shortWeapon;
    public Image longWeapon;

    [Header("血条UI")]
    public GameObject HPprefab;
    public GameObject HProot;
    private List<GameObject> HPs = new List<GameObject>();


    [Header("体力条UI")]
    public Image energyUI;
    public Image energyTransparent;
    public float changeRate = 1.5f;
    public float forwardRate = 5f;//调下手感
    public float maxEnergy;
    public float nowEnergy;
    private float targetEnergy;

    // Start is called before the first frame update
    void Start()
    {
 
       
    }

    // Update is called once per frame
    void Update()
    {
        SetEnergyUI();
    }

    public void SetWeaponUI(EquipmentKind equipment, ItemSO weapon)
    {
        if (equipment == EquipmentKind.Short)
        {
            shortWeapon.sprite = weapon.icon;
            return;
        }
        else
        {
            longWeapon.sprite = weapon.icon;
            return;
        }
    }
    public void SetHPUI(int hp)
    {
        foreach (var item in HPs)
        {
            Destroy(item);
        }
        HPs.Clear();
        for (int i = 0; i < hp; i++)
        {
            GameObject item = Instantiate(HPprefab, HProot.transform);
            HPs.Add(item);
        }
    }
    public void SetTargetEnergy(float energy)
    {
        targetEnergy = energy;
        if (nowEnergy>=targetEnergy)
        {
            energyUI.fillAmount = targetEnergy / maxEnergy;

        }
        else
        {
            energyTransparent.fillAmount = Mathf.Min(targetEnergy+forwardRate) / maxEnergy;
        }
    }
    private void SetEnergyUI()
    {
        if (targetEnergy!=nowEnergy)
        {

            if (targetEnergy>nowEnergy)
            {
                nowEnergy += changeRate * Time.deltaTime;
                energyUI.fillAmount = nowEnergy / maxEnergy;
            }
            else
            {
                nowEnergy -= changeRate * Time.deltaTime;
                energyTransparent.fillAmount = nowEnergy / maxEnergy;
            }
        }
        if (Mathf.Abs(targetEnergy-nowEnergy)<0.001f)
        {
            nowEnergy = targetEnergy;
        }
    }
}
