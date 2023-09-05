using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Attack
{
    public bool active = true;
    public Player player;
    public float damage;
    public float timer = 0;
    float timerMax;
    public Bounds2 bounds;
    public Vector2 direction;
    public Attack(float damage, Player player, float timer, Bounds2 bounds, Vector2 direction)
    {
        this.damage = damage;
        this.player = player;
        this.timerMax = timer;
        this.bounds = bounds;
        this.direction = direction;
    }

    //expire the attack
    public void expire()
    {
        
        if (timer > timerMax)
        {
            active = false;
        }
        timer += Engine.TimeDelta;
        //bounds.Position = player.Position;
    }




}