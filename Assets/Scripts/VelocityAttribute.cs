using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityAttribute {
    public Vector2 Velocity { get; set; }
    public bool AutoClear { get;private set; }
    public bool IsCollider { get;  set; }
    public bool IsAddClearSelf { get; private set; }
    public bool IsAddClearOther { get; private set; }
    public float AttenuationFactor { get; private set; }
    public bool IsGroundClear { get; private set; }
    public bool IsOtherClear { get; private set; }
    public VelocityAttribute(bool autoClear = false, bool isCollider = false,  bool isAddClearSelf = false, bool isAddClearOther = false, float attenuationFactor = 0,bool isGroundClear=true,bool isOtherClear=true)
    {
        AutoClear = autoClear;
        IsCollider = isCollider;
        IsAddClearSelf = isAddClearSelf;
        IsAddClearOther = isAddClearOther;
        AttenuationFactor = attenuationFactor;
        IsGroundClear = isGroundClear;
        IsOtherClear=isOtherClear;
    }
}
