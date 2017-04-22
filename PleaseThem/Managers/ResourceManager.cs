using PleaseThem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem.Managers
{
  public class ResourceManager
  {
    private Resources _resources;

    #region Properties
    public int Food
    {
      get { return _resources.Food; }
      private set { _resources.Food = value; }
    }

    public int Wood
    {
      get { return _resources.Wood; }
      private set { _resources.Wood = value; }
    }

    public int Stone
    {
      get { return _resources.Stone; }
      private set { _resources.Stone = value; }
    }

    public int Gold
    {
      get { return _resources.Gold; }
      private set { _resources.Gold = value; }
    }
    #endregion

    public ResourceManager()
    {
      _resources = new Resources()
      {
        Food = 1500,
        Wood = 2000,
        Stone = 4000,
        Gold = 3500,
      };
    }

    public bool CanAfford(Resources resouces)
    {
      return this.Food >= resouces.Food &&
             this.Wood >= resouces.Wood &&
             this.Stone >= resouces.Stone &&
             this.Gold >= resouces.Gold;
    }

    public void Use(Resources resources)
    {
      if (!CanAfford(resources))
        throw new Exception("Check to see if you can afford this!");

      this.Food -= resources.Food;
      this.Wood -= resources.Wood;
      this.Stone -= resources.Stone;
      this.Gold -= resources.Gold;
    }

    public void Add(Resources resources)
    {
      this.Food += resources.Food;
      this.Wood += resources.Wood;
      this.Stone += resources.Stone;
      this.Gold += resources.Gold;

      resources.Reset();
    }

    public void Increment(int amount = 1)
    {
      this.Food += amount;
      this.Wood += amount;
      this.Stone += amount;
      this.Gold += amount;
    }

    public int Sum(Resources resources)
    {
      return resources.Food +
             resources.Wood +
             resources.Stone +
             resources.Gold;
    }
  }
}
