using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class INV_ScreenManager : MonoBehaviour
{
    public bool opened;
    public KeyCode invKey = KeyCode.Tab;
    public KeyCode equipKey = KeyCode.Q;
    public KeyCode dropKey = KeyCode.R;

    [Header("Settings")]
    public int invSize = 7;

    public int selectedSlot = 0;
    public int currentSlot = 0;

    [Header("Refs")]
    public Transform dropPos;

    public GameObject slotTemp;
    public Transform contentHolder;

    public GameObject allButtons;
    public TextMeshProUGUI Qtext;

    public TextMeshProUGUI descSpace;

    public Slot[] invSlots;
    public Slot[] allSlots;
    [SerializeField] private SpawnableList _spawnableList;

    private bool _bCanBeDestroyed = false;

    public STR_UI strui;
    public bool strui_op = false;
    // Start is called before the first frame update
    void Start()
    {
        SurvivorsEscape.CharacterController cc = dropPos.GetComponentInParent<SurvivorsEscape.CharacterController>();

        GenSlots();
        ChangeSelected(1);
        strui = GetComponentInChildren<STR_UI>();
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
                //if (allSlots[currentSlot].itisEmpty == false)
                //{
                    if (allSlots[currentSlot].data.itEqup)
                    {
                        switch (currentSlot)
                        {
                            case 0:
                                break;
                            default:
                                SwapSlots(currentSlot);
                                break;
                        }
                    }
                    else
                    {
                        ConsumeSlot(currentSlot);
                    }
                //}
            }

            if (Input.GetKeyDown(dropKey)) { if (allSlots[currentSlot].itisEmpty == false) { allSlots[currentSlot].Drop(); } }
        }
        else
        {
            transform.localPosition = new Vector3(-10000, 0, 0);
            if (strui.op)
            {
                strui.op = false;
                strui.Close();
            }
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
            if (currentSlot < 6) { selectedSlot += 1; ChangeSelected(selectedSlot); }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentSlot > 0) { selectedSlot -= 1; ChangeSelected(selectedSlot); }
        }
    }

    public void UpdateSelected(int newSelected)
    {
        selectedSlot = newSelected;
    }

    public void ChangeSelected(int newSlotPos)
    {
        SurvivorsEscape.CharacterController cc = dropPos.GetComponentInParent<SurvivorsEscape.CharacterController>();

        if (cc != null)
        {
            if (cc.IsOwner)
            {
                if (newSlotPos > 0 && newSlotPos <= 6)
                {
                    allSlots[currentSlot].UnselectS();
                    allSlots[newSlotPos].SelectS();
                    currentSlot = newSlotPos;

                    if (allSlots[currentSlot].stackSize != 0)
                    {
                        if (allSlots[currentSlot].data.itEqup)
                        {
                            SetToButtonsA();
                        }
                        else
                        {
                            SetToButtonsB();
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
        allButtons.SetActive(true);
        Qtext.text = "Equip";
    }

    void SetToButtonsB()
    {
        allButtons.SetActive(true);
        Qtext.text = "Consume";
    }

    void SetToNoButtons()
    {
        allButtons.SetActive(false);
    }
    
    public void StoreSlot(int ss)
    {
        Inv_itemSO dt = allSlots[ss].data;
        int st = allSlots[ss].stackSize;

        //SI TIENE QUE RETORNAR SI SI SE PUDO METER
        bool s = strui.StoreItem(dt, st, ss);

        //SOLO SI SI SE PUDO A�ADIR
        if (s)
        {
            allSlots[ss].Clean();
        }
        else
        {
            allSlots[ss].UpdateSlot();
        }
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

        //allSlots[0].data.WepDmg;

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
                if (s.data.itEqup)
                {
                    SetToButtonsA();
                }
                else
                {
                    SetToButtonsB();
                }
                UpdateDesc(s.data.itName, s.data.itType.ToString(), s.data.itDesc);
            }
        }  
    }

    public void SetSlot(int ss, int st)
    {
        invSlots[ss].stackSize = st;
    }

    public bool SaveItem(Inv_itemSO dt, int st, int ss)
    {
        bool f = false;

        if (dt.isStackable) // IF ITS STACKABLE
        {
            Slot stackableSlot = null;

            // TRY FINDING STACKABLE SLOT
            for (int i = 0; i < invSlots.Length; i++)
            {
                if (!invSlots[i].itisEmpty)
                {
                    if (invSlots[i].data == dt && invSlots[i].stackSize < dt.maxStack)
                    {
                        stackableSlot = invSlots[i];
                        break;
                    }
                }
            }

            if (stackableSlot != null)
            {
                if (stackableSlot.stackSize + st > dt.maxStack) // IF IT CANNOT FIT THE PICKED UP AMOUNT
                {
                    int amountLeft = (stackableSlot.stackSize + st) - dt.maxStack;

                    // ADD IT TO THE STACKABLE SLOT
                    stackableSlot.AddItemToSlot(dt, dt.maxStack);

                    // TRY FIND A NEW EMPTY STACK
                    for (int i = 0; i < invSlots.Length; i++)
                    {
                        if (invSlots[i].itisEmpty)
                        {
                            invSlots[i].AddItemToSlot(dt, amountLeft);
                            invSlots[i].UpdateSlot();
                            f = true;
                            break;
                        }
                    }
                    if (!f)
                    {
                        stackableSlot.UpdateSlot();
                        strui.SetSlot(ss, amountLeft);
                        return false;
                    }
                }
                else // IF IT CAN FIT THE PICKED UP AMOUNT
                {
                    stackableSlot.AddStackAmount(st);
                }
                stackableSlot.UpdateSlot();
                return true;
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
                        break;
                    }
                }

                // IF WE HAVE AN EMPTY SLOT THAN ADD THE ITEM
                if (emptySlot != null)
                {
                    emptySlot.AddItemToSlot(dt, st);
                    emptySlot.UpdateSlot();
                    return true;
                }
                else
                {
                    return false;
                    //pickUp.transform.position = dropPos.position;
                }
            }
        }
        else // IF ITS NOT STACKABLE
        {
            Slot emptySlot = null;

            // FIND EMPTY SLOT
            for (int i = 0; i < invSlots.Length; i++)
            {
                if (invSlots[i].itisEmpty)
                {
                    emptySlot = invSlots[i];
                    break;
                }
            }

            // IF WE HAVE AN EMPTY SLOT THAN ADD THE ITEM
            if (emptySlot != null)
            {
                emptySlot.AddItemToSlot(dt, st);
                emptySlot.UpdateSlot();
                return true;
            }
            else
            {
                return false;
                //pickUp.transform.position = dropPos.position;
            }
        }
        Debug.Log("Salida final en INV_SM");
        return false;
    }

    public bool CreateItem(Inv_itemSO dt, int st)
    {
        bool f = false;

        if (dt.isStackable) // IF ITS STACKABLE
        {
            Slot stackableSlot = null;

            // TRY FINDING STACKABLE SLOT
            for (int i = 0; i < invSlots.Length; i++)
            {
                if (!invSlots[i].itisEmpty)
                {
                    if (invSlots[i].data == dt && invSlots[i].stackSize < dt.maxStack)
                    {
                        stackableSlot = invSlots[i];
                        break;
                    }
                }
            }

            if (stackableSlot != null)
            {
                if (stackableSlot.stackSize + st > dt.maxStack) // IF IT CANNOT FIT THE PICKED UP AMOUNT
                {
                    int amountLeft = (stackableSlot.stackSize + st) - dt.maxStack;

                    // ADD IT TO THE STACKABLE SLOT
                    stackableSlot.AddItemToSlot(dt, dt.maxStack);

                    // TRY FIND A NEW EMPTY STACK
                    for (int i = 0; i < invSlots.Length; i++)
                    {
                        if (invSlots[i].itisEmpty)
                        {
                            invSlots[i].AddItemToSlot(dt, amountLeft);
                            invSlots[i].UpdateSlot();
                            f = true;
                            break;
                        }
                    }
                    if (!f)
                    {
                        stackableSlot.UpdateSlot();
                        //strui.SetSlot(ss, amountLeft);
                        DropCraftItem(dt, amountLeft);
                        return false;
                    }
                }
                else // IF IT CAN FIT THE PICKED UP AMOUNT
                {
                    stackableSlot.AddStackAmount(st);
                }
                stackableSlot.UpdateSlot();
                return true;
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
                        break;
                    }
                }

                // IF WE HAVE AN EMPTY SLOT THAN ADD THE ITEM
                if (emptySlot != null)
                {
                    emptySlot.AddItemToSlot(dt, st);
                    emptySlot.UpdateSlot();
                    return true;
                }
                else
                {
                    return false;
                    //pickUp.transform.position = dropPos.position;
                }
            }
        }
        else // IF ITS NOT STACKABLE
        {
            Slot emptySlot = null;

            // FIND EMPTY SLOT
            for (int i = 0; i < invSlots.Length; i++)
            {
                if (invSlots[i].itisEmpty)
                {
                    emptySlot = invSlots[i];
                    break;
                }
            }

            // IF WE HAVE AN EMPTY SLOT THAN ADD THE ITEM
            if (emptySlot != null)
            {
                emptySlot.AddItemToSlot(dt, st);
                emptySlot.UpdateSlot();
                return true;
            }
            else
            {
                return false;
                //pickUp.transform.position = dropPos.position;
            }
        }
        Debug.Log("Salida final en INV_SM");
        return false;
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
                            Destroy(pickUp.gameObject);
                            break;
                        }
                        else
                        {
                            pickUp.stackSize = amountLeft;
                            pickUp.transform.position = dropPos.position;
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

    public void DropCraftItem(Inv_itemSO dt, int nl)
    {
        Debug.Log(dt.itType.ToString());
        GameObject itDropModel = dt.itPrefab;
        UpdateNoDesc();
        SetToNoButtons();
        INV_PickUp pickup = Instantiate(itDropModel, dropPos).AddComponent<INV_PickUp>();
        pickup.transform.position = dropPos.position;
        pickup.transform.SetParent(null);

        pickup.data = dt;
        pickup.stackSize = nl;
    }

    public bool CanBeDestroyed() { return _bCanBeDestroyed; }

    public Transform GetCurrentDropPos()
    {
        return dropPos;
    }
}
