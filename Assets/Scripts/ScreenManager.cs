using System.Collections.Generic;
using UnityEngine;
using UnityExtension;


public class ScreenManager : PersistentSingleton<ScreenManager>
{
    [SerializeField] private List<GameScreen> screens = new List<GameScreen>();
    
    private Dictionary<ScreenTypes, GameScreen> _screenMap;
    private readonly Stack<int> _history = new Stack<int>();
    
    private int _select = 0;

    protected override void Awake()
    {
        base.Awake();
        Initial();
    }

    private void Initial()
    {
        _screenMap = new Dictionary<ScreenTypes, GameScreen>();
        foreach (var s in screens)
            _screenMap[s.type] = s;

        foreach (var s in screens)
        {
            s.UI.SetActive(false);
        }

        OpenScreen(ScreenTypes.Menu);
    }

    public void OpenScreen(int id)
    {
        _history.Push(_select);
        screens[_select].UI.SetActive(false);
        screens[id].UI.SetActive(true);
        _select = id;
    }

    public void OpenScreen(ScreenTypes type) {
        _history.Push(_select);
        screens[_select].UI.SetActive(false);
        _screenMap[type].UI.SetActive(true);
        _select = screens.IndexOf(_screenMap[type]);
    }

    public void GoBack()
    {
        if (_history.Count == 0) Application.Quit();

        screens[_select].UI.SetActive(false);
        _select = _history.Pop();
        screens[_select].UI.SetActive(true);
    }
}