﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

public class AssetManager
{
    protected ContentManager contentManager;
    protected GraphicsDevice graphicsDevice;
    protected SpriteBatch spriteBatch;
    protected SpriteFont defaultFont;

    protected Dictionary<string, Texture2D> textures;
    protected Dictionary<string, SpriteFont> fonts;

    protected RenderTarget2D maskRenderTarget;

    public AssetManager(ContentManager content, GraphicsDevice graphics)
    {
        contentManager = content;
        graphicsDevice = graphics;
        //this.spriteBatch = spriteBatch;
        textures = new Dictionary<string, Texture2D>();
        fonts = new Dictionary<string, SpriteFont>();

        PresentationParameters pp = graphicsDevice.PresentationParameters;
        maskRenderTarget = new RenderTarget2D(graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, SurfaceFormat.Single, pp.DepthStencilFormat);
    }

    /// <summary>
    /// Get a 1x1 Texture2D that has the specified color.
    /// </summary>
    public Texture2D GetSingleColorPixel(Color color)
    {
        string key = "singleColor:" + color.ToString();
        if (!textures.ContainsKey(key))
        {
            Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData<Color>(new Color[] { color });
            textures.Add(key, texture);
        }
        return textures[key];
    }

    public SpriteFont GetFont(string assetName)
    {
        if (!fonts.ContainsKey(assetName))
        {
            fonts.Add(assetName, contentManager.Load<SpriteFont>(assetName));
        }
        return fonts[assetName];
    }

    public Texture2D GetSprite(string assetName)
    {
        if (assetName == "" || assetName == null)
        { 
            return null;
        }
        else if (textures.ContainsKey(assetName))
        {
            return textures[assetName];
        }
        else if (assetName.StartsWith("empty"))
        {
            return GetEmptySprite(assetName);
        }
        else
        {
            textures.Add(assetName, contentManager.Load<Texture2D>(assetName));
            return textures[assetName];
        }
    }

    /// <summary>
    /// This method is used to add trans
    /// </summary>
    /// <returns></returns>
    public Texture2D GetTransparentTallSprite(Texture2D asset)
    {
        graphicsDevice.SetRenderTarget(maskRenderTarget);
        //graphicsDevice.BlendState.AlphaBlendFunction = BlendFunction.Add;
        graphicsDevice.BlendState.AlphaDestinationBlend = Blend.Zero;
        graphicsDevice.BlendState.AlphaSourceBlend = Blend.One;

        graphicsDevice.BlendState.ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;
        spriteBatch.Draw(asset, new Vector2(0, 0), Color.White);

        graphicsDevice.BlendState.ColorWriteChannels = ColorWriteChannels.Alpha;
        spriteBatch.Draw(GetSingleColorPixel(Color.White), new Rectangle(0, 0, asset.Width, asset.Height - Wink.Tile.TileHeight), Color.White);

        graphicsDevice.BlendState.ColorWriteChannels = ColorWriteChannels.All;
        graphicsDevice.SetRenderTarget(null);
        return maskRenderTarget;
    }

    public Texture2D GetEmptySprite(string emptyString)
    {
        //Split up the string and get the parameters
        string[] parts = emptyString.Split(':');
        int width = int.Parse(parts[1]);
        int height = int.Parse(parts[2]);
        int size = int.Parse(parts[3]);

        //Use Reflection to get the specified Color Property.
        Color color = Color.Black;
        color = (Color)color.GetType().GetProperty(parts[4]).GetValue(null);

        //New empty texture
        Texture2D emptyTexture = new Texture2D(graphicsDevice, width, height);
        Color[] pixels = new Color[width * height];
        for (int y1 = 0; y1 < height; y1++)
        {
            //Whether or not to start with a colored square.
            bool pinkStart = y1 % (2*size) < size;
            for (int x1 = 0; x1 < width; x1++)
            {
                //Whether or not this square is colored or black.
                bool pink = x1 % (2*size) < size ? pinkStart : !pinkStart;
                pixels[y1 * width + x1] = pink ? color : Color.Black;
            }
        }
        emptyTexture.SetData<Color>(pixels);

        textures.Add(emptyString, emptyTexture);
        return emptyTexture;
    }

    public void PlaySound(string assetName)
    {
        SoundEffect snd = contentManager.Load<SoundEffect>(assetName);
        snd.Play();
    }

    public void PlayMusic(string assetName, bool repeat = true)
    {
        MediaPlayer.IsRepeating = repeat;
        MediaPlayer.Play(contentManager.Load<Song>(assetName));
    }

    public ContentManager Content
    {
        get { return contentManager; }
    }
}