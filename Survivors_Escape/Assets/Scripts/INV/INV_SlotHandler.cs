using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class INV_SlotHandler : MonoBehaviour
{
    public bool isSelect;
    // Start is called before the first frame update
    public Slot selected;
    public Slot mainslot;

    public INV_ScreenManager inv;

    void Start()
    {
        inv = GetComponentInChildren<INV_ScreenManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if(inv.opened && selected != null)
        {

        }
    }
}
