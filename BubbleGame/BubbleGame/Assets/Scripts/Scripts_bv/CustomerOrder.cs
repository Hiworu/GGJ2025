using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using System.IO;
using Unity.VisualScripting;

public class CustomerOrder : MonoBehaviour
{
    [Header("Customer")]
    [SerializeField] private CustomerSO customer;

    [Header("Order")]
    [SerializeField] private float waitTime = 10;

    private GameManagerScript _gameManager;
    private BubbleTeaManager _bubbleTeaManager;
    private SeagullManager _seagullManager;
    private WaveManagerScript _waveManager;

    private SyrupBridge _customerSyrup;
    private BubbleBridge _customerBubble;
    private ToppingBridge _customerTopping;

    public GameObject orderPrefab;
    private GameObject _spritePrefab;
    private GameObject _bobaPrefab;
    private GameObject _syrupPrefab;
    private GameObject _toppingPrefab;
    private GameObject _syrupBGPrefab;

    private bool _isOrderCompleted;
    private float _currentTime;
    private float _currentSeagullTime;
    private SpriteRenderer spriteRenderer;



    private void Awake()
    {
        //get GameManager
        GameObject gameManager = GameObject.Find("GameManager");
        _gameManager = gameManager.GetComponent<GameManagerScript>();
        _bubbleTeaManager = gameManager.GetComponent<BubbleTeaManager>();
        _seagullManager = gameManager.GetComponent<SeagullManager>();
        _waveManager = gameManager.GetComponent<WaveManagerScript>();

        //RANDOMIZED INGREDIENTS IF LIST == NULL
        // customer.Bubbles = new List<BubbleSO>(); customer.Bubbles.Clear();
        // if (customer.Bubbles == null || customer.Bubbles.Count == 0)
        // { customer.Bubbles.Add(RandomFromResources<BubbleSO>("Bubbles")); }
        // customer.Syrups = new List<SyrupSO>(); customer.Syrups.Clear();
        // if (customer.Syrups == null || customer.Syrups.Count == 0)
        // { customer.Syrups.Add(RandomFromResources<SyrupSO>("Syrups")); }
        // customer.Toppings = new List<ToppingSO>(); customer.Toppings.Clear();
        // if (customer.Toppings == null || customer.Toppings.Count == 0)
        // { customer.Toppings.Add(RandomFromResources<ToppingSO>("Toppings")); }




        _bubbleTeaManager.selectedBubble = null;
        _bubbleTeaManager.selectedSyrup = null;
        _bubbleTeaManager.selectedTopping = null;
        _bubbleTeaManager.selectedBubble = RandomFromResources<BubbleSO>("Bubbles");
        _bubbleTeaManager.selectedSyrup = RandomFromResources<SyrupSO>("Syrups");
        _bubbleTeaManager.selectedTopping = RandomFromResources<ToppingSO>("Toppings");

    }

    private T RandomFromResources<T>(string resourceFolder) where T : Object
    {
        T[] assets = Resources.LoadAll<T>(resourceFolder);
        if (assets.Length > 0)
        { return assets[Random.Range(0, assets.Length)]; }
        return null;
    }

