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
    private Button buttonPrefab;

    [SerializeField]
    private GameObject iconPrefab; // Assign the icon prefab in the Inspector

    [SerializeField]
    private GameObject currentIcon;

    [SerializeField]
    private Transform vrController;

    [SerializeField]
    private Transform centerEyeAnchor;

    private LibraryElement currentSelected;

    [SerializeField]
    private float spawnHeight = 0.5f;

    // Start is called before the first frame update
    void Awake()
    {
        // Clear existing children
        foreach (Transform child in elementsParent.transform)
        {
            Destroy(child.gameObject);
        }

        // Create buttons for each LibraryElement

        for (int i = 0; i < libraryElements.Count; ++i)
        {
            CreateButton(libraryElements[i], i);
        }

        if (currentIcon != null)
            currentIcon.SetActive(false);
    }

    private void CreateButton(LibraryElement element, int index)
    {
        // Instantiate the button from the prefab
        Button button = Instantiate(buttonPrefab, elementsParent.transform);

        RectTransform rectTransform = button.GetComponent<RectTransform>();

        rectTransform.anchoredPosition = new Vector3(69 + 2 * index * 69, -69);

        // Find the "Icon" child gameObject
        Transform iconTransform = button.transform.Find("Icon");

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
        button.onClick.AddListener(() => OnButtonClick(element));
    }

    private void OnButtonClick(LibraryElement elt)
    {
        if (elt != null)
        {
            currentSelected = elt;
            //Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            Transform iconTransform = currentIcon.transform.Find("Icon");
            Image icon = iconTransform.GetComponent<Image>();
            icon.sprite = elt.sprite;

            ShowIconAtIntersection();
        }
    }

    private void ShowIconAtIntersection()
    {
        // Perform a raycast from the VR controller
        Ray ray = new Ray(vrController.position, vrController.forward);
        RaycastHit hit;

        Image icon = currentIcon.GetComponent<Image>();
        icon.transform.LookAt(2 * icon.transform.position - centerEyeAnchor.position);

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("hit => " + hit.collider.name + ", with tag => " + hit.collider.tag);
            if (hit.collider.CompareTag("Ground"))
            {
                // Move the icon to the intersection point on the ground
                currentIcon.transform.position = hit.point;
                if (!currentIcon.activeSelf)
                    currentIcon.SetActive(true);
            } else {
                currentIcon.SetActive(false);
            }
        }
    }

    private void SpawnAtIntersection()
    {
        Ray ray = new Ray(vrController.position, vrController.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                Instantiate(currentSelected.gameObject, hit.point + Vector3.up * spawnHeight, Quaternion.identity);
            }
        }
    }

    bool isButtonHeld = false;

    void Update()
    {
        // Check if the VR controller button is being held down
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
            isButtonHeld = true;
        else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
        {
            isButtonHeld = false;
            SpawnAtIntersection();
        }

        if (isButtonHeld)
        {
            Debug.Log("BUTTON BEING HELD");
            ShowIconAtIntersection();
        }
        else
        {
            if (currentIcon != null)
            {
                currentIcon.SetActive(false);
            }
        }
    }
}
