using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Item", menuName = "SE/InvItem")]

public class Inv_itemSO : ScriptableObject
{
    public enum itemType { Material, Weapon, Tool, Consumable, Booster }

    [Header("Generals")]
    public itemType itType;
    public string itName = "New";
    public string itDesc = "Description";
    [Space]
    public Sprite itIcon;
    public GameObject itPrefab;
    [Space]
    public bool isStackable;
    public int maxStack = 1;

    [Header("Consumables")]
    public float plusHB = 10.0f;
    public float plusHP = 10.0f;
}