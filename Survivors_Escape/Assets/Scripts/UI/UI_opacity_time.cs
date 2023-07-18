using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

public class UI_opacity_time : MonoBehaviour
{
    public Image hpImg;
    public Image hbImg;
    public Image brImg;

    public bool inLose = false;
    public bool countLose = true;

    public bool inAdd = false;
    public bool countAdd = true;

    // Update is called once per frame
    void Update()
    {
        if (countLose) { Invoke("StartOp", 3); countLose = false; }
        if (inLose)
        {
            
            if (LoseOp() <= 0)
            {
                inLose = false;
            }
        }
        if (inAdd)
        {
            if (AddOp() >= 1)
            {
                inAdd = false;
            }
        }
    }

    public void Danger()
    {
        var brc = brImg.color;
        brc.a = 1;

        var hpc = hpImg.color;
        hpc.a = 1;

        var hbc = hbImg.color;
        hbc.a = 1;

        brImg.color = brc;
        hpImg.color = hpc;
        hbImg.color = hbc;

        countLose = true;
    }

    float AddOp()
    {
        var brc = brImg.color;
        brc.a = brc.a - (0.5f * Time.deltaTime);

        var hpc = hpImg.color;
        hpc.a = hpc.a - (0.5f * Time.deltaTime);

        var hbc = hbImg.color;
        hbc.a = hbc.a - (0.5f * Time.deltaTime);

        brImg.color = brc;
        hpImg.color = hpc;
        hbImg.color = hbc;

        return brc.a;
    }


    float LoseOp()
    {
        var brc = brImg.color;
        brc.a = brc.a - (0.1f * Time.deltaTime);

        var hpc = hpImg.color;
        hpc.a = hpc.a - (0.1f * Time.deltaTime);

        var hbc = hbImg.color;
        hbc.a = hbc.a - (0.1f * Time.deltaTime);

        brImg.color = brc;
        hpImg.color = hpc;
        hbImg.color = hbc;

        return brc.a;
    }

    void StartOp()
    {
        inLose = true;
    }
}
