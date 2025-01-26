using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] private float bobaShootForce = 50f;
    [SerializeField] private float bobaSpawnTime = 10f;
    public event Action<Transform> SeagullHit;
    private SeagullManager _seagullManager;


    private SwitchCamera _switchCamera;
    private GameObject _bobaPrefab;
    // private Transform _bobaSpawnPoint;
    //private SoundManager _soundManager;

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


    private GameObject _draggedObject;
    private Vector3 _dragOffset;
    private Plane _dragPlane;
    private bool _isStrawEquipped = false;

    private void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        _switchCamera = gameManager.GetComponent<SwitchCamera>();
        _bubbleTeaManager = gameManager.GetComponent<BubbleTeaManager>();
        _panelManager = gameManager.GetComponent<PanelManager>();
        _seagullManager = gameManager.GetComponent<SeagullManager>();
        //_soundManager = gameManager.GetComponent<SoundManager>();
        
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
                //_soundManager.PlayAudio("Suoni/Interazione.mp3");

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

            }

            if (_isStrawEquipped == true)
            {
                ShootBoba();
            }
        }
        if (_draggedObject != null && Input.GetMouseButton(0)) 
        {
            DragObject();
            //_soundManager.PlayAudio("/Suoni/Interazione");
        }

        //drop
        if (Input.GetButtonUp("Fire1"))
        {
            if (_draggedObject != null)
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                //_soundManager.PlayAudio("/Suoni/Interazione");
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
        if (Input.GetMouseButtonDown(1))
        {
            _panelManager.ToggleStrawPanel();
            _isStrawEquipped = !_isStrawEquipped;
            // _ammo++;
            //_soundManager.PlayAudio("/Suoni/Sucking ballz");
            // if (_ammo > _maxAmmo) { _ammo = _maxAmmo; }
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



    // private void SelectStraw(GameObject straw)
    // {
    //     Debug.Log($"Straw selected: {straw.name}");
    //     _isStrawEquipped = true;
    //     //_soundManager.PlayAudio("Interazione");
    // }


    private void ShootBoba()
    {
        // if (_ammo <= 0) { return; }
        
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            Vector3 spawnPoint = _camera.transform.position + _camera.transform.forward * 2f; 
            Vector3 targetPoint = hit.point;
            Vector3 direction = (targetPoint - spawnPoint).normalized;
            
            GameObject boba = Instantiate(bobaPrefab, spawnPoint, Quaternion.identity);
            
            Rigidbody bobaRigidbody = boba.GetComponent<Rigidbody>();
            if (bobaRigidbody != null)
            {
                bobaRigidbody.velocity = direction * bobaShootForce;
            }
            // _ammo--;

            StartCoroutine(TrackBobaHitCoroutine(boba));

        }
    }

    private IEnumerator TrackBobaHitCoroutine(GameObject boba)
    {
        float elapsedTime = 0f;
        int waterLayerMask = LayerMask.GetMask("Water");
        Vector3 previousPosition = boba.transform.position;
        
        while (elapsedTime < bobaSpawnTime)
        {
            elapsedTime += Time.deltaTime;
            
            Vector3 currentPosition = boba.transform.position;
            Vector3 direction = (currentPosition - previousPosition).normalized;
            previousPosition = currentPosition;
            
            //_soundManager.PlayAudio("/Suoni/Sparo");
            RaycastHit hit;
            if (Physics.Raycast(boba.transform.position, direction, out hit, Mathf.Infinity, waterLayerMask))
            {
                Transform hitObject = hit.collider.gameObject.transform;
                if (hitObject != null)
                {
                    Debug.Log("Hit Seagull");
                }

                ;
                // Check if the hit object is a seagull
                if (hitObject.CompareTag("Seagull"))
                {
                    var targetHit = hitObject;
                    Debug.Log($"Seagull hit: {hitObject.name}");
                    //_soundManager.PlayAudio("/Suoni/Gabbiano Death");

                    _seagullManager.ResetSeagull();
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