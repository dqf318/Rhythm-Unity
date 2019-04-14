﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.TapTapAim.Assets.TapTapAim;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.TapTapAim
{
    public class HitSlider : MonoBehaviour, IHitSlider
    {

        public int QueueID { get; set; }
        public TimeSpan PerfectHitTime { get; set; }
        public int VisibleStartOffsetMs { get; } = 400;
        public int VisibleEndOffsetMs { get; } = 50;
        public TapTapAimSetup TapTapAimSetup { get; set; }
        public ICircle BlankCircle { get; set; }
        public ISliderHitCircle InitialHitCircle { get; set; }
        public ISliderPositionRing SliderPositionRing { get; set; }
        public ISlider Slider { get; set; }
        public List<Vector2> Points { get; set; }
        public int Bounces { get; set; }
        public int Number { get; set; }
        public float Duration { get; set; }
        public float Progress { get; set; }
        public bool GoingForward { get; set; }
        public bool LookForward { get; set; }
        private bool Ready { get; set; }
        public bool fadeInTriggered { get; set; }
        private float alpha = 0;
        public int AccuracyLaybackMs { get; set; } = 100;

        public void SetUp(ISliderHitCircle initialHitCircle, ISlider slider, ISliderPositionRing sliderPositionRing, TimeSpan perfectHitTime, int bounces, ITapTapAimSetup tapTapAimSetup)
        {
            InitialHitCircle = initialHitCircle;
            ((SliderHitCircle) InitialHitCircle).ParentSlider = this;
            Slider = slider;
            SliderPositionRing = sliderPositionRing;
            PerfectHitTime = perfectHitTime;
            Bounces = bounces;
            TapTapAimSetup = TapTapAimSetup;
            Ready = true;

            TapTapAimSetup.Tracker = GameObject.Find("Tracker").GetComponent<Tracker>();
            SetAlpha(alpha);
            Visibility = new Visibility()
            {
                VisibleStartOffsetMs = 400,
                VisibleEndOffsetMs = 100
            };

            Visibility.VisibleStartStart = PerfectHitTime - TimeSpan.FromMilliseconds(VisibleStartOffsetMs);
            Visibility.VisibleEndStart = PerfectHitTime + TimeSpan.FromMilliseconds(VisibleEndOffsetMs);
            gameObject.SetActive(false);
        }

        void Update()
        {

            if (!fadeInTriggered && TapTapAimSetup.Tracker.Stopwatch.Elapsed >= Visibility.VisibleStartStart)
            {
                ((MonoBehaviour)InitialHitCircle).enabled = true;
                ((MonoBehaviour)InitialHitCircle).gameObject.SetActive(true);
                StartCoroutine(FadeIn());

            }

            if (TapTapAimSetup.Tracker.Stopwatch.Elapsed >= Visibility.VisibleEndStart && !fadeOutTriggered)
            {
                Outcome(TapTapAimSetup.Tracker.Stopwatch.Elapsed, false);
                StartCoroutine(FadeOut());
            }
            if (((MonoBehaviour)Slider).transform.position == Slider.Points[pointToFollow] && pointToFollow >= Slider.Points.Count)
            {
                SetDestination(Slider.Points[pointToFollow], 1 * ((Slider)Slider).SliderSpeed);
                pointToFollow++;
            }


        }
        float t;

        private int pointToFollow = 0;
        double timeToReachTarget;
        Vector3 startPosition;
        Vector3 target;
        public void SetDestination(Vector3 destination, double time)
        {
            t = 0;
            startPosition = transform.position;
            timeToReachTarget = time;
            target = destination;
        }
        void FixedUpdate()
        {
            //t += Time.deltaTime / (float)timeToReachTarget;
            //((MonoBehaviour)SliderPositionRing).transform.position = Vector3.Lerp(startPosition, target, t); ;
        }

        IEnumerator FadeIn()
        {
            fadeInTriggered = true;
            float elapsedTime = 0.0f;

            while (elapsedTime < 1 != fadeOutTriggered)
            {
                yield return instruction;
                elapsedTime += Time.deltaTime;
                SetAlpha(Mathf.Clamp01(elapsedTime * 4));
            }
        }
        private void Outcome(TimeSpan time, bool hit)
        {

            // gameObject.SetActive(false);
            Destroy(gameObject, 3);
        }
        private void SetAlpha(float alpha)
        {
            List<Image> children = new List<Image>();
            List<Text> textChild = new List<Text>();
            LineRenderer lRenderer = transform.GetComponent<LineRenderer>();

            foreach (Transform child in transform)
            {
                foreach (var ic in child.GetComponentsInChildren<Image>())
                {
                    children.Add(ic);
                }

                foreach (var tc in child.GetComponentsInChildren<Text>())
                {
                    textChild.Add(tc);
                }

            }


            var start = Color.white;
            start.a = alpha;
            var end = Color.white;
            end.a = alpha;
            lRenderer.startColor = start;
            lRenderer.endColor = end;
            foreach (var child in children)
            {
                var newColor = child.color;
                newColor.a = alpha;
                child.color = newColor;
            }

            foreach (var child in textChild)
            {
                var newColor = child.color;
                newColor.a = alpha;
                child.color = newColor;
            }
        }

        private YieldInstruction instruction = new YieldInstruction();

        public bool fadeOutTriggered { get; set; }
        public Visibility Visibility { get; set; }

        IEnumerator FadeOut()
        {
            fadeOutTriggered = true;
            float elapsedTime = 0.0f;

            while (elapsedTime < 1)
            {
                yield return instruction;
                elapsedTime += Time.deltaTime;
                SetAlpha(1.0f - Mathf.Clamp01(elapsedTime * 9));
            }

        }


        public void HideCircle()
        {
            List<Image> children = new List<Image>();
            List<Text> textChild = new List<Text>();
            var circle = ((MonoBehaviour)InitialHitCircle).transform;
            foreach (var ic in circle.GetComponentsInChildren<Image>())
            {
                children.Add(ic);
            }

            foreach (var tc in circle.GetComponentsInChildren<Text>())
            {
                textChild.Add(tc);
            }


            foreach (var child in children)
            {
                var newColor = child.color;
                newColor.a = 0;
                child.color = newColor;
            }

            foreach (var child in textChild)
            {
                var newColor = child.color;
                newColor.a = 0;
                child.color = newColor;
            }
        }
    }
}