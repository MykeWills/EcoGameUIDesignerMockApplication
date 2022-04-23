using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;

public class StoreSystem : MonoBehaviour
{
    //========================================================
    //STORE WINDOW FIELDS
    //========================================================
    private enum ItemCat { green, blue, red, grey}
    private bool[] currentWindowOpen = new bool[3] { false, false, false };
    [Header("Store Window Assignment")]
    [SerializeField]
    private Image storeBackground;
    [SerializeField]
    private RectTransform PurchasePanel;
    [SerializeField]
    private Text purchaseHeader;
    [SerializeField]
    private float fadeTransitionTime = 1;
    [SerializeField]
    private float windowTransitionTime = 1;
    [SerializeField]
    private Scrollbar[] scrollbars = new Scrollbar[2];
    [SerializeField]
    private Image ekeyUI;
    [SerializeField]
    private Text messageText;
    //========================================================
    //RECEIPT WINDOW FIELDS
    //========================================================
    [Header("Receipt Window Assignment")]
    [Space]

    [SerializeField]
    private GameObject receiptWindow;
    [SerializeField]
    private Color[] itemCategoryColors = new Color[4];

    [SerializeField]
    private Image[] receiptWindowIElements;
    [SerializeField]
    private Text[] receiptWindowTElements;
    [SerializeField]
    private Text receiptTotalCreditText;

    [SerializeField]
    private Transform slotPool;
    [SerializeField]
    private GameObject slotPrefab;

    private List<Sprite> slotItemSprites = new List<Sprite>(); 
    private List<string> slotItemNames = new List<string>();
    private List<int> slotItemValues = new List<int>();
    [SerializeField]
    private List<int> slotItemQuanities = new List<int>();
    private List<int> slotItemCategories = new List<int>();
    [SerializeField]
    private List<int> slotItemPosition = new List<int>();

    private int dividedPoint = 0;
    private int totalSlots = 0;
    private int totalCredits = 0;
    [SerializeField]
    private float receiptTransactionTime = 1;
    private float receiptTransactionTimer = 0;
    private bool receiptOpen = false;
    private bool receiptEffect = false;
    private float[] receiptAlphasI = new float[8] 
    { 
        1, 0.5f, 1, 1, 1, 0.5f, 1, 1
    };
    [Space]
    //========================================================
    //INVALID WINDOW FIELDS
    //========================================================
    [Header("Invalid Window")]
    [SerializeField]
    private GameObject invalidWindow;
    [SerializeField]
    private Image[] invalidWindowIElements;
    [SerializeField]
    private Text[] invalidWindowTElements;
    [SerializeField]
    private float invalidTransactionTime = 1;
    private float invalidTransactionTimer = 0;
    private bool invalidOpen = false;
    private bool invalidEffect = false;
    private float[] invalidAlphasI = new float[4]
    {
        0.9f, 1, 1, 1
    };
    //========================================================
    //PLAYER INVENTORY FIELDS
    //========================================================
    [Space]
    [Header("Player Inventory")]
    [SerializeField]
    private ItemCat[] playerItemCat = new ItemCat[11];
    [SerializeField]
    private Sprite[] playerStockSprites = new Sprite[11];
    [SerializeField]
    private Text[] playerStockValueTexts = new Text[11];
    [SerializeField]
    private Text[] playerStockQuanityTexts = new Text[11];
    [SerializeField]
    private InputField[] playerInput;
    [SerializeField]
    private int[] playerStockValue = new int[11];
    [SerializeField]
    private int[] playerStockQuanity = new int[11];
    [SerializeField]
    private Text playerCreditText;
    private int[] playerSellQuanity = new int[11];
    private int playerCredits = 1636;
    //========================================================
    //STORE OWNER INVENTORY FIELDS
    //========================================================
    [Space]
    [Header("Owner Inventory")]
    [SerializeField]
    private ItemCat[] ownerItemCat = new ItemCat[9];
    [SerializeField]
    private Sprite[] ownerStockSprites = new Sprite[9];
    [SerializeField]
    private Text[] ownerStockValueTexts = new Text[9];    
    [SerializeField]
    private Text[] ownerStockQuanityTexts = new Text[9];
    [SerializeField]
    private InputField[] ownerInput;
    [SerializeField]
    private int[] ownerStockValue = new int[9];
    [SerializeField]
    private int[] ownerStockQuanity = new int[9];
    [SerializeField]
    private Text ownerCreditText;
    private int[] ownerSellQuanity = new int[9];
    private int ownerCredits = 9999;
    //========================================================
    //EFFECT FIELDS
    //========================================================
    private StringBuilder stockMessage = new StringBuilder();
    private StringBuilder messageBuild = new StringBuilder();
    private string[] playerStockNames = new string[11]
    {
        "Amanita Mushroom Spores",
        "Beet Greens",
        "CamasBulb",
        "Corn",
        "Tomato",
        "Wheat",
        "Clam",
        "Otter Carcass",
        "Mountain Goat Carcass Baked",
        "Jaguar Carcass Baked",
        "Lattice Mushroom Species"
      
    };
    private string[] ownerStockNames = new string[9]
    {
        "Basic Salad",
        "Bread",
        "Charred Camas Bulb",
        "Charred Heart Of Palm",
        "Charred Meat",
        "Charred  Papaya",
        "Crispy Bacon",
        "Fern Campfire Salad",
        "Wild Mix"
    };
    private Vector2 endSize = new Vector2(0.001f, 0.001f);
    private Vector2 maxSize = new Vector2(1.25f, 1.25f);
    private int messageIndex = 0;
    private float activeTransitionTime = 1;
    private float activeTransitionTimer;
    private float time = 0;
    private float messageTime = 0.01f;
    private float messageTimer;
    private bool maxWindowGrowth = false;
    private bool scalePop = false;
    private bool storeIsOpen = false;
    private bool fade = false;
    private bool messageActive = false;
    private bool playerINVSelected = false;
    private string messageStart = "Selected item [";
    private string messageValue = "] valued at:  ";


