using System.Runtime.CompilerServices; // [CallerMemberName]

namespace CoLib.ExCo;

public class ExCoUnit : Unit
{
  #region Properties
  /// <summary>
  /// DeCo Unit's type overrides the base type.
  /// </summary>
  private ExCoUnitType _Type = ExCoUnitType.PROPOSITION;
  public new ExCoUnitType Type
  {
    get
    {
      return _Type;
    }
    set
    {
      if (value != _Type)
      {
        _Type = value;
        switch (_Type)
        {
          case ExCoUnitType.SU:
            base.Type = UnitType.SU;
            break;
          default:
            base.Type = UnitType.UNIT;
            break;
        }
      }
    }
  }
  #endregion

  #region Ctor
  public ExCoUnit(ExCoUnitType type, double priority, [CallerMemberName] string name = null)
    : base()
  {
    Name = name;
    Priority = priority;
    Type = type;
  }
  public ExCoUnit(ExCoUnitType type, [CallerMemberName] string name = null)
    : base(name:name)
  {
    Name = name;
    Type = type;
  }

  public ExCoUnit(ExCoUnitType type, string name, double priority)
    : this(type, name)
  {
    Priority = priority;
  }

  public ExCoUnit(ExCoUnit unit)
    : this(unit.Type, unit.Name, unit.Priority)
  {
    Activation = unit.Activation;
  }
  #endregion
}
