using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SEHurtBox : MonoBehaviour, IHurtBox
{
    [SerializeField] private bool _active = true;
    [SerializeField] private GameObject _owner = null;

    private IHurtResponder _hurtResponder;

    public bool Active { get => _active; }

    public GameObject Owner { get => _owner; }

    public Transform Transform { get => transform; }

    public IHurtResponder HurtResponder { get => _hurtResponder; set => _hurtResponder = value; }

    public bool CheckHit(HitInteraction hitData)
    {
        if(HurtResponder == null)
        {
            Debug.Log("No responder");
        }
        return true;
    }
}
