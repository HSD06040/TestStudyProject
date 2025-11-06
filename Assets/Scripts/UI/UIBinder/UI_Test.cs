using System.Collections;
using System.Collections.Generic;
using UIBind;
using UnityEngine;
using UnityEngine.UI;

public class UI_Test : UIBase
{
    [Binding("GoldIcon"), SerializeField] Image goldIcon;
    [Binding, SerializeField] Image subImage;


}
