using PleaseThem.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleaseThem.Controllers
{
  public class AnimationController : ISaveable
  {
    public Animation WalkLeft { get; set; }
    public Animation WalkRight { get; set; }
    public Animation WalkUp { get; set; }
    public Animation WalkDown { get; set; }

    public string GetSaveData()
    {
      var data = "\n--AnimationController--";
      
      data += WalkLeft != null ? $"\nWalkLeft={WalkLeft.GetSaveData()}" : "";
      data += WalkRight != null ? $"\nWalkRight={WalkRight.GetSaveData()}" : "";
      data += WalkUp != null ? $"\nWalkUp={WalkUp.GetSaveData()}" : "";
      data += WalkDown != null ? $"\nWalkDown={WalkDown.GetSaveData()}" : "";

      return data;
    }
  }
}
