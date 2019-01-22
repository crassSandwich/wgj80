﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using crass;

public class HackInventoryMenu : MonoBehaviour
{
    public string OpenButton;
    [Range(0, 1)]
    public float PausedTimeScale;
    public MouseLook MouseLookScript;
    public List<GameObject> ThingsToDisableWhenOpen, ThingsToEnableWhenOpen;
    [TextArea]
    public string BlankInfoBoxText;
    public TextMeshProUGUI InfoBox;
    public List<HackButton> HackButtons;

    bool pauseState;
    float oldTimeScale;
    
    void Start ()
    {
		for (int i = 0; i < HackButtons.Count; i++)
        {
            if (i < InventoryManager.Instance.AvailableHacks.Count)
            {
                HackButtons[i].Initialize(InventoryManager.Instance.AvailableHacks[i], onButtonPointerEnter, onButtonPointerExit, onButtonClick);
            }
            else
            {
                HackButtons[i].enabled = false;
            }
        }

        oldTimeScale = Time.timeScale; // make sure oldTimeScale is accurate and not a default value, nor a potentially innaccurate compile-time constant
        setPauseState(false);

        InfoBox.text = BlankInfoBoxText;
    }

    void Update ()
    {
        if (Input.GetButtonDown(OpenButton))
        {
            setPauseState(!pauseState);
        }
    }

    void setPauseState (bool state)
    {
        foreach (var go in ThingsToDisableWhenOpen)
        {
            go.SetActive(!state);
        }
        foreach (var go in ThingsToEnableWhenOpen)
        {
            go.SetActive(state);
        }

        MouseLookScript.enabled = !state;

        if (state)
        {
            oldTimeScale = Time.timeScale;
            Time.timeScale *= PausedTimeScale;

            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
        else
        {
            Time.timeScale = oldTimeScale;
        }

        pauseState = state;
    }

    void onButtonPointerEnter (Hack hack)
    {
        InfoBox.text =
$@"<b><u>{hack.DisplayName}</u></b>

<i>{hack.FlavorText}</i>
{hack.EffectText}
{Mathf.RoundToInt(hack.PatchChance * 100)}% chance to leave trace
{Mathf.RoundToInt(hack.RevealChance * 100)}% chance to trigger alarm
{(hack.InfiniteRange ? "Infinite" : hack.Range + " meter")} range";
    }

    void onButtonPointerExit (Hack hack)
    {
        InfoBox.text = BlankInfoBoxText;
    }

    void onButtonClick (Hack hack)
    {
        setPauseState(false);
        Scanner.Instance.ActiveHack = hack;
    }
}