    //====================================================================================================
    //==================================UnityMethods======================================================
    //====================================================================================================
    void Update()
    {
        //store deltaTime as a variable
        time = Time.deltaTime;
        //open store with "E" key
        if (Input.GetKeyDown(KeyCode.E) && !WindowsOpen()) OpenStore(true);
        //close store with "ESC" key
        else if (Input.GetKeyDown(KeyCode.Escape) && currentWindowOpen[0] && !currentWindowOpen[1] && !currentWindowOpen[2]) CloseStore();
        //store background fade effect
        ActiveFadeTransition();
        //store purchase panel scale pop effect
        ActiveScaleTransition();
        //store item discription message
        BuildMessage(stockMessage.ToString());
        //store receipt fade effect
        FadeReceipt();
        //focus on which item is selected
        InputFieldFocused();
        //fade effect for invalid window
        ActiveInvalidFade();
    }

    //====================================================================================================
    //==================================StoreWindow=======================================================
    //====================================================================================================
    //----------------------------------------------------------------------------------------------------
    //Method for checking the windows window
    //----------------------------------------------------------------------------------------------------
    private bool WindowsOpen()
    {
        for(int b = 0; b < currentWindowOpen.Length; b++)
        {
            if (currentWindowOpen[b]) return true;
        }
        return false;
    }
    //----------------------------------------------------------------------------------------------------
    //Method for opening the store window
    //----------------------------------------------------------------------------------------------------
    private void OpenStore(bool active)
    {
        currentWindowOpen[0] = true;
        //setup the player inventory values
        SetupPlayerInventory();
        //setup owner inventory values
        SetupOwnerInventory();
        //set the window effect reversal boolean
        storeIsOpen = !active;
        //shut off the "E" Key active on screen
        ekeyUI.enabled = false;
        //reset the transition time value
        activeTransitionTimer = activeTransitionTime;
        //activate the store window object
        storeBackground.gameObject.SetActive(true);
        //reset the scroll bar window values to the top of list
        for (int s = 0; s < scrollbars.Length; s++) scrollbars[s].value = 1;
        //activate window fade effect transition
        fade = true;
    }
    //----------------------------------------------------------------------------------------------------
    //Method for closing the store window
    //----------------------------------------------------------------------------------------------------
    public void CloseStore()
    {
        //closes the store window
        OpenStore(false);
    }
    //====================================================================================================
    //==================================InvalidWindow=====================================================
    //====================================================================================================

    //----------------------------------------------------------------------------------------------------
    //Method for opening the invalid window
    //----------------------------------------------------------------------------------------------------
    private void OpenInvalidWindow(bool active)
    {
        currentWindowOpen[2] = true;
        invalidOpen = !active;
        invalidWindow.SetActive(true);
        invalidTransactionTimer = invalidTransactionTime;
        invalidEffect = true;
    }
    //----------------------------------------------------------------------------------------------------
    //Method for closing the invalid window
    //----------------------------------------------------------------------------------------------------
    public void CloseInvalidWindow()
    {
        //shut off the invalid fade effect
        invalidEffect = false;
        //reset all text and image alphas to 0
        for (int i = 0; i < invalidWindowIElements.Length; i++)
        {

            invalidWindowIElements[i].color = new Color(invalidWindowIElements[i].color.r,
                                                        invalidWindowIElements[i].color.g,
                                                        invalidWindowIElements[i].color.b,
                                                        0);
        }
        for (int t = 0; t < invalidWindowTElements.Length; t++)
        {
            invalidWindowTElements[t].color = new Color(invalidWindowTElements[t].color.r,
                                                        invalidWindowTElements[t].color.g,
                                                        invalidWindowTElements[t].color.b,
                                                        0);
        }
        //turn off the invalid window
        currentWindowOpen[2] = false;
        invalidWindow.SetActive(false);
    }

