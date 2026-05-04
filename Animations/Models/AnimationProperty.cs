using System;
using System.Collections.Generic;
using System.Text;

namespace BlurFileFormats.Animations.Models;

public class AnimationProperty
{
    public string PropertyName { get; set; } = "";
    public PoseFieldType PropertyType { get; set; }
    public PoseFieldElement PropertyField { get; set; }
}
