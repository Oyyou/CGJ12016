using PleaseThem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem.Managers
{
  public class MinionManager
  {
    public MinionManager()
    {
    }

    public void Employ(Minion minion, Models.LabourBuilding building)
    {
      if (building.MinionCount < building.MinionCountMax)
      {
        Game1.MessageBox.Show("There are no spaces");
        return;
      }

      minion.Occuptation = building.Occuptation;
      minion.Workplace = building;
    }

    public void Unemploy(Minion minion)
    {
      minion.Occuptation = Occuptations.Unemployed;
      minion.Workplace = null;
    }
  }
}
