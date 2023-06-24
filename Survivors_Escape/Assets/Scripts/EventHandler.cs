using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivorsEscape
{
    public class EventHandler : MonoBehaviour
    {
        [SerializeField] private bool _debug = true;
        [SerializeField] private UnityEvent<string> _event = new UnityEvent<string>();
        public UnityEvent<string> Event { get => _event; }

        private void Awake()
        {
            _event.AddListener(OnEvent);
        }

        private void OnEvent(string eventName)
        {
            if (_debug)
            {
                Debug.Log(eventName);
            }
        }
    }
}
