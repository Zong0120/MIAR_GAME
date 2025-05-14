using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AirFishLab.ScrollingList.Demo
{
    public class ListEventDemo : MonoBehaviour
    {
        private CircularScrollingList _list =>
            GetComponent<CircularScrollingList>();
        [SerializeField]
        public BagPageManager _bagPageManager;
        public void OnButtonClick(int index)
        {
            _list.SetFocusingBoxByContent(index);
        }
        
        public void DisplayFocusingContent()
        {
            
        }

        public void OnBoxSelected(ListBox listBox)
        {
            
        }

        public void OnFocusingBoxChanged(
            ListBox prevFocusingBox, ListBox curFocusingBox)
        {
            _bagPageManager.topageCheld = ((IntListBox) curFocusingBox).Content;
        }

        public void OnMovementEnd()
        {
            Debug.Log("Movement Ends");
        }
    }
}
