using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using System.IO;

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
        if (customer.Bubbles == null) { customer.Bubbles = new List<BubbleSO>(); }
        if (customer.Bubbles == null || customer.Bubbles.Count == 0)
        { customer.Bubbles.Add(RandomFromResources<BubbleSO>("Bubbles")); }
        if (customer.Syrups == null) { customer.Syrups = new List<SyrupSO>(); }
        if (customer.Syrups == null || customer.Syrups.Count == 0)
        { customer.Syrups.Add(RandomFromResources<SyrupSO>("Syrups")); }
        if (customer.Toppings == null) { customer.Toppings = new List<ToppingSO>(); }
        if (customer.Toppings == null || customer.Toppings.Count == 0)
        { customer.Toppings.Add(RandomFromResources<ToppingSO>("Toppings")); }
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
    }

    private void Update()
    {
        if (this.gameObject != null)
        {
            //timer
            _currentTime += Time.deltaTime;
            if (CompareChoices()) { _isOrderCompleted = true; }

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

    private bool CompareChoices()
    {
        if (!CompareLists(customer.Bubbles, _bubbleTeaManager.selectedBubbles)) return false;
        if (!CompareLists(customer.Syrups, _bubbleTeaManager.selectedSyrups)) return false;
        if (!CompareLists(customer.Toppings, _bubbleTeaManager.selectedToppings)) return false;
        return true;
    }
    private bool CompareLists<T>(List<T> list1, List<T> list2) where T : Object
    {
        if (list1.Count != list2.Count) return false;

        for (int i = 0; i < list1.Count; i++)
        { if (list1[i] != list2[i]) return false; }
        return true;
    }

    private void CustomerDissatisfied()
    {
        _gameManager.playerHealth -= -1;
        _waveManager.removeCustomer(this.gameObject);
        Debug.Log($"customer removed:{this.gameObject.name}");
    }

    private void CustomerSatisfied()
    {
        _gameManager.cash += customer.CashGiven;
        _waveManager.removeCustomer(this.gameObject);
    }

    private void AddTextures()
    {
        List<GameObject> customerStyles = GetGameObjectsFromRandomFolder("Prefabs/CustomerFeatures");
        GameObject body = customerStyles[0];
        Debug.Log(body);
        GameObject hair = customerStyles[1];
        Debug.Log(hair);
        GameObject shirt = customerStyles[2];
        Debug.Log(shirt);




        if (body != null && hair != null )
        {
            GameObject bodyClone = Instantiate(body, this.transform.position , Quaternion.identity);
            bodyClone.transform.SetParent(this.transform); // Imposta this come genitore
            Debug.Log("Body aggiunto come figlio: " + body.name);

            GameObject hairClone = Instantiate(hair, this.transform.position, Quaternion.identity);
            hairClone.transform.SetParent(this.transform); // Imposta this come genitore
            GameObject hairChild = hairClone.transform.Find("Hair").gameObject;
            Debug.Log(hair);
            spriteRenderer = hairChild.GetComponent<SpriteRenderer>();
            spriteRenderer.color = RandomColorCreator();

            GameObject shirtClone = Instantiate(shirt, this.transform.position, Quaternion.identity);
            shirtClone.transform.SetParent(this.transform); // Imposta this come genitore
            GameObject shirtChild = shirtClone.transform.Find("Shirt").gameObject;
            spriteRenderer = shirtChild.GetComponent<SpriteRenderer>();
            spriteRenderer.color = RandomColorCreator();


            Debug.Log("Hair aggiunto come figlio: " + hair.name);
        }
        else
        {
            Debug.LogWarning("Impossibile aggiungere body o hair come figli: uno o entrambi non sono stati trovati.");
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
