using System.Collections.Generic;

namespace Hive.Backend.DataModels
{
  public class Reporting : Card
  {
    public Reporting() : base()
    {
    }

    public int Views { get; set; }
    public int Answers { get; set; }
    public int Likes { get; set; }
    public List<Result> Results { get; set; }
  }
}