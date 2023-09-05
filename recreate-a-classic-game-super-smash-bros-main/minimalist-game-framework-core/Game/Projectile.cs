using System;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;

public class Projectile
{
    public Player Enemy;

    public Vector2 Position;
    public Vector2 Velocity;
    public Vector2 FantasyVelocity;
    public Vector2 Acceleration;

    public Vector2 Target;
    public Vector2 TargetVelocity;
    public float Speed;
    public bool SelfGuided;

    public List<Texture> Tex;

    public bool exploded;
    public int deathTimer;

    public SoundInstance sound;

    public Projectile(Vector2 position, Vector2 target, Vector2 targetVelocity, float speed, List<Texture> tex, bool selfGuided = false, Player enemy = null)
    {
        Position = position;
        Target = target;
        Velocity = new Vector2();
        Acceleration = new Vector2();
        TargetVelocity = targetVelocity;
        Speed = speed;
        Tex = tex;
        SelfGuided = selfGuided;
        Enemy = enemy;
        FantasyVelocity = new Vector2();
        exploded = false;
        deathTimer = 0;
        sound = null;
    }

    public void updateEnemy()
    {
        if (Enemy != null)
        {
            Target = Enemy.Position;
            TargetVelocity = Enemy.Velocity;
        }
    }

    float minpos(double A, double B)
    {
        if (A > 0 && B > 0)
        {
            return (float) Math.Min(A, B);
        }
        else if (A > 0)
        {
            return (float) A;
        }
        else if (B > 0)
        {
            return (float) B;
        }
        else
        {
            return 0;
        }
    }


    public void recalculate()
    {
        Vector2 T = Target + TargetVelocity;
        Vector2 D = Target - Position;
        float a = Vector2.Dot(TargetVelocity, TargetVelocity) - Speed * Speed;
        float b = 2 * Vector2.Dot(D, TargetVelocity);
        float c = Vector2.Dot(D, D);

        float det = b * b - 4 * a * c; if (det < 0) return; if (a == 0) return;
        float t = minpos(((-b + Math.Sqrt(det)) / (2 * a)), ((-b - Math.Sqrt(det)) / (2 * a)));

        Vector2 Terminal = Target + t * TargetVelocity;

        FantasyVelocity = (Terminal - Position).Normalized() * Speed;
        // Acceleration = 0.1f * ((Terminal - Position).Normalized() * Speed) - Velocity;
    }
    public void render()
    {
        if ((Target - Position).Length() < 20f) {
            exploded = true;
            Engine.PlaySound(Engine.LoadSound("explosion.ogg"), false, 0.5f);
            return;
        }
        if (SelfGuided)
        {
            Random rand = new Random();
            int ind = rand.Next(3);
            if (ind == 0) Engine.DrawTexture(Tex[0], Position, size: new Vector2(120, 120), rotation: -35);
            else if (ind == 1) Engine.DrawTexture(Tex[1], Position, size: new Vector2(120, 120), rotation: 35);
            else if (ind == 2) Engine.DrawTexture(Tex[2], Position, size: new Vector2(120, 120));
        }
        else
        {
            Engine.DrawTexture(Tex[0], Position, size: new Vector2(200, 60));
        }
        if (exploded) return;
        Random randv = new Random();
        Position += Velocity;
        Velocity += (FantasyVelocity - Velocity) * 0.1f;
    }

}