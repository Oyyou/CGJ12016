using PleaseThem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem.Managers
{
  public class JobManager
  {
    public LabourBuilding Building;

    public List<Minion> Minions { get; set; }

    public List<Resource> Resources { get; set; }
  }
}
