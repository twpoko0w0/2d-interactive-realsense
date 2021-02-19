using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureImage : MonoBehaviour
{
    public Image image;
    private RectTransform imageRectTransform;

    public int gestureIndex;

    [HideInInspector]
    public GameController controller;

    private int prevRoundIndex;

    private Image backgroundImage;

    public Color activeColor;
    public Color inactiveColor;

    // Start is called before the first frame update
    void Start() {
        this.controller = GameObject.Find("Game Controller").GetComponent<GameController>();
        this.imageRectTransform = this.image.GetComponent<RectTransform>();
        this.backgroundImage = this.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update() {
        //var scaleX = Mathf.Clamp01((Time.time - this.CurrentGesture.startTime) / GameController.SECONDS_PER_GESTURE);
        if (this.CurrentGesture == null) return;

        this.image.sprite = Resources.Load<Sprite>(this.CurrentGesture.gesture.ToString());

        var alpha = Mathf.Clamp01((Time.time - this.CurrentGesture.startTime + 1));
        var scale = Mathf.Max(0.7f, alpha);

        var isActive = this.controller.CurrentGestureIndex >= this.gestureIndex;
        var green = this.controller.CurrentGestureIndex >= this.gestureIndex ? 1 : 0;

        this.image.color = isActive ? new Color(0.1f, 0.29f, 0.27f, 1) : new Color(0, 0, 0, Mathf.Max(0.5f, alpha));
        this.imageRectTransform.localScale = new Vector3(scale, scale, 1);
        this.backgroundImage.color = isActive ? activeColor : inactiveColor;
    }


    // Utils

    private GestureRef CurrentGesture {
        get => this.controller.gestureRefs[this.gestureIndex];
    }
}

