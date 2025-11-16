using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompareManager : MonoBehaviour
{
    public static CompareManager Instance;

    private Queue<Card> compareQueue = new Queue<Card>();
    private bool comparing = false;
    public bool inputEnabled = false;
    void Awake()
    {
        // Simple singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public void EnableInput()
    {
        inputEnabled = true;
    }


    /// <summary>
    /// Called when a card is flipped open.
    /// Cards are compared in the order they are revealed.
    /// </summary>
    public void OnCardRevealed(Card card)
    {
        compareQueue.Enqueue(card);

        if (!comparing)
            StartCoroutine(ProcessQueue());
    }

    IEnumerator ProcessQueue()
    {
        comparing = true;

        while (compareQueue.Count >= 2)
        {
            Card c1 = compareQueue.Dequeue();
            Card c2 = compareQueue.Dequeue();

            
            yield return new WaitForSeconds(0.4f);

            
            if (!c1.isFlipped || !c2.isFlipped)
                continue;

            // Check match
            if (c1.faceSprite == c2.faceSprite)
            {
                c1.SetMatched();
                c2.SetMatched();

                AudioManager.Instance.PlayMatch();

                //add score here
                ScoreManager.Instance?.AddMatchScore();

                GameManager.Instance.CheckGameOver();
            }
            else
            {
                AudioManager.Instance.PlayMismatch();

                c1.FlipClose();
                c2.FlipClose();

                ScoreManager.Instance?.AddMismatchPenalty();
            }

            yield return new WaitForSeconds(0.25f);
        }

        comparing = false;
    }


}
