using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;
using System.Text;
using UnityEngine.Assertions;

public class TextTrackBehavior : PlayableBehaviour 
{
    public string text;


    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {

        TextMeshProUGUI text = playerData as TextMeshProUGUI;


        //text.text = this.text;
        text.color = new Color(1, 1, 1, info.weight);
    }
}

