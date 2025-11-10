using Core.Game.Chunk.Universe.Data;
using TMPro;

namespace Core.Game.View.Details
{
    public class UI_EditorDetail_UniverseDetailShow : UI_Details
    {
        protected TextMeshProUGUI UniverseIdText;
        protected TMP_InputField UniverseNameInput;
        protected TMP_InputField UniverseDescInput;
        
        protected override void OnInit()
        {
            UniverseIdText = transform.Find("UniverseIdText/Text").GetComponent<TextMeshProUGUI>();
            UniverseNameInput = transform.Find("UniverseNameInput").GetComponent<TMP_InputField>();
            UniverseDescInput = transform.Find("UniverseDescInput").GetComponent<TMP_InputField>();
        }

        protected override void OnShow()
        {
            
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnClose()
        {
            
        }

        public void UpdateDetailShow(UniverseDtoDef universeDef)
        {
            
        }
    }
}