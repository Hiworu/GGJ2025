using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class CreatorWindow : EditorWindow
{
    //CUSTOMERS
    private List<CustomerSO> customers = new List<CustomerSO>();
    private CustomerSO customerPreviousSelected;
    private CustomerSO customerSelected;
    
    //BUBBLES
    private List<BubbleSO> bubbles = new List<BubbleSO>();
    private BubbleSO bubblePreviousSelected;
    private BubbleSO bubbleSelected;
    
    //SYRUPS
    private List<SyrupSO> syrups = new List<SyrupSO>();
    private SyrupSO syrupPreviousSelected;
    private SyrupSO syrupSelected;
    
    //TOPPINGS
    private List<ToppingSO> toppings = new List<ToppingSO>();
    private ToppingSO toppingPreviousSelected;
    private ToppingSO toppingSelected;

    
    //all
    private string currentName;

    private Sprite currentIcon;
   
    private string newName; 
    private string newDescription;
    private string newContents;
    private Sprite newIcon;
   
    //column settings
    private int lengthWidth = 130;

    private Vector2 scrollCenter;
    private Vector2 scrollRight;
    private Vector2 scrollDetails;
    private CreatorWindow.TabType currentTab = CreatorWindow.TabType.NoTab;
    private enum TabType
    {
        NoTab,
        Customers,
        Bubbles,
        Syrups,
        Toppings,
    }
    [MenuItem("Window/SO Editor")]
    public static void ShowWindow()
    { GetWindow<CreatorWindow>("SO Editor"); }

    public void OnEnable()
    {
       LoadCustomers();
       LoadBubbles();
       LoadSyrups();
       LoadToppings();
    }

   private void OnGUI()
   {
      EditorGUILayout.BeginHorizontal();
      
      //LEFT COLUMN
      EditorGUILayout.BeginVertical(GUILayout.Width(position.width/16));
      if (GUILayout.Button("Show Customers"))          { currentTab = CreatorWindow.TabType.Customers; }
      if (GUILayout.Button("Show Bubbles"))            { currentTab = CreatorWindow.TabType.Bubbles; }
      if (GUILayout.Button("Show Syrups"))            { currentTab = CreatorWindow.TabType.Syrups; }
      if (GUILayout.Button("Show Toppings"))            { currentTab = CreatorWindow.TabType.Toppings; }

      EditorGUILayout.EndVertical();
      
      //CENTER COLUMN
      EditorGUILayout.BeginVertical(GUILayout.Width(position.width/4));
      scrollCenter = EditorGUILayout.BeginScrollView(scrollCenter);
      ShowSelectedList();                                                                                               //show selected list (left column) in center column
      EditorGUILayout.EndScrollView();
      EditorGUILayout.EndVertical();
      
      //RIGHT COLUMN
      EditorGUILayout.BeginVertical(GUILayout.Width(position.width/2));
      ShowSelectedDetails();                                                                                            //show selected object
      EditorGUILayout.EndVertical();
      EditorGUILayout.EndHorizontal();
      
      EditorGUILayout.Space();
      EditorGUILayout.BeginHorizontal();                                                                                //button changes depending on list selected
      if(GUILayout.Button("Add"))
      {
         switch (currentTab)
         {
            case CreatorWindow.TabType.Customers:        CreateCustomer();
               SaveCustomer(customerSelected, newName, newIcon);        break;
            case CreatorWindow.TabType.Bubbles:        CreateBubble();
               SaveBubble(bubbleSelected, newName, newIcon);        break;
            case CreatorWindow.TabType.Syrups:        CreateSyrup();
               SaveSyrup(syrupSelected, newName, newIcon);        break;
            case CreatorWindow.TabType.Toppings:        CreateTopping();
               SaveTopping(toppingSelected, newName, newIcon);        break;
            
         }
      }                                                                             
      if (GUILayout.Button("Delete"))
      {
         switch (currentTab)
         {
            case CreatorWindow.TabType.Customers:        DeleteCustomer();        break;
            case CreatorWindow.TabType.Bubbles:        DeleteBubble();        break;
            case CreatorWindow.TabType.Syrups:        DeleteSyrup();        break;
            case CreatorWindow.TabType.Toppings:        DeleteTopping();        break;



         }
      }                                                                         
      if (GUILayout.Button("Save"))
      {
         switch (currentTab)
         {
            case CreatorWindow.TabType.Customers:        SaveCustomer(customerSelected, newName, newIcon);  break;
            case CreatorWindow.TabType.Bubbles:        SaveBubble(bubbleSelected, newName, newIcon);  break;
            case CreatorWindow.TabType.Syrups:        SaveSyrup(syrupSelected, newName, newIcon);  break;
            case CreatorWindow.TabType.Toppings:        SaveTopping(toppingSelected, newName, newIcon);  break;



         }
      }
      EditorGUILayout.EndHorizontal();
   }

   //SHOW SELECTED
   private void ShowSelectedList()
   {
      switch (currentTab)
      {
         case CreatorWindow.TabType.Customers:
            LoadCustomers();
            foreach (var customer in customers)
            { if (GUILayout.Button(customer.name)) { customerSelected = customer; } } break;
         case CreatorWindow.TabType.Bubbles:
            LoadBubbles();
            foreach (var bubble in bubbles)
            { if (GUILayout.Button(bubble.name)) { bubbleSelected = bubble; } } break;
         case CreatorWindow.TabType.Syrups:
            LoadSyrups();
            foreach (var syrup in syrups)
            { if (GUILayout.Button(syrup.name)) { syrupSelected = syrup; } } break;
         case CreatorWindow.TabType.Toppings:
            LoadToppings();
            foreach (var topping in toppings)
            { if (GUILayout.Button(topping.name)) { toppingSelected = topping; } } break;
         
         
         case CreatorWindow.TabType.NoTab: 
         {EditorGUILayout.HelpBox("No list selected", MessageType.Info); }                    break; 
      }
   }
   private void ShowSelectedDetails()
   {
      switch (currentTab)
      {
         case CreatorWindow.TabType.Customers:           ShowSelectedCustomer();           break;
         case CreatorWindow.TabType.Bubbles:           ShowSelectedBubble();           break;
         case CreatorWindow.TabType.Syrups:           ShowSelectedSyrup();           break;
         case CreatorWindow.TabType.Toppings:           ShowSelectedToppings();           break;
         default: 
            EditorGUILayout.HelpBox("No list selected", MessageType.Info); 
            break;
      }
   }
   private void ShowSelectedCustomer()
   {
      if (customerSelected == null) { return; }
      if (customerSelected != customerPreviousSelected)
      {
         newName = string.Empty;
         newIcon = customerSelected.sprite;
         customerPreviousSelected = customerSelected;
      }
      currentName = customerSelected.name;
      currentIcon = customerSelected.sprite;

      EditorGUILayout.LabelField("Customer Details", EditorStyles.boldLabel);
      
      //NAME
      GUILayout.BeginHorizontal();
      GUILayout.Label("Current Name: ", GUILayout.Width(lengthWidth));
      GUILayout.Label(customerSelected.name);
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal();
      GUILayout.Label("Rename:", GUILayout.Width(lengthWidth));
      newName = 
         EditorGUILayout.TextField(string.IsNullOrEmpty(newName) ? "" : newName, GUILayout.Width(150));
      GUILayout.EndHorizontal();
      GUILayout.Space(10);
      
      //TOPPING LIST
      EditorGUILayout.BeginHorizontal();
      GUILayout.Label("Toppings:", GUILayout.Width(lengthWidth));
      EditorGUILayout.BeginVertical();
      if (customerSelected.Toppings == null) 
      { customerSelected.Toppings = new List<ToppingSO>(); }

      for (int i = 0; i < customerSelected.Toppings.Count; i++)                                                      //show drops
      {
         EditorGUILayout.BeginHorizontal();
         customerSelected.Toppings[i] = 
            (ToppingSO)EditorGUILayout.ObjectField
            (customerSelected.Toppings[i], typeof(ToppingSO), false, 
               GUILayout.Width(lengthWidth)
            );
         if (GUILayout.Button("X", GUILayout.Width((20))))
         {
            customerSelected.Toppings.RemoveAt(i);
            i--; 
            continue;
         }
         EditorGUILayout.EndHorizontal();
      }
      EditorGUILayout.EndVertical();
      EditorGUILayout.EndHorizontal();
      EditorGUILayout.Space(10);
      if (GUILayout.Button("Add", GUILayout.Width(lengthWidth)))
      {
         customerSelected.Toppings.Add(null);
         EditorUtility.SetDirty(customerSelected);
      }
      GUILayout.Space(20);
      
      //BUBBLE LIST
      EditorGUILayout.BeginHorizontal();
      GUILayout.Label("Bubbles:", GUILayout.Width(lengthWidth));
      EditorGUILayout.BeginVertical();
      if (customerSelected.Bubbles == null) 
      { customerSelected.Bubbles = new List<BubbleSO>(); }

      for (int i = 0; i < customerSelected.Bubbles.Count; i++)                                                      //show drops
      {
         EditorGUILayout.BeginHorizontal();
         customerSelected.Bubbles[i] = 
            (BubbleSO)EditorGUILayout.ObjectField
            (customerSelected.Bubbles[i], typeof(BubbleSO), false, 
               GUILayout.Width(lengthWidth)
            );
         if (GUILayout.Button("X", GUILayout.Width((20))))
         {
            customerSelected.Bubbles.RemoveAt(i);
            i--; 
            continue;
         }
         EditorGUILayout.EndHorizontal();
      }
      EditorGUILayout.EndVertical();
      EditorGUILayout.EndHorizontal();
      EditorGUILayout.Space(10);
      if (GUILayout.Button("Add", GUILayout.Width(lengthWidth)))
      {
         customerSelected.Bubbles.Add(null);
         EditorUtility.SetDirty(customerSelected);
      }
      GUILayout.Space(20);
      
      //SYRUP LIST
      EditorGUILayout.BeginHorizontal();
      GUILayout.Label("Syrups:", GUILayout.Width(lengthWidth));
      EditorGUILayout.BeginVertical();
      if (customerSelected.Syrups == null) 
      { customerSelected.Syrups = new List<SyrupSO>(); }

      for (int i = 0; i < customerSelected.Syrups.Count; i++)                                                      //show drops
      {
         EditorGUILayout.BeginHorizontal();
         customerSelected.Syrups[i] = 
            (SyrupSO)EditorGUILayout.ObjectField
            (customerSelected.Syrups[i], typeof(SyrupSO), false, 
               GUILayout.Width(lengthWidth)
            );
         if (GUILayout.Button("X", GUILayout.Width((20))))
         {
            customerSelected.Syrups.RemoveAt(i);
            i--; 
            continue;
         }
         EditorGUILayout.EndHorizontal();
      }
      EditorGUILayout.EndVertical();
      EditorGUILayout.EndHorizontal();
      EditorGUILayout.Space(10);
      if (GUILayout.Button("Add", GUILayout.Width(lengthWidth)))
      {
         customerSelected.Syrups.Add(null);
         EditorUtility.SetDirty(customerSelected);
      }
      GUILayout.Space(20);
      
      
      //SPRITE
      var sprite = customerSelected.sprite;
      if (sprite == null)
      {
         if (GUILayout.Button("Select Sprite", GUILayout.Width(EditorGUIUtility.currentViewWidth/6),
                GUILayout.Height(EditorGUIUtility.currentViewWidth/6)))
         {
            string path = EditorUtility.OpenFilePanel
               ("Select Sprite", "Assets/Resources/Imports/PNGs", "png,jpg,asset");                      
            if (!string.IsNullOrEmpty(path))                                                                            //convert to asset path
            {
               string assetPath = FileUtil.GetProjectRelativePath(path);
               Sprite selectedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
               if (selectedSprite != null)
               {
                  newIcon = selectedSprite;
               }
            }
         }
      }
      else if (GUILayout.Button(new GUIContent(sprite.texture),
                  GUILayout.Width(EditorGUIUtility.currentViewWidth/6),
                  GUILayout.Height(EditorGUIUtility.currentViewWidth/6)))
      {
         string path = EditorUtility.OpenFilePanel
            ("Select Sprite", "Assets/Resources/Imports/PNGs", "png,jpg,asset");
         if (!string.IsNullOrEmpty(path))                                                                               //convert to asset path
         {
            string assetPath = FileUtil.GetProjectRelativePath(path);
            Sprite selectedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (selectedSprite != null)
            {
               customerSelected.sprite = selectedSprite;
               EditorUtility.SetDirty(customerSelected);
            }
         }
      }
   }
    private void ShowSelectedBubble()
   {
      if (bubbleSelected == null) { return; }
      if (bubbleSelected != bubblePreviousSelected)
      {                  
         newName = string.Empty;
         newIcon = bubbleSelected.sprite;
         bubblePreviousSelected = bubbleSelected;  
      }
      currentName = bubbleSelected.name;
      currentIcon = bubbleSelected.sprite;
      
      EditorGUILayout.LabelField("Bubble Details", EditorStyles.boldLabel);
      
      //NAME
      GUILayout.BeginHorizontal();
      GUILayout.Label("Current Name: ", GUILayout.Width(lengthWidth)); 
      GUILayout.Label(bubbleSelected.name);
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal();
      GUILayout.Label("Rename:", GUILayout.Width(lengthWidth));
      newName = 
         EditorGUILayout.TextField(string.IsNullOrEmpty(newName) ? "" : newName, GUILayout.Width(150));
      GUILayout.EndHorizontal();
      GUILayout.Space(10);
      
      

      //SPRITE
      var sprite = bubbleSelected.sprite;
      if (sprite == null)
      {
         if (GUILayout.Button("Select Sprite", GUILayout.Width(EditorGUIUtility.currentViewWidth/6),
                GUILayout.Height(EditorGUIUtility.currentViewWidth/6)))
         {
            string path = EditorUtility.OpenFilePanel
               ("Select Sprite", "Assets/Resources/Imports/PNGs", "png,jpg,asset");                      
            if (!string.IsNullOrEmpty(path))                                                                            //convert to asset path
            {
               string assetPath = FileUtil.GetProjectRelativePath(path);
               Sprite selectedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
               if (selectedSprite != null)
               {
                  newIcon = selectedSprite;
               }
            }
         }
      }
      else if (GUILayout.Button(new GUIContent(sprite.texture),
                  GUILayout.Width(EditorGUIUtility.currentViewWidth/6),
                  GUILayout.Height(EditorGUIUtility.currentViewWidth/6)))
      {
         string path = EditorUtility.OpenFilePanel
            ("Select Sprite", "Assets/Resources/Imports/PNGs", "png,jpg,asset");
         if (!string.IsNullOrEmpty(path))                                                                               //convert to asset path
         {
            string assetPath = FileUtil.GetProjectRelativePath(path);
            Sprite selectedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (selectedSprite != null)
            {
               customerSelected.sprite = selectedSprite;
               EditorUtility.SetDirty(customerSelected);
            }
         }
      }
   }
    private void ShowSelectedSyrup()
   {
      if (syrupSelected == null) { return; }
      if (syrupSelected != syrupPreviousSelected)
      {                  
         newName = string.Empty;
         newIcon = syrupSelected.sprite;
         syrupPreviousSelected = syrupSelected;  
      }
      currentName = syrupSelected.name;
      currentIcon = syrupSelected.sprite;
      
      EditorGUILayout.LabelField("Syrup Details", EditorStyles.boldLabel);
      
      //NAME
      GUILayout.BeginHorizontal();
      GUILayout.Label("Current Name: ", GUILayout.Width(lengthWidth)); 
      GUILayout.Label(syrupSelected.name);
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal();
      GUILayout.Label("Rename:", GUILayout.Width(lengthWidth));
      newName = 
         EditorGUILayout.TextField(string.IsNullOrEmpty(newName) ? "" : newName, GUILayout.Width(150));
      GUILayout.EndHorizontal();
      GUILayout.Space(10);
      
      
      //SPRITE
      var sprite = syrupSelected.sprite;
      if (sprite == null)
      {
         if (GUILayout.Button("Select Sprite", GUILayout.Width(EditorGUIUtility.currentViewWidth/6),
                GUILayout.Height(EditorGUIUtility.currentViewWidth/6)))
         {
            string path = EditorUtility.OpenFilePanel
               ("Select Sprite", "Assets/Resources/Imports/PNGs", "png,jpg,asset");                      
            if (!string.IsNullOrEmpty(path))                                                                            //convert to asset path
            {
               string assetPath = FileUtil.GetProjectRelativePath(path);
               Sprite selectedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
               if (selectedSprite != null)
               {
                  newIcon = selectedSprite;
               }
            }
         }
      }
      else if (GUILayout.Button(new GUIContent(sprite.texture),
                  GUILayout.Width(EditorGUIUtility.currentViewWidth/6),
                  GUILayout.Height(EditorGUIUtility.currentViewWidth/6)))
      {
         string path = EditorUtility.OpenFilePanel
            ("Select Sprite", "Assets/Resources/Imports/PNGs", "png,jpg,asset");
         if (!string.IsNullOrEmpty(path))                                                                               //convert to asset path
         {
            string assetPath = FileUtil.GetProjectRelativePath(path);
            Sprite selectedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (selectedSprite != null)
            {
               customerSelected.sprite = selectedSprite;
               EditorUtility.SetDirty(customerSelected);
            }
         }
      }
   }
     private void ShowSelectedToppings()
   {
      if (toppingSelected == null) { return; }
      if (toppingSelected != toppingPreviousSelected)
      {                  
         newName = string.Empty;
         newIcon = toppingSelected.sprite;
         toppingPreviousSelected = toppingSelected;  
      }
      currentName = toppingSelected.name;
      currentIcon = toppingSelected.sprite;
      
      EditorGUILayout.LabelField("Topping Details", EditorStyles.boldLabel);
      
      //NAME
      GUILayout.BeginHorizontal();
      GUILayout.Label("Current Name: ", GUILayout.Width(lengthWidth)); 
      GUILayout.Label(toppingSelected.name);
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal();
      GUILayout.Label("Rename:", GUILayout.Width(lengthWidth));
      newName = 
         EditorGUILayout.TextField(string.IsNullOrEmpty(newName) ? "" : newName, GUILayout.Width(150));
      GUILayout.EndHorizontal();
      GUILayout.Space(10);
      
     
      //SPRITE
      var sprite = toppingSelected.sprite;
      if (sprite == null)
      {
         if (GUILayout.Button("Select Sprite", GUILayout.Width(EditorGUIUtility.currentViewWidth/6),
                GUILayout.Height(EditorGUIUtility.currentViewWidth/6)))
         {
            string path = EditorUtility.OpenFilePanel
               ("Select Sprite", "Assets/Resources/Imports/PNGs", "png,jpg,asset");                      
            if (!string.IsNullOrEmpty(path))                                                                            //convert to asset path
            {
               string assetPath = FileUtil.GetProjectRelativePath(path);
               Sprite selectedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
               if (selectedSprite != null)
               {
                  newIcon = selectedSprite;
               }
            }
         }
      }
      else if (GUILayout.Button(new GUIContent(sprite.texture),
                  GUILayout.Width(EditorGUIUtility.currentViewWidth/6),
                  GUILayout.Height(EditorGUIUtility.currentViewWidth/6)))
      {
         string path = EditorUtility.OpenFilePanel
            ("Select Sprite", "Assets/Resources/Imports/PNGs", "png,jpg,asset");
         if (!string.IsNullOrEmpty(path))                                                                               //convert to asset path
         {
            string assetPath = FileUtil.GetProjectRelativePath(path);
            Sprite selectedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (selectedSprite != null)
            {
               customerSelected.sprite = selectedSprite;
               EditorUtility.SetDirty(customerSelected);
            }
         }
      }
   }
   
   
   
   
   
     //EDIT CUSTOMERS
   private void LoadCustomers()
   { customers = new List<CustomerSO>(Resources.LoadAll<CustomerSO>("")); }
   private void CreateCustomer()
   {
      var newCustomer = CreateInstance<CustomerSO>();
      string folderPath = "Assets/Resources/Customers";
      
      var assetGUIDs = AssetDatabase.FindAssets("t:CustomerSO", new[] { folderPath });
      var existingNames = assetGUIDs
         .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
         .Select(path => System.IO.Path.GetFileNameWithoutExtension(path))
         .ToArray();
      string uniqueName = ObjectNames.GetUniqueName(existingNames, "New Customer");
      string assetPath = $"{folderPath}/{uniqueName}.asset";
     
      AssetDatabase.CreateAsset(newCustomer, assetPath);
      AssetDatabase.Refresh();
      newCustomer.name = uniqueName;
      customers.Add(newCustomer); 
      customerSelected = newCustomer;
      currentName = newCustomer.name;
      EditorUtility.SetDirty(newCustomer);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
   }
   private void DeleteCustomer()
   {
      if (customerSelected == null)  { return; }
      customers.Remove(customerSelected);
      string assetPath = AssetDatabase.GetAssetPath(customerSelected);
      AssetDatabase.DeleteAsset(assetPath);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      customerSelected = null;
   }
   private void SaveCustomer(CustomerSO customer, string newName, Sprite newIcon)
   {
      if (string.IsNullOrEmpty(newName))        { newName = currentName ?? "New Customer"; }
      if (newIcon == null)                   { newIcon = currentIcon ?? null; }

      string assetPath = AssetDatabase.GetAssetPath(customerSelected);
      AssetDatabase.RenameAsset(assetPath, newName);
      customer.name = newName;
      customer.sprite = newIcon;
      EditorUtility.SetDirty(customerSelected);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      
      this.newName = string.Empty;
      this.newIcon = null;
   }
   
   //EDIT BUBBLES
    private void LoadBubbles()
   { bubbles = new List<BubbleSO>(Resources.LoadAll<BubbleSO>("")); }
   private void CreateBubble()
   {
      var newBubble = CreateInstance<BubbleSO>();
      string folderPath = "Assets/Resources/Bubbles";
      
      var assetGUIDs = AssetDatabase.FindAssets("t:BubbleSO", new[] { folderPath });
      var existingNames = assetGUIDs
         .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
         .Select(path => System.IO.Path.GetFileNameWithoutExtension(path))
         .ToArray();
      string uniqueName = ObjectNames.GetUniqueName(existingNames, "New Bubble");
      string assetPath = $"{folderPath}/{uniqueName}.asset";
     
      AssetDatabase.CreateAsset(newBubble, assetPath);
      AssetDatabase.Refresh();
      newBubble.name = uniqueName;
      bubbles.Add(newBubble); 
      bubbleSelected = newBubble;
      currentName = newBubble.name;
      EditorUtility.SetDirty(newBubble);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
   }
   private void DeleteBubble()
   {
      if (bubbleSelected == null)  { return; }
      bubbles.Remove(bubbleSelected);
      string assetPath = AssetDatabase.GetAssetPath(bubbleSelected);
      AssetDatabase.DeleteAsset(assetPath);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      bubbleSelected = null;
   }
   private void SaveBubble(BubbleSO bubble, string newName, Sprite newIcon)
   {
      if (string.IsNullOrEmpty(newName))        { newName = currentName ?? "New Bubble"; }

      if (newIcon == null)                   { newIcon = currentIcon ?? null; }

      string assetPath = AssetDatabase.GetAssetPath(bubbleSelected);
      AssetDatabase.RenameAsset(assetPath, newName);
      bubble.name = newName;

      bubble.sprite = newIcon;
      EditorUtility.SetDirty(bubbleSelected);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      
      this.newName = string.Empty;
      this.newDescription = string.Empty;
      this.newContents = string.Empty;
      this.newIcon = null;
   }
   
    //EDIT SYRUPS
   private void LoadSyrups()
   { syrups = new List<SyrupSO>(Resources.LoadAll<SyrupSO>("")); }
   private void CreateSyrup()
   {
      var newSyrup = CreateInstance<SyrupSO>();
      string folderPath = "Assets/Resources/Syrups";
      
      var assetGUIDs = AssetDatabase.FindAssets("t:SyrupSO", new[] { folderPath });
      var existingNames = assetGUIDs
         .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
         .Select(path => System.IO.Path.GetFileNameWithoutExtension(path))
         .ToArray();
      string uniqueName = ObjectNames.GetUniqueName(existingNames, "New Syrup");
      string assetPath = $"{folderPath}/{uniqueName}.asset";
      
      AssetDatabase.CreateAsset(newSyrup, assetPath);
      AssetDatabase.Refresh();  
      newSyrup.name = uniqueName;
      syrups.Add(newSyrup);
      syrupSelected = newSyrup;
      currentName = newSyrup.name;
      EditorUtility.SetDirty(newSyrup);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();  
   }
   private void DeleteSyrup()
   {
      if (syrupSelected == null)  { return; }
      syrups.Remove(syrupSelected);
      string assetPath = AssetDatabase.GetAssetPath(syrupSelected);
      AssetDatabase.DeleteAsset(assetPath);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      syrupSelected = null;
   }
   private void SaveSyrup(SyrupSO syrup, string newName, Sprite newIcon)
   {
      if (string.IsNullOrEmpty(newName))        { newName = currentName ?? "New Syrup"; }
      if (newIcon == null)                   { newIcon = currentIcon ?? null; }

      string assetPath = AssetDatabase.GetAssetPath(syrupSelected);
      AssetDatabase.RenameAsset(assetPath, newName);
      syrup.name = newName;
      syrup.sprite = newIcon;
      EditorUtility.SetDirty(syrupSelected);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      
      this.newName = string.Empty;
      this.newDescription = string.Empty;
      this.newIcon = null;
   }
   
    //EDIT TOPPINGS
   private void LoadToppings()
   { toppings = new List<ToppingSO>(Resources.LoadAll<ToppingSO>("")); }
   private void CreateTopping()
   {
      var newTopping = CreateInstance<ToppingSO>();
      string folderPath = "Assets/Resources/Toppings";
      
      var assetGUIDs = AssetDatabase.FindAssets("t:ToppingSO", new[] { folderPath });
      var existingNames = assetGUIDs
         .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
         .Select(path => System.IO.Path.GetFileNameWithoutExtension(path))
         .ToArray();
      string uniqueName = ObjectNames.GetUniqueName(existingNames, "New Topping");
      string assetPath = $"{folderPath}/{uniqueName}.asset";
      
      AssetDatabase.CreateAsset(newTopping, assetPath);
      AssetDatabase.Refresh();  
      newTopping.name = uniqueName;
      toppings.Add(newTopping);
      toppingSelected = newTopping;
      currentName = newTopping.name;
      EditorUtility.SetDirty(newTopping);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();  
   }
   private void DeleteTopping()
   {
      if (toppingSelected == null)  { return; }
      toppings.Remove(toppingSelected);
      string assetPath = AssetDatabase.GetAssetPath(toppingSelected);
      AssetDatabase.DeleteAsset(assetPath);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      toppingSelected = null;
   }
   private void SaveTopping(ToppingSO topping, string newName, Sprite newIcon)
   {
      if (string.IsNullOrEmpty(newName))        { newName = currentName ?? "New Ingredient"; }
      if (newIcon == null)                   { newIcon = currentIcon ?? null; }

      string assetPath = AssetDatabase.GetAssetPath(toppingSelected);
      AssetDatabase.RenameAsset(assetPath, newName);
      topping.name = newName;
      topping.sprite = newIcon;
      EditorUtility.SetDirty(toppingSelected);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      
      this.newName = string.Empty;
      this.newDescription = string.Empty;
      this.newIcon = null;
   }


}
