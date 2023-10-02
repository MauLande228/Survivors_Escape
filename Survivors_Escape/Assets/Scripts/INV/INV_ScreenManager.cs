using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class INV_ScreenManager : MonoBehaviour
{
    public bool opened;
    public KeyCode invKey = KeyCode.Tab;
    public KeyCode equipKey = KeyCode.Q;
    public KeyCode dropKey = KeyCode.R;

    [Header("Settings")]
    public int invSize = 8;

    public int selectedSlot = 1;
    public int currentSlot = 1;
    public int currentType = 0;

    [Header("Refs")]
    public Transform dropPos;

    public GameObject slotTemp;
    public Transform contentHolder;

    public Image buttonsImgA;
    public Image buttonsImgB;
    public TextMeshProUGUI descSpace;

    private Slot[] invSlots;
    [SerializeField] private Slot[] allSlots;
    [SerializeField] private SpawnableList _spawnableList;

    private bool _bCanBeDestroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        SurvivorsEscape.CharacterController cc = dropPos.GetComponentInParent<SurvivorsEscape.CharacterController>();

        if (cc != null)
        {
            if (cc.IsOwner)
            {
                GenSlots();
                ChangeSelected(1);
                buttonsImgA.enabled = false;
                buttonsImgB.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(invKey)) { opened = !opened; }

        if (opened)
        {
            transform.localPosition = new Vector3(0, 0, 0);

            if (Input.GetKeyDown(equipKey))
            {
                if (allSlots[currentSlot].itisEmpty == false)
                {
                    switch (currentType)
                    {
                        case 0:
                            SwapSlots(currentSlot);
                            break;
                        case 1:
                            ConsumeSlot(currentSlot);
                            break;
                    }
                    
                }
            }

            if (Input.GetKeyDown(dropKey)) { if (allSlots[currentSlot].itisEmpty == false) { allSlots[currentSlot].Drop(); } }
        }
        else
        {
            transform.localPosition = new Vector3(-10000, 0, 0);
        }

        //if (Input.GetKeyDown(KeyCode.UpArrow))
        //{
        //    if (currentSlot > 6) { selectedSlot -= 7; ChangeSelected(selectedSlot); }
        //}
        //if (Input.GetKeyDown(KeyCode.DownArrow))
        //{
        //    if (currentSlot < 7) { selectedSlot += 7; ChangeSelected(selectedSlot); }
        //}
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentSlot < 7) { selectedSlot += 1; ChangeSelected(selectedSlot); }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentSlot > 1) { selectedSlot -= 1; ChangeSelected(selectedSlot); }
        }


    }

    void ChangeSelected(int newSlotPos)
    {
        SurvivorsEscape.CharacterController cc = dropPos.GetComponentInParent<SurvivorsEscape.CharacterController>();

        if (cc != null)
        {
            if (cc.IsOwner)
            {
                if (newSlotPos > 0 && newSlotPos < 8)
                {
                    allSlots[currentSlot].UnselectS();
                    allSlots[newSlotPos].SelectS();
                    currentSlot = newSlotPos;

                    if (allSlots[currentSlot].stackSize != 0)
                    {
                        if (allSlots[currentSlot].data.itType.ToString() == "Consumable")
                        {
                            SetToButtonsB();
                            currentType = 1;
                        }
                        else
                        {
                            SetToButtonsA();
                            currentType = 0;
                        }
                        UpdateDesc(allSlots[currentSlot].data.itName, allSlots[currentSlot].data.itType.ToString(), allSlots[currentSlot].data.itDesc);
                        //Debug.Log(allSlots[currentSlot].data.itType.ToString());
                    }
                    else
                    {
                        UpdateNoDesc();
                        SetToNoButtons();
                    }
                }
                else
                {
                    selectedSlot = currentSlot;
                }
            }
        }
    }

    void SetToButtonsA()
    {
        buttonsImgB.enabled = false;
        buttonsImgA.enabled = true;
    }

    void SetToButtonsB()
    {
        buttonsImgA.enabled = false;
        buttonsImgB.enabled = true;
    }

    void SetToNoButtons()
    {
        buttonsImgA.enabled = false;
        buttonsImgB.enabled = false;
    }

    void SwapSlots(int cs)
    {
        Inv_itemSO dt = allSlots[cs].data;
        int ss = allSlots[cs].stackSize;

        //Debug.Log(ss);

        allSlots[cs].data = allSlots[0].data;
        allSlots[cs].stackSize = allSlots[0].stackSize;

        allSlots[0].data = dt;
        allSlots[0].stackSize = ss;

        allSlots[cs].UpdateSlot();
        allSlots[0].UpdateSlot();

        UpdateCurrentSlot(allSlots[cs]);
    }

    void ConsumeSlot(int cs)
    {
        PlayerStats stats = GetComponentInParent<PlayerStats>();

        stats.hunger += allSlots[cs].data.plusHB;
        stats.health += allSlots[cs].data.plusHP;

        allSlots[cs].stackSize--;
        if(allSlots[cs].stackSize <= 0)
        {
            SetToNoButtons();
            UpdateNoDesc();
        }
        allSlots[cs].UpdateSlot();
    }

    void UseSlot(int mp)
    {
        allSlots[0].stackSize = allSlots[0].stackSize - mp;

    }

    public void UpdateDesc(string n, string t, string d)
    {
        string dc = "";
        switch (t[0])
        {
            case 'M':
                dc = "#42E4E4";
                break;
            case 'W':
                dc = "#F22D60";
                break;
            case 'T':
                dc = "#F1C232";
                break;
            case 'C':
                dc = "#FFAA47";
                break;
            case 'B':
                dc = "#842554";
                break;
        }
        descSpace.text = "<color=" + dc + ">" + n + "</color> (" + t + "): " + d;
    }

    public void UpdateNoDesc()
    {
        descSpace.text = "No item in slot!";
    }

    private void GenSlots()
    {
        List<Slot> invSlots_ = new List<Slot>();
        List<Slot> allSlots_ = new List<Slot>();

        for (int i = 0; i < allSlots.Length; i++)
        {
            allSlots_.Add(allSlots[i]);
        }

        for (int i = 0; i < invSize; i++)
        {
            Slot slot = Instantiate(slotTemp.gameObject, contentHolder).GetComponent<Slot>();

            invSlots_.Add(slot);
            allSlots_.Add(slot);
        }

        invSlots = invSlots_.ToArray();
        allSlots = allSlots_.ToArray();
    }

    public void UpdateCurrentSlot(Slot s)
    {


        SurvivorsEscape.CharacterController cc = dropPos.GetComponentInParent<SurvivorsEscape.CharacterController>();

        if (cc != null)
        {
            if (cc.IsOwner)
            {
                if (s.data.itType.ToString() == "Consumable")
                {
                    SetToButtonsB();
                    currentType = 1;
                }
                else
                {
                    SetToButtonsA();
                    currentType = 0;
                }
                UpdateDesc(s.data.itName, s.data.itType.ToString(), s.data.itDesc);
            }
        }  
    }

    public bool AddItem(INV_PickUp pickUp, SurvivorsEscape.CharacterController cc)
    {
        Debug.Log("PICKED ITEM UP BRO");
        bool isIn = false;

        if (pickUp.data.isStackable)
        {
            Slot stackableSlot = null;

            // TRY FINDING STACKABLE SLOT
            for (int i = 0; i < invSlots.Length; i++)
            {
                if (!invSlots[i].itisEmpty)
                {
                    if (invSlots[i].data == pickUp.data && invSlots[i].stackSize < pickUp.data.maxStack)
                    {
                        stackableSlot = invSlots[i];
                        break;
                    }

                }
            }

            if (stackableSlot != null)
            {

                // IF IT CANNOT FIT THE PICKED UP AMOUNT
                if (stackableSlot.stackSize + pickUp.stackSize > pickUp.data.maxStack)
                {
                    int amountLeft = (stackableSlot.stackSize + pickUp.stackSize) - pickUp.data.maxStack;

                    // ADD IT TO THE STACKABLE SLOT
                    stackableSlot.AddItemToSlot(pickUp.data, pickUp.data.maxStack);

                    // TRY FIND A NEW EMPTY STACK
                    for (int i = 0; i < invSlots.Length; i++)
                    {
                        if (invSlots[i].itisEmpty)
                        {
                            if (i == currentSlot) { isIn = true; }
                            invSlots[i].AddItemToSlot(pickUp.data, amountLeft);
                            invSlots[i].UpdateSlot();
                            if (isIn)
                            {
                                UpdateCurrentSlot(invSlots[i]);
                            }
                            break;
                        }
                    }

                    stackableSlot.UpdateSlot();
                    //Destroy(pickUp.gameObject);
                    return true;
                }
                // IF IT CAN FIT THE PICKED UP AMOUNT
                else
                {
                    stackableSlot.AddStackAmount(pickUp.stackSize);
                    stackableSlot.UpdateSlot();
                    //Destroy(pickUp.gameObject);
                    return true;
                }
            }
            else
            {
                Slot emptySlot = null;


                // FIND EMPTY SLOT
                for (int i = 0; i < invSlots.Length; i++)
                {
                    if (invSlots[i].itisEmpty)
                    {
                        emptySlot = invSlots[i];
                        if (i == currentSlot) { isIn = true; }
                        break;
                    }
                }

                // IF WE HAVE AN EMPTY SLOT THAN ADD THE ITEM
                if (emptySlot != null)
                {
                    emptySlot.AddItemToSlot(pickUp.data, pickUp.stackSize);
                    emptySlot.UpdateSlot();
                    if (isIn)
                    {
                        UpdateCurrentSlot(emptySlot);
                    }
                    //Destroy(pickUp.gameObject);
                }
                else
                {
                    pickUp.transform.position = dropPos.position;
                }
            }

        }
        else
        {
            Slot emptySlot = null;


            // FIND EMPTY SLOT
            for (int i = 0; i < invSlots.Length; i++)
            {
                if (invSlots[i].itisEmpty)
                {
                    emptySlot = invSlots[i];
                    if (i == currentSlot) { isIn = true; }
                    break;
                }
            }

            // IF WE HAVE AN EMPTY SLOT THEN ADD THE ITEM
            if (emptySlot != null)
            {
                emptySlot.AddItemToSlot(pickUp.data, pickUp.stackSize);
                emptySlot.UpdateSlot();
                if (isIn)
                {
                    UpdateCurrentSlot(emptySlot);
                }

                cc.TakeWeapon(pickUp);
                //Vector3 newPosition = new Vector3(0.087f, 0.082f, 0.07f);
                //Vector3 newRotation = new Vector3(-162.30f, 71.77f, -29.83f);
                //
                //pickUp.gameObject.transform.localPosition = newPosition;
                //pickUp.gameObject.transform.localEulerAngles = newRotation;

                //Destroy(pickUp.gameObject);
            }
            else
            {
                pickUp.transform.position = dropPos.position;
            }

        }

        return false;
    }

    public void DropItem(Slot slot)
    {
        int i = Spawner.Instace.GetItemIndex(slot.data);
        Spawner.Instace._spawnableList._itemsList[i].dropPos = dropPos;

        Vector3 positon = dropPos.position;
        float x = positon.x;
        float y = positon.y;
        float z = positon.z;

        Spawner.Instace.SpawnObjectServerRpc(Spawner.Instace.GetItemIndex(slot.data), slot.stackSize, x, y, z);
        slot.Clean();
    }

    public bool CanBeDestroyed() { return _bCanBeDestroyed; }

    public Transform GetCurrentDropPos()
    {
        return dropPos;
    }
}
