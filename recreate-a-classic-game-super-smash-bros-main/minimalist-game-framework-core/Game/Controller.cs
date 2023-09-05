using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

public class Controller
{

    //number of controllers not including keyboard
    public static int controllers = 0;

    //player number
    public int pNum;

    //is it taken by a player
    public bool taken = false;
    public Player player;
    public IntPtr connectedController;

    public int keyboard;

    int deadzone = 5000;
    public bool movementFlag = false;
    public float lx = 0;
    public float ly = 0;
    public float rx = 0;
    public float ry = 0;

    public bool downlocked = false;
    public bool uplocked = false;
    public int color;
    public bool canAttack;

    public Bounds2 cursor = new Bounds2(new Vector2(540, 320), new Vector2(40, 40));
    Dictionary<SDL.SDL_GameControllerButton, bool> locks = new Dictionary<SDL.SDL_GameControllerButton, bool>();

    Dictionary<string, SDL.SDL_GameControllerButton> buttonBinds = new Dictionary<string, SDL.SDL_GameControllerButton>();
    Dictionary<string, Key> keyBinds = new Dictionary<string, Key>();
    public Controller(IntPtr controller, int keyboard)
    {

        this.keyboard = keyboard;

        if (keyboard == 1)
        {
            keyBinds.Add("jump", Key.W);
            keyBinds.Add("left", Key.A);
            keyBinds.Add("right", Key.D);


            keyBinds.Add("up", Key.W);
            keyBinds.Add("down", Key.S);
            keyBinds.Add("s", Key.G);
            keyBinds.Add("d", Key.H);
            keyBinds.Add("next", Key.Return);
            keyBinds.Add("connect", Key.Q);
            keyBinds.Add("switch", Key.Q);
            keyBinds.Add("select", Key.A);
            keyBinds.Add("la", Key.F);
            keyBinds.Add("ua", Key.T);
            keyBinds.Add("ra", Key.H);
            keyBinds.Add("da", Key.G);
            keyBinds.Add("pause", Key.Escape);


        }
        else if (keyboard == 2)
        {
            keyBinds.Add("jump", Key.P);
            keyBinds.Add("left", Key.L);
            keyBinds.Add("right", Key.SingleQuote);

            keyBinds.Add("up", Key.Up);
            keyBinds.Add("down", Key.Down);

            keyBinds.Add("next", Key.Backslash);
            keyBinds.Add("connect", Key.O);
            keyBinds.Add("switch", Key.O);
            keyBinds.Add("select", Key.Return);
            keyBinds.Add("la", Key.Left);
            keyBinds.Add("ua", Key.Up);
            keyBinds.Add("ra", Key.Right);
            keyBinds.Add("da", Key.Down);
            keyBinds.Add("pause", Key.Escape);
        }
        else
        {
            connectedController = controller;

            buttonBinds.Add("jump", SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A);
            buttonBinds.Add("b", SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B);
            buttonBinds.Add("x", SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X);
            buttonBinds.Add("next", SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START);
            buttonBinds.Add("pause", SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START);
            buttonBinds.Add("down", SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_DOWN);
            buttonBinds.Add("dr", SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_RIGHT);
            buttonBinds.Add("up", SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_UP);
            buttonBinds.Add("dl", SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_LEFT);

            buttonBinds.Add("switch", SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER);
            buttonBinds.Add("rb", SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER);

            

            buttonBinds.Add("back", SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B);
            buttonBinds.Add("connect", SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START);
            buttonBinds.Add("select", SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A);
            buttonBinds.Add("remove", SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B);
            foreach (SDL.SDL_GameControllerButton b in buttonBinds.Values)
            {
                if (!locks.ContainsKey(b))
                {
                    locks.Add(b, false);
                }
            }
        }






    }


    public string getType()
    {
        if (keyboard == 0)
        {

            return "controller";
        }
        if (keyboard == 1)
        {
            return "keyboardLeft";
        }
        if (keyboard == 2)
        {
            return "keyboardRight";
        }
        return "unknown";
    }
    public bool isConnected()
    {
        if (keyboard > 0)
        {
            return true;
        }
        return SDL.SDL_GameControllerGetAttached(connectedController) > 0;
    }





    public bool getButton(string button)
    {

        if (keyboard > 0)
        {
            return Engine.GetKeyDown(keyBinds.GetValueOrDefault(button));
        }

        if (!(SDL.SDL_GameControllerGetButton(connectedController, buttonBinds.GetValueOrDefault(button)) > 0) && locks.GetValueOrDefault(buttonBinds.GetValueOrDefault(button)))
        {
            locks.Remove(buttonBinds.GetValueOrDefault(button));
            locks.Add(buttonBinds.GetValueOrDefault(button), false);

        }
        if (!locks.GetValueOrDefault(buttonBinds.GetValueOrDefault(button)) && SDL.SDL_GameControllerGetButton(connectedController, buttonBinds.GetValueOrDefault(button)) > 0)
        {
            locks.Remove(buttonBinds.GetValueOrDefault(button));
            locks.Add(buttonBinds.GetValueOrDefault(button), true);
            return true;
        }
        return false;
    }

