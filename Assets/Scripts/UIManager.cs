using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _stonePrefab;
    [SerializeField] private Transform _stoneParent;
    [SerializeField] private GameObject _noWayPanel;

    public bool StonePickedUp;

    const float minX = -7f, maxX = 7f, minY = -4f, maxY = 3f;

    private void Awake()
    {
        Assert.IsNotNull(_stonePrefab, "No reference to Stone prefab.");

        Assert.IsNotNull(_stoneParent, "No reference to Obstacles game object.");

        Assert.IsNotNull(_noWayPanel, "No reference to No_Way_Panel.");
    }

    private void Start()
    {
        _noWayPanel.SetActive(false);
    }

    private void Update()
    {
        if (StonePickedUp)
        {
            if (Input.GetMouseButtonDown(0))
                PlaceStoneOnScene();
        }
    }

    public void ActivateNoWayPanel()
    {
        _noWayPanel.SetActive(true);
    }

    public void PickUpStone()
    {
        StonePickedUp = true;
    }

    private void PlaceStoneOnScene()
    {
        Vector3 placePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        placePos = new Vector3(Mathf.RoundToInt(placePos.x), Mathf.RoundToInt(placePos.y), 0f);

        if ((placePos.x >= minX && placePos.x <= maxX) && (placePos.y >= minY && placePos.y <= maxY))
            Instantiate(_stonePrefab, placePos, Quaternion.identity, _stoneParent);

    }
}
