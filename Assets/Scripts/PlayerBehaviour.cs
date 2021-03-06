﻿using Battlehub.Dispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour {

    [SerializeField]
    private float speed;
    private float xMax = 9;
    private float yMax = 5;
    [SerializeField]
    private GameObject laser;
    [SerializeField]
    private GameObject laserReference;
    [SerializeField]
    private GameObject laserTripleShoot;
    private int lives = 3;
    [SerializeField]
    private Text livesText;
    private bool isSpeed = false;
    private bool isTripleShoot = false;
    [SerializeField]
    private Text scoreText;
    public int score;

    void Start() {

    }

    void Update() {
        Moviment();
        Shoot();
        CheckLives();
        UpdateScore();
    }

    void Moviment() {
        if (isSpeed) {
            transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * speed * 2 * Time.deltaTime);
            transform.Translate(Vector3.up * Input.GetAxis("Vertical") * speed * 2 * Time.deltaTime);
        } else {
            transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime);
            transform.Translate(Vector3.up * Input.GetAxis("Vertical") * speed * Time.deltaTime);
        }

        if (transform.position.x > xMax) {
            transform.position = new Vector3(-xMax, transform.position.y);
        } else if (transform.position.x < -xMax) {
            transform.position = new Vector3(xMax, transform.position.y);
        } else if (transform.position.y > yMax) {
            transform.position = new Vector3(transform.position.x, -yMax);
        } else if (transform.position.y < -yMax) {
            transform.position = new Vector3(transform.position.x, yMax);
        }
    }

    void Shoot() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (isTripleShoot) {
                Instantiate(laserTripleShoot, laserReference.transform.position, Quaternion.identity);
            } else {
                Instantiate(laser, laserReference.transform.position, Quaternion.identity);
            }
        }
    }

    void CheckLives() {
        Dispatcher.Current.BeginInvoke(() => {
            livesText.text = "❤" + lives.ToString();
        });
        if (lives <= 0) {
            ScoreController.SaveScore(score);
            SceneManager.LoadScene("EndScene");
        }
    }

    void UpdateScore() {
        Dispatcher.Current.BeginInvoke(() => {
            scoreText.text = score.ToString();
        });
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "asteroid") {
            Destroy(collision.gameObject);
            lives--;
            // Mostar que ele recebeu dano
            StartCoroutine(BlinkEffect(new[] { Color.white, Color.clear }));
        }

        if (collision.tag == "life") {
            Destroy(collision.gameObject);
            lives++;
            StartCoroutine(BlinkEffect(new[] {Color.red, new Color(255, 165, 0) }));
        }

        if (collision.tag == "pu_speed") {
            Destroy(collision.gameObject);
            StartCoroutine(StartSpeedPU());
        }

        if (collision.tag == "pu_triple_shoot") {
            Destroy(collision.gameObject);
            StartCoroutine(StartTripleShootPU());
        }
    }

    IEnumerator StartSpeedPU() {
        isSpeed = true;
        yield return new WaitForSeconds(5f);
        isSpeed = false;
    }

    IEnumerator StartTripleShootPU() {
        isTripleShoot = true;
        yield return new WaitForSeconds(5f);
        isTripleShoot = false;
    }

    IEnumerator BlinkEffect(Color[] colors, int blinkTime = 6) {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        for (int i = 0; i < blinkTime; i++) {
            foreach (Color color in colors) {
                sprite.color = color;
                yield return new WaitForSeconds(0.1f);
            }
        }

        sprite.color = Color.white;
    }

}
