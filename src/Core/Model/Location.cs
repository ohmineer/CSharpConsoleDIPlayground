namespace ConsoleDIPlayground;

public class Location
{
  public static Location Default => new() { City = "Undefined" };

  public string AdminName { get; set; } = string.Empty;

  public string Capital { get; set; } = string.Empty;

  public string City { get; set; } = string.Empty;

  public string Country { get; set; } = string.Empty;

  public string Iso2 { get; set; } = string.Empty;

  public string Lat { get; set; } = string.Empty;

  public string Lng { get; set; } = string.Empty;

  public string Population { get; set; } = string.Empty;

  public string PopulationProper { get; set; } = string.Empty;
}
