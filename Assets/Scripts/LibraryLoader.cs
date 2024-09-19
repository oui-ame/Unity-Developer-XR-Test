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

    [SerializeField]
    private Color itemSelectedColor = Color.cyan;

    [SerializeField]
    private Animator tooltipAnimator;

    [SerializeField]
    public RuntimeAnimatorController animatorController;

    bool isButtonHeld = false;

    [SerializeField]
    private GameObject particleSystemPrefab;

    // Start is called before the first frame update
    void Start()
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

        VirtualLayout vl = elementsParent.GetComponent<VirtualLayout>();
        if (vl != null)
            elementsParent.GetComponent<VirtualLayout>().enabled = true;
        else
            Debug.LogError("VirtualLayout is missing in items parent !");
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
        button.onClick.AddListener(() => OnButtonClick(element, index));
    }

    private void OnButtonClick(LibraryElement elt, int index)
    {
        if (elt != null)
        {
            currentSelected = elt;
            RefreshSelected(index);
            //Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            Transform iconTransform = currentIcon.transform.Find("Icon");
            Image icon = iconTransform.GetComponent<Image>();
            icon.sprite = elt.sprite;
        }
    }

    private void RefreshSelected(int index)
    {

        for (int i = 0; i < elementsParent.transform.childCount; ++i)
        {
            Transform child = elementsParent.transform.GetChild(i);
            Image outlineComp = child.GetComponent<Image>();
            if (outlineComp == null) continue;

            outlineComp.color = i == index ? itemSelectedColor : Color.white;
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
                currentIcon.SetActive(true);
                tooltipAnimator.Play("Idle");
                // Move the icon to the intersection point on the ground
                currentIcon.transform.position = hit.point + Vector3.up * 0.1f;
            } else
                currentIcon.SetActive(false);
        }
    }

    private void SpawnAtIntersection()
    {
        Ray ray = new Ray(vrController.position, vrController.forward);
        RaycastHit hit;

        if (currentSelected == null)
            return;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                tooltipAnimator.SetTrigger("ButtonReleased");
                GameObject instance = Instantiate(currentSelected.gameObject, hit.point + Vector3.up * spawnHeight, Quaternion.identity);
                //instance.transform.localScale = currentSelected.gameObject.transform.localScale;
                instance.tag = "Spawned";
                Animator animator = instance.AddComponent<Animator>();
                animator.runtimeAnimatorController = animatorController;
            }
        }
    }

    public void ClearSpawnedObjects()
    {
        GameObject[] spawnedObjects = GameObject.FindGameObjectsWithTag("Spawned");
        
        for (int i = 0; i < spawnedObjects.Length; ++i)
        {
            Instantiate(particleSystemPrefab, spawnedObjects[i].transform.position, Quaternion.Euler(-90, 0, 0));
            Destroy(spawnedObjects[i]);
        }
    }

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

        if (isButtonHeld && currentSelected != null)
        {
            Debug.Log("BUTTON BEING HELD");
            ShowIconAtIntersection();
        }
    }
}
