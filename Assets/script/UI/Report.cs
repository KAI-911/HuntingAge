using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Report : UIBase
{
    [SerializeField] GameObject ImagePrefab;
    [SerializeField] GameObject TextPrefab;
    private void Start()
    {
        _currentState = new CloseState();
        _currentState.OnEnter(this, null);
    }

    public void ReportView()
    {
        if (_currentState.GetType() == typeof(CloseState))
        {
            ChangeState<ReportState>();
        }
    }
    private class CloseState : UIStateBase
    {

    }
    private class ReportState : UIStateBase
    {
        Report OWNER;
        GameObject ImageObj;
        GameObject TextObj;

        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            UISoundManager.Instance._player.IsAction = false;
            OWNER = owner.GetComponent<Report>();
            ImageObj = Instantiate(OWNER.ImagePrefab, GameManager.Instance.ItemCanvas.Canvas.transform);
            var imageRect = ImageObj.GetComponent<RectTransform>();
            imageRect.sizeDelta = new Vector2(300, 80);
            imageRect.pivot = new Vector2(0.5f, 0.5f);
            imageRect.anchoredPosition = new Vector2();
            TextObj = Instantiate(OWNER.TextPrefab, GameManager.Instance.ItemCanvas.Canvas.transform);
            var textRect = TextObj.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(300, 80);
            textRect.pivot = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = new Vector2();
            var text = TextObj.GetComponent<Text>();
            text.text = "‘ºƒŒƒxƒ‹‚ª" + GameManager.Instance.VillageData.VillageLevel + "‚É‚È‚è‚Ü‚µ‚½";
        }
        public override void OnProceed(UIBase owner)
        {
            Destroy(ImageObj);
            Destroy(TextObj);
            UISoundManager.Instance._player.IsAction = true;
            GameManager.Instance.LevelUp = false;
            owner.ChangeState<CloseState>();
        }
    }

}
