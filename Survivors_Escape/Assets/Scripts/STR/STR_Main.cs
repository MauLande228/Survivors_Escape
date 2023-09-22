using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STR_Main : MonoBehaviour
{
    public STR_Slot[] sslots;
    public STR_Slot sslotPrefab;
    public int chestSize = 14;
    public bool opened;

    // Start is called before the first frame update
    void Start()
    {
        List<STR_Slot> slotList = new List<STR_Slot>();

        for(int i = 0; i < chestSize; i++)
        {
            STR_Slot slot = Instantiate(sslotPrefab, transform).GetComponent<STR_Slot>();
            slotList.Add(slot);
        }
        sslots = slotList.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open(STR_UI ui)
    {
        ui.Open(this);

        opened = true;
    }

    public void Close(S_Slot[] uiSlots)
    {
        for (int i = 0; i < sslots.Length; i++)
        {
            if (uiSlots[i] == null)
            {
                sslots[i].itemdata = null;
            }
            else
            {
                sslots[i].itemdata = uiSlots[i].data;
            }

            sslots[i].stack = uiSlots[i].stackSize;
        }

        opened = false;
    }
}
