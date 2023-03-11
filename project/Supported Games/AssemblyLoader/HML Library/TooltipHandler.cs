using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HMLLibrary
{
    public class TooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public static List<TooltipHandler> tooltips = new List<TooltipHandler>();
        public GameObject tooltip;

        public void Start()
        {
            tooltips.Add(this);
            tooltip.SetActive(false);
        }

        public void OnDestroy()
        {
            tooltips.Remove(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltips.ForEach(t => t.OnPointerExit(null));
            tooltip.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (gameObject.GetComponent<Button>() != null)
            {
                gameObject.GetComponent<Button>().OnDeselect(null);
            }

            tooltip.SetActive(false);
        }
    }
}