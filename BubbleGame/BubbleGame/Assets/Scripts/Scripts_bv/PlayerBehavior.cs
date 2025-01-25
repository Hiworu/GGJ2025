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
    private Plane _dragPlane;
    private bool _isStrawEquipped = false;

    private void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        _switchCamera = gameManager.GetComponent<SwitchCamera>();
        _camera = Camera.main; 
        _dragPlane = new Plane(Vector3.up, Vector3.zero);
        
        _isStrawEquipped = false;
    }

    private void Update()
    {
        if (_switchCamera.isCamera1)
        { _camera = _switchCamera.camera1; }
        
        else
        { _camera = _switchCamera.camera2; }

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
                    
                    if (_ammo > 0 && !_isStrawEquipped)
                    {
                        ShootBoba();
                    }
                }
            }
        }
        if (_draggedObject != null && Input.GetButton("Fire1"))
        {
            DragObject();
        }

        //drop
        if (Input.GetButtonUp("Fire1"))
        {
            _draggedObject = null;
        }

        //charge straw
        if (Input.GetMouseButtonDown(1) && _isStrawEquipped)
        {
            _ammo++;
            if (_ammo > _maxAmmo)
            {
                _ammo = _maxAmmo;
                Debug.Log("Max Ammo");
            }
        }
    }

    private void StartDragging(GameObject target, Vector3 hitPoint)
    {
        _draggedObject = target;
        _dragOffset = target.transform.position - hitPoint;
    }
    
    private void DragObject()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        float distanceToPlane;
        
        if (_dragPlane.Raycast(ray, out distanceToPlane))
        {
            Vector3 worldPosition = ray.GetPoint(distanceToPlane);
            _draggedObject.transform.position = worldPosition + _dragOffset; 
        }
    }



    private void SelectStraw(GameObject straw)
    {
        Debug.Log($"Straw selected: {straw.name}");
        _isStrawEquipped = true;
    }


    private void ShootBoba()
    {
        if (_ammo <= 0)
        { return; }

        GameObject boba = Instantiate(_bobaPrefab, _bobaSpawnPoint.position, _bobaSpawnPoint.rotation);
        Rigidbody bobaRigidbody = boba.GetComponent<Rigidbody>();
        bobaRigidbody.AddForce(_camera.transform.forward * bobaShootForce, ForceMode.Impulse);
        _ammo--;
        StartCoroutine(TrackBobaHitCoroutine(boba));
    }

    private IEnumerator TrackBobaHitCoroutine(GameObject boba)
    {
        float elapsedTime = 0f;
        while (elapsedTime < bobaSpawnTime)
        {
            elapsedTime += Time.deltaTime;
            RaycastHit hit;
            Vector3 direction = boba.GetComponent<Rigidbody>().velocity.normalized;
            if (Physics.Raycast(boba.transform.position, direction, out hit, Mathf.Infinity)) 
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