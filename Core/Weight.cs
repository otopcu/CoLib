namespace CoLib;

public class Weight
{
  public Unit Unit { get; set; }
  public double Value { get; set; }
  public double UpdateValue { get; set; } // Used to store the weight update values (i.e. delta weight)

  public Weight()
  {
    Value = 0;
    UpdateValue = 0.0;
  }
  public Weight(Unit unit)
    : this()
  {
    Unit = unit;
  }
  public Weight(Unit unit, double weight)
    : this()
  {
    Unit = unit;
    Value = weight;
  }
}