    public bool getButtonHeld(string button)
    {
        if (keyboard > 0)
        {
            return Engine.GetKeyHeld(keyBinds.GetValueOrDefault(button));
        }
        return SDL.SDL_GameControllerGetButton(connectedController, buttonBinds.GetValueOrDefault(button)) > 0;

    }
    public bool getUp()
    {

        if (keyboard > 0)
        {
            return Engine.GetKeyDown(keyBinds.GetValueOrDefault("up"));
        }
        getAxis();
        if (ly == 0 && uplocked)
        {
            uplocked = false;
        }
        if (ly < 0 && !uplocked)
        {
            uplocked = true;
            return true;
        }

        return false;
    }
    public bool getDown()
    {
        if (keyboard > 0)
        {
            return Engine.GetKeyDown(keyBinds.GetValueOrDefault("down"));
        }
        getAxis();
        //Console.WriteLine(ly);
        if (ly == 0 && downlocked)
        {
            downlocked = false;
        }
        if (ly > 0 && !downlocked)
        {
            downlocked = true;
            return true;
        }

        return false;
    }
    public float getMovement()
    {
        Console.WriteLine(movementFlag);
        getAxis();
        if (keyboard > 0)
        {
            if (Engine.GetKeyHeld(keyBinds.GetValueOrDefault("right")))
            {

                return 1;
            }
            else if (Engine.GetKeyHeld(keyBinds.GetValueOrDefault("left")))
            {

                return -1;
            }
            movementFlag = false;
            return 0;
        }
        //Console.WriteLine(lx);
        if (lx / (32767 - deadzone) == 0)
        {
            movementFlag = false;
        }
        return lx / (32767 - deadzone);

    }


    public void getCursor(int sens)
    {

        if (keyboard == 1)
        {
            cursor.Position.X = Engine.MousePosition.X;
            cursor.Position.Y = Engine.MousePosition.Y;
            return;
        }
        if (keyboard == 2)
        {
            if (Engine.GetKeyHeld(Key.Down))
            {
                cursor.Position.Y += 8;
            }
            if (Engine.GetKeyHeld(Key.Up))
            {
                cursor.Position.Y -= 8;
            }
            if (Engine.GetKeyHeld(Key.Left))
            {
                cursor.Position.X -= 8;
            }
            if (Engine.GetKeyHeld(Key.Right))
            {
                cursor.Position.X += 8;
            }

        }
        if (Engine.GetKeyDown(Key.Down))
        {
            cursor.Position.Y -= 10;
        }
        if (Engine.GetKeyDown(Key.Up))
        {
            cursor.Position.Y += 10;
        }
        getAxis();

        cursor.Position.Y += ly / (32767 - deadzone) * sens;
        cursor.Position.X += lx / (32767 - deadzone) * sens;

    }



    public float getMovementY()
    {
        getAxis();

        //Console.WriteLine(lx);
        return ly / (32767 - deadzone);

    }

    public int getAttack()
    {

        if (keyboard > 0)
        {
            if (Engine.GetKeyDown(keyBinds.GetValueOrDefault("la")))
            {
                return 0;
            }
            else if (Engine.GetKeyDown(keyBinds.GetValueOrDefault("ua")))
            {
                return 1;
            }
            else if (Engine.GetKeyDown(keyBinds.GetValueOrDefault("ra")))
            {
                Console.WriteLine(true);
                return 2;
            }
            else if (Engine.GetKeyDown(keyBinds.GetValueOrDefault("da")))
            {
                return 3;
            }
            return -1;
        }
        getAxis();
        if (rx < 0)
        {
            //Console.WriteLine(rx);
            return 0;
        }
        if (ry < 0)
        {
            return 1;
        }
        if (rx > 0)
        {
            return 2;
        }
        if (ry > 0)
        {
            return 3;
        }
        return -1;

    }
    public bool getSelect()
    {
        if (keyboard == 1)
        {
            return Engine.GetMouseButtonDown(MouseButton.Left);
        }

        return this.getButton("select");
    }

    public bool getSelectMapMaker()
    {
        if (keyboard == 1)
        {
            return Engine.GetMouseButtonHeld(MouseButton.Left);
        }

        return this.getButtonHeld("select");
    }
    public bool getBack()
    {
        if (keyboard > 0)
        {
            return Engine.GetKeyDown(Key.Escape);
        }
        

        return this.getButton("back");
    }
    public void getAxis()
    {

        //65034 max ushort value
        lx = SDL.SDL_GameControllerGetAxis(connectedController, SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX);
        ly = SDL.SDL_GameControllerGetAxis(connectedController, SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY);
        rx = SDL.SDL_GameControllerGetAxis(connectedController, SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTX);
        ry = SDL.SDL_GameControllerGetAxis(connectedController, SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTY);
        //Console.WriteLine(ly);
        if (Math.Abs(lx) < deadzone)
        {
            lx = 0;
        }
        else if (lx < 0)
        {
            lx = deadzone + lx;
        }
        else
        {
            lx = lx - deadzone;
        }

        if (Math.Abs(ly) < deadzone)
        {
            ly = 0;
        }
        else if (ly < 0)
        {
            ly = deadzone + ly;
        }
        else
        {
            ly = ly - deadzone;
        }

        if (Math.Abs(rx) < deadzone)
        {
            rx = 0;
        }
        else if (rx < 0)
        {
            rx = deadzone + rx;
        }
        else
        {
            rx = rx - deadzone;
        }


        if (Math.Abs(ry) < deadzone)
        {
            ry = 0;
        }
        else if (ry < 0)
        {
            ry = deadzone + ry;
        }
        else
        {
            ry = ry - deadzone;
        }
        //Console.WriteLine(lx);







        //30000/500

        //29767 max


    }

}