using System;
using UnityEngine;

[Serializable]
public class ConfigModel
{
    public Resolution resolution { get; set; }
    public LimitFps limitFps { get; set; }
    public bool windowMode { get; set; }
    public bool vSinc { get; set; }
    public Quality quality { get; set; }
    [Header("PostProcessVolume")]
    public bool bloom { get; set; }
    public bool ambientOcclusioon { get; set; }
    public bool reflection { get; set; }
    public bool motionBloor { get; set; }
    public bool deptthOfField { get; set; }
    public bool autoSave { get; set; }

    [Serializable]
    public enum Quality
    {
        VeryLow,
        Low,
        Medium,
        High,
        Ultra
    }

    [Serializable]
    public class Resolution
    {
        public int width { get; set; }
        public int height { get; set; }
    }

    [Serializable]
    public class LimitFps
    {
        public bool limit { get; set; }
        public int fps { get; set; }
    }
}
