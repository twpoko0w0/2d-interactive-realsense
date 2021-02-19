using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GesturePlayer : MonoBehaviour
{

    private GameController gameController;

    private bool isSettedUp;

    private int? prevIndex;
    private int? prevRoundIndex;

    private CharacterController character;

    [HideInInspector]
    public float healthPoint = 100;

    public const float MAX_HEALTH_POINT = 100;


    // Display

    public int playerIndex;
    public Text titleText;


    [HideInInspector]
    public PlayerGestureRef[] gestureRefs = new PlayerGestureRef[GameController.GESTURE_PER_ROUND];


    // Start

    protected virtual void Start() {
        this.gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        this.titleText.text = $"Player {playerIndex + 1}";
    }


    // Update

    protected virtual void Update() {

        /*if (Input.GetKeyDown(KeyCode.A)) {
            this.GestureAction(PlayerGesture.BothHand);
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            this.GestureAction(PlayerGesture.LeftHand);
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            this.GestureAction(PlayerGesture.RightHand);
        }
        if (Input.GetKeyDown(KeyCode.F)) {
            this.GestureAction(PlayerGesture.Jump);
        }
        if (Input.GetKeyDown(KeyCode.G)) {
            this.GestureAction(PlayerGesture.LeftLean);
        }
        if (Input.GetKeyDown(KeyCode.H)) {
            this.GestureAction(PlayerGesture.RightLean);
        }*/


        // Check Round Start

        if (!this.IsSettedUp) {
            return;
        }

        if (this.prevRoundIndex == null && this.prevIndex == null && this.gameController.CurrentGestureIndex != 0) {
            return;
        }


        // Round Change

        if (this.gameController.RoundIndex != this.prevRoundIndex) {
            for (int i = 0; i < GameController.GESTURE_PER_ROUND; i++) {
                this.gestureRefs[i] = new PlayerGestureRef(this.gameController.gestureRefs[i]);
            }
        }


        // Gesture Change

        if (this.gameController.CurrentGestureIndex != this.prevIndex) {
            this.CheckTakeDamage();
        }


        // Set Previous

        this.prevRoundIndex = this.gameController.RoundIndex;
        this.prevIndex = this.gameController.CurrentGestureIndex;
    }

    void CheckTakeDamage() {
        if (this.PreviousGesture != null && !this.PreviousGesture.isCorrect) {
            // Take Damage
            this.healthPoint -= UnityEngine.Random.Range(1, 5);
        }
    }

    public void GestureAction(PlayerGesture gesture) {
        var currentRef = this.gameController.CurrentGesture;
        if (currentRef != null && this.CurrentGesture != null && gesture == currentRef.gesture && !this.CurrentGesture.isCorrect) {
            // Do Damage
            this.CurrentGesture.isCorrect = true;
            this.gameController.TakeDamage(UnityEngine.Random.Range(5, 10));
            this.character.TriggerAttack();
        }
    }

    public void SetupPlayer(int index) {
        if (!isSettedUp) {
            this.gameObject.SetActive(true);
            this.isSettedUp = true;
            this.character = Instantiate(Resources.Load<GameObject>("Prefabs/Character")).GetComponent<CharacterController>();
            this.character.Setup(index);
        }
    }

    public void ResetPlayer() {
        if (isSettedUp) {
            this.isSettedUp = false;
            this.healthPoint = MAX_HEALTH_POINT;
            Destroy(this.character.gameObject, 5);
            this.character.SetExit();
            this.character = null;
            this.gameObject.SetActive(false);
        }
    }


    // Utils

    public int CurrentGestureIndex {
        get => this.gameController.CurrentGestureIndex;
    }

    public PlayerGestureRef CurrentGesture {
        get => this.gestureRefs[this.CurrentGestureIndex];
    }

    public PlayerGestureRef PreviousGesture {
        get => this.CurrentGestureIndex - 1 >= 0 ? this.gestureRefs[this.CurrentGestureIndex - 1] : null;
    }

    public bool IsSettedUp {
        get => this.isSettedUp;
    }
}

public class PlayerGestureRef
{
    public GestureRef gestureRef;
    public bool isCorrect = false;

    public PlayerGestureRef(GestureRef g) {
        this.gestureRef = g;
    }
}