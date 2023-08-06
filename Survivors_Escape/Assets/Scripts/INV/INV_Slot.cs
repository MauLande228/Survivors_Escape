using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Inv_itemSO data;
    public int stackSize;
    [Space]
    public Image icon;
    public TextMeshProUGUI stackText;

    public Color32 selectC = new(255, 255, 0, 255);
    public Color32 unselectC = new(255, 255, 255, 255);

    //public GameObject mainSlot;

    private bool isEmpty;
    public bool itisEmpty => isEmpty;

    // Start is called before the first frame update
    void Start()
    {
        UpdateSlot();
    }

    public void SelectS()
    {
        gameObject.GetComponent<Image>().color = selectC;
    }
    public void UnselectS()
    {
        gameObject.GetComponent<Image>().color = unselectC;
    }

    public void UpdateUse()
    {
        if (stackSize <= 0)
        {
            data = null;
            stackSize = 0;
            isEmpty = true;
            icon.gameObject.SetActive(false);
            stackText.gameObject.SetActive(false);
        }
        else
        {
            stackText.text = $"{stackSize}/{data.maxStack}";
        }
    }
    public void UpdateSlot()
    {
        if (stackSize <= 0) {
            data = null;
        }

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
            
            if (data.isStackable)
            {
                stackText.text = $"x{stackSize}";
            } else
            {
                stackText.text = $"{stackSize}/{data.maxStack}";
            }

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
