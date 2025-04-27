using UnityEngine;
using UnityEngine.UI;

namespace AirFishLab.ScrollingList.Demo
{
    public class ListEventDemoLock : MonoBehaviour
    {
        private CircularScrollingList _list =>
            GetComponent<CircularScrollingList>();
        public int _password{ get; private set; }

        public void OnButtonClick(int index)
        {
            _list.SetFocusingBoxByContent(index);
        }

        public void _InitList()
        {
            _list.InitializeMembers();
        }
        public void DisplayFocusingContent()
        {
            
        }

        public void OnBoxSelected(ListBox listBox)
        {
            var content =
                (IntListContent)_list.ListBank.GetListContent(listBox.ContentID);
            _password = content.Value;
        }

        public void OnFocusingBoxChanged(
            ListBox prevFocusingBox, ListBox curFocusingBox)
        {
            _password = ((IntListBox) curFocusingBox).Content;
        }
    }
}
