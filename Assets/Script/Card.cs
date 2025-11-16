using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{

    [Header("Card Face Icon")]
    public Image iconImage; 
    [HideInInspector] public Sprite faceSprite;

    public bool isFlipped = false;
    public bool isMatched = false;

    private bool isAnimating = false;
    void Awake()
    {
        iconImage.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetCardFace(Sprite sprite)
    {
        faceSprite = sprite;
    }

    public void OnCardClicked()
    {
        if (!CompareManager.Instance.inputEnabled)
            return;

        if (isMatched || isFlipped || isAnimating) return;

        StartCoroutine(FlipOpenAnim());
        CompareManager.Instance.OnCardRevealed(this);
    }


    IEnumerator FlipOpenAnim()
    {
        isAnimating = true;
        AudioManager.Instance.PlayFlip();

        // Scale to 0 on X (shrink)
        for (float t = 1f; t > 0f; t -= Time.deltaTime * 10f)
        {
            transform.localScale = new Vector3(t, 1f, 1f);
            yield return null;
        }

        // Show icon 
        isFlipped = true;
        iconImage.sprite = faceSprite;
        iconImage.enabled = true;

        // Scale back to full size
        for (float t = 0f; t < 1f; t += Time.deltaTime * 10f)
        {
            transform.localScale = new Vector3(t, 1f, 1f);
            yield return null;
        }

        transform.localScale = Vector3.one;
        isAnimating = false;
    }

    public void FlipClose()
    {
        if (!isFlipped || isMatched || isAnimating) return;
        StartCoroutine(FlipCloseAnim());
    }

    IEnumerator FlipCloseAnim()
    {
        isAnimating = true;

        // Scale to 0 on X (shrink)
        for (float t = 1f; t > 0f; t -= Time.deltaTime * 10f)
        {
            transform.localScale = new Vector3(t, 1f, 1f);
            yield return null;
        }

        // Hide icon 
        iconImage.enabled = false;
        isFlipped = false;

        // Scale back to full size
        for (float t = 0f; t < 1f; t += Time.deltaTime * 10f)
        {
            transform.localScale = new Vector3(t, 1f, 1f);
            yield return null;
        }

        transform.localScale = Vector3.one;
        isAnimating = false;
    }
    public void FlipOpen()
    {
        isFlipped = true;
        iconImage.sprite = faceSprite;
        iconImage.enabled = true;
    }

    public void FlipOpenInstant()
    {
        SetMatched();
        isFlipped = true;
        iconImage.sprite = faceSprite;
        iconImage.enabled = true;
    }


    public void ShowImmediate()
    {
        iconImage.sprite = faceSprite;
        iconImage.enabled = true;
    }

    public void HideImmediate()
    {
        iconImage.enabled = false;
    }

    public void SetMatched()
    {
        isMatched = true;
    }



}
