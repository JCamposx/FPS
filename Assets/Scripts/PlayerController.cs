using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float shootDistance = 4f;
    [SerializeField] private ParticleSystem shotgunPS;
    [SerializeField] private ParticleSystem assaultRiflePS;
    [SerializeField] private float health;

    private Vector2 mDirection;
    private Vector2 mDeltaLook;
    private bool isEquippedShotgun = true;
    private bool isFiring = false;
    private float fireRate = 0.15f;
    private float accumulatedFireTime = 0.15f;
    private float fireDamage = 4f;

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
    }

    private void FixedUpdate()
    {
        VerifyShootAssaultRifle();

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
        if (value.isPressed)
        {
            if (isEquippedShotgun)
            {
                shootDistance = 4f;
                fireDamage = 4f;

                Shoot();
            }
            else
            {
                isFiring = true;
                shootDistance = 20f;
                fireDamage = 0.5f;
            }
        }
    }

    private void OnChangeGun(InputValue value)
    {
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
        health -= damage;

        if (health <= 0f)
        {
            // Fin del juego
            Debug.Log("Fin del juego");
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
