using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RESC_Structure : MonoBehaviour, ITargetable, IHurtResponder
{
    public RESC_DropDataSO alldrops;
    public int hp = 0;
    public int droptype = 0;
    public Transform droploc;
    public int luck = 0;
    public int bhv = 0;

    private static readonly System.Random rnd = new();

    [SerializeField] private bool _isTargetable = true;
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Rigidbody _RbTarget;

    private List<SEHurtBox> _hurtBoxes = new List<SEHurtBox>();
    bool ITargetable.Targetable { get => _isTargetable; }
    Transform ITargetable.TargetTransform { get => _targetTransform; }

    public bool CheckHit(HitInteraction data)
    {
        return true;
    }

    public void Response(HitInteraction data)
    {
        hp -= data.Damage;
        if (hp <= 0)
        {
            Destroy(gameObject);
            BreakAndDrop();
        }
        //Debug.Log(hp.ToString());
    }

    void Start()
    {
        _hurtBoxes = new List<SEHurtBox>(GetComponentsInChildren<SEHurtBox>());

        foreach (SEHurtBox hb in _hurtBoxes)
        {
            hb.HurtResponder = this;
        }
    }

    void Update()
    {
        
    }

    void BreakAndDrop()
    {
        switch(droptype)
        {
            case 0:
                NormalDrop(); break;
            case 1:
                SelectDrop(); break;
            case 2:
                SelectBaseDrop(); break;
            default:
                break;
        }
    }

    public void NormalDrop()
    {
        int len = alldrops.drops.Count();
        for (int i = 0; i < len; i++)
        {
            //Debug.Log(alldrops.drops[i].items.itName);
            int r = rnd.Next(5);
            if (r > -(luck)) { r += (luck * 2); if (r > 4) { r = 4; } }
            SpawnDrops(i, r);
            //Debug.Log(alldrops.drops[i].pDrops[r]);
        }
    }

    public void SelectDrop()
    {
        int len = alldrops.drops.Count();
        int i = rnd.Next(len);
        //Debug.Log(alldrops.drops[i].items.itName);
        int r = rnd.Next(5);
        if (r > -(luck)) { r += (luck * 2); if (r > 4) { r = 4; } }
        SpawnDrops(i, r);
        //Debug.Log(alldrops.drops[i].pDrops[r]);
    }

    public void SelectBaseDrop()
    {
        int len = alldrops.drops.Count();

        //Selecting one
        int i = rnd.Next(len - 1);
        //Debug.Log(alldrops.drops[i].items.itName);
        int r = rnd.Next(5);
        if (r > -(luck)) { r += (luck * 2); if (r > 4) { r = 4; } }
        SpawnDrops(i, r);
        //Debug.Log(alldrops.drops[i].pDrops[r]);

        //Selecting the last one
        //Debug.Log(alldrops.drops[len-1].items.itName);
        int s = rnd.Next(5);
        if (s > -(luck)) { s += (luck * 2); if (s > 4) { s = 4; } }
        SpawnDrops(len-1, s);
        //Debug.Log(alldrops.drops[len-1].pDrops[s]);
    }

    public void SpawnDrops(int i, int r)
    {
        GameObject dropm = alldrops.drops[i].items.itPrefab;
        var provloc = droploc.position;
        int s = rnd.Next(1);
        switch (s)
        {
            case 0:
                provloc.x += r / 5;
                break;
            case 1:
                provloc.y += r / 5;
                break;
        }

        INV_PickUp pickup = Instantiate(dropm, droploc).AddComponent<INV_PickUp>();
        pickup.transform.position = droploc.position;
        pickup.transform.SetParent(null);

        pickup.data = alldrops.drops[i].items;
        pickup.stackSize = alldrops.drops[i].pDrops[r];
    }
}
