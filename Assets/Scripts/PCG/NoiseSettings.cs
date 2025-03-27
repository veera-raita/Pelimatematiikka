using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    [Range (0.001f, 1f)] public float frequency = 0.01f;
    [Range (0f, 20f)] public float amplitude = 0.01f;
}
