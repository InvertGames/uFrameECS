using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uFrame.ECS;
using UnityEngine;


public partial class FlapBird {
    
    public override void Execute() {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Bird.DidFlap = true;
        }
        Bird.Velocity += new Vector3(0f, -9.25f, 0f) * Time.deltaTime;
        if (Bird.DidFlap)
        {
            Bird.DidFlap = false;
            Bird.Velocity += new Vector3(0f, Bird.FlapVelocity, 0f);
        }
        Bird.Velocity = Vector3.ClampMagnitude(Bird.Velocity, Bird.MaxSpeed);
        Bird.transform.position += Bird.Velocity * Time.deltaTime;

        float angle = Bird.Velocity.y < 0 ? Mathf.Lerp(0f, -90, -Bird.Velocity.y / Bird.MaxSpeed) : 0;
        Bird.transform.rotation = Quaternion.Euler(0f, 0f, angle);
     
    }
}
