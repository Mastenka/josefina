using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Josefina.Entities
{
  public enum EBankProxyType
  {
    FIO
  }

  public class BankProxy
  {
    public int BankProxyID { get; set; }

    public EBankProxyType BankProxyType { get; set; }

    [ForeignKey("FioBankProxy")]
    public int? FioBankProxyID { get; set; }

    public FioBankProxy FioBankProxy { get; set; }
  }
}