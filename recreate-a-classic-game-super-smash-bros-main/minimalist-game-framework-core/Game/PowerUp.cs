using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

public class PowerUp
{
    Vector2 spawnPoint;
    public Bounds2 hitbox;
    Random randX = new Random();
    public int powerUpType;
    Texture texture;
    public bool disabled = false;
    public PowerUp()
    {
        spawnPoint = new Vector2(randX.Next(40, 1040), 0);
        hitbox = new Bounds2(spawnPoint,new Vector2(20,20));
        powerUpType = randX.Next(0, 3);
        if (powerUpType == 0)
        {
            texture = Engine.LoadTexture("health.png");
        }
        else if (powerUpType == 1)
        {
            texture = Engine.LoadTexture("attackup.png");
        }
        else
        {
            texture = Engine.LoadTexture("damagedown.png");
        }

    }

    public void render()
    {
        
        Engine.DrawTexture(texture,spawnPoint);
        spawnPoint.Y += Engine.TimeDelta *100;
        hitbox.Position = spawnPoint;
    }

}