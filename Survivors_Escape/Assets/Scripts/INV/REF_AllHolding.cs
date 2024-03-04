using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class REF_AllHolding : MonoBehaviour
{
    public GameObject s_axe; // 1
    public GameObject e_axe; // 2
    public GameObject r_axe; // 3
    public GameObject d_axe; // 4
    public GameObject s_pck; // 5
    public GameObject e_pck; // 6
    public GameObject r_pck; // 7
    public GameObject d_pck; // 8

    public INV_ScreenManager inv;

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(GetAllItems), 5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetAllItems()
    {
        inv = GetComponentInChildren<INV_ScreenManager>();
        inv.SetAllItems(s_axe, e_axe, r_axe, d_axe, s_pck, e_pck, r_pck, d_pck);
    }
}