    //====================================================================================================
    //==================================WindowEffects=====================================================
    //====================================================================================================

    //----------------------------------------------------------------------------------------------------
    //Method for store background effect
    //----------------------------------------------------------------------------------------------------
    private void ActiveFadeTransition()
    {
        //don't run this method if fade is false
        if (!fade) return;
        //set the actual time of transition
        float actualTime = 1 / fadeTransitionTime;
        //reduce deltaTime from actual time
        activeTransitionTimer -= time * actualTime;
        //clamp the lowest and maximum amount of time
        activeTransitionTimer = Mathf.Clamp01(activeTransitionTimer);
        //lerp the store background alpha color
        storeBackground.color = new Color(storeBackground.color.r, storeBackground.color.g, storeBackground.color.b, Mathf.Lerp(!storeIsOpen ? 0.5f : 0, !storeIsOpen ? 0 : 0.5f, activeTransitionTimer));
        //when timer reaches 0, run window scale pop effect
        if (activeTransitionTimer == 0)
        {
            //on/off the purchase store header
            purchaseHeader.enabled = !storeIsOpen;
            //reset the tranisition time
            activeTransitionTimer = activeTransitionTime;
            //shut off fade effect
            fade = false;
            //activate window scale pop effect
            scalePop = true;
        }
    }
    //----------------------------------------------------------------------------------------------------
    //Method for window scale pop effect
    //----------------------------------------------------------------------------------------------------
    private void ActiveScaleTransition()
    { //don't run this method if scale pop effect is false
        if (!scalePop) return;
        float actualTime = 1 / windowTransitionTime;
        //reduce deltaTime from actual time based on if pop reaached maximum growth
        activeTransitionTimer -= time * (maxWindowGrowth ? 7 : actualTime);
        //clamp the lowest and maximum amount of time
        activeTransitionTimer = Mathf.Clamp01(activeTransitionTimer);

        //if store has [NOT] been open yet
        if (!storeIsOpen)
        {
            //if window scale has [NOT] reached maximum growth
            if (!maxWindowGrowth)
            {
                //lerp the purchase panel window scale from smallest to max size
                PurchasePanel.localScale = Vector2.Lerp(maxSize, endSize, activeTransitionTimer);
                //when timer reaches 0, scale pop effect has reached maximum growth
                if (activeTransitionTimer == 0)
                {
                    //reset the transition timer
                    activeTransitionTimer = activeTransitionTime;
                    // active scale pop maximum growth bool
                    maxWindowGrowth = true;
                }
            }
            else
            {
                //lerp the purchase panel scale from maximum to normal scale value
                PurchasePanel.localScale = Vector2.Lerp(Vector2.one, maxSize, activeTransitionTimer);
                //when timer reaches 0, scale pop effect has finished & store is open
                if (activeTransitionTimer == 0)
                {
                    //scale pop effect no longer at maximum growth
                    maxWindowGrowth = false;
                    //store is now open
                    storeIsOpen = !storeIsOpen;
                    //shut off scale pop effect method
                    scalePop = false;
                }
               
            }
        }
        //if store has [ALREADY] been open
        else
        {
            //lerp the purchase window from current to smallest
            PurchasePanel.localScale = Vector2.Lerp(!storeIsOpen ? Vector2.one : endSize, !storeIsOpen ? endSize : Vector2.one, activeTransitionTimer);
            //when timer reaches 0, close the store
            if (activeTransitionTimer == 0)
            {
                //store is no longer open
                storeIsOpen = !storeIsOpen;
                //deactivate the store gameobject
                storeBackground.gameObject.SetActive(false);
                //shut off scale pop effect method
                scalePop = false;
                //reactivate "E" key UI interaction
                ekeyUI.enabled = true;
                //shut off all open windows
                for (int b = 0; b < currentWindowOpen.Length; b++)
                    currentWindowOpen[b] = false;
               
            }
        }
    }
    //----------------------------------------------------------------------------------------------------
    //Method for building the inventory item description message text
    //----------------------------------------------------------------------------------------------------
    private void BuildMessage(string message)
    {
        //don't run this method if message effect is false
        if (!messageActive) return;
        //reduce deltaTime from actual time
        messageTimer -= time;
        //clamp the lowest and maximum amount of time
        messageTimer = Mathf.Clamp(messageTimer, 0, messageTime);
        //when message timer reaches 0, add thee next character in string then reset message timer
        if (messageTimer == 0)
        {
            //reset the message timer
            messageTimer = messageTime;
            //add new character to message build
            messageBuild.Append(message[messageIndex]);
            //set the message text with new string
            messageText.text = messageBuild.ToString();
            //increase the character index
            messageIndex++;
            //when character index reaches message length
            if (messageIndex > message.Length - 1)
            {
                //shut off build message effect
                messageActive = false;
                //break ouf of the loop
                return;
            }
        }
    }
    //----------------------------------------------------------------------------------------------------
    //Method for invalid window fade effect
    //----------------------------------------------------------------------------------------------------
    private void ActiveInvalidFade()
    {
        //don't run this method if effect is false
        if (!invalidEffect) return;
        //set the actual time of transition
        float actualTime = 1 / invalidTransactionTime;
        //reduce deltaTime from actual time
        invalidTransactionTimer -= time * actualTime;
        //clamp the lowest and maximum amount of time
        invalidTransactionTimer = Mathf.Clamp(invalidTransactionTimer, 0, invalidTransactionTime);
        //lerp through each image alpha color
        for (int i = 0; i < invalidWindowIElements.Length; i++)
        {

            invalidWindowIElements[i].color = new Color(invalidWindowIElements[i].color.r,
                                                        invalidWindowIElements[i].color.g,
                                                        invalidWindowIElements[i].color.b,
                                                        Mathf.Lerp(!receiptOpen ? invalidAlphasI[i] : 0, !invalidOpen ? 0 : invalidAlphasI[i], invalidTransactionTimer));
        }
        //lerp through each text alpha color
        for (int t = 0; t < invalidWindowTElements.Length; t++)
        {
            invalidWindowTElements[t].color = new Color(invalidWindowTElements[t].color.r,
                                                        invalidWindowTElements[t].color.g,
                                                        invalidWindowTElements[t].color.b,
                                                        Mathf.Lerp(!invalidOpen ? 1 : 0, !invalidOpen ? 0 : 1, invalidTransactionTimer));
        }
        //when timer reaches 0, run window scale pop effect
        if (invalidTransactionTimer == 0)
        {
            //reset the tranisition time
            invalidTransactionTimer = invalidTransactionTime;
            //shut off fade effect
            invalidEffect = false;
        }
    }

