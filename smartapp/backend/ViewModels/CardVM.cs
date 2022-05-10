using System;
using System.Collections.Generic;
using Hive.Backend.DataModels;

namespace Hive.Backend.ViewModels
{
  public partial class CardVM
  {
    public CardVM()
    {
    }

    public string Id { get; set; }
    public string LinkedCardId { get; set; }
    public string Content { get; set; }
    public string TargetGroups { get; set; }
    public DateTime PublicationDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Author { get; set; }
    public string Type { get; set; }
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Answers { get; set; }
    public bool IsLiked { get; set; }
    public string PictureId { get; set; }
    public string Picture { get; set; }
    public List<Result> Results { get; set; }

  }
}