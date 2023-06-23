using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SurvivorsEscape
{
    public enum Timing { ON_ENTER, ON_EXIT, ON_UPDATE, ON_END }

    public class SMBEvent : StateMachineBehaviour
    {
        [System.Serializable]
        public class Event
        {
            public string _name;
            public float _onUpdateFrame;
            public bool _fired;
            public Timing _timing;
        }

        [SerializeField] private int _totalFrames;
        [SerializeField] private int _currentFrame;
        [SerializeField] private float _normalizedTime;
        [SerializeField] private float _normalizedTimeUncapped;
        [SerializeField] private string _motionTime = "";

        public List<Event> _Events = new List<Event>();

        private bool _hasParam;
        private EventHandler _eventHandler;



        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _hasParam = HasParameter(animator, _motionTime);
            _eventHandler = animator.GetComponent<EventHandler>();

            _totalFrames = GetTotalFrames(animator, layerIndex);
            _normalizedTimeUncapped = stateInfo.normalizedTime;
            _normalizedTime = _hasParam ? animator.GetFloat(_motionTime) : GetNormalizedTime(stateInfo);
            _currentFrame = GetCurrentFrame(_totalFrames, _normalizedTime);

            if(_eventHandler != null)
            {
                foreach (Event e in _Events)
                {
                    e._fired = false;
                    if (e._timing == Timing.ON_ENTER)
                    {
                        e._fired = true;
                        _eventHandler.Event.Invoke(e._name);
                    }
                }
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _normalizedTimeUncapped = stateInfo.normalizedTime;
            _normalizedTime = _hasParam ? animator.GetFloat(_motionTime) : GetNormalizedTime(stateInfo);
            _currentFrame = GetCurrentFrame(_totalFrames, _normalizedTime);

            if (_eventHandler != null)
            {
                foreach (Event e in _Events)
                {
                    if(!e._fired)
                    {
                        if(e._timing == Timing.ON_UPDATE)
                        {
                            if(_currentFrame >= e._onUpdateFrame)
                            {
                                e._fired = true;
                                _eventHandler.Event.Invoke(e._name);
                            }
                        }
                        else if(e._timing == Timing.ON_END)
                        {
                            if(_currentFrame >= _totalFrames)
                            {
                                e._fired = true;
                                _eventHandler.Event.Invoke(e._name);
                            }
                        }
                    }
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_eventHandler != null)
            {
                foreach (Event e in _Events)
                {
                    if (e._timing == Timing.ON_EXIT)
                    {
                        e._fired = true;
                        _eventHandler.Event.Invoke(e._name);
                    }
                }
            }
        }

        private bool HasParameter(Animator animator, string param)
        {
            if (string.IsNullOrEmpty(param) || string.IsNullOrWhiteSpace(param))
                return false;

            foreach(var parameter in animator.parameters)
            {
                if (parameter.name == param)
                    return true;
            }

            return false;
        }

        public int GetTotalFrames(Animator animator,  int layerIndex)
        {
            AnimatorClipInfo[] clipInfos = animator.GetNextAnimatorClipInfo(layerIndex);

            if (clipInfos.Length == 0)
                clipInfos = animator.GetCurrentAnimatorClipInfo(layerIndex);

            AnimationClip animClip = clipInfos[0].clip;

            return Mathf.RoundToInt(animClip.length * animClip.frameRate);
        }

        private float GetNormalizedTime(AnimatorStateInfo stateInfo)
        {
            return stateInfo.normalizedTime > 1 ? 1 : stateInfo.normalizedTime;
        }

        private int GetCurrentFrame(int totalFrames, float normalizedTime)
        {
            return Mathf.RoundToInt(totalFrames * normalizedTime);
        }
    }
}
