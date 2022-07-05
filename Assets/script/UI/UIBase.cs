using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class UIBase : MonoBehaviour
{
    [SerializeField] private List<ItemIcon> _itemIconList;
    protected UIStateBase _currentState;

    public List<ItemIcon> ItemIconList { get => _itemIconList; set => _itemIconList = value; }

    virtual public void Awake()
    {
        UIManager.Instance.AddUIList(this);
    }
    virtual public void Update()
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
    public void SubMenu()
    {
        _currentState.OnSubMenu(this);
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
        public virtual void OnSubMenu(UIBase owner) { }

    }
}