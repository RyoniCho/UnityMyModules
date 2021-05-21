using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public class Agent: MonoBehaviour
    {
       protected ControlRoom.PhysicsController controller;
       protected ControlRoom.Brick[] bricks;
       protected ControlRoom.AnimationController animationController;


       private void Awake() 
       {
            this.controller=this.GetComponent<ControlRoom.PhysicsController>();
            this.bricks=this.GetComponents<ControlRoom.Brick>();
            this.animationController=this.GetComponentInChildren<ControlRoom.AnimationController>();
       }


       private void Update() 
       {
            foreach(var brick in this.bricks)
            {
                if(brick.isActiveAndEnabled)
                {
                    brick.UpdateFrame();
                }
            }   
       }

    }
}