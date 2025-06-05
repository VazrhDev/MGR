using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MGR
{

    public enum CarRenderType
    {
        JustRender,
        RenderWithVariant
    }

    public class CarRenderClicked : MonoBehaviour
    {

        public string carDefaultCode;
        public CarRenderType carRenderType;
        public TextMeshProUGUI NFTNumberText;

        public void CarRenderClickedd()
        {
            if (carRenderType == CarRenderType.JustRender)
            {
                GarageScript.instance.ClickedOneCar(carDefaultCode);
            }
            else
            {
                GarageScript.instance.SelectACarAndGoBack(NFTNumberText.text);
            }
        }
    }
}