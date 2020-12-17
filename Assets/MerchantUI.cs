using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MerchantUI : MonoBehaviour
{
    [SerializeField] private Button quitButton;
    [SerializeField] private TextMeshProUGUI shopName;
    [SerializeField] private TextMeshProUGUI playerCoins;
    [SerializeField] private TextMeshProUGUI shopCoins;
    [SerializeField] private Transform itemRoot;
    [SerializeField] private GameObject shopItemPrefab;

    private PlayerMerchant playerMerchant;


    void Awake()
    {
        playerMerchant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMerchant>();
    }


    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();
    }

    // Update is called once per frame
    void UpdateUI()
    {
        gameObject.SetActive(playerMerchant.IsActive());

        if (!playerMerchant.IsActive()) return;

        shopName.text = playerMerchant.GetShopName();
        shopCoins.text = playerMerchant.GetShopCoins();
        playerCoins.text = playerMerchant.GetPlayerCoins();

        if (playerMerchant.IsBuying())
        {
            //Build Buying List
        }
        else
        {
            //Build Selling List
        }

        //itemRoot.

    }

    private void BuildMerchantItemList()
    {
        foreach (Transform child in itemRoot)
        {
            Destroy(child);
        }

        foreach (var item in playerMerchant.GetCurrentItems())
        {
            //item.items
        }
    }
}
