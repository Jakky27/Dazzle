using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class TextTrackMixer : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {

        TextMeshProUGUI text = playerData as TextMeshProUGUI;

        if (text == null) {
            return;
        }

        string currentText = "";
        float currentAlpha = 0f;
        int inputCount = playable.GetInputCount();


        for (int i = 0; i < inputCount; i++) {
            float inputWeight = playable.GetInputWeight(i);

            if (inputWeight > 0f) {

                ScriptPlayable<TextTrackBehavior> inputPlayable = (ScriptPlayable<TextTrackBehavior>) playable.GetInput(i);
                TextTrackBehavior input = inputPlayable.GetBehaviour();

                //currentText = input.text;

                int currTextLength = (int)(input.text.Length * inputWeight);

                currentText = input.text.Substring(0, currTextLength);

                //Debug.Log(currTextLength);
                //Debug.Log(currentText);

                currentAlpha = inputWeight;
            }
        }

        text.text = currentText;
        text.color = new Color(1, 1, 1, currentAlpha);

    }
}
