using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] private float bobaShootForce = 10f;
    [SerializeField] private float bobaSpawnTime = 10f;
    public event Action<Transform> SeagullHit;


    private SwitchCamera _switchCamera;
    private GameObject _bobaPrefab;
    private Transform _bobaSpawnPoint;
    private SoundManager _soundManager;

    public GameObject cup;
    private GameObject _syrup;
    private GameObject _cupCoverGeo;
    private GameObject _tapioca;
    private GameObject _strawGeo;
    private BubbleTeaManager _bubbleTeaManager;
    private PanelManager _panelManager;
    
    private SyrupBridge _selectedSyrup;
    private BubbleBridge _selectedBubble;
    private ToppingBridge _selectedTopping;
    
    private GameObject _activeCup;
    private bool doesCupExist;
    
    public Transform cupSpawnPoint;
    public Transform cupReadyPosition;

    public GameObject bobaPrefab;
    private GameObject _activeBoba;
    private bool doesBobaExist;
    
    private bool doesSyrupExist;
    private bool isReady;
    
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
        _bubbleTeaManager = gameManager.GetComponent<BubbleTeaManager>();
        _soundManager = gameManager.GetComponent<SoundManager>();
        
        _camera = Camera.main; 
        
        _isStrawEquipped = false;
        doesCupExist = false;
        doesBobaExist = false;
        doesSyrupExist = false;
        isReady = false;
    }

    private void Update()
    {
        if (_switchCamera.isCamera1)
        { _camera = _switchCamera.camera1; }
        
        else
        { _camera = _switchCamera.camera2; }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                var target = hit.collider.gameObject;
                _soundManager.PlayAudio("/Suoni/Interazione");
                Debug.LogWarning(target.name);

                if (!_isStrawEquipped)
                {
                    if (target.CompareTag("Cup"))
                    {
                        if (doesCupExist)
                        {
                            return;
                        }

                        _activeCup = Instantiate(cup, cupSpawnPoint.position, Quaternion.identity);


                        // Find specific child objects by name
                        _syrup = _activeCup.transform.Find("Liquido").gameObject;
                        _tapioca = _activeCup.transform.Find("Tapioca").gameObject;
                        _strawGeo = _activeCup.transform.Find("straw_geo").gameObject;
                        _cupCoverGeo = _activeCup.transform.Find("cup_cover_geo").gameObject;

                        _syrup.SetActive(false);
                        _tapioca.SetActive(false);
                        _strawGeo.SetActive(false);
                        _cupCoverGeo.SetActive(false);

                        doesCupExist = true;
                    }

                    if (target.CompareTag("Bubble"))
                    {
                        if (!doesCupExist || doesBobaExist)
                        {
                            return;
                        }

                        _activeBoba = Instantiate(bobaPrefab, hit.point, Quaternion.identity);
                        _selectedBubble = target.GetComponent<BubbleBridge>();

                        doesBobaExist = true;
                        _draggedObject = _activeBoba;
                    }

                    if (target.CompareTag("Syrup") && doesBobaExist)
                    {
                        _syrup.SetActive(true);
                        _selectedSyrup = target.GetComponent<SyrupBridge>();

                        doesSyrupExist = true;
                    }

                    if (target.CompareTag("Topping") && doesSyrupExist)
                    {
                        _strawGeo.SetActive(true);
                        _cupCoverGeo.SetActive(true);
                        _selectedTopping = target.GetComponent<ToppingBridge>();

                        _activeCup.transform.position = cupReadyPosition.position;
                        isReady = true;
                    }


                    if (target.CompareTag("Customer") && isReady)
                    {
                        CustomerOrder customerOrder = target.GetComponentInParent<CustomerOrder>();
                        if (customerOrder != null)
                        {
                            bool isOrderValid = customerOrder.ValidateOrder
                            (_bubbleTeaManager.selectedBubble, _bubbleTeaManager.selectedSyrup,
                                _bubbleTeaManager.selectedTopping);
                            if (isOrderValid)
                            {
                                Debug.Log("Customer is satisfied!");
                            }
                            else
                            {
                                Debug.Log("Customer is dissatisfied.");
                            }

                            ResetCup();
                        }
                    }
                }

                if (_isStrawEquipped )
                {
                    if (_ammo > 0)
                    { ShootBoba(); }
                    
                }
                
            }
        }
        if (_draggedObject != null && Input.GetMouseButton(0)) 
        {
            DragObject();
            _soundManager.PlayAudio("/Suoni/Interazione");
        }

        //drop
        if (Input.GetButtonUp("Fire1"))
        {
            if (_draggedObject != null)
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                _soundManager.PlayAudio("/Suoni/Interazione");
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    var target = hit.collider.gameObject;
                    if (target.CompareTag("ActiveCup"))
                    { _tapioca.SetActive(true); }
                    Destroy(_draggedObject);
                    _draggedObject = null;
                }
                else
                {
                    Destroy(_draggedObject);
                    _draggedObject = null;
                    doesBobaExist = false;
                }
            }
            else
            {
                Destroy(_draggedObject);
                _draggedObject = null;
                doesBobaExist = false;
            }
        }



        //charge straw
        if (Input.GetMouseButtonDown(1) && _isStrawEquipped)
        {
            _panelManager.ToggleStrawPanel();
            _ammo++;
            _soundManager.PlayAudio("/Suoni/Sucking ballz");
            if (_ammo > _maxAmmo)
            {
                _ammo = _maxAmmo;
                Debug.Log("Max Ammo");
            }
        }
    }

    
    private void DragObject()
    {
        _dragPlane = new Plane(Vector3.up, _draggedObject.transform.position);
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition); 
        float distanceToPlane;


        if (_dragPlane.Raycast(ray, out distanceToPlane))
        {
            Vector3 worldPosition = ray.GetPoint(distanceToPlane); 
            _draggedObject.transform.position = worldPosition; 
        }
    }



    private void SelectStraw(GameObject straw)
    {
        Debug.Log($"Straw selected: {straw.name}");
        _isStrawEquipped = true;
        _soundManager.PlayAudio("Interazione");
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
            _soundManager.PlayAudio("/Suoni/Sparo");
            RaycastHit hit;
            Vector3 direction = boba.GetComponent<Rigidbody>().velocity.normalized;
            if (Physics.Raycast(boba.transform.position, direction, out hit, Mathf.Infinity)) 
            {
                Transform hitObject = hit.collider.gameObject.transform;

                // Check if the hit object is a seagull
                if (hitObject.CompareTag("Seagull"))
                {
                    Debug.Log($"Seagull hit: {hitObject.name}");
                    _soundManager.PlayAudio("/Suoni/Gabbiano Death");
                    SeagullHit?.Invoke(hitObject);
                    Destroy(boba);
                    yield break;
                }
            }
            yield return null;
        }
        Destroy(boba);
    }
    
    
    private void ResetCup()
    {
        if (_activeCup != null)
        {
            Destroy(_activeCup);
            doesCupExist = false;
        }
        doesCupExist = false;
        doesBobaExist = false;
        doesSyrupExist = false;
        isReady = false;    
    }

}