using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureBar : MonoBehaviour
{
    [HideInInspector]
    public bool isCorrect = false;

    public int gestureIndex;
    [HideInInspector]
    public GesturePlayer player;

    //public RectTransform barTransform;
    //public Image barImage;

    public Color waitingColor;
    public Color correctColor;
    public Color wrongColor;

    public Image backgroundImage;
    public Image maskImage;
    public Image iconImage;


    // Start

    void Start() {
        this.player = GetComponentInParent<GesturePlayer>();
    }


    // Update

    void Update() {

        if (this.player == null || this.CurrentGesture == null || !this.player.IsSettedUp) return;

        var scaleX = Mathf.Clamp01((Time.time - this.CurrentGesture.gestureRef.startTime) / GameController.SECONDS_PER_GESTURE);

        this.iconImage.sprite = Resources.Load<Sprite>(this.CurrentGesture.gestureRef.gesture.ToString());

        if (this.CurrentGesture.isCorrect) {
            this.backgroundImage.color = correctColor;
            this.maskImage.fillAmount = 1;

        } else if (Time.time < this.CurrentGesture.gestureRef.endTime) {
            this.backgroundImage.color = waitingColor;
            this.maskImage.fillAmount = scaleX;

        } else {
            this.backgroundImage.color = wrongColor;
            this.maskImage.fillAmount = 1;

        }

    }


    // Utils

    private PlayerGestureRef CurrentGesture {
        get => this.player.gestureRefs[this.gestureIndex];
    }
}
