using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
class LibraryElement
{
    public GameObject gameObject;
    public Sprite sprite;
}

public class LibraryLoader : MonoBehaviour
{
    [SerializeField]
    private List<LibraryElement> libraryElements;

    [SerializeField]
    private GameObject elementsParent;

    [SerializeField]
    private Button buttonPrefab; // Assign a button prefab in the Inspector

    [SerializeField]
    private Transform spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        // Clear existing children
        foreach (Transform child in elementsParent.transform)
        {
            Destroy(child.gameObject);
        }

        // Create buttons for each LibraryElement
        foreach (var element in libraryElements)
        {
            CreateButton(element);
        }
    }

    private void CreateButton(LibraryElement element)
    {
        // Instantiate the button from the prefab
        Button button = Instantiate(buttonPrefab, elementsParent.transform);

        RectTransform rectTransform = button.GetComponent<RectTransform>();

        //if (rectTransform != null)
        //{
        //    // Set anchors to bottom-left
        //    rectTransform.anchorMin = new Vector2(0, 0); // Bottom-left
        //    rectTransform.anchorMax = new Vector2(0, 0); // Bottom-left

        //    // Set the pivot to the bottom-left
        //    rectTransform.pivot = new Vector2(0, 0); // Bottom-left

        //    // Optionally, set the position of the button
        //    rectTransform.anchoredPosition = new Vector2(0, 0); // Adjust as needed
        //}


        // Find the specific child GameObject that contains the Image component
        Transform iconTransform = button.transform.Find("Icon"); // Replace with the actual name of the child

        if (iconTransform != null)
        {
            // Get the Image component from the specific child GameObject
            Image iconImage = iconTransform.GetComponent<Image>();

            if (iconImage != null)
            {
                iconImage.sprite = element.sprite;
            }
            else
            {
                Debug.LogWarning("Image component not found on the specified child.");
            }
        }
        else
        {
            Debug.LogWarning("Child GameObject with the specified name not found.");
        }

        // Set the button's click event
        button.onClick.AddListener(() => OnButtonClick(element.gameObject));
    }

    private void OnButtonClick(GameObject prefab)
    {
        if (prefab != null && spawnPoint != null)
        {
            Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        }
    }
}
