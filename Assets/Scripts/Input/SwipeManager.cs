using System;
using UnityEngine;
using UnityEngine.Events;

namespace rqgames.InputManagement
{
    [System.Serializable]
    public class ScreenPositionEvent : UnityEvent<Vector2>
    {
        public Vector2 ScreenPosition;
    }


    public class SwipeManager : MonoBehaviour
    {
        public float swipeThreshold = 25f;
        public float timeThreshold = 0.3f;

        public readonly UnityEvent OnSwipeLeft = new UnityEvent();
        public readonly UnityEvent OnSwipeRight = new UnityEvent();
        public readonly UnityEvent OnSwipeUp = new UnityEvent();
        public readonly UnityEvent OnSwipeDown = new UnityEvent();
        public readonly UnityEvent<Vector2> OnStationaryPressDownUp = new ScreenPositionEvent();

        private Vector2 fingerDown;
        private DateTime fingerDownTime;
        private Vector2 fingerUp;
        private DateTime fingerUpTime;
        private bool _initStationaryFingerDown;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.fingerDown = Input.mousePosition;
                this.fingerUp = Input.mousePosition;
                this.fingerDownTime = DateTime.Now;
                _initStationaryFingerDown = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                this.fingerDown = Input.mousePosition;
                this.fingerUpTime = DateTime.Now;
                this.CheckSwipe();
                if (_initStationaryFingerDown)
                    OnStationaryPressDownUp.Invoke(Input.mousePosition);
            }


            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    this.fingerDown = touch.position;
                    this.fingerUp = touch.position;
                    this.fingerDownTime = DateTime.Now;
                    _initStationaryFingerDown = true;
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    this.fingerDown = touch.position;
                    this.fingerUpTime = DateTime.Now;
                    this.CheckSwipe();
                    if (_initStationaryFingerDown)
                        OnStationaryPressDownUp.Invoke(touch.position);
                }
            }

            if (Input.GetKey(KeyCode.LeftArrow))
                this.OnSwipeLeft.Invoke();

            if (Input.GetKey(KeyCode.RightArrow))
                this.OnSwipeRight.Invoke();
        }

        private void CheckSwipe()
        {
            float duration = (float)this.fingerUpTime.Subtract(this.fingerDownTime).TotalSeconds;
            if (duration > this.timeThreshold) return;

            float deltaX = this.fingerDown.x - this.fingerUp.x;
            if (Mathf.Abs(deltaX) > this.swipeThreshold)
            {
                if (deltaX > 0)
                {
                    this.OnSwipeRight.Invoke();
                    _initStationaryFingerDown = false;
                }
                else if (deltaX < 0)
                {
                    this.OnSwipeLeft.Invoke();
                    _initStationaryFingerDown = false;
                }
            }

            float deltaY = fingerDown.y - fingerUp.y;
            if (Mathf.Abs(deltaY) > this.swipeThreshold)
            {
                if (deltaY > 0)
                {
                    this.OnSwipeUp.Invoke();
                    _initStationaryFingerDown = false;
                }
                else if (deltaY < 0)
                {
                    this.OnSwipeDown.Invoke();
                    _initStationaryFingerDown = false;
                }
            }

            this.fingerUp = this.fingerDown;
        }
    }

}