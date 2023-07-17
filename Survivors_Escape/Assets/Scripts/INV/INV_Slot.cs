using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour
{
    public Inv_itemSO data;
    public int stackSize;
    [Space]
    public Image icon;
    public TextMeshProUGUI stackText;

    public Color32 selectC = new Color32(0, 255, 0, 255);
    public Color32 unselectC = new Color32(255, 255, 225, 255);

    //public GameObject mainSlot;

    private bool isEmpty;
    public bool itisEmpty => isEmpty;

    // Start is called before the first frame update
    void Start()
    {
        UpdateSlot();
    }
    public void Awake()
    {
        UnselectS();
    }

    public void SelectS()
    {
        gameObject.GetComponent<Image>().color = selectC;
    }
    public void UnselectS()
    {
        gameObject.GetComponent<Image>().color = unselectC;
    }

    public void UpdateSlot()
    {
        if (data == null)
        {
            isEmpty = true;
            icon.gameObject.SetActive(false);
            stackText.gameObject.SetActive(false);
        }
        else
        {
            isEmpty = false;

            icon.sprite = data.itIcon;
            stackText.text = $"x{stackSize}";
            
            icon.gameObject.SetActive(true);
            stackText.gameObject.SetActive(true);
        }
    }

    public void AddItemToSlot(Inv_itemSO data1, int stackSize1)
    {
        data = data1;
        stackSize = stackSize1;
    }

    public void AddStackAmount(int stackSize1)
    {
        stackSize += stackSize1;
    }

    public void Drop()
    {
        GetComponentInParent<INV_ScreenManager>().DropItem(this);
    }

    public void Clean()
    {
        data = null;
        stackSize = 0;

        UpdateSlot();
    }
}
