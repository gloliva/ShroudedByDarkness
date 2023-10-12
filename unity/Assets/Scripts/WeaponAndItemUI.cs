using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponAndItemUI : MonoBehaviour
{
    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private Text itemText;

    [SerializeField]
    private Image weaponImage;

    [SerializeField]
    private Text weaponText;

    [SerializeField]
    private Sprite[] itemSprites;

    [SerializeField]
    private Sprite[] weaponSprites;

    public void SetItemImage(Sprite itemSprite, string itemName, int itemCount, int posX, int posY)
    {
        if (itemSprite == null)
        {
            itemImage.gameObject.SetActive(false);
            return;
        }

        if (!itemImage.gameObject.activeSelf)
        {
            itemImage.gameObject.SetActive(true);
        }

        itemImage.sprite = itemSprite;
        itemImage.rectTransform.position = new Vector3(posX, posY, 0);

        if (itemName.Equals("Flashlight"))
        {
            itemImage.rectTransform.rotation.Set(0, 0, 90, 1);
            itemImage.rectTransform.localScale = new Vector3(0.55f, 0.55f, 1f);
        } else
        {
            itemImage.rectTransform.rotation.Set(0, 0, 0, 1);
            itemImage.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }

        itemText.text = itemName + ": " + itemCount;
    }

    public void SetWeaponImage(Sprite weaponSprite, string weaponName, int posX, int posY)
    {
        if (weaponSprite == null)
        {
            weaponImage.gameObject.SetActive(false);
            weaponText.text = weaponName;
            return;
        }

        if (!weaponImage.gameObject.activeSelf)
        {
            weaponImage.gameObject.SetActive(true);
        }

        weaponImage.sprite = weaponSprite;
        weaponImage.rectTransform.position = new Vector3(posX, posY, 0);
        weaponText.text = weaponName;
    }

    public void SetCurrentItemCount(int itemType, int itemCount)
    {
        itemText.text = LightMasterController.lanternNames[itemType] + ": " + itemCount;
    }

    public Sprite GetItemSprite(int itemType)
    {
        return itemSprites[itemType];
    }

    public Sprite GetWeaponSprite(string weaponName)
    {
        switch (weaponName)
        {
            case "Gun":
                return weaponSprites[0];
            case "Pipe":
                return weaponSprites[1];
            default:
                return null;
        }
    }

    public void SetAmmoCount(string weaponName, int ammo)
    {
        weaponText.text = weaponName + " x" + ammo.ToString();
    }
}