    //====================================================================================================
    //==================================OnPointerEnterEffects=============================================
    //====================================================================================================

    //----------------------------------------------------------------------------------------------------
    //Method for hovering mouse cursor over current inventory item
    //----------------------------------------------------------------------------------------------------
    public void HighlightStock(int id)
    {
        //reset the current message index
        messageIndex = 0;
        //reset the message timer
        messageTimer = messageTime;
        //clear both build message and stock message
        if (messageBuild.Length > 0) messageBuild.Clear();
        if (stockMessage.Length > 0) stockMessage.Clear();
        //set the stock message to the current stock selected by id number
        stockMessage.Append(messageStart + (playerINVSelected ? playerStockNames[id] : ownerStockNames[id]) + messageValue + (playerINVSelected ? playerStockValue[id] : ownerStockValue[id]));
        //activate the scrolling message effect
        messageActive = true;

    }

    //----------------------------------------------------------------------------------------------------
    //Method for setting up all of the player inventory item values, credits & prices
    //----------------------------------------------------------------------------------------------------
    private void SetupPlayerInventory()
    {   
        //set the credit UI text to player credit value
        playerCreditText.text = "Credits: " + playerCredits.ToString();
        //reset all of the inventory buy/sell quanities back to 0
        for (int s = 0; s < playerSellQuanity.Length; s++)
        {  
            //sell quanity of selected item is 0
            playerSellQuanity[s] = 0;
            //set the sell quanity UI text to new value
            playerInput[s].text = playerSellQuanity[s].ToString();
        }
        //set all of the prices for player inventory items
        for (int s = 0; s < playerStockValueTexts.Length; s++)
        {
            //set the prices to the value texts
            playerStockValueTexts[s].text = playerStockValue[s].ToString();
        }

        //set the total inventory quanity for owner
        for (int s = 0; s < playerStockQuanityTexts.Length; s++)
        {
            //set the inventory count to the stock texts
            if (playerStockQuanity[s] < 1) playerStockQuanityTexts[s].color = Color.red;
            else playerStockQuanityTexts[s].color = Color.white;
            playerStockQuanityTexts[s].text = playerStockQuanity[s].ToString();
        }
    }

