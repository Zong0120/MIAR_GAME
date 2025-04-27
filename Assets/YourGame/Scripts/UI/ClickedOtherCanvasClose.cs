using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CanvasType
{
    SceneStoryDisplay,
    Other
}

public class ClickedOtherCanvasClose : MonoBehaviour
{
    [SerializeField] private CanvasType canvasType;

    public void CloseWindow()
    {
        gameObject.SetActive(false);
    }
    void Update()
    {   
        if (Input.GetMouseButtonDown(0))
        {
            DetectUILayer();
        }
    }
     private void DetectUILayer()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        if((results.Count >0 && !(results[0].gameObject.name == "Content" ||results[0].gameObject.name == "Handle"))||results.Count == 0)
        {
            switch (canvasType)
            {
                case CanvasType.SceneStoryDisplay:
                    StoryManager.Instance.CloseWindow();
                    break;
                case CanvasType.Other:
                    gameObject.SetActive(false);
                    break;
            }
        }
    }
}