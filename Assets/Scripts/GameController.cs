using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum PlayerGesture
{
    None,
    BothHand,
    LeftHand,
    RightHand,
    Jump,
    LeftLean,
    RightLean
}

public class GameController : MonoBehaviour
{
    // Constant
    public const int GESTURE_PER_ROUND = 4;
    public const float SECONDS_PER_GESTURE = 1;
    public const float ROUND_SECONDS_MARGIN = 2;


    private bool isRunning = false;
    private float roundStartTime = 0f;
    private int roundIndex = 0;

    [HideInInspector]
    public GestureRef[] gestureRefs = new GestureRef[GESTURE_PER_ROUND];

    public HealthBar bossHealthBar;

    [HideInInspector]
    public float bossHealth = 3000;

    [HideInInspector]
    public float bossMaxHealth = 3000;

    public Animator bossAnimator;

    public GesturePlayer[] players = new GesturePlayer[GESTURE_PER_ROUND];


    // Start

    void Start() {
        this.isRunning = true;
        StartCoroutine(Running());
        this.bossHealthBar.maxHealth = this.bossMaxHealth;
    }

    private IEnumerator Running() {
        while (this.isRunning) {
            this.RandomGestureSet();
            yield return new WaitForSeconds((SECONDS_PER_GESTURE * GESTURE_PER_ROUND) + ROUND_SECONDS_MARGIN);
        }
    }


    //Random

    private void RandomGestureSet() {
        for (int i = 0; i < GESTURE_PER_ROUND; i++) {
            var startTime = Time.time + (i * SECONDS_PER_GESTURE) + ROUND_SECONDS_MARGIN;
            this.gestureRefs[i] = new GestureRef(startTime, startTime + SECONDS_PER_GESTURE);
        }
        this.roundStartTime = Time.time + ROUND_SECONDS_MARGIN;
        this.roundIndex++;
        this.bossAnimator.SetTrigger("Attack");
    }


    // Update

    private void Update() {
        this.bossHealthBar.health = this.bossHealth;
    }


    // Game

    public void TakeDamage(int damage) {
        if (this.bossHealth - damage < 0) {
            this.bossAnimator.SetBool("Dead", true);
        } else {
            this.bossHealth -= damage;
        }
    }


    // Utils

    public int CurrentGestureIndex {
        get => Mathf.FloorToInt((Time.time - this.roundStartTime) / SECONDS_PER_GESTURE);
    }

    public GestureRef CurrentGesture {
        get => this.CurrentGestureIndex >= 0 && this.CurrentGestureIndex < GESTURE_PER_ROUND ? this.gestureRefs[this.CurrentGestureIndex] : null;
    }

    public GestureRef PreviousGesture {
        get => this.CurrentGestureIndex - 1 >= 0 ? this.gestureRefs[this.CurrentGestureIndex - 1] : null;
    }

    public float RoundStartTime {
        get => this.roundStartTime;
    }

    public int RoundIndex {
        get => roundIndex;
    }


    // Destroy

    void OnDestroy() {
        this.isRunning = false;
    }

}

public class GestureRef
{
    public PlayerGesture gesture;
    public float startTime;
    public float endTime;

    public GestureRef(float startTime, float endTime) {
        this.startTime = startTime;
        this.endTime = endTime;
        this.gesture = RandomGesture();
    }

    private PlayerGesture RandomGesture() {
        Array values = Enum.GetValues(typeof(PlayerGesture));
        return (PlayerGesture)values.GetValue(UnityEngine.Random.Range(1, values.Length));
    }

    public float CurrentDiff {
        get => Time.time - this.startTime;
    }
}