    //----------------------------------------------------------------------------------------------------
    //Method for setting up all of the shop owner inventory item values, credits & prices
    //----------------------------------------------------------------------------------------------------
    private void SetupOwnerInventory()
    {
        //set the credit UI text to owner credit value
        ownerCreditText.text = "Credits: " + ownerCredits.ToString();
        //reset all of the inventory buy quanities back to 0
        for (int s = 0; s < ownerSellQuanity.Length; s++)
        {
            //buy quanity of selected item is 0
            ownerSellQuanity[s] = 0;
            //set the buy quanity UI text to new value
            ownerInput[s].text = ownerSellQuanity[s].ToString();
        }
        //set all of the prices for owner inventory items
        for (int s = 0; s < ownerStockValueTexts.Length; s++)
            //set the prices to the value texts
            ownerStockValueTexts[s].text = ownerStockValue[s].ToString();
        //set the total inventory quanity for owner
        for (int s = 0; s < ownerStockQuanityTexts.Length; s++)
        {
            //set the inventory count to the stock texts
            if (ownerStockQuanity[s] < 1) ownerStockQuanityTexts[s].color = Color.red;
            else ownerStockQuanityTexts[s].color = Color.white;
            ownerStockQuanityTexts[s].text = ownerStockQuanity[s].ToString();
        }
    }
    //----------------------------------------------------------------------------------------------------
    //Method for setting if player is using their own inventory
    //----------------------------------------------------------------------------------------------------
    public void OnSelectedPlayerItem()
    {
        //player is [NOW] using their inventory
        playerINVSelected = true;
    }
    //----------------------------------------------------------------------------------------------------
    //Method for setting if player is using store owners inventory
    //----------------------------------------------------------------------------------------------------
    public void OnSelectedOwnerItem()
    {
        //player is [NOT] using their inventory
        playerINVSelected = false;
    }

    //====================================================================================================
    //==================================ButtonMethods=====================================================
    //====================================================================================================

