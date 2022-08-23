using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class UIBase : MonoBehaviour
{
    [SerializeField] private List<ItemIcon> _itemIconList;
    protected UIStateBase _currentState;
    private UIManager uIManager;
    public List<ItemIcon> ItemIconList { get => _itemIconList; set => _itemIconList = value; }

    virtual public void Awake()
    {
        _currentState = new UIStateBase();
        uIManager = UIManager.Instance;
        uIManager.AddUIList(this);
    }
    virtual public void OnDestroy()
    {
        if (uIManager != null)
        {
            UIManager.Instance.EraseUIList(this);
        }
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
    public void UseItemSelectStart()
    {
        _currentState.OnSelectItemStart(this);
    }
    public void UseItemSelectEnd()
    {
        _currentState.OnSelectItemEnd(this);
    }
    public void PushBoxButton()
    {
        _currentState.OnPushBoxButton(this);
    }
    public void PushTriangleButton()
    {
        _currentState.OnPushTriangleButton(this);
    }
    public void SceneChenge()
    {
        _currentState.OnSceneChenge(this);
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
        public virtual void OnSelectItemStart(UIBase owner) { }
        public virtual void OnSelectItemEnd(UIBase owner) { }
        public virtual void OnPushBoxButton(UIBase owner) { }
        public virtual void OnPushTriangleButton(UIBase owner) { }
        public virtual void OnSceneChenge(UIBase owner) { }

    }


}
