using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic;

namespace BlurFileFormats.Animations.Models;

public class AnimationCycle
{
    public JointTransform RootMotion { get; set; } = JointTransform.Identity;
    public JointTransform? ReferenceTransform { get; set; }
    
    public int FrameCount { get; set; }
    public float PlaybackDuration { get; set; }
    
    public List<AnimationCurve> AnimationCuves { get; } = new();
}
