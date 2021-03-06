﻿using System.Drawing;

namespace BinhlEmul
{
    
    class Render
    {
        private readonly World _world;
        public Render(World world)
        {
            _world = world;
        }
        public Image GetSingeLayeImage()
        {
            Image i = new Bitmap(_world.WorldSizeX * 10, _world.WorldSizeY*10);
            var gr = Graphics.FromImage(i);
            gr.Clear(Color.Black);
            for (int x = 0; x < _world.WorldSizeX; x++)
            {
                for (int y = 0; y < _world.WorldSizeY; y++)
                {
                    for (int z = 0; z < _world.WorldSizeZ; z++)
                    {
                        if (_world.GetObject(x,_world.WorldSizeY - y -1 , z).GetType() == typeof(WorldObjects.RedstoneWire))
                        {
                            Color c = Color.FromArgb(50 + 12 * _world.GetObject(x, _world.WorldSizeY - y - 1, z).RedValue, 0, 0);
                            Brush b = new SolidBrush(c);
                            gr.FillRectangle(b, x * 10, y * 10, 10, 10);
                        }

                        if (_world.GetObject(x, _world.WorldSizeY - y - 1, z).GetType() == typeof(WorldObjects.RedstoneRepiter))
                        {
                            Color c = Color.FromArgb(0, 50, 0);
                            if (_world.GetObject(x, _world.WorldSizeY - y - 1, z).IsActivated)
                            {
                                c = Color.FromArgb(0, 250, 0);
                            }
                            Brush b = new SolidBrush(c);
                            gr.FillRectangle(b, x * 10, y * 10, 10, 10);
                        }
                    }   
                }
            }
            for (int x = 0; x < _world.WorldSizeX; x++)
            {
                for (int y = 0; y < _world.WorldSizeY; y++)
                {
                    for (int z = 0; z < _world.WorldSizeZ; z++)
                    {
                        if (_world.GetObject(x, _world.WorldSizeY - y - 1, z).GetType() == typeof(WorldObjects.RedstoneTorch))
                        {
                            Color c = Color.FromArgb(0, 0, 50);
                            if (_world.GetObject(x, _world.WorldSizeY - y - 1, z).IsActivated)
                            {
                                c = Color.FromArgb(0, 0, 250);
                            }
                            Brush b = new SolidBrush(c);
                            gr.FillEllipse(b, x * 10+2, y * 10+2, 6, 6);
                        }
                    }
                }
            }

            return i;
        }
    }
}
