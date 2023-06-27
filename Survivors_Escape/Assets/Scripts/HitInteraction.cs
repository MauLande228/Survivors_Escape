using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitInteraction
{
    public int Damage;
    public Vector3 HitPoint;
    public Vector3 HitNormal;
    public IHurtBox HurtBox;
    public IHitDetector HitDetector;

    public bool Validate()
    {
        if(HurtBox != null)
        {
            if(HurtBox.CheckHit(this))
            {
                if(HurtBox.HurtResponder == null || HurtBox.HurtResponder.CheckHit(this))
                {
                    if(HitDetector.HitResponder==null || HitDetector.HitResponder.CheckHit(this))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}


public interface IHitResponder
{
    int Damage { get; }

    public bool CheckHit(HitInteraction data);

    public void Response(HitInteraction data);
}

public interface IHitDetector
{
    public IHitResponder HitResponder { get; set; }

    public void CheckHit();
}

public interface IHurtResponder
{
    public bool CheckHit(HitInteraction data);

    public void Response(HitInteraction data);
}

public interface IHurtBox
{
    public bool Active { get; }

    public GameObject Owner { get; }

    public Transform Transform { get; }

    public IHurtResponder HurtResponder { get; set; }

    public bool CheckHit(HitInteraction hitData);
}

public interface ITargetable
{
    public bool Targetable { get; }

    public Transform TargetTransform { get; }
}