    private void Start()
    {
        _currentTime = 0;
        _isOrderCompleted = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        AddTextures();
        this.transform.Rotate(new Vector3(0, -90, 0));
        this.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f); // Uniform scaling
    }

    private void Update()
    {
        if (this.gameObject != null)
        {
            //timer
            _currentTime += Time.deltaTime;

            if (!_isOrderCompleted && _currentTime >= waitTime) { CustomerDissatisfied(); return; }
            if (_seagullManager.IsAttackedBySeagull && _seagullManager.SeagullHasWon)
            {
                Debug.Log("gabbiano ha vinto :(");
                CustomerDissatisfied();
                return;
            }

            if (_isOrderCompleted == true) { CustomerSatisfied(); }
        }

    }

    // private bool CompareChoices()
    // {
    //     if (!CompareLists(customer.Bubbles, _bubbleTeaManager.selectedBubble)) return false;
    //     if (!CompareLists(customer.Syrups, _bubbleTeaManager.selectedSyrup)) return false;
    //     if (!CompareLists(customer.Toppings, _bubbleTeaManager.selectedTopping)) return false;
    //     return true;
    // }


    public bool ValidateOrder(BubbleSO bubble, SyrupSO syrup, ToppingSO topping)
    {
        // bool bubblesMatch = CompareLists(customer.Bubbles, bubbles);
        // bool syrupsMatch = CompareLists(customer.Syrups, syrups);
        // bool toppingsMatch = CompareLists(customer.Toppings, toppings);
        if (_bubbleTeaManager.selectedBubble && syrup == _bubbleTeaManager.selectedSyrup && topping == _bubbleTeaManager.selectedTopping)
        {
            Debug.Log("Customer Satisfied");
            CustomerSatisfied();
            return true;
        }
        else
        {
            Debug.Log("Customer Dissatisfied");

            CustomerDissatisfied();
            return false;
        }
        // if (bubblesMatch && syrupsMatch && toppingsMatch)
        // {
        //     CustomerSatisfied();
        //     return true; // Order is correct
        // }
        // else
        // {
        //     CustomerDissatisfied();
        //     return false; // Order is incorrect
        // }
    }

    public void CustomerDissatisfied()
    {
        _gameManager.playerHealth -=1;
        _waveManager.removeCustomer(this.gameObject);
    }

    public void CustomerSatisfied()
    {
        _gameManager.cash += customer.CashGiven;
        _waveManager.removeCustomer(this.gameObject);
    }

    private void AddTextures()
    {
        List<GameObject> customerStyles = GetGameObjectsFromRandomFolder("Prefabs/CustomerFeatures");
        GameObject body = customerStyles[0];
        //Debug.Log(body);
        GameObject hair = customerStyles[1];
        //Debug.Log(hair);
        GameObject shirt = customerStyles[2];
        //Debug.Log(shirt);

        //GameObject order = Resources.Load<GameObject>("Prefabs/OrderBubble");

        //order
        //Sprite boba = GetComponent<SpriteRenderer>().sprite;
        GameObject order = Instantiate(orderPrefab, this.transform.position, Quaternion.identity);
        order.transform.SetParent(this.transform);
        _syrupPrefab = orderPrefab.transform.Find("Syrup")?.gameObject;
        _spritePrefab = orderPrefab.transform.Find("Sprite")?.gameObject;
        _bobaPrefab = orderPrefab.transform.Find("Boba")?.gameObject;
        _toppingPrefab = orderPrefab.transform.Find("Topping")?.gameObject;
        _syrupBGPrefab = orderPrefab.transform.Find("SyrupBG")?.gameObject;

        if (body != null && hair != null)
        {
            GameObject bodyClone = Instantiate(body, this.transform.position, Quaternion.identity);
            bodyClone.transform.SetParent(this.transform); // Imposta this come genitore
            //Debug.Log("Body aggiunto come figlio: " + body.name);

            GameObject hairClone = Instantiate(hair, this.transform.position, Quaternion.identity);
            hairClone.transform.SetParent(this.transform); // Imposta this come genitore
            GameObject hairChild = hairClone.transform.Find("Hair").gameObject;
            //Debug.Log(hair);
            spriteRenderer = hairChild.GetComponent<SpriteRenderer>();
            spriteRenderer.color = RandomColorCreator();

            GameObject shirtClone = Instantiate(shirt, this.transform.position, Quaternion.identity);
            shirtClone.transform.SetParent(this.transform); // Imposta this come genitore
            GameObject shirtChild = shirtClone.transform.Find("Shirt").gameObject;
            spriteRenderer = shirtChild.GetComponent<SpriteRenderer>();
            spriteRenderer.color = RandomColorCreator();

            //order
            // order = Instantiate(order, this.transform.position, Quaternion.identity);
            // order.transform.SetParent(this.transform); // Imposta this come genitore
            // SpriteRenderer bobaSpriteRenderer = order.transform.Find("Boba").GetComponent<SpriteRenderer>();
            // bobaSpriteRenderer.sprite = boba;



            //Debug.Log("Hair aggiunto come figlio: " + hair.name);
        }
        else
        {
            Debug.LogWarning("Impossibile aggiungere body o hair come figli: uno o entrambi non sono stati trovati.");
        }

        if (_syrupPrefab != null && _spritePrefab != null && _bobaPrefab != null && _toppingPrefab != null)
        {
            if (_spritePrefab != null)
            {
                SpriteRenderer spriteBobaRenderer = _spritePrefab.GetComponent<SpriteRenderer>();
                spriteBobaRenderer.sprite = _spritePrefab.GetComponent<SpriteRenderer>().sprite;
            }
            else
            {
                Debug.LogError("_spritePrefab is not assigned!");
            }

            SpriteRenderer syrupRenderer = _syrupPrefab.GetComponent<SpriteRenderer>();
            syrupRenderer.sprite = _bubbleTeaManager.selectedSyrup.sprite; // Assuming selectedSyrup has a Sprite property
            SpriteRenderer bobaRenderer = _bobaPrefab.GetComponent<SpriteRenderer>();
            bobaRenderer.sprite = _bubbleTeaManager.selectedBubble.sprite; // Assuming selectedBubble has a Sprite property
            SpriteRenderer toppingRenderer = _toppingPrefab.GetComponent<SpriteRenderer>();
            toppingRenderer.sprite = _bubbleTeaManager.selectedTopping.sprite; // Assuming selectedTopping has a Sprite property
            SpriteRenderer syrupBGRenderer = _syrupBGPrefab.GetComponent<SpriteRenderer>();
            syrupBGRenderer.sprite = _bubbleTeaManager.selectedSyrup.sprite; // Assuming selectedTopping has a Sprite property

        }
    }

    public Color RandomColorCreator()
    {
        float r = Random.Range(0f, 1f); // Valore casuale per il rosso
        float g = Random.Range(0f, 1f); // Valore casuale per il verde
        float b = Random.Range(0f, 1f); // Valore casuale per il blu
        return new Color(r, g, b);
    }

    public List<GameObject> GetGameObjectsFromRandomFolder(string baseFolderPath)
    {
        // Ottieni tutte le sottocartelle nel percorso base (relativo a Resources)
        string resourcesPath = Path.Combine(Application.dataPath, "Resources", baseFolderPath);
        string[] subFolders = Directory.GetDirectories(resourcesPath);

        if (subFolders.Length > 0)
        {
            // Scegli una cartella casuale
            int randomIndex = Random.Range(0, subFolders.Length);
            string randomFolder = subFolders[randomIndex];

            // Ottieni il nome della cartella (ad esempio, "Type1")
            string folderName = Path.GetFileName(randomFolder);

            // Costruisci il percorso relativo a Resources
            string relativePath = Path.Combine(baseFolderPath, folderName);

            // Carica tutti i GameObject dalla cartella casuale
            GameObject[] gameObjects = Resources.LoadAll<GameObject>(relativePath);

            if (gameObjects.Length > 0)
            {
                // Converti l'array in una lista e restituiscila
                return new List<GameObject>(gameObjects);
            }
            else
            {
                Debug.LogWarning($"Nessun GameObject trovato nella cartella: {relativePath}");
                return null;
            }
        }
        else
        {
            Debug.LogWarning($"Nessuna sottocartella trovata nel percorso: {baseFolderPath}");
            return null;
        }
    }

}
