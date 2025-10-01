using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class LinkHandler : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text textMeshPro;

    void Start()
    {
        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TMP_Text>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, eventData.position, eventData.pressEventCamera);

        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];
            string linkID = linkInfo.GetLinkID();
            
            if (linkID.Contains("http"))
            {
                Application.OpenURL(linkID);
            }
            else
            {
                Debug.Log("Clicked link with ID: " + linkID);
            }
        }
    }
}