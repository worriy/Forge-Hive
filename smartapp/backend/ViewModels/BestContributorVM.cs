
namespace Hive.Backend.ViewModels
{
  public partial class BestContributorVM
  {
    public BestContributorVM()
    {
    }

    public int Posts { get; set; }
    public double Answers { get; set; }
    public string PictureUrl { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Department { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public int Likes { get; set; }

  }
}