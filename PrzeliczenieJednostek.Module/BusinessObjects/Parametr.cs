using Common.Module.Module.BusinessObjects;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrzeliczenieJednostek.Module.BusinessObjects
{
    [DefaultClassOptions]
    [XafDefaultProperty(nameof(Nazwa))]
    public class Parametr : XPObject
    {
        public Parametr(Session session) : base(session)
        { }

        decimal liczbaMolowa;
        JednostkaMiary jednostkaBazowa;
        string nazwa;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Nazwa
        {
            get => nazwa;
            set => SetPropertyValue(nameof(Nazwa), ref nazwa, value);
        }
        
        [XafDisplayName("Masa molowa g/mol")]
        public decimal LiczbaMolowa
        {
            get => liczbaMolowa;
            set => SetPropertyValue(nameof(LiczbaMolowa), ref liczbaMolowa, value);
        }

        public JednostkaMiary JednostkaBazowa
        {
            get => jednostkaBazowa;
            set => SetPropertyValue(nameof(JednostkaBazowa), ref jednostkaBazowa, value);
        }
    }
}
