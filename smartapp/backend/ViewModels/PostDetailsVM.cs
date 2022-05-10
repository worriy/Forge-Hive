using System;
using System.Collections.Generic;
using Hive.Backend.DataModels;

namespace Hive.Backend.ViewModels
{
  public partial class PostDetailsVM
  {
    public PostDetailsVM()
    {
    }

    public string Id { get; set; }
    public string Content { get; set; }
    public string Type { get; set; }
    public DateTime PublicationDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Answers { get; set; }
    public string TargetGroups { get; set; }
    public string Status { get; set; }
    public List<Result> Results { get; set; }
    public string Picture { get; set; }
  }
}