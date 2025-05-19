using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsScreen : MonoBehaviour
{
    public Toggle fullscreenTog,vsyncTog;
    public List<ResItem> resolutions = new List<ResItem>();
    private int selecteResolution;
    public TextMeshProUGUI resolutionLabel;
    void Start()
    {
        fullscreenTog.isOn = Screen.fullScreen;

        if(QualitySettings.vSyncCount == 0)
        {
            vsyncTog.isOn = false;
        }
        else
        {
            vsyncTog.isOn = true;
        }

        bool foundRes = false;
        for(int i = 0; i < resolutions.Count; i++)
        {
            if(Screen.width == resolutions[i].horizontal && Screen.height == resolutions[i].vertical)
            {
                selecteResolution = i;
                foundRes = true;
                UpdateResLabel();
            }
        }
        if(!foundRes)
        {
            ResItem newRes = new ResItem();
            newRes.horizontal = Screen.width;
            newRes.vertical = Screen.height;
            resolutions.Add(newRes);
            selecteResolution = resolutions.Count - 1;
            UpdateResLabel();
        }
    }

    public void ResLeft()
    {
        selecteResolution--;
        if(selecteResolution < 0)
        {
            selecteResolution = 0;
        }
        UpdateResLabel();
    }

    public void ResRight()
    {
        selecteResolution++;
        if(selecteResolution > resolutions.Count - 1)
        {
            selecteResolution = resolutions.Count - 1;
        }
        UpdateResLabel();
    }

    public void UpdateResLabel()
    {
        resolutionLabel.text = resolutions[selecteResolution].horizontal.ToString() + " x " + resolutions[selecteResolution].vertical.ToString();
        ApplyGraphics();
    }
    public void ApplyGraphics()
    {
        Screen.fullScreen = fullscreenTog.isOn;

        if(vsyncTog.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
        Screen.SetResolution(resolutions[selecteResolution].horizontal, resolutions[selecteResolution].vertical, fullscreenTog.isOn);
    }
}

[System.Serializable]
public class ResItem
{
    public int horizontal, vertical;
}