using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitResponder : MonoBehaviour, IHitResponder
{
    [SerializeField] private bool _attack;
    [SerializeField] private int _damage = 10;
    [SerializeField] private SEHitBox _hitBox;

    private void Start()
    {
        _hitBox.HitResponder = this;
    }

    private void Update()
    {
        if(_attack)
        {
            _hitBox.CheckHit();
        }
    }

    int IHitResponder.Damage { get => _damage; }

    bool IHitResponder.CheckHit(HitInteraction data)
    {
        return true;
    }

    void IHitResponder.Response(HitInteraction data)
    {

    }
}
