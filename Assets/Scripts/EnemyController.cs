using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float Speed = 2f;
    public float AwakeRadio = 2f;
    public float AttackRadio = .5f;


    private Animator mAnimator;
    private Rigidbody mRb;

    private Vector2 mDirection; // XZ

    private bool mIsAttacking = false;


    private void Start()
    {
        mRb = GetComponent<Rigidbody>();
        mAnimator = transform
            .GetComponentInChildren<Animator>(false);
    }

    private void Update()
    {
        var collider = IsPlayerInAttackArea();
        if (collider != null && !mIsAttacking)
        {
            mRb.velocity = new Vector3(
                0f,
                0f,
                0f
            );
            mAnimator.SetBool("IsWalking", false);
            mAnimator.SetTrigger("Attack");
            return;
        }

        collider = IsPlayerNearby();
        if (collider != null && !mIsAttacking)
        {
            // caminar
            var playerPosition = collider.transform.position;
            var direction = playerPosition - transform.position;
            mDirection = new Vector2(direction.x, direction.z);

            //transform.LookAt(playerPosition, Vector3.up);
            direction.y = 0f;
            
            transform.rotation = Quaternion.Lerp(
                transform.rotation, 
                Quaternion.LookRotation(direction, Vector3.up), 
                0.1f
            );

            mRb.velocity = new Vector3(
                mDirection.x * Speed,
                0f,
                mDirection.y * Speed
            );

            mAnimator.SetBool("IsWalking",true);
            mAnimator.SetFloat("Horizontal", mDirection.x);
            mAnimator.SetFloat("Vertical", mDirection.y);
        }else 
        {
            // parar
            mRb.velocity = Vector3.zero;
            mAnimator.SetBool("IsWalking",false);
        }
    }

    private Collider IsPlayerNearby()
    {
        var colliders = Physics.OverlapSphere(
            transform.position,
            AwakeRadio,
            LayerMask.GetMask("Player")
        );
        if (colliders.Length == 1) return colliders[0];
        else return null;
    }

    private Collider IsPlayerInAttackArea()
    {
        var colliders = Physics.OverlapSphere(
            transform.position,
            AttackRadio,
            LayerMask.GetMask("Player")
        );
        if (colliders.Length == 1) return colliders[0];
        else return null;
    }

    public void StartAtack()
    {
        mIsAttacking = true;
    }

    public void StopAttack()
    {
        mIsAttacking = false;
    }
}
