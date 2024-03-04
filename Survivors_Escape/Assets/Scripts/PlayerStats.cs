using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    public float health;
    public float maxhealth = 100.0f;
    [Space]
    public float hunger;
    public float maxhunger = 100.0f;
    [Space]
    public float defense;
    public float damage;
    public float speed;
    public int luck;

    [Header("Refs")]
    public INV_ScreenManager inv;

    [Header("Enough")]
    public float regenhealth = 0.25f;

    [Header("Idle")]
    public float idlehunger = 0.5f;

    [Header("Hungry")]
    public float hungerdmg = 2.5f;

    [Header("UI")]
    public StatsBar healthBar;
    public StatsBar hungerBar;
    public INV_CanvRef canvx;

    private float possibledmg;
    //public UI_opacity_time uicolor;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(canvx, this.gameObject.transform);

        health = maxhealth;
        hunger = maxhunger;
        defense = 1;
        damage = 1;
        speed = 1;
        luck = -1;

        inv = canvx.GetComponentInChildren<INV_ScreenManager>();
        //uicolor = GetComponentInChildren<UI_opacity_time>();
    }

    bool hunger_lock = false;
    bool life_lock = false;
    bool crazy_lock = false;
    bool regen_lock = false;

    // Update is called once per frame
    void Update()
    {
        UpdateStats();
        UpdateUI();
    }

    public void SetBars(StatsBar hpb, StatsBar hbb)
    {
        healthBar = hpb;
        hungerBar = hbb;
    }

    //Update the UI bars
    private void UpdateUI()
    {
        healthBar.numberText.text = health.ToString("f0");
        healthBar.bar.fillAmount = health / 100;

        hungerBar.numberText.text = hunger.ToString("f0");
        hungerBar.bar.fillAmount = hunger / 100;
    }

    private void UpdateStats()
    {
        // Idle HUNGER states
        if (hunger > 0)
        {
            hunger -= idlehunger * Time.deltaTime;
            if (health < 100 && !regen_lock) { health += regenhealth * Time.deltaTime; }
        }
        if (health > 0 && hunger_lock)
        {
            health -= hungerdmg * Time.deltaTime;
        }
        // Debuff and Recover HUNGER states
        if (hunger < 75 && !regen_lock) // DEBUFF of no more regen
        {
            regen_lock = true;
            healthBar.bar.color = new Color32(255, 55, 55, 255);
        }
        if (hunger > 75 && regen_lock) // RECOVER ability to regen
        {
            regen_lock = false;
            healthBar.bar.color = new Color32(255, 194, 0, 255);
        }

        if (hunger < 25 && !crazy_lock) // DEBUFF of luck and damage and defense // uicolor.CoDanger();
        {
            crazy_lock = true;

            hungerBar.bar.color = new Color32(255, 55, 55, 255);
            inv.IsHungry();
        }
        if (hunger > 50 && crazy_lock) // RECOVER the luck and damage and defense // uicolor.CoNormal();
        {
            crazy_lock = false;
            hunger_lock = false;

            hungerBar.bar.color = new Color32(255, 194, 0, 255);
            inv.IsFeeding();
        }

        // Prevent HEALTH out of limits
        if (health <= 0 && !life_lock) {
            life_lock = true;
            health = 0;
            inv.IsDeadAsHell();
        }
        if (health > maxhealth)
        {
            health = maxhealth;
        }
        // Prevent HUNGER out of limits
        if (hunger <= 0 && !hunger_lock)
        {
            hunger_lock = true;
            hunger = 0;
        }
        if (hunger > maxhunger)
        {
            hunger = maxhunger;
        }
    }

    //var hpbc = healthBar.bar.color;
    //hpbc.r = 1;
    //hpbc.g = 0.2f;
    //hpbc.b = 0.2f;
    //healthBar.bar.color = hpbc;

    private void FallDamage(float spd)
    {
        possibledmg = spd * 2.0f - 12.0f;
        // Apply fall damage if possible
        if (possibledmg > 0) { health -= possibledmg; }
    }

    private void ApplyDamage(float dmg)
    {
        possibledmg = dmg * defense;
        health -= possibledmg;
    }

    // All frutal permanent buffs
    private void FDessert() { defense -= 0.1f; }
    private void FDrink() { damage += 0.1f; }
    private void FDelight() { luck += 1; }
}
