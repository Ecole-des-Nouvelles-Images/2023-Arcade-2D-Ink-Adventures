using System.Collections.Generic;
using UnityEngine;

public class OutfitChanger : MonoBehaviour
{
    [Header("Sprite To Change")] 
    public SpriteRenderer bodypart;

    [Header("Sprites to Cycle Through")] 
    public List<Sprite> options = new List<Sprite>();

    [Header("Local Positions for Each Sprite Option")]
    public List<Vector2> initialLocalPositions = new List<Vector2>(); // Stores initial local X and Y positions

    private int currentOption = 0;
    private Vector3 initialPositionRelativeToPlayer;

    void Start()
    {
        // Store the initial local position of the SpriteRenderer relative to the player object
        initialPositionRelativeToPlayer = bodypart.transform.localPosition;

        UpdateSpriteAndPosition();
    }

    public void NextOption()
    {
        currentOption = (currentOption + 1) % options.Count;
        UpdateSpriteAndPosition();
    }

    public void PreviousOption()
    {
        currentOption = (currentOption - 1 + options.Count) % options.Count;
        UpdateSpriteAndPosition();
    }

    void UpdateSpriteAndPosition()
    {
        if (currentOption >= 0 && currentOption < options.Count)
        {
            bodypart.sprite = options[currentOption];

            if (currentOption < initialLocalPositions.Count)
            {
                Vector2 newLocalPosition = initialLocalPositions[currentOption];
                bodypart.transform.localPosition = initialPositionRelativeToPlayer + new Vector3(newLocalPosition.x, newLocalPosition.y, 0);
            }
            else
            {
                Debug.LogError("Position data missing for this option.");
            }
        }
        else
        {
            Debug.LogError("Invalid option index.");
        }
    }
    public void Randomize()
    {
        currentOption = Random.Range(0, options.Count - 1);
        
    }
}