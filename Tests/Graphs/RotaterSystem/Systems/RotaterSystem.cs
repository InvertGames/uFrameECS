using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uFrame.Kernel;
using uFrame.ECS;
using UniRx;


public class RotaterSystem : RotaterSystemBase {
    
    public override void Setup() {
        base.Setup();

        EnsureDispatcherOnComponents<OnCollisionEnterDispatcher>(SelectablePlayersContext.WithAnyTypes);
        EnsureDispatcherOnComponents<OnCollisionEnterDispatcher>(DamageablesContext.WithAnyTypes);
        EnsureDispatcherOnComponents<OnMouseDownDispatcher>(SelectablePlayersContext.WithAnyTypes);

        this.OnEvent<OnMouseDownDispatcher>()
           .Subscribe(OnMouseDownFilter)
           .DisposeWith(this);

        this.OnEvent<OnCollisionEnterDispatcher>()
            .Subscribe(OnCollisionEnterFilter)
            .DisposeWith(this);
    }
    private void OnMouseDownFilter(OnMouseDownDispatcher x)
    {
        var selectablePlayer = SelectablePlayersContext.MatchAndSelect(x.EntityId);
        if (selectablePlayer == null)
            return;

        OnMouseDownHandler(selectablePlayer);
    }

    private void OnCollisionEnterFilter(OnCollisionEnterDispatcher x)
    {
        var selectablePlayer = SelectablePlayersContext.MatchAndSelect(x.EntityId);
        if (selectablePlayer == null)
            return;
        var damageable = DamageablesContext.MatchAndSelect(x.ColliderId);
        if (damageable == null)
            return;

        OnCollisionEnterHandler(selectablePlayer, damageable);
    }

}
