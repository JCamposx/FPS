using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float shootDistance = 4f;
    [SerializeField] private ParticleSystem shotgunPS;
    [SerializeField] private ParticleSystem assaultRiflePS;
    [SerializeField] private Image canvasHealthImage;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Sprite[] healthImages1;
    [SerializeField] private Sprite[] healthImages2;
    [SerializeField] private Sprite[] healthImages3;
    [SerializeField] private Sprite[] healthImages4;

    private Vector2 mDirection;
    private Vector2 mDeltaLook;
    private bool isEquippedShotgun = true;
    private bool isFiring = false;
    private float fireRate = 0.125f;
    private float accumulatedFireTime = 0.15f;
    private float fireDamage = 4f;
    private Sprite[] healthImagesCopy;
    public float health = 30f;
    private bool isReceivingDamage = false;
    private float receiveDamageTime = 0f;
    private bool hadReceivedDamage = false;

    private Rigidbody mRb;
    private Transform cameraMain;
    private GameObject Shotgun;
    private GameObject AssaultRiffle;
    private GameObject debugImpactSphere;
    private GameObject bloodObjectParticles;

    private void Start()
    {
        mRb = GetComponent<Rigidbody>();

        cameraMain = transform.Find("Main Camera");
        AssaultRiffle = cameraMain.GetChild(0).gameObject;
        Shotgun = cameraMain.GetChild(1).gameObject;

        debugImpactSphere = Resources.Load<GameObject>("DebugImpactSphere");
        bloodObjectParticles = Resources.Load<GameObject>("BloodSplat_FX Variant");

        Cursor.lockState = CursorLockMode.Locked;
        AssaultRiffle.SetActive(!isEquippedShotgun);
        Shotgun.SetActive(isEquippedShotgun);

        healthImagesCopy = healthImages1;
        canvasHealthImage.sprite = healthImagesCopy[0];
        healthBar.maxValue = health;
        healthBar.value = health;
    }

    private void FixedUpdate()
    {
        if (health <= 0f) {
            AssaultRiffle.SetActive(false);
            Shotgun.SetActive(false);
            VerifyRestartMatch();
            return;
        }

        VerifyShootAssaultRifle();

        ChangeCanvasHealthImage();

        VerifyIsReceivingDamage();

        VerifyRestartMatch();

        mRb.velocity = mDirection.y * speed * transform.forward
            + mDirection.x * speed * transform.right;

        transform.Rotate(
            Vector3.up,
            turnSpeed * Time.deltaTime * mDeltaLook.x
        );

        cameraMain.GetComponent<CameraMovement>().RotateUpDown(
            -turnSpeed * Time.deltaTime * mDeltaLook.y
        );
    }

    private void OnMove(InputValue value)
    {
        mDirection = value.Get<Vector2>();
    }

    private void OnLook(InputValue value)
    {
        mDeltaLook = value.Get<Vector2>();
    }

    private void OnFire(InputValue value)
    {
        if (health <= 0f) return;

        if (value.isPressed)
        {
            if (isEquippedShotgun)
            {
                shootDistance = 4f;
                fireDamage = 5f;

                Shoot();
            }
            else
            {
                isFiring = true;
                shootDistance = 20f;
                fireDamage = 2.5f;
            }
        }
    }

    private void OnChangeGun(InputValue value)
    {
        if (health <= 0f) return;

        if (value.isPressed)
        {
            isEquippedShotgun = !isEquippedShotgun;

            AssaultRiffle.SetActive(!isEquippedShotgun);
            Shotgun.SetActive(isEquippedShotgun);
        }
    }

    private void Shoot()
    {
        (isEquippedShotgun ? shotgunPS : assaultRiflePS).Play();

        RaycastHit hit;

        if (Physics.Raycast(
            cameraMain.position,
            cameraMain.forward,
            out hit,
            shootDistance
        ))
        {
            if (hit.collider.CompareTag("Enemigos"))
            {
                var bloodPS = Instantiate(bloodObjectParticles, hit.point, Quaternion.identity);

                Destroy(bloodPS, 3f);

                var enemyController = hit.collider.GetComponent<EnemyController>();

                enemyController.TakeDamage(fireDamage);
            }
            else
            {
                var otherPS = Instantiate(
                    (isEquippedShotgun ? shotgunPS : assaultRiflePS).gameObject,
                    hit.point,
                    Quaternion.identity
                );

                otherPS.GetComponent<ParticleSystem>().Play();

                Destroy(otherPS, 3f);
            }
        }
    }

    private void VerifyShootAssaultRifle()
    {
        if (Mouse.current.rightButton.isPressed && !isEquippedShotgun)
        {
            AssaultRiffle.transform.localPosition = new Vector3(
                0.001f,
                -0.116f,
                0.109f
            );

            GameManager.Instance.HideCrossHair();
        }
        else
        {
            AssaultRiffle.transform.localPosition = new Vector3(
                0.209f,
                -0.172f,
                0.166f
            );

            GameManager.Instance.ShowCrossHair();
        }


        if (Mouse.current.leftButton.isPressed && !isEquippedShotgun && isFiring)
        {
            accumulatedFireTime += Time.deltaTime;

            if (accumulatedFireTime >= fireRate)
            {
                Shoot();
                accumulatedFireTime = 0f;
            }
        }
        else
        {
            isFiring = false;
            accumulatedFireTime = fireRate;
        }
    }

    public void TakeDamage(float damage)
    {
        isReceivingDamage = true;

        health -= damage;
        healthBar.value = health;

        if (health <= healthBar.maxValue * 0.25f)
        {
            hadReceivedDamage = true;
            healthImagesCopy = healthImages4;
        }
        else if (health <= healthBar.maxValue * 0.5f)
        {
            hadReceivedDamage = true;
            healthImagesCopy = healthImages3;
        }
        else if (health <= healthBar.maxValue * 0.75f)
        {
            hadReceivedDamage = true;
            healthImagesCopy = healthImages2;
        }
        else
        {
            healthImagesCopy = healthImages1;
        }
    }

    private void ChangeCanvasHealthImage()
    {
        if (health <= 0f)
        {
            canvasHealthImage.sprite = healthImagesCopy[2];
            return;
        }

        if (isReceivingDamage)
        {
            canvasHealthImage.sprite = healthImagesCopy[2];
            return;
        }

        if (hadReceivedDamage)
        {
            canvasHealthImage.sprite = healthImagesCopy[4];
            return;
        }

        canvasHealthImage.sprite = healthImagesCopy[0];
    }

    private void VerifyIsReceivingDamage()
    {
        if (!isReceivingDamage) return;

        receiveDamageTime += Time.deltaTime;

        if (receiveDamageTime < 1f) return;

        isReceivingDamage = false;
        receiveDamageTime = 0f;
    }

    private void VerifyRestartMatch()
    {
        if (health <= 0f)
        {
            GameManager.Instance.ManageRestartMatch();
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Enemigo-Attack"))
        {
            Debug.Log("Player recibio danho");
            TakeDamage(1f);
        }
    }
}
