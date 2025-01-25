using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] private float bobaShootForce = 10f;
    [SerializeField] private float bobaSpawnTime = 10f;
    public event Action<Transform> SeagullHit;


    private SwitchCamera _switchCamera;
    private GameObject _bobaPrefab;
    private Transform _bobaSpawnPoint;

    private Camera _camera;
    private int _maxAmmo = 1;
    private int _ammo = 0;

    private GameObject _draggedObject;
    private Vector3 _dragOffset;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (_switchCamera.camera1.enabled)
        {
            _camera = _switchCamera.camera1;
        }

        if (_switchCamera.camera2.enabled)
        {
            _camera = _switchCamera.camera2;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var target = hit.collider.gameObject;
                //if (target == bubble || topping)
                if (target.CompareTag("Bubble") || target.CompareTag("Topping"))
                {
                    StartDragging(target, hit.point);
                }
                //if (target == straw)
                else if (target.CompareTag("Straw"))
                {
                    SelectStraw(target);
                }
                //if (ammo > 0)
                else if (_ammo > 0)
                {
                    ShootBoba();
                }
            }
        }

        //drop
        if (Input.GetButtonUp("Fire1"))
        {
            _draggedObject = null;
        }

        //charge straw
        if (Input.GetMouseButtonDown(1))
        {
            _ammo++;
            if (_ammo > _maxAmmo)
            {
                _ammo = _maxAmmo;
                Debug.Log("Max Ammo");
                return;
            }
        }
    }

    private void StartDragging(GameObject target, Vector3 hitPoint)
    {
        _draggedObject = target;
        _dragOffset = target.transform.position - hitPoint;
    }


    private void SelectStraw(GameObject straw)
    {
        Debug.Log($"Straw selected: {straw.name}");
    }


    private void ShootBoba()
    {
        if (_ammo <= 0)
        {
            return;
        }

        GameObject boba = Instantiate(_bobaPrefab, _bobaSpawnPoint.position, _bobaSpawnPoint.rotation);
        Rigidbody bobaRigidbody = boba.GetComponent<Rigidbody>();
        bobaRigidbody.AddForce(_camera.transform.forward * bobaShootForce, ForceMode.Impulse);
        _ammo--;
        Destroy(boba, bobaSpawnTime);

        StartCoroutine(TrackBobaHitCoroutine(boba));
        //if (hits object && objectHit.CompareTag("Seagull"))
        //if hits a gameObject CompareTag("seagull")
        //vat target = hit gameObject
        //SeagullHit?.Invoke(target)
        //Destroy(boba)

    }

    private IEnumerator TrackBobaHitCoroutine(GameObject boba)
    {
        float elapsedTime = 0f;
        while (elapsedTime < bobaSpawnTime)
        {
            elapsedTime += Time.deltaTime;
            RaycastHit hit;
            Vector3 direction = boba.GetComponent<Rigidbody>().velocity.normalized; // Direction of boba's movement
            if (Physics.Raycast(boba.transform.position, direction, out hit, 0.5f)) // Adjust the range as needed
            {
                Transform hitObject = hit.collider.gameObject.transform;

                // Check if the hit object is a seagull
                if (hitObject.CompareTag("Seagull"))
                {
                    Debug.Log($"Seagull hit: {hitObject.name}");
                    SeagullHit?.Invoke(hitObject);
                    Destroy(boba);
                    yield break;
                }
            }
            yield return null;
        }
        Destroy(boba);
    }
}