using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureBarsController : MonoBehaviour
{
    public GesturePlayer player;

    public GameObject gestureBar;
    public GameObject gestureBarContainer;

    public HealthBar healthBar;

     
    // Start

    void Start() {
        for (int i = 0; i < GameController.GESTURE_PER_ROUND; i++) {
            var gestureBar = Instantiate(this.gestureBar, gestureBarContainer.transform).GetComponent<GestureBar>();
            gestureBar.gestureIndex = i;
            gestureBar.player = this.player;
        }

        this.healthBar.player = this.player;
    }


    // Update

    void Update() {

    }
}
