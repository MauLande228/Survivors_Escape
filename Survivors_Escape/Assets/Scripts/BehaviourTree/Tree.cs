using System.Collections;
using System.Collections.Generic;
using BT;
using Unity.Netcode;
using UnityEngine;

namespace  BT
{
    public abstract class Tree : NetworkBehaviour
    {
        private Node _root = null;

        protected void Start()
        {
            _root = SetupTree();
        }

        private void Update()
        {
            if (_root != null)
                _root.Evaluate();
        }

        protected abstract Node SetupTree();
    }
}
