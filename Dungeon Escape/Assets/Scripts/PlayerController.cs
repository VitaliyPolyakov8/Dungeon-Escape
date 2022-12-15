using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 2;
    public Slider noiseSlider; 
    public float noiseCount;
    public float noiseCount2;
    public GameObject WinMenu;
    public GameObject LoseMenu;

    private Vector3 moveDirection;
    private Rigidbody rb;
    private Animator anim;
    private Vector3 position;
    private float time;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Time.timeScale = 1;
        moveDirection = Vector3.zero;
        anim = GetComponent<Animator>();
        position = rb.position;
        time = Time.time;
        noiseSlider.value = 0;
}

    void Update()
    {
            Move();
            Noise();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * Time.fixedDeltaTime);
    }

    void Move()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * speed;
        if (moveDirection != Vector3.zero)
        {
            anim.SetInteger("State", 1);
        }
        else
        anim.SetInteger("State", 0);

    }

    void Noise()
    {
        if (noiseSlider.value<= 12)
        {
            if (Vector3.Distance(position, rb.transform.position) > 0.2f)
            {
                noiseSlider.value += noiseCount;
                if (noiseSlider.value == 9)
                    noiseSlider.value += 1;
                position = rb.transform.position;

            }

            if (Time.time - time > 0.1f && noiseSlider.value > 0 && Vector3.Distance(position, rb.transform.position) <= 0.2f)
            {
                noiseSlider.value -= noiseCount2;
                time = Time.time;
            }
        }


    }

    void OnTriggerEnter(Collider exit)
    {
        if (exit.tag == "Exit")
        {
            Time.timeScale = 0;

           WinMenu.SetActive(true);
        }
    }

    void OnCollisionEnter(Collision enemy)
    {
        if (enemy.gameObject.tag == "Enemy")
        {
            Time.timeScale = 0;

            LoseMenu.SetActive(true);
        }
    }
}
