using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseController : MonoBehaviour {

    public AudioClip coinCollectSound;
    public AudioSource jetpackAudio;
    public AudioSource footstepsAudio;

    public float jetPackForce = 75.0f;
    public float forwardMovementSpeed = 3.0f;
    public Rigidbody2D rb;

    public Transform groundCheckTransform;
    private bool grounded;
    public LayerMask groundCheckLayerMask;
    Animator animator;
    private bool jetPackActive;

    public ParticleSystem jetPack;

    private bool dead = false;

    private uint coins = 0;

    public Texture2D coinIconTexture;

    public ParallaxScroll parallax;


    // Use this for initialization
    void Start () {
        animator = this.GetComponent<Animator>();
        
    }
	
	// Update is called once per frame
	void Update () {
        //UpdateGroundStatus();
    }

    void FixedUpdate() {
        setJetPackForce();
        setMovementSpeed(dead);
        UpdateGroundStatus();
        AdjustJetPack(jetPackActive);
        AdjustFootstepsAndJetpackSound(jetPackActive);
        parallax.offset = transform.position.x;
    }

    void UpdateGroundStatus() {
        grounded = Physics2D.OverlapCircle(groundCheckTransform.position, 0.1f, groundCheckLayerMask);
        animator.SetBool("grounded", grounded);
    }

    void setMovementSpeed(bool isDead) {
        if (!isDead) {
            Vector2 newVelocity = this.GetComponent<Rigidbody2D>().velocity;
            newVelocity.x = forwardMovementSpeed;
            this.GetComponent<Rigidbody2D>().velocity = newVelocity;
        }       
    }

    void setJetPackForce() {        
        jetPackActive = Input.GetButton("Fire1");

        jetPackActive = jetPackActive && !dead;

        if (jetPackActive) {
            this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jetPackForce));
        }
    }

    void AdjustJetPack(bool jetPackActive) {
        jetPack.enableEmission = !grounded;
        jetPack.emissionRate = jetPackActive ? 300.0f : 75.0f;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Coins")) {
            CollectCoin(collision);
        } else
            HitByLaser(collision);

    }

    private void HitByLaser(Collider2D collision) {
        if (!dead) collision.GetComponent<AudioSource>().Play();
        dead = true;
        animator.SetBool("dead", true);
    }

    void CollectCoin(Collider2D coinCollider) {
        AudioSource.PlayClipAtPoint(coinCollectSound, transform.position);
        coins++;
        Destroy(coinCollider.gameObject);
    }

    void DisplayCoinsCount() {
        Rect coinIconRect = new Rect(10, 10, 32, 32);
        GUI.DrawTexture(coinIconRect, coinIconTexture);

        GUIStyle style = new GUIStyle();
        style.fontSize = 30;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.yellow;

        Rect labelRect = new Rect(coinIconRect.xMax, coinIconRect.y, 60, 32);
        GUI.Label(labelRect, coins.ToString(), style);
    }

    private void OnGUI() {
        DisplayCoinsCount();
        DisplayRestartButton();
    }

    void DisplayRestartButton() {
        if(dead && grounded) {
            Rect buttonRect = new Rect(Screen.width * 0.35f, Screen.height * 0.45f, Screen.width * 0.30f, Screen.height * 0.1f);
            if (GUI.Button(buttonRect, "Tap to Restart!")) {
                SceneManager.LoadScene(Application.loadedLevelName);
                //Application.LoadLevel(Application.loadedLevelName); 
            }
        }
    }

    void AdjustFootstepsAndJetpackSound(bool jetpackActive) {
        footstepsAudio.enabled = !dead && grounded;

        jetpackAudio.enabled = !dead && !grounded;
        jetpackAudio.volume = jetpackActive ? 1.0f : 0.5f;
    }
}