    //----------------------------------------------------------------------------------------------------
    //Method for the trade button to activate purchase transaction
    //----------------------------------------------------------------------------------------------------
    public void TradePurchase()
    {
        //if player and owner have no credits then open invalid window
        if (playerCredits < totalCredits * -1 || ownerCredits < totalCredits) { OpenInvalidWindow(true); return; }

        //loop through the total number of active slots
        for (int p = 0; p < totalSlots; p++)
        {
            //check for player slots 
            if (p < dividedPoint)
            {
                //loop through total inventory until item position found
                for (int s = 0; s < 11; s++)
                {
                    //if the selected item position number matches the inventory item position
                    if (slotItemPosition[p] == s)
                    {
                        //if the player does not have enough stock then leave method
                        if (playerStockQuanity[s] < slotItemQuanities[p]) return;
                        //remove the amount of stock from players inventory
                        playerStockQuanity[s] -= slotItemQuanities[p];
                        //apply the new value to the Text UI object
                        playerStockQuanityTexts[s].text = playerStockQuanity[s].ToString();
                        break;
                    }
                }
            }
            //check for owner slots
            else
            {
                //loop through total inventory until item position found
                for (int s = 0; s < 9; s++)
                {
                    //if the selected item position number matches the inventory item position
                    if (slotItemPosition[p] == s)
                    {
                        //if the owner does not have enough stock then leave method
                        if (ownerStockQuanity[s] < slotItemQuanities[p]) return;
                        //remove the amount of stock from owners inventory
                        ownerStockQuanity[s] -= slotItemQuanities[p];
                        //apply the new value to the Text UI object
                        ownerStockQuanityTexts[s].text = ownerStockQuanity[s].ToString();
                        break;
                    }
                }
            }
        }
        //if credits are taken from the player
        if (totalCredits < 1)
        {
            //remove the total credits to the player credits
            playerCredits += totalCredits;
            //add the total credits to the owner credits
            ownerCredits += totalCredits * -1;
            //close the recipt window
            CloseReceipt(true);
        }
        //if credits are given to the player
        else if(totalCredits > 0)
        {
            //remove the total credits to the owner credits
            ownerCredits += totalCredits * -1;
            //add the total credits to the player credits
            playerCredits += totalCredits;
            //close the recipt window
            CloseReceipt(true);
        }
        //apply new credit values to credit text
        SetCreditUI();
    }
    //----------------------------------------------------------------------------------------------------
    //Method for applying credit text UI
    //----------------------------------------------------------------------------------------------------
    private void SetCreditUI()
    {
        //apply credit string to text object
        playerCreditText.text = "Credits: " + playerCredits.ToString();
        ownerCreditText.text = "Credits: " + ownerCredits.ToString();
        //check for low credits and apply color
        playerCreditText.color = playerCredits < 1 ? Color.red : Color.white;
        ownerCreditText.color = ownerCredits < 1 ? Color.red : Color.white;
    }
    //----------------------------------------------------------------------------------------------------
    //Method for coverting inputfield string into integer
    //----------------------------------------------------------------------------------------------------
    public void SetInputField(int id)
    {
        //if player inventory is active
        if (playerINVSelected)
        {
            //convert input string value to a number
            int inputNum = int.Parse(playerInput[id].text);
            //apply the new number to the current sell quanity
            playerSellQuanity[id] = inputNum;
            //if the new number is larger than players stock them make it equal the stock
            if (playerSellQuanity[id] > playerStockQuanity[id])
                playerSellQuanity[id] = playerStockQuanity[id];
            //set the new input text to the new number
            playerInput[id].text = playerSellQuanity[id].ToString();
        }
        //if owner inventory is active
        else
        {
            //convert input string value to a number
            int inputNum = int.Parse(ownerInput[id].text);
            //apply the new number to the current sell quanity
            ownerSellQuanity[id] = inputNum;
            //if the new number is larger than owners stock them make it equal the stock
            if (ownerSellQuanity[id] > ownerStockQuanity[id])
                ownerSellQuanity[id] = ownerStockQuanity[id];
            //set the new input text to the new number
            ownerInput[id].text = ownerSellQuanity[id].ToString();
        }
    }
    //----------------------------------------------------------------------------------------------------
    //Method for checking which inputfield is currently focused on
    //----------------------------------------------------------------------------------------------------
    private void InputFieldFocused()
    {
        //if player inventory is active
        if (playerINVSelected)
        {
            //loop through inventory for focus inputfield
            for (int i = 0; i < playerInput.Length; i++)
            {
                //focused input was found
                if (playerInput[i].isFocused)
                {
                    //submit text to the focused input field
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        SetInputField(i);
                        break;
                    }
                }
            }
        }
        //if owner inventory is active
        else
        {
            //loop through inventory for focus inputfield
            for (int i = 0; i < ownerInput.Length; i++)
            {  //focused input was found
                if (ownerInput[i].isFocused)
                {
                    //submit text to the focused input field
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        SetInputField(i);
                        break;
                    }
                }
            }
        }

    }
    //----------------------------------------------------------------------------------------------------
    //Method to use increase arrow button to increase quanity amount of selected sellable/purchasable item
    //----------------------------------------------------------------------------------------------------
    public void IncreaseSellStock(int stockID)
    {
        //if player is using their own inventory
        if (playerINVSelected)
        {
            //increase the sell value by 1
            playerSellQuanity[stockID] += 1;
            //clamp the maximum at playerStockQuanity[stockID]
            if (playerSellQuanity[stockID] > playerStockQuanity[stockID]) playerSellQuanity[stockID] = playerStockQuanity[stockID];
            //set the sell quanity text with new value
            playerInput[stockID].text = playerSellQuanity[stockID].ToString();
        }
        //if player is using store owners inventory
        else
        {
            //increase the buy value by 1
            ownerSellQuanity[stockID] += 1;
            //clamp the maximum at ownerStockQuanity[stockID]
            if (ownerSellQuanity[stockID] > ownerStockQuanity[stockID]) ownerSellQuanity[stockID] = ownerStockQuanity[stockID];
            //set the sell quanity text with new value
           ownerInput[stockID].text = ownerSellQuanity[stockID].ToString();
        }
    }
    //----------------------------------------------------------------------------------------------------
    //Method to use decrease arrow button to decrease quanity amount of selected sellable/purchasable item
    //----------------------------------------------------------------------------------------------------
    public void DecreaseSellStock(int stockID)
    {
        //if player is using their own inventory
        if (playerINVSelected)
        {
            //reduce the sell value by 1
            playerSellQuanity[stockID] -= 1;
            //clamp the minimum at 0
            if (playerSellQuanity[stockID] < 0) playerSellQuanity[stockID] = 0;
            //set the sell quanity text with new value
            playerInput[stockID].text = playerSellQuanity[stockID].ToString();
        }
        //if player is using store owners inventory
        else
        { 
            //reduce the buy value by 1
            ownerSellQuanity[stockID] -= 1;
            //clamp the minimum at 0
            if (ownerSellQuanity[stockID] < 0) ownerSellQuanity[stockID] = 0;
            //set the sell quanity text with new value
            ownerInput[stockID].text = ownerSellQuanity[stockID].ToString();
        }
    }
    //----------------------------------------------------------------------------------------------------
    //Method for opening the receipt window
    //----------------------------------------------------------------------------------------------------
    public void OpenReceipt(bool active)
    {
        //set the reverse effect
        receiptOpen = !active;
        //reset the transition timer
        receiptTransactionTimer = receiptTransactionTime;
        //reset the total credit value
        totalCredits = 0;
        //set the total credit text to new 0 value
        receiptTotalCreditText.text = totalCredits.ToString();
        //reset the text to black
        receiptTotalCreditText.color = Color.black;
        //turn on the recipt window
        receiptWindow.SetActive(true);
        //activate the receipt effect
        receiptEffect = true;
        currentWindowOpen[1] = true;
    }
    //----------------------------------------------------------------------------------------------------
    //Method for closing the receipt window
    //----------------------------------------------------------------------------------------------------
    public void CloseReceipt(bool resetInventory)
    {
        ClearSlotWindow();
        if (resetInventory) ResetInventory();
        OpenReceipt(false);
    }
    //----------------------------------------------------------------------------------------------------
    //Method for reseting inventory default values
    //----------------------------------------------------------------------------------------------------
    private void ResetInventory()
    {
        SetupOwnerInventory();
        SetupPlayerInventory();
    }
    //----------------------------------------------------------------------------------------------------
    //Method for fading the receipt window
    //----------------------------------------------------------------------------------------------------
    private void FadeReceipt()
    {
        //if effect is not active don't run this method
        if (!receiptEffect) return;

        //reduce current time by delta time
        receiptTransactionTimer -= time;
        //clamp the transaction time
        receiptTransactionTimer = Mathf.Clamp(receiptTransactionTimer, 0, receiptTransactionTime);
        //loop through all image objects lerping their alpha values
        for (int i = 0; i < receiptWindowIElements.Length; i++)
        {

            receiptWindowIElements[i].color = new Color(receiptWindowIElements[i].color.r, receiptWindowIElements[i].color.g, receiptWindowIElements[i].color.b, Mathf.Lerp(!receiptOpen ? receiptAlphasI[i] : 0, !receiptOpen ? 0 : receiptAlphasI[i], receiptTransactionTimer));
        }
        //loop through all text objects lerping their alpha values
        for (int t = 0; t < receiptWindowTElements.Length; t++)
        {
            receiptWindowTElements[t].color = new Color(receiptWindowTElements[t].color.r,
                                                        receiptWindowTElements[t].color.g,
                                                        receiptWindowTElements[t].color.b,
                                                        Mathf.Lerp(!receiptOpen ?1 : 0, !receiptOpen ? 0 : 1, receiptTransactionTimer));
        }
        //when the timer reaches 0
        if (receiptTransactionTimer == 0)
        {
            //build the slot window with selected inventory if window was not previously opened
            if (!receiptOpen) BuildSlotWindow();
            //if the window was already opened clear slot pool and shut off receipt window
            else { ClearSlotWindow(); currentWindowOpen[1] = false; receiptWindow.SetActive(false); }
            //shut off fade effect
            receiptEffect = false;
        }
    }
    //----------------------------------------------------------------------------------------------------
    //Method for finding which inventory items are being traded
    //----------------------------------------------------------------------------------------------------
    private void BuildSlotWindow()
    {
        //reset the total slots
        totalSlots = 0;
        //reset the divided point
        dividedPoint = 0;
        //setting position for first slot
        int positionY = 0;
        //adding height to content for first slot
        int heightCanvas = 50;
        //reset the slot item color category
        int itemCat = 0;

        // loop through player inventory and find trading items
        for(int v = 0; v < playerSellQuanity.Length; v++)
        {
            //if trade values are more than 0 add the prepare slot
            if (playerSellQuanity[v] > 0) 
            {
                //set the color of the new slot
                switch (playerItemCat[v])
                {
                    case ItemCat.green: itemCat = 0; break;
                    case ItemCat.blue: itemCat = 1; break;
                    case ItemCat.red: itemCat = 2; break;
                    case ItemCat.grey: itemCat = 3; break;
                }
                //set the position in the inventory
                slotItemPosition.Add(v);
                //add the current color to slot
                slotItemCategories.Add(itemCat);
                //set the name of the slot
                slotItemNames.Add(playerStockNames[v]);
                //add the amount of items being traded
                slotItemQuanities.Add(playerSellQuanity[v]);
                //add the price based on item amount
                slotItemValues.Add(playerStockValue[v] * playerSellQuanity[v]);
                //set the slot sprite
                slotItemSprites.Add(playerStockSprites[v]);
                //add to the slot counter
                totalSlots++; 
            }
        }
        //slot number that divides from player to owner
        dividedPoint = totalSlots;
        // loop through player inventory and find trading items
        for (int v = 0; v < ownerSellQuanity.Length; v++)
        {
            //if trade values are more than 0 add the prepare slot
            if (ownerSellQuanity[v] > 0)
            {
                //set the color of the new slot
                switch (ownerItemCat[v])
                {
                    case ItemCat.green: itemCat = 0; break;
                    case ItemCat.blue: itemCat = 1; break;
                    case ItemCat.red: itemCat = 2; break;
                    case ItemCat.grey: itemCat = 3; break;
                }
                //set the position in the inventory
                slotItemPosition.Add(v);
                //add the current color to slot
                slotItemCategories.Add(itemCat);
                //set the name of the slot
                slotItemNames.Add(ownerStockNames[v]);
                //add the amount of items being traded
                slotItemQuanities.Add(ownerSellQuanity[v]);
                //add the price based on item amount
                slotItemValues.Add(-ownerStockValue[v] * ownerSellQuanity[v]);
                //set the slot sprite
                slotItemSprites.Add(ownerStockSprites[v]);
                //add to the slot counter
                totalSlots++; 
            }
        }
        //if no items in inventories were found then quit method
        if (totalSlots == 0) return; 

        //add the number of slots based on total slot counter
        for(int s = 0; s < totalSlots; s++)
        {
            //access the object pool
            GameObject slot = AccessSlotPool();
            //grab the rectTransform
            RectTransform slotTransform = slot.GetComponent<RectTransform>();
            //set the new slot position to rect Transform
            slotTransform.anchoredPosition = new Vector2(0, positionY);
            //Grab the rect Transform of the content window
            RectTransform pool = slotPool.GetComponent<RectTransform>();
            //set the new height value to rect Transform
            pool.sizeDelta = new Vector2(0, heightCanvas);
            //grab the background of the new slot
            Image slotBackground = slot.GetComponent<Image>();
            //apply the new slot color
            switch (slotItemCategories[s])
            {
                case 0: slotBackground.color = itemCategoryColors[0]; break;
                case 1: slotBackground.color = itemCategoryColors[1]; break;
                case 2: slotBackground.color = itemCategoryColors[2]; break;
                case 3: slotBackground.color = itemCategoryColors[3]; break;
            }
            //shows the item quanity on slot
            Text quanity = slot.transform.GetChild(3).GetComponent<Text>();
            quanity.text = "x" + slotItemQuanities[s].ToString();
            //shows the item value on slot
            Text value = slot.transform.GetChild(2).GetComponent<Text>();
            value.text = slotItemValues[s].ToString() + " Credits";
            value.color = (s >= dividedPoint ? Color.red : Color.white);
            //applies the correct slot name
            Text name = slot.transform.GetChild(0).GetComponent<Text>();
            name.text = slotItemNames[s];
            //applies the correct icon sprite
            Image icon = slot.transform.GetChild(1).GetComponent<Image>();
            icon.sprite = slotItemSprites[s];
            //turn on the slot
            slot.SetActive(true);
            //set the position and height of content window for next slot
            positionY -= 50;
            heightCanvas += 50;
        }
        //combine total value of all slots
        foreach(int val in slotItemValues)
        {
            totalCredits += val;
            receiptTotalCreditText.text = totalCredits.ToString();
            receiptTotalCreditText.color = (totalCredits > -1 ? Color.black : Color.red);
        }
    }
    //----------------------------------------------------------------------------------------------------
    //Method for reseting the slot content window
    //----------------------------------------------------------------------------------------------------
    private void ClearSlotWindow()
    {
        dividedPoint = 0;
        totalSlots = 0;
        totalCredits = 0;
        slotItemPosition.Clear();
        slotItemCategories.Clear();
        slotItemNames.Clear();
        slotItemQuanities.Clear();
        slotItemValues.Clear();
        slotItemSprites.Clear();
        for (int sp = 0; sp < slotPool.childCount; sp++)
        {
            if (slotPool.GetChild(sp).gameObject.activeInHierarchy) 
                slotPool.GetChild(sp).gameObject.SetActive(false);
        }
       
    }
    //----------------------------------------------------------------------------------------------------
    //Method for reseting the inventory sell values 
    //----------------------------------------------------------------------------------------------------
    public void ResetSellValues()
    {
        for(int v = 0; v < ownerSellQuanity.Length; v++)
        {
            ownerSellQuanity[v] = 0;
            ownerInput[v].text = ownerSellQuanity[v].ToString();
        }
        for (int v = 0; v < playerSellQuanity.Length; v++)
        {
            playerSellQuanity[v] = 0;
            playerInput[v].text = playerSellQuanity[v].ToString();
        }
    }
    //----------------------------------------------------------------------------------------------------
    //Method for accessing a slot from a pool or prefab
    //----------------------------------------------------------------------------------------------------
    private GameObject AccessSlotPool()
    {
        //check the transform for slots
        for(int sp = 0; sp < slotPool.childCount; sp++)
        {
            //if inactive slot found then use it
            if (!slotPool.GetChild(sp).gameObject.activeInHierarchy) return slotPool.GetChild(sp).gameObject;
        }
        //if not slots were found create a new one
        GameObject slot = Instantiate(slotPrefab, slotPool);
        return slot;
    }
}
