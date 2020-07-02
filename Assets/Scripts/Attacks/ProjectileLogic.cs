using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLogic : MonoBehaviour, IPooleableObject
{
    [SerializeField]
    private float _speed = 15f;
    [SerializeField]
    private float _maxDistance = 15f;
    [SerializeField]
    private float _hitOffset = 0f;
    [SerializeField]
    private bool _useFirePointRotation;
    [SerializeField]
    private Vector3 _rotationOffset = new Vector3(0, 0, 0);

    public GameObject hit;
    public GameObject flash;
    public GameObject[] Detached;

    private Rigidbody _rb;
    private float _currentDistance = 0.0f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void OnSpawn()
    {
        //Debug.Log(gameObject.name + " Spawned!");
        _currentDistance = 0.0f;
        if (flash != null)
        {
            var flashInstance = Instantiate(flash, transform.position, Quaternion.identity);
            flashInstance.transform.forward = transform.forward;
            var flashPs = flashInstance.GetComponent<ParticleSystem>();
            if (flashPs != null)
            {
                Destroy(flashInstance, flashPs.main.duration);
            }
            else
            {
                var flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(flashInstance, flashPsParts.main.duration);
            }
        }        
	}

    void FixedUpdate ()
    {
        float step = _speed * Time.fixedDeltaTime;
        Vector3 nextPosition = transform.position + transform.forward * step;
        _rb.MovePosition(nextPosition);
        _currentDistance += step;
        if (_currentDistance >= _maxDistance)
        {
            ObjectPoolController.Instance.Recycle(gameObject);
        }
        //transform.position += transform.forward * (speed * Time.deltaTime);         
	}

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point + contact.normal * _hitOffset;

        if (hit != null)
        {
            var hitInstance = Instantiate(hit, pos, rot);
            if (_useFirePointRotation) { hitInstance.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0); }
            else if (_rotationOffset != Vector3.zero) { hitInstance.transform.rotation = Quaternion.Euler(_rotationOffset); }
            else { hitInstance.transform.LookAt(contact.point + contact.normal); }

            var hitPs = hitInstance.GetComponent<ParticleSystem>();
            if (hitPs != null)
            {
                Destroy(hitInstance, hitPs.main.duration);
            }
            else
            {
                var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitInstance, hitPsParts.main.duration);
            }
        }

        foreach (var detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
            {
                detachedPrefab.transform.parent = null;
            }
        }

        ObjectPoolController.Instance.Recycle(gameObject);
    }

    public void OnAfterRecycle()
    {
        // post recycle method
        // Debug.Log(gameObject.name + " Recycled!");
    }
}
