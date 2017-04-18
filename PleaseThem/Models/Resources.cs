using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem.Models
{
  public class Resources
  {
    public int Food { get; set; }
    public int Wood { get; set; }
    public int Stone { get; set; }
    public int Gold { get; set; }

    public int GetTotal()
    {
      return Food +
        Wood +
        Stone +
        Gold;
    }

    public void Reset()
    {
      Food = 0;
      Wood = 0;
      Stone = 0;
      Gold = 0;
    }
  }
}
