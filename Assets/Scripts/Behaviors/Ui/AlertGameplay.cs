using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertGameplay : Alert
{
    public Button restartButton;
    public CustomButton soundToggleButton;

    protected override void Start()
    {
        base.Start();
    }

    public void ToggleExtras(bool? restart = null, bool? sound = null)
    {
        if (restart.HasValue)
        {
            this.restartButton?.gameObject?.SetActive(restart.Value);
        }
        if (sound.HasValue)
        {
            this.soundToggleButton?.gameObject?.SetActive(sound.Value);
        }
    }
}
