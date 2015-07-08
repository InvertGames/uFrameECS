using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uFrame.Kernel;
using uFrame.ECS;
using UniRx;
using UnityEngine;


public class RotaterSystem : RotaterSystemBase
{

    public override void Setup()
    {
        base.Setup();
    }

    protected override void Rotate(RotatersContextItem item)
    {
        base.Rotate(item);

        item.Rotater.transform.Rotate(Vector3.right * (5 * Time.deltaTime));

    }

    protected override void MakeRotater(SelectablePlayersContextItem entityidItem)
    {
        base.MakeRotater(entityidItem);
        entityidItem.Player.gameObject.AddComponent<Rotater>();

    }
}

