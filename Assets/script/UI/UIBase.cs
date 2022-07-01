using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class UIBase : MonoBehaviour
{
    [SerializeField] private ItemIcon firstSelect;
    [SerializeField] private ItemIcon secondSelect;
    protected UIStateBase _currentState;

    /// <summary>
    /// ‰¼‚Å‚ÌŽÀ‘•
    /// </summary>
    [SerializeField] UIManager _UIManager;

    public ItemIcon FirstSelect { get => firstSelect; set => firstSelect = value; }
    public ItemIcon SecondSelect { get => secondSelect; set => secondSelect = value; }
    public UIManager UIManager { get => _UIManager; set => _UIManager = value; }

    virtual public void Awake()
    {
        _UIManager.AddUIList(this);
    }
    virtual public  void Update()
    {
        _currentState.OnUpdate(this);
    }

    public void Proceed()
    {
        _currentState.OnProceed(this);
    }
    public void Backd()
    {
        _currentState.OnBack(this);
    }
    public void Menu()
    {
        _currentState.OnMenu(this);
    }
    public void ChangeState<T>() where T : UIStateBase, new()
    {
        var nextState = new T();
        _currentState.OnExit(this, nextState);
        nextState.OnEnter(this, _currentState);
        _currentState = nextState;
    }

    public class UIStateBase
    {
        public virtual void OnEnter(UIBase owner, UIStateBase prevState) { }
        public virtual void OnUpdate(UIBase owner) { }
        public virtual void OnExit(UIBase owner, UIStateBase nextState) { }
        public virtual void OnProceed(UIBase owner) { }
        public virtual void OnBack(UIBase owner) { }
        public virtual void OnMenu(UIBase owner) { }
    }
}
