using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class INV_ScreenManager : MonoBehaviour
{
    public bool opened;
    public KeyCode invKey = KeyCode.Tab;

    [Header("Settings")]
    public int invSize = 14;
    public int selectedSlot = 0;

    [Header("Refs")]
    public GameObject dropModel;
    public Transform dropPos;

    public GameObject slotTemp;
    public Transform contentHolder;

    private Slot[] invSlots;
    [SerializeField] private Slot[] allSlots;

    // Start is called before the first frame update
    void Start()
    {
        GenSlots();
        ChangeSelected(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(invKey)) { opened = !opened; }

        if (opened)
        {
            transform.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            transform.localPosition = new Vector3(-10000, 0, 0);
        }
    }

    void ChangeSelected(int newSlotPos)
    {
        if (newSlotPos >= 0)
        {
            allSlots[selectedSlot].UnselectS();
        }
        allSlots[newSlotPos].SelectS();
        selectedSlot = newSlotPos;
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

    public void Check()
    {
        Debug.Log("recognized inventory manager");
    }

    public void AddItem(INV_PickUp pickUp)
    {
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
                            invSlots[i].AddItemToSlot(pickUp.data, amountLeft);
                            invSlots[i].UpdateSlot();

                            break;
                        }
                    }

                    Destroy(pickUp.gameObject);
                }
                // IF IT CAN FIT THE PICKED UP AMOUNT
                else
                {
                    stackableSlot.AddStackAmount(pickUp.stackSize);

                    Destroy(pickUp.gameObject);
                }

                stackableSlot.UpdateSlot();
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
                    emptySlot.AddItemToSlot(pickUp.data, pickUp.stackSize);
                    emptySlot.UpdateSlot();

                    Destroy(pickUp.gameObject);
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
                    break;
                }
            }

            // IF WE HAVE AN EMPTY SLOT THAN ADD THE ITEM
            if (emptySlot != null)
            {
                emptySlot.AddItemToSlot(pickUp.data, pickUp.stackSize);
                emptySlot.UpdateSlot();

                Destroy(pickUp.gameObject);
            }
            else
            {
                pickUp.transform.position = dropPos.position;
            }

        }
    }

    public void DropItem(Slot slot)
    {
        GameObject itDropModel = slot.data.itPrefab;
        INV_PickUp pickup = Instantiate(itDropModel, dropPos).AddComponent<INV_PickUp>();

        pickup.data = slot.data;
        pickup.stackSize = slot.stackSize;

        slot.Clean();
    }
}
