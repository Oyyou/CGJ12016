using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PleaseThem.Tiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem
{
  public class Map : Component
  {
    #region Fields
    
    public List<Tile> BackgroundTiles { get; private set; }

    private int[,] _resourceMap;
    
    #endregion
    
    #region Properties
    
    public int Height { get; private set; }
    
    public List<Models.Resource> Resources { get; private set; }
    
    public List<ResourceTile> ResourceTiles { get; private set; }
    
    public const int TileSize = 32;
    
    public int Width { get; private set; }
    
    #endregion
    
    #region Methods

    public void Add(Rectangle rectangle)
    {
      var newPosition = new Vector2(rectangle.X / Map.TileSize, rectangle.Y / Map.TileSize);
      var newWidth = rectangle.Width / Map.TileSize;
      var newHeight = rectangle.Height / Map.TileSize;

      for (int y = (int)newPosition.Y; y < (int)(newPosition.Y + newHeight); y++)
      {
        for (int x = (int)newPosition.X; x < (int)(newPosition.X + newWidth); x++)
        {
          _resourceMap[y, x] = (int)TileType.Occupied;
        }
      }

      //using (StreamWriter writer = new StreamWriter(@"D:\derp.txt"))
      //{
      //  for (int y = 0; y < Width; y++)
      //  {
      //    for (int x = 0; x < Height; x++)
      //    {
      //      writer.Write(_resourceMap[y, x]);
      //    }

      //    writer.WriteLine();
      //  }
      //}
    }
    
    public void CleanArea(Vector2 position, int radius)
    {
      var actualPosition = position / Map.TileSize;

      for (int y = 0; y < _resourceMap.GetLength(0); y++)
      {
        for (int x = 0; x < _resourceMap.GetLength(1); x++)
        {
          var val = _resourceMap[y, x];

          if (val != 0)
          {
            var mapPosition = new Vector2(x, y);

            if (Vector2.Distance(actualPosition, mapPosition) < radius)
            {
              _resourceMap[y, x] = 0;

              var tile = ResourceTiles.Where(c => c.Position == mapPosition * Map.TileSize).FirstOrDefault();
              ResourceTiles.Remove(tile);
            }
          }
        }
      }

      //using (StreamWriter writer = new StreamWriter(@"D:\derp.txt"))
      //{
      //  for (int y = 0; y < Width; y++)
      //  {
      //    for (int x = 0; x < Height; x++)
      //    {
      //      writer.Write(_resourceMap[y, x]);
      //    }

      //    writer.WriteLine();
      //  }
      //}
    }

    private void CreateForests()
    {
      int min = (Width * Height) / 100;
      int max = (Width * Height) / 50;

      int forestCount = Game1.Random.Next(min, max);

      for (int i = 0; i < forestCount; i++)
      {
        int size = Game1.Random.Next(5, 20);
        var startPosition = new Vector2(Game1.Random.Next(0, Width + 1),
                                        Game1.Random.Next(0, Height + 1));
        var radius = size / 2;

        var positions = new List<Vector2>();
        int attempt = 100;
        while (size > 0 && attempt > 0)
        {
          int x = Game1.Random.Next((int)(startPosition.X - radius), (int)(startPosition.X + radius));
          int y = Game1.Random.Next((int)(startPosition.Y - radius), (int)(startPosition.Y + radius));
          var position = new Vector2(x, y);

          if (x < 0 || x >= Width ||
             y < 0 || y >= Height)
            continue;

          if (!positions.Contains(position))
          {
            positions.Add(position);
            _resourceMap[(int)position.Y, (int)position.X] = 1;
            size--;
          }
          else
          {
            attempt--;
          }
        }
      }
    }

    private void CreateOres()
    {
      int min = (Width * Height) / 300;
      int max = (Width * Height) / 200;

      int count = Game1.Random.Next(min, max);

      for (int i = 0; i < count; i++)
      {
        int size = Game1.Random.Next(3, 7);
        var startPosition = new Vector2(Game1.Random.Next(0, Width + 1),
                                        Game1.Random.Next(0, Height + 1));
        var radius = size / 2;

        var positions = new List<Vector2>();
        int attempt = 100;
        while (size > 0 && attempt > 0)
        {
          int x = Game1.Random.Next((int)(startPosition.X - radius), (int)(startPosition.X + radius));
          int y = Game1.Random.Next((int)(startPosition.Y - radius), (int)(startPosition.Y + radius));
          var position = new Vector2(x, y);

          if (x < 0 || x >= Width ||
             y < 0 || y >= Height)
            continue;

          if (!positions.Contains(position))
          {
            positions.Add(position);
            _resourceMap[(int)position.Y, (int)position.X] = 2;
            size--;
          }
          else
          {
            attempt--;
          }
        }
      }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      foreach (var tile in BackgroundTiles)
        tile.Draw(spriteBatch);

      foreach (var tile in ResourceTiles)
        tile.Draw(spriteBatch);
    }

    public override string GetSaveData()
    {
      var data = "--Map--";



      return data;
    }

    /// <summary>
    /// Returns the tile index for the given cell.
    /// </summary>
    public TileType GetIndex(int cellX, int cellY)
    {
      if (cellX < 0 || cellX > Width - 1 || cellY < 0 || cellY > Height - 1)
        return 0;

      return (TileType)_resourceMap[cellY, cellX];
    }
    
    public Map(ContentManager Content, int width, int height)
    {
      Width = width;
      Height = height;

      Texture2D backgroundTexture = Content.Load<Texture2D>("Tiles/Grass");

      BackgroundTiles = new List<Tile>();

      for (int y = 0; y < width; y++)
      {
        for (int x = 0; x < height; x++)
        {
          BackgroundTiles.Add(new Tile(backgroundTexture, new Vector2(x * TileSize, y * TileSize), TileType.Grass));
        }
      }

      _resourceMap = new int[Width, Height];
      ResourceTiles = new List<ResourceTile>();

      CreateForests();
      
      CreateOres();

      var woodTexture = Content.Load<Texture2D>("Tiles/Wood");
      var OreTexture = Content.Load<Texture2D>("Tiles/Ore");

      for (int y = 0; y < _resourceMap.GetLength(0); y++)
      {
        for (int x = 0; x < _resourceMap.GetLength(1); x++)
        {
          var position = new Vector2(x * Map.TileSize, y * TileSize);
          switch (_resourceMap[y, x])
          {
            case 1:
              ResourceTiles.Add(new ResourceTile(woodTexture, position, TileType.Tree));
              break;
            case 2:
              ResourceTiles.Add(new ResourceTile(OreTexture, position, TileType.Stone));
              break;
          }
        }
      }
    }

    public void Remove(Vector2 position)
    {
      _resourceMap[(int)(position.Y / Map.TileSize), (int)(position.X / Map.TileSize)] = (int)TileType.Grass;
    }
    
    public override void Update(GameTime gameTime)
    {
    
    }
    
    #endregion
  }
}
