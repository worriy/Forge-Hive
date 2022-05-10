using System.Collections.Generic;
using Hive.Backend.DataModels;

namespace Hive.Backend.ViewModels
{
  public partial class ReportVM
  {
    public ReportVM()
    {
    }

    public string Id { get; set; }
    public string Content { get; set; }
    public string Author { get; set; }
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Answers { get; set; }
    public List<Result> Results { get; set; }
  }
}