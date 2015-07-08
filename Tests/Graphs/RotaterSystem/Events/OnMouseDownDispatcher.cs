using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using uFrame.ECS;


public partial class OnMouseDownDispatcher : uFrame.ECS.EcsDispatcher {
    public void OnMouseDown()
    {
        this.Publish(this);
    }
}
