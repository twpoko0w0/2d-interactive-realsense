using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private float runSpeed = 0.05f;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isEntering = true;
    private bool isExiting = false;

    private GesturePlayer player;
    private int characterIndex;

    // Const

    public static Vector3 START_POSITION = new Vector3(-5.63f, -0.42f, -5.11f);
    const float RIGHT_MOST = -0.8f;
    const float SPACING = 0.7f;

    // Start is called before the first frame update
    void Start()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.animator = GetComponent<Animator>();

        this.transform.position = CharacterController.START_POSITION;
    }

    public void Setup(int index) {
        this.characterIndex = index;
    }

    // Start -5.63
    // End -0.8 (Right Most)
    // Spacing 0.7

    // Update is called once per frame
    void Update()
    {
        if (isEntering && this.transform.position.x < RIGHT_MOST - (SPACING * characterIndex)) {
            this.animator.SetBool("Stand", false);
            this.transform.position += new Vector3(runSpeed, 0, 0);
        } else {
            this.animator.SetBool("Stand", true);
            this.isEntering = false;
        }

        if (isExiting && this.transform.position.x > START_POSITION.x) {
            this.animator.SetBool("Stand", false);
            this.transform.localScale = new Vector3(-0.5f, 0.5f, 1);
            this.transform.position -= new Vector3(runSpeed, 0, 0);
        } 

    }

    public void TriggerAttack() {
        this.animator.SetTrigger("Attack");
        Instantiate(Resources.Load<GameObject>("Prefabs/Effect"));
    }

    public void SetExit() {
        this.isEntering = false;
        this.isExiting = true;
    }
}
