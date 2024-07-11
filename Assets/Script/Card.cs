using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    private Image image;

    public Sprite[] cardFaces;
    public Sprite cardBack;
    public int cardIndex;
    public int num;

    private void Awake()
    {
        image = GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("Image component not found on " + gameObject.name);
        }
    }

    public void SetCard(int index, bool show)
    {
        cardIndex = index;
        if (show)
        {
            image.sprite = cardFaces[cardIndex];
            Debug.Log($"Setting card face: {cardFaces[cardIndex].name}, Card Index: {cardIndex}");
        }
        else
        {
            image.sprite = cardBack;
            Debug.Log("Setting card back");
        }
        num = cardIndex % 13 + 1;

        Debug.Log($"Card sprite is now: {image.sprite.name}");
    }
}