using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private DijkstraAI _dijkstraAI;
    [SerializeField] private AStarAI _aStarAI;
    [SerializeField] private Player _player;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private Texture2D _hand;

    private void Awake()
    {
        Assert.IsNotNull(_dijkstraAI, "No reference to DijkstraAI script.");

        Assert.IsNotNull(_aStarAI, "No reference to AStarAI script.");

        Assert.IsNotNull(_player, "No reference to Player script.");

        Assert.IsNotNull(_uiManager, "No reference to UIManager script.");

        Assert.IsNotNull(_hand, "No reference to Hand image.");
    }
    private void Start()
    {
        Cursor.SetCursor(_hand, Vector2.zero, CursorMode.Auto);
    }

    private void Update()
    {
        if (_dijkstraAI.NoPathFound || _aStarAI.NoPathFound )
            _uiManager.ActivateNoWayPanel();
    }
    public void ActivateDijkstraAIScript()
    {
        _player.SetAIPathSource(PathFindingWays.DijkstraAI);

        _dijkstraAI.enabled = true;
        _aStarAI.enabled = false;
        _uiManager.StonePickedUp = false;
    }

    public void ActivateAStarAIScript()
    {
        _player.SetAIPathSource(PathFindingWays.AstarAI);

        _dijkstraAI.enabled = false;
        _aStarAI.enabled = true;
        _uiManager.StonePickedUp = false;
    }


    public void CleanUpScene()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitScene()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

