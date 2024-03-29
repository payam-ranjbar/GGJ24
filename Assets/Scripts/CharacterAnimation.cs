﻿using System;
using System.Collections;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class CharacterAnimation : MonoBehaviour
{
    public FacialFeatures facialFeatures;
    public Animator animator;
    public float slapTime = 1f;
    public float pushTime = 0.5f;
    private bool _isRunning;
    private bool _slapped;
    private bool _picking;
    private bool _pushed;

    private float _pushedTimer;
    private float _slapTimer;
    public float lerpDuration;

    public Transform handTransform;
    public GameObject ghost;
    [Header("callbacks")] 
    public UnityEvent onSlap;
    public UnityEvent onPushed;
    public UnityEvent onIdle;
    
    public Vector3 handSlapPos;
    public Vector3 handSlapRot;
    public Vector3 handSlapScale;


    private Vector3 handStartPos;
    private Vector3 handStartRot;
    private Vector3 handStartScale;

    private Coroutine _growingCoroutine;
    
    
    public bool IsPicking
    {
        get => _picking;
        set => _picking = value;
    }

    private void Awake()
    {
        // RotateRandomDirection();
    }

    private void Start()
    {
        handStartPos = handTransform.position;
        handStartRot = handTransform.rotation.eulerAngles;
        handStartScale = handTransform.localScale;
        
    }


    
    public void Die(Vector3 position, Quaternion rotation)
    {
        var deadGhost = Instantiate(ghost, position, rotation);
    }
    public void Run()
    {
        if(_isRunning) return;
        facialFeatures.Blinking();
        if (ProbablityExtentions.Luck(.5f))
        {
            AudioManager.Instance.Play(AudioCommand.StartRun);
        }
        _isRunning = true;
        animator.SetBool("Running", _isRunning);

        animator.SetTrigger("Run");
    }

    public void Idle()
    {
        if(!_isRunning) return;
        //AudioManager.Instance.Play(AudioCommand.Idle);
        onIdle?.Invoke();
        facialFeatures.Blinking();
        _isRunning = false;
        animator.SetBool("Running", _isRunning);

        animator.SetTrigger("Idle");
    }

    public void Slap(bool _isPlayer)
    {
        onSlap?.Invoke();
        if(!_isPlayer)
            AudioManager.Instance.Play(AudioCommand.Slap);
        else
        {
            AudioManager.Instance.Play(AudioCommand.SlapPlayer);

        }
        facialFeatures.Angry();
        _slapped = true;
        if(_growingCoroutine == null)
            _growingCoroutine = StartCoroutine(LerpTransform(handSlapPos, handSlapRot, handSlapScale));
        animator.SetBool("Running", false);

        animator.SetTrigger("Slap");
    }

    public void Pushed(bool isPlayer)
    {
        facialFeatures.Shocked();
        if (isPlayer)
        {
            AudioManager.Instance.Play(AudioCommand.PushPlayer);

            if(ProbablityExtentions.Luck(.6f))
             CameraManager.Instance.HitEffect();
            
            if(ProbablityExtentions.Luck(.9f))
                CameraManager.Instance.ScreenShake();
        }
        else
        {
            AudioManager.Instance.Play(AudioCommand.Push);
        }
        onPushed?.Invoke();
        _pushed = true;
        animator.SetBool("Running", false);
        animator.SetTrigger("Pushed");
    }
    
    
    public void Update()
    {
        animator.SetBool("Picking", _picking);
        
        if (_slapped)
        {
            StartSlapTimer();
        }

        if (_pushed)
        {
            PushTimer();
        }
    }


    private void PushTimer()
    {
        if (_pushedTimer >= pushTime)
        {
            _pushed = false;
            _pushedTimer = 0f;
            RevertToRun();

        }

        _pushedTimer += Time.deltaTime;

    }
    private void StartSlapTimer()
    {
        if (_slapTimer >= slapTime)
        {
            _slapped = false;
            _slapTimer = 0f;
            
            StartCoroutine(LerpTransform(handStartPos, handStartRot, handStartScale));

            RevertToRun();
        } 
        _slapTimer += Time.deltaTime;

    }

    private void RevertToRun()
    {
        if (_isRunning)
        {
            _isRunning = false;
            Run();
        }
        else
        {
            _isRunning = true;
            Idle();
        }
    }

    private IEnumerator LerpTransform(Vector3 pos, Vector3 rot, Vector3 scale)
    {
        
        float lerpTimer = 0f;

        while (lerpTimer < lerpDuration)
        {
            // Increment the timer
            lerpTimer += Time.deltaTime;

            // Calculate the interpolation factor between 0 and 1 based on the timer and duration
            float t = Mathf.Clamp01(lerpTimer / lerpDuration);

            // Lerp the position between the start and end transforms
            handTransform.position = Vector3.Lerp(handTransform.position, pos, t);
            handTransform.localScale = Vector3.Lerp(handTransform.localScale, scale, t);

            // Lerp the rotation as well if needed
            handTransform.rotation = Quaternion.Slerp(handTransform.rotation, Quaternion.Euler(rot), t);

            yield return null;
        }

        // Optional: Perform any actions after the lerp is complete
        //Debug.Log("Lerp complete!");

        _growingCoroutine = null;
        // You might want to reset the lerp values or perform other actions here
    }
    #region Debuging

    //private void OnGUI()
    //{
    //    // Set up GUI layout
    //    GUI.BeginGroup(new Rect(10, 10, 200, 140));
        
    //    // Run button
    //    if (GUI.Button(new Rect(10, 10, 80, 30), "Run"))
    //    {
    //        Run();
    //    }

    //    // Idle button
    //    if (GUI.Button(new Rect(100, 10, 80, 30), "Idle"))
    //    {
    //        Idle();
    //    }

    //    // Slap button
    //    if (GUI.Button(new Rect(10, 50, 80, 30), "Slap"))
    //    {
    //        Slap();
    //    }      
    //    if (GUI.Button(new Rect(100, 50, 80, 30), "Pick"))
    //    {
    //        _picking = !_picking;

    //    }        
    //    if (GUI.Button(new Rect(10, 90, 80, 30), "Pushed"))
    //    {
    //        Pushed();
    //    }

    //    GUI.EndGroup();
    //}

  

    #endregion

    public void SetOffestCycle()
    {
        // animator.GetCurrentAnimatorClipInfo(0)[0]
    }
}