using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Module.Module.BusinessObjects
{
    [DefaultClassOptions]
    [XafDefaultProperty(nameof(JM))]
    [NavigationItem("Konfiguracja")]
    public class JednostkaMiary : XPObject
    {
        public JednostkaMiary(Session session) : base(session)
        { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            Przelicznik = 1;
        }

        JednostkaMiary jednostkaBazowa;
        decimal przelicznik;
        string jM;


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string JM
        {
            get => jM;
            set => SetPropertyValue(nameof(JM), ref jM, value);
        }

        [ModelDefault("DisplayFormat", "{0:N4}")]
        [ModelDefault("EditMask", "N4")]
        [XafDisplayName("Przelicznik jm/bazowa")]
        public decimal Przelicznik
        {
            get => przelicznik;
            set => SetPropertyValue(nameof(Przelicznik), ref przelicznik, value);
        }

       [Association("JednostkaMiary-JednostkiPochodne")]
        public JednostkaMiary JednostkaBazowa
        {
            get => jednostkaBazowa;
            set => SetPropertyValue(nameof(JednostkaBazowa), ref jednostkaBazowa, value);
        }

        [Association("JednostkaMiary-JednostkiPochodne")]
        public XPCollection<JednostkaMiary> JednostkiPochodne => GetCollection<JednostkaMiary>(nameof(JednostkiPochodne));
    }
}
