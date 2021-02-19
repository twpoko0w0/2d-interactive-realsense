using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [HideInInspector]
    public GesturePlayer player;

    public string title;

    public RectTransform barTransform;
    public Text healthText;

    [HideInInspector]
    public float maxHealth;
    [HideInInspector]
    public float health;

    // Start is called before the first frame update
    void Start()
    {
        this.player = GetComponentInParent<GesturePlayer>();
        if (this.player != null) {
            this.maxHealth = GesturePlayer.MAX_HEALTH_POINT;
        }
        this.UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.player != null) {
            this.health = this.player.healthPoint;
        }

        var scaleX = Mathf.Clamp01(this.health / this.maxHealth);
        this.barTransform.localScale = new Vector3(scaleX, 1, 1);
        this.UpdateText();
    }

    void UpdateText() {
        this.healthText.text = "";

        if (title != "") {
            this.healthText.text = $"{title} ";
        }

        this.healthText.text += $"{Mathf.RoundToInt(this.health)}/{Mathf.RoundToInt(this.maxHealth)}";
    }
}